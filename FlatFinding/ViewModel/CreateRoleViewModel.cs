using System.ComponentModel.DataAnnotations;

namespace FlatFinding.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
