using nng.DatabaseModels;

namespace nng.Extensions;

public static class PagesTokenListExtensions
{
    public static bool TryGetToken(this List<PagesToken> tokens, string perms, out PagesToken? pagesToken)
    {
        if (tokens.Any(x => x.HasPermissionHard(perms)))
        {
            pagesToken = tokens.First(x => x.HasPermissionHard(perms));
            return true;
        }

        if (tokens.Any(x => x.HasPermission(perms)))
        {
            pagesToken = tokens.First(x => x.HasPermission(perms));
            return true;
        }

        pagesToken = null;
        return false;
    }
}
