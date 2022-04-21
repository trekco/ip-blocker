using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using IpBLocker.MailEnable.Objects;

namespace IpBLocker.MailEnable
{
    internal class LogReader<T> : IDisposable
    {
        private readonly CsvReader _csvReader;
        private readonly FileStream _fileStream;
        private readonly StreamReader _streamReader;

        public LogReader(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _streamReader = new StreamReader(_fileStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                BadDataFound = BadDataFound
            };

            _csvReader = new CsvReader(_streamReader, config);
            _csvReader.Context.RegisterClassMap<ActivityEntryMap>();
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
            _streamReader?.Dispose();
            _csvReader?.Dispose();
        }

        private void BadDataFound(BadDataFoundArgs args)
        {
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
                return default;
            }
        }
    }
}