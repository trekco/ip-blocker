using System;
using System.Linq;

using IpBlocker.SqlLite.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IpBlocker.Core.TempProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new DatabaseContext())
            {
                DbInitializer.Initialize(db);
                
                var list = db.BlockedIpRecords.ToList();

                Console.WriteLine("{0} records saved to database", list.Count);

                Console.WriteLine();
               
            }
        }
    }

}
