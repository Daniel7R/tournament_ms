using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Text.Json.Serialization;
using TournamentMS.Application.EventHandler;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Mapping;
using TournamentMS.Application.Service;
using TournamentMS.Application.Services;
using TournamentMS.Infrastructure.Auth;
using TournamentMS.Infrastructure.BackgroundJobs;
using TournamentMS.Infrastructure.Data;
using TournamentMS.Infrastructure.EventBus;
using TournamentMS.Infrastructure.Repository;
using TournamentMS.Infrastructure.Swagger;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TournamentManagementMS API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SchemaFilter<EnumSchemaFilter>(); // Enables los enums as string
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//builder.Services.AddNpgsql<TournamentDbContext>(builder.Configuration.GetConnectionString("dbConnectionTournaments"));
builder.Services.AddDbContextPool<TournamentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("dbConnectionTournaments")));


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPrizeService, PrizeService>();
builder.Services.AddScoped<IUserTournamentRoleService, UserTournamentRoleService>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<ITournamentUserRoleRepository, TournamentUserRoleRepository>();
builder.Services.AddScoped<ITeamsRepository, TeamsRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITournamentValidations, TournamentService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();
builder.Services.AddSingleton<IReminderService, ReminderService>();
builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddScoped<TournamentHandler>();
builder.Services.AddScoped<UsersTournamentHandler>();
builder.Services.AddScoped<StreamsHandler>();

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IEventBusProducer, EventBusProducer>();
builder.Services.AddSingleton<IEventBusConsumer, EventBusConsumer>();

builder.Services.AddScoped<StreamsHandler>();


builder.Services.AddHostedService<EventBusProducer>();
builder.Services.AddHostedService<EventBusConsumer>();
builder.Services.AddHostedService<ReminderBackgroundService>();

/*
builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("rutaservice");
});*/

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

Metrics.SuppressDefaultMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpMetrics();

app.UseEndpoints(endpoints => {
    endpoints.MapMetrics();
});

app.MapGet("/", () => Results.Ok("Healthy"));
app.MapControllers();

app.Run();
