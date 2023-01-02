namespace nng.Containers;

public static class VkScripts
{
    public const string GetAllMembersVkScript =
        @"var group = '{GROUP}';

        var apiResult = API.groups.getMembers({'group_id': group, 'offset': 0, 'sort': 'time_asc', 'fields': 'sex', 'count': 1000});
        var membersCount = apiResult['count'];
        
        var admins = API.groups.getMembers({'group_id': group, 'offset': 0, 'sort': 'time_asc', 'filter': 'managers', 'fields': 'sex', 'count': 1000});
        
        var count = 0;
        var users = [];
        var manager_count = 0;
        var managers = [];
        
        if (membersCount > 1000){
            var divisor = (membersCount / 1000);
            divisor = divisor + 1;
            var memberUsers = [];
            var i = 0;
            while (i < divisor){
                var query = API.groups.getMembers({'group_id': group, 'offset': i*1000, 'sort': 'time_asc', 'fields': 'sex', 'count': 1000})['items'];
                memberUsers = memberUsers + query;
                i = i + 1;
            }
        
            count = memberUsers.length;
            users = memberUsers; 
            manager_count = admins['count'];
            managers = admins['items'];
        } else {
            count = apiResult['count'];
            users = apiResult['items'];
            manager_count = admins['count'];
            managers = admins['items'];
        }
        
        return {'count': count, 'users': users, 'manager_count': manager_count, 'managers': managers};";

    /// <summary>
    ///     В этом случае в вызовах «API.groups.getMembers» используем
    ///     «fields: "sex"», чтобы VK вернул список из полных пользователей, в частности заблокированных (костыль)
    /// </summary>
    public const string GetAllMembersLegacyVkScript =
        @"var group = '{GROUP}';

        var apiResult = API.groups.getMembers({'group_id': group, 'offset': 0, 'sort': 'time_asc', 'count': 1000});
        var membersCount = apiResult['count'];
        
        var admins = API.groups.getMembers({'group_id': group, 'offset': 0, 'sort': 'time_asc', 'filter': 'managers', 'fields': 'sex', 'count': 1000});
        
        var count = 0;
        var users = [];
        var manager_count = 0;
        var managers = [];
        
        if (membersCount > 1000){
            var divisor = (membersCount / 1000);
            divisor = divisor + 1;
            var memberUsers = [];
            var i = 0;
            while (i < divisor){
                var query = API.groups.getMembers({'group_id': group, 'offset': i*1000, 'sort': 'time_asc', 'count': 1000})['items'];
                memberUsers = memberUsers + query;
                i = i + 1;
            }
        
            count = memberUsers.length;
            users = memberUsers; 
            manager_count = admins['count'];
            managers = admins['items'];
        } else {
            count = apiResult['count'];
            users = apiResult['items'];
            manager_count = admins['count'];
            managers = admins['items'];
        }
        
        return {'count': count, 'users': users, 'manager_count': manager_count, 'managers': managers};";

    public const string SetWallStatusVkScript =
        @"var group = {GROUP};
        var targetWall = {WALL};
        
        var settings = API.groups.getSettings({'group_id': group});
        var oldWall = settings.wall;
        
        API.groups.edit({'group_id': group, 'wall': targetWall});
        
        return {'old_wall': oldWall, 'current_wall': targetWall};
        ";

    public const string GetAllPostsVkScript =
        @"var domain = {GROUP};
        var offset = {OFFSET};
        
        offset = offset * 25000;
        
        var response = {'count': 0, 'items': []};
        var query = API.wall.get({'owner_id': domain, 'count': 100, 'offset': offset});
        
        var neededCount = query.count - offset;
        
        if (neededCount <= 100){
            response.items = query.items;
            response.count = query.count;
            return response;
        }
        
        var i = neededCount / 100;
        var localOffset = offset + 100;
        
        while (neededCount > 0){
            response.items = response.items + query.items;
            localOffset = localOffset + 100;
            neededCount = neededCount - 100;
            query = API.wall.get({'owner_id': domain, 'count': 100, 'offset': offset});
        }
        
        return response;";
}
