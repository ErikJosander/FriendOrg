using FriendOrganizer.Models;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationViewModel navigationViewModel)
        {
            NavigationViewModel = navigationViewModel;
        }

        public INavigationViewModel NavigationViewModel { get; }

        public async Task LoadAync()
        {
           await NavigationViewModel.LoadAsync();
        }  
    }
}
