using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TournamentMS.Application.EventHandler;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Application.Queues;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Infrastructure.EventBus
{
    public class EventBusConsumer: EventBusBase, IEventBusConsumer
    {
        //private IConnection _connection;
        //private IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQSettings _rabbitmqSettings;
        private readonly Dictionary<string, Func<string, Task<string>>> _handlers;
        private readonly Dictionary<string, Func<string, Task>> _eventHandlers;

        public EventBusConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options): base(options)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new();
            _eventHandlers = new();
            InitializeAsync().GetAwaiter().GetResult();
        }

        public static async Task<EventBusConsumer> CreateAsync(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options)
        {
            var instance = new EventBusConsumer(serviceScopeFactory, options);
            await instance.InitializeAsync();
            return instance;
        }


        private async Task InitializeAsync()
        {
            await base.InitializeAsync();

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            _ = Task.Run(async() =>
            {
                await RegisterEventHandlerAsync<AssignTeamMemberRequest>(Queues.ASSIGN_TEAM, async (request) =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var handler = scope.ServiceProvider.GetRequiredService<TeamMembershandler>();

                    await handler.AssignTeamMemberHandler(request);
                });
            });


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
                        Status = match.Status.ToString()
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

        /// <summary>
        ///     Register an async Event queue manager
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="handler"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task RegisterEventHandlerAsync<TEvent>(string queueName, Func<TEvent, Task> handler)
        {
            if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
            {
                await InitializeAsync();
            }

            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, null);
            await _channel.BasicQosAsync(0, 1, false);

            _eventHandlers[queueName] = async (message) =>
            {
                var @event = JsonConvert.DeserializeObject<TEvent>(message);
                await handler(@event);
            };

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (_eventHandlers.TryGetValue(ea.RoutingKey, out var handlerAsync))
                    {
                        await handlerAsync(message);
                    }
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                }
                catch (Exception ex)
                {
                    if (!_connection.IsOpen)
                    {
                        await InitializeAsync();
                    }
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

    }
}
