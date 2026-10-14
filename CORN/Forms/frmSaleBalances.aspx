<%@ Page Title="Sale Balances" Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="frmSaleBalances.aspx.cs" Inherits="Forms_frmSaleBalances" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script type="text/javascript" src="../css/jquery.min.js"></script>
    <script type="text/javascript" src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function pageLoad() {
            <%--$("#<%=ddlSup_Customer.ClientID %>").searchable();--%>
            
        }
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        
        function ErrorMessage(msg) {
            document.getElementById('<%=btnSave.ClientID%>').value = msg;
        }
        function ValidateForm() {
            var Amount = document.getElementById("<%= txtAmount.ClientID %>").value;
            if (Amount.length == 0 && Amount.length == "") {
                alert('Amount Must Enter');
                return false;
            }
            return true;
        }
        <%--function jsFunction(value) {
            if (value == 1) {
                document.getElementById('<%=Label1.ClientID%>').innerText = "Supplier";
            }
            else {
                document.getElementById('<%=Label1.ClientID%>').innerText = "Customer";
            }
        }--%>
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
        function CalculateNetAmount() {
            var Amount = document.getElementById('<%=txtAmount.ClientID%>').value;
            var DiscPercent = document.getElementById('<%=txtDiscPercent.ClientID%>').value;
            var netAmount = document.getElementById('<%=txtNetAmount.ClientID%>').value;
            var Discount = 0;
            var NetAmount = 0;
            if (DiscPercent == null || DiscPercent.length == 0 || DiscPercent == "") {
                DiscPercent = 0;
            }
            Discount = parseFloat((parseFloat(Amount) * parseFloat(DiscPercent)) / 100).toFixed(2);
            document.getElementById("<%= txtDiscountAmount.ClientID %>").value = Discount;
            NetAmount = parseFloat(parseFloat(document.getElementById("<%= txtAmount.ClientID %>").value) - parseFloat(document.getElementById("<%= txtDiscountAmount.ClientID %>").value)).toFixed(2);
            document.getElementById("<%= txtNetAmount.ClientID %>").value = NetAmount;
        }
        //$("input[id*=txtAmount]").mask("#,##0", { reverse: true });
        //$("input[id*=txtDiscPercent]").mask("#,##0", { reverse: true });
        //$("input[id*=txtDiscountAmount]").mask("#,##0", { reverse: true });
        //$("input[id*=txtNetAmount]").mask("#,##0", { reverse: true });
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <div class="row">
                <asp:Panel ID="PanelSearch" runat="server">
                    <div class="col-md-7" style="padding-bottom: 15px;">
                        <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Sale Balance" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
                    </div>
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
            </div>
            <asp:Panel ID="PanelData" runat="server">
                <div class="row">
                    <div class="col-md-2">
                        <label><span class="fa fa-caret-right rgt_cart"></span>Document Date</label>
                        <asp:TextBox ID="txtTransactionDate" Style="width: 215px !important;" runat="server" onkeyup="BlockEndDateKeyPress()" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-1" style="margin-top: 30px">
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                            Width="30px" Style="margin-left: 35px; margin-top: -4px;" />
                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ImageButton2"
                            TargetControlID="txtTransactionDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
                    </div>
                    <div class="col-md-3">
                        <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                        <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                    </div>

                </div>
                <div class="row">
                    <div class="col-md-3">
                        <label id="LabelSupplier" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                        <dx:ASPxComboBox ID="ddlCustomer" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                    </div>
                    <div class="col-md-3">
                        <label id="LabelAccountHead" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Vendor</label>
                        <dx:ASPxComboBox ID="ddlVendor" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Item Name</label>
                            <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Block Name</label>
                            <asp:TextBox ID="txtBlockName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Street Name</label>
                            <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                    <div class="col-md-6">
                        <label><span class="fa fa-caret-right rgt_cart"></span>Balance Type</label>
                        <dx:ASPxComboBox ID="ddlBalanceType" runat="server" CssClass="form-control">
                            <Items>
                                <dx:ListEditItem Value="1" Text="Sport City" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>
                    <div class="col-md-6">
                        <label id="lblAmount" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                        <asp:TextBox ID="txtAmount" runat="server" onkeypress="return onlyDotsAndNumbers(this,event);" onkeyup="CalculateNetAmount(this.value)" CssClass="form-control" Style="text-align: right;"></asp:TextBox>
                    </div>
                        </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Discount (%)</label>
                            <asp:TextBox ID="txtDiscPercent" onkeypress="return onlyDotsAndNumbers(this,event);" runat="server" CssClass="form-control" onkeyup="CalculateNetAmount(this.value)" Style="text-align: right;">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Discount Amount</label>
                            <asp:TextBox ID="txtDiscountAmount" onkeypress="return onlyDotsAndNumbers(this,event);" runat="server" CssClass="form-control" Style="text-align: right;"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Net Amount</label>
                            <asp:TextBox ID="txtNetAmount" onkeypress="return onlyDotsAndNumbers(this,event);" runat="server" CssClass="form-control" Style="text-align: right;"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <label id="lblRemarks" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Reamarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" MaxLength="500" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4 ">
                        <asp:UpdatePanel ID="abc" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col-md-2 ">
                        <div class="btnlist pull-right">
                            <asp:Button ID="btnSave" runat="server" AccessKey="S" OnClick="btnSave_Click" Text="Save" CssClass=" btn btn-success" />
                            <asp:Button ID="btnCancel" runat="server" AccessKey="S" OnClick="btnCancel_Click" Text="Cancel" CssClass=" btn btn-danger" />
                        </div>
                    </div>

                </div>
            </asp:Panel>
            <asp:Panel ID="PanelGrid" runat="server">
                <div class="row">
                    <div class="col-md-12">
                        <asp:GridView ID="GrdInvoiceBooking" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                            HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="PurchaseMaster_PageIndexChanging" PageSize="50"
                            AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
                            <Columns>
                                <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" Width="13%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ITEM_DESC" HeaderText="Item Description" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" Width="25%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="TOTAL_SALE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Amount" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="10%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="TOTAL_DOWN_PAYMENT" DataFormatString="{0:N2}" HeaderText="Discount" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="10%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="TOTAL_BALANCE" DataFormatString="{0:N2}" HeaderText="Net Amount" ReadOnly="true">
                                    <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="10%"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="MASTER_ID" ReadOnly="true">
                                    <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                    <ItemStyle CssClass="HidePanel"></ItemStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemStyle CssClass="grdDetail" Width="3%" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemStyle CssClass="grdDetail" Width="3%" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("MASTER_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                            <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>
            <%--</ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>

    </div>
</asp:Content>
