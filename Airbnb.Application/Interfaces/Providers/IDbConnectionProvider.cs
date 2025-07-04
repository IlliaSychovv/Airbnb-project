using System.Data;

namespace Airbnb.Application.Interfaces.Providers;

public interface IDbConnectionProvider
{
    IDbConnection CreateConnection();
}