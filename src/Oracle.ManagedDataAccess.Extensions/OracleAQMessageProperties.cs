using System;
using System.Text;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Describes the properties of a message.
    /// </summary>
    public class OracleAQMessageProperties
    {

        /// <summary>
        /// The priority attribute specifies the priority of the message. It can be any number, including negative numbers. A smaller number indicates higher priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The delay attribute specifies the number of seconds during which a message is in the WAITING state.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// The expiration attribute specifies the number of seconds during which the message is available for dequeuing, starting from when the message reaches the READY state.
        /// </summary>
        public int Expiration { get; set; }

        /// <summary>
        /// The correlation attribute is an identifier supplied by the producer of the message at enqueue time.
        /// </summary>
        public string Correlation { get; set; }

        /// <summary>
        /// The attempts attribute specifies the number of attempts that have been made to dequeue the message.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// The recipient list parameter is valid only for queues that allow multiple consumers.
        /// </summary>
        public OracleAQAgent[] RecipientList { get; set; }

        /// <summary>
        /// The exception queue attribute specifies the name of the queue into which the message is moved if it cannot be processed successfully.
        /// </summary>
        public string ExceptionQueue { get; set; }

        /// <summary>
        /// The value specified in enqueue options is used to set the delivery mode of the message.
        /// </summary>
        public OracleAQMessageDeliveryMode DeliveryMode { get; set; } = OracleAQMessageDeliveryMode.Persistent;

        /// <summary>
        /// The enqueue time attribute specifies the time the message was enqueued.
        /// </summary>
        public DateTime EnqueueTime { get; set; }

        /// <summary>
        /// The state attribute specifies the state of the message at the time of the dequeue.
        /// </summary>
        public OracleAQMessageState State { get; set; }

        /// <summary>
        /// The sender ID attribute is an identifier of type aq$_agent specified at enqueue time by the message producer.
        /// </summary>
        public OracleAQAgent SenderId { get; set; }

        /// <summary>
        /// The original message ID attribute is used by Oracle Streams AQ for propagating messages.
        /// </summary>
        public byte[] OriginalMessageId { get; set; }

        /// <summary>
        /// The transaction group attribute specifies the transaction group for the message.
        /// </summary>
        public string TransactionGroup { get; set; }

        /// <summary>
        /// The user property attribute is optional. It is used to store additional information about the payload.
        /// </summary>
        public string UserProperty { get; set; }

        /// <summary>
        /// Throws an exception if the message is not valid.
        /// </summary>
        /// <returns></returns>
        internal bool ThrowIfInvalid()
        {
            if (Correlation != null)
            {
                if (Correlation.Length > 128)
                    throw new OracleAQException($"Message correlation value cannot exceed 128 characters.", OracleAQErrorSeverity.Permanent);
                if (Encoding.UTF8.GetByteCount(Correlation) != Correlation.Length)
                    throw new OracleAQException($"Message correlation value must be ASCII.", OracleAQErrorSeverity.Permanent);
            }

            if (OriginalMessageId != null)
            {
                if (OriginalMessageId.Length > 16)
                    throw new OracleAQException("Original message ID length cannot exceed 16.", OracleAQErrorSeverity.Permanent);
            }

            return true;
        }

    }

}
