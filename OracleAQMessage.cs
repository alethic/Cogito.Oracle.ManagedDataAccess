using System;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// An <see cref="OracleAQMessage"/> object represents a message to be enqueued and dequeued.
    /// </summary>
    /// <remarks>
    /// An <see cref="OracleAQMessage"/> object consists of control information (metadata) and Payload (data). The
    /// control information is exposed by various properties on the <see cref="OracleAQMessage"/> object and is used
    /// by Oracle Streams Advanced Queuing to manage messages. The payload is the information stored in the queue.
    /// </remarks>
    public class OracleAQMessage
    {

        OracleAQMessageProperties properties;
        object payload;

        /// <summary>
        /// This constructor instantiates the <see cref="OracleAQMessage"/> class.
        /// </summary>
        public OracleAQMessage()
        {
            this.properties = new OracleAQMessageProperties();
        }

        /// <summary>
        /// This constructor instantiates the <see cref="OracleAQMessage"/> class using the <see cref="object"/> provided as the payload.
        /// </summary>
        /// <param name="payload">
        /// An Object specifying payload. It can be one of the following types:
        /// <list>
        /// <item><see cref="byte[]"/></item>
        /// <item><see cref="OracleBinary"/></item>
        /// <item><see cref="OracleXmlType"/></item>
        /// <item><see cref="String"/></item>
        /// <item><see cref="XmlReader"/></item>
        /// </list>
        /// </param>
        public OracleAQMessage(object payload) :
            this()
        {
            SetPayload(payload);
        }

        /// <summary>
        /// This constructor instantiates the <see cref="OracleAQMessage"/> class using the <see cref="object"/> provided as the payload.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="payload">
        /// An Object specifying payload. It can be one of the following types:
        /// <list>
        /// <item><see cref="byte[]"/></item>
        /// <item><see cref="OracleBinary"/></item>
        /// <item><see cref="OracleXmlType"/></item>
        /// <item><see cref="String"/></item>
        /// <item><see cref="XmlReader"/></item>
        /// </list>
        /// </param>
        /// <param name="properties"></param>
        internal OracleAQMessage(byte[] messageId, object payload, OracleAQMessageProperties properties) :
            this(payload)
        {
            MessageId = messageId;
            this.properties = properties;
        }

        /// <summary>
        /// Gets the message properties structure.
        /// </summary>
        internal OracleAQMessageProperties Properties => properties;

        /// <summary>
        /// This instance property specifies an identification for the message.
        /// </summary>
        public string Correlation
        {
            get => properties.Correlation;
            set => properties.Correlation = value;
        }

        /// <summary>
        /// This instance property specifies the duration, in seconds, after which an enqueued message is available for dequeuing.
        /// </summary>
        public int Delay
        {
            get => properties.Delay;
            set => properties.Delay = value;
        }

        /// <summary>
        /// This instance property specifies the delivery mode of the dequeued message.
        /// </summary>
        public OracleAQMessageDeliveryMode DeliveryMode { get => properties.DeliveryMode; }

        /// <summary>
        /// This instance property returns the number of attempts that have been made to dequeue the message.
        /// </summary>
        public int DequeueAttempts { get => properties.Attempts; }

        /// <summary>
        /// This instance property specifies the time when the message was enqueued.
        /// </summary>
        public DateTime EnqueueTime { get => properties.EnqueueTime; }

        /// <summary>
        /// This instance property specifies the name of the queue that the message should be moved to if it cannot be processed successfully.
        /// </summary>
        public string ExceptionQueue
        {
            get => properties.ExceptionQueue;
            set => properties.ExceptionQueue = value;
        }

        /// <summary>
        /// This instance property specifies the duration, in seconds, for which an enqueued message is available for dequeuing.
        /// </summary>
        public int Expiration
        {
            get => properties.Expiration;
            set => properties.Expiration = value;
        }

        /// <summary>
        /// This instance property returns the message identifier.
        /// </summary>
        public byte[] MessageId { get; }

        /// <summary>
        /// This instance property specifies the identifier of the message in the last queue that generated this message.
        /// </summary>
        public byte[] OriginalMessageId { get => properties.OriginalMessageId; }

        /// <summary>
        /// This instance property specifies the payload of the message.
        /// </summary>
        public object Payload
        {
            get => payload;
            set => SetPayload(value);
        }

        /// <summary>
        /// Sets the payload.
        /// </summary>
        /// <param name="value"></param>
        void SetPayload(object value)
        {
            payload = value;
        }

        /// <summary>
        /// This instance property specifies the priority of the message.
        /// </summary>
        public int Priority
        {
            get => properties.Priority;
            set => properties.Priority = value;
        }

        /// <summary>
        /// This instance property specifies the list of recipients that overrides the default queue subscribers.
        /// </summary>
        public OracleAQAgent[] Recipients
        {
            get => properties.RecipientList;
            set => properties.RecipientList = value;
        }

        /// <summary>
        /// This instance property identifies the original sender of the message.
        /// </summary>
        public OracleAQAgent SenderId
        {
            get => properties.SenderId;
            set => properties.SenderId = value;
        }

        /// <summary>
        /// This instance property specifies the state of the message at the time of dequeue.
        /// </summary>
        public OracleAQMessageState State { get => properties.State; }

        /// <summary>
        /// This instance property specifies the transaction group for the dequeued message.
        /// </summary>
        public string TransactionGroup { get => properties.TransactionGroup; }

        /// <summary>
        /// Throws an exception if the message is invalid.
        /// </summary>
        /// <returns></returns>
        internal bool ThrowIfInvalid()
        {
            return Properties.ThrowIfInvalid();
        }


    }

}
