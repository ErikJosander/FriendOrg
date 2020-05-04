using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Models
{
    public class Friend
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
    }
}
