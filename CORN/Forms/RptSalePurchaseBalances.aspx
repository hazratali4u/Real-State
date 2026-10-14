<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="RptSalePurchaseBalances.aspx.cs" Inherits="Forms_RptSalePurchaseBalances" Title="Balances Report" %>
<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }
        function SelectAllBalanceTypes() {
            var chkBoxList = document.getElementById('<%= LstBalanceTypes.ClientID %>');
             var chkBox = document.getElementById('<%= ChbAllBalanceTypes.ClientID %>');
             if (chkBox.checked == true) {
                 var chkBoxCount = chkBoxList.getElementsByTagName("input");

                 for (var i = 0; i < chkBoxCount.length; i++) {
                     chkBoxCount[i].checked = true;
                 }
             }
             else {
                 var chkBoxCount = chkBoxList.getElementsByTagName("input");

                 for (var i = 0; i < chkBoxCount.length; i++) {
                     chkBoxCount[i].checked = false;
                 }
             }
         }
        function UnCheckSelectAllBalanceTypes() {
             var chkBox = document.getElementById('<%= ChbAllBalanceTypes.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstBalanceTypes.ClientID %>');
            var chkBoxCount = chkBoxList.getElementsByTagName("input");
            var count = 0;
            for (var i = 0; i < chkBoxCount.length; i++) {
                if (chkBoxCount[i].checked == false) {
                    count += 1;
                }
            }
            if (count > 0) {
                chkBox.checked = false;
            }
            else {
                chkBox.checked = true;
            }
        }
    </script>

    <div class="main-contents">
        <div class="container employee-infomation">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="z-index: 101; left: 50%; top: 50%; position: absolute; height: 100px;"
                        id="DIV1">
                        <asp:UpdateProgress ID="UpdateProgress5" runat="server">
                            <ProgressTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif"></asp:ImageButton>&nbsp; Loading....
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Report Type</label>
                            <dx:ASPxComboBox ID="ddlType" runat="server" CssClass="form-control">
                                <Items>
                                    <dx:ListEditItem Value="1" Selected="true" Text="Sales Balances"></dx:ListEditItem>
                                    <dx:ListEditItem Value="2" Text="Purchase Balances"></dx:ListEditItem>
                                    <dx:ListEditItem Value="3" Text="Combine Balances"></dx:ListEditItem>
                                </Items>
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                     <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox ID="ddlLocation" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Patry</label>
                            <dx:ASPxComboBox ID="ddlParty" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:CheckBox ID="ChbAllBalanceTypes" onclick="SelectAllBalanceTypes()" runat="server" Text="All Balance Types"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel1" runat="server" Height="250px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstBalanceTypes" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small" onclick="UnCheckSelectAllBalanceTypes()"
                                                CssClass="table-striped table-bordered table-hover table-condensed cf mtb mlbl" runat="server">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Panel ID="PanelFromDate" Visible="true" runat="server">
                            <div class="col-md-3">
                                <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                            </div>
                            <div class="col-md-1" style="margin-top: 27px">
                                <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                    Width="30px" />
                            </div>
                        </asp:Panel>
                    </div>
                    <div class="row">
                        <asp:Panel ID="PanelToDate" Visible="true" runat="server">
                            <div class="col-md-3">
                                <label><span class="fa fa-caret-right rgt_cart"></span>To Date</label>
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass=" form-control" MaxLength="10"></asp:TextBox>
                            </div>
                            <div class="col-md-1" style="margin-top: 27px">
                                <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                    Width="30px" />
                            </div>
                        </asp:Panel>
                    </div>
                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                        TargetControlID="txtEndDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <div class="col-md-3 ">
                    <div class="btnlist pull-left">
                        <asp:HiddenField ID="opType" runat="server" Value="0" />
                        <asp:Button ID="btnViewPDF" OnClick="btnViewPDF_Click" Text="View PDF" CssClass=" btn btn-success" runat="server" />
                        <asp:Button ID="btnViewExcel" OnClick="btnViewExcel_Click" Text="View Excel" CssClass=" btn btn-success" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
