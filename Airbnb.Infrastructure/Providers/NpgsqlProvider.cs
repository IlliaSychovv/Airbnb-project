using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Providers;

namespace Airbnb.Infrastructure.Providers;

public class NpgsqlProvider : INpgsqlProvider
{
    public string Get(string path)
    {
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found exception", fullPath);
            
        return File.ReadAllText(fullPath);
    }
}