namespace TournamentMS.Application.Interfaces
{
    public interface IEventBusConsumer
    {
        void RegisterQueueHandler<TRequest, TResponse>(string queueName, Func<TRequest, Task<TResponse>> handler);
    }
}
