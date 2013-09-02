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

public partial class Code_SortingManage_NoOnePro : BasePage
{
    private static Dictionary<string, string> parameter = null;
    private static Thread thread = null;
    private static UploadData uploadData = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            GetParameter();
    }

    private void GetParameter()
    {
        parameter = new ParameterDal().FindParameter();

        txtLocation.Text = parameter["NoOneProFilePath"];
        txtIP.Text = parameter["NoOneProIP"];
        txtPort.Text = parameter["NoOneProPort"];
    }

    protected void lnkBtnGetBatchNo_Click(object sender, EventArgs e)
    {
        BatchDal batchDal = new BatchDal();

        ddlBatchNo.DataSource = batchDal.GetBatchNo(txtOrderDate.Text);
        ddlBatchNo.DataTextField = "BATCHNO";
        ddlBatchNo.DataValueField = "BATCHNO";
        ddlBatchNo.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        parameter["NoOneProFilePath"] = txtLocation.Text;
        parameter["NoOneProIP"] = txtIP.Text;
        parameter["NoOneProPort"] = txtPort.Text;
        new ParameterDal().SaveParameter(parameter);
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        if (this.ddlCigaretteType.SelectedValue == "1")
        {
            uploadData = new UploadData(true);
        }
        else
        {
            uploadData = new UploadData();
        }
        
        if (txtOrderDate.Text.Trim().Length != 0)
        {
            if (ddlBatchNo.Items.Count != 0)
            {
                ProcessState.InProcessing = true;
                ProcessState.Status = "START";
                ProcessState.OrderDate = txtOrderDate.Text;
                ProcessState.BatchNo = Convert.ToInt32(ddlBatchNo.SelectedValue);
                ProcessState.UserName = Session["G_user"].ToString();
                ProcessState.UserIP = Session["Client_IP"].ToString();

                JScript.Instance.RegisterScript(this, "post=true");
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnExit.Enabled = false;

                thread = new Thread(new ThreadStart(Run));
                thread.Start();
            }
            else
                JScript.Instance.ShowMessage(Page, "没有要上传一号工程的订单。");
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
        uploadData.Upload(ProcessState.OrderDate, ProcessState.BatchNo);        
        if (ProcessState.Status == "PROCESSING")
            ProcessState.Status = "COMPLETE";
        ProcessState.InProcessing = false;
    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }
}
