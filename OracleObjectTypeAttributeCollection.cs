using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Cogito.Collections;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Represents a collection of UDT attribute metadata.
    /// </summary>
    public class OracleObjectTypeAttributeCollection :
        IEnumerable<OracleObjectTypeAttribute>
    {

        readonly IDictionary<string, OracleObjectTypeAttribute> attributes;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="attributes"></param>
        internal OracleObjectTypeAttributeCollection(IDictionary<string, OracleObjectTypeAttribute> attributes)
        {
            this.attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        }

        /// <summary>
        /// Gets the attribute with the specified name.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public OracleObjectTypeAttribute this[string attributeName]
        {
            get { return attributes.GetOrDefault(attributeName); }
        }

        #region IEnumerable

        public IEnumerator<OracleObjectTypeAttribute> GetEnumerator()
        {
            return attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns <c>true</c> if the two objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) || 
                obj is OracleObjectTypeAttributeCollection other &&
                Enumerable.SequenceEqual(other.attributes.OrderBy(i => i.Key), attributes.OrderBy(i => i.Key));
        }

        /// <summary>
        /// Gets the hashcode for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1723798591 ^ attributes.Aggregate(589590329, (hash, kvp) => hash ^ kvp.GetHashCode());
        }

    }

}
