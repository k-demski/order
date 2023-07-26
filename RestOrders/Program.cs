using RestOrdersLib.Producers;
using RestOrdersLib.Models.DatabaseModels;
using RestOrdersLib.Repositories;
using RestOrdersLib.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.Configure<OrderDatabaseSettings>(
    builder.Configuration.GetSection("OrderDatabase"));

int redisDatabase;
int sentinel1Port;
int sentinel2Port;
int sentinel3Port;
int.TryParse(Environment.GetEnvironmentVariable("REDIS_DATABASE"), out redisDatabase);
int.TryParse(Environment.GetEnvironmentVariable("SENTINEL_1_PORT"), out sentinel1Port);
int.TryParse(Environment.GetEnvironmentVariable("SENTINEL_2_PORT"), out sentinel2Port);
int.TryParse(Environment.GetEnvironmentVariable("SENTINEL_3_PORT"), out sentinel3Port);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new ConfigurationOptions
    {
        EndPoints = {
            { Environment.GetEnvironmentVariable("SENTINEL_1_HOST"), sentinel1Port },
            { Environment.GetEnvironmentVariable("SENTINEL_2_HOST"), sentinel2Port },
            { Environment.GetEnvironmentVariable("SENTINEL_3_HOST"), sentinel3Port }
        },
        ServiceName = Environment.GetEnvironmentVariable("SENTINEL_SERVICENAME"),
        Password = Environment.GetEnvironmentVariable("REDIS_MASTER_PASSWORD"),
        ConnectRetry = 5,
        ReconnectRetryPolicy = new LinearRetry(1500),
        Ssl = false,
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
        SyncTimeout = 5000,
        DefaultDatabase = redisDatabase
    };
});

builder.Services.AddScoped<IOrderAggregateRepository, OrderAggregateRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
