using complaintApp_API.Data;
using complaintApp_API.Interfaces;
using complaintApp_API.Models;
using Dapper;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace complaintApp_API.Repositories
{
    public class ComplaintRepository : IComplaint
    {
        private readonly DataContext _context;

        public ComplaintRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddComplaintAsync(complaint complaint)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserEmail", complaint.UserEmail);
                parameters.Add("AccussedEmail", complaint.AccussedEmail);
                parameters.Add("Issue", complaint.Issue);
                parameters.Add("ComplaintDate", complaint.ComplaintDate);
                parameters.Add("Status", complaint.Status);
                parameters.Add("DaysPosted", complaint.DaysPosted);


                const string sql = $"Insert into complaints(UserEmail, AccussedEmail, Issue, ComplaintDate, Status, DaysPosted) " +
                    $"Values(@UserEmail, @AccussedEmail, @Issue, @ComplaintDate, @Status, @DaysPosted)";

                IDbConnection connection = _context.CreateConnection();

                var complaintAdded = await connection.ExecuteAsync(sql, parameters);

                return complaintAdded >= 0;

            } catch(Exception ex)
            {
                Console.WriteLine($"Error in the repo in the repo while trying to Add a complaint: {ex.Message}");

                return false;
            }
        }

        public async Task<bool> DeleteComplaintAsync(int id)
        {
            try
            {

                var parameter = new { ComplaintId = id };

                const string sql = "Delete From complaints where ComplaintId = @ComplaintId";

                IDbConnection connection = _context.CreateConnection();

                var complaintDeleted = await connection.ExecuteAsync(sql, parameter);

                return complaintDeleted >= 0;

            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Error in the repo  while trying to delete a complaint: {ex.Message}");

                return false;
            }

        }

        public async Task<IEnumerable<complaint>> GetAllComplaintsAsync()
        {
            try
            {
                const string sql = "Select * from complaints";

                IDbConnection connection = _context.CreateConnection();

                var complaints = await connection.QueryAsync<complaint>(sql);

                return complaints;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to get all complaints: {ex.Message}");

                return null;
            }

        }

        public async Task<IEnumerable<complaint>> GetComplaintByAccusedEmailAsync(string accussedEmail)
        {
            try
            {
                var parameter = new { AccussedEmail = accussedEmail };

                const string sql = "Select * from complaints where AccussedEmail = @AccussedEmail";

                IDbConnection connection = _context.CreateConnection();

                var complaints = await connection.QueryAsync<complaint>(sql, parameter);

                return complaints;

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to get complaint by accussed email: {ex.Message}");

                return null;
            }
        }

        public async Task<IEnumerable<complaint>> GetComplaintByUserEmailAsync(string userEmail)
        {
            try
            {
                var parameter = new { UserEmail = userEmail };

                const string sql = "Select * from complaints where UserEmail = @UserEmail";

                IDbConnection connection = _context.CreateConnection();

                var complaints = await connection.QueryAsync<complaint>(sql, parameter);

                return complaints;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to get complaint by user email: {ex.Message}");

                return null;
            }
        }

        public async Task<complaint> GetComplaintByIdAsync(int id)
        {
            try
            {
                var parameter = new { ComplaintId = id };

                const string sql = "Select * from complaints where ComplaintId = @ComplaintId";

                IDbConnection connection = _context.CreateConnection();

                var complaint = await connection.QueryFirstAsync<complaint>(sql, parameter);

                return complaint;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to get complaint by Complaint Id: {ex.Message}");

                return null;
            }
        }

        public async Task<IEnumerable<complaint>> GetComplaintsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("startDate", startDate);
                parameters.Add("endDate", endDate);

                const string sql = $"SELECT * FROM complaints WHERE ComplaintDate " +
                    $"BETWEEN @startDate AND @endDate ORDER BY ComplaintDate DESC";

                IDbConnection connection = _context.CreateConnection();

                var complaints = await connection.QueryAsync<complaint>(sql, parameters);

                return complaints;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to get complaint by date: {ex.Message}");

                return null;
            }
        }

        public async Task<bool> UpdateComplaintAsync(UpdateComplaint updateComplaint)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ComplaintId", updateComplaint.ComplaintId);
                parameters.Add("Issue", updateComplaint.Issue);
                parameters.Add("ComplaintDate", updateComplaint.ComplaintDate);

                const string sql = $"Update complaints SET Issue = @Issue, ComplaintDate = @ComplaintDate WHERE ComplaintId = @ComplaintId";

                IDbConnection connection = _context.CreateConnection();

                var complaintUpdated = await connection.ExecuteAsync(sql, parameters);

                return complaintUpdated > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to update a complaint: {ex.Message}");

                return false;
            }
        }

        public async Task<bool> UpdateComplaintStatusAsync(int id, string status)
        {
            try
            {
                var parameters = new { Status = status, ComplaintId = id };

                const string sql = $"Update complaints SET Status = @Status WHERE ComplaintId = @ComplaintId";

                IDbConnection connection = _context.CreateConnection();

                var complaintUpdated = await connection.ExecuteAsync(sql, parameters);

                return complaintUpdated > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to update complaint status: {ex.Message}");

                return false;
            }
        }

        public async Task<bool> UserEmailExistsAsync(string email)
        {
            try
            {
                var parameter = new { UserEmail = email };

                const string sql = "Select * from users where UserEmail = @UserEmail";

                IDbConnection connection = _context.CreateConnection();

                var complaintExists = connection.QueryFirstOrDefault<complaint>(sql, parameter);

                return complaintExists != null;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to check if User Email Exists: {ex.Message}");

                return false;
            }
        }

        public async Task<bool> AccussedEmailExistsAsync(string email)
        {
            try
            {
                var parameter = new { AccussedEmail = email };

                const string sql = "Select * from complaints where AccussedEmail = @AccussedEmail";

                IDbConnection connection = _context.CreateConnection();

                var complaintExists = connection.QueryFirstOrDefault<complaint>(sql, parameter);

                return complaintExists != null;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo  while trying to check if Accussed Email Exists: {ex.Message}");

                return false;
            }
        }

        public async Task UpdateComplaintDaysPosted(int id, int days)
        {
            try
            {
                var parameters = new { DaysPosted = days, ComplaintId = id };

                const string sql = $"Update complaints SET DaysPosted = @DaysPosted WHERE ComplaintId = @ComplaintId";

                IDbConnection connection = _context.CreateConnection();

                await connection.ExecuteAsync(sql, parameters);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the repo while trying to update complaint days posted: {ex.Message}");

            }
        }
    }
}
