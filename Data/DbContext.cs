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
                .HasKey(ms => new { ms.MovieId, ms.SubscriptionId });

            modelBuilder.Entity<MovieSubscription>()
                .HasOne(ms => ms.Movie)
                .WithMany(m => m.MovieSubscription)
                .HasForeignKey(ms => ms.MovieId);

            modelBuilder.Entity<MovieSubscription>()
                .HasOne(ms => ms.Subscription)
                .WithMany(s => s.MovieSubscription)
                .HasForeignKey(ms => ms.SubscriptionId);
        }
    }
}