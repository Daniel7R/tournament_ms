using Azure.Core;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TournamentMS.Application.Interfaces;
//producer
namespace TournamentMS.Infrastructure.EventBus
{
    public class EventBusProducer: IEventBusProducer, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly RabbitMQSettings _rabbitmqSettings;

        public EventBusProducer(IOptions<RabbitMQSettings> option)
        {
            _rabbitmqSettings = option.Value;
            InitiliazeAsync().GetAwaiter().GetResult();
        }

        private async Task InitiliazeAsync()
        {
            var factory = new ConnectionFactory { HostName = _rabbitmqSettings.Host, UserName= _rabbitmqSettings.Username, Password= _rabbitmqSettings.Password, Port= _rabbitmqSettings.Port };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request, string queueName)
        {
            await _channel.QueueDeclareAsync(queue: queueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);
            var replyQueue = await _channel.QueueDeclareAsync();
            var replyQueueName = replyQueue.QueueName;

            var correlationId = Guid.NewGuid().ToString();

            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueueName
            };

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

            await _channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, basicProperties: props, body: messageBytes);
            
            var tcs = new TaskCompletionSource<TResponse>();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    tcs.SetResult(response);
                }
            };

            await _channel.BasicConsumeAsync(consumer: consumer, queue: replyQueueName, autoAck: false);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            return await tcs.Task;
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }

    }
}
