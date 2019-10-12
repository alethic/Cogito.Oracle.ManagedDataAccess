namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Specifies options to use when dequeuing.
    /// </summary>
    public class OracleAQDequeueOptions
    {

        /// <summary>
        /// Specifies the consumer name for which to dequeue the message.
        /// </summary>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Specifies the correlation identifier of the message to be dequeued.
        /// </summary>
        public string Correlation { get; set; }

        /// <summary>
        /// Specifies the expected delivery mode of the message being dequeued.
        /// </summary>
        public OracleAQMessageDeliveryMode DeliveryMode { get; set; } = OracleAQMessageDeliveryMode.Persistent;

        /// <summary>
        /// The dequeue mode attribute specifies the locking behavior associated with the dequeue.
        /// </summary>
        public OracleAQDequeueMode DequeueMode { get; set; } = OracleAQDequeueMode.Remove;

        /// <summary>
        /// Specifies the message identifier of the message to be dequeued.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Specifies the position of the message that will be retrieved
        /// </summary>
        public OracleAQNavigationMode NavigationMode { get; set; } = OracleAQNavigationMode.NextMessage;

        /// <summary>
        /// Specifies whether or not the new message is dequeued as part of the current transaction.
        /// </summary>
        public OracleAQVisibilityMode Visibility { get; set; } = OracleAQVisibilityMode.OnCommit;

        /// <summary>
        /// This instance property specifies the wait time, in seconds, for a message that matches the search criteria.
        /// </summary>
        public int Wait { get; set; }

    }

}
