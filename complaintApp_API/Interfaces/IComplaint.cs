using complaintApp_API.Models;

namespace complaintApp_API.Interfaces
{
    public interface IComplaint
    {
        public Task<IEnumerable<complaint>> GetAllComplaintsAsync();
        public Task<complaint> GetComplaintByIdAsync(int id);
        public Task<IEnumerable<complaint>> GetComplaintByUserEmailAsync(string userEmail);
        public Task<IEnumerable<complaint>> GetComplaintByAccusedEmailAsync(string accussedEmail);
        public Task<IEnumerable<complaint>> GetComplaintsByDateRangeAsync(DateTime startDate, DateTime endDate);
        public Task<bool> AddComplaintAsync(complaint complaint);
        public Task<bool> UpdateComplaintAsync(UpdateComplaint updateComplaint);
        public Task<bool> UpdateComplaintStatusAsync(int id, string status); 
        public Task<bool> DeleteComplaintAsync(int id);
        public Task<bool> UserEmailExistsAsync(string email);
        public Task<bool> AccussedEmailExistsAsync(string email);
        public Task UpdateComplaintDaysPosted(int id, int days);

    }
}
