using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarisCallLog
{
    public class CallLogRecord
    {
        public DateTime FirstSeen { get; set; }
        public string PatronId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsHold { get; set; }
        public bool IsOverdue { get; set; }
        public string ConnectionTime { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
