using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squad.Bot.Models.Base;

namespace Squad.Bot.Data
{
    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            {
#pragma warning disable IDE0063 // Use a simple using statement
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<SquadDBContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    var guild = new Guilds { Id = 909801532126543874, ServerName = "Squad" };
                    var privateRoom = new PrivateRooms { CategoryID = 1214971512780623902, ChannelID = 1214971514278117396, Guilds = guild, SettingsChannelID = 1214971516190728253 };
                    context.Guilds.Add(guild);
                    context.PrivateRooms.Add(privateRoom);
                    context.SaveChanges();
                }
#pragma warning restore IDE0063 // Use a simple using statement
            }
        }
    }
}
