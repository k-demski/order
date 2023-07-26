using Microsoft.EntityFrameworkCore;
using OrdersEventHandler.Consumers;
using OrdersEventHandler.Models.DatabaseModels;
using OrdersEventHandler.Repositories;
using OrdersEventHandler.Services;

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionstring = $"server={dbHost}; port={dbPort}; database={dbName}; user={dbUser}; password={dbPassword}";

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring)));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHostedService<RabbitMQConsumer>();
//builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var orderDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        orderDbContext.Database.EnsureCreated();
    }
}

app.Run();
