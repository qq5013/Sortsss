<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cigarette.aspx.cs" Inherits="Code_BaseInfo_Cigarette" %>

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
                        <table style="height: 30px" bordercolor="#111111" cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tbody>
                                <tr>
                                    <td bordercolor=#cccc99> 
                                        <asp:DropDownList ID="ddlField" runat="server">
                                            <asp:ListItem Value="CIGARETTECODE">卷烟代码</asp:ListItem>
                                            <asp:ListItem Value="CIGARETTENAME">卷烟名称</asp:ListItem>
                                            <asp:ListItem Value="BARCODE">条形码</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtValue" runat="server" CssClass="TextBox" Height="20px"></asp:TextBox>
                                        <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click" Text="查询" />
                                        <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                            <asp:GridView id="gvMain" runat="server" Width="100%" OnRowEditing="gvMain_RowEditing" AutoGenerateColumns="False" OnRowDataBound="gvMain_RowDataBound">
                                <Columns>
                                    <asp:CommandField HeaderText="操作" ShowEditButton="True">
                                    <HeaderStyle Width="40px" HorizontalAlign="Center"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:CommandField>
                                    <asp:BoundField DataField="CIGARETTECODE" HeaderText="卷烟代码">
                                    <HeaderStyle Width="80px"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CIGARETTENAME" HeaderText="卷烟名称"></asp:BoundField>
                                    <asp:BoundField DataField="ISABNORMITY" HeaderText="异形烟">
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BARCODE" HeaderText="条码" HtmlEncode="False" NullDisplayText=" ">
                                        <HeaderStyle Width="100px" />
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
                        <table class="operationbar">
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:Button id="btnUpdate" onclick="btnUpdate_Click" runat="server" CssClass="ButtonSave" Text="保存"></asp:Button> 
                                    </td>
                                    <td>
                                        <asp:Button id="btnCanel" onclick="btnCanel_Click" runat="server" CssClass="ButtonBack" Text="返回"></asp:Button>
                                    </td>
                                    <td width="100%">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <table width="100%">
                            <tr >
                                <td class="tdTitle">卷烟代码:</td>
                                <td><asp:TextBox ID="txtCigaretteCode" MaxLength="12" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle">卷烟名称:</td>
                                <td ><asp:TextBox ID="txtCigaretteName" MaxLength="50" runat="server" CssClass="TextBox"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle">异型烟:</td>
                                <td><asp:DropDownList ID="ddlIsAbnormity" runat="server">
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                    <asp:ListItem Value="1">是</asp:ListItem>
                                </asp:DropDownList></td>
                            </tr>
                            <tr >
                                <td class="tdTitle" style="height: 26px">条形码:</td>
                                <td style="height: 26px">
                                    <asp:TextBox ID="txtBarcode" runat="server" CssClass="TextBox"></asp:TextBox></td>
                            </tr>
                    </asp:Panel> 
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
