using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;
using Restauran_API.SignalR; // Thêm namespace cho SignalR

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ðãng k? SignalR
builder.Services.AddSignalR();

IConfigurationRoot cf = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddDbContext<QLNhaHangContext>(opt =>
    opt.UseSqlServer(cf.GetConnectionString("cnn"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseStaticFiles();
// Ðãng k? endpoint cho SignalR Hub
app.MapHub<MenuItemHub>("/menuHub");

app.MapControllers();

app.Run();
