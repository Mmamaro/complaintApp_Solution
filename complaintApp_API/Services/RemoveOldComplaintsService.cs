
using complaintApp_API.Interfaces;

namespace complaintApp_API.Services
{
    public class RemoveOldComplaintsService : IHostedService
    {
        private readonly IComplaint _complaintRepo;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public RemoveOldComplaintsService(IComplaint complaint)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _complaintRepo = complaint;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => DeleteOldComplaint(_cancellationTokenSource.Token), cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DeleteOldComplaint(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var complaints = await _complaintRepo.GetAllComplaintsAsync();

                foreach(var complaint in complaints)
                {
                    var monthsPosted = (DateTime.Now.Year - complaint.ComplaintDate.Year) * 12 + DateTime.Now.Month - complaint.ComplaintDate.Month;

                    if (monthsPosted >= 12) { await _complaintRepo.DeleteComplaintAsync(complaint.ComplaintId); }
                }

                await Task.Delay(TimeSpan.FromDays(31), cancellationToken);

            }
        }
    }
}
