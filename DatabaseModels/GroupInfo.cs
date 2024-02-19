using System.Text.Json.Serialization;
using Redis.OM.Modeling;

namespace nng.DatabaseModels;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"groups"})]
public class GroupInfo
{
    /// <summary>
    ///     Айди группы (без минуса)
    /// </summary>
    [JsonPropertyName("group_id")]
    [Indexed(PropertyName = "group_id")]
    [RedisIdField]
    public long GroupId { get; set; }

    /// <summary>
    ///     Короткая ссылка группы
    /// </summary>
    [JsonPropertyName("screen_name")]
    [Indexed(PropertyName = "screen_name")]
    public string ScreenName { get; set; } = string.Empty;

    public bool Validate()
    {
        return GroupId > 0 && !string.IsNullOrEmpty(ScreenName);
    }
}
