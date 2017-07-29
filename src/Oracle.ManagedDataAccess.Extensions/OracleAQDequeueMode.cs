using System.Runtime.Serialization;

namespace Oracle.ManagedDataAccess.Extensions
{

    public enum OracleAQDequeueMode
    {

        /// <summary>
        /// Reads the message without acquiring any lock on the message. This is equivalent to a SELECT statement.
        /// </summary>
        [EnumMember(Value = "BROWSE")]
        Browse = 1,

        /// <summary>
        /// Reads and obtains a write lock on the message. The lock lasts for the duration of the transaction. This is equivalent to a SELECT FOR UPDATE statement.
        /// </summary>
        [EnumMember(Value = "LOCKED")]
        Locked = 2,

        /// <summary>
        /// Reads the message and updates or deletes it. This is the default. The message can be retained in the queue table based on the retention properties.
        /// </summary>
        [EnumMember(Value = "REMOVE")]
        Remove = 3,

        /// <summary>
        /// Confirms receipt of the message but does not deliver the actual message content.
        /// </summary>
        [EnumMember(Value = "REMOVE_NO_DATA")]
        RemoveNoData = 4,

    }

}
