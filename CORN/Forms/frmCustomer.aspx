<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmCustomer.aspx.cs" Inherits="frmCustomer" Title="Add Customer" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">

    <script type="text/javascript" src="../AjaxLibrary/ValidateDotsAndNumbers.js"></script>
    <script language="JavaScript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }
        function ValidateForm() {
            var str;
            <%--str = document.getElementById('<%=txtCode.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Code is required');
                return false;
            }--%>
            str = document.getElementById('<%=txtName.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Customer name is required');
                return false;
            }
            str = document.getElementById('<%=txtContact.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Primary Contact no. is required');
                return false;
            }
            str = document.getElementById('<%=txtAddress.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Address is required');
                return false;
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
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <div class="row top">
                <asp:Panel ID="Panel4" runat="server" DefaultButton="btnsearch">
                    <div class="col-md-4">
                        <div class="search">
                            <asp:TextBox ID="txtSearch" runat="server" placeholder="Search" CssClass="form-control"
                                TabIndex="0"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2" style="margin-left: -60px;">
                        <asp:LinkButton ID="btnsearch" OnClick="btnFilter_Click" runat="server" Text="Search"
                            CssClass="btn btn-success"><i class="fa fa-search"  style="font-size:20px;"></i></asp:LinkButton>
                    </div>
                    <asp:LinkButton ID="btndummy" runat="server" UseSubmitBehavior="false" />
                </asp:Panel>
                <div class="col-md-offset-5 col-md-3 ">
                    <div class="btnlist pull-right">
                        <asp:LinkButton CssClass="btn btn-warning" runat="server" ID="btnAdd" Text="Add"
                            OnClick="btnAdd_Click">
                               <span class="fa fa-plus-circle"></span>Add
                        </asp:LinkButton>
                        <asp:LinkButton class="btn btn-success" OnClick="btnActive_Click" ID="btnActive" runat="server"
                            OnClientClick="javasacript:return confirm('Are you sure you want to perform this action?'); return false;">
                                    <span class="fa fa-check"></span>Active</asp:LinkButton>
                        <!-- POP UP MODEL-->
                        <cc1:ModalPopupExtender ID="mPopUpLocation" runat="server" PopupControlID="pnlParameters"
                            TargetControlID="btnAdd" BehaviorID="ModelPopup" BackgroundCssClass="modal-background"
                            CancelControlID="btnClose">
                        </cc1:ModalPopupExtender>
                        <asp:Panel ID="pnlParameters" runat="server" Style="display: none;" DefaultButton="btnSave">
                            <div class="modal-dialog" style="width: 830px; margin-left: 0px">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" id="btnClose" class="close" runat="server" OnClick="btnClose_Click">
                                            <span>&times;</span><span class="sr-only">Close</span></button>
                                        <h1 class="modal-title" id="myModalLabel" runat="server">
                                            <span></span>Add New Customer</h1>
                                    </div>
                                    <div class="modal-body">
                                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:RadioButtonList ID="RbCustomerType" RepeatDirection="Horizontal" OnSelectedIndexChanged="RbCustomerType_SelectedIndexChanged" AutoPostBack="true" runat="server" TextAlign="Right">
                                                            <asp:ListItem Text=" Customer" Value="1" Selected="True" style="margin-right: 22px;"></asp:ListItem>
                                                            <asp:ListItem Text=" Customer & Vendor" Value="2"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label><br />
                                                    </div>
                                                    <asp:HiddenField ID="hfStatus" runat="server" />
                                                    <asp:HiddenField ID="hfCustomerID" runat="server" />
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-4">
                                                        <label id="lbldesignationID"><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                                        <asp:DropDownList ID="ddDistributorId" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label id="lblID"><span class="fa fa-caret-right rgt_cart"></span>Customer Type</label>
                                                        <asp:DropDownList ID="DrpType" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="1">Local</asp:ListItem>
                                                            <asp:ListItem Value="2">Foreign</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label id="lblIDSub"><span class="fa fa-caret-right rgt_cart"></span>Customer Sub Type</label>
                                                        <asp:DropDownList ID="DrpSubType" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="1">Trade Customer</asp:ListItem>
                                                            <asp:ListItem Value="2">Other Customer</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-8">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Name</label>
                                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control "></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Registraton No.</label>
                                                        <asp:TextBox ID="txtContact2" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>CNIC</label>
                                                        <asp:TextBox ID="txtCNIC" runat="server" CssClass="form-control "></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Custom"
                                                            TargetControlID="txtCNIC" ValidChars="0123456789-"></cc1:FilteredTextBoxExtender>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Primary Contact</label>
                                                        <asp:TextBox ID="txtContact" runat="server" CssClass="form-control "></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="ftetxtPhoneNumber" runat="server" FilterType="Custom"
                                                            TargetControlID="txtContact" ValidChars="0123456789+-"></cc1:FilteredTextBoxExtender>
                                                    </div>
                                                    
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Email</label>
                                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control "></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                            ErrorMessage="Invalid Email" ControlToValidate="txtEmail" ValidationGroup="emailvalidate"
                                                            ValidationExpression="^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"
                                                            Display="Dynamic"></asp:RegularExpressionValidator>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>NTN No</label>
                                                        <asp:TextBox ID="txtNTNNO" runat="server" CssClass="form-control"
                                                            onkeypress="return onlyDotsAndNumbers(txt, event);"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-8">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Address</label>
                                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control "></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    
                                                    <div class="col-md-12">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                        <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Height="60px" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <div class="row">
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>
                                                    <div class="col-md-12" align="right">
                                                        <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" CausesValidation="true" ValidationGroup="emailvalidate" />
                                                        <asp:Button ID="btnCancel" OnClick="btnCancel_Click" runat="server" Style="margin-left: 5px" Text="Cancel" CssClass="btn btn-danger" CausesValidation="false" ValidationGroup="emailvalidate" />
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <%--//////////////////////////////////////////////////////////////////////////////////////////////////--%>
            <div style="z-index: 101; left: 50%; width: 100px; position: absolute; top: 10px; height: 90px">
                &nbsp;<asp:Panel ID="Panel21" runat="server">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
                        <ProgressTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif" />
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                <ContentTemplate>
                    <div class="row center">
                        <div class="col-md-12">
                            <div class="emp-table">
                                <asp:GridView ID="GrdCustomer" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                    AllowPaging="true" AutoGenerateColumns="False"
                                    EmptyDataText="No Record exist" OnPageIndexChanging="GrdCustomer_PageIndexChanging"
                                    PageSize="8">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                            <ItemStyle Width="13%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Name" ReadOnly="true">
                                            <HeaderStyle Width="20%"></HeaderStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PRIMARY_CONTACT" HeaderText="Contact" ReadOnly="true">
                                            <ItemStyle Width="12%" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SECONDARY_CONTACT" HeaderText="Registration No." ReadOnly="true">
                                            <ItemStyle Width="13%" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ADDRESS" HeaderText="Address" ReadOnly="true"></asp:BoundField>
                                        <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VENDOR_ID" HeaderText="Status" ReadOnly="true">
                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                            <ItemStyle CssClass="HidePanel "></ItemStyle>
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("VENDOR_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit" class="fa fa-pencil">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="3%" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="cf head"></HeaderStyle>
                                    <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                    <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
