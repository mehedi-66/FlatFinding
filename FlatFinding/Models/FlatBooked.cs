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


    }
}
