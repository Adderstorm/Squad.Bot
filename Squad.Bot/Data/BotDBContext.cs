using Microsoft.EntityFrameworkCore;
using Squad.Bot.Models.Bot_special;

namespace Squad.Bot.Data
{
    public class BotDBContext : DbContext
    {
        public BotDBContext(in DbContextOptions<BotDBContext> options) : base(options) { }

        public DbSet<ServersToLogData> ServersToLogData { get; set; }
    }
}
