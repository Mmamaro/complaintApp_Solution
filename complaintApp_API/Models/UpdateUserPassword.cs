using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.Models
{
    public class UpdateUserPassword
    {
        [EmailAddress]public string UserEmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
