using IpBlocker.SqlLite.Core.Objects;

using Microsoft.EntityFrameworkCore;
using System.IO;

namespace IpBlocker.SqlLite.Core
{
    /// <summary>
    /// dotnet ef --startup-project ../IpBlocker.Core.TempProgram/ migrations add RemovePoop
    /// </summary>
    public class DatabaseContext : DbContext
    {        
        private const string DBFileName = "test.sqlite";


        public DatabaseContext()
        {
            
        }

        
        public DbSet<BlockedIpRecord> BlockedIpRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite($"Data Source=test.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockedIpRecord>().ToTable("BlockedIpRecords");
        }

        public void DeleteExistingDB()
        {
            File.Delete(DBFileName);
        }
    }
}