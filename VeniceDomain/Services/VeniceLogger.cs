using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VeniceDomain.Enums;
using VeniceDomain.Models;

namespace VeniceDomain.Services
{
    /// <summary>
    /// An abstract class for logging across the Venice libraries
    /// </summary>
    public abstract class VeniceLogger
    {
        #region Static instance

        /// <summary>
        /// Static instance of the logger
        /// </summary>
        public static VeniceLogger Instance { private get; set; }

        /// <summary>
        /// Event raised when a log entry is sent to the <see cref="VeniceLogger"/>
        /// </summary>
        public static event EventHandler<LogEntry> OnLogEntryCreated;

        /// <summary>
        /// All log entries created for current running application
        /// </summary>
        public static List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        /// <summary>
        /// Saves given <paramref name="logEntry"/> inside the log
        /// </summary>
        public static void Log(LogEntry logEntry)
        {
            Instance?.WriteLog(logEntry);
            OnLogEntryCreated?.Invoke(null, logEntry);
            if (LogEntries.Count == 0)
                LogEntries.Add(logEntry);
            else
                LogEntries.Insert(0, logEntry);
        }

        /// <summary>
        /// Logs a <paramref name="message"/> with a <paramref name="category"/>, giving it a certain <paramref name="importance"/><br></br>
        /// The created <see cref="LogEntry"/> could be saved into the database, depending on the implementation of <see cref="ShouldPersist(LogImportance)"/>
        /// </summary>
        public static void Log(string message, string category, LogImportance importance = LogImportance.Debug)
        {
            LogEntry entry = new LogEntry 
            { 
                Category = category, 
                DateTime = DateTime.Now, 
                Text = message,
                Importance = importance
            };
            Log(entry);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes given <paramref name="logEntry"/> to the application console
        /// </summary>
        protected abstract void WriteLogToConsole(LogEntry logEntry);

        /// <summary>
        /// Saves given <paramref name="logEntry"/> into a database
        /// </summary>
        /// <param name="logEntry"></param>
        protected abstract void WriteLogToDatabase(LogEntry logEntry);

        /// <summary>
        /// Override this method to decide which <paramref name="importance"/> a <see cref="LogEntry"/> must have to be saved into the database too
        /// </summary>
        protected virtual bool ShouldPersist(LogImportance importance) => (int)importance >= (int)LogImportance.Info;

        private void WriteLog(LogEntry logEntry)
        {
            WriteLogToConsole(logEntry);
            if (ShouldPersist(logEntry.Importance))
                WriteLogToDatabase(logEntry);
        }

        #endregion
    }
}
