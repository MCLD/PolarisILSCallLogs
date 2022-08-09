using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PolarisCallLog
{
    public class CallLogSummary
    {
        [Key]
        public DateTime Date { get; set; }
        public string Filename { get; set; }
        public int CallCount { get; set; }
        public int StartedCalls { get; set; }
        public int OverdueCalls { get; set; }
        public int HoldCalls { get; set; }
        public DateTime FirstEntry { get; set; }
        public DateTime LastEntry { get; set; }
    }
}
