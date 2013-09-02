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
using THOK.AS.Dal;

public partial class Code_BaseInfo_LineInfo: BasePage
{
    private int pageIndex = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            BindData();
    }

    private void BindData()
    {
        pager.PageSize = PagingSize;

        LineInfoDal lineinfoDal = new LineInfoDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = lineinfoDal.GetCount(filter);
        DataTable table = lineinfoDal.GetAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        pager.CurrentPageIndex = 1;
        pageIndex = 1;
        ViewState["Filter"] = string.Format("{0} like '%{1}%'", ddlField.SelectedValue, txtValue.Text);
        BindData();
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }
    
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        ////保存修改到数据库
        LineInfoDal lineinfoDal = new LineInfoDal();
        lineinfoDal.Save(txtLineCode.Text, ddlStatus.SelectedValue);
        JScript.Instance.ShowMessage(UpdatePanel1, "保存数据成功。");
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
        BindData();
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        txtLineCode.Text = gvMain.Rows[e.NewEditIndex].Cells[1].Text;
        txtLineName.Text = gvMain.Rows[e.NewEditIndex].Cells[2].Text;
        txtLineType.Text = gvMain.Rows[e.NewEditIndex].Cells[3].Text;

        ddlStatus.ClearSelection();
        ddlStatus.Items.FindByText(gvMain.Rows[e.NewEditIndex].Cells[4].Text).Selected = true;
        SwitchView(false);
    }

    private void SwitchView(bool showList)
    {
        pnlList.Visible = showList;
        pnlEdit.Visible = !showList;
    }

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        pageIndex = e.NewPageIndex;
        BindData();
    }
    protected void gvMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.Cells[4].Text == "1")
            e.Row.Cells[4].Text = "启用";
        else
            e.Row.Cells[4].Text = "停用";
    }
}
