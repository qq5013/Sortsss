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

public partial class ResetPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strUserName;
        if (Session["G_user"] != null)
        {
            strUserName = Session["G_user"].ToString();
        }
        else
        {
            strUserName = "";
        }

        HttpContext.Current.Cache.Remove(strUserName);
        Session.Abandon();
        GC.Collect();
        string strScript = "<script language='javascript'>window.opener=null; window.open('','_self','');window.close();</script>";
        Page.ClientScript.RegisterStartupScript(GetType(), "a", strScript);
    }
}
