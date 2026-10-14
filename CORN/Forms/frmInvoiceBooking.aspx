<%@ Page Title="Invoice Booking" Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="frmInvoiceBooking.aspx.cs" Inherits="Forms_frmInvoiceBooking" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../AjaxLibrary/jquery.searchabledropdown-1.0.8.min.js"></script>
    <script type="text/javascript"  src="../css/jquery.min.js"></script>
    <script type="text/javascript"  src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function pageLoad() {
            $("#<%=ddlSup_Customer.ClientID %>").searchable();
        }
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        $("input[id*=txtAmount]").mask("#,##0", { reverse: true });
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
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <%--<div style="z-index: 5000; left: 700px; width: 100px; position: absolute; top: 10%; height: 100px">
                &nbsp;<asp:Panel ID="Panel1" runat="server">
                    <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                        <ProgressTemplate>
                            <asp:ImageButton ID="uPImageButton" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif" />
                            Wait....
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </div>--%>
            <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>--%>
                    <div class="row">
                        <div class="col-md-6" style="margin-bottom: 15px;">
                            <asp:RadioButtonList ID="Hp" RepeatDirection="Horizontal" runat="server" TextAlign="Right" OnSelectedIndexChanged="Hp_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="Supplier Invoice" Value="2" Selected="True" style="margin-right: 50px;"></asp:ListItem>
                                <asp:ListItem Text="Customer Invoice" Value="1"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Panel ID="PanelSearch" runat="server">
                        <div class="col-md-7" style="padding-bottom: 15px;">
                            <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Purchase" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
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
                    <panel id="PanelData" runat="server">
                    <div class="row">
                         <div class="col-md-2">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Invoice Date</label>
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
                            <asp:DropDownList ID="drpDistributor" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label id="LabelSupplier" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Supplier</label>
                            <asp:DropDownList ID="ddlSup_Customer" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                         <div class="col-md-3">
                                    <label id="LabelAccountHead" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Debit Account Head</label>
                             <asp:DropDownList ID="DrpBankAccount" runat="server" CssClass="form-control"></asp:DropDownList>       
                                </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Invoice Number</label>
                            <%--<span id="spanInvoiceNumber" runat="server" class="fa fa-caret-right rgt_cart"></span>
                            <asp:Label ID="InvoiceNumber" runat="server" Text="Invoice Number" Font-Bold="true" />--%>
                            <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label id="lblAmount" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                            <asp:TextBox ID="txtAmount" runat="server" onkeypress="return onlyDotsAndNumbers(this,event);" CssClass="form-control" Style="text-align: right;"></asp:TextBox>
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
                        </panel>
                    <panel id="PanelGrid" runat="server">
                            <div class="row">
                                <div class="col-md-12">
                       <asp:GridView ID="GrdInvoiceBooking" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="PurchaseMaster_PageIndexChanging" PageSize="20"
                                                    AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
                                                    <Columns>
                                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <%--<asp:BoundField DataField="PROJECT_NAME" HeaderText="Project" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="27%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        
                                                        <asp:BoundField DataField="AMOUNT" DataFormatString="{0:N2}" HeaderText="Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                                        </asp:BoundField>
                                                       
                                                        <asp:BoundField DataField="MASTER_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
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
                          </panel>
                <%--</ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>

    </div>
</asp:Content>
