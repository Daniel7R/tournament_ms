using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Application.Queues;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Infrastructure.EventBus
{
    public class EventBusConsumer: BackgroundService, IEventBusConsumer, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQSettings _rabbitmqSettings;
        private readonly Dictionary<string, Func<string, Task<string>>> _handlers;

        public EventBusConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options)
        {
            _rabbitmqSettings = options.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new();
        }

        public static async Task<EventBusConsumer> CreateAsync(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options)
        {
            var instance = new EventBusConsumer(serviceScopeFactory, options);
            await instance.InitializeAsync();
            return instance;
        }

        private async Task InitializeAsync()
        {
            var basePath = AppContext.BaseDirectory;
            var pfxCertPath = Path.Combine(basePath, "Infrastructure", "Security", _rabbitmqSettings.CertFile);
            if (!File.Exists(pfxCertPath))
            {
                throw new FileNotFoundException("PFX certificate not found");
            }

            var factory = new ConnectionFactory
            {
                HostName = _rabbitmqSettings.Host,
                UserName = _rabbitmqSettings.Username,
                Password = _rabbitmqSettings.Password,
                Port = _rabbitmqSettings.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                ContinuationTimeout = TimeSpan.FromSeconds(30),
                Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _rabbitmqSettings.ServerName,
                    CertPath = pfxCertPath,
                    CertPassphrase = _rabbitmqSettings.CertPassphrase,
                    Version = System.Security.Authentication.SslProtocols.Tls12
                }
            };
            while (_connection == null || !_connection.IsOpen || _channel == null || _channel.IsClosed)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                }
                catch (Exception ex)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            RegisterQueueHandler<GetTournamentById, GetTournamentByIdResponse>(Queues.GET_TOURNAMENT_BY_ID, async (request) =>
            {
                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    ITournamentRepository tournamentRepository = scope.ServiceProvider.GetRequiredService<ITournamentRepository>();
                    Tournament? tournament = await tournamentRepository.GetByIdAsync(request.IdTournament);

                    return new GetTournamentByIdResponse
                    {
                        IdTournament = tournament?.Id ?? 0,
                        IsFree = tournament?.IsFree ?? false,
                    };
                }
            });

            RegisterQueueHandler<int, GetMatchByIdResponse>(Queues.GET_MATCH_INFO, async (idMatch) =>
            {
                using(IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    IRepository<Matches> matchesRepo = scope.ServiceProvider.GetRequiredService<IRepository<Matches>>();
                    Matches? match = await matchesRepo.GetByIdAsync(idMatch);
                    if(match == null)
                    {
                        return new GetMatchByIdResponse();
                    }
                    return new GetMatchByIdResponse
                    {
                        IdMatch = match?.Id ?? 0,
                        Date = match.Date,
                        Name =match.Name,
                        Status = match.Status?? string.Empty
                    };
                }
            });
        }

        public void RegisterQueueHandler<TRequest, TResponse>(string queueName, Func<TRequest, Task<TResponse>> handler) 
        {
            if (_channel == null) throw new InvalidOperationException("EventBusRabbitMQ is not initialized.");
            if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
            {
                Task.Run(InitializeAsync).GetAwaiter().GetResult();
            }
            _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, null).Wait();
            _channel.BasicQosAsync(0, 1, false);

            _handlers[queueName] = async (message) =>
            {
                var request = JsonConvert.DeserializeObject<TRequest>(message);
                var response = await handler(request);
                return JsonConvert.SerializeObject(response);
            };
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body =ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var replyProps = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId,
                };

                try
                {
                    if(_handlers.TryGetValue(ea.RoutingKey, out var handler))
                    {
                        var responseMessage = await handler(message);
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        await _channel.BasicPublishAsync(exchange: "", routingKey: ea.BasicProperties.ReplyTo, mandatory: false, basicProperties: replyProps, body: responseBytes);
                    }
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                } catch(Exception ex)
                {
                    if (!_connection.IsOpen)
                    {
                        await InitializeAsync();
                    }

                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer).Wait();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen) await InitializeAsync();
                try
                {
                    await Task.Delay(500, stoppingToken);
                }
                catch (Exception ex)
                {
                    await InitializeAsync();
                }
            }
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
