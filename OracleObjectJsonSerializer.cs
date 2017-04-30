using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Provides methods to serialize Oracle objects to and from JSON.
    /// </summary>
    public static class OracleObjectJsonSerializer
    {

        /// <summary>
        /// Converts the given <see cref="OracleObjectValue"/> to JSON.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JObject SerializeObject(OracleObjectValue value)
        {
            switch (value.Type.ObjectDbType)
            {
                case OracleObjectDbType.Object:
                    return SerializeObjectAsObject(value);
                case OracleObjectDbType.Array:
                    return SerializeObjectAsArray(value);
                case OracleObjectDbType.Table:
                    return SerializeObjectAsTable(value);
                default:
                    throw new InvalidOperationException($"Unxpected {nameof(OracleObjectDbType)}.");
            }
        }

        /// <summary>
        /// Deserializes the given <see cref="OracleObjectValue"/> of <see cref="OracleObjectDbType.Object"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static JObject SerializeObjectAsObject(OracleObjectValue value)
        {
            if (value == null ||
                value.IsNull)
                return null;

            return new JObject(
                new JProperty("$$oracle",
                    new JObject(
                        new JProperty("version", 1),
                        new JProperty("type-owner", value.Type.Owner),
                        new JProperty("type-name", value.Type.Name))),
                value.Type.Attributes
                    .Select(i => new JProperty(i.Name, SerializeObjectAttribute(i, value[i.Name]))));
        }

        /// <summary>
        /// 
        /// Deserializes the given <see cref="OracleObjectValue"/> of <see cref="OracleObjectDbType.Array"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static JObject SerializeObjectAsArray(OracleObjectValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the given <see cref="OracleObjectValue"/> of <see cref="OracleObjectDbType.Table"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static JObject SerializeObjectAsTable(OracleObjectValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the given UDT value to an appropriate JSON value-type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static JToken SerializeObjectAttribute(OracleObjectTypeAttribute attribute, object value)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (attribute.DbType != null)
                return SerializeDbType((OracleDbType)attribute.DbType, attribute.Length, attribute.Precision, attribute.Scale, value);
            else if (attribute.ObjectType != null && value is OracleObjectValue objectValue)
                return SerializeObject(objectValue);
            else
                throw new InvalidOperationException($"Unable to handle attribute type for attribute '{attribute.Name}'");
        }

        /// <summary>
        /// Serializes the given Oracle DB type value.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static JToken SerializeDbType(OracleDbType dbType, int? length, int? precision, int? scale, object value)
        {
            switch (dbType)
            {
                case OracleDbType.Blob:
                case OracleDbType.Byte:
                case OracleDbType.Char:
                case OracleDbType.Clob:
                case OracleDbType.Date:
                case OracleDbType.Decimal:
                case OracleDbType.Double:
                case OracleDbType.Int16:
                case OracleDbType.Int32:
                case OracleDbType.Int64:
                case OracleDbType.Long:
                case OracleDbType.NChar:
                case OracleDbType.NClob:
                case OracleDbType.NVarchar2:
                case OracleDbType.Single:
                case OracleDbType.TimeStamp:
                case OracleDbType.Varchar2:
                    return value != null ? JToken.FromObject(value) : JValue.CreateNull();
                case OracleDbType.XmlType when value is XNode x:
                    return x.ToString(SaveOptions.None);
                case OracleDbType.XmlType when value is XmlNode x:
                    return x.OuterXml;
                case OracleDbType.XmlType when value is string s:
                    return JValue.FromObject(s);
                case OracleDbType.XmlType when value is null:
                    return JValue.CreateNull();
                default:
                    throw new JsonSerializationException($"Unsupported OracleDbType '{dbType}'.");
            }
        }

        /// <summary>
        /// Deserializes the given JSON object into an object of the specified <see cref="OracleObjectType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static OracleObjectValue DeserializeObject(OracleObjectType type, JToken json)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // return null object type.
            if (json == null)
                return type.CreateNullValue();

            switch (type.ObjectDbType)
            {
                case OracleObjectDbType.Object:
                    return DeserializeObjectAsObject(type, json);
                case OracleObjectDbType.Array:
                    return DeserializeObjectAsArray(type, json);
                case OracleObjectDbType.Table:
                    return DeserializeObjectAsTable(type, json);
                default:
                    throw new InvalidOperationException($"Unexpected {nameof(OracleObjectDbType)} value.");
            }
        }

        /// <summary>
        /// Deserializes the given JSON token into a Oracle object value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        static OracleObjectValue DeserializeObjectAsObject(OracleObjectType type, JToken json)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (json == null)
                return type.CreateNullValue();

            var jobject = json as JObject;
            if (jobject == null)
                throw new JsonSerializationException("Cannot deserialize to Oracle object from non JSON object.");

            var obj = type.CreateValue();

            foreach (var attribute in type.Attributes)
                obj[attribute.Name] = DeserializeObjectAttribute(attribute, jobject.Property(attribute.Name)?.Value);

            return obj;
        }

        /// <summary>
        /// Deserializes the given attribute from the given value.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static object DeserializeObjectAttribute(OracleObjectTypeAttribute attribute, JToken value)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (attribute.DbType != null)
                return DeserializeDbType((OracleDbType)attribute.DbType, attribute.Length, attribute.Precision, attribute.Scale, value);
            else if (attribute.ObjectType != null)
                return DeserializeObject(attribute.ObjectType, value);
            else
                throw new InvalidOperationException($"Unable to handle attribute type for attribute '{attribute.Name}'");
        }

        /// <summary>
        /// Deserializes the given JSON token into a Oracle array value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        static OracleObjectValue DeserializeObjectAsArray(OracleObjectType type, JToken json)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the given JSON token into a Oracle table value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        static OracleObjectValue DeserializeObjectAsTable(OracleObjectType type, JToken json)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the given Oracle DB type value.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static object DeserializeDbType(OracleDbType dbType, int? length, int? precision, int? scale, JToken value)
        {
            switch (dbType)
            {
                case OracleDbType.Blob:
                    return (byte[])value;
                case OracleDbType.Byte:
                    return (byte?)value;
                case OracleDbType.Char:
                    return (char?)value;
                case OracleDbType.Clob:
                    return (string)value;
                case OracleDbType.Date:
                    return (DateTime?)value;
                case OracleDbType.Decimal:
                    return (decimal?)value;
                case OracleDbType.Double:
                    return (double?)value;
                case OracleDbType.Int16:
                    return (short?)value;
                case OracleDbType.Int32:
                    return (int?)value;
                case OracleDbType.Int64:
                    return (long?)value;
                case OracleDbType.Long:
                    return (long?)value;
                case OracleDbType.NChar:
                    return (string)value;
                case OracleDbType.NClob:
                    return (string)value;
                case OracleDbType.NVarchar2:
                    return (string)value;
                case OracleDbType.Single:
                    return (float?)value;
                case OracleDbType.TimeStamp:
                    return (TimeSpan?)value;
                case OracleDbType.Varchar2:
                    return (string)value;
                case OracleDbType.XmlType:
                    return value != null ? XDocument.Parse((string)value) : null;
                default:
                    throw new JsonSerializationException($"Unsupported OracleDbType '{dbType}'.");
            }
        }

    }

}
