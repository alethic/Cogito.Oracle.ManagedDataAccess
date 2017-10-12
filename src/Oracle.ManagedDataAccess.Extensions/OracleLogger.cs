using System;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Used to output log messages.
    /// </summary>
    class OracleLogger
    {

        readonly Action<OracleLogEvent> log;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="log"></param>
        public OracleLogger(Action<OracleLogEvent> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Verbose(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Verbose, message));
        }

        public void Debug(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Debug, message));
        }

        public void Informational(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Informational, message));
        }

        public void Warning(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Warning, message));
        }

        public void Error(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Error, message));
        }

        public void Fatal(string message)
        {
            log(new OracleLogEvent(OracleLogLevel.Fatal, message));
        }

    }

}
