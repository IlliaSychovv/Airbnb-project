using Shared.Kafka.Interfaces;
using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using AuditService.Infrastructure.Clients;
using AuditService.Infrastructure.Data;
using AuditService.Infrastructure.Repositories;
using AuditService.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shared.Kafka.Kafka;
using Shared.Kafka.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IMonolithClient, MonolithClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5296");
});

builder.Services.AddScoped<IAuditService, AuditService.Application.Services.AuditService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IKafkaMessageHandler<AuditDto>, ProfileKafkaHandler>();

builder.Services.AddHostedService<KafkaConsumer<AuditDto>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var ex = exceptionHandlerFeature.Error;
            await exceptionHandler.TryHandleAsync(context, ex, CancellationToken.None);
        }
    });
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();