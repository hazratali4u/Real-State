<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true" CodeFile="RptSaleInvoices.aspx.cs" Inherits="Forms_RptSaleInvoices" Title="Sale Invoices" %>
<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
    <script language="JavaScript" type="text/javascript">
        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }
    </script>
    <div class="main-contents">
        <div class="container employee-infomation">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                        <div class="col-md-4">
                            <label style="visibility:hidden;"><span class="fa fa-caret-right rgt_cart"></span>Report Type</label>
                                <asp:RadioButtonList ID="RbtReportType" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Value="1" Text="Sales" Selected="True" style="margin-right:25px;"></asp:ListItem>
                                    <%--<asp:ListItem Value="2" Text="Sales Return"></asp:ListItem>--%>
                                </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>Location</label>
                            <dx:ASPxComboBox ID="drpDistributor" runat="server" CssClass="form-control" OnSelectedIndexChanged="drpDistributor_SelectedIndexChanged" AutoPostBack="true">
                            </dx:ASPxComboBox>

                            <label><span class="fa fa-caret-right rgt_cart"></span>Customer</label>
                            <dx:ASPxComboBox ID="DrpPrincipal" runat="server" CssClass="form-control">
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>From Date</label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <label><span class="fa fa-caret-right rgt_cart"></span>To Date</label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass=" form-control" MaxLength="10"></asp:TextBox>
                        </div>
                        <div class="col-md-1" style="margin-top: 27px">
                            <asp:ImageButton ID="ibnEndDate" runat="server" ImageUrl="~/App_Themes/Granite/Images/date.gif"
                                Width="30px" />
                        </div>
                    </div>
                    <cc1:CalendarExtender ID="CEStartDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibtnStartDate"
                        TargetControlID="txtStartDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                    <cc1:CalendarExtender ID="CEEndDate" runat="server" Format="dd-MMM-yyyy" PopupButtonID="ibnEndDate"
                        TargetControlID="txtEndDate" OnClientShown="calendarShown" PopupPosition="TopLeft"></cc1:CalendarExtender>
                    <div class="row">
                        <div class="col-md-offset-1 col-md-3 ">
                            <div class="btnlist pull-right">
                                <asp:Button ID="btnGetData" runat="server" OnClick="btnGetData_Click" Text="Get Data" CssClass=" btn btn-success" />
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="Panel1" runat="server" Height="200px" ScrollBars="Vertical" BorderColor="Silver" BorderStyle="Groove" BorderWidth="1px" Width="100%">
                                <asp:GridView ID="GrdInvoices" runat="server" AutoGenerateColumns="False" Width="100%" CssClass="table table-striped table-bordered table-hover table-condensed cf">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="INVOICE_ID" HeaderText="Invoice No">
                                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="INVOICE_DATE" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}">
                                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DISTRIBUTOR_NAME" HeaderText="Location">
                                            <ItemStyle HorizontalAlign="Left" Width="20%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VENDOR_NAME" HeaderText="Vendor Name">
                                            <ItemStyle HorizontalAlign="Left" Width="20%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="INVOICE_AMOUNT" HeaderText="Sale Amount" DataFormatString="{0:n2}">
                                            <ItemStyle HorizontalAlign="Right" Width="12%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ADD_DISCOUNT" HeaderText="Discount (+)" DataFormatString="{0:n2}">
                                            <ItemStyle HorizontalAlign="Right" Width="12%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LESS_DISCOUNT" HeaderText="Discount (-)" DataFormatString="{0:n2}">
                                            <ItemStyle HorizontalAlign="Right" Width="12%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NET_AMOUNT" HeaderText="Net Amount" DataFormatString="{0:n2}">
                                            <ItemStyle HorizontalAlign="Right" Width="15%" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="cf head" />
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </div>
                    
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
                    <div class="row">
                <div class="col-md-offset-1 col-md-3 ">
                    <div class="btnlist pull-left">
                        <asp:HiddenField ID="opType" runat="server" Value="0" />
                        <asp:Button ID="btnViewPDF" runat="server" OnClick="btnViewPDF_Click" Text="View PDF" CssClass=" btn btn-success" Style="margin-left: -135px;" />
                        <asp:Button ID="btnViewExcel" runat="server" OnClick="btnViewExcel_Click" Text="View Excel" CssClass=" btn btn-success" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
