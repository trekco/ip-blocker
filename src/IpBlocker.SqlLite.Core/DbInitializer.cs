using Microsoft.EntityFrameworkCore;

namespace Trekco.IpBlocker.SqlLite.Core
{
    public class DbInitializer
    {
        public static void Initialize(IpBlockerDatabase context)
        {
            // context.Database.EnsureCreated();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }
    }
}