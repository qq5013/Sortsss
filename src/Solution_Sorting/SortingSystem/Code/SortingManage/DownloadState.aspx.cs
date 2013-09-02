using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Code_SortingManage_UploadState : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["Operator"] != null && Request["Operator"].ToString().Trim().Equals("State"))
        {
            Response.ContentType = "text/xml";
            Response.Write(THOK.AS.Schedule.ProcessState.GetMessage());
            Response.Flush();
            Response.Close();
            System.Diagnostics.Debug.WriteLine(THOK.AS.Schedule.ProcessState.GetMessage());
        }
    }
}
