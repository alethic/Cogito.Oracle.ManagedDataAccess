namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Describes the severity of an error.
    /// </summary>
    public enum OracleErrorSeverity
    {

        /// <summary>
        /// Error may clear up after some amount of time.
        /// </summary>
        Transient,

        /// <summary>
        /// Error is likely to be permanent.
        /// </summary>
        Permanent,

    }

}
