using FriendOrganizer.Models;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepostory;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
        private FriendWrapper _friend;
        private FriendPhoneNumberWrapper _selectedPhoneNumber;

        //Constructor
        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService) : base(eventAggregator, messageDialogService)
        {
            _friendRepostory = friendRepository;       
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }
        public override async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepostory.GetByIdAsync(friendId.Value)
                : CreateNewFriend();

            Id = friend.Id;

            InitializeFriend(friend);

            InitializeFriendPhoneNumber(friend.PhoneNumbers);

            await LoadProgrammingLanguagesLookupAsync();
        }
        private void InitializeFriendPhoneNumber(ICollection<FriendPhoneNumber> phoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }
            PhoneNumbers.Clear();
            foreach (var friendPhoneNUmber in phoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNUmber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }
        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepostory.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
        private void InitializeFriend(Friend friend)
        {
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepostory.HasChanges();
                }
                if (e.PropertyName == nameof(ViewModel.FriendDetailViewModel.Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if( e.PropertyName == nameof(Friend.FirstName)
                || e.PropertyName == nameof(Friend.LastName))
                {
                    SetTitle();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            // Little trick to trigger the validation
            if (Friend.Id == 0)
            {
                Friend.FirstName = "";
                Friend.LastName = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title =$"{Friend.FirstName} {Friend.LastName}";
        }

        private async Task LoadProgrammingLanguagesLookupAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem { DisplayMember = " - " });
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
            }
        }
        public FriendWrapper Friend
        {
            get { return _friend; }
            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }
        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }
        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }
        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }
        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepostory.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepostory.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = ""; // trigger validation Changes
        }   
        protected override async void OnSaveExecute()
        {
            await _friendRepostory.SaveAsync();
            HasChanges = _friendRepostory.HasChanges();
            Id = Friend.Id;
            RaiseDetailSavedEvent(Friend.Id, (Friend.FirstName + " " + Friend.LastName));
        }
        protected override async void OnDeleteExecute()
        {
            if(await _friendRepostory.HasMeetingAsync(Friend.Id))
            {
                MessageDialogService.ShowInfoDialog($"Friend: {Friend.FirstName} can't be deleted, as this friend is part of at least one meeting.");
                return;
            }
            var result = MessageDialogService.ShowOkCancleDialog($"Delete Friend: {Friend.FirstName} {Friend.LastName}?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepostory.Remove(Friend.Model);
                await _friendRepostory.SaveAsync();
                RaiseDetailDeletedEvent(Friend.Id);
            }
        }
        protected override bool OnSaveCanExecute()
        {
            return Friend != null
                && !Friend.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && HasChanges;
        }
        private Friend CreateNewFriend()
        {
            Friend friend = new Friend();
            _friendRepostory.Add(friend);
            return friend;
        }
    }
}
