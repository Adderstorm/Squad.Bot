using Discord.WebSocket;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.AI;
using Squad.Bot.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squad.Bot.Events
{
    public class Guild
    {
        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        public Guild(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task OnGuildJoined(SocketGuild newGuild)
        {
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds g) => g.Id == newGuild.Id);
            if (guild == null)
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
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds g) => g.Id == oldGuild.Id);

            if (guild == null)
            {
                _logger.LogError("Couldn't find old Guild {oldGuildName}, id = {id}",ex: new NullReferenceException(), oldGuild.Name, oldGuild.Id);
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
