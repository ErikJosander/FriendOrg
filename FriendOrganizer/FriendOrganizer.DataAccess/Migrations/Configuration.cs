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

            context.ProgrammingLanguages.AddOrUpdate(p => p.Name,
                new Models.ProgrammingLanguage { Name = "C#" },
                new Models.ProgrammingLanguage { Name = "TypeScript" },
                new Models.ProgrammingLanguage { Name = "F#" },
                new Models.ProgrammingLanguage { Name = "Swift" },
                new Models.ProgrammingLanguage { Name = "Java" }
                );
            
            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(ph => ph.Number,
                new Models.FriendPhoneNumber { Number = "+46 12345678", FriendId = context.Friends.First().Id });
        }
    }
}
