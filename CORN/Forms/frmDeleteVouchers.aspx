<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmDeleteVouchers.aspx.cs" Inherits="Forms_frmDeleteVouchers" Title="Delete Vouchers" %>
<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="javascript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
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
    </script>
    <style>
        .CenterPanel {
            align-items: center;
        }
    </style>

    <div class="main-contents">
        <div class="container employee-infomation">
            <%--<asp:UpdatePanel ID="UPDATEPANEL1" runat="server">
                <ContentTemplate>--%>
            <div class="row">
                <div class="col-md-8">
                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                    <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control">
                    </dx:ASPxComboBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <label><span class="fa fa-caret-right rgt_cart"></span>Voucher Type</label>
                    <dx:ASPxComboBox ID="DrpVoucherType" runat="server" CssClass="form-control"
                        AutoPostBack="True" OnSelectedIndexChanged="DrpVoucherType_SelectedIndexChanged">
                    </dx:ASPxComboBox>
                </div>
                <div class="col-md-4">
                    <label><span class="fa fa-caret-right rgt_cart"></span>Voucher Sub Type</label>
                    <dx:ASPxComboBox ID="DrpVoucherSubType" runat="server" CssClass="form-control">
                    </dx:ASPxComboBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3">
                    <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                    <asp:TextBox ID="txtStartDate" Width="285px" runat="server" CssClass="form-control" MaxLength="10" onkeyup="BlockStartDateKeyPress()"></asp:TextBox>
                </div>
                <div class="col-md-1" style="margin-top: 27px">
                    <asp:ImageButton ID="ibtnStartDate" Style="margin-left: 35px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                        Width="30px" />
                </div>
                <div class="col-md-3">
                    <label><span class="fa fa-caret-right rgt_cart"></span>To Date</label>
                    <asp:TextBox ID="txtEndDate" Width="285px" runat="server" CssClass="form-control" MaxLength="10" onkeyup="BlockEndDateKeyPress()"></asp:TextBox>
                </div>
                <div class="col-md-1" style="margin-top: 27px">
                    <asp:ImageButton ID="ibnEndDate" Style="margin-left: 35px;" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                        Width="30px" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <asp:Button ID="btnView" runat="server" CssClass="btn btn-success" OnClick="btnView_Click" Width="100px" Text="View" />
                </div>
            </div>
            <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                TargetControlID="txtStartDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
            <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                TargetControlID="txtEndDate" OnClientShown="calendarShown"></cc1:CalendarExtender>
            <%-- </ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>
        <br />
        <div class="row">
            <div class="col-md-12">
                <asp:Label ID="lblErrors" runat="server" Text="" Font-Bold="true"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:Label ID="lblError" runat="server" Text="" Font-Bold="true"></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="emp-table">
                    <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Auto" Width="100%" Style="margin-top: 10px">
                        <asp:GridView ID="GrdLedger" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-striped table-bordered table-hover table-condensed cf">
                            <%--OnRowEditing="GrdLedger_RowEditing"--%>
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPrevious" NextPageText="Next"
                                PreviousPageText="Previous" />
                            <Columns>
                                <%--<asp:TemplateField HeaderText="SR.#">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="grdDetail" HorizontalAlign="Center" Width="3px" />
                                    <ItemStyle CssClass="grdDetail" HorizontalAlign="Center" Width="3px" />
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                    </ItemTemplate>
                                    <ItemStyle Width="2%" HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="CenterPanel" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="VOUCHER_NO" HeaderText="Voucher No" ReadOnly="true">
                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Ledger_date" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Voucher Date" ReadOnly="true">
                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="80px" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CHEQUE_NO" HeaderText="Cheque No" ReadOnly="true">
                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="80px" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DEBIT" HeaderText="Voucher Amount" ReadOnly="true" DataFormatString="{0:n2}">
                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="80px" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" ReadOnly="true">
                                    <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="400px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="VOUCHER_TYPE_ID" HeaderText="VOUCHER_TYPE_ID" ReadOnly="true">
                                    <HeaderStyle CssClass="HidePanel" />
                                    <ItemStyle CssClass="HidePanel" />
                                </asp:BoundField>
                                <asp:BoundField DataField="VOUCHER_DATE" HeaderText="VOUCHER_DATE" ReadOnly="true">
                                    <HeaderStyle CssClass="HidePanel" />
                                    <ItemStyle CssClass="HidePanel" />
                                </asp:BoundField>
                                <%--<asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="fa fa-trash-o" CommandName="Edit" ToolTip="Delete" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="grdDetail" HorizontalAlign="Center" Width="40px" />
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Delete">
                                            <ItemStyle CssClass="grdDetail" Width="3%" HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("VOUCHER_NO")%>' OnClick="btnDelete_Click" ToolTip="Delete" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                <%--<asp:CommandField HeaderText="Print" ShowEditButton="True" EditText="Print">
                                                        <ItemStyle BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Width="40px" />
                                                    </asp:CommandField>--%>
                            </Columns>
                            <HeaderStyle CssClass="tblhead" />
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <br />
        <div id="DivButtons" runat="server" class="row">
                <div class="col-md-12">
                    <asp:Button ID="BtnRemove" runat="server" CssClass="btn btn-success" OnClick="BtnRemove_Click" Width="100px" Text="Delete" />
                    <asp:Button ID="BtnCancel" runat="server" CssClass="btn btn-success" OnClick="BtnCancel_Click" Width="100px" Text="Cancel" />
                </div>
            </div>

    </div>

</asp:Content>
