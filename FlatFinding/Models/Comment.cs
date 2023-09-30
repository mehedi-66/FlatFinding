namespace FlatFinding.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int FlatId { get; set; }
        public string Name { get; set; }
        public string comment { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
