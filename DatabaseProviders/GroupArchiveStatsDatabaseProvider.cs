using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class GroupArchiveStatsDatabaseProvider : DatabaseProvider<GroupArchiveStats>
{
    public GroupArchiveStatsDatabaseProvider(ILogger<DatabaseProvider<GroupArchiveStats>> logger,
        RedisConnectionProvider provider) : base(logger, provider)
    {
    }
}
