using System.ComponentModel.DataAnnotations;

namespace Warden.Data.Models;

public class GuildConfig
{
    [Key]
    public ulong GuildId { get; set; }
    public ulong ScrimChannelId { get; set; }
    public ulong RingerChannelId { get; set; }
    public ulong TeamCaptainRoleId { get; set; }
}