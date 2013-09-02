<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Batch.aspx.cs" Inherits="Code_SortingManage_Batch" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../../JScript/Check.js"></script>
</head>
<body style="margin-top:0px; margin-left:0px">
    <form id="form1" runat="server">
        <asp:ScriptManager id="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlMain" runat="server" Height="100%" Style="left: 0px; position: relative;
                    top: 0px" Width="100%">
                    <asp:Panel ID="pnlList" runat="server" Height="401px" Width="100%">
                        <table border="0" bordercolor="#111111" cellpadding="0" cellspacing="0" style="height: 30px"
                            width="100%">
                            <tobdy></tobdy>
                            <tbody>
                                <tr>
                                    <td style="border-left-color: #cccc99; border-bottom-color: #cccc99; border-top-color: #cccc99;
                                        border-right-color: #cccc99">
                                        <asp:DropDownList ID="ddlField" runat="server">
                                            <asp:ListItem Value="ORDERDATE">订单日期</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtValue" runat="server" CssClass="TextBox" Height="20px"></asp:TextBox>
                                        <asp:Button ID="btnQuery" runat="server" CssClass="ButtonQuery" OnClick="btnQuery_Click"
                                            Text="查询" />&nbsp;
                                        <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click"
                                            Text="退出" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <asp:Panel ID="pnlGrid" runat="server" Height="460px" Width="100%">
                            <asp:GridView ID="gvMain" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvMain_RowDataBound"
                                OnRowDeleting="gvMain_RowDeleting" Width="100%" OnRowEditing="gvMain_RowEditing">
                                <Columns>
                                    <asp:CommandField DeleteText="清除优化" HeaderText="操作" ShowCancelButton="False" ShowDeleteButton="True" ShowEditButton="True">
                                        <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:CommandField>
                                    <asp:BoundField DataField="ORDERDATE" DataFormatString="{0:yyyy-MM-dd}" HeaderText="订单日期"
                                        HtmlEncode="False">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BATCHNO" HeaderText="批次">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <HeaderStyle Width="60px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ISUPTONOONEPRO" HeaderText="是否上传">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ISVALID" HeaderText="是否优化">
                                        <HeaderStyle Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="EXECUTEUSER" HeaderText="操作员" />
                                    <asp:BoundField DataField="EXECUTEIP" HeaderText="操作IP">
                                        <HeaderStyle Width="100px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NO1BATCH" HeaderText="一号工程批次号" />
                                </Columns>
                                <RowStyle BackColor="White" Height="28px" />
                                <HeaderStyle CssClass="gridheader" />
                                <AlternatingRowStyle BackColor="#E8F4FF" />
                            </asp:GridView>
                        </asp:Panel>
                        <NetPager:AspNetPager ID="pager" runat="server" AlwaysShow="True" Height="24px" OnPageChanging="pager_PageChanging" ShowInputBox="Never" ShowPageIndex="False" Width="555px"></NetPager:AspNetPager>
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
                                    订单日期:</td>
                                <td>
                                    <asp:TextBox ID="txtOrderDate" runat="server" CssClass="TextBox" MaxLength="12" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="tdTitle">
                                    分拣批次:</td>
                                <td>
                                    <asp:TextBox ID="txtSortBatch" runat="server" CssClass="TextBox" MaxLength="50" ReadOnly="True"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="tdTitle">
                                    <font color="red">*</font>一号工程批次号:</td>
                                <td>
                                    <asp:TextBox ID="txtNO1Batch" runat="server" CssClass="TextBox" MaxLength="10"></asp:TextBox></td>
                            </tr>
                        </table>
                        <table width="100%">
                            <tr>
                                <td class="tdTitle">
                                    <font color="red">*</font>一号工程状态值:</td>
                                <td>
                                    <asp:DropDownList ID="DDLNo1State" runat="server" CssClass="TEXTBOX">
                                        <asp:ListItem Value="1">已上传</asp:ListItem>
                                        <asp:ListItem Value="0">未上传</asp:ListItem>
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
