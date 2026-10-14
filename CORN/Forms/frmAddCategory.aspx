<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmAddCategory.aspx.cs" Inherits="Forms_frmAddCategory" Title="Add Category" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="Server">
    <script language="JavaScript" type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) {
            var oControl = args.get_postBackElement();
            oControl.value = "Wait...";
            oControl.disabled = true;
        }
        function ValidateForm() {
            var str;
            <%--str = document.getElementById('<%=txtCategoryName.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Category Name is required');
                return false;
            }--%>
        }
    </script>
    <style type="text/css">
        .cmp input {
            width: 90%;
        }

        .cmp select {
            width: 90%;
        }

        .list input {
            width: 20%;
        }

        .list {
            width: 90%;
        }

        .tblheading {
            background: #006699;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }

            .tblheading td {
                color: #ffffff;
                padding: 5px 5px 5px 5px;
            }
    </style>
    <%--<asp:UpdatePanel ID="UpdatePanel10" runat="server">
        <ContentTemplate>--%>

    <cc1:TabContainer ID="TabContainer1" runat="server" Height="590px" Width="75%"
        ActiveTabIndex="0">
        <cc1:TabPanel ID="TabPanel1" runat="server">
            <HeaderTemplate>
                Category
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="RefId" runat="server" Value="0" />
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSaveChannelType">

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Category Name</label>
                                        <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtRemarksCategory" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="lblErrorMsg" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="btnSaveChannelType" OnClick="btnSaveChannelType_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="btnCancel" OnClick="btnCancel_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="grdChannelData" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        AllowPaging="true" PageSize="9"
                                        AutoGenerateColumns="False" OnPageIndexChanging="grdChannelData_PageIndexChanging">

                                        <Columns>
                                            <asp:BoundField DataField="CATEGORY_NAME" HeaderText="Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true"></asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("CATEGORY_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteCategory" runat="server" CommandArgument='<%#Eval("CATEGORY_ID")%>' OnClick="btnDeleteCategory_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel2" runat="server">
            <HeaderTemplate>
                Sub Category
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" DefaultButton="btnSaveBusType">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Category Name</label>
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category Name</label>
                                    <asp:TextBox ID="txtSubCategoryName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtRemarksSubCategory" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <asp:Literal ID="lblErrorMsgDivsion" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" align="right">
                                    <asp:Button ID="btnSaveBusType" OnClick="btnSaveBusType_Click" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                    <asp:Button ID="btncancelDestype" OnClick="btncancelDestype_Click" runat="server" Style="margin-left: 5px" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdBusType" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdBusType_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="CATEGORY_NAME" HeaderText="Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SUB_CATEGORY_NAME" HeaderText="Sub Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditSubCategory" runat="server" CommandArgument='<%#Eval("SUB_CATEGORY_ID")%>' OnClick="btnEditSubCategory_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteSubCategory" runat="server" CommandArgument='<%#Eval("SUB_CATEGORY_ID")%>' OnClick="btnDeleteSubCategory_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel10" runat="server">
            <HeaderTemplate>
                Detail Category
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel19" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel9" runat="server" DefaultButton="btnSaveDetailCategory">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>
                            <%--<div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Category Name</label>
                                    <asp:DropDownList ID="ddlCatgoryMeasurment" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCatgoryMeasurment_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category</label>
                                    <asp:DropDownList ID="ddlSubCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>--%>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Detail Category Name</label>
                                    <asp:TextBox ID="txtDetailCategoryName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtDetailCategoryRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <asp:Literal ID="Literal8" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" align="right">
                                    <asp:Button ID="btnSaveDetailCategory" OnClick="btnSaveDetailCategory_Click" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                    <asp:Button ID="btnCancelDetailCategory" OnClick="btnCancelDetailCategory_Click" runat="server" Style="margin-left: 5px" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel20" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdDetailCategory" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdDetailCategory_PageIndexChanging">
                                        <Columns>
                                            <%--<asp:BoundField DataField="CATEGORY_NAME" HeaderText="Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SUB_CATEGORY_NAME" HeaderText="Sub Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="DETAIL_CATEGORY_NAME" HeaderText="Detail Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditDtailCategory" runat="server" CommandArgument='<%#Eval("DETAIL_CATEGORY_ID")%>' OnClick="btnEditDtailCategory_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteDeatilCategory" runat="server" CommandArgument='<%#Eval("DETAIL_CATEGORY_ID")%>' OnClick="btnDeleteDeatilCategory_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel6" runat="server">
            <HeaderTemplate>
                Item Type
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="HiddenField1" runat="server" Value="0" />
                        <asp:Panel ID="Panel5" runat="server" DefaultButton="BtnSaveInventoryType">

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Type</label>
                                        <asp:TextBox ID="txtInventoryType" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtInventoryTypeRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="Literal4" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="BtnSaveInventoryType" OnClick="BtnSaveInventoryType_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="BtnCancelInventoryType" OnClick="BtnCancelInventoryType_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdInventoryType" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        AllowPaging="true" PageSize="7"
                                        AutoGenerateColumns="False" OnPageIndexChanging="GrdInventoryType_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="INVENTORYTYPE_NAME" HeaderText="Item Type" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true"></asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditINVENTORYTYPE" runat="server" CommandArgument='<%#Eval("INVENTORYTYPE_ID")%>' OnClick="btnEditINVENTORYTYPE_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteINVENTORYTYPE" runat="server" CommandArgument='<%#Eval("INVENTORYTYPE_ID")%>' OnClick="btnDeleteINVENTORYTYPE_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel3" runat="server">
            <HeaderTemplate>
                Measurement
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <%--<asp:Panel ID="Panel3" runat="server" DefaultButton="BtnSaveMeasurment">--%>
                        <%--<div class="row">
                            <div class="col-md-10">
                            </div>
                        </div>--%>
                        <%--<div class="row">
                            <div class="col-md-6">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Inventory Type</label>
                                <asp:DropDownList ID="ddlInventoryType" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label><span class="fa fa-caret-right rgt_cart"></span>Villa Type</label>
                                <asp:DropDownList ID="ddlVillaType" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>--%>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="col-md-12">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Measurement</label>
                                    <asp:TextBox ID="txtMeasurement" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <%-- <div class="col-md-6">
                                <label><span class="fa fa-caret-right rgt_cart"></span>No. of Beds</label>
                                <asp:TextBox ID="txtMeasurementNo_ofBeds" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <asp:Literal ID="Literal1" runat="server" Visible="false"></asp:Literal>
                            </div>--%>
                                <div class="col-md-12">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtRemarksMeasurment" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="col-md-12" align="right">
                                    <asp:Button ID="BtnSaveMeasurment" OnClick="BtnSaveMeasurment_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                    <asp:Button ID="BtnCancelMeasurment" OnClick="BtnCancelMeasurment_Click" runat="server" Style="margin-left: 5px;margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                        </div>
                        <hr />
                        <%-- </asp:Panel>--%>
                    </ContentTemplate>
                    <%--<Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlCatgoryMeasurment" EventName="SelectedIndexChanged" />
                    </Triggers>--%>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridMeasurment" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="5" OnPageIndexChanging="GridMeasurment_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="MEASURMENT" HeaderText="Measurment" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditMeasurment" runat="server" CommandArgument='<%#Eval("MEASURMENT_ID")%>' OnClick="btnEditMeasurment_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteMeasurment" runat="server" CommandArgument='<%#Eval("MEASURMENT_ID")%>' OnClick="btnDeleteMeasurment_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel4" runat="server">
            <HeaderTemplate>
                Blocks
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server" DefaultButton="BtnBlockSave">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                    <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Block Name</label>
                                    <asp:TextBox ID="txtBlock" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtBlockRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <asp:Literal ID="Literal2" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" align="right">
                                    <asp:Button ID="BtnBlockSave" OnClick="BtnBlockSave_Click" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                    <asp:Button ID="BtnBlockCencal" OnClick="BtnBlockCencal_Click" runat="server" Style="margin-left: 5px" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridViewBlock" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="8" OnPageIndexChanging="GridViewBlock_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="35%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="BLOCK_NAME" HeaderText="Block" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditBlock" runat="server" CommandArgument='<%#Eval("BLOCK_ID")%>' OnClick="btnEditBlock_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteBlock" runat="server" CommandArgument='<%#Eval("BLOCK_ID")%>' OnClick="btnDeleteBlock_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel5" runat="server">
            <HeaderTemplate>
                Streets
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" DefaultButton="BtnStreetSave">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                    <asp:DropDownList ID="ddlProjectStreet" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProjectStreet_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Block</label>
                                    <asp:DropDownList ID="ddlBlockStreet" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Street Name</label>
                                    <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                    <asp:TextBox ID="txtRemarksStreet" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <asp:Literal ID="Literal3" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12" align="right">
                                    <asp:Button ID="BtnStreetSave" OnClick="BtnStreetSave_Click" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                    <asp:Button ID="BtnStreetCancel" OnClick="BtnStreetCancel_Click" runat="server" Style="margin-left: 5px" Text="Cancel" CssClass="btn btn-danger" />
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridViewStreet" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GridViewStreet_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="25%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="BLOCK_NAME" HeaderText="Block" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="STREET_NAME" HeaderText="Street" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditStreet" runat="server" CommandArgument='<%#Eval("STREET_ID")%>' OnClick="btnEditStreet_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteStreet" runat="server" CommandArgument='<%#Eval("STREET_ID")%>' OnClick="btnDeleteStreet_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel7" runat="server">
            <HeaderTemplate>
                Phase
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel6" runat="server" DefaultButton="BtnSavePhase">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Phase Name</label>
                                        <asp:TextBox ID="txtPhaseName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtPhaseRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>

                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-6">
                                        <asp:Literal ID="Literal5" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="BtnSavePhase" OnClick="BtnSavePhase_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="BtnCancelPhase" OnClick="BtnCancelPhase_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel14" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdPhase" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdPhase_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PHASE_NAME" HeaderText="Phase" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditPhase" runat="server" CommandArgument='<%#Eval("PHASE_ID")%>' OnClick="btnEditPhase_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeletePhase" runat="server" CommandArgument='<%#Eval("PHASE_ID")%>' OnClick="btnDeletePhase_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel8" runat="server">
            <HeaderTemplate>
                Sector
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel15" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel7" runat="server" DefaultButton="BtnSaveSector">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Sector Name</label>
                                        <asp:TextBox ID="txtSectorName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtSectorRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="Literal6" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="BtnSaveSector" OnClick="BtnSaveSector_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="BtnCancelSector" OnClick="BtnCancelSector_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdSector" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdSector_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="SECTOR_NAME" HeaderText="Sector" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditSector" runat="server" CommandArgument='<%#Eval("SECTOR_ID")%>' OnClick="btnEditSector_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteSector" runat="server" CommandArgument='<%#Eval("SECTOR_ID")%>' OnClick="btnDeleteSector_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel9" runat="server">
            <HeaderTemplate>
                Precent
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel17" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel8" runat="server" DefaultButton="BtnSavePrecent">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Precent Name</label>
                                        <asp:TextBox ID="txtPrecentName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtPrecentRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="Literal7" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="BtnSavePrecent" OnClick="BtnSavePrecent_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="BtnCancelPrecent" OnClick="BtnCancelPrecent_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel18" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdPrecent" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdPrecent_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PRECENT_NAME" HeaderText="Precent" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditPrecent" runat="server" CommandArgument='<%#Eval("PRECENT_ID")%>' OnClick="btnEditPrecent_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeletePrecent" runat="server" CommandArgument='<%#Eval("PRECENT_ID")%>' OnClick="btnDeletePrecent_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel11" runat="server">
            <HeaderTemplate>
                Precent Street
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel21" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel10" runat="server" DefaultButton="BtnSavePrecentStreet">
                            <div class="row">
                                <div class="col-md-10">
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-6">
                                    <label><span class="fa fa-caret-right rgt_cart"></span>Precent</label>
                                    <asp:DropDownList ID="ddlPrecent" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                    <div class="col-md-6">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Street</label>
                                        <asp:TextBox ID="txtPrecentStreet" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtPrecentStreetRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="Literal1" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="BtnSavePrecentStreet" OnClick="BtnSavePrecentStreet_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="BtnSavePrecentCencel" OnClick="BtnSavePrecentCencel_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel22" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdPrecentStreet" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        HorizontalAlign="Center" AutoGenerateColumns="False" AllowPaging="true" PageSize="9" OnPageIndexChanging="GrdPrecentStreet_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PRECENT_NAME" HeaderText="Precent" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail" Width="20%"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="PRECENTSTREET_NAME" HeaderText="Street" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditPrecentStreet" runat="server" CommandArgument='<%#Eval("PRECENTSTREET_ID")%>' OnClick="btnEditPrecentStreet_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeletePrecentStreet" runat="server" CommandArgument='<%#Eval("PRECENTSTREET_ID")%>' OnClick="btnDeletePrecentStreet_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="TabPanel12" runat="server">
            <HeaderTemplate>
                Facing Type
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel23" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="HiddenField2" runat="server" Value="0" />
                        <asp:Panel ID="Panel11" runat="server" DefaultButton="btnFacingType">

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Type</label>
                                        <asp:TextBox ID="txtFacingType" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                        <asp:TextBox ID="txtFacingRemarks" runat="server" MaxLength="250" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="col-md-12">
                                        <asp:Literal ID="Literal9" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                    <div class="col-md-12" align="right">
                                        <asp:Button ID="btnFacingType" OnClick="btnFacingType_Click" Style="margin-top: 101px;" runat="server" Text="Save" ValidationGroup="vg" CssClass="btn btn-success" />
                                        <asp:Button ID="btnFacingTypeCancel" OnClick="btnFacingTypeCancel_Click" runat="server" Style="margin-left: 5px; margin-top: 101px;" Text="Cancel" CssClass="btn btn-danger" />
                                    </div>
                                </div>
                            </div>
                            <hr />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                    <div class="col-md-12">
                        <div class="emp-table">
                            <asp:UpdatePanel ID="UpdatePanel24" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GrdFacing" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                        AllowPaging="true" PageSize="9"
                                        AutoGenerateColumns="False" OnPageIndexChanging="GrdFacing_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="FACING_TYPE_NAME" HeaderText="Category" ReadOnly="true">
                                                <ItemStyle CssClass="grdDetail"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="REMARKS" HeaderText="Remarks" ReadOnly="true"></asp:BoundField>
                                            <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditFacing" runat="server" CommandArgument='<%#Eval("FACING_TYPE_ID")%>' OnClick="btnEditFacing_Click" ToolTip="Edit" class="fa fa-pencil">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" CssClass="grdDetail" Width="7%" />
                                                <HeaderStyle HorizontalAlign="Center" CssClass="grdHead" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemStyle CssClass="grdDetail" Width="7%" HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDeleteFacing" runat="server" CommandArgument='<%#Eval("FACING_TYPE_ID")%>' OnClick="btnDeleteFacing_Click" ToolTip="Edit" class="fa fa-trash-o" OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
    <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
