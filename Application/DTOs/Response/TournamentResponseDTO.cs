namespace TournamentMS.Application.DTOs.Response
{
    public class TournamentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string GameName { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsFree { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
