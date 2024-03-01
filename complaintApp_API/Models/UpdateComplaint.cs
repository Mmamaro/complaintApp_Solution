using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.Models
{
    public class UpdateComplaint
    {
        public int ComplaintId { get; set; }
        public string Issue { get; set; }
        public DateTime ComplaintDate { get; set; } = DateTime.Now;
    }
}
