using System;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions.Tests
{

    [TestClass]
    public class OracleObjectTypeTests
    {

        static readonly OracleObjectType TestOracleObjectType = new OracleObjectType("TEST", "TYPE", new OracleObjectTypeAttribute[]
        {
            new OracleObjectTypeAttribute("Int32", null, "NUMBER", OracleDbType.Int32, null, null, null),
            new OracleObjectTypeAttribute("Attr1", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr2", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr3", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
            new OracleObjectTypeAttribute("Attr4", null, "BLOB", OracleDbType.Blob, null, null, null),
        });

        [TestMethod]
        public void Test_basic_type_def()
        {
            Assert.IsTrue(TestOracleObjectType.Attributes.Count == 5);
            Assert.IsTrue(TestOracleObjectType.Attributes[0].Name == "Int32");
            Assert.IsTrue(TestOracleObjectType.Attributes[0].TypeName == "NUMBER");
            Assert.IsTrue(TestOracleObjectType.Attributes[0].DbType == OracleDbType.Int32);
            Assert.IsTrue(TestOracleObjectType.Attributes[3].Name == "Attr3");
            Assert.IsTrue(TestOracleObjectType.Attributes[3].TypeName == "VARCHAR");
            Assert.IsTrue(TestOracleObjectType.Attributes[3].DbType == OracleDbType.Varchar2);
            Assert.IsTrue(TestOracleObjectType.Attributes[4].Name == "Attr4");
            Assert.IsTrue(TestOracleObjectType.Attributes[4].TypeName == "BLOB");
            Assert.IsTrue(TestOracleObjectType.Attributes[4].DbType == OracleDbType.Blob);
        }

        /// <summary>
        /// Tests whether we can serialize the given object type.
        /// </summary>
        [TestMethod]
        public void Test_xml_serialization()
        {
            var o = TestOracleObjectType.CreateValue();
            o["Int32"] = 123123;
            o["Attr1"] = "value1";
            o["Attr2"] = "value2";
            o["Attr3"] = "value3";
            o["Attr4"] = new Guid("BD971DD2-7CD1-459E-B901-08705EC71A17").ToByteArray();
            var x = OracleObjectXmlTransferSerializer.Serialize(o);

            var t = XDocument.Parse(@"
                <TYPE>
                  <Int32>123123</Int32>
                  <Attr1>value1</Attr1>
                  <Attr2>value2</Attr2>
                  <Attr3>value3</Attr3>
                  <Attr4>D21D97BDD17C9E45B90108705EC71A17</Attr4>
                </TYPE>");

            Assert.IsTrue(XNode.DeepEquals(t, x));
        }

        [TestMethod]
        public void Test_xml_deserialization()
        {
            var t = XDocument.Parse(@"
                <TYPE>
                  <Int32>123123</Int32>
                  <Attr1>value1</Attr1>
                  <Attr2>value2</Attr2>
                  <Attr3>value3</Attr3>
                  <Attr4>D21D97BDD17C9E45B90108705EC71A17</Attr4>
                </TYPE>");
            var x = OracleObjectXmlTransferSerializer.Deserialize(TestOracleObjectType, t);

            Assert.IsTrue((int)x["Int32"] == 123123);
            Assert.IsTrue((string)x["Attr1"] == "value1");
            Assert.IsTrue((string)x["Attr2"] == "value2");
            Assert.IsTrue((string)x["Attr3"] == "value3");
            Assert.IsTrue(new Guid((byte[])x["Attr4"]) == new Guid("BD971DD2-7CD1-459E-B901-08705EC71A17"));
        }

    }

}
