using Microsoft.EntityFrameworkCore;
using NetworkSharp.Data.Models;

namespace NetworkSharp.Data
{
    /// <summary>
    /// Database context for NetworkSharp application
    /// </summary>
    public class NetworkSharpDbContext : DbContext
    {
        public DbSet<NetworkStatistics> NetworkStatistics { get; set; }
        public DbSet<PingHistory> PingHistory { get; set; }
        public DbSet<LanDeviceDb> LanDevices { get; set; }
        public DbSet<DnsTestResult> DnsTestResults { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=networksharp.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure NetworkStatistics
            modelBuilder.Entity<NetworkStatistics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.DownloadSpeed).IsRequired();
                entity.Property(e => e.UploadSpeed).IsRequired();
                entity.HasIndex(e => e.Timestamp);
            });

            // Configure PingHistory
            modelBuilder.Entity<PingHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Target).IsRequired();
                entity.Property(e => e.ResponseTime).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Target);
            });

            // Configure LanDevices
            modelBuilder.Entity<LanDeviceDb>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IPAddress).IsRequired();
                entity.Property(e => e.MacAddress).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.LastSeen).IsRequired();
                entity.HasIndex(e => e.IPAddress).IsUnique();
                entity.HasIndex(e => e.MacAddress);
            });

            // Configure DnsTestResults
            modelBuilder.Entity<DnsTestResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Server).IsRequired();
                entity.Property(e => e.Query).IsRequired();
                entity.Property(e => e.ResponseTime).IsRequired();
                entity.Property(e => e.Result).IsRequired();
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Server);
            });

            // Configure UserSettings
            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired();
                entity.Property(e => e.Value).IsRequired();
                entity.HasIndex(e => e.Key).IsUnique();
            });
        }
    }
}
