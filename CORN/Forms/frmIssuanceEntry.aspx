<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmIssuanceEntry.aspx.cs" Inherits="Forms_frmIssuanceEntry" Title="Open Form Issuance" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">

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
            var TotalCommission = 0;
            var TotalBalance = 0;
            for (var i = 0; i < grid.rows.length - 1; i++) {
                var txtPurchaseAmount = $("input[id*=txtPurchaseAmount]")
                var txtDiscount = $("input[id*=txtDiscount]")
                //var txtCommission = $("input[id*=txtCommission]")
                var txtBalance = $("input[id*=txtBalance]")
                txtBalance[i].value = parseInt(txtPurchaseAmount[i].value) - parseInt(txtDiscount[i].value);
                TotalPurchaseAmount += parseInt(txtPurchaseAmount[i].value);
                TotalDiscount += parseInt(txtDiscount[i].value);
                //TotalCommission += parseInt(txtCommission[i].value);
                TotalBalance += parseInt(txtBalance[i].value);
                //alert(TotalPurchaseAmount);
                //document.getelementbyid('#lblTotalPurchaseAmount').value = TotalPurchaseAmount;
                ////$('#lblTotalPurchaseAmount').val(TotalPurchaseAmount);
                //$('#lblTotalDiscount').val(TotalDiscount);
                //$('#lblTotalCommission').val(TotalCommission);
                //$('#lblTotalBalance').val(TotalBalance);

                $('.TotalPurchaseAmount').html(TotalPurchaseAmount);
                $('.TotalDiscount').html(TotalDiscount);
                //$('.TotalCommission').html(TotalCommission);
                $('.TotalBalance').html(TotalBalance);
            }
        }
    </script>
    <div class="col-md-7" style="padding-bottom: 15px;">
        <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Purchase" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
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
                            <%--<div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-12">
                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                            <ContentTemplate>
                                                <dx:ASPxGridView ID="grid" runat="server" KeyFieldName="ITEM_INFO_ID" Width="100%"
                                                    AutoGenerateColumns="false" OnHeaderFilterFillItems="grid_HeaderFilterFillItems" Settings-ShowFooter="true">
                                                    <Columns>
                                                        <dx:GridViewDataColumn FieldName="PROJECT_NAME" Caption="Project" />
                                                        <dx:GridViewDataColumn FieldName="ITEM_NAME" Caption="Item Name" HeaderStyle-CssClass="myclass">
                                                            <HeaderStyle HorizontalAlign="Left" CssClass="myclass"></HeaderStyle>
                                                        </dx:GridViewDataColumn>
                                                        <dx:GridViewDataColumn FieldName="MEASURMENT" Caption="Measurement" />
                                                        <dx:GridViewDataColumn FieldName="BLOCK_NAME" Caption="Block" />
                                                        <dx:GridViewDataColumn FieldName="STREET_NAME" Caption="Street" />
                                                        <dx:GridViewDataColumn FieldName="PRECENT_NAME" Caption="Precent" />
                                                        <dx:GridViewDataColumn FieldName="PRECENTSTREET_NAME" Caption="Precent Street" />
                                                        <dx:GridViewDataColumn FieldName="OPEN_FROM_REGISTRATION" Caption="O.F Reg.#" />
                                                        <dx:GridViewDataColumn FieldName="TOWER_NO" Caption="Tower.#" />
                                                        <dx:GridViewDataColumn FieldName="FLOOR_NO" Caption="Floor.#" />
                                                        <dx:GridViewDataTextColumn Caption="Action" Width="4%" HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                                                            <DataItemTemplate>
                                                                <asp:LinkButton ID="lnkBtn" runat="server" CommandArgument='<%# Eval("ITEM_INFO_ID") %>' OnClick="lnkBtn_Click">Select</asp:LinkButton>
                                                            </DataItemTemplate>
                                                        </dx:GridViewDataTextColumn>
                                                    </Columns>
                                                    <Styles>
                                                        <AlternatingRow Enabled="true" />
                                                    </Styles>
                                                    <SettingsPopup>
                                                        <HeaderFilter Height="200"></HeaderFilter>
                                                    </SettingsPopup>
                                                    <Settings ShowFooter="true" ShowHeaderFilterButton="true" />
                                                </dx:ASPxGridView>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:PostBackTrigger ControlID="grid" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>--%>

                            <br />
                            <br />
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="col-md-12">
                                        <asp:HiddenField ID="RowId" runat="server" Value="-1" />
                                        <asp:Panel ID="PanelItems" Visible="false" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>

                                                    <asp:GridView ID="ASPxGridView1" CssClass="table table-striped table-bordered table-hover table-condensed cf" Style="border-spacing: 1px; min-width: 100%; margin-left: auto; margin-right: auto;" runat="server"
                                                        CellPadding="4" AutoGenerateColumns="false" DataKeyNames="ITEM_INFO_ID" GridLines="Both"
                                                        ShowFooter="false" PageSize="20" AllowPaging="True" EmptyDataText="No Record exist">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Font-Bold="true">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="ChbIsAssigned" runat="server" />
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="3%" />
                                                                <ItemStyle HorizontalAlign="Center" Width="3%" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="ITEM_INFO_ID" HeaderText="ITEM_INFO_ID" />
                                                            <asp:BoundField DataField="ITEM_INFO_NAME" HeaderText="Item Description" HeaderStyle-Font-Bold="true" ItemStyle-Font-Bold="false" ControlStyle-Width="45%" />
                                                            <asp:BoundField DataField="PurchaseAmount" DataFormatString="{0:N2}" HeaderText="Item Purchased Amount" HeaderStyle-Font-Bold="true" ItemStyle-Font-Bold="false" ControlStyle-Width="20%" ItemStyle-HorizontalAlign="Right" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="Discount" DataFormatString="{0:N2}" HeaderText="Discount" HeaderStyle-Font-Bold="true" ItemStyle-Font-Bold="false" ControlStyle-Width="45%" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="Balance" DataFormatString="{0:N2}" HeaderText="Balance" HeaderStyle-Font-Bold="true" ItemStyle-Font-Bold="false" ControlStyle-Width="45%" />
                                                            <%--<asp:TemplateField HeaderText="Item Amount" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" HeaderStyle-Height="30px" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPurchaseAmount" Width="100%" Class="lblPurchaseAmount" Text='<%# Eval("PurchaseAmount") %>' Style="text-align: right;" runat="server" onkeydown="Calculation(this.value)" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalPurchaseAmount" Class="TotalPurchaseAmount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Discount" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtDiscount" Width="100%" Text='<%# Eval("Discount") %>' Style="text-align: right;" runat="server" onkeydown="Calculation(this.value)" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalDiscount" Class="TotalDiscount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>--%>
                                                            <%--<asp:TemplateField HeaderText="Commission Type" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="false" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="abc">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlCommissionType" Style="padding-top: 3px; padding-bottom: 3px;" class="form-control" runat="server" Text='<%# Eval("CommissionType") %>' Width="100%">
                                                                        <asp:ListItem Text="Percentage" Value="1" />
                                                                        <asp:ListItem Text="Value" Value="2" />
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Commission" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="false" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtCommission" Width="100%" Text='<%# Eval("Commission") %>' Style="text-align: right;" runat="server" onkeydown="Calculation(this.value)" onkeypress="return allowOnlyNumber(event);" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalCommission" Class="TotalCommission" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>--%>
                                                            <%--<asp:TemplateField HeaderText="Balance" HeaderStyle-Width="10%" HeaderStyle-Font-Bold="true" ControlStyle-CssClass="right" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="right">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtBalance" Width="100%" Text='<%# Eval("Balance") %>' ReadOnly="true" Style="text-align: right;" runat="server" onkeypress="return allowOnlyNumber(event);" MaxLength="20"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblTotalBalance" Class="TotalBalance" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="COMMISSION_PER" HeaderText="COMMISSION_PER" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="COMMISSION_VALUE" HeaderText="COMMISSION_VALUE" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="TOKEN_AMOUNT" HeaderText="TOKEN_AMOUNT" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="DOWN_PAYMENT" HeaderText="DOWN_PAYMENT" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="PROJECT_ID" HeaderText="PROJECT_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="CATEGORY_ID" HeaderText="CATEGORY_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="SUB_CATEGORY_ID" HeaderText="SUB_CATEGORY_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="TRANS_ID" HeaderText="TRANS_ID" />
                                                            <asp:BoundField ControlStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" DataField="TRANSDETAIL_ID" HeaderText="TRANSDETAIL_ID" />
                                                            <%--<asp:TemplateField HeaderText="Action" HeaderStyle-Font-Bold="false" HeaderStyle-Width="3%" HeaderStyle-HorizontalAlign="center">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%# Eval("ITEM_INFO_ID") %>' OnClick="btnDelete_Click" OnClientClick="javascript:return confirm('Are you sure you want to Delete?');return false;"
                                                                        CssClass=" fa fa-trash-o"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" Width="33px" />
                                                            </asp:TemplateField>--%>
                                                        </Columns>
                                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775"></AlternatingRowStyle>
                                                        <EditRowStyle BackColor="#999999"></EditRowStyle>
                                                        <FooterStyle BackColor="#d7d7d7" Font-Bold="True" ForeColor="Black"></FooterStyle>
                                                        <HeaderStyle BackColor="#d7d7d7" Font-Bold="True" ForeColor="Black"></HeaderStyle>
                                                        <PagerStyle HorizontalAlign="Center" BackColor="#284775" ForeColor="White"></PagerStyle>
                                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333"></RowStyle>
                                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333"></SelectedRowStyle>
                                                        <SortedAscendingCellStyle BackColor="#d7d7d7"></SortedAscendingCellStyle>
                                                        <SortedAscendingHeaderStyle BackColor="#d7d7d7"></SortedAscendingHeaderStyle>
                                                        <SortedDescendingCellStyle BackColor="#d7d7d7"></SortedDescendingCellStyle>
                                                        <SortedDescendingHeaderStyle BackColor="#d7d7d7"></SortedDescendingHeaderStyle>
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
                            <br />
                            <div class="row">
                                <div class="btnlist">
                                    <asp:Button ID="btnSave" Style="margin-left: 31px;" OnClick="btnSave_Click" runat="server" Text="Hand Over" CssClass="btn btn-success" />
                                    <asp:Button ID="btnCancel" Style="margin-right: 33px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
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
                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="PurchaseMaster_PageIndexChanging" PageSize="20"
                                    AllowPaging="True" EmptyDataText="No Record exist" ShowFooter="true">
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
                                        <asp:BoundField DataField="DATE" HeaderText="Date" ReadOnly="true" DataFormatString="{0:dd-MMM-yyyy}">
                                            <ItemStyle CssClass="grdDetail" Width="17%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="25%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Hand Over To" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="25%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PURCHASE_MASTER_ID" HeaderText="Ref.#" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="7%"></ItemStyle>
                                        </asp:BoundField>
                                        <%--<asp:BoundField DataField="ITEM_NAME" HeaderText="Item" ReadOnly="true">
                                                            <ItemStyle CssClass="grdDetail"></ItemStyle>
                                                        </asp:BoundField>--%>

                                        <asp:BoundField DataField="TOTAL_PURCHASE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Total Amount" ReadOnly="true">
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
