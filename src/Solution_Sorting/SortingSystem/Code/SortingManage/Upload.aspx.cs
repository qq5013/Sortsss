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
using System.Threading;
using THOK.AS.Schedule;
using THOK.AS.Dal;

public partial class Code_SortingManage_Upload : BasePage
{
    private static Dictionary<string, string> parameter = null;
    private static Thread thread = null;
    private static UploadSortingData uploadData = new UploadSortingData();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            GetParameter();
    }

    private void GetParameter()
    {
        parameter = new ParameterDal().FindParameter();
        txtIP.Text = parameter["UploadDataIp"];
        txtPort.Text = parameter["UploadDataPort"];
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        parameter["UploadDataIp"] = txtIP.Text;
        parameter["UploadDataPort"] = txtPort.Text;
        new ParameterDal().SaveParameter(parameter);
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        if (txtOrderDate.Text.Trim().Length != 0)
        {
            DataTable UploadState = uploadData.GetUploadState(txtOrderDate.Text.Trim());
            if (UploadState.Rows.Count ==0)
            {
                ProcessState.InProcessing = true;
                ProcessState.Status = "START";
                ProcessState.OrderDate = txtOrderDate.Text;
                //ProcessState.BatchNo = Convert.ToInt32(ddlBatchNo.SelectedValue);
                ProcessState.UserName = Session["G_user"].ToString();
                ProcessState.UserIP = Session["Client_IP"].ToString();

                JScript.Instance.RegisterScript(this, "post=true");
                btnStart.Enabled = false;
                btnExit.Enabled = false;

                thread = new Thread(new ThreadStart(Run));
                thread.Start();
            }
            else
                JScript.Instance.ShowMessage(Page, "所选日期的订单已上报。");
        }
        else
            JScript.Instance.ShowMessage(Page, "请选择日期。");
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        ProcessState.InProcessing = false;
        ProcessState.Status = "STOP";
        JScript.Instance.RegisterScript(this, "post=false");
        thread.Abort();
        btnStart.Enabled = true;
        btnStop.Enabled = false;
        btnExit.Enabled = true;
    }

    private void Run()
    {
        uploadData.UploadSorting(ProcessState.OrderDate, ProcessState.UserName, ProcessState.UserIP);
        if (ProcessState.Status == "PROCESSING")
            ProcessState.Status = "COMPLETE";
        ProcessState.InProcessing = false;
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }
}
