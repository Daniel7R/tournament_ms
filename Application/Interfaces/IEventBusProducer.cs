namespace TournamentMS.Application.Interfaces
{
    public interface IEventBusProducer
    {
        Task<TResponse> SendRequestAsync<TResquest, TResponse>(TResquest resquest, string queueName);
    }
}
