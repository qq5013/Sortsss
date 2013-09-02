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
using System.Text;
using THOK.AS.Dal;

public partial class Code_SysInfomation_ConfigPlan_ConfigPlan : BasePage
{
    public string strImagePath;
    SysUserDal userDal = new SysUserDal();

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!this.Page.IsPostBack)
        {
          InitSmartTree();
          QuickDestopBind();
        }
    }

    public void InitSmartTree()
    {
        this.sTreeModule.Nodes.Clear();
        try
        {
            string strUserName = Session["G_User"].ToString();
            DataTable dtModules = userDal.GetUserOperateModule(strUserName).Tables[0] ;//dc.GetModules(strUserName);
            DataTable dtSubModules = userDal.GetUserOperateSubModule(strUserName).Tables[0];//dc.GetSubModules(strUserName);

            foreach (DataRow dr in dtModules.Rows)
            {
                TreeNode tnRoot = new TreeNode(dr["MenuTitle"].ToString(), dr["ID"].ToString());
                tnRoot.SelectAction = TreeNodeSelectAction.Expand;
                tnRoot.ShowCheckBox = true;
                this.sTreeModule.Nodes.Add(tnRoot);
            }

            if (dtModules.Rows.Count > 0)
            {
                foreach (DataRow drSub in dtSubModules.Rows)
                {
                    for (int i = 0; i < sTreeModule.Nodes.Count; i++)
                    {
                        if (sTreeModule.Nodes[i].Text == drSub["MenuParent"].ToString())
                        {
                            TreeNode tnChild = new TreeNode(drSub["MenuTitle"].ToString(), drSub["ID"].ToString());
                            tnChild.ShowCheckBox = true;
                            tnChild.SelectAction = TreeNodeSelectAction.Expand;
                            this.sTreeModule.Nodes[i].ChildNodes.Add(tnChild);
                            break;
                        }
                    }
                }
            }
        }
        catch(Exception e)
        {
            Session["ModuleName"] = "浏览公共模块";
            Session["FunctionName"] = "Page_Load";
            Session["ExceptionalType"] = e.GetType().FullName;
            Session["ExceptionalDescription"] = e.Message;
            Response.Redirect("~/Common/MistakesPage.aspx");
        }
    }
    private void QuickDestopBind()
    {
        DataTable dtOP = userDal.GetUserQuickDesktop(Convert.ToInt32(Session["UserID"].ToString())).Tables[0];
        if (dtOP.Rows.Count > 0)
        {
            foreach (DataRow dr in dtOP.Rows)
            {
                foreach (TreeNode tnRoot in this.sTreeModule.Nodes)
                {
                    bool IsAllSelected = true;
                    foreach (TreeNode tnSub in tnRoot.ChildNodes)
                    {
                        bool IsSubAllSelected = true;

                        if (tnSub.Value == dr["ModuleID"].ToString())
                        {
                            tnSub.Checked = true;
                            break;
                        }
                        if (!tnSub.Checked)
                        {
                            IsSubAllSelected = false;
                        }
                    }
                    if (IsAllSelected)
                    {
                        tnRoot.Checked = true;
                    }
                }
            }

        }
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            userDal.DeleteUserQuickDesktop(Session["UserID"].ToString());
            foreach (TreeNode tnRoot in this.sTreeModule.Nodes)
            {
                foreach (TreeNode tnSub in tnRoot.ChildNodes)
                {
                    if (tnSub.Checked)
                    {
                        userDal.InsertUserQuickDesktop(Session["UserID"].ToString(), tnSub.Value);
                    }
                }
            }

            JScript.Instance.ShowMessage(this, "设置成功！");
        }
        catch
        {
            JScript.Instance.ShowMessage(this, "设置失败，请与管理员联系！");
        }
    }
    protected void btnExit_Click(object sender, EventArgs e)
    {
        string strtime = DateTime.Now.Millisecond.ToString();
        Response.Redirect("../../MainPage.aspx?time=" + strtime);
    }
}
