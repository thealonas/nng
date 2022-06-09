using Newtonsoft.Json;
using nng.Enums;

namespace nng.Models;

public struct UserModel
{
    [JsonProperty("id")] public long Id;

    [JsonProperty("name")] public string Name;

    [JsonProperty("priority")] public BanPriority Priority;

    [JsonProperty("warned")] public int Warnings;

    [JsonProperty("compliant")] public long[]? Complaint;

    [JsonProperty("deleted")] public long? Deleted;
}
