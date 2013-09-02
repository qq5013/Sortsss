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

public partial class NavigationPage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        NavigationBind();
    }
    protected void NavigationBind()
    {
        if(Request["PG"]==null)
        {
            this.labNavigation.Text = "快速通道";
        }
        else
        {
            THOK.AS.Dal.SysGroupDal sysGroupDal = new THOK.AS.Dal.SysGroupDal();

            this.labNavigation.Text = sysGroupDal.GetNavigation(Request["PG"]);
        }
    }
    public string getColorValue(string s)
    {
        return s;
    }
}