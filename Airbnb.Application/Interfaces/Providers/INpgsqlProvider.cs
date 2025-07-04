namespace Airbnb.Application.Interfaces.Providers;

public interface INpgsqlProvider
{
    string Get(string path);
}