namespace TournamentMS.Application.Queues
{
    public static class Queues
    {
        //to produce
        //users
        public const string GET_USER_BY_ID = "user.by_id";
        public const string USERS_BULK_INFO = "users.bulk.info";
        public const string GENERATE_PARTICIPANTS_TICKETS_ASYNC = "tournament.participant.tickets";

        //tickets
        public const string ASSIGN_ROLE_VIEWER = "viewer.role";
        public const string GET_BULK_TOURNAMENTS = "tournament.bulk.names";

        //streams
        public const string VALIDATE_MATCH_AND_ROLE = "match.role.user";
        public const string IS_FREE_MATCH_TOURNAMENT = "tournament.validate";

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
        public const string SEND_EMAIL_UPDATE_TOURNAMENT = "tournament.update";
    }
}
