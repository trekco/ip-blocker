using System;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace IpBLocker.MailEnable.Objects
{
    internal class ActivityEntry
    {
        public DateTime EntryDateTime { get; set; }
        public string TransactionType { get; set; }
        public string MessageId { get; set; }
        public string RemoteTcpIp { get; set; }
        public string RemoteTcpPort { get; set; }
        public string SmtpCommand { get; set; }
        public string CommandDetails { get; set; }
        public string CommandResponse { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }

        public string Data { get; set; }
    }

    internal sealed class ActivityEntryMap : ClassMap<ActivityEntry>
    {
        public ActivityEntryMap()
        {

            Map(m => m.EntryDateTime).Index(0).TypeConverter<ActivityDateTimeConverter>();
            Map(m => m.TransactionType).Index(1);
            Map(m => m.MessageId).Index(2);
            Map(m => m.RemoteTcpPort).Index(3);
            Map(m => m.RemoteTcpIp).Index(4);
            Map(m => m.SmtpCommand).Index(5);
            Map(m => m.CommandDetails).Index(6);
            Map(m => m.CommandResponse).Index(7);
            Map(m => m.BytesSent).Index(8);
            Map(m => m.BytesReceived).Index(9);
            Map(m => m.Data).Index(10).Optional();
            
        }
    }

    internal class ActivityDateTimeConverter : ITypeConverter
    {


        private const string DateFormat = "MM/dd/yy HH:mm:ss";
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime)value).ToString(DateFormat);
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return DateTime.ParseExact(text, DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
