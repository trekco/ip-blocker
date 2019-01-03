using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CsvHelper;

using IpBlocker.Core.Extensions;
using IpBlocker.Core.Interfaces;
using IpBlocker.Core.Objects;

using IpBLocker.MailEnable.Objects;

using Microsoft.Win32;

namespace IpBLocker.MailEnable
{
    public class MailEnableActivityLogReader : ILogReader
    {
        private const string MailEnableActivityLogRegistryLocation = "SOFTWARE\\Wow6432Node\\Mail Enable\\Mail Enable\\Connectors\\SMTP";
        private const string ActivityEnabledKey = "Activity Log Enabled";
        private const string ActivityDirectoryKey = "Activity Log Directory";
        private const string ActivityLogFileNameFormat = "SMTP-Activity-{0:yyMMdd}.log";

        private string _logPath;

        public MailEnableActivityLogReader()
        {
            InitSettings();
        }
        
        public List<IpEntry> GetBadIps(DateTime fromDate)
        {
            if (string.IsNullOrEmpty(_logPath) || !Directory.Exists(_logPath))
            {
                return new List<IpEntry>();
            }

            var fileName = string.Format(ActivityLogFileNameFormat, fromDate);
            var filePath = Path.Combine(_logPath, fileName);


            if (!File.Exists(filePath))
            {
                return new List<IpEntry>();
            }

            var entries = GetInvalidPasswordEntries(fromDate, filePath);
           
            return entries.Select(e => new IpEntry()
            {
                IP = e.RemoteTcpIp,
                Message = e.CommandResponse,                
                Time = e.EntryDateTime,
                ValidationData = e.Data,
                Port = 25,
                Protocol = NetworkProtocol.TCP
            }).ToList();
        }

        public string GetName()
        {
            return "MailEnable";
        }
             
        private static List<ActivityEntry> GetInvalidPasswordEntries(DateTime fromDate, string filePath)
        {
            var entries = new List<ActivityEntry>();

            using (var reader = new LogReader<ActivityEntry>(filePath))
            {

                while (reader.HasRecords())
                {
                    var record = reader.Next();

                    if (record == null)
                    {
                        continue;
                    }

                    if (record.EntryDateTime > fromDate && record.CommandResponse.Contains("Invalid Username or Password"))
                    {
                        entries.Add(record);
                    }
                }
            }

            return entries;
        }

        private void InitSettings()
        {
            _logPath = GetLogPathFromRegistry();
        }

        private string GetLogPathFromRegistry()
        {
            string path = null;

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(MailEnableActivityLogRegistryLocation))
                {
                    path = GetDirectoryFromKey(path, key);
                }
            }
            catch (Exception ex)
            {
                //react appropriately
                path = null;
            }

            return path;
        }

        private static string GetDirectoryFromKey(string path, RegistryKey key)
        {
            if (key != null && key.GetValue(ActivityEnabledKey).To<bool>())
            {
                var directory = key.GetValue(ActivityDirectoryKey).To<string>();

                if (!string.IsNullOrEmpty(directory))
                {
                    path = directory;
                }
            }

            return path;
        }

    }
}