using complaintApp_API.DTOs;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace complaintApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUser _userRepo;
        private readonly IAuthService _authService;

        public AuthController(IUser userRepo, IAuthService authService )
        {
           _authService = authService;
           _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LogInModel loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.UserEmail) || string.IsNullOrEmpty(loginModel.UserEmail))
            {
                return BadRequest("Please provide the necessary information");
            }

            //Check if the userName exists
            var UserExists = await _userRepo.UserExistsAsync(loginModel.UserEmail);

            if (UserExists == null) return Unauthorized("Invalid Credentials");

            //Compare the passwords
            bool isValid = BCrypt.Net.BCrypt.Verify(loginModel.Password, UserExists.Password);

            if (!isValid) return Unauthorized("Invalid Credentials");

            //Send the model to the auth service
            var (user, token) = await _authService.LogIn(loginModel);

            var userDto = new UserDTO
            {
                UserEmail = user.UserEmail,
                Role = user.Role
            };

            return Ok(new { User = userDto, Token = token });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User userObj)
        {
            try
            {
                if (userObj == null || string.IsNullOrEmpty(userObj.UserEmail) || string.IsNullOrEmpty(userObj.Role)
                    || string.IsNullOrEmpty(userObj.Password) || string.IsNullOrEmpty(userObj.confirmPassword))
                {
                    return BadRequest("Please provide the needed information");
                }

                if (userObj.Password != userObj.confirmPassword)
                {
                    return BadRequest("Your Passwords don't match");
                }

                var userExists = await _userRepo.UserExistsAsync(userObj.UserEmail);

                if (userExists != null) return BadRequest("User already exists");

                //Hashing the password
                userObj.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);

                var userAdded = await _authService.Register(userObj);

                if (!userAdded) return BadRequest("Registering User was unsuccessful");

                return Ok("User Registration successfull");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
