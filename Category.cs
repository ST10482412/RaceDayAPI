namespace RaceDayAPI.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        public string CategoryName { get; set; } = "";

        public string? Description { get; set; }

        public int MaxParticipants { get; set; }
    }
}
