using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace IpBlocker.SqlLite.Core
{
    public class DbInitializer
    {
        public static void Initialize(DatabaseContext context)
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
