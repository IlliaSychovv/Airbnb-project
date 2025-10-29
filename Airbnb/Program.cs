using Airbnb.Application.DTO.Authorization;
using Airbnb.Infrastructure.Data;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using Airbnb.Application.Validators;
using FluentValidation.AspNetCore;
using Airbnb.Middlewares;
using Airbnb.Extensions;
using Mapster;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

TypeAdapterConfig.GlobalSettings.Scan(typeof(ApplicationUser).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(typeof(Apartment).Assembly);

builder.Services.AddConfiguration(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("BookingService"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()    
            .AddRuntimeInstrumentation()       
            .AddProcessInstrumentation()   
            .AddMeter("ApartmentService.Metrics")
            .AddMeter("BookingService.Metrics")
            .AddPrometheusExporter();           
    });

builder.Host.UseSerilog((context, services, configuration) => configuration
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "BookingService")
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)  
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "bookingservice-logs-{0:yyyy.MM.dd}",
        InlineFields = true
    })
);

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateDto>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagContext, httpContext) =>
    {
        var userId = httpContext.User?.Identity?.IsAuthenticated == true
            ? httpContext.User.Identity.Name
            : "Anonymous";

        diagContext.Set("UserId", userId);
    };
});

app.AddMapApartmentsQueriesEndpoints();
app.MapControllers();

app.Run();