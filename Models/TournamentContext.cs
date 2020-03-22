using Microsoft.EntityFrameworkCore;

namespace PlatformaTurniejowaAPI.Models
{
    public class TournamentContext : DbContext
    {
        public TournamentContext(DbContextOptions<TournamentContext> options) : base(options)
        { }
        public DbSet<TournamentModel> Tournaments { get; set; }

        public DbSet<PlayerModel> Players { get; set; }

        public DbSet<Opponent> Opponents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
