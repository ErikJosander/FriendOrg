﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private Func<FriendOrganizerDbContext> _contextCreator;
        public FriendDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public async Task<Friend> GetByIdAsync(int friendId)
        {
            using (var context = _contextCreator())
            {
                // context might be done closed before all Models have been collected therefore the the await keword
                return await context.Friends.AsNoTracking().SingleAsync(f=>f.Id == friendId);
            }
        }

        public async Task SaveAsync(Friend friend)
        {
            using(var context = _contextCreator())
            {
                context.Friends.Attach(friend);
                context.Entry(friend).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
