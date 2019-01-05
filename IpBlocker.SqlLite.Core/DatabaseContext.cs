using IpBlocker.SqlLite.Core.Objects;

using Microsoft.EntityFrameworkCore;
using System.IO;

namespace IpBlocker.SqlLite.Core
{
    /// <summary>
    /// dotnet ef --startup-project ../IpBlocker.Core.TempProgram/ migrations add ConfigEntriesUpdate
    /// </summary>
    public class DatabaseContext : DbContext
    {        
        private const string DBFileName = "IP-Blocker-Data.sqlite";


        public DatabaseContext()
        {
            
        }

        
        public DbSet<BlockedIpRecord> BlockedIpRecords { get; set; }
        public DbSet<ConfigEntry> ConfigEntries { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite($"Data Source={DBFileName}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockedIpRecord>().ToTable("BlockedIpRecords");
            modelBuilder.Entity<ConfigEntry>().ToTable("ConfigEntries");
        }

        public void DeleteExistingDB()
        {
            File.Delete(DBFileName);
        }
    }
}