<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="RptPartyBalances.aspx.cs" Inherits="Forms_RptPartyBalances" Title="Party Balances Report" %>

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
         <%--function SelectAllProjects() {
            var chkBoxList = document.getElementById('<%= LstProjects.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllProjects.ClientID %>');
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

        function UnCheckSelectAllProjects() {
            var chkBox = document.getElementById('<%= ChbAllProjects.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstProjects.ClientID %>');
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

        function SelectAllCategory() {
            var chkBoxList = document.getElementById('<%= LstCategory.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllCategory.ClientID %>');
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

        function UnCheckSelectAllCategory() {
            var chkBox = document.getElementById('<%= ChbAllCategory.ClientID %>');
           var chkBoxList = document.getElementById('<%= LstCategory.ClientID %>');
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

       function SelectAllSubCategory() {
           var chkBoxList = document.getElementById('<%= LstSubCategory.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllSubCategory.ClientID %>');
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

        function UnCheckSelectAllSubCategory() {
            var chkBox = document.getElementById('<%= ChbAllSubCategory.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstSubCategory.ClientID %>');
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
        }--%>

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
                            <dx:ASPxComboBox ID="ddlType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true">
                                <Items>
                                    <dx:ListEditItem Value="0" Text="All"></dx:ListEditItem>
                                    <dx:ListEditItem Value="1" Text="Customer"></dx:ListEditItem>
                                    <dx:ListEditItem Value="2" Text="Vendor"></dx:ListEditItem>
                                    <dx:ListEditItem Value="3" Text="Customer / Vendor"></dx:ListEditItem>
                                </Items>
                            </dx:ASPxComboBox>
                        </div>
                         </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="true">
                            </dx:ASPxComboBox>
                        </div>
                         </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span><asp:Label Text="" runat="server" ID="lblCustomer"></asp:Label></label>
                            <dx:ASPxComboBox ID="ddlCustomer" runat="server" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <asp:CheckBox ID="ChkClosingBalance" runat="server" Text="  With Zero Closing Balance" Checked="true" />
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                    <div class="col-md-3">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <asp:CheckBox ID="ChbAllProjects" onclick="SelectAllProjects()" runat="server" Text="All Projects"
                                    Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                            </div>
                            <div class="panel-body">
                                <div class="module_contents">
                                    <asp:Panel ID="Panel5" runat="server" Height="350px" ScrollBars="Vertical"
                                        BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                        <asp:CheckBoxList ID="LstProjects" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small" onclick="UnCheckSelectAllProjects()"
                                            CssClass="table-striped table-bordered table-hover table-condensed cf mtb mlbl" runat="server">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                        <div class="col-md-3">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:CheckBox ID="ChbAllCategory" onclick="SelectAllCategory()" OnCheckedChanged="ChbAllCategory_CheckedChanged" AutoPostBack="true" runat="server" Text="All Categories"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstCategory" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small"
                                                OnSelectedIndexChanged="LstCategory_SelectedIndexChanged" AutoPostBack="true" onclick="UnCheckSelectAllCategory()"
                                                CssClass="table-striped table-bordered table-hover table-condensed cf mtb mlbl" runat="server">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:CheckBox ID="ChbAllSubCategory" onclick="SelectAllSubCategory()" runat="server" Text="All Sub Categories"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel3" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstSubCategory" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small" onclick="UnCheckSelectAllSubCategory()"
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
