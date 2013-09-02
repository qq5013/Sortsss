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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Data.SqlClient;
using THOK.AS.Dal;

public partial class Code_SysInfomation_BusinessProcessSetup_BusinessProcess :BasePage
{
    SysParameterDal parameterDal = new SysParameterDal();

    public string SingleLineColor;
    public string DoubleLineColor;
    public string[] HeadFont = new string[4];
    public string[] TableFont = new string[4];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtSessionTimeOut.Attributes.Add("onkeypress", "if(!/[0-9]/.test(String.fromCharCode(event.keyCode)))event.keyCode=0");
            txtSessionTimeOut.Attributes.Add("onpaste", "return   false");
            txtSessionTimeOut.Attributes.Add("ondragenter", "return   false");
            ControlsDataBind();
            Session.Timeout = int.Parse(txtSessionTimeOut.Text);
        }
    }

    #region 初始化页面控件数据    2007-09-25 Authored by Huang
    public void ControlsDataBind()
    {
        DataTable parameterTable = null;
        DataTable optionTable = null;
        try
        {
            parameterTable = parameterDal.GetAll();
            optionTable = parameterDal.GetFormatParameter();

        }
        catch (Exception e)
        {
            Session["ModuleName"] = "系统信息参数设置";
            Session["FunctionName"] = "ControlsDataBind";
            Session["ExceptionalType"] = e.GetType().FullName;
            Session["ExceptionalDescription"] = e.Message;
            Response.Redirect("MistakesPage.aspx");
        }

        #region 网格设置
            for (int i = 0; i < 4; i++)
            {
                HeadFont[i] = Session["grid_ColumnTitleFont"].ToString().Split(',')[i];// drPar["ParameterValue"].ToString().Split(',')[i];
            }

            for (int i = 0; i < 4; i++)
            {

                TableFont[i] = Session["grid_ContentFont"].ToString().Split(',')[i];//
            }
        #endregion


        #region 参数设置项数据绑定
        foreach (DataRow drPar in parameterTable.Rows)   //
        {

            #region 格式设置
            if (drPar["ParameterName"].ToString() == "sys_FormatNumberMode")
            {
                DataTable dtNum = optionTable.Copy();
                for (int i = 0; i < dtNum.Rows.Count; i++)
                {
                    if (dtNum.Rows[i]["ParameterName"].ToString() != "ddl_FormatNumberMode")
                    {
                        dtNum.Rows[i].Delete();
                    }
                }

                this.ddlSys_FormatNumberMode.DataSource = dtNum;
                this.ddlSys_FormatNumberMode.DataTextField = "ParameterText";
                this.ddlSys_FormatNumberMode.DataValueField = "ParameterValue";
                this.ddlSys_FormatNumberMode.DataBind();
                this.ddlSys_FormatNumberMode.SelectedValue = drPar["ParameterValue"].ToString();
            }

            //
            if (drPar["ParameterName"].ToString() == "sys_FormatMoneyMode")
            {
                DataTable dtMoney = optionTable.Copy();
                for (int i = 0; i < dtMoney.Rows.Count; i++)
                {
                    if (dtMoney.Rows[i]["ParameterName"].ToString() != "ddl_FormatMoneyMode")
                    {
                        dtMoney.Rows[i].Delete();
                    }
                }

                this.ddlSys_FormatMoneyMode.DataSource = dtMoney;

                this.ddlSys_FormatMoneyMode.DataTextField = "ParameterText";
                this.ddlSys_FormatMoneyMode.DataValueField = "ParameterValue";
                this.ddlSys_FormatMoneyMode.DataBind();
                this.ddlSys_FormatMoneyMode.SelectedValue = drPar["ParameterValue"].ToString();
            }

            //
            if (drPar["ParameterName"].ToString() == "sys_FormatDateTimeMode")
            {
                DataTable dtDate = optionTable.Copy();
                for (int i = 0; i < dtDate.Rows.Count; i++)
                {
                    if (dtDate.Rows[i]["ParameterName"].ToString() != "ddl_FormatDateTimeMode")
                    {
                        dtDate.Rows[i].Delete();
                    }
                }

                this.ddlSys_FormatDateTimeMode.DataSource = dtDate;

                this.ddlSys_FormatDateTimeMode.DataTextField = "ParameterText";
                this.ddlSys_FormatDateTimeMode.DataValueField = "ParameterValue";
                this.ddlSys_FormatDateTimeMode.DataBind();
                this.ddlSys_FormatDateTimeMode.SelectedValue = drPar["ParameterValue"].ToString();
            }
            if (drPar["ParameterName"].ToString() == "sys_SessionTimeOut")
            {
                
                this.txtSessionTimeOut.Text = drPar["ParameterValue"].ToString();
            }
            

            #endregion
        }
        #endregion  foreach循环结束

        if (HeadFont[3] == "加粗")
            HeadFont[3] = "bold";
        if (TableFont[3] == "加粗")
            TableFont[3] = "bolder";


    }
    #endregion  初始化结束




    #region 保存设置的数据   2007-09-26
    public void SaveData()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("sys_FormatNumberMode", this.ddlSys_FormatNumberMode.SelectedValue);
        parameters.Add("sys_FormatMoneyMode", this.ddlSys_FormatMoneyMode.SelectedValue);
        parameters.Add("sys_FormatDateTimeMode", this.ddlSys_FormatDateTimeMode.SelectedValue);
        parameters.Add("sys_SessionTimeOut", this.txtSessionTimeOut.Text);
        bool flag = false;
        try
        {
            parameterDal.SaveParameter(parameters);
            flag = true;
        }
        catch (SqlException e)
        {
            Session["ModuleName"] = " 系统信息参数设置";
            Session["FunctionName"] = "ControlsDataBind";
            Session["ExceptionalType"] = e.GetType().FullName;
            Session["ExceptionalDescription"] = e.Message;
            Response.Redirect("~/Common/MistakesPage.aspx");
        }
        if (flag)
        {
            JScript.Instance.ShowMessage(this, "保存成功！");
        }
        else
        {
            JScript.Instance.ShowMessage(this, "保存失败！");
        }
        if (Session["IsUseGlobalParameter"].ToString() == "1")
        {
            DoubleLineColor = Session["grid_EvenRowColor"].ToString();//dc.GetFieldValue("sys_SystemParameter", "ParameterValue", "ParameterName", "grid_EvenRowColor");
            SingleLineColor = Session["grid_OddRowColor"].ToString();//dc.GetFieldValue("sys_SystemParameter", "ParameterValue", "ParameterName", "grid_OddRowColor");
        
        }
        else
        {
            DoubleLineColor = "";
            SingleLineColor = "";
        }

        if (HeadFont[3] == "加粗")
            HeadFont[3] = "bold";
        if (TableFont[3] == "加粗")
            TableFont[3] = "bolder";
    }
    #endregion





    #region 事件处理
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            Session.Timeout = int.Parse(txtSessionTimeOut.Text);
        }
        catch(Exception ex)
        {
            Session["ModuleName"] = " 系统信息参数设置";
            Session["FunctionName"] = "btnSave_Click";
            Session["ExceptionalType"] = e.GetType().FullName;
            Session["ExceptionalDescription"] = ex.Message;
            Response.Redirect("~/Common/MistakesPage.aspx");
        }
        SaveData();   //新的保存方法     
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Exit();
    }
    

    #endregion



    
    
}
