using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class SettingsDatabaseProvider : DatabaseProvider<Settings>
{
    public SettingsDatabaseProvider(ILogger<DatabaseProvider<Settings>> logger, RedisConnectionProvider provider) :
        base(logger, provider)
    {
    }
}
