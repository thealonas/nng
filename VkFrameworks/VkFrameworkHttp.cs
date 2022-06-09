using Newtonsoft.Json;
using nng.Exceptions;
using VkNet.Model;

namespace nng.VkFrameworks;

public partial class VkFrameworkHttp
{
    private const string VkApiEndpoint = "https://api.vk.com/method/messages.send";
    private const string VkGroupsEditEndpoint = "https://api.vk.com/method/groups.edit";
    private const string UsingVkApiVersion = "5.131";

    private readonly HttpClient _client;
    private readonly Random _random;
    private readonly string _token;

    public VkFrameworkHttp(string token)
    {
        _token = token;
        _client = new HttpClient();
        _random = new Random();
    }

    private Dictionary<string, string?> FormQuery(string? message, string? keyboard, long peer)
    {
        return new Dictionary<string, string?>
        {
            {"random_id", _random.Next(int.MaxValue).ToString()},
            {"peer_id", peer.ToString()},
            {"message", message},
            {"keyboard", keyboard},
            {"dont_parse_links", "1"},
            {"access_token", _token},
            {"v", UsingVkApiVersion}
        };
    }

    /// <summary>
    ///     Отправляет сообщение в диалог с клавиатурой
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="keyboard">Клавиатура</param>
    /// <param name="peer">Диалог</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка</exception>
    public void SendMessage(string? message, string? keyboard, long peer)
    {
        try
        {
            var values = FormQuery(message, keyboard, peer);
            var post = new FormUrlEncodedContent(values);
            var responseObject = _client.PostAsync(VkApiEndpoint, post);
            var text = responseObject.Result.Content.ReadAsStringAsync().Result;
            if (!text.Contains("error")) return;
            throw new VkFrameworkMethodException(nameof(SendMessage), text);
        }
        catch (Exception e)
        {
            if (e is VkFrameworkMethodException) throw;
            throw new VkFrameworkMethodException(nameof(SendMessage), e);
        }
    }

    /// <summary>
    ///     Переключает стену группы
    /// </summary>
    /// <param name="group">Айди группы</param>
    /// <param name="state">Включена ли стена</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при переключении стены</exception>
    public void SwitchWall(long group, bool state)
    {
        try
        {
            var postValues = new Dictionary<string, string>
            {
                {"group_id", group.ToString()},
                {"wall", state ? "3" : "0"},
                {"access_token", _token},
                {"v", UsingVkApiVersion}
            };
            var content = new FormUrlEncodedContent(postValues);

            var response = _client.PostAsync(VkGroupsEditEndpoint, content);
            var text = response.Result.Content.ReadAsStringAsync().Result;
            if (!text.Contains("error")) return;
            var error = JsonConvert.DeserializeObject<VkError>(text);
            if (error == null) return;
            throw new VkFrameworkMethodException(nameof(SwitchWall), error);
        }
        catch (Exception e)
        {
            if (e is VkFrameworkMethodException) throw;
            throw new VkFrameworkMethodException(nameof(SwitchWall), e);
        }
    }
}
