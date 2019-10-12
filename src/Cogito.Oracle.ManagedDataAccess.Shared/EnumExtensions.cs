using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Various extension methods for working with <see cref="Enum"/> instances.
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// Gets the value of the <see cref="EnumMemberAttribute"/> applied to the <see cref="Enum"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue(this Enum value)
        {
            var m = value.GetType().GetMember(value.ToString());
            if (m == null || m.Length <= 0)
                throw new NullReferenceException($"Unable to find {nameof(Enum)} member.");

            var a = m[0].GetCustomAttribute<EnumMemberAttribute>();
            if (a == null)
                throw new NullReferenceException($"Unable to find {nameof(EnumMemberAttribute)} on {m[0]}");

            return a.Value;
        }

    }

}
