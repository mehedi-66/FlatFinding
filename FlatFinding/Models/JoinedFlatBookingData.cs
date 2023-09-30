namespace FlatFinding.Models
{

        public class JoinedFlatBookingData
        {
            public string? FlatName { get; set; }
            public string? Address { get; set; }
            public string? OwnerName { get; set; }
            public string? BuyerName { get; set; }
            public DateTime? BookingDate { get; set; }
            public DateTime? BookingCancel { get; set; }
            public float FlatCost { get; set; }
            public string? Type { get; set; }
            public string? OwnerPhone { get; set; }
            public string? BuyerPhone { get; set; }
            public float FlatProfit { get; set; }
        // police varification Info
        public string? PictureOfPerson { get; set; }
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
