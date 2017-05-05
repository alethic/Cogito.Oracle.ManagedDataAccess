using Microsoft.VisualStudio.TestTools.UnitTesting;

using Oracle.ManagedDataAccess.Client;

namespace Oracle.ManagedDataAccess.Extensions.Tests
{

    [TestClass]
    public class OracleObjectTypeTests
    {

        [TestMethod]
        public void Test_basic_type_def()
        {
            var t = new OracleObjectType("TEST", "TYPE", new OracleObjectTypeAttribute[]
            {
                new OracleObjectTypeAttribute("Attr1", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
                new OracleObjectTypeAttribute("Attr2", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null),
                new OracleObjectTypeAttribute("Attr3", null, "VARCHAR", OracleDbType.Varchar2, 20, null, null)
            });

            Assert.IsTrue(t.Attributes.Count == 3);
            Assert.IsTrue(t.Attributes[2].Name == "Attr3" && t.Attributes[2].TypeName == "VARCHAR");
        }

    }

}
