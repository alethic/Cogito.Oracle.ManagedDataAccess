using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Provides metadata information regarding Oracle types.
    /// </summary>
    public static class OracleObjectTypeProvider
    {

        /// <summary>
        /// Gets the metadata for the given type name.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<OracleObjectType> GetObjectMetadataAsync(OracleConnection connection, string owner, string name)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentNullException(nameof(owner));
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentNullException(nameof(name));

            return await GetObjectMetadataAsync(connection, owner + "." + name);
        }

        /// <summary>
        /// Gets the metadata for the given type name.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static async Task<OracleObjectType> GetObjectMetadataAsync(OracleConnection connection, string typeName)
        {
            return await DeserializeTypeMetadata(connection, await GetTypeMetadataXmlAsync(connection, typeName));
        }

        /// <summary>
        /// Gets the metadata XML for the given type name.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        static async Task<XDocument> GetTypeMetadataXmlAsync(OracleConnection connection, string typeName)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
SELECT      XMLELEMENT(TYPE,
                XMLELEMENT(TYPE_OWNER, ALL_TYPES.OWNER),
                XMLELEMENT(TYPE_NAME, ALL_TYPES.TYPE_NAME),
                XMLELEMENT(TYPE_CODE, ALL_TYPES.TYPECODE),
                XMLELEMENT(ATTRIBUTES, (
                    SELECT  XMLELEMENT(ATTRIBUTE,
                                XMLFOREST(
                                    ALL_TYPE_ATTRS.ATTR_NAME                    AS NAME,
                                    ALL_TYPE_ATTRS.ATTR_TYPE_OWNER              AS TYPE_OWNER,
                                    ALL_TYPE_ATTRS.ATTR_TYPE_NAME               AS TYPE_NAME,
                                    ALL_TYPE_ATTRS.ATTR_TYPE_MOD                AS TYPE_MOD,
                                    ALL_TYPE_ATTRS.LENGTH                       AS LENGTH,
                                    ALL_TYPE_ATTRS.PRECISION                    AS PRECISION,
                                    ALL_TYPE_ATTRS.SCALE                        AS SCALE,
                                    ALL_TYPE_ATTRS.CHARACTER_SET_NAME           AS CHARACTER_SET_NAME))
                    FROM    ALL_TYPE_ATTRS
                    WHERE   ALL_TYPE_ATTRS.OWNER = ALL_TYPES.OWNER
                        AND ALL_TYPE_ATTRS.TYPE_NAME = ALL_TYPES.TYPE_NAME)),
                XMLELEMENT(COLL_TYPE, ALL_COLL_TYPES.COLL_TYPE),
                XMLELEMENT(COLL_CAPACITY, ALL_COLL_TYPES.UPPER_BOUND),
                XMLELEMENT(ELEM_TYPE_OWNER, ALL_COLL_TYPES.ELEM_TYPE_OWNER),
                XMLELEMENT(ELEM_TYPE_NAME,  ALL_COLL_TYPES.ELEM_TYPE_NAME),
                XMLELEMENT(ELEM_TYPE_MOD,  ALL_COLL_TYPES.ELEM_TYPE_MOD),
                XMLELEMENT(ELEM_LENGTH, ALL_COLL_TYPES.LENGTH),
                XMLELEMENT(ELEM_PRECISION,  ALL_COLL_TYPES.PRECISION),
                XMLELEMENT(ELEM_SCALE, ALL_COLL_TYPES.SCALE),
                XMLELEMENT(ELEM_CHARACTER_SET_NAME,  ALL_COLL_TYPES.CHARACTER_SET_NAME))
FROM        ALL_TYPES
LEFT JOIN   ALL_COLL_TYPES
    ON      ALL_COLL_TYPES.OWNER = ALL_TYPES.OWNER
    AND     ALL_COLL_TYPES.TYPE_NAME = ALL_TYPES.TYPE_NAME
WHERE       ALL_TYPES.TYPE_NAME = :type_name
    OR      ALL_TYPES.OWNER || '.' || ALL_TYPES.TYPE_NAME = :type_name";

                // collect type metadata
                var typeNameParameter = cmd.CreateParameter();
                typeNameParameter.ParameterName = ":type_name";
                typeNameParameter.OracleDbType = OracleDbType.Varchar2;
                typeNameParameter.Direction = ParameterDirection.Input;
                typeNameParameter.Value = typeName;
                cmd.Parameters.Add(typeNameParameter);

                var typeXmlValue = (string)await cmd.ExecuteScalarAsync();
                if (typeXmlValue == null)
                    return null;

                // deserialize XML into metadata
                return XDocument.Parse(typeXmlValue);
            }
        }

        /// <summary>
        /// Deserializes type metadata into a <see cref="OracleObjectType"/>.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        static async Task<OracleObjectType> DeserializeTypeMetadata(OracleConnection connection, XDocument xml)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));

            // remove all empty nodes
            foreach (var x in xml.Descendants().ToList())
                if (!x.HasElements && string.IsNullOrWhiteSpace(x.Value))
                    x.Remove();

            var TYPE_CODE = (string)xml.Root.Element("TYPE_CODE");
            var TYPE_OWNER = (string)xml.Root.Element("TYPE_OWNER");
            var TYPE_NAME = (string)xml.Root.Element("TYPE_NAME");

            // plain object type
            if (TYPE_CODE == "OBJECT")
                return new OracleObjectType(
                    TYPE_OWNER,
                    TYPE_NAME,
                    xml.Root.Element("ATTRIBUTES").Elements("ATTRIBUTE")
                        .Select(i => new OracleObjectTypeAttribute(
                            (string)i.Element("NAME"),
                            (string)i.Element("TYPE_OWNER"),
                            (string)i.Element("TYPE_NAME"),
                            OracleDbTypeNameParser.ParseDbTypeName((string)i.Element("TYPE_NAME")),
                            (int?)i.Element("LENGTH"),
                            (int?)i.Element("PRECISION"),
                            (int?)i.Element("SCALE"))));

            // collection type
            if (TYPE_CODE == "COLLECTION")
            {
                var COLL_TYPE = (string)xml.Root.Element("COLL_TYPE");
                var ELEM_TYPE_OWNER = (string)xml.Root.Element("ELEM_TYPE_OWNER");
                var ELEM_TYPE_NAME = (string)xml.Root.Element("ELEM_TYPE_NAME");
                var ELEM_LENGTH = (int?)xml.Root.Element("ELEM_LENGTH");
                var ELEM_PRECISION = (int?)xml.Root.Element("ELEM_PRECISION");
                var ELEM_SCALE = (int?)xml.Root.Element("ELEM_SCALE");
                var ELEM_CHARACTER_SET_NAME = (string)xml.Root.Element("ELEM_CHARACTER_SET_NAME");

                if (COLL_TYPE == "VARYING ARRAY")
                {
                    var COLL_CAPACITY = (int)xml.Root.Element("COLL_CAPACITY");

                    // with DbType body
                    if (ELEM_TYPE_OWNER == null &&
                        OracleDbTypeNameParser.TryParseDbTypeName(ELEM_TYPE_NAME, out OracleDbType itemDbType))
                        return new OracleObjectType(
                            TYPE_OWNER,
                            TYPE_NAME,
                            OracleObjectDbType.Array,
                            new OracleDbTypeDef(itemDbType, ELEM_LENGTH, ELEM_PRECISION, ELEM_SCALE, ELEM_CHARACTER_SET_NAME),
                            null,
                            COLL_CAPACITY);

                    // with Object body
                    if (ELEM_TYPE_OWNER != null)
                    {
                        var itemObjectType = await GetObjectMetadataAsync(connection, ELEM_TYPE_OWNER, ELEM_TYPE_NAME);
                        if (itemObjectType == null)
                            throw new NullReferenceException($"Could not locate element type {ELEM_TYPE_OWNER}.{ELEM_TYPE_NAME}");

                        return new OracleObjectType(
                            TYPE_OWNER,
                            TYPE_NAME,
                            itemObjectType.ObjectDbType,
                            null,
                            itemObjectType,
                            COLL_CAPACITY);
                    }
                }

                if (COLL_TYPE == "TABLE")
                {
                    // with DbType body
                    if (ELEM_TYPE_OWNER == null &&
                        OracleDbTypeNameParser.TryParseDbTypeName(ELEM_TYPE_NAME, out OracleDbType itemDbType))
                        return new OracleObjectType(
                            TYPE_OWNER,
                            TYPE_NAME,
                            OracleObjectDbType.Table,
                            new OracleDbTypeDef(itemDbType, ELEM_LENGTH, ELEM_PRECISION, ELEM_SCALE, ELEM_CHARACTER_SET_NAME),
                            null);

                    // with Object body
                    if (ELEM_TYPE_OWNER != null)
                    {
                        var itemObjectType = await GetObjectMetadataAsync(connection, ELEM_TYPE_OWNER, ELEM_TYPE_NAME);
                        if (itemObjectType == null)
                            throw new NullReferenceException($"Could not locate element type {ELEM_TYPE_OWNER}.{ELEM_TYPE_NAME}");

                        return new OracleObjectType(
                            TYPE_OWNER,
                            TYPE_NAME,
                            itemObjectType.ObjectDbType,
                            null,
                            itemObjectType);
                    }
                }
            }

            throw new InvalidOperationException("Could not parse object type XML.");
        }

    }

}