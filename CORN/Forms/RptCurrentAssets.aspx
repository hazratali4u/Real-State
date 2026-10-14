<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="RptCurrentAssets.aspx.cs" Inherits="Forms_RptCurrentAssets" Title="Current Assets Report" %>
<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row" style="height:20px;">
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Account Category</label>
                            <dx:ASPxComboBox ID="DrpAccountCategory" runat="server" AutoPostBack="true" SelectedIndex="0" CssClass="form-control" OnSelectedIndexChanged="DrpAccountCategory_SelectedIndexChanged">
                                <Items>
                                    <dx:ListEditItem Text="Balance Sheet Account" Selected="true" Value="0"></dx:ListEditItem>
                                    <dx:ListEditItem Text="Income Statment Account" Value="1"></dx:ListEditItem>
                                </Items>
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class=" row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Account Type</label>
                            <dx:ASPxComboBox ID="DrpMainType" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="DrpMainType_SelectedIndexChanged"></dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class=" row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Account Sub Type</label>
                            <dx:ASPxComboBox ID="DrpSubType" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DrpSubType_SelectedIndexChanged"></dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class=" row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Account Detail Type</label>
                            <dx:ASPxComboBox ID="DrpDetailType" runat="server" CssClass="form-control" AutoPostBack="True"></dx:ASPxComboBox>
                        </div>
                    </div>
                     <div class="row">
                        <div class="col-md-4">
                            <asp:CheckBox ID="ChkClosingBalance" runat="server" Text="  With Zero Closing Balance" Checked="true" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                            &nbsp;<asp:TextBox ID="txtStartDate" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>To Date</label>
                            &nbsp;<asp:TextBox ID="txtEndDate" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                        TargetControlID="txtEndDate"></cc1:CalendarExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row">
            <div class="col-md-offset-1 col-md-3 ">
                <div class="btnlist pull-right">
                    <asp:Button ID="btnViewPDF" runat="server" CssClass=" btn btn-success" Text="View PDF"
                        OnClick="btnViewPDF_Click" />
                    <asp:Button ID="btnViewExcel" runat="server" CssClass="btn btn-success" Text="View Excel"
                        OnClick="btnViewExcel_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
