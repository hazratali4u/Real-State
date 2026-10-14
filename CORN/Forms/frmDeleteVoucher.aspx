 <%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="frmDeleteVoucher.aspx.cs" Inherits="Forms_frmDeleteVoucher" Title="Voucher Entry" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <script type="text/javascript" src="../css/jquery.min.js"></script>
    <script type="text/javascript" src="../css/jquery.mask.min.js"></script>
    <script language="JavaScript" type="text/javascript">

        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }

        function ErrorMessage(msg) {
            document.getElementById('<%=btnDone.ClientID%>').value = msg;
        }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(startRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        function startRequest(sender, e) {
            document.getElementById('<%=btnDone.ClientID%>').disabled = true;
        }
        function endRequest(sender, e) {
            document.getElementById('<%=btnDone.ClientID%>').disabled = false;
        }
        <%--function ValidateForm() {
            var Debit = document.getElementById("<%= txtDebitAmount.ClientID %>").value;
            var Credit = document.getElementById("<%= txtCreditAmount.ClientID %>").value;

            if (Debit.length == 0 && Credit.length == 0) {
                alert('Debit or Credit Must Enter');
                return false;
            }


            if (Debit > 0 && Credit > 0) {
                alert('Only Debit or Credit can enter');
                return false;
            }
            return true;
        }--%>

    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <div style="z-index: 5000; left: 700px; width: 100px; position: absolute; top: 10%; height: 100px">
                &nbsp;<asp:Panel ID="Panel1" runat="server">
                    <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                        <ProgressTemplate>
                            <asp:ImageButton ID="uPImageButton" runat="server" ImageUrl="~/OrderPOS/images/wheel.gif" />
                            Wait....
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12 ">
                            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox runat="server" ID="drpDistributor" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Voucher Type</label>
                            <dx:ASPxComboBox runat="server" ID="DrpVoucherType" AutoPostBack="True"
                                OnSelectedIndexChanged="DrpVoucherType_SelectedIndexChanged" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3" id="SubType" runat="server">
                            <span id="spanlblSubType" runat="server" class="fa fa-caret-right rgt_cart"></span>
                            <asp:Label ID="lblSubType" runat="server" Text="Voucher Sub Type" Font-Bold="true" />
                            <dx:ASPxComboBox runat="server" ID="DrpVoucherSubType" NullText="Plz Select Voucher Sub Type" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label id="lblpayeesName" runat="server"><span class="fa fa-caret-right rgt_cart"></span>Voucher No's</label>
                            <asp:TextBox ID="txtpayeesName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3 btnlist pull-left">
                            <asp:Button ID="btnDone" runat="server" AccessKey="S" OnClick="btnDone_Click" Text="Delete" CssClass=" btn btn-success" Width="80px" Style="margin-right:15px;" />
                            <asp:Button ID="btnCancel" runat="server" AccessKey="C" OnClick="btnCancel_Click" Text="Cancel" CssClass=" btn btn-danger" Width="80px" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            
        </div>

    </div>
</asp:Content>
