<%@ Page Title="Customer Advance Knock Off" Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="frmCustomerKnockOff.aspx.cs" Inherits="Forms_frmCustomerKnockOff" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript"  src="../css/jquery.min.js"></script>
    <script type="text/javascript"  src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 100005;
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
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
    <div class="main-contents">
        <div class="container employee-infomation">
            <div style="z-index: 101; left: 50%; width: 100px; position: absolute; top: 150px; height: 100px">
                <asp:Panel ID="Panel21" runat="server">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif" />
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-10">
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-4">

                                    <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                    <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>

                                    <label><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                                    <dx:ASPxComboBox ID="DrpVendor" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="DrpVendor_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>

                                    <label id="lblFavouring" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Reference No.</label>
                                    <asp:TextBox ID="txtReferenceNo" runat="server" CssClass="form-control"></asp:TextBox>

                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-md-12">
                                    <fieldset id="fieldset3" title="Purchaser Information" runat="server" class="form-control" style="min-height: 245px; width: 100%;">
                                        <legend style="font-family: Verdana">Advance Reciepts</legend>
                                        <asp:Panel ID="Panel2" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                            BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                            <asp:GridView ID="GridAdvance" runat="server" AutoGenerateColumns="False"
                                                Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                DataKeyNames="CUSTOMER_LEDGER_ID" ShowFooter="true">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CUSTOMER_LEDGER_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CUSTOMER_LEDGER_DATE" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}">
                                                        <ItemStyle Width="10%" HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VOUCHER_NO" HeaderText="Voucher No">
                                                        <ItemStyle Width="12%" HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="REMARKS" HeaderText="Remarks">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CREDIT" HeaderText="Amount" DataFormatString="{0:n2}">
                                                        <ItemStyle HorizontalAlign="Right" Width="13%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="KNOCKED_OFF" HeaderText="KNOCKED_OFF" DataFormatString="{0:n2}">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                        </asp:Panel>
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-md-12">
                                    <fieldset id="fieldset1" title="Purchaser Information" runat="server" class="form-control" style="min-height: 245px; width: 100%;">
                                        <legend style="font-family: Verdana">Pending Invoices</legend>
                                        <asp:Panel ID="Panel1" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                            BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                            <asp:GridView ID="GrdCredit" runat="server" AutoGenerateColumns="False"
                                                Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                DataKeyNames="PURCHASE_DETAIL_ID" ShowFooter="true">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="PURCHASE_DETAIL_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ITEM" HeaderText="Item Name">
                                                        <ItemStyle />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DATE" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}">
                                                        <ItemStyle Width="15%" HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DEBIT_AMOUNT" HeaderText="Credit Amount" DataFormatString="{0:n2}">
                                                        <ItemStyle HorizontalAlign="Right" Width="20%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="TYPE_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ITEM_INFO_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SALE_MASTER_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ITEM_LOG_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                        </asp:Panel>
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:Button AccessKey="S" ID="btnSave" OnClick="btnSave_Click" runat="server"
                                Text="Save" CssClass="btn btn-success" />

                            <asp:Button AccessKey="C" ID="btnCancel" runat="server"
                                Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-danger" />
                    <%--<div class="row">
                        <div class="col-md-12">

                        </div>
                        </div>--%>
                </ContentTemplate>
            </asp:UpdatePanel>
            <hr />
            <div class="row center">
                <div class="col-md-12" style="width: 91% !Important;">
                    <div class="emp-table">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="row" style="display:none;">
                                    <div class="col-md-12">

                                        <div class="col-md-2">
                                            <strong>
                                                <label id="Label110"><span class="fa fa-caret-right rgt_cart"></span>Search Type</label></strong>
                                        </div>
                                        <div class="col-md-4">
                                            <dx:ASPxComboBox ID="ddSearchType" runat="server" CssClass="form-control" SelectedIndex="0">
                                                <Items>
                                                    <dx:ListEditItem Value="CHEQUE_NO" Text="All Records"></dx:ListEditItem>
                                                    <dx:ListEditItem Value="VENDOR_NAME" Text="Customer"></dx:ListEditItem>
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:TextBox ID="txtSeach" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Button ID="btnFilter" runat="server" CssClass="btn box-primary" Text="Filter"
                                                OnClick="btnFilter_Click"></asp:Button>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                                            <ContentTemplate>
                                                <asp:Panel ID="Panel12" runat="server" Width="110%" Height="250px" ScrollBars="Vertical"
                                                    BorderWidth="1px" BorderStyle="Solid" BorderColor="Silver">
                                                    <asp:GridView ID="GrdCheque" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                        AutoGenerateColumns="False" OnRowEditing="GrdCheque_RowEditing"
                                                        OnRowDeleting="GrdCheque_RowDeleting" ShowFooter="true">
                                                        <Columns>
                                                            <asp:BoundField DataField="CUSTOMER_LEDGER_ID" HeaderText="CHEQUE_PROCESS_ID" ReadOnly="true">
                                                                <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="VENDOR_NAME" HeaderText="Customer" ReadOnly="true">
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                                <ControlStyle />
                                                                <ItemStyle />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="RECEIVED_DATE" HeaderText="Knock off Date" DataFormatString="{0:dd-MM-yyyy}">
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CHEQUE_AMOUNT" DataFormatString="{0:n2}" HeaderText="Amount" ReadOnly="true">
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Action">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="btnEdit" CommandName="Edit" Text="Edit" />
                                                                </ItemTemplate>
                                                                 <HeaderStyle CssClass="HidePanel" />
                                                                <ItemStyle CssClass="HidePanel" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="cf head" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
