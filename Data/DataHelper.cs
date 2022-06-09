using System.Net.Http.Headers;
using Newtonsoft.Json;
using nng.Models;

namespace nng.Data;

public static class DataHelper
{
    private const string DataUrl = "https://likhner.com/nng/data.json";

    public static DataModel GetData(string url)
    {
        string json;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            json = client.GetStringAsync(url).Result;
        }

        return JsonConvert.DeserializeObject<DataModel>(json) ?? throw new NullReferenceException(nameof(json));
    }

    public static DataModel GetData()
    {
        string json;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            json = client.GetStringAsync(DataUrl).Result;
        }

        return JsonConvert.DeserializeObject<DataModel>(json) ?? throw new NullReferenceException(nameof(json));
    }

    public static async Task<DataModel> GetDataAsync()
    {
        string json;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            json = await client.GetStringAsync(DataUrl);
        }

        return JsonConvert.DeserializeObject<DataModel>(json) ?? throw new NullReferenceException(nameof(json));
    }

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
