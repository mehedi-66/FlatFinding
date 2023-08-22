using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FlatFinding.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        public string returnUrl { get; set; } = "";

    }
}
