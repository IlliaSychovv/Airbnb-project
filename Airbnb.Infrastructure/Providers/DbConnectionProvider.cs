using System.Data;
using Airbnb.Application.Interfaces.Providers;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Airbnb.Infrastructure.Providers;

public class DbConnectionProvider : IDbConnectionProvider
{
    private readonly string _connectionString;

    public DbConnectionProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}