namespace RaceDayAPI.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public int? Position { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public string Status { get; set; } = "Finished";
    }
}
