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

public partial class Code_SortingManage_Default : BasePage
{
    private static AutoSchedule schedule = new AutoSchedule();
    private static DownLoadData downLoad = new DownLoadData();
    private static Thread thread = null;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnExit_Click(object sender, EventArgs e)
    {
        Exit();
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStart_Click(object sender, EventArgs e)
    {
        if (txtOrderDate.Text.Trim().Length != 0)
        {
            ParameterDal pDal = new ParameterDal();
            pDal.UpdateParameter(ddlInterface.SelectedValue.ToString(), "SALESSYSTEMDBTYPE");
            int batchNo = Convert.ToInt32(ddlBatchNo.SelectedItem.Text);
            bool canOptimize = false;

            BatchDal batchDal = new BatchDal();
            DataTable batchTable = batchDal.GetBatch(txtOrderDate.Text, batchNo);

            if (batchTable.Rows[0]["ISVALID"].ToString().Trim() == "0")
            {
                Application.Lock();
                if (Application["ExecuteUser"] == null || (Application["ExecuteUser"].ToString() == Session["G_user"].ToString() &&
                    Application["ExecuteIP"].ToString() == Session["Client_IP"].ToString()))
                {
                    Application["ExecuteUser"] = Session["G_user"];
                    Application["ExecuteIP"] = Session["Client_IP"];

                    Session["OrderDate"] = txtOrderDate.Text;
                    Session["BatchNo"] = batchNo;
                    Session["DataBase"] = ddlInterface.SelectedItem.Text;

                    batchDal.SaveExecuter(Session["G_user"].ToString(), Session["Client_IP"].ToString(), txtOrderDate.Text, batchNo);
                    canOptimize = true;
                }
                Application.UnLock();

                if (canOptimize)
                {
                    btnStart.Enabled = false;
                    btnExit.Enabled = false;
                    btnStop.Enabled = true;
                    
                    ProcessState.InProcessing = true;
                    ProcessState.Status = "START";

                    JScript.Instance.RegisterScript(this, "post=true;");
                    ParameterDal parameterDal = new ParameterDal();
                    Dictionary<string, string> parameters = parameterDal.FindParameter();
                    if (parameters["OptimizAllOrder"] == "1")//如果优化所有数据，则在下载数据和数据优化之间不需要用户进行数据选择
                    {
                       // thread = new Thread(new ThreadStart(OptimizeAll));
                    }
                    else//下载数据后由用户选择需要对哪几条线路进行数据优化,所以先只下载数据而不进行数据优化
                    {
                        thread = new Thread(new ThreadStart(Download));
                    }
                    thread.Start();
                    
                }
                else
                    JScript.Instance.ShowMessage(Page, string.Format("用户'{0}'在机器'{1}'上做优化操作，请稍候。", Application["ExecuteUser"], Application["ExecuteIP"]));
            }
            else
                JScript.Instance.ShowMessage(Page, "该批数据已优化，请选择未优化批次。");
        }
        else
            JScript.Instance.ShowMessage(Page, "请选择日期。");

    }

    /// <summary>
    /// 停止优化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStop_Click(object sender, EventArgs e)
    {
        thread.Abort();

        Application.Lock();
        if (Application["ExecuteUser"] != null)
        {
            BatchDal batchDal = new BatchDal();
            batchDal.SaveExecuter("0", "0", Session["OrderDate"].ToString(), Convert.ToInt32(Session["BatchNo"]));
            schedule.ClearSchedule(Session["OrderDate"].ToString(), Convert.ToInt32(Session["BatchNo"]));

            Application.Remove("ExecuteUser");
            Application.Remove("ExecuteIP");
            Session.Remove("OrderDate");
            Session.Remove("BatchNo");
        }
        Application.UnLock();

        JScript.Instance.RegisterScript(this, "post=false");
        Session.Remove("OptimizeStatus");
        

        btnStart.Enabled = true;
        btnExit.Enabled = true;
        btnStop.Enabled = false;        
    }

    /// <summary>
    /// 只下载数据
    /// </summary>
    private void Download()
    {
        try
        {
            downLoad.DownloadData(Session["OrderDate"].ToString(), Convert.ToInt32(Session["BatchNo"]),Session["DataBase"].ToString());
            if (ProcessState.Status == "PROCESSING")
                ProcessState.Status = "COMPLETE";
            ProcessState.InProcessing = false;
        }
        catch (Exception ex)
        {
            JScript.Instance.ShowMessage(Page, ex.Message.ToString());
        }
    }

    /// <summary>
    /// 只进行数据优化
    /// </summary>
    private void Optimize()
    {
        AfterOptimize();
    }


    /// <summary>
    /// 数据优化后清理优化过程中的变量
    /// </summary>
    private void AfterOptimize()
    {
        BatchDal batchDal = new BatchDal();
        batchDal.SaveExecuter(Session["G_user"].ToString(), Session["Client_IP"].ToString(),
            Session["OrderDate"].ToString(), Convert.ToInt32(Session["BatchNo"]));
        Thread.Sleep(1000);
        Session.Remove("OptimizeStatus");

        Application.Lock();
        if (Application["ExecuteUser"] != null)
        {
            Application.Remove("ExecuteUser");
            Application.Remove("ExecuteIP");
            Session.Remove("OrderDate");
            Session.Remove("BatchNo");
        }
        Application.UnLock();
    }

    /// <summary>
    /// 获取指定日期的所有分拣批次
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lnkBtnGetBatchNo_Click(object sender, EventArgs e)
    {
        BatchDal batchDal = new BatchDal();
        ddlBatchNo.Items.Clear();
        DataTable table = batchDal.GetBatchNo(txtOrderDate.Text);
        bool hasNoSchedul = false;
        foreach (DataRow row in table.Rows)
        {
            ddlBatchNo.Items.Add(new ListItem(row["BATCHNO"].ToString().Trim()));
            if (row["ISVALID"].ToString() == "0")
                hasNoSchedul = true;
        }
        if (!hasNoSchedul)
        {
            batchDal.AddBatch(txtOrderDate.Text, table.Rows.Count + 1);
            ddlBatchNo.Items.Add(new ListItem(Convert.ToString(table.Rows.Count + 1), "0"));
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        pnlMain.Visible = true;
        pnlRoute.Visible = false;
    }
}
