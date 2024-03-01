using complaintApp_API.Models;

namespace complaintApp_API.Interfaces
{
    public interface IUser
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(int id);
        public Task<User> GetUserByEmail(string email);
        public Task<List<User>> SearchUsersByRoleAsync(string Role);
        public Task<bool> UserExistsAsync(int id);
        public Task<User> UserExistsAsync(string email);
        public Task<bool> UpdateUserPasswordAsync(UpdateUserPassword updateUserPassword);
        public Task<bool> UpdateUserEmailAsync(UpdateUserEmail eupdateUserEmail);
        public Task<bool> UpdateUserRoleAsync(UpdateUserRole updateUserRole);
        public Task<bool> DeleteUserAsync(int id);
    }
}
