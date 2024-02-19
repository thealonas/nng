using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class GroupStatsDatabaseProvider : DatabaseProvider<GroupStats>
{
    public GroupStatsDatabaseProvider(ILogger<DatabaseProvider<GroupStats>> logger, RedisConnectionProvider provider) :
        base(
            logger, provider, "stats")
    {
    }
}
