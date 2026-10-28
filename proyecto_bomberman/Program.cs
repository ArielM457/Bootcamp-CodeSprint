using Bomberman.Api.Data;
using Bomberman.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BombermanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddSingleton<BombService>();

var app = builder.Build();

app.MapControllers();

app.Run();
