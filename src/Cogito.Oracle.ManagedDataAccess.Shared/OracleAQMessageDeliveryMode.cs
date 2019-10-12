using System.Runtime.Serialization;

namespace Oracle.ManagedDataAccess.Extensions
{

    public enum OracleAQMessageDeliveryMode
    {

        [EnumMember(Value = "PERSISTENT")]
        Persistent = 1,

        [EnumMember(Value = "BUFFERED")]
        Buffered = 2,

        [EnumMember(Value = "PERSISTENT_OR_BUFFERED")]
        PersistentOrBuffered = 3

    }

}
