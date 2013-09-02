using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// 脚本提示信息
/// </summary>
public class JScript
{
    public static JScript Instance = new JScript();
	public JScript()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    static int p = 0;
    /// <summary>
    /// 信息提示框


    /// </summary>
    /// <param name="page">Page 对象</param>
    /// <param name="Message">提示信息</param>
    public void ShowMessage(Page page,string Message)
    {
        string str = "<script> "+
                     " alert('" + Message.Replace("'", "").Replace("\r", "").Replace("\n", "").Replace("\t", "") + "');\n"+
                     "</script>";
        page.ClientScript.RegisterStartupScript(page.GetType(), DateTime.Now.ToLongTimeString(), str);
    }

    /// <summary>
    /// 信息提示框
    /// </summary>
    /// <param name="p">UpdatePanel</param>
    /// <param name="Message">提示内容</param>
    public void ShowMessage(UpdatePanel p, string Message)
    {
        Message = Message.Replace("'", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
        ScriptManager.RegisterStartupScript(p, typeof(UpdatePanel), "test", string.Format("alert('{0}');", Message), true);
    }

    public void RegisterScript(Page page, string ScriptContent)
    {
        
        string str = "<script> \n " +ScriptContent+" \n </script>";
        //page.RegisterStartupScript("s1",str);
        page.ClientScript.RegisterStartupScript(page.GetType(), DateTime.Now.ToLongTimeString(), str);
    }
    public void RegisterScript(UpdatePanel p, string ScriptContent)
    {

        string str = "<script> \n " + ScriptContent + " \n </script>";
        ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "key", str, true);
    }  
}
