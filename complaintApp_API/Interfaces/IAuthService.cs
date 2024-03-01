using complaintApp_API.Models;

namespace complaintApp_API.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> Register(User user);
        public Task<(User user, string token)> LogIn(LogInModel logInModel);
    }
}
