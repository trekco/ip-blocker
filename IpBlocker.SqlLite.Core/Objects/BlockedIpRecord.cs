using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IpBlocker.SqlLite.Core.Objects
{

  
    public class BlockedIpRecord
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public bool IsBlocked { get; set; }        
        public string IpLocation { get; set; }
        public DateTime DateBlocked { get; set; }
        public DateTime DateToUnblockIp { get; set; }
        public DateTime? DateUnblocked { get; set; }
        public string Source { get; set; }
        public string RuleName { get; set; }
    }
}
