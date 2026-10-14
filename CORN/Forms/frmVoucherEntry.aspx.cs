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

public partial class Forms_frmVoucherEntry : System.Web.UI.Page
{
    DataTable dtVoucher;
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
            GetFinancialYear();
            //if (IsDayClosed())
            //{
            //    UserController UserCtl = new UserController();
            //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            //    Session.Clear();
            //    System.Web.Security.FormsAuthentication.SignOut();
            //    Response.Redirect("../Login.aspx");
            //}
            VoucherType();
            this.LoadDistributor();
            this.LoadAccountDetail();
            this.CreatTable();

            this.HideShowControls();
            string strProcessScript = "this.value='Wait...';this.disabled=true;";
            btnSave.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnSave, "").ToString());
            btnDone.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnDone, "").ToString());
            //btnDone.Attributes.Add("onclick", "return ValidateForm();");
            btnImport.Attributes.Add("onclick", "jQuery('#" + FileUpload1.ClientID + "').click();return false;");
            //txtDueDate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");
            txtTransactionDate.Text = ((DateTime)Session["CurrentWorkDate"]).ToString("dd-MMM-yyyy");
            txtTransactionDate.Attributes.Add("readonly", "readonly");
            RowId.Value = "-1";

            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;

            DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Cash", "19"));
            DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "30"));
            DrpPaymentMode.SelectedIndex = 0;
            this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
            //LoadCheque(2);
        }
    }
    public void GetFinancialYear()
    {
        try
        {
            DataTable dt = _yearController.SelectFinancialYear(null, null, null, null, Constants.ShortNullValue, false, true, 1);
            DateTime StartDate = Convert.ToDateTime(dt.Rows[0]["dtStart"].ToString());
            DateTime EndDate = Convert.ToDateTime(dt.Rows[0]["dtEnd"].ToString());
            CalendarExtender1.StartDate = StartDate;
            CalendarExtender1.EndDate = EndDate;
        }
        catch (IndexOutOfRangeException)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Please Start a financial year')", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
    }

    private void LoadAccountHead(int p_Parent)
    {
        DataTable dtHead = new DataTable();
        int TypeID;
        if (int.Parse(DrpVoucherType.SelectedItem.Value.ToString()) == 25)
        {
            TypeID = 4;
        }
        else if (p_Parent == 18)
        {
            TypeID = 46;  //TypeID = p_Parent;
        }
        else if (p_Parent == 44)
        {
            TypeID = 46;  //TypeID = p_Parent;
        }
        else if (p_Parent == 46 || p_Parent == 30 || p_Parent == 34)
        {
            TypeID = 46;
        }
        else if (p_Parent == 21)
        {
            TypeID = p_Parent;
        }
        else
        {
            TypeID = 1;
        }
        AccountHeadController mAccountController = new AccountHeadController();
        dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, TypeID);
        clsWebFormUtil.FillDxComboBoxList(drpBanks, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_HEAD", true);
        if (dtHead.Rows.Count > 0)
        {
            drpBanks.SelectedIndex = 0;
        }
        Session.Add("dtHead", dtHead);
        drpBanks_SelectedIndexChanged(null, null);
    }
    protected void drpBanks_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.Parse(DrpVoucherType.SelectedItem.Value.ToString()) == 15 && drpBanks.Items.Count > 0 || int.Parse(DrpVoucherType.SelectedItem.Value.ToString()) == 25 && drpBanks.Items.Count > 0)
        {
            //if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "46")
            //{
            LoadCheque(2);
            //}

        }
        else
        {
            PanelOpenChqPayOrder.Visible = false;
        }
    }
    private void CreatTable()
    {
        dtVoucher = new DataTable();
        dtVoucher.Columns.Add("LEDGER_ID", typeof(long));
        dtVoucher.Columns.Add("Account_Head_Id", typeof(long));
        dtVoucher.Columns.Add("Account_Code", typeof(string));
        dtVoucher.Columns.Add("Account_Name", typeof(string));
        dtVoucher.Columns.Add("Debit", typeof(decimal));
        dtVoucher.Columns.Add("Credit", typeof(decimal));
        dtVoucher.Columns.Add("Remarks", typeof(string));
        dtVoucher.Columns.Add("InvoiceNo", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(string));
        dtVoucher.Columns.Add("Principal", typeof(string));
        this.Session.Add("dtVoucher", dtVoucher);
    }
    public void OpenChequeGrid()
    {
        drpChequeNo.Items.Clear();
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(drpBanks.SelectedItem.Value.ToString()), 2);
        //clsWebFormUtil.FillDxComboBoxList(this.drpCheque, dt, 0, 1, true);
        if (dt.Rows.Count > 0)
        {
            decimal TotalAmount = 0;
            GridChqPayOrder.DataSource = dt;
            GridChqPayOrder.DataBind();
            foreach (DataRow gvr in dt.Rows)
            {
                TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
            }
            GridChqPayOrder.FooterRow.Cells[0].Visible = false;
            GridChqPayOrder.FooterRow.Cells[8].Visible = false;
            GridChqPayOrder.FooterRow.Cells[9].Visible = false;
            GridChqPayOrder.FooterRow.Cells[7].Text = string.Format("{0:n2}", TotalAmount);
            GridChqPayOrder.FooterRow.Cells[6].Text = "Total";
            GridChqPayOrder.FooterRow.Cells[6].Font.Bold = true;
            GridChqPayOrder.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            GridChqPayOrder.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
            GridChqPayOrder.FooterRow.Cells[7].Font.Bold = true;
            GridChqPayOrder.HeaderRow.Cells[4].Text = "Cheque #";
            GridChqPayOrder.HeaderRow.Cells[5].Text = "Cheque Date";
            GridChqPayOrder.HeaderRow.Cells[7].Text = "Cheque Amount";
            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            spanlblChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            PanelOpenChqPayOrder.Visible = true;
            //drpCheque.SelectedIndex = 0;
            //Session.Add("OpenChequesInHand", dt);
            //OpenChequesDetail(int.Parse(drpCheque.SelectedItem.Value.ToString()));
        }
    }
    public void PayOrderGrid()
    {
        drpChequeNo.Items.Clear();
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(drpBanks.SelectedItem.Value.ToString()), 2);
        //clsWebFormUtil.FillDxComboBoxList(this.drpCheque, dt, 0, 1, true);
        if (dt.Rows.Count > 0)
        {
            decimal TotalAmount = 0;
            //drpCheque.SelectedIndex = 0;
            GridChqPayOrder.DataSource = dt;
            GridChqPayOrder.DataBind();
            foreach (DataRow gvr in dt.Rows)
            {
                TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
            }
            GridChqPayOrder.FooterRow.Cells[0].Visible = false;
            GridChqPayOrder.FooterRow.Cells[8].Visible = false;
            GridChqPayOrder.FooterRow.Cells[9].Visible = false;
            GridChqPayOrder.FooterRow.Cells[7].Text = string.Format("{0:n2}", TotalAmount);
            GridChqPayOrder.FooterRow.Cells[6].Text = "Total";
            GridChqPayOrder.FooterRow.Cells[6].Font.Bold = true;
            GridChqPayOrder.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            GridChqPayOrder.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
            GridChqPayOrder.FooterRow.Cells[7].Font.Bold = true;
            GridChqPayOrder.HeaderRow.Cells[4].Text = "PO #";
            GridChqPayOrder.HeaderRow.Cells[5].Text = "PO Date";
            GridChqPayOrder.HeaderRow.Cells[7].Text = "PO Amount";
            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            spanlblChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            PanelOpenChqPayOrder.Visible = true;
            //Session.Add("PayOrdersInHand", dt);
            //PayOrdersDetail(int.Parse(drpCheque.SelectedItem.Value.ToString()));

        }
    }
    public void LoadCheque(int TYPE)
    {
        try
        {
            if (Session["Val"] != null)
            {
                int VAL = Convert.ToInt32(Session["Val"]);
                if (DrpPaymentMode.SelectedItem.Value.ToString() == "18") //VAL == 84 && 
                {
                    if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                    {
                        txtDebitAmount.Enabled = false;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = true;
                        txtCreditAmount.Text = "";
                    }
                    else
                    {
                        OpenChequeGrid();
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                    }
                    Session["Val"] = null;
                }
                else if (DrpPaymentMode.SelectedItem.Value.ToString() == "44") //VAL == 85 && 
                {
                    if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                    {
                        txtDebitAmount.Enabled = false;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = true;
                        txtCreditAmount.Text = "";
                    }
                    else
                    {
                        PayOrderGrid();
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                    }
                    Session["Val"] = null;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(VAL), null, null, null, true, Constants.DateNullValue, TYPE);
                    clsWebFormUtil.FillDxComboBoxList(this.drpChequeNo, dt, 0, 1, true);

                    if (dt.Rows.Count > 0)
                    {
                        drpChequeNo.SelectedIndex = 0;
                        Session["Val"] = null;
                    }
                    txtDebitAmount.Enabled = true;
                    txtDebitAmount.Text = "";
                    txtCreditAmount.Enabled = false;
                    txtCreditAmount.Text = "";
                }

            }
            else
            {
                // Open Cheques In Hand
                if (DrpPaymentMode.SelectedItem.Value.ToString() == "18") //drpBanks.SelectedItem.Value.ToString() == "84" && 
                {
                    if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                    {
                        txtDebitAmount.Enabled = false;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = true;
                        txtCreditAmount.Text = "";
                    }
                    else
                    {
                        drpChequeNo.Items.Clear();
                        OpenChequeGrid();
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                    }
                }
                else if (DrpPaymentMode.SelectedItem.Value.ToString() == "44") //drpBanks.SelectedItem.Value.ToString() == "85" && 
                {
                    // Pay Orders In Hand
                    if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                    {
                        txtDebitAmount.Enabled = false;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = true;
                        txtCreditAmount.Text = "";
                    }
                    else
                    {
                        drpChequeNo.Items.Clear();
                        PayOrderGrid();
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                    }
                }
                else
                {
                    if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "46")
                    {
                        drpChequeNo.Items.Clear();
                        DataTable dt = new DataTable();
                        dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(drpBanks.SelectedItem.Value.ToString()), null, null, null, true, Constants.DateNullValue, 2);
                        clsWebFormUtil.FillDxComboBoxList(this.drpChequeNo, dt, 0, 1, true);
                        if (dt.Rows.Count > 0)
                        {
                            drpChequeNo.SelectedIndex = 0;
                        }
                        spanlblChequeNo.Visible = true;
                        lblChequeNo.Visible = true;
                        lblChequeNo.Text = "Cheque No";
                        drpChequeNo.Visible = true;
                        txtChequeNo.Visible = false;
                        PanelOpenChqPayOrder.Visible = false;
                        GridChqPayOrder.DataSource = null;
                        GridChqPayOrder.DataBind();
                        //txtDueDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                    }
                    else
                    {
                        if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                        {
                            if (DrpPaymentMode.SelectedItem.Value.ToString() == "30")
                            {
                                lblChequeNo.Text = "Slip No";
                            }
                            else
                            {
                                lblChequeNo.Text = "Cheque No";
                            }
                            spanlblChequeNo.Visible = true;
                            lblChequeNo.Visible = true;
                            drpChequeNo.Visible = false;
                            txtDebitAmount.Enabled = false;
                            txtDebitAmount.Text = "";
                            txtCreditAmount.Enabled = true;
                            txtCreditAmount.Text = "";
                        }
                        else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "24")
                        {
                            spanlblChequeNo.Visible = false;
                            lblChequeNo.Visible = false;
                            drpChequeNo.Visible = false;
                            txtDebitAmount.Enabled = true;
                            txtDebitAmount.Text = "";
                            txtCreditAmount.Enabled = false;
                            txtCreditAmount.Text = "";
                        }
                        else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "17")
                        {
                            if (DrpPaymentMode.SelectedItem.Value.ToString() == "30" || DrpPaymentMode.SelectedItem.Value.ToString() == "34")
                            {
                                spanlblChequeNo.Visible = true;
                                lblChequeNo.Visible = true;
                                lblChequeNo.Text = "Slip No";
                                txtChequeNo.Visible = true;
                                drpChequeNo.Visible = false;
                                txtDebitAmount.Enabled = true;
                                txtDebitAmount.Text = "";
                                txtCreditAmount.Enabled = false;
                                txtCreditAmount.Text = "";
                                PanelOpenChqPayOrder.Visible = false;
                            }
                            else
                            {
                                spanlblChequeNo.Visible = false;
                                lblChequeNo.Visible = false;
                                drpChequeNo.Visible = false;
                                txtDebitAmount.Enabled = false;
                                txtDebitAmount.Text = "";
                                txtCreditAmount.Enabled = true;
                                txtCreditAmount.Text = "";
                                PanelOpenChqPayOrder.Visible = false;
                            }
                        }
                        else
                        {
                            spanlblChequeNo.Visible = false;
                            lblChequeNo.Visible = false;
                            drpChequeNo.Visible = false;
                            txtDebitAmount.Enabled = false;
                            txtDebitAmount.Text = "";
                            txtCreditAmount.Enabled = true;
                            txtCreditAmount.Text = "";
                            PanelOpenChqPayOrder.Visible = false;
                        }
                    }

                    //if (DrpVoucherSubType.SelectedItem.Value.ToString() != "15")
                    //{
                    //    drpChequeNo.Items.Clear();
                    //    DataTable dt = new DataTable();
                    //    dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(drpBanks.SelectedItem.Value.ToString()), null, null, null, true, Constants.DateNullValue, TYPE);
                    //    clsWebFormUtil.FillDxComboBoxList(this.drpChequeNo, dt, 0, 1, true);
                    //    if (dt.Rows.Count > 0)
                    //    {
                    //        drpChequeNo.SelectedIndex = 0;
                    //    }
                    //    spanlblChequeNo.Visible = true;
                    //    lblChequeNo.Visible = true;
                    //    drpChequeNo.Visible = true;
                    //    PanelOpenChqPayOrder.Visible = false;
                    //    GridChqPayOrder.DataSource = null;
                    //    GridChqPayOrder.DataBind();
                    //    //txtDueDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    //    txtDebitAmount.Enabled = true;
                    //    txtDebitAmount.Text = "";
                    //}
                    //else
                    //{
                    //    if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                    //    {
                    //        txtDebitAmount.Enabled = false;
                    //        txtDebitAmount.Text = "";
                    //        txtCreditAmount.Enabled = true;
                    //        txtCreditAmount.Text = "";
                    //    }
                    //    else
                    //    {
                    //        spanlblChequeNo.Visible = true;
                    //        lblChequeNo.Visible = true;
                    //        drpChequeNo.Visible = false;
                    //        txtDebitAmount.Enabled = true;
                    //        txtDebitAmount.Text = "";
                    //        txtCreditAmount.Enabled = false;
                    //        txtCreditAmount.Text = "";
                    //    }
                    //}


                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
    }
    private void OpenChequesDetail(int p_Cheque_No)
    {
        DataTable dt = (DataTable)this.Session["OpenChequesInHand"];
        DataRow[] foundRows = dt.Select("CHEQUE_PROCESS_ID  = '" + p_Cheque_No + "'");
        if (foundRows.Length > 0)
        {
            //DateTime ChqDate = DateTime.Parse(foundRows[0]["CHEQUE_DATE"].ToString()); //ToString("dd-MM-yyyy");
            //txtDueDate.Text = ChqDate.ToString("dd-MMM-yyyy");
            decimal amount = decimal.Parse(foundRows[0]["CHEQUE_AMOUNT"].ToString());
            txtDebitAmount.Text = Math.Round(amount, 0).ToString();
        }
    }
    private void PayOrdersDetail(int p_Cheque_No)
    {
        DataTable dt = (DataTable)this.Session["PayOrdersInHand"];
        DataRow[] foundRows = dt.Select("CHEQUE_PROCESS_ID  = '" + p_Cheque_No + "'");
        if (foundRows.Length > 0)
        {
            //DateTime ChqDate = DateTime.Parse(foundRows[0]["CHEQUE_DATE"].ToString()); //ToString("dd-MM-yyyy");
            //txtDueDate.Text = ChqDate.ToString("dd-MMM-yyyy");
            decimal amount = decimal.Parse(foundRows[0]["CHEQUE_AMOUNT"].ToString());
            txtDebitAmount.Text = Math.Round(amount, 0).ToString();
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
    private void LoadAccountDetail()
    {
        int TypeID;
        if (int.Parse(DrpVoucherType.SelectedItem.Value.ToString()) == 25)
        {
            TypeID = 4;
        }
        else
        {
            TypeID = 1;
        }
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, TypeID);
        clsWebFormUtil.FillDxComboBoxList(this.ddlAccountHead, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_DETAIL", true);

        if (dtHead.Rows.Count > 0)
        {
            ddlAccountHead.SelectedIndex = 0;
        }
        this.Session.Add("dtHead", dtHead);
    }


    private void ClearAll()
    {
        txtTotalCredit.Text = "0";
        txtTotalDebit.Text = "0";
        txtRemarks.Text = "";
        txtDebitAmount.Text = "";
        txtCreditAmount.Text = "";
        txtAccountDes.Text = "";
        GrdOrder.DataSource = null;
        GrdOrder.DataBind();
        txtChequeNo.Text = "";
        txtBankName.Text = "";
        txtFavouring.Text = "";
        Session.Remove("dtVoucher");
        CreatTable();
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
            DrpVoucherSubType_SelectedIndexChanged(null, null);
        }
        if (DrpVoucherType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
            LoadAccountDetail();
        }
        else
        {
            this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
            LoadAccountDetail();
        }
        //this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));

        //txtDebitAmount.Enabled = true;
        //txtCreditAmount.Enabled = true;

        DrpVoucherType.Focus();

        if (DrpVoucherType.SelectedItem.Value.ToString() == "812")
        {
            divImport.Visible = true;
        }
        else
        {
            divImport.Visible = false;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        decimal TotalDebit = 0;
        decimal TotalCredit = 0;

        DataControl dc = new DataControl();
        DataTable dtHead = (DataTable)this.Session["dtHead"];
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        if (Convert.ToInt32(RowId.Value) >= 0)
        {
            DataRow dr = dtVoucher.Rows[Convert.ToInt32(RowId.Value)];
            dr["ACCOUNT_HEAD_ID"] = ddlAccountHead.SelectedItem.Value.ToString();
            dr["ACCOUNT_CODE"] = "";
            dr["ACCOUNT_NAME"] = ddlAccountHead.SelectedItem.Text;
            dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtDebitAmount.Text));
            dr["CREDIT"] = decimal.Parse(dc.chkNull_0(txtCreditAmount.Text));
            dr["Principal"] = "0";
            if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
            {
                dr["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
            }
            else
            {
                dr["Principal_id"] = "0";
            }
            dr["REMARKS"] = txtAccountDes.Text;

        }
        else
        {
            DataRow dr = dtVoucher.NewRow();
            dr["Ledger_ID"] = Constants.LongNullValue.ToString();
            dr["ACCOUNT_HEAD_ID"] = ddlAccountHead.SelectedItem.Value.ToString();
            dr["ACCOUNT_CODE"] = "";
            dr["ACCOUNT_NAME"] = ddlAccountHead.SelectedItem.Text;
            dr["DEBIT"] = decimal.Parse(dc.chkNull_0(txtDebitAmount.Text));
            dr["CREDIT"] = decimal.Parse(dc.chkNull_0(txtCreditAmount.Text));
            dr["REMARKS"] = txtAccountDes.Text;
            dr["Principal"] = "0";
            if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
            {
                dr["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
            }
            else
            {
                dr["Principal_id"] = "0";
            }
            dtVoucher.Rows.Add(dr);
        }

        #region Set Total

        foreach (DataRow dr in dtVoucher.Rows)
        {
            TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
            TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
        }
        txtTotalDebit.Text = decimal.Round(TotalDebit, 2).ToString();
        txtTotalCredit.Text = decimal.Round(TotalCredit, 2).ToString();

        #endregion

        #region Clear Txtbox

        txtDebitAmount.Text = "";
        txtCreditAmount.Text = "";
        //txtAccountDes.Text = "";
        RowId.Value = "-1";

        #endregion

        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        if (GrdOrder.Rows.Count > 0)
        {
            DrpVoucherSubType.Enabled = false;
            DrpVoucherType.Enabled = false;
            drpDistributor.Enabled = false;
        }
        this.Session.Add("dtVoucher", dtVoucher);
        ScriptManager.GetCurrent(Page).SetFocus(ddlAccountHead);
    }

    protected void GrdOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        decimal TotalDebit = 0;
        decimal TotalCredit = 0;
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        dtVoucher.Rows.RemoveAt(e.RowIndex);

        #region Set Total

        //foreach (DataRow dr in dtVoucher.Rows)
        //{
        //    TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
        //    TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
        //}
        txtTotalDebit.Text = TotalDebit.ToString();
        txtTotalCredit.Text = TotalCredit.ToString();

        #endregion

        GrdOrder.DataSource = dtVoucher;
        GrdOrder.DataBind();
        RowId.Value = "-1";
    }

    protected void GrdOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowId.Value = e.NewEditIndex.ToString();
        ddlAccountHead.Value = GrdOrder.Rows[e.NewEditIndex].Cells[0].Text;
        txtAccountDes.Text = GrdOrder.Rows[e.NewEditIndex].Cells[3].Text;
        txtDebitAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[4].Text;
        txtCreditAmount.Text = GrdOrder.Rows[e.NewEditIndex].Cells[5].Text;
    }
    public bool CheckOpenChqPay()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GridChqPayOrder.Rows)
        {
            Session["CHEQUE_PROCESS_ID"] = null;
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                Session["CHEQUE_PROCESS_ID"] = dr.Cells[1].Text;
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
    private string GetChqPayOrder()
    {

        string ChqNo = "";
        foreach (GridViewRow dr in GridChqPayOrder.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                ChqNo += "," + dr.Cells[4].Text;
            }
        }
        if (ChqNo != "")
        {
            ChqNo = ChqNo.Remove(0, 1);
        }
        else
        {
            ChqNo = "";
        }
        return ChqNo;
    }
    private string ChequeNO()
    {
        string ChequeNo;
        if (DrpPaymentMode.SelectedIndex == 0)
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
            {
                ChequeNo = txtChequeNo.Text;
            }
            else
            {
                if (DrpPaymentMode.SelectedItem.Value.ToString() == "18") //drpBanks.SelectedItem.Value.ToString() == "84" && 
                {
                    ChequeNo = GetChqPayOrder();
                }
                else
                {
                    ChequeNo = drpChequeNo.SelectedItem.Text;
                }
            }

        }
        else if (DrpPaymentMode.SelectedIndex == 1 || DrpPaymentMode.SelectedIndex == 2)
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
            {
                ChequeNo = txtChequeNo.Text;
            }
            else
            {
                if (DrpPaymentMode.SelectedItem.Value.ToString() == "44") //drpBanks.SelectedItem.Value.ToString() == "85" && 
                {
                    ChequeNo = GetChqPayOrder();
                }
                else
                {
                    ChequeNo = drpChequeNo.SelectedItem.Text;
                }
            }
        }
        else
        {
            ChequeNo = txtChequeNo.Text;
        }
        return ChequeNo;
    }
    protected void btnDone_Click(object sender, EventArgs e)
    {
        dtVoucher = (DataTable)this.Session["dtVoucher"];
        if (dtVoucher.Rows.Count > 0)
        {
            //if (IsDayClosed())
            //{
            //    UserController UserCtl = new UserController();

            //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            //    Session.Clear();
            //    System.Web.Security.FormsAuthentication.SignOut();
            //    Response.Redirect("../Login.aspx");
            //}

            LedgerController mLController = new LedgerController();
            DataControl dc = new DataControl();
            dtVoucher = (DataTable)this.Session["dtVoucher"];
            DataTable dtHead = (DataTable)this.Session["dtHead"];

            decimal TotalDebit = 0;
            decimal TotalCredit = 0;
            decimal DiffAmount = 0;
            decimal ChqAmount = 0;
            #region Set Total

            foreach (DataRow dr in dtVoucher.Rows)
            {
                TotalDebit += decimal.Parse(dr["DEBIT"].ToString());
                TotalCredit += decimal.Parse(dr["CREDIT"].ToString());
            }
            DiffAmount = TotalDebit - TotalCredit;
            if (DiffAmount > 0)
            {
                ChqAmount = DiffAmount;
            }
            else
            {
                ChqAmount = -(DiffAmount);
            }
            #endregion

            #region In Case of Bank & Cash Voucher

            if (DrpVoucherType.Value.ToString() == "14" || DrpVoucherType.Value.ToString() == "15" || DrpVoucherType.Value.ToString() == "25")
            {
                decimal differnceAmt = TotalDebit - TotalCredit;

                if (differnceAmt > 0)
                {
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
                    dr1["ACCOUNT_HEAD_ID"] = drpBanks.SelectedItem.Value.ToString();
                    dr1["ACCOUNT_CODE"] = "";
                    dr1["ACCOUNT_NAME"] = drpBanks.SelectedItem.Text;
                    dr1["REMARKS"] = txtAccountDes.Text;
                    dr1["CREDIT"] = differnceAmt;
                    dr1["DEBIT"] = 0;
                    dr1["Principal"] = "";
                    if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
                    {
                        dr1["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
                    }
                    else
                    {
                        dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    }
                    //dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    dtVoucher.Rows.Add(dr1);

                }
                else if (differnceAmt < 0)
                {
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["Ledger_ID"] = Constants.LongNullValue.ToString();
                    dr1["ACCOUNT_HEAD_ID"] = drpBanks.SelectedItem.Value.ToString();
                    dr1["ACCOUNT_CODE"] = "";
                    dr1["ACCOUNT_NAME"] = drpBanks.SelectedItem.Text;
                    dr1["REMARKS"] = txtAccountDes.Text;
                    dr1["CREDIT"] = 0;
                    dr1["DEBIT"] = -(differnceAmt);
                    dr1["Principal"] = "";
                    if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
                    {
                        dr1["Principal_id"] = DrpVoucherSubType.SelectedItem.Value.ToString();
                    }
                    else
                    {
                        dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    }
                    //dr1["Principal_id"] = dtVoucher.Rows[0]["Principal_id"].ToString();
                    dtVoucher.Rows.Add(dr1);
                }
            }

            #endregion

            int VoucherType = Constants.IntNullValue;
            if (DrpVoucherType.Items.Count > 0)
            {
                if (DrpVoucherType.SelectedItem.Value.ToString() == "16" || DrpVoucherType.SelectedItem.Value.ToString() == "812" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
                {
                    VoucherType = Convert.ToInt32(DrpVoucherType.SelectedItem.Value.ToString());
                }
                else
                {
                    VoucherType = Convert.ToInt32(DrpVoucherSubType.SelectedItem.Value.ToString());
                }
            }
            DateTime ChequeDate;
            DateTime Voucherdate;
            DateTime DueDate;
            DateTime TransactionDate;
            ChequeDate = Constants.DateNullValue;
            //if (txtDueDate.Text.Length > 0 && txtDueDate.Visible)
            //{
            //    DueDate = DateTime.Parse(txtDueDate.Text + " 00:00:00");
            //}
            //else
            //{
            DueDate = Constants.DateNullValue;
            //}
            if (txtTransactionDate.Text.Length > 0 && txtTransactionDate.Visible)
            {
                TransactionDate = DateTime.Parse(txtTransactionDate.Text + " 00:00:00");
            }
            else
            {
                TransactionDate = Constants.DateNullValue;
            }
            //if (drpBanks.SelectedItem.Value.ToString() == "84" && DrpPaymentMode.SelectedItem.Value.ToString() == "18" ||
            //        drpBanks.SelectedItem.Value.ToString() == "85" && DrpPaymentMode.SelectedItem.Value.ToString() == "44")
            if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "44")
            {
                if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                {

                }
                else
                {
                    if (CheckOpenChqPay())
                    {
                        string type = "";
                        if (DrpPaymentMode.SelectedItem.Value.ToString() == "44")
                        {
                            type = "a Pay Order.";
                        }
                        if (DrpPaymentMode.SelectedItem.Value.ToString() == "46")
                        {
                            type = "an Own Cheque.";
                        }
                        else
                        {
                            type = "an Open Cheque.";
                        }
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select " + type + "');", true);
                        return;
                    }
                }
            }
            string ChequeNo = null;
            if (DrpVoucherType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25")
            {
                if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                {
                    ChequeNo = txtChequeNo.Text;

                }
                else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "17")
                {
                    ChequeNo = ChequeNO();
                    //ChequeNo = drpChequeNo.SelectedItem.Text;
                }
            }

            Voucherdate = TransactionDate;
            int subtype_id = 0;
            string MaxDocumentId = mLController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Voucherdate);
            if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
            {
                subtype_id = int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString());
            }
            if (TotalDebit == TotalCredit || DrpVoucherType.SelectedItem.Value.ToString() != "16")
            {
                try
                {
                    bool IsSaveVoucher = mLController.Edit_Voucher(int.Parse(drpDistributor.SelectedItem.Value.ToString()), subtype_id, MaxDocumentId, VoucherType,
                       TransactionDate, int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()), txtpayeesName.Text, txtRemarks.Text, TransactionDate, ChequeNo, dtVoucher, int.Parse(this.Session["UserId"].ToString())
                       , "", DueDate, txtBankName.Text, txtFavouring.Text);
                    if (IsSaveVoucher == true)
                    {
                        // Add Open Cheque & Pay Order in Cheque Process table
                        if (DrpVoucherType.SelectedItem.Value.ToString() == "15" && DrpVoucherSubType.SelectedItem.Value.ToString() == "15" || DrpVoucherType.SelectedItem.Value.ToString() == "25" && DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                        {
                            if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "44" || DrpPaymentMode.SelectedItem.Value.ToString() == "34")
                            {
                                int ISSUED = 0;
                                if (DrpPaymentMode.SelectedItem.Value.ToString() == "34")
                                {
                                    ISSUED = 1;
                                }
                                string Favouring = txtFavouring.Text;
                                int ChequeType = int.Parse(DrpPaymentMode.SelectedItem.Value.ToString());
                                DateTime Date = Constants.DateNullValue;
                                DateTime ChqDate = Convert.ToDateTime(txtTransactionDate.Text);
                                string chqdate = ChqDate.ToString("dd/MM/yyyy");
                                //string TransDate = txtTransactionDate.Text.Replace("-", "/");
                                Date = DateTime.Parse(ConvertDate.British_To_American(chqdate));
                                string ChqProccessID = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, 0, ChequeNo, txtBankName.Text, Date,
                                    Date, Constants.DateNullValue, Date, ChqAmount, 529, DateTime.Now, int.Parse(Session["UserId"].ToString())
                                    , "1", txtRemarks.Text, long.Parse(drpBanks.SelectedItem.Value.ToString()), 1, 0, 1, 0, 1, Favouring, ChequeType, ISSUED, "");
                            }
                        }
                        if (Session["CHEQUE_PROCESS_ID"] != null)
                        {
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("UPDATE CHEQUE_PROCESS set ISSUED = 1,  ISSUED_ACCOUNT_HEAD_ID = " + Convert.ToInt64(ddlAccountHead.SelectedItem.Value) + "  WHERE CHEQUE_PROCESS_ID=" + Convert.ToInt64(Session["CHEQUE_PROCESS_ID"]), con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " has been saved');", true);
                        ClearAll();
                        PrintVoucher(MaxDocumentId);
                        LoadCheque(2);
                        //Response.Redirect("Forms/frmVoucherEntry.aspx");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some error occur wrong entry');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Some of entries are wrong');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Debit must equal to credit balance');", true);
            }
            DrpVoucherType_SelectedIndexChanged(null, null);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('No record found....!');", true);
        }
    }

    protected void GrdOrder_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[3].Text = "Total";
            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
            e.Row.Cells[4].Text = Convert.ToDecimal(txtTotalDebit.Text).ToString("N");
            e.Row.Cells[5].Text = Convert.ToDecimal(txtTotalCredit.Text).ToString("N");
            e.Row.Cells[3].Font.Bold = true;
            e.Row.Cells[4].Font.Bold = true;
            e.Row.Cells[5].Font.Bold = true;
            e.Row.Cells[8].Visible = false;
        }
    }

    private void HideShowControls()
    {
        if (DrpVoucherType.SelectedItem.Value.ToString() == "14")
        {
            spanlblSlipNo.Visible = false;
            spanlblChequeNo.Visible = false;
            spanlblFavouring.Visible = false;

            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            lblSlipNo.Visible = false;
            txtBankName.Visible = false;
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            //ibnEndDate.Visible = false;

            spanlblPaymentMode.Visible = true;
            spanlblBankHead.Visible = true;

            lblPaymentMode.Visible = true;
            DrpPaymentMode.Visible = true;
            lblBankHead.Visible = true;
            drpBanks.Visible = true;

            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Receipt Voucher", "14"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Payment Voucher", "24"));
            SubType.Visible = true;
            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
            txtRemarks.Visible = true;
            lblRemarks.Visible = true;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "16")
        {
            spanlblSlipNo.Visible = false;
            spanlblPaymentMode.Visible = false;
            spanlblBankHead.Visible = false;
            spanlblChequeNo.Visible = false;
            spanlblFavouring.Visible = false;

            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            lblSlipNo.Visible = false;
            txtBankName.Visible = false;
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            //ibnEndDate.Visible = false;

            lblPaymentMode.Visible = false;
            DrpPaymentMode.Visible = false;
            lblBankHead.Visible = false;
            drpBanks.Visible = false;


            SubType.Visible = false;

            txtDebitAmount.Enabled = true;
            txtCreditAmount.Enabled = true;

            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
            txtRemarks.Visible = true;
            lblRemarks.Visible = true;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "812")
        {
            spanlblSlipNo.Visible = false;
            spanlblPaymentMode.Visible = false;
            spanlblBankHead.Visible = false;
            spanlblChequeNo.Visible = false;
            spanlblFavouring.Visible = false;

            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            lblSlipNo.Visible = false;
            txtBankName.Visible = false;
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            //ibnEndDate.Visible = false;
            txtpayeesName.Visible = false;
            lblpayeesName.Visible = false;
            txtRemarks.Visible = false;
            lblRemarks.Visible = false;

            lblPaymentMode.Visible = false;
            DrpPaymentMode.Visible = false;
            lblBankHead.Visible = false;
            drpBanks.Visible = false;

            SubType.Visible = false;

            txtDebitAmount.Enabled = true;
            txtCreditAmount.Enabled = true;
        }
        else if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
        {
            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Receipt Voucher", "14"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Cash Payment Voucher", "24"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Receipt Voucher", "15"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Payment Voucher", "17"));
            spanlblSlipNo.Visible = false;
            spanlblChequeNo.Visible = false;
            spanlblFavouring.Visible = false;
            lblChequeNo.Visible = false;
            drpChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            lblSlipNo.Visible = false;
            txtBankName.Visible = false;
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            spanlblPaymentMode.Visible = true;
            spanlblBankHead.Visible = true;
            lblPaymentMode.Visible = true;
            DrpPaymentMode.Visible = true;
            lblBankHead.Visible = true;
            drpBanks.Visible = true;

            SubType.Visible = true;
            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;
            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
            txtRemarks.Visible = true;
            lblRemarks.Visible = true;
        }
        else
        {
            DrpVoucherSubType.Items.Clear();
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Receipt Voucher", "15"));
            DrpVoucherSubType.Items.Add(new DevExpress.Web.ListEditItem("Bank Payment Voucher", "17"));
            spanlblSlipNo.Visible = true;
            spanlblPaymentMode.Visible = true;
            spanlblBankHead.Visible = true;
            spanlblChequeNo.Visible = true;
            spanlblFavouring.Visible = true;

            lblChequeNo.Visible = true;
            drpChequeNo.Visible = false;
            txtChequeNo.Visible = true;

            lblSlipNo.Visible = true;
            txtBankName.Visible = true;
            lblFavouring.Visible = true;
            txtFavouring.Visible = true;
            //ibnEndDate.Visible = true;
            lblPaymentMode.Visible = true;
            DrpPaymentMode.Visible = true;
            lblBankHead.Visible = true;
            drpBanks.Visible = true;

            txtpayeesName.Visible = true;
            lblpayeesName.Visible = true;
            txtRemarks.Visible = true;
            lblRemarks.Visible = true;



            SubType.Visible = true;

            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;
        }

        if (DrpVoucherSubType.Items.Count > 0)
        {
            DrpVoucherSubType.SelectedIndex = 0;
        }
    }

    protected void DrpVoucherSubType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //drpChequeNo.Enabled = false;
        DrpPaymentMode.Items.Clear();
        //if (DrpVoucherSubType.SelectedIndex == 0)
        if (int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString()) == 14 || int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString()) == 15)
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "14")
            {
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Cash", "19"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "30"));
                if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
                {
                    lblSlipNo.Visible = false;
                    txtBankName.Visible = false;
                    lblFavouring.Visible = false;
                    txtFavouring.Visible = false;
                    txtChequeNo.Visible = false;
                }
            }
            else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
            {
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "34"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "30"));
                if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                {
                    if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
                    {
                        lblSlipNo.Visible = true;
                        txtBankName.Visible = true;
                        lblFavouring.Visible = true;
                        txtFavouring.Visible = true;
                    }
                    spanlblChequeNo.Visible = true;
                    lblChequeNo.Text = "Open Cheque No";
                    drpChequeNo.Visible = false;
                    txtChequeNo.Visible = true;
                    GridChqPayOrder.DataSource = null;
                    GridChqPayOrder.DataBind();
                    PanelOpenChqPayOrder.Visible = false;
                    txtDebitAmount.Text = "";
                }
                else
                {
                    drpChequeNo.Visible = true;
                    txtChequeNo.Visible = false;

                    lblSlipNo.Visible = false;
                    txtBankName.Visible = false;
                    lblFavouring.Visible = false;
                    txtFavouring.Visible = false;
                }
            }
            txtDebitAmount.Enabled = false;
            txtCreditAmount.Enabled = true;
        }
        //else if (DrpVoucherSubType.SelectedIndex == 1)
        else if (int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString()) == 24 || int.Parse(DrpVoucherSubType.SelectedItem.Value.ToString()) == 17)
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "24")
            {
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Cash", "19"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "30"));
                if (DrpVoucherType.SelectedItem.Value.ToString() == "25")
                {
                    lblSlipNo.Visible = false;
                    txtBankName.Visible = false;
                    lblFavouring.Visible = false;
                    txtFavouring.Visible = false;
                    txtChequeNo.Visible = false;
                }
            }
            else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "17")
            {
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "30"));
                DrpPaymentMode.Items.Add(new DevExpress.Web.ListEditItem("Other Bank Transfer", "34"));
                if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                {
                    drpChequeNo.Visible = false;
                    txtChequeNo.Visible = true;
                    //PanelOpenChqPayOrder.Visible = false;
                }
                else
                {
                    if (DrpPaymentMode.Items.Count > 0)
                    {
                        DrpPaymentMode.SelectedIndex = 0;
                    }
                    this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
                    spanlblChequeNo.Visible = false;
                    lblChequeNo.Visible = false;
                    drpChequeNo.Visible = false;
                    txtChequeNo.Visible = false;
                    PanelOpenChqPayOrder.Visible = true;
                    //LoadCheque(2);

                    lblSlipNo.Visible = false;
                    txtBankName.Visible = false;
                    lblFavouring.Visible = false;
                    txtFavouring.Visible = false;
                }
                if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "30")
                {
                    lblChequeNo.Text = "Cheque No";
                }
                else if (DrpPaymentMode.SelectedItem.Value.ToString() == "44")
                {
                    lblChequeNo.Text = "Pay Order No";
                }
            }

            txtDebitAmount.Enabled = true;
            txtCreditAmount.Enabled = false;
            //drpChequeNo.Enabled = true;
        }
        DrpVoucherSubType.Focus();
        if (DrpPaymentMode.Items.Count > 0)
        {
            DrpPaymentMode.SelectedIndex = 0;
        }
        DrpPaymentMode_SelectedIndexChanged(null, null);
    }

    private void toggleControls(string pAccountType)
    {
        if (pAccountType == "21")
        {

            lblChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            drpChequeNo.Visible = false;


        }
        else if (pAccountType == "33")
        {
            lblChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            drpChequeNo.Visible = false;
        }
        else
        {

            if (pAccountType == "18")
            {
                lblChequeNo.Text = "Cheque No";
                txtChequeNo.Visible = false;
                drpChequeNo.Visible = true;
                LoadCheque(2);
            }
            else if (pAccountType == "44")
            {
                lblChequeNo.Text = "Pay Order No";
                txtChequeNo.Visible = false;
                drpChequeNo.Visible = true;
                LoadCheque(2);
            }

        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.ClearAll();
        LoadCheque(2);
    }

    //protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    LoadAccountDetail();
    //    LoadAccountHead();
    //}

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

    #region Import Vouchers
    protected void btnImport_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        if (FileUpload1.HasFile)
        {
            string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
            string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
            string FolderPath = @"../Docs/";
            if (Extension != ".xls" && Extension != ".xlsx")
            {
                lblError.Text = "Only Excel files are allowed.";
                return;
            }
            string FilePath = Server.MapPath(FolderPath + FileName);
            FileUpload1.SaveAs(FilePath);
            Import_To_Grid(FilePath, Extension, "Yes");
        }
    }
    private void Import_To_Grid(string FilePath, string Extension, string isHDR)
    {
        try
        {
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"]
                             .ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    conStr = System.Configuration.ConfigurationManager.ConnectionStrings["Excel07ConString"]
                              .ConnectionString;
                    break;
            }
            conStr = String.Format(conStr, FilePath, isHDR);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            cmdExcel.Connection = connExcel;

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();

            //Read Data from First Sheet
            connExcel.Open();
            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);
            connExcel.Close();

            if (dt.Rows.Count > 0)
            {
                lblError.Text = BindWithGrid(dt);
            }
            else
            {
                lblError.Text = "No record found to import";
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
    public string BindWithGrid(DataTable dtExcel)
    {
        string errorMessage = "";
        try
        {

            decimal TotalDebit = 0;
            decimal TotalCredit = 0;

            decimal TotalDebitSheet = 0;
            decimal TotalCreditSheet = 0;

            DataControl dc = new DataControl();
            dtVoucher = (DataTable)this.Session["dtVoucher"];

            #region Validations
            int RowsCount = dtExcel.Rows.Count;

            for (int i = 0; i < RowsCount; i++)
            {
                decimal DEBIT = Convert.ToDecimal(dc.chkNull_0(dtExcel.Rows[i]["DEBIT"].ToString()));
                decimal CREDIT = Convert.ToDecimal(dc.chkNull_0(dtExcel.Rows[i]["CREDIT"].ToString()));
                if ((dtExcel.Rows[i]["DEBIT"].ToString().Length == 0) && (dtExcel.Rows[i]["CREDIT"].ToString().Length == 0))
                {
                    errorMessage = "At record number '" + (i + 1) + "' Debit or Credit must enter";
                    return errorMessage;
                }
                if ((DEBIT > 0) && (CREDIT > 0))
                {
                    errorMessage = "At record number '" + (i + 1) + "' Only Debit or Credit can enter";
                    return errorMessage;
                }
                if (dtExcel.Rows[i][0].ToString().Length == 0)
                {
                    errorMessage = "At record number '" + (i + 1) + "' Account Head must enter";
                    return errorMessage;
                }
                TotalDebitSheet += Convert.ToDecimal(dc.chkNull_0(dtExcel.Rows[i]["DEBIT"].ToString()));
                TotalCreditSheet += Convert.ToDecimal(dc.chkNull_0(dtExcel.Rows[i]["CREDIT"].ToString()));
            }

            if (TotalDebitSheet != TotalCreditSheet)
            {
                errorMessage = "Total Dabit and Credit should be same";
                return errorMessage;
            }
            #endregion

            #region Load Grid
            DataTable dt;
            string COA_ID;
            string COA_NAME;

            foreach (DataRow dtrow in dtExcel.Rows)
            {
                if (dtExcel.Columns[0].ColumnName.ToString().ToUpper() == "COA CODE DESCRIPTION")
                {
                    dt = mAccountController.GetAccountHeadIDbyNameOrCode(Constants.AC_AccountHeadId, null, null, 3);
                    DataRow[] drow = dt.Select("ACCOUNT_DETAIL = '" + dtrow["COA CODE DESCRIPTION"].ToString() + "' ");
                    if (drow == null || drow.Length == 0)
                    {
                        errorMessage = "Account Head '" + dtrow[0].ToString() + "' is invalid ";
                        return errorMessage;
                    }
                    COA_ID = drow[0]["ACCOUNT_HEAD_ID"].ToString();
                    COA_NAME = drow[0]["ACCOUNT_DETAIL"].ToString();
                }
                else
                {
                    if (dtExcel.Columns[0].ColumnName.ToString().ToUpper() == "COA DESCRIPTION")
                    {
                        dt = mAccountController.GetAccountHeadIDbyNameOrCode(Constants.AC_AccountHeadId, null, dtrow["COA DESCRIPTION"].ToString(), 3);
                    }
                    else
                    {
                        dt = mAccountController.GetAccountHeadIDbyNameOrCode(Constants.AC_AccountHeadId, dtrow["COA CODE"].ToString(), null, 3);
                    }
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        errorMessage = "Account Head '" + dtrow[0].ToString() + "' is invalid ";
                        return errorMessage;
                    }
                    COA_ID = dt.Rows[0]["ACCOUNT_HEAD_ID"].ToString();
                    COA_NAME = dt.Rows[0]["ACCOUNT_DETAIL"].ToString();
                }

                DataRow dr = dtVoucher.NewRow();
                dr["Ledger_ID"] = Constants.LongNullValue.ToString();
                dr["ACCOUNT_HEAD_ID"] = COA_ID;
                dr["ACCOUNT_CODE"] = "";
                dr["ACCOUNT_NAME"] = COA_NAME;
                dr["DEBIT"] = decimal.Parse(dc.chkNull_0(dtrow["DEBIT"].ToString()));
                dr["CREDIT"] = decimal.Parse(dc.chkNull_0(dtrow["CREDIT"].ToString()));
                dr["REMARKS"] = dtrow["REMARKS"].ToString();
                dr["Principal"] = "0";
                dr["Principal_id"] = "0";
                dtVoucher.Rows.Add(dr);
            }
            #endregion

            #region Set Total

            foreach (DataRow drow in dtVoucher.Rows)
            {
                TotalDebit += decimal.Parse(drow["DEBIT"].ToString());
                TotalCredit += decimal.Parse(drow["CREDIT"].ToString());
            }
            txtTotalDebit.Text = decimal.Round(TotalDebit, 2).ToString();
            txtTotalCredit.Text = decimal.Round(TotalCredit, 2).ToString();


            #endregion

            #region Binding
            GrdOrder.DataSource = dtVoucher;
            GrdOrder.DataBind();
            if (GrdOrder.Rows.Count > 0)
            {
                DrpVoucherSubType.Enabled = false;
                DrpVoucherType.Enabled = false;
                drpDistributor.Enabled = false;
            }
            #endregion

            this.Session.Add("dtVoucher", dtVoucher);
            ScriptManager.GetCurrent(Page).SetFocus(ddlAccountHead);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        return errorMessage;
    }
    #endregion

    protected void drpChequeNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpPaymentMode.SelectedItem.Value.ToString() == "18") //drpBanks.SelectedItem.Value.ToString() == "84" && 
        {
            OpenChequesDetail(int.Parse(drpChequeNo.SelectedItem.Value.ToString()));
        }
        else if (DrpPaymentMode.SelectedItem.Value.ToString() == "44") //drpBanks.SelectedItem.Value.ToString() == "85" && 
        {
            PayOrdersDetail(int.Parse(drpChequeNo.SelectedItem.Value.ToString()));
        }
        else
        {
            //txtDueDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtDebitAmount.Text = "";
        }
    }

    protected void DrpPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
        {
            this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
        }
        else
        {
            this.LoadAccountHead(int.Parse(DrpPaymentMode.SelectedItem.Value.ToString()));
        }

        if (DrpPaymentMode.SelectedItem.Value.ToString() == "18") //drpBanks.SelectedItem.Value.ToString() == "84" && 
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
            {
                spanlblChequeNo.Visible = true;
                lblChequeNo.Visible = true;
                lblChequeNo.Text = "Cheque No";
                txtDebitAmount.Enabled = false;
                txtDebitAmount.Text = "";
                txtCreditAmount.Enabled = true;
                txtCreditAmount.Text = "";
            }
            else
            {
                OpenChequeGrid();
                lblChequeNo.Visible = false;
                drpChequeNo.Visible = false;
                PanelOpenChqPayOrder.Visible = true;
                txtDebitAmount.Enabled = true;
                txtDebitAmount.Text = "";
                txtCreditAmount.Enabled = false;
                txtCreditAmount.Text = "";
            }
        }
        else if (DrpPaymentMode.SelectedItem.Value.ToString() == "44") //drpBanks.SelectedItem.Value.ToString() == "85" && 
        {
            if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
            {
                spanlblChequeNo.Visible = true;
                lblChequeNo.Visible = true;
                lblChequeNo.Text = "Pay Order No";
                txtDebitAmount.Enabled = false;
                txtDebitAmount.Text = "";
                txtCreditAmount.Enabled = true;
                txtCreditAmount.Text = "";
            }
            else
            {
                PayOrderGrid();
                lblChequeNo.Visible = false;
                drpChequeNo.Visible = false;
                PanelOpenChqPayOrder.Visible = true;
                txtDebitAmount.Enabled = true;
                txtDebitAmount.Text = "";
                txtCreditAmount.Enabled = false;
                txtCreditAmount.Text = "";
            }
        }
        else
        {
            if (DrpPaymentMode.SelectedItem.Value.ToString() == "18" || DrpPaymentMode.SelectedItem.Value.ToString() == "46")
            {
                drpChequeNo.Items.Clear();
                DataTable dt = new DataTable();
                dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(drpBanks.SelectedItem.Value.ToString()), null, null, null, true, Constants.DateNullValue, 2);
                clsWebFormUtil.FillDxComboBoxList(this.drpChequeNo, dt, 0, 1, true);
                if (dt.Rows.Count > 0)
                {
                    drpChequeNo.SelectedIndex = 0;
                }
                spanlblChequeNo.Visible = true;
                lblChequeNo.Visible = true;
                lblChequeNo.Text = "Cheque No";
                drpChequeNo.Visible = true;
                txtChequeNo.Visible = false;
                PanelOpenChqPayOrder.Visible = false;
                GridChqPayOrder.DataSource = null;
                GridChqPayOrder.DataBind();
                //txtDueDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                txtDebitAmount.Enabled = true;
                txtDebitAmount.Text = "";
                txtCreditAmount.Enabled = false;
                txtCreditAmount.Text = "";
            }
            else
            {
                if (DrpVoucherSubType.SelectedItem.Value.ToString() == "15")
                {
                    if (DrpPaymentMode.SelectedItem.Value.ToString() == "30")
                    {
                        lblChequeNo.Text = "Slip No";
                    }
                    else
                    {
                        lblChequeNo.Text = "Cheque No";
                    }
                    spanlblChequeNo.Visible = true;
                    lblChequeNo.Visible = true;
                    drpChequeNo.Visible = false;
                    txtDebitAmount.Enabled = false;
                    txtDebitAmount.Text = "";
                    txtCreditAmount.Enabled = true;
                    txtCreditAmount.Text = "";
                }
                else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "24")
                {
                    spanlblChequeNo.Visible = false;
                    lblChequeNo.Visible = false;
                    drpChequeNo.Visible = false;
                    txtDebitAmount.Enabled = true;
                    txtDebitAmount.Text = "";
                    txtCreditAmount.Enabled = false;
                    txtCreditAmount.Text = "";
                }
                else if (DrpVoucherSubType.SelectedItem.Value.ToString() == "17")
                {
                    if (DrpPaymentMode.SelectedItem.Value.ToString() == "30" || DrpPaymentMode.SelectedItem.Value.ToString() == "34")
                    {
                        spanlblChequeNo.Visible = true;
                        lblChequeNo.Visible = true;
                        lblChequeNo.Text = "Slip No";
                        txtChequeNo.Visible = true;
                        drpChequeNo.Visible = false;
                        txtDebitAmount.Enabled = true;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = false;
                        txtCreditAmount.Text = "";
                        PanelOpenChqPayOrder.Visible = false;
                    }
                    else
                    {
                        spanlblChequeNo.Visible = false;
                        lblChequeNo.Visible = false;
                        drpChequeNo.Visible = false;
                        txtDebitAmount.Enabled = false;
                        txtDebitAmount.Text = "";
                        txtCreditAmount.Enabled = true;
                        txtCreditAmount.Text = "";
                        PanelOpenChqPayOrder.Visible = false;
                    }

                }
                else
                {
                    spanlblChequeNo.Visible = false;
                    lblChequeNo.Visible = false;
                    drpChequeNo.Visible = false;
                    txtDebitAmount.Enabled = false;
                    txtDebitAmount.Text = "";
                    txtCreditAmount.Enabled = true;
                    txtCreditAmount.Text = "";
                    PanelOpenChqPayOrder.Visible = false;
                }

            }

        }
    }

    protected void ChbIsAssigned_CheckedChanged(object sender, EventArgs e)
    {
        decimal totalamount = 0;
        foreach (GridViewRow row in GridChqPayOrder.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                bool Check = ((System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("ChbIsAssigned")).Checked;
                if (Check)
                {
                    totalamount += decimal.Parse(row.Cells[7].Text);
                }
            }
        }
        txtDebitAmount.Text = Math.Round(totalamount, 0).ToString();
        txtDebitAmount.Enabled = false;
    }

    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        drpBanks_SelectedIndexChanged(null, null);
    }
}