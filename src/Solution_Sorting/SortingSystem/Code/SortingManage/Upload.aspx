<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="Code_SortingManage_Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=aa" rel="Stylesheet" type="text/css" />
    <link href="../../css/css.css?t=099" rel="Stylesheet" />
    <link href="../../css/op.css" rel="Stylesheet" />
    <script src="../../JScript/setday9.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JScript/Check.js?time=11"></script>
    <script type="text/javascript" src="../../JScript/ajax.js"></script>
    

    <script language="JavaScript" type="text/javascript">
        var ajaxOBJSubmit = new CallBackObject(OnComplete);
        window.setInterval("stateRequest()", 1000);
        var post = false;
                
        function initImg() {
            var div = document.getElementById("img1");
            div.style.width = 0;
        }

        function OnComplete(text, xml)
        {
            var status = xml.getElementsByTagName("status")[0].text;
            if (status == "PROCESSING")
            {
                var div = document.getElementById("img1");
                var completeCount = xml.getElementsByTagName("completecount")[0].text;
                var totalCount = xml.getElementsByTagName("totalcount")[0].text;
                div.style.width = 290 / totalCount * completeCount;
            }
            else if (status == "ERROR")
            {
                post = false;
                var message = xml.getElementsByTagName("message")[0].text;
                alert(message);
            }
            else if (status == "COMPLETE")
            {
                post = false;
                var div = document.getElementById("img1");
                div.style.width = 290;
                document.form1.btnStart.disabled = false;
                document.form1.btnExit.disabled = false;
                alert("上报数据完成");                
            }
        }
        
        function stateRequest() {
            if (post)
                ajaxOBJSubmit.PostData("UploadState.aspx", "Operator=State");
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="pnlParameter" runat="server" Height="161px" Width="100%">
            <br />
            <br />
            <br />
            <table align="center" style="border-right: silver 1px solid; border-left: silver 1px solid;">
                <tr style="HEIGHT: 50px; ">
                    <td colspan="2" style="height: 31px;BACKGROUND-COLOR: #c1d9f3"></td>
                </tr>
                <tr>
                    <td class="tdTitle" style="width: 166px; height: 31px;">
                        上报国家局控制机IP:</td>
                    <td style="width: 48px; height: 31px;"><asp:TextBox ID="txtIP" runat="server" CssClass="TextBox" MaxLength="12"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="tdTitle" style="width: 166px; height:31px">
                        上报国家局控制机端口:</td>
                    <td style="width: 48px"><asp:TextBox ID="txtPort" runat="server" CssClass="TextBox" MaxLength="25"></asp:TextBox></td>
                </tr>
                <%--
                <tr>
                    <td class="tdTitle" style="width: 166px; height:31px">上传文件地址:</td>
                    <td style="width: 48px"><asp:TextBox ID="txtLocation" runat="server" CssClass="TextBox" MaxLength="12"></asp:TextBox></td>
                </tr> --%>
                <tr style="height: 50px; background-color: #c1d9f3;">
                    <td colspan="2">
                        <asp:Button ID="btnSave" runat="server" CssClass="ButtonSave" Text="保存" OnClick="btnSave_Click" /></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlUpload" runat="server" Height="157px" Width="100%">
            <br />
            <br />
            <table align="center" style="border-right: silver 1px solid; border-left: silver 1px solid;">
                <tr style="height: 50px; background-color: #c1d9f3;">
                    <td colspan="3" style="height: 31px"></td>
                </tr>
                <tr>
                    <td class="tdTitle" style="width: 166px; height: 31px;">
                        上报日期:</td>
                    <td style="width: 40px; height: 31px;"><asp:TextBox ID="txtOrderDate" runat="server" onpropertychange="GetBatchNo();" CssClass="TextBox" MaxLength="10" Width="78px"></asp:TextBox></td>
                    <td style="width: 115px">
                        <input class="ButtonDate" onclick="setday(document.getElementById('txtOrderDate'))" type="button"/></td>
                </tr>
                <%-- 
                <tr>
                    <td class="tdTitle" style="width: 166px; height:31px">
                        上报状态:</td>
                    <td style="width: 40px">
                        <asp:DropDownList ID="ddlBatchNo" runat="server" Visible="False">
                        </asp:DropDownList></td>
                    <td style="width: 115px">
                        <asp:LinkButton ID="lnkBtnGetBatchNo" runat="server" OnClick="lnkBtnGetBatchNo_Click" Visible="False"></asp:LinkButton></td>
                </tr>--%>
                <tr>
                    <td colspan="3" style="height: 60px">
                        上报国家局分拣数据<div id="div1" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                            <img id="img1" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/>
                        </div>
                    </td>
                </tr>
                <tr style="height: 50px; background-color: #c1d9f3; border-top:silver 1px solid">
                    <td colspan="3">
                        <asp:Button ID="btnStart" runat="server" CssClass="ButtonStart" Text="上报" OnClick="btnStart_Click" />
                        <asp:Button ID="btnStop" runat="server" CssClass="ButtonStop" Text="取消" OnClick="btnStop_Click" Enabled="False" Visible="False" />
                        <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit2" OnClick="btnExit_Click"
                            Text="返回" /></td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
