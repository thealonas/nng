using Microsoft.Extensions.Logging;
using nng_api.Exceptions;
using nng.Services;
using Redis.OM;
using Redis.OM.Searching;

namespace nng.DatabaseProviders;

public abstract class DatabaseProvider<T> where T : notnull
{
    protected DatabaseProvider(ILogger<DatabaseProvider<T>> logger, RedisConnectionProvider provider)
    {
        Provider = provider;
        Collection = provider.RedisCollection<T>();
        Logger = logger;
        Init(typeof(T).Name.ToLower(), provider);
        Logger.LogInformation("Провайдер для базы {Name} был запущен", typeof(T).Name.ToLower());
    }

    protected DatabaseProvider(ILogger<DatabaseProvider<T>> logger, RedisConnectionProvider provider, string name)
    {
        Provider = provider;
        Collection = provider.RedisCollection<T>();
        Logger = logger;
        Init(name, provider);
        Logger.LogInformation("Провайдер для базы {Name} был запущен", name);
    }

    public IRedisCollection<T> Collection { get; }

    public RedisConnectionProvider Provider { get; }

    private ILogger<DatabaseProvider<T>> Logger { get; }


    private void Init(string name, RedisConnectionProvider provider)
    {
        Logger.LogInformation("Создаю индекс для {Name}…", name);
        try
        {
            new IndexerCreationService<T>(provider, name).CreateIndexIfNeeded().GetAwaiter().GetResult();
            Logger.LogInformation("Индекс для соеденения {Name} был создан", name);
        }
        catch (Exception e)
        {
            if (e is IndexHasBeenAlreadyCreated)
            {
                Logger.LogWarning("Индекс для соеденения {Name} уже был создан", name);
            }
            else
            {
                Logger.LogError(e, "При создании инедкса произошла ошибка: {Name}", name);
                throw;
            }
        }
    }
}
