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

public partial class Code_BaseInfo_Channel: BasePage
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

        StockChannelDal stockChannelDal = new StockChannelDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = stockChannelDal.GetCount(filter);
        DataTable table = stockChannelDal.GetAll(pageIndex, PagingSize, filter);
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
        int quantity = 0;
        if (!Int32.TryParse(txtLedNo.Text, out quantity))
        {
            quantity = 0;
        }
        StockChannelDal stockChannelDal = new StockChannelDal();
        stockChannelDal.Save(txtChannelCode.Text, txtCigaretteCode.Text, txtCigaretteName.Text, quantity, ddlStatus.SelectedValue, ddlIsStockIn.SelectedValue);
        JScript.Instance.ShowMessage(UpdatePanel1, "保存数据成功。");
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
        BindData();
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        txtChannelCode.Text = gvMain.Rows[e.NewEditIndex].Cells[1].Text;
        txtCigaretteCode.Text = gvMain.Rows[e.NewEditIndex].Cells[4].Text == "&nbsp;" ? "" : gvMain.Rows[e.NewEditIndex].Cells[4].Text;
        txtCigaretteName.Text = gvMain.Rows[e.NewEditIndex].Cells[5].Text == "&nbsp;" ? "" : gvMain.Rows[e.NewEditIndex].Cells[5].Text;
        txtLedNo.Text = gvMain.Rows[e.NewEditIndex].Cells[6].Text;
        string strstatus = gvMain.Rows[e.NewEditIndex].Cells[8].Text;
        ddlStatus.ClearSelection();
        ddlStatus.Items.FindByText(strstatus).Selected = true; ;

        string strIsStockIn = gvMain.Rows[e.NewEditIndex].Cells[7].Text;
        ddlIsStockIn.ClearSelection();
        ddlIsStockIn.Items.FindByText(strIsStockIn).Selected = true; 

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
        if (e.Row.Cells[3].Text == "3")
            e.Row.Cells[0].Enabled = false;

        if (e.Row.Cells[8].Text == "1")
            e.Row.Cells[8].Text = "启用";
        else if (e.Row.Cells[8].Text == "0")
            e.Row.Cells[8].Text = "未启用";

        if (e.Row.Cells[7].Text == "1")
            e.Row.Cells[7].Text = "已入库";
        else if (e.Row.Cells[7].Text == "0")
            e.Row.Cells[7].Text = "未入库";

        if (e.Row.Cells[3].Text.Trim() == "2")
            e.Row.Cells[3].Text = "单一烟道";
        else if (e.Row.Cells[3].Text == "3")
            e.Row.Cells[3].Text = "混合烟道";
    }
}
