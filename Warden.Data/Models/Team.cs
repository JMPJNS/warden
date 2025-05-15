namespace Warden.Data.Models;

public class Team
{
    public int Id { get; set; }
    /// <summary>
    /// discord role id
    /// </summary>
    public ulong RoleId { get; set; }
    public string Name { get; set; }
    public virtual List<Player> Players { get; set; }
}