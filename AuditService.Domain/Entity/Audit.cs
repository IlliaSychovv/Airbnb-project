using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuditService.Domain.Entity;

public class Audit
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid? Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid? UserId { get; set; }
    public string ProfileJson { get; set; }
    public DateTime CreatedAt { get; set; }
}