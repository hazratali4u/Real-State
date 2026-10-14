using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmChequeEntryCustomerNew : System.Web.UI.Page
{
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    readonly DistributorController DController = new DistributorController();
    DataTable SaleDetail = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["CHEQUE_PROCESS_IDs"] = "";
            //if (IsDayClosed())
            //{
            //    UserController UserCtl = new UserController();

            //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            //    Session.Clear();
            //    System.Web.Security.FormsAuthentication.SignOut();
            //    Response.Redirect("../Login.aspx");
            //}
            //btnSave.Enabled = true;
            string strProcessScript = "this.value='Wait...';this.disabled=true;";
            btnSave.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnSave, "").ToString());
            //btnSave.Attributes.Add("onclick", "javascript: if (Page_ClientValidate() ){" + btnSave.ClientID + ".disabled=true;}" + ClientScript.GetPostBackEventReference(btnSave, ""));
            CreatTable();
            PaymentType();
            SaleChqueStatus();
            //btnSave.Attributes.Add("onclick", "return ValidateForm();");
            txtStartDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            //txtTransactionDate.Text = Convert.ToString(DateTime.Now);
            txtTransactionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
            LoadDistributor();
            LoadAccountDetail();
            LoadData();
            CustomerItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            LoadCheque(2);
            PanelCrossPayment.Visible = true;
            //Vendor();
            DrpStatus.SelectedIndex = 0;
            DrpStatus.Enabled = false;
            CrossPanel1.Visible = false;
            DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            toggleControls(DrpAccountType.SelectedItem.Value.ToString());
            Session.Add("dtConfig", dtConfig);
            Session.Add("IsFinanceSetting", IsFinanceSetting);
        }
    }
    public void SaleChqueStatus()
    {
        DrpStatus.Items.Clear();
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Received", "526"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));
        DrpStatus.SelectedIndex = 0;
    }
    public void SaleReturnChqueStatus()
    {
        DrpStatus.Items.Clear();
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Issue", "527"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));
        DrpStatus.SelectedIndex = 0;
    }
    private void toggleControls(string pAccountType)
    {
        ddlCrossPAyment.Items.Clear();
        ddlCrossPAyment.Items.Add(new DevExpress.Web.ListEditItem("Chart of Account", "1"));
        ddlCrossPAyment.Enabled = false;
        ddlCrossPAyment.SelectedIndex = 0;
        if (pAccountType == "21" || pAccountType == "46" || pAccountType == "47")
        {
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            lblBankName.Visible = false;
            txtBankName.Visible = false;
            lblChequeNo.Visible = false;
            txtChequeNumber.Visible = false;
            drpCheque.Visible = false;
            lblChequeDate.Visible = false;
            txtStartDate.Visible = false;
            ibtnStartDate.Visible = false;
            DrpStatus.Visible = false;
            lblStatus.Visible = false;
            GrdCheque.Visible = false;
            GrdCO.Visible = true;
            CrossPanel1.Visible = false;
            if (pAccountType == "46")
            {
                CrossPanel1.Visible = true;
                GrdCO.Visible = true;
                ddlCrossPAyment.Enabled = true;
                ddlCrossPAyment.Items.Clear();
                ddlCrossPAyment.Items.Add(new DevExpress.Web.ListEditItem("Supplier", "2"));
                ddlCrossPAyment.Items.Add(new DevExpress.Web.ListEditItem("Customer", "3"));
                ddlCrossPAyment.SelectedIndex = 0;
                ddlCrossPAyment_SelectedIndexChanged(null, null);
            }

        }
        else if (pAccountType == "33")
        {
            lblFavouring.Visible = false;
            txtFavouring.Visible = false;
            lblBankName.Visible = false;
            txtBankName.Visible = false;
            lblChequeNo.Visible = false;
            txtChequeNumber.Visible = false;
            drpCheque.Visible = false;
            lblChequeDate.Visible = true;
            txtStartDate.Visible = true;
            ibtnStartDate.Visible = true;
            DrpStatus.Visible = false;
            lblStatus.Visible = false;
            lblChequeDate.InnerText = "Transfer Date";
            //GrdCO.Visible = false;
            CrossPanel1.Visible = false;
            GrdCheque.Visible = false;

        }
        else
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // Sale
            {
                lblFavouring.Visible = true;
                txtFavouring.Visible = true;
                lblBankName.Visible = true;
                txtBankName.Visible = true;
                lblChequeNo.Visible = true;
                txtChequeNumber.Visible = true;
                drpCheque.Visible = false;
                SaleChqueStatus();
                //GrdCO.Visible = false;
                CrossPanel1.Visible = false;
            }
            else // Sale Return
            {
                lblFavouring.Visible = true;
                txtFavouring.Visible = true;
                lblBankName.Visible = true;
                txtBankName.Visible = true;
                lblChequeNo.Visible = true;
                txtChequeNumber.Visible = false;
                drpCheque.Visible = true;
                SaleReturnChqueStatus();
                //GrdCO.Visible = false;
                CrossPanel1.Visible = false;
            }

            if (pAccountType == "18" || pAccountType == "45")
            {
                lblChequeDate.InnerText = "Cheque Date";
                lblChequeNo.InnerText = "Cheque No";
            }
            else
            {
                lblChequeDate.InnerText = "Pay Order Date";
                lblChequeNo.InnerText = "Pay Order No";
            }

            lblFavouring.Visible = true;
            txtFavouring.Visible = true;
            lblBankName.Visible = true;
            txtBankName.Visible = true;
            lblChequeDate.Visible = true;
            txtStartDate.Visible = true;
            ibtnStartDate.Visible = true;
            DrpStatus.Visible = true;
            lblStatus.Visible = true;
            //lblChequeDate.Text = "Cheque Date";
            //GrdCO.Visible = false;
            CrossPanel1.Visible = false;
            GrdCheque.Visible = true;

        }
    }
    public void CustomerItems()
    {
        if (DrpVendor.Items.Count > 0)
        {
            if (Hp.SelectedValue != "2")
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
                {
                    int CustomerID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    DbCalls db = new DbCalls();
                    var qry = (from k in db.tbl_SALE_MASTER
                               join sd in db.tbl_SALE_DETAIL on k.SALE_MASTER_ID equals sd.SALE_MASTER_ID
                               join i in db.ITEM_INFORMATION on sd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                               join b in db.BLOCKs on i.BLOCK_ID equals b.BLOCK_ID
                               join s in db.STREETs on i.STREET_ID equals s.STREET_ID
                               select new { k.SALE_MASTER_ID, sd.SALE_DETAIL_ID, k.CUSTOMER_ID, sd.ITEM_INFO_ID, ITEM_NAME = "Ref.# " + sd.REF_ID + ", " + i.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME, sd.SALE_CLOSED }).Where(x => x.SALE_CLOSED == false && x.CUSTOMER_ID == CustomerID).ToList();
                    if (qry.Count != 0)
                    {
                        ddlItem.Items.Clear();
                        ddlItem.DataSource = qry;
                        ddlItem.TextField = "ITEM_NAME";
                        ddlItem.ValueField = "SALE_DETAIL_ID";
                        ddlItem.DataBind();
                    }
                    else
                    {
                        ddlItem.Items.Clear();
                        ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                    }
                }
                else
                {
                    ddlItem.Items.Clear();
                    ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                }
            }
            else
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
                {
                    int CustomerID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    DataTable dt = LController.SelectBSI(CustomerID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 49);
                    if (dt.Rows.Count > 0)
                    {
                        ddlItem.Items.Clear();
                        ddlItem.DataSource = dt;
                        ddlItem.TextField = "ITEM_NAME";
                        ddlItem.ValueField = "SALE_MASTER_ID";
                        ddlItem.DataBind();
                        ddlItem.SelectedIndex = 0;
                    }
                    else
                    {
                        ddlItem.Items.Clear();
                        ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                        ddlItem.SelectedIndex = 0;
                    }
                }
                else
                {
                    ddlItem.Items.Clear();
                    ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                }
            }
            ddlItem.SelectedIndex = 0;
        }

    }
    public void LoadSaleInstallments(int ID)
    {
        DbCalls db = new DbCalls();
        //int ItemID = Convert.ToInt32(ddlItem.SelectedItem.Value);
        var qry = (from k in db.tbl_SALE_INS_DETAIL
                   join sm in db.tbl_SALE_INS_MASTER on k.SALE_INS_MASTER_ID equals sm.SALE_INS_MASTER_ID
                   select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.PAYMENT_DESC, k.DUE_DATE, k.OS_AMOUNT, k.IS_PAID, sm.DEBIT_AMOUNT, k.INST_NO, sm.SALE_CLOSED, TRANSACTION_TYPE_ID = 13 }).Where(x => x.IS_PAID == false && x.SALE_CLOSED == false && x.SALE_INS_MASTER_ID == ID).OrderBy(x => x.DUE_DATE).ToList();
        GrdCreditInstallment.DataSource = qry;
        GrdCreditInstallment.DataBind();
        CheckBox cb = null;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in GrdCreditInstallment.Rows)
        {
            cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
            //cb.Checked = true;
            TotalAmount += Convert.ToDecimal(gvr.Cells[6].Text);
        }
        txtTotalCreditAmount.Text =
            string.Format("{0:n2}", TotalAmount);
        Panel1.Visible = false;
        Panel2.Visible = false;
        Panel3.Visible = true;
    }
    public void PaymentType()
    {
        if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
        {
            DrpAccountType.Items.Clear();
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cross Cheque", "45"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("HWL", "46"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("JV", "47"));
            DrpAccountType.SelectedIndex = 0;
            //ddlCrossPAyment.SelectedItem.Value = "1";
            //ddlCrossPAyment.Enabled = true;
            ddlCrossPAyment.SelectedIndex = 0;
            ddlCrossPAyment.Enabled = true;
        }
        else if (ddlTransactionType.SelectedItem.Value.ToString() == "3")
        {
            DrpAccountType.Items.Clear();
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cross Cheque", "45"));
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("HWL", "46"));
            DrpAccountType.SelectedIndex = 0;
            //ddlCrossPAyment.SelectedItem.Value = "1";
            //ddlCrossPAyment.Enabled = true;
            ddlCrossPAyment.SelectedIndex = 0;
            ddlCrossPAyment.Enabled = true;
        }
        else
        {
            DrpAccountType.Items.Clear();
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cross Cheque", "45"));
            //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("HWL", "46"));
            DrpAccountType.SelectedIndex = 0;
            ////ddlCrossPAyment.SelectedItem.Value = "1";
            //ddlCrossPAyment.Enabled = true;
            ddlCrossPAyment.SelectedIndex = 0;
            ddlCrossPAyment.Enabled = false;
            CrossPanel1.Visible = false;
        }
    }
    #region Load
    private void CreatTable()
    {
        SaleDetail = new DataTable();
        SaleDetail.Columns.Add("SALE_MASTER_ID", typeof(long));
        SaleDetail.Columns.Add("ITEM", typeof(string));
        SaleDetail.Columns.Add("DATE", typeof(DateTime));
        SaleDetail.Columns.Add("DEBIT_AMOUNT", typeof(int));
        SaleDetail.Columns.Add("VENDOR_ID", typeof(int));
        SaleDetail.Columns.Add("IS_DELETED", typeof(bool));
        SaleDetail.Columns.Add("TYPE_ID", typeof(int));
        this.Session.Add("SaleDetail", SaleDetail);
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

    private void LoadPaymentRecieved()
    {
        DateTime Date = Constants.DateNullValue;
        string abd = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
        ChequeEntryController CController = new ChequeEntryController();
        LedgerController LController = new LedgerController();


        if (drpDistributor.Items.Count > 0)
        {
            DataSet dsReceived = LController.SelectBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, 21,
                Date);

            DataTable dtRealized = dsReceived.Tables[2];

            if (dtRealized.Rows.Count > 0)
            {
                lblAmount.Text = string.Format("{0:0,0.00}", Convert.ToDecimal(dtRealized.Rows[0][0].ToString()));

            }

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

    private void LoadData()
    {
        if (Hp.SelectedValue != "2")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
            {
                var PController = new SKUPriceDetailController();
                DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString())
                    , Constants.IntNullValue, 7, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
                if (dtVendor.Rows.Count > 0)
                {
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Customer";
                }
            }
            else if (ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                var PController = new SKUPriceDetailController();
                DataTable dtVendor = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 19);
                clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
                if (dtVendor.Rows.Count > 0)
                {
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Vendor";
                }
            }
            else
            {
                var PController = new SKUPriceDetailController();
                DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 7, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
                if (dtVendor.Rows.Count > 0)
                {
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Customer";
                }
            }
        }
        else
        {
            DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 17);
            if (dt.Rows.Count > 0)
            {
                DrpVendor.Items.Clear();
                DrpVendor.DataSource = dt;
                DrpVendor.TextField = "VENDOR_NAME";
                DrpVendor.ValueField = "VENDOR_ID";
                DrpVendor.DataBind();
                DrpVendor.SelectedIndex = 0;
            }
            else
            {
                DrpVendor.Items.Clear();
                DrpVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                DrpVendor.SelectedIndex = 0;
            }
        }
    }

    private void LoadReceviedCheque()
    {
        ChequeEntryController CController = new ChequeEntryController();
        GrdCheque.DataSource = null;
        GrdCheque.DataBind();

        GrdCO.DataSource = null;
        GrdCO.DataBind();

        decimal chqAmount = 0;
        if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "46")
        {
            if (drpDistributor.Items.Count > 0)
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
                {
                    DateTime Date = Constants.DateNullValue;
                    string a = txtTransactionDate.Text;
                    string abd = a.Replace("-", "/");
                    Date = DateTime.Parse(ConvertDate.British_To_American(abd));
                    if (Hp.SelectedValue == "1")
                    {
                        int Type = 0;
                        if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                        {
                            Type = 6;
                        }
                        else
                        {
                            if (ddlTransactionType.SelectedItem.Value.ToString() == "3")
                            {
                                Type = 3;
                            }
                            else
                            {
                                Type = 1;
                            }

                        }
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
                          Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Type);
                        Session.Add("dt", dt);
                        GrdCO.DataSource = dt;
                        GrdCO.DataBind();
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow gvr in dt.Rows)
                            {
                                chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                            }
                            lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[0].Visible = false;
                            GrdCO.FooterRow.Cells[5].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[4].Text = "Total";
                            GrdCO.FooterRow.Cells[4].Font.Bold = true;
                            GrdCO.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;

                        }
                    }
                    else
                    {
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
                          Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), 5);
                        Session.Add("dt", dt);
                        GrdCO.DataSource = dt;
                        GrdCO.DataBind();
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow gvr in dt.Rows)
                            {
                                chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                            }
                            lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[0].Visible = false;
                            GrdCO.FooterRow.Cells[5].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[4].Text = "Total";
                            GrdCO.FooterRow.Cells[4].Font.Bold = true;
                            GrdCO.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }

                }
                else // For Sale Return
                {
                    DateTime Date = Constants.DateNullValue;
                    string a = txtTransactionDate.Text;
                    string abd = a.Replace("-", "/");
                    Date = DateTime.Parse(ConvertDate.British_To_American(abd));
                    if (Hp.SelectedValue == "1")
                    {
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
                         Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), 2);
                        Session.Add("dt", dt);
                        GrdCO.DataSource = dt;
                        GrdCO.DataBind();
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow gvr in dt.Rows)
                            {
                                chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                            }
                            lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[0].Visible = false;
                            GrdCO.FooterRow.Cells[5].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[4].Text = "Total";
                            GrdCO.FooterRow.Cells[4].Font.Bold = true;
                            GrdCO.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }
                    else
                    {
                        GrdCO.DataSource = null;
                        GrdCO.DataBind();
                        //GrdCO.FooterRow.Cells[0].Visible = false;
                        //GrdCO.FooterRow.Cells[5].Text = string.Format("{0:n2}", chqAmount);
                        //GrdCO.FooterRow.Cells[4].Text = "Total";
                        //GrdCO.FooterRow.Cells[4].Font.Bold = true;
                        //GrdCO.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                        //GrdCO.FooterRow.Cells[5].Font.Bold = true;
                        //GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    }
                }
            }
        }

        else
        {
            if (DrpStatus.SelectedItem.Value.ToString() != Constants.Cheque_Clear.ToString())
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
                {
                    DataTable dt = null;
                    if (DrpVendor.Items.Count > 0)
                    {
                        int Ststus = 0;
                        DateTime Date = Constants.DateNullValue;
                        string a = txtTransactionDate.Text;
                        string abd = a.Replace("-", "/");
                        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
                        //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                        //{
                        //    Ststus = 626628;
                        //}
                        //else
                        //{
                        Ststus = 526528;
                        //}
                        dt = CController.SelectVendorChequeEntry(Ststus, Date, int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), int.Parse(Session["UserId"].ToString())
                            , int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                        Session.Add("dt", dt);

                        GrdCheque.DataSource = dt;
                        GrdCheque.DataBind();

                    }
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow gvr in dt.Rows)
                        {
                            chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                        }
                        GrdCheque.FooterRow.Cells[0].Visible = false;
                        GrdCheque.FooterRow.Cells[1].Visible = false;
                        GrdCheque.FooterRow.Cells[7].Text = string.Format("{0:n2}", chqAmount);
                        GrdCheque.FooterRow.Cells[6].Text = "Total";
                        GrdCheque.FooterRow.Cells[6].Font.Bold = true;
                        GrdCheque.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                        GrdCheque.FooterRow.Cells[7].Font.Bold = true;
                        GrdCheque.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                        GrdCheque.FooterRow.Cells[8].Visible = false;
                        GrdCheque.FooterRow.Cells[9].Visible = false;
                        GrdCheque.FooterRow.Cells[10].Visible = false;
                        GrdCheque.FooterRow.Cells[11].Visible = false;
                        GrdCheque.FooterRow.Cells[12].Visible = false;
                        GrdCheque.FooterRow.Cells[13].Visible = false;
                        GrdCheque.FooterRow.Cells[14].Visible = false;
                    }
                    lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);

                    foreach (GridViewRow gvr in GrdCredit.Rows)
                    {
                        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                        if (row.Enabled)
                        {
                            row.Enabled = true;
                        }
                        else
                        {
                            row.Enabled = true;
                        }
                    }
                }
                else // For Sale Return
                {
                    DataTable dt = null;
                    if (DrpVendor.Items.Count > 0)
                    {
                        DateTime Date = Constants.DateNullValue;
                        string a = txtTransactionDate.Text;
                        string abd = a.Replace("-", "/");
                        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
                        dt = CController.SelectVendorChequeEntry(528528, Date, int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), int.Parse(Session["UserId"].ToString())
                            , int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                        Session.Add("dt", dt);
                        GrdCheque.DataSource = dt;
                        GrdCheque.DataBind();

                    }
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow gvr in dt.Rows)
                        {
                            chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                        }
                        GrdCheque.FooterRow.Cells[0].Visible = false;
                        GrdCheque.FooterRow.Cells[1].Visible = false;
                        GrdCheque.FooterRow.Cells[7].Text = string.Format("{0:n2}", chqAmount);
                        GrdCheque.FooterRow.Cells[6].Text = "Total";
                        GrdCheque.FooterRow.Cells[6].Font.Bold = true;
                        GrdCheque.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                        GrdCheque.FooterRow.Cells[7].Font.Bold = true;
                        GrdCheque.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                        GrdCheque.FooterRow.Cells[8].Visible = false;
                        GrdCheque.FooterRow.Cells[9].Visible = false;
                        GrdCheque.FooterRow.Cells[10].Visible = false;
                        GrdCheque.FooterRow.Cells[11].Visible = false;
                        GrdCheque.FooterRow.Cells[12].Visible = false;
                        GrdCheque.FooterRow.Cells[13].Visible = false;
                        GrdCheque.FooterRow.Cells[14].Visible = false;
                    }
                    lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);

                    foreach (GridViewRow gvr in GrdCreditReturn.Rows)
                    {
                        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                        if (row.Enabled)
                        {
                            row.Enabled = true;
                        }
                        else
                        {
                            row.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                DateTime Date = Constants.DateNullValue;
                string a = txtTransactionDate.Text;
                string abd = a.Replace("-", "/");
                Date = DateTime.Parse(ConvertDate.British_To_American(abd));
                ///Load Cash Realization Detail
                if (Convert.ToInt32(DrpStatus.SelectedItem.Value.ToString()) < 530)
                {

                    DataSet ds = CController.SelectVendorChequeEntry(int.Parse(DrpStatus.SelectedItem.Value.ToString()), Date, int.Parse(drpDistributor.SelectedItem.Value.ToString()), DrpAccountType.SelectedIndex);
                    DataTable dt2 = ds.Tables[1];
                    if (dt2.Rows.Count > 0)
                    {

                        foreach (DataRow dr in dt2.Rows)
                        {
                            chqAmount += Convert.ToDecimal(dr[2].ToString());
                        }
                        lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);
                    }
                }
                ////////////////////////////////////////////////

                DataTable dt = CController.SelectVendorChequeEntry(int.Parse(DrpStatus.SelectedItem.Value.ToString()), Date, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(Session["UserId"].ToString())
                    , int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                Session.Add("dt", dt);

                GrdCheque.DataSource = dt;
                GrdCheque.DataBind();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow gvr in dt.Rows)
                    {
                        chqAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                    }
                    GrdCheque.FooterRow.Cells[0].Visible = false;
                    GrdCheque.FooterRow.Cells[1].Visible = false;
                    GrdCheque.FooterRow.Cells[7].Text = string.Format("{0:n2}", chqAmount);
                    GrdCheque.FooterRow.Cells[6].Text = "Total";
                    GrdCheque.FooterRow.Cells[6].Font.Bold = true;
                    GrdCheque.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                    GrdCheque.FooterRow.Cells[7].Font.Bold = true;
                    GrdCheque.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                    GrdCheque.FooterRow.Cells[8].Visible = false;
                    GrdCheque.FooterRow.Cells[9].Visible = false;
                    GrdCheque.FooterRow.Cells[10].Visible = false;
                    GrdCheque.FooterRow.Cells[11].Visible = false;
                    GrdCheque.FooterRow.Cells[12].Visible = false;
                    GrdCheque.FooterRow.Cells[13].Visible = false;
                    GrdCheque.FooterRow.Cells[14].Visible = false;
                }

                foreach (GridViewRow gvr in GrdCheque.Rows)
                {
                    LinkButton row = gvr.FindControl("btnEdit") as LinkButton;
                    row.Enabled = false;
                }
                foreach (GridViewRow gvr in GrdCredit.Rows)
                {
                    CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                    row.Checked = false;
                    row.Enabled = false;
                }
                foreach (GridViewRow gvr in GrdCreditReturn.Rows)
                {
                    CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                    row.Checked = false;
                    row.Enabled = false;
                }
            }
            //if (DrpStatus.SelectedItem.Value.ToString() != Constants.Cheque_Pending.ToString())
            //{
            //    foreach (GridViewRow gvr in GrdCredit.Rows)
            //    {
            //        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
            //        row.Checked = false;
            //        row.Enabled = false;
            //    }
            //}
        }
    }

    private void checkDuplication()
    {
        DataTable dt = (DataTable)Session["dt"];
        DataRow[] foundRows = null;
        if (dt.Rows.Count > 0)
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
            {
                foundRows = dt.Select("VENDOR_ID = '" + DrpVendor.SelectedItem.Value.ToString() + "' and CHEQUE_NO='" + txtChequeNumber.Text + "'");
            }
            else
            {
                foundRows = dt.Select("VENDOR_ID = '" + DrpVendor.SelectedItem.Value.ToString() + "' and CHEQUE_NO='" + drpCheque.SelectedItem.Text + "'");
            }

            if (foundRows.Length > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Cheque No Already exist against this customer!');", true);
            }
        }

    }

    private void SelectCreditInvoice()
    {
        LedgerController CDC = new LedgerController();

        GrdCredit.DataSource = null;
        GrdCredit.DataBind();

        if (DrpVendor.Items.Count > 0)
        {
            int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            int ItemID = Convert.ToInt32(ddlItem.SelectedItem.Value);
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13") //For Sale
            {
                if (Hp.SelectedValue != "2")
                {
                    LoadSaleInvoices(VendorID);
                }
                else
                {
                    LoadSaleInstallments(ItemID);
                }
            }
            else if (ddlTransactionType.SelectedItem.Value.ToString() == "3") //For Purchase Return
            {
                if (Hp.SelectedValue != "2")
                {
                    LoadPurchaseReturnInvoices(VendorID);
                }
                else
                {
                    LoadSaleInstallments(ItemID);
                }
            }
            else //For Sale Return
            {
                if (Hp.SelectedValue != "2")
                {
                    LoadSaleReturnInvoices(VendorID);
                }
                else
                {

                }
            }
        }
    }
    private void LoadSaleInvoices(int ID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, ID, Constants.IntNullValue, Constants.IntNullValue, null, 12);
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
            txtTotalCreditAmount.Text =
            string.Format("{0:n2}", TotalAmount);
            GrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            GrdCredit.FooterRow.Cells[3].Text = "Total";
            GrdCredit.FooterRow.Cells[3].Font.Bold = true;
            GrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            GrdCredit.FooterRow.Cells[4].Font.Bold = true;
            GrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            GrdCredit.FooterRow.Cells[1].Visible = false;
            GrdCredit.FooterRow.Cells[5].Visible = false;
            GrdCredit.FooterRow.Cells[6].Visible = false;
            GrdCredit.FooterRow.Cells[7].Visible = false;
            GrdCredit.FooterRow.Cells[8].Visible = false;
            GrdCredit.FooterRow.Cells[9].Visible = false;
        }
        Panel1.Visible = true;
        Panel2.Visible = false;
        Panel3.Visible = false;
        Session["SaleDetail"] = null;
    }
    private void LoadSaleReturnInvoices(int ID)
    {
        DataTable dt = LController.SelectBSI(int.Parse(drpDistributor.SelectedItem.Value.ToString()), ID, Constants.IntNullValue, Constants.IntNullValue, null, 44);
        GrdCreditReturn.DataSource = dt;
        GrdCreditReturn.DataBind();
        CheckBox cb = null;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in GrdCreditReturn.Rows)
        {
            cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
            //cb.Checked = true;
            TotalAmount += Convert.ToDecimal(gvr.Cells[4].Text);
        }
        if (dt.Rows.Count > 0)
        {
            txtTotalCreditAmount.Text =
            string.Format("{0:n2}", TotalAmount);
            GrdCreditReturn.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            GrdCreditReturn.FooterRow.Cells[3].Text = "Total";
            GrdCreditReturn.FooterRow.Cells[3].Font.Bold = true;
            GrdCreditReturn.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            GrdCreditReturn.FooterRow.Cells[4].Font.Bold = true;
            GrdCreditReturn.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            GrdCreditReturn.FooterRow.Cells[1].Visible = false;
            GrdCreditReturn.FooterRow.Cells[5].Visible = false;
            GrdCreditReturn.FooterRow.Cells[6].Visible = false;
            GrdCreditReturn.FooterRow.Cells[7].Visible = false;
            //GrdCreditReturn.FooterRow.Cells[8].Visible = false;
        }
        Panel1.Visible = false;
        Panel2.Visible = true;
        Panel3.Visible = false;
    }
    public void LoadPurchaseReturnInvoices(int VendorID)
    {
        DataTable dt = LController.SelectBSI(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VendorID, Constants.IntNullValue, Constants.IntNullValue, null, 45);
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
            txtTotalCreditAmount.Text =
            string.Format("{0:n2}", TotalAmount);
            GrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            GrdCredit.FooterRow.Cells[3].Text = "Total";
            GrdCredit.FooterRow.Cells[3].Font.Bold = true;
            GrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            GrdCredit.FooterRow.Cells[4].Font.Bold = true;
            GrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            GrdCredit.FooterRow.Cells[1].Visible = false;
            GrdCredit.FooterRow.Cells[5].Visible = false;
            GrdCredit.FooterRow.Cells[6].Visible = false;
            GrdCredit.FooterRow.Cells[7].Visible = false;
            GrdCredit.FooterRow.Cells[8].Visible = false;
            GrdCredit.FooterRow.Cells[9].Visible = false;
        }
        Panel1.Visible = true;
        Panel2.Visible = false;
        Panel3.Visible = false;
        Session["SaleDetail"] = null;
    }
    private void LoadAccountHead(int p_Parent)
    {
        //Configuration.GetAccountHead();
        //AccountHeadController mAccountController = new AccountHeadController();

        //if (DrpAccountType.SelectedItem.Value.ToString() == "21")
        //{
        //    DataTable dt = mAccountController.SelectAccountHeadByMapping(Constants.AC_AccountHeadId, Constants.LongNullValue, 2, Constants.AC_CashInHandAccountHead);
        //    clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dt, 0, 4, true);

        //    if (dt.Rows.Count > 0)
        //    {
        //        DrpBankAccount.SelectedIndex = 0;
        //    }
        //}
        //else
        //{
        //    DataTable dt = mAccountController.SelectAccountHeadByMapping(Constants.AC_AccountHeadId, Constants.LongNullValue, 2, Constants.AC_BankAccountHead);
        //    clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dt, 0, 4, true);

        //    if (dt.Rows.Count > 0)
        //    {
        //        DrpBankAccount.SelectedIndex = 0;
        //    }
        //}
        ///////////////////// For Fourth Level //////////////////
        //DrpBankAccount.Items.Clear();
        //AccountHeadController mAccountController = new AccountHeadController();
        //DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, 1);
        //clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_HEAD", true);
        //if (dtHead.Rows.Count > 0)
        //{
        //    DrpBankAccount.SelectedIndex = 0;
        //}
        //Session.Add("dtHead", dtHead);
        DataTable dtHead = new DataTable();
        int TypeID;
        if (p_Parent == 18)
        {
            TypeID = 46;  //TypeID = p_Parent;
        }
        else if (p_Parent == 44)
        {
            TypeID = 46;  //TypeID = p_Parent;
        }
        else if (p_Parent == 46 || p_Parent == 33 || p_Parent == 34)
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
        DrpBankAccount.Items.Clear();
        AccountHeadController mAccountController = new AccountHeadController();
        dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, TypeID);
        clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_HEAD", true);
        if (dtHead.Rows.Count > 0)
        {
            DrpBankAccount.SelectedIndex = 0;
        }
        Session.Add("dtHead", dtHead);
        ///////////////////// END Fourth Level //////////////////
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
    private void CashAdvance(int p_TYPE)
    {
        string ChequeNo = null;
        DateTime chqDate = Constants.DateNullValue;
        string MaxDocumentID = "";
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        string remarks = "";
        int ChequeType = Constants.IntNullValue;
        int TransactionType;
        string Favouring = "";
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        int VoucherType = Constants.CashPayment_Voucher;
        DateTime ChequeDate = Constants.DateNullValue;
        try
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33") //|| DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
            {
                if (txtStartDate.Text.Length == 10)
                {
                    string StartDate = txtStartDate.Text.Replace("-", "/");
                    ChequeDate = DateTime.Parse(ConvertDate.British_To_American(StartDate));

                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Correct Cheque Date Pattern is DD/MM/YYYY.');", true);
            return;
        }
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
        //|| DrpAccountType.SelectedItem.Value.ToString() == "45")//Cheque
        {
            ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
        }

        if (ddlTransactionType.SelectedItem.Value.ToString() == "2")
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                VoucherType = Constants.CashPayment_Voucher;
                TransactionType = 1;
            }
            else
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                {
                    VoucherType = Constants.Journal_Voucher;
                    TransactionType = 0;
                }
                else
                {
                    VoucherType = Constants.Expanse_Voucher;
                    TransactionType = 0;
                }
            }
        }
        else
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                VoucherType = Constants.CashPayment_Voucher;
                TransactionType = 1;
            }
            else
            {
                VoucherType = Constants.Bank_Voucher;
                TransactionType = 0;
            }
        }
        if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "47")
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "47")
            {
                VoucherType = Constants.Journal_Voucher;
            }
            else
            {
                VoucherType = Constants.Cash_Voucher;
            }
            remarks = DrpAccountType.SelectedItem.Text + " / " + txtRemarks.Text;
        }
        else //if (DrpAccountType.SelectedItem.Value.ToString() == "33")
        {
            VoucherType = Constants.Bank_Voucher;
            remarks = DrpAccountType.SelectedItem.Text + " / " + txtRemarks.Text;
        }
        if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
        {
            MaxDocumentID = "";
        }
        else
        {
            MaxDocumentID = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
        }
        Session.Add("VoucherNo", MaxDocumentID);

        if (DrpAccountType.SelectedIndex == 0)
        {
            ChequeNo = ChequeNO();
        }
        else if (DrpAccountType.SelectedIndex == 3)
        {
            ChequeNo = ChequeNO();
        }
        else
        {
            ChequeNo = txtChequeNumber.Text;
        }

        //Advance Entry for sale
        IDbConnection mConnection = null;
        IDbTransaction mTransaction = null;
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            if (realizeAmount < decimal.Parse(txtAmount.Text))
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentID, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, "", int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentID, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, "", int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
            }
            // GL VOUCHER ENTRY
            #region GL LEDGER
            if (p_TYPE == 1)
            {
                InsertGL(MaxDocumentID, mConnection, mTransaction);
            }
            else
            {
                string ChequeNo2 = null;
                //if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                //{
                //    ChequeNo2 = txtChequeNumber.Text;
                //}
                InsertGL2(ChequeNo2, MaxDocumentID, mConnection, mTransaction);
            }
            #endregion

            mTransaction.Commit();
        }
        catch (Exception exp)
        {
            mTransaction.Rollback();
            ExceptionPublisher.PublishException(exp);
        }
        finally
        {
            if (mConnection != null && mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
        }
        PrintVoucher(MaxDocumentID);
    }
    private string ChequeNO()
    {
        string ChequeNo;
        if (DrpAccountType.SelectedIndex == 0)
        {
            if (DrpBankAccount.SelectedItem.Value.ToString() == "84" && DrpAccountType.SelectedItem.Value.ToString() == "18")
            {
                ChequeNo = "";
            }
            else
            {
                ChequeNo = drpCheque.SelectedItem.Text;
            }
        }
        else if (DrpAccountType.SelectedIndex == 3)
        {
            if (DrpBankAccount.SelectedItem.Value.ToString() == "85" && DrpAccountType.SelectedItem.Value.ToString() == "44")
            {
                ChequeNo = "";
            }
            else
            {
                ChequeNo = drpCheque.SelectedItem.Text;
            }
        }
        else if (DrpAccountType.SelectedIndex == 4)
        {
            ChequeNo = drpCheque.SelectedItem.Text;
        }
        else
        {
            ChequeNo = txtChequeNumber.Text;
        }
        return ChequeNo;
    }
    private void ChequeRealization(int p_TYPE)
    {
        #region General Veriables
        DbCalls db = new DbCalls();
        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        string MaxDocumentIdForCross = "";
        string Item_Name = Session["ITEM_NAME"].ToString();
        string Customer_Name = Session["Customer_Name"].ToString();
        int VoucherType;
        int TransactionType;
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal OfferAmount2 = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;
        decimal realizeAmount2 = 0;
        decimal realizeAmount3 = 0;
        string remarks = "";
        string remarks2 = "";
        int ChequeType = Constants.IntNullValue;
        string Favouring = "";
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        Session.Add("VoucherNo", MaxDocumentId);
        Session.Add("CrossVoucherNo", MaxDocumentIdForCross);
        string ChequeNo = null;
        string DocumentNo = null;
        bool FE = false;
        #endregion
        #region ChequeDate
        DateTime ChequeDate = Constants.DateNullValue;
        try
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
            {
                if (txtStartDate.Text.Length == 10)
                {
                    string transactionDate = txtStartDate.Text.Replace("-", "/");
                    ChequeDate = DateTime.Parse(ConvertDate.British_To_American(transactionDate));

                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Correct Cheque Date Pattern is DD/MM/YYYY.');", true);
            return;
        }
        #endregion
        #region Transaction Type
        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                {
                    VoucherType = Constants.HWL_Voucher;
                    TransactionType = 1;
                }
                else
                {
                    VoucherType = Constants.Journal_Voucher;
                    TransactionType = 1;
                }
            }
            else
            {
                VoucherType = Constants.Bank_Voucher;
                TransactionType = 0;
            }
        }
        else
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                VoucherType = Constants.Journal_Voucher;
                TransactionType = 1;
            }
            else
            {
                VoucherType = Constants.Expanse_Voucher;
                TransactionType = 0;
            }
        }
        #endregion
        #region Set Payments Types
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")//Cheque
        {
            ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
            Favouring = txtFavouring.Text;
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            //remarks = "Chq# " + txtChequeNumber.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                }

            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                }

            }

        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "33")//Online
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + " Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + txtRemarks.Text;
                }

            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + " Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + txtRemarks.Text;
                }

            }
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "46" || DrpAccountType.SelectedItem.Value.ToString() == "47") // Cash & JV
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        VoucherType = Constants.HWL_Voucher;
                        TransactionType = 1;
                    }
                    else
                    {
                        VoucherType = Constants.Journal_Voucher;
                        TransactionType = 1;
                    }
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                    {
                        VoucherType = Constants.Journal_Voucher;
                        TransactionType = 0;
                    }
                    else
                    {
                        VoucherType = Constants.Cash_Voucher;
                        TransactionType = 0;
                    }
                }
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    VoucherType = Constants.Journal_Voucher;
                    TransactionType = 1;
                }
                else
                {
                    VoucherType = Constants.CashPayment_Voucher;
                    TransactionType = 0;
                }
            }
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                        {
                            remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", HWL, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                            remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", HWL, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                        else
                        {
                            remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Receipt, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                            remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Receipt, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                        {
                            remarks = ddlTransactionType.SelectedItem.Text + ", JV, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                        else
                        {
                            remarks = ddlTransactionType.SelectedItem.Text + ", Cash Receipt, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Receipt, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + txtRemarks.Text;
                }

            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + txtRemarks.Text;
                }
            }
        }

        #endregion

        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") //For Sale
        {
            if (Hp.SelectedValue != "2")
            {
                #region For Sale

                int CrossMasterID = 0;
                int ITEM_LOG_ID = 0;
                if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                {
                    ChequeNo = txtChequeNumber.Text;
                }
                bool isadvanceentered = false;
                bool IsEnteredBeforeVendor = false;
                bool IsEnteredBeforeCustomer = false;
                IDbConnection mConnection = null;
                IDbTransaction mTransaction = null;
                try
                {
                    mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                    mConnection.Open();
                    mTransaction = ProviderFactory.GetTransaction(mConnection);

                    #region Complete Transaction

                    foreach (GridViewRow dr in GrdCredit.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            #region Check Cross Invoices Selected or Not
                            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                            {
                                foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                {
                                    CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized2.Checked == true)
                                    {
                                        CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                        //ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region Set Transection Type

                            int TYPE;
                            int type = Convert.ToInt32(dr.Cells[5].Text);
                            if (type == Constants.InvoiceBooking || type == Constants.SaleBalance || type == Constants.PurchaseBalance || type == Constants.Document_Opening)
                            {
                                if (type == Constants.InvoiceBooking)
                                {
                                    TYPE = Constants.InvoiceBooking;
                                }
                                else if (type == Constants.SaleBalance)
                                {
                                    TYPE = Constants.SaleBalance;
                                }
                                if (type == Constants.PurchaseBalance)
                                {
                                    TYPE = Constants.PurchaseBalance;
                                }
                                else
                                {
                                    TYPE = Constants.Document_Opening;
                                }
                            }
                            else if (type == Constants.Document_Purchase_Return)
                            {
                                TYPE = Constants.Document_Purchase_Return;
                            }
                            else
                            {
                                TYPE = int.Parse(dr.Cells[5].Text);
                            }
                            string manualNo = dr.Cells[1].Text;
                            DocumentNo = dr.Cells[1].Text;
                            if (Convert.ToString(dr.Cells[1].Text) == "opng")
                            {
                                manualNo = "opng";
                            }
                            #endregion

                            if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
                            {
                                realizeAmount += OfferAmount;
                                //if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 13) // For Sale 
                                if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale 
                                {
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                   Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                   ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);
                                }
                                else // For Purchase Return
                                {
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                   Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                   ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);
                                }
                                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                                {
                                    #region For Cross Payment to Vendor
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Payment
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeVendor)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    //ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);

                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET DEBIT_AMOUNT = DEBIT_AMOUNT - " + OfferAmount2 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount
                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            #region Advance Payment
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                }
                                                isadvanceentered = true;
                                            }
                                            IsEnteredBeforeVendor = true;
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                                {
                                    #region For Cross Payment to Customer
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Recipt
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeCustomer)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = CREDIT_AMOUNT - " + OfferAmount2 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }

                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount

                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + 0 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = " + 0 + " WHERE ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    #region Advance Recipt
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                    isadvanceentered = true;

                                                    #endregion
                                                }
                                            }
                                            IsEnteredBeforeCustomer = true;
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
                                if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale
                                {
                                    #region Settel Main Customer Invoice
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT = " + OfferAmount + " where SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }
                                else // For Purchase Return
                                {
                                    #region Settel Main Vendor Invoice
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_RETURN SET CREDIT_AMOUNT = " + OfferAmount + " WHERE PURCHASE_RETURN_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }
                                break;
                            }
                            else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
                            {
                                realizeAmount += decimal.Parse(dr.Cells[4].Text);
                                if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale
                                {
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                               Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);
                                }
                                else // For Purchase Return
                                {
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                               Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                                    Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);
                                }

                                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                                {
                                    #region For Cross Payment to Vendor
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Payment
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeVendor)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);

                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET DEBIT_AMOUNT = DEBIT_AMOUNT - " + OfferAmount2 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount
                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            {
                                                                command2.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            #region Advance Payment
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                }
                                                isadvanceentered = true;
                                            }
                                            IsEnteredBeforeVendor = true;
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                                {
                                    #region For Cross Payment to Customer
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Recipt
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Paymnet Against Invoices
                                        if (!IsEnteredBeforeCustomer)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = CREDIT_AMOUNT - " + OfferAmount2 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }

                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount

                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + 0 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = " + 0 + " WHERE ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    #region Advance Recipt
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                    isadvanceentered = true;

                                                    #endregion
                                                }
                                            }
                                            IsEnteredBeforeCustomer = true;
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
                                if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale
                                {
                                    #region Settel Main Customer Invoice
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT = " + 0 + ", SALE_CLOSED =" + 1 + " where SALE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["ITEM_LOG_ID"]), con))
                                        {
                                            command2.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }
                                else // For Purchase Return
                                {
                                    #region Settel Main Customer Invoice
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_RETURN SET CREDIT_AMOUNT = " + 0 + ", RETURN_CLOSED =" + 1 + " where PURCHASE_RETURN_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["ITEM_LOG_ID"]), con))
                                        {
                                            command2.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }

                            }
                        }
                    }
                    #region Advance Entry for sale
                    if (realizeAmount < decimal.Parse(txtAmount.Text))
                    {
                        if (Convert.ToInt32(ddlTransactionType.SelectedItem.Value.ToString()) == 13) // For Sale
                        {
                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        }
                        else // For Purchase Return
                        {
                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        }
                    }
                    #endregion

                    #endregion

                    // GL VOUCHER ENTRY
                    if (p_TYPE == 1)
                    {
                        InsertGL(MaxDocumentId, mConnection, mTransaction);
                    }
                    else
                    {
                        string ChequeNo2 = null;
                        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                        {
                            ChequeNo2 = txtChequeNumber.Text;
                        }
                        InsertGL2(ChequeNo2, MaxDocumentId, mConnection, mTransaction);
                    }
                    mTransaction.Commit();
                }
                catch (Exception exp)
                {
                    mTransaction.Rollback();
                    ExceptionPublisher.PublishException(exp);
                }
                finally
                {
                    if (mConnection != null && mConnection.State == ConnectionState.Open)
                    {
                        mConnection.Close();
                    }
                }
                PrintVoucher(MaxDocumentId);

                #endregion
            }
            else // For Installments Sale
            {
                #region For Installments Sale
                int CrossMasterID = 0;
                int ITEM_LOG_ID = 0;
                bool isadvanceentered = false;
                bool IsEnteredBeforeVendor = false;
                bool IsEnteredBeforeCustomer = false;
                IDbConnection mConnection = null;
                IDbTransaction mTransaction = null;
                try
                {
                    mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                    mConnection.Open();
                    mTransaction = ProviderFactory.GetTransaction(mConnection);


                    #region Installment Sales Receipts
                    foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {

                            #region Check Cross Invoices Selected or Not
                            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                            {
                                foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                {
                                    CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized2.Checked == true)
                                    {
                                        CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                        //ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                        break;
                                    }
                                }
                            }
                            #endregion
                            
                            string manualNo = dr.Cells[2].Text;
                            DocumentNo = dr.Cells[1].Text;
                            if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                            {
                                ChequeNo = txtChequeNumber.Text;
                            }
                            if (Convert.ToString(dr.Cells[1].Text) == "opng")
                            {
                                manualNo = "opng";
                            }
                            if (decimal.Parse(dr.Cells[6].Text) > OfferAmount)
                            {
                                realizeAmount3 = OfferAmount;
                                realizeAmount += OfferAmount;
                                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                     Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                     ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Installment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment, Favouring, ChequeType);

                                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                 Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                 ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Installment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment, Favouring, ChequeType);

                                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                                {
                                    #region For Cross Payment to Vendor
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Payment
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeVendor)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    //ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);

                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET DEBIT_AMOUNT = DEBIT_AMOUNT - " + OfferAmount2 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount
                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            #region Advance Payment
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                }
                                                isadvanceentered = true;
                                            }
                                            IsEnteredBeforeVendor = true;
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                                {
                                    #region For Cross Payment to Customer
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Recipt
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeCustomer)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = CREDIT_AMOUNT - " + OfferAmount2 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }

                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount

                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + 0 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = " + 0 + " WHERE ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    #region Advance Recipt
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                    isadvanceentered = true;

                                                    #endregion
                                                }
                                            }
                                            IsEnteredBeforeCustomer = true;
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }

                                OfferAmount = decimal.Parse(dr.Cells[6].Text) - OfferAmount;

                                #region Proccess Installment Detail Payment
                                int MasterID = Convert.ToInt32(dr.Cells[1].Text); // SALE_INS_MASTER_ID
                                int DetailID = Convert.ToInt32(dr.Cells[2].Text); // SALE_INS_DETAIL_ID
                                //DbCalls db = new DbCalls();
                                var qry = (from k in db.tbl_SALE_INS_DETAIL
                                           select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.SALE_INS_MASTER_ID == MasterID && x.SALE_INS_DETAIL_ID == DetailID).ToList();
                                //int DueAmount = Convert.ToInt32(qry.First().DUE_AMOUNT);
                                //decimal ReceivedAmount = decimal.Parse(txtAmount.Text);
                                decimal ReceivedAmount = realizeAmount3;
                                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                {
                                    con.Open();
                                    using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_INS_MASTER set DEBIT_AMOUNT = DEBIT_AMOUNT - " + realizeAmount3 + " where SALE_INS_MASTER_ID=" + MasterID, con))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    int firstentry = 0;
                                    bool check = false;
                                    for (int i = 0; i <= qry.Count - 1; i++)
                                    {
                                        var query = (from k in db.tbl_SALE_INS_DETAIL
                                                     select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.REC_AMOUNT, k.OS_AMOUNT, k.IS_PAID, k.DUE_DATE }).Where(x => x.IS_PAID == false && x.SALE_INS_MASTER_ID == MasterID && x.SALE_INS_DETAIL_ID == DetailID).ToList();
                                        int SALE_DETAIL_ID = Convert.ToInt32(query.First().SALE_INS_DETAIL_ID);
                                        decimal DUE_AMOUNT = Convert.ToDecimal(query.First().DUE_AMOUNT);
                                        decimal OS_AMOUNT = Convert.ToDecimal(query.First().OS_AMOUNT);
                                        decimal REC_AMOUNT = Convert.ToDecimal(query.First().REC_AMOUNT);
                                        decimal RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                        if (ReceivedAmount > OS_AMOUNT)
                                        {
                                            if (firstentry != i)
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                check = true;
                                            }
                                            else
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                check = true;
                                            }
                                            ReceivedAmount = ReceivedAmount - OS_AMOUNT;
                                        }
                                        else
                                        {
                                            decimal recv = 0;
                                            int Paid;
                                            decimal OS_Amount = OS_AMOUNT - ReceivedAmount;
                                            if (OS_Amount == 0)
                                            {
                                                recv = RECEIVED;
                                                Paid = 1;
                                            }
                                            else
                                            {
                                                recv = ReceivedAmount;
                                                Paid = 0;
                                            }
                                            if (!check)
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(recv) + ", OS_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", IS_PAID=" + Paid + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_NO = '" + MaxDocumentId + "', PAYMENT_DATE ='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(recv) + ", OS_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", IS_PAID=" + Paid + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    con.Close();
                                }

                                #endregion

                                break;
                            }
                            else if (decimal.Parse(dr.Cells[6].Text) <= OfferAmount)
                            {
                                realizeAmount3 = OfferAmount;
                                realizeAmount += decimal.Parse(dr.Cells[6].Text);
                                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[6].Text), 0,
                                Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                 ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Installment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment, Favouring, ChequeType);

                                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[6].Text),
                                Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Installment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment, Favouring, ChequeType);

                                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                                {
                                    #region For Cross Payment to Vendor
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Payment
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Payment Against Invoices
                                        if (!IsEnteredBeforeVendor)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);

                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET DEBIT_AMOUNT = DEBIT_AMOUNT - " + OfferAmount2 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount
                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            {
                                                                command2.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            #region Advance Payment
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                }
                                                isadvanceentered = true;
                                            }
                                            IsEnteredBeforeVendor = true;
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                                {
                                    #region For Cross Payment to Customer
                                    if (CrossMasterID == 0)
                                    {
                                        if (!isadvanceentered)
                                        {
                                            #region Advance Recipt
                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                            Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                            ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                            isadvanceentered = true;
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        #region Paymnet Against Invoices
                                        if (!IsEnteredBeforeCustomer)
                                        {
                                            foreach (GridViewRow dr2 in CrossGrdCredit.Rows)
                                            {
                                                CheckBox chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");
                                                if (chRelized2.Checked == true)
                                                {
                                                    CrossMasterID = Convert.ToInt32(dr2.Cells[1].Text);
                                                    ITEM_LOG_ID = Convert.ToInt32(dr2.Cells[8].Text);
                                                    if (decimal.Parse(dr2.Cells[4].Text) > OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Greater Then Entered Amoount
                                                        realizeAmount2 += OfferAmount2;
                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount2, 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount2,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = CREDIT_AMOUNT - " + OfferAmount2 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            con.Close();
                                                        }

                                                        #endregion
                                                        break;
                                                    }
                                                    else if (decimal.Parse(dr2.Cells[4].Text) <= OfferAmount2)
                                                    {
                                                        #region When Invoice Amount Lesser Then & Equal to Entered Amoount

                                                        realizeAmount2 += decimal.Parse(dr2.Cells[4].Text);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr2.Cells[4].Text), 0,
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr2.Cells[4].Text),
                                                        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                                        ChequeNo, int.Parse(Session["UserId"].ToString()), CrossMasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType, mConnection, mTransaction);

                                                        OfferAmount2 = OfferAmount2 - decimal.Parse(dr2.Cells[4].Text);

                                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                                        {
                                                            con.Open();
                                                            using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + 0 + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(CrossMasterID), con))
                                                            {
                                                                command.ExecuteNonQuery();
                                                            }
                                                            //using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = " + 0 + " WHERE ITEM_LOG_ID=" + Convert.ToInt64(ITEM_LOG_ID), con))
                                                            //{
                                                            //    command2.ExecuteNonQuery();
                                                            //}
                                                            con.Close();
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                            if (!isadvanceentered)
                                            {
                                                if (realizeAmount2 < decimal.Parse(txtAmount.Text))
                                                {
                                                    #region Advance Recipt
                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                                    isadvanceentered = true;

                                                    #endregion
                                                }
                                            }
                                            IsEnteredBeforeCustomer = true;
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }

                                OfferAmount = OfferAmount - decimal.Parse(dr.Cells[6].Text);

                                #region Proccess Installment Detail Payment
                                int MasterID = Convert.ToInt32(dr.Cells[1].Text); // SALE_INS_MASTER_ID
                                int DetailID = Convert.ToInt32(dr.Cells[2].Text); // SALE_INS_DETAIL_ID
                                //DbCalls db = new DbCalls();
                                var qry = (from k in db.tbl_SALE_INS_DETAIL
                                           select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.SALE_INS_MASTER_ID == MasterID && x.SALE_INS_DETAIL_ID == DetailID).ToList();
                                decimal DUE_AMOUNT = Convert.ToDecimal(qry.FirstOrDefault().DUE_AMOUNT);
                                decimal ReceivedAmount = realizeAmount3; //Convert.ToDecimal(txtAmount.Text);
                                int SALE_MASTER_ID = Convert.ToInt32(qry.FirstOrDefault().SALE_INS_MASTER_ID);

                                var qry2 = (from k in db.tbl_SALE_INS_MASTER select new { k.SALE_INS_MASTER_ID, k.DEBIT_AMOUNT }).Where(x=>x.SALE_INS_MASTER_ID == MasterID).ToList();
                                decimal DEBIT_AMOUNT = Convert.ToDecimal(qry2.FirstOrDefault().DEBIT_AMOUNT);

                                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                {
                                    con.Open();
                                    if ((DEBIT_AMOUNT - ReceivedAmount) <= 0)
                                    {
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_INS_MASTER SET DEBIT_AMOUNT = " + 0 + ", SALE_CLOSED =" + 1 + " WHERE SALE_INS_MASTER_ID=" + Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_INS_MASTER SET DEBIT_AMOUNT = DEBIT_AMOUNT - " + realizeAmount3 + " WHERE SALE_INS_MASTER_ID=" + Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_INS_MASTER_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    
                                    int firstentry = 0;
                                    //bool check = false;
                                    for (int i = 0; i <= qry.Count - 1; i++)
                                    {
                                        var query = (from k in db.tbl_SALE_INS_DETAIL
                                                     select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.REC_AMOUNT, k.OS_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.SALE_INS_MASTER_ID == MasterID && x.SALE_INS_DETAIL_ID == DetailID).ToList();
                                        int SALE_DETAIL_ID = Convert.ToInt32(query.First().SALE_INS_DETAIL_ID);
                                        //int DUE_AMOUNT = Convert.ToInt32(query.First().DUE_AMOUNT);
                                        decimal OS_AMOUNT = Convert.ToDecimal(query.First().OS_AMOUNT);
                                        decimal REC_AMOUNT = Convert.ToDecimal(query.First().REC_AMOUNT);
                                        decimal RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                        if (ReceivedAmount >= OS_AMOUNT)
                                        {
                                            if (firstentry != i)
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                //check = true;
                                            }
                                            else
                                            {
                                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', REC_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                //check = true;
                                            }
                                            ReceivedAmount = ReceivedAmount - OS_AMOUNT;
                                            if (ReceivedAmount <= 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    con.Close();
                                }
                                #endregion

                            }
                        }
                    }
                    #region Advance Entry for sale
                    if (realizeAmount < decimal.Parse(txtAmount.Text))
                    {
                        if (Convert.ToInt32(ddlTransactionType.SelectedItem.Value.ToString()) == 13) // For Sale
                        {
                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        }
                        else // For Purchase Return
                        {
                            //LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                            //                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                            //LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                            //                      ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        }
                    }
                    #endregion
                    #endregion

                    // GL VOUCHER ENTRY
                    if (p_TYPE == 1)
                    {
                        InsertGL(MaxDocumentId, mConnection, mTransaction);
                    }
                    else
                    {
                        string ChequeNo2 = null;
                        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                        {
                            ChequeNo2 = txtChequeNumber.Text;
                        }
                        InsertGL2(ChequeNo2, MaxDocumentId, mConnection, mTransaction);
                    }
                    mTransaction.Commit();
                }
                catch (Exception exp)
                {
                    mTransaction.Rollback();
                    ExceptionPublisher.PublishException(exp);
                }
                finally
                {
                    if (mConnection != null && mConnection.State == ConnectionState.Open)
                    {
                        mConnection.Close();
                    }
                }
                PrintVoucher(MaxDocumentId);

                #endregion
            }
        }
        else  //For Sale Return
        {
            #region Sales Returns Payments
            foreach (GridViewRow dr in GrdCreditReturn.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    string manualNo = dr.Cells[1].Text;
                    DocumentNo = dr.Cells[1].Text;
                    if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                    {
                        ChequeNo = drpCheque.SelectedItem.Text;
                    }
                    if (Convert.ToString(dr.Cells[1].Text) == "opng")
                    {
                        manualNo = "opng";
                    }
                    if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
                    {
                        realizeAmount += OfferAmount;
                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                        ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                         Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                         ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

                        //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // For Cross Payment Suppleir
                        //{
                        //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

                        //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
                        //}
                        //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3") // For Cross Payment Customer
                        //{
                        //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

                        //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
                        //}

                        OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + OfferAmount + " where SALE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), con))
                            {
                                command.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        break;
                    }
                    else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
                    {
                        // string date = Convert.ToDateTime(txtTransactionDate.Text + " 23:59:59").ToString("dd/MM/yyyy");
                        realizeAmount += decimal.Parse(dr.Cells[4].Text);
                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                         ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                        ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

                        //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // For Cross Payment Suppleir
                        //{
                        //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

                        //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
                        //}
                        //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3") // For Cross Payment Customer
                        //{
                        //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

                        //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
                        //}

                        OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {

                            ////, SALE_CLOSED = " + 1 + "  /// work for sale return close
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + 0 + " where SALE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["ITEM_LOG_ID"]), con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                }
            }
            //Advance Entry for sale return
            if (realizeAmount < decimal.Parse(txtAmount.Text))
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                      Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);

                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                      Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
            }
            InsertGL2(ChequeNo, MaxDocumentId, null, null);
            #endregion
        }
    }
    private void ChequeRealizationHWL(int p_TYPE)
    {
        #region General Veriables
        DbCalls db = new DbCalls();
        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        string MaxDocumentIdForCross = "";
        string Item_Name = ""; //Session["ITEM_NAME"].ToString();
        string Customer_Name = DrpVendor.SelectedItem.Text; //Session["Customer_Name"].ToString();
        int VoucherType = 0;
        int TransactionType = 1;
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal OfferAmount2 = decimal.Parse(txtAmount.Text);
        string remarks = "";
        string remarks2 = "";
        int ChequeType = Constants.IntNullValue;
        string Favouring = "";
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        Session.Add("VoucherNo", MaxDocumentId);
        Session.Add("CrossVoucherNo", MaxDocumentIdForCross);
        string ChequeNo = null;
        DateTime ChequeDate = Constants.DateNullValue;
        #endregion
        #region Set Payments Types
        if (DrpAccountType.SelectedItem.Value.ToString() == "46")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        VoucherType = Constants.HWL_Voucher;
                    }
                }
            }
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                        {
                            remarks = "HWL Payment, "  + Customer_Name + " to " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                            remarks2 = "HWL Payment, "  + Customer_Name + " to " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                        }
                    }
                }
            }
        }

        #endregion

        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") //For Sale
        {
            if (Hp.SelectedValue != "2")
            {
                #region For Sale
                int CrossMasterID = 0;
                bool isadvanceentered = false;
                IDbConnection mConnection = null;
                IDbTransaction mTransaction = null;
                try
                {
                    mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                    mConnection.Open();
                    mTransaction = ProviderFactory.GetTransaction(mConnection);

                    #region Complete Transaction

                    #region HWL Payment to Vendor Advance
                    if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                    {
                        #region For Cross Payment to Vendor
                        if (CrossMasterID == 0)
                        {
                            if (!isadvanceentered)
                            {
                                #region Advance Receipt
                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                #endregion


                                #region Advance Payment
                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                isadvanceentered = true;
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                    {
                        #region For Cross Payment to Customer
                        if (CrossMasterID == 0)
                        {
                            if (!isadvanceentered)
                            {
                                #region Advance Recipt
                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                #endregion

                                #region Advance Payment
                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                isadvanceentered = true;
                                #endregion
                            }
                        }
                        #endregion
                    }

                    #endregion
                    #endregion

                    // GL VOUCHER ENTRY
           
                        string ChequeNo2 = null;
                        InsertGL_HWL(ChequeNo2, MaxDocumentId, mConnection, mTransaction);
                    mTransaction.Commit();
                }
                catch (Exception exp)
                {
                    mTransaction.Rollback();
                    ExceptionPublisher.PublishException(exp);
                }
                finally
                {
                    if (mConnection != null && mConnection.State == ConnectionState.Open)
                    {
                        mConnection.Close();
                    }
                }
                PrintVoucher(MaxDocumentId);

                #endregion
            }
            else // For Installments Sale
            {
                #region For Installments Sale
                int CrossMasterID = 0;
                int ITEM_LOG_ID = 0;
                bool isadvanceentered = false;
                bool IsEnteredBeforeVendor = false;
                bool IsEnteredBeforeCustomer = false;
                IDbConnection mConnection = null;
                IDbTransaction mTransaction = null;
                try
                {
                    mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                    mConnection.Open();
                    mTransaction = ProviderFactory.GetTransaction(mConnection);

                    
                        #region Complete Transaction

                        #region HWL Payment to Vendor Advance
                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  // For Cross Payment to Vendor
                        {
                            #region For Cross Payment to Vendor
                            if (CrossMasterID == 0)
                            {
                                if (!isadvanceentered)
                                {
                                    #region Advance Payment
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                      Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                    isadvanceentered = true;
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  // For Cross Payment to Customer
                        {
                            #region For Cross Payment to Customer
                            if (CrossMasterID == 0)
                            {
                                if (!isadvanceentered)
                                {
                                    #region Advance Recipt
                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                    Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                    ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);

                                    LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                                    isadvanceentered = true;
                                    #endregion
                                }
                            }
                            #endregion
                        }

                    #endregion
                    #endregion

                    // GL VOUCHER ENTRY
                    if (p_TYPE == 1)
                    {
                        InsertGL(MaxDocumentId, mConnection, mTransaction);
                    }
                    else
                    {
                        string ChequeNo2 = null;
                        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                        {
                            ChequeNo2 = txtChequeNumber.Text;
                        }
                        InsertGL2(ChequeNo2, MaxDocumentId, mConnection, mTransaction);
                    }
                    mTransaction.Commit();
                }
                catch (Exception exp)
                {
                    mTransaction.Rollback();
                    ExceptionPublisher.PublishException(exp);
                }
                finally
                {
                    if (mConnection != null && mConnection.State == ConnectionState.Open)
                    {
                        mConnection.Close();
                    }
                }
                PrintVoucher(MaxDocumentId);

                #endregion
            }
        }
        else  //For Sale Return
        {
            #region Sales Returns Payments
            #region
            //foreach (GridViewRow dr in GrdCreditReturn.Rows)
            //{
            //    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //    if (chRelized.Checked == true)
            //    {
            //        string manualNo = dr.Cells[1].Text;
            //        DocumentNo = dr.Cells[1].Text;
            //        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
            //        {
            //            ChequeNo = drpCheque.SelectedItem.Text;
            //        }
            //        if (Convert.ToString(dr.Cells[1].Text) == "opng")
            //        {
            //            manualNo = "opng";
            //        }
            //        if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
            //        {
            //            realizeAmount += OfferAmount;
            //            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //            ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

            //            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //             Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

            //            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // For Cross Payment Suppleir
            //            //{
            //            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

            //            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
            //            //}
            //            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3") // For Cross Payment Customer
            //            //{
            //            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

            //            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
            //            //}

            //            OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
            //            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            //            {
            //                con.Open();
            //                using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + OfferAmount + " where SALE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), con))
            //                {
            //                    command.ExecuteNonQuery();
            //                }
            //                con.Close();
            //            }
            //            break;
            //        }
            //        else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
            //        {
            //            // string date = Convert.ToDateTime(txtTransactionDate.Text + " 23:59:59").ToString("dd/MM/yyyy");
            //            realizeAmount += decimal.Parse(dr.Cells[4].Text);
            //            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
            //            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

            //            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
            //            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //            ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, Constants.Document_SaleReturn, Favouring, ChequeType);

            //            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // For Cross Payment Suppleir
            //            //{
            //            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

            //            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
            //            //}
            //            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3") // For Cross Payment Customer
            //            //{
            //            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);

            //            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
            //            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate);
            //            //}

            //            OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
            //            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            //            {

            //                ////, SALE_CLOSED = " + 1 + "  /// work for sale return close
            //                con.Open();
            //                using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + 0 + " where SALE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), con))
            //                {
            //                    command.ExecuteNonQuery();
            //                }
            //                using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = " + 0 + "where ITEM_LOG_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["ITEM_LOG_ID"]), con))
            //                {
            //                    command2.ExecuteNonQuery();
            //                }
            //                con.Close();
            //            }
            //        }
            //    }
            //}
            ////Advance Entry for sale return
            //if (realizeAmount < decimal.Parse(txtAmount.Text))
            //{
            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
            //                          Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);

            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
            //                          Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
            //                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
            //}
            //InsertGL2(ChequeNo, MaxDocumentId, null, null);
            #endregion
            #endregion

        }
    }
    protected void GrdCheque_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        ChequeEntryController CController = new ChequeEntryController();
        CController.DeleteChequeEntry(long.Parse(GrdCheque.Rows[e.RowIndex].Cells[0].Text));
        LoadReceviedCheque();
    }
    protected void GrdCheque_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            hfChequeNo.Value = "";
            ChequeEntryController Ccontroller = new ChequeEntryController();

            int HPValue = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[10].Text);
            if (HPValue != 1)
            {
                Hp.SelectedValue = "2";
                CustomerItems();
                SelectCreditInvoice();
                PanelItem.Visible = true;

            }
            else
            {
                Hp.SelectedValue = "1";
                SelectCreditInvoice();
                PanelItem.Visible = false;
            }
            int TType = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[11].Text);
            if (TType == 18 || TType == 45)
            {
                PaymentType();
                if (TType == 18)
                {
                    DrpAccountType.SelectedIndex = 0;
                }
                else
                {
                    DrpAccountType.SelectedIndex = 4;
                }

                toggleControls(TType.ToString());
            }
            else
            {
                PaymentType();
                DrpAccountType.SelectedIndex = 3;
                toggleControls(TType.ToString());
            }
            HFChqueProcessId.Value = GrdCheque.Rows[e.NewEditIndex].Cells[0].Text;
            //ddlAccountHead.Value = GrdCheque.Rows[e.NewEditIndex].Cells[13].Text;
            DrpVendor.Value = GrdCheque.Rows[e.NewEditIndex].Cells[1].Text;
            ddlCrossPAyment.Value = GrdCheque.Rows[e.NewEditIndex].Cells[14].Text;
            if (ddlCrossPAyment.Value.ToString() == "2")
            {
                // Supplier
                Vendor();
                DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                CrossPanel1.Visible = true;
            }
            else if (ddlCrossPAyment.Value.ToString() == "3")
            {
                // Customer
                Customers(int.Parse(GrdCheque.Rows[e.NewEditIndex].Cells[1].Text));
                DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                CrossPanel1.Visible = true;
            }
            else
            {
                LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
                DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                CrossPanel1.Visible = false;
            }

            LoadCheque(5);
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.Value.ToString() == "3") // For Sale & Purchase Return
            {
                txtChequeNumber.Text = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
            }
            else
            {
                drpCheque.SelectedIndex = drpCheque.Items.IndexOf(drpCheque.Items.FindByText(GrdCheque.Rows[e.NewEditIndex].Cells[3].Text.ToString()));
            }
            txtStartDate.Text = GrdCheque.Rows[e.NewEditIndex].Cells[5].Text;
            txtReceivedDate.Text = GrdCheque.Rows[e.NewEditIndex].Cells[6].Text;
            string Amount = GrdCheque.Rows[e.NewEditIndex].Cells[7].Text;
            decimal AMOUNT = Math.Round(decimal.Parse(Amount), 0);
            txtAmount.Text = AMOUNT.ToString();
            txtRemarks.Text = GrdCheque.Rows[e.NewEditIndex].Cells[8].Text.Replace("&nbsp;", "");
            //ChkIsDiscount.Checked = Convert.ToBoolean(GrdCheque.Rows[e.NewEditIndex].Cells[11].Text);
            //txtDiscount.Text = GrdCheque.Rows[e.NewEditIndex].Cells[10].Text.Replace("&nbsp;", "");
            //txtTax.Text = GrdCheque.Rows[e.NewEditIndex].Cells[12].Text.Replace("&nbsp;", "");
            btnSave.Text = "Update";
            SelectCreditInvoice();
            if (ddlTransactionType.Value.ToString() == "13" || ddlTransactionType.Value.ToString() == "3") // For Sale & Purchase Return
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
                {
                    DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                    if (Hp.SelectedValue != "2")
                    {
                        // For Normal Sale
                        foreach (GridViewRow dr in GrdCredit.Rows)
                        {
                            foreach (DataRow dbr in dt.Rows)
                            {
                                if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 13) // For Sale
                                {
                                    if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]) == Convert.ToInt64(dbr["SALE_INVOICE_ID"]))
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        chRelized.Checked = true;
                                    }
                                }
                                else // For Purchase Return
                                {
                                    if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        chRelized.Checked = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // For Installment Sale
                        foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                        {
                            foreach (DataRow dbr in dt.Rows)
                            {
                                if (Convert.ToInt32(dr.Cells[8].Text) == 13) // For Sale
                                {
                                    if (Convert.ToInt32(dr.Cells[2].Text) == Convert.ToInt32(dbr["SALE_INS_DETAIL_ID"]))
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        chRelized.Checked = true;
                                    }
                                }
                                else // For Purchase Return
                                {
                                    //if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                                    //{
                                    //    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    //    chRelized.Checked = true;
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            else // For Sale Return
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
                {
                    DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                    {
                        foreach (DataRow dbr in dt.Rows)
                        {
                            if (Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]) == Convert.ToInt64(dbr["SALE_RETURN_ID"]))
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                chRelized.Checked = true;
                            }
                        }
                    }
                }
            }

            hfChequeNo.Value = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text.ToString();
            txtFavouring.Text = GrdCheque.Rows[e.NewEditIndex].Cells[12].Text.Replace("&nbsp;", "");
            txtBankName.Text = GrdCheque.Rows[e.NewEditIndex].Cells[13].Text.Replace("&nbsp;", "");
            DrpStatus.Enabled = true;
            ddlTransactionType.Enabled = false;
            Hp.Enabled = false;
            DrpAccountType.Enabled = false;
            DrpVendor.Enabled = false;
            ddlItem.Enabled = false;
            drpDistributor.Enabled = false;
            ddlCrossPAyment.Enabled = false;
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Invoice not found for selected cheque');", true);
        }
    }
    #region Sel/Index Change
    protected void DrpVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "2")
        {
            SelectCreditInvoice();
            LoadReceviedCheque();
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                int CustomerID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                Customers(CustomerID);
                DrpBankAccount.SelectedIndex = 0;
                int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                LoadCrossSaleInvoices(ID);
                CrossPanel1.Visible = true;
            }
        }
        else
        {
            CustomerItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
        }

    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {

        SelectCreditInvoice();
        LoadReceviedCheque();
    }
    protected void DrpStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (btnSave.Text == "Save")
        {
            LoadReceviedCheque();
        }
    }
    protected void DrpBankAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (btnSave.Text == "Save")
            {
                LoadReceviedCheque();
            }
            LoadCheque(2);
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
            {
                int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                LoadCrossSupplierInvoices(ID);
                CrossPanel1.Visible = true;
                LoadReceviedCheque();
            }
            else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                LoadCrossSaleInvoices(ID);
                CrossPanel1.Visible = true;
                LoadReceviedCheque();
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
    }
    protected void DrpAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
        toggleControls(DrpAccountType.SelectedItem.Value.ToString());
        SelectCreditInvoice();
        LoadReceviedCheque();
    }
    #endregion
    #region Click
    private bool InvalidDate()
    {
        //DateTime Date = Constants.DateNullValue;
        //string TransactionDate = txtStartDate.Text.Replace("-", "/");
        //Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        //if (DrpAccountType.SelectedItem.Value.ToString() == "18")
        //{
        //    if (DrpStatus.SelectedItem.Value.ToString() == "527")
        //    {
        //        DateTime CurruntDate = Convert.ToDateTime(Session["CurrentWorkDate"]);
        //        if (Convert.ToDateTime(ConvertDate.British_To_American(TransactionDate)) < CurruntDate)
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Invalid Cheque Date');", true);

        //            return false;
        //        }
        //    }

        //}
        return true;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Session["ITEM_NAME"] = null;
        Session["Customer_Name"] = null;
        Session["SALE_RETURN_ID"] = null;
        Session["PURCHASE_DETAIL_ID"] = null;
        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
        {
            if (DrpVendor.SelectedItem.Value == DrpBankAccount.SelectedItem.Value)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Cross Payment can not be done with same Customer);", true);
                return;
            }
        }
        try
        {
            ChequeEntryController CController = new ChequeEntryController();
            DateTime ChequeDate;
            #region Date Formate
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                try
                {
                    if (txtStartDate.Text.Length == 10)
                    {
                        DateTime Date = Constants.DateNullValue;
                        string TransactionDate = txtStartDate.Text.Replace("-", "/");
                        ChequeDate = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                    }
                    else
                    {
                        ChequeDate = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Correct Cheque Date Pattern is DD/MM/YYYY.');", true);
                    return;
                }
            }
            else
            {
                ChequeDate = DateTime.Now;
            }
            #endregion
            int InvoiceCount = Constants.IntNullValue;
            int CrossInvoiceCount = Constants.IntNullValue;
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale
            {
                if (Hp.SelectedValue != "2")
                {
                    #region Normal Sale
                    foreach (GridViewRow dr in GrdCredit.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            InvoiceCount++;
                            Session["ITEM_NAME"] = dr.Cells[2].Text;
                            Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                            break;
                        }
                    }
                    if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                    {
                        foreach (GridViewRow dr in CrossGrdCredit.Rows)
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            if (chRelized.Checked == true)
                            {
                                CrossInvoiceCount++;
                                Session["SALE_RETURN_ID"] = null;
                                Session["PURCHASE_DETAIL_ID"] = null;
                                break;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Installment Sale
                    //if (DrpAccountType.SelectedItem.Value.ToString() == "21")
                    //{
                    foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            InvoiceCount++;
                            Session["ITEM_NAME"] = ddlItem.SelectedItem.Text;
                            Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                            break;
                        }
                    }
                    if (InvoiceCount == Constants.IntNullValue)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an installment');", true);
                        return;
                    }
                    //}
                    #endregion
                }
            }
            else // For Sale Return
            {
                #region Sales Return
                if (Hp.SelectedValue != "2")
                {
                    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            InvoiceCount++;
                            Session["ITEM_NAME"] = dr.Cells[2].Text;
                            Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                            break;
                        }
                    }
                    if (InvoiceCount == Constants.IntNullValue)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an invoice');", true);
                        return;
                    }
                }
                else
                {
                    //if (ddlPaymentDesc.SelectedItem.Value.ToString() == "1")
                    //{
                    //}
                }
                #endregion
            }
            if (Hp.SelectedValue != "2")
            {
                if (InvalidDate())
                {
                    if (btnSave.Text == "Save")
                    {
                        #region Save Transaction
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale And Purchase Return
                        {
                            #region For Sale And Purchase Return
                            if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 2 || DrpAccountType.SelectedIndex == 5 || DrpAccountType.SelectedIndex == 6) // On Cash Realization check Invoice Selection for Cash, Online & JV
                            {
                                #region Cash Realization
                                foreach (GridViewRow dr in GrdCredit.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        InvoiceCount++;
                                        Session["ITEM_NAME"] = dr.Cells[2].Text;
                                        Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                                        break;
                                    }
                                }
                                if (InvoiceCount == Constants.IntNullValue && DrpAccountType.SelectedIndex != 5)
                                {
                                    Session["ITEM_NAME"] = "";
                                    Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                                    if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale Only
                                    {
                                        CashAdvance(2);
                                    }
                                    Session.Remove("VoucherNo");
                                }
                                else if (InvoiceCount != Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                                {
                                    ChequeRealization(2); // used as All cash, online, cheque
                                    Session.Remove("VoucherNo");
                                    SelectCreditInvoice();
                                }
                                else if (InvoiceCount == Constants.IntNullValue && DrpAccountType.SelectedIndex == 5)
                                {
                                    ChequeRealizationHWL(2);
                                    Session.Remove("VoucherNo");
                                    SelectCreditInvoice();
                                }
                                #endregion
                            }
                            else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4) // On Cheque Realization check Invoice Selection
                            {
                                #region Cheque Realization

                                checkDuplication();
                                if (DrpStatus.SelectedIndex == 0)
                                {
                                    short chkDiscount = 0;
                                    if (ChkIsDiscount.Checked)
                                    {
                                        chkDiscount = 1;
                                    }
                                    string Favouring = txtFavouring.Text;
                                    int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
                                    DateTime Date = Constants.DateNullValue;
                                    string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
                                    Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                                    int Customer_ID = 0;
                                    int Vendor_ID = 0;
                                    if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale 
                                    {
                                        Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                        Vendor_ID = 0;
                                    }
                                    else if (ddlTransactionType.SelectedItem.Value.ToString() == "3") // Purchase Return
                                    {
                                        Customer_ID = 0;
                                        Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                    }
                                    HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, txtChequeNumber.Text, txtBankName.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now
                                       , int.Parse(Session["UserId"].ToString()), Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1, Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text))
                                       , short.Parse(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Favouring, ChequeType, 0, "");

                                    foreach (GridViewRow dr in GrdCredit.Rows)
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        if (chRelized.Checked == true)
                                        {
                                            if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 13) // For Sale
                                            {
                                                CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue
                                                    , Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                            }
                                            else // For Purchase Return
                                            {
                                                CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                                    , Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                            }
                                        }
                                    }
                                    //PrintChequeVoucher(HFChqueProcessId.Value);
                                }
                                SelectCreditInvoice();
                                txtChequeNumber.Text = "";
                                #endregion
                            }
                            #endregion
                        }
                        else // For Sale Return
                        {
                            #region For Sale Return
                            DateTime Date = Constants.DateNullValue;
                            string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
                            Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                            if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 2) //on Cash Realization check Invoice Selection
                            {
                                #region Cash Realization
                                foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        InvoiceCount++;
                                        Session["ITEM_NAME"] = dr.Cells[2].Text;
                                        Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                                        break;
                                    }
                                }
                                //if (InvoiceCount == Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                                //{
                                //    CashAdvance();
                                //    //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                                //    //{
                                //        if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                                //        {
                                //            InsertGL2(null);
                                //        }
                                //    //}

                                //    Session.Remove("VoucherNo");
                                //}
                                //else 
                                if (InvoiceCount != Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                                {
                                    ChequeRealization(2);// used as All cash, online, cheque
                                                         //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                                                         //{
                                                         //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                                                         //{
                                                         //    string ChequeNo = null;
                                                         //    if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                                                         //    {
                                                         //        ChequeNo = txtChequeNumber.Text;
                                                         //    }
                                                         //InsertGL2(ChequeNo);
                                                         //}
                                                         //}
                                    Session.Remove("VoucherNo");
                                    SelectCreditInvoice();
                                }
                                #endregion
                            }
                            else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)//on Cash Realization check Invoice Selection
                            {
                                #region Cheque Realization
                                checkDuplication();
                                if (DrpStatus.SelectedIndex == 0)
                                {
                                    short chkDiscount = 0;
                                    if (ChkIsDiscount.Checked)
                                    {
                                        chkDiscount = 1;
                                    }
                                    string Favouring = txtFavouring.Text;
                                    int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
                                    HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, int.Parse(DrpVendor.SelectedItem.Value.ToString()), drpCheque.SelectedItem.Text, txtBankName.Text, ChequeDate,
                                        Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                                        , "", txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1
                                        , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount, Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString())
                                        , Favouring, ChequeType, 0, "");
                                    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        if (chRelized.Checked == true)
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                        }
                                    }
                                    //                     PrintChequeVoucher(HFChqueProcessId.Value);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region Update Transaction
                        Session["ITEM_NAME"] = "";
                        Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                        DateTime Date = Constants.DateNullValue;
                        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
                        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
                        {
                            #region Update Sales Cheque
                            int Customer_ID = 0;
                            int Vendor_ID = 0;
                            if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale 
                            {
                                Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                Vendor_ID = 0;
                            }
                            else if (ddlTransactionType.SelectedItem.Value.ToString() == "3") // Purchase Return
                            {
                                Customer_ID = 0;
                                Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                            }
                            if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == DateTime.Parse(txtTransactionDate.Text).ToString("dd-MM-yyyy"))
                            {
                                short chkDiscount = 0;
                                if (ChkIsDiscount.Checked)
                                {
                                    chkDiscount = 1;
                                }
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, txtChequeNumber.Text, txtBankName.Text
                                    , ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount
                                    , Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), "");

                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                foreach (GridViewRow dr in GrdCredit.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 13) // For Sale
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue
                                                , Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                        }
                                        else // For Purchase Return
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                                , Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                        }
                                        //CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue);
                                    }
                                }
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text
                                    , long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text
                                    , long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");

                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                Session["CHEQUE_PROCESS_IDs"] = HFChqueProcessId.Value;
                                foreach (GridViewRow dr in GrdCredit.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 13) // For Sale
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue
                                                , Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                        }
                                        else // For Purchase Return
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                                , Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                        }
                                        //CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue);
                                    }
                                }
                                ChequeRealization(1);
                                ////if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                                ////{
                                //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                                //{
                                //    InsertGL();
                                //}
                                ////}

                                Session.Remove("VoucherNo");
                                SelectCreditInvoice();
                            }
                            txtChequeNumber.Text = "";
                            #endregion
                        }
                        else  // For Sale Return
                        {
                            #region Update Sales Return Cheque
                            DataTable dt = _bnkController.SelectCheckBook(Convert.ToInt32(drpCheque.SelectedItem.Value.ToString()), Constants.IntNullValue, null, null, null, true, Constants.DateNullValue, 4);
                            if (dt.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) == 0 && DrpStatus.SelectedIndex != 0)
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected cheque is not issued yet');", true);
                                    return;
                                }
                            }
                            if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == txtTransactionDate.Text)
                            {
                                short chkDiscount = 0;
                                if (ChkIsDiscount.Checked)
                                {
                                    chkDiscount = 1;
                                }

                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, int.Parse(DrpVendor.SelectedItem.Value.ToString()), drpCheque.SelectedItem.Text, DrpBankAccount.SelectedItem.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue,
                                   Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                                   , txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                   , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount, Convert.ToDecimal(dc.chkNull_0(txtTax.Text))
                                   , Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), "");
                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                    }
                                }
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, drpCheque.SelectedItem.Text, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
                                                                 Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                                                                 , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                                                 , Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue
                                                                 , Convert.ToString(hfChequeNo.Value), "");
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, drpCheque.SelectedItem.Text, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
                                                                 Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                                                                 , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                                                 , Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue
                                                                 , Convert.ToString(hfChequeNo.Value), "");
                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                Session["CHEQUE_PROCESS_IDs"] = HFChqueProcessId.Value;
                                foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                    }
                                }
                                ChequeRealization(1);
                                ////if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                                ////{
                                //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                                //{
                                //    InsertGL();
                                //}
                                ////}
                                Session.Remove("VoucherNo");
                                SelectCreditInvoice();
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            else // Installments
            {
                #region Installments Realization
                if (InvalidDate())
                {
                    if (btnSave.Text == "Save")
                    {
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale
                        {
                            #region Check Is Installment Selected
                            foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    InvoiceCount++;
                                    Session["ITEM_NAME"] = ddlItem.SelectedItem.Text;
                                    Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                                    break;
                                }
                            }
                            if (InvoiceCount == Constants.IntNullValue)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an installment');", true);
                                return;
                            }
                            #endregion

                            if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 2 || DrpAccountType.SelectedIndex == 5) // On Cash Realization check Invoice Selection for Cash, Online & JV
                            {
                                #region Cash Realization 
                                if (InvoiceCount != Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                                {
                                    ChequeRealization(2); // used as All cash, online, cheque
                                    Session.Remove("VoucherNo");
                                    CustomerItems();
                                    LoadSaleInstallments(Convert.ToInt32(ddlItem.SelectedItem.Value));
                                }
                                #endregion
                            }
                            else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4) // On Cheque Realization check Invoice Selection
                            {
                                #region Cheque Realization
                                checkDuplication();
                                if (DrpStatus.SelectedIndex == 0)
                                {
                                    short chkDiscount = 0;
                                    if (ChkIsDiscount.Checked)
                                    {
                                        chkDiscount = 1;
                                    }
                                    string Favouring = txtFavouring.Text;
                                    int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
                                    DateTime Date = Constants.DateNullValue;
                                    string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
                                    Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                                    int Customer_ID = 0;
                                    int Vendor_ID = 0;
                                    if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale 
                                    {
                                        Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                        Vendor_ID = 0;
                                    }
                                    else if (ddlTransactionType.SelectedItem.Value.ToString() == "3") // Purchase Return
                                    {
                                        Customer_ID = 0;
                                        Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                    }
                                    HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, txtChequeNumber.Text, txtBankName.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now
                                       , int.Parse(Session["UserId"].ToString()), Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1, Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text))
                                       , short.Parse(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Favouring, ChequeType, 0, "");

                                    foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        if (chRelized.Checked == true)
                                        {
                                            if (Convert.ToInt32(dr.Cells[8].Text) == 13) // For Sale
                                            {
                                                CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue
                                                    , Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt32(dr.Cells[2].Text));
                                            }
                                            else // For Purchase Return
                                            {
                                                //CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                                //    , Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                            }
                                        }
                                    }
                                    //PrintChequeVoucher(HFChqueProcessId.Value);
                                }
                                SelectCreditInvoice();
                                txtChequeNumber.Text = "";
                                #endregion
                            }
                        }
                        else // For Sale Return
                        {

                        }
                    }
                    else
                    {
                        #region Update Transaction
                        Session["ITEM_NAME"] = "";
                        Session["Customer_Name"] = DrpVendor.SelectedItem.Text;
                        DateTime Date = Constants.DateNullValue;
                        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
                        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
                        {
                            #region Update Sales Cheque
                            int Customer_ID = 0;
                            int Vendor_ID = 0;
                            if (ddlTransactionType.SelectedItem.Value.ToString() == "13") // For Sale 
                            {
                                Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                                Vendor_ID = 0;
                            }
                            else if (ddlTransactionType.SelectedItem.Value.ToString() == "3") // Purchase Return
                            {
                                Customer_ID = 0;
                                Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                            }
                            if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == DateTime.Parse(txtTransactionDate.Text).ToString("dd-MM-yyyy"))
                            {
                                short chkDiscount = 0;
                                if (ChkIsDiscount.Checked)
                                {
                                    chkDiscount = 1;
                                }
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, txtChequeNumber.Text, txtBankName.Text
                                    , ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount
                                    , Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), "");

                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        if (Convert.ToInt32(dr.Cells[8].Text) == 13) // For Sale
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue
                                                , Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt32(dr.Cells[2].Text));
                                        }
                                        else // For Purchase Return
                                        {
                                            //CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                            //    , Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                        }
                                    }
                                }
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text
                                    , long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");
                            }
                            else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                            {
                                CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()), txtRemarks.Text
                                    , long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");

                                CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                                Session["CHEQUE_PROCESS_IDs"] = HFChqueProcessId.Value;
                                foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        if (Convert.ToInt32(dr.Cells[8].Text) == 13) // For Sale
                                        {
                                            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue
                                                , Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt32(dr.Cells[2].Text));
                                        }
                                        else // For Purchase Return
                                        {
                                            //CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue
                                            //    , Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.IntNullValue);
                                        }
                                    }
                                }
                                ChequeRealization(1);
                                Session.Remove("VoucherNo");
                                SelectCreditInvoice();
                            }
                            txtChequeNumber.Text = "";
                            #endregion
                        }
                        else  // For Sale Return
                        {
                            #region Update Sales Return Cheque
                            //DataTable dt = _bnkController.SelectCheckBook(Convert.ToInt32(drpCheque.SelectedItem.Value.ToString()), Constants.IntNullValue, null, null, null, true, Constants.DateNullValue, 4);
                            //if (dt.Rows.Count > 0)
                            //{
                            //    if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) == 0 && DrpStatus.SelectedIndex != 0)
                            //    {
                            //        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected cheque is not issued yet');", true);
                            //        return;
                            //    }
                            //}
                            //if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == txtTransactionDate.Text)
                            //{
                            //    short chkDiscount = 0;
                            //    if (ChkIsDiscount.Checked)
                            //    {
                            //        chkDiscount = 1;
                            //    }

                            //    CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, int.Parse(DrpVendor.SelectedItem.Value.ToString()), drpCheque.SelectedItem.Text, DrpBankAccount.SelectedItem.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue,
                            //       Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                            //       , txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                            //       , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount, Convert.ToDecimal(dc.chkNull_0(txtTax.Text))
                            //       , Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), "");
                            //    CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            //    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            //    {
                            //        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            //        if (chRelized.Checked == true)
                            //        {
                            //            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                            //        }
                            //    }
                            //}
                            //else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                            //{
                            //    CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, drpCheque.SelectedItem.Text, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
                            //                                     Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                            //                                     , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                            //                                     , Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue
                            //                                     , Convert.ToString(hfChequeNo.Value), "");
                            //}
                            //else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                            //{
                            //    CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, drpCheque.SelectedItem.Text, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
                            //                                     Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                            //                                     , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                            //                                     , Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue
                            //                                     , Convert.ToString(hfChequeNo.Value), "");
                            //    CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            //    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            //    {
                            //        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            //        if (chRelized.Checked == true)
                            //        {
                            //            CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["SALE_RETURN_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                            //        }
                            //    }
                            //    ChequeRealization(1);
                            //    ////if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                            //    ////{
                            //    //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                            //    //{
                            //    //    InsertGL();
                            //    //}
                            //    ////}
                            //    Session.Remove("VoucherNo");
                            //    SelectCreditInvoice();
                            //}
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
        LoadCheque(2);
        ClearAll();
        LoadReceviedCheque();
        LoadPaymentRecieved();
        ddlCrossPAyment_SelectedIndexChanged(null, null);
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearAll();
        LoadCheque(2);
    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["dt"];
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
        {
            switch (ddSearchType.SelectedIndex)
            {
                case 1:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 3:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 4:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    dt.DefaultView.RowFilter = "CHEQUE_NO" + " like '%" + "" + "%'";
                    break;
            }
            GrdCheque.DataSource = dt.DefaultView;
            GrdCheque.DataBind();
        }
        else
        {
            switch (ddSearchType.SelectedIndex)
            {
                case 1:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                case 2:
                    dt.DefaultView.RowFilter = ddSearchType.SelectedItem.Value.ToString() + " like '%" + txtSeach.Text + "%'";
                    break;
                default:
                    dt.DefaultView.RowFilter = "CHEQUE_NO" + " like '%" + "" + "%'";
                    break;
            }
            GrdCO.DataSource = dt.DefaultView;
            GrdCO.DataBind();
        }
    }
    #endregion  
    private void InsertGL(string MaxDocumentId, IDbConnection mConnection, IDbTransaction mTransaction)
    {
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        string Item_Name = Session["ITEM_NAME"].ToString();
        string Customer_Name = Session["Customer_Name"].ToString();
        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
        {
            DataTable dtVoucher = new DataTable();
            dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
            dtVoucher.Columns.Add("DEBIT", typeof(decimal));
            dtVoucher.Columns.Add("CREDIT", typeof(decimal));
            dtVoucher.Columns.Add("REMARKS", typeof(string));
            dtVoucher.Columns.Add("Principal_Id", typeof(int));

            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Customer to Supplier
                {
                    #region Customer to Supplier
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher (Cross Payment Voucher for Supplier)
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                    #endregion
                }
                else  // Customer to Customer
                {
                    #region Customer to Customer
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher  (Cross Payment Voucher for Customer)
                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);

                    //Credit Side Entry
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    #endregion
                }
            }
            else
            {
                #region Normal Entry
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                dr["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = 0;
                dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["CREDIT"] = 0;
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
                #endregion
            }
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
            }
            else
            {
                string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);
            }
        }
        else // For Sale Return
        {
            //Configuration.GetAccountHead();
            DataTable dtVoucher = new DataTable();
            dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
            dtVoucher.Columns.Add("DEBIT", typeof(decimal));
            dtVoucher.Columns.Add("CREDIT", typeof(decimal));
            dtVoucher.Columns.Add("REMARKS", typeof(string));
            dtVoucher.Columns.Add("Principal_Id", typeof(int));
            DataRow[] drConfig = null;
            DataTable dtConfig = (DataTable)Session["dtConfig"];
            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.AccountPayable + "'");
            int PayableAccount = Convert.ToInt32(drConfig[0]["VALUE"].ToString());

            DataRow dr = dtVoucher.NewRow();

            dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
            dr["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dr["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr["CREDIT"] = 0;
            dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dtVoucher.Rows.Add(dr);
            //Debit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
            dr1["DEBIT"] = 0;
            dr1["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dr1["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
            LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, drpCheque.SelectedItem.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);
        }
        Session["ITEM_NAME"] = null;
        Session["Customer_Name"] = null;
        Session["CHEQUE_PROCESS_IDs"] = null;
    }
    private void InsertGL2(string ChequeNo, string MaxDocumentId, IDbConnection mConnection, IDbTransaction mTransaction)
    {
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        DateTime ChequeDate = Constants.DateNullValue;
        string Item_Name = Session["ITEM_NAME"].ToString();
        string Customer_Name = Session["Customer_Name"].ToString();
        try
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "45")
            {
                if (txtStartDate.Text.Length == 10)
                {
                    string StartDate = txtStartDate.Text.Replace("-", "/");
                    ChequeDate = DateTime.Parse(ConvertDate.British_To_American(StartDate));
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Correct Cheque Date Pattern is DD/MM/YYYY.');", true);
            return;
        }
        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(int));
        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Customer to Supplier
                {
                    #region Customer to Supplier
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher (Cross Payment Voucher for Supplier)
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                    #endregion
                }
                else  // Customer to Customer
                {
                    #region Customer to Customer
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher  (Cross Payment Voucher for Customer)

                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                    //Credit Side Entry
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    #endregion
                }
            }
            else
            {
                #region Normal Entry
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = 0;
                dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["CREDIT"] = 0;
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
                #endregion
            }

            if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "46" || DrpAccountType.SelectedItem.Value.ToString() == "47")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.HWL_Voucher, Date, Constants.HWL_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    else
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    else
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Cash_Voucher, Date, Constants.CashPayment_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                }
                //PrintVoucher(MaxDocumentId);
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                    //PrintVoucher(MaxDocumentIdForCross);
                }
                else
                {
                    string PROCESS_IDs = null;
                    if (Session["CHEQUE_PROCESS_IDs"] != null)
                    {
                        PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                    }
                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                }
            }
        }
        else // For Sale Return
        {
            DataRow dr = dtVoucher.NewRow();
            dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
            dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dr["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr["CREDIT"] = 0;
            dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dtVoucher.Rows.Add(dr);
            //Debit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
            dr1["DEBIT"] = 0;
            dr1["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            //string MaxDocumentId = "";
            //using chequeDate as Transfer Date
            if (DrpAccountType.SelectedItem.Value.ToString() == "21")
            {
                //MaxDocumentId = LController.SelectMaxVoucherId(Constants.CashPayment_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.CashPayment_Voucher, Date, Constants.CashPayment_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                PrintVoucher(MaxDocumentId);
            }
            else
            {
                string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                PrintVoucher(MaxDocumentId);
            }
        }
        Session["ITEM_NAME"] = null;
        Session["Customer_Name"] = null;
        Session["CHEQUE_PROCESS_IDs"] = null;
    }
    private void InsertGL_HWL(string ChequeNo, string MaxDocumentId, IDbConnection mConnection, IDbTransaction mTransaction)
    {
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        DateTime ChequeDate = Constants.DateNullValue;
        string Item_Name = ""; //Session["ITEM_NAME"].ToString();
        string Customer_Name = DrpVendor.SelectedItem.Text; //Session["Customer_Name"].ToString();
        DataTable dtVoucher = new DataTable();
        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(int));
        if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3") // For Sale & Purchase Return
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Customer to Supplier
                {
                    #region Customer to Supplier
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher (Cross Payment Voucher for Supplier)
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                    #endregion
                }
                else  // Customer to Customer
                {
                    #region Customer to Customer
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher  (Cross Payment Voucher for Customer)

                    //Debit Side Entry
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                    //Credit Side Entry
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr2);
                    #endregion
                }
            }
            else
            {
                #region Normal Entry
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = 0;
                dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["CREDIT"] = 0;
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Received from " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
                #endregion
            }

            if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "46" || DrpAccountType.SelectedItem.Value.ToString() == "47")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.HWL_Voucher, Date, Constants.HWL_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    else
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    else
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Cash_Voucher, Date, Constants.CashPayment_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                }
                //PrintVoucher(MaxDocumentId);
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                    //PrintVoucher(MaxDocumentIdForCross);
                }
                else
                {
                    string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Date, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                }
            }
        }
        else // For Sale Return
        {
            DataRow dr = dtVoucher.NewRow();
            dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
            dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dr["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr["CREDIT"] = 0;
            dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dtVoucher.Rows.Add(dr);
            //Debit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
            dr1["DEBIT"] = 0;
            dr1["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return to " + DrpVendor.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            //string MaxDocumentId = "";
            //using chequeDate as Transfer Date
            if (DrpAccountType.SelectedItem.Value.ToString() == "21")
            {
                //MaxDocumentId = LController.SelectMaxVoucherId(Constants.CashPayment_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.CashPayment_Voucher, Date, Constants.CashPayment_Voucher, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                PrintVoucher(MaxDocumentId);
            }
            else
            {
                string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Customer_Name + ", " + Item_Name + ", " + txtRemarks.Text, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                PrintVoucher(MaxDocumentId);
            }
        }
        Session["ITEM_NAME"] = null;
        Session["Customer_Name"] = null;
        Session["CHEQUE_PROCESS_IDs"] = null;
    }
    private void PrintVoucher(string VoucherNo)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();

        int VoucherType = Constants.IntNullValue;
        string VoucherType2 = "";
        string pVoucherType = "";
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                pVoucherType = "Journal Voucher";
                VoucherType = 16;
                VoucherType2 = "Journal Voucher";
            }
            else
            {
                pVoucherType = "Bank Voucher";
                VoucherType = 15;
                VoucherType2 = "Bank Receipt Voucher";
            }
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "47")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    pVoucherType = "Journal Voucher";
                    VoucherType = 16;
                    VoucherType2 = "Journal Voucher";
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                    {
                        pVoucherType = "Journal Voucher";
                        VoucherType = 16;
                        VoucherType2 = "Journal Voucher";
                    }
                    else
                    {
                        pVoucherType = "Cash Voucher";
                        VoucherType = 14;
                        VoucherType2 = "Cash Receipt Voucher";
                    }
                }
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    pVoucherType = "Journal Voucher";
                    VoucherType = 16;
                    VoucherType2 = "Journal Voucher";
                }
                else
                {
                    pVoucherType = "Cash Voucher";
                    VoucherType = 24;
                    VoucherType2 = "Cash Payment Voucher";
                }
            }
        }
        else
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13" || ddlTransactionType.SelectedItem.Value.ToString() == "3")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        pVoucherType = "HWL Voucher";
                        VoucherType = 22;
                        VoucherType2 = "HWL Voucher";
                    }
                    else
                    {
                        pVoucherType = "Journal Voucher";
                        VoucherType = 16;
                        VoucherType2 = "Journal Voucher";
                    }
                }
                else
                {
                    pVoucherType = "Bank Voucher";
                    VoucherType = 15;
                    VoucherType2 = "Bank Receipt Voucher";
                }
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    pVoucherType = "Journal Voucher";
                    VoucherType = 16;
                    VoucherType2 = "Journal Voucher";
                }
                else
                {
                    pVoucherType = "Bank Voucher";
                    VoucherType = 17;
                    VoucherType2 = "Bank Payment Voucher";
                }
            }
        }
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        //ds = RptAccountCtl.SelectUnpostVoucherForPrint_New(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, Constants.IntNullValue);
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, Constants.IntNullValue);
                CrpReport = new CORNBusinessLayer.Reports.crpVoucherViewSupplier();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();
            }
            else
            {
                ds = RptAccountCtl.SelectUnpostVoucherForPrintWithInstrument(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, null, 1);
                CrpReport = new CORNBusinessLayer.Reports.crpVoucherViewWithInstrument();
                ReportDocument SubReport = CrpReport.OpenSubreport("RptInstrument");
                CrpReport.SetDataSource(ds);
                SubReport.SetDataSource(ds);
                CrpReport.Refresh();
            }
        }
        else
        {
            ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, Constants.IntNullValue);
            CrpReport = new CORNBusinessLayer.Reports.crpVoucherViewSupplier();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
        }
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VoucherType", pVoucherType);
        CrpReport.SetParameterValue("VoucherSubType", VoucherType2);
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", 0);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    private void PrintChequeVoucher(string ChequeProcessId)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        CORNBusinessLayer.Reports.crpChequeVoucher CrpReport = new CORNBusinessLayer.Reports.crpChequeVoucher();
        string VoucherType2 = "";
        string pVoucherType = "";
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "13")
            {
                pVoucherType = "Bank Voucher";
                VoucherType2 = "Bank Receipt Voucher";
            }
            else
            {
                pVoucherType = "Bank Voucher";
                VoucherType2 = "Bank Payment Voucher";
            }
        }
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        if (Hp.SelectedValue == "2")
        {
            ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), null, Convert.ToInt32(ChequeProcessId), 2);
        }
        else
        {
            ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), null, Convert.ToInt32(ChequeProcessId), 1);
        }
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VoucherType", pVoucherType);
        CrpReport.SetParameterValue("VoucherSubType", VoucherType2);
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", 0);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    private void ClearAll()
    {
        txtChequeNumber.Text = "";
        txtAmount.Text = "";
        txtBankName.Text = "";
        txtStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        btnSave.Text = "Save";
        txtReceivedDate.Text = "";
        txtRemarks.Text = "";
        txtTax.Text = "";
        txtDiscount.Text = "";
        txtFavouring.Text = "";
        ChkIsDiscount.Checked = false;
        DrpStatus.SelectedIndex = 0;
        DrpStatus.Enabled = false;
        ddlTransactionType.Enabled = true;
        Hp.Enabled = true;
        DrpAccountType.Enabled = true;
        DrpVendor.Enabled = true;
        ddlItem.Enabled = true;
        drpDistributor.Enabled = true;
        ddlCrossPAyment.Enabled = true;
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
    protected void ddlTransactionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "2")
        {
            LoadData();
            PaymentType();
            CustomerItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            toggleControls(DrpAccountType.SelectedItem.Value.ToString());
            Panel3.Visible = false;

        }
        else
        {
            LoadData();
            PaymentType();
            CustomerItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = true;
            LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));


        }


    }
    protected void Hp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "2")
        {
            LoadData();
            SelectCreditInvoice();
            LoadReceviedCheque();
            PanelItem.Visible = false;
        }
        else
        {
            LoadData();
            CustomerItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            PanelItem.Visible = true;
        }

    }
    protected void ChkCrossPayment_CheckedChanged(object sender, EventArgs e)
    {
        //if (ChkCrossPayment.Checked)
        //{
        //    PanelCrossPayment.Visible = true;
        //    Vendor();
        //}
        //else
        //{
        //    PanelCrossPayment.Visible = false;
        //}
    }
    public void Vendor()
    {
        //DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 14);
        DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 5);
        if (dt.Rows.Count > 0)
        {
            DrpBankAccount.Items.Clear();
            DrpBankAccount.DataSource = dt;
            DrpBankAccount.TextField = "VENDOR_NAME";
            DrpBankAccount.ValueField = "VENDOR_ID";
            DrpBankAccount.DataBind();
            DrpBankAccount.SelectedIndex = 0;
        }
        else
        {
            DrpBankAccount.Items.Clear();
            DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            DrpBankAccount.SelectedIndex = 0;
        }
        //DbCalls db = new DbCalls();
        //int LocationID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
        //var query = (from k in db.VENDORS
        //             //join SM in db.tbl_PURCHASE_MASTER on k.VENDOR_ID equals SM.VENDOR_ID
        //             //join SR in db.tbl_PURCHASE_DETAIL on SM.PURCHASE_MASTER_ID equals SR.PURCHASE_MASTER_ID
        //             select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.DISTRIBUTOR_ID,k.CUSTOMER_TYPE }).Where(x => x.TYPE_ID == Constants.Vendor && x.DISTRIBUTOR_ID == LocationID
        //             || x.TYPE_ID == Constants.Customer && x.CUSTOMER_TYPE == 2 && x.DISTRIBUTOR_ID == LocationID).ToList();
        //if (query.Any())
        //{
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.DataSource = query;
        //    DrpBankAccount.TextField = "VENDOR_NAME";
        //    DrpBankAccount.ValueField = "VENDOR_ID";
        //    DrpBankAccount.DataBind();
        //    //DrpBankAccount.SelectedIndex = 0;
        //}
        //else
        //{
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //    DrpBankAccount.SelectedIndex = 0;
        //}
    }
    public void Customers(int CustomerID)
    {
        //DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 31);
        DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 4);
        //DataRow[] dtr = dt.Select("VENDOR_ID=" + CustomerID);
        //if (dtr.Any())
        //{
        //    foreach (var drow in dtr)
        //    {
        //        drow.Delete();
        //    }
        //    dt.AcceptChanges();
        //}
        if (dt.Rows.Count > 0)
        {
            DrpBankAccount.Items.Clear();
            DrpBankAccount.DataSource = dt;
            DrpBankAccount.TextField = "VENDOR_NAME";
            DrpBankAccount.ValueField = "VENDOR_ID";
            DrpBankAccount.DataBind();
            DrpBankAccount.SelectedIndex = 0;
        }
        else
        {
            DrpBankAccount.Items.Clear();
            DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            DrpBankAccount.SelectedIndex = 0;
        }
        //DbCalls db = new DbCalls();
        //int LocationID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
        //var query = (from k in db.VENDORS
        //             //join SM in db.tbl_SALE_MASTER on k.VENDOR_ID equals SM.CUSTOMER_ID
        //             //join SR in db.tbl_SALE_DETAIL on SM.SALE_MASTER_ID equals SR.SALE_MASTER_ID
        //             select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.DISTRIBUTOR_ID }).Where(x => x.TYPE_ID == Constants.Customer && x.DISTRIBUTOR_ID == LocationID).ToList();
        //query.RemoveAll(x => x.VENDOR_ID == CustomerID);
        //if (query.Any())
        //{
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.DataSource = query;
        //    DrpBankAccount.TextField = "VENDOR_NAME";
        //    DrpBankAccount.ValueField = "VENDOR_ID";
        //    DrpBankAccount.DataBind();
        //}
        //else
        //{
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //    DrpBankAccount.SelectedIndex = 0;
        //}
    }
    protected void ddlCrossPAyment_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
        {
            Vendor();
            DrpBankAccount.SelectedIndex = 0;
            int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
            LoadCrossSupplierInvoices(ID);
            CrossPanel1.Visible = true;
            LoadReceviedCheque();
        }
        else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
        {
            int CustomerID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            Customers(CustomerID);
            DrpBankAccount.SelectedIndex = 0;
            int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
            LoadCrossSaleInvoices(ID);
            CrossPanel1.Visible = true;
            LoadReceviedCheque();
        }
        else
        {
            LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
            DrpBankAccount.SelectedIndex = 0;
            CrossPanel1.Visible = false;
            LoadReceviedCheque();
        }
    }
    private void LoadCrossSaleInvoices(int ID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, ID, Constants.IntNullValue, Constants.IntNullValue, null, 21);
        CrossGrdCredit.DataSource = dt;
        CrossGrdCredit.DataBind();
        CheckBox cb = null;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in CrossGrdCredit.Rows)
        {
            cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
            //cb.Checked = true;
            TotalAmount += Convert.ToDecimal(gvr.Cells[4].Text);
        }
        if (dt.Rows.Count > 0)
        {
            txtTotalCreditAmount2.Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[3].Text = "Total";
            CrossGrdCredit.FooterRow.Cells[3].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[4].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[1].Visible = false;
            CrossGrdCredit.FooterRow.Cells[5].Visible = false;
            CrossGrdCredit.FooterRow.Cells[6].Visible = false;
            CrossGrdCredit.FooterRow.Cells[7].Visible = false;
            CrossGrdCredit.FooterRow.Cells[8].Visible = false;
        }

        //Panel1.Visible = true;
        //Panel2.Visible = false;
        //Panel3.Visible = false;
        //Session["SaleDetail"] = null;
    }
    public void LoadCrossSupplierInvoices(int ID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, ID, Constants.IntNullValue, Constants.IntNullValue, null, 22);
        CrossGrdCredit.DataSource = dt;
        CrossGrdCredit.DataBind();
        CheckBox cb = null;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in CrossGrdCredit.Rows)
        {
            cb = gvr.Cells[0].FindControl("ChbIsAssigned") as CheckBox;
            //cb.Checked = true;
            TotalAmount += Convert.ToDecimal(gvr.Cells[4].Text);
        }
        if (dt.Rows.Count > 0)
        {
            txtTotalCreditAmount2.Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[3].Text = "Total";
            CrossGrdCredit.FooterRow.Cells[3].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[4].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[1].Visible = false;
            CrossGrdCredit.FooterRow.Cells[5].Visible = false;
            CrossGrdCredit.FooterRow.Cells[6].Visible = false;
            CrossGrdCredit.FooterRow.Cells[7].Visible = false;
            CrossGrdCredit.FooterRow.Cells[8].Visible = false;
        }
    }
    protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
    {
        int ID = Convert.ToInt32(ddlItem.SelectedItem.Value);
        LoadSaleInstallments(ID);
    }
    public decimal ADVANCE;
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string VOUCHER_NO;
        int DOCUMENT_NO;
        int TRANSACTION_TYPE;
        int TYPE_ID;
        int VOUCHER_TYPE_ID;
        decimal DEBIT;
        decimal CREDIT;
        decimal Amount;

        var ProID = ((LinkButton)sender).CommandArgument;
        Customer_Ledger_Logics Ser = new Customer_Ledger_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, data.VOUCHER_NO, 15);
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                VOUCHER_NO = row["VOUCHER_NO"].ToString();
                DOCUMENT_NO = int.Parse(row["DOCUMENT_NO"].ToString());
                TRANSACTION_TYPE = int.Parse(row["TRANSACTION_TYPE"].ToString());
                TYPE_ID = int.Parse(row["TYPE_ID"].ToString());
                VOUCHER_TYPE_ID = int.Parse(row["VOUCHER_TYPE_ID"].ToString());
                DEBIT = decimal.Parse(row["DEBIT"].ToString());
                CREDIT = decimal.Parse(row["CREDIT"].ToString());
                var qry = dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == Constants.Document_Advance);
                if (!qry.Any())
                {
                    if (DEBIT > 0)
                    {
                        ADVANCE = DEBIT;
                    }
                    else
                    {
                        ADVANCE = CREDIT;
                    }
                }
                else
                {
                    ADVANCE = 0;
                }
                if (DEBIT > 0)
                {
                    Amount = DEBIT;
                }
                else
                {
                    Amount = CREDIT;
                }
                if (TRANSACTION_TYPE != 0)
                {
                    // DELETE CROSS PAYMENTS
                }
                else
                {
                    if (TYPE_ID == Constants.Document_SaleInvoice)
                    {
                        #region Document_SaleInvoice
                        SaleDetailLogics Per = new SaleDetailLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            if (DEBIT_AMOUNT > 0)
                            {
                                Amount = DEBIT_AMOUNT + Amount;
                                using (SqlCommand command3 = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT =" + Amount + "where SALE_DETAIL_ID=" + Purchase.SALE_DETAIL_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command3 = new SqlCommand("update tbl_SALE_DETAIL set DEBIT_AMOUNT =" + Amount + ", SALE_CLOSED = " + 0 + " where SALE_DETAIL_ID=" + Purchase.SALE_DETAIL_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            //var qry = dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == Constants.Document_Advance);
                            //if (!qry.Any())
                            //{
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.SALE_DETAIL_ID) + "and TYPE_ID=" + TYPE_ID, con))
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
                            //}
                            con.Close();
                        }
                        #endregion
                    }
                    else if (TYPE_ID == Constants.Document_Invoice_Installment)
                    {
                        #region Document_Invoice_Installment
                        DbCalls db = new DbCalls();
                        SaleInstallmentLogics Per = new SaleInstallmentLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);
                        var qryy = (from k in db.tbl_SALE_INS_DETAIL
                                    select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID, k.RECEIPT_NO }).Where(x => x.SALE_INS_MASTER_ID == DOCUMENT_NO && x.RECEIPT_NO == VOUCHER_NO).ToList();
                        int ReceivedAmount = Convert.ToInt32(Amount);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();

                            int firstentry = 0;
                            bool check = false;
                            for (int i = 0; i <= qryy.Count - 1; i++)
                            {
                                var query = (from k in db.tbl_SALE_INS_DETAIL
                                             select new { k.SALE_INS_MASTER_ID, k.SALE_INS_DETAIL_ID, k.DUE_AMOUNT, k.REC_AMOUNT, k.OS_AMOUNT, k.IS_PAID, k.DUE_DATE, k.RECEIPT_NO }).Where(x => x.SALE_INS_MASTER_ID == Purchase.SALE_INS_MASTER_ID && x.RECEIPT_NO == VOUCHER_NO).ToList();
                                int SALE_DETAIL_ID = Convert.ToInt32(query.First().SALE_INS_DETAIL_ID);
                                int DUE_AMOUNT = Convert.ToInt32(query.First().DUE_AMOUNT);
                                int OS_AMOUNT = Convert.ToInt32(query.First().OS_AMOUNT);
                                int REC_AMOUNT = Convert.ToInt32(query.First().REC_AMOUNT);
                                int RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                if (ReceivedAmount > REC_AMOUNT)
                                {
                                    if (firstentry != i)
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', REC_AMOUNT=" + Convert.ToDecimal(0.00) + ", OS_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", IS_PAID=" + 0 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        check = true;
                                    }
                                    else
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO = '', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', REC_AMOUNT=" + Convert.ToDecimal(0.00) + ", OS_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", IS_PAID=" + 0 + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        check = true;
                                    }
                                    ReceivedAmount = ReceivedAmount - REC_AMOUNT;
                                }
                                else
                                {
                                    int recv = 0;
                                    int Paid;
                                    int OS_Amount = REC_AMOUNT - ReceivedAmount;
                                    if (OS_Amount == 0)
                                    {
                                        recv = RECEIVED;
                                        Paid = 0;
                                    }
                                    else
                                    {
                                        recv = ReceivedAmount;
                                        Paid = 0;
                                    }
                                    if (!check)
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ",RECEIPT_NO='', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', REC_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", OS_AMOUNT=" + Convert.ToDecimal(recv) + ", IS_PAID=" + Paid + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_DETAIL set  RECEIPT_NO = '', PAYMENT_DATE ='" + Convert.ToDateTime(Constants.DateNullValue) + "', REC_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", OS_AMOUNT=" + Convert.ToDecimal(recv) + ", IS_PAID=" + Paid + " where SALE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        break;
                                    }
                                }
                            }
                            if (DEBIT_AMOUNT > 0)
                            {
                                Amount = DEBIT_AMOUNT + Amount;
                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_MASTER set DEBIT_AMOUNT = " + Amount + " where SALE_INS_MASTER_ID=" + Purchase.SALE_INS_MASTER_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command = new SqlCommand("update tbl_SALE_INS_MASTER set DEBIT_AMOUNT = " + Amount + ", SALE_CLOSED = " + 0 + " where SALE_INS_MASTER_ID=" + Purchase.SALE_INS_MASTER_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.SALE_INS_MASTER_ID) + "and TYPE_ID=" + TYPE_ID, con))
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
                        #endregion
                        CustomerItems();
                    }
                    else if (TYPE_ID == Constants.Document_SaleReturn)
                    {
                        #region Document_SaleReturn
                        SaleReturnLogics Per = new SaleReturnLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.CREDIT_AMOUNT);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            if (DEBIT_AMOUNT > 0)
                            {
                                Amount = DEBIT_AMOUNT + Amount;
                                using (SqlCommand command3 = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT =" + Amount + "where SALE_RETURN_ID=" + Purchase.SALE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command3 = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT =" + Amount + ", IS_DELETED = " + 0 + " where SALE_RETURN_ID=" + Purchase.SALE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            //var qry = dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == Constants.Document_Advance);
                            //if (!qry.Any())
                            //{
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.SALE_RETURN_ID) + "and TYPE_ID=" + TYPE_ID, con))
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
                            //}
                            con.Close();
                        }
                        #endregion
                    }
                    else if (TYPE_ID == Constants.Document_Opening)
                    {
                        //#region Document_Opening
                        //SaleMasterLogics Per = new SaleMasterLogics();
                        //var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        //decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);
                        //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        //{
                        //    con.Open();
                        //    if (DEBIT_AMOUNT > 0)
                        //    {
                        //        Amount = DEBIT_AMOUNT + Amount;
                        //        using (SqlCommand command3 = new SqlCommand("update tbl_SALE_MASTER set DEBIT_AMOUNT =" + Amount + "where SALE_MASTER_ID=" + Purchase.SALE_MASTER_ID, con))
                        //        {
                        //            command3.ExecuteNonQuery();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                        //        using (SqlCommand command3 = new SqlCommand("update tbl_SALE_MASTER set DEBIT_AMOUNT =" + Amount + ", SALE_CLOSED = " + 0 + " where SALE_MASTER_ID=" + Purchase.SALE_MASTER_ID, con))
                        //        {
                        //            command3.ExecuteNonQuery();
                        //        }
                        //    }
                        //    //var qry = dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == Constants.Document_Advance);
                        //    //if (!qry.Any())
                        //    //{
                        //    using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.SALE_MASTER_ID) + "and TYPE_ID=" + TYPE_ID, con))
                        //    {
                        //        command.ExecuteNonQuery();
                        //    }
                        //    using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                        //    {
                        //        command1.ExecuteNonQuery();
                        //    }
                        //    using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                        //    {
                        //        command2.ExecuteNonQuery();
                        //    }
                        //    //}
                        //    con.Close();
                        //}
                        //#endregion
                    }
                    if (TYPE_ID == Constants.InvoiceBooking)
                    {
                        //#region InvoiceBooking
                        //SaleMasterLogics Per = new SaleMasterLogics();
                        //var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        //decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);
                        //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        //{
                        //    con.Open();
                        //    if (DEBIT_AMOUNT > 0)
                        //    {
                        //        Amount = DEBIT_AMOUNT + Amount;
                        //        using (SqlCommand command3 = new SqlCommand("update tbl_SALE_MASTER set DEBIT_AMOUNT =" + Amount + "where SALE_MASTER_ID=" + Purchase.SALE_MASTER_ID, con))
                        //        {
                        //            command3.ExecuteNonQuery();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                        //        using (SqlCommand command3 = new SqlCommand("update tbl_SALE_MASTER set DEBIT_AMOUNT =" + Amount + ", SALE_CLOSED = " + 0 + " where SALE_MASTER_ID=" + Purchase.SALE_MASTER_ID, con))
                        //        {
                        //            command3.ExecuteNonQuery();
                        //        }
                        //    }
                        //    //var qry = dt.AsEnumerable().Where(x => x.Field<int>("TYPE_ID") == Constants.Document_Advance);
                        //    //if (!qry.Any())
                        //    //{
                        //    using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.SALE_MASTER_ID) + "and TYPE_ID=" + TYPE_ID, con))
                        //    {
                        //        command.ExecuteNonQuery();
                        //    }
                        //    using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                        //    {
                        //        command1.ExecuteNonQuery();
                        //    }
                        //    using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                        //    {
                        //        command2.ExecuteNonQuery();
                        //    }
                        //    //}
                        //    con.Close();
                        //}
                        //#endregion
                    }
                    else // For Advance Payments
                    {
                        #region Document_Advance
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();

                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(DOCUMENT_NO) + "and TYPE_ID=" + TYPE_ID, con))
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
                        #endregion
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record deleted successfully.');", true);
                    SelectCreditInvoice();
                    LoadReceviedCheque();
                }
            }
        }
    }
}