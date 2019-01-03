using System;
using System.IO;

using CsvHelper;

using IpBLocker.MailEnable.Objects;

namespace IpBLocker.MailEnable
{
    internal class LogReader<T> : IDisposable
    {
        private readonly CsvReader _csvReader;
        private readonly StreamReader _streamReader;

        public LogReader(string filePath)
        {
            _streamReader = new StreamReader(filePath);
            _csvReader = new CsvReader(_streamReader);

            _csvReader.Configuration.Delimiter = "\t";
            _csvReader.Configuration.HasHeaderRecord = true;
            _csvReader.Configuration.RegisterClassMap<ActivityEntryMap>();
            _csvReader.Configuration.BadDataFound = x => { Console.WriteLine($"Bad data: <{x.RawRecord}>"); };
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
            _csvReader?.Dispose();
        }

        public bool HasRecords()
        {
            return _csvReader.Read();
        }

        public T Next()
        {
            try
            {
                return _csvReader.GetRecord<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default(T);
            }
        }
    }
}