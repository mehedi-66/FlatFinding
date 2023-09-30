using System.ComponentModel.DataAnnotations;

namespace FlatFinding.Models
{
    public class Flat
    {
        public int FlatId { get; set; }


        public string? OwnerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string HouseNo { get; set; }
        [Required]
        public string RoadNo { get; set; }
        [Required]
        public string sectorNo { get; set; }
        [Required]
        public string AreaName { get; set; }
        [Required]
        public int TotalCost { get; set; }
        [Required]
        public int RoomNo { get; set; }
        [Required]
        public string Available { get; set; }
        [Required]
        public string Types { get; set; }
        public string? Picture { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Phone { get; set; }

        public int Views { get; set; }

        public int IsBooking { get; set; }

        [Required]
        public int? FlatNumber {get; set;}
        public string? FlatStatus { get; set; }

    }
}
