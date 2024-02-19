using System.Text.Json.Serialization;
using Redis.OM.Modeling;

namespace nng.DatabaseModels;

/// <summary>
///     Текущие настройки скриптов
/// </summary>
[Document(StorageType = StorageType.Json, Prefixes = new[] {"settings"})]
public class Settings
{
    /// <summary>
    ///     Комменатрий при блокировке пользователя
    /// </summary>
    [JsonPropertyName("ban_comment")]
    [Indexed(PropertyName = "ban_comment")]
    public string BanComment { get; set; } = string.Empty;

    /// <summary>
    ///     Максимальное количество редакторов на одного человека
    /// </summary>
    [JsonPropertyName("max_editors")]
    [Indexed(PropertyName = "max_editors")]
    public int EditorRestriction { get; set; }

    /// <summary>
    ///     Айди администратора (!), которому будут отправляться логи
    /// </summary>
    [JsonPropertyName("log_user")]
    [Indexed(PropertyName = "log_user")]
    public long LogUser { get; set; }

    /// <summary>
    ///     Сикрет приложения
    /// </summary>
    [JsonPropertyName("app_secret")]
    [Indexed(PropertyName = "app_secret")]
    public string AppSecret { get; set; } = string.Empty;

    public bool Validate()
    {
        return EditorRestriction is > 0 and <= 100 && LogUser > 0 && !string.IsNullOrEmpty(BanComment);
    }
}
