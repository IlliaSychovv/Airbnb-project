namespace Airbnb.Application.Interfaces;

public interface INpgsqlProvider
{
    string Get(string path);
}