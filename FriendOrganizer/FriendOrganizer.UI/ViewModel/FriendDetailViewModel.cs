using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepostory;
        private IEventAggregator _eventAggregator;
        private FriendWrapper _friend;
        //Constructor
        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator)
        {
            _friendRepostory = friendRepository;
            _eventAggregator = eventAggregator;
            
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
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
        public ICommand SaveCommand { get; }   
        private async void OnSaveExecute()
        {
            await _friendRepostory.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FirstName + " " + Friend.LastName
                });
        }
        private bool OnSaveCanExecute()
        {          
            return Friend != null && !Friend.HasErrors;          
        }
       
        public async Task LoadAsync(int friendId)
        {
            var friend = await _friendRepostory.GetByIdAsync(friendId);
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }        
    }
}
