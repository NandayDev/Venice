using System;
using VeniceDomain.Enums;

namespace VeniceDomain.Models
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; }

        public string Category { get; set; }

        public string Text { get; set; }

        public LogImportance Importance { get; set; }
    }
}
