using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IFriendDetailViewModel _friendDetailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _friendDetailViewModelCreator = friendDetailViewModelCreator;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetaiView);

            NavigationViewModel = navigationViewModel;
        }         
        public async Task LoadAync()
        {
            await NavigationViewModel.LoadAsync();
        }
        private async void OnOpenFriendDetaiView(int friendId)
        {
            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }
        public INavigationViewModel NavigationViewModel { get; }
        public IFriendDetailViewModel FriendDetailViewModel
        {
            get { return _friendDetailViewModel; }
            private set
            {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
