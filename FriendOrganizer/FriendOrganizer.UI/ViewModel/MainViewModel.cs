using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private Func<IMeetingDetailViewModel> _meetingDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IDetailViewModel _detailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            Func<IMeetingDetailViewModel> meetingDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _meetingDetailViewModelCreator = meetingDetailViewModelCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);

            NavigationViewModel = navigationViewModel;
        }
        public async Task LoadAync()
        {
            await NavigationViewModel.LoadAsync();
        }
        public ICommand CreateNewDetailCommand { get; }
        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancleDialog("You made changes. Navigate away?", "Question");
                if (result == MessageDialogResult.Cancle)
                {
                    return;
                }
            }
            switch (args.ViewModelName)
            {
                case (nameof(FriendDetailViewModel)):
                    DetailViewModel = _friendDetailViewModelCreator();
                    break;
                case (nameof(MeetingDetailViewModel)):
                    DetailViewModel = _meetingDetailViewModelCreator();
                    break;
                default:
                    throw new Exception($"Viewmodel {args.ViewModelName} could not load");
            }

            await DetailViewModel.LoadAsync(args.Id);
        }
        public INavigationViewModel NavigationViewModel { get; }
        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            private set
            {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { ViewModelName = viewModelType.Name });
        }
        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }
    }
}
