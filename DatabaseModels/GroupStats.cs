using System.Text.Json.Serialization;
using Redis.OM.Modeling;
using VkNet.Utils;

namespace nng.DatabaseModels;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"stats"}, IndexName = "stats")]
public class GroupStats
{
    [RedisIdField]
    [JsonPropertyName("group_id")]
    [Indexed(PropertyName = "group_id")]
    public long GroupId { get; set; }

    [JsonPropertyName("members")]
    [Indexed(PropertyName = "members")]
    public long Members { get; set; }

    [JsonPropertyName("managers")]
    [Indexed(PropertyName = "managers")]
    public int Managers { get; set; }

    public bool Validate()
    {
        return GroupId > 0 && Members >= 0 && Managers >= 0;
    }

    public static GroupStats FromJson(VkResponse response)
    {
        return new GroupStats
        {
            GroupId = long.Parse(response["id"]),
            Members = long.Parse(response["members"]),
            Managers = int.Parse(response["admins"])
        };
    }
}
