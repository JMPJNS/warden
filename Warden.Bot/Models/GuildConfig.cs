using System.ComponentModel.DataAnnotations;

namespace Warden.Bot.Models;

public class GuildConfig
{
    [Key]
    public ulong GuildId { get; set; }
    public ulong ScrimChannelId { get; set; }
    public ulong TeamCaptainRoleId { get; set; }
}