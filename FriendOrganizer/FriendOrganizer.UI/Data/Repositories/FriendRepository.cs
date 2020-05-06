using FriendOrganizer.DataAccess;
using FriendOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>,
        IFriendRepository
    {
        /// <summary>
        /// Checks if an Friend is part of a meeting async.
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<bool> HasMeetingAsync(int friendId)
        {
            return await Context.Meetings.AsNoTracking()
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }
        public FriendRepository(FriendOrganizerDbContext context):base(context)
        {           
        }
        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            return await Context.Friends
                .Include(f => f.PhoneNumbers)
                .SingleAsync(f => f.Id == friendId);
        }
        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }       
    }
}
