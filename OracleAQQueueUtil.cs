using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Cogito;

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Provides the underlying implementation for interacting with Oracle queues.
    /// </summary>
    public static class OracleAQQueueUtil
    {

        static string FormatDequeueMode(OracleAQDequeueMode mode)
        {
            return "DBMS_AQ." + mode.GetEnumMemberValue();
        }

        static string FormatNavigationMode(OracleAQNavigationMode mode)
        {
            return "DBMS_AQ." + mode.GetEnumMemberValue();
        }

        static string FormatVisibility(OracleAQVisibilityMode mode)
        {
            return "DBMS_AQ." + mode.GetEnumMemberValue();
        }

        static string FormatDeliveryMode(OracleAQMessageDeliveryMode mode)
        {
            return "DBMS_AQ." + mode.GetEnumMemberValue();
        }

        static string FormatWait(int wait)
        {
            switch (wait)
            {
                case -1:
                    return "FOREVER";
                case 0:
                    return "NO_WAIT";
                default:
                    return wait.ToString();
            }
        }

        /// <summary>
        /// Dequeues the next RAW message from the specified queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage> DequeueRawAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dequeues the given number of RAW messages.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage[]> DequeueRawArrayAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options,
            int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dequeues the next UDT message from the specified queue.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage> DequeueUdtAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // fetch UDT type information
            var payloadType = await queue.GetPayloadTypeAsync();
            if (payloadType == null)
                throw new NullReferenceException("Unable to fetch message payload type.");

            using (var cmd = queue.Connection.CreateCommand())
            {
                cmd.BindByName = true;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
DECLARE
    dequeue_options                 DBMS_AQ.dequeue_options_t;

    message_id                      RAW(16);
    message_properties              DBMS_AQ.message_properties_t;
    message_payload                 {payloadType};

    no_messages                     exception;
    PRAGMA EXCEPTION_INIT           (no_messages, -25228);
BEGIN
    dequeue_options.consumer_name   := :dequeue_options__consumer_name;
    dequeue_options.dequeue_mode    := {FormatDequeueMode(options.DequeueMode)};
    dequeue_options.navigation      := {FormatNavigationMode(options.NavigationMode)};
    dequeue_options.visibility      := {FormatVisibility(options.Visibility)};
    dequeue_options.wait            := {FormatWait(options.Wait)};
    dequeue_options.correlation     := :dequeue_options__correlation;

    DBMS_AQ.DEQUEUE(
        queue_name                      => :queue_name,
        dequeue_options                 => dequeue_options,
        message_properties              => message_properties,
        payload                         => message_payload,
        msgid                           => message_id);

    :message_count := 1;

    :message_id    := message_id;

    SELECT  XMLELEMENT(MESSAGE,
                XMLELEMENT(PRIORITY, message_properties.priority),
                XMLELEMENT(DELAY, message_properties.delay),
                XMLELEMENT(EXPIRATION, message_properties.expiration),
                XMLELEMENT(CORRELATION, message_properties.correlation),
                XMLELEMENT(ATTEMPTS, message_properties.attempts),
                -- XMLELEMENT(RECIPIENT_LIST, message_properties.recipient_list),
                XMLELEMENT(EXCEPTION_QUEUE, message_properties.exception_queue),
                XMLELEMENT(ENQUEUE_TIME, message_properties.enqueue_time),
                XMLELEMENT(STATE, message_properties.state),
                XMLELEMENT(SENDER_ID, message_properties.sender_id),
                XMLELEMENT(ORIGINAL_MSGID, message_properties.original_msgid),
                XMLELEMENT(SIGNATURE, message_properties.signature),
                XMLELEMENT(TRANSACTION_GROUP, message_properties.transaction_group),
                XMLELEMENT(USER_PROPERTY, message_properties.user_property),
                XMLELEMENT(DELIVERY_MODE, message_properties.delivery_mode))
    INTO    :message_properties
    FROM    DUAL;

    IF message_payload IS NOT NULL THEN
        :message_payload := XMLTYPE(message_payload);
    ELSE
        :message_payload := NULL;
    END IF;

EXCEPTION
    WHEN no_messages
        THEN :message_count := 0;
END;";

                var dequeueOptionsConsumerNameParameter = cmd.CreateParameter();
                dequeueOptionsConsumerNameParameter.ParameterName = ":dequeue_options__consumer_name";
                dequeueOptionsConsumerNameParameter.OracleDbType = OracleDbType.Varchar2;
                dequeueOptionsConsumerNameParameter.Direction = ParameterDirection.Input;
                dequeueOptionsConsumerNameParameter.Value = options.ConsumerName;
                cmd.Parameters.Add(dequeueOptionsConsumerNameParameter);

                var dequeueOptionsCorrelationParameter = cmd.CreateParameter();
                dequeueOptionsCorrelationParameter.ParameterName = ":dequeue_options__correlation";
                dequeueOptionsCorrelationParameter.OracleDbType = OracleDbType.Varchar2;
                dequeueOptionsCorrelationParameter.Direction = ParameterDirection.Input;
                dequeueOptionsCorrelationParameter.Value = options.Correlation;
                cmd.Parameters.Add(dequeueOptionsCorrelationParameter);

                var queueNameParameter = cmd.CreateParameter();
                queueNameParameter.ParameterName = ":queue_name";
                queueNameParameter.OracleDbType = OracleDbType.Varchar2;
                queueNameParameter.Direction = ParameterDirection.Input;
                queueNameParameter.Value = queue.Name;
                cmd.Parameters.Add(queueNameParameter);

                var messageIdParameter = cmd.CreateParameter();
                messageIdParameter.ParameterName = ":message_id";
                messageIdParameter.Direction = ParameterDirection.Output;
                messageIdParameter.Size = 16;
                messageIdParameter.OracleDbType = OracleDbType.Raw;
                cmd.Parameters.Add(messageIdParameter);

                var messagePropertiesParameter = cmd.CreateParameter();
                messagePropertiesParameter.ParameterName = ":message_properties";
                messagePropertiesParameter.Direction = ParameterDirection.Output;
                messagePropertiesParameter.OracleDbType = OracleDbType.XmlType;
                cmd.Parameters.Add(messagePropertiesParameter);

                var messagePayloadParameter = cmd.CreateParameter();
                messagePayloadParameter.ParameterName = ":message_payload";
                messagePayloadParameter.Direction = ParameterDirection.Output;
                messagePayloadParameter.OracleDbType = OracleDbType.XmlType;
                cmd.Parameters.Add(messagePayloadParameter);

                var messageCountParameter = cmd.CreateParameter();
                messageCountParameter.ParameterName = ":message_count";
                messageCountParameter.Direction = ParameterDirection.Output;
                messageCountParameter.OracleDbType = OracleDbType.Decimal;
                cmd.Parameters.Add(messageCountParameter);

                await cmd.ExecuteNonQueryAsync();

                // parses the result into the set of messages
                return ((OracleDecimal)messageCountParameter.Value).ToInt32() == 1 ? ReadUdtMessage(
                    payloadType,
                    ((OracleBinary)messageIdParameter.Value).Value,
                    ((OracleXmlType)messagePropertiesParameter.Value).Value,
                    ((OracleXmlType)messagePayloadParameter.Value).Value) : null;
            }
        }

        /// <summary>
        /// Dequeues the given number of UDT messages.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage[]> DequeueUdtArrayAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options,
            int count)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // fetch UDT type information
            var payloadType = await queue.GetPayloadTypeAsync();
            if (payloadType == null)
                throw new NullReferenceException("Unable to fetch message payload type.");

            var payloadArrayType = await queue.GetPayloadArrayTypeAsync();
            if (payloadArrayType == null)
                throw new OracleAQException("Unable to find message payload array type. Required for batch sizes of more than one.");

            using (var cmd = queue.Connection.CreateCommand())
            {
                cmd.BindByName = true;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
DECLARE
    dequeue_options                 DBMS_AQ.DEQUEUE_OPTIONS_T;

    message_id_array                DBMS_AQ.MSGID_ARRAY_T;
    message_properties_array        DBMS_AQ.MESSAGE_PROPERTIES_ARRAY_T;
    message_payload_array           {payloadArrayType};
    message_count                   PLS_INTEGER;

    message_id                      RAW(16);
    message_properties              DBMS_AQ.MESSAGE_PROPERTIES_T;
    message_payload                 {payloadType};

    no_messages                     exception;
    PRAGMA EXCEPTION_INIT           (no_messages, -25228);
BEGIN
    message_id_array                := DBMS_AQ.MSGID_ARRAY_T();
    message_properties_array        := DBMS_AQ.MESSAGE_PROPERTIES_ARRAY_T();
    message_payload_array           := {payloadArrayType}();

    dequeue_options.consumer_name   := :dequeue_options__consumer_name;
    dequeue_options.dequeue_mode    := {FormatDequeueMode(options.DequeueMode)};
    dequeue_options.navigation      := {FormatNavigationMode(options.NavigationMode)};
    dequeue_options.visibility      := {FormatVisibility(options.Visibility)};
    dequeue_options.wait            := {FormatWait(options.Wait)};
    dequeue_options.correlation     := :dequeue_options__correlation;

    message_count                   := DBMS_AQ.DEQUEUE_ARRAY(
        queue_name                      => :queue_name,
        dequeue_options                 => dequeue_options,
        array_size                      => :count,
        message_properties_array        => message_properties_array,
        payload_array                   => message_payload_array,
        msgid_array                     => message_id_array);

    -- return number of retrieved messages
    :message_count := message_count;
    
    -- copy data to output structures
    FOR idx IN 1..message_count LOOP
        message_id          := message_id_array(idx);
        message_properties  := message_properties_array(idx);
        message_payload     := message_payload_array(idx);

        :message_id(idx)    := message_id;

        SELECT  XMLELEMENT(MESSAGE,
                XMLELEMENT(PRIORITY, message_properties.priority),
                XMLELEMENT(DELAY, message_properties.delay),
                XMLELEMENT(EXPIRATION, message_properties.expiration),
                XMLELEMENT(CORRELATION, message_properties.correlation),
                XMLELEMENT(ATTEMPTS, message_properties.attempts),
                -- XMLELEMENT(RECIPIENT_LIST, message_properties.recipient_list),
                XMLELEMENT(EXCEPTION_QUEUE, message_properties.exception_queue),
                XMLELEMENT(ENQUEUE_TIME, message_properties.enqueue_time),
                XMLELEMENT(STATE, message_properties.state),
                XMLELEMENT(SENDER_ID, message_properties.sender_id),
                XMLELEMENT(ORIGINAL_MSGID, message_properties.original_msgid),
                XMLELEMENT(SIGNATURE, message_properties.signature),
                XMLELEMENT(TRANSACTION_GROUP, message_properties.transaction_group),
                XMLELEMENT(USER_PROPERTY, message_properties.user_property),
                XMLELEMENT(DELIVERY_MODE, message_properties.delivery_mode)).getClobVal()
        INTO    :message_properties(idx)
        FROM DUAL;

        IF message_payload IS NOT NULL THEN
            :message_payload(idx) := XMLTYPE(message_payload).getClobVal();
        ELSE
            :message_payload(idx) := NULL;
        END IF;
    END LOOP;

EXCEPTION
    WHEN no_messages
        THEN :message_count := 0;
END;";

                var dequeueOptionsConsumerNameParameter = cmd.CreateParameter();
                dequeueOptionsConsumerNameParameter.ParameterName = ":dequeue_options__consumer_name";
                dequeueOptionsConsumerNameParameter.OracleDbType = OracleDbType.Varchar2;
                dequeueOptionsConsumerNameParameter.Direction = ParameterDirection.Input;
                dequeueOptionsConsumerNameParameter.Value = options.ConsumerName;
                cmd.Parameters.Add(dequeueOptionsConsumerNameParameter);

                var dequeueOptionsCorrelationParameter = cmd.CreateParameter();
                dequeueOptionsCorrelationParameter.ParameterName = ":dequeue_options__correlation";
                dequeueOptionsCorrelationParameter.OracleDbType = OracleDbType.Varchar2;
                dequeueOptionsCorrelationParameter.Direction = ParameterDirection.Input;
                dequeueOptionsCorrelationParameter.Value = options.Correlation;
                cmd.Parameters.Add(dequeueOptionsCorrelationParameter);

                var queueNameParameter = cmd.CreateParameter();
                queueNameParameter.ParameterName = ":queue_name";
                queueNameParameter.OracleDbType = OracleDbType.Varchar2;
                queueNameParameter.Direction = ParameterDirection.Input;
                queueNameParameter.Value = queue.Name;
                cmd.Parameters.Add(queueNameParameter);

                var countParameter = cmd.CreateParameter();
                countParameter.ParameterName = ":count";
                countParameter.OracleDbType = OracleDbType.Int32;
                countParameter.Direction = ParameterDirection.Input;
                countParameter.Value = count;
                cmd.Parameters.Add(countParameter);

                var messageIdParameter = cmd.CreateParameter();
                messageIdParameter.ParameterName = ":message_id";
                messageIdParameter.Direction = ParameterDirection.Output;
                messageIdParameter.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                messageIdParameter.Size = count;
                messageIdParameter.OracleDbType = OracleDbType.Raw;
                messageIdParameter.Value = new OracleBinary[count];
                messageIdParameter.ArrayBindSize = Enumerable.Range(0, count).Select(i => 16).ToArray();
                cmd.Parameters.Add(messageIdParameter);

                var messagePropertiesParameter = cmd.CreateParameter();
                messagePropertiesParameter.ParameterName = ":message_properties";
                messagePropertiesParameter.Direction = ParameterDirection.Output;
                messagePropertiesParameter.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                messagePropertiesParameter.Size = count;
                messagePropertiesParameter.OracleDbType = OracleDbType.Varchar2;
                messagePropertiesParameter.Value = new OracleString[count];
                messagePropertiesParameter.ArrayBindSize = Enumerable.Range(0, count).Select(i => 1024 * 64).ToArray();
                cmd.Parameters.Add(messagePropertiesParameter);

                var messagePayloadParameter = cmd.CreateParameter();
                messagePayloadParameter.ParameterName = ":message_payload";
                messagePayloadParameter.Direction = ParameterDirection.Output;
                messagePayloadParameter.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                messagePayloadParameter.Size = count;
                messagePayloadParameter.OracleDbType = OracleDbType.Varchar2;
                messagePayloadParameter.Value = new OracleString[count];
                messagePayloadParameter.ArrayBindSize = Enumerable.Range(0, count).Select(i => 1024 * 64).ToArray();
                cmd.Parameters.Add(messagePayloadParameter);

                var messageCountParameter = cmd.CreateParameter();
                messageCountParameter.ParameterName = ":message_count";
                messageCountParameter.Direction = ParameterDirection.Output;
                messageCountParameter.OracleDbType = OracleDbType.Decimal;
                cmd.Parameters.Add(messageCountParameter);

                await cmd.ExecuteNonQueryAsync();

                // parses the result set into the set of messages
                return ReadUdtMessages(
                        payloadType,
                        ((OracleDecimal)messageCountParameter.Value).ToInt32(),
                        ((OracleBinary[])messageIdParameter.Value)?.Select(i => i.Value)?.ToArray() ?? Array.Empty<byte[]>(),
                        ((OracleString[])messagePropertiesParameter.Value)?.Select(i => i.Value)?.ToArray() ?? Array.Empty<string>(),
                        ((OracleString[])messagePayloadParameter.Value)?.Select(i => !i.IsNull ? i.Value : null)?.ToArray() ?? Array.Empty<string>())
                    .ToArray();
            }
        }

        /// <summary>
        /// Dequeues the next XML message from the specified queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage> DequeueXmlAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dequeues the given number of XML messages.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task<OracleAQMessage[]> DequeueXmlArrayAsync(
            OracleAQQueue queue,
            OracleAQDequeueOptions options,
            int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the set of messages from the output arrays.
        /// </summary>
        /// <param name="payloadType"></param>
        /// <param name="count"></param>
        /// <param name="ids"></param>
        /// <param name="properties"></param>
        /// <param name="payloads"></param>
        /// <returns></returns>
        static IEnumerable<OracleAQMessage> ReadUdtMessages(OracleObjectType payloadType, int count, byte[][] ids, string[] properties, string[] payloads)
        {
            if (payloadType == null)
                throw new ArgumentNullException(nameof(payloadType));
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (payloads == null)
                throw new ArgumentNullException(nameof(payloads));

            for (int i = 0; i < count; i++)
                yield return ReadUdtMessage(payloadType, ids[i], properties[i], payloads[i]);
        }

        /// <summary>
        /// Reads the given results into the message.
        /// </summary>
        /// <param name="payloadType"></param>
        /// <param name="messageId"></param>
        /// <param name="properties"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        static OracleAQMessage ReadUdtMessage(OracleObjectType payloadType, byte[] messageId, string properties, string payload)
        {
            if (payloadType == null)
                throw new ArgumentNullException(nameof(payloadType));
            if (messageId == null)
                throw new ArgumentNullException(nameof(messageId));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return new OracleAQMessage(
                messageId,
                OracleObjectXmlTransferSerializer.Deserialize(payloadType, payload != null ? XDocument.Parse(payload) : null),
                DeserializeMessageProperties(XDocument.Parse(properties)));
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<byte[]> EnqueueRawAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage message)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Task<byte[][]> EnqueueRawArrayAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage[] messages)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<byte[]> EnqueueUdtAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage message)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // fetch UDT type information
            var payloadType = await queue.GetPayloadTypeAsync();
            if (payloadType == null)
                throw new NullReferenceException("Unable to fetch message payload type.");

            using (var cmd = queue.Connection.CreateCommand())
            {
                cmd.BindByName = true;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
DECLARE
    enqueue_options                     DBMS_AQ.enqueue_options_t;
    message_properties                  DBMS_AQ.message_properties_t;
    message_payload                     {payloadType};
    message_id                          RAW(16);

    message_properties_xml              XMLTYPE;
    message_payload_xml                 XMLTYPE;

    no_messages                         exception;
    PRAGMA EXCEPTION_INIT               (no_messages, -25228);
BEGIN
    enqueue_options.delivery_mode       := {FormatDeliveryMode(options.DeliveryMode)};
    enqueue_options.visibility          := {FormatVisibility(options.Visibility)};

    message_properties_xml              := XMLTYPE(:message_properties);
    message_properties.priority         := message_properties_xml.extract('/MESSAGE/PRIORITY/text()').getNumberVal();
 -- message_properties.delay            := message_properties_xml.extract('/MESSAGE/DELAY/text()').getNumberVal();
 -- message_properties.expiration       := message_properties_xml.extract('/MESSAGE/EXPIRATION/text()').getNumberVal();
    message_properties.correlation      := message_properties_xml.extract('/MESSAGE/CORRELATION/text()').getStringVal();
 -- message_properties.exception_queue  := message_properties_xml('/MESSAGE/EXCEPTION_QUEUE/text()').getStringVal();
 -- message_properties.sender_id        := message_properties_xml.extract('/MESSAGE/SENDER_ID/text()').getStringVal();
 -- message_properties.user_property    := message_properties_xml.extract('/MESSAGE/USER_PROPERTY/text()').getStringVal();

    message_payload_xml                 := XMLTYPE(:message_payload);
    message_payload_xml.toObject(message_payload);

    DBMS_AQ.ENQUEUE(
        queue_name                          => :queue_name,
        enqueue_options                     => enqueue_options,
        message_properties                  => message_properties,
        payload                             => message_payload,
        msgid                               => message_id);

    :message_id                         := message_id;

END;";

                var queueNameParameter = cmd.CreateParameter();
                queueNameParameter.ParameterName = ":queue_name";
                queueNameParameter.OracleDbType = OracleDbType.Varchar2;
                queueNameParameter.Direction = ParameterDirection.Input;
                queueNameParameter.Value = queue.Name;
                cmd.Parameters.Add(queueNameParameter);

                var messagePropertiesXmlParameter = cmd.CreateParameter();
                messagePropertiesXmlParameter.ParameterName = ":message_properties";
                messagePropertiesXmlParameter.OracleDbType = OracleDbType.Varchar2;
                messagePropertiesXmlParameter.Direction = ParameterDirection.Input;
                messagePropertiesXmlParameter.Value = SerializeMessageProperties(message.Properties).ToString();
                cmd.Parameters.Add(messagePropertiesXmlParameter);

                var messagePayloadXmlParameter = cmd.CreateParameter();
                messagePayloadXmlParameter.ParameterName = ":message_payload";
                messagePayloadXmlParameter.OracleDbType = OracleDbType.Varchar2;
                messagePayloadXmlParameter.Direction = ParameterDirection.Input;
                messagePayloadXmlParameter.Value = OracleObjectXmlTransferSerializer.Serialize((OracleObjectValue)message.Payload ?? payloadType.CreateNullValue()).ToString();
                cmd.Parameters.Add(messagePayloadXmlParameter);

                var messageIdParameter = cmd.CreateParameter();
                messageIdParameter.ParameterName = ":message_id";
                messageIdParameter.Direction = ParameterDirection.Output;
                messageIdParameter.OracleDbType = OracleDbType.Raw;
                messageIdParameter.Size = 16;
                cmd.Parameters.Add(messageIdParameter);

                await cmd.ExecuteNonQueryAsync();

                // return new message id
                return ((OracleBinary)messageIdParameter.Value).Value;
            }
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Task<byte[][]> EnqueueUdtArrayAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage[] messages)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<byte[]> EnqueueXmlAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage message)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queues the given message and returns the new message ID.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static Task<byte[][]> EnqueueXmlArrayAsync(OracleAQQueue queue, OracleAQEnqueueOptions options, OracleAQMessage[] messages)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the message properties XML into a message properties instance.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        static OracleAQMessageProperties DeserializeMessageProperties(XDocument xml)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));

            return new OracleAQMessageProperties()
            {
                Priority = (int?)xml.Root.Element("PRIORITY") ?? 1,
                Delay = (int?)xml.Root.Element("DELAY") ?? 0,
                Expiration = (int?)xml.Root.Element("EXPIRATION") ?? -1,
                Correlation = ((string)xml.Root.Element("CORRELATION"))?.TrimOrNull(),
                Attempts = (int?)xml.Root.Element("ATTEMPTS") ?? 0,
                ExceptionQueue = ((string)xml.Root.Element("EXCEPTION_QUEUE"))?.TrimOrNull(),
                EnqueueTime = (DateTime)xml.Root.Element("ENQUEUE_TIME"),
                State = (OracleAQMessageState?)(int?)xml.Root.Element("STATE") ?? OracleAQMessageState.Expired,
                DeliveryMode = (OracleAQMessageDeliveryMode?)(int?)xml.Root.Element("DELIVERY_MODE") ?? OracleAQMessageDeliveryMode.Persistent,
                OriginalMessageId = null,
            };
        }

        /// <summary>
        /// Serializes the message properties into XML.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        static XDocument SerializeMessageProperties(OracleAQMessageProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return new XDocument(
                new XElement("MESSAGE",
                    new XElement("PRIORITY", properties.Priority),
                    new XElement("DELAY", properties.Delay),
                    new XElement("EXPIRATION", properties.Expiration),
                    new XElement("CORRELATION", properties.Correlation),
                    new XElement("ATTEMPTS", properties.Attempts),
                    new XElement("EXCEPTION_QUEUE", properties.ExceptionQueue),
                    new XElement("ENQUEUE_TIME", properties.EnqueueTime),
                    new XElement("STATE", (int)properties.State),
                    new XElement("DELIVERY_MODE", (int)properties.DeliveryMode)));
        }

    }

}
