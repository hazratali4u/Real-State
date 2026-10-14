<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="RptGainOrLossReport.aspx.cs" Inherits="Forms_RptGainOrLossReport" Title="Gain Or Loss On Sale" %>

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
         <%--function SelectAll() {
            var chkBoxList = document.getElementById('<%= LstCustomer.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllCustomer.ClientID %>');
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

        function UnCheckSelectAll() {
            var chkBox = document.getElementById('<%= ChbAllCustomer.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstCustomer.ClientID %>');
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

        function SelectAllCategory() {
            var chkBoxList = document.getElementById('<%= LstBlock.ClientID %>');
           var chkBox = document.getElementById('<%= ChbAllBlock.ClientID %>');
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
           var chkBox = document.getElementById('<%= ChbAllBlock.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstBlock.ClientID %>');
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
        }

        function SelectAllPlots() {
            var chkBoxList = document.getElementById('<%= LstPlots.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllPlots.ClientID %>');
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

        function UnCheckSelectAllPlots() {
            var chkBox = document.getElementById('<%= ChbAllPlots.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstPlots.ClientID %>');
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

        function SelectAllMeasurement() {
            var chkBoxList = document.getElementById('<%= LstMeasurement.ClientID %>');
            var chkBox = document.getElementById('<%= ChbAllMeasurement.ClientID %>');
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

        function UnCheckSelectAllMeasurement() {
            var chkBox = document.getElementById('<%= ChbAllMeasurement.ClientID %>');
            var chkBoxList = document.getElementById('<%= LstMeasurement.ClientID %>');
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
                            <label><span class="fa fa-caret-right rgt_cart"></span>Sale Type</label>
                            <dx:ASPxComboBox ID="ddlType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true">
                                <Items>
                                    <dx:ListEditItem Value="1" Text="Normal Sale"></dx:ListEditItem>
                                    <dx:ListEditItem Value="2" Text="Installment Sale"></dx:ListEditItem>
                                </Items>
                            </dx:ASPxComboBox>
                        </div>
                        <div class="col-md-3">
                            <asp:RadioButtonList ID="Hp" RepeatDirection="Horizontal" Visible="false" runat="server" TextAlign="Right" OnSelectedIndexChanged="Hp_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="Sale" Value="1" Selected="True" style="margin-right: 22px;"></asp:ListItem>
                                <asp:ListItem Text="Insallment Sale" Value="2"></asp:ListItem>
                            </asp:RadioButtonList>

                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                            <dx:ASPxComboBox ID="DrpProjects" runat="server" CssClass="form-control" OnSelectedIndexChanged="DrpProjects_SelectedIndexChanged" AutoPostBack="true">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <%--<div class="col-md-3" style="display:none;">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:CheckBox ID="ChbAllCustomer" onclick="SelectAll()" runat="server" Text=" All Customers"
                                         Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight:300 !important;">
                                            <asp:CheckBoxList ID="LstCustomer" Style="font-weight:300 !important;" Width="100%" Font-Bold="false" Font-Size="Small" onclick="UnCheckSelectAll()"
                                                CssClass="table-striped table-bordered table-hover table-condensed cf mtb mlbl" runat="server">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                        </div>--%>
                        <div class="col-md-3">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <asp:CheckBox ID="ChbAllSubCategory" onclick="SelectAllSubCategory()" OnCheckedChanged="ChbAllSubCategory_CheckedChanged" AutoPostBack="true" runat="server" Text="All Categories"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel3" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstSubCategory" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small"
                                                OnSelectedIndexChanged="ChbAllSubCategory_CheckedChanged" AutoPostBack="true" onclick="UnCheckSelectAllSubCategory()"
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
                                    <asp:CheckBox ID="ChbAllMeasurement" onclick="SelectAllMeasurement()" OnCheckedChanged="ChbAllMeasurement_CheckedChanged" AutoPostBack="true" runat="server" Text="All Measurements"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel5" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstMeasurement" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small"
                                                OnSelectedIndexChanged="ChbAllMeasurement_CheckedChanged" AutoPostBack="true" onclick="UnCheckSelectAllMeasurement()"
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
                                    <asp:CheckBox ID="ChbAllBlock" onclick="SelectAllCategory()" OnCheckedChanged="ChbAllBlock_CheckedChanged" AutoPostBack="true" runat="server" Text="All Blocks & Streets"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel2" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstBlock" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small"
                                                OnSelectedIndexChanged="LstBlock_SelectedIndexChanged" AutoPostBack="true" onclick="UnCheckSelectAllCategory()"
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
                                    <asp:CheckBox ID="ChbAllPlots" onclick="SelectAllPlots()" runat="server" Text="All Plots, Houses & Offices"
                                        Font-Bold="true" ForeColor="Black"></asp:CheckBox>
                                </div>
                                <div class="panel-body">
                                    <div class="module_contents">
                                        <asp:Panel ID="Panel4" runat="server" Height="350px" ScrollBars="Vertical"
                                            BorderWidth="0px" BorderStyle="Groove" BorderColor="Silver" Style="font-weight: 300 !important;">
                                            <asp:CheckBoxList ID="LstPlots" Style="font-weight: 300 !important;" Width="100%" Font-Bold="false" Font-Size="Small" onclick="UnCheckSelectAllPlots()"
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
                    <%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
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
