using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using CORNBusinessLayer.Reports;
using System.Web;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;

public partial class Forms_frmDeleteVoucher : System.Web.UI.Page
{
    DataTable dtVoucher;
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    readonly BankController _bnkController = new BankController();
    readonly FinancialYearController _yearController = new FinancialYearController();
    readonly AccountHeadController mAccountController = new AccountHeadController();
    readonly ChequeEntryController CController = new ChequeEntryController();
    protected void Page_Load(object sender, EventArgs e)
    {

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            VoucherType();
            this.LoadDistributor();
            this.HideShowControls();
            string strProcessScript = "this.value='Wait...';this.disabled=true;";
            btnDone.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnDone, "").ToString());
        }
    }

    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDxComboBoxList(this.drpDistributor, dt, 0, 2, true);

        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }

    private void VoucherType()
    {
        //Load Voucher Type by User Rights 
        LedgerController mLedgerController = new LedgerController();
        DataTable dt = mLedgerController.SelectVoucherType(int.Parse(this.Session["UserId"].ToString()));
        clsWebFormUtil.FillDxComboBoxList(this.DrpVoucherType, dt, 0, 1, true);
        if (dt.Rows.Count > 0)
        {
            DrpVoucherType.SelectedIndex = 0;
        }
    }
    private void ClearAll()
    {
        txtpayeesName.Text = "";
        drpDistributor.Enabled = true;
        DrpVoucherType.Enabled = true;
        drpDistributor.Enabled = true;
        DrpVoucherSubType.Enabled = true;
        lblError.Text = "";
    }

    private void PrintVoucher(string VoucherNo)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        crpVoucherView CrpReport = new crpVoucherView();
        int VoucherType = Constants.IntNullValue;

        if (DrpVoucherType.SelectedItem.Value.ToString() == "16" || DrpVoucherType.SelectedItem.Value.ToString() == "812" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            VoucherType = Convert.ToInt32(DrpVoucherType.SelectedItem.Value.ToString());
        }
        else
        {
            if (DrpVoucherSubType.Items.Count > 0)
            {
                VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedItem.Value.ToString());
            }
        }

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VoucherType", DrpVoucherType.SelectedItem.Text);
        if (DrpVoucherType.SelectedItem.Value.ToString() == "16")
        {
            CrpReport.SetParameterValue("VoucherSubType", "Journal Voucher");
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "812")
        {
            CrpReport.SetParameterValue("VoucherSubType", "Opening Balance Voucher");
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            CrpReport.SetParameterValue("VoucherSubType", "Contra Voucher");
        }
        else
        {
            CrpReport.SetParameterValue("VoucherSubType", DrpVoucherSubType.SelectedItem.Text);
        }
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", 0);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }

    protected void DrpVoucherType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.ClearAll();
        this.HideShowControls();
        if (DrpVoucherType.SelectedItem.Value.ToString() != "16" && DrpVoucherType.SelectedItem.Value.ToString() != "812")
        {
            //DrpVoucherSubType_SelectedIndexChanged(null, null);
        }
        if (DrpVoucherType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            //this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
            //LoadAccountDetail();
        }
        else
        {
            //this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
            //LoadAccountDetail();
        }
        //this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));

        //txtDebitAmount.Enabled = true;
        //txtCreditAmount.Enabled = true;

        DrpVoucherType.Focus();

        if (DrpVoucherType.SelectedItem.Value.ToString() == "812")
        {
            //divImport.Visible = true;
        }
        else
        {
            //divImport.Visible = false;
        }
    }

    protected void btnDone_Click(object sender, EventArgs e)
    {
        int VoucherType = Constants.IntNullValue;

        if (DrpVoucherType.SelectedItem.Value.ToString() == "16")
        {
            VoucherType = Convert.ToInt32(DrpVoucherType.SelectedItem.Value);
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "14" || DrpVoucherType.SelectedItem.Value.ToString() == "15")
        {
            VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedItem.Value);
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            VoucherType = 25;
        }

        DataTable dt = RptCustCtl.DeleteVoucher(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVoucherType.SelectedItem.Value.ToString()), txtpayeesName.Text, 0);
        if (dt.Rows.Count > 0)
        {
            //ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Voucher Can't be deleted becuase its mapped with an invoice.');", true);
            lblError.Text = "Voucher Can't be deleted becuase its mapped with an invoice.";
        }
        else
        {
            dt = RptCustCtl.DeleteVoucher(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVoucherType.SelectedItem.Value.ToString()), txtpayeesName.Text, 1);
            //ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Voucher removed successfully.');", true);
            ClearAll();
            lblError.ForeColor = System.Drawing.Color.Green;
            lblError.Text = "Voucher removed successfully.";
        }


        //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //{
        //    con.Open();
        //    using (SqlCommand command = new SqlCommand("UPDATE CHEQUE_PROCESS set ISSUED = 1,  ISSUED_ACCOUNT_HEAD_ID = " + Convert.ToInt64(ddlAccountHead.SelectedItem.Value) + "  WHERE CHEQUE_PROCESS_ID=" + Convert.ToInt64(Session["CHEQUE_PROCESS_ID"]), con))
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    con.Close();
        //}



        //dtVoucher = (DataTable)this.Session["dtVoucher"];
        //if (dtVoucher.Rows.Count > 0)
        //{
        //    //if (IsDayClosed())
        //    //{
        //    //    UserController UserCtl = new UserController();

        //    //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
        //    //    Session.Clear();
        //    //    System.Web.Security.FormsAuthentication.SignOut();
        //    //    Response.Redirect("../Login.aspx");
        //    //}

        //    LedgerController mLController = new LedgerController();
        //    DataControl dc = new DataControl();
        //    dtVoucher = (DataTable)this.Session["dtVoucher"];
        //    DataTable dtHead = (DataTable)this.Session["dtHead"];

        //    decimal TotalDebit = 0;
        //    decimal TotalCredit = 0;
        //    decimal DiffAmount = 0;
        //    decimal ChqAmount = 0;
        //    #region Set Total

        //    foreach (DataRow dr in dtVoucher.Rows)
        //    {
        //        TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
        //        TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
        //    }
        //    DiffAmount = TotalDebit - TotalCredit;
        //    if (DiffAmount > 0)
        //    {
        //        ChqAmount = DiffAmount;
        //    }
        //    else
        //    {
        //        ChqAmount = -(DiffAmount);
        //    }
        //    #endregion

        //    #region In Case of Bank & Cash Voucher

        //    if (DrpVoucherType.Value.ToString() == "14" || DrpVoucherType.Value.ToString() == "15" || DrpVoucherType.Value.ToString() == "25")
        //    {
        //        decimal differnceAmt = TotalDebit - TotalCredit;

        //        if (differnceAmt > 0)
        //        {
        //            DataRow dr1 = dtVoucher.NewRow();
        //            dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
        //            dr1["ACCOUNT_HEAD_ID"] = drpBanks.SelectedItem.Value.ToString();
        //            dr1["ACCOUNT_CODE"] = "";
        //            dr1["ACCOUNT_NAME"] = drpBanks.SelectedItem.Text;
        //            dr1["REMARKS"] = txtAccountDes.Text;
        //            dr1["CREDIT"] = differnceAmt;
        //            dr1["DEBIT"] = 0;
        //            dr1["Principal"] = "";
        //            if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        //            {
        //                dr1["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
        //            }
        //            else
        //            {
        //                dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
        //            }
        //            //dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
        //            dtVoucher.Rows.Add(dr1);

        //        }
        //        else if (differnceAmt < 0)
        //        {
        //            DataRow dr1 = dtVoucher.NewRow();
        //            dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
        //            dr1["ACCOUNT_HEAD_ID"] = drpBanks.SelectedItem.Value.ToString();
        //            dr1["ACCOUNT_CODE"] = "";
        //            dr1["ACCOUNT_NAME"] = drpBanks.SelectedItem.Text;
        //            dr1["REMARKS"] = txtAccountDes.Text;
        //            dr1["CREDIT"] = 0;
        //            dr1["DEBIT"] = -(differnceAmt);
        //            dr1["Principal"] = "";
        //            if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        //            {
        //                dr1["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
        //            }
        //            else
        //            {
        //                dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
        //            }
        //            //dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
        //            dtVoucher.Rows.Add(dr1);
        //        }
        //    }

        //    #endregion

        //    int VoucherType = Constants.IntNullValue;
        //    if (DrpVoucherType.Items.Count > 0)
        //    {
        //        if (DrpVoucherType.SelectedItem.Value.ToString() == "16" || DrpVoucherType.SelectedItem.Value.ToString() == "812" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
        //        {
        //            VoucherType = Convert.ToInt32(DrpVoucherType.SelectedItem.Value.ToString());
        //        }
        //        else
        //        {
        //            VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedItem.Value.ToString());
        //        }
        //    }
        //    DateTime ChequeDate;
        //    DateTime Voucherdate;
        //    DateTime DueDate;
        //    DateTime TransactionDate;
        //    ChequeDate = Constants.DateNullValue;
        //    //if (txtDueDate.Text.Length > 0 && txtDueDate.Visible)
        //    //{
        //    //    DueDate = DateTime.Parse(txtDueDate.Text + " 00:00:00");
        //    //}
        //    //else
        //    //{
        //    DueDate = Constants.DateNullValue;
        //    //}
        //    if (txtTransactionDate.Text.Length > 0 && txtTransactionDate.Visible)
        //    {
        //        TransactionDate = DateTime.Parse(txtTransactionDate.Text + " 00:00:00");
        //    }
        //    else
        //    {
        //        TransactionDate = Constants.DateNullValue;
        //    }
        //    //if (drpBanks.SelectedItem.Value.ToString() == "84" && DrpPaymentMode.SelectedItem.Value.ToString() == "18" ||
        //    //        drpBanks.SelectedItem.Value.ToString() == "85" && DrpPaymentMode.SelectedItem.Value.ToString() == "44")
        //    if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "44")
        //    {
        //        if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
        //        {

        //        }
        //        else
        //        {
        //            if (CheckOpenChqPay())
        //            {
        //                string type = "";
        //                if (DrpPaymentMode.SelectedItem.Value.ToString() == "44")
        //                {
        //                    type = "a Pay Order.";
        //                }
        //                if (DrpPaymentMode.SelectedItem.Value.ToString() == "46")
        //                {
        //                    type = "an Own Cheque.";
        //                }
        //                else
        //                {
        //                    type = "an Open Cheque.";
        //                }
        //                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select " + type + "');", true);
        //                return;
        //            }
        //        }
        //    }
        //    string ChequeNo = null;
        //    if (DrpVoucherType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
        //    {
        //        if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
        //        {
        //            ChequeNo = txtChequeNo.Text;

        //        }
        //        else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "17")
        //        {
        //            ChequeNo = ChequeNO();
        //            //ChequeNo = drpChequeNo.SelectedItem.Text;
        //        }
        //    }

        //    Voucherdate = TransactionDate;
        //    int subtype_id = 0;
        //    string MaxDocumentId = mLController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Voucherdate);
        //    if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        //    {
        //        subtype_id = int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString());
        //    }
        //    if (TotalDebit == TotalCredit || DrpVoucherType.SelectedItem.Value.ToString() != "16")
        //    {
        //        try
        //        {
        //            bool IsSaveVoucher = mLController.Edit_Voucher(int.Parse(drpDistributor.SelectedItem.Value.ToString()), subtype_id, MaxDocumentId, VoucherType,
        //               TransactionDate, int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()), txtpayeesName.Text, txtRemarks.Text, TransactionDate, ChequeNo, dtVoucher, int.Parse(this.Session["UserId"].ToString())
        //               , "", DueDate, txtBankName.Text, txtFavouring.Text);
        //            if (IsSaveVoucher == true)
        //            {
        //                // Add Open Cheque & Pay Order in Cheque Process table
        //                if (DrpVoucherType.SelectedItem.Value.ToString() == "15" && DrpVoucherSubType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25" && DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
        //                {
        //                    if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "44" || DrpPaymentMode.SelectedItem.Value.ToString() == "34")
        //                    {
        //                        int ISSUED = 0;
        //                        if (DrpPaymentMode.SelectedItem.Value.ToString() == "34")
        //                        {
        //                            ISSUED = 1;
        //                        }
        //                        string Favouring = txtFavouring.Text;
        //                        int ChequeType = int.Parse(DrpPaymentMode.SelectedItem.Value.ToString());
        //                        DateTime Date = Constants.DateNullValue;
        //                        DateTime ChqDate = Convert.ToDateTime(txtTransactionDate.Text);
        //                        string chqdate = ChqDate.ToString("dd/MM/yyyy");
        //                        //string TransDate = txtTransactionDate.Text.Replace("-", "/");
        //                        Date = DateTime.Parse(ConvertDate.British_To_American(chqdate));
        //                        string ChqProccessID = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, 0, ChequeNo, txtBankName.Text, Date,
        //                            Date, Constants.DateNullValue, Date, ChqAmount, 529, DateTime.Now, int.Parse(Session["UserId"].ToString())
        //                            , "1", txtRemarks.Text, long.Parse(drpBanks.SelectedItem.Value.ToString()), 1, 0, 1, 0, 1, Favouring, ChequeType, ISSUED, "");
        //                    }
        //                }
        //                if (Session["CHEQUE_PROCESS_ID"] != null)
        //                {
        //                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //                    {
        //                        con.Open();
        //                        using (SqlCommand command = new SqlCommand("UPDATE CHEQUE_PROCESS set ISSUED = 1,  ISSUED_ACCOUNT_HEAD_ID = " + Convert.ToInt64(ddlAccountHead.SelectedItem.Value) + "  WHERE CHEQUE_PROCESS_ID=" + Convert.ToInt64(Session["CHEQUE_PROCESS_ID"]), con))
        //                        {
        //                            command.ExecuteNonQuery();
        //                        }
        //                        con.Close();
        //                    }
        //                }
        //                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " has been saved');", true);
        //                ClearAll();
        //                PrintVoucher(MaxDocumentId);
        //                LoadCheque(2);
        //                //Response.Redirect("Forms/frmVoucherEntry.aspx");
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some error occur wrong entry');", true);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some of entries are wrong');", true);
        //        }
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Debit must equal to credit balance');", true);
        //    }
        //    DrpVoucherType_SelectedIndexChanged(null, null);
        //}
        //else
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('No record found....!');", true);
        //}
    }

    private void HideShowControls()
    {
        if (DrpVoucherType.SelectedItem.Value.ToString() == "14")
        {
            
            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Receipt Voucher", "14"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Payment Voucher", "24"));
            SubType.Visible = true;
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "16")
        {

            SubType.Visible = false;
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "812")
        {
            txtpayeesName.Visible = false;
            lblpayeesName.Visible = false;
            SubType.Visible = false;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Receipt Voucher", "14"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Payment Voucher", "24"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Receipt Voucher", "15"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Payment Voucher", "17"));
            SubType.Visible = true;
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
        }
        else
        {
            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Receipt Voucher", "15"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Payment Voucher", "17"));
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
            SubType.Visible = true;
        }
        if (DrpVoucherSubType.Items.Count > 0)
        {
            DrpVoucherSubType.SelectedIndex = 0;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.ClearAll();
    }

    private bool IsDayClosed()
    {
        DistributorController DistrCtl = new DistributorController();
        try
        {
            DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["DISTRIBUTOR_ID"]), 3);
            if (dtDayClose.Rows.Count > 0)
            {
                if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

}