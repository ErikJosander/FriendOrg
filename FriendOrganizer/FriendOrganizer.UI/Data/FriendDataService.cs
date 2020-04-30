using FriendOrganizer.Models;
using System.Collections.Generic;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            // TODO: Load from real database
            yield return new Friend { FirstName = "Alexander", LastName = "Johansson" };
            yield return new Friend { FirstName = "Jamie", LastName = "Wilton" };
            yield return new Friend { FirstName = "Robin", LastName = "Johansson" };
            yield return new Friend { FirstName = "Caj", LastName = "Rydholm" };
        }
    }
}
