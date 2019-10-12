using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Cogito.Collections;

namespace Cogito.Oracle.ManagedDataAccess
{

    /// <summary>
    /// Represents a collection of UDT attribute metadata.
    /// </summary>
    public class OracleObjectTypeAttributeCollection :
        IEnumerable<OracleObjectTypeAttribute>
    {

        readonly OracleObjectTypeAttribute[] attributes;
        readonly Dictionary<string, OracleObjectTypeAttribute> attributesByName;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="attributes"></param>
        internal OracleObjectTypeAttributeCollection(IEnumerable<OracleObjectTypeAttribute> attributes)
        {
            this.attributes = attributes?.ToArray() ?? throw new ArgumentNullException(nameof(attributes));
            this.attributesByName = attributes?.ToDictionary(i => i.Name, i => i);
        }

        /// <summary>
        /// Gets the attribute at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OracleObjectTypeAttribute this[int index] => attributes[index];

        /// <summary>
        /// Gets the attribute with the specified name.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public OracleObjectTypeAttribute this[string attributeName] => attributesByName.GetOrDefault(attributeName);

        /// <summary>
        /// Gets the number of attributes in the collection.
        /// </summary>
        public int Count => attributes.Length;

        #region IEnumerable

        public IEnumerator<OracleObjectTypeAttribute> GetEnumerator()
        {
            foreach (var i in attributes)
                yield return i;
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
                Enumerable.SequenceEqual(other.attributesByName.OrderBy(i => i.Key), attributesByName.OrderBy(i => i.Key));
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
