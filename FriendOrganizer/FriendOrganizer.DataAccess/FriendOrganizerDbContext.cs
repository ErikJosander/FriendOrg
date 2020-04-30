using FriendOrganizer.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContext : DbContext
    {
        public FriendOrganizerDbContext():base("FriendOrganizerDb")
        {

        }
        public DbSet<Friend> Friends { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Removes the pluralisation of the naming of tables in this DB
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
