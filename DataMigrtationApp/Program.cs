using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };
        
        var service = new ServiceCollection();
        
        service.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql("Host=localhost;Port=5432;Database=airbnb;Username=postgres;Password=i1112006s"));

        service.AddScoped<IJsonDataReader, JsonDateReader>();
        service.AddScoped<IDataMigrationService, DataMigrationService>();
        
        service.AddLogging(configure => configure.AddConsole());
        service.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        
        var serviceProvider = service.BuildServiceProvider();
        var migrationService = serviceProvider.GetRequiredService<IDataMigrationService>();

        const string filePath = "airbnbData.json";

        try
        {
            await migrationService.RunAsync(filePath, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex.Message);
        }
    }
}