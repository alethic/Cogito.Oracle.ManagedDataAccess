using System.Runtime.Serialization;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Describes the type of an Oracle queue tables.
    /// </summary>
    public enum OracleAQMessageType
    {

        /// <summary>
        /// Indicates the Raw message type. The data type of the payload must be either <see cref="OracleBinary"/> or <see cref="byte[]"/> to enqueue the message.
        /// </summary>
        [EnumMember(Value = "RAW")]
        Raw = 1,

        /// <summary>
        /// Indicates the Oracle UDT message type.
        /// </summary>
        [EnumMember(Value = "UDT")]
        UDT = 2,

        /// <summary>
        /// Indicates the XML message type. The data type of the payload must be <see cref="OracleXmlType"/>, <see cref="XmlReader"/>, or <see cref="string"/> in order to enqueue the message.
        /// </summary>
        [EnumMember(Value = "XML")]
        Xml = 3,

    }

}
