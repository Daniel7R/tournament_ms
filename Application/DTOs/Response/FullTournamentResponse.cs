namespace TournamentMS.Application.DTOs.Response
{
    public class FullTournamentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private string Description { get; set; }
        public string CategoryName { get; set; }
        public string PrizeDescription { get; set; }
        public int MaxPlayers { get; set; }
        public double TotalPrize { get; set; }
        public string GameName { get; set; }
        public bool IsFree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
