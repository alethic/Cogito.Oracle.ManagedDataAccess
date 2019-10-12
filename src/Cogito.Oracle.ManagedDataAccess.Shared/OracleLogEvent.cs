using System;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Represents an internal event from the Oracle Managed Extensions.
    /// </summary>
    public class OracleLogEvent
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public OracleLogEvent(OracleLogLevel level, string message)
        {
            Level = level;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Level of the log event.
        /// </summary>
        public OracleLogLevel Level { get; set; }

        /// <summary>
        /// Message raised as part of the log event.
        /// </summary>
        public string Message { get; set; }

    }

}
