using Microsoft.Extensions.DependencyInjection;
using nng.Enums;
using nng.Logging;
using nng.VkFrameworks;
using VkNet.Model;

namespace nng.Helpers;

public class CallbackHelper
{
    private readonly VkFramework _framework;
    private readonly LoggerWrapper _logger;

    public CallbackHelper(VkFramework framework, LoggerWrapper loggerWrapper)
    {
        _framework = framework;
        _logger = loggerWrapper;
    }

    public CallbackHelper(IServiceCollection collection)
    {
        var provider = collection.BuildServiceProvider();
        _framework = (VkFramework) (provider.GetService(typeof(VkFramework)) ??
                                    throw new ArgumentNullException(nameof(VkFramework)));
        _logger = (LoggerWrapper) (provider.GetService(typeof(LoggerWrapper)) ??
                                   throw new ArgumentNullException(nameof(LoggerWrapper)));
    }

    private void SetCallback(Group group, CallbackOperation operation, bool targetStatus, bool isAllowed = true)
    {
        if (!isAllowed) return;
        var servers = _framework.GetGroupCallbackServes(group).ToList();
        if (!servers.Any()) _logger.Log($"Не найдено Callback серверов у группы {group.Id}", LogType.Debug);
        foreach (var server in servers)
        {
            _framework.ChangeGroupCallbackSettings(group, server, operation, targetStatus);
            _logger.Log($"Поменяли Callback сервер {server.Id} c операцией {operation} на {targetStatus}",
                LogType.Debug);
        }
    }

    public void SetCallback(long group, CallbackOperation operation, bool targetStatus, bool isAllowed = true)
    {
        if (!isAllowed) return;
        var targetGroup = new Group {Id = group};
        SetCallback(targetGroup, operation, targetStatus);
    }
}
