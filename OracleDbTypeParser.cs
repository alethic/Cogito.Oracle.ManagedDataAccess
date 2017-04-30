using System;
using System.Collections.Generic;
using System.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Provides methods for parsing <see cref="OracleDbType"/> values.
    /// </summary>
    public static class OracleDbTypeParser
    {

        static readonly (string Name, OracleDbType DbType)[] dbTypeMapping = new[]
        {
            ("BLOB", OracleDbType.Blob)
        };

        static readonly IDictionary<string, OracleDbType> toDbType = dbTypeMapping.ToDictionary(i => i.Name, i => i.DbType);
        static readonly IDictionary<OracleDbType, string> fromDbType = dbTypeMapping.ToDictionary(i => i.DbType, i => i.Name);

        /// <summary>
        /// Converts a type name string into a <see cref="OracleDbType"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static OracleDbType ParseDbTypeName(string name)
        {
            if (!TryParseDbTypeName(name, out OracleDbType dbType))
                throw new NotSupportedException($"Cannot convert DbType '{name}' to {nameof(OracleDbType)}");

            return dbType;
        }

        /// <summary>
        /// Converts a type name string into a <see cref="OracleDbType"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool TryParseDbTypeName(string name, out OracleDbType dbType)
        {
            return toDbType.TryGetValue(name, out dbType);
        }

    }

}
