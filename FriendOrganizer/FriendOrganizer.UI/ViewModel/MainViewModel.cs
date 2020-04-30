using FriendOrganizer.Models;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationViewModel navigationViewModel,
            IFriendDetailViewModel friendDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            FriendDetailViewModel = friendDetailViewModel;
        }

        public IFriendDetailViewModel FriendDetailViewModel { get; }
        public INavigationViewModel NavigationViewModel { get; }

        public async Task LoadAync()
        {
           await NavigationViewModel.LoadAsync();
        }  
    }
}
