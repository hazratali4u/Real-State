<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddServicesNew.aspx.cs" Inherits="Forms_frmAddServicesNew" Title="Services" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
   <script type="text/javascript" src="../css/jquery.min.js"></script>
    <script type="text/javascript" src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }

        $("input[id*=txtCommisionAmount]").mask("#,##0.00", { reverse: true });
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        <%-- function ValidateForm() {
            var str;
            str = document.getElementById('<%=ddlProject.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Project name is required.');
                return false;
            }--%>

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
        //    return true;
        //}

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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="col-md-7">
                <asp:ImageButton ID="AddPopUp" runat="server" Style="padding-bottom: 15px;" ImageUrl="~/images/add-png.png" ToolTip="Add New Service" Width="30px" OnClick="AddPopUp_Click" />
            </div>
            <asp:Panel ID="Panel1" runat="server" Visible="true">
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

                    <div class="row top" style="margin-left: 15px !important; margin-right: 22px !important;">
                        <asp:Panel ID="Panel3" runat="server">
                        </asp:Panel>
                        <div class="emp-table">

                           <%-- <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>--%>
                                    <asp:Panel ID="PanelServices" runat="server">
                                        <div class="row">
                                            <div class="row">

                                                <div class="col-md-9">

                                                    <div class="col-md-3">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Date</label>
                                                        <asp:TextBox ID="txtServiceDate" runat="server" CssClass="form-control" Style="width: 203px !important"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-1" style="margin-top: 30px">
                                                        <asp:ImageButton ID="ibtnServiceDate" Style="margin-left: 8px; margin-top: -4px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                                            Width="30px" />
                                                        <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
                                                        <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnServiceDate"
                                                            TargetControlID="txtServiceDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                                        <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                        <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commission Agent</label>
                                                        <asp:DropDownList ID="DrpCommissionAgent" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commission Amount</label>
                                                        <asp:TextBox ID="txtCommisionAmount" Style="text-align: right;" runat="server" CssClass="form-control" Width="99%" MaxLength="50"></asp:TextBox>
                                                        <%--<cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom"
                                                            TargetControlID="txtCommisionAmount" ValidChars="0123456789."></cc1:FilteredTextBoxExtender>--%>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Commission Type</label>
                                                        <asp:DropDownList ID="ddlCommisionType" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="0" Text="Commission Receivable"></asp:ListItem>
                                                            <asp:ListItem Value="1" Text="Commission Payable"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <%--<div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item Name</label>
                                                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Street No</label>
                                                        <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Block No</label>
                                                        <asp:TextBox ID="txtBlockNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>--%>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Block</label>
                                                        <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Street</label>
                                                        <asp:DropDownList ID="ddlStreet" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlStreet_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                                        <asp:DropDownList ID="ddlItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Measurement</label>
                                                        <asp:TextBox ID="txtMeasurement" runat="server" CssClass="form-control"></asp:TextBox>

                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Extra Area</label>
                                                        <asp:TextBox ID="txtExtraArea" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Floor No</label>
                                                        <asp:TextBox ID="txtFloorNo" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <label style="margin-left: 10px;"><span class="fa fa-caret-right rgt_cart"></span>Commission Resource</label>
                                                        <asp:DropDownList ID="ddlCommissionResource" Style="margin-left: 0px;" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="1" Text="Own Inventory" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Value="2" Text="For Sale Inventory"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-8">
                                                        <label style="margin-left: 15px;"><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                        <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="margin-left: 0px;" Width="100%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-md-3" style="margin-top: 25px; display: none;">
                                                    <fieldset id="fieldset1" runat="server" class="form-control" style="height: 191px; margin-left: -15px;">
                                                        <div class="col-md-12" style="text-align: left;">
                                                            <div class="row" style="margin-top: 10px;">
                                                                <div class="row">
                                                                    <asp:CheckBox ID="Corner" runat="server" Style="margin-top: 25px; margin-left: 20px;" Text=" Corner" />
                                                                </div>
                                                                <div class="row">
                                                                    <asp:CheckBox ID="FacingCommercial" runat="server" Style="margin-top: 10px; margin-left: 20px;" Text="Facing Commercial" />
                                                                </div>
                                                                <div class="row">
                                                                    <asp:CheckBox ID="FacingMosque" runat="server" Style="margin-top: 10px; margin-left: 20px;" Text="Facing Mosque" />
                                                                </div>
                                                                <div class="row">
                                                                    <asp:CheckBox ID="FacingPark" runat="server" Style="margin-top: 10px; margin-left: 20px;" Text="Facing Park" />
                                                                </div>
                                                                <div class="row">
                                                                    <asp:CheckBox ID="MainRoad" runat="server" Style="margin-top: 10px; margin-left: 20px;" Text="Main Road" />
                                                                </div>
                                                                <div class="row">
                                                                    <asp:CheckBox ID="Other" runat="server" Style="margin-top: 10px; margin-left: 20px;" Text="Other" />
                                                                </div>
                                                            </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="col-md-4" style="width: 290px !important; display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item Amount</label>
                                                        <asp:TextBox ID="txtItemAmount" runat="server" CssClass="form-control" OnTextChanged="txtItemAmount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="ftetxtPhoneNumber" runat="server" FilterType="Custom"
                                                            TargetControlID="txtItemAmount" ValidChars="0123456789."></cc1:FilteredTextBoxExtender>
                                                    </div>
                                                    <div class="col-md-4" style="padding-left: 17px !important; width: 33% !important; display: none;">
                                                        <div class="col-md-12" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                            <div class="col-md-5" style="padding-left: 0px !important; padding-right: 0px !important;">
                                                                <label><span class="fa fa-caret-right rgt_cart"></span>Commission</label>
                                                            </div>
                                                            <div class="col-md-7" style="padding-left: 0px !important;">
                                                                <asp:RadioButtonList ID="RbtCommissionType" runat="server" CellPadding="4" CellSpacing="4" RepeatDirection="Horizontal" OnSelectedIndexChanged="RbtCommissionType_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Value="1" Selected="True" style="margin-right: 22px; margin-left: -25px;" Text="&nbsp;&nbsp;&nbsp;% Age"></asp:ListItem>
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
                                                                <asp:TextBox ID="txtCommissionValue" runat="server" CssClass="form-control" Style="padding-bottom: 15px !important;" OnTextChanged="txtCommissionValue_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" FilterType="Custom"
                                                                    TargetControlID="txtCommissionValue" ValidChars="0123456789."></cc1:FilteredTextBoxExtender>
                                                            </div>
                                                        </div>
                                                    </div>


                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="col-md-6">
                                                        <fieldset id="fieldset2" header-text="lksglef" title="Owner Information" runat="server" class="form-control" style="min-height: 335px;">
                                                            <legend style="font-family: Verdana">Seller Information</legend>
                                                            <div class="row">
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Seller Name</label>
                                                                    <asp:TextBox ID="txtOwnerName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>CNIC</label>
                                                                    <asp:TextBox ID="txtOwnerCNIC" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Custom"
                                                                        TargetControlID="txtOwnerCNIC" ValidChars="0123456789-"></cc1:FilteredTextBoxExtender>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Contact No</label>
                                                                    <asp:TextBox ID="txtOwnerContact" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" FilterType="Custom"
                                                                        TargetControlID="txtOwnerContact" ValidChars="0123456789-+"></cc1:FilteredTextBoxExtender>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Email ID</label>
                                                                    <asp:TextBox ID="txtOwnerEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                                                                        ErrorMessage="Invalid Email" ControlToValidate="txtOwnerEmail" ValidationGroup="emailvalidate"
                                                                        ValidationExpression="^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"
                                                                        Display="Dynamic"></asp:RegularExpressionValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Address</label>
                                                                    <asp:TextBox ID="txtOwnerAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                                    <asp:TextBox ID="txtOwnerRemarks" TextMode="MultiLine" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <fieldset id="fieldset3" title="Purchaser Information" runat="server" class="form-control" style="min-height: 335px; width: 102%;">
                                                            <legend style="font-family: Verdana">Purchaser Information</legend>
                                                            <div class="row">
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Purchaser Name</label>
                                                                    <asp:TextBox ID="txtPurchaserName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>CNIC</label>
                                                                    <asp:TextBox ID="txtPurchaserCNIC" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" FilterType="Custom"
                                                                        TargetControlID="txtPurchaserCNIC" ValidChars="0123456789-"></cc1:FilteredTextBoxExtender>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Contact No</label>
                                                                    <asp:TextBox ID="txtPurchaserContact" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" FilterType="Custom"
                                                                        TargetControlID="txtPurchaserContact" ValidChars="0123456789-+"></cc1:FilteredTextBoxExtender>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Email ID</label>
                                                                    <asp:TextBox ID="txtPurchaserEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                                        ErrorMessage="Invalid Email" ControlToValidate="txtPurchaserEmail" ValidationGroup="emailvalidate"
                                                                        ValidationExpression="^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"
                                                                        Display="Dynamic"></asp:RegularExpressionValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Address</label>
                                                                    <asp:TextBox ID="txtPurchaserAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                                    <asp:TextBox ID="txtPurchaserRemarks" TextMode="MultiLine" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="row">
                                                        <div class="btnlist pull-left" style="margin-top: 0%">
                                                            <asp:Button ID="btnSave" Style="margin-left: 33px;" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                                            <asp:Button ID="btnCancel" Style="margin-right: 15px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                               <%-- </ContentTemplate>
                            </asp:UpdatePanel>--%>

                        </div>
                    </div>
                    <div class="row center" style="margin-left: 5px !important; margin-right: -5px !important;">
                        <div class="col-md-12">
                            <div class="emp-table">

                                <asp:UpdatePanel ID="UpdatePanelDetail" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="PanelGrid" runat="server">
                                            <asp:HiddenField ID="hfUserId" runat="server" Value="0" />
                                            <asp:GridView ID="Grid_users" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="Grid_users_PageIndexChanging" PageSize="20"
                                                AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
                                                <Columns>
                                                    <%-- <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                        </asp:TemplateField>--%>
                                                    <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true"></asp:BoundField>
                                                    <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project Name" ReadOnly="true"></asp:BoundField>
                                                    <asp:BoundField DataField="VENDOR_NAME" HeaderText="Coomission Agent" ReadOnly="true">
                                                        <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SERVICE_ID" HeaderText="Invoice #" ReadOnly="true">
                                                        <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DATE" HeaderText="Date" ReadOnly="true" DataFormatString="{0:dd-MMM-yyyy}">
                                                        <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ITEM_AMOUNT" HeaderText="Amount" DataFormatString="{0:N2}" ReadOnly="true">
                                                        <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Right"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                        <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SERVICE_ID" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                        <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("SERVICE_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit" class="fa fa-pencil">
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("SERVICE_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
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
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
