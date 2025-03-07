namespace TournamentMS.Application.Queues
{
    public static class Queues
    {
        //to produce
        public const string GET_USER_BY_ID = "user.by_id";
        public const string GENERATE_PARTICIPANTS_TICKETS_ASYNC = "tournament.participant.tickets";
        //to consume/process
        //public const string GET_TOURNAMENT_BY_ID = "tournament.by_id";
        public const string GET_MATCH_BY_ID = "match.by_id";

        //produced in PaymentsMS
        public const string GET_TOURNAMENT_INFO = "tournament.info";
        public const string GET_MATCH_INFO = "match.info";
        public const string GET_TOURNAMENT_BY_ID = "tournament.by_id";
        public const string ASSIGN_TEAM = "team.assign";
        //NOTIFICATIONS
        public const string SEND_EMAIL_CREATE_TOURNAMENT= "tournament.created";
    }
}
