using EventManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
       
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public override string Email { get; set; }
        public ICollection<Event>? Events { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }

    }
}
