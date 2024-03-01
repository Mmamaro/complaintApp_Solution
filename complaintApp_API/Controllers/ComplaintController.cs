using complaintApp_API.DTOs;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using complaintApp_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace complaintApp_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaint _complaintRepo;
        private readonly IUser _userRepo;

        public ComplaintController(IComplaint complaintRepo, IUser userRepo)
        {
            _complaintRepo = complaintRepo;
            _userRepo = userRepo;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<complaint>>> GetAllComplaints()
        {
            try
            {
                IEnumerable<complaint> complaints = await _complaintRepo.GetAllComplaintsAsync();

                if(complaints == null) return BadRequest("Error while trying to retrieve complaints");

                return Ok(complaints);

            }
            catch(Exception ex)
            {
                return BadRequest($"Error in the controller while trying to get all complaints: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetComplaintById")]
        public async Task<ActionResult<complaint>> GetComplaintById(int id)
        {
            try
            {
                if( id <= 0) return BadRequest("Please provide an Id");

                var complaint = await _complaintRepo.GetComplaintByIdAsync(id);

                if (complaint == null) return NotFound("No results");


                return Ok(complaint);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to get complaint by id: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetComplaintByUserEmail")]
        public async Task<ActionResult<IEnumerable<ComplaintDTO>>> GetComplaintByUserEmail(string userEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail)) return BadRequest("Please provide userEmail");

                IEnumerable<complaint> complaints = await _complaintRepo.GetComplaintByUserEmailAsync(userEmail);

                if (complaints == null) return NotFound("No results");

                var complaintDto = complaints.Select(x => new ComplaintDTO
                {
                    UserEmail = x.UserEmail,
                    AccussedEmail = x.AccussedEmail,
                    Issue = x.Issue,
                    ComplaintDate = x.ComplaintDate,
                    DaysPosted = x.DaysPosted,
                    Status = x.Status

                });

                return Ok(complaintDto);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to get complaints by user email: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetComplaintByAccusedEmail")]
        public async Task<ActionResult<IEnumerable<ComplaintDTO >>> GetComplaintByAccusedEmail(string accussedEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(accussedEmail)) return BadRequest("Please provide AccussedEmail");

                IEnumerable<complaint> complaints = await _complaintRepo.GetComplaintByAccusedEmailAsync(accussedEmail);

                if (complaints == null) return NotFound("No results");

                var complaintDto = complaints.Select(x => new ComplaintDTO
                {
                    UserEmail = x.UserEmail,
                    AccussedEmail = x.AccussedEmail,
                    Issue = x.Issue,
                    ComplaintDate = x.ComplaintDate,
                    DaysPosted = x.DaysPosted,
                    Status = x.Status

                });

                return Ok(complaintDto);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to get complaints by accussed email: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetComplaintsByDateRange")]
        public async Task<ActionResult<IEnumerable<complaint>>> GetComplaintsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {

                IEnumerable<complaint> complaints = await _complaintRepo.GetComplaintsByDateRangeAsync(startDate, endDate);

                if (complaints == null) return NotFound("No results");


                return Ok(complaints);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to get complaints by date range: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("AddComplaint")]
        public async Task<ActionResult<complaint>> AddComplaint(complaint complaintObj)
        {

            try
            {
                if (complaintObj == null || string.IsNullOrEmpty(complaintObj.UserEmail) || string.IsNullOrEmpty(complaintObj.AccussedEmail)
                    || string.IsNullOrEmpty(complaintObj.Issue))
                {
                    return BadRequest("Please provide the necessary information");
                }

                var userEmailExists = await _userRepo.UserExistsAsync(complaintObj.UserEmail);

                var accussedEmailExists = await _userRepo.UserExistsAsync(complaintObj.AccussedEmail);

                if (userEmailExists == null || accussedEmailExists == null) return NotFound("One of the emails does not exist");

                complaintObj.Status = "Submitted";
                complaintObj.DaysPosted = 0;

                var complaintAdded = await _complaintRepo.AddComplaintAsync(complaintObj);

                if (!complaintAdded) return BadRequest("Complaint not added");

                return Ok("Complaint added successfully");


            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to add a complaint: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("UpdateComplaint")]
        public async Task<ActionResult<complaint>> UpdateComplaint(UpdateComplaint updateComplaint)
        {

            try
            {
                if (updateComplaint == null || string.IsNullOrEmpty(updateComplaint.Issue))
                {
                    return BadRequest("Please provide the necessary information");
                }

                var complaintExists = await _complaintRepo.GetComplaintByIdAsync(updateComplaint.ComplaintId);

                if (complaintExists == null) return NotFound("Complaint does not exist");

                var complaintUpdated = await _complaintRepo.UpdateComplaintAsync(updateComplaint);


                if (!complaintUpdated) return BadRequest("Complaint not updated");

                return Ok("Complaint updated successfully");


            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to update the complaint: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateComplaintStatus")]
        public async Task<ActionResult<complaint>> UpdateComplaintStatus(int id, string status)
        {

            try
            {
                if (string.IsNullOrEmpty(status) || id <= 0)
                {
                    return BadRequest("Please provide the necessary information");
                }

                var complaintExists = await _complaintRepo.GetComplaintByIdAsync(id);

                if (complaintExists == null) return NotFound("Complaint does not exist"); 

                var complaintUpdated = await _complaintRepo.UpdateComplaintStatusAsync(id, status);

                if (!complaintUpdated) return BadRequest("Complaint not updated");

                return Ok("Complaint updated successfully");


            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to update the complaint status: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("DeleteComplaint")]
        public async Task<ActionResult<complaint>> DeleteComplaint(int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Please provide correct id");

                var complaintExists = await _complaintRepo.GetComplaintByIdAsync(id);

                if (complaintExists == null) return NotFound("Complaint does not exist");

                var complaintDeleted = await _complaintRepo.DeleteComplaintAsync(id);

                if (!complaintDeleted) return BadRequest("Complaint not updated");

                return Ok("Complaint deleted successfully");


            }
            catch (Exception ex)
            {
                return BadRequest($"Error in the controller while trying to delete the complaint status: {ex.Message}");
            }
        }
    }
}
