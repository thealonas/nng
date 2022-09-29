using System.Net.Http.Headers;
using Newtonsoft.Json;
using nng.Models;

namespace nng.Helpers;

public static class DataHelper
{
    public static async Task<DataModel> GetDataAsync(string url)
    {
        string json;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            json = await client.GetStringAsync(url);
        }

        return JsonConvert.DeserializeObject<DataModel>(json) ?? throw new NullReferenceException(nameof(json));
    }
}
