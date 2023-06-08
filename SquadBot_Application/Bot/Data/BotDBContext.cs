using Microsoft.EntityFrameworkCore;
using SquadBot_Application.Bot.Models.Bot_special;

namespace SquadBot_Application.Bot.Data
{
    public class BotDBContext : DbContext
    {
        public BotDBContext(DbContextOptions<BotDBContext> options) : base(options) { }

        public DbSet<ServersToLogData> ServersToLogData { get; set;}
    }
}
