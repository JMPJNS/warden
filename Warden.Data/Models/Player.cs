namespace Warden.Data.Models;

public class Player
{
    public int Id { get; set; }
    /// <summary>
    /// discord user id
    /// </summary>
    public ulong UserId { get; set; }
    
    public virtual Team Team { get; set; }
    public int TeamId { get; set; }
}