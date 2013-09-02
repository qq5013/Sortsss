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

public partial class HandleSortOrder: BasePage
{
    private int pageIndex = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
            BindData();
    }

    private void BindData()
    {
        pager.PageSize = PagingSize;

        HandleSortOrderDal handleSortOrderDal = new HandleSortOrderDal();

        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();

        pager.RecordCount = handleSortOrderDal.GetCount(filter);
        DataTable table = handleSortOrderDal.GetAll(pageIndex, PagingSize, filter);
        BindTable2GridView(gvMain, table);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        pageIndex = 1;
        if (txtOrderDateQ.Text != string.Empty)
        {
            ViewState["Filter"] = string.Format(" ORDERDATE='{0}' AND ORDERID like '%{1}%'", txtOrderDateQ.Text, txtOrderIdValue.Text);
        }
        else
        {
            ViewState["Filter"] = string.Format(" ORDERID like '%{0}%'",txtOrderIdValue.Text);
        }
        
        BindData();
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }

    private void SwitchView(bool showList)
    {
        pnlList.Visible = showList;
        pnlEdit.Visible = !showList;
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        //保存修改到数据库
        try
        {
            HandleSortOrderDal handleSortOrderDal = new HandleSortOrderDal();
            if (ViewState["OP"].ToString() == "ADD")
            {
                handleSortOrderDal.Insert(txtOrderDate.Text,txtOrderId.Text);
            }
            else
            {
                handleSortOrderDal.Save(txtOrderDate.Text, txtOrderId.ToolTip,txtOrderId.Text);

            }
            JScript.Instance.ShowMessage(UpdatePanel1, "保存数据成功。");
        }
        catch (Exception ex)
        {
            JScript.Instance.ShowMessage(UpdatePanel1, string.Format("保存数据失败，原因：'{0}'", ex.Message));
        }
    }  

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ViewState["OP"] = "ADD";
        SwitchView(false);
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        ViewState["OP"] = "EDIT";
        txtOrderDate.Text = gvMain.Rows[e.NewEditIndex].Cells[2].Text;
        txtOrderId.Text = gvMain.Rows[e.NewEditIndex].Cells[3].Text;
        txtOrderId.ToolTip = gvMain.Rows[e.NewEditIndex].Cells[3].Text;
        txtOrderDate.ReadOnly = true;
        Button1.Disabled = true;
        SwitchView(false);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        HandleSortOrderDal handleSortOrderDal = new HandleSortOrderDal();
        int deleteCount = 0;
        foreach (GridViewRow row in gvMain.Rows)
        {
            if (row.Cells[0].Controls.Count != 0 && ((CheckBox)row.Cells[0].Controls[0]).Checked)
            {
                handleSortOrderDal.Delete(row.Cells[2].Text, row.Cells[3].Text);
                deleteCount++;
            }
        }
        if (deleteCount == 0)
            JScript.Instance.ShowMessage(UpdatePanel1, "请选择要删除的记录。");
        else
            JScript.Instance.ShowMessage(UpdatePanel1, "删除数据成功。");
        BindData();
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
        BindData();
        txtOrderDate.ReadOnly = false;
        Button1.Disabled = false;

        txtOrderId.Text = "";
        txtOrderId.ToolTip = "";
    }

    protected void cbHeader_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvMain.Rows)
        {
            ((CheckBox)row.Cells[0].Controls[1]).Checked = true;
        }
    }
    protected void gvMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            CheckBox chk = new CheckBox();
            chk.ID = "checkAll";
            chk.Attributes.Add("style", "word-break:keep-all; white-space:nowrap");
            e.Row.Cells[0].Controls.Add(chk);
            chk.Attributes.Add("onclick", "checkboxChange(this,'gvMain');");
            e.Row.Attributes.Add("class", "GridHeader");
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chk = new CheckBox();
            e.Row.Cells[0].Controls.Add(chk);
            e.Row.Cells[0].Style.Add("align", "center");
        }
    }

    protected void pager_PageChanging(object src, PageChangingEventArgs e)
    {
        pageIndex = e.NewPageIndex;
        BindData();
    }
}
