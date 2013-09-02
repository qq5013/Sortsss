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

public partial class Code_BaseInfo_Route: BasePage
{
    private int pageIndex = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnCanel.ToolTip = "1";
            BindData();
        }
    }

    private void BindData()
    {
        pager.PageSize = PagingSize;

        RouteDal routeDal = new RouteDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = routeDal.GetCount(filter);
        DataTable table = routeDal.GetAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
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
        //保存修改到数据库
        RouteDal routeDal = new RouteDal();
        routeDal.Save(txtSortID.Text,txtLineCode.Text, txtRouteCode.Text);
        JScript.Instance.ShowMessage(UpdatePanel1, "保存数据成功。");
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
        pageIndex = Convert.ToInt32(btnCanel.ToolTip);
        BindData();
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        txtRouteCode.Text = gvMain.Rows[e.NewEditIndex].Cells[1].Text;
        txtRouteName.Text = gvMain.Rows[e.NewEditIndex].Cells[2].Text;
        txtSortID.Text = gvMain.Rows[e.NewEditIndex].Cells[6].Text;
        txtLineCode.Text = gvMain.Rows[e.NewEditIndex].Cells[7].Text == "&nbsp;" ? "" : gvMain.Rows[e.NewEditIndex].Cells[7].Text; ;
        SwitchView(false);
    }

    private void SwitchView(bool showList)
    {
        pnlList.Visible = showList;
        pnlEdit.Visible = !showList;
    }

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        btnCanel.ToolTip = e.NewPageIndex.ToString();
        pageIndex = e.NewPageIndex;
        BindData();
    }
}
