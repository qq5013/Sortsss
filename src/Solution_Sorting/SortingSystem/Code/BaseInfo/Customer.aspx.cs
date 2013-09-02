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

public partial class Code_BaseInfo_Customer : BasePage
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

        CustomerDal customerDal = new CustomerDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = customerDal.GetCount(filter);
        DataTable table = customerDal.GetAll(pageIndex, PagingSize, filter);
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

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        pageIndex = e.NewPageIndex;
        BindData();
    }
}
