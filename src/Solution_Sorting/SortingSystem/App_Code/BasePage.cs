using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
/// <summary>
/// BasePage 的摘要说明

/// </summary>
public class BasePage : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            Session["IsUseGlobalParameter"] = "1";
            if (Request.QueryString["SubModuleCode"] != null)
            {
                Session["SubModuleCode"] = Request.QueryString["SubModuleCode"];
            }
            if (Session["G_user"] == null)
            {
                Response.Redirect("~/SessionTimeOut.aspx");
            }
            
        }
        catch
        {
        }
    }

    protected void Page_PreLoad(object sender, EventArgs e)
    {
        #region 权限控制
        try
        {
            if (Session["SubModuleCode"] != null)
            {
                DataTable dtOP = (DataTable)(Session["DT_UserOperation"]);
                DataRow[] drs = dtOP.Select(string.Format("SubModuleCode='{0}'", Session["SubModuleCode"].ToString()));
                foreach (DataRow dr in drs)
                {
                    int op = int.Parse(dr["OperatorCode"].ToString());
                    switch (op)
                    {
                        case 0: ((Button)Page.FindControl("btnCreate")).Enabled = true; break;
                        case 1: ((Button)Page.FindControl("btnDelete")).Enabled = true; 

                                break;
                        case 2: ((HiddenField)Page.FindControl("hdnXGQX")).Value = "1"; break;
                        case 3: break;
                        case 4: ((Button)Page.FindControl("btnExport")).Enabled = true; break;
                        case 5: ((Button)Page.FindControl("btnPrint")).Enabled = true; break;
                        case 6: ((Button)Page.FindControl("btnAudit")).Enabled = true; break;
                        default: break;
                    }
                }
            }
        }
        catch 
        {
            ////JScript.Instance.JScript.Instance.ShowMessage(UpdatePanel1, (Page,ex.Message);
        }

        #endregion    
    }

    protected int PagingSize
    {
        get
        {
            if (Session["sys_PageCount"] == null)
                return 10;
            else
                return Convert.ToInt32(Session["sys_PageCount"]);
        }
    }

    /// <summary>
    /// 绑定DataTable到GridView
    /// </summary>
    /// <param name="gridView">显示数据的GridView</param>
    /// <param name="table">数据源</param>
    protected void BindTable2GridView(GridView gridView, DataTable table)
    {
        if (Session["grid_oddrowcolor"] != null)
        {
            gridView.AlternatingRowStyle.BackColor = System.Drawing.Color.FromName(Session["grid_oddrowcolor"].ToString());
            gridView.RowStyle.BackColor = System.Drawing.Color.FromName(Session["grid_evenrowcolor"].ToString());
        }

        if (table.Rows.Count != 0)
        {
            gridView.DataSource = table;
            gridView.DataBind();
        }
        else
        {
            table.Rows.Add(table.NewRow());
            gridView.DataSource = table;
            gridView.DataBind();
            int columnCount = gridView.Rows[0].Cells.Count;
            
            gridView.Rows[0].Cells.Clear();
            gridView.Rows[0].Cells.Add(new TableCell());
            gridView.Rows[0].Cells[0].ColumnSpan = columnCount;
            gridView.Rows[0].Cells[0].Text = "没有符合条件的数据";
            gridView.Rows[0].Visible = true;
        }
    }

    /// <summary>
    /// 退出到主页面
    /// </summary>
    protected void Exit()
    {
        Response.Redirect("~/MainPage.aspx");
    }
}
