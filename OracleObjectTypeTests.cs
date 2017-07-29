using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions.Tests
{

    [TestClass]
    public class OracleObjectTypeTests
    {

        static readonly OracleObjectType TestOracleObjectType = new OracleObjectType("TEST", "TYPE", new OracleObjectTypeAttribute[]
        {
            new OracleObjectTypeAttribute("Attr1", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr2", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr3", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr4", null, "BLOB", OracleDbType.Blob, null, null, null),
        });

        [TestMethod]
        public void Test_basic_type_def()
        {
            Assert.IsTrue(TestOracleObjectType.Attributes.Count == 4);
            Assert.IsTrue(TestOracleObjectType.Attributes[2].Name == "Attr3");
            Assert.IsTrue(TestOracleObjectType.Attributes[2].TypeName == "VARCHAR");
            Assert.IsTrue(TestOracleObjectType.Attributes[2].DbType == OracleDbType.Varchar2);
            Assert.IsTrue(TestOracleObjectType.Attributes[3].Name == "Attr4");
            Assert.IsTrue(TestOracleObjectType.Attributes[3].TypeName == "BLOB");
            Assert.IsTrue(TestOracleObjectType.Attributes[3].DbType == OracleDbType.Blob);
        }

        /// <summary>
        /// Tests whether we can serialize the given object type.
        /// </summary>
        [TestMethod]
        public void Test_serialization()
        {
            var o = TestOracleObjectType.CreateValue();
            o["Attr1"] = "value1";
            o["Attr2"] = "value2";
            o["Attr3"] = "value3";
            o["Attr4"] = Guid.NewGuid().ToByteArray();
            var x = OracleObjectXmlTransferSerializer.Serialize(o);
        }

    }

}
