using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TournamentMS.Application.EventHandler;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Mapping;
using TournamentMS.Application.Service;
using TournamentMS.Application.Services;
using TournamentMS.Infrastructure.Auth;
using TournamentMS.Infrastructure.Data;
using TournamentMS.Infrastructure.EventBus;
using TournamentMS.Infrastructure.Repository;
using TournamentMS.Infrastructure.Swagger;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITournamentValidations, TournamentService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();
builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddScoped<TeamMembershandler>();


builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IEventBusProducer, EventBusProducer>();
builder.Services.AddSingleton<IEventBusConsumer, EventBusConsumer>();

builder.Services.AddHostedService<EventBusProducer>();
builder.Services.AddHostedService<EventBusConsumer>();

/*
builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("rutaservice");
});*/

builder.AddAppAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
