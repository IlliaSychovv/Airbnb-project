namespace Airbnb.Application.Interfaces;

public interface IDataMigrationService
{
    Task RunAsync(string filePath, CancellationToken cancellationToken = default);
}