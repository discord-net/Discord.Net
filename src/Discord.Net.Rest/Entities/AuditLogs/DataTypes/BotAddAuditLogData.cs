using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest;

/// <summary>
///     Contains a piece of audit log data related to a adding a bot to a guild.
/// </summary>
public class BotAddAuditLogData : IAuditLogData
{
    private BotAddAuditLogData(IUser bot)
    {
        Target = bot;
    }

    internal static BotAddAuditLogData Create(BaseDiscordClient discord, EntryModel entry, Model log)
    {
        var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
        return new BotAddAuditLogData((userInfo != null) ? RestUser.Create(discord, userInfo) : null);
    }

    /// <summary>
    ///     Gets the bot that was added.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the bot is a 'Deleted User#....' because Discord does send user data for deleted users.
    /// </remarks>
    /// <returns>
    ///     A user object representing the bot.
    /// </returns>
    public IUser Target { get; }
}
