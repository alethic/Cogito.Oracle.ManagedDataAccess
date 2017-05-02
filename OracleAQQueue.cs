using System;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// An <see cref="OracleAQQueue"/> object represents a queue.
    /// </summary>
    /// <remarks>
    /// A queue is a repository of messages and may either be a user queue, or an exception queue. A user queue is for
    /// normal message processing. A message is moved from a user queue to an exception queue if it cannot be retrieved
    /// and processed for some reason.
    /// </remarks>
    public class OracleAQQueue :
        IDisposable
    {

        OracleObjectType payloadType;
        OracleObjectType payloadArrayType;

        /// <summary>
        /// This constructor takes a queue name to initialize a queue object.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        public OracleAQQueue(string name)
        {
            Name = name;
        }

        /// <summary>
        /// This constructor takes a queue name and connection to initialize a queue object. The connection does not need be open during the queue object construction.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        public OracleAQQueue(string name, OracleConnection connection) :
            this(name)
        {
            Connection = connection;
        }

        /// <summary>
        /// This constructor takes a queue name, connection, and message type enumeration to initialize a queue object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        /// <param name="messageType"></param>
        public OracleAQQueue(string name, OracleConnection connection, OracleAQMessageType messageType) :
            this(name, connection)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// This constructor takes a queue name, connection, message type enumeration, and UDT type name to initialize a queue object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        /// <param name="messageType"></param>
        /// <param name="payloadTypeName"></param>
        /// <param name="payloadArrayTypeName"></param>
        public OracleAQQueue(string name, OracleConnection connection, OracleAQMessageType messageType, string payloadTypeName, string payloadArrayTypeName) :
            this(name, connection, messageType)
        {
            PayloadTypeName = payloadTypeName;
            PayloadArrayTypeName = payloadArrayTypeName;
        }

        /// <summary>
        /// Name of the queue.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Specifies the <see cref="OracleConnection"/> object associated with the queue.
        /// </summary>
        public OracleConnection Connection { get; }

        /// <summary>
        /// Specifies the dequeueing options to use when dequeuing a message from the queue.
        /// </summary>
        public OracleAQDequeueOptions DequeueOptions { get; set; } = new OracleAQDequeueOptions();

        /// <summary>
        /// Specifies the enqueueing options used to enqueue a message to a queue.
        /// </summary>
        public OracleAQEnqueueOptions EnqueueOptions { get; set; } = new OracleAQEnqueueOptions();

        /// <summary>
        /// Specifies the type of queue table associated with this queue.
        /// </summary>
        public OracleAQMessageType MessageType { get; set; } = OracleAQMessageType.UDT;

        /// <summary>
        /// Specifies the type name on which the queue and the corresponding queue table is based if the <see cref="MessageType"/> is <see cref="OracleAQMessageType.UDT"/>.
        /// </summary>
        public string PayloadTypeName { get; set; }

        /// <summary>
        /// Specifies the type name to be used for array operations.
        /// </summary>
        public string PayloadArrayTypeName { get; set; }

        /// <summary>
        /// This instance method dequeues messages from a queue using the <see cref="DequeueOptions"/> for the instance.
        /// </summary>
        public OracleAQMessage Dequeue()
        {
            return DequeueAsync().Result;
        }

        /// <summary>
        /// This instance method dequeues messages from a queue using the <see cref="DequeueOptions"/> for the instance.
        /// </summary>
        public Task<OracleAQMessage> DequeueAsync()
        {
            return DequeueAsync(DequeueOptions);
        }

        /// <summary>
        /// This instance method dequeues messages from a queue using the supplied dequeue options.
        /// </summary>
        /// <param name="options"></param>
        public OracleAQMessage Dequeue(OracleAQDequeueOptions options)
        {
            return DequeueAsync(options).Result;
        }

        /// <summary>
        /// This instance method dequeues messages from a queue using the supplied dequeue options.
        /// </summary>
        /// <param name="options"></param>
        public Task<OracleAQMessage> DequeueAsync(OracleAQDequeueOptions options)
        {
            switch (MessageType)
            {
                case OracleAQMessageType.Raw:
                    return OracleAQQueueUtil.DequeueRawAsync(this, options);
                case OracleAQMessageType.UDT:
                    return OracleAQQueueUtil.DequeueUdtAsync(this, options);
                case OracleAQMessageType.Xml:
                    return OracleAQQueueUtil.DequeueXmlAsync(this, options);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// This instance method dequeues multiple messages from a queue using the <see cref=""/> of the instance.
        /// </summary>
        /// <param name="count"></param>
        public OracleAQMessage[] DequeueArray(int count)
        {
            return DequeueArray(count, DequeueOptions);
        }

        /// <summary>
        /// This instance method dequeues multiple messages from a queue using the <see cref=""/> of the instance.
        /// </summary>
        /// <param name="count"></param>
        public Task<OracleAQMessage[]> DequeueArrayAsync(int count)
        {
            return DequeueArrayAsync(count, DequeueOptions);
        }

        /// <summary>
        /// This instance method dequeues multiple messages from a queue using the supplied dequeue options.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        public OracleAQMessage[] DequeueArray(int count, OracleAQDequeueOptions options)
        {
            return OracleAQQueueUtil.DequeueUdtArrayAsync(this, options, count).Result;
        }

        /// <summary>
        /// This instance method dequeues multiple messages from a queue using the supplied dequeue options.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="options"></param>
        public Task<OracleAQMessage[]> DequeueArrayAsync(int count, OracleAQDequeueOptions options)
        {
            switch (MessageType)
            {
                case OracleAQMessageType.Raw:
                    return OracleAQQueueUtil.DequeueRawArrayAsync(this, options, count);
                case OracleAQMessageType.UDT:
                    return OracleAQQueueUtil.DequeueUdtArrayAsync(this, options, count);
                case OracleAQMessageType.Xml:
                    return OracleAQQueueUtil.DequeueXmlArrayAsync(this, options, count);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// This instance method enqueues messages to a queue using the EnqueueOptions of the instance.
        /// </summary>
        /// <param name="message"></param>
        public byte[] Enqueue(OracleAQMessage message)
        {
            return EnqueueAsync(message).Result;
        }

        /// <summary>
        /// This instance method enqueues messages to a queue using the EnqueueOptions of the instance.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<byte[]> EnqueueAsync(OracleAQMessage message)
        {
            return EnqueueAsync(message, EnqueueOptions);
        }

        /// <summary>
        /// This instance method enqueues messages to a queue using the supplied enqueue options.
        /// </summary>
        /// <param name="options"></param>
        public byte[] Enqueue(OracleAQMessage message, OracleAQEnqueueOptions options)
        {
            return EnqueueAsync(message, options).Result;
        }

        /// <summary>
        /// This instance method enqueues messages to a queue using the supplied enqueue options.
        /// </summary>
        /// <param name="options"></param>
        public Task<byte[]> EnqueueAsync(OracleAQMessage message, OracleAQEnqueueOptions options)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // validate message
            message.ThrowIfInvalid();

            switch (MessageType)
            {
                case OracleAQMessageType.Raw:
                    return OracleAQQueueUtil.EnqueueRawAsync(this, options, message);
                case OracleAQMessageType.UDT:
                    return OracleAQQueueUtil.EnqueueUdtAsync(this, options, message);
                case OracleAQMessageType.Xml:
                    return OracleAQQueueUtil.EnqueueXmlAsync(this, options, message);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// This instance method enqueues multiple messages to a queue using the EnqueueOptions of the instance.
        /// </summary>
        /// <param name="messages"></param>
        public byte[][] EnqueueArray(OracleAQMessage[] messages)
        {
            return EnqueueArray(messages, EnqueueOptions);
        }

        /// <summary>
        /// This instance method enqueues multiple messages to a queue using the supplied enqueue options.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="options"></param>
        public byte[][] EnqueueArray(OracleAQMessage[] messages, OracleAQEnqueueOptions options)
        {
            return EnqueueArrayAsync(messages, options).Result;
        }

        /// <summary>
        /// This instance method enqueues multiple messages to a queue using the supplied enqueue options.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="options"></param>
        public Task<byte[][]> EnqueueArrayAsync(OracleAQMessage[] messages, OracleAQEnqueueOptions options)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // validate messages
            foreach (var message in messages)
                message.ThrowIfInvalid();

            switch (MessageType)
            {
                case OracleAQMessageType.Raw:
                    return OracleAQQueueUtil.EnqueueRawArrayAsync(this, options, messages);
                case OracleAQMessageType.UDT:
                    return OracleAQQueueUtil.EnqueueUdtArrayAsync(this, options, messages);
                case OracleAQMessageType.Xml:
                    return OracleAQQueueUtil.EnqueueXmlArrayAsync(this, options, messages);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the UDT type associated with the queue.
        /// </summary>
        /// <returns></returns>
        public async Task<OracleObjectType> GetPayloadTypeAsync()
        {
            if (PayloadTypeName == null)
                throw new InvalidOperationException($"{nameof(PayloadTypeName)} is null during attempt to dequeue a UDT message.");

            // retrieve type
            if (payloadType == null)
                payloadType = await OracleObjectTypeProvider.GetObjectMetadataAsync(Connection, PayloadTypeName);

            // failure to get metadata
            if (payloadType == null)
                throw new OracleAQException($"Unable to retrieve metadata for UDT {PayloadTypeName}.");

            return payloadType;
        }

        /// <summary>
        /// Gets the UDT type associated with the queue.
        /// </summary>
        /// <returns></returns>
        public OracleObjectType GetPayloadType()
        {
            return GetPayloadTypeAsync().Result;
        }

        /// <summary>
        /// Gets the Oracle object type associated with the queue for batched operations.
        /// </summary>
        /// <returns></returns>
        public async Task<OracleObjectType> GetPayloadArrayTypeAsync()
        {
            if (PayloadArrayTypeName == null)
                throw new InvalidOperationException($"{nameof(PayloadArrayTypeName)} is null during attempt to dequeue multiple UDT messages.");

            // retrieve type
            if (payloadArrayType == null)
                payloadArrayType = await OracleObjectTypeProvider.GetObjectMetadataAsync(Connection, PayloadArrayTypeName);

            return payloadArrayType;
        }

        /// <summary>
        /// Gets the Oracle object type associated with the queue for batched operations.
        /// </summary>
        /// <returns></returns>
        public OracleObjectType GetPayloadArrayType()
        {
            return GetPayloadArrayTypeAsync().Result;
        }

        /// <summary>
        /// This method releases any resources or memory allocated by the object.
        /// </summary>
        public void Dispose()
        {

        }

    }

}
