using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.Models
{
    public class User
    {
        public int  UserId { get; set; }
        [EmailAddress]public string UserEmail { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string confirmPassword { get; set; }
    }
}
