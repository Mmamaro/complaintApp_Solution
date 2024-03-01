using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.DTOs
{
    public class ComplaintDTO
    {
        [EmailAddress] public string UserEmail { get; set; }
        [EmailAddress] public string AccussedEmail { get; set; }
        public string Issue { get; set; }
        public DateTime ComplaintDate { get; set; } = DateTime.Now;
        public int DaysPosted { get; set; }
        public string Status { get; set; }
    }
}
