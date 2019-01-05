using IpBlocker.Core;
using IpBlocker.Core.Interfaces;
using IpBlocker.Core.Objects;
using IpBlocker.SqlLite.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public string GetConfigValue(string id)
        {
            return _context.ConfigEntries.FirstOrDefault(ce => ce.Id.Equals(id.ToLower()))?.Value;
        }

        public void SaveConfigValue(string id, string value)
        {
            var data = _context.ConfigEntries.FirstOrDefault(ce => ce.Id.Equals(id.ToLower()));

            if (data == null)
            {
                data = new ConfigEntry
                {
                    Id = id.ToLower(),
                    Value = value
                };

                _context.ConfigEntries.Add(data);
            }
            else
            {
                data.Value = value;                
            }

            _context.SaveChanges();
        }

        public bool IsIPBlocked(string ipEntryIp, string source, int[] ports, string protocol)
        {
            return _context.BlockedIpRecords
                       .Where(b => b.Ip.Equals(ipEntryIp)
                                   && b.Source.Equals(source)
                                   && b.Ports.Equals(String.Join(",", ports))
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
                Ports = String.Join(",", blockEntry.Ports),
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

        public List<BlockedEntry> GetIpsToUnblock(DateTime before)
        {
            var records = _context.BlockedIpRecords.Where(i => i.DateToUnblockIp < before && i.IsBlocked).ToList();
                                 
            return records.Select(i => new BlockedEntry()
            {
                IsBLocked = i.IsBlocked,
                Ip = i.Ip,
                IpLocation = i.IpLocation,
                Ports = i.Ports.Split(',').Select(p => int.Parse(p)).ToArray(),
                Protocol = (NetworkProtocol)Enum.Parse(typeof(NetworkProtocol), i.Protocol, true),
                RuleName = i.RuleName,
                Source = i.Source
            }).ToList();

        }

        public void UnblockBlockedIpRecord(BlockedEntry entry)
        {
            var record = _context.BlockedIpRecords.FirstOrDefault(b => b.Ip.Equals(entry.Ip)
                                   && b.Source.Equals(entry.Source)
                                   && b.Ports.Equals(String.Join(",", entry.Ports))
                                   && b.Protocol.Equals(entry.Protocol.ToString().ToLower())
                                   && b.IsBlocked);

            record.IsBlocked = false;
            record.RuleName = null;
            record.DateUnblocked = DateTime.Now;

            _context.SaveChanges();
        }
    }
}