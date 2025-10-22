using AuditService.Domain.Entity;
using MongoDB.Driver;

namespace AuditService.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
    
    public IMongoCollection<Audit> Audits => _database.GetCollection<Audit>("Audits");
}