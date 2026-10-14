using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Forms_frmSaleBalances : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    readonly DistributorController DController = new DistributorController();
    readonly DbCalls db = new DbCalls();
    int TYPE_ID;
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!IsPostBack)
        {
            string strProcessScript = "this.value='Wait...';this.disabled=true;";
            btnSave.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnSave, "").ToString());
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtTransactionDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtDiscountAmount.Attributes.Add("readonly", "readonly");
            txtNetAmount.Attributes.Add("readonly", "readonly");
            LoadDistributor();
            LoadBalanceTypes();
            LoadData();
            LoadCustomer();
            LoadVendor();
            LoadRecord("");
            ddlBalanceType.SelectedIndex = 0;
            PanelSearch.Visible = true;
            PanelData.Visible = false;
            PanelGrid.Visible = true;
        }
    }
    #region Load
    private void LoadDistributor()
    {
        DbCalls db = new DbCalls();
        try
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            drpDistributor.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
            drpDistributor.TextField = "DISTRIBUTOR_NAME";
            drpDistributor.ValueField = "DISTRIBUTOR_ID";
            drpDistributor.DataBind();
            drpDistributor.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }
    private void LoadBalanceTypes()
    {
        try
        {
            ddlBalanceType.DataSource = DropDowns.BALANCE_TYPEDropDown();
            ddlBalanceType.TextField = "BalanceTypeName";
            ddlBalanceType.ValueField = "BalanceTypeID";
            ddlBalanceType.DataBind();
            ddlBalanceType.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }
    public void LoadData()
    {
        //DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 23);
        //Session.Add("dt", dt);
    }
    public void LoadCustomer()
    {
        TYPE_ID = 4;
        if (drpDistributor.Items.Count > 0)
        {
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, TYPE_ID);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.Items.Clear();
                ddlCustomer.DataSource = dt;
                ddlCustomer.TextField = "VENDOR_NAME";
                ddlCustomer.ValueField = "VENDOR_ID";
                ddlCustomer.DataBind();
                ddlCustomer.SelectedIndex = 0;
            }
            else
            {
                ddlCustomer.Items.Clear();
                ddlCustomer.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                ddlCustomer.SelectedIndex = 0;
            }
        }
    }
    public void LoadVendor()
    {
        TYPE_ID = 5;
        if (drpDistributor.Items.Count > 0)
        {
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, TYPE_ID);
            if (dt.Rows.Count > 0)
            {
                ddlVendor.Items.Clear();
                ddlVendor.DataSource = dt;
                ddlVendor.TextField = "VENDOR_NAME";
                ddlVendor.ValueField = "VENDOR_ID";
                ddlVendor.DataBind();
                ddlVendor.SelectedIndex = 0;
            }
            else
            {
                ddlVendor.Items.Clear();
                ddlVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                ddlVendor.SelectedIndex = 0;
            }
        }
    }
    public void LoadRecord(string Type)
    {
        DbCalls db = new DbCalls();
        int TYPE_ID = Constants.SaleBalance;
        if (Type.Equals(""))
        {
            var qry = (from k in db.tbl_SALE_MASTER
                       join dt in db.tbl_SALE_DETAIL on k.SALE_MASTER_ID equals dt.SALE_MASTER_ID
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       join c in db.VENDORS on k.CUSTOMER_ID equals c.VENDOR_ID
                       join v in db.VENDORS on k.COMMISSIONAGENT_ID equals v.VENDOR_ID
                       select new { MASTER_ID = k.SALE_MASTER_ID, k.TYPE_ID, AMOUNT = k.TOTAL_BALANCE, k.DATE, k.REMARKS, d.DISTRIBUTOR_NAME, CUSTOMER_NAME = c.VENDOR_NAME, VENDOR_NAME = v.VENDOR_NAME
                       , ITEM_DESC = k.ITEM_NAME + ", " + k.BLOCK_NAME + ", " + k.STREET_NAME, k.TOTAL_SALE_AMOUNT,k.TOTAL_DOWN_PAYMENT,k.TOTAL_BALANCE, dt.SALE_DETAIL_ID}).Where(x => x.TYPE_ID == TYPE_ID).ToList();
            GrdInvoiceBooking.DataSource = qry;
            GrdInvoiceBooking.DataBind();
            decimal total = 0;
            decimal totaldiscount = 0;
            decimal totalnetamount = 0;
            foreach (GridViewRow row in GrdInvoiceBooking.Rows)
            {
                decimal amount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                decimal discount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                decimal netamount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                total += amount;
                totaldiscount += discount;
                totalnetamount += netamount;
            }
            if (qry.Count() > 0)
            {
                GrdInvoiceBooking.FooterRow.Cells[4].Text = string.Format("{0:N2}", total);
                GrdInvoiceBooking.FooterRow.Cells[5].Text = string.Format("{0:N2}", totaldiscount);
                GrdInvoiceBooking.FooterRow.Cells[6].Text = string.Format("{0:N2}", totalnetamount);
                GrdInvoiceBooking.FooterRow.Cells[3].Text = "Total";
                GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[4].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[5].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[6].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
            }
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(16);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR CUSTOMER_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_DESC LIKE '%" + txtSearch.Text + "%'";
            if (dt.Rows.Count > 0)
            {
                GrdInvoiceBooking.PageIndex = 0;
            }
            GrdInvoiceBooking.DataSource = dt;
            GrdInvoiceBooking.DataBind();
            decimal total = 0;
            decimal totaldiscount = 0;
            decimal totalnetamount = 0;
            foreach (GridViewRow row in GrdInvoiceBooking.Rows)
            {
                decimal amount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                decimal discount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                decimal netamount = Convert.ToDecimal(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                total += amount;
                totaldiscount += discount;
                totalnetamount += netamount;
            }
            if (dt.Rows.Count > 0)
            {
                GrdInvoiceBooking.FooterRow.Cells[4].Text = string.Format("{0:N2}", total);
                GrdInvoiceBooking.FooterRow.Cells[5].Text = string.Format("{0:N2}", totaldiscount);
                GrdInvoiceBooking.FooterRow.Cells[6].Text = string.Format("{0:N2}", totalnetamount);
                GrdInvoiceBooking.FooterRow.Cells[3].Text = "Total";
                GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[4].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[5].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[6].Font.Bold = true;
                GrdInvoiceBooking.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
            }
        }
    }
    public void Clear()
    {
        txtAmount.Text = "";
        txtDiscPercent.Text = "";
        txtDiscountAmount.Text = "";
        txtNetAmount.Text = "";
        txtItemName.Text = "";
        txtBlockName.Text = "";
        txtStreet.Text = "";
        txtRemarks.Text = "";
        txtSearch.Text = "";
        btnSave.Text = "Save";
        txtDiscountAmount.Attributes.Add("readonly", "readonly");
        txtNetAmount.Attributes.Add("readonly", "readonly");
        Session["SaleBalances"] = null;
    }
    #endregion
    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        if (drpDistributor.SelectedItem.Value.Equals("0") || drpDistributor.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Distributor is required.');", true);
            drpDistributor.Focus();
            return;
        }
        if (ddlCustomer.SelectedItem.Value.Equals("0") || ddlCustomer.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + LabelSupplier.InnerText + " is required.');", true);
            ddlCustomer.Focus();
            return;
        }
        if (ddlVendor.SelectedItem.Value.Equals("0") || ddlVendor.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + LabelAccountHead.InnerText + " is required.');", true);
            ddlVendor.Focus();
            return;
        }
        if (txtAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Amount is required.');", true);
            txtAmount.Focus();
            return;
        }
        string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value), Convert.ToDateTime(txtTransactionDate.Text));
        ///////// Customer Sale Balances //////////
        SaleMasterLogics sml = new SaleMasterLogics();
        tbl_SALE_MASTER sm = new tbl_SALE_MASTER();
        sm.DATE = Convert.ToDateTime(txtTransactionDate.Text);
        sm.DISTRIBUTOR_ID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
        sm.CATEGORY_ID = Convert.ToInt32(ddlBalanceType.SelectedItem.Value);
        sm.PROJECT_ID = Convert.ToInt32(0);
        sm.SUBCATEGORY_ID = 0;
        sm.CUSTOMER_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
        sm.COMMISSIONAGENT_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
        sm.TOTAL_SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
        sm.TOTAL_DISCOUNT = Convert.ToDecimal(0);
        sm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(0);
        sm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(txtDiscPercent.Text);
        sm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(txtDiscountAmount.Text);
        sm.TOTAL_BALANCE = Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text);
        sm.TOTAL_COMMISSION = Convert.ToDecimal(0);
        sm.REMARKS = txtRemarks.Text;
        sm.ITEM_NAME = txtItemName.Text;
        sm.BLOCK_NAME = txtBlockName.Text;
        sm.STREET_NAME = txtStreet.Text;
        sm.USER_ID = Convert.ToInt32(Session["UserID"]);
        sm.TIME_STEMP = DateTime.Now;
        sm.TYPE_ID = 4;
        sm.REF_ID = 0;
        sm.VOUCHER_NO = VoucherNo2; //txtInvoiceNumber.Text;
        sm.IS_DELETED = false;
        if (Session["SaleBalances"] != null)
        {
            int SALEMASTERID = Convert.ToInt32(Session["SaleBalances"]);
            sm.SALE_MASTER_ID = SALEMASTERID;
            var sdqry = (from k in db.tbl_SALE_DETAIL select k).Where(x => x.SALE_MASTER_ID == SALEMASTERID);
            int Sale_Deatil_ID = sdqry.FirstOrDefault().SALE_DETAIL_ID;
            var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == Sale_Deatil_ID && x.PAYMENT_MODE == Constants.SaleBalance && x.DOCUMENT_TYPE_ID == Constants.SaleBalance && x.IS_DELETED == 0).ToList();
            if (q.Count > 0)
            {
                string VNO = q.FirstOrDefault().VOUCHER_NO;
                var pqry = (from k in db.tbl_PURCHASE_MASTER select k).Where(x => x.VOUCHER_NO == VNO && x.REF_ID == SALEMASTERID);
                int Purchas_Master_ID = pqry.FirstOrDefault().PURCHASE_MASTER_ID;
                
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_DETAIL WHERE SALE_MASTER_ID =" + Convert.ToInt32(SALEMASTERID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command1 = new SqlCommand("DELETE tbl_PURCHASE_MASTER WHERE PURCHASE_MASTER_ID =" + Convert.ToInt32(Purchas_Master_ID), con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    using (SqlCommand command2 = new SqlCommand("DELETE tbl_PURCHASE_DETAIL WHERE PURCHASE_MASTER_ID =" + Convert.ToInt32(Purchas_Master_ID), con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    using (SqlCommand command3 = new SqlCommand("UPDATE CUSTOMER_LEDGER SET IS_DELETED = 1 WHERE DISTRIBUTOR_ID=" + Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()) + " AND VOUCHER_TYPE_ID = " + Constants.Journal_Voucher + "AND VOUCHER_NO='" + VNO + "' AND DOCUMENT_TYPE_ID=" + Constants.SaleBalance + "AND PAYMENT_MODE=" + Constants.SaleBalance, con))
                    {
                        command3.ExecuteNonQuery();
                    }
                    using (SqlCommand command4 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO='" + VNO + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), con))
                    {
                        command4.ExecuteNonQuery();
                    }
                    using (SqlCommand command5 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO='" + VNO + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), con))
                    {
                        command5.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            sml.Update(sm);

            #region SALE DETAIL
            SaleDetailLogics sdl = new SaleDetailLogics();
            tbl_SALE_DETAIL sd = new tbl_SALE_DETAIL();
            sd.SALE_MASTER_ID = sm.SALE_MASTER_ID;
            sd.ITEM_INFO_ID = 0;
            sd.PROJECT_ID = 0;
            sd.CATEGORY_ID = 0;
            sd.SUB_CATEGORY_ID = 0;
            sd.SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            sd.DISCOUNT = Convert.ToDecimal(txtDiscountAmount.Text);
            sd.TOKEN_AMOUNT = 0;
            sd.DOWN_PAYMENT = Convert.ToDecimal(txtDiscountAmount.Text);
            sd.COMMISSION_PERCENT = 0;
            sd.COMMISSION_VALUE = 0;
            sd.COMMISSION = 0;
            sd.BALANCE = Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text);
            sd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text);
            sd.SALE_CLOSED = Convert.ToBoolean(0);
            sd.USER_ID = Convert.ToInt32(Session["UserID"]);
            sd.TIME_STEMP = DateTime.Now;
            sd.IS_DELETED = Convert.ToBoolean(0);
            sd.REF_ID = 0;
            sdl.Save(sd);
            int SALE_MasterId = sd.SALE_DETAIL_ID;
            #endregion

            #region Purchase Balances Entry
            PurchaseMasterLogics pml = new PurchaseMasterLogics();
            tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
            pm.DATE = Convert.ToDateTime(txtTransactionDate.Text);
            pm.DISTRIBUTOR_ID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
            pm.PROJECT_ID = Convert.ToInt32(0);
            pm.CATEGORY_ID = Convert.ToInt32(ddlBalanceType.SelectedItem.Value);
            pm.SUBCATEGORY_ID = 0;
            pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
            pm.COMMISSIONAGENT_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
            pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(0);
            pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(0);
            pm.TOTAL_BALANCE = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_COMMISSION = Convert.ToDecimal(0);
            pm.REMARKS = txtRemarks.Text;
            pm.ITEM_NAME = txtItemName.Text;
            pm.BLOCK_NAME = txtBlockName.Text;
            pm.STREET_NAME = txtStreet.Text;
            pm.USER_ID = Convert.ToInt32(Session["UserID"]);
            pm.TIME_STEMP = DateTime.Now;
            pm.TYPE_ID = 4; //Constants.InvoiceBooking;
            pm.REF_ID = sm.SALE_MASTER_ID;
            pm.VOUCHER_NO = VoucherNo2; //txtInvoiceNumber.Text;
            pm.IS_DELETED = false;
            pml.Save(pm);

            PurchaseDetailLogics pdl = new PurchaseDetailLogics();
            tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();

            pd.PURCHASE_MASTER_ID = pm.PURCHASE_MASTER_ID;
            pd.ITEM_INFO_ID = 0;
            pd.PROJECT_ID = 0;
            pd.CATEGORY_ID = 0;
            pd.SUB_CATEGORY_ID = 0;
            pd.PURCHASE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pd.DISCOUNT = Convert.ToDecimal(0);
            pd.LESS_DISCOUNT = Convert.ToDecimal(0);
            pd.TOKEN_AMOUNT = 0;
            pd.DOWN_PAYMENT = 0;
            pd.COMMISSION_PERCENT = 0;
            pd.COMMISSION_VALUE = 0;
            pd.COMMISSION = 0;
            pd.BALANCE = Convert.ToDecimal(txtAmount.Text);
            pd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
            pd.USER_ID = Convert.ToInt32(Session["UserID"]);
            pd.TIME_STEMP = DateTime.Now;
            pd.IS_DELETED = Convert.ToBoolean(0);
            pdl.Save(pd);
            int PURCHASE_MasterId = pd.PURCHASE_DETAIL_ID;
            #endregion

            #region LEDGER ENTRY
            string Item_Detail = txtItemName.Text + ", " + txtBlockName.Text + ", " + txtStreet.Text;
            InsertCustomerBalances(Convert.ToInt32(drpDistributor.SelectedItem.Value), Convert.ToInt32(ddlCustomer.SelectedItem.Value), Convert.ToInt32(ddlVendor.SelectedItem.Value)
             , SALE_MasterId, PURCHASE_MasterId, 4, Convert.ToDateTime(txtTransactionDate.Text), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), Convert.ToDecimal(dc.chkNull_0(txtDiscountAmount.Text))
             , Convert.ToDecimal(Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text)), int.Parse(this.Session["UserId"].ToString()), ddlCustomer.SelectedItem.Text, ddlVendor.SelectedItem.Text
             , txtRemarks.Text, VoucherNo2, Item_Detail);
            #endregion
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["SaleBalances"] = null;
        }
        else
        {
            sml.Save(sm);

            #region SALE DETAIL
            SaleDetailLogics sdl = new SaleDetailLogics();
            tbl_SALE_DETAIL sd = new tbl_SALE_DETAIL();
            sd.SALE_MASTER_ID = sm.SALE_MASTER_ID;
            sd.ITEM_INFO_ID = 0;
            sd.PROJECT_ID = 0;
            sd.CATEGORY_ID = 0;
            sd.SUB_CATEGORY_ID = 0;
            sd.SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            sd.DISCOUNT = Convert.ToDecimal(txtDiscountAmount.Text);
            sd.TOKEN_AMOUNT = 0;
            sd.DOWN_PAYMENT = Convert.ToDecimal(txtDiscountAmount.Text);
            sd.COMMISSION_PERCENT = 0;
            sd.COMMISSION_VALUE = 0;
            sd.COMMISSION = 0;
            sd.BALANCE = Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text);
            sd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text);
            sd.SALE_CLOSED = Convert.ToBoolean(0);
            sd.USER_ID = Convert.ToInt32(Session["UserID"]);
            sd.TIME_STEMP = DateTime.Now;
            sd.IS_DELETED = Convert.ToBoolean(0);
            sd.REF_ID = 0;
            sdl.Save(sd);
            int SALE_MasterId = sd.SALE_DETAIL_ID;
            #endregion

            #region Purchase Balances Entry
            PurchaseMasterLogics pml = new PurchaseMasterLogics();
            tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
            pm.DATE = Convert.ToDateTime(txtTransactionDate.Text);
            pm.DISTRIBUTOR_ID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
            pm.PROJECT_ID = Convert.ToInt32(0);
            pm.CATEGORY_ID = Convert.ToInt32(ddlBalanceType.SelectedItem.Value);
            pm.SUBCATEGORY_ID = 0;
            pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
            pm.COMMISSIONAGENT_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
            pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(0);
            pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(0);
            pm.TOTAL_BALANCE = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_COMMISSION = Convert.ToDecimal(0);
            pm.REMARKS = txtRemarks.Text;
            pm.ITEM_NAME = txtItemName.Text;
            pm.BLOCK_NAME = txtBlockName.Text;
            pm.STREET_NAME = txtStreet.Text;
            pm.USER_ID = Convert.ToInt32(Session["UserID"]);
            pm.TIME_STEMP = DateTime.Now;
            pm.TYPE_ID = 4; //Constants.InvoiceBooking;
            pm.REF_ID = sm.SALE_MASTER_ID;
            pm.VOUCHER_NO = VoucherNo2; //txtInvoiceNumber.Text;
            pm.IS_DELETED = false;
            pml.Save(pm);

            PurchaseDetailLogics pdl = new PurchaseDetailLogics();
            tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();

            pd.PURCHASE_MASTER_ID = pm.PURCHASE_MASTER_ID;
            pd.ITEM_INFO_ID = 0;
            pd.PROJECT_ID = 0;
            pd.CATEGORY_ID = 0;
            pd.SUB_CATEGORY_ID = 0;
            pd.PURCHASE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pd.DISCOUNT = Convert.ToDecimal(0);
            pd.LESS_DISCOUNT = Convert.ToDecimal(0);
            pd.TOKEN_AMOUNT = 0;
            pd.DOWN_PAYMENT = 0;
            pd.COMMISSION_PERCENT = 0;
            pd.COMMISSION_VALUE = 0;
            pd.COMMISSION = 0;
            pd.BALANCE = Convert.ToDecimal(txtAmount.Text);
            pd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
            pd.USER_ID = Convert.ToInt32(Session["UserID"]);
            pd.TIME_STEMP = DateTime.Now;
            pd.IS_DELETED = Convert.ToBoolean(0);
            pdl.Save(pd);
            //int PURCHASE_MasterId = sd.SALE_DETAIL_ID;
            int PURCHASE_MasterId = pd.PURCHASE_DETAIL_ID;
            #endregion

            #region LEDGER ENTRY
            decimal Nem_Amount = Convert.ToDecimal(Convert.ToDecimal(txtAmount.Text) - Convert.ToDecimal(txtDiscountAmount.Text));
            string Item_Detail = txtItemName.Text + ", " + txtBlockName.Text + ", " + txtStreet.Text;
            InsertCustomerBalances(Convert.ToInt32(drpDistributor.SelectedItem.Value), Convert.ToInt32(ddlCustomer.SelectedItem.Value), Convert.ToInt32(ddlVendor.SelectedItem.Value)
             , SALE_MasterId, PURCHASE_MasterId, 4, Convert.ToDateTime(txtTransactionDate.Text), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), Convert.ToDecimal(dc.chkNull_0(txtDiscountAmount.Text))
             , Nem_Amount, int.Parse(this.Session["UserId"].ToString()), ddlCustomer.SelectedItem.Text,ddlVendor.SelectedItem.Text, txtRemarks.Text, VoucherNo2, Item_Detail);
            #endregion

            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadRecord("");
        Clear();
        PanelSearch.Visible = true;
        PanelData.Visible = false;
        PanelGrid.Visible = true;
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {

        PanelSearch.Visible = true;
        PanelData.Visible = false;
        PanelGrid.Visible = true;
    }
    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        PanelSearch.Visible = false;
        PanelData.Visible = true;
        PanelGrid.Visible = false;
        Clear();
    }
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadRecord(txtSearch.Text);
    }
    protected void PurchaseMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdInvoiceBooking.PageIndex = e.NewPageIndex;
        LoadRecord("");
    }
    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleMasterLogics Ser = new SaleMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var Datadetail = (from k in db.tbl_SALE_DETAIL select k).Where(x => x.SALE_MASTER_ID == data.SALE_MASTER_ID).ToList();
        int SaleDetail_ID = Datadetail.FirstOrDefault().SALE_DETAIL_ID;
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_NO == SaleDetail_ID && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Sale Balance could not update. Some payments are received against this transaction.');", true);
        }
        else
        {
            Session["SaleBalances"] = null;
            int UserID = Convert.ToInt32(Session["UserID"]);
            LoadDistributor();
            drpDistributor.Value = Convert.ToString(data.DISTRIBUTOR_ID);
            LoadCustomer();
            ddlCustomer.Value = Convert.ToString(data.CUSTOMER_ID);
            LoadVendor();
            ddlVendor.Value = Convert.ToString(data.COMMISSIONAGENT_ID);
            LoadBalanceTypes();
            ddlBalanceType.Value = Convert.ToString(data.CATEGORY_ID);
            txtTransactionDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            decimal saleamount = decimal.Parse(data.TOTAL_SALE_AMOUNT.ToString());
            txtAmount.Text = Convert.ToString(Math.Round(saleamount, 0));
            decimal discountper = decimal.Parse(data.TOTAL_TOKEN_AMOUNT.ToString());
            txtDiscPercent.Text = Convert.ToString(Math.Round(discountper, 0));
            decimal discountamount = decimal.Parse(data.TOTAL_DOWN_PAYMENT.ToString());
            txtDiscountAmount.Text = Convert.ToString(Math.Round(discountamount, 0));
            decimal Net_Amount = saleamount - discountamount;
            txtNetAmount.Text = Convert.ToString(Math.Round(Net_Amount, 0));
            txtItemName.Text = data.ITEM_NAME;
            txtBlockName.Text = data.BLOCK_NAME;
            txtStreet.Text = data.STREET_NAME;
            txtRemarks.Text = data.REMARKS;
            Session.Add("SaleBalances", data.SALE_MASTER_ID);
            PanelData.Visible = true;
            PanelGrid.Visible = false;
            PanelSearch.Visible = false;
            btnSave.Text = "Update";
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleMasterLogics Ser = new SaleMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        int SALEMASTERID = Convert.ToInt32(data.SALE_MASTER_ID);
        var sdqry = (from k in db.tbl_SALE_DETAIL select k).Where(x => x.SALE_MASTER_ID == SALEMASTERID);
        int Sale_Deatil_ID = sdqry.FirstOrDefault().SALE_DETAIL_ID;
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_TYPE_ID == Constants.SaleBalance && x.DOCUMENT_NO == Sale_Deatil_ID && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoice could not be deleted. Some payments are received against this Invoice.');", true);
        }
        else
        {
            var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_NO == Sale_Deatil_ID && x.PAYMENT_MODE == Constants.SaleBalance && x.DOCUMENT_TYPE_ID == Constants.SaleBalance && x.IS_DELETED == 0).ToList();
            try
            {
                if (!Active)
                {
                    string VNO = q.FirstOrDefault().VOUCHER_NO;
                    var pqry = (from k in db.tbl_PURCHASE_MASTER select k).Where(x => x.VOUCHER_NO == VNO && x.REF_ID == SALEMASTERID);
                    int Purchas_Master_ID = pqry.FirstOrDefault().PURCHASE_MASTER_ID;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_MASTER WHERE SALE_MASTER_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command0 = new SqlCommand("DELETE tbl_SALE_DETAIL WHERE SALE_MASTER_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command0.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand("DELETE tbl_PURCHASE_MASTER WHERE PURCHASE_MASTER_ID =" + Convert.ToInt32(Purchas_Master_ID), con))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("DELETE tbl_PURCHASE_DETAIL WHERE PURCHASE_MASTER_ID =" + Convert.ToInt32(Purchas_Master_ID), con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        if (q.Count > 0)
                        {
                            using (SqlCommand command3 = new SqlCommand("DELETE CUSTOMER_LEDGER WHERE DISTRIBUTOR_ID=" + Convert.ToInt32(data.DISTRIBUTOR_ID) + " AND VOUCHER_TYPE_ID = " + Constants.Journal_Voucher + "AND VOUCHER_NO='" + VNO + "' AND DOCUMENT_TYPE_ID=" + Constants.SaleBalance + "AND PAYMENT_MODE=" + Constants.SaleBalance, con))
                            {
                                command3.ExecuteNonQuery();
                            }
                            using (SqlCommand command4 = new SqlCommand("DELETE GL_MASTER WHERE VOUCHER_NO='" + VNO + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(data.DISTRIBUTOR_ID), con))
                            {
                                command4.ExecuteNonQuery();
                            }
                            using (SqlCommand command5 = new SqlCommand("DELETE GL_DETAIL WHERE VOUCHER_NO='" + VNO + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(data.DISTRIBUTOR_ID), con))
                            {
                                command5.ExecuteNonQuery();
                            }
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }
            }
            catch (Exception ex) { }
        }
        LoadRecord("");
    }
    public void InsertCustomerBalances(int p_DISTRIBUTOR_ID, int p_CustomerID, int p_PrincipalId, int p_SALE_INVOICE_ID, int p_PURCHASE_INVOICE_ID, int p_TYPE_ID, DateTime p_DOCUMENT_DATE
          , decimal p_TOTAL_AMOUNT, decimal p_DISCOUNT_AMOUNT, decimal p_NET_AMOUNT, int p_UserId, string Customer_Name, string Principal_Name, string pRemarks, string VoucherNo2, string p_Item_Detail)
    {

        IDbConnection mConnection = null;
        IDbTransaction mTransaction = null;
        try
        {
            string Remarks = "";
            string Remarks2 = "";
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            LedgerController LController = new LedgerController();

            Remarks = "Doc.# " + p_SALE_INVOICE_ID + " - " + p_PURCHASE_INVOICE_ID + " / " + Customer_Name + " / " + Principal_Name + " / " + p_Item_Detail + " / " + pRemarks;
            Remarks2 = "Doc.# " + p_PURCHASE_INVOICE_ID + " / " + Principal_Name + " / " + Customer_Name + " / " + p_Item_Detail + " / " + pRemarks;

            // For Customer
            LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, p_DISTRIBUTOR_ID, p_TOTAL_AMOUNT - p_DISCOUNT_AMOUNT, 0, p_DOCUMENT_DATE, "Sale Balance, " + Remarks, DateTime.Now, int.Parse(ddlCustomer.SelectedItem.Value.ToString()), p_SALE_INVOICE_ID, "", Constants.SaleBalance, p_UserId, mTransaction, mConnection, Constants.SaleBalance, "Sale Balance", 0, Constants.SaleBalance);
            //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, 149, p_DISTRIBUTOR_ID, p_DISCOUNT_AMOUNT, 0, p_DOCUMENT_DATE, "Discount Sale Balance, " + Remarks, DateTime.Now, int.Parse(ddlCustomer.SelectedItem.Value.ToString()), p_SALE_INVOICE_ID, "", Constants.SaleBalance, p_UserId, mTransaction, mConnection, Constants.SaleBalance, "Sale Balance", 0, Constants.SaleBalance);
            LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_DISTRIBUTOR_ID, 0, p_TOTAL_AMOUNT - p_DISCOUNT_AMOUNT, p_DOCUMENT_DATE, "Sale Balance, " + Remarks, DateTime.Now, int.Parse(ddlCustomer.SelectedItem.Value.ToString()), p_SALE_INVOICE_ID, "", Constants.SaleBalance, p_UserId, mTransaction, mConnection, Constants.SaleBalance, "Sale Balance", 0, Constants.SaleBalance);


            // For Vendor
            LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, p_DISTRIBUTOR_ID, 0, p_TOTAL_AMOUNT, p_DOCUMENT_DATE, "Sale Balance, " + Remarks, DateTime.Now, int.Parse(ddlVendor.SelectedItem.Value.ToString()), p_PURCHASE_INVOICE_ID, "", Constants.SaleBalance, p_UserId, mTransaction, mConnection, Constants.SaleBalance, "Sale Balance", 0, Constants.SaleBalance);
            LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_DISTRIBUTOR_ID, p_TOTAL_AMOUNT, 0, p_DOCUMENT_DATE, "Sale Balance, " + Remarks, DateTime.Now, int.Parse(ddlVendor.SelectedItem.Value.ToString()), p_PURCHASE_INVOICE_ID, "", Constants.SaleBalance, p_UserId, mTransaction, mConnection, Constants.SaleBalance, "Sale Balance", 0, Constants.SaleBalance);


            LController.PostingGLMaster(p_DISTRIBUTOR_ID, 0, VoucherNo2, Constants.Journal_Voucher, p_DOCUMENT_DATE, Constants.SaleBalance, p_SALE_INVOICE_ID + " - " + p_PURCHASE_INVOICE_ID, "Sale Balance Voucher / " + Customer_Name + " - " + Principal_Name + " / " + Remarks, p_UserId, Constants.SaleBalance.ToString(), mTransaction, mConnection);

            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, p_TOTAL_AMOUNT - p_DISCOUNT_AMOUNT, 0, "Trade Receivables Sale Balance Voucher", mTransaction, mConnection);
            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo2, 149, p_DISCOUNT_AMOUNT, 0, "Discount On Balances Sale Balance Voucher", mTransaction, mConnection);
            LController.PostingGLDetail(p_DISTRIBUTOR_ID, 0, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, 0, p_TOTAL_AMOUNT, "Trade Payables Sale Balance Voucher", mTransaction, mConnection);
            mTransaction.Commit();
        }
        catch (Exception exp)
        {
            ExceptionPublisher.PublishException(exp);
            mTransaction.Rollback();
        }
        finally
        {
            if (mConnection != null && mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
        }
    }
}