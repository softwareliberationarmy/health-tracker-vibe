using System.Data;

namespace HealthTracker.Api.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
