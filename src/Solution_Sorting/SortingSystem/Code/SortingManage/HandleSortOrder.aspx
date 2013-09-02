<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HandleSortOrder.aspx.cs" Inherits="HandleSortOrder" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
    <script language="JavaScript" type="text/javascript" src="../../JScript/setday9.js"></script>
    <script type="text/javascript" src="../../JScript/Check.js"></script>
</head>
<body topmargin = 0 leftmargin="0">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server"><ContentTemplate>
<asp:Panel style="LEFT: 0px; POSITION: relative; TOP: 0px" id="pnlMain" runat="server" Width="100%" Height="100%"><asp:Panel id="pnlList" runat="server" Width="100%" Height="401px"><TABLE style="HEIGHT: 30px" borderColor=#111111 cellSpacing=0 cellPadding=0 width="100%" border=0><TBODY><TR>
<TD borderColor=#cccc99>日期：<asp:TextBox ID="txtOrderDateQ" runat="server" CssClass="TextBox" Width="80px"></asp:TextBox>
                                    <input id="btnDate" class="ButtonDate" onclick="setday(document.getElementById('txtORDERDATEQ'))" type="button" />
                                    
                                    订单号:<asp:TextBox id="txtOrderIdValue" runat="server" CssClass="TextBox"></asp:TextBox> 
                                    <asp:Button id="btnQuery" onclick="btnQuery_Click" runat="server" CssClass="ButtonQuery" Text="查询">
                                    </asp:Button> <asp:Button id="btnAdd" onclick="btnAdd_Click" runat="server" CssClass="ButtonCreate" Text="新增">
                                    </asp:Button> <asp:Button id="btnDelete" onclick="btnDelete_Click" runat="server" CssClass="ButtonDel" Text="删除">
                                    </asp:Button> <asp:Button id="btnExit" onclick="btnExit_Click" runat="server" CssClass="ButtonExit" Text="退出"></asp:Button> 
                                    </TD></TR></TBODY></TABLE><asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                                        <asp:GridView id="gvMain" runat="server" Width="100%" AutoGenerateColumns="False" OnRowEditing="gvMain_RowEditing" OnRowDataBound="gvMain_RowDataBound"><Columns>
            <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle Width="30px" />
                    </asp:TemplateField>
<asp:CommandField InsertVisible="False" ShowCancelButton="False" HeaderText="操作" ShowEditButton="True">
<HeaderStyle Width="60px"></HeaderStyle>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
</asp:CommandField>
<asp:BoundField DataField="ORDERDATE" HeaderText="订单日期">
<HeaderStyle Width="80px"></HeaderStyle>
</asp:BoundField>
<asp:BoundField DataField="ORDERID" HeaderText="订单号">
<HeaderStyle Width="200px"></HeaderStyle>
</asp:BoundField>
</Columns>

<RowStyle BackColor="White" Height="28px"></RowStyle>

<HeaderStyle CssClass="gridheader"></HeaderStyle>

<AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
</asp:GridView> </asp:Panel> <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" OnPageChanging="pager_PageChanging" AlwaysShow="True" ShowPageIndex="False" ShowInputBox="Never"></NetPager:AspNetPager> </asp:Panel> <asp:Panel style="LEFT: 0px; POSITION: relative; TOP: 0px" id="pnlEdit" runat="server" Width="100%" Height="131px" Visible="False">
                    <TABLE class="OperationBar">
                        <TBODY>
                            <TR>
                                <TD style="WIDTH: 44px">
                                    <asp:Button id="btnUpdate" onclick="btnUpdate_Click" runat="server" CssClass="ButtonSave" Text="保存"></asp:Button> 
                                </TD>
                                <TD>
                                    <asp:Button id="btnCanel" onclick="btnCanel_Click" runat="server" CssClass="ButtonBack" Text="返回"></asp:Button> 
                                </TD>
                            </TR>
                        </TBODY>
                    </TABLE>
                    <table width="100%">
                        <tr>
                            <td class="tdTitle">
                                订单日期:</td>
                            <td ><asp:TextBox ID="txtOrderDate" runat="server" Width="168px" MaxLength="25" CssClass="TextBox"></asp:TextBox>
                            <input id="Button1"  runat ="server" class="ButtonDate" onclick="setday(document.getElementById('txtORDERDATE'))" type="button" />
                            </td>
                        </tr>
                        <tr >
                            <td class="tdTitle">
                                订单号:</td>
                            <td><asp:TextBox ID="txtOrderId" runat="server" Width="168px" MaxLength="25" CssClass="TextBox"></asp:TextBox></td>
                          
                        </tr>
                           
                   </table>
                </asp:Panel> </asp:Panel> 
</ContentTemplate>
</asp:UpdatePanel>
        &nbsp;&nbsp;&nbsp;&nbsp;
        
    </form>
</body>
</html>
