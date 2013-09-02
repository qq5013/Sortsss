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

public partial class Code_Query_LineOrderQuery : BasePage
{
    private int pageIndex = 1;
    private int detailPageIndex = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindData();
    }

    private void BindData()
    {
        pager.PageSize = PagingSize;

        OrderScheduleDal osDal = new OrderScheduleDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = osDal.GetMasterCount(filter);
        DataTable table = osDal.GetMasterAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
    }

    private void BindDetailData()
    {
        string filter = ViewState["DetailFilter"].ToString();
        OrderScheduleDal osDal = new OrderScheduleDal();
        pagerDetail.PageSize = PagingSize;
        pagerDetail.RecordCount = osDal.GetDetailCount(filter) ;
        DataTable table = osDal.GetDetailAll(detailPageIndex, PagingSize, filter);
        BindTable2GridView(gvDetail, table);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        pageIndex = 1;
        ViewState["Filter"] = string.Format("ORDERDATE='{0}' AND BATCHNO='{1}'", txtOrderDate.Text, ddlBatchNo.SelectedValue);
        BindData();
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }
    
    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string detailFilter = string.Format("ORDERDATE='{0}' AND BATCHNO='{1}' AND LINECODE='{2}' AND SORTNO='{3}'", gvMain.Rows[e.NewEditIndex].Cells[1].Text.Trim(),
            gvMain.Rows[e.NewEditIndex].Cells[2].Text.Trim(), gvMain.Rows[e.NewEditIndex].Cells[4].Text.Trim(), gvMain.Rows[e.NewEditIndex].Cells[3].Text.Trim());
        ViewState["DetailFilter"] = detailFilter;
        BindDetailData();
        SwitchView(false);
    }

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        pageIndex = e.NewPageIndex;
        BindData();
    }
    protected void pagerDetail_PageChanging(object src, PageChangingEventArgs e)
    {
        detailPageIndex = e.NewPageIndex;
        BindDetailData();
    }

    private void SwitchView(bool masterShow)
    {
        pnlList.Visible = masterShow;
        pnlDetail.Visible = !masterShow;
    }

    protected void lnkBtnGetBatchNo_Click(object sender, EventArgs e)
    {
        ddlBatchNo.DataTextField = "BATCHNO";
        ddlBatchNo.DataValueField = "BATCHNO";
        ddlBatchNo.DataSource = new BatchDal().GetBatchNo(txtOrderDate.Text);
        ddlBatchNo.DataBind();
    }
    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
    }
}
