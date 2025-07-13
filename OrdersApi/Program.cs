using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Models;
using OrdersApi.Utils;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add logger
var logDirectory = builder.Configuration["Logging:LogDirectory"] ?? "Logs";
Log.Logger = new LoggerConfiguration()
 .MinimumLevel.Information()
 .WriteTo.Console()
 .WriteTo.File(
 path: $"{logDirectory}/log-.txt",
 rollingInterval: RollingInterval.Day,
 fileSizeLimitBytes: 10_000_000,
 rollOnFileSizeLimit: true,
 shared: true
 )
 .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddScoped<IRequestValidator<Order>, OrderRequestValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrdersDbContext>(options =>
 options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

public partial class Program { }