<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddPurchasesNew.aspx.cs" Inherits="Forms_frmAddPurchasesNew" Title="Purchases" %>

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

        function MyFunction() {
            var txtPurchaseAmount = document.getElementById('<%=txtPurchaseAmount.ClientID%>').value;
            var txtDiscount = document.getElementById('<%=txtDiscount.ClientID%>').value;
            <%--var txtTokenAmount = document.getElementById('<%=txtTokenAmount.ClientID%>').value;
            var txtDownPayment = document.getElementById('<%=txtDownPayment.ClientID%>').value;--%>
            var txtBalance = document.getElementById('<%=txtBalance.ClientID%>').value;
            if (txtPurchaseAmount == "")
                document.getElementById('<%=txtPurchaseAmount.ClientID%>').value = 0;
            if (txtDiscount == "")
                document.getElementById('<%=txtDiscount.ClientID%>').value = 0;
            <%--if (txtTokenAmount == "")
                document.getElementById('<%=txtTokenAmount.ClientID%>').value = 0;
            if (txtDownPayment == "")
                document.getElementById('<%=txtDownPayment.ClientID%>').value = 0;--%>
            if (txtBalance == "")
                document.getElementById('<%=txtBalance.ClientID%>').value = 0;
            var result = parseInt(txtPurchaseAmount) - parseInt(txtDiscount); //- parseInt(txtTokenAmount) - parseInt(txtDownPayment);
            if (!isNaN(result)) {
                document.getElementById('<%=txtBalance.ClientID%>').value = result;
            }
            else {
                document.getElementById('<%=txtBalance.ClientID%>').value = 0;
            }
        }
        function operation() {
            <%--var txtCommissionValue = document.getElementById('<%=txtCommissionValue.ClientID%>').value;--%>
            <%--var txtCommisionAmount = document.getElementById('<%=txtCommisionAmount.ClientID%>').value;--%>
            <%--if (txtCommissionValue == "")
                document.getElementById('<%=txtCommissionValue.ClientID%>').value = 0;--%>
           <%-- if (txtCommisionAmount == "")
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = 0;
            var result = parseInt(txtCommissionValue);
            if (!isNaN(result)) {
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = result;
            }
            else {
                document.getElementById('<%=txtCommisionAmount.ClientID%>').value = 0;
            }--%>
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

    </script>
    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <ContentTemplate>
            <div class="col-md-7" style="padding-bottom: 15px;">
                <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Purchase" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
            </div>
            <asp:Panel ID="Panel3" runat="server" DefaultButton="btnsearch">
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
            <div class="main-contents">
                <div class="container employee-infomation">
                    <div class="row top" style="margin-left: 15px !important; margin-right: 15px !important;">
                        <div class="emp-table">
                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="PanelPerchase" runat="server">
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-3">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Date</label>
                                                        <asp:TextBox ID="txtPurchaseDate" Style="width: 200px!important;" runat="server" CssClass="form-control"></asp:TextBox>
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
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                        <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" style="width: 75% !important;">
                                                    <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Category</label>
                                                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category</label>
                                                                    <asp:DropDownList ID="ddlSubCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Detail Category</label>
                                                                    <asp:DropDownList ID="ddlDetailCategory" runat="server" OnSelectedIndexChanged="ddlDetailCategory_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Block</label>
                                                        <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Street</label>
                                                        <asp:DropDownList ID="ddlStreet" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlStreet_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                                        <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>--%>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Vendor</label>
                                                        <asp:DropDownList ID="ddlVendor" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Purchase Amount</label>
                                                        <asp:TextBox ID="txtPurchaseAmount" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>--%>

                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Discount</label>
                                                        <asp:TextBox ID="txtDiscount" Style="text-align: right;" runat="server" CssClass="form-control" onkeyup="MyFunction()"></asp:TextBox>
                                                    </div>--%>
                                                    <%--<div class="col-md-4">
                                                        <label id="lblToketAmount" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Token Amount</label>
                                                        <asp:TextBox ID="txtTokenAmount" Style="text-align: right;" runat="server" CssClass="form-control" onkeyup="MyFunction()"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Down Payment</label>
                                                        <asp:TextBox ID="txtDownPayment" Style="text-align: right;" runat="server" CssClass="form-control" onkeyup="MyFunction()"></asp:TextBox>
                                                    </div>--%>
                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Balance</label>
                                                        <asp:TextBox ID="txtBalance" Style="text-align: right;" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>--%>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commission Agent</label>
                                                        <asp:DropDownList ID="ddlCommissionAgent" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-8">
                                                    <label style="margin-left: 15px;"><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                    <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 60px; margin-left: 15px;" Width="96%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                </div>
                                                    <%--<div class="col-md-4">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <%--<div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Commision</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtCommissionType" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal" OnSelectedIndexChanged="RbtCommissionType_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right: 12px; margin-left: -5px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
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
                                                                <asp:TextBox ID="txtCommissionValue" Style="text-align: right;" runat="server" CssClass="form-control" onkeyup="operation()"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>--%>
                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commision Amount</label>
                                                        <asp:TextBox ID="txtCommisionAmount" Style="text-align: right;" runat="server" CssClass="form-control" Width="99%" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                                    </div>--%>
                                                </div>
                                                </div>
                                            <%--<div class="row">
                                                <div class="col-md-12" style="width: 75% !important">
                                                    <label style="margin-left: 15px;"><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                    <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 60px; margin-left: 15px;" Width="96%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>--%>

                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="col-md-3">
                                                        <label id="lblInstallmentType" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Item Description</label>
                                                        <asp:DropDownList ID="ddlItem" Style="margin-top: 3px;" runat="server" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>P.Amount/Down Payment</label>
                                                        <asp:TextBox ID="txtPurchaseAmount" onkeyup="MyFunction()" Style="text-align: right; margin-top: 3px;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-2">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Discount</label>
                                                        <asp:TextBox ID="txtDiscount" Style="text-align: right; margin-top: 3px;" runat="server" CssClass="form-control" onkeyup="MyFunction()"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Comm</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtCommissionType" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right:7px;" Text="% "></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text=" Value"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <asp:TextBox ID="txtCommissionValue" runat="server" CssClass="form-control" Style="width: 170px !important; text-align:right;"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Balance</label>
                                                        <asp:TextBox ID="txtBalance" Style="text-align: right; margin-top: 3px;" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-1">
                                                        <asp:Button ID="btnAdd" OnClick="btnAdd_Click" runat="server" Text="Add" Width="80px" Style="margin-top: 28px;" CssClass="btn btn-success" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="col-md-12">
                                                    <asp:HiddenField ID="RowId" runat="server" Value="-1" />
                                                    <asp:Panel ID="PanelItems" runat="server">
                                                        <asp:GridView ID="GrdItems" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                            AutoGenerateColumns="False" OnRowDataBound="GrdItems_RowDataBound" OnRowDeleting="GrdItems_RowDeleting"
                                                            OnRowEditing="GrdItems_RowEditing" Width="100%" ShowFooter="False">
                                                            <Columns>
                                                                <asp:BoundField DataField="ITEM_INFO_ID" HeaderText="ITEM_INFO_ID" ReadOnly="true">
                                                                    <HeaderStyle CssClass="HidePanel" />
                                                                    <ItemStyle CssClass="HidePanel" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="ITEM_NAME" HeaderText="Item Description" ReadOnly="true">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Left" Width="203px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="PURCHASE_AMOUNT" HeaderText="Purchase Amount" ReadOnly="true" DataFormatString="{0:N2}">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                        Width="134px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="DISCOUNT" HeaderText="Discount" ReadOnly="true" DataFormatString="{0:N2}">
                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                        Width="140px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="COMMISSION" HeaderText="Commission" ReadOnly="true" DataFormatString="{0:N2}">
                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right"
                                                                        Width="137px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="BALANCE" DataFormatString="{0:N}" HeaderText="Balance" ReadOnly="true">
                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Right" Width="134px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="COMMISSION_PER" HeaderText="COMMISSION_PER" ReadOnly="true">
                                                                    <HeaderStyle CssClass="HidePanel" />
                                                                    <ItemStyle CssClass="HidePanel" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="COMMISSION_VALUE" HeaderText="COMMISSION_VALUE" ReadOnly="true">
                                                                    <HeaderStyle CssClass="HidePanel" />
                                                                    <ItemStyle CssClass="HidePanel" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="TOKEN_AMOUNT" HeaderText="TOKEN_AMOUNT" ReadOnly="true">
                                                                    <HeaderStyle CssClass="HidePanel" />
                                                                    <ItemStyle CssClass="HidePanel" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="DOWN_PAYMENT" HeaderText="DOWN_PAYMENT" ReadOnly="true">
                                                                    <HeaderStyle CssClass="HidePanel" />
                                                                    <ItemStyle CssClass="HidePanel" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" class="fa fa-pencil" ToolTip="Edit">
                                                                        </asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" Width="33px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                            CssClass=" fa fa-trash-o"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" Width="33px" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle CssClass="tblhead" />
                                                        </asp:GridView>
                                                    </asp:Panel>
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                        <ContentTemplate>
                                                            <table id="TotalTable" runat="server" width="100%" style="margin-top: 10px; display:none;">
                                                                <tr>
                                                                    <td align="right">
                                                                        <strong style="margin-right: 7px;">Purchase Amount:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalPurchaseAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td align="right">
                                                                        <strong style="margin-right: 7px;">Discount Amount:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalDiscountAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td align="right">
                                                                        <strong style="margin-right: 7px;">Token Amount:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalTokenAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td align="right">
                                                                        <strong style="margin-right: 7px;">Down Payment:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalDownPayment" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                     <td align="right">
                                                                        <strong style="margin-right: 7px;">Balance:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalBalance" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td align="right">
                                                                        <strong style="margin-right: 7px;">Commission:
                                                                        </strong>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTotalCommission" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                                    </div>
                                            </div>

                                            <div class="row">
                                                <div class="btnlist">
                                                    <asp:Button ID="btnSave" Style="margin-left: 31px;" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                                    <asp:Button ID="btnCancel" Style="margin-right: 33px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="row" style="width: 100% !important; margin-left: 0px;">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="panelgrid" runat="server">
                                            <asp:Panel ID="mastergrid" runat="server">
                                                <asp:GridView ID="PurchaseMaster" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="PurchaseMaster_PageIndexChanging" PageSize="20"
                                                    AllowPaging="True" EmptyDataText="No Record exist">
                                                    <Columns>
                                                        <%--<asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="checkAll" Style="margin-left: 15px;" runat="server" onclick="checkAll(this);" />
                                                                <HeaderStyle Width="5%" HorizontalAlign="Center" />
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
                                                        <asp:BoundField DataField="PURCHASE_MASTER_ID" HeaderText="Ref.#" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        <%--<asp:BoundField DataField="ITEM_NAME" HeaderText="Item" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="TOTAL_PURCHASE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Purchase Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="IS_DELETED" HeaderText="Status" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_MASTER_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("PURCHASE_MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("PURCHASE_MASTER_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
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
