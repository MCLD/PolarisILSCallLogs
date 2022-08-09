using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace PolarisCallLog
{
    class PolarisCallLogContext : DbContext
    {
        public DbSet<CallLogSummary> Summaries { get; set; }
    }
}
