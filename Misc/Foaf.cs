namespace nng.Misc;

public static class Foaf
{
    private const string FoafEndpoint = "https://vk.com/foaf.php";

    public static string GetAccountUrl(int id)
    {
        return $"{FoafEndpoint}?id={id}";
    }

    public static Uri GetAccountUri(int id)
    {
        return new Uri($"{FoafEndpoint}?id={id}");
    }
}
