using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChristmasCalendar.Domain
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(256)]
        public string? FirstName { get; set; }

        [MaxLength(256)]
        public string? LastName { get; set; }

        [MaxLength(512)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? DateOfBirth { get; set; }

        [MaxLength(300)]
        public string? EmailAddressFromAuthProvider { get; set; }
    }
}
