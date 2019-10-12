namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// The <see cref="OracleAQMessageState"/> enumeration type identifies the state of the message at the time of dequeue.
    /// </summary>
    public enum OracleAQMessageState
    {

        /// <summary>
        /// Indicates that the message is ready to be processed.
        /// </summary>
        Ready = 0,

        /// <summary>
        /// Indicates that the message delay has not been reached.
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// Indicates that the message has been processed and retained.
        /// </summary>
        Processed = 2,

        /// <summary>
        /// Indicates that the message has been moved to the exception queue.
        /// </summary>
        Expired = 3,

        Deferred = 8,

        BufferedExpired = 10,

    }

}
