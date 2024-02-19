using System.Text.Json.Serialization;
using Redis.OM.Modeling;

namespace nng.DatabaseModels;

/// <summary>
///     Пользователь
/// </summary>
[Document(StorageType = StorageType.Json, Prefixes = new[] {"users"})]
public class User
{
    /// <summary>
    ///     Имя и фамилия на момент появления у нас
    /// </summary>
    [JsonPropertyName("name")]
    [Indexed(PropertyName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Айди страинцы
    /// </summary>
    [RedisIdField]
    [JsonPropertyName("user_id")]
    [Indexed(PropertyName = "user_id")]
    public long UserId { get; set; }

    /// <summary>
    ///     Является ли пользователь администратором
    /// </summary>
    [JsonPropertyName("admin")]
    [Indexed(PropertyName = "admin")]
    public bool Admin { get; set; }

    /// <summary>
    ///     Находится ли пользователь в списке приоритетных
    /// </summary>
    [JsonPropertyName("thx")]
    [Indexed(PropertyName = "thx", Sortable = true)]
    public bool Thanks { get; set; }

    /// <summary>
    ///     Пользовался ли человек ботом или приложением
    /// </summary>
    [JsonPropertyName("app")]
    [Indexed(PropertyName = "app")]
    public bool App { get; set; }

    /// <summary>
    ///     Список групп, в которых пользователь редактор
    /// </summary>
    [JsonPropertyName("groups")]
    [Indexed(CascadeDepth = 1, PropertyName = "groups")]
    public List<long>? Groups { get; set; }

    /// <summary>
    ///     Заблокирован ли пользователь на данный момент
    /// </summary>
    [JsonPropertyName("bnnd")]
    [Indexed(Sortable = true, PropertyName = "bnnd")]
    public bool Banned { get; set; }

    /// <summary>
    ///     Информация о нарушении правил (остаётся даже если человек был разблокирован)
    /// </summary>
    [JsonPropertyName("bnnd_info")]
    [Indexed(PropertyName = "bnnd_info")]
    public BannedInfo? BannedInfo { get; set; }

    /// <summary>
    ///     Дата последнего обновления
    /// </summary>
    [JsonPropertyName("upd")]
    [Indexed(PropertyName = "upd")]
    public DateTime LastUpdated { get; set; }

    public bool Validate()
    {
        return !string.IsNullOrEmpty(Name) && UserId > 0 &&
               (BannedInfo is null || (BannedInfo is not null && BannedInfo.Validate()));
    }
}

public class BannedInfo
{
    [JsonPropertyName("group_id")] public long? GroupId { get; set; }

    [JsonPropertyName("priority")] public int Priority { get; set; }

    [JsonPropertyName("complaint")] public long? Complaint { get; set; }

    [JsonPropertyName("date")] public DateTime? Date { get; set; }

    public bool Validate()
    {
        return GroupId is null or > 0 && Priority is >= 1 and <= 4 && Complaint is null or > 0;
    }
}
