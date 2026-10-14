<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="RptCustomerWiseSalesReport.aspx.cs" Inherits="Forms_RptCustomerWiseSalesReport" Title="Customer Wise Sales Report" %>

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
                    <div class="row" style="display: none;">
                        <div class="col-md-4">
                            <asp:RadioButtonList ID="ReportType" RepeatDirection="Horizontal" runat="server" TextAlign="Right">
                                <asp:ListItem Text="Sale Insallment" Value="1" Selected="True" style="margin-right: 22px;"></asp:ListItem>
                                <asp:ListItem Text="Purchase Insallment" Value="2"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <asp:DropDownList ID="drpDistributor" runat="server" CssClass="form-control"></asp:DropDownList>

                            <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                            <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            
                            <label><span class="fa fa-caret-right rgt_cart"></span>Cusromer</label>
                            <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSupplier_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            
                            <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Type</label>
                            <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="form-control" 
                                OnSelectedIndexChanged="ddlTransactionType_SelectedIndexChanged" AutoPostBack="true">
                                <Items>
                                    <asp:ListItem Value="1" Text="Sale"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Sale return"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="Sale Installment"></asp:ListItem>
                                </Items>
                            </asp:DropDownList>
                            
                            <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                            <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                            
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <div class="row" style="display: none;">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>To Date</label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass=" form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                        TargetControlID="txtEndDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <div class="col-md-offset-1 col-md-3 ">
                    <div class="btnlist pull-right">
                        <asp:HiddenField ID="opType" runat="server" Value="0" />
                        <asp:Button ID="btnViewPDF" runat="server" OnClick="btnViewPDF_Click" Text="View PDF" CssClass=" btn btn-success" />
                        <asp:Button ID="btnViewExcel" runat="server" OnClick="btnViewExcel_Click" Text="View Excel" CssClass=" btn btn-success" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>