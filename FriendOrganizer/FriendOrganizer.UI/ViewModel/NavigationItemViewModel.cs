namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;
        public NavigationItemViewModel(int id, string displayMember)
        {
            Id = id;
            _displayMember = displayMember;
        }
        public int Id { get; }

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

    }
}
