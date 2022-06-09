using Newtonsoft.Json;

namespace nng.Models;

public class DataModel
{
    public DataModel(long[] groupList, UserModel[] users)
    {
        GroupList = groupList;
        Users = users;
    }

    [JsonProperty("lst")] public long[] GroupList { get; set; }

    [JsonProperty("bnnd")] public UserModel[] Users { get; set; }
}
