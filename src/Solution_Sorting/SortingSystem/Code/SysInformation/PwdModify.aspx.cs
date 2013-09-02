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
using System.Drawing;
using THOK.AS.Dal;

public partial class Code_SysInfomation_PwdModify_PwdModify :BasePage
{
    SysUserDal userDal = new SysUserDal();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            txtUserName.Text = Session["G_user"].ToString();
        }
        catch
        {
            txtUserName.Text = "";
        }
    }
    protected void lbtnSave_Click(object sender, EventArgs e)
    {

        DataTable userTable = userDal.GetUser(txtUserName.Text.Trim());

        if (userTable.Rows[0]["UserPassword"].ToString() == THOK.Util.Coding.Encoding(txtOldPwd.Text))
        {
            if (userDal.ChangePassword(txtUserName.Text.Trim(), THOK.Util.Coding.Encoding(txtAckPwd.Text)))
            {
                Response.Redirect("ModifyPwdSuccess.aspx");
            }
            else
                Response.Write("<script>alert(\"密码修改失败!\")</script>");
        }
        else
        {
            JScript.Instance.ShowMessage(this,"原密码错误!");
        }

        
    }
    public string getColorValue(string s)
    {
        if (Session["IsUseGlobalParameter"].ToString() == "1")
            return s;
        else return "";
    }
    protected void lbtnCancel_Click(object sender, EventArgs e)
    {
        Exit();
    }
}
