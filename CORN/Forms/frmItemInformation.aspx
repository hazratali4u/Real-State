<%@ Page Language="C#" MasterPageFile="~/Forms/PageMaster.master" AutoEventWireup="true"
    CodeFile="frmItemInformation.aspx.cs" Inherits="Forms_frmItemInformation" Title="Item Name / No" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphPage">
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
        function ValidateForm() {
            var str;
            str = document.getElementById('<%=ddlProject.ClientID%>').value;
            if (str == null || str.length == 0) {
                alert('Project name is required.');
                return false;
            }
            str = document.getElementById('<%=ddlCategory.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Category is required.');
            return false;
        }

        str = document.getElementById('<%=ddlSubCategory.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Sub Category is required.');
            return false;
        }
        str = document.getElementById('<%=txtItemName.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Item Name is required.');
            return false;
        }

       <%-- str = document.getElementById('<%=txtStreetNo.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Street no is required');
            return false;
        }--%>
       <%-- str = document.getElementById('<%=txtBlockNo.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Block no is required');
            return false;
        }--%>
        <%--str = document.getElementById('<%=txtpassword.ClientID%>').value;
        if (str == null || str.length == 0) {
            alert('Password is required.');
            return false;
        }--%>
            return true;
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
    <div class="main-contents">
        <div class="container employee-infomation">
            <div class="row center">
                <div class="col-md-5">
                        <asp:Label ID="lblMsg" runat="server" Text="" Style="color:red; font-size:medium; font-family:Verdana;" Visible="false"></asp:Label>
                    </div>
                </div>
            <div class="row top">
                <asp:Panel ID="Panel3" runat="server" DefaultButton="btnsearch">
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
                </asp:Panel>
                <div class="col-md-offset-5 col-md-3 ">
                    <div class="btnlist pull-right">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:LinkButton CssClass="btn btn-warning" runat="server" ID="btnAdd" Text="Add"
                                    OnClick="btnAdd_Click">
                               <span class="fa fa-plus-circle"></span>Add
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" ID="hbutton"></asp:LinkButton>
                                <asp:LinkButton ID="btnActive" runat="server" class="btn btn-success" OnClick="btnActive_Click"
                                    OnClientClick="javascript:return confirm('Are you sure you want to perform this action?');return false;">
                                    <span class="fa fa-check"></span>Active</asp:LinkButton>
                                <!-- POP UP MODEL-->
                                <cc1:ModalPopupExtender ID="mPopUpLocation" runat="server" PopupControlID="pnlParameters"
                                    TargetControlID="hbutton" BehaviorID="ModelPopup" BackgroundCssClass="modal-background"
                                    CancelControlID="btnClose">
                                </cc1:ModalPopupExtender>
                                <asp:Panel ID="pnlParameters" runat="server" Style="display: none;" ScrollBars="Auto" DefaultButton="btnSave">
                                    <div class="modal-dialog" style="width: 1030px; margin-left: 0px">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <button type="button" id="btnClose" class="close" runat="server" onserverclick="btnClose_ServerClick">
                                                    <span>&times;</span><span class="sr-only">Close</span></button>

                                                <h1 class="modal-title" id="myModalLabel">
                                                    <span></span>Add New Item Name / No</h1>
                                            </div>
                                            <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                                                <ContentTemplate>
                                                    <div class="modal-body">
                                                        <div class="row">
                                                            <div class="col-md-9">
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Project</label>
                                                                    <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Category</label>
                                                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Sub Category</label>
                                                                    <asp:DropDownList ID="ddlSubCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Detail Category</label>
                                                                    <asp:DropDownList ID="ddlDetailCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Inventory Type</label>
                                                                    <asp:DropDownList ID="ddlInventoryType" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Value="1">Available for Sale</asp:ListItem>
                                                                        <asp:ListItem Value="2">Own Inventory</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Item Type</label>
                                                                    <asp:DropDownList ID="ddlItemType" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Open Form Registration #</label>
                                                                    <asp:TextBox ID="txtOpenRegistration" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Item Name / No</label>
                                                                    <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Form No</label>
                                                                    <asp:TextBox ID="txtFormNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                
                                                                 
                                                            </div>

                                                            <div class="col-md-3" style="margin-top: 14px;">
                                                                <asp:Panel ID="Panel2" runat="server" style="margin-left:1px; margin-top:13px;" Width="213px" Height="195px" ScrollBars="Vertical"
                                        BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px">
                                                                        <div class="row">
                                                                            <asp:CheckBoxList ID="lstFacingType" style="margin-left:20px;"  runat="server" Width="150px" Height="194px"></asp:CheckBoxList>
                                                                        </div>
                                                                </asp:Panel>
                                                            </div>
                                                            
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Measurement</label>
                                                                    <asp:DropDownList ID="ddlMeasurment" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Block No</label>
                                                                     <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                                 <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Street No</label>
                                                                    <asp:DropDownList ID="ddlStreet" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                               <div class="col-md-3">
                                                                    <label Style="margin-left:10px;"><span class="fa fa-caret-right rgt_cart"></span>Phase</label>
                                                                    <asp:DropDownList ID="ddlPhase" Style="margin-left:10px;" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Sector</label>
                                                                    <asp:DropDownList ID="ddlSector" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Precent</label>
                                                                    <asp:DropDownList ID="ddlPrecent" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Precent Street</label>
                                                                    <asp:DropDownList ID="ddlPrecentStreet" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label Style="margin-left:10px;"><span class="fa fa-caret-right rgt_cart"></span>Villa Type</label>
                                                                    <asp:DropDownList ID="ddlVillaType" Style="margin-left:10px;" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Value="1" Text="Not Applicable" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Value="2" Text="Asian"></asp:ListItem>
                                                                        <asp:ListItem Value="3" Text="European"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Tower No</label>
                                                                    <asp:TextBox ID="txtTowerNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Floor No</label>
                                                                    <asp:TextBox ID="txtFloorNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>No. Of Beds</label>
                                                                    <asp:TextBox ID="txtNo_Of_Beds" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label Style="margin-left:10px;"><span class="fa fa-caret-right rgt_cart"></span>No. Of Floors</label>
                                                                    <asp:TextBox ID="txtNo_Of_Floor" Style="margin-left:10px;" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                                </div>
                                                                
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Extra Area</label>
                                                                    <asp:TextBox ID="txtExtraArea" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                           <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Paid Installment</label>
                                                                    <asp:DropDownList ID="ddlPaidInstallment" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Value="17">Not Applicable</asp:ListItem>
                                                                        <asp:ListItem Value="16">Fully Paid</asp:ListItem>
                                                                        <asp:ListItem Value="0">Zero</asp:ListItem>
                                                                        <asp:ListItem Value="1">One</asp:ListItem>
                                                                        <asp:ListItem Value="2">Two</asp:ListItem>
                                                                        <asp:ListItem Value="3">Three</asp:ListItem>
                                                                        <asp:ListItem Value="4">Four</asp:ListItem>
                                                                        <asp:ListItem Value="5">Five</asp:ListItem>
                                                                        <asp:ListItem Value="6">Six</asp:ListItem>
                                                                        <asp:ListItem Value="7">Seven</asp:ListItem>
                                                                        <asp:ListItem Value="8">Eight</asp:ListItem>
                                                                        <asp:ListItem Value="9">Nine</asp:ListItem>
                                                                        <asp:ListItem Value="10">Ten</asp:ListItem>
                                                                        <asp:ListItem Value="11">Eleven</asp:ListItem>
                                                                        <asp:ListItem Value="12">Twelve</asp:ListItem>
                                                                        <asp:ListItem Value="13">Thirteen</asp:ListItem>
                                                                        <asp:ListItem Value="14">Fourteen</asp:ListItem>
                                                                        <asp:ListItem Value="15">Fifteen</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label><span class="fa fa-caret-right rgt_cart"></span>Registered Name</label>
                                                                    <asp:TextBox ID="txtRegisteredName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            <div class="col-md-3">
                                                                <label style=""><span class="fa fa-caret-right rgt_cart"></span>Remarks</label>
                                                                <asp:TextBox ID="txtRemarks" runat="server" MaxLength="250" Style="height: 60px;" Width="102%" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                                </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                 <div class="col-md-6">
                                                                     <asp:CheckBox ID="ChkPossession" runat="server" OnCheckedChanged="ChkPossession_CheckedChanged" AutoPostBack="true" Text=" Possession" />
                                                                     <asp:CheckBox ID="ChkNonPossession" runat="server" Style="margin-left:15px;" OnCheckedChanged="ChkNonPossession_CheckedChanged" AutoPostBack="true" Text=" Non Possession" />
                                                                    <%-- <asp:CheckBoxList ID="ChkPossession" runat="server" OnSelectedIndexChanged="ChkPossession_SelectedIndexChanged" AutoPostBack="True" Width="432px">  
                                                                        <asp:ListItem Value="0" Selected="True">Possession</asp:ListItem>  
                                                                        <asp:ListItem Value="1">Non Possession</asp:ListItem>
                                                                    </asp:CheckBoxList>--%>  
                                                                     <%--<asp:checkboxlist ID="" runat="server" CellPadding="4" CellSpacing="4" OnSelectedIndexChanged="RbtPossession_SelectedIndexChanged" RepeatDirection="Horizontal">
                                                                        <asp:ListItem Value="0" Selected="True" style="margin-right: 22px;" Text="&nbsp;&nbsp;&nbsp;Possession"></asp:ListItem>
                                                                        <asp:ListItem Value="1" Text=" &nbsp;&nbsp;&nbsp;Non Possession"></asp:ListItem>
                                                                    </asp:checkboxlist>--%>
                                                                 </div>
                                                                 <div class="col-md-6">
                                                                    <div class="btnlist pull-right" style="margin-top: 0%">
                                                                        <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save" CssClass="btn btn-success" />
                                                                        <asp:Button ID="btnCancel" Style="margin-right: 12px;" OnClick="btnCancel_Click" runat="server" Text="Cancel" CssClass="btn btn-danger" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:PostBackTrigger ControlID="btnSave" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div class="row center">
                <div class="col-md-12">
                    <div class="emp-table">
                        <%--<asp:UpdatePanel ID="UpdatePanelDetail" runat="server">
                            <ContentTemplate>--%>
                                <asp:HiddenField ID="hfUserId" runat="server" Value="0" />
                                <asp:GridView ID="Grid_users" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed cf"
                                    HorizontalAlign="Center" AutoGenerateColumns="False" OnPageIndexChanging="Grid_users_PageIndexChanging" PageSize="75"
                                    AllowPaging="True" EmptyDataText="No Record exist">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChbIsAssigned" runat="server" onclick="Check_Click(this)" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="3%" />
                                            <ItemStyle Width="3%" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PROJECT_NAME" HeaderText="Project Name" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="10%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CATEGORY_NAME" HeaderText="Category" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="9%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SUB_CATEGORY_NAME" HeaderText="Sub Category" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="10%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DETAIL_CATEGORY_NAME" HeaderText="Detail Category" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="8%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ITEM_NAME" HeaderText="Item Name" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="14%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="INVENTORYTYPE_NAME" HeaderText="Inv Type" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="8%"></ItemStyle>
                                        </asp:BoundField>
                                         <asp:BoundField DataField="MEASUREMENT" HeaderText="Measurement" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="7%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BLOCK_NO" HeaderText="Block #" ReadOnly="true">
                                            <ItemStyle CssClass="grdDetail" Width="8%"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Active" HeaderText="Status" ReadOnly="true">
                                            <ItemStyle Width="4%" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ITEM_INFO_ID" ReadOnly="true">
                                            <HeaderStyle CssClass="HidePanel"></HeaderStyle>
                                            <ItemStyle CssClass="HidePanel"></ItemStyle>
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%#Eval("ITEM_INFO_ID")%>' OnClick="btnEdit_Click" ToolTip="Edit" class="fa fa-pencil">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings PageButtonCount="10" NextPageText=">" PreviousPageText="<" />
                                    <PagerStyle CssClass="GridPager" HorizontalAlign="Right" />
                                </asp:GridView>
                            <%--</ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
