using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Oracle object value.
    /// </summary>
    public class OracleObjectValue :
        DynamicObject
    {

        readonly OracleObjectType metadata;
        readonly Dictionary<string, object> attributes;
        bool isNull;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="isNull"></param>
        internal OracleObjectValue(OracleObjectType metadata, bool isNull = false)
        {
            this.metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.attributes = new Dictionary<string, object>();
            this.isNull = isNull;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="attributes"></param>
        internal OracleObjectValue(OracleObjectType metadata, IEnumerable<(string, object)> attributes) :
            this(metadata, false)
        {
            this.attributes = attributes?.ToDictionary(i => i.Item1, i => i.Item2) ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the type metadata.
        /// </summary>
        public OracleObjectType Type => metadata;

        /// <summary>
        /// Returns <c>true</c> if this instance represents a <c>null</c> value.
        /// </summary>
        public bool IsNull => isNull;

        /// <summary>
        /// Gets or sets the attribute value by name.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public object this[string attributeName]
        {
            get { return attributes[attributeName]; }
            set { attributes[attributeName] = value; isNull = false; }
        }

        #region DynamicObject

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (metadata.Attributes[binder.Name] != null)
                return attributes.TryGetValue(binder.Name, out result);
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (metadata.Attributes[binder.Name] != null)
            {
                this[binder.Name] = value;
                return true;
            }
            else
                return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return metadata.Attributes.Select(i => i.Name);
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return metadata.ToString();
        }

        /// <summary>
        /// Returns <c>true</c> if the two objects are equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // other value is null and so am I
            if (obj == null)
                return IsNull;

            // must at least be object type
            if (obj is OracleObjectValue other)
            {
                // types must match
                if (!other.Type.Equals(Type))
                    return false;

                // other value is marked as null, same type, and so am I
                if (other.IsNull)
                    return IsNull;

                // result to comparing attribute values
                return Enumerable.SequenceEqual(other.attributes.OrderBy(i => i.Key), attributes.OrderBy(i => i.Key));
            }

            return false;
        }

        /// <summary>
        /// Gets the hash of this UDT value include the values of all attributes.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 226692130 ^
                Type.GetHashCode() ^
                (IsNull ? 0 : attributes.OrderBy(i => i.Key).Aggregate(636517162, (hash, kvp) => hash ^ kvp.GetHashCode()));
        }

    }

}
