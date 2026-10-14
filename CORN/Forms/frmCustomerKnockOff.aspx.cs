using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmCustomerKnockOff : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    decimal Amount;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDistributor();
            LoadPrincipal();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            DrpVendor_SelectedIndexChanged(null, null);
            LoadAdvanceKnockOffGrid();
        }
    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, true);
        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    private void LoadPrincipal()
    {
        if (drpDistributor.Items.Count > 0)
        {
            var PController = new SKUPriceDetailController();
            DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 7, DateTime.Parse(Session["CurrentWorkDate"].ToString()));

            clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
            if (dtVendor.Rows.Count > 0)
            {
                DrpVendor.SelectedIndex = 0;
            }
        }
        //if (drpDistributor.Items.Count > 0)
        //{
        //    DbCalls db = new DbCalls();
        //    var qry = (from k in db.VENDORS
        //               select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
        //    DrpVendor.DataSource = qry;
        //    DrpVendor.TextField = "VENDOR_NAME";
        //    DrpVendor.ValueField = "VENDOR_ID";
        //    DrpVendor.DataBind();
        //    DrpVendor.SelectedIndex = 0;
        //    if (qry.Any())
        //    {
        //        int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
        //        loadPurchases(VendorID);
        //        LoadAdvancePayments();
        //    }
        //}
    }
    public void loadPurchases(int VendorID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, VendorID, Constants.IntNullValue, Constants.IntNullValue, null, 12);
        GrdCredit.DataSource = dt;
        GrdCredit.DataBind();
        CheckBox cb = null;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in GrdCredit.Rows)
        {
            cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
            //cb.Checked = true;
            TotalAmount += Convert.ToDecimal(gvr.Cells[4].Text);
        }
        if (dt.Rows.Count > 0)
        {
            //string.Format("{0:n2}", TotalAmount);
            //if (TotalAmount == 0)
            //{
            //    GrdCheque.FooterRow.Cells[4].Text = "0";
            //}
            //else
            //{
            //    GrdCheque.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            //}
            //GrdCredit.FooterRow.Cells[3].Text = "Total";
            //GrdCredit.FooterRow.Cells[3].Font.Bold = true;
            //GrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            //GrdCredit.FooterRow.Cells[4].Font.Bold = true;
            //GrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            GrdCredit.FooterRow.Cells[1].Visible = false;
            GrdCredit.FooterRow.Cells[5].Visible = false;
            GrdCredit.FooterRow.Cells[6].Visible = false;
            GrdCredit.FooterRow.Cells[7].Visible = false;
            GrdCredit.FooterRow.Cells[8].Visible = false;
        }
        Panel1.Visible = true;
        Session["PurchaseDetail"] = null;
    }
    private void LoadAdvancePayments()
    {
        ChequeEntryController CController = new ChequeEntryController();
        LedgerController LController = new LedgerController();
        DateTime Date = Constants.DateNullValue;
        //string abd = txtTransactionDate.Text.Replace("-", "/");
        //Date = DateTime.Parse(ConvertDate.British_To_American(abd));
        if (drpDistributor.Items.Count > 0)
        {
            DataTable dtRealized = LController.SelectAdvanceTransctions(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 1,
                Date);
            if (dtRealized.Rows.Count > 0)
            {
                GridAdvance.DataSource = dtRealized;
                GridAdvance.DataBind();
                CheckBox cb = null;
                decimal TotalAmount = 0;
                foreach (GridViewRow gvr in GridAdvance.Rows)
                {
                    cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
                    //cb.Checked = true;
                    TotalAmount += Convert.ToDecimal(gvr.Cells[5].Text);
                }
                //string.Format("{0:n2}", TotalAmount);
                //GridAdvance.FooterRow.Cells[5].Text = string.Format("{0:n2}", TotalAmount);
                //GridAdvance.FooterRow.Cells[4].Text = "Total";
                //GridAdvance.FooterRow.Cells[4].Font.Bold = true;
                //GridAdvance.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                //GridAdvance.FooterRow.Cells[5].Font.Bold = true;
                //GridAdvance.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GridAdvance.FooterRow.Cells[1].Visible = false;
            }
            else
            {
                GridAdvance.DataSource = null;
                GridAdvance.DataBind();
            }
            Panel2.Visible = true;
        }
    }

    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
        loadPurchases(VendorID);
        LoadAdvancePayments();
        LoadAdvanceKnockOffGrid();
    }

    protected void DrpVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
        loadPurchases(VendorID);
        LoadAdvancePayments();
        LoadAdvanceKnockOffGrid();
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {

    }

    protected void GrdCheque_RowEditing(object sender, GridViewEditEventArgs e)
    {

    }

    protected void GrdCheque_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (CheckInvoice())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an invoice');", true);
            return;
        }
        if (CheckAdvancePayment())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select Advance Reciept.');", true);
            return;
        }
        //if (CheckAdvanceAmount())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoices amount should be greated then or equal to selected Advance Reciepts.');", true);
        //    return;
        //}
        if (btnSave.Text == "Save")
        {
            ChequeRealization();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Invoices are knocked off.');", true);
            int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            loadPurchases(VendorID);
            LoadAdvancePayments();
            txtReferenceNo.Text = "";
            txtRemarks.Text = "";
        }
    }
    public bool CheckInvoice()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                break;
            }
        }
        if (InvoiceCount == Constants.IntNullValue)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }
        return flag;
    }
    public bool CheckAdvancePayment()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GridAdvance.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                break;
            }
        }
        if (InvoiceCount == Constants.IntNullValue)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }
        return flag;
    }
    public bool CheckAdvanceAmount()
    {
        decimal AdvanceAmount = Constants.DecimalNullValue;
        decimal InvoiceAmount = Constants.DecimalNullValue;
        bool flag;
        foreach (GridViewRow dr in GridAdvance.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                AdvanceAmount += Convert.ToDecimal(dr.Cells[5].Text);
            }
        }

        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceAmount += Convert.ToDecimal(dr.Cells[4].Text);
            }
        }
        if (AdvanceAmount > InvoiceAmount)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    private void ChequeRealization()
    {
        string AdvanceVouchers = "";
        DateTime ChequeDate = Constants.DateNullValue;
        try
        {
            DateTime CDate = DateTime.Now;
            string CCDate = CDate.ToString("dd/MM/yyyy");
            string StartDate = CCDate.Replace("-", "/");
            ChequeDate = DateTime.Parse(ConvertDate.British_To_American(StartDate));
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Correct Cheque Date Pattern is DD/MM/YYYY.');", true);
            return;
        }

        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        int VoucherType;
        int TransactionType = 1;
        VoucherType = Constants.Journal_Voucher;
        decimal OfferAmount = 0;
        decimal realizeAmount = 0;
        string remarks = "";
        int ChequeType = Constants.IntNullValue;
        string Favouring = txtReferenceNo.Text;
        DateTime Date = ChequeDate;
        MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);

        Session.Add("VoucherNo", MaxDocumentId);
        string ChequeNo = null;
        decimal TotalAdvanceAmount = 0;
        decimal TotalPurchaseAmount = 0;
        #region Calculate Total Checked Advance & Purchases
        foreach (GridViewRow row in GridAdvance.Rows)
        {
            CheckBox ch = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ch.Checked == true)
            {
                TotalAdvanceAmount += decimal.Parse(row.Cells[5].Text);
                AdvanceVouchers += row.Cells[1].Text + ",";
            }
        }
        AdvanceVouchers = AdvanceVouchers.TrimEnd(',');
        foreach (GridViewRow row in GrdCredit.Rows)
        {
            CheckBox ch = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ch.Checked == true)
            {
                TotalPurchaseAmount += decimal.Parse(row.Cells[4].Text);
            }
        }
        #endregion
        OfferAmount = TotalAdvanceAmount;
        foreach (GridViewRow dr in GrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                int TYPE = Constants.Document_Advance_KnockOff;
                remarks = "Customer Advance Knock Off, " + DrpVendor.SelectedItem.Text + "/ " + dr.Cells[2].Text + "/ " + txtRemarks.Text;
                string manualNo;
                ChequeNo = AdvanceVouchers;
                manualNo = "Knock Off";
                if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
                {
                    realizeAmount += OfferAmount;
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, 0, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Advance_KnockOff,
                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, 50, "", "Customer (Knock Off)", ChequeDate, TransactionType, TYPE, Favouring, ChequeType);

                    OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        //using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + OfferAmount + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                        //{
                        //    command.ExecuteNonQuery();
                        //}
                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_DETAIL SET DEBIT_AMOUNT = " + OfferAmount + " WHERE SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    break;
                }
                else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
                {
                    realizeAmount += decimal.Parse(dr.Cells[4].Text);
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, 0, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Advance_KnockOff,
                     ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, 50, "", "Customer (Knock Off)", ChequeDate, TransactionType, TYPE, Favouring, ChequeType);

                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_DETAIL SET DEBIT_AMOUNT = " + 0 + ", SALE_CLOSED =" + 1 + " WHERE SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
        }
        foreach (GridViewRow row in GridAdvance.Rows)
        {
            CheckBox ch = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ch.Checked == true)
            {
                decimal advance = decimal.Parse(row.Cells[5].Text);
                decimal knocked = decimal.Parse(row.Cells[6].Text);
                string CUSTOMER_LEDGER_ID = row.Cells[1].Text;
                if (realizeAmount >= advance)
                {
                    realizeAmount = realizeAmount - advance;
                    using (SqlConnection mycon = new SqlConnection(Configuration.ConnectionString))
                    {
                        mycon.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER SET CHEQUE_TYPE = 1 WHERE CUSTOMER_LEDGER_ID IN ('" + CUSTOMER_LEDGER_ID + "')", mycon))
                        {
                            command.ExecuteNonQuery();
                        }
                        mycon.Close();
                    }
                }
                else
                {
                    decimal KNOCKED_OFF = knocked + realizeAmount;
                    using (SqlConnection mycon = new SqlConnection(Configuration.ConnectionString))
                    {
                        mycon.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER SET KNOCKED_OFF ='" + KNOCKED_OFF + "' WHERE CUSTOMER_LEDGER_ID IN ('" + CUSTOMER_LEDGER_ID + "')", mycon))
                        {
                            command.ExecuteNonQuery();
                        }
                        mycon.Close();
                    }
                }
            }
        }


        //bool CheckMultipPurchases = false;
        //decimal PurchasesAmount = 0;
        //Amount = 0;
        //foreach (GridViewRow row in GridAdvance.Rows)
        //{
        //    CheckBox ch = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
        //    if (ch.Checked == true)
        //    {
        //        OfferAmount = decimal.Parse(row.Cells[5].Text);
        //        PurchasesAmount += OfferAmount;
        //        AdvanceVouchers = row.Cells[1].Text;

        //        foreach (GridViewRow dr in GrdCredit.Rows)
        //        {
        //            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
        //            if (chRelized.Checked == true)
        //            {
        //                if (CheckMultipPurchases == false)
        //                {
        //                    Amount = decimal.Parse(dr.Cells[4].Text);
        //                    CheckMultipPurchases = true;
        //                }
        //                else
        //                {
        //                    Amount = decimal.Parse(dr.Cells[4].Text) - PurchasesAmount;
        //                }
        //                int TYPE;
        //                TYPE = Constants.Document_Advance_KnockOff;
        //                remarks = "Customer Advance Knock Off, " + DrpVendor.SelectedItem.Text + "/ " + dr.Cells[2].Text + "/ " + txtRemarks.Text;
        //                string manualNo;
        //                ChequeNo = AdvanceVouchers;
        //                manualNo = "Knock Off";
        //                if (Amount > OfferAmount)
        //                {
        //                    realizeAmount += OfferAmount;
        //                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, 0, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
        //                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Advance_KnockOff,
        //                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, 50, "", "Customer (Knock Off)", ChequeDate, TransactionType, TYPE, Favouring, ChequeType);

        //                    OfferAmount = Amount - OfferAmount;
        //                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //                    {
        //                        con.Open();
        //                        using (SqlCommand command = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT = " + OfferAmount + " where SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
        //                        {
        //                            command.ExecuteNonQuery();
        //                        }
        //                        con.Close();
        //                    }

        //                    break;
        //                }
        //                else if (Amount <= OfferAmount)
        //                {
        //                    realizeAmount += Amount;

        //                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, 0, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, Amount,
        //                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Advance_KnockOff,
        //                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, 50, "", "Customer (Knock Off)", ChequeDate, TransactionType, TYPE, Favouring, ChequeType);

        //                    OfferAmount = OfferAmount - Amount;
        //                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //                    {
        //                        con.Open();
        //                        using (SqlCommand command = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT = " + 0 + ", SALE_CLOSED =" + 1 + " where SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
        //                        {
        //                            command.ExecuteNonQuery();
        //                        }
        //                        using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["ITEM_LOG_ID"]), con))
        //                        {
        //                            command2.ExecuteNonQuery();
        //                        }
        //                        con.Close();
        //                    }
        //                }
        //            }
        //        }
        //        AdvanceVouchers = AdvanceVouchers.TrimEnd(',');
        //        if (realizeAmount >= decimal.Parse(row.Cells[5].Text))
        //        {
        //            using (SqlConnection mycon = new SqlConnection(Configuration.ConnectionString))
        //            {
        //                mycon.Open();
        //                using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER SET CHEQUE_TYPE = 1 WHERE CUSTOMER_LEDGER_ID IN ('" + AdvanceVouchers + "')", mycon))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //                mycon.Close();
        //            }
        //        }
        //    }
        //}
    

        DrpVendor_SelectedIndexChanged(null, null);
        LoadAdvanceKnockOffGrid();
    }
    public void LoadAdvanceKnockOffGrid()
    {
        DateTime Date;
        DateTime CDate = DateTime.Now;
        string CCDate = CDate.ToString("dd/MM/yyyy");
        string StartDate = CCDate.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(StartDate));
        DataTable dt = LController.AdvanceKnockOffTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, 50,
                        Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), 1);
        GrdCheque.DataSource = dt;
        GrdCheque.DataBind();
        decimal TotalAmount = 0;
        if (dt.Rows.Count > 0)
        {
            foreach (GridViewRow gvr in GrdCheque.Rows)
            {
                TotalAmount += Convert.ToDecimal(gvr.Cells[4].Text);
            }
            string.Format("{0:n2}", TotalAmount);
            GrdCheque.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            GrdCheque.FooterRow.Cells[3].Text = "Total";
            GrdCheque.FooterRow.Cells[3].Font.Bold = true;
            GrdCheque.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            GrdCheque.FooterRow.Cells[4].Font.Bold = true;
            GrdCheque.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            GrdCheque.FooterRow.Cells[1].Visible = false;
            GrdCheque.FooterRow.Cells[5].Visible = false;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {

    }
}