using Newtonsoft.Json;

namespace nng.Models;

public class DataModel
{
    public DataModel(long[] groupList, UserModel[] users, UserModelShort[] priorityUsers)
    {
        GroupList = groupList;
        Users = users;
        PriorityUsers = priorityUsers;
    }

    [JsonProperty("lst")] public long[] GroupList { get; set; }

    [JsonProperty("bnnd")] public UserModel[] Users { get; set; }

    [JsonProperty("thx")] public UserModelShort[] PriorityUsers { get; set; }
}
