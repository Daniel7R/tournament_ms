using System.Text;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class ReminderService: IReminderService
    {
        IServiceProvider _serviceProvider;
        IEventBusProducer _eventBusProducer;

        public ReminderService(IServiceProvider service, IEventBusProducer producer)
        {
            _serviceProvider= service;
            _eventBusProducer = producer;
        }
        public async Task SendReminder()
        {
            Console.WriteLine("📧 Enviando recordatorio diario 🚀");
            using var scope = _serviceProvider.CreateScope();
            var _tournamentRepo = scope.ServiceProvider.GetRequiredService<ITournamentRepository>();

            var tournamentMathces = await _tournamentRepo.GetTournamentsAndMatchesCurrentDay();

            //if matches send reminder
            if (tournamentMathces.Any())
            {
                StringBuilder body = new StringBuilder();
                body.AppendLine("<p>Matches scheduled for today:</p>");
                body.AppendLine("<ul>");
                foreach (var tournament in tournamentMathces)
                {
                    body.AppendLine($"<li><h3>🏆 {tournament.Name}</h3></li>");
                    body.AppendLine("<ul>");

                    foreach (var match in tournament.Matches)
                    {
                        var teams = match.TeamsMatches?.Select(t => t.Team.Name).ToList();
                        string teamsInfo = (teams != null && teams.Count > 0) ? string.Join(" vs ", teams) : "Not definided teams";

                        body.AppendLine("<li>");
                        body.AppendLine($"<strong>⚽ Match:</strong> {match.Name} <br>");
                        body.AppendLine($"<strong>📅 Date:</strong> {match.Date:dd/MM/yyyy HH:mm} <br>");
                        body.AppendLine($"<strong>📍 Teams:</strong> {teamsInfo} <br>");
                        body.AppendLine("</li><hr>");
                    }

                    body.AppendLine("</ul>");
                }

                body.AppendLine("</ul>");

                var emailreminder = new EmailBulkNotificationRequest
                {
                    Body = body.ToString(),
                    Subject = "🔔 Reminder Today Matches🔔 "
                };

                await _eventBusProducer.PublishEventAsync<EmailBulkNotificationRequest>(emailreminder, Queues.Queues.REMINDER);
            }
        }
    }
}
