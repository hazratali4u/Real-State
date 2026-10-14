using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmAddSale : System.Web.UI.Page
{
    private static int RowNo;
    DataTable SalesDetail;
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    IDbTransaction mTransaction;
    IDbConnection mConnection;
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            Hp.SelectedValue = "1";
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            //Items(2);
            DATA(1);
            LoadProject();
            LoadDistributor();
            LoadBlock("");
            LoadStreet();
            //LoadItems("");
            LoadItemInfo();
            LoadVendors();
            LoadCommissionAgent();
            LoadGrid("");
            DoEmptyTextBox();
            RbChange();
            CreatTable();
            txtBalance.Attributes.Add("readonly", "readonly");
            //txtPswd.Attributes.Add("autocomplete", "off");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            mastergrid.Visible = true;
            detailgrid.Visible = false;
            Grid_users.DataSource = null;
            Grid_users.DataBind();
            //FDReceipt.Value = "0";
            //AddReceipt.Visible = false;
            Panel3.Visible = true;
        }
    }
    #region Load Events
    public DataTable GetAppConfiguration()
    {
        try
        {
            ConfigurationController _cController = new ConfigurationController();
            DataTable dt = _cController.SelectAppConfiguration(1, (int)Enums.AppSettingMaster.WebSettings, null, Constants.IntNullValue);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg3", "alert('Error Occured: \n" + ex + "');", true);
            return null;
        }
    }
    public void DoEmptyTextBox()
    {
        Session["SaleMasterID"] = null;
        txtBalance.Text = "";
        txtDownPayment.Text = "";
        txtSaleAmount.Text = "";
        //txtPurchaseDate.Text = 
        txtTokenAmount.Text = "";
        //txtSearch.Text = "";
        txtCommissionValue.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        btnAddReceipt.Text = "Add Receipt";
        txtReceiptRemarks.Text = "";
        txtReceiptAmount.Text = "";
        txtDiscount.Text = "";
        ddlInstallmentType.SelectedValue = "1";
        txtNoofInstallment.Text = "";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        //ddlPaymentMode.SelectedValue = "0";
        txtReceiptDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtCommisionAmount.Text = "";
        //ddlCommissionAgent.Items.Add(new DevExpress.Web.ListEditItem("--Select--", "0"));
        ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
        ddlCommissionPercent.SelectedIndex = 0;
        ddlCommissionAgent.SelectedValue = "0";
        mastergrid.Visible = true;
        detailgrid.Visible = false;
        Session["SalesDetail"] = null;
        CreatTable();
    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            //DbCalls db = new DbCalls();
            //var qry = (from k in db.tbl_SALE_MASTER
            //           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
            //           join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
            //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
            //           join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on i.STREET_ID equals s.STREET_ID
            //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
            //           select new {k.REF_ID, d.DISTRIBUTOR_NAME, p.PROJECT_NAME, ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME, v.VENDOR_NAME, k.SALE_MASTER_ID, k.SALE_AMOUNT, Active = k.IS_DELETED == false ? "Active" : "Inactive", k.SALE_CLOSED,k.TYPE_ID }).Where(x => x.Active == "Active" && x.SALE_CLOSED == false && x.TYPE_ID == 13).ToList();
            //GridSaleMaster.DataSource = qry;
            //GridSaleMaster.DataBind();
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(4);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%'  OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR Convert(REF_ID, System.String)  LIKE '%" + txtSearch.Text + "%'";
            //OR SALE_AMOUNT = " + txtSearch.Text + "
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.PageIndex = 0;
            }
            GridSaleMaster.DataSource = dt;
            GridSaleMaster.DataBind();
        }
        
    }
    protected void ClearControls()
    {
        try
        {

            Session["SaleMasterID"] = null;
            txtBalance.Text = "";
            txtDownPayment.Text = "";
            txtSaleAmount.Text = "";
            //txtPurchaseDate.Text = 
            txtTokenAmount.Text = "";
            //txtSearch.Text = "";
            txtCommissionValue.Text = "";
            txtRemarks.Text = "";
            btnSave.Text = "Save";
            btnAddReceipt.Text = "Add Receipt";
            txtReceiptRemarks.Text = "";
            txtReceiptAmount.Text = "";
            //ddlPaymentMode.SelectedValue = "0";
            txtReceiptDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtCommisionAmount.Text = "";
            //ddlCommissionAgent.Items.Add(new DevExpress.Web.ListEditItem("--Select--", "0"));
            ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
            ddlCommissionPercent.SelectedIndex = 0;
            ddlCommissionAgent.SelectedValue = "0";
            mastergrid.Visible = true;
            detailgrid.Visible = false;
            Session["SalesDetail"] = null;
            CreatTable();
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    private void CreatTable()
    {
        SalesDetail = new DataTable();
        SalesDetail.Columns.Add("SALE_DETAIL_ID", typeof(long));
        SalesDetail.Columns.Add("ReceiptDate", typeof(DateTime));
        SalesDetail.Columns.Add("RECEIPT_MODE", typeof(int));
        SalesDetail.Columns.Add("AMOUNT", typeof(decimal));
        SalesDetail.Columns.Add("REMARKS", typeof(string));
        SalesDetail.Columns.Add("PayMod", typeof(string));
        this.Session.Add("SalesDetail", SalesDetail);
    }
    public void DATA(int Type)
    {
        if (Type != 5)
        {
            this.Session["dt"] = null;
            DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
            this.Session.Add("dt", dt);
        }
        else
        {
            this.Session["dtItem"] = null;
            DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
            this.Session.Add("dtItem", dt);
        }
        
    }
    //public void Items(int Type)
    //{
    //    this.Session["Items"] = null;
    //    DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Type);
    //    this.Session.Add("Items", dt);
    //}
    private void LoadItemInfo()
    {
       DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = dt.AsEnumerable()
                  .Where(r => r.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue) && r.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue) && r.Field<int>("STREET_ID") == int.Parse(ddlStreet.SelectedValue));
            if (qry.Any())
            {
                var Items = qry.CopyToDataTable();
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ITEM";
                ddlItem.DataValueField = "REF_ID";
                ddlItem.DataBind();
                ddlItem.SelectedIndex = 0;
            }
            else
            {
                ddlItem.Items.Clear();
                ddlItem.Items.Add(new ListItem("", "0"));
                ddlItem.SelectedIndex = 0;
            }
        }
        else
        {
            ddlItem.Items.Clear();
            ddlItem.Items.Add(new ListItem("", "0"));
            ddlItem.SelectedIndex = 0;
        }

    }
    protected void LoadProject()
    {
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
    }
    protected void LoadDistributor()
    {
        int UserID = Convert.ToInt32(Session["UserID"]);
        ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
        ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
        ddlLocation.DataValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
    }
    protected void LoadBlock(string Val)
    {
        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
                      group rows by new { BLOCK_ID = rows["BLOCK_ID"], BLOCK_NAME = rows["BLOCK_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
               var Blocks = qry.CopyToDataTable();
                ddlBlock.DataSource = Blocks;
                ddlBlock.DataTextField = "BLOCK_NAME";
                ddlBlock.DataValueField = "BLOCK_ID";
                ddlBlock.DataBind();
                //ddlBlock.SelectedIndex = 0;
            }
            else
            {
                ddlBlock.Items.Clear();
                ddlBlock.Items.Add(new ListItem("", "0"));
                ddlBlock.SelectedIndex = 0;
            }
        }
        else
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Add(new ListItem("", "0"));
            ddlBlock.SelectedIndex = 0;
        }

    }
    protected void LoadItems(string Val)
    {
        //int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
        //int BlockID = Convert.ToInt32(ddlBlock.SelectedValue);
        //int StreetID = Convert.ToInt32(ddlStreet.SelectedValue);
        //ddlItem.DataSource = DropDowns.Sale_ITEMDropDown(Val).Where(x => x.PROJECT_ID == ProjectID && x.BLOCK_ID == BlockID && x.STREET_ID == StreetID);
        //ddlItem.DataTextField = "ITEM_NAME";
        //ddlItem.DataValueField = "ITEM_INFO_ID";
        //ddlItem.DataBind();
    }
    protected void LoadStreet()
    {
        //int ProjectID = Convert.ToInt32(ddlBlock.SelectedValue);
        //ddlStreet.DataSource = DropDowns.STREETDropDown().Where(x => x.BLOCK_ID == ProjectID);
        //ddlStreet.DataTextField = "STREET_NAME";
        //ddlStreet.DataValueField = "STREET_ID";
        //ddlStreet.DataBind();


        //ddlStreet.Items.Clear();
        //DataTable dt = LController.SelectBSI(int.Parse(ddlLocation.SelectedValue), int.Parse(ddlProject.SelectedValue), int.Parse(ddlBlock.SelectedValue), Constants.IntNullValue, 1);
        ////  dt = dt.AsEnumerable().Where(p => p.Field<string>("LEVEL") == drpAccountLevel.SelectedItem.Value.ToString()).CopyToDataTable();
        //clsWebFormUtil.FillDropDownList(ddlStreet, dt, 5, 6, false);
        //if (dt.Rows.Count > 0)
        //{
        //    ddlStreet.SelectedIndex = 0;
        //}

        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue) && x.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue))
                      group rows by new { STREET_ID = rows["STREET_ID"], STREET_NAME = rows["STREET_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Streets = qry.CopyToDataTable();
                ddlStreet.DataSource = Streets;
                ddlStreet.DataTextField = "STREET_NAME";
                ddlStreet.DataValueField = "STREET_ID";
                ddlStreet.DataBind();
                //ddlStreet.SelectedIndex = 0;
            }
            else
            {
                ddlStreet.Items.Clear();
                ddlStreet.Items.Add(new ListItem("", "0"));
                ddlStreet.SelectedIndex = 0;
            }
        }
        else
        {
            ddlStreet.Items.Clear();
            ddlStreet.Items.Add(new ListItem("", "0"));
            ddlStreet.SelectedIndex = 0;
        }

    }
    protected void LoadVendors()
    {
        ddlCustomer.DataSource = DropDowns.CustomerDropDown();
        ddlCustomer.DataTextField = "VENDOR_NAME";
        ddlCustomer.DataValueField = "VENDOR_ID";
        ddlCustomer.DataBind();
    }
    protected void LoadCommissionAgent()
    {
        ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
        ddlCommissionAgent.DataTextField = "VENDOR_NAME";
        ddlCommissionAgent.DataValueField = "VENDOR_ID";
        ddlCommissionAgent.DataBind();
    }
    protected void LoadDetailGrid()
    {
        detailgrid.Visible = true;
        SalesDetail = (DataTable)this.Session["SalesDetail"];
        Grid_users.DataSource = SalesDetail;
        Grid_users.DataBind();
    }
    #endregion
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearch.Text);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlLocation.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Location is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlLocation);
            return;
        }
        if (ddlProject.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
            return;
        }
        if (ddlBlock.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
            return;
        }
        if (ddlStreet.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlStreet);
            return;
        }
        if (ddlItem.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        if (ddlCustomer.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Customer is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCustomer);
            return;
        }
        if (txtSaleAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sale Amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
            return;
        }
        if (txtSaleAmount.Text.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sale Amount could not be zero.');", true);
            txtSaleAmount.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtSaleAmount.Text.Equals("0.00"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sale Amount could not be zero.');", true);
            txtSaleAmount.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtCommisionAmount.Text.Equals(""))
        {
            txtCommisionAmount.Text = "0";
        }
        decimal Consumption;
        string ItemInfo = "";
        int Item_ID;
        DataTable dt = (DataTable)this.Session["dt"];
        DataRow[] foundRows = dt.Select("REF_ID  = " + int.Parse(ddlItem.SelectedValue));
        if (foundRows.Length > 0)
        {
            Consumption = Convert.ToDecimal(foundRows[0]["Consumption"]);
            ItemInfo = Convert.ToString(foundRows[0]["ItemInfo"]);
            Item_ID = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"]);
        }
        else
        {
            Consumption = 0;
            ItemInfo = "";
            Item_ID = 0;
        }
            //decimal Consumption = dr.R
            bool INS = false;
        SaleMasterLogics pml = new SaleMasterLogics();
        tbl_SALE_MASTER pm = new tbl_SALE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedValue);
        pm.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedValue);
        //pm.BLOCK_ID = Convert.ToInt32(ddlBlock.SelectedValue);
        //pm.STREET_ID = Convert.ToInt32(ddlStreet.SelectedValue);
        //pm.ITEM_INFO_ID = Item_ID; //Convert.ToInt32(ddlItem.SelectedValue);
        //pm.REF_ID = Convert.ToInt32(ddlItem.SelectedValue);
        //pm.VENDOR_ID = Convert.ToInt32(ddlCustomer.SelectedValue);
        //pm.COMMISSIONAGENT_ID = Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        //pm.SALE_AMOUNT = Convert.ToDecimal(txtSaleAmount.Text);
        //pm.DISCOUNT = Convert.ToDecimal(txtDiscount.Text);
        //pm.TOKEN_AMOUNT = Convert.ToDecimal(txtTokenAmount.Text);
        //pm.DOWN_PAYMENT = Convert.ToDecimal(txtDownPayment.Text);
        //pm.BALANCE = Convert.ToDecimal(txtBalance.Text);
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    pm.COMMISSION_PERCENT = Convert.ToInt32(ddlCommissionPercent.SelectedValue);
        //    pm.COMMISSION_VALUE = 0;
        //}
        //else
        //{
        //    pm.COMMISSION_PERCENT = 0;
        //    pm.COMMISSION_VALUE = Convert.ToInt32(txtCommissionValue.Text);
        //}
        //pm.REMARKS = txtRemarks.Text;
        //pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        //pm.TIME_STEMP = DateTime.Now;
        //pm.SALE_CLOSED = false;
        //if (Session["SaleMasterID"] != null)
        //{
        //    int Def = Convert.ToInt32(Session["BAL_Amount"]) - Convert.ToInt32(Session["DBT_Amount"]);
        //    if (Convert.ToDecimal(txtBalance.Text) > Convert.ToDecimal(Session["BAL_Amount"]))
        //    {
        //        pm.DEBIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtBalance.Text) - Convert.ToDecimal(Def));
        //    }
        //    else if (Convert.ToDecimal(txtBalance.Text) < Convert.ToDecimal(Session["BAL_Amount"]))
        //    {
        //        pm.DEBIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtBalance.Text) - Convert.ToDecimal(Def));
        //    }
        //    else
        //    {
        //        pm.DEBIT_AMOUNT = Convert.ToDecimal(txtReceiptAmount.Text);
        //    }

        //    pm.TYPE_ID = Constants.Document_SaleInvoice;
        //    pm.VOUCHER_NO = "";
        //    pm.DEBIT_AMOUNT = Convert.ToDecimal(txtBalance.Text);
        //    pm.IS_DELETED = false;
        //    pm.SALE_MASTER_ID = Convert.ToInt32(Session["SaleMasterID"]);
        //    pml.Update(pm);
        //    DbCalls db = new DbCalls();
        //    var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher  && x.DOCUMENT_NO == pm.SALE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
        //    if (q.Count > 0)
        //    {
        //        string VNO = q.FirstOrDefault().VOUCHER_NO;
        //        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //        {
        //            con.Open();
        //            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VNO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.SALE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_SaleInvoice + "and PAYMENT_MODE=" + Constants.CreditSale, con))
        //            {
        //                command.ExecuteNonQuery();
        //            }
        //            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
        //            {
        //                command1.ExecuteNonQuery();
        //            }
        //            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
        //            {
        //                command2.ExecuteNonQuery();
        //            }
        //            con.Close();
        //        }
        //    }
            //LedgerEntries(pm.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(pm.SALE_AMOUNT)
            //    , pm.DATE, pm.VENDOR_ID, Constants.Document_SaleInvoice
            //    , "", Convert.ToInt32(Session["UserID"]), pm.SALE_MASTER_ID
            //    , Convert.ToString(pm.SALE_MASTER_ID)
            //    , Convert.ToDecimal(pm.DISCOUNT), Convert.ToDecimal(pm.TOKEN_AMOUNT), Convert.ToDecimal(pm.DOWN_PAYMENT), ItemInfo, Convert.ToDecimal(txtCommisionAmount.Text));
            //ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            //Session["SaleMasterID"] = null;
            //Session["BAL_Amount"] = null;
            //Session["DBT_Amount"] = null;
        //}
        //else
        //{
           
        //    pm.DEBIT_AMOUNT = Convert.ToDecimal(txtBalance.Text);
        //    pm.IS_DELETED = false;
        //    pm.TYPE_ID = Constants.Document_SaleInvoice;
        //    pm.VOUCHER_NO = "";
        //    pml.Save(pm);
        //    LedgerEntries(pm.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(pm.SALE_AMOUNT)
        //        , pm.DATE, pm.VENDOR_ID, Constants.Document_SaleInvoice
        //        , "", Convert.ToInt32(Session["UserID"]), pm.SALE_MASTER_ID
        //        , Convert.ToString(pm.SALE_MASTER_ID)
        //        , Convert.ToDecimal(pm.DISCOUNT), Convert.ToDecimal(pm.TOKEN_AMOUNT), Convert.ToDecimal(pm.DOWN_PAYMENT), ItemInfo, Convert.ToDecimal(txtCommisionAmount.Text));
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);

        //}
        LoadGrid("");
        DoEmptyTextBox();
        mastergrid.Visible = true;
        detailgrid.Visible = false;
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        Panel3.Visible = true;
        DATA(1);
    }
    private void LedgerEntries(int p_Distributor_Id
      , decimal p_Debit, decimal p_Credit, DateTime p_Ledger_Date, int p_PRINCIPAL_ID
      , int p_DOCUMENT_TYPE_ID, string pChequeNo, int p_UserId, long p_DOCUMENT_NO
      , string p_MANUAL_DOCUMENT_NO, decimal p_Discount_Amount, decimal p_TokenAmount, decimal p_DownPayment, string p_Remarks, decimal p_Commisson)
    {
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);

            #region GL Master, Detail

            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);
            

            if (p_DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice)
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Sale-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Sale-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);

                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales_Dicount, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);

                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);

                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);

                //if (IsFinanceSetting)
                //{
                LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                    , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_SaleInvoice
                    , p_MANUAL_DOCUMENT_NO, "Sale Voucher / " + ddlCustomer.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Sale", mTransaction, mConnection);
                decimal Sales = p_Credit - p_Discount_Amount;
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales, 0, Sales, "Sale Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, Sales, 0, "Sale Voucher", mTransaction, mConnection);
                //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales_Dicount, p_Discount_Amount, 0, "Sale Discount Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchases, p_Debit, 0, "Sale (Consumptions) Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, 0, p_Debit, "Sale (Consumptions) Voucher", mTransaction, mConnection);

                //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales, 0, p_Discount_Amount, "Sale Discount Voucher", mTransaction, mConnection);
                //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchases, p_Commisson, 0, "Sale Commission Voucher", mTransaction, mConnection);
                //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, 0, p_Commisson, "Sale Commission Voucher", mTransaction, mConnection);
                //}
            }
            #endregion
            mTransaction.Commit();
            //return mPurchaseMaster.PURCHASE_MASTER_ID;
        }
        catch (Exception exp)
        {
            ExceptionPublisher.PublishException(exp);
            mTransaction.Rollback();
            //return Constants.LongNullValue;
        }
        finally
        {
            if (mConnection != null && mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
        }


    }
    private DataTable GetCOAConfiguration()
    {
        try
        {
            COAMappingController _cController = new COAMappingController();
            DataTable dt = _cController.SelectCOAConfiguration(5, Constants.ShortNullValue, Constants.LongNullValue, "Level 4");
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg3", "alert('Error Occured: \n" + ex + "');", true);
            return null;
        }
    }
    private bool GetFinanceConfig()
    {
        try
        {
            ConfigurationController _cController = new ConfigurationController();
            DataTable dt = _cController.SelectAppConfiguration(1, (int)Enums.AppSettingMaster.FinanceSetting, null, Constants.IntNullValue);

            if (dt.Rows.Count > 0)
            {
                DataRow[] dr = dt.Select("CODE = '" + (int)Enums.AppSetting.IsFinanceIntegrate + "'");
                if (dr.Length > 0)
                {
                    return Convert.ToInt32(dr[0][2]) == 1 ? true : false;
                }
            }
            return false;
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Error in Financial Setting!');", true);
            throw;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //mPopUpLocation.Show();
        //LoadUser();
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        ClearControls();
        Panel3.Visible = true;
        DATA(1);
    }
    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        Grid_users.PageIndex = e.NewPageIndex;
        LoadDetailGrid();
    }
    protected void Grid_users_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowNo = e.NewEditIndex;
        txtReceiptDate.Text = Grid_users.Rows[e.NewEditIndex].Cells[0].Text;
        ddlPaymentMode.SelectedItem.Text = Grid_users.Rows[e.NewEditIndex].Cells[1].Text;
        txtReceiptAmount.Text = Grid_users.Rows[e.NewEditIndex].Cells[2].Text;
        txtReceiptRemarks.Text = Grid_users.Rows[e.NewEditIndex].Cells[3].Text;
        btnAddReceipt.Text = "Update Receipt";
    }
    protected void Grid_users_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SalesDetail = (DataTable)this.Session["SalesDetail"];
        if (SalesDetail.Rows.Count > 0)
        {
            SalesDetail.Rows.RemoveAt(e.RowIndex);
            this.Session.Add("SalesDetail", SalesDetail);
            this.LoadDetailGrid();
            btnAddReceipt.Text = "Add Receipt";
        }
        btnAddReceipt.Text = "Add Receipt";
    }
    protected void RbtCommissionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCommissionAgent.SelectedValue == "0")
        {
            RbtCommissionType.SelectedValue = "1";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please select commission agent first.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
            return;
        }
        RbChange();
    }
    public void RbChange()
    {
        if (RbtCommissionType.SelectedValue == "1")
        {
            txtCommissionValue.ReadOnly = true;
            ddlCommissionPercent.Enabled = true;
            ddlCommissionPercent.SelectedIndex = 0;
            txtCommissionValue.Text = "0";
            txtCommisionAmount.Text = "0";
            ddlCommissionPercent.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionPercent);
        }
        else
        {
            ddlCommissionPercent.SelectedIndex = 0;
            txtCommissionValue.ReadOnly = false;
            ddlCommissionPercent.Enabled = false;
            txtCommisionAmount.Text = "0";
            txtCommissionValue.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtCommissionValue);
        }
    }
    protected void btnAddReceipt_Click(object sender, EventArgs e)
    {
        if (txtReceiptAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Receipt Amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtReceiptAmount);
            return;
        }
        SalesDetail = (DataTable)this.Session["SalesDetail"];
        if (btnAddReceipt.Text == "Add Receipt")
        {
            if (txtReceiptAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Receipt amount is required.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtReceiptAmount);
                return;
            }
            DataRow dr = SalesDetail.NewRow();
            dr["ReceiptDate"] = Convert.ToDateTime(txtReceiptDate.Text);
            dr["RECEIPT_MODE"] = Convert.ToInt32(ddlPaymentMode.SelectedValue);
            dr["AMOUNT"] = Convert.ToDecimal(txtReceiptAmount.Text);
            dr["REMARKS"] = txtReceiptRemarks.Text;
            dr["PayMod"] = ddlPaymentMode.SelectedItem.Text;
            SalesDetail.Rows.Add(dr);
        }
        else if (btnAddReceipt.Text == "Update Receipt")
        {
            if (txtReceiptAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Receipt amount is required.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtReceiptAmount);
                return;
            }
            DataRow dr = SalesDetail.Rows[RowNo];
            dr["ReceiptDate"] = Convert.ToDateTime(txtReceiptDate.Text);
            dr["RECEIPT_MODE"] = Convert.ToInt32(ddlPaymentMode.SelectedValue);
            dr["AMOUNT"] = Convert.ToDecimal(txtReceiptAmount.Text);
            dr["REMARKS"] = txtReceiptRemarks.Text;
            dr["PayMod"] = ddlPaymentMode.SelectedItem.Text;
            btnAddReceipt.Text = "Add Receipt";
        }
        this.Session.Add("SalesDetail", SalesDetail);
        LoadDetailGrid();
        clear();
    }
    public void clear()
    {
        txtReceiptDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        ddlPaymentMode.SelectedValue = "1";
        txtReceiptRemarks.Text = "";
        txtReceiptAmount.Text = "";
    }
    protected void ddlCommissionPercent_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCommissionAgent.SelectedValue == "0")
        {
            ddlCommissionPercent.SelectedValue = "0";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please select commission agent first.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
            return;
        }
        if (RbtCommissionType.SelectedValue == "1")
        {
            if (txtSaleAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
                ddlCommissionPercent.SelectedValue = "0";
                return;
            }
            else
            {
                //decimal discount = 0;
                //if (txtDiscount.Text.Equals(""))
                //{ discount = 0; }
                //else { discount = Convert.ToDecimal(txtDiscount.Text); }
                decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
                decimal itemamount = Convert.ToDecimal(txtBalance.Text);
                //decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
                //decimal sale_amount = itemamount - discount - downpayment;
                txtCommissionValue.Text = "0";
                decimal Amount = itemamount / 100 * Percent;
                txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
                ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
            }
        }
        else
        {
            if (txtSaleAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
                ddlCommissionPercent.SelectedValue = "0";
                return;
            }
            else
            {
                txtCommissionValue_TextChanged(null, null);
            }
        }
    }
    protected void txtCommissionValue_TextChanged(object sender, EventArgs e)
    {
        string Commission = txtCommissionValue.Text;
        txtCommisionAmount.Text = Commission;
        ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
    }
    protected void txtSaleAmount_TextChanged(object sender, EventArgs e)
    {
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    if (txtSaleAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
        //        decimal itemamount = Convert.ToDecimal(txtSaleAmount.Text);
        //        decimal discount = 0;
        //        if (txtDiscount.Text.Equals(""))
        //        { discount = 0; }
        //        else { discount = Convert.ToDecimal(txtDiscount.Text); }
        //        decimal sale_amount = itemamount - discount;
        //        txtCommissionValue.Text = "0";
        //        decimal Amount = sale_amount / 100 * Percent;
        //        txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
        //        //txtBalance.Text = Math.Round(itemamount, 2).ToString();
        //        if (txtTokenAmount.Text.Equals(""))
        //        {
        //            txtTokenAmount.Text = "0";
        //        }
        //        if (txtDownPayment.Text.Equals(""))
        //        {
        //            txtDownPayment.Text = "0";
        //        }
        //        //decimal discount = 0;
        //        //if (txtDiscount.Text.Equals(""))
        //        //{ discount = 0; }
        //        //else { discount = Convert.ToDecimal(txtDiscount.Text); }
        //        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        //        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        //        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        //        ScriptManager.GetCurrent(Page).SetFocus(txtDiscount);
        //        txtTokenAmount.Attributes.Add("onfocusin", " select();");
        //    }
        //}
        //else
        //{
        //    if (txtSaleAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        if (txtCommissionValue.Text.Equals(""))
        //        {
        //            txtCommissionValue.Text = "0";
        //            txtCommisionAmount.Text = "0";
        //            //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
        //        }
        //        else
        //        {
        //            decimal itemamount = Convert.ToDecimal(txtSaleAmount.Text);
        //            string Commission = txtCommissionValue.Text;
        //            txtCommisionAmount.Text = Commission;
        //            //txtBalance.Text = Math.Round(itemamount, 2).ToString();
        //            if (txtTokenAmount.Text.Equals(""))
        //            {
        //                txtTokenAmount.Text = "0";
        //            }
        //            if (txtDownPayment.Text.Equals(""))
        //            {
        //                txtDownPayment.Text = "0";
        //            }
        //            //decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
        //            decimal discount = 0;
        //            if (txtDiscount.Text.Equals(""))
        //            { discount = 0; }
        //            else { discount = Convert.ToDecimal(txtDiscount.Text); }
        //            decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        //            decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        //            txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        //            ScriptManager.GetCurrent(Page).SetFocus(txtDiscount);
        //            txtTokenAmount.Attributes.Add("onfocusin", " select();");
        //        }
        //    }
        //}
    }
    protected void txtTokenAmount_TextChanged(object sender, EventArgs e)
    {
        decimal discount = 0;
        decimal itemamount = Convert.ToDecimal(txtSaleAmount.Text);
        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        if (txtDiscount.Text.Equals(""))
        { discount = 0; }
        else { discount = Convert.ToDecimal(txtDiscount.Text); }
        if (txtDownPayment.Text.Equals(""))
        {
            txtDownPayment.Text = "0";
        }
        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        ScriptManager.GetCurrent(Page).SetFocus(txtDownPayment);
        txtDownPayment.Attributes.Add("onfocusin", " select();");
    }
    protected void txtDownPayment_TextChanged(object sender, EventArgs e)
    {
        decimal discount = 0;
        decimal itemamount = Convert.ToDecimal(txtSaleAmount.Text);
        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        if (txtDiscount.Text.Equals(""))
        { discount = 0; }
        else { discount = Convert.ToDecimal(txtDiscount.Text); }
        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
    }
    protected void GridSaleMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridSaleMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }
    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["SaleMasterID"] = null;
        Session["BAL_Amount"] = null;
        Session["DBT_Amount"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleMasterLogics Ser = new SaleMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        //var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
        //if (qry.Any())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected sale could not update. Some payments are received against this sale.');", true);
        //}
        //else
        //{
        //    DataTable dt = new DataTable();
        //    DataTable dtItem = new DataTable();
        //    ddlProject.DataSource = DropDowns.PROJECTDropDown();
        //    ddlProject.DataTextField = "PROJECT_NAME";
        //    ddlProject.DataValueField = "PROJECT_ID";
        //    ddlProject.DataBind();
        //    ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
        //    ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
        //    ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
        //    ddlLocation.DataValueField = "DISTRIBUTOR_ID";
        //    ddlLocation.DataBind();
        //    ddlLocation.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);
        //    DATA(1);
        //    DATA(5);
        //    //dtItem = (DataTable)this.Session["dtItem"];
        //    //DataRow[] foundRows = dtItem.Select("REF_ID  = '" + data.REF_ID + "'");
        //    //if (foundRows.Length > 0)
        //    //{
        //    //    dt = (DataTable)this.Session["dt"];
        //    //    DataRow dr = dt.NewRow();
        //    //    dr["DISTRIBUTOR_ID"] = Convert.ToInt32(foundRows[0]["DISTRIBUTOR_ID"].ToString());
        //    //    dr["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
        //    //    dr["PROJECT_NAME"] = foundRows[0]["PROJECT_NAME"].ToString();
        //    //    dr["BLOCK_ID"] = Convert.ToInt32(foundRows[0]["BLOCK_ID"].ToString());
        //    //    dr["BLOCK_NAME"] = foundRows[0]["BLOCK_NAME"].ToString();
        //    //    dr["STREET_ID"] = Convert.ToInt32(foundRows[0]["STREET_ID"].ToString());
        //    //    dr["STREET_NAME"] = foundRows[0]["STREET_NAME"].ToString();
        //    //    dr["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
        //    //    dr["ITEM_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
        //    //    dr["ITEM"] = foundRows[0]["ITEM"].ToString();
        //    //    dr["Consumption"] = foundRows[0]["Consumption"].ToString();
        //    //    dr["REF_ID"] = foundRows[0]["REF_ID"].ToString();
        //    //    dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
        //    //    dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
        //    //    dt.Rows.Add(dr);
        //    //    Session["dt"] = dt;
        //    //}
        //    LoadBlock("");
        //    //ddlBlock.SelectedValue = Convert.ToString(data.BLOCK_ID);
        //    //LoadStreet();
        //    //ddlStreet.SelectedValue = Convert.ToString(data.STREET_ID);
        //    //LoadItemInfo();
        //    //ddlItem.SelectedValue = Convert.ToString(data.REF_ID);
        //    ddlCustomer.DataSource = DropDowns.CustomerDropDown();
        //    ddlCustomer.DataTextField = "VENDOR_NAME";
        //    ddlCustomer.DataValueField = "VENDOR_ID";
        //    ddlCustomer.DataBind();
        //    //ddlCustomer.SelectedValue = Convert.ToString(data.VENDOR_ID);
        //    ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
        //    ddlCommissionAgent.DataTextField = "VENDOR_NAME";
        //    ddlCommissionAgent.DataValueField = "VENDOR_ID";
        //    ddlCommissionAgent.DataBind();
        //    ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
        //    ddlCommissionAgent.SelectedValue = Convert.ToString(data.COMMISSIONAGENT_ID);

        //    //txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
        //    //decimal saleamount = decimal.Parse(data.SALE_AMOUNT.ToString());
        //    //txtSaleAmount.Text = Convert.ToString(Math.Round(saleamount, 2));

        //    //decimal discountamount = decimal.Parse(data.DISCOUNT.ToString());
        //    //txtDiscount.Text = Convert.ToString(Math.Round(discountamount, 2));

        //    //decimal tokenamount = decimal.Parse(data.TOKEN_AMOUNT.ToString());
        //    //txtTokenAmount.Text = Convert.ToString(Math.Round(tokenamount, 2));

        //    //decimal downpayment = decimal.Parse(data.DOWN_PAYMENT.ToString());
        //    //txtDownPayment.Text = Convert.ToString(Math.Round(downpayment, 2));

        //    //decimal balance = decimal.Parse(data.BALANCE.ToString());
        //    //txtBalance.Text = Convert.ToString(Math.Round(balance, 2));

        //    //ddlCommissionPercent.SelectedValue = Convert.ToString(data.COMMISSION_PERCENT);
        //    //if (ddlCommissionPercent.SelectedValue != "0")
        //    //{
        //    //    txtCommissionValue.Text = "0";
        //    //    decimal amount = Convert.ToDecimal(data.BALANCE / 100 * data.COMMISSION_PERCENT);
        //    //    txtCommisionAmount.Text = Convert.ToString(Math.Round(amount, 2));
        //    //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
        //    //    RbtCommissionType.SelectedValue = "1";
        //    //}
        //    //else
        //    //{
        //    //    RbtCommissionType.SelectedValue = "2";
        //    //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
        //    //    txtCommisionAmount.Text = Convert.ToString(data.COMMISSION_VALUE);
        //    //}
        //    //txtRemarks.Text = data.REMARKS;
        //    //txtReceiptAmount.Text = Convert.ToString(data.DEBIT_AMOUNT);
        //    //Session.Add("BAL_Amount", data.BALANCE);
        //    //Session.Add("DBT_Amount", data.DEBIT_AMOUNT);
            
        //    Session.Add("SaleMasterID", data.SALE_MASTER_ID);
        //    mastergrid.Visible = false;
        //    detailgrid.Visible = true;
        //    Panelgrid.Visible = false;
        //    PanelSale.Visible = true;
        //    btnSave.Text = "Update";
        //    Panel3.Visible = false;
        //}

    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //DbCalls db = new DbCalls();
        //bool flag = false;
        //var ProID = ((LinkButton)sender).CommandArgument;
        //SaleMasterLogics Ser = new SaleMasterLogics();
        //var data = Ser.Get(Convert.ToInt32(ProID));
        //bool Active = Convert.ToBoolean(data.IS_DELETED);
        //var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
        //if (qry.Any())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected sale could not be deleted. Some payments are received against this sale.');", true);
        //}
        //else
        //{
        //    //var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
        //    //try
        //    //{
        //    //    //if (!Active)
        //    //    //{
        //    //    //    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //    //    //    {
        //    //    //        con.Open();
        //    //    //        using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_MASTER where SALE_MASTER_ID=" + Convert.ToInt32(ProID), con))
        //    //    //        {
        //    //    //            command.ExecuteNonQuery();
        //    //    //        }
        //    //    //        if (q.Count > 0)
        //    //    //        {
        //    //    //            string VNO = q.FirstOrDefault().VOUCHER_NO;
        //    //    //            using (SqlCommand command3 = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.SALE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_SaleInvoice + "and PAYMENT_MODE=" + Constants.CreditSale, con))
        //    //    //            {
        //    //    //                command3.ExecuteNonQuery();
        //    //    //            }
        //    //    //            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
        //    //    //            {
        //    //    //                command1.ExecuteNonQuery();
        //    //    //            }
        //    //    //            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
        //    //    //            {
        //    //    //                command2.ExecuteNonQuery();
        //    //    //            }
        //    //    //        }
                        
        //    //    //        con.Close();
        //    //    //    }
        //    //    //    flag = true;
        //    //    //    if (flag)
        //    //    //    {
        //    //    //        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
        //    //    //    }
        //    //    //}
        //    //}
        //    catch (Exception ex) { }
        //}
        //LoadGrid("");
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
        LoadStreet();
        LoadItemInfo();
        ddlBlock.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
    }
    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Panelgrid.Visible = false;
        PanelSale.Visible = true;
        ddlInstallmentType.Visible = false;
        txtNoofInstallment.Visible = false;
        lblInstallmentType.Visible = false;
        lblNoofInstallment.Visible = false;
        lblFromMonth.Visible = false;
        txtFromMonth.Visible = false;
        ibtnStartMonth.Visible = false;
        lblToketAmount.InnerText = "Token Amount";
        txtDownPayment.Enabled = true;
        Hp.Enabled = true;
        txtDiscount.Text = "";
        ddlInstallmentType.SelectedValue = "1";
        txtNoofInstallment.Text = "";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        Hp.SelectedValue = "1";
        LoadProject();
        DATA(1);
        LoadBlock("");
        LoadStreet();
        LoadItemInfo();
        //LoadGrid("");
        Panel3.Visible = false;

    }
    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        //AddReceipt.Visible = false;
        Session["SALE_MASTER_ID"] = null;
        //FDReceipt.Value = "0";
        mPopUpLocation.Hide();
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        Panel3.Visible = true;
    }
    protected void BtnPopUpSave_Click(object sender, EventArgs e)
    {
        if (txtAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Amount is required.');", true);
            return;
        }
        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
        {
            if (drpCheque.SelectedItem.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Cheque is required.');", true);
                return;
            }
            if (txtStartDate.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Cheque date is required.');", true);
                return;
            }
        }
        if (DrpAccountType.SelectedItem.Value.ToString() == "33")
        {
            if (txtStartDate.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Transfer date is required.');", true);
                return;
            }
        }
        if (Session["SALE_MASTER_ID"] != null)
        {

            SaleDetailLogics pdl = new SaleDetailLogics();
            //tbl_SALE_DETAIL pd = new tbl_SALE_DETAIL();
            //pd.SALE_MASTER_ID = Convert.ToInt32(Session["SALE_MASTER_ID"]);
            //if (DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "18")
            //{
            //    pd.DATE = Convert.ToDateTime(txtStartDate.Text);
            //}
            //else
            //{
            //    pd.DATE = Convert.ToDateTime(DateTime.Now);
            //}
            //pd.PAYMENT_MODE = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
            //pd.AMOUNT = decimal.Parse(txtAmount.Text.ToString());
            //pd.REMARKS = txtRecRemarks.Text;
            //pd.USER_ID = Convert.ToInt32(Session["UserID"]);
            //pd.TIME_STEMP = Convert.ToDateTime(DateTime.Now);
            //pd.IS_DELETED = false;
            //pdl.Save(pd);
            Session["SALE_MASTER_ID"] = null;
            //FDReceipt.Value = "0";
            mPopUpLocation.Hide();
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Customer receipt added successfully.');", true);
        }
    }
    protected void BtnPopUpCancel_Click(object sender, EventArgs e)
    {
        if (DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "18")
        {
            txtAmount.Text = "";
            txtStartDate.Text = "";
            txtRecRemarks.Text = "";
        }
        else
        {
            txtAmount.Text = "";
            txtRecRemarks.Text = "";
        }
    }
    protected void DrpAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAccountHead();
        toggleControls(DrpAccountType.SelectedItem.Value.ToString());
        //SelectCreditInvoice();
        //LoadReceviedCheque();
    }
    private void toggleControls(string pAccountType)
    {
        if (pAccountType == "21")
        {
            lblChequeNo.Visible = false;
            drpCheque.Visible = false;
            lblChequeDate.Visible = false;
            txtStartDate.Visible = false;
            ibtnStartDate.Visible = false;
            DrpStatus.Visible = false;
            lblStatus.Visible = false;
            //GrdCO.Visible = true;
            //GrdCheque.Visible = false;

        }
        else if (pAccountType == "33")
        {
            lblChequeNo.Visible = false;
            drpCheque.Visible = false;
            lblChequeDate.Visible = true;
            txtStartDate.Visible = true;
            ibtnStartDate.Visible = true;
            DrpStatus.Visible = false;
            lblStatus.Visible = false;
            lblChequeDate.Text = "Transfer Date";
            //GrdCO.Visible = true;
            //GrdCheque.Visible = false;

        }
        else
        {

            lblChequeNo.Visible = true;
            drpCheque.Visible = true;
            lblChequeDate.Visible = true;
            txtStartDate.Visible = true;
            ibtnStartDate.Visible = true;
            DrpStatus.Visible = true;
            lblStatus.Visible = true;
            lblChequeDate.Text = "Cheque Date";
            //GrdCO.Visible = false;
            //GrdCheque.Visible = true;
            LoadCheque(2);

        }
    }
    public void LoadCheque(int TYPE)
    {
        try
        {
            DataTable dt = new DataTable();
            dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(DrpBankAccount.SelectedItem.Value.ToString()), null, null, null, true, Constants.DateNullValue, TYPE);
            clsWebFormUtil.FillDxComboBoxList(this.drpCheque, dt, 0, 1, true);

            if (dt.Rows.Count > 0)
            {
                drpCheque.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void DrpBankAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCheque(2);
    }
    protected void DrpStatus_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void DrpVendor_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    private void LoadAccountHead()
    {
        Configuration.GetAccountHead();
        AccountHeadController mAccountController = new AccountHeadController();

        if (DrpAccountType.SelectedItem.Value.ToString() == "21")
        {
            DataTable dt = mAccountController.SelectAccountHeadByMapping(Constants.AC_AccountHeadId, Constants.LongNullValue, 2, Constants.AC_CashInHandAccountHead);
            clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dt, 0, 4, true);

            if (dt.Rows.Count > 0)
            {
                DrpBankAccount.SelectedIndex = 0;
            }
        }
        else
        {
            DataTable dt = mAccountController.SelectAccountHeadByMapping(Constants.AC_AccountHeadId, Constants.LongNullValue, 2, Constants.AC_BankAccountHead);
            clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dt, 0, 4, true);

            if (dt.Rows.Count > 0)
            {
                DrpBankAccount.SelectedIndex = 0;
            }
        }

    }
    private bool InvalidDate()
    {
        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
        {
            if (DrpStatus.SelectedItem.Value.ToString() == "527")
            {
                if (Convert.ToDateTime(ConvertDate.British_To_American(txtStartDate.Text)) < Convert.ToDateTime(Session["CurrentWorkDate"]))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Invalid Cheque Date');", true);

                    return false;
                }
            }

        }
        return true;
    }
    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        if (txtSaleAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sale Amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtSaleAmount);
            return;
        }
        if (txtTokenAmount.Text.Equals(""))
        {
            txtTokenAmount.Text = "0";
        }
        if (txtDownPayment.Text.Equals(""))
        {
            txtDownPayment.Text = "0";
        }
        decimal itemamount = Convert.ToDecimal(txtSaleAmount.Text);
        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        decimal discount = Convert.ToDecimal(txtDiscount.Text);
        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        ScriptManager.GetCurrent(Page).SetFocus(txtTokenAmount);
        txtDownPayment.Attributes.Add("onfocusin", " select();");
    }
    protected void Hp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue == "2")
        {
            txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
            ddlInstallmentType.Visible = true;
            txtNoofInstallment.Visible = true;
            lblInstallmentType.Visible = true;
            lblNoofInstallment.Visible = true;
            lblFromMonth.Visible = true;
            txtFromMonth.Visible = true;
            ibtnStartMonth.Visible = true;
            lblToketAmount.InnerText = "Booking Amount";
            txtDownPayment.Enabled = false;
        }
        else
        {
            ddlInstallmentType.Visible = false;
            txtNoofInstallment.Visible = false;
            lblInstallmentType.Visible = false;
            lblNoofInstallment.Visible = false;
            lblFromMonth.Visible = false;
            txtFromMonth.Visible = false;
            ibtnStartMonth.Visible = false;
            lblToketAmount.InnerText = "Token Amount";
            txtDownPayment.Enabled = true;
        }
    }
    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadStreet();
        LoadItemInfo();
        ddlStreet.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlStreet);
    }
    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItemInfo();
        ddlItem.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
    }

   
}