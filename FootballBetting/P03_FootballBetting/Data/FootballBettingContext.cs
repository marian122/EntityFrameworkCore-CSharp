namespace P03_FootballBetting.Data
{
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {
     
        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamId);

                entity.HasMany(e => e.Players)
                .WithOne(t => t.Team);

                entity.HasMany(e => e.HomeGames)
                .WithOne(t => t.HomeTeam)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AwayGames)
                .WithOne(t => t.AwayTeam)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);

                entity.HasMany(e => e.PrimaryKitTeams)
                .WithOne(c => c.PrimaryKitColor)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.SecondaryKitTeams)
                .WithOne(t => t.SecondaryKitColor)
                .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity.HasMany(t => t.Teams)
                .WithOne(e => e.Town);
            });

            builder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);

                entity.HasMany(g => g.PlayerStatistics)
                .WithOne(ps => ps.Game);

                entity.HasMany(b => b.Bets)
                .WithOne(g => g.Game);
            });

            builder.Entity<Bet>(entity => 
            {
                entity.HasKey(b => b.BetId);

                entity.Property(b => b.Prediction)
                .IsRequired();

                entity.Property(e => e.Amount)
                .HasColumnType("MONEY")
                .IsRequired();
            });

            builder.Entity<Country>(country => 
            {
                country.HasKey(c => c.CountryId);

                country.HasMany(t => t.Towns)
                .WithOne(c => c.Country);
            });

            builder.Entity<Player>(player =>
            {
                player.HasKey(p => p.PlayerId);

                player.HasMany(ps => ps.PlayerStatistics)
                .WithOne(p => p.Player);

                player.Property(p => p.Name)
                .IsRequired();         
            });

            builder.Entity<PlayerStatistic>(ps =>
            {
                ps.HasKey(p => new { p.PlayerId, p.GameId });
            });

            builder.Entity<Position>(position =>
            {
                position.HasKey(p => p.PositionId);

                position.HasMany(p => p.Players)
                .WithOne(pos => pos.Position);

                position.Property(p => p.Name)
                .IsRequired();
            });

            builder.Entity<User>(user =>
            {
                user.HasKey(u => u.UserId);

                user.HasMany(b => b.Bets)
                .WithOne(u => u.User);
            });
        }
    }
}
