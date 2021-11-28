using ChristmasCalendar.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChristmasCalendar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DailyScore> DailyScore { get; set; } = null!;
        public DbSet<Door> Doors { get; set; } = null!;
        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<FirstTimeOpeningDoor> FirstTimeOpeningDoor { get; set; } = null!;
        public DbSet<DailyScoreLastUpdated> DailyScoreLastUpdated { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DailyScore>().ToTable(nameof(DailyScore));
            builder.Entity<Door>().ToTable(nameof(Door));
            builder.Entity<Answer>().ToTable(nameof(Answer));
            builder.Entity<FirstTimeOpeningDoor>().ToTable(nameof(FirstTimeOpeningDoor));
            builder.Entity<DailyScoreLastUpdated>().ToTable(nameof(DailyScoreLastUpdated));

            builder.Entity<DailyScore>()
                .HasIndex(u => new { u.DoorId, u.UserId })
                .IsUnique();

            base.OnModelCreating(builder);
        }
    }
}