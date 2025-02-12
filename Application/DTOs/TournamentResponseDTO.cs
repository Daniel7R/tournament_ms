namespace TournamentMS.Application.DTO
{
    public class TournamentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string GameName { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
