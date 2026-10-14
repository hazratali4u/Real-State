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

public partial class Forms_frmAddPurchases : System.Web.UI.Page
{
    private static int RowNo;
    DataTable PurchaseDetail;
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
            panelgrid.Visible = true;
            PanelPerchase.Visible = false;
            Items(2);
            LoadProject();
            LoadDistributor();
            LoadBlock("");
            LoadStreet();
            LoadItems("");
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
            GetCOAConfiguration();
            GetFinanceConfig();
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
        Session["PurchasMasterID"] = null;
        txtBalance.Text = "";
        txtDownPayment.Text = "";
        txtPurchaseAmount.Text = "";
        txtDiscount.Text = "";
        //txtPurchaseDate.Text = 
        txtTokenAmount.Text = "";
        txtSearch.Text = "";
        txtCommissionValue.Text = "";
        txtRemarks.Text = "";
        txtDiscount.Text = "";
        ddlInstallmentType.SelectedValue = "1";
        txtNoofInstallment.Text = "";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        btnSave.Text = "Save";
        btnAddPurPayment.Text = "Add Payment";
        txtPaymnetRemarks.Text = "";
        txtPurchaseAmount.Text = "";
        txtDebitAmount.Text = "";
        //ddlPaymentMode.SelectedValue = "0";
        txtPaymnetDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtCommisionAmount.Text = "";
        //ddlCommissionAgent.Items.Add(new DevExpress.Web.ListEditItem("--Select--", "0"));
        ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
        ddlCommissionPercent.SelectedIndex = 0;
        ddlCommissionAgent.SelectedValue = "0";
        mastergrid.Visible = true;
        detailgrid.Visible = false;
        Grid_users.DataSource = null;
        Grid_users.DataBind();
        
    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            //DbCalls db = new DbCalls();
            //var qry = (from k in db.tbl_PURCHASE_MASTER
            //           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
            //           join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
            //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
            //           join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on i.STREET_ID equals s.STREET_ID
            //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
            //           select new { d.DISTRIBUTOR_NAME, p.PROJECT_NAME, ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME, v.VENDOR_NAME, k.PURCHASE_MASTER_ID, k.PURCHASE_AMOUNT, Active = k.IS_DELETED == false ? "Active" : "Inactive", k.PURCHASE_CLOSED, k.IS_DELETED, k.TYPE_ID }).Where(x => x.Active == "Active" && x.PURCHASE_CLOSED == false && x.TYPE_ID == 2).ToList();
            //PurchaseMaster.DataSource = qry;
            //PurchaseMaster.DataBind();
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(1);
            //        var filtered = dt.AsEnumerable()
            //.Where(r => r.Field<String>("DISTRIBUTOR_NAME").Contains(txtSearch.Text)
            //       || r.Field<String>("PROJECT_NAME").Contains(txtSearch.Text)
            //       || r.Field<String>("ITEM_NAME").Contains(txtSearch.Text)
            //       || r.Field<String>("VENDOR_NAME").Contains(txtSearch.Text)
            //       );
            //|| r.Field<Decimal>("PURCHASE_AMOUNT").Contains(txtSearch.Text)
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR Convert(PURCHASE_MASTER_ID, System.String)  LIKE '%" + txtSearch.Text + "%'";
            //OR PURCHASE_MASTER_ID = '" + txtSearch.Text + "'
            //OR PURCHASE_AMOUNT LIKE '%" + txtSearch.Text + "%'
            if (dt.Rows.Count > 0)
            {
                PurchaseMaster.PageIndex = 0;
            }
            PurchaseMaster.DataSource = dt;
            PurchaseMaster.DataBind();
            
        }
        
    }
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("PURCHASE_DETAIL_ID", typeof(long));
        PurchaseDetail.Columns.Add("PaymentDate", typeof(DateTime));
        PurchaseDetail.Columns.Add("PAYMENT_MODE", typeof(int));
        PurchaseDetail.Columns.Add("AMOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("REMARKS", typeof(string));
        PurchaseDetail.Columns.Add("PayMod", typeof(string));
        this.Session.Add("PurchaseDetail", PurchaseDetail);
    }
    protected void btnAddPurPayment_Click(object sender, EventArgs e)
    {
        //if (txtPaymnetAmount.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Payment Amount is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPaymnetAmount);

        //    return;
        //}
        //PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        //if (btnAddPurPayment.Text == "Add Payment")
        //{
        //    if (txtPaymnetAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Payment amount is required.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtPaymnetAmount);
        //        return;
        //    }
        //    DataRow dr = PurchaseDetail.NewRow();
        //    dr["PaymentDate"] = Convert.ToDateTime(txtPaymnetDate.Text);
        //    dr["PAYMENT_MODE"] = Convert.ToInt32(ddlPaymentMode.SelectedValue);
        //    dr["AMOUNT"] = Convert.ToDecimal(txtPaymnetAmount.Text);
        //    dr["REMARKS"] = txtPaymnetRemarks.Text;
        //    dr["PayMod"] = ddlPaymentMode.SelectedItem.Text;
        //    PurchaseDetail.Rows.Add(dr);
        //}
        //else if (btnAddPurPayment.Text == "Update Payment")
        //{
        //    if (txtPaymnetAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Payment amount is required.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtPaymnetAmount);
        //        return;
        //    }
        //    DataRow dr = PurchaseDetail.Rows[RowNo];
        //    dr["PaymentDate"] = Convert.ToDateTime(txtPaymnetDate.Text);
        //    dr["PAYMENT_MODE"] = Convert.ToInt32(ddlPaymentMode.SelectedValue);
        //    dr["AMOUNT"] = Convert.ToDecimal(txtPaymnetAmount.Text);
        //    dr["REMARKS"] = txtPaymnetRemarks.Text;
        //    dr["PayMod"] = ddlPaymentMode.SelectedItem.Text;
        //    btnAddPurPayment.Text = "Add Payment";
        //}
        //this.Session.Add("PurchaseDetail", PurchaseDetail);
        //LoadDetailGrid();
        //clear();
    }
    public void clear()
    {
        txtPaymnetDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtPaymnetRemarks.Text = "";
        txtDebitAmount.Text = "";
        ddlPaymentMode.SelectedValue = "1";
    }
    protected void LoadDetailGrid()
    {
        detailgrid.Visible = true;
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        Grid_users.DataSource = PurchaseDetail;
        Grid_users.DataBind();
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
    public void Items(int Type)
    {
        if (Type != 8)
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
    protected void LoadItems(string Val)
    {
        //int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
        //int BlockID = Convert.ToInt32(ddlBlock.SelectedValue);
        //int StreetID = Convert.ToInt32(ddlStreet.SelectedValue);
        //ddlItem.DataSource = DropDowns.Purchase_ITEMDropDown(Val).Where(x => x.PROJECT_ID == ProjectID && x.BLOCK_ID == BlockID && x.STREET_ID == StreetID);
        //ddlItem.DataTextField = "ITEM_NAME";
        //ddlItem.DataValueField = "ITEM_INFO_ID";
        //ddlItem.DataBind();
        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = dt.AsEnumerable()
                  .Where(r => r.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue) && r.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue) && r.Field<int>("STREET_ID") == int.Parse(ddlStreet.SelectedValue));
            if (qry.Any())
            {
                var Items = qry.CopyToDataTable();
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ITEM_NAME";
                ddlItem.DataValueField = "ITEM_INFO_ID";
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
    protected void LoadBlock(string Val)
    {
        //int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
        //ddlBlock.DataSource = DropDowns.BLOCKDropDown().Where(x => x.PROJECT_ID == ProjectID);
        //ddlBlock.DataTextField = "BLOCK_NAME";
        //ddlBlock.DataValueField = "BLOCK_ID";
        //ddlBlock.DataBind();
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

    protected void LoadStreet()
    {
        //int BlockID = Convert.ToInt32(ddlBlock.SelectedValue);
        //ddlStreet.DataSource = DropDowns.STREETDropDown().Where(x => x.BLOCK_ID == BlockID);
        //ddlStreet.DataTextField = "STREET_NAME";
        //ddlStreet.DataValueField = "STREET_ID";
        //ddlStreet.DataBind();
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
        ddlVendor.DataSource = DropDowns.VendorDropDown();
        ddlVendor.DataTextField = "VENDOR_NAME";
        ddlVendor.DataValueField = "VENDOR_ID";
        ddlVendor.DataBind();
    }

    protected void LoadCommissionAgent()
    {
        ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
        ddlCommissionAgent.DataTextField = "VENDOR_NAME";
        ddlCommissionAgent.DataValueField = "VENDOR_ID";
        ddlCommissionAgent.DataBind();
    }

    protected void LoadStatus()
    {
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Issue", "527"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));

        DrpStatus.SelectedIndex = 0;
        DrpStatus.Enabled = false;
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

    private void LoadAccountDetail()
    {
        AccountHeadController mAccountController = new AccountHeadController();

        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, 1);
        clsWebFormUtil.FillDxComboBoxList(this.ddlAccountHead, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_NAME", true);

        if (dtHead.Rows.Count > 0)
        {
            ddlAccountHead.SelectedIndex = 0;
        }
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
            ddlLocation.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlLocation);
            return;
        }
        if (ddlProject.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
            ddlProject.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
            return;
        }
        if (ddlBlock.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
            ddlBlock.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
            return;
        }
        if (ddlStreet.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street is required.');", true);
            ddlStreet.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
            return;
        }
        if (ddlItem.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
            ddlItem.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        if (ddlVendor.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ddlVendor.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (txtPurchaseAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount is required.');", true);
            txtPurchaseAmount.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtPurchaseAmount.Text.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount could not be zero.');", true);
            txtPurchaseAmount.Focus();
            ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtPurchaseAmount.Text.Equals("0.00"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount could not be zero.');", true);
            txtPurchaseAmount.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtCommisionAmount.Text.Equals(""))
        {
            txtCommisionAmount.Text = "0";
        }
        string ItemInfo = "";
        DataTable dt = (DataTable)this.Session["dt"];
        DataRow[] foundRows = dt.Select("ITEM_INFO_ID  = " + int.Parse(ddlItem.SelectedValue));
        if (foundRows.Length > 0)
        {
            //Consumption = Convert.ToDecimal(foundRows[0]["Consumption"]);
            ItemInfo = Convert.ToString(foundRows[0]["ItemInfo"]);
        }
        else
        {
            //Consumption = 0;
            ItemInfo = "";
        }
        bool INS = false;
        //string myStr = "ABCDEPER 343573";
        //string subStr = myStr.Right(6);
        PurchaseMasterLogics pml = new PurchaseMasterLogics();
        tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedValue);
        pm.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedValue);
        //pm.BLOCK_ID = Convert.ToInt32(ddlBlock.SelectedValue);
        //pm.STREET_ID = Convert.ToInt32(ddlStreet.SelectedValue);
        //pm.ITEM_INFO_ID = Convert.ToInt32(ddlItem.SelectedValue);
        //pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedValue);
        //pm.COMMISSIONAGENT_ID = Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        //pm.PURCHASE_AMOUNT = Convert.ToDecimal(txtPurchaseAmount.Text);
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
        pm.REMARKS = txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        //pm.PURCHASE_CLOSED = false;
        if (Session["PurchasMasterID"] != null)
        {
            //int Def = Convert.ToInt32(Session["BAL_Amount"]) - Convert.ToInt32(Session["DBT_Amount"]);
            //if (Convert.ToDecimal(txtBalance.Text) > Convert.ToDecimal(Session["BAL_Amount"]))
            //{
            //    pm.DEBIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtBalance.Text) - Convert.ToDecimal(Def));
            //}
            //else if (Convert.ToDecimal(txtBalance.Text) < Convert.ToDecimal(Session["BAL_Amount"]))
            //{
            //    pm.DEBIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtBalance.Text) - Convert.ToDecimal(Def));
            //}
            //else
            //{
            //    pm.DEBIT_AMOUNT = Convert.ToDecimal(txtDebitAmount.Text);
            //}
            pm.TYPE_ID = 2;
            pm.VOUCHER_NO = "";
            //pm.DEBIT_AMOUNT = Convert.ToDecimal(txtBalance.Text);
            pm.IS_DELETED = false;
            pm.PURCHASE_MASTER_ID = Convert.ToInt32(Session["PurchasMasterID"]);
            pml.Update(pm);
            DbCalls db = new DbCalls();
            var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher  && x.DOCUMENT_NO == pm.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
            if (q.Count > 0)
            {
                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + ", " + ItemInfo;
            //LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.PURCHASE_AMOUNT)
            //   , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
            //   , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
            //   , Convert.ToString(pm.PURCHASE_MASTER_ID)
            //   , Convert.ToDecimal(pm.DISCOUNT), Convert.ToDecimal(pm.TOKEN_AMOUNT), Convert.ToDecimal(pm.DOWN_PAYMENT), Info, Convert.ToDecimal(txtCommisionAmount.Text));
            Session["DBT_Amount"] = null;
            Session["BAL_Amount"] = null;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PurchasMasterID"] = null;
            mastergrid.Visible = true;
            detailgrid.Visible = false;
        }
        else
        {
            pm.TYPE_ID = 2;
            pm.VOUCHER_NO = "";
            //pm.DEBIT_AMOUNT = Convert.ToDecimal(txtBalance.Text);
            pm.IS_DELETED = false;
            pml.Save(pm);
            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + ", " + ItemInfo;
            //LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.PURCHASE_AMOUNT)
            //    , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
            //    , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
            //    , Convert.ToString(pm.PURCHASE_MASTER_ID)
            //    , Convert.ToDecimal(pm.DISCOUNT), Convert.ToDecimal(pm.TOKEN_AMOUNT), Convert.ToDecimal(pm.DOWN_PAYMENT), Info, Convert.ToDecimal(txtCommisionAmount.Text));
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadGrid("");
        DoEmptyTextBox();
        mastergrid.Visible = true;
        detailgrid.Visible = false;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
        Panel3.Visible = true;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        panelgrid.Visible = true;
        mastergrid.Visible = true;
        PanelPerchase.Visible = false;
        Panel3.Visible = true;
        DoEmptyTextBox();
        Items(2);
        LoadBlock("");
        LoadStreet();
        LoadItems("");
    }

    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        Grid_users.PageIndex = e.NewPageIndex;
        LoadDetailGrid();
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
        }
        else
        {
            ddlCommissionPercent.SelectedIndex = 0;
            txtCommissionValue.ReadOnly = false;
            ddlCommissionPercent.Enabled = false;
            txtCommisionAmount.Text = "0";
            txtCommissionValue.Focus();
        }
    }

    protected void ddlCommissionPercent_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCommissionAgent.SelectedValue == "0")
        {
            ddlCommissionPercent.SelectedValue = "0";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please select commission agent first.');", true);
            ddlCommissionAgent.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
            return;
        }
        if (RbtCommissionType.SelectedValue == "1")
        {
            if (txtPurchaseAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                txtPurchaseAmount.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                ddlCommissionPercent.SelectedValue = "0";
                return;
            }
            else
            {
                decimal discount = 0;
                if (txtDiscount.Text.Equals(""))
                { discount = 0; }
                else { discount = Convert.ToDecimal(txtDiscount.Text); }
                decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
                decimal itemamount = Convert.ToDecimal(txtBalance.Text);
                //decimal purchase_amount = itemamount - discount;
                txtCommissionValue.Text = "0";
                decimal Amount = itemamount / 100 * Percent;
                txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
                txtRemarks.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
            }
        }
        else
        {
            if (txtPurchaseAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                txtPurchaseAmount.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
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
        txtRemarks.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
    }

    protected void txtPurchaseAmount_TextChanged(object sender, EventArgs e)
    {
        if (RbtCommissionType.SelectedValue == "1")
        {
            if (txtPurchaseAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                txtPurchaseAmount.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                ddlCommissionPercent.SelectedValue = "0";
                return;
            }
            else
            {
                decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
                decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
                decimal discount = 0;
                if (txtDiscount.Text.Equals(""))
                { discount = 0; }
                else { discount = Convert.ToDecimal(txtDiscount.Text); }
                decimal Purchase_amount = itemamount - discount;
                txtCommissionValue.Text = "0";
                decimal Amount = Purchase_amount / 100 * Percent;
                txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
                //txtBalance.Text = Math.Round(itemamount, 2).ToString();
                if (txtTokenAmount.Text.Equals(""))
                {
                    txtTokenAmount.Text = "0";
                }
                if (txtDownPayment.Text.Equals(""))
                {
                    txtDownPayment.Text = "0";
                }
                decimal token = Convert.ToDecimal(txtTokenAmount.Text);
                decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
                txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
                txtDiscount.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtDiscount);
                txtDiscount.Attributes.Add("onfocusin", " select();");
            }
        }
        else
        {
            if (txtPurchaseAmount.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchase amount first.');", true);
                txtPurchaseAmount.Focus();
                //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                ddlCommissionPercent.SelectedValue = "0";
                return;
            }
            else
            {
                if (txtCommissionValue.Text.Equals(""))
                {
                    txtCommissionValue.Text = "0";
                    txtCommisionAmount.Text = "0";
                    //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
                }
                else
                {
                    decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
                    string Commission = txtCommissionValue.Text;
                    txtCommisionAmount.Text = Commission;
                    //txtBalance.Text = Math.Round(itemamount, 2).ToString();
                    if (txtTokenAmount.Text.Equals(""))
                    {
                        txtTokenAmount.Text = "0";
                    }
                    if (txtDownPayment.Text.Equals(""))
                    {
                        txtDownPayment.Text = "0";
                    }
                    //decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
                    decimal discount = 0;
                    if (txtDiscount.Text.Equals(""))
                    { discount = 0; }
                    else { discount = Convert.ToDecimal(txtDiscount.Text); }
                    decimal token = Convert.ToDecimal(txtTokenAmount.Text);
                    decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
                    txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
                    txtDiscount.Focus();
                    //ScriptManager.GetCurrent(Page).SetFocus(txtDiscount);
                    txtDiscount.Attributes.Add("onfocusin", " select();");

                }

            }
        }
    }

    protected void PurchaseMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        PurchaseMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["PurchasMasterID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update. Some payments are issued against this purchase.');", true);
        }
        else
        {
            //var q = (from k in db.tbl_SALE_MASTER
            //         join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
            //         select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
            //if (q.Any())
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update because this item has been sold.');", true);
            //}
            //else
            //{
            DataTable dt = new DataTable();
            DataTable dtItem = new DataTable();
            Items(2);
            Items(8);
            //dtItem = (DataTable)this.Session["dtItem"];
            //DataRow[] foundRows = dtItem.Select("ITEM_INFO_ID  = '" + data.ITEM_INFO_ID + "'");
            //if (foundRows.Length > 0)
            //{
            //    dt = (DataTable)this.Session["dt"];
            //    DataRow dr = dt.NewRow();
            //    dr["DISTRIBUTOR_ID"] = Convert.ToInt32(foundRows[0]["DISTRIBUTOR_ID"].ToString());
            //    dr["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
            //    dr["PROJECT_NAME"] = foundRows[0]["PROJECT_NAME"].ToString();
            //    dr["BLOCK_ID"] = Convert.ToInt32(foundRows[0]["BLOCK_ID"].ToString());
            //    dr["BLOCK_NAME"] = foundRows[0]["BLOCK_NAME"].ToString();
            //    dr["STREET_ID"] = Convert.ToInt32(foundRows[0]["STREET_ID"].ToString());
            //    dr["STREET_NAME"] = foundRows[0]["STREET_NAME"].ToString();
            //    dr["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
            //    dr["ITEM_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
            //    dr["Consumption"] = foundRows[0]["Consumption"].ToString();
            //    //dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
            //    dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
            //    dt.Rows.Add(dr);
            //    Session["dt"] = dt;
            //}
            ddlProject.DataSource = DropDowns.PROJECTDropDown();
            ddlProject.DataTextField = "PROJECT_NAME";
            ddlProject.DataValueField = "PROJECT_ID";
            ddlProject.DataBind();
            ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
            ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
            ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
            ddlLocation.DataValueField = "DISTRIBUTOR_ID";
            ddlLocation.DataBind();
            //LoadBlock("");
            //ddlBlock.SelectedValue = Convert.ToString(data.BLOCK_ID);

            //LoadStreet();
            //ddlStreet.SelectedValue = Convert.ToString(data.STREET_ID);

            //LoadItems(ProID);
            //ddlItem.SelectedValue = Convert.ToString(data.ITEM_INFO_ID);
            ddlVendor.DataSource = DropDowns.VendorDropDown();
            ddlVendor.DataTextField = "VENDOR_NAME";
            ddlVendor.DataValueField = "VENDOR_ID";
            ddlVendor.DataBind();
            ddlVendor.SelectedValue = Convert.ToString(data.VENDOR_ID);
            ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
            ddlCommissionAgent.DataTextField = "VENDOR_NAME";
            ddlCommissionAgent.DataValueField = "VENDOR_ID";
            ddlCommissionAgent.DataBind();
            ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
            ddlCommissionAgent.SelectedValue = Convert.ToString(data.COMMISSIONAGENT_ID);

            txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            //decimal purchaseamount = decimal.Parse(data.PURCHASE_AMOUNT.ToString());
            //txtPurchaseAmount.Text = Convert.ToString(Math.Round(purchaseamount, 2));

            //decimal discountamount = decimal.Parse(data.DISCOUNT.ToString());
            //txtDiscount.Text = Convert.ToString(Math.Round(discountamount, 2));

            //decimal tokenamount = decimal.Parse(data.TOKEN_AMOUNT.ToString());
            //txtTokenAmount.Text = Convert.ToString(Math.Round(tokenamount, 2));

            //decimal downpayment = decimal.Parse(data.DOWN_PAYMENT.ToString());
            //txtDownPayment.Text = Convert.ToString(Math.Round(downpayment, 2));

            //decimal balance = decimal.Parse(data.BALANCE.ToString());
            //txtBalance.Text = Convert.ToString(Math.Round(balance, 2));

            //ddlCommissionPercent.SelectedValue = Convert.ToString(data.COMMISSION_PERCENT);
            //if (ddlCommissionPercent.SelectedValue != "0")
            //{
            //    txtCommissionValue.Text = "0";
            //    decimal amount = Convert.ToDecimal(data.BALANCE / 100 * data.COMMISSION_PERCENT);
            //    txtCommisionAmount.Text = Convert.ToString(Math.Round(amount, 2));
            //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
            //    RbtCommissionType.SelectedValue = "1";
            //}
            //else
            //{
            //    RbtCommissionType.SelectedValue = "2";
            //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
            //    txtCommisionAmount.Text = Convert.ToString(data.COMMISSION_VALUE);
            //}
            //txtRemarks.Text = data.REMARKS;
            //txtDebitAmount.Text = Convert.ToString(data.DEBIT_AMOUNT);
            //Session.Add("BAL_Amount", data.BALANCE);
            //Session.Add("DBT_Amount", data.DEBIT_AMOUNT);
            Session.Add("PurchasMasterID", data.PURCHASE_MASTER_ID);
            mastergrid.Visible = false;
            detailgrid.Visible = true;
            panelgrid.Visible = false;
            PanelPerchase.Visible = true;
            btnSave.Text = "Update";
            Panel3.Visible = false;
        //}
    }
        
    }

    protected void Grid_users_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowNo = e.NewEditIndex;
        txtPaymnetDate.Text = Grid_users.Rows[e.NewEditIndex].Cells[0].Text;
        ddlPaymentMode.SelectedItem.Text = Grid_users.Rows[e.NewEditIndex].Cells[1].Text;
        txtDebitAmount.Text = Grid_users.Rows[e.NewEditIndex].Cells[2].Text;
        txtPaymnetRemarks.Text = Grid_users.Rows[e.NewEditIndex].Cells[3].Text;
        btnAddPurPayment.Text = "Update Payment";
    }

    protected void Grid_users_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        if (PurchaseDetail.Rows.Count > 0)
        {
            PurchaseDetail.Rows.RemoveAt(e.RowIndex);
            this.Session.Add("PurchaseDetail", PurchaseDetail);
            this.LoadDetailGrid();
            btnAddPurPayment.Text = "Add Payment";
        }
        btnAddPurPayment.Text = "Add Payment";
    }

    protected void txtTokenAmount_TextChanged(object sender, EventArgs e)
    {
        decimal discount = 0;
        decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
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
        decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        if (txtDiscount.Text.Equals(""))
        { discount = 0; }
        else { discount = Convert.ToDecimal(txtDiscount.Text); }
        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool flag = false;
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not be deleted. Some payments are issued against this purchase.');", true);
        }
        else
        {
            try
            {
                //var qq = (from k in db.tbl_SALE_MASTER
                //         join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
                //         select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
                //if (qq.Any())
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete because this item has been sold.');", true);
                //}
                //else
                //{
                    var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
                    if (!Active)
                    {
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_MASTER where PURCHASE_MASTER_ID =" + Convert.ToInt32(ProID), con))
                            {
                                command.ExecuteNonQuery();
                            }
                            if (q.Count > 0)
                            {
                                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                                using (SqlCommand command3 = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and PRINCIPAL_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                                using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                            }
                            con.Close();
                        }
                        flag = true;
                        if (flag)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                        }
                    }
                //}
            }
            catch (Exception ex) { }
        }
        LoadGrid("");
    }

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
        LoadStreet();
        LoadItems("");
        ddlBlock.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
    }

    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        panelgrid.Visible = false;
        PanelPerchase.Visible = true;
        ddlInstallmentType.Visible = false;
        txtNoofInstallment.Visible = false;
        lblInstallmentType.Visible = false;
        lblNoofInstallment.Visible = false;
        lblFromMonth.Visible = false;
        txtFromMonth.Visible = false;
        ibtnStartMonth.Visible = false;
        lblToketAmount.InnerText = "Token Amount";
        txtDownPayment.Enabled = true;
        txtDiscount.Text = "";
        ddlInstallmentType.SelectedValue = "1";
        txtNoofInstallment.Text = "";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        Hp.Enabled = true;
        Hp.SelectedValue = "1";
        DoEmptyTextBox();
        LoadVendors();
        Items(2);
        LoadProject();
        LoadBlock("");
        LoadStreet();
        LoadItems("");
        //LoadGrid("");
        Panel3.Visible = false;
    }

    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        Session["PURCHASE_MASTER_ID"] = null;
        Session["VENDOR_ID"] = null;
        Session["BALANCE"] = null;
        Session["DIST_ID"] = null;
        Session["Purchase_Date"] = null;
        mPopUpLocation.Hide();
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
        Panel3.Visible = true;
        Items(2);
        LoadProject();
        LoadBlock("");
        LoadStreet();
        LoadItems("");
    }

    protected void BtnPopUpSave_Click(object sender, EventArgs e)
    {
        
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
                    mPopUpLocation.Show();
                    return false;
                }
            }

        }
        return true;
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

            DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            DataRow[] drConfig = null;

            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.Inventoryatstore + "'");
            long PurchaseAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());


            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.AccountPayable + "'");
            long PayableAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.PurchaseDiscount + "'");
            long PurchaseDiscount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            if (p_DOCUMENT_TYPE_ID == Constants.Document_Purchase)
            {
                //if (!Is_INS)
                //{
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase",0,Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                //}
                //else
                //{
                //    LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase");
                //    LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase");

                //    LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "BOOKING-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase");
                //    LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "BOOKING-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase");
                //}
                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Purchase
                        , p_MANUAL_DOCUMENT_NO, "Purchase Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Purchase", mTransaction, mConnection);
                    decimal Purchases = p_Credit - p_Discount_Amount;
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, Purchases, 0, "Purchase Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, 0, Purchases, "Purchase Voucher", mTransaction, mConnection);

                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Discount_Amount, 0, "Purchase Discount Voucher", mTransaction, mConnection);
                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchase_Discount, 0, p_Discount_Amount, "Purchase Discount Voucher", mTransaction, mConnection);

                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Commission_Charges, p_Commisson, 0, "Purchase Commission Voucher", mTransaction, mConnection);
                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Other_Payables, 0, p_Commisson, "Purchase Commission Voucher", mTransaction, mConnection);
                }
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



    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        if (txtPurchaseAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
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
        decimal itemamount = Convert.ToDecimal(txtPurchaseAmount.Text);
        decimal token = Convert.ToDecimal(txtTokenAmount.Text);
        decimal downpayment = Convert.ToDecimal(txtDownPayment.Text);
        decimal discount = Convert.ToDecimal(txtDiscount.Text);
        txtBalance.Text = Math.Round(itemamount - discount - token - downpayment, 2).ToString();
        ScriptManager.GetCurrent(Page).SetFocus(txtTokenAmount);
        txtTokenAmount.Attributes.Add("onfocusin", " select();");
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
        LoadItems("");
        ddlStreet.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlStreet);
    }

    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
        ddlItem.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
    }
}