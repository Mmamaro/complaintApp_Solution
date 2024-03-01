using complaintApp_API.Data;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace complaintApp_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IUser _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUser userRepo, DataContext context, IConfiguration config)
        {
            _userRepo = userRepo;
            _context = context;
            _config = config;
        }
        public async Task<(User user, string token)> LogIn(LogInModel logInModel)
        {
            try
            {

                var user = await _userRepo.UserExistsAsync(logInModel.UserEmail);

                //Create JWT token handler and get secret key
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["JWT:SecretKey"]);

                //Prepare list of user claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                // Create a token Descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    IssuedAt = DateTime.UtcNow,
                    Issuer = _config["JWT:Issuer"],
                    Audience = _config["JWT:Audience"],
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };

                //Create token and set it to user
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return (user, tokenString);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the Auth Service while trying to log in {ex.Message}");

                return (null, null);
            }
        }

        public async Task<bool> Register(User user)
        {
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UserEmail", user.UserEmail);
                    parameters.Add("Role", user.Role);
                    parameters.Add("Password", user.Password);

                    const string sql = $"insert into users(UserEmail, Role, Password)" +
                        $"Values(@UserEmail, @Role, @Password)";

                    using IDbConnection connection = _context.CreateConnection();

                    var AddUser = await connection.ExecuteAsync(sql, parameters);

                    return AddUser > 0;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in the Auth Service while trying to Register the User: {ex.Message}");
                    return false;
                }
            }
        }
    }
}