namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Models;
    using System;
    using System.Collections.Generic;
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
            new Friend { FirstName = "Erik", LastName = "Josander" },
            new Friend { FirstName = "Jamie", LastName = "Wilton" },
            new Friend { FirstName = "Malena", LastName = "Persson" },
            new Friend { FirstName = "Johan", LastName = "Karlsson" }
            );

            context.ProgrammingLanguages.AddOrUpdate(p => p.Name,
                new ProgrammingLanguage { Name = "C#" },
                new ProgrammingLanguage { Name = "TypeScript" },
                new ProgrammingLanguage { Name = "F#" },
                new ProgrammingLanguage { Name = "Swift" },
                new ProgrammingLanguage { Name = "Java" }
                );

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(ph => ph.Number,
                new FriendPhoneNumber { Number = "+46 12345678", FriendId = context.Friends.First().Id });

            context.Meetings.AddOrUpdate(m => m.Title,
                new Meeting
                {
                    Title = "Watching Soccer",
                    DateFrom = new DateTime(2020, 06, 06),
                    DateTo = new DateTime(2020, 06, 06),
                    Friends = new List<Friend>
                    {
                        context.Friends.Single(f=>f.FirstName == "Erik" && f.LastName == "Josander"),
                        context.Friends.Single(f=>f.FirstName == "Jamie" && f.LastName == "Wilton")
                    }
                }); 
        }
    }
}
