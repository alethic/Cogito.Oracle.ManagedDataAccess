using System;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Describes an exception that happens during Oracle extension operations.
    /// </summary>
    public class OracleExtensionsException :
        Exception
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        public OracleExtensionsException(string message) :
            base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public OracleExtensionsException(string message, OracleErrorSeverity severity) :
            this(message)
        {
            Severity = severity;
        }

        /// <summary>
        /// Gets the severity of the exception.
        /// </summary>
        public OracleErrorSeverity Severity { get; set; } = OracleErrorSeverity.Transient;

    }

}
