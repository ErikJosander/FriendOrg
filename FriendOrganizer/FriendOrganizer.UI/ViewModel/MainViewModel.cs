﻿using FriendOrganizer.Models;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {     
        private IFriendDataService _friendDataService;
        private Friend _selectedFriend;

        public MainViewModel(IFriendDataService friendDataService)
        {
            Friends = new ObservableCollection<Friend>();
            _friendDataService = friendDataService;
        }       

        public void Load()
        {
            var friends = _friendDataService.GetAll();
            Friends.Clear();
            foreach (Friend friend in friends)
            {
                Friends.Add(friend);
            }
        }

        // Notityfies when the collections changes
        public ObservableCollection<Friend> Friends { get; set; }


        public Friend SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
            }
        }
    }
}
