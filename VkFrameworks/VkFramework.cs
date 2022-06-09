using nng.Containers;
using nng.Exceptions;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils.AntiCaptcha;

namespace nng.VkFrameworks;

public class CaptchaEventArgs : EventArgs
{
    public CaptchaEventArgs(string methodName, int secondsToWait)
    {
        MethodName = methodName;
        SecondsToWait = secondsToWait;
    }

    public string MethodName { get; }
    public int SecondsToWait { get; }
}

public partial class VkFramework
{
    public VkFramework(string token)
    {
        Token = token;
        Api = new VkApi();
        CurrentUser = new User();
        Authorize(token);
    }

    public VkFramework()
    {
        Token = string.Empty;
        Api = new VkApi();
        Api.RequestsPerSecond = 1;
        CurrentUser = Api.Users.Get(ArraySegment<string>.Empty, ProfileFields.All).First();
        Api.UserId = CurrentUser.Id;
        CaptchaSecondsToWait = 10;
    }

    public string Token { get; private set; }

    public VkApi Api { get; private set; }

    /// <summary>
    ///     Текущий пользователь
    /// </summary>
    public User CurrentUser { get; set; }

    /// <summary>
    ///     Кол-во секунд, которое нужно ожидать при капчте (если <see cref="SetCaptchaSolver">CaptchaSolver</see> не
    ///     установлен)
    /// </summary>
    public static int CaptchaSecondsToWait
    {
        get => VkFrameworkExecution.WaitTime;
        set => VkFrameworkExecution.WaitTime = value;
    }

    public void Authorize(string token)
    {
        Token = token;
        Api.Authorize(new ApiAuthParams
        {
            AccessToken = token
        });
        Api.RequestsPerSecond = 1;
        CurrentUser = Api.Users.Get(ArraySegment<string>.Empty, ProfileFields.All).First();
        Api.UserId = CurrentUser.Id;
        CaptchaSecondsToWait = 10;
    }

    public static event EventHandler<CaptchaEventArgs>? OnCaptchaWait;

    public static void InvokeOnCaptchaWait(string methodName, int secondsToWait)
    {
        OnCaptchaWait?.Invoke(null, new CaptchaEventArgs(methodName, secondsToWait));
    }

    /// <summary>
    ///     Сеттер для <see cref="CaptchaSecondsToWait">SecondsToWait</see>
    /// </summary>
    /// <param name="secondsToWait">Секунды</param>
    public void SetSecondsToWait(int secondsToWait)
    {
        CaptchaSecondsToWait = secondsToWait;
    }

    /// <summary>
    ///     Устанавливает число запросов в секунду
    /// </summary>
    /// <param name="numberOfRequests">Число запросов в секунду</param>
    public void SetRequestsPerSeconds(int numberOfRequests)
    {
        Api.RequestsPerSecond = numberOfRequests;
    }

    /// <summary>
    ///     Устанавливает <see cref="ICaptchaSolver">ICaptchaSolver</see>
    /// </summary>
    /// <param name="solver">ICaptchaSolver</param>
    public void SetCaptchaSolver(ICaptchaSolver solver)
    {
        Api = new VkApi(null, solver);
        Api.Authorize(new ApiAuthParams
        {
            AccessToken = Token
        });
        Api.RequestsPerSecond = 1;
    }

    /// <summary>
    ///     Сбрасывает <see cref="ICaptchaSolver">ICaptchaSolver</see>
    /// </summary>
    public void ResetCaptchaSolver()
    {
        Api = new VkApi();
        Api.Authorize(new ApiAuthParams
        {
            AccessToken = Token
        });
        Api.RequestsPerSecond = 1;
    }

    /// <summary>
    ///     Выдает определенную роль участнику сообщества
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="group">Группа</param>
    /// <param name="role">Енум роли</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при выдаче редактора</exception>
    public void EditManager(long user, long group, ManagerRole? role)
    {
        VkFrameworkExecution.Execute(() => Api.Groups.EditManager(new GroupsEditManagerParams
        {
            UserId = user,
            GroupId = group,
            Role = role
        }));
    }

    /// <summary>
    ///     Аналогично <see cref="EditManager" />, но не использует <see cref="VkFrameworkExecution" />
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="group">Группа</param>
    /// <param name="role">Енум роли</param>
    /// <exception cref="CaptchaNeededException">Ошибка при каптче</exception>
    /// <exception cref="VkFrameworkMethodException">Ошибка</exception>
    [Obsolete("Use EditManager()")]
    public void EditManagerLegacy(long user, long group, ManagerRole? role)
    {
        try
        {
            Api.Groups.EditManager(new GroupsEditManagerParams
            {
                UserId = user,
                GroupId = group,
                Role = role
            });
        }
        catch (Exception e)
        {
            if (e is CaptchaNeededException) throw;
            throw new VkFrameworkMethodException(nameof(EditManagerLegacy), e);
        }
    }

    /// <summary>
    ///     Удалить фото
    /// </summary>
    /// <param name="photoId">Фото</param>
    /// <param name="ownerId">
    ///     <para>Айди владельца</para>
    ///     Для сообществ — в начале минус
    /// </param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при удалении фото</exception>
    public void DeletePhoto(ulong photoId, ulong ownerId)
    {
        VkFrameworkExecution.Execute(() => Api.Photo.Delete(photoId, (long?) ownerId));
    }

    /// <summary>
    ///     Удаляет пост в сообществе
    /// </summary>
    /// <param name="groupId">Группа</param>
    /// <param name="postId">Пост</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при удалении поста</exception>
    public void DeletePost(long groupId, long postId)
    {
        var resp = VkFrameworkExecution.ExecuteWithReturn(() => Api.Wall.Delete(-groupId, postId));
        if (!resp) throw new VkFrameworkMethodException(nameof(DeletePost), $"Не удалось удалить пост {postId}");
    }

    /// <summary>
    ///     Репостит пост в сообществе
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="post">Пост</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public void Repost(Group group, string post)
    {
        if (group == null) throw new NullReferenceException(nameof(group));
        Repost(group.Id, post);
    }

    public void Repost(long group, string post)
    {
        VkFrameworkExecution.Execute(() => Api.Wall.Repost(post, string.Empty, group, false));
    }

    /// <summary>
    ///     Блокирует пользователя в сообществе
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="user">Юзер</param>
    /// <param name="com">Комментарий</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка при блоке</exception>
    public void Block(Group group, User user, string com)
    {
        if (group == null) throw new NullReferenceException(nameof(group));
        Block(group.Id, user.Id, com);
    }

    /// <summary>
    ///     Блокирует пользователя в сообществе
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="user">Юзер</param>
    /// <param name="com">Комментарий</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public void Block(long group, long user, string com)
    {
        VkFrameworkExecution.Execute(() => Api.Groups.BanUser(new GroupsBanUserParams
        {
            GroupId = group,
            Comment = com.Trim(),
            CommentVisible = true,
            OwnerId = user,
            Reason = BanReason.Other
        }));
    }

    /// <summary>
    ///     Разблокирует пользователя в сообществе
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="user">Забаненный</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public void UnBlock(Group group, User user)
    {
        if (group == null) throw new NullReferenceException(nameof(group));
        UnBlock(group.Id, user.Id);
    }

    /// <summary>
    ///     Разблокирует пользователя в сообществе
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="user">Забаненный</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public void UnBlock(long group, long user)
    {
        VkFrameworkExecution.Execute(() => Api.Groups.Unban(group, user));
    }

    /// <summary>
    ///     Устанавливает стену
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="state">
    ///     true — <see cref="WallContentAccess.Restricted" />, false — <see cref="WallContentAccess.Off" />
    /// </param>
    /// <returns>
    ///     <see cref="WallContentAccess">Прошлое состояние стены</see>
    /// </returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public WallContentAccess SetWall(long group, bool state)
    {
        var wall = state ? WallContentAccess.Restricted : WallContentAccess.Off;
        return SetWall(group, wall);
    }

    /// <summary>
    ///     Устанавливает стену
    /// </summary>
    /// <param name="group">Группа</param>
    /// <param name="state">Значение стены</param>
    /// <returns>
    ///     <see cref="WallContentAccess">Прошлое состояние стены</see>
    /// </returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public WallContentAccess SetWall(long group, WallContentAccess state)
    {
        var code = VkScripts.SetWallStatusVkScript
            .Replace("{GROUP}", group.ToString())
            .Replace("{WALL}", ((int) state).ToString());
        var response = VkFrameworkExecution.ExecuteWithReturn(() => Api.Execute.Execute(code));
        return (WallContentAccess) int.Parse(response["old_wall"].ToString());
    }
}
