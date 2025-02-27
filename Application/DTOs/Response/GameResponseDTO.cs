namespace TournamentMS.Application.DTOs.Response
{
    public class GameResponseDTO
    {
        public int IdGame { get; set; }
        public required string Name { get; set; }
        public int Players { get; set; }
    }
}
