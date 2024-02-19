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
    /// <exception cref="VkApiException">Ошибка в методе</exception>
    public User GetUser(long id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(new List<long> {id}).First());
    }

    /// <summary>
    ///     Ищет пользователя по заданному screen_name
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Объект пользователя</returns>
    /// <exception cref="VkApiException">Ошибка в методе</exception>
    public User GetUser(string id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(new List<string> {id}).First());
    }

    /// <summary>
    ///     Ищет пользователя по заданному screen_name
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>Объект пользователя</returns>
    /// <exception cref="VkApiException"></exception>
    public IEnumerable<User> GetUsers(IEnumerable<string> id)
    {
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Users.Get(id));
    }

    /// <summary>
    ///     Получить всех забаненных
    /// </summary>
    /// <param name="groupId">Группа</param>
    /// <returns>Список юзеров</returns>
    /// <exception cref="VkApiException">Ошибка</exception>
    public IEnumerable<User> GetBanned(long groupId)
    {
        var banned = VkFrameworkExecution.ExecuteWithReturn(()
            => Api.Call("execute.getAllBanned", new VkParameters
            {
                {"group", groupId}
            })).ToCollectionOf(response =>
        {
            var banInfo = BanInfo.FromJson(response["ban_info"]);
            var profile = User.FromJson(response["profile"]);
            return new GetBannedResult
            {
                BanInfo = banInfo,
                Group = new Group
                {
                    Id = groupId
                },
                Profile = profile
            };
        });

        return banned.Where(x => x.Profile is not null).Select(x => x.Profile);
    }

    /// <summary>
    ///     Получить всех забаненных
    /// </summary>
    /// <param name="groupId">Группа</param>
    /// <returns>Список юзеров</returns>
    /// <exception cref="VkApiException">Ошибка</exception>
    [Obsolete("Предпочтительнее GetBanned")]
    public IEnumerable<GetBannedResult> GetBannedAlt(long groupId)
    {
        var banned = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetBanned(groupId, 0, 200));
        if (banned.TotalCount <= 200)
            return banned.Where(x => x.Type == SearchResultType.Profile);

        var output = new List<GetBannedResult>();
        var divisor = (int) MathF.Ceiling(200 / (float) banned.TotalCount) + 1;
        for (var i = 0; i < divisor; i++)
        {
            var collection = VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetBanned(groupId, i * 200, 200))
                .Where(x => x.Type == SearchResultType.Profile);
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
    ///     <see cref="GroupData" />
    /// </returns>
    /// <exception cref="VkApiException">Ошибка при выполнении Execute скрипта</exception>
    /// <exception cref="TooManyRequestsException">Слишком много запросов</exception>
    public GroupData GetGroupData(long groupId)
    {
        var response = VkFrameworkExecution.ExecuteWithReturn(() =>
            Api.Call("execute.getAllMembers", new VkParameters
            {
                {"group", groupId}
            }));
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
    /// <exception cref="VkApiException">Ошибка</exception>
    public IEnumerable<Group> GetGroups(IEnumerable<string> id)
    {
        if (id == null) throw new NullReferenceException(nameof(id));
        var ids = id.ToList();
        return VkFrameworkExecution.ExecuteWithReturn(() => Api.Groups.GetById(ids, null, GroupsFields.All));
    }

    /// <summary>
    ///     Получить посты
    /// </summary>
    /// <param name="group">Группа</param>
    /// <returns><see cref="IEnumerable{T}">Список</see> <see cref="Post">постов</see> сообщества</returns>
    [Obsolete("Предпочительнее VkScrpit")]
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

    /// <summary>
    ///     Получить все посты из сообщества, используя VkScript
    /// </summary>
    /// <param name="group">Сообщество</param>
    /// <returns>Список постов в <see cref="WallGetObject" /></returns>
    /// <exception cref="InvalidOperationException">Ошибка</exception>
    public WallGetObject GetAllPostsVkScript(long group)
    {
        var result = VkFrameworkExecution.ExecuteWithReturn(() =>
        {
            var response = Api.Call("execute.getAllPosts", new VkParameters
            {
                {"group", group}
            });
            return new WallGetObject
            {
                TotalCount = response["count"],
                WallPosts = response["items"].ToListOf(Post.FromJson).ToReadOnlyCollection()
            };
        });

        return result ?? throw new InvalidOperationException();
    }
}
