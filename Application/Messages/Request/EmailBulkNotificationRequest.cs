namespace TournamentMS.Application.Messages.Request
{
    public class EmailBulkNotificationRequest
    {
        /// <summary>
        ///  Email subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        ///  Body of the email
        /// </summary>
        public string Body { get; set; }
    }
}
