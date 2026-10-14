<%@ Page Title="Vendor Entry" Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="frmChequeEntryVendor.aspx.cs" Inherits="Forms_frmChequeEntryVendor" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript"  src="../css/jquery.min.js"></script>
    <script type="text/javascript"  src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 100005;
        }
        $("input[id*=txtAmount]").mask("#,##0", { reverse: true });
        function CheckboxClick(Check) {
            var gridview = getElementById("GridChqPayOrder");
            var TotalAmount;
            for (i = 1; i < gridview.rows.length; i++) {
                gridview.rows[i].cells[0].getElementByTagName("INPUT")[0].checked = Check.checked;
                //Alert("asdfhjklasdf");
                //if () {
                //    TotalAmount += gridview.rows[i].cells[7].Text;
                //}
            }
            document.getElementById("<%= txtAmount.ClientID %>").value = TotalAmount;

        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }
        function ValidateForm() {
            var str;
            str = document.getElementById("<%= txtAmount.ClientID %>").value;
            if (str == null || str.length == 0) {
                alert('Amount is required');
                return false;
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
                        <div class="col-md-5">
                            <div class="col-md-12">
                                <asp:RadioButtonList ID="Hp" RepeatDirection="Horizontal" runat="server" TextAlign="Right" OnSelectedIndexChanged="Hp_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Purchase Payment" Value="1" Selected="True" style="margin-right: 22px;"></asp:ListItem>
                                    <%--<asp:ListItem Text="Insallment Purchase Payment" Value="2"></asp:ListItem>--%>
                                </asp:RadioButtonList>
                            </div>
                            <div class="row">
                                <div class="col-md-5">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Type</label>
                                    <dx:ASPxComboBox ID="ddlTransactionType" runat="server" SelectedIndex="0"
                                        CssClass="form-control" OnSelectedIndexChanged="ddlTransactionType_SelectedIndexChanged" AutoPostBack="true">
                                        <Items>
                                            <dx:ListEditItem Value="2" Text="Purchase"></dx:ListEditItem>
                                            <dx:ListEditItem Value="3" Text="Purchase Return"></dx:ListEditItem>
                                        </Items>
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-md-5">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Payment Type</label>
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
                            <div class="row">
                                <div class="col-md-10">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                    <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-md-10">
                                    <asp:Panel ID="PanelCrossPayment" runat="server" Visible="true">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Cross Payment</label>
                                        <dx:ASPxComboBox ID="ddlCrossPAyment" Enabled="false" runat="server" SelectedIndex="0"
                                            CssClass="form-control" OnSelectedIndexChanged="ddlCrossPAyment_SelectedIndexChanged" AutoPostBack="true">
                                            <Items>
                                                <dx:ListEditItem Value="1" Text="Chart of Account"></dx:ListEditItem>
                                                <%--<dx:ListEditItem Value="3" Text="Customer"></dx:ListEditItem>
                                                <dx:ListEditItem Value="2" Text="Supplier"></dx:ListEditItem>--%>
                                            </Items>
                                        </dx:ASPxComboBox>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-10">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Account Head</label>
                                    <dx:ASPxComboBox ID="DrpBankAccount" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="DrpBankAccount_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-10">
                                    <label id="lblFavouring" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Favouring</label>
                                    <asp:TextBox ID="txtFavouring" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-5">
                                    <label runat="server" id="lblChequeNo"><span class="fa fa-caret-right rgt_cart"></span>Cheque No</label>
                                    <asp:TextBox ID="txtChequeNumber" runat="server" CssClass="form-control"
                                        onkeypress="return onlyDotsAndNumbers(this,event);" Visible="false"></asp:TextBox>
                                    <dx:ASPxComboBox ID="drpCheque" runat="server" CssClass="form-control" OnSelectedIndexChanged="drpCheque_SelectedIndexChanged" AutoPostBack="true">
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-md-5">
                                    <label id="lblChequeDate" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Cheque Date</label>
                                    <asp:TextBox ID="txtStartDate" Style="margin-top: 0px;" runat="server" CssClass="form-control" Enabled="true"></asp:TextBox>
                                </div>
                                <div class="col-md-1" style="padding-top: 21px;">
                                    <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" Width="30px" Style="padding-top: 0px; margin-top: 5px;" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-5">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Amount</label>
                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"
                                        onkeypress="return onlyDotsAndNumbers(this,event);"></asp:TextBox>
                                </div>
                                <div class="col-md-5">
                                    <label runat="server" id="lblStatus"><span class="fa fa-caret-right rgt_cart"></span>Status</label>
                                    <dx:ASPxComboBox ID="DrpStatus" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="DrpStatus_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-10">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>

                                    <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <asp:Button AccessKey="S" ID="btnSave" OnClick="btnSave_Click" runat="server"
                                Text="Save" CssClass="btn btn-success" />

                            <asp:Button AccessKey="C" ID="btnCancel" runat="server"
                                Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-danger" />
                        </div>

                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart" style="margin-top: 34px;"></span>Supplier</label>
                                    <dx:ASPxComboBox ID="DrpVendor" runat="server" Style="margin-top: 0px;" CssClass="form-control"
                                        OnSelectedIndexChanged="DrpVendor_SelectedIndexChanged" AutoPostBack="True">
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-md-5">
                                    <strong>
                                        <asp:Literal runat="server" ID="Literal1"><span class="fa fa-caret-right rgt_cart" style="margin-top:34px;"></span>Transaction Date</asp:Literal></strong>
                                    <asp:TextBox ID="txtTransactionDate" Style="margin-top: 3px;" runat="server" CssClass="form-control" Enabled="true"></asp:TextBox>
                                </div>
                                <div class="col-md-1" style="padding-top: 21px;">
                                    <asp:ImageButton ID="ibtnTransactionDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif" Width="30px" Style="margin-top: 34px; margin-right: 10px;" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Panel ID="PanelItem" runat="server" Visible="false">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                        <dx:ASPxComboBox ID="ddlItem" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" AutoPostBack="true">
                                        </dx:ASPxComboBox>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
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
                                    <asp:Panel ID="Panel2" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                        BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                        <asp:GridView ID="GrdCreditReturn" runat="server" AutoGenerateColumns="False"
                                            Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                            DataKeyNames="PURCHASE_RETURN_ID" ShowFooter="true">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                    <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="PURCHASE_RETURN_ID">
                                                    <HeaderStyle CssClass="HidePanel" />
                                                    <ItemStyle CssClass="HidePanel" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="ITEM" HeaderText="Item Name">
                                                    <ItemStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="DATE" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}">
                                                    <ItemStyle Width="15%" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="CREDIT_AMOUNT" HeaderText="Credit Amount" DataFormatString="{0:n2}">
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
                                                <asp:BoundField DataField="ITEM_LOG_ID">
                                                    <HeaderStyle CssClass="HidePanel" />
                                                    <ItemStyle CssClass="HidePanel" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="cf head" />
                                        </asp:GridView>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel3" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                        BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                        <asp:GridView ID="GrdCreditInstallment" runat="server" AutoGenerateColumns="False"
                                            Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                            DataKeyNames="PURCHASE_INS_MASTER_ID">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                    <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="PURCHASE_INS_MASTER_ID">
                                                    <HeaderStyle CssClass="HidePanel" />
                                                    <ItemStyle CssClass="HidePanel" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="PURCHASE_INS_DETAIL_ID">
                                                    <HeaderStyle CssClass="HidePanel" />
                                                    <ItemStyle CssClass="HidePanel" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="PAYMENT_DESC" HeaderText="Payment Desc">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="INST_NO" HeaderText="Ins #">
                                                    <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="DUE_DATE" HeaderText="Due Date" DataFormatString="{0:dd-MMM-yyyy}">
                                                    <ItemStyle Width="15%" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="OS_AMOUNT" HeaderText="Due Amount" DataFormatString="{0:n2}">
                                                    <ItemStyle HorizontalAlign="Right" Width="25%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="DEBIT_AMOUNT">
                                                    <HeaderStyle CssClass="HidePanel" />
                                                    <ItemStyle CssClass="HidePanel" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="cf head" />
                                        </asp:GridView>
                                    </asp:Panel>
                                    <table width="100%" style="margin-top: 10px; margin-bottom: 10px; display:none;">
                                        <tr>
                                            <td style="width: 60%;" align="right">
                                                <strong>Total Credit Amount:
                                                </strong>
                                            </td>
                                            <td style="width: 30%;" align="right">
                                                <asp:TextBox ID="txtTotalCreditAmount" Style="text-align: right;" ReadOnly="true" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-12">
                                    <%--================================ Cross Payment Sale & Purchase Grid =====================================--%>
                                    <asp:Panel ID="CrossPanel1" runat="server" Width="100%">
                                        <asp:Panel ID="CrossPanel2" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                            BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                            <asp:GridView ID="CrossGrdCredit" runat="server" AutoGenerateColumns="False" ShowFooter="true"
                                                Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="MASTER_ID">
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
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                        </asp:Panel>
                                        <table width="100%" style="margin-top: 10px; display:none;">
                                            <tr>
                                                <td style="width: 60%;" align="right">
                                                    <strong>Total Credit Amount:
                                                    </strong>
                                                </td>
                                                <td style="width: 30%;" align="right">
                                                    <asp:TextBox ID="txtTotalCreditAmount2" Style="text-align: right;" ReadOnly="true" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <%--================================ End Cross Payment Sale & Purchase Grid =====================================--%>
                                    <%--================================ Open Cheque & Pay Order Grid =====================================--%>
                                    <asp:Panel ID="Panel4" runat="server" Width="100%">
                                        <asp:Panel ID="PanelOpenChqPayOrder" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver"
                                            BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                            <asp:GridView ID="GridChqPayOrder" runat="server" AutoGenerateColumns="False" ShowFooter="true"
                                                Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <%--<HeaderTemplate>
                                                            <asp:CheckBox ID="ChbIsAssignedHeader" onClick="CheckboxClick(this)" runat="server" />
                                                        </HeaderTemplate>--%>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChbIsAssigned" runat="server" OnCheckedChanged="ChbIsAssigned_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CHEQUE_PROCESS_ID">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VENDOR_NAME" HeaderText="Party Name">
                                                        <ItemStyle />
                                                    </asp:BoundField>
                                                        <asp:BoundField DataField="BANK_NAME" HeaderText="Bank Name">
                                                        <ItemStyle />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_NO" HeaderText="PO #">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_DATE" HeaderText="PO Date" DataFormatString="{0:dd-MM-yyyy}">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="FAVOURING" HeaderText="Favouring">
                                                        <ItemStyle HorizontalAlign="Left"/>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_AMOUNT" HeaderText="PO Amount" DataFormatString="{0:n2}">
                                                        <ItemStyle HorizontalAlign="Right" Width="15%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_TYPE">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                        </asp:Panel>
                                        <table width="100%" style="margin-top: 10px; display:none;">
                                            <tr>
                                                <td style="width: 60%;" align="right">
                                                    <strong>Total Credit Amount:
                                                    </strong>
                                                </td>
                                                <td style="width: 30%;" align="right">
                                                    <asp:TextBox ID="TextBox1" Style="text-align: right;" ReadOnly="true" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <%--================================ End Open Cheque & Pay Order Grid =====================================--%>
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:CheckBox ID="ChkIsDiscount" Text="%" runat="server" Visible="false" />
                    <asp:TextBox ID="txtDiscount" runat="server"
                        onkeypress="return onlyDotsAndNumbers(this,event);" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtTax" runat="server"
                        onkeypress="return onlyDotsAndNumbers(this,event);" Visible="false"></asp:TextBox>
                    <dx:ASPxComboBox ID="ddlAccountHead" runat="server" Visible="false">
                    </dx:ASPxComboBox>
                    <asp:Label ID="Label1" Visible="false" runat="server" Text="Received Date" CssClass="lblbox"></asp:Label>
                    <asp:TextBox ID="txtReceivedDate" runat="server" ReadOnly="True" Visible="false"></asp:TextBox>

                    <asp:TextBox ID="txtBankName" runat="server" Width="192px" CssClass="txtBox" Visible="false"></asp:TextBox>
                    <asp:Label runat="server" ID="lblAmount" Visible="false"></asp:Label>

                    <cc1:CalendarExtender ID="CEStartDate" OnClientShown="calendarShown" runat="server" Format="dd-MM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CalendarExtender1" OnClientShown="calendarShown" runat="server" Format="dd-MM-yyyy" PopupButtonID="ibtnTransactionDate"
                        TargetControlID="txtTransactionDate"></cc1:CalendarExtender>
                    <asp:HiddenField ID="HFChqueProcessId" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hfChequeNo" Value="" runat="server"></asp:HiddenField>

                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnSave" />
                </Triggers>
            </asp:UpdatePanel>
            <hr />
            <div class="row center">
                <div class="col-md-12" style="width: 91% !Important;">
                    <div class="emp-table">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-md-12">

                                        <div class="col-md-2">
                                            <strong>
                                                <label id="Label110"><span class="fa fa-caret-right rgt_cart"></span>Search Type</label></strong>
                                        </div>
                                        <div class="col-md-4">
                                            <dx:ASPxComboBox ID="ddSearchType" runat="server" CssClass="form-control" SelectedIndex="0">
                                                <Items>
                                                    <dx:ListEditItem Value="CHEQUE_NO" Text="All Records"></dx:ListEditItem>
                                                    <dx:ListEditItem Value="VENDOR_NAME" Text="Supplier"></dx:ListEditItem>
                                                    <dx:ListEditItem Value="account_name" Text="Account Head"></dx:ListEditItem>
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

                                        <asp:Panel ID="Panel12" runat="server" Width="110%" Height="200px" ScrollBars="Vertical"
                                            BorderWidth="1px" BorderStyle="Solid" BorderColor="Silver">

                                            <asp:GridView ID="GrdCheque" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                AutoGenerateColumns="False" OnRowEditing="GrdCheque_RowEditing"
                                                OnRowDeleting="GrdCheque_RowDeleting" ShowFooter="true">
                                                <Columns>
                                                    <asp:BoundField DataField="CHEQUE_PROCESS_ID" HeaderText="CHEQUE_PROCESS_ID" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VENDOR_ID" HeaderText="VENDOR_ID" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VENDOR_NAME" HeaderText="Supplier" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_NO" HeaderText="PO/Cheque No" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="account_name" HeaderText="Account Head" ReadOnly="true">
                                                        <ControlStyle />
                                                        <ItemStyle />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_DATE" HeaderText="PO/Chq.Date" DataFormatString="{0:dd-MM-yyyy}">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="RECEIVED_DATE" HeaderText="Paid Date" DataFormatString="{0:dd-MM-yyyy}">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_AMOUNT" DataFormatString="{0:n2}" HeaderText="Amount" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Remarks" HeaderText="Remarks" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="account_head_id" HeaderText="account_head_id" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SlipNo" HeaderText="SlipNo" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DISCOUNT_TYPE" HeaderText="Transaction_Type" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="TAX_ACCOUNT_ID" HeaderText="CrossPayment" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_TYPE" HeaderText="CHEQUE_TYPE" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="FAVOURING" HeaderText="FAVOURING" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server" ID="btnEdit" CommandName="Edit" Text="Edit" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                            <asp:GridView ID="GrdCO" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                AutoGenerateColumns="False"
                                                Visible="false" ShowFooter="true">
                                                <Columns>
                                                    <asp:BoundField DataField="VENDOR_ID" HeaderText="VENDOR_ID" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VENDOR_NAME" HeaderText="Supplier" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ITEM_NAME" HeaderText="Item Name" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Left" Width="30%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="VOUCHER_NO" HeaderText="Voucher No" ReadOnly="true">
                                                        <ControlStyle />
                                                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="account_name" HeaderText="Account Head" ReadOnly="true">
                                                        <ControlStyle />
                                                        <ItemStyle HorizontalAlign="Left" Width="18%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="RECEIVED_DATE" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CHEQUE_AMOUNT" DataFormatString="{0:n2}" HeaderText="Amount" ReadOnly="true">
                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                    </asp:BoundField>
                                                    <%--<asp:BoundField DataField="VOUCHER_NO" HeaderText="VOUCHER_NO" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DOCUMENT_NO" HeaderText="DOCUMENT_NO" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="TRANSACTION_TYPE" HeaderText="TRANSACTION_TYPE" ReadOnly="true">
                                                        <HeaderStyle CssClass="HidePanel" />
                                                        <ItemStyle CssClass="HidePanel" />
                                                    </asp:BoundField>--%>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemStyle CssClass="grdDetail" Width="4%" HorizontalAlign="Center"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("CUSTOMER_LEDGER_ID")%>' OnClick="btnDelete_Click" ToolTip="Delete" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle CssClass="cf head" />
                                            </asp:GridView>
                                        </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <table width="106% !Imprtant" style="display:none;">
                                    <tr>
                                        <td style="text-align: right;">
                                            <strong>Total Amount:&nbsp;&nbsp; </strong>
                                            <asp:Label ID="lblTotalAmount" runat="server" Style="text-align: right;" Text="0" Font-Bold="true"></asp:Label>
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
</asp:Content>
