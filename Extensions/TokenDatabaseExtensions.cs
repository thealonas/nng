using nng.DatabaseModels;
using nng.DatabaseProviders;

namespace nng.Extensions;

public static class TokenDatabaseExtensions
{
    private static T Selector<T>(IEnumerable<T> list, Func<T, bool> selector)
    {
        return list.First(selector);
    }

    private static bool TryExecuteSelector<T>(IEnumerable<T> list, Func<T, bool> selector, out T? result)
    {
        try
        {
            result = Selector(list, selector);
            return true;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    public static PagesToken GetTokenWithPermission(this TokensDatabaseProvider provider, string permission)
    {
        var allTokens = provider.Collection.ToList();

        if (TryExecuteSelector(allTokens, token => token.HasPermissionHard(permission), out var result))
            return result ?? throw new InvalidOperationException();

        if (TryExecuteSelector(allTokens, token => token.HasPermission(permission), out result))
            return result ?? throw new InvalidOperationException();

        throw new InvalidOperationException();
    }

    public static PagesToken GetTokenWithPermissions(this TokensDatabaseProvider provider, string[] permissions)
    {
        var allTokens = provider.Collection.ToList();

        if (TryExecuteSelector(allTokens, token => token.HasPermissionsHard(permissions), out var result))
            return result ?? throw new InvalidOperationException();

        if (TryExecuteSelector(allTokens, token => token.HasPermissions(permissions), out result))
            return result ?? throw new InvalidOperationException();

        throw new InvalidOperationException();
    }
}
