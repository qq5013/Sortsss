<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Customer.aspx.cs" Inherits="Code_BaseInfo_Customer" %>

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
                                        <asp:ListItem Value="CUSTOMERNAME">客户名称</asp:ListItem>
                                        <asp:ListItem Value="CUSTOMERCODE">客户代码</asp:ListItem>
                                        <asp:ListItem Value="ROUTECODE">线路代码</asp:ListItem>
                                        <asp:ListItem Value="ROUTENAME">线路名称</asp:ListItem>
                                        <asp:ListItem Value="LICENSENO">专卖证号</asp:ListItem>
                                    </asp:DropDownList><asp:TextBox ID="txtValue" runat="server" CssClass="TextBox"></asp:TextBox>
                                    <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click" Text="查询" />
                                    <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />
                                </TD>
                            </TR>
                        </TBODY>
                    </TABLE>
                    <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                        <asp:GridView id="gvMain" runat="server" Width="100%" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="CUSTOMERCODE" HeaderText="客户代码">
                                    <HeaderStyle Width="80px"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="CUSTOMERNAME" HeaderText="客户名称">
                                    <HeaderStyle Width="180px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ROUTECODE" HeaderText="线路代码">
                                    <HeaderStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ROUTENAME" HeaderText="线路名称">
                                    <HeaderStyle Width="180px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="LICENSENO" HeaderText="专卖证号">
                                    <HeaderStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SORTID" HeaderText="顺序">
                                    <HeaderStyle Width="35px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ADDRESS" HeaderText="送货地址" />
                                <asp:BoundField DataField="TELNO" HeaderText="联系电话">
                                    <HeaderStyle Width="80px" />
                                </asp:BoundField>
                            </Columns>

                            <RowStyle BackColor="White" Height="28px"></RowStyle>

                            <HeaderStyle CssClass="gridheader"></HeaderStyle>

                            <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                        </asp:GridView> 
                    </asp:Panel> 
                    <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pager_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager> 
                </asp:Panel> &nbsp;
            </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
