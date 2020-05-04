namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(f => f.FirstName,
            new Models.Friend { FirstName = "Erik", LastName = "Josander" },
            new Models.Friend { FirstName = "Jamie", LastName = "Wilton" },
            new Models.Friend { FirstName = "Malena", LastName = "Persson" },
            new Models.Friend { FirstName = "Johan", LastName = "Karlsson" }
            );
        }
    }
}
