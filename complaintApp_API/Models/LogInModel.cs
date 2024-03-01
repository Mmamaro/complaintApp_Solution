using System.ComponentModel.DataAnnotations;

namespace complaintApp_API.Models
{
    public class LogInModel
    {
        [EmailAddress] public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
