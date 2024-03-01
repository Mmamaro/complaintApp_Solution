using complaintApp_API.DTOs;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace complaintApp_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;

        public UserController(IUser userRepo)
        {
            _userRepo = userRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            try
            {
                IEnumerable<User> users = await _userRepo.GetAllUsersAsync();

                if (users == null) return BadRequest();

                var usersDto = users.Select(x => new UserDTO
                {
                    UserEmail = x.UserEmail,
                    Role = x.Role
                });

                return Ok(usersDto);

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserById")]
        public async Task<ActionResult<UserDTO>> GetUserById(int Id)
        {
            try
            {
                if(Id <= 0) return BadRequest("Please provide valid Id");

                var user = await _userRepo.GetUserByIdAsync(Id);

                if(user == null) return NotFound("User Not Found");

                var userDto = new UserDTO
                {
                    UserEmail = user.UserEmail,
                    Role = user.Role
                };

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult<UserDTO>> GetUserByEmal(string email)
        {
            try
            {
                if (email == null) return BadRequest("Please provide valid Email");

                var user = await _userRepo.GetUserByEmail(email);

                if (user == null) return NotFound("User Not Found");

                var userDto = new UserDTO
                {
                    UserEmail = user.UserEmail,
                    Role = user.Role
                };

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("SearchUsersByRole")]
        public async Task<ActionResult<List<UserDTO>>> SearchUsersByRole(string Role)
        {
            try
            {

                if (Role == null) return BadRequest("Please Provide Role");

                var users = await _userRepo.SearchUsersByRoleAsync(Role);

                if (users == null) return NotFound("Provide valid role");

                var usersDto = users.Select(x => new UserDTO
                {
                    UserEmail = x.UserEmail,
                    Role = x.Role
                });

                return Ok(usersDto);

            } 
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("UpdatePassword")]
        public async Task<ActionResult<User>> UpdateUserPassword(UpdateUserPassword updateUserPassword)
        {
            try
            {
                if (updateUserPassword == null || string.IsNullOrEmpty(updateUserPassword.UserEmail) || string.IsNullOrEmpty(updateUserPassword.Password) || 
                    string.IsNullOrEmpty(updateUserPassword.ConfirmPassword))
                {
                    return BadRequest("Please provide the needed information");
                }

                var userExists = await _userRepo.UserExistsAsync(updateUserPassword.UserEmail);

                if (userExists == null) return NotFound("User does not exist");

                if (updateUserPassword.Password != updateUserPassword.ConfirmPassword)
                {
                    return BadRequest("Your Passwords don't match");
                }

                updateUserPassword.Password = BCrypt.Net.BCrypt.HashPassword(updateUserPassword.Password);

                var updatePassword = await _userRepo.UpdateUserPasswordAsync(updateUserPassword);

                if (!updatePassword) return BadRequest("Updating Password failed");

                return Ok("User Password Updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("UpdateEmail")]
        public async Task<ActionResult<User>> UpdateUserEmail(UpdateUserEmail updateUserEmail)
        {
            try
            {
                if (updateUserEmail == null || string.IsNullOrEmpty(updateUserEmail.UserEmail) || updateUserEmail.UserId <= 0)
                {
                    return BadRequest("Please provide the needed information");
                }

                var userExists = await _userRepo.UserExistsAsync(updateUserEmail.UserId);

                if (!userExists) return NotFound("User does not exist");

                var updateEmail = await _userRepo.UpdateUserEmailAsync(updateUserEmail);

                if (!updateEmail) return BadRequest("Updating Email failed");

                return Ok("User Email Updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserRole")]
        public async Task<ActionResult<User>> UpdateUserRole(UpdateUserRole updateUserRole)
        {
            try
            {
                if (updateUserRole == null || string.IsNullOrEmpty(updateUserRole.Role) || updateUserRole.UserId <= 0)
                {
                    return BadRequest("Please provide the needed information");
                }

                var userExists = await _userRepo.UserExistsAsync(updateUserRole.UserId);

                if (!userExists) return NotFound("User does not exist");

                var updateRole = await _userRepo.UpdateUserRoleAsync(updateUserRole);

                if (!updateRole) return BadRequest("Updating Role failed");

                return Ok("User Role Updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Provide valid id");

                var userExists = await _userRepo.UserExistsAsync(id);

                if (!userExists) return NotFound("User does not exist");

                var userDeleted = await _userRepo.DeleteUserAsync(id);

                if (!userDeleted) return BadRequest("Deleting user failed");

                return Ok("User deleted successfully");

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



    }
}
