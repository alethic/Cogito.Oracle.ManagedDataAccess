using System;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions
{

    /// <summary>
    /// Describes an attribute of a <see cref="OracleObjectType"/>.
    /// </summary>
    public class OracleObjectTypeAttribute
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeOwner"></param>
        /// <param name="typeName"></param>
        /// <param name="dbType"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        internal OracleObjectTypeAttribute(
            string name,
            string typeOwner,
            string typeName,
            OracleDbType dbType,
            int? length,
            int? precision,
            int? scale)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));

            Name = name;
            TypeOwner = typeOwner;
            TypeName = typeName;
            DbType = dbType;
            Length = length;
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeOwner"></param>
        /// <param name="typeName"></param>
        /// <param name="objectType"></param>
        internal OracleObjectTypeAttribute(
            string name,
            string typeOwner,
            string typeName,
            OracleObjectType objectType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            Name = name;
            TypeOwner = typeOwner;
            TypeName = typeName;
            ObjectType = ObjectType;
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the owner of the type.
        /// </summary>
        public string TypeOwner { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets the underlying DB type.
        /// </summary>
        public OracleDbType? DbType { get; internal set; }

        /// <summary>
        /// Gets the underlying complex object type.
        /// </summary>
        public OracleObjectType ObjectType { get; internal set; }

        /// <summary>
        /// Gets the length of the type if the type supports a length.
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// Gets the precision of the type if the type supports a precision.
        /// </summary>
        public int? Precision { get; }

        /// <summary>
        /// Gets the scale of the type if the type supports a scale.
        /// </summary>
        public int? Scale { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return  Name;
        }

        /// <summary>
        /// Returns <c>true</c> if the two objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) ||
                obj is OracleObjectTypeAttribute other &&
                other.Name == Name &&
                other.TypeName == TypeName &&
                other.TypeOwner == TypeOwner &&
                other.DbType == DbType &&
                other.Length == Length &&
                other.Precision == Precision &&
                other.Scale == Scale;
        }

        /// <summary>
        /// Gets the hashcode for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1379870155 ^
                Name.GetHashCode() ^
                TypeName.GetHashCode() ^
                TypeOwner.GetHashCode() ^
                DbType.GetHashCode() ^
                Length.GetHashCode() ^
                Precision.GetHashCode() ^
                Scale.GetHashCode();
        }

    }

}
