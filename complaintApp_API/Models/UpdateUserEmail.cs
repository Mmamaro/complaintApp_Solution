using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.Models
{
    public class UpdateUserEmail
    {
        public int UserId { get; set; }
        [EmailAddress]public string UserEmail { get; set; }
    }
}
