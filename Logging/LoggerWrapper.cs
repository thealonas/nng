using nng.Enums;

namespace nng.Logging;

public class LoggerWrapper
{
    private readonly Logger? _logger;

    public LoggerWrapper(Logger logger)
    {
        _logger = logger;
    }

    public LoggerWrapper()
    {
    }

    public void Log(string message, LogType type = LogType.Info, string name = "nng one",
        bool force = false, bool withoutTitle = false, bool skipLine = true, bool withoutColor = false)
    {
        _logger?.Log(message, type, name, force, withoutTitle, skipLine, withoutColor);
    }

    public void Log(Message message, bool withoutTitle = false, bool skipLine = true, bool withoutColor = false)
    {
        _logger?.Log(message, withoutTitle, skipLine, withoutColor);
    }
}
