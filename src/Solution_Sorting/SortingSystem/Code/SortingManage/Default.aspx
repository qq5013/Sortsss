<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Code_SortingManage_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
    <link href="../../css/op.css" rel="Stylesheet" />
    <link href="../../css/css.css" rel="Stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../../JScript/ajax.js"></script>
    <script language="JavaScript" type="text/javascript" src="../../JScript/setday9.js"></script>
    <script language="JavaScript" type="text/javascript" src="../../JScript/Check.js"></script>
    
</head>
<body>
    <script language="JavaScript" type="text/javascript">
        var ajaxOBJSubmit = new CallBackObject(OnComplete);
        window.setInterval("stateRequest()", 1000);
        var iStep = 8;
        var post = false;
                
        function initImg() {
            for (var i = 1; i <= iStep; i++)
            {
                var div = document.getElementById("img" + i);
                div.style.width = 0;
            }
        }

        function completeImg(step)
        {
            for (var i = 1; i < step; i++)
            {
                var div = document.getElementById("img" + i);
                if (div.style.width != 290)
                    div.style.width = 290;
            }
        }
                                
        function OnComplete(text, xml)
        {
            var status = xml.getElementsByTagName("status")[0].text;
            if (status == "PROCESSING")
            {
                var step = xml.getElementsByTagName("step")[0].text;
                var div = document.getElementById("img" + step);
                var completeCount = xml.getElementsByTagName("completecount")[0].text;
                var totalCount = xml.getElementsByTagName("totalcount")[0].text;
                
                completeImg(step);
                div.style.width = 290 / totalCount * completeCount;
            }
            else if (status == "ERROR")
            {
                post = false;
                var message = xml.getElementsByTagName("message")[0].text;
                alert(message);
                document.getElementById("btnStop").click();
            }
            else if (status == "COMPLETE")
            {
                post = false;
                completeImg(iStep + 1);                
                document.form1.btnStart.disabled = false;
                document.form1.btnStop.disabled = true;
                document.form1.btnExit.disabled = false;   
                var message = xml.getElementsByTagName("message")[0].text;
                alert("订单数据处理完成，总数量："+message+"条");                
            }
            else if (status == "SwitchView")
            {
                post = false;
                document.getElementById("lbtnSwitch").click();
            }
        }
        
        function stateRequest() {
            if (post)
                ajaxOBJSubmit.PostData("OptimizeState.aspx", "Operator=State");
        }
        
    </script>
    <form id="form1" runat="server">
      <asp:Panel ID="pnlMain" runat="server" Height="80px" Width="100%">
            <asp:Panel ID="pnlTool" runat="server" Height="5px" Width="100%">
                <asp:LinkButton ID="lnkBtnGetBatchNo" runat="server" OnClick="lnkBtnGetBatchNo_Click"></asp:LinkButton>
                <asp:LinkButton ID="lbtnSwitch" runat="server" OnClick="lbtnSwitch_Click"></asp:LinkButton>
            </asp:Panel>
            <table width="460" height="50px" style="background-color:#c1d9f3;" align="center">
                <tr style="padding:20 10 10 10">
                  <td style="width:60px;">&nbsp;&nbsp;分拣日期</td>
                  <td style="width:120px;">
                     <asp:TextBox ID="txtOrderDate" runat="server" CssClass="TextBox" Width="80"  onpropertychange="GetBatchNo();"></asp:TextBox>
                     <input type="button"  class="ButtonDate" onclick="setday(document.getElementById('txtOrderDate'))" id="Button1" />
                  </td>
                  <td style="width:35px;">批次</td>
                  <td style="width:80px;">
                      <asp:DropDownList ID="ddlBatchNo" runat="server">
                          
                      </asp:DropDownList></td>
                      <td style="">
                          接口数据库<asp:DropDownList ID="ddlInterface" runat="server" CssClass="TEXTBOX" Visible="False">
                              <asp:ListItem Value="ncyc-db2">OuterConnection</asp:ListItem>
                              <asp:ListItem Value="ayyc-mssql">OldConnection</asp:ListItem>
                          </asp:DropDownList></td>
                </tr>
             </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;" align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">数据提取与备份</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 24px;"></td>
                                <td style="width: 290px; height: 24px;">
                                    <div id="div1" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img1" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 24px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">
                                    分拣线线路优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 24px;"></td>
                                <td style="width: 290px; height: 24px;">
                                    <div id="div2" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img2" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 24px;"></td>
                            </tr>
                         </table>
                    </td>
                </tr>
            </table>
            
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">
                                    分拣线烟道优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 24px;"></td>
                                <td style="width: 290px; height: 24px;">
                                    <div id="div3" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img3" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 24px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">
                                    拆分订单</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 24px;"></td>
                                <td style="width: 290px; height: 24px;">
                                    <div id="div4" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img4" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 24px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">订单优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 26px;"></td>
                                <td style="width: 290px; height: 26px;">
                                    <div id="div6" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img5" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 26px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 41px;">
                        <table cellspacing="3" style="width: 100%; border-bottom: #d8dfe7 1px dashed">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">备货烟道优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 54px; height: 26px;"></td>
                                <td style="width: 290px; height: 26px;">
                                    <div id="div7" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img6" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 26px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 46px;">
                        <table cellspacing="3" style="width: 100%;">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">补货优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 60px; height: 33px;"></td>
                                <td style="width: 290px; height: 33px;">
                                    <div id="div5" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img7" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 33px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table cellpadding="0" cellspacing="5" style="width: 460px; border-right: silver 1px solid; border-left: silver 1px solid;"  align="center">
                <tr style="display: block; width: 100%">
                    <td style="width: 480px; height: 46px;">
                        <table cellspacing="3" style="width: 100%;">
                           <tr style="display: block; width: 100%">
                                <td colspan="4" style="padding-left: 3em; height: 18px">手工补货优化</td>
                            </tr>
                            <tr style="color: #333300">
                                <td style="width: 60px; height: 33px;"></td>
                                <td style="width: 290px; height: 33px;">
                                    <div id="div8" style="border-right: lavender 1px solid; border-top: lavender 1px solid; left: 332px;
                                                border-left: lavender 1px solid; width: 289px; border-bottom: lavender 1px solid; top: 97px; height: 5px">
                                        <img id="img8" style="background-image: url(../../images/process/process_bar.gif); width: 0px; height: 14px; left: 1px;"/></div>
                                </td>
                                <td style="width: 60px; height: 33px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <table style="border-top: lightgrey 1px solid; width: 460px; height: 40px; border-bottom: lightgrey 1px solid; border-right: lightgrey 1px solid; border-left: lightgrey 1px solid;"  align="center">
                <tr>
                    <td style="padding-left: 100px; padding-bottom: 15px; padding-top: 15px; height: 45px; width: 40px;">
                        <asp:Button ID="btnStart" runat="server" CssClass="ButtonStart" Text="开始" OnClick="btnStart_Click" /></td>
                    <td style="width: 117px; height: 45px" align="center">
                    
                    <asp:Button ID="btnStop" runat="server" CssClass="ButtonStop"  Enabled="False" Text="终止" OnClick="btnStop_Click" /></td>
                    <td style="height: 45px">
                    
                    <asp:Button ID="btnExit" runat="server" Text="退出" CssClass="ButtonExit2" OnClick="btnExit_Click" /></td>
                </tr>
            </table>     
         </asp:Panel>
        <asp:Panel ID="pnlRoute" runat="server" Height="50px" Width="100%" Visible="False">
            <asp:GridView ID="gvRoute" runat="server" Width="100%" AutoGenerateColumns="False" OnRowDataBound="gvRoute_RowDataBound">
                <RowStyle BackColor="White" Height="28px"></RowStyle>

                <HeaderStyle CssClass="GridHeader"></HeaderStyle>
                <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle Width="30px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ROUTECODE" HeaderText="线路代码">
                        <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ROUTENAME" HeaderText="线路名称" />
                    <asp:BoundField DataField="AREACODE" HeaderText="区域代码">
                        <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AREANAME" HeaderText="区域名称">
                        <HeaderStyle Width="250px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SORTID" HeaderText="顺序">
                        <ItemStyle HorizontalAlign="Right" />
                        <HeaderStyle Width="40px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DELIVERY" HeaderText="送货员">
                        <HeaderStyle Width="60px" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnContinue" runat="server" CssClass="ButtonContinue"
                OnClick="btnContinue_Click" Text="继续" />
            <asp:Button ID="btnBack" runat="server" CssClass="ButtonExit2" Text="返回" OnClick="btnBack_Click" /></asp:Panel>
    </form>
</body>
</html>
