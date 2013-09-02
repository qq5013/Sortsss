<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Employee.aspx.cs" Inherits="Code_BaseInfo_Employee" %>

<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AspNetPager" Namespace="AspNetPager" TagPrefix="NetPager" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <link href="../../css/css.css?p=12" rel="Stylesheet" type="text/css" />
    <link href="../../css/op.css?l=0" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../../JScript/Check.js"></script>
    <script type="text/javascript" src="../../JScript/SelectDialog.js"></script>
    
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
                                        <asp:ListItem Value="EMPLOYEECODE">员工代码</asp:ListItem>
                                        <asp:ListItem Value="EMPLOYEENAME">员工姓名</asp:ListItem>
                                        <asp:ListItem Value="DEPARTMENTNAME">部门名称</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtValue" runat="server" CssClass="TextBox" Height="20px"></asp:TextBox>
                                    <asp:Button id="btnQuery" onclick="btnQuery_Click" runat="server" CssClass="ButtonQuery" Text="查询" />
                                    <asp:Button ID="btnAdd" runat="server" CssClass="ButtonCreate" Text="新增" OnClick="btnAdd_Click" />
                                    <asp:Button ID="btnDelete" runat="server" CssClass="ButtonDel" Text="删除" OnClick="btnDelete_Click" />
                                    <asp:Button ID="btnExit" runat="server" CssClass="ButtonExit" OnClick="btnExit_Click" Text="退出" />
                                </TD>
                            </TR>
                        </TBODY>
                    </TABLE>
                    <asp:Panel id="pnlGrid" runat="server" Width="100%" Height="460px">
                        <asp:GridView id="gvMain" runat="server" Width="100%" OnRowEditing="gvMain_RowEditing" AutoGenerateColumns="False" OnRowDataBound="gvMain_RowDataBound">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle Width="20px" />
                                </asp:TemplateField>
                                <asp:CommandField HeaderText="操作" InsertVisible="False" ShowCancelButton="False"
                                    ShowEditButton="True">
                                    <HeaderStyle Width="40px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:CommandField>
                                <asp:BoundField HeaderText="员工代码" DataField="EMPLOYEECODE">
                                    <HeaderStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="员工名称" DataField="EMPLOYEENAME">
                                    <HeaderStyle Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="部门代码" DataField="DEPARTMENTID">
                                    <HeaderStyle Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="部门名称" DataField="DEPARTMENTNAME">
                                    <HeaderStyle Width="180px" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="状态" DataField="STATUS">
                                    <HeaderStyle Width="60px" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="备注" DataField="REMARK" />
                            </Columns>

                            <RowStyle BackColor="White" Height="28px"></RowStyle>

                            <HeaderStyle CssClass="gridheader"></HeaderStyle>

                            <AlternatingRowStyle BackColor="#E8F4FF"></AlternatingRowStyle>
                        </asp:GridView> &nbsp; &nbsp;
                    </asp:Panel> 
                    <NetPager:AspNetPager id="pager" runat="server" Width="555px" Height="24px" ShowPageIndex="False" AlwaysShow="True" OnPageChanging="pager_PageChanging" ShowInputBox="Never"></NetPager:AspNetPager> 
                </asp:Panel> 
                <asp:Panel style="LEFT: 0px; POSITION: relative; TOP: 0px" id="pnlEdit" runat="server" Width="100%" Height="131px" Visible="False">
                    <table class="OperationBar">
                        <tr>
                            <td style="width: 32px"><asp:Button ID="btnUpdate" runat="server" CssClass="ButtonSave" OnClick="btnUpdate_Click"  Text="保存" /></td>
                            <td><asp:Button ID="btnCanel" runat="server" CssClass="ButtonBack" OnClick="btnCanel_Click" Text="返回" /></td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td class="tdTitle">员工代码:</td>
                            <td style="width: 48px"><asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="TextBox" MaxLength="12"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="tdTitle"><font color="red">*</font>员工姓名:</td>
                            <td style="width: 48px"><asp:TextBox ID="txtEmployeeName" runat="server" CssClass="TextBox" MaxLength="25"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="tdTitle"><font color="red">*</font>部门ID:</td>
                            <td style="width: 48px"><asp:TextBox ID="txtDepartmentID" runat="server" CssClass="TextBox" MaxLength="12"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="tdTitle"><font color="red">*</font>部门名称:</td>
                            <td style="width: 48px"><asp:TextBox ID="txtDepartmentName" runat="server" CssClass="TextBox" MaxLength="12"></asp:TextBox></td>
                            <td><asp:Button ID="Button2" runat="server" CssClass="ButtonBrowse" OnClientClick="return SelectDialog2('txtDEPARTMENTID,txtDEPARTMENTNAME','AS_BI_DEPARTMENT','DEPARTMENTID,DEPARTMENTNAME')" text="..." /></td>
                        </tr>
                        <tr>
                            <td class="tdTitle">在职情况:</td>
                            <td style="width: 48px">
                                <asp:DropDownList ID="ddlStatus" runat="server">
                                    <asp:ListItem Value="1">在职</asp:ListItem>
                                    <asp:ListItem Value="0">离职</asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td class="tdTitle" style="height: 91px">备注:</td>
                            <td style="width: 48px; height: 91px;"><asp:TextBox ID="txtRemark" runat="server" CssClass="MultiLineTextBox" MaxLength="100" TextMode="MultiLine"></asp:TextBox></td>
                        </tr>
                    </table>
                    
                </asp:Panel> 
            </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
