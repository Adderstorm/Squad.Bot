using Microsoft.EntityFrameworkCore;
using Squad.Bot.DisBot.Models.Bot_special;

namespace Squad.Bot.DisBot.Data
{
    public class BotDBContext : DbContext
    {
        public BotDBContext(DbContextOptions<BotDBContext> options) : base(options) { }

        public DbSet<ServersToLogData> ServersToLogData { get; set;}
    }
}
