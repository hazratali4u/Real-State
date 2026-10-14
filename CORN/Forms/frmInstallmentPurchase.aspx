<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmInstallmentPurchase.aspx.cs" Inherits="Forms_frmInstallmentPurchase" Title="Installment Purchase" %>

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
            var HFeild = document.getElementById('<%=HFeild.ClientID%>').value;
            if (HFeild > 0)
            {
                var txtTotalRemainingAmount = document.getElementById('<%=txtTotalRemainingAmount.ClientID%>').value;
                if (txtTotalRemainingAmount == 0)
                {
                    alert('Delete installment plans first then you can update the value.');
                    document.getElementById('<%=txtPurchaseAmount.ClientID%>').value = document.getElementById('<%=HiddenPurchaseAmount.ClientID%>').value;
                    document.getElementById('<%=txtDiscount.ClientID%>').value = document.getElementById('<%=HiddenDiscount.ClientID%>').value;
                    document.getElementById('<%=txtBookingPercent.ClientID%>').value = document.getElementById('<%=HiddenBookingPercent.ClientID%>').value;
                    document.getElementById('<%=txtBookingValue.ClientID%>').value = document.getElementById('<%=HiddenBookingValue.ClientID%>').value;
                    document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = document.getElementById('<%=HiddenConfirmationPercent.ClientID%>').value;
                    document.getElementById('<%=txtConfirmationValue.ClientID%>').value = document.getElementById('<%=HiddenConfirmationValue.ClientID%>').value;
                    document.getElementById('<%=txtAllocationPercent.ClientID%>').value = document.getElementById('<%=HiddenAllocationPercent.ClientID%>').value;
                    document.getElementById('<%=txtAllocationValue.ClientID%>').value = document.getElementById('<%=HiddenAllocationValue.ClientID%>').value;
                    document.getElementById('<%=txtBalance.ClientID%>').value = document.getElementById('<%=HiddenBalance.ClientID%>').value;
                }
                else
                {
                    var totalRows = $("#<%=GrdInstallment.ClientID %> tr").length;
                    if (totalRows > 0)
                    {
                        alert('Delete installment plans first then you can update the value.');
                        document.getElementById('<%=txtPurchaseAmount.ClientID%>').value = document.getElementById('<%=HiddenPurchaseAmount.ClientID%>').value;
                    document.getElementById('<%=txtDiscount.ClientID%>').value = document.getElementById('<%=HiddenDiscount.ClientID%>').value;
                    document.getElementById('<%=txtBookingPercent.ClientID%>').value = document.getElementById('<%=HiddenBookingPercent.ClientID%>').value;
                    document.getElementById('<%=txtBookingValue.ClientID%>').value = document.getElementById('<%=HiddenBookingValue.ClientID%>').value;
                    document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = document.getElementById('<%=HiddenConfirmationPercent.ClientID%>').value;
                    document.getElementById('<%=txtConfirmationValue.ClientID%>').value = document.getElementById('<%=HiddenConfirmationValue.ClientID%>').value;
                    document.getElementById('<%=txtAllocationPercent.ClientID%>').value = document.getElementById('<%=HiddenAllocationPercent.ClientID%>').value;
                    document.getElementById('<%=txtAllocationValue.ClientID%>').value = document.getElementById('<%=HiddenAllocationValue.ClientID%>').value;
                        document.getElementById('<%=txtBalance.ClientID%>').value = document.getElementById('<%=HiddenBalance.ClientID%>').value;
                    }
                    else
                    {
                        var txtPurchaseAmount = document.getElementById('<%=txtPurchaseAmount.ClientID%>').value;
                        var txtDiscount = document.getElementById('<%=txtDiscount.ClientID%>').value;
                        var txtBookingPercent = document.getElementById('<%=txtBookingPercent.ClientID%>').value;
                        var txtBookingValue = document.getElementById('<%=txtBookingValue.ClientID%>').value;
                        var txtConfirmationPercent = document.getElementById('<%=txtConfirmationPercent.ClientID%>').value;
                        var txtConfirmationValue = document.getElementById('<%=txtConfirmationValue.ClientID%>').value;
                        var txtAllocationPercent = document.getElementById('<%=txtAllocationPercent.ClientID%>').value;
                        var txtAllocationValue = document.getElementById('<%=txtAllocationValue.ClientID%>').value;

                        var txtBalance = document.getElementById('<%=txtBalance.ClientID%>').value;
                        var RbtBooking = $("[id$='RbtBooking']").find(":checked").val();
                        var RbtConfirmation = $("[id$='RbtConfirmation']").find(":checked").val();
                        var RbtAllocation = $("[id$='RbtAllocation']").find(":checked").val();
                        var RbtCommissionType = $("[id$='RbtCommissionType']").find(":checked").val();

                        if (txtBalance == "")
                            document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                        if (txtPurchaseAmount == "")
                            document.getElementById('<%=txtPurchaseAmount.ClientID%>').value = 0;
                    if (txtDiscount == "")
                        document.getElementById('<%=txtDiscount.ClientID%>').value = 0;

                if (RbtBooking != 1) {
                    if (txtBookingValue == "")
                        document.getElementById('<%=txtBookingValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtBookingPercent.ClientID%>').value = 0;
                    var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                    if (!isNaN(result)) {
                        document.getElementById('<%=txtBalance.ClientID%>').value = result;
                    }
                    else {
                        document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                    }
                }
                else
                {
                    if (txtBookingPercent == "")
                    {
                        document.getElementById('<%=txtBookingPercent.ClientID%>').value = 0;
                    }
                    else
                    {

                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtBookingPercent);
                        document.getElementById('<%=txtBookingValue.ClientID%>').value = booking;
                        var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                        if (!isNaN(result)) {
                            document.getElementById('<%=txtBalance.ClientID%>').value = result;
                        }
                        else {
                            document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                        }
                    }
                }
                if (RbtConfirmation != 1) {
                    if (txtConfirmationValue == "")
                        document.getElementById('<%=txtConfirmationValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = 0;
                }
                else {
                    if (txtConfirmationPercent == "") {
                        document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = 0;
                    }
                    else {
                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtConfirmationPercent);
                        document.getElementById('<%=txtConfirmationValue.ClientID%>').value = booking;
                    }
                }
                if (RbtAllocation != 1) {
                    if (txtAllocationValue == "")
                        document.getElementById('<%=txtAllocationValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtAllocationPercent.ClientID%>').value = 0;
                }
                else {
                    if (txtAllocationPercent == "") {
                        document.getElementById('<%=txtAllocationPercent.ClientID%>').value = 0;
                    }
                    else {
                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtAllocationPercent);
                        document.getElementById('<%=txtAllocationValue.ClientID%>').value = booking;
                    }
                }

                var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                if (!isNaN(result)) {
                    document.getElementById('<%=txtBalance.ClientID%>').value = result;
                    document.getElementById('<%=txtTotalRemainingAmount.ClientID%>').value = result;
                }
                else {
                    document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                }
            }
                }
            }
            else
            {
            var txtPurchaseAmount = document.getElementById('<%=txtPurchaseAmount.ClientID%>').value;
                    var txtDiscount = document.getElementById('<%=txtDiscount.ClientID%>').value;
                    var txtBookingPercent = document.getElementById('<%=txtBookingPercent.ClientID%>').value;
                    var txtBookingValue = document.getElementById('<%=txtBookingValue.ClientID%>').value;
                    var txtConfirmationPercent = document.getElementById('<%=txtConfirmationPercent.ClientID%>').value;
                    var txtConfirmationValue = document.getElementById('<%=txtConfirmationValue.ClientID%>').value;
                    var txtAllocationPercent = document.getElementById('<%=txtAllocationPercent.ClientID%>').value;
                    var txtAllocationValue = document.getElementById('<%=txtAllocationValue.ClientID%>').value;

                    var txtBalance = document.getElementById('<%=txtBalance.ClientID%>').value;
                    var RbtBooking = $("[id$='RbtBooking']").find(":checked").val();
                    var RbtConfirmation = $("[id$='RbtConfirmation']").find(":checked").val();
                    var RbtAllocation = $("[id$='RbtAllocation']").find(":checked").val();
                    var RbtCommissionType = $("[id$='RbtCommissionType']").find(":checked").val();

                    if (txtBalance == "")
                        document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                    if (txtPurchaseAmount == "")
                        document.getElementById('<%=txtPurchaseAmount.ClientID%>').value = 0;
                    if (txtDiscount == "")
                        document.getElementById('<%=txtDiscount.ClientID%>').value = 0;

                if (RbtBooking != 1) {
                    if (txtBookingValue == "")
                        document.getElementById('<%=txtBookingValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtBookingPercent.ClientID%>').value = 0;
                    var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                    if (!isNaN(result)) {
                        document.getElementById('<%=txtBalance.ClientID%>').value = result;
                    }
                    else {
                        document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                    }
                }
                else {
                    if (txtBookingPercent == "") {
                        document.getElementById('<%=txtBookingPercent.ClientID%>').value = 0;
                    }
                    else {

                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtBookingPercent);
                        document.getElementById('<%=txtBookingValue.ClientID%>').value = booking;
                        var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                        if (!isNaN(result)) {
                            document.getElementById('<%=txtBalance.ClientID%>').value = result;
                        }
                        else {
                            document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                        }
                    }
                }
                if (RbtConfirmation != 1) {
                    if (txtConfirmationValue == "")
                        document.getElementById('<%=txtConfirmationValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = 0;
                }
                else {
                    if (txtConfirmationPercent == "") {
                        document.getElementById('<%=txtConfirmationPercent.ClientID%>').value = 0;
                    }
                    else {
                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtConfirmationPercent);
                        document.getElementById('<%=txtConfirmationValue.ClientID%>').value = booking;
                    }
                }
                if (RbtAllocation != 1) {
                    if (txtAllocationValue == "")
                        document.getElementById('<%=txtAllocationValue.ClientID%>').value = 0;
                    document.getElementById('<%=txtAllocationPercent.ClientID%>').value = 0;
                }
                else {
                    if (txtAllocationPercent == "") {
                        document.getElementById('<%=txtAllocationPercent.ClientID%>').value = 0;
                    }
                    else {
                        var value = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount);
                        var booking = parseFloat(value) / 100 * parseFloat(txtAllocationPercent);
                        document.getElementById('<%=txtAllocationValue.ClientID%>').value = booking;
                    }
                }

                var result = parseFloat(txtPurchaseAmount) - parseFloat(txtDiscount) - parseFloat(txtBookingValue) - parseFloat(txtConfirmationValue) - parseFloat(txtAllocationValue);
                if (!isNaN(result)) {
                    document.getElementById('<%=txtBalance.ClientID%>').value = result;
                }
                else {
                    document.getElementById('<%=txtBalance.ClientID%>').value = 0;
                }
        }

    }


        function jsFunction(value) {
            if (value != 4) {
                if (value == 5) {
                    document.getElementById('<%=txtNoofInstallment.ClientID%>').value = "1";
                    document.getElementById('<%=txtNoofInstallment.ClientID%>').disabled = true;
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').disabled = false;
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').focus();
                    document.getElementById('<%=Label1.ClientID%>').innerText = "Possession Amount";
                }
                else {
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "";
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').disabled = false;
                    document.getElementById('<%=txtNoofInstallment.ClientID%>').disabled = false;
                    document.getElementById('<%=txtNoofInstallment.ClientID%>').focus();
                    document.getElementById('<%=Label1.ClientID%>').innerText = "Installment Amount";
                }
            }
            else {
                document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = 0;
                document.getElementById('<%=txtInstallmentAmount.ClientID%>').disabled = true;
                document.getElementById('<%=txtNoofInstallment.ClientID%>').disabled = false;
                document.getElementById('<%=txtNoofInstallment.ClientID%>').focus();
                document.getElementById('<%=Label1.ClientID%>').innerText = "Installment Amount";
            }
        }
        function JSFunc() {
            var txtInstallmentAmount = document.getElementById('<%=txtInstallmentAmount.ClientID%>').value;
            var txtNoofInstallment = document.getElementById('<%=txtNoofInstallment.ClientID%>').value;
            var txtBalance = document.getElementById('<%=txtBalance.ClientID%>').value;
            var txtPurchaseAmount = document.getElementById('<%=txtPurchaseAmount.ClientID%>').value;
            if (txtInstallmentAmount == "")
                document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "0";
            if (txtNoofInstallment == "")
                document.getElementById('<%=txtNoofInstallment.ClientID%>').value = "0";
            if (txtBalance.length == 0 || txtBalance == "") {
                if (txtPurchaseAmount.length == 0 || txtPurchaseAmount == "" || txtBalance.length == 0) {
                    alert('Please enter Purchase Amount first.');
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "0";
                    document.getElementById('<%=txtPurchaseAmount.ClientID%>').focus();
                }


                if (txtBalance == "0") {
                    alert('Balance should be greater then zero.');
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "0";
                    document.getElementById('<%=txtBalance.ClientID%>').focus();
                }


            }
            <%--if (txtNoofInstallment == "0") {
                alert('No of Installment should be greater then zero.');
                document.getElementById('<%=txtNoofInstallment.ClientID%>').focus();
            }--%>

            if (txtTotalRemainingAmount > 0) {
                //alert('hello');
                var Amount = parseFloat(txtNoofInstallment) * parseFloat(txtInstallmentAmount);
                if (parseFloat(Amount) > parseFloat(txtTotalRemainingAmount)) {
                    alert(txtNoofInstallment + " X " + txtInstallmentAmount + " = " + Amount + " should not be greater then Total Remaining Amount");
                    document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "0";
                } else {
                    var Amount = parseFloat(txtNoofInstallment) * parseFloat(txtInstallmentAmount);
                    if (parseFloat(Amount) > parseFloat(txtBalance)) {
                        alert(txtNoofInstallment + " X " + txtInstallmentAmount + " = " + Amount + " should not be greater then Balance");
                        document.getElementById('<%=txtInstallmentAmount.ClientID%>').value = "0";
                    }
                }
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                                <asp:Panel ID="PanelSale" runat="server">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="col-md-6">
                                                <fieldset id="fieldset1" title="Purchase Information" runat="server" class="form-control" style="min-height: 255px;">
                                                    <legend style="font-family: Verdana">Purchase Information</legend>
                                                    <div class="row">
                                                        <div class="col-md-5">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Date</label>
                                                            <asp:TextBox ID="txtPurchaseDate" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-1" style="margin-top: 27px">
                                                            <asp:ImageButton ID="ibtnPurchaseDate" Style="margin-left: -10px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                                Width="30px" />
                                                            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                            <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnPurchaseDate"
                                                                TargetControlID="txtPurchaseDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                        </div>


                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                                            <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                            <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Block</label>
                                                            <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Street</label>
                                                            <asp:DropDownList ID="ddlStreet" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlStreet_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                                            <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                            <div class="col-md-6">
                                                <fieldset id="fieldset2" title="Payment Information" runat="server" class="form-control" style="min-height: 255px;">
                                                    <legend style="font-family: Verdana">Payment Information</legend>
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Vendor</label>
                                                            <asp:DropDownList ID="ddlVendor" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Purchase Amount</label>
                                                            <asp:TextBox ID="txtPurchaseAmount" onkeyup="MyFunction()" runat="server" CssClass="form-control" Style="text-align: right;"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Booking</label>
                                                                </div>
                                                                <div class="col-md-7" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                    <asp:RadioButtonList ID="RbtBooking" onclick="MyFunction()" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal" onkeyup="MyFunction()">
                                                                        <asp:ListItem Value="1" Selected="True" style="margin-right: 22px; margin-left: -5px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
                                                                        <asp:ListItem Value="2" Text=" &nbsp;&nbsp;&nbsp;Value"></asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <div class="col-md-6" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                    <asp:TextBox ID="txtBookingPercent" Style="text-align: right; width: 230px;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-6" style="padding-left: 15px !important; padding-right: 0px !important;">
                                                                    <asp:TextBox ID="txtBookingValue" Style="text-align: right; width: 230px;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Discount Amount</label>
                                                            <asp:TextBox ID="txtDiscount" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Balance</label>
                                                            <asp:TextBox ID="txtBalance" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                            <div class="col-md-12">
                                                <div class="row">

                                                    <div class="col-md-6">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Confirmation</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtConfirmation" onclick="MyFunction()" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right: 17px; margin-left: 0px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text=" &nbsp;&nbsp;&nbsp;Value"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-6" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:TextBox ID="txtConfirmationPercent" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-6" style="padding-left: 15px !important; padding-right: 0px !important;">
                                                                <asp:TextBox ID="txtConfirmationValue" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <div class="col-md-5">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Confirmation Date</label>
                                                            <asp:TextBox ID="txtConfirmationDate" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-1" style="margin-top: 27px">
                                                            <asp:ImageButton ID="ibtnConfirmationDate" Style="margin-left: -10px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                                Width="30px" />
                                                            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnConfirmationDate"
                                                                TargetControlID="txtConfirmationDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Allocation</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtAllocation" onclick="MyFunction()" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right: 22px; margin-left: -5px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text=" &nbsp;&nbsp;&nbsp;Value"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-6" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <asp:TextBox ID="txtAllocationPercent" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                            <div class="col-md-6" style="padding-left: 15px !important; padding-right: 0px !important;">
                                                                <asp:TextBox ID="txtAllocationValue" Style="text-align: right;" onkeyup="MyFunction()" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <div class="col-md-5">
                                                            <label><span class="fa fa-caret-right rgt_cart"></span>Allocation Date</label>
                                                            <asp:TextBox ID="txtAllocationDate" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-1" style="margin-top: 27px">
                                                            <asp:ImageButton ID="ibtnAllocationDate" Style="margin-left: -10px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                                Width="30px" />
                                                            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                            <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnAllocationDate"
                                                                TargetControlID="txtAllocationDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                        <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 60px;" Width="100%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2">
                                                        <label id="lblInstallmentType" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Installment Type</label>
                                                        <asp:DropDownList ID="ddlInstallmentType" runat="server" CssClass="form-control" onchange="jsFunction(this.value);">
                                                            <asp:ListItem Value="1" Text="Quarterly"></asp:ListItem>
                                                            <asp:ListItem Value="2" Text="Half Yearly"></asp:ListItem>
                                                            <asp:ListItem Value="3" Text="Yearly"></asp:ListItem>
                                                            <asp:ListItem Value="5" Text="Possession"></asp:ListItem>
                                                            <asp:ListItem Value="4" Text="Monthly"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <label id="lblNoofInstallment" runat="server"><span class="fa fa-caret-right rgt_cart"></span>No of Installment</label>
                                                        <asp:TextBox ID="txtNoofInstallment" onkeyup="JSFunc()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-2">
                                                        <label id="Label1" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                                                        <asp:TextBox ID="txtInstallmentAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <label id="lblFromMonth" runat="server" title="Month"><span class="fa fa-caret-right rgt_cart"></span>Start Month</label>
                                                        <asp:TextBox ID="txtFromMonth" runat="server" CssClass="form-control" Style="width: 200px !important"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-1" style="margin-top: 25px;">
                                                        <asp:ImageButton ID="ibtnStartMonth" Style="margin-left: 35px; margin-top: 2px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                            Width="30px" />
                                                        <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                        <cc1:CalendarExtender ID="CEStartMonth" runat="server" BehaviorID="calendar3" OnClientShown="onCalendarShown3"
                                                            OnClientHidden="onCalendarHidden3" Format="MMM-yyyy" PopupButtonID="ibtnStartMonth"
                                                            TargetControlID="txtFromMonth"></cc1:CalendarExtender>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <asp:Button ID="btnAdd" OnClick="btnAdd_Click" runat="server" Text="Add" Width="80px" Style="margin-top: 25px;" CssClass="btn btn-success" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <asp:HiddenField ID="RowId" runat="server" Value="-1" />
                                                        <asp:Panel ID="PanelInstallments" runat="server">
                                                            <asp:GridView ID="GrdInstallment" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                                AutoGenerateColumns="False" OnRowDataBound="GrdInstallment_RowDataBound" OnRowDeleting="GrdInstallment_RowDeleting"
                                                                OnRowEditing="GrdInstallment_RowEditing" Width="85%" ShowFooter="False">
                                                                <Columns>
                                                                    <asp:BoundField DataField="InstallmentType_ID" HeaderText="InstallmentType_ID" ReadOnly="true">
                                                                        <HeaderStyle CssClass="HidePanel" />
                                                                        <ItemStyle CssClass="HidePanel" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="InstallmentType" HeaderText="Installment Type" ReadOnly="true">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left"
                                                                            Width="200px" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="StartMonth" HeaderText="Start Month" ReadOnly="true" DataFormatString="{0:MMM-yyyy}">
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                            Width="130px" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="NoOfInstallment" HeaderText="No Of Installment" ReadOnly="true">
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                            Width="150px" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="InstallmentAmount" DataFormatString="{0:N}" HeaderText="Amount" ReadOnly="true">
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                            Width="150px" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="TotalInstallmentAmount" DataFormatString="{0:N}" HeaderText="Total Installment Amount" ReadOnly="true">
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                            Width="240px" />
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" class="fa fa-pencil" ToolTip="Edit">
                                                                            </asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                                CssClass=" fa fa-trash-o"></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <HeaderStyle CssClass="tblhead" />
                                                            </asp:GridView>
                                                        </asp:Panel>
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                            <ContentTemplate>
                                                                <table id="TotalTable" runat="server" width="100%" style="margin-top: 10px;">
                                                                    <tr>
                                                                        <td style="width: 70%;" align="right">
                                                                            <strong style="margin-right: 7px;">Balance Amount:
                                                                            </strong>
                                                                        </td>
                                                                        <td style="width: 30%;" align="left">
                                                                            <asp:TextBox ID="txtTotalRemainingAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>


                                                    </div>
                                                </div>
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
                                    <asp:HiddenField ID="HFeild" Value="0" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenPurchaseAmount" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenDiscount" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenBookingPercent" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenBookingValue" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenConfirmationPercent" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenConfirmationValue" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenAllocationPercent" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenAllocationValue" Value="" runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="HiddenBalance" Value="" runat="server"></asp:HiddenField>
                                </asp:Panel>
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
                                                        <asp:BoundField DataField="ITEM_NAME" HeaderText="Item" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Purchase Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="12%" HorizontalAlign="Right"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_INS_MASTER_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("PURCHASE_INS_MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("PURCHASE_INS_MASTER_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
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
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
