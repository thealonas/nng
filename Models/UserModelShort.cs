using Newtonsoft.Json;

namespace nng.Models;

public struct UserModelShort
{
    [JsonProperty("id")] public long Id;

    [JsonProperty("name")] public string Name;
}
