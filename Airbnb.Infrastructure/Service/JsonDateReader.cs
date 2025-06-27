using Airbnb.Application.DTOs.Migrations;
using Airbnb.Application.Interfaces;
using Newtonsoft.Json;

namespace Airbnb.Infrastructure.Service;

public class JsonDateReader : IJsonDataReader
{
    public async IAsyncEnumerable<ExternalUser> ReadUsers(string filePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);
        
        var serializer = new JsonSerializer();
        while (await jsonReader.ReadAsync(cancellationToken))
        {
            if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value?.ToString() == "user")
            {
                await jsonReader.ReadAsync(cancellationToken);

                if (jsonReader.TokenType != JsonToken.StartArray)
                    throw new JsonException();

                while (await jsonReader.ReadAsync(cancellationToken))
                {
                    if (jsonReader.TokenType == JsonToken.EndArray)
                        break;
                    
                    var user = serializer.Deserialize<ExternalUser>(jsonReader);
                    if (user != null)
                        yield return user;
                }
            }
        }
    }

    public async IAsyncEnumerable<ExternalApartment> ReadApartment(string filePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(stream);
        await using var jsonReader = new JsonTextReader(streamReader);
        
        var serializer = new JsonSerializer();
        
        while (await jsonReader.ReadAsync(cancellationToken))
        {
            if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value?.ToString() == "apartment")
            {
                await jsonReader.ReadAsync(cancellationToken);
                
                if (jsonReader.TokenType != JsonToken.StartArray)
                    throw new JsonException();

                while (await jsonReader.ReadAsync(cancellationToken))
                {
                    if (jsonReader.TokenType == JsonToken.EndArray)
                        break;
                    
                    var apartment = serializer.Deserialize<ExternalApartment>(jsonReader);
                    if (apartment != null)
                        yield return apartment;
                }
            }
        } 
    }
}