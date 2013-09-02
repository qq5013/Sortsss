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

public partial class Code_BaseInfo_Employee: BasePage
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

        EmployeeDal employeeDal = new EmployeeDal();
        string filter = null;
        if (ViewState["Filter"] != null)
            filter = ViewState["Filter"].ToString();
        pager.RecordCount = employeeDal.GetCount(filter);
        DataTable table = employeeDal.GetAll(pageIndex, PagingSize, filter);

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
        ////保存修改到数据库
        EmployeeDal employeeDal = new EmployeeDal();
        if (ViewState["OP"].ToString() == "ADD")
        {
            employeeDal.Insert(txtEmployeeCode.Text, txtEmployeeName.Text, txtDepartmentID.Text, ddlStatus.SelectedValue, txtRemark.Text);
        }
        else
        {
            employeeDal.Save(txtEmployeeCode.Text, txtEmployeeName.Text, txtDepartmentID.Text, ddlStatus.SelectedValue, txtRemark.Text);
        }
        JScript.Instance.ShowMessage(UpdatePanel1, "保存数据成功。");
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        SwitchView(true);
    }

    protected void gvMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        ViewState["OP"] = "EDIT";
        txtEmployeeCode.Text = gvMain.Rows[e.NewEditIndex].Cells[2].Text;
        txtEmployeeName.Text = gvMain.Rows[e.NewEditIndex].Cells[3].Text;
        txtDepartmentID.Text = gvMain.Rows[e.NewEditIndex].Cells[4].Text;
        txtDepartmentName.Text = gvMain.Rows[e.NewEditIndex].Cells[5].Text;
        ddlStatus.ClearSelection();
        ddlStatus.Items.FindByText(gvMain.Rows[e.NewEditIndex].Cells[6].Text).Selected = true;
        txtRemark.Text = gvMain.Rows[e.NewEditIndex].Cells[7].Text;
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

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        EmployeeDal employeeDal = new EmployeeDal();
        int deleteCount = 0;
        foreach (GridViewRow row in gvMain.Rows)
        {
            if (row.Cells[0].Controls.Count != 0 && ((CheckBox)row.Cells[0].Controls[0]).Checked)
            {
                employeeDal.Delete(row.Cells[2].Text);
                deleteCount++;
            }
        }
        if (deleteCount == 0)
            JScript.Instance.ShowMessage(UpdatePanel1, "请选择要删除的记录。");
        else
            JScript.Instance.ShowMessage(UpdatePanel1, "删除数据成功。");
        BindData();
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
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ViewState["OP"] = "ADD";
        SwitchView(false);
    }
}
