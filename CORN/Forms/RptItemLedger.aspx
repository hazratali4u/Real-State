<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="RptItemLedger.aspx.cs" Inherits="Forms_RptItemLedger" Title="Item Ledger Report" %>
<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-9">
                            <div class="col-md-4">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                            </div>
                            <div class="col-md-9">
                            <div class="col-md-4">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Category</label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                                </div>
                                <div class="col-md-9">
                            <div class="col-md-4">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category</label>
                                <asp:DropDownList ID="ddlSubCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                                    </div>
                                    <div class="col-md-9">
                            <div class="col-md-4">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="display: none;">
                        <%--<asp:Panel ID="PanelFromDate" Visible="true" runat="server">--%>
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                        <%--</asp:Panel>--%>
                    </div>
                    <div class="row" style="display: none;">
                        <%--<asp:Panel ID="PanelToDate" Visible="true" runat="server">--%>
                        <div class="col-md-9">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>As On Date</label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass=" form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                            </div>
                        <%--</asp:Panel>--%>
                    </div>
                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                        TargetControlID="txtEndDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="row">
                <div class="col-md-9">
                <div class="col-md-4">
                    <div class="btnlist pull-left">
                        <asp:HiddenField ID="opType" runat="server" Value="0" />
                        <asp:Button ID="btnViewPDF" runat="server" OnClick="btnViewPDF_Click" Text="View PDF" CssClass=" btn btn-success" />
                        <asp:Button ID="btnViewExcel" runat="server" OnClick="btnViewExcel_Click" Text="View Excel" CssClass=" btn btn-success" />
                    </div>
                </div>
                    </div>
            </div>
        </div>
    </div>
</asp:Content>
