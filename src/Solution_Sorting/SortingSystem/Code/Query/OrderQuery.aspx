<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderQuery.aspx.cs" Inherits="Code_Query_OrderQuery" %>

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
                                    <TD borderColor=#cccc99> &nbsp; 日期：
                                    <asp:TextBox ID="txtOrderDate" runat="server" CssClass="TextBox" Width="80px" onpropertychange="GetBatchNo();"></asp:TextBox>
                                    <input id="btnDate" class="ButtonDate" onclick="setday(document.getElementById('txtORDERDATE'))" type="button" />
                                        批次：
                                        <asp:DropDownList ID="ddlBatchNo" runat="server">
                                            <asp:ListItem Selected="True">1</asp:ListItem>
                                        </asp:DropDownList>
                                        
                                        <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click"
                                            OnClientClick="return CheckCondition();" Text="查询" /><asp:Button ID="btnExit" runat="server"
                                                CssClass="ButtonExit" OnClientClick="return Exit();" Text="退出" OnClick="btnExit_Click" />
                                        <asp:LinkButton ID="lnkBtnGetBatchNo" runat="server" OnClick="lnkBtnGetBatchNo_Click"></asp:LinkButton>
                                        <asp:TextBox ID="txtCusCode" runat="server" CssClass="TextBox"></asp:TextBox>
                                        <asp:Button ID="btnQueryCust" runat="server" CssClass="ButtonQuery" OnClick="btnQueryCust_Click"
                                            OnClientClick="return CheckCondition();" Text="按户查询" /></TD>
                                </TR>
                            </TBODY>
                        </TABLE>
                        <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                            <asp:GridView id="gvMain" runat="server" Width="100%" OnRowEditing="gvMain_RowEditing" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:CommandField HeaderText="操作" InsertVisible="False" ShowCancelButton="False"
                                        ShowEditButton="True" EditText="明细">
                                        <HeaderStyle Width="50px" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:CommandField>
                                    <asp:BoundField DataField="ROUTECODE" HeaderText="线路代码">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ROUTENAME" HeaderText="线路名称" >
                                        <HeaderStyle Width="150px" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="客户代码" DataField="CUSTOMERCODE" >
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="客户名称" DataField="CUSTOMERNAME" />
                                    <asp:BoundField DataField="ORDERID" HeaderText="订单号">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BATCHNO" HeaderText="批次号" />
                                    <asp:BoundField DataField="ORDERDATE" HeaderText="订单日期" />
                                </Columns>

                                <RowStyle BackColor="White" Height="28px"></RowStyle>

                                <HeaderStyle CssClass="gridheader"></HeaderStyle>

                                <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                            </asp:GridView> 
                        </asp:Panel> 
                        <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pager_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager> 
                    </asp:Panel> 
                    <asp:Panel ID="pnlDetail" runat="server" Height="464px" Width="100%" Visible=false>
                        <asp:Panel style="LEFT: 0px; POSITION: relative; TOP: 0px" id="pnlEdit" runat="server" Width="100%" Height="460px">
                            <table class="OperationBar" >
                                <tr>
                                    <td style="height: 25px; width: 64px;"><asp:Button ID="btnCanel" CssClass="ButtonBack" runat="server" Text="返回" OnClick="btnCanel_Click"  /></td>
                                    <td width=64></td>
                                    <td width="100%" style="height: 25px"></td>
                                </tr>
                             </table>
                                <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="ORDERID" HeaderText="订单号" >
                                            <HeaderStyle Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CIGARETTECODE" HeaderText="卷烟代码" >
                                            <HeaderStyle Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CIGARETTENAME" HeaderText="卷烟名称" />
                                        <asp:BoundField DataField="QUANTITY" HeaderText="数量" >
                                            <ItemStyle HorizontalAlign="Right" />
                                            <HeaderStyle Width="40px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="JQUANTITY" HeaderText="件数"  >
                                            <ItemStyle HorizontalAlign="Right" />
                                            <HeaderStyle Width="40px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TQUANTITY" HeaderText="条数"  >
                                            <ItemStyle HorizontalAlign="Right" />
                                            <HeaderStyle Width="40px" />
                                        </asp:BoundField>
                                    </Columns>
                                    <RowStyle BackColor="White" Height="28px" />
                                    <HeaderStyle CssClass="gridheader" />
                                    <AlternatingRowStyle BackColor="#E8F4FF" />
                                </asp:GridView>
                            
                            </asp:Panel> 
                        <NetPager:AspNetPager id="pagerDetail" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pagerDetail_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager>
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
