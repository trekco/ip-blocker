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
        private readonly FileStream _fileStream;

        public LogReader(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _streamReader = new StreamReader(_fileStream);
            _csvReader = new CsvReader(_streamReader);

            _csvReader.Configuration.Delimiter = "\t";
            _csvReader.Configuration.HasHeaderRecord = true;
            _csvReader.Configuration.RegisterClassMap<ActivityEntryMap>();
            _csvReader.Configuration.BadDataFound = x => {
               // Console.WriteLine($"Bad data: <{x.RawRecord}>");
            };
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
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
                //Console.WriteLine(e);
                return default(T);
            }
        }
    }
}