using Newtonsoft.Json;
using nng.VkFrameworks;
using VkNet.Model;

namespace nng.Models;

/// <summary>
///     <para>Используется как объект десериализиции</para>
///     <see cref="VkFramework.GetGroupData">
///         <b>GetGroupData</b>
///     </see>
/// </summary>
public struct GroupData
{
    [JsonProperty("users")] public List<User> AllUsers { get; set; }

    [JsonProperty("managers")] public List<User> Managers { get; set; }

    [JsonProperty("screen_name")] public string ShortName { get; set; }

    [JsonProperty("count")] public int Count { get; set; }

    [JsonProperty("manager_count")] public int ManagerCount { get; set; }
}

/// <summary>
///     <para>Используется как объект десериализиции</para>
///     <see cref="VkFramework.GetGroupDataLegacy">
///         <b>GetGroupData</b>
///     </see>
/// </summary>
public struct GroupDataLegacy
{
    [JsonProperty("users")] public List<int> AllUsers { get; set; }

    [JsonProperty("managers")] public List<User> Managers { get; set; }

    [JsonProperty("screen_name")] public string ShortName { get; set; }

    [JsonProperty("count")] public int Count { get; set; }

    [JsonProperty("manager_count")] public int ManagerCount { get; set; }
}
