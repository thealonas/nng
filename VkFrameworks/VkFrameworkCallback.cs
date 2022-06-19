using nng.Enums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace nng.VkFrameworks;

public partial class VkFramework
{
    /// <summary>
    ///     Возвращает все <see cref="CallbackServerItem"> Callback сервера </see> сообщества
    /// </summary>
    /// <param name="group">Группа</param>
    /// <returns>
    ///     <see cref="CallbackServerItem">Список callback серверов</see>
    /// </returns>
    /// <exception cref="VkApiException">Ошибка</exception>
    public IEnumerable<CallbackServerItem> GetGroupCallbackServes(Group group)
    {
        var servers = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetCallbackServers((ulong) group.Id));
        return servers.Where(x => x.Url.ToLower().Contains("cbbot.ifx.su"));
    }

    /// <summary>
    ///     Меняет настройки Callback сервера
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="server">Сервер</param>
    /// <param name="operation">Тип операции</param>
    /// <param name="value">Значение параметров</param>
    /// <exception cref="VkApiException">Ошибка</exception>
    public void ChangeGroupCallbackSettings(Group group, CallbackServerItem server, CallbackOperation operation,
        bool value)
    {
        var settings = operation switch
        {
            CallbackOperation.Block => new CallbackSettings
                {UserBlock = value, UserUnblock = value, GroupLeave = value},
            CallbackOperation.Editor => new CallbackSettings
            {
                GroupOfficersEdit = value
            },
            CallbackOperation.Wall => new CallbackSettings
            {
                WallRepost = value, WallPostNew = value, GroupChangeSettings = value
            },
            _ => new CallbackSettings()
        };
        SetGroupCallbackSettings(group, server, settings);
    }

    private void SetGroupCallbackSettings(IVkModel group, CallbackServerItem server, CallbackSettings settings)
    {
        VkFrameworkExecution.Execute(() => Api.Groups.SetCallbackSettings(new CallbackServerParams
        {
            CallbackSettings = settings,
            GroupId = (ulong) group.Id,
            ServerId = server.Id
        }));
    }
}
