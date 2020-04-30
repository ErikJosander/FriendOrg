namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(f => f.FirstName,
                new Friend { FirstName = "Alexander", LastName = "Johansson" },
                new Friend { FirstName = "Jamie", LastName = "Wilton" },
                new Friend { FirstName = "Robin", LastName = "Johansson" },
                new Friend { FirstName = "Caj", LastName = "Rydholm" });
        }

    }
}
