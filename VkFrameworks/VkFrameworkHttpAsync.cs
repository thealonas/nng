using Newtonsoft.Json;
using nng.Exceptions;
using VkNet.Model;

namespace nng.VkFrameworks;

public partial class VkFrameworkHttp
{
    /// <summary>
    ///     <para> Асинхронно отправляет сообщение в диалог через HTTP </para>
    ///     <b>Предназначен для использования в сообществах</b>
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="keyboard">Клавиатура</param>
    /// <param name="peer">Айди диалога</param>
    /// <exception cref="VkFrameworkMethodException">Не удалось отправить сообщение</exception>
    public async Task SendMessageAsync(string? message, string? keyboard, long peer)
    {
        try
        {
            var values = new Dictionary<string, string?>
            {
                {"random_id", _random.Next(int.MaxValue).ToString()},
                {"peer_id", peer.ToString()},
                {"message", message},
                {"keyboard", keyboard},
                {"dont_parse_links", "1"},
                {"access_token", _token},
                {"v", UsingVkApiVersion}
            };
            var post = new FormUrlEncodedContent(values);
            var responseObject = await _client.PostAsync(VkApiEndpoint, post);
            var text = await responseObject.Content.ReadAsStringAsync();
            if (!text.Contains("error")) return;
            var error = JsonConvert.DeserializeObject<VkError>(text);
            if (error == null) return;
            throw new VkFrameworkMethodException(nameof(SendMessageAsync), text);
        }
        catch (Exception e)
        {
            if (e is VkFrameworkMethodException) throw;
            throw new VkFrameworkMethodException(nameof(SendMessageAsync), e);
        }
    }

    /// <summary>
    ///     Асинхронно переключает стену группы
    /// </summary>
    /// <param name="group">Айди группы</param>
    /// <param name="state">Включена ли стена</param>
    /// <param name="token">Токен сообщества или пользователя</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при переключении стены</exception>
    public async Task SwitchWallAsync(long group, bool state)
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

            var response = await _client.PostAsync(VkGroupsEditEndpoint, content);
            var text = await response.Content.ReadAsStringAsync();
            if (!text.Contains("error")) return;
            var error = JsonConvert.DeserializeObject<VkError>(text);
            if (error == null) return;
            throw new VkFrameworkMethodException(nameof(SwitchWallAsync), text);
        }
        catch (Exception e)
        {
            if (e is VkFrameworkMethodException) throw;
            throw new VkFrameworkMethodException(nameof(SwitchWallAsync), e);
        }
    }
}
