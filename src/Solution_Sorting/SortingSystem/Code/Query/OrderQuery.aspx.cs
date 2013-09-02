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

public partial class Code_Query_OrderQuery : BasePage
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

        OrderDal orderDal = new OrderDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = orderDal.GetMasterCount(filter);
        DataTable table = orderDal.GetMasterAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
    }

    private void BindDetailData()
    {
        string filter = ViewState["DetailFilter"].ToString();
        OrderDal orderDal = new OrderDal();
        pagerDetail.PageSize = PagingSize;
        pagerDetail.RecordCount = orderDal.GetDetailCount(filter);
        DataTable table = orderDal.GetDetailAll(detailPageIndex, PagingSize, filter);
        BindTable2GridView(gvDetail, table);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        pageIndex = 1;
        ViewState["Filter"] = string.Format("A.ORDERDATE='{0}' AND A.BATCHNO='{1}'", txtOrderDate.Text, ddlBatchNo.SelectedValue);
        BindData();
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string detailFilter = string.Format("ORDERID='{0}'", gvMain.Rows[e.NewEditIndex].Cells[5].Text.Trim());
        ViewState["DetailFilter"] = detailFilter;
        BindDetailData();
        SwitchView(false);
    }

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        pageIndex = e.NewPageIndex;
        BindData();
    }
    protected void lnkBtnGetBatchNo_Click(object sender, EventArgs e)
    {
        ddlBatchNo.DataTextField = "BATCHNO";
        ddlBatchNo.DataValueField = "BATCHNO";
        ddlBatchNo.DataSource = new BatchDal().GetBatchNo(txtOrderDate.Text);
        ddlBatchNo.DataBind();
    }

    private void SwitchView(bool masterShow)
    {
        pnlList.Visible = masterShow;
        pnlDetail.Visible = !masterShow;
    }
    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
    }
    protected void pagerDetail_PageChanging(object src, PageChangingEventArgs e)
    {
        detailPageIndex = e.NewPageIndex;
        BindDetailData();
    }
    protected void btnQueryCust_Click(object sender, EventArgs e)
    {
        pageIndex = 1;
        ViewState["Filter"] = string.Format("A.ORDERDATE='{0}' AND A.BATCHNO='{1}' AND A.CUSTOMERCODE='{2}'", txtOrderDate.Text, ddlBatchNo.SelectedValue, txtCusCode.Text);
        BindData();
    }
}
