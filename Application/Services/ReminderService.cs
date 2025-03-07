using TournamentMS.Application.Interfaces;

namespace TournamentMS.Application.Services
{
    public class ReminderService: IReminderService
    {

        public Task SendReminder()
        {
            Console.WriteLine("📧 Enviando recordatorio diario 🚀");
            throw new NotImplementedException();
            // Aquí puedes agregar la lógica para enviar el correo
        }
    }
}
