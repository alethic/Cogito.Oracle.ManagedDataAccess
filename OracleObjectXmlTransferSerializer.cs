using System;
using System.Linq;
using System.Xml.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Provides methods for converting Oracle UDT values to and from the internal UDT XML serialization format.
    /// </summary>
    public static class OracleObjectXmlTransferSerializer
    {

        /// <summary>
        /// Deserializes the given UDT result value in XML into a type described by the <see cref="OracleObjectType"/>.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static OracleObjectValue Deserialize(OracleObjectType metadata, XDocument xml)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (xml == null)
                return new OracleObjectValue(metadata, true);

            // map XML to UDT
            return new OracleObjectValue(
                metadata,
                metadata.Attributes
                    .Select(i => (i.Name, DeserializeAttribute(i, xml.Root.Element(i.Name)))));
        }

        /// <summary>
        /// Converts the contents of a merged XML element to the appropriate Oracle representation.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        static object DeserializeAttribute(OracleObjectTypeAttribute metadata, XElement xml)
        {
            if (xml == null)
                return null;

            if (metadata.DbType != null)
                return DeserializeOracleDbTypeFromXmlValue((OracleDbType)metadata.DbType, xml.Value);
            else
                throw new NotImplementedException("Cannot deserialize UDT attribute of non-OracleDbType.");
        }

        /// <summary>
        /// Returns the appropriate Oracle object for the given parameters and value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static object DeserializeOracleDbTypeFromXmlValue(OracleDbType type, string value)
        {
            // properly handle null values
            if (value == null)
                return null;

            // otherwise; handle specific types
            switch (type)
            {
                case OracleDbType.Int32:
                    return int.Parse(value);
                case OracleDbType.Varchar2:
                    return value;
                case OracleDbType.NVarchar2:
                    return value;
                case OracleDbType.Blob:
                    return ConvertHexToByteArray(value);
                default:
                    throw new NotImplementedException($"Unable to deserialize UDT return value for {type}.");
            }
        }

        /// <summary>
        /// Serializes the given object value into XML.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XDocument Serialize(OracleObjectValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.IsNull)
                return new XDocument(new XElement(value.Type.Name));

            return new XDocument(
                new XElement(value.Type.Name,
                    value.Type.Attributes
                        .Select(i => SerializeAttribute(i, value[i.Name]))));
        }

        /// <summary>
        /// Serializes the specified attribute and attribute value.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static XElement SerializeAttribute(OracleObjectTypeAttribute attribute, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (attribute.DbType != null)
                return SerializeOracleDbType(attribute.Name, (OracleDbType)attribute.DbType, value);
            else
                throw new NotImplementedException("Cannot serialize UDT attribute of non-OracleDbType.");
        }

        /// <summary>
        /// Serializes teh specified 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static XElement SerializeOracleDbType(string name, OracleDbType type, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            switch (type)
            {
                case OracleDbType.Int32:
                    return value != null ? new XElement(name, value) : null;
                case OracleDbType.Varchar2:
                    return value != null ? new XElement(name, value) : null;
                case OracleDbType.NVarchar2:
                    return value != null ? new XElement(name, value) : null;
                case OracleDbType.Blob:
                    return value != null ? new XElement(name, ConvertByteArrayToHex((byte[])value)) : null;
                default:
                    throw new NotImplementedException($"Unable to serialize value for {type}.");
            }
        }

        /// <summary>
        /// Converts a byte array to a HEX string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static string ConvertByteArrayToHex(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // this is inefficient; watch https://github.com/dotnet/corefx/issues/519
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts a HEX string to a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static byte[] ConvertHexToByteArray(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(value))
                return Array.Empty<byte>();

            int charIndex = value.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0; // does the string define leading HEX indicator '0x'?              
            int charLength = value.Length - charIndex;

            if (charLength % 2 != 0)
                throw new FormatException("Invalid HEX value. Odd number of characters.");

            // destination byte array
            var bytes = new byte[charLength / 2];

            // walk characters, convert to upper/lower values, and merge
            int d = 0;
            for (int s = charIndex; s < value.Length; s += 2)
            {
                var upper = ConvertHexToByte(value[s], s, 4);
                var lower = ConvertHexToByte(value[s + 1], s + 1);

                bytes[d++] = (byte)(upper | lower);
            }

            return bytes;
        }

        /// <summary>
        /// Converts a single component of a HEX character pair to it's byte value.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="index"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        static byte ConvertHexToByte(char character, int index, int shift = 0)
        {
            var value = (byte)character;
            if (((0x40 < value) && (0x47 > value)) || ((0x60 < value) && (0x67 > value)))
            {
                if (0x40 == (0x40 & value))
                {
                    if (0x20 == (0x20 & value))
                        value = (byte)(((value + 0xA) - 0x61) << shift);
                    else
                        value = (byte)(((value + 0xA) - 0x41) << shift);
                }
            }
            else if ((0x29 < value) && (0x40 > value))
                value = (byte)((value - 0x30) << shift);
            else
                throw new InvalidOperationException($"Character '{character}' at index '{index}' is not valid alphanumeric character.");

            return value;
        }


    }

}
