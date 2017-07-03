using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIMS_Datalayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMS_Datalayer.Tests
{
    [TestClass()]
    public class DAccessInfoTests
    {
        [TestMethod()]
        public void RunStringReturnStringValue1WhereTest()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string tableName = "instructions_view";
            string whereClause = "instruction_id";
            string selector = "branch_recall_allowed";
            string value = "23561";

            string expected = "0";

            //act
            string actual = daccess.RunStringReturnStringValue1Where(tableName, selector, whereClause, value);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void RunNonQuery1WhereTest()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string queryType = "Update";
            string tableName = "instructions";
            string[] columns = { "allocated_to", "allocated_date", "ftro_allocated_by", "ftro_allocated_date" };
            string[] values = { "1", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", "1", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" };
            string whereClause = "instruction_id";
            string whereValue = "23558";

            bool expected = true;

            //act
            bool actual = false;
            actual = daccess.RunNonQuery1Where(queryType, tableName, columns, values, whereClause, whereValue);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void HowManyRecordsExist3WheresTest()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string tableName = "user_team_leader";
            string whereClause1 = "system_user_id";
            string whereClause2 = "system_tl_1";
            string whereClause3 = "active";
            string whereValue1 = "1498";
            string whereValue2 = "1";
            string whereValue3 = "1";

            int expected = 1;

            //act
            int actual = 0;
            actual = daccess.HowManyRecordsExist3Wheres(tableName, whereClause1, whereValue1, whereClause2, whereValue2, whereClause3, whereValue3);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void RunStringReturnStringValueINTest()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string mainTableName = "system_users";
            string mainSelector = "system_user_email";
            string mainWhereClause = "system_user_id";
            string secondaryTableName = "instructions";
            string secondarySelector = "referred_to";
            string secondaryWhereClause = "instruction_id";
            string secondaryWhereValue = "23561";

            string expected = "murow@stanbic.com";

            //act
            string actual = "";
            actual = daccess.RunStringReturnStringValueIN(mainTableName, mainSelector, mainWhereClause, secondaryTableName, secondarySelector, secondaryWhereClause, secondaryWhereValue);

            //assert
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void RunNonQueryEqualsSelectTest()
        {
            DAccessInfo daccess = new DAccessInfo();

            //arrange
            string queryType = "Update";
            string tableName = "instructions";
            string[] columns = { "locked_by", "picked_by" };
            string[] values = { "0", "0" };
            string equalsSelectColumn = "locked_date";
            string equalsSelectTableName = "instructions";
            string equalsSelectSelector = "locked_date";
            string equalsSelectWhereClause = "instruction_id";
            string equalsSelectWhereValue = "23561";
            string whereClause = "instruction_id";
            string whereValue = "23560";

            bool expected = true;

            //act
            bool actual = false;
            actual = daccess.RunNonQueryEqualsSelect(queryType, tableName, columns, values, equalsSelectColumn, equalsSelectTableName, equalsSelectSelector, equalsSelectWhereClause, equalsSelectWhereValue, whereClause, whereValue);

            //assert
            Assert.AreEqual(actual, expected);
        }
    }
}