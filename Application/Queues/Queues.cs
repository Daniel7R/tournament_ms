namespace TournamentMS.Application.Queues
{
    public static class Queues
    {
        //to produce
        public const string GET_USER_BY_ID = "user.by_id";
        public const string GENERATE_PARTICIPANTS_TICKETS_ASYNC = "generate.tournament.participant.tickets.async";
        //to consume/process
        public const string GET_TOURNAMENT_BY_ID = "tournament.by_id";
        public const string GET_MATCH_BY_ID = "match.by_id";
    }
}
