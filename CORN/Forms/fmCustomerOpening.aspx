<%@ Page Title="Customer Opening" Language="C#" MasterPageFile="~/Forms/PageMaster.master"
    AutoEventWireup="true" CodeFile="fmCustomerOpening.aspx.cs" Inherits="Forms_fmCustomerOpening" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script type="text/javascript" src="../css/jquery.min.js"></script>
    <script type="text/javascript" src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }

    </script>
    <script type="text/javascript">
        $("input[id*=txtOpeningBalance]").mask("#,##0", { reverse: true });
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
   <%-- <div class="col-md-7" style="padding-bottom: 15px;">
        <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Purchase" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
    </div>--%>
    <div class="main-contents">
        <div class="container employee-infomation">
            <div class="row top" style="margin-left: 15px !important; margin-right: 15px !important;">
                <div class="emp-table">
                    <div class="row">
                        <asp:Panel ID="PanelCustomerOpening" runat="server">
                            <asp:HiddenField ID="masterId" runat="server" Value="0" />
                            <div class="row">
                                <div class="col-md-4">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                    <dx:ASPxComboBox ID="drpDistributor" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" CssClass="form-control">
                                    </dx:ASPxComboBox>
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                                    <dx:ASPxComboBox ID="drpPrincipal" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="drpPrincipal_SelectedIndexChanged" CssClass="form-control">
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4" style="min-width: 120px;">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Opening Type</label>
                                    <asp:RadioButtonList ID="rblOpening" runat="server"
                                        RepeatDirection="Horizontal" Style="width: 348px; box-shadow: none; margin-bottom: 20px; border-radius: 0px !important; height: 34px; padding: 6px 12px; font-size: 14px; line-height: 1.42857143; color: #555; background-color: #fff; background-image: none; border: 1px solid #ccc;">
                                        <asp:ListItem Value="0" Text="Credit" Selected="True" style="margin-left: 15px; margin-top: 10px;"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Debit" style="margin-top: 10px;"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Date</label>
                                    <asp:TextBox ID="txtOpeningDate" Width="348px" runat="server" CssClass="form-control"></asp:TextBox>

                                </div>
                                <div class="col-md-1">
                                    <asp:ImageButton ID="ibOpeningDate" runat="server" Width="30px"
                                        Style="padding-top: 25px; margin-left: 128px;" ImageUrl="~/App_Themes/Granite/Images/date.gif"></asp:ImageButton>
                                    <cc1:CalendarExtender ID="ceOpeningDate" runat="server" TargetControlID="txtOpeningDate"
                                        PopupButtonID="ibOpeningDate" Format="dd-MMM-yyyy" OnClientShown="calendarShown"></cc1:CalendarExtender>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Balance</label>
                                    <asp:TextBox ID="txtOpeningBalance" Width="348px" Style="text-align: right;" runat="server" CssClass="form-control"
                                        onkeypress="return onlyDotsAndNumbers(this,event);"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtRemarks" Width="348px" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3 ">
                                    <asp:Button ID="btnSaveOpeningBalance" runat="server" OnClick="btnOpeningBalance_Click"
                                        CssClass="btn btn-success" Text="Save" />
                                    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click"
                                        CssClass="btn btn-danger" Text="Cancel" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="row">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                    <asp:Panel ID="PanelMaster" runat="server">
                        <div class="row">
                            <div class="col-md-7" style="padding-bottom: 15px;">
                                            <asp:ImageButton ID="AddPopUp" runat="server" ImageUrl="~/images/add-png.png" ToolTip="Add New Opening" Style="padding-bottom: 0px;" Width="30px" OnClick="AddPopUp_Click" />
                                        </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                                <dx:ASPxComboBox ID="drpDistributormaster" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="drpDistributormaster_SelectedIndexChanged" CssClass="form-control">
                                </dx:ASPxComboBox>
                                <label><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                                <dx:ASPxComboBox ID="drpPrincipalmaster" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="drpPrincipalmaster_SelectedIndexChanged" CssClass="form-control">
                                </dx:ASPxComboBox>
                            </div>
                        </div>
                        <div class="row">
                            <asp:GridView ID="GrdSalesMaster" runat="server" Width="98%" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                HorizontalAlign="Center" AutoGenerateColumns="False" PageSize="20"
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
                                    <asp:BoundField DataField="VENDOR_NAME" HeaderText="Customer" ReadOnly="true">
                                        <ItemStyle CssClass="grdDetail" Width="18%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="OPENING_TYPE" HeaderText="Opening Type" ReadOnly="true">
                                        <ItemStyle CssClass="grdDetail" HorizontalAlign="Left" Width="12%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TOTAL_SALE_AMOUNT" DataFormatString="{0:N2}" HeaderText="Amount" ReadOnly="true">
                                        <ItemStyle CssClass="grdDetail" HorizontalAlign="Right" Width="12%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IS_DELETED" HeaderText="Status" ReadOnly="true">
                                        <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                        <ItemStyle CssClass="HidePanel"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SALE_MASTER_ID" ReadOnly="true">
                                        <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                        <ItemStyle CssClass="HidePanel"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditMaster" runat="server" CommandArgument='<%#Eval("SALE_MASTER_ID")%>' OnClick="btnEditMaster_Click" ToolTip="Edit" class="fa fa-pencil">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                     </ContentTemplate>
                        </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

