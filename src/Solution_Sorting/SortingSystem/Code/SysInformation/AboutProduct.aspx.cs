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
using System.Data.SqlClient;

public partial class main_Default : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

        LabVersion.Font.Size = FontUnit.Smaller;
        labCompany.Font.Size = FontUnit.Smaller;
        labCopyrigth.Font.Size = FontUnit.Smaller;
        labCompanyTelephone.Font.Size = FontUnit.Smaller;
        labCompanyFax.Font.Size = FontUnit.Smaller;
        labCompanyAddress.Font.Size = FontUnit.Smaller;
        labCompanyEmail.Font.Size = FontUnit.Smaller;
        lbtnCompanyEmail.Font.Size = FontUnit.Smaller;
        labCompanyWeb.Font.Size = FontUnit.Smaller;
        lbtnCompanyWeb.Font.Size = FontUnit.Smaller;
        lbtnQuit.Font.Size = FontUnit.Smaller;


           
        LabVersion.Text = "软件版本:V2.0";
        LabVersion.Font.Size = FontUnit.Smaller;

        labCompany.Text = "公司名称:天海欧康科技信息（厦门）有限公司";
        labCompany.Font.Size = FontUnit.Smaller;
        labCopyrigth.Text = ""; 
        labCopyrigth.Font.Size = FontUnit.Smaller;
        labCompanyTelephone.Text = "公司电话:(0592)2521388";
        labCompanyTelephone.Font.Size = FontUnit.Smaller;
        labCompanyFax.Text = "公司传真:(0592)2521299";
        labCompanyFax.Font.Size = FontUnit.Smaller;
        labCompanyAddress.Text = "公司地址:福建厦门思明区软件园创新大厦Ａ区７楼";
        labCompanyAddress.Font.Size = FontUnit.Smaller;
        labCompanyEmail.Text = "公司邮件:";
        labCompanyEmail.Font.Size = FontUnit.Smaller;
        lbtnCompanyEmail.Text = "service@skyseaok.com";
        lbtnCompanyEmail.Font.Size = FontUnit.Smaller;
        labCompanyWeb.Text = "公司网址:";
        labCompanyWeb.Font.Size = FontUnit.Smaller;
        lbtnCompanyWeb.Text = "http://www.skyseaok.com";
        lbtnCompanyWeb.Font.Size = FontUnit.Smaller;
        labCopyrigth.Text = "天海欧康，版权所有";
        labCopyrigth.Font.Size = FontUnit.Smaller;
            
    }



    protected void lbtnCompanyWeb_Click(object sender, EventArgs e)
    {
        string strScript = @"<SCRIPT LANGUAGE='javascript'> " + "window.open ('http://" + lbtnCompanyWeb.Text + "','_blank')" + "</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(GetType(), "a1", strScript);
    }
    protected void lbtnCompanyEmail_Click(object sender, EventArgs e)
    {
        string strScript = @"<SCRIPT LANGUAGE='javascript'> " + "window.open ('mailto:" + lbtnCompanyEmail.Text + "','newwindow')" + "</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(GetType(), "a1", strScript);
    }
    protected void lbtnSoftWareName_Click(object sender, EventArgs e)
    {

    }


    protected void lbtnVersion_Click(object sender, EventArgs e)
    {

    }

    protected void lbtnQuit_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MainPage.aspx");
    }
}
