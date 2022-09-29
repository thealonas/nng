using nng.Enums;

namespace nng.Logging;

public class Message
{
    public Message(string text, LogType type, string name = "nng one", bool forceSend = false)
    {
        Text = text;
        Type = type;
        Name = name;
        ForceSend = forceSend;
    }

    public string Text { get; set; }
    public LogType Type { get; }
    public string Name { get; }
    public bool ForceSend { get; }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text);
    }
}
