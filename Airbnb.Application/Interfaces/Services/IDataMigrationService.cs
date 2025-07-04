namespace Airbnb.Application.Interfaces.Services;

public interface IDataMigrationService
{
    Task RunAsync(string filePath, CancellationToken cancellationToken = default);
}