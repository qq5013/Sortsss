<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Area.aspx.cs" Inherits="Code_BaseInfo_Area" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
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
                                           <asp:ListItem Value="AREACODE">区域代码</asp:ListItem>
                                           <asp:ListItem Value="AREANAME">区域名称</asp:ListItem>
                                         </asp:DropDownList> 
                                         <asp:TextBox id="txtValue" runat="server" Height="20px" CssClass="TextBox"></asp:TextBox> 
                                         <asp:Button id="btnQuery" onclick="btnQuery_Click" runat="server" CssClass="ButtonQuery" Text="查询"></asp:Button> 
                                         <asp:Button id="btnExit" onclick="btnExit_Click" runat="server" CssClass="ButtonExit" Text="退出"></asp:Button> 
                                    </TD>
                                </TR>
                            </TBODY>
                        </TABLE>
                        <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                            <asp:GridView id="gvMain" runat="server" Width="100%" OnRowEditing="gvMain_RowEditing" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:CommandField HeaderText="操作" ShowEditButton="True">
                                    <HeaderStyle Width="40px" HorizontalAlign="Center"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:CommandField>
                                    <asp:BoundField DataField="AREACODE" HeaderText="区域代码">
                                    <HeaderStyle Width="100px"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AREANAME" HeaderText="区域名称"></asp:BoundField>
                                    <asp:BoundField DataField="SORTID" HeaderText="配送顺序">
                                    <HeaderStyle Width="80px"></HeaderStyle>
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
                        <table class="OperationBar" >
                            <tr>
                                <td style="height: 25px"><asp:Button ID="btnUpdate" CssClass="ButtonSave" runat="server" Text="保存" OnClick="btnUpdate_Click" /> </td>
                                <td style="height: 25px"><asp:Button ID="btnCanel" CssClass="ButtonBack" runat="server" Text="返回" OnClick="btnCanel_Click"  /></td>
                                <td width="100%" style="height: 25px"></td>
                            </tr>
                         </table>
                        <table width="100%">
                            <tr >
                                <td class="tdTitle">区域代码:</td>
                                <td><asp:TextBox ID="txtAreaCode" MaxLength="12" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle"> 区域名称:</td>
                                <td><asp:TextBox ID="txtAreaName" MaxLength="50" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle"><font color="red">*</font>配送顺序:</td>
                                <td><asp:TextBox MaxLength="10" ID="txtSortID" runat="server" CssClass="TextBox"></asp:TextBox></td>
                            </tr>
                        </table>
                    </asp:Panel> 
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
