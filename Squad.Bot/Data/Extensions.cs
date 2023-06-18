using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Squad.Bot.Data
{
    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            {
#pragma warning disable IDE0063 // Использовать простой оператор using
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<SquadDBContext>();
                    context.Database.EnsureCreated();
                }
#pragma warning restore IDE0063 // Использовать простой оператор using
            }
        }
    }
}
