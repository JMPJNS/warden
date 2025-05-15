namespace Warden.Bot.Extensions;

public static class GuildUserExtensions
{
    /// <summary>
    /// Determines whether the specified user has a particular role by role ID.
    /// </summary>
    /// <param name="user">The guild user to check.</param>
    /// <param name="roleId">The role ID to look for.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    public static bool HasRole(this GuildUser user, ulong roleId)
    {
        return user.RoleIds.Contains(roleId);
    }
}