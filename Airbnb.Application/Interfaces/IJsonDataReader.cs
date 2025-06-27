using Airbnb.Application.DTOs.Migrations;

namespace Airbnb.Application.Interfaces;

public interface IJsonDataReader
{
    IAsyncEnumerable<ExternalUser> ReadUsers(string filePath, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ExternalApartment> ReadApartment(string filePath, CancellationToken cancellationToken = default);
}