<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddPurchaseEntryaspx.aspx.cs" Inherits="Forms_frmAddPurchaseEntryaspx" Title="Purchases" %>

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
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
        function Calculation() {
            var grid = document.getElementById("<%= ASPxGridView1.ClientID%>");
            var TotalPurchaseAmount = 0;
            var TotalDiscount = 0;
            var TotalLessDiscount = 0;
            var TotalBalance = 0;
            for (var i = 0; i < grid.rows.length - 1; i++) {

                var txtPurchaseAmount = $("input[id*=txtPurchaseAmount]")
                var txtDiscount = $("input[id*=txtDiscount]")
                var txtLessDiscount = $("input[id*=txtLessDiscount]")
                var txtBalance = $("input[id*=txtBalance]")
                var balance = parseInt(CheckComma(empty(txtPurchaseAmount[i].value))) + parseInt(CheckComma(empty(txtDiscount[i].value))) - parseInt(CheckComma(empty(txtLessDiscount[i].value)));
                txtBalance[i].value = numberWithCommas(balance);
                TotalPurchaseAmount += parseInt(CheckComma(empty(txtPurchaseAmount[i].value)));
                TotalDiscount += parseInt(CheckComma(empty(txtDiscount[i].value)));
                TotalLessDiscount += parseInt(CheckComma(empty(txtLessDiscount[i].value)));
                TotalBalance += parseInt(balance);
                $('.TotalPurchaseAmount').html(numberWithCommas(TotalPurchaseAmount));
                $('.TotalDiscount').html(numberWithCommas(TotalDiscount));
                $('.TotalLessDiscount').html(numberWithCommas(TotalLessDiscount));
                $('.TotalBalance').html(numberWithCommas(TotalBalance));
            }
            function empty(str) {
                if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "") {
                    return 0;
                }
                else {
                    return str;
                }
            }
            function isAlphanumeric(str) {
                return /^[0-9a-zA-Z]+$/.test(str);
            }
            function CheckComma(str) {
                if (isAlphanumeric(str)) {
                    return str;
                }
                else {
                    return str.replace(/,/g, '');
                }
            }
            function numberWithCommas(number) {
                var parts = number.toString().split(".");
                parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                return parts.join(".");
            }
        }
        $("input[id*=txtPurchaseAmount]").mask("#,##0", { reverse: true });
        $("input[id*=txtDiscount]").mask("#,##0", { reverse: true });
        $("input[id*=txtLessDiscount]").mask("#,##0", { reverse: true });
        $("input[id*=txtBalance]").mask("#,##0", { reverse: true });


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
    <div class="row">
        <asp:Panel ID="PanelSearch" runat="server">
        <div class="col-md-7" style="padding-bottom: 15px;">
            <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Purchase" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
        </div>
        <div class="col-md-4" style="text-align: right;">
            <div class="search">
                <asp:TextBox ID="txtSearchMasterGrid" runat="server" Style="margin-left: 48px;" placeholder="Search" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="col-md-1" style="text-align: right;">
            <asp:LinkButton ID="BtnMasterGridSearch" OnClick="BtnMasterGridSearch_Click" runat="server" Text="Search" Style="margin-left: -48px;"
                CssClass="btn btn-success"><i class="fa fa-search"  style="font-size:20px;"></i></asp:LinkButton>
        </div>
            </asp:Panel>
    </div>
    <div class="main-contents">
        <div class="container employee-infomation">
            <div class="row top" style="margin-left: 15px !important; margin-right: 15px !important;">
                <div class="emp-table">
                    <div class="row">
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
                                        <dx:ASPxComboBox ID="ddlLocation" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Vendor</label>
                                        <dx:ASPxComboBox ID="ddlVendor" runat="server" CssClass="form-control"></dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" style="width: 75% !important;">
                                    <div class="col-md-4">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Category</label>
                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                            <ContentTemplate>
                                                <dx:ASPxComboBox ID="ddlCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="col-md-4">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category</label>
                                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                            <ContentTemplate>
                                                <dx:ASPxComboBox ID="ddlSubCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="col-md-4" style="display: none;">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                            <ContentTemplate>
                                                <dx:ASPxComboBox ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></dx:ASPxComboBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" style="width: 75% !important;">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtRemarks" Width="100%" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-4">
                                        <div class="search">
                                            <asp:TextBox ID="txtSearch" runat="server" placeholder="Search" CssClass="form-control"
                                                TabIndex="0"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-2" style="margin-left: -60px;">
                                        <asp:LinkButton ID="btnsearch" OnClick="btnsearch_Click" runat="server" Text="Search"
                                            CssClass="btn btn-success"><i class="fa fa-search"  style="font-size:20px;"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-12">
                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                            <ContentTemplate>
                                                <asp:GridView ID="grid" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                    AllowPaging="true" AutoGenerateColumns="False" EmptyDataText="No Record exist" PageSize="10"
                                                    OnPageIndexChanging="grid_PageIndexChanging">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" CssClass="myclass" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project Name" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="ITEM_NAME" HeaderText="Item Name" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="MEASURMENT" HeaderText="Measurment" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="BLOCK_NAME" HeaderText="Block" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="STREET_NAME" HeaderText="Street" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="PRECENT_NAME" HeaderText="Precent" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="PRECENTSTREET_NAME" HeaderText="Precent Street" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="OPEN_FROM_REGISTRATION" HeaderText="O.F Reg.#" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="TOWER_NO" HeaderText="Tower.#" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="FLOOR_NO" HeaderText="Floor.#" ReadOnly="true"></asp:BoundField>
                                                        <asp:BoundField DataField="ITEM_INFO_ID" HeaderText="ITEM_INFO_ID" ReadOnly="true">
                                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                                            <ItemStyle CssClass="HidePanel "></ItemStyle>
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <PagerSettings PageButtonCount="5" NextPageText=">" PreviousPageText="<" />
                                                    <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="col-md-12" style="text-align: right;">
                                        <asp:Button ID="btnAdd" Style="margin-left: 31px;" Width="80px" OnClick="btnAdd_Click" runat="server" Text="Add" CssClass="btn btn-success" />
                                    </div>
                                </div>
                            </div>

                            <br />
                            <br />
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-12">
                                        <asp:HiddenField ID="RowId" runat="server" Value="-1" />
                                        <asp:Panel ID="PanelItems" Visible="false" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>

                                                    <asp:GridView ID="ASPxGridView1" Style="border-spacing: 1px; min-width: 100%; margin-left: auto; margin-right: auto;" runat="server"
                                                        CellPadding="4" AutoGenerateColumns="false" DataKeyNames="ITEM_INFO_ID" GridLines="Both"
                                                        CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                                        ShowFooter="true">
                                                        <Columns>
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="ITEM_INFO_ID" HeaderText="ITEM_INFO_ID" />
                                                            <asp:TemplateField HeaderText="SR.#" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="ITEM_INFO_NAME" HeaderText="Item Description" HeaderStyle-Font-Bold="true" ItemStyle-Font-Bold="false" ControlStyle-Width="45%" />
                                                            <asp:TemplateField HeaderText="Purchase Amount" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" HeaderStyle-Height="30px" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPurchaseAmount" Width="100%" Class="lblPurchaseAmount" Text='<%# Eval("PurchaseAmount") %>' Style="text-align: right;" runat="server" onkeyup="Calculation(this.value)" DataFormatString="{0:N2}" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalPurchaseAmount" Class="TotalPurchaseAmount" DataFormatString="{0:N2}" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Add Discount" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtDiscount" Width="100%" Text='<%# Eval("Discount") %>' Style="text-align: right;" runat="server" onkeyup="Calculation(this.value)" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalDiscount" Class="TotalDiscount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Less Discount" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtLessDiscount" Width="100%" Text='<%# Eval("LessDiscount") %>' Style="text-align: right;" runat="server" onkeyup="Calculation(this.value)" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalLessDiscount" Class="TotalLessDiscount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <%--<asp:TemplateField HeaderText="Commission Type" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="abc">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlCommissionType" Style="padding-top: 3px; padding-bottom: 3px;" class="form-control" runat="server" Text='<%# Eval("CommissionType") %>' Width="100%">
                                                                        <asp:ListItem Text="Percentage" Value="1" />
                                                                        <asp:ListItem Text="Value" Value="2" />
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Commission" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtCommission" Width="100%" Text='<%# Eval("Commission") %>' Style="text-align: right;" runat="server" onkeyup="Calculation(this.value)" onkeypress="return allowOnlyNumber(event);" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalCommission" Class="TotalCommission" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:TemplateField HeaderText="Balance" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtBalance" Width="100%" Text='<%# Eval("Balance") %>' ReadOnly="true" Style="text-align: right;" runat="server" onkeypress="return allowOnlyNumber(event);" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalBalance" Class="TotalBalance" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="COMMISSION_PER" HeaderText="COMMISSION_PER" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="COMMISSION_VALUE" HeaderText="COMMISSION_VALUE" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="TOKEN_AMOUNT" HeaderText="TOKEN_AMOUNT" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="DOWN_PAYMENT" HeaderText="DOWN_PAYMENT" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="PROJECT_ID" HeaderText="PROJECT_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="CATEGORY_ID" HeaderText="CATEGORY_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="SUB_CATEGORY_ID" HeaderText="SUB_CATEGORY_ID" />
                                                            <asp:TemplateField HeaderText="Action" HeaderStyle-Font-Bold="true" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="center">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%# Eval("ITEM_INFO_ID") %>' OnClick="btnDelete_Click" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                        CssClass=" fa fa-trash-o"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" Width="33px" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </asp:Panel>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                            <ContentTemplate>
                                                <table id="TotalTable" runat="server" width="100%" style="margin-top: 10px; display: none;">
                                                    <tr>
                                                        <td align="right">
                                                            <strong style="margin-right: 7px;">Purchase Amount:
                                                            </strong>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtTotalPurchaseAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td align="right">
                                                            <strong style="margin-right: 7px;">Add Discount Amount:
                                                            </strong>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtTotalDiscountAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td align="right">
                                                            <strong style="margin-right: 7px;">Less Discount Amount:
                                                            </strong>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtTotalLessDiscountAmount" onkeyup="JSFunc()" Style="text-align: right;" runat="server"></asp:TextBox>
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
                            <br />
                            <div class="row">
                                <div class="btnlist">
                                    <asp:Button ID="btnSave" Style="margin-left: 31px;" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                    <asp:Button ID="btnCancel" Style="margin-right: 33px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                    <asp:HiddenField ID="hfVoucher_No" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                    <div class="row">
                        <%--<asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>--%>
                        <asp:Panel ID="panelgrid" runat="server">
                            <asp:Panel ID="mastergrid" runat="server">
                                <asp:GridView ID="PurchaseMaster" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="PurchaseMaster_PageIndexChanging" PageSize="70"
                                    AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SR.#" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DATE" HeaderText="Date" ReadOnly="true" DataFormatString="{0:dd-MMM-yyyy}">
                                            <ItemStyle CssClass="grdDetail" Width="15%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="18%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="18%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PURCHASE_MASTER_ID" HeaderText="Ref.#" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="5%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TOTAL_PURCHASE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Purchase Amount" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TOTAL_DISCOUNT" DataFormatString="{0:N2}" HeaderText="Total Discount (+)" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TOTAL_LESS_DISCOUNT" DataFormatString="{0:N2}" HeaderText="Total Discount (-)" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BALANCE" DataFormatString="{0:N2}" HeaderText="Balance" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="15%"></ItemStyle>
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
                                                <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%#Eval("PURCHASE_MASTER_ID")%>' OnClick="LinkButton1_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                    <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                </asp:GridView>
                            </asp:Panel>
                        </asp:Panel>
                        <%-- </ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
