using Microsoft.Extensions.Logging;
using nng.DatabaseModels;
using Redis.OM;

namespace nng.DatabaseProviders;

public class TokensDatabaseProvider : DatabaseProvider<PagesToken>
{
    public TokensDatabaseProvider(ILogger<DatabaseProvider<PagesToken>> logger, RedisConnectionProvider provider) :
        base(logger, provider, "tokens")
    {
    }
}
