using Microsoft.AspNetCore.Identity;

namespace FlatFinding.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }
}
