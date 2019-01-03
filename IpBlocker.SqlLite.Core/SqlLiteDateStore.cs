using System;
using System.Linq;

using IpBlocker.Core;
using IpBlocker.Core.Interfaces;
using IpBlocker.Core.Objects;
using IpBlocker.SqlLite.Core.Objects;

namespace IpBlocker.SqlLite.Core
{
    public class SqlLiteDateStore : IDataStore, IDisposable
    {
        private readonly DatabaseContext _context;
        public SqlLiteDateStore()
        {
            Initialize();
            _context = new DatabaseContext();
        }

        public bool IsIPBlocked(string ipEntryIp, string source, int port, string protocol)
        {
            return _context.BlockedIpRecords
                       .Where(b => b.Ip.Equals(ipEntryIp) 
                                   && b.Source.Equals(source) 
                                   && b.Port.Equals(port) 
                                   && b.Protocol.Equals(protocol.ToLower()) 
                                   && b.IsBlocked)
                       .OrderBy(b => b.Id)
                       .LastOrDefault()
                       ?.IsBlocked ?? false;
        }

        public void Add(BlockedEntry blockEntry, IPBlockPolicy policy, string source)
        {
            var record = new BlockedIpRecord
            {
                Source = source,
                RuleName = blockEntry.RuleName,
                DateBlocked = DateTime.Now,
                DateToUnblockIp = policy.GetUnblockDate(),
                Ip = blockEntry.Ip,
                IpLocation = blockEntry.IpLocation,
                IsBlocked = blockEntry.IsBLocked,
                Port = blockEntry.Port,
                Protocol = blockEntry.Protocol.ToString().ToLower()
            };

            _context.BlockedIpRecords.Add(record);
            _context.SaveChanges();

        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public void Initialize()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    DbInitializer.Initialize(db);
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}