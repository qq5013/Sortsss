<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AbnormalSupplyQuery.aspx.cs" Inherits="Code_Query_AbnormalSupplyQuery" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
    <script language="JavaScript" type="text/javascript" src="../../JScript/setday9.js"></script>
    <script language="JavaScript" type="text/javascript" src="../../JScript/Check.js"></script>
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
                                    <TD borderColor=#cccc99 style="height: 30px"> &nbsp; 日期:
                                        <asp:TextBox ID="txtOrderDate" runat="server" CssClass="TextBox" Width="90px" onpropertychange="GetBatchNo();"></asp:TextBox>&nbsp;<input id="btnDate" class="ButtonDate" type="button" onclick="setday(document.getElementById('txtOrderDate')) "/>
                                        &nbsp;
                                        &nbsp;
                                        &nbsp; &nbsp;&nbsp; 批次:<asp:DropDownList ID="ddlBatchNo" runat="server">
                                            <asp:ListItem Selected="True">1</asp:ListItem>
                                        </asp:DropDownList>&nbsp;
                                        <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click"
                                            OnClientClick="return CheckCondition();" Text="查询" /><asp:Button ID="btnExit" runat="server"
                                                CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />&nbsp;
                                        <asp:LinkButton ID="lnkBtnGetBatchNo" runat="server" OnClick="lnkBtnGetBatchNo_Click"></asp:LinkButton></TD>
                                </TR>
                            </TBODY>
                        </TABLE>
                        <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                            <asp:GridView id="gvMain" runat="server" Width="100%" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:BoundField DataField="ORDERDATE" HeaderText="分拣日期" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="False" >
                                    <HeaderStyle Width="60px"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BATCHNO" HeaderText="批次">
                                        <HeaderStyle Width="50px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LINECODE" HeaderText="分拣线代码">
                                    <HeaderStyle Width="80px"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CIGARETTECODE" HeaderText="卷烟代码">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CIGARETTENAME" HeaderText="卷烟名称">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="QUANTITY" HeaderText="数量">
                                        <ItemStyle HorizontalAlign="Right" />
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>
                                </Columns>

                                <RowStyle BackColor="White" Height="28px"></RowStyle>

                                <HeaderStyle CssClass="gridheader"></HeaderStyle>

                                <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                            </asp:GridView> 
                        </asp:Panel> 
                        <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pager_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager> 
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
