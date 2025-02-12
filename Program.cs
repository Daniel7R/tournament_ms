using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Mapping;
using TournamentMS.Application.Service;
using TournamentMS.Application.Services;
using TournamentMS.Infrastructure.Data;
using TournamentMS.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddNpgsql<AppDbTournamentContext>(builder.Configuration.GetConnectionString("dbConnectionTournaments"));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("rutaservice");
});


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
