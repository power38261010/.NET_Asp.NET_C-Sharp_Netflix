using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;

namespace NetflixClone.Data {
    public class ApplicationDbContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MovieSubscription> MovieSubscription { get; set; }
        public DbSet<Movie> Movies { get; internal set; }
        public DbSet<Pay> Payments { get; internal set; }
        public DbSet<PaySubscription> PaySubscriptions { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:netflix-clon.database.windows.net,1433;Initial Catalog=NetflixClon;Persist Security Info=False;User ID=arrua.ale;Password=Camilo.94;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<MovieSubscription>()
                .HasKey(ms => ms.Id);

            modelBuilder.Entity<MovieSubscription>()
                .HasOne(ms => ms.Movie)
                .WithMany(m => m.MovieSubscription)
                .HasForeignKey(ms => ms.MovieId);

            modelBuilder.Entity<MovieSubscription>()
                .HasOne(ms => ms.Subscription)
                .WithMany(s => s.MovieSubscription)
                .HasForeignKey(ms => ms.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pay>()
                .HasOne(p => p.Subscription)
                .WithMany()
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Subscription)
                .WithMany()
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
