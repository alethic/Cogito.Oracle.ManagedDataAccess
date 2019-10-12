using System.Runtime.Serialization;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// 
    /// </summary>
    public enum OracleAQVisibilityMode
    {

        /// <summary>
        /// Indicates that the enqueue or dequeue operation is not part of the current transaction. The operation constitutes a transaction of its own.
        /// </summary>
        [EnumMember(Value = "IMMEDIATE")]
        Immediate = 1,

        /// <summary>
        /// Indicates that the enqueue or dequeue operation is part of the current transaction. This is the default case.
        /// </summary>
        [EnumMember(Value = "ON_COMMIT")]
        OnCommit = 2,

    }

}
