using Bomberman.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<PlayerService>();

var app = builder.Build();

app.MapControllers();

app.Run();
