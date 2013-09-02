<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LineInfo.aspx.cs" Inherits="Code_BaseInfo_LineInfo" %>

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
                                        <asp:DropDownList ID="ddlField" runat="server">
                                            <asp:ListItem Value="LINECODE">分拣线代码</asp:ListItem>
                                            <asp:ListItem Value="LINENAME">分拣线名称</asp:ListItem>
                                        </asp:DropDownList><asp:TextBox ID="txtValue" runat="server" CssClass="TextBox" Height="20px"></asp:TextBox><asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click" Text="查询" />
                                        <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />
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
                                    <asp:BoundField DataField="LINECODE" HeaderText="分拣线代码">
                                    <HeaderStyle Width="100px"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LINENAME" HeaderText="分拣线名称"></asp:BoundField>
                                    <asp:BoundField DataField="LINETYPE" HeaderText="分拣线类型">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STATUS" HeaderText="状态">
                                    <HeaderStyle Width="80px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" />
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
                            <tr>
                                <td><asp:Button ID="btnUpdate" runat="server" CssClass="ButtonSave" OnClick="btnUpdate_Click" Text="保存" />
                                <td><asp:Button ID="btnCanel" runat="server" CssClass="ButtonBack" OnClick="btnCanel_Click" Text="返回" /></td>
                                <td width="100%"></td>
                            </tr>
                        </table>
                        <table width="100%">            
                            <tr >
                                <td class="tdTitle">分拣线代码:</td>
                                <td><asp:TextBox ID="txtLineCode" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle">分拣线名称:</td>
                                <td><asp:TextBox ID="txtLineName" MaxLength="20" runat="server" CssClass="TextBox" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle">
                                    分拣线类型:</td>
                                <td>
                                    <asp:TextBox ID="txtLineType" runat="server" CssClass="TextBox" MaxLength="20" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr >
                                <td class="tdTitle" ><font color="red">*</font>状态:</td>
                                <td>
                                    <asp:DropDownList ID="ddlStatus" runat="server">
                                        <asp:ListItem Value="1">启用</asp:ListItem>
                                        <asp:ListItem Value="0">停用</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>   
                        </table>
                    </asp:Panel> 
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
