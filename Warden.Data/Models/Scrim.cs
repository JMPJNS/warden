namespace Warden.Data.Models;

public class Scrim
{
    public int Id { get; set; }
    /// <summary>
    /// list of discord user ids
    /// </summary>
    public List<ulong> Ringers { get; set; }

    public virtual Team Team1 { get; set; }
    public int Team1Id { get; set; }
    
    /// <summary>
    /// null if no other team has signed up as a scrim partner yet
    /// </summary>
    public virtual Team? Team2 { get; set; }
    public int? Team2Id { get; set; }
    
    public DateTimeOffset Time { get; set; }
    
    public bool Cancelled { get; set; }
}