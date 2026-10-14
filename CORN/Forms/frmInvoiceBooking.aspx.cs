using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmInvoiceBooking : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    readonly DistributorController DController = new DistributorController();
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
            LoadDistributor();
            LoadData();
            Vendors();
            LoadAccountHead();
            LoadRecord("");
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
            drpDistributor.DataTextField = "DISTRIBUTOR_NAME";
            drpDistributor.DataValueField = "DISTRIBUTOR_ID";
            drpDistributor.DataBind();
            drpDistributor.SelectedIndex = 0;
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
    public void Vendors()
    {
        if (int.Parse(Hp.SelectedValue.ToString()) == 2)
        {
            // For Suppliers
            TYPE_ID = 5;
        }
        else
        {
            // For Customers
            TYPE_ID = 4;
        }
        if (drpDistributor.Items.Count > 0)
        {
            //DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 23);
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, TYPE_ID);
            if (dt.Rows.Count > 0)
            {
                //var qry = from k in dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == int.Parse(Hp.SelectedValue))
                //          select k;
                //if (qry.Any())
                //{
                    ddlSup_Customer.Items.Clear();
                    ddlSup_Customer.DataSource = dt;
                    ddlSup_Customer.DataTextField = "VENDOR_NAME";
                    ddlSup_Customer.DataValueField = "VENDOR_ID";
                    ddlSup_Customer.DataBind();
                    ddlSup_Customer.SelectedIndex = 0;
                //}
                //else
                //{
                //    ddlSup_Customer.Items.Clear();
                //    ddlSup_Customer.Items.Add(new ListItem("", "0"));
                //    ddlSup_Customer.SelectedIndex = 0;
                //}
            }
            else
            {
                ddlSup_Customer.Items.Clear();
                ddlSup_Customer.Items.Add(new ListItem("", "0"));
                ddlSup_Customer.SelectedIndex = 0;
            }
        }
    }
    public void LoadRecord(string Type)
    {
        DbCalls db = new DbCalls();
        int TYPE_ID = Constants.InvoiceBooking;
        if (Hp.SelectedValue != "1")
        {
            if (Type.Equals(""))
            {
                var qry = (from k in db.tbl_PURCHASE_MASTER
                           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
                           select new { MASTER_ID = k.PURCHASE_MASTER_ID, k.TYPE_ID, AMOUNT = k.TOTAL_BALANCE, k.DATE, k.REMARKS, d.DISTRIBUTOR_NAME, v.VENDOR_NAME }).Where(x => x.TYPE_ID == TYPE_ID).ToList();
                GrdInvoiceBooking.DataSource = qry;
                GrdInvoiceBooking.DataBind();
                int total = 0;
                foreach (GridViewRow row in GrdInvoiceBooking.Rows)
                {
                    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[3].Text), 0));
                    total += amount;
                }
                if (qry.Count() > 0)
                {
                    GrdInvoiceBooking.FooterRow.Cells[3].Text = string.Format("{0:n2}", total);
                    GrdInvoiceBooking.FooterRow.Cells[2].Text = "Total";
                    GrdInvoiceBooking.FooterRow.Cells[2].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[6].Visible = false;
                    //GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
                }
            }
            else
            {
                DataTable dt = LController.SelectDataForSearching(14);
                dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR REMARKS LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%'";
                if (dt.Rows.Count > 0)
                {
                    GrdInvoiceBooking.PageIndex = 0;
                }
                GrdInvoiceBooking.DataSource = dt;
                GrdInvoiceBooking.DataBind();
                int total = 0;
                foreach (GridViewRow row in GrdInvoiceBooking.Rows)
                {
                    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[3].Text), 0));
                    total += amount;
                }
                if (dt.Rows.Count > 0)
                {
                    GrdInvoiceBooking.FooterRow.Cells[3].Text = string.Format("{0:n2}", total);
                    GrdInvoiceBooking.FooterRow.Cells[2].Text = "Total";
                    GrdInvoiceBooking.FooterRow.Cells[2].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[6].Visible = false;
                    //GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
                }
            }

        }
        else
        {
            if (Type.Equals(""))
            {
                var qry = (from k in db.tbl_SALE_MASTER
                           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                           join v in db.VENDORS on k.CUSTOMER_ID equals v.VENDOR_ID
                           select new { MASTER_ID = k.SALE_MASTER_ID, k.TYPE_ID, AMOUNT = k.TOTAL_BALANCE, k.DATE, k.REMARKS, d.DISTRIBUTOR_NAME, v.VENDOR_NAME }).Where(x => x.TYPE_ID == TYPE_ID).ToList();
                GrdInvoiceBooking.DataSource = qry;
                GrdInvoiceBooking.DataBind();
                int total = 0;
                foreach (GridViewRow row in GrdInvoiceBooking.Rows)
                {
                    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[3].Text), 0));
                    total += amount;
                }
                if (qry.Count() > 0)
                {
                    GrdInvoiceBooking.FooterRow.Cells[3].Text = string.Format("{0:n2}", total);
                    GrdInvoiceBooking.FooterRow.Cells[2].Text = "Total";
                    GrdInvoiceBooking.FooterRow.Cells[2].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[6].Visible = false;
                    //GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
                }
            }
            else
            {
                DataTable dt = LController.SelectDataForSearching(13);
                dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR REMARKS LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%'";
                if (dt.Rows.Count > 0)
                {
                    GrdInvoiceBooking.PageIndex = 0;
                }
                GrdInvoiceBooking.DataSource = dt;
                GrdInvoiceBooking.DataBind();
                int total = 0;
                foreach (GridViewRow row in GrdInvoiceBooking.Rows)
                {
                    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[3].Text), 0));
                    total += amount;
                }
                if (dt.Rows.Count > 0)
                {
                    GrdInvoiceBooking.FooterRow.Cells[3].Text = string.Format("{0:n2}", total);
                    GrdInvoiceBooking.FooterRow.Cells[2].Text = "Total";
                    GrdInvoiceBooking.FooterRow.Cells[2].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[3].Font.Bold = true;
                    GrdInvoiceBooking.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GrdInvoiceBooking.FooterRow.Cells[6].Visible = false;
                    //GrdInvoiceBooking.FooterRow.Cells[7].Visible = false;
                }
            }
        }
    }
    public void Clear()
    {
        txtAmount.Text = "";
        txtInvoiceNumber.Text = "";
        txtRemarks.Text = "";
        txtSearch.Text = "";
        btnSave.Text = "Save";
        Session["InvoiceBooking"] = null;
    }

    public void LoadAccountHead()
    {
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, 1);
        clsWebFormUtil.FillDropDownList(DrpBankAccount, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_HEAD", true);
        if (dtHead.Rows.Count > 0)
        {
            DrpBankAccount.SelectedIndex = 0;
        }
        //Session.Add("dtHead", dtHead);
    }
    #endregion
    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataControl dc = new DataControl();
        LedgerController ledgerCtl = new LedgerController();
        long MasterID = Constants.LongNullValue;
        string PrincipalName = ddlSup_Customer.SelectedItem.Text;
        if (drpDistributor.SelectedValue.Equals("0") || drpDistributor.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Distributor is required.');", true);
            drpDistributor.Focus();
            return;
        }
        if (ddlSup_Customer.SelectedValue.Equals("0") || ddlSup_Customer.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + LabelSupplier.InnerText + " is required.');", true);
            ddlSup_Customer.Focus();
            return;
        }
        if (txtInvoiceNumber.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Invoice Number is required.');", true);
            txtInvoiceNumber.Focus();
            return;
        }
        if (txtAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Amount is required.');", true);
            txtAmount.Focus();
            return;
        }
        if (Hp.SelectedValue != "1")
        {
            ///////// Supplier Invoice Booking //////////
            PurchaseMasterLogics pml = new PurchaseMasterLogics();
            tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
            pm.DATE = Convert.ToDateTime(txtTransactionDate.Text);
            pm.DISTRIBUTOR_ID = Convert.ToInt32(drpDistributor.SelectedValue);
            pm.PROJECT_ID = Convert.ToInt32(0);
            pm.CATEGORY_ID = 0;
            pm.SUBCATEGORY_ID = 0;
            pm.VENDOR_ID = Convert.ToInt32(ddlSup_Customer.SelectedValue); 
            pm.COMMISSIONAGENT_ID = Convert.ToInt32(DrpBankAccount.SelectedValue);
            pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(0);
            pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(0);
            pm.TOTAL_BALANCE = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_COMMISSION = Convert.ToDecimal(0);
            pm.REMARKS = txtRemarks.Text;
            pm.USER_ID = Convert.ToInt32(Session["UserID"]);
            pm.TIME_STEMP = DateTime.Now;
            pm.TYPE_ID = Constants.InvoiceBooking;
            pm.VOUCHER_NO = txtInvoiceNumber.Text;
            pm.IS_DELETED = false;
           
            
            if (Session["InvoiceBooking"] != null)
            {
                int PURCHASE_DETAIL_ID = Convert.ToInt32(Session["InvoiceBookingDetail"]);
                pm.PURCHASE_MASTER_ID = Convert.ToInt32(Session["InvoiceBooking"]);
                pml.Update(pm);

                #region Purchase Detail

                //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                //{
                //    con.Open();
                //    using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_DETAIL where PURCHASE_MASTER_ID=" + pm.PURCHASE_MASTER_ID , con))
                //    {
                //        command.ExecuteNonQuery();
                //    }
                //    con.Close();
                //}

                PurchaseDetailLogics pdl = new PurchaseDetailLogics();
                tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();
                pd.PURCHASE_DETAIL_ID = PURCHASE_DETAIL_ID;
                pd.PURCHASE_MASTER_ID = Convert.ToInt32(Session["InvoiceBooking"]); 
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
                pdl.Update(pd);
                int MasterId = pd.PURCHASE_DETAIL_ID;

                #endregion

                DbCalls db = new DbCalls();
                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == MasterId && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking && x.IS_DELETED == 0).ToList();
                if (q.Count > 0)
                {
                    string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                    int DISTRIBUTOR_ID = q.FirstOrDefault().DISTRIBUTOR_ID;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(MasterId) + "and DOCUMENT_TYPE_ID=" + Constants.InvoiceBooking + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID=" + DISTRIBUTOR_ID, con))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID=" + DISTRIBUTOR_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                ledgerCtl.InsertVendorInvoiceBooking(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, Constants.InvoiceBooking
                 , Convert.ToDateTime(txtTransactionDate.Text), Convert.ToInt32(ddlSup_Customer.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text))
                 , int.Parse(this.Session["UserId"].ToString()), ref MasterID, PrincipalName, pd.PURCHASE_DETAIL_ID, pm.COMMISSIONAGENT_ID);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
                Session["InvoiceBooking"] = null;
                Session["InvoiceBookingDetail"] = null;

            }
            else
            {
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
                int MasterId = pd.PURCHASE_DETAIL_ID;

                ledgerCtl.InsertVendorInvoiceBooking(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, Constants.InvoiceBooking
                 , Convert.ToDateTime(txtTransactionDate.Text), Convert.ToInt32(ddlSup_Customer.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text))
                 , int.Parse(this.Session["UserId"].ToString()), ref MasterID, PrincipalName, pd.PURCHASE_DETAIL_ID, pm.COMMISSIONAGENT_ID);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }


        }
        else
        {
            ///////// Customer Invoice Booking //////////
            SaleMasterLogics pml = new SaleMasterLogics();
            tbl_SALE_MASTER pm = new tbl_SALE_MASTER();
            pm.DATE = Convert.ToDateTime(txtTransactionDate.Text);
            pm.DISTRIBUTOR_ID = Convert.ToInt32(drpDistributor.SelectedValue);
            pm.PROJECT_ID = Convert.ToInt32(0);
            pm.CATEGORY_ID = 0;
            pm.SUBCATEGORY_ID = 0;
            pm.CUSTOMER_ID = Convert.ToInt32(ddlSup_Customer.SelectedValue);
            pm.COMMISSIONAGENT_ID = Convert.ToInt32(DrpBankAccount.SelectedValue);
            pm.TOTAL_SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(0);
            pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(0);
            pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(0);
            pm.TOTAL_BALANCE = Convert.ToDecimal(txtAmount.Text);
            pm.TOTAL_COMMISSION = Convert.ToDecimal(0);
            pm.REMARKS = txtRemarks.Text;
            pm.USER_ID = Convert.ToInt32(Session["UserID"]);
            pm.TIME_STEMP = DateTime.Now;
            pm.TYPE_ID = Constants.InvoiceBooking;
            pm.VOUCHER_NO = txtInvoiceNumber.Text;
            pm.IS_DELETED = false;
           
           
            if (Session["InvoiceBooking"] != null)
            {
                int SALE_DETAIL_ID = Convert.ToInt32(Session["InvoiceBookingDetail"]);
                pm.SALE_MASTER_ID = Convert.ToInt32(Session["InvoiceBooking"]);
                pml.Update(pm);

                #region SALE DETAIL
                //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                //{
                //    con.Open();
                //    using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_DETAIL where SALE_MASTER_ID=" + pm.SALE_MASTER_ID, con))
                //    {
                //        command.ExecuteNonQuery();
                //    }
                //    con.Close();
                //}
                int SALE_MASTER_ID = pm.SALE_MASTER_ID;
                SaleDetailLogics pdl = new SaleDetailLogics();
                tbl_SALE_DETAIL pd = new tbl_SALE_DETAIL();
                pd.SALE_DETAIL_ID = SALE_DETAIL_ID;
                pd.SALE_MASTER_ID = SALE_MASTER_ID;
                pd.ITEM_INFO_ID = 0;
                pd.PROJECT_ID = 0;
                pd.CATEGORY_ID = 0;
                pd.SUB_CATEGORY_ID = 0;
                pd.SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                pd.DISCOUNT = Convert.ToDecimal(0);
                pd.TOKEN_AMOUNT = 0;
                pd.DOWN_PAYMENT = 0;
                pd.COMMISSION_PERCENT = 0;
                pd.COMMISSION_VALUE = 0;
                pd.COMMISSION = 0;
                pd.BALANCE = Convert.ToDecimal(txtAmount.Text);
                pd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                pd.SALE_CLOSED = Convert.ToBoolean(0);
                pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                pd.TIME_STEMP = DateTime.Now;
                pd.IS_DELETED = Convert.ToBoolean(0);
                pd.REF_ID = 0;
                pdl.Update(pd);
                int MasterId = pd.SALE_DETAIL_ID;
                #endregion

                DbCalls db = new DbCalls();
                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == MasterId && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking && x.IS_DELETED == 0).ToList();
                if (q.Count > 0)
                {
                    string VNO = q.FirstOrDefault().VOUCHER_NO;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VNO + "' and DOCUMENT_NO=" + Convert.ToInt64(MasterId) + "and DOCUMENT_TYPE_ID=" + Constants.InvoiceBooking + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                ledgerCtl.InsertCustomerInvoiceBooking(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, Constants.InvoiceBooking
                 , Convert.ToDateTime(txtTransactionDate.Text), Convert.ToInt32(ddlSup_Customer.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text))
                 , int.Parse(this.Session["UserId"].ToString()), ref MasterID, PrincipalName, MasterId.ToString(), Convert.ToInt32(pm.COMMISSIONAGENT_ID));

                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
                Session["InvoiceBooking"] = null;
                Session["InvoiceBookingDetail"] = null;

            }
            else
            {
                pml.Save(pm);

                int SALE_MASTER_ID = pm.SALE_MASTER_ID;
                SaleDetailLogics pdl = new SaleDetailLogics();
                tbl_SALE_DETAIL pd = new tbl_SALE_DETAIL();

                pd.SALE_MASTER_ID = SALE_MASTER_ID;
                pd.ITEM_INFO_ID = 0;
                pd.PROJECT_ID = 0;
                pd.CATEGORY_ID = 0;
                pd.SUB_CATEGORY_ID = 0;
                pd.SALE_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                pd.DISCOUNT = Convert.ToDecimal(0);
                pd.TOKEN_AMOUNT = 0;
                pd.DOWN_PAYMENT = 0;
                pd.COMMISSION_PERCENT = 0;
                pd.COMMISSION_VALUE = 0;
                pd.COMMISSION = 0;
                pd.BALANCE = Convert.ToDecimal(txtAmount.Text);
                pd.DEBIT_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                pd.SALE_CLOSED = Convert.ToBoolean(0);
                pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                pd.TIME_STEMP = DateTime.Now;
                pd.IS_DELETED = Convert.ToBoolean(0);
                pd.REF_ID = 0;
                pdl.Save(pd);
                int MasterId = pd.SALE_DETAIL_ID;

                ledgerCtl.InsertCustomerInvoiceBooking(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, Constants.InvoiceBooking
                 , Convert.ToDateTime(txtTransactionDate.Text), Convert.ToInt32(ddlSup_Customer.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtAmount.Text))
                 , int.Parse(this.Session["UserId"].ToString()), ref MasterID, PrincipalName, MasterId.ToString(),Convert.ToInt32(pm.COMMISSIONAGENT_ID));
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
                
            }
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
    protected void Hp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "1")
        {
            if (Session["InvoiceBooking"] != null)
            {
                Clear();
            }
            LabelSupplier.InnerText = "Supllier";
            LabelAccountHead.InnerText = "Debit Account Head";
            Vendors();
            LoadAccountHead();
            LoadRecord("");
        }
        else
        {
            if (Session["InvoiceBooking"] != null)
            {
                Clear();
            }
            LabelSupplier.InnerText = "Customer";
            LabelAccountHead.InnerText = "Credit Account Head";
            Vendors();
            LoadAccountHead();
            LoadRecord("");
        }
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
        if (Hp.SelectedValue != "1")
        {
            DbCalls db = new DbCalls();
            Session["InvoiceBooking"] = null;
            Session["InvoiceBookingDetail"] = null;
            var ProID = ((LinkButton)sender).CommandArgument;
            PurchaseMasterLogics Ser = new PurchaseMasterLogics();
            var data = Ser.Get(Convert.ToInt32(ProID));
            var PD = (from k in db.tbl_PURCHASE_DETAIL select new { k.PURCHASE_DETAIL_ID, k.PURCHASE_MASTER_ID }).Where(x => x.PURCHASE_MASTER_ID == data.PURCHASE_MASTER_ID).FirstOrDefault();
            //&& x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking
            var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == PD.PURCHASE_DETAIL_ID && x.IS_DELETED == 0).ToList();
            if (qry.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoice could not update. Some payments are issued against this Invoice.');", true);
            }
            else
            {
                Session["InvoiceBooking"] = null;
                Session["InvoiceBookingDetail"] = null;
                int UserID = Convert.ToInt32(Session["UserID"]);
                drpDistributor.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
                drpDistributor.DataTextField = "DISTRIBUTOR_NAME";
                drpDistributor.DataValueField = "DISTRIBUTOR_ID";
                drpDistributor.DataBind();
                drpDistributor.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);
                Vendors();
                LoadAccountHead();
                ddlSup_Customer.SelectedValue = Convert.ToString(data.VENDOR_ID);
                DrpBankAccount.SelectedValue = Convert.ToString(data.COMMISSIONAGENT_ID);
                txtTransactionDate.Text = data.DATE.ToString("dd-MMM-yyyy");
                decimal saleamount = decimal.Parse(data.TOTAL_BALANCE.ToString());
                txtAmount.Text = Convert.ToString(Math.Round(saleamount, 0));
                txtInvoiceNumber.Text = data.VOUCHER_NO;
                txtRemarks.Text = data.REMARKS;
                Session.Add("InvoiceBooking", data.PURCHASE_MASTER_ID);
                Session.Add("InvoiceBookingDetail", PD.PURCHASE_DETAIL_ID);
                PanelData.Visible = true;
                PanelGrid.Visible = false;
                PanelSearch.Visible = false;
                btnSave.Text = "Update";
            }
        }
        else
        {

            DbCalls db = new DbCalls();
            var ProID = ((LinkButton)sender).CommandArgument;
            SaleMasterLogics Ser = new SaleMasterLogics();
            var data = Ser.Get(Convert.ToInt32(ProID));
            var SD = (from k in db.tbl_SALE_DETAIL select new { k.SALE_DETAIL_ID, k.SALE_MASTER_ID }).Where(x => x.SALE_MASTER_ID == data.SALE_MASTER_ID).FirstOrDefault();
            //&& x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking
            var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_NO == SD.SALE_DETAIL_ID && x.IS_DELETED == 0).ToList();
            if (qry.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoice could not update. Some payments are received against this Invoice.');", true);
            }
            else
            {
                Session["InvoiceBooking"] = null;
                Session["InvoiceBookingDetail"] = null;
                int UserID = Convert.ToInt32(Session["UserID"]);
                drpDistributor.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
                drpDistributor.DataTextField = "DISTRIBUTOR_NAME";
                drpDistributor.DataValueField = "DISTRIBUTOR_ID";
                drpDistributor.DataBind();
                drpDistributor.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);
                Vendors();
                ddlSup_Customer.SelectedValue = Convert.ToString(data.CUSTOMER_ID);
                txtTransactionDate.Text = data.DATE.ToString("dd-MMM-yyyy");
                decimal saleamount = decimal.Parse(data.TOTAL_BALANCE.ToString());
                txtAmount.Text = Convert.ToString(Math.Round(saleamount, 0));
                txtInvoiceNumber.Text = data.VOUCHER_NO;
                txtRemarks.Text = data.REMARKS;
                Session.Add("InvoiceBooking", data.SALE_MASTER_ID);
                Session.Add("InvoiceBookingDetail", SD.SALE_DETAIL_ID);
                PanelData.Visible = true;
                PanelGrid.Visible = false;
                PanelSearch.Visible = false;
                btnSave.Text = "Update";
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        bool flag = false;
        if (Hp.SelectedValue != "1")
        {
            var ProID = ((LinkButton)sender).CommandArgument;
            PurchaseMasterLogics Ser = new PurchaseMasterLogics();
            var data = Ser.Get(Convert.ToInt32(ProID));
            bool Active = Convert.ToBoolean(data.IS_DELETED);
            var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.IS_DELETED == 0).ToList();
            if (qry.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not be deleted. Some payments are issued against this purchase.');", true);
            }
            else
            {
                try
                {
                    var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking && x.IS_DELETED == 0).ToList();
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
                                using (SqlCommand command3 = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and PRINCIPAL_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.InvoiceBooking + "and PAYMENT_MODE=" + Constants.CreditSale, con))
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
                }
                catch (Exception ex) { }
            }
            LoadRecord("");
        }
        else
        {
            var ProID = ((LinkButton)sender).CommandArgument;
            SaleMasterLogics Ser = new SaleMasterLogics();
            var data = Ser.Get(Convert.ToInt32(ProID));
            bool Active = Convert.ToBoolean(data.IS_DELETED);
            //var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.IS_DELETED == 0).ToList();
            //if (qry.Any())
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoice could not be deleted. Some payments are received against this Invoice.');", true);
            //}
            //else
            //{
            //    var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.InvoiceBooking && x.IS_DELETED == 0).ToList();
            //    try
            //    {
            //        if (!Active)
            //        {
            //            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            //            {
            //                con.Open();
            //                using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_MASTER where SALE_MASTER_ID=" + Convert.ToInt32(ProID), con))
            //                {
            //                    command.ExecuteNonQuery();
            //                }
            //                if (q.Count > 0)
            //                {
            //                    string VNO = q.FirstOrDefault().VOUCHER_NO;
            //                    using (SqlCommand command3 = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.SALE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.InvoiceBooking + "and PAYMENT_MODE=" + Constants.CreditSale, con))
            //                    {
            //                        command3.ExecuteNonQuery();
            //                    }
            //                    using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
            //                    {
            //                        command1.ExecuteNonQuery();
            //                    }
            //                    using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VNO + "'", con))
            //                    {
            //                        command2.ExecuteNonQuery();
            //                    }
            //                }

            //                con.Close();
            //            }
            //            flag = true;
            //            if (flag)
            //            {
            //                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
            //            }
            //        }
            //    }
            //    catch (Exception ex) { }
            //}
            LoadRecord("");
        }
    }
}