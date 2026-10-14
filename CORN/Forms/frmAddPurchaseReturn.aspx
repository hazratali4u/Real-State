<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddPurchaseReturn.aspx.cs" Inherits="Forms_frmAddPurchaseReturn" Title="Purchase Return" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.13.4/jquery.mask.min.js"></script>
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
        $("input[id*=txtAmountReceiveable]").mask("#,##0.00", { reverse: true });
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
                <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add Purchase Return" Width="30px" OnClick="AddPopUp_Click" />
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
                                    <%--<asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>--%>
                                            <div class="row">
                                                <div class="col-md-9">
                                                    <div class="col-md-3">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Transaction Date</label>
                                                        <asp:TextBox ID="txtPurchaseDate" Style="width: 300px !important;" runat="server" CssClass="form-control"></asp:TextBox>
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
                                                        <asp:DropDownList ID="ddlLocation" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlLocation" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                        <asp:DropDownList ID="ddlProject" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlProject" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>--%>
                                                    </div>

                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Vendor</label>
                                                        <asp:DropDownList ID="ddlCustomer" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlCustomer" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-8">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Item</label>
                                                        <asp:DropDownList ID="ddlItem" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <%--<dx:ASPxComboBox ID="ddlItem" runat="server" Width="100%" CssClass="form-control" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>--%>
                                                    </div>
                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Purchase Amount</label>
                                                        <asp:TextBox ID="txtSaleAmount" ReadOnly="true" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Amount Payable</label>
                                                        <asp:TextBox ID="txtAmountReceiveable" onkeyup="MyFunction()" Style="text-align: right;" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-md-4" style="display: none;">
                                                        <label><span class="fa fa-caret-right rgt_cart"></span>Difference</label>
                                                        <asp:TextBox ID="txtDifference" ReadOnly="true" Style="text-align: right;" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <label style="margin-left: 0px;"><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                        <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 78px; margin-left: 0px;" Width="100%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3">
                                                    <fieldset style="width: 107%; margin-left: -27px; margin-top: 25px; height: 235px;" class="form-control">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <strong>Purchase Amount:</strong>
                                                                </td>
                                                                <td style="text-align: right; width: 120px;">
                                                                    <strong style="text-align: right;">
                                                                        <asp:Label runat="server" ID="lblPurchaseAmount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Discount (+):</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblDiscount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Discount (-):</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblLessDiscount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Rebate (-):</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblRebate"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>Net Amount:</strong>
                                                                </td>
                                                                <td style="text-align: right; border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblNetAmount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Token Amount:</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblTokenAmount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Down Payment:</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblDownPayment"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td style="border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>Payments:</strong>
                                                                </td>
                                                                <td style="text-align: right; border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblAmountReceived"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Total Paid Amount:</strong>
                                                                </td>
                                                                <td style="text-align: right;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblTotalPaidAmount"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>Balance:</strong>
                                                                </td>
                                                                <td style="text-align: right; border-bottom: 1px solid black; height: 25px;">
                                                                    <strong>
                                                                        <asp:Label runat="server" ID="lblBalance"></asp:Label>
                                                                    </strong>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <asp:HiddenField ID="hfRefID" runat="server"></asp:HiddenField>
                                                </div>
                                            </div>
                                            
                                        <%--</ContentTemplate>
                                    </asp:UpdatePanel>--%>
                                    <div class="row">
                                                <div class="col-md-12" style="margin-left: 30px; width: 97% !important;">
                                                    <div class="row">
                                                        <div class="btnlist" style="margin-top: 0%;">
                                                            <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                                            <asp:Button ID="btnCancel" Style="margin-right: 33px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                                            <asp:HiddenField ID="hfVoucher_No" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                </asp:Panel>

                            </div>

                            <div class="row" style="width: 100% !important; margin-left: 0px;">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panelgrid" runat="server">
                                            <asp:Panel ID="mastergrid" runat="server">
                                                <asp:GridView ID="GridSaleMaster" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="GridSaleMaster_PageIndexChanging" PageSize="20"
                                                    AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="SR.No" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="13%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="13%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="ITEM_NAME" HeaderText="Item" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Customer" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Purchase Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="10%" HorizontalAlign="Right"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_RETURN_AMOUNT" DataFormatString="{0:N2}" HeaderText="Receivable Amount" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail" Width="10%" HorizontalAlign="Right"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="PURCHASE_RETURN_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("PURCHASE_RETURN_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemStyle CssClass="grdDetail" Width="5%" HorizontalAlign="Center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("PURCHASE_RETURN_ID")%>' OnClick="btnDelete_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
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
