using System.Runtime.Serialization;

namespace Cogito.Oracle.ManagedDataAccess
{

    public enum OracleAQNavigationMode
    {

        [EnumMember(Value = "FIRST_MESSAGE")]
        FirstMessage = 1,

        [EnumMember(Value = "FIRST_MESSAGE_MULTI_GROUP")]
        FirstMessageMultiGroup = 4,

        [EnumMember(Value = "NEXT_MESSAGE")]
        NextMessage = 3,

        [EnumMember(Value = "NEXT_MESSAGE_MULTI_GROUP")]
        NextMessageMultiGroup = 5,

        [EnumMember(Value = "NEXT_TRANSACTION")]
        NextTransaction = 2,

    }

}
