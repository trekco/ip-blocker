using System.Net;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using MaxMind.GeoIP2;

using Microsoft.Extensions.Configuration;

using Trekco.IpBlocker.Core.Interfaces;

namespace Trekco.IpBlocker.IpLocation.Maxmind
{
    public class MaxmindIpLocation : IIPLocator, IDisposable
    {
        private const string TarFileName = "GeoLite2-Country.tar.gz";
        private const string DbFileName = "GeoLite2-Country.mmdb";
        private readonly string _dataPath;
        private readonly object _dbLock = new();
        private readonly string _maxMindDbFilePath;

        private readonly string _maxMindDbUrl =
            "https://geolite.maxmind.com/download/geoip/database/GeoLite2-Country.tar.gz";

        private DatabaseReader _reader;
        private readonly string _tempPath;

        public MaxmindIpLocation(IConfiguration configuration)
        {
            _maxMindDbUrl = configuration.GetSection("MaxMind:CountryDatabaseDownloadUrl").Value;

            _tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

            _maxMindDbFilePath = Path.Combine(_dataPath, DbFileName);
        }

        public void Dispose()
        {
            CloseReader();
        }

        public string GetIpLocation(string Ip)
        {
            lock (_dbLock)
            {
                OpenReader();

                try
                {
                    var response = _reader.Country(Ip);
                    Console.WriteLine(response.Country);
                    Console.WriteLine(response.Continent);

                    return $"{response.Country.Name}, {response?.Continent?.Name ?? "Unknown"}";
                }
                catch (Exception)
                {
                    return "Unknown";
                }
            }
        }

        public void Initialize()
        {
            if (File.Exists(_maxMindDbFilePath))
            {
                return;
            }

            CreateDir(_tempPath, true);
            CreateDir(_dataPath, false);

            var downloadedFile = Path.Combine(_tempPath, TarFileName);

            using (var client = new WebClient())
            {
                client.DownloadFile(_maxMindDbUrl, Path.Combine(_tempPath, TarFileName));
            }

            ExtractFile(downloadedFile);
        }

        private void OpenReader()
        {
            if (_reader == null)
            {
                _reader = new DatabaseReader(_maxMindDbFilePath);
            }
        }

        private void CloseReader()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        private void ExtractFile(string zipFileName)
        {
            var tarFile = ExtractGZipFile(zipFileName);

            var dataFile = ExtractTarFile(tarFile);

            if (dataFile != null)
            {
                lock (_dbLock)
                {
                    CloseReader();
                    File.Copy(dataFile, _maxMindDbFilePath, true);
                }
            }
        }

        private string ExtractGZipFile(string zipFileName)
        {
            string tarName;

            using (var originalFileStream = new FileStream(zipFileName, FileMode.Open, FileAccess.Read))
            {
                using (var gzipStream = new GZipInputStream(originalFileStream))
                {
                    // Change this to your needs
                    tarName = Path.Combine(_tempPath, Path.GetFileNameWithoutExtension(TarFileName));

                    var dataBuffer = new byte[4096];

                    using (var tarFileStream = File.Create(tarName))
                    {
                        StreamUtils.Copy(gzipStream, tarFileStream, dataBuffer);
                    }
                }
            }

            return tarName;
        }

        public string ExtractTarFile(string tarFileName)
        {
            using (Stream inStream = File.OpenRead(tarFileName))
            {
                using (var tarArchive = TarArchive.CreateInputTarArchive(inStream))
                {
                    tarArchive.ExtractContents(_tempPath);
                }
            }

            foreach (var directory in Directory.GetDirectories(_tempPath))
            {
                var files = Directory.GetFiles(directory);

                if (files != null && files.Any(f => f.EndsWith(".mmdb")))
                {
                    return files.First(f => f.EndsWith(".mmdb"));
                }
            }

            return null;
        }

        private void CreateDir(string path, bool deleteExists)
        {
            if (deleteExists && Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}