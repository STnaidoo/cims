using System;
using System.Web.UI;

using System.Data;
using CIMS_Datalayer;

public class ToolSet
{
    //this file has been created to handle the code in the dnx.vb file from the previous version
    // It containts multiple scripts that operate from the page

    //create popup with alert message
    public void alertErrorMsg(Page pg, string errorMsg)
    {
        pg.ClientScript.RegisterStartupScript(this.GetType(),
                            "ErrorAlert", "alert(' " + errorMsg + "');", true);
    }


    //returns the dataset from the uploaded client excel format file
    public DataSet getExcelDataSetFromFile(string filePath, string folderName )
    {
        ToolsSetInfo tools = new ToolsSetInfo();
        try
        {
            return tools.getDataSetFromExcel(filePath, folderName);
        }
        catch(Exception ex)
        {
            ErrorLogging logError = new ErrorLogging();
            logError.LogError("Tools.getExcelDataSetFromFile", ex.StackTrace);
        }
        return new DataSet();
    }

    public static string checkIfNullString(string check)
    {
        if (check != null)
        {
            return check;
        }
        return "";
    }
        
}