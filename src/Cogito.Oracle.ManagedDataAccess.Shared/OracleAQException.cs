using System;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Describes an exception that happens during Oracle AQ operations.
    /// </summary>
    public class OracleAQException :
        OracleExtensionsException
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        public OracleAQException(string message) :
            base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public OracleAQException(string message, OracleErrorSeverity severity) :
            this(message)
        {
            Severity = severity;
        }

    }

}
