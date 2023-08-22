using Microsoft.AspNetCore.Identity;

namespace FlatFinding.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; } 
        public string? Address { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Picture { get; set; }

    }
}
