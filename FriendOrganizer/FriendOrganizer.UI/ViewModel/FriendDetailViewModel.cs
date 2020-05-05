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
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepostory;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
        private FriendWrapper _friend;
        private bool _hasChanges;
        private FriendPhoneNumberWrapper _selectedPhoneNumber;

        //Constructor
        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
        {
            _friendRepostory = friendRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }
        public async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepostory.GetByIdAsync(friendId.Value)
                : CreateNewFriend();

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
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            // Little trick to trigger the validation
            if (Friend.Id == 0)
            {
                Friend.FirstName = "";
                Friend.LastName = "";
            }
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
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }
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
        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancleDialog($"Delete Friend: {Friend.FirstName} {Friend.LastName}?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepostory.Remove(Friend.Model);
                await _friendRepostory.SaveAsync();
                _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
            }
        }
        private async void OnSaveExecute()
        {
            await _friendRepostory.SaveAsync();
            HasChanges = _friendRepostory.HasChanges();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FirstName + " " + Friend.LastName
                });
        }
        private bool OnSaveCanExecute()
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
