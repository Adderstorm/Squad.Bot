using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.AI;
using Squad.Bot.Models.Base;

namespace Squad.Bot.FunctionalModules.Events
{
    public class GuildEvent
    {
        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        public GuildEvent(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task OnGuildJoined(SocketGuild newGuild)
        {
            Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(g => g.Id == newGuild.Id);
            if (guild == default)
            {
                guild = new Guilds
                {
                    Id = newGuild.Id,
                    ServerName = newGuild.Name,
                };
                await _dbContext.AddAsync(guild);
            }
            else
            {
                guild.DeletedAt = null;
                _dbContext.Update(guild);
            }

            TotalMembers totalMembers = new()
            {
                Guilds = guild,
                TotalUsers = guild.TotalMembers.Count
            };

            await _dbContext.AddAsync(totalMembers);
            await _dbContext.SaveChangesAsync();
        }

        public async Task OnGuildLeft(SocketGuild oldGuild)
        {
            Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(g => g.Id == oldGuild.Id);

            if (guild == default)
            {
                _logger.LogError("Couldn't find old Guild {oldGuildName}, id = {id}", ex: new NullReferenceException(), oldGuild.Name, oldGuild.Id);
            }
            else
            {
                guild.DeletedAt = DateTime.UtcNow;

                _dbContext.Update(guild);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
