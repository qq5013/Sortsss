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
using THOK.AS.Dal;

public partial class LoginPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["Logout"] != null)
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
            GC.Collect();
        }
    }

    #region 控件事件
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        
        if (txtUserName.Text.Trim() != "")
        {
            try
            {
                string key = txtUserName.Text.ToLower();
                string UserCache = Convert.ToString(Cache[key]);
                if (UserCache == null || UserCache == string.Empty || Cache[key].ToString() == Page.Request.UserHostAddress)
                {
                    DataTable dtUser = new SysUserDal().GetUser(txtUserName.Text.Trim());
                    if (dtUser.Rows.Count > 0)
                    {
                        if (dtUser.Rows[0]["UserPassword"].ToString().Trim() == THOK.Util.Coding.Encoding(txtPassWord.Text.Trim()))
                        {
                            Session["UserID"] = dtUser.Rows[0]["UserID"].ToString();
                            Session["GroupID"] = dtUser.Rows[0]["GroupID"].ToString();
                            Session["G_user"] = txtUserName.Text.ToLower();
                            Session["Client_IP"] = Page.Request.UserHostAddress;
                            Session["IsFirstLogin"] = "1";

                            //个人显示设置,WUQH添加
                            Session["sys_PageCount"] = dtUser.Rows[0]["sys_PageCount"].ToString();
                            Session["grid_ColumnTitleFont"] = dtUser.Rows[0]["grid_ColumnTitleFont"].ToString();
                            Session["grid_ContentFont"] = dtUser.Rows[0]["grid_ContentFont"].ToString();
                            Session["grid_ColumnTextAlign"] = dtUser.Rows[0]["grid_ColumnTextAlign"].ToString();
                            Session["grid_ContentTextAlign"] = dtUser.Rows[0]["grid_ContentTextAlign"].ToString();
                            Session["grid_NumberColumnAlign"] = dtUser.Rows[0]["grid_NumberColumnAlign"].ToString();
                            Session["grid_MoneyColumnAlign"] = dtUser.Rows[0]["grid_MoneyColumnAlign"].ToString();
                            Session["grid_SelectMode"] = dtUser.Rows[0]["grid_SelectMode"].ToString();
                            Session["grid_OddRowColor"] = dtUser.Rows[0]["grid_OddRowColor"].ToString();
                            Session["grid_EvenRowColor"] = dtUser.Rows[0]["grid_EvenRowColor"].ToString();
                            Session["grid_IsRefreshBeforeAdd"] = dtUser.Rows[0]["grid_IsRefreshBeforeAdd"].ToString();
                            Session["grid_IsRefreshBeforeUpdate"] = dtUser.Rows[0]["grid_IsRefreshBeforeUpdate"].ToString();
                            Session["grid_IsRefreshBeforeDelete"] = dtUser.Rows[0]["grid_IsRefreshBeforeDelete"].ToString();
                            Session["sys_PrintForm"] = dtUser.Rows[0]["sys_PrintForm"].ToString();
                            Session["pager_ShowPageIndex"] = dtUser.Rows[0]["pager_ShowPageIndex"].ToString();
                            Session.Timeout = int.Parse(dtUser.Rows[0]["SessionTimeOut"].ToString());

                            TimeSpan stLogin = new TimeSpan(0, 0, System.Web.HttpContext.Current.Session.Timeout, 0, 0);
                            HttpContext.Current.Cache.Insert(key, Page.Request.UserHostAddress, null,DateTime.MaxValue, stLogin, System.Web.Caching.CacheItemPriority.NotRemovable,null);
   
                            //setLog.InsertOperationLog(System.DateTime.Now, this.txtUserName.Text.Trim(), "登录页面", "登录(成功)");
                            string strScript = @"<SCRIPT LANGUAGE='javascript'> " + "window.opener=null;window.open ('MDIPage.aspx','newwindow','top=0,left=0,depended=no,toolbar=no,menubar=no,scrollbars=no,resizable=yes,location=no,status=yes');window.opener=null;window.close();" + "</SCRIPT>";
                            Page.ClientScript.RegisterStartupScript(GetType(), "a1", strScript);
                        }
                        else
                        {
                            //setLog.InsertOperationLog(System.DateTime.Now, this.txtUserName.Text.Trim(), "登录页面", "登录(用户密码有误)");
                            labMessage.Text = "对不起,您输入的密码有误!";
                        }
                    }
                    else
                    {
                        labMessage.Text = "对不起,您输入的用户名不存在!";
                    }
                }
                else
                {
                    labMessage.Text = "对不起,该帐号已经有人登录!请与管理员联系!";
                }

            }
            catch (Exception exp)
            {
                Session["ModuleName"] = "登录界面";
                Session["FunctionName"] = "btnLogin_Click";
                Session["ExceptionalType"] = exp.GetType().FullName;
                Session["ExceptionalDescription"] = exp.Message;
                Response.Redirect("Common/MistakesPage.aspx");
            }
        }
        else
        {
            labMessage.Text = "请输入用户名!";
        }
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        string strScript = @"<SCRIPT LANGUAGE='javascript'> " + "window.opener=null;window.close();" + "</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(GetType(), "a2", strScript);
    }
#endregion
}
