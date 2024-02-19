using System.Text.Json.Serialization;
using Redis.OM.Modeling;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace nng.DatabaseModels;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"archive:stats"}, IndexName = "archive:stats")]
public class GroupArchiveStats
{
    [RedisIdField]
    [JsonPropertyName("date")]
    [Indexed(PropertyName = "date")]
    public string Date { get; set; }

    [JsonPropertyName("stats")]
    [Indexed(PropertyName = "stats")]
    public List<GroupArchiveStat> Stats { get; set; }
}

public class GroupArchiveStat
{
    [JsonPropertyName("group_id")]
    [Indexed(PropertyName = "group_id")]
    public long GroupId { get; set; }

    [JsonPropertyName("members")]
    [Indexed(PropertyName = "members")]
    public long Members { get; set; }

    [JsonPropertyName("managers")]
    [Indexed(PropertyName = "managers")]
    public int Managers { get; set; }
}
