using complaintApp_API.Interfaces;
using System.Linq.Expressions;

namespace complaintApp_API.Services
{
    public class DaysPostedService : IHostedService 
    {
        private readonly IComplaint _complaintRepo;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DaysPostedService(IComplaint complaint)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _complaintRepo = complaint;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => UpdateDaysPosted(_cancellationTokenSource.Token), cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public async Task UpdateDaysPosted(CancellationToken cancellationToken)
        {
            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    var complaints = await _complaintRepo.GetAllComplaintsAsync();

                    foreach (var complaint in complaints)
                    {
                        complaint.DaysPosted = (DateTime.Now - complaint.ComplaintDate).Days;

                        await _complaintRepo.UpdateComplaintDaysPosted(complaint.ComplaintId, complaint.DaysPosted);

                        Console.WriteLine($"Complaint: {complaint.ComplaintId} was posted: {complaint.DaysPosted}");
                    }

                    await Task.Delay(TimeSpan.FromHours(24), cancellationToken);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the DaysPosted Service {ex.Message}");
            }
        }
    }
}
