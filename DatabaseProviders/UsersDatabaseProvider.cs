using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class UsersDatabaseProvider : DatabaseProvider<User>
{
    public UsersDatabaseProvider(ILogger<DatabaseProvider<User>> logger, RedisConnectionProvider provider) : base(
        logger, provider)
    {
    }
}
