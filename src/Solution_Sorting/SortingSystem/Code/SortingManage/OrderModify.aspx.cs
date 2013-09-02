using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
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
    private static string quantity = "";
    private int orderQuantity = 0;
    private static string orderId = "";
    private static string batchNo = "";
    private static string orderDate = "";
    private static string cigaretteCode="";
    private static string cigaretteName = "";
    //private string 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindData();
            pnlComfirm.Visible = false;
        }
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
        orderId = gvMain.Rows[e.NewEditIndex].Cells[5].Text.Trim();
        orderDate = gvMain.Rows[e.NewEditIndex].Cells[7].Text.Trim();
        batchNo = gvMain.Rows[e.NewEditIndex].Cells[6].Text.Trim();
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
        pnlComfirm.Visible = false;
        gvDetail.EditIndex = -1;
        lblTip.Text = "";
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
    protected void gvDetail_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvDetail.EditIndex = e.NewEditIndex;
        //BindDetailData();
        cigaretteCode=gvDetail.Rows[e.NewEditIndex].Cells[1].Text.Trim();
        cigaretteName=gvDetail.Rows[e.NewEditIndex].Cells[2].Text.Trim();
    }
    protected void gvDetail_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvDetail.EditIndex = -1;
        pnlComfirm.Visible = false;
        lblTip.Text = "";
        BindDetailData();
    }
    protected void gvDetail_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        quantity = ((TextBox)gvDetail.Rows[e.RowIndex].FindControl("txtQuantity")).Text;
        
        if (!string.IsNullOrEmpty(quantity))
        {
            pnlComfirm.Visible = true;
            lblTip.Text = "";
            lblTip.Text = lblTip.Text + "将订单号为：'"+orderId+"'卷烟名称为：'"+cigaretteName+"'的分拣数量修改为：'"+quantity+"'是否保存？";
        }
        //BindDetailData();
    }
    protected void btnNo_Click(object sender, EventArgs e)
    {
        pnlComfirm.Visible = false;
        lblTip.Text = "";
    }
    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {
            HandleSortOrderDal handleSortOrderDal=new HandleSortOrderDal();
            orderQuantity = handleSortOrderDal.GetQuantityByValue(orderId, orderDate,batchNo, cigaretteCode);
            if (Convert.ToInt32(quantity)>orderQuantity)
            {
                JScript.Instance.ShowMessage(UpdatePanel1, "实际分拣数量为："+quantity+"不能大于订单数量："+orderQuantity+"！");
            }
            else
            {
                handleSortOrderDal.updateSortQuantity(Convert.ToInt32(quantity),orderId,orderDate,batchNo,cigaretteCode);
                JScript.Instance.ShowMessage(UpdatePanel1, "已将" + Convert.ToDateTime(orderDate).ToShortDateString() + "批次：" + batchNo + "订单号：" + orderId + cigaretteName + "的分拣数量已成功修改为：" + quantity);
                pnlComfirm.Visible = false;
            }
            BindDetailData();
        }
        catch (Exception EX)
        {
            JScript.Instance.ShowMessage(UpdatePanel1,EX.Message.ToString());
        }
    }
}
