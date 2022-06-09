using nng.Containers;
using nng.Exceptions;
using nng.Models;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace nng.VkFrameworks;

public partial class VkFramework
{
    private string GetShortName(long group)
    {
        var shortName = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups
            .GetById(ArraySegment<string>.Empty, group.ToString(), GroupsFields.All)
            .First().ScreenName ?? $"club{group}");
        return shortName;
    }
    
    /// <summary>
    ///     Ищет пользователя по заданному ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Объект пользователя</returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public User GetUser(long id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(new List<long> {id}).First());
    }

    /// <summary>
    ///     Ищет пользователя по заданному screen_name
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Объект пользователя</returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка в методе</exception>
    public User GetUser(string id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(new List<string> {id}).First());
    }

    /// <summary>
    ///     Ищет пользователя по заданному screen_name
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Объект пользователя</returns>
    /// <exception cref="VkFrameworkMethodException"></exception>
    public IEnumerable<User> GetUsers(IEnumerable<string> id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(id));
    }

    /// <summary>
    ///     Получить всех забаненных
    /// </summary>
    /// <param name="groupId">Группа</param>
    /// <returns>Список юзеров</returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка</exception>
    public IEnumerable<User> GetBanned(long groupId)
    {
        var banned = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetBanned(groupId, 0, 200));
        if (banned.TotalCount <= 200)
            return banned.Where(x => x.Type == SearchResultType.Profile).Select(x => x.Profile);

        var output = new List<User>();
        var divisor = (int) MathF.Ceiling(200 / (float) banned.TotalCount) + 1;
        for (var i = 0; i < divisor; i++)
        {
            var collection = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetBanned(groupId, i * 200, 200))
                .Where(x => x.Type == SearchResultType.Profile)
                .Select(x => x.Profile);
            output.AddRange(collection);
        }

        return output;
    }

    /// <summary>
    ///     Возвращает информацию о сообществе через Execute
    ///     Забаненные пользователи возвращаются, но роли будут недоступны
    /// </summary>
    /// <param name="groupId">Айди группы</param>
    /// <returns>
    ///     <see cref="GroupDataLegacy" />
    /// </returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка при выполнении Execute скрипта</exception>
    /// <exception cref="TooManyRequestsException">Слишком много запросов</exception>
    public GroupDataLegacy GetGroupDataLegacy(long groupId)
    {
        var response = VkFrameworkExecution.ExecuteWithReturn(() =>
            Api.Execute.Execute(VkScripts.GetAllMembersLegacyVkScript.Replace("{GROUP}", groupId.ToString())));
        var shortName = GetShortName(groupId);

        var data = new GroupDataLegacy
        {
            AllUsers = response["users"].ToListOf(x => int.Parse(x.ToString())),
            Managers = response["managers"].ToListOf(User.FromJson),
            Count = (int) response["count"],
            ManagerCount = (int) response["manager_count"],
            ShortName = shortName
        };

        return data;
    }

    /// <summary>
    ///     Возвращает информацию о сообществе через Execute
    ///     Забаненные пользователи возвращаются, но роли будут недоступны
    /// </summary>
    /// <param name="groupId">Айди группы</param>
    /// <returns>
    ///     <see cref="GroupData" />
    /// </returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка при выполнении Execute скрипта</exception>
    /// <exception cref="TooManyRequestsException">Слишком много запросов</exception>
    public GroupData GetGroupData(long groupId)
    {
        var response = VkFrameworkExecution.ExecuteWithReturn(() =>
            Api.Execute.Execute(VkScripts.GetAllMembersVkScript.Replace("{GROUP}", groupId.ToString())));
        var shortName = GetShortName(groupId);
        var data = new GroupData
        {
            AllUsers = response["users"].ToListOf(User.FromJson),
            Managers = response["managers"].ToListOf(User.FromJson),
            Count = (int) response["count"],
            ManagerCount = (int) response["manager_count"],
            ShortName = shortName
        };

        return data;
    }

    /// <summary>
    ///     Получить группы
    /// </summary>
    /// <param name="id">Список айди или адресов</param>
    /// <returns>Список групп</returns>
    /// <exception cref="VkFrameworkMethodException">Ошибка</exception>
    public IEnumerable<Group> GetGroups(IEnumerable<string> id)
    {
        if (id == null) throw new NullReferenceException(nameof(id));
        var ids = id.ToList();
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetById(ids, null, GroupsFields.All));
    }

    public IEnumerable<Post> GetAllPosts(long group)
    {
        var posts = VkFrameworkExecution.ExecuteWithReturn(() => Api.Wall.Get(new WallGetParams
        {
            OwnerId = -group,
            Count = 100
        }));

        if (posts.TotalCount < 100) return posts.WallPosts;

        var iterator = (int) MathF.Ceiling(posts.TotalCount / 100f);
        var output = new List<Post>();
        for (var i = 0; i < iterator; i++)
        {
            var localIterator = i;
            var collection = VkFrameworkExecution.ExecuteWithReturn(() => Api.Wall.Get(new WallGetParams
            {
                OwnerId = -group,
                Count = 100,
                Offset = (ulong) (localIterator * 100)
            })).WallPosts;
            output.AddRange(collection);
        }

        return output;
    }
}
