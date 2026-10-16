namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        public int EnrolmentID { get; set; }

        public int ParticipantID { get; set; }

        public int CategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Today;

        public string Status { get; set; } = "Active";
    }
}
