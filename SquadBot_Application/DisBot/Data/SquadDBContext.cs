using Microsoft.EntityFrameworkCore;
using Squad.Bot.DisBot.Models.AI;
using Squad.Bot.DisBot.Models.Base;

namespace Squad.Bot.DisBot.Data
{
    public class SquadDBContext : DbContext
    {
        public SquadDBContext(DbContextOptions<SquadDBContext> options) : base(options)
        {
        }

        public DbSet<BlackList> BlackList { get; set; }
        public DbSet<PrivateRooms> PrivateRooms { get; set; }
        public DbSet<Servers> Servers { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Warns> Warns { get; set; }
        public DbSet<JoinDate> JoinDate { get; set; }
        public DbSet<MemberMessagesActivity> MemberMessagesActivity { get; set; }
        public DbSet<MembersActivity> MembersActivity { get; set; }
        public DbSet<MemberVoiceActivity> MemberVoiceActivity { get; set; }
        public DbSet<MessagesActivity> MessagesActivity { get; set; }
        public DbSet<NewMembers> NewMembers { get; set; }
        public DbSet<NewMembersActivity> NewMembersActivity { get; set; }
        public DbSet<NewMembersLeftWeek> NewMembersLeftWeek { get; set; }
        public DbSet<PeakOnline> PeakOnline { get; set; }
        public DbSet<TotalLeaves> TotalLeaves { get; set; }
        public DbSet<TotalMembers> TotalMembers { get; set; }
        public DbSet<VoiceActivity> VoiceActivity { get; set; }
    }

}
