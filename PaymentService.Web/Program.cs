using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentService.Application.Event;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Infrastructure.Services;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Shared.Kafka.Interfaces;
using Shared.Kafka.Kafka;
using Shared.Kafka.Options;
using Shared.Redis.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

builder.Services.Configure<RedisSettingsOption>(
    builder.Configuration.GetSection("Redis"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IRedisLock>(sp =>
{
    var redisOptions = sp.GetRequiredService<IOptions<RedisSettingsOption>>().Value;
    var muxer = ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
    var multiplexers = new List<RedLockMultiplexer> { new RedLockMultiplexer(muxer) };
    var factory = RedLockFactory.Create(multiplexers);
    var logger = sp.GetRequiredService<ILogger<RedisLock>>();
    var db = muxer.GetDatabase();

    return new RedisLock(factory, logger, db);
});

builder.Services.AddScoped<IBalanceRepository, BalanceRepository>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IKafkaMessageHandler<UserCreatedEvent>, UserCreatedBalanceHandler>();

builder.Services.AddHostedService<KafkaConsumer<UserCreatedEvent>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();