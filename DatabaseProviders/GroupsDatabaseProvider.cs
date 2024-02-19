using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class GroupsDatabaseProvider : DatabaseProvider<GroupInfo>
{
    public GroupsDatabaseProvider(ILogger<DatabaseProvider<GroupInfo>> logger, RedisConnectionProvider provider)
        : base(logger, provider)
    {
    }
}
