using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIMS_Datalayer;
using System.Data;

namespace CIMS_Datalayer.Tests
{
<<<<<<< HEAD
    [TestClass]
    public class DaccessTests
    {
        [TestMethod]
        public void RunStringReturnStringValue()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string tableName = "instructions_view";
            string whereClause = "instruction_id";
            string selector = "branch_recall_allowed";
            string value = "23561";

            string expected = "0";

            //act
            string actual = daccess.RunStringReturnStringValue(tableName, whereClause, selector, value);
            
            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void HowManyRecordsExist()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string tableName = "user_team_leader";
            string column1 = "system_tl_1";
            string column2 = "system_user_id";
            string column3 = "active";
            string value1 = "1";
            string value2 = "1498";
            string value3 = "1";

            int expected = 1;

            //act
            int actual = daccess.HowManyRecordsExist(tableName, column1, value1, column2, value2, column3, value3);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void RunStringReturnStringValueIN()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string tableName1 = "system_users";
            string tableName2 = "instructions";
            string whereClause1 = "system_user_id";
            string whereClause2 = "instruction_id";
            string selector1 = "system_user_email";
            string selector2 = "referred_to";
            string value = "23561";

            string expected = "murow@stanbic.com";

            //act
            string actual = daccess.RunStringReturnStringValueIN(tableName1, whereClause1, selector1, tableName2, whereClause2, selector2, value);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void RunNonQuery()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string queryType = "Update";
            string tableName = "instructions";
            string column1 = "allocated_to";
            string value1 = "1";
            string column2 = "allocated_date";
            string value2 = "";
            string column3 = "ftro_allocated_by";
            string value3 = "1";
            string column4 = "ftro_allocated_date";
            string value4 = "";
            string whereClause = "instruction_id";
            string whereValue1 = "23558";
            string whereValue2 = null;
            string whereValue3 = null;

            bool expected = true;

            //act
            bool actual = false;
            actual = daccess.RunNonQuery(queryType, tableName, column1, value1, column2, value2, column3, value3, column4, value4, whereClause, whereValue1, whereValue2, whereValue3);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void GetDataSourceUserGridViewInfo()
        {
            GenericDbFunctions gb = new GenericDbFunctions();
            DataTable dt = new DataTable();
            //arrange
            string tableName = "user_team_leader_view";
            string whereClause = "system_user_id";
            string value = "1498";

            //act
            dt = gb.GetDataSourceUserGridViewInfo(tableName, whereClause, value);

            //assert
            Assert.AreEqual(null, dt);
        }
    }
=======
    
>>>>>>> 153cc615252b8508671c682cbe9b9d6144264cf7
}
