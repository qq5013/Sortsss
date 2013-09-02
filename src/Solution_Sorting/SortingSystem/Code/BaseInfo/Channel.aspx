<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Channel.aspx.cs" Inherits="Code_BaseInfo_Channel" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
    <script language="JavaScript" type="text/javascript" src="../../JScript/selectdialog.js"></script>
</head>
<body topmargin = 0 leftmargin="0">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlMain" runat="server" Height="100%" Width="100%" style="LEFT: 0px; POSITION: relative; TOP: 0px">
                    <asp:Panel id="pnlList" runat="server" Width="100%" Height="401px">
                        <TABLE style="HEIGHT: 30px" borderColor=#111111 cellSpacing=0 cellPadding=0 width="100%" border=0>
                            <TBODY>
                                <TR>
                                    <TD borderColor=#cccc99>
                                        <asp:DropDownList id="ddlField" runat="server">
                                            <asp:ListItem Value="LINECODE">分拣线代码</asp:ListItem>
                                            <asp:ListItem Value="CIGARETTECODE">卷烟代码</asp:ListItem>
                                            <asp:ListItem Value="CIGARETTENAME">卷烟名称</asp:ListItem>
                                         </asp:DropDownList> 
                                         <asp:TextBox id="txtValue" runat="server" Height="20px" CssClass="TextBox"></asp:TextBox> 
                                         <asp:Button id="btnQuery" onclick="btnQuery_Click" runat="server" CssClass="ButtonQuery" Text="查询"></asp:Button> 
                                         <asp:Button id="btnExit" onclick="btnExit_Click" runat="server" CssClass="ButtonExit" Text="退出"></asp:Button> 
                                    </TD>
                                </TR>
                            </TBODY>
                        </TABLE>
                        <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                            <asp:GridView id="gvMain" runat="server" Width="100%" OnRowEditing="gvMain_RowEditing" AutoGenerateColumns="False" OnRowDataBound="gvMain_RowDataBound">
                                <Columns>
                                    <asp:CommandField HeaderText="操作" ShowEditButton="True">
                                    <HeaderStyle Width="40px" HorizontalAlign="Center"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:CommandField>
                                    <asp:BoundField DataField="CHANNELCODE" HeaderText="烟道代码">
                                        <HeaderStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHANNELNAME" HeaderText="烟道名称">
                                        <HeaderStyle Width="70px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LINECODE" HeaderText="分拣线">
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHANNELTYPE" HeaderText="烟道类型">
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CIGARETTECODE" HeaderText="卷烟代码">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CIGARETTENAME" HeaderText="卷烟名称" />
<%--                                    <asp:BoundField DataField="LEDGROUP" HeaderText="LED组号" Visible ="false">
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LEDNO" HeaderText="LED编号" Visible = "false">
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>--%>
                                    <asp:BoundField DataField="STATUS" HeaderText="状态">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <HeaderStyle Width="40px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHANNELID" HeaderText="烟道编号" >
                                        <HeaderStyle Width="70px" />
                                    </asp:BoundField>
                                </Columns>

                                <RowStyle BackColor="White" Height="28px"></RowStyle>

                                <HeaderStyle CssClass="gridheader"></HeaderStyle>

                                <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                            </asp:GridView> 
                        </asp:Panel> 
                        <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pager_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager> 
                    </asp:Panel> 
                    <asp:Panel style="LEFT: 0px; POSITION: relative; TOP: 0px" id="pnlEdit" runat="server" Width="100%" Height="131px" Visible="False">
                        <table class="OperationBar">   
                            <tr >
                                <td style="width: 47px" ><asp:Button ID="btnUpdate" CssClass="ButtonSave" runat="server" Text="保存" OnClick="btnUpdate_Click" /></td>
                                <td style="width: 33px" ><asp:Button ID="btnCanel" CssClass="ButtonBack" runat="server" Text="返回" OnClick="btnCanel_Click"  /></td>
                                <td style="height: 20px" ></td>
                            </tr>   
                           </table>
                           <table width="100%">
                                <tr>
                                    <td class="tdTitle">
                                        烟道代码:</td>
                                    <td ><asp:TextBox ID="txtChannelCode" runat="server" MaxLength="12" CssClass="Textbox" Text = "" ></asp:TextBox></td>
                                    <td style="height: 0px; width: 72px;" > 
                                    </td>
                                </tr>
                                <tr class="tdTitle">
                                    <td class="tdTitle">卷烟代码:</td>
                                    <td ><asp:TextBox ID="txtCigaretteCode" runat="server" MaxLength="10" CssClass="TextBox"  Text = ""></asp:TextBox></td>
                                    <td style="height: 20px; width: 72px;" >
                                     <asp:Button ID="Button2"　CssClass="ButtonBrowse" OnClientClick="return SelectDialog2('txtCIGARETTECODE,txtCIGARETTENAME','AS_BI_CIGARETTE','CIGARETTECODE,CIGARETTENAME')" runat="server" Text="..." />
                                        <asp:Button ID="Button1" runat="server" Text="清空" CssClass="ButtonClear"/> 
                                    </td>
                                    <td ></td>
                                </tr>
                                <tr >
                                    <td class="tdTitle">卷烟名称:</td>
                                    <td><asp:TextBox ID="txtCigaretteName" runat="server" MaxLength="50" CssClass="TextBox"  Text = ""></asp:TextBox></td>
                                    <td height="20" style="width: 72px" >
                                        </td>
                                </tr>
<%--                                <tr visible = "false" >
                                    <td visible = "false" class="tdTitle" ><font color="red">*</font>LED组号:</td>
                                    <td visible = "false" ><asp:TextBox ID="txtLedGroup" runat="server" MaxLength="4" CssClass="TextBox"  Text = ""></asp:TextBox></td>
                                    <td visible = "false" height="20" style="width: 72px" ></td>
                                </tr>
                                
                                <tr visible = "false">
                                    <td visible = "false" class="tdTitle"><font color="red">*</font>LED编号:</td>
                                    <td visible = "false"><asp:TextBox ID="txtLedNo" runat="server" MaxLength="4" CssClass="TextBox"  Text = ""></asp:TextBox></td>
                                    <td visible = "false" height="20" style="width: 72px" ></td>
                                </tr>--%>
                                    <tr>
                                    <td class="tdTitle">
                                        是否启用:</td>
                                    <td height="20" align="left" style="width: 132px">
                                        <asp:DropDownList ID="ddlStatus" runat="server">
                                        <asp:ListItem Value="1">启用</asp:ListItem>
                                        <asp:ListItem Value="0">未启用</asp:ListItem>
                                        </asp:DropDownList></td>
                                    <td height="20" style="width: 72px" ></td>
                                </tr>
                        </table>
                    </asp:Panel> 
                </asp:Panel>
                
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
