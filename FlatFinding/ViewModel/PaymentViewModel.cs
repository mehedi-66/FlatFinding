using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace FlatFinding.ViewModel
{
    public class PaymentViewModel
    {
        public int FlatId { get; set; }
        public string AccountType { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNo { get; set; }
    }
}
