<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddSale.aspx.cs" Inherits="Forms_frmAddSale" Title="Sales" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/ValidateDotsAndNumbers.js"></script>
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }

        function MyFunction() {
            var txtSaleAmount = document.getElementById('<%=txtSaleAmount.ClientID%>').value;
            var txtDiscount = document.getElementById('<%=txtDiscount.ClientID%>').value;
            var txtTokenAmount = document.getElementById('<%=txtTokenAmount.ClientID%>').value;
            var txtDownPayment = document.getElementById('<%=txtDownPayment.ClientID%>').value;
            var txtBalance = document.getElementById('<%=txtBalance.ClientID%>').value;
            if (txtSaleAmount == "")
                document.getElementById('<%=txtSaleAmount.ClientID%>').value = 0;
            if (txtDiscount == "")
                document.getElementById('<%=txtDiscount.ClientID%>').value = 0;
            if (txtTokenAmount == "")
                document.getElementById('<%=txtTokenAmount.ClientID%>').value = 0;
            if (txtDownPayment == "")
                document.getElementById('<%=txtDownPayment.ClientID%>').value = 0;
            if (txtBalance == "")
                document.getElementById('<%=txtBalance.ClientID%>').value = 0;
            var result = parseInt(txtSaleAmount) - parseInt(txtDiscount) - parseInt(txtTokenAmount) - parseInt(txtDownPayment);
            if (!isNaN(result)) {
                document.getElementById('<%=txtBalance.ClientID%>').value = result;
            }
            else {
                document.getElementById('<%=txtBalance.ClientID%>').value = 0;
            }
        }
        function operation() {
            var txtCommissionValue = document.getElementById('<%=txtCommissionValue.ClientID%>').value;
            var txtCommisionAmount = document.getElementById('<%=txtCommisionAmount.ClientID%>').value;
            if (txtCommissionValue == "")
                document.getElementById('<%=txtCommissionValue.ClientID%>').value = 0;
            if (txtCommisionAmount == "")
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = 0;
            var result = parseInt(txtCommissionValue);
            if (!isNaN(result)) {
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = result;
            }
            else {
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = 0;
            }
        }

        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        function ValidateForm() {
            var str;
            str = document.getElementById('<%=ddlProject.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Project name is required.');
                return false;
            }

            <%--  str = document.getElementById('<%=txtItemName.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Item Name is required.');
            return false;
        }--%>

            <%--  str = document.getElementById('<%=txtStreetNo.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Street no is required');
            return false;
        }
        str = document.getElementById('<%=txtBlockNo.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Block no is required');
            return false;
        }--%>
        <%--str = document.getElementById('<%=txtpassword.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Password is required.');
            return false;
        }--%>
            return true;
        }


        function onCalendarShown3() {
            var cal = $find("calendar3");
            cal._switchMode("months", true);
            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", call3);
                    }
                }
            }
        }

        function onCalendarHidden3() {
            var cal = $find("calendar3");

            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", call3);
                    }
                }
            }
        }

        function call3(eventElement) {
            var target = eventElement.target;
            switch (target.mode) {
                case "month":
                    var cal = $find("calendar3");
                    cal._visibleDate = target.date;
                    cal.set_selectedDate(target.date);
                    cal._switchMonth(target.date);
                    cal._blur.post(true);
                    cal.raiseDateSelectionChanged();
                    break;
            }
        }

        function Check_Click(objRef) {
            var row = objRef.parentNode.parentNode;
            var GridView = row.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var headerCheckBox = inputList[0];
                var checked = true;
                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                    if (!inputList[i].checked) {
                        checked = false;
                        break;
                    }
                }
            }

            headerCheckBox.checked = checked;
        }

        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;
                    }
                    else {
                        inputList[i].checked = false;
                    }
                }
            }
        }
        function onlyDotsAndNumbers(txt, event) {
            var charCode = (event.which) ? event.which : event.keyCode;

            if (charCode == 9 || charCode == 8) {
                return true;
            }
            if (charCode == 46) {
                if (txt.value.indexOf(".") < 0)
                    return true;
                return false;
            }
            if (charCode == 31 || charCode < 48 || charCode > 57)
                return false;
            return true;
        }



    </script>
    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <ContentTemplate>
            <div class="col-md-7" style="padding-bottom: 15px;">
                <asp:ImageButton ID="AddPopUp" runat="server" Style="padding-bottom: 0px;" ImageUrl="~/images/add-png.png" ToolTip="Add New Sale" Width="30px" OnClick="AddPopUp_Click" />
            </div>
            <asp:Panel ID="Panel3" runat="server" Visible="true">
                <div class="col-md-4" style="text-align: right;">
                    <div class="search">
                        <asp:TextBox ID="txtSearch" runat="server" Style="margin-left: 48px;" placeholder="Search" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-1" style="text-align: right;">
                    <asp:LinkButton ID="btnsearch" OnClick="btnsearch_Click" runat="server" Text="Search" Style="margin-left: -48px;"
                        CssClass="btn btn-success"><i class="fa fa-search"  style="font-size:20px;"></i></asp:LinkButton>
                </div>
            </asp:Panel>
            <br />
            <div class="main-contents">
                <div class="container employee-infomation">
                    <div class="row top" style="margin-left: 15px !important; margin-right: 15px !important;">
                        <div class="emp-table">
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="PanelSale" runat="server">
                                            <div class="row" style="display: none;">
                                                <div class="col-md-12" style="width: 75% !important; margin-left: 15px;">
                                                    <asp:RadioButtonList ID="Hp" RepeatDirection="Horizontal" runat="server" TextAlign="Right" OnSelectedIndexChanged="Hp_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Text="Normal Sale" Value="1" Selected="True" style="margin-right: 22px;"></asp:ListItem>
                                                        <asp:ListItem Text="Insallment Sale" Value="2"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-3">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Date</label>
                                                        <asp:TextBox ID="txtPurchaseDate" Style="width: 200px !important;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-1" style="margin-top: 30px">
                                                        <asp:ImageButton ID="ibtnPurchaseDate" Style="margin-left: 10px; margin-top: -3px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                            Width="30px" />
                                                        <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                        <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnPurchaseDate"
                                                            TargetControlID="txtPurchaseDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                                        <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlLocation" runat="server" CssClass="form-control">
                                    </dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                        <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlProject" runat="server" CssClass="form-control">
                                    </dx:ASPxComboBox>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Block</label>
                                                        <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Street</label>
                                                        <asp:DropDownList ID="ddlStreet" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlStreet_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                                        <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                                                        <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlVendor" runat="server" CssClass="form-control">
                                    </dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Sale Amount</label>
                                                        <asp:TextBox ID="txtSaleAmount" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Discount</label>
                                                        <asp:TextBox ID="txtDiscount" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label id="lblToketAmount" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Token Amount</label>
                                                        <asp:TextBox ID="txtTokenAmount" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Down Payment</label>
                                                        <asp:TextBox ID="txtDownPayment" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Balance</label>
                                                        <asp:TextBox ID="txtBalance" Style="text-align: right;" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" style="display: none;">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-3">
                                                        <label id="lblInstallmentType" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Installment Type</label>
                                                        <asp:DropDownList ID="ddlInstallmentType" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="1" Text="Monthly"></asp:ListItem>
                                                            <asp:ListItem Value="2" Text="Quarterly"></asp:ListItem>
                                                            <asp:ListItem Value="3" Text="Half Yearly"></asp:ListItem>
                                                            <asp:ListItem Value="3" Text="Yearly"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <label id="lblNoofInstallment" runat="server"><span class="fa fa-caret-right rgt_cart"></span>No of Installment</label>
                                                        <asp:TextBox ID="txtNoofInstallment" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <label id="lblFromMonth" runat="server" title="Month"><span class="fa fa-caret-right rgt_cart"></span>Start Month</label>
                                                        <asp:TextBox ID="txtFromMonth" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-1" style="margin-top: 25px;">
                                                        <asp:ImageButton ID="ibtnStartMonth" Style="margin-left: 10px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                            Width="30px" />
                                                        <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                        <cc1:CalendarExtender ID="CEStartMonth" runat="server" BehaviorID="calendar3" OnClientShown="onCalendarShown3"
                                                            OnClientHidden="onCalendarHidden3" Format="MMM-yyyy" PopupButtonID="ibtnStartMonth"
                                                            TargetControlID="txtFromMonth"></cc1:CalendarExtender>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commission Agent</label>
                                                        <asp:DropDownList ID="ddlCommissionAgent" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlCommissionAgent" runat="server" CssClass="form-control">
                                    </dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Commision</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtCommissionType" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal" OnSelectedIndexChanged="RbtCommissionType_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right: 22px; margin-left: -5px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text=" &nbsp;&nbsp;&nbsp;Value"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-6" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:DropDownList ID="ddlCommissionPercent" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCommissionPercent_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                                                    <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                                                    <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                                                    <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                                                    <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                                                    <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                                                    <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                                                    <asp:ListItem Value="8" Text="8"></asp:ListItem>
                                                                    <asp:ListItem Value="9" Text="9"></asp:ListItem>
                                                                    <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                            <div class="col-md-6" style="padding-left: 15px !important; padding-right: 0px !important;">
                                                                <asp:TextBox ID="txtCommissionValue" onkeyup="operation()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commision Amount</label>
                                                        <asp:TextBox ID="txtCommisionAmount" Style="text-align: right;" runat="server" CssClass="form-control" Width="99%" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <label style="margin-left: 15px;"><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                    <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 60px; margin-left: 15px;" Width="96%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row" style="display: none;">
                                                <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important; width: 97% !important;">
                                                    <div class="col-md-11" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                        <div class="col-md-12">
                                                            <div class="col-md-2">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Receipt Date</label>
                                                                <asp:TextBox ID="txtReceiptDate" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-1" style="margin-top: 30px">
                                                                <asp:ImageButton ID="ibtnReceiptDate" Style="margin-left: 20px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                                    Width="30px" />
                                                                <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnReceiptDate"
                                                                    TargetControlID="txtReceiptDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                            </div>
                                                            <div class="col-md-3">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Receipt Mode</label>
                                                                <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-control">
                                                                    <asp:ListItem Value="1" Text="Cash"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text="Cheque"></asp:ListItem>
                                                                    <asp:ListItem Value="3" Text="Online Transfer"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                            <div class="col-md-3">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                                                                <asp:TextBox ID="txtReceiptAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-3">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                                <asp:TextBox ID="txtReceiptRemarks" TextMode="MultiLine" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-1" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                        <label><span></span>&nbsp;&nbsp;&nbsp;</label>
                                                        <div class="btnlist pull-right" style="margin-top: 0%">
                                                            <asp:Button ID="btnAddReceipt" OnClick="btnAddReceipt_Click" runat="server" Text="Add Receipt" CssClass="btn btn-success" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" style="display: none;">
                                                <div class="col-md-12" style="width: 97% !important; margin-left: 15px;">
                                                    <div class="emp-table">
                                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                            <ContentTemplate>
                                                                <asp:Panel ID="detailgrid" runat="server">
                                                                    <asp:GridView ID="Grid_users" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                                        HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="Grid_users_PageIndexChanging" PageSize="20"
                                                                        OnRowEditing="Grid_users_RowEditing" OnRowDeleting="Grid_users_RowDeleting" AllowPaging="True" EmptyDataText="No Record exist">
                                                                        <Columns>
                                                                            <%--<asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="5%" />
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>--%>
                                                                            <asp:BoundField DataField="ReceiptDate" DataFormatString="{0:d}" HeaderText="Receipt Date" ReadOnly="true">
                                                                                <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="PayMod" HeaderText="Receipt Mode" ReadOnly="true">
                                                                                <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="AMOUNT" HeaderText="Amount" DataFormatString="{0:N2}" ReadOnly="true">
                                                                                <ItemStyle CssClass="grdDetail" Width="15%" HorizontalAlign="Right"></ItemStyle>
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Action">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="btnEditDetail" runat="server" CommandName="Edit" ToolTip="Edit" class="fa fa-pencil">
                                                                                    </asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Action">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="btnDeleteDetail" runat="server" CommandName="Delete" ToolTip="Delete" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                                                    </asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="margin-left: 30px; width: 97% !important;">
                                                    <div class="row">
                                                        <div class="btnlist" style="margin-top: 0%;">
                                                            <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                                            <asp:Button ID="btnCancel" Style="margin-right: 33px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row" style="width: 100% !important; margin-left: 0px;">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panelgrid" runat="server">
                                            <asp:Panel ID="mastergrid" runat="server">
                                                <asp:GridView ID="GridSaleMaster" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="GridSaleMaster_PageIndexChanging" PageSize="20"
                                                    AllowPaging="True" EmptyDataText="No Record exist">
                                                    <Columns>
                                                        <%--<asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="checkAll" Style="margin-left: 15px;" runat="server" onclick="checkAll(this);" />
                                                                    <headerstyle width="5%" horizontalalign="Center" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                            </asp:TemplateField>--%>
                                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="REF_ID" HeaderText="Ref.#" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="ITEM_NAME" HeaderText="Item" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SALE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Sale Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="12%" HorizontalAlign="Right"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="SALE_MASTER_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("SALE_MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("SALE_MASTER_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                                    <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                                </asp:GridView>
                                            </asp:Panel>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <asp:LinkButton runat="server" ID="hbtn"></asp:LinkButton>
                            <cc1:ModalPopupExtender ID="mPopUpLocation" runat="server" PopupControlID="pnlParameters"
                                TargetControlID="hbtn" BehaviorID="ModelPopup" BackgroundCssClass="modal-background"
                                CancelControlID="btnClose">
                            </cc1:ModalPopupExtender>
                            <asp:Panel ID="pnlParameters" runat="server" Style="display: none;" ScrollBars="Auto" DefaultButton="btnSave">
                                <div class="modal-dialog" style="width: 500px; margin-left: 0px">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" id="btnClose" class="close" runat="server" onserverclick="btnClose_ServerClick">
                                                <span>&times;</span><span class="sr-only">Close</span></button>
                                            <h1 class="modal-title" id="myModalLabel">
                                                <span></span>Add Customer Receipt</h1>
                                        </div>
                                        <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                            <ContentTemplate>
                                                <div class="modal-body">
                                                    <div style="z-index: 101; left: 40px; width: 100px; position: absolute; top: 10px; height: 100px"
                                                        id="DIV1">
                                                        <asp:UpdateProgress ID="UpdateProgress5" runat="server">
                                                            <ProgressTemplate>
                                                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif"></asp:ImageButton>&nbsp; Loading....
                                                            </ProgressTemplate>
                                                        </asp:UpdateProgress>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <asp:Literal ID="lblErrorMsg" runat="server" Visible="false"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <div class="row">
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Receipt Type</label>
                                                                    <dx:ASPxComboBox ID="DrpAccountType" runat="server" AutoPostBack="True" SelectedIndex="0"
                                                                        OnSelectedIndexChanged="DrpAccountType_SelectedIndexChanged" CssClass="form-control">
                                                                        <Items>
                                                                            <dx:ListEditItem Value="18" Text="Cheque Payment"></dx:ListEditItem>
                                                                            <dx:ListEditItem Value="21" Text="Cash Payment"></dx:ListEditItem>
                                                                            <dx:ListEditItem Value="33" Text="Online Transfer"></dx:ListEditItem>
                                                                        </Items>
                                                                    </dx:ASPxComboBox>

                                                                </div>
                                                            </div>
                                                            <div class="row" style="display: none;">
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                                                    <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control"
                                                                        OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Account Head</label>
                                                                    <dx:ASPxComboBox ID="DrpBankAccount" runat="server" CssClass="form-control"
                                                                        OnSelectedIndexChanged="DrpBankAccount_SelectedIndexChanged" AutoPostBack="True">
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-6">
                                                                    <label runat="server" id="lblChequeNo"><span class="fa fa-caret-right rgt_cart"></span>Cheque No</label>
                                                                    <dx:ASPxComboBox ID="drpCheque" runat="server" CssClass="form-control">
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label runat="server" id="lblStatus"><span class="fa fa-caret-right rgt_cart"></span>Status</label>
                                                                    <dx:ASPxComboBox ID="DrpStatus" runat="server" CssClass="form-control"
                                                                        OnSelectedIndexChanged="DrpStatus_SelectedIndexChanged" AutoPostBack="True">
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                                                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"
                                                                        onkeypress="return onlyDotsAndNumbers(this,event);"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <strong>
                                                                        <asp:Literal runat="server" ID="lblChequeDate"><span class="fa fa-caret-right rgt_cart"></span>Cheque Date</asp:Literal></strong>
                                                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-1">
                                                                    <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" Width="30px" Style="padding-top: 21px" />
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>

                                                                    <asp:TextBox ID="txtRecRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <asp:Button AccessKey="S" ID="BtnPopUpSave" OnClick="BtnPopUpSave_Click" runat="server"
                                                                Text="Save" CssClass="btn btn-success" />

                                                            <asp:Button AccessKey="C" ID="BtnPopUpCancel" runat="server"
                                                                Text="Cancel" OnClick="BtnPopUpCancel_Click" CssClass="btn btn-danger" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:CheckBox ID="ChkIsDiscount" Text="%" runat="server" Visible="false" />
                                                        <%--<asp:TextBox ID="txtDiscount" runat="server"
                                                    onkeypress="return onlyDotsAndNumbers(this,event);" Visible="false"></asp:TextBox>--%>
                                                        <asp:TextBox ID="txtTax" runat="server"
                                                            onkeypress="return onlyDotsAndNumbers(this,event);" Visible="false"></asp:TextBox>
                                                        <dx:ASPxComboBox ID="ddlAccountHead" runat="server" Visible="false">
                                                        </dx:ASPxComboBox>
                                                        <asp:Label ID="Label1" Visible="false" runat="server" Text="Received Date" CssClass="lblbox"></asp:Label>
                                                        <asp:TextBox ID="txtReceivedDate" runat="server" ReadOnly="True" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtBankName" runat="server" Width="192px" CssClass="txtBox" Visible="false"></asp:TextBox>
                                                        <asp:Label runat="server" ID="lblAmount" Visible="false"></asp:Label>
                                                        <cc1:CalendarExtender ID="CalendarExtender2" OnClientShown="calendarShown" runat="server" Format="dd/MM/yyyy" PopupButtonID="ibtnStartDate"
                                                            TargetControlID="txtStartDate"></cc1:CalendarExtender>
                                                        <asp:HiddenField ID="HFChqueProcessId" runat="server"></asp:HiddenField>
                                                        <asp:HiddenField ID="hfChequeNo" Value="" runat="server"></asp:HiddenField>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
