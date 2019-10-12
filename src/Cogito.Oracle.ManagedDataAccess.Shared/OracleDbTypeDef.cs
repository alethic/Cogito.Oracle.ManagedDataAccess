using Oracle.ManagedDataAccess.Client;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Describes a specific definition of a <see cref="OracleDbType"/>.
    /// </summary>
    public class OracleDbTypeDef
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="dbType"></param>
        public OracleDbTypeDef(OracleDbType dbType)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        public OracleDbTypeDef(OracleDbType dbType, int? length = null, int? precision = null, int? scale = null, string characterSet = null) :
            this(dbType)
        {
            Length = length;
            Precision = precision;
            Scale = scale;
            CharacterSet = characterSet;
        }

        /// <summary>
        /// Gets the Oracle type being described.
        /// </summary>
        public OracleDbType DbType { get; }

        /// <summary>
        /// Gets the length of the type.
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// Gets the precision of the type.
        /// </summary>
        public int? Precision { get; }

        /// <summary>
        /// Gets the scale of the type.
        /// </summary>
        public int? Scale { get; }

        /// <summary>
        /// Gets the character set of the type.
        /// </summary>
        public string CharacterSet { get; }

    }

}
