using Microsoft.EntityFrameworkCore;

using Trekco.IpBlocker.SqlLite.Core.Objects;

namespace Trekco.IpBlocker.SqlLite.Core
{
    /// <summary>
    ///     dotnet ef --startup-project ../IpBlocker.Core.TempProgram/ migrations add ConfigEntriesUpdate
    /// </summary>
    public class IpBlockerDatabase : DbContext
    {
        public IpBlockerDatabase(DbContextOptions<IpBlockerDatabase> options)
            : base(options)
        {
        }

        public DbSet<BlockedIpRecord> BlockedIpRecords { get; set; }
        public DbSet<ConfigEntry> ConfigEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockedIpRecord>().ToTable("BlockedIpRecords");
            modelBuilder.Entity<ConfigEntry>().ToTable("ConfigEntries");
        }
    }
}