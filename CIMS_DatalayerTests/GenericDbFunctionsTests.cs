using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIMS_Datalayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CIMS_Datalayer.Tests
{
    [TestClass()]
    public class GenericDbFunctionsTests
    {
        [TestMethod()]
        public void GetDataSourceUserGridViewInfoTest()
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

        //[TestMethod()]
        //public void GetDataSourceUserGridViewInfoTest()
        //{

        //    GenericDbFunctions gb = new GenericDbFunctions();
        //    DataTable dt = new DataTable();
        //    //arrange
        //    string tableName = "user_team_leader_view";
        //    string whereClause = "system_user_id";
        //    string value = "1498";

        //    //act
        //    dt = gb.GetDataSourceUserGridViewInfo(tableName, whereClause, value);

        //    //assert
        //    Assert.AreEqual(null, dt);


        //    //Assert.Fail();
        //}

    }
}