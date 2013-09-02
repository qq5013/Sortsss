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

public partial class Code_Query_DeliveryAbnormalQuery : BasePage
{
    private int pageIndex = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindData();
    }

    private void BindData()
    {
        pager.PageSize = PagingSize;

        AbnormityCigaretteDal acDal = new AbnormityCigaretteDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = acDal.GetRouteCount(filter);
        DataTable table = acDal.GetRouteAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
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

}
