using complaintApp_API.Data;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using Dapper;
using System.Data;
using System.Reflection.Metadata;

namespace complaintApp_API.Repositories
{
    public class UserRepository : IUser
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                const string sql = "Select * from users";

                IDbConnection connection = _context.CreateConnection();

                var users = await connection.QueryAsync<User>(sql);

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the Repo while trying to get all users: {ex.Message}");

                return null;
            }

        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {

                var parameter = new { UserEmail = email };

                const string sql = "Select * from users where UserEmail = @UserEmail";

                IDbConnection connection = _context.CreateConnection();

                var user = await connection.QueryFirstAsync<User>(sql, parameter);

                return user;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to get user by email: {ex.Message}");

                return null;
            }

        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {

                var parameter = new { UserId = id };

                const string sql = "Select * from users where UserId = @UserId";

                IDbConnection connection = _context.CreateConnection();

                var user = await connection.QueryFirstAsync<User>(sql, parameter);

                return user;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to get user by Id: {ex.Message}");

                return null;
            }
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            try
            {
                var parameter = new { UserId = id };

                const string sql = "Select * from users where UserId = @UserId";

                IDbConnection connection = _context.CreateConnection();

                var user = await connection.QueryFirstAsync<User>(sql, parameter);

                return user != null;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to check if user exists: {ex.Message}");

                return false;
            }
        }

        public async Task<User> UserExistsAsync(string email)
        {
            try
            {
                var parameter = new { UserEmail = email };

                const string sql = "Select * from users where UserEmail = @UserEmail";

                IDbConnection connection = _context.CreateConnection();

                var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameter);

                return user;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to check if user exists: {ex.Message}");

                return null;
            }
        }

        public async Task<List<User>> SearchUsersByRoleAsync(string Role)
        {
            try
            {

                var parameter = new { Role = Role };

                const string sql = "Select * from users where Role = @Role";

                IDbConnection connection = _context.CreateConnection();

                var users = await connection.QueryAsync<User>(sql, parameter);

                return users.ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to search users by Role: {ex.Message}");

                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var parameter = new { UserId = id };

                const string sql = "Delete From users where UserId = @UserId";

                IDbConnection connection = _context.CreateConnection();

                var count= await connection.ExecuteAsync(sql, parameter);

                return count > 0;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to Delete User: {ex.Message}");

                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRole updateUserRole)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", updateUserRole.UserId);
                parameters.Add("Role", updateUserRole.Role);

                const string sql = $"Update users Set Role = @Role where UserId = @UserId";

                IDbConnection connection = _context.CreateConnection();

                var updateRole = await connection.ExecuteAsync(sql, parameters);

                return updateRole > 0;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to Update User Role: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserPasswordAsync(UpdateUserPassword updateUserPassword)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserEmail", updateUserPassword.UserEmail);
                parameters.Add("Password", updateUserPassword.Password);

                const string sql = $"Update users Set Password = @Password where UserEmail = @UserEmail";

                IDbConnection connection = _context.CreateConnection();

                var updatePassword = await connection.ExecuteAsync(sql, parameters);

                return updatePassword > 0;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to Update User Password: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> UpdateUserEmailAsync(UpdateUserEmail updateUserEmail)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", updateUserEmail.UserId);
                parameters.Add("UserEmail", updateUserEmail.UserEmail);

                const string sql = $"Update users Set UserEmail = @UserEmail where UserId = @UserId";

                IDbConnection connection = _context.CreateConnection();

                await connection.QueryAsync(sql, parameters);

                var updateEmail = await connection.ExecuteAsync(sql, parameters);

                return updateEmail > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to Update User Email: {ex.Message}");
                return false;
            }
        }

    }

}
