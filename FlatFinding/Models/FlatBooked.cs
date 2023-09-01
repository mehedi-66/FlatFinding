using System;

namespace FlatFinding.Models
{
    public class FlatBooked
    {
        public int FlatBookedId { get; set; }
        public string OwnerId { get; set; }
        public string UserId { get; set; }

        public string PaymentId { get; set; }

        public int FlatId { get; set; }

        public float FlatCost { get; set; }

        public float FlatProfit { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        public int IsDelete { get; set; }
        public DateTime? BookingCancel { get; set; }

        // Police varification 
        public string?  PictureOfPerson {get; set;}
        public string? NameOfWhoLive { get; set; }
        public string? FatherName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ParmanetAddress { get; set; }
        public string? MaritalStatus { get; set; }
        public string? EmploymentAddress { get; set; }
        public string? Religion { get; set; }
        public string? PhoneNumberOfWhoTakeFlat { get; set; }
        public string? EmailId { get; set; }
        public string? NID { get; set; }
        public string? Person1Name { get; set; }
        public string? Person1Phone { get; set; }
        public string? Person2Name { get; set; }
        public string? Person2Phone { get; set; }
        public string? NameOfBeforeHouseHolderName { get; set; }
        public string? AddressOfBeforeHouse { get; set; }
        public string? ResoneOfLeveingHouse { get; set; }

        public int? Condition { get; set; }
    }
}
