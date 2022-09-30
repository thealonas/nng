using Microsoft.Extensions.DependencyInjection;
using nng.Enums;
using nng.Services;

namespace nng.Logging;

public class Logger
{
    private readonly bool _debug;
    private readonly string? _name;
    private readonly Version _version;

    /// <summary>
    ///     Конструктор <see cref="Logger" />.
    /// </summary>
    /// <param name="collection">Коллекция с <see cref="ProgramInformationService" /></param>
    public Logger(IServiceCollection collection)
    {
        var provider = collection.BuildServiceProvider();
        var info = (ProgramInformationService) (provider.GetService(typeof(ProgramInformationService)) ??
                                                throw new ArgumentNullException(nameof(provider)));
        _version = info.Version;
        _debug = info.Debug;
    }

    /// <summary>
    ///     Конструктор <see cref="Logger" />.
    /// </summary>
    /// <param name="collection">Коллекция с <see cref="ProgramInformationService" /></param>
    /// <param name="name">Имя</param>
    public Logger(IServiceCollection collection, string name)
    {
        var provider = collection.BuildServiceProvider();
        var info = (ProgramInformationService) (provider.GetService(typeof(ProgramInformationService)) ??
                                                throw new ArgumentNullException(nameof(provider)));
        _version = info.Version;
        _debug = info.Debug;
        _name = name;
    }

    /// <summary>
    ///     Конструктор <see cref="Logger" />.
    /// </summary>
    /// <param name="info">Имя</param>
    public Logger(ProgramInformationService info)
    {
        _version = info.Version;
        _debug = info.Debug;
    }

    /// <summary>
    ///     Конструктор <see cref="Logger" />.
    /// </summary>
    /// <param name="info">Версия и дебаг</param>
    /// <param name="name">Имя</param>
    public Logger(ProgramInformationService info, string name)
    {
        _version = info.Version;
        _debug = info.Debug;
        _name = name;
    }

    private string GetVersion()
    {
        var ver = _version;
        return $"{ver.Major}.{ver.Minor}";
    }

    private void ProcessMessage(string message, string name, ConsoleColor color, bool withoutTitle,
        bool skipLine, bool withoutColor)
    {
        if (!withoutColor) Console.ForegroundColor = color;
        var output = withoutTitle ? $"{message}" : $"[{name}] {message}";
        if (skipLine) Console.WriteLine(output);
        else Console.Write(output);
        Console.ResetColor();
    }

    private bool IsAllowed(LogType type, bool force)
    {
        if (force) return true;
        return type != LogType.Debug || _debug;
    }

    /// <summary>
    ///     Выводит сообщение в консоль.
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="type">Тип сообщения</param>
    /// <param name="name">Имя</param>
    /// <param name="force">Принудительно отправить сообщение</param>
    /// <param name="withoutTitle">Без заголовка</param>
    /// <param name="skipLine">Пропускать пространство</param>
    /// <param name="withoutColor">Без цвета</param>
    public void Log(string message, LogType type = LogType.Info, string name = "nng",
        bool force = false, bool withoutTitle = false, bool skipLine = true, bool withoutColor = false)
    {
        if (!IsAllowed(type, force)) return;
        if (name == "nng" && _name != null) name = _name;
        switch (type)
        {
            case LogType.Info:
                ProcessMessage(message, name, ConsoleColor.Green, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.InfoVersionShow:
                ProcessMessage($"v{GetVersion()} | {message}", name,
                    ConsoleColor.Green, withoutTitle, skipLine,
                    withoutColor);
                break;
            case LogType.Warning:
                ProcessMessage(message, name, ConsoleColor.Blue, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.Error:
                ProcessMessage(message, name, ConsoleColor.Red, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.Debug:
                ProcessMessage(message, name, ConsoleColor.Gray, withoutTitle, skipLine, withoutColor);
                break;
            default:
                ProcessMessage(message, name, ConsoleColor.Green, withoutTitle, skipLine, withoutColor);
                break;
        }
    }

    /// <summary>
    ///     Выводит сообщение в консоль
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="withoutTitle">Без заголовка</param>
    /// <param name="skipLine">Пропустить пространство</param>
    /// <param name="withoutColor">Без цвета</param>
    public void Log(Message message, bool withoutTitle = false, bool skipLine = true, bool withoutColor = false)
    {
        if (!IsAllowed(message.Type, message.ForceSend)) return;
        switch (message.Type)
        {
            case LogType.Info:
                ProcessMessage(message.Text, message.Name, ConsoleColor.Green, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.InfoVersionShow:
                ProcessMessage($"v{GetVersion()} | {message.Text}", message.Name,
                    ConsoleColor.Green, withoutTitle, skipLine,
                    withoutColor);
                break;
            case LogType.Warning:
                ProcessMessage(message.Text, message.Name, ConsoleColor.Blue, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.Error:
                ProcessMessage(message.Text, message.Name, ConsoleColor.Red, withoutTitle, skipLine, withoutColor);
                break;
            case LogType.Debug:
                ProcessMessage(message.Text, message.Name, ConsoleColor.Gray, withoutTitle, skipLine, withoutColor);
                break;
            default:
                ProcessMessage(message.Text, message.Name, ConsoleColor.Green, withoutTitle, skipLine, withoutColor);
                break;
        }
    }

    /// <summary>
    ///     Выводит исплючение в консоль.
    /// </summary>
    /// <param name="e">Исключение</param>
    public void Log(Exception e)
    {
        Log($"Произошла ошибка: {e.GetType()}: {e.Message}", LogType.Debug);
    }


    /// <summary>
    ///     Очищает консоль.
    /// </summary>
    public void Clear()
    {
        Console.Clear();
    }


    /// <summary>
    ///     Очищает консоль и выводит сообщения.
    /// </summary>
    /// <param name="messages">Сообщения</param>
    public void Clear(IReadOnlyCollection<Message> messages)
    {
        Console.Clear();
        foreach (var message in messages) Log(message);
    }


    /// <summary>
    ///     Ожидает вывода сообщения.
    /// </summary>
    public void Idle()
    {
        Log("Нажмите Enter для продолжения…", LogType.Warning);
        Console.ReadKey();
    }
}
