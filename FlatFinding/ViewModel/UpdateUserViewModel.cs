using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FlatFinding.ViewModel
{
    public class UpdateUserViewModel
    {
        public string id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber {get; set; }
        public string? Picture { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }

    }
}
