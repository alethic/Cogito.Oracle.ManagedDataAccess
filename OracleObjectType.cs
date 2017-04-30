using System;
using System.Collections.Generic;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Describes an Oracle object type. Can represent either an OBJECT a VARRAY or a TABLE.
    /// </summary>
    public class OracleObjectType
    {

        readonly string owner;
        readonly string name;
        readonly OracleObjectDbType objectDbType = OracleObjectDbType.Object;

        // Object
        readonly OracleObjectTypeAttributeCollection attributes;
        
        // Collection
        readonly OracleObjectDbType itemObjectDbType = OracleObjectDbType.DbType;
        readonly OracleDbTypeDef itemDbTypeDef;
        readonly OracleObjectType itemObjectType;

        // Array
        readonly int? capacity = null;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        internal OracleObjectType(string owner, string name, IDictionary<string, OracleObjectTypeAttribute> attributes)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException(nameof(owner));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            this.owner = owner;
            this.name = name;
            this.objectDbType = OracleObjectDbType.Object;
            this.attributes = new OracleObjectTypeAttributeCollection(attributes ?? throw new ArgumentNullException(nameof(attributes)));
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="itemObjectDbType"></param>
        /// <param name="itemDbTypeDef"></param>
        /// <param name="itemObjectType"></param>
        internal OracleObjectType(string owner, string name, OracleObjectDbType itemObjectDbType, OracleDbTypeDef itemDbTypeDef, OracleObjectType itemObjectType)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException(nameof(owner));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (itemObjectDbType == OracleObjectDbType.DbType && itemDbTypeDef == null)
                throw new ArgumentNullException(nameof(itemDbTypeDef), "Table of DbType requires a ItemDbTypeDef.");
            if (itemObjectDbType == OracleObjectDbType.Object && itemObjectType?.objectDbType != OracleObjectDbType.Object)
                throw new ArgumentNullException(nameof(itemDbTypeDef), "Table of Object requires a ItemObjectDbType of type Object.");

            this.owner = owner;
            this.name = name;
            this.objectDbType = OracleObjectDbType.Table;
            this.itemObjectDbType = itemObjectDbType;
            this.itemDbTypeDef = itemDbTypeDef;
            this.itemObjectType = itemObjectType;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="itemObjectDbType"></param>
        /// <param name="itemDbTypeDef"></param>
        /// <param name="itemObjectType"></param>
        /// <param name="capacity"></param>
        internal OracleObjectType(string owner, string name, OracleObjectDbType itemObjectDbType, OracleDbTypeDef itemDbTypeDef, OracleObjectType itemObjectType, int capacity)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException(nameof(owner));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            this.owner = owner;
            this.name = name;
            this.objectDbType = OracleObjectDbType.Array;
            this.itemObjectDbType = itemObjectDbType;
            this.itemDbTypeDef = itemDbTypeDef;
            this.itemObjectType = itemObjectType;
            this.capacity = capacity;
        }

        /// <summary>
        /// Gets the owner of the type.
        /// </summary>
        public string Owner => owner;

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Type of the object.
        /// </summary>
        public OracleObjectDbType ObjectDbType => objectDbType;

        /// <summary>
        /// Gets the set of attributes that are available on the UDT.
        /// </summary>
        public OracleObjectTypeAttributeCollection Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Describes the type of objects to expect as items.
        /// </summary>
        public OracleObjectDbType ItemObjectDbType => itemObjectDbType;

        /// <summary>
        /// Oracle definition type of the items when the items are <see cref="OracleObjectDbType.DbType"/>.
        /// </summary>
        public OracleDbTypeDef ItemDbType => itemDbTypeDef;

        /// <summary>
        /// Type description of the items when the items are objects, arrays or tables.
        /// </summary>
        public OracleObjectType ItemObjectType => itemObjectType;

        /// <summary>
        /// Capacity of the type when it is an array or table.
        /// </summary>
        public int? Capacity => capacity;

        /// <summary>
        /// Creates a new value.
        /// </summary>
        /// <returns></returns>
        public OracleObjectValue CreateValue()
        {
            return new OracleObjectValue(this);
        }

        /// <summary>
        /// Creates a new null value.
        /// </summary>
        /// <returns></returns>
        public OracleObjectValue CreateNullValue()
        {
            return new OracleObjectValue(this, true);
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Owner}.{Name}";
        }

        /// <summary>
        /// Returns <c>true</c> if the two objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) || obj is OracleObjectType other && other.Owner == Owner && other.Name == Name && other.attributes.Equals(attributes);
        }

        /// <summary>
        /// Returns the hashcode of this metadata object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 252928569 ^ Owner.GetHashCode() ^ Name.GetHashCode() ^ attributes.GetHashCode();
        }

    }

}
