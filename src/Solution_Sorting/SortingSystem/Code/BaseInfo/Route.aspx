<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Route.aspx.cs" Inherits="Code_BaseInfo_Route" %>

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
                                        <asp:ListItem Value="ROUTECODE">线路代码</asp:ListItem>
                                        <asp:ListItem Value="ROUTENAME">线路名称</asp:ListItem>
                                        <asp:ListItem Value="AREACODE">区域代码</asp:ListItem>
                                        <asp:ListItem Value="AREANAME">区域名称</asp:ListItem>
                                        <asp:ListItem Value="DELIVERY">送货员</asp:ListItem>
                                    </asp:DropDownList><asp:TextBox ID="txtValue" runat="server" CssClass="TextBox" Height="20px"></asp:TextBox>
                                    <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click" Text="查询" />
                                    <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />
                                </TD>
                            </TR>
                        </TBODY>
                    </TABLE>
                    <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                        <asp:GridView id="gvMain" runat="server" Width="100%" AutoGenerateColumns="False" OnRowEditing="gvMain_RowEditing">
                            <Columns>
                                <asp:CommandField HeaderText="操作" ShowEditButton="True">
                                <HeaderStyle Width="40px" HorizontalAlign="Center"></HeaderStyle>

                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:CommandField>
                                <asp:BoundField DataField="ROUTECODE" HeaderText="线路代码">
                                <HeaderStyle Width="80px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ROUTENAME" HeaderText="线路名称"></asp:BoundField>
                                <asp:BoundField DataField="AREACODE" HeaderText="区域代码">
                                <HeaderStyle Width="80px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="AREANAME" HeaderText="区域名称">
                                    <HeaderStyle Width="180px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DELIVERY" HeaderText="配送员">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SORTID" HeaderText="顺序">
                                    <HeaderStyle Width="35px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="LINECODE" HeaderText="分拣线">
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
                    <asp:Panel ID="pnlEdit" runat="server" Height="131px" Style="left: 0px; position: relative;
                        top: 0px" Visible="False" Width="100%">
                        <table class="OperationBar">
                            <tr>
                                <td style="height: 25px">
                                    <asp:Button ID="btnUpdate" runat="server" CssClass="ButtonSave" OnClick="btnUpdate_Click"
                                        Text="保存" />
                                </td>
                                <td style="height: 25px">
                                    <asp:Button ID="btnCanel" runat="server" CssClass="ButtonBack" OnClick="btnCanel_Click"
                                        Text="返回" /></td>
                                <td style="height: 25px" width="100%">
                                </td>
                            </tr>
                        </table>
                        <table width="100%">
                            <tr>
                                <td class="tdTitle">
                                    线路代码:</td>
                                <td>
                                    <asp:TextBox ID="txtRouteCode" runat="server" CssClass="TextBox" MaxLength="12" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="tdTitle">
                                    线路名称:</td>
                                <td>
                                    <asp:TextBox ID="txtRouteName" runat="server" CssClass="TextBox" MaxLength="50" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="tdTitle">
                                    <font color="red">*</font>配送顺序:</td>
                                <td>
                                    <asp:TextBox ID="txtSortID" runat="server" CssClass="TextBox" MaxLength="10"></asp:TextBox></td>
                            </tr>
                                                        <tr>
                                <td class="tdTitle">
                                    <font color="red">*</font>分拣线:</td>
                                <td>
                                    <asp:TextBox ID="txtLineCode" runat="server" CssClass="TextBox" MaxLength="10"></asp:TextBox></td>
                            </tr>
                        </table>
                    </asp:Panel>
            </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
