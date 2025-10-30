using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuditService.Domain.Entity;

public class Apartment
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ApartmentId { get; set; }
    public string ProfileJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}