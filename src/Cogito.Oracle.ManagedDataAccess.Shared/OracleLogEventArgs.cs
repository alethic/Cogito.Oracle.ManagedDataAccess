using System;

namespace Cogito.Oracle.ManagedDataAccess
{

    public class OracleLogEventArgs :
        EventArgs
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="e"></param>
        public OracleLogEventArgs(OracleLogEvent e)
        {
            Event = e ?? throw new ArgumentNullException(nameof(e));
        }

        /// <summary>
        /// Logging event that occurred.
        /// </summary>
        public OracleLogEvent Event { get; set; }

    }

}