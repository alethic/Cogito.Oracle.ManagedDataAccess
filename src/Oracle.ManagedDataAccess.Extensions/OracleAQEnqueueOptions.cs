namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// The <see cref="OracleAQEnqueueOptions"/> class represents the options available when enqueuing a message to an <see cref="OracleAQQueue"/>.
    /// </summary>
    public class OracleAQEnqueueOptions
    {

        /// <summary>
        /// Specifies the delivery mode of the message being enqueued.
        /// </summary>
        public OracleAQMessageDeliveryMode DeliveryMode { get; set; } = OracleAQMessageDeliveryMode.Persistent;

        /// <summary>
        /// Specifies whether or not the new message is enqueued as part of the current transaction.
        /// </summary>
        public OracleAQVisibilityMode Visibility { get; set; } = OracleAQVisibilityMode.OnCommit;

    }

}
