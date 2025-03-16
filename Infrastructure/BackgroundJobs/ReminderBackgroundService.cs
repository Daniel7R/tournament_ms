using TournamentMS.Application.Interfaces;

namespace TournamentMS.Infrastructure.BackgroundJobs
{
    public class ReminderBackgroundService: BackgroundService
    {
        private readonly IReminderService _reminderService;

        public ReminderBackgroundService(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.UtcNow.AddHours(-5);
                //implemente to execute
                if (now.Hour == 0 && now.Minute == 1) // execute at midnight
                {
                  await _reminderService.SendReminder();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
