using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System.Linq;
using System.Data.SqlClient;
using CORNBusinessLayer.Logics;
using CORNDataAccessLayer.Classes;
using CrystalDecisions.CrystalReports.Engine;

public partial class Forms_frmChequeEntryVendorNew : System.Web.UI.Page
{
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    readonly ChequeEntryController CController = new ChequeEntryController();
    DataTable PurchaseDetail = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["CHEQUE_PROCESS_IDs"] = "";
            Session["CurrentWorkDate"] = null;
            Session.Add("CurrentWorkDate", DateTime.Now);
            CreatTable();
            string strProcessScript = "this.value='Wait...';this.disabled=true;";
            btnSave.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnSave, "").ToString());
            PurchaseChqueStatus();
            ddlCrossPAyment.Attributes.Add("disabled", "disabled");
            txtStartDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            txtTransactionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            LoadDistributor();
            PaymentType();
            LoadAccountDetail();
            LoadData();
            VendorItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            LoadCheque(2);
            DrpStatus.SelectedIndex = 0;
            DrpStatus.Enabled = false;
            DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            Session.Add("dtConfig", dtConfig);
            Session.Add("IsFinanceSetting", IsFinanceSetting);
            toggleControls(DrpAccountType.SelectedItem.Value.ToString());
            CrossPanel1.Visible = false;
            CrossPanel2.Visible = false;
        }
    }
    private void toggleControls(string pAccountType)
    {
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            if (pAccountType == "21" || pAccountType == "47") // Cash Receipt
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblFavouring.Visible = false;
                txtFavouring.Visible = false;
                lblChequeNo.Visible = false;
                txtChequeNumber.Visible = false;
                drpCheque.Visible = false;
                lblChequeDate.Visible = false;
                txtStartDate.Visible = false;
                ibtnStartDate.Visible = false;
                DrpStatus.Visible = false;
                lblStatus.Visible = false;
                GrdCO.Visible = true;
                GrdCheque.Visible = false;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
            else if (pAccountType == "33" || pAccountType == "34") // Online Transfer & Other Bank Transfer
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblFavouring.Visible = false;
                txtFavouring.Visible = false;
                lblChequeNo.Visible = false;
                txtChequeNumber.Visible = false;
                drpCheque.Visible = false;
                lblChequeDate.Visible = true;
                txtStartDate.Visible = true;
                ibtnStartDate.Visible = true;
                DrpStatus.Visible = false;
                lblStatus.Visible = false;
                lblChequeDate.InnerText = "Transfer Date";
                GrdCO.Visible = true;
                GrdCheque.Visible = false;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
            else if (pAccountType == "18") // Open Cheque
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblChequeDate.InnerText = "Cheque Date";
                txtChequeNumber.Visible = false;
                lblChequeNo.InnerText = "Cheque No";
                lblChequeNo.Visible = false;
                drpCheque.Visible = false;
                DrpStatus.Visible = true;
                lblStatus.Visible = true;
                GrdCheque.Visible = true;
                PanelOpenChqPayOrder.Visible = true;
                LoadCheque(2);
            }
            else if (pAccountType == "44") // Pay Order
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblChequeDate.InnerText = "Pay Order Date";
                lblChequeNo.InnerText = "Pay Order No";
                txtChequeNumber.Visible = false;
                lblChequeNo.Visible = false;
                drpCheque.Visible = false;
                DrpStatus.Visible = true;
                lblStatus.Visible = true;
                GrdCheque.Visible = true;
                PanelOpenChqPayOrder.Visible = true;
                txtAmount.Enabled = false;
                txtAmount.Text = "";
                LoadCheque(2);
            }
            else if (pAccountType == "46") // Own Cheque
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblChequeDate.InnerText = "Cheque Date";
                lblChequeNo.InnerText = "Cheque No";
                lblChequeNo.Visible = true;
                txtChequeNumber.Visible = false;
                DrpStatus.Visible = true;
                lblStatus.Visible = true;
                drpCheque.Visible = true;
                GrdCheque.Visible = true;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
                LoadCheque(2);
            }
        }
        else // For Purchase Return
        {
            if (pAccountType == "21")  // Cash Receipt
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblFavouring.Visible = false;
                txtFavouring.Visible = false;
                lblChequeNo.Visible = false;
                txtChequeNumber.Visible = false;
                drpCheque.Visible = false;
                lblChequeDate.Visible = false;
                txtStartDate.Visible = false;
                ibtnStartDate.Visible = false;
                DrpStatus.Visible = false;
                lblStatus.Visible = false;
                GrdCO.Visible = true;
                GrdCheque.Visible = false;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
            else if (pAccountType == "33" || pAccountType == "34") // Online Transfer & Other Bank Transfer
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblFavouring.Visible = false;
                txtFavouring.Visible = false;
                lblChequeNo.Visible = false;
                txtChequeNumber.Visible = false;
                drpCheque.Visible = false;
                lblChequeDate.Visible = true;
                txtStartDate.Visible = true;
                ibtnStartDate.Visible = true;
                DrpStatus.Visible = false;
                lblStatus.Visible = false;
                lblChequeDate.InnerText = "Transfer Date";
                GrdCO.Visible = true;
                GrdCheque.Visible = false;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
            else if (pAccountType == "18") // Open Cheque
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblFavouring.Visible = true;
                txtFavouring.Visible = true;
                lblChequeDate.InnerText = "Cheque Date";
                lblChequeDate.Visible = true;
                txtChequeNumber.Visible = true;
                lblChequeNo.InnerText = "Cheque No";
                lblChequeNo.Visible = true;
                drpCheque.Visible = false;
                DrpStatus.Visible = true;
                lblStatus.Visible = true;
                GrdCheque.Visible = true;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
            else if (pAccountType == "44") // Pay Order
            {
                LoadAccountHead(int.Parse(pAccountType));
                lblChequeDate.InnerText = "Pay Order Date";
                lblChequeNo.InnerText = "Pay Order No";
                txtChequeNumber.Visible = true;
                lblChequeNo.Visible = true;
                drpCheque.Visible = false;
                DrpStatus.Visible = true;
                lblStatus.Visible = true;
                GrdCheque.Visible = true;
                PanelOpenChqPayOrder.Visible = false;
                txtAmount.Enabled = true;
                txtAmount.Text = "";
            }
        }
    }

    #region Load
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("PURCHASE_MASTER_ID", typeof(long));
        PurchaseDetail.Columns.Add("ITEM", typeof(string));
        PurchaseDetail.Columns.Add("DATE", typeof(DateTime));
        PurchaseDetail.Columns.Add("DEBIT_AMOUNT", typeof(int));
        PurchaseDetail.Columns.Add("VENDOR_ID", typeof(int));
        PurchaseDetail.Columns.Add("IS_DELETED", typeof(bool));
        PurchaseDetail.Columns.Add("TYPE_ID", typeof(int));
        this.Session.Add("PurchaseDetail", PurchaseDetail);
    }
    public void PurchaseReturnChqueStatus()
    {
        DrpStatus.Items.Clear();
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Received", "526"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
        DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));
        DrpStatus.SelectedIndex = 0;
    }
    public void PurchaseChqueStatus()
    {
        DrpStatus.Items.Clear();
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Issue", "527"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));
        }
        else
        {
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Received", "526"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Clear", "529"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Bounce", "530"));
            DrpStatus.Items.Add(new DevExpress.Web.ListEditItem("Cancel", "560"));
        }
        DrpStatus.SelectedIndex = 0;
    }
    public void PaymentType()
    {
        if (Hp.SelectedValue == "1")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                DrpAccountType.Items.Clear();
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Other Bank Transfer", "34"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("JV", "47"));
                DrpAccountType.SelectedIndex = 0;
                ddlCrossPAyment.SelectedIndex = 0;
                ddlCrossPAyment.Enabled = false;
            }
            else
            {
                DrpAccountType.Items.Clear();
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Other Bank Transfer", "34"));
                DrpAccountType.SelectedIndex = 0;
                ddlCrossPAyment.SelectedIndex = 0;
                ddlCrossPAyment.Enabled = true;
            }
        }
        else
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2")
            {
                DrpAccountType.Items.Clear();
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Other Bank Transfer", "34"));
                DrpAccountType.SelectedIndex = 0;
                ddlCrossPAyment.SelectedIndex = 0;
                ddlCrossPAyment.Enabled = false;
            }
            else
            {
                DrpAccountType.Items.Clear();
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash", "21"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
                //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
                DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Other Bank Transfer", "34"));
                DrpAccountType.SelectedIndex = 0;
            }
        }
    }
    public void OpenChequeGrid(string Type)
    {
        drpCheque.Items.Clear();
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = new DataTable();
        DataTable CommonEntry_dt = new DataTable();
        DataTable DetailEntry_dt = new DataTable();
        if (Type == "Update")
        {
            Session["ChequeProccessIDs"] = null;
            Session["OldChequeID"] = null;
            string ChequeProccessIDs = "";
            Session["OldChequeID"] = int.Parse(HFChqueProcessId.Value);
            CommonEntry_dt = CController.SelectOpenChequePayorder(535, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(HFChqueProcessId.Value), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow IDRow in CommonEntry_dt.Rows)
                {
                    string Ids = IDRow["CHEQUE_IDs"].ToString();
                    if (!Ids.Equals(""))
                    {
                        string[] CHID = Ids.Split(',');
                        foreach (string ID in CHID)
                        {
                            DetailEntry_dt = CController.SelectOpenChequePayorder(541, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ID), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                            foreach (DataRow row in DetailEntry_dt.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr["CHEQUE_PROCESS_ID"] = int.Parse(row["CHEQUE_PROCESS_ID"].ToString());
                                dr["CHEQUE_NO"] = row["CHEQUE_NO"].ToString();
                                dr["area_id"] = int.Parse(row["area_id"].ToString());
                                dr["VENDOR_ID"] = int.Parse(IDRow["VENDOR_ID"].ToString());
                                dr["SKU_HIE_NAME"] = row["SKU_HIE_NAME"].ToString();
                                dr["VENDOR_NAME"] = row["VENDOR_NAME"].ToString();
                                dr["BANK_NAME"] = row["BANK_NAME"].ToString();
                                dr["CHEQUE_DATE"] = DateTime.Parse(row["CHEQUE_DATE"].ToString());
                                dr["RECEIVED_DATE"] = DateTime.Parse(row["RECEIVED_DATE"].ToString());
                                dr["DEPOSIT_DATE"] = DateTime.Parse(row["DEPOSIT_DATE"].ToString());
                                dr["CHEQUE_AMOUNT"] = decimal.Parse(row["CHEQUE_AMOUNT"].ToString());
                                dr["STATUS"] = row["STATUS"].ToString();
                                dr["STATUS_ID"] = int.Parse(row["STATUS_ID"].ToString());
                                dr["USER_ID"] = int.Parse(row["USER_ID"].ToString());
                                dr["SlipNo"] = row["SlipNo"].ToString();
                                dr["Remarks"] = row["Remarks"].ToString();
                                dr["account_head_id"] = int.Parse(row["account_head_id"].ToString());
                                dr["account_name"] = row["account_name"].ToString();
                                dr["DeliveryManID"] = int.Parse(row["DeliveryManID"].ToString());
                                dr["DISCOUNT"] = decimal.Parse(row["DISCOUNT"].ToString());
                                dr["DISCOUNT_TYPE"] = int.Parse(row["DISCOUNT_TYPE"].ToString());
                                dr["TAX_ACCOUNT_ID"] = int.Parse(row["TAX_ACCOUNT_ID"].ToString());
                                dr["CHEQUE_TYPE"] = int.Parse(row["CHEQUE_TYPE"].ToString());
                                dr["FAVOURING"] = row["FAVOURING"].ToString();
                                dr["ISSUED"] = int.Parse(row["ISSUED"].ToString());
                                dr["DISTRIBUTOR_ID"] = int.Parse(row["DISTRIBUTOR_ID"].ToString());
                                dr["PRINCIPAL_ID"] = int.Parse(row["PRINCIPAL_ID"].ToString());
                                dr["CUSTOMER_ID"] = int.Parse(row["CUSTOMER_ID"].ToString());
                                dr["PROCESS_DATE"] = DateTime.Parse(row["PROCESS_DATE"].ToString());
                                dr["chequeType"] = int.Parse(row["chequeType"].ToString());
                                dr["TAX"] = decimal.Parse(row["TAX"].ToString());
                                dt.Rows.Add(dr);
                                ChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
                            }
                        }
                    }
                }





                decimal TotalAmount = 0;
                GridChqPayOrder.DataSource = dt;
                GridChqPayOrder.DataBind();
                foreach (DataRow gvr in dt.Rows)
                {
                    TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                }
                GridChqPayOrder.FooterRow.Cells[0].Visible = false;
                GridChqPayOrder.FooterRow.Cells[8].Visible = false;
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
                drpCheque.Visible = false;

                if (ChequeProccessIDs != "")
                {
                    ChequeProccessIDs = ChequeProccessIDs.Remove(0, 1);
                }
                Session["ChequeProccessIDs"] = ChequeProccessIDs;
                string[] IDs = ChequeProccessIDs.Split(',');
                foreach (string ID in IDs)
                {
                    foreach (GridViewRow dr in GridChqPayOrder.Rows)
                    {
                        if (int.Parse(dr.Cells[1].Text) == int.Parse(ID))
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            chRelized.Checked = true;
                        }
                    }
                }
                ChbIsAssigned_CheckedChanged(null, null);
            }
            else
            {
                Session["ChequeProccessIDs"] = null;
                foreach (DataRow IDRow in CommonEntry_dt.Rows)
                {
                    string Ids = IDRow["CHEQUE_IDs"].ToString();
                    if (!Ids.Equals(""))
                    {
                        string[] CHID = Ids.Split(',');
                        foreach (string ID in CHID)
                        {
                            DetailEntry_dt = CController.SelectOpenChequePayorder(541, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ID), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                            foreach (DataRow row in DetailEntry_dt.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr["CHEQUE_PROCESS_ID"] = int.Parse(row["CHEQUE_PROCESS_ID"].ToString());
                                dr["CHEQUE_NO"] = row["CHEQUE_NO"].ToString();
                                dr["area_id"] = int.Parse(row["area_id"].ToString());
                                dr["VENDOR_ID"] = int.Parse(row["VENDOR_ID"].ToString());
                                dr["SKU_HIE_NAME"] = row["SKU_HIE_NAME"].ToString();
                                dr["VENDOR_NAME"] = row["VENDOR_NAME"].ToString();
                                dr["BANK_NAME"] = row["BANK_NAME"].ToString();
                                dr["CHEQUE_DATE"] = DateTime.Parse(row["CHEQUE_DATE"].ToString());
                                dr["RECEIVED_DATE"] = DateTime.Parse(row["RECEIVED_DATE"].ToString());
                                dr["DEPOSIT_DATE"] = DateTime.Parse(row["DEPOSIT_DATE"].ToString());
                                dr["CHEQUE_AMOUNT"] = decimal.Parse(row["CHEQUE_AMOUNT"].ToString());
                                dr["STATUS"] = row["STATUS"].ToString();
                                dr["STATUS_ID"] = int.Parse(row["STATUS_ID"].ToString());
                                dr["USER_ID"] = int.Parse(row["USER_ID"].ToString());
                                dr["SlipNo"] = row["SlipNo"].ToString();
                                dr["Remarks"] = row["Remarks"].ToString();
                                dr["account_head_id"] = int.Parse(row["account_head_id"].ToString());
                                dr["account_name"] = row["account_name"].ToString();
                                dr["DeliveryManID"] = int.Parse(row["DeliveryManID"].ToString());
                                dr["DISCOUNT"] = decimal.Parse(row["DISCOUNT"].ToString());
                                dr["DISCOUNT_TYPE"] = int.Parse(row["DISCOUNT_TYPE"].ToString());
                                dr["TAX_ACCOUNT_ID"] = int.Parse(row["TAX_ACCOUNT_ID"].ToString());
                                dr["CHEQUE_TYPE"] = int.Parse(row["CHEQUE_TYPE"].ToString());
                                dr["FAVOURING"] = row["FAVOURING"].ToString();
                                dr["ISSUED"] = int.Parse(row["ISSUED"].ToString());
                                dr["DISTRIBUTOR_ID"] = int.Parse(row["DISTRIBUTOR_ID"].ToString());
                                dr["PRINCIPAL_ID"] = int.Parse(row["PRINCIPAL_ID"].ToString());
                                dr["CUSTOMER_ID"] = int.Parse(row["CUSTOMER_ID"].ToString());
                                dr["PROCESS_DATE"] = DateTime.Parse(row["PROCESS_DATE"].ToString());
                                dr["chequeType"] = int.Parse(row["chequeType"].ToString());
                                dr["TAX"] = decimal.Parse(row["TAX"].ToString());
                                dt.Rows.Add(dr);
                                ChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
                            }
                        }
                    }
                }

                decimal TotalAmount = 0;
                GridChqPayOrder.DataSource = dt;
                GridChqPayOrder.DataBind();
                foreach (DataRow gvr in dt.Rows)
                {
                    TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                }
                GridChqPayOrder.FooterRow.Cells[0].Visible = false;
                GridChqPayOrder.FooterRow.Cells[8].Visible = false;
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
                drpCheque.Visible = false;

                if (ChequeProccessIDs != "")
                {
                    ChequeProccessIDs = ChequeProccessIDs.Remove(0, 1);
                }
                Session["ChequeProccessIDs"] = ChequeProccessIDs;
                string[] IDs = ChequeProccessIDs.Split(',');
                foreach (string ID in IDs)
                {
                    foreach (GridViewRow dr in GridChqPayOrder.Rows)
                    {
                        if (int.Parse(dr.Cells[1].Text) == int.Parse(ID))
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            chRelized.Checked = true;
                        }
                    }
                }
                ChbIsAssigned_CheckedChanged(null, null);
            }


        }
        else
        {
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
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
                drpCheque.Visible = false;
            }
            else
            {
                GridChqPayOrder.DataSource = null;
                GridChqPayOrder.DataBind();
            }
        }
    }
    public void PayOrderGrid(string Type)
    {
        drpCheque.Items.Clear();
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = new DataTable();
        DataTable CommonEntry_dt = new DataTable();
        DataTable DetailEntry_dt = new DataTable();
        if (Type == "Update")
        {
            Session["ChequeProccessIDs"] = null;
            Session["OldChequeID"] = null;
            string ChequeProccessIDs = "";
            Session["OldChequeID"] = int.Parse(HFChqueProcessId.Value);
            CommonEntry_dt = CController.SelectOpenChequePayorder(535, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(HFChqueProcessId.Value), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow IDRow in CommonEntry_dt.Rows)
                {
                    string Ids = IDRow["CHEQUE_IDs"].ToString();
                    if (!Ids.Equals(""))
                    {
                        string[] CHID = Ids.Split(',');
                        foreach (string ID in CHID)
                        {
                            DetailEntry_dt = CController.SelectOpenChequePayorder(541, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ID), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                            foreach (DataRow row in DetailEntry_dt.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr["CHEQUE_PROCESS_ID"] = int.Parse(row["CHEQUE_PROCESS_ID"].ToString());
                                dr["CHEQUE_NO"] = row["CHEQUE_NO"].ToString();
                                dr["area_id"] = int.Parse(row["area_id"].ToString());
                                dr["VENDOR_ID"] = int.Parse(IDRow["VENDOR_ID"].ToString());
                                dr["SKU_HIE_NAME"] = row["SKU_HIE_NAME"].ToString();
                                dr["VENDOR_NAME"] = IDRow["VENDOR_NAME"].ToString();
                                dr["BANK_NAME"] = row["BANK_NAME"].ToString();
                                dr["CHEQUE_DATE"] = DateTime.Parse(row["CHEQUE_DATE"].ToString());
                                dr["RECEIVED_DATE"] = DateTime.Parse(row["RECEIVED_DATE"].ToString());
                                dr["DEPOSIT_DATE"] = DateTime.Parse(row["DEPOSIT_DATE"].ToString());
                                dr["CHEQUE_AMOUNT"] = decimal.Parse(row["CHEQUE_AMOUNT"].ToString());
                                dr["STATUS"] = row["STATUS"].ToString();
                                dr["STATUS_ID"] = int.Parse(row["STATUS_ID"].ToString());
                                dr["USER_ID"] = int.Parse(row["USER_ID"].ToString());
                                dr["SlipNo"] = row["SlipNo"].ToString();
                                dr["Remarks"] = row["Remarks"].ToString();
                                dr["account_head_id"] = int.Parse(row["account_head_id"].ToString());
                                dr["account_name"] = row["account_name"].ToString();
                                dr["DeliveryManID"] = int.Parse(row["DeliveryManID"].ToString());
                                dr["DISCOUNT"] = decimal.Parse(row["DISCOUNT"].ToString());
                                dr["DISCOUNT_TYPE"] = int.Parse(row["DISCOUNT_TYPE"].ToString());
                                dr["TAX_ACCOUNT_ID"] = int.Parse(row["TAX_ACCOUNT_ID"].ToString());
                                dr["CHEQUE_TYPE"] = int.Parse(row["CHEQUE_TYPE"].ToString());
                                dr["FAVOURING"] = row["FAVOURING"].ToString();
                                dr["ISSUED"] = int.Parse(row["ISSUED"].ToString());
                                dr["DISTRIBUTOR_ID"] = int.Parse(row["DISTRIBUTOR_ID"].ToString());
                                dr["PRINCIPAL_ID"] = int.Parse(row["PRINCIPAL_ID"].ToString());
                                dr["CUSTOMER_ID"] = int.Parse(row["CUSTOMER_ID"].ToString());
                                dr["PROCESS_DATE"] = DateTime.Parse(row["PROCESS_DATE"].ToString());
                                dr["chequeType"] = int.Parse(row["chequeType"].ToString());
                                dr["TAX"] = decimal.Parse(row["TAX"].ToString());
                                dt.Rows.Add(dr);
                                ChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
                            }
                        }
                    }
                }





                decimal TotalAmount = 0;
                GridChqPayOrder.DataSource = dt;
                GridChqPayOrder.DataBind();
                foreach (DataRow gvr in dt.Rows)
                {
                    TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                }
                GridChqPayOrder.FooterRow.Cells[0].Visible = false;
                GridChqPayOrder.FooterRow.Cells[8].Visible = false;
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
                drpCheque.Visible = false;

                if (ChequeProccessIDs != "")
                {
                    ChequeProccessIDs = ChequeProccessIDs.Remove(0, 1);
                }
                Session["ChequeProccessIDs"] = ChequeProccessIDs;
                string[] IDs = ChequeProccessIDs.Split(',');
                foreach (string ID in IDs)
                {
                    foreach (GridViewRow dr in GridChqPayOrder.Rows)
                    {
                        if (int.Parse(dr.Cells[1].Text) == int.Parse(ID))
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            chRelized.Checked = true;
                        }
                    }
                }
                ChbIsAssigned_CheckedChanged(null, null);
            }
            else
            {
                Session["ChequeProccessIDs"] = null;
                foreach (DataRow IDRow in CommonEntry_dt.Rows)
                {
                    string Ids = IDRow["CHEQUE_IDs"].ToString();
                    if (!Ids.Equals(""))
                    {
                        string[] CHID = Ids.Split(',');
                        foreach (string ID in CHID)
                        {
                            DetailEntry_dt = CController.SelectOpenChequePayorder(541, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ID), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
                            foreach (DataRow row in DetailEntry_dt.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr["CHEQUE_PROCESS_ID"] = int.Parse(row["CHEQUE_PROCESS_ID"].ToString());
                                dr["CHEQUE_NO"] = row["CHEQUE_NO"].ToString();
                                dr["area_id"] = int.Parse(row["area_id"].ToString());
                                dr["VENDOR_ID"] = int.Parse(row["VENDOR_ID"].ToString());
                                dr["SKU_HIE_NAME"] = row["SKU_HIE_NAME"].ToString();
                                dr["VENDOR_NAME"] = row["VENDOR_NAME"].ToString();
                                dr["BANK_NAME"] = row["BANK_NAME"].ToString();
                                dr["CHEQUE_DATE"] = DateTime.Parse(row["CHEQUE_DATE"].ToString());
                                dr["RECEIVED_DATE"] = DateTime.Parse(row["RECEIVED_DATE"].ToString());
                                dr["DEPOSIT_DATE"] = DateTime.Parse(row["DEPOSIT_DATE"].ToString());
                                dr["CHEQUE_AMOUNT"] = decimal.Parse(row["CHEQUE_AMOUNT"].ToString());
                                dr["STATUS"] = row["STATUS"].ToString();
                                dr["STATUS_ID"] = int.Parse(row["STATUS_ID"].ToString());
                                dr["USER_ID"] = int.Parse(row["USER_ID"].ToString());
                                dr["SlipNo"] = row["SlipNo"].ToString();
                                dr["Remarks"] = row["Remarks"].ToString();
                                dr["account_head_id"] = int.Parse(row["account_head_id"].ToString());
                                dr["account_name"] = row["account_name"].ToString();
                                dr["DeliveryManID"] = int.Parse(row["DeliveryManID"].ToString());
                                dr["DISCOUNT"] = decimal.Parse(row["DISCOUNT"].ToString());
                                dr["DISCOUNT_TYPE"] = int.Parse(row["DISCOUNT_TYPE"].ToString());
                                dr["TAX_ACCOUNT_ID"] = int.Parse(row["TAX_ACCOUNT_ID"].ToString());
                                dr["CHEQUE_TYPE"] = int.Parse(row["CHEQUE_TYPE"].ToString());
                                dr["FAVOURING"] = row["FAVOURING"].ToString();
                                dr["ISSUED"] = int.Parse(row["ISSUED"].ToString());
                                dr["DISTRIBUTOR_ID"] = int.Parse(row["DISTRIBUTOR_ID"].ToString());
                                dr["PRINCIPAL_ID"] = int.Parse(row["PRINCIPAL_ID"].ToString());
                                dr["CUSTOMER_ID"] = int.Parse(row["CUSTOMER_ID"].ToString());
                                dr["PROCESS_DATE"] = DateTime.Parse(row["PROCESS_DATE"].ToString());
                                dr["chequeType"] = int.Parse(row["chequeType"].ToString());
                                dr["TAX"] = decimal.Parse(row["TAX"].ToString());
                                dt.Rows.Add(dr);
                                ChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
                            }
                        }
                    }
                }

                decimal TotalAmount = 0;
                GridChqPayOrder.DataSource = dt;
                GridChqPayOrder.DataBind();
                foreach (DataRow gvr in dt.Rows)
                {
                    TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
                }
                GridChqPayOrder.FooterRow.Cells[0].Visible = false;
                GridChqPayOrder.FooterRow.Cells[8].Visible = false;
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
                drpCheque.Visible = false;

                if (ChequeProccessIDs != "")
                {
                    ChequeProccessIDs = ChequeProccessIDs.Remove(0, 1);
                }
                Session["ChequeProccessIDs"] = ChequeProccessIDs;
                string[] IDs = ChequeProccessIDs.Split(',');
                foreach (string ID in IDs)
                {
                    foreach (GridViewRow dr in GridChqPayOrder.Rows)
                    {
                        if (int.Parse(dr.Cells[1].Text) == int.Parse(ID))
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            chRelized.Checked = true;
                        }
                    }
                }
                ChbIsAssigned_CheckedChanged(null, null);
            }
            //dt = CController.SelectOpenChequePayorder(535, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(HFChqueProcessId.Value));
            //if (dt.Rows.Count > 0)
            //{
            //    decimal TotalAmount = 0;
            //    GridChqPayOrder.DataSource = dt;
            //    GridChqPayOrder.DataBind();
            //    foreach (DataRow gvr in dt.Rows)
            //    {
            //        TotalAmount += Convert.ToDecimal(gvr["CHEQUE_AMOUNT"]);
            //    }
            //    GridChqPayOrder.FooterRow.Cells[0].Visible = false;
            //    GridChqPayOrder.FooterRow.Cells[8].Visible = false;
            //    GridChqPayOrder.FooterRow.Cells[7].Text = string.Format("{0:n2}", TotalAmount);
            //    GridChqPayOrder.FooterRow.Cells[6].Text = "Total";
            //    GridChqPayOrder.FooterRow.Cells[6].Font.Bold = true;
            //    GridChqPayOrder.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            //    GridChqPayOrder.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
            //    GridChqPayOrder.FooterRow.Cells[7].Font.Bold = true;
            //    GridChqPayOrder.HeaderRow.Cells[4].Text = "PO #";
            //    GridChqPayOrder.HeaderRow.Cells[5].Text = "PO Date";
            //    GridChqPayOrder.HeaderRow.Cells[7].Text = "PO Amount";
            //    lblChequeNo.Visible = false;
            //    drpCheque.Visible = false;
            //}
            //foreach (GridViewRow dr in GridChqPayOrder.Rows)
            //{
            //    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //    chRelized.Checked = true;
            //}

        }
        else
        {
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                , int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
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
                drpCheque.Visible = false;
            }
            else
            {
                GridChqPayOrder.DataSource = null;
                GridChqPayOrder.DataBind();
            }
        }
    }
    public void LoadCheque(int TYPE)
    {
        try
        {
            if (Session["Val"] != null)
            {
                int VAL = Convert.ToInt32(Session["Val"]);
                if (DrpAccountType.SelectedItem.Value.ToString() == "18") //VAL == 84 && 
                {
                    OpenChequeGrid("Update");
                    txtAmount.Enabled = false;
                    txtAmount.Text = "";
                    Session["Val"] = null;
                }
                else if (DrpAccountType.SelectedItem.Value.ToString() == "44") //VAL == 85 && 
                {
                    PayOrderGrid("Update");
                    txtAmount.Enabled = false;
                    txtAmount.Text = "";
                    Session["Val"] = null;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(VAL), null, null, null, true, Constants.DateNullValue, TYPE);
                    clsWebFormUtil.FillDxComboBoxList(this.drpCheque, dt, 0, 1, true);

                    if (dt.Rows.Count > 0)
                    {
                        Session["drpCheque"] = null;
                        drpCheque.SelectedIndex = 0;
                        Session.Add("drpCheque", dt);
                        Session["Val"] = null;
                    }
                    txtAmount.Enabled = true;
                    txtAmount.Text = "";
                }
            }
            else
            {
                // Open Cheques In Hand
                if (DrpAccountType.SelectedItem.Value.ToString() == "18") //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
                {
                    OpenChequeGrid("");
                    txtAmount.Enabled = false;
                    txtAmount.Text = "";
                }
                else if (DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" && 
                {
                    // Pay Orders In Hand
                    PayOrderGrid("");
                    txtAmount.Enabled = false;
                    txtAmount.Text = "";
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "46")
                    {
                        drpCheque.Items.Clear();
                        DataTable dt = new DataTable();
                        dt = _bnkController.SelectCheckBook(Constants.IntNullValue, Convert.ToInt32(DrpBankAccount.SelectedItem.Value.ToString()), null, null, null, true, Constants.DateNullValue, TYPE);
                        clsWebFormUtil.FillDxComboBoxList(this.drpCheque, dt, 0, 1, true);
                        if (dt.Rows.Count > 0)
                        {
                            drpCheque.SelectedIndex = 0;
                        }
                        lblChequeNo.Visible = true;
                        drpCheque.Visible = true;
                        GridChqPayOrder.DataSource = null;
                        GridChqPayOrder.DataBind();
                        txtStartDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                        txtAmount.Enabled = true;
                        txtAmount.Text = "";
                    }
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
            DateTime ChqDate = DateTime.Parse(foundRows[0]["CHEQUE_DATE"].ToString()); //ToString("dd-MM-yyyy");
            txtStartDate.Text = ChqDate.ToString("dd-MM-yyyy");
            decimal amount = decimal.Parse(foundRows[0]["CHEQUE_AMOUNT"].ToString());
            txtAmount.Text = Math.Round(amount, 0).ToString();
        }
    }
    private void PayOrdersDetail(int p_Cheque_No)
    {
        DataTable dt = (DataTable)this.Session["PayOrdersInHand"];
        DataRow[] foundRows = dt.Select("CHEQUE_PROCESS_ID  = '" + p_Cheque_No + "'");
        if (foundRows.Length > 0)
        {
            DateTime ChqDate = DateTime.Parse(foundRows[0]["CHEQUE_DATE"].ToString()); //ToString("dd-MM-yyyy");
            txtStartDate.Text = ChqDate.ToString("dd-MM-yyyy");
            decimal amount = decimal.Parse(foundRows[0]["CHEQUE_AMOUNT"].ToString());
            txtAmount.Text = Math.Round(amount, 0).ToString();
        }
    }
    private void LoadPaymentRecieved()
    {
        ChequeEntryController CController = new ChequeEntryController();
        LedgerController LController = new LedgerController();

        DateTime Date = Constants.DateNullValue;
        string abd = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
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
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2")
            {
                var PController = new SKUPriceDetailController();
                DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 12, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
                if (dtVendor.Rows.Count > 0)
                {
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Vendor";
                }
            }
            else if (ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                var PController = new SKUPriceDetailController();
                DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString())
                    , Constants.IntNullValue, 13, DateTime.Parse(Session["CurrentWorkDate"].ToString()));
                clsWebFormUtil.FillDxComboBoxList(DrpVendor, dtVendor, 0, 1, true);
                if (dtVendor.Rows.Count > 0)
                {
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Customer";
                }
                else
                {
                    DrpVendor.Items.Clear();
                    DrpVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Customer";
                }
            }
            else
            {
                DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 19);
                if (dt.Rows.Count > 0)
                {
                    DrpVendor.Items.Clear();
                    DrpVendor.DataSource = dt;
                    DrpVendor.TextField = "VENDOR_NAME";
                    DrpVendor.ValueField = "VENDOR_ID";
                    DrpVendor.DataBind();
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Vendor";
                }
                else
                {
                    DrpVendor.Items.Clear();
                    DrpVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                    DrpVendor.SelectedIndex = 0;
                    lblVendor.InnerText = "Vendor";
                }
            }

        }
        else
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2")
            {
                DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 18);
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
            else
            {
                DrpVendor.Items.Clear();
                DrpVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                DrpVendor.SelectedIndex = 0;
            }
        }
    }
    public void VendorItems()
    {
        if (DrpVendor.Items.Count > 0)
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2")
            {
                int CustomerID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                DbCalls db = new DbCalls();
                var qry = (from k in db.tbl_PURCHASE_MASTER
                           join pd in db.tbl_PURCHASE_DETAIL on k.PURCHASE_MASTER_ID equals pd.PURCHASE_MASTER_ID
                           join i in db.ITEM_INFORMATION on pd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                           join b in db.BLOCKs on i.BLOCK_ID equals b.BLOCK_ID
                           join s in db.STREETs on i.STREET_ID equals s.STREET_ID
                           select new { pd.PURCHASE_DETAIL_ID, k.PURCHASE_MASTER_ID, k.VENDOR_ID, pd.ITEM_INFO_ID, ITEM_NAME = i.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME, pd.PURCHASE_CLOSED }).Where(x => x.PURCHASE_CLOSED == false && x.VENDOR_ID == CustomerID).ToList();
                if (qry.Count != 0)
                {
                    ddlItem.Items.Clear();
                    ddlItem.DataSource = qry;
                    ddlItem.TextField = "ITEM_NAME";
                    ddlItem.ValueField = "PURCHASE_DETAIL_ID";
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

            ddlItem.SelectedIndex = 0;
        }

    }
    public void LoadSaleInstallments(int ID)
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.tbl_PURCHASE_INS_DETAIL
                   join sm in db.tbl_PURCHASE_INS_MASTER on k.PURCHASE_INS_MASTER_ID equals sm.PURCHASE_INS_MASTER_ID
                   select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.PAYMENT_DESC, k.DUE_DATE, k.OS_AMOUNT, k.IS_PAID, sm.DEBIT_AMOUNT, k.INST_NO, sm.PURCHASE_CLOSED }).Where(x => x.IS_PAID == false && x.PURCHASE_CLOSED == false && x.PURCHASE_INS_MASTER_ID == ID).OrderBy(x => x.DUE_DATE).ToList();
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
    private void LoadReceviedCheque()
    {

        GrdCheque.DataSource = null;
        GrdCheque.DataBind();

        GrdCO.DataSource = null;
        GrdCO.DataBind();
        DateTime Date = Constants.DateNullValue;
        string abd = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
        decimal chqAmount = 0;
        if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "34")
        {
            if (drpDistributor.Items.Count > 0)
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
                {
                    if (Hp.SelectedValue == "1")
                    {
                        int Type = 0;
                            if (ddlTransactionType.SelectedItem.Value.ToString() == "14")
                            {
                                Type = 2;
                            }
                            else
                            {
                                Type = 0;
                            }
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
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
                            GrdCO.FooterRow.Cells[6].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[5].Text = "Total";
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[6].Font.Bold = true;
                            GrdCO.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }
                    else
                    {
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
                        Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), 4);
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
                            GrdCO.FooterRow.Cells[6].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[5].Text = "Total";
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[6].Font.Bold = true;
                            GrdCO.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }
                }
                else // For Purchase Return
                {
                    if (Hp.SelectedValue == "1")
                    {
                        DataTable dt = LController.VendorBankCashTransction(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(DrpAccountType.SelectedItem.Value.ToString()),
                        Date, Date, int.Parse(DrpVendor.SelectedItem.Value.ToString()), 3);
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
                            GrdCO.FooterRow.Cells[6].Text = string.Format("{0:n2}", chqAmount);
                            GrdCO.FooterRow.Cells[5].Text = "Total";
                            GrdCO.FooterRow.Cells[5].Font.Bold = true;
                            GrdCO.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                            GrdCO.FooterRow.Cells[6].Font.Bold = true;
                            GrdCO.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
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
                DataTable dt = null;
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
                {
                    if (DrpVendor.Items.Count > 0)
                    {

                        dt = CController.SelectVendorChequeEntry(527528, Date, int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), int.Parse(Session["UserId"].ToString())
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
                            break;
                        }
                        else
                        {
                            row.Enabled = true;
                        }
                    }
                }
                else // For Purchase Return
                {
                    int Ststus = 0;
                    if (DrpVendor.Items.Count > 0)
                    {
                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                        {
                            Ststus = 626628;
                        }
                        else
                        {
                            Ststus = 526528;
                        }
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

                    foreach (GridViewRow gvr in GrdCreditReturn.Rows)
                    {
                        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                        if (row.Enabled)
                        {
                            break;
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
                ///Load Cash Realization Detail
                if (Convert.ToInt32(DrpStatus.SelectedItem.Value.ToString()) < 530)
                {
                    DataSet ds = CController.SelectVendorChequeEntry(int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Parse(Session["CurrentWorkDate"].ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                        );
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
                DataTable dt = CController.SelectVendorChequeEntry(int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Parse(Session["CurrentWorkDate"].ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(ddlTransactionType.SelectedItem.Value.ToString()));
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
            if (DrpStatus.SelectedItem.Value.ToString() != Constants.Cheque_Pending.ToString())
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
                {
                    foreach (GridViewRow gvr in GrdCredit.Rows)
                    {
                        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                        row.Checked = false;
                        row.Enabled = false;
                    }
                }
                else
                {
                    foreach (GridViewRow gvr in GrdCreditReturn.Rows)
                    {
                        CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
                        row.Checked = false;
                        row.Enabled = true;
                    }
                }

            }
        }
    }
    private void checkDuplication()
    {
        DataTable dt = (DataTable)Session["dt"];
        DataRow[] foundRows = null;
        if (dt.Rows.Count > 0)
        {
            //if (ddlTransactionType.SelectedItem.Value.ToString() == "2" && DrpBankAccount.SelectedItem.Value.ToString() != "84")
            //{
            //    foundRows = dt.Select("VENDOR_ID = '" + DrpVendor.SelectedItem.Value.ToString() + "' and CHEQUE_NO='" + drpCheque.SelectedItem.Text + "'");
            //}
            //else
            //{
            //    foundRows = dt.Select("VENDOR_ID = '" + DrpVendor.SelectedItem.Value.ToString() + "' and CHEQUE_NO='" + txtChequeNumber.Text + "'");
            //}
            //if (foundRows.Length > 0)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Cheque No Already exist against this vendor!');", true);
            //}
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
            DbCalls db = new DbCalls();
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase 
            {
                if (Hp.SelectedValue != "2")
                {
                    loadPurchases(VendorID);
                }
                else
                {
                    LoadSaleInstallments(ItemID);
                }

            }
            else if (ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Sale Return
            {
                if (Hp.SelectedValue != "2")
                {
                    LoadSaleReturnInvoices(VendorID);
                }
                else
                {
                    LoadSaleInstallments(ItemID);
                }
            }
            else // For Purchase Return
            {

                if (Hp.SelectedValue != "2")
                {
                    LoadSaleReturnInvoices(VendorID);
                }
                else
                {
                    GrdCreditInstallment.DataSource = null;
                    GrdCreditInstallment.DataBind();
                    GrdCreditReturn.DataSource = null;
                    GrdCreditReturn.DataBind();
                    GrdCredit.DataSource = null;
                    GrdCredit.DataBind();
                }

            }

        }
    }
    public void loadPurchases(int VendorID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, VendorID, Constants.IntNullValue, Constants.IntNullValue, null, 11);
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
        Session["PurchaseDetail"] = null;
    }
    public void LoadSaleReturnInvoices(int VendorID)
    {
        DataTable dt = LController.SelectBSI(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VendorID, Constants.IntNullValue, Constants.IntNullValue, null, 44);
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
        Session["PurchaseDetail"] = null;
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
        AccountHeadController mAccountController = new AccountHeadController();
        dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, TypeID);
        clsWebFormUtil.FillDxComboBoxList(DrpBankAccount, dtHead, "ACCOUNT_HEAD_ID", "ACCOUNT_HEAD", true);
        if (dtHead.Rows.Count > 0)
        {
            DrpBankAccount.SelectedIndex = 0;
        }
        Session.Add("dtHead", dtHead);
        DrpBankAccount_SelectedIndexChanged(null, null);
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
            txtTotalCreditAmount2.Text =
            string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[3].Text = "Total";
            CrossGrdCredit.FooterRow.Cells[3].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[4].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[1].Visible = false;
            CrossGrdCredit.FooterRow.Cells[5].Visible = false;
        }
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
            txtTotalCreditAmount2.Text =
            string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[4].Text = string.Format("{0:n2}", TotalAmount);
            CrossGrdCredit.FooterRow.Cells[3].Text = "Total";
            CrossGrdCredit.FooterRow.Cells[3].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[4].Font.Bold = true;
            CrossGrdCredit.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            CrossGrdCredit.FooterRow.Cells[1].Visible = false;
            CrossGrdCredit.FooterRow.Cells[5].Visible = false;
        }
    }
    #endregion

    #region Check Invioces for Payments
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
                Session["ITEM_NAME"] = dr.Cells[2].Text;
                Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                Session["CrossITEM_NAME"] = "";
                Session["CrossVendor_Name"] = "";
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
    public bool CheckOpenChqPay()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GridChqPayOrder.Rows)
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
    public bool CheckInvoiceInstallment()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GrdCreditInstallment.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                Session["ITEM_NAME"] = dr.Cells[2].Text;
                Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                Session["CrossITEM_NAME"] = "";
                Session["CrossVendor_Name"] = "";
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
    public bool CheckInvoiceReturn()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GrdCreditReturn.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                Session["ITEM_NAME"] = dr.Cells[2].Text;
                Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                Session["CrossITEM_NAME"] = "";
                Session["CrossVendor_Name"] = "";
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
    public bool CrossCheckInvoice()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in CrossGrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                Session["CrossITEM_NAME"] = dr.Cells[2].Text;
                Session["CrossVendor_Name"] = DrpBankAccount.SelectedItem.Text;
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
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "34") //|| DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
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
                VoucherType = Constants.CashPayment_Voucher;
            }
            remarks = DrpAccountType.SelectedItem.Text + " / " + txtRemarks.Text;
        }
        else //if (DrpAccountType.SelectedItem.Value.ToString() == "33")
        {
            VoucherType = Constants.Expanse_Voucher;
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
        //Advance Entry for Purchase
        IDbConnection mConnection = null;
        IDbTransaction mTransaction = null;
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            if (realizeAmount < decimal.Parse(txtAmount.Text))
        {
            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentID, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentID, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                  Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Constants.LongNullValue, null, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
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
                if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                {
                    ChequeNo2 = ChequeNO();
                }
                InsertGL2(ChequeNo2, MaxDocumentID, mConnection, mTransaction);
            }
            #endregion
            Session["VoucherNo"] = null;
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
    private void ChequeRealization(int p_TYPE)
    {
        #region General Veriables
        LedgerController LController = new LedgerController();
        string MaxDocumentId = "";
        string MaxDocumentIdForCross = "";
        int VoucherType;
        int TransactionType;
        string Item_Name = Session["ITEM_NAME"].ToString();
        string Vendor_Name = Session["Vendor_Name"].ToString();

        string CrossItem_Name = Session["CrossITEM_NAME"].ToString();
        string CrossVendor_Name = Session["CrossVendor_Name"].ToString();
        decimal OfferAmount = decimal.Parse(txtAmount.Text);
        decimal realizeAmount = 0;

        string remarks = "";
        string remarks2 = "";
        int ChequeType = Constants.IntNullValue;
        string Favouring = "";
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        string ChequeNo = null;
        string DocumentNo = null;
        string p_PYEES_NAME = "";
        bool FE = false;
        long ItemLogID = 0;
        #endregion
        #region ChequeDate
        DateTime ChequeDate = Constants.DateNullValue;
        try
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "34") //|| DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "45")
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
        #endregion
        #region Transaction Type
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                VoucherType = Constants.Journal_Voucher;
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
                VoucherType = Constants.Journal_Voucher;
                TransactionType = 1;
            }
            else
            {
                VoucherType = Constants.Bank_Voucher;
                TransactionType = 0;
            }
        }
        #endregion
        #region Set Payments Types
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "46")
        //|| DrpAccountType.SelectedItem.Value.ToString() == "45")//Cheque
        {
            ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
            Favouring = txtFavouring.Text;
            //MaxDocumentId = LController.SelectLedgerMaxDocumentId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 1);
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                }
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + DrpBankAccount.SelectedItem.Text + ", " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + DrpBankAccount.SelectedItem.Text + ", " + txtRemarks.Text;
                }
            }
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "34")//Online
        {
            //MaxDocumentId = LController.SelectLedgerMaxDocumentId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 1);
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + " Online Transfer, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
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
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", " + " Online Transfer, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", " + "Online Transfer, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", " + " Online Transfer, " + txtRemarks.Text;
                }
            }
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "47")//Cash
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    VoucherType = Constants.Journal_Voucher;
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
                        VoucherType = Constants.CashPayment_Voucher;
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
                    VoucherType = Constants.Cash_Voucher;
                    TransactionType = 0;
                }
            }
            //MaxDocumentId = LController.SelectLedgerMaxDocumentId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 1);
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            else
            {
                MaxDocumentId = LController.SelectMaxVoucherId(VoucherType, int.Parse(drpDistributor.SelectedItem.Value.ToString()), Date);
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2" || ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                        {
                            remarks = ddlTransactionType.SelectedItem.Text + ", JV, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                        else
                        {
                            remarks = ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        }
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
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
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Receipt, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Receipt, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                        remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Payment, " + Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text;
                    }

                }
                else
                {
                    if (Hp.SelectedValue == "1")
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + ", Cash Receipt, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }
                    else
                    {
                        remarks = ddlTransactionType.SelectedItem.Text + " Installment" + ", Cash Receipt, " + Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text;
                    }

                    //remarks2 = "Cross Payment, " + ddlTransactionType.SelectedItem.Text + ", Cash Payment, " + txtRemarks.Text;
                }
            }
        }
        #endregion

        Session.Add("VoucherNo", MaxDocumentId);
        Session.Add("CrossVoucherNo", MaxDocumentIdForCross);

        
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sales Return
        {
            if (Hp.SelectedValue != "2")
            {
                IDbConnection mConnection = null;
                IDbTransaction mTransaction = null;
                try
                {
                    mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                    mConnection.Open();
                    mTransaction = ProviderFactory.GetTransaction(mConnection);
                    
                    #region Complete Transaction

                    if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                {
                    ChequeNo = ChequeNO();
                }
                else
                {
                    ChequeNo = txtChequeNumber.Text;
                }
                foreach (GridViewRow dr in GrdCredit.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    if (chRelized.Checked == true)
                    {
                        #region Set Transection Type
                        int TYPE;
                        int type = Convert.ToInt32(dr.Cells[5].Text);
                        if (type == Constants.InvoiceBooking || type == Constants.Document_Acess || type == Constants.Document_Opening)
                        {
                            if (type == Constants.InvoiceBooking)
                            {
                                TYPE = Constants.InvoiceBooking;
                                p_PYEES_NAME = "Invoice Booking";
                            }
                            else if (type == Constants.Document_Acess)
                            {
                                TYPE = Constants.Document_Acess;
                                p_PYEES_NAME = "Inventory Update";
                                ItemLogID = Convert.ToInt32(dr.Cells[8].Text);
                            }
                            else
                            {
                                TYPE = Constants.Document_Opening;
                                p_PYEES_NAME = "Opening";
                            }
                        }
                        else if (type == Constants.Document_Sale_Return)
                        {
                            TYPE = Constants.Document_Sale_Return;
                            p_PYEES_NAME = "Sale Return";
                        }
                        else
                        {
                            TYPE = int.Parse(dr.Cells[5].Text);
                            p_PYEES_NAME = "Purchase";
                            ItemLogID = Convert.ToInt32(dr.Cells[8].Text);
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
                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), TYPE,
                                ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", p_PYEES_NAME, ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                             Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), TYPE,
                             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", p_PYEES_NAME, ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);


                            #region Commented Code
                            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                            //{
                            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase);

                            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase);
                            //}
                            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                            //{
                            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleInvoice);

                            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleInvoice);
                            //}
                            #endregion

                            OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;

                            #region Upddate Vendor's Main Invoice 
                            if (TYPE != 9)
                            {
                                    //if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 2)  // For Purchases
                                    if (TYPE == 2 || TYPE == 30)  // For Purchases or Invoice Booking 
                                    {
                                    #region For Purchase, Opening & Invoice Booking
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET DEBIT_AMOUNT = " + OfferAmount + " WHERE PURCHASE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }
                                else // For Sales Return
                                {
                                    #region For Sales Return
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + OfferAmount + " WHERE SALE_RETURN_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                #region For Inventory Update
                                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                {
                                    con.Open();
                                    using (SqlCommand command = new SqlCommand("UPDATE tbl_INVENTORY_UPDATE_DETAIL SET DEBIT_AMOUNT = " + OfferAmount + " WHERE INVENTORY_UPDATE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    con.Close();
                                }
                                #endregion
                            }
                            #endregion
                            break;
                        }
                        else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
                        {
                            realizeAmount += decimal.Parse(dr.Cells[4].Text);
                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), TYPE,
                             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", p_PYEES_NAME, ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                            LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), TYPE,
                            ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", p_PYEES_NAME, ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                            #region Commented Code
                            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                            //{
                            //    if (!FE)
                            //    {
                            //        LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase);

                            //        LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //        ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase);
                            //    }
                            //}
                            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                            //{
                            //    if (!FE)
                            //    {
                            //        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleInvoice);

                            //        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //        ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleInvoice);
                            //    }
                            //}
                            #endregion

                            OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);

                            #region Upddate Vendor's Main Invoice
                            if (TYPE != 9)
                            {
                                if (TYPE == 2) //(Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 2)  // For Purchases
                                {
                                    #region For Purchase, Opening & Invoice Booking
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        //if (TYPE != 30)
                                        //{
                                        //    using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = 0 WHERE ITEM_LOG_ID=" + ItemLogID, con))
                                        //    {
                                        //        command2.ExecuteNonQuery();
                                        //    }
                                        //}
                                        con.Close();
                                    }
                                    #endregion
                                }
                                else // For Sales Return
                                {
                                    #region For Sales Return
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_RETURN SET CREDIT_AMOUNT = " + 0 + ", RETURN_CLOSED =" + 1 + " where SALE_RETURN_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
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
                            else
                            {
                                #region For Inventory Update
                                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                {
                                    con.Open();
                                    using (SqlCommand command = new SqlCommand("UPDATE tbl_INVENTORY_UPDATE_DETAIL SET DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " WHERE INVENTORY_UPDATE_DETAIL_ID=" + Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), con))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    con.Close();
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
                #region Advance Entry for Purchase
                if (realizeAmount < decimal.Parse(txtAmount.Text))
                {
                    if (Convert.ToInt32(ddlTransactionType.SelectedItem.Value.ToString()) == 2) // For Purchases
                    {
                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                    }
                    else // For Sales Return
                    {
                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                        LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                              Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                              ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType, mConnection, mTransaction);
                    }
                }
                    #endregion



                    #endregion

                    #region GL VOUCHER ENTRY
                    if (p_TYPE == 1)
                    {
                        InsertGL(MaxDocumentId, mConnection, mTransaction);
                    }
                    else
                    {
                        string ChequeNo2 = null;
                        if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                        {
                            ChequeNo2 = ChequeNO();
                        }
                        InsertGL2(ChequeNo2, MaxDocumentId, mConnection, mTransaction);
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
                PrintVoucher(MaxDocumentId);
            }
            else // For Installments 
            {
                #region Purchase Installments
                foreach (GridViewRow dr in GrdCreditInstallment.Rows)
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    if (chRelized.Checked == true)
                    {
                        string manualNo = dr.Cells[1].Text;
                        DocumentNo = dr.Cells[1].Text;
                        if (DrpAccountType.SelectedIndex == 0)
                        {
                            ChequeNo = drpCheque.SelectedItem.Text;
                        }
                        else if (DrpAccountType.SelectedIndex == 3)
                        {
                            ChequeNo = txtChequeNumber.Text;
                        }
                        if (Convert.ToString(dr.Cells[1].Text) == "opng")
                        {
                            manualNo = "opng";
                        }
                        if (decimal.Parse(dr.Cells[7].Text) > OfferAmount)
                        {
                            realizeAmount += OfferAmount;
                            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment, Favouring, ChequeType);

                            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                             Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment, Favouring, ChequeType);

                            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                            //{
                            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment);

                            //    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment);
                            //}
                            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                            //{
                            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment);

                            //    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment);
                            //}

                            OfferAmount = decimal.Parse(dr.Cells[7].Text) - OfferAmount;
                            int MasterID = Convert.ToInt32(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]);
                            DbCalls db = new DbCalls();
                            var qry = (from k in db.tbl_PURCHASE_INS_DETAIL
                                       select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.PURCHASE_INS_MASTER_ID == MasterID).ToList();
                            int DueAmount = Convert.ToInt32(qry.First().DUE_AMOUNT);
                            int ReceivedAmount = Convert.ToInt32(txtAmount.Text);
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_MASTER set DEBIT_AMOUNT = " + OfferAmount + " where PURCHASE_INS_MASTER_ID=" + MasterID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                int firstentry = 0;
                                bool check = false;
                                for (int i = 0; i <= qry.Count - 1; i++)
                                {
                                    var query = (from k in db.tbl_PURCHASE_INS_DETAIL
                                                 select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.PAY_AMOUNT, k.OS_AMOUNT, k.IS_PAID, k.DUE_DATE }).Where(x => x.IS_PAID == false && x.PURCHASE_INS_MASTER_ID == MasterID).ToList();
                                    int SALE_DETAIL_ID = Convert.ToInt32(query.First().PURCHASE_INS_DETAIL_ID);
                                    int DUE_AMOUNT = Convert.ToInt32(query.First().DUE_AMOUNT);
                                    int OS_AMOUNT = Convert.ToInt32(query.First().OS_AMOUNT);
                                    int REC_AMOUNT = Convert.ToInt32(query.First().PAY_AMOUNT);
                                    int RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                    if (ReceivedAmount > OS_AMOUNT)
                                    {
                                        if (firstentry != i)
                                        {
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            check = true;
                                        }
                                        else
                                        {
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            check = true;
                                        }
                                        ReceivedAmount = ReceivedAmount - OS_AMOUNT;
                                    }
                                    else
                                    {
                                        int recv = 0;
                                        int Paid;
                                        int OS_Amount = OS_AMOUNT - ReceivedAmount;
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
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(recv) + ", OS_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", IS_PAID=" + Paid + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_NO = '" + MaxDocumentId + "', PAYMENT_DATE ='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(recv) + ", OS_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", IS_PAID=" + Paid + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            break;
                                        }
                                    }
                                }
                                con.Close();
                            }
                            break;
                        }
                        else if (decimal.Parse(dr.Cells[7].Text) <= OfferAmount)
                        {
                            realizeAmount += decimal.Parse(dr.Cells[7].Text);
                            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[7].Text),
                            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                             ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment, Favouring, ChequeType);

                            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[7].Text), 0,
                            Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment, Favouring, ChequeType);

                            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                            //{
                            //    if (!FE)
                            //    {
                            //        LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment);

                            //        LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //        ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Purchase_Installment);
                            //    }
                            //}
                            //else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                            //{
                            //    if (!FE)
                            //    {
                            //        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                            //    Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //    ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment);

                            //        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentIdForCross, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                            //        Date, remarks2, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                            //        ChequeNo, int.Parse(Session["UserId"].ToString()), 0, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_Invoice_Installment);
                            //    }
                            //}

                            OfferAmount = OfferAmount - decimal.Parse(dr.Cells[7].Text);
                            int MasterID = Convert.ToInt32(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]);
                            DbCalls db = new DbCalls();
                            var qry = (from k in db.tbl_PURCHASE_INS_DETAIL
                                       select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.PURCHASE_INS_MASTER_ID == MasterID).ToList();
                            int DUE_AMOUNT = Convert.ToInt32(qry.FirstOrDefault().DUE_AMOUNT);
                            int ReceivedAmount = Convert.ToInt32(txtAmount.Text);
                            int SALE_MASTER_ID = Convert.ToInt32(qry.FirstOrDefault().PURCHASE_INS_MASTER_ID);
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_MASTER set DEBIT_AMOUNT = " + 0 + ", PURCHASE_CLOSED =" + 1 + " where PURCHASE_INS_MASTER_ID=" + Convert.ToInt64(GrdCreditInstallment.DataKeys[dr.RowIndex].Values["PURCHASE_INS_MASTER_ID"]), con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                int firstentry = 0;
                                //bool check = false;
                                for (int i = 0; i <= qry.Count - 1; i++)
                                {
                                    var query = (from k in db.tbl_PURCHASE_INS_DETAIL
                                                 select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.PAY_AMOUNT, k.OS_AMOUNT, k.IS_PAID }).Where(x => x.IS_PAID == false && x.PURCHASE_INS_MASTER_ID == MasterID).ToList();
                                    int SALE_DETAIL_ID = Convert.ToInt32(query.First().PURCHASE_INS_DETAIL_ID);
                                    //int DUE_AMOUNT = Convert.ToInt32(query.First().DUE_AMOUNT);
                                    int OS_AMOUNT = Convert.ToInt32(query.First().OS_AMOUNT);
                                    int REC_AMOUNT = Convert.ToInt32(query.First().PAY_AMOUNT);
                                    int RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                    if (ReceivedAmount >= OS_AMOUNT)
                                    {
                                        if (firstentry != i)
                                        {
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            //check = true;
                                        }
                                        else
                                        {
                                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ", RECEIPT_NO='" + MaxDocumentId + "', PAYMENT_DATE='" + Convert.ToDateTime(DateTime.Now.ToShortDateString()) + "', PAY_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", OS_AMOUNT=" + Convert.ToDecimal(0.00) + ", IS_PAID=" + 1 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
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
                        }
                    }
                }
                //Advance Entry for Purchase Installment
                if (realizeAmount < decimal.Parse(txtAmount.Text))
                {
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                          Date, "Advance, " + remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase,
                                          ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Installment (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
                }
                #endregion
            }
        }
        else // For Purchase Return
        {
            foreach (GridViewRow dr in GrdCreditReturn.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    ItemLogID = long.Parse(dr.Cells[7].Text);
                    string manualNo = dr.Cells[1].Text;
                    DocumentNo = dr.Cells[1].Text;
                    //if (DrpAccountType.SelectedIndex == 0)
                    //{
                    ChequeNo = txtChequeNumber.Text;

                    //}
                    if (Convert.ToString(dr.Cells[1].Text) == "opng")
                    {
                        manualNo = "opng";
                    }
                    if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
                    {
                        realizeAmount += OfferAmount;
                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                        ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                         Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                         ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                        //LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                        //           Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //           ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);

                        //LController.PostingPrinvipalInvoiceAccountforCustomer_New(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                        //Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_SaleInvoice,
                        //ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "", ChequeDate, TransactionType, TYPE, Favouring, ChequeType, mConnection, mTransaction);


                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                        {
                            CequeRealizationCrossSupplier(OfferAmount, VoucherType, MaxDocumentId, Date, remarks2, ChequeDate, TransactionType, Favouring, ChequeType);
                        }
                        else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                        {
                            CequeRealizationCrossCustomer(OfferAmount, VoucherType, MaxDocumentId, Date, remarks2, ChequeDate, TransactionType, Favouring, ChequeType);
                        }

                        OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT = " + OfferAmount + " where PURCHASE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), con))
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
                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                         ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                        LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                        Date, remarks, DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                        ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")  //For Cross Payment Vendor
                        {
                            if (!FE)
                            {
                                CequeRealizationCrossSupplier(OfferAmount, VoucherType, MaxDocumentId, Date, remarks2, ChequeDate, TransactionType, Favouring, ChequeType);
                            }
                        }
                        else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")  //For Cross Payment Customer
                        {
                            if (!FE)
                            {
                                CequeRealizationCrossCustomer(OfferAmount, VoucherType, MaxDocumentId, Date, remarks2, ChequeDate, TransactionType, Favouring, ChequeType);
                            }
                        }

                        OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            ////, PURCHASE_CLOSED =" + 1 + " /// work for purchase closed
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT = " + 0 + " where PURCHASE_RETURN_ID=" + Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update tbl_ITEM_LOG set IsActive = 0 where ITEM_LOG_ID=" + ItemLogID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }

                }
            }
            //Advance Entry for Purchase Return
            if (realizeAmount < decimal.Parse(txtAmount.Text))
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                      Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
                LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                      Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpVendor.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                      ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
            }
        }
    }
    public void CequeRealizationCrossCustomer(decimal OfferAmount, int VoucherType, string MaxDocumentId, DateTime Date, string remarks, DateTime ChequeDate, int TransactionType, string p_Favouring, int Cheque_Type)
    {
        decimal realizeAmount2 = 0;
        string ChequeNo = null;
        if (DrpAccountType.SelectedIndex == 0)
        {
            ChequeNo = drpCheque.SelectedItem.Text;
        }
        foreach (GridViewRow row in CrossGrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                //Convert.ToInt64(CrossGrdCredit.DataKeys[row.RowIndex].Values["MASTER_ID"])
                string manualNo = row.Cells[1].Text;
                int MasterID = Convert.ToInt32(row.Cells[1].Text);


                if (Convert.ToString(row.Cells[1].Text) == "opng")
                {
                    manualNo = "opng";
                }
                if (decimal.Parse(row.Cells[4].Text) > OfferAmount)
                {
                    realizeAmount2 += OfferAmount;
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                    ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, p_Favouring, Cheque_Type);

                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                     Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                     ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, p_Favouring, Cheque_Type);

                    OfferAmount = decimal.Parse(row.Cells[4].Text) - OfferAmount;

                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + OfferAmount + " where SALE_RETURN_ID=" + Convert.ToInt64(MasterID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    break;
                }
                else if (decimal.Parse(row.Cells[4].Text) <= OfferAmount)
                {
                    realizeAmount2 += decimal.Parse(row.Cells[4].Text);
                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(row.Cells[4].Text),
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                     ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, p_Favouring, Cheque_Type);

                    LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(row.Cells[4].Text), 0,
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                    ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Cross Payment", ChequeDate, TransactionType, Constants.Document_SaleReturn, p_Favouring, Cheque_Type);

                    OfferAmount = OfferAmount - decimal.Parse(row.Cells[4].Text);
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {

                        ////, SALE_CLOSED = " + 1 + "  /// work for sale return close
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update tbl_SALE_RETURN set CREDIT_AMOUNT = " + 0 + " where SALE_RETURN_ID=" + Convert.ToInt64(MasterID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
        }
        //Advance Entry for sale return
        if (realizeAmount2 < decimal.Parse(txtAmount.Text))
        {
            //string date = Convert.ToDateTime(txtTransactionDate.Text + " 23:59:59").ToString("dd/MM/yyyy");
            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                  Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), 0, "0", int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, p_Favouring, Cheque_Type);

            LController.PostingPrinvipalInvoiceAccountforCustomer(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                  Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_SaleReturn,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), 0, "0", int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Sale Return", ChequeDate, TransactionType, Constants.Document_Advance, p_Favouring, Cheque_Type);

        }
    }
    public void CequeRealizationCrossSupplier(decimal OfferAmount, int VoucherType, string MaxDocumentId, DateTime Date, string remarks, DateTime ChequeDate, int TransactionType, string Favouring, int ChequeType)
    {
        decimal realizeAmount3 = 0;
        string DocumentNo = "0";
        string ChequeNo = null;
        ChequeNo = txtChequeNumber.Text;
        foreach (GridViewRow dr in CrossGrdCredit.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                string manualNo = dr.Cells[1].Text;
                DocumentNo = dr.Cells[1].Text;
                int MasterID = Convert.ToInt32(dr.Cells[1].Text);
                //if (DrpAccountType.SelectedIndex == 0)
                //{


                //}
                if (Convert.ToString(dr.Cells[1].Text) == "opng")
                {
                    manualNo = "opng";
                }
                if (decimal.Parse(dr.Cells[4].Text) > OfferAmount)
                {
                    realizeAmount3 += OfferAmount;
                    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                    ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                     Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                     ChequeNo, int.Parse(Session["UserId"].ToString()), MasterID, manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);


                    OfferAmount = decimal.Parse(dr.Cells[4].Text) - OfferAmount;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT = " + OfferAmount + " where PURCHASE_RETURN_ID=" + Convert.ToInt64(MasterID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    break;
                }
                else if (decimal.Parse(dr.Cells[4].Text) <= OfferAmount)
                {
                    realizeAmount3 += decimal.Parse(dr.Cells[4].Text);
                    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), decimal.Parse(dr.Cells[4].Text), 0,
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                     ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(MasterID), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                    LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, decimal.Parse(dr.Cells[4].Text),
                    Date, remarks, DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                    ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(MasterID), manualNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return", ChequeDate, TransactionType, Constants.Document_Purchase_Return, Favouring, ChequeType);

                    OfferAmount = OfferAmount - decimal.Parse(dr.Cells[4].Text);
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        ////, PURCHASE_CLOSED =" + 1 + " /// work for purchase closed
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT = " + 0 + " where PURCHASE_RETURN_ID=" + Convert.ToInt64(MasterID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }

            }
        }
        //Advance Entry for Purchase Return
        if (realizeAmount3 < decimal.Parse(txtAmount.Text))
        {
            LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, Constants.AccountReceivable, int.Parse(drpDistributor.SelectedItem.Value.ToString()), 0, OfferAmount,
                                  Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
            LController.PostingPrinvipalInvoiceAccount(VoucherType, MaxDocumentId, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()), OfferAmount, 0,
                                  Date, remarks + " (Advance)", DateTime.Now, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.Document_Purchase_Return,
                                  ChequeNo, int.Parse(Session["UserId"].ToString()), Convert.ToInt64(DocumentNo), DocumentNo, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), "", "Purchase Return (Advance)", ChequeDate, TransactionType, Constants.Document_Advance, Favouring, ChequeType);
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
            //LController
            hfChequeNo.Value = "";
            ChequeEntryController Ccontroller = new ChequeEntryController();
            int HPValue = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[10].Text);
            Session["Val"] = null;
            HFChqueProcessId.Value = GrdCheque.Rows[e.NewEditIndex].Cells[0].Text;
            //ddlAccountHead.Value = GrdCheque.Rows[e.NewEditIndex].Cells[13].Text;
            DrpVendor.Value = GrdCheque.Rows[e.NewEditIndex].Cells[1].Text;
            Session["Val"] = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
            if (HPValue != 1)
            {
                Hp.SelectedValue = "2";
                SelectCreditInvoice();
                PanelItem.Visible = true;
                int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                Vendor(VendorID);
                DrpBankAccount.SelectedIndex = 0;
            }
            else
            {
                int Transaction_Type = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[11].Text);
                if (Transaction_Type == 3)
                {
                    int Cross_Payment_Type = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[12].Text);
                    if (Cross_Payment_Type == 2) // For Supplier 
                    {
                        ddlCrossPAyment.Value = GrdCheque.Rows[e.NewEditIndex].Cells[12].Text;
                        Vendor(int.Parse(GrdCheque.Rows[e.NewEditIndex].Cells[1].Text));
                        DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                    }
                    else if (Cross_Payment_Type == 3)  // For Customer
                    {
                        ddlCrossPAyment.Value = GrdCheque.Rows[e.NewEditIndex].Cells[12].Text;
                        Customers();
                        DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                    }
                    else
                    {
                        ddlCrossPAyment.Value = GrdCheque.Rows[e.NewEditIndex].Cells[12].Text;
                        Hp.SelectedValue = "1";
                        SelectCreditInvoice();
                        PanelItem.Visible = false;
                        LoadAccountHead(1);
                        DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                    }
                }
                else
                {
                    Hp.SelectedValue = "1";
                    SelectCreditInvoice();
                    PanelItem.Visible = false;
                    //LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
                    DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;
                }
            }
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                LoadCheque(5);
            }
            int TType = Convert.ToInt32(GrdCheque.Rows[e.NewEditIndex].Cells[13].Text);
            if (TType == 18 || TType == 44 || TType == 46)
            {
                //PaymentType();
                if (TType == 18)
                {
                    DrpAccountType.SelectedIndex = 0;
                }
                else if (TType == 44)
                {
                    DrpAccountType.SelectedIndex = 3;
                }
                else if (TType == 46)
                {
                    DrpAccountType.SelectedIndex = 4;
                }
                //toggleControls(TType.ToString());
            }
            else
            {
                PaymentType();
                DrpAccountType.SelectedIndex = 3;
                toggleControls(TType.ToString());
            }

            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (TType == 18 || TType == 44)
                {
                    //////DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 4);
                    ////DataTable dt = LController.SelectChequeInvoiceUpdate(long.Parse(HFChqueProcessId.Value), 0);
                    ////string ChqNoList = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
                    //////string[] values = ChqNoList.Split(',');
                    //////for (int i = 0; i < values.Length; i++)
                    //////{
                    //////    values[i] = values[i].Trim();
                    //////    string ChkNo = values[i];
                    //////DataRow[] drr = dt.Select("CHEQUE_NO='" + ChqNoList + "'");
                    ////    foreach (GridViewRow dr in GridChqPayOrder.Rows)
                    ////    {
                    ////    foreach (DataRow dbr in dt.Rows)
                    ////    {
                    ////        if (Convert.ToInt32(GridChqPayOrder.DataKeys[dr.RowIndex].Values["CHEQUE_PROCESS_ID"].ToString()) == Convert.ToInt32(dbr["CHEQUE_PROCESS_ID"].ToString()))
                    ////        {
                    ////            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    ////            chRelized.Checked = true;
                    ////        }
                    ////    }
                    ////}
                    //}

                    //DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 4);
                    ////drpCheque.SelectedIndex = drpCheque.Items.IndexOf(drpCheque.Items.FindByText(GrdCheque.Rows[e.NewEditIndex].Cells[3].Text.ToString()));
                    ///////////////////////////////////////
                    //string ChqNoList = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
                    //string[] values = ChqNoList.Split(',');
                    //for (int i = 0; i < values.Length; i++)
                    //{
                    //    values[i] = values[i].Trim();
                    //    string ChkNo = values[i];
                    //    DataRow[] dr = dt.Select("CHEQUE_NO='" + ChkNo + "'");
                    //    if (dr.Length > 0)
                    //    {
                    //        foreach (GridViewRow drr in GridChqPayOrder.Rows)
                    //        {
                    //                if (Convert.ToInt32(GridChqPayOrder.DataKeys[drr.RowIndex].Values["CHEQUE_PROCESS_ID"]) == Convert.ToInt32(dr[0]["CHEQUE_PROCESS_ID"]))
                    //                {
                    //                    CheckBox chRelized = (CheckBox)drr.Cells[0].FindControl("ChbIsAssigned");
                    //                    chRelized.Checked = true;
                    //                }
                    //        }
                    //    }

                    //}

                    lblChequeNo.Visible = false;
                    drpCheque.Visible = false;
                    PanelOpenChqPayOrder.Visible = true;
                }
                else if (TType == 46)
                {
                    DataTable dt = (DataTable)Session["drpCheque"];
                    DataRow[] dtr = dt.Select("CHEQUE_NO='" + GrdCheque.Rows[e.NewEditIndex].Cells[3].Text + "'");
                    if (dtr.Any())
                    {
                        int Cheque_ID = int.Parse(dtr[0]["CHEQUE_ID"].ToString());
                        drpCheque.Value = Convert.ToString(Cheque_ID);
                        Session["drpCheque"] = null;
                    }
                    lblChequeNo.Visible = true;
                    drpCheque.Visible = true;
                    PanelOpenChqPayOrder.Visible = false;
                }
                else
                {
                    txtChequeNumber.Text = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
                    lblChequeNo.Visible = true;
                    drpCheque.Visible = true;
                    PanelOpenChqPayOrder.Visible = false;
                }
            }
            else
            {
                txtChequeNumber.Text = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
            }


            txtStartDate.Text = GrdCheque.Rows[e.NewEditIndex].Cells[5].Text;
            txtReceivedDate.Text = GrdCheque.Rows[e.NewEditIndex].Cells[6].Text;
            string Amount = GrdCheque.Rows[e.NewEditIndex].Cells[7].Text;
            decimal AMOUNT = Math.Round(decimal.Parse(Amount), 0);
            txtAmount.Text = AMOUNT.ToString();
            txtRemarks.Text = GrdCheque.Rows[e.NewEditIndex].Cells[8].Text.Replace("&nbsp;", "");
            //ChkIsDiscount.Checked = Convert.ToBoolean(GrdCheque.Rows[e.NewEditIndex].Cells[11].Text);
            //txtDiscount.Text = GrdCheque.Rows[e.NewEditIndex].Cells[10].Text.Replace("&nbsp;", "");
            txtFavouring.Text = GrdCheque.Rows[e.NewEditIndex].Cells[14].Text.Replace("&nbsp;", "");

            btnSave.Text = "Update";
            SelectCreditInvoice();
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "46")
                {
                    DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                    foreach (GridViewRow dr in GrdCredit.Rows)
                    {
                        foreach (DataRow dbr in dt.Rows)
                        {

                            if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 2) // For Purchase
                            {
                                if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]) == Convert.ToInt64(dbr["PURCHASE_INVOICE_ID"]))
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    chRelized.Checked = true;
                                }
                            }
                            else // For Sales Return
                            {
                                if (Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]) == Convert.ToInt64(dbr["SALE_RETURN_ID"]))
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    chRelized.Checked = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                #region Purchase Return
                if (DrpAccountType.SelectedItem.Value.ToString() == "18")
                {
                    if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                    {
                        //////////// For Main Invoice //////////////
                        DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                        foreach (GridViewRow dr in GrdCreditReturn.Rows)
                        {
                            foreach (DataRow dbr in dt.Rows)
                            {
                                if (Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    chRelized.Checked = true;
                                }
                            }
                        }
                        ///////////////////// For Cross Invoices /////////////////////
                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
                        {
                            DataTable dtt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 2);
                            foreach (GridViewRow dr in CrossGrdCredit.Rows)
                            {
                                foreach (DataRow dbr in dtt.Rows)
                                {
                                    if (Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        chRelized.Checked = true;
                                    }
                                }
                            }
                            CrossPanel1.Visible = true;
                            CrossPanel2.Visible = true;
                        }
                        else
                        {
                            DataTable dtt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 2);
                            foreach (GridViewRow dr in CrossGrdCredit.Rows)
                            {
                                foreach (DataRow dbr in dtt.Rows)
                                {
                                    if (Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"]) == Convert.ToInt64(dbr["SALE_RETURN_ID"]))
                                    {
                                        CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                        chRelized.Checked = true;
                                    }
                                }
                            }
                            CrossPanel1.Visible = true;
                            CrossPanel2.Visible = true;
                        }
                    }
                    else
                    {
                        DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                        foreach (GridViewRow dr in GrdCreditReturn.Rows)
                        {
                            foreach (DataRow dbr in dt.Rows)
                            {
                                if (Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    chRelized.Checked = true;
                                }
                            }
                        }
                        CrossPanel1.Visible = false;
                        CrossPanel2.Visible = false;
                    }

                }
                else
                {
                    DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
                    foreach (GridViewRow dr in GrdCreditReturn.Rows)
                    {
                        foreach (DataRow dbr in dt.Rows)
                        {
                            if (Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]) == Convert.ToInt64(dbr["PURCHASE_RETURN_ID"]))
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                chRelized.Checked = true;
                            }
                        }
                    }
                    CrossPanel1.Visible = false;
                    CrossPanel2.Visible = false;
                }
                #endregion
            }

            hfChequeNo.Value = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text.ToString();
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
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
            {
                Vendor(Convert.ToInt32(DrpVendor.SelectedItem.Value));
                DrpBankAccount.SelectedIndex = 0;
            }
        }
        else
        {
            VendorItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
        }
    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {

        SelectCreditInvoice();
        LoadReceviedCheque();
        DrpBankAccount_SelectedIndexChanged(null, null);
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
            //if (btnSave.Text == "Save")
            //{
            //    LoadReceviedCheque();
            //}
            //LoadCheque(2);
            //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
            //{
            //}
            //else
            if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
            {
                int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                LoadCrossSupplierInvoices(ID);
            }
            else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
            {
                int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                LoadCrossSaleInvoices(ID);
            }
            else
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase
                {
                    LoadCheque(2);
                }
                else
                {
                    // For Purchase Return
                    LoadCheque(2);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        }
    }
    protected void DrpAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadAccountHead();
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
    private string GetChqPayOrder()
    {
        string ChqNo = "";
        string CHEQUE_PROCESS_IDs = "";
        foreach (GridViewRow dr in GridChqPayOrder.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                if (dr.Cells[4].Text != "&nbsp;" || dr.Cells[4].Text != "" || dr.Cells[4].Text != null)
                {
                    ChqNo += ", " + dr.Cells[4].Text.Replace("&nbsp;", "0");
                }
                CHEQUE_PROCESS_IDs += ", " + dr.Cells[1].Text;
            }
        }
        if (ChqNo != "")
        {
            ChqNo = ChqNo.Remove(0, 2);
        }
        else
        {
            ChqNo = "";
        }
        if (CHEQUE_PROCESS_IDs != "")
        {
            CHEQUE_PROCESS_IDs = CHEQUE_PROCESS_IDs.Remove(0, 2);
            Session.Add("CHEQUE_PROCESS_IDs", CHEQUE_PROCESS_IDs);
        }
        else
        {
            CHEQUE_PROCESS_IDs = "";
            Session.Add("CHEQUE_PROCESS_IDs", CHEQUE_PROCESS_IDs);
        }
        return ChqNo;
    }
    private string ChequeNO()
    {
        string ChequeNo;
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3) // On Open Cheque & Pay Order
            {
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
                {
                    ChequeNo = GetChqPayOrder();
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
        }
        else  // For Purchase Return
        {
            ChequeNo = txtChequeNumber.Text;
        }
        //if (DrpAccountType.SelectedIndex == 0)
        //{
        //    if (DrpAccountType.SelectedItem.Value.ToString() == "18") //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
        //    {
        //        ChequeNo = GetChqPayOrder();
        //    }
        //    else
        //    {
        //        ChequeNo = drpCheque.SelectedItem.Text;
        //    }
        //}
        //else if (DrpAccountType.SelectedIndex == 3)
        //{
        //    if (DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" && 
        //    {
        //        ChequeNo = GetChqPayOrder();
        //    }
        //    else
        //    {
        //        ChequeNo = drpCheque.SelectedItem.Text;
        //    }
        //}
        //else if (DrpAccountType.SelectedIndex == 4)
        //{
        //    ChequeNo = drpCheque.SelectedItem.Text;
        //}
        //else
        //{
        //    ChequeNo = txtChequeNumber.Text;
        //}
        return ChequeNo;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Session["ITEM_NAME"] = null;
        Session["Vendor_Name"] = null;
        Session["CrossITEM_NAME"] = null;
        Session["CrossVendor_Name"] = null;
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        ChequeEntryController CController = new ChequeEntryController();
        DateTime ChequeDate;
        int InvoiceCount = Constants.IntNullValue;

        #region Check
        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
        {
            if (drpCheque.Items.Count == 0)
            {
                if (DrpBankAccount.SelectedItem.Value.ToString().Equals(""))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select Cheque No first.');", true);
                    drpCheque.Focus();
                    return;
                }

            }
        }
        #endregion

        #region Set Cheque Date
        try
        {
            if (txtStartDate.Text.Length == 10)
            {
                string StartDate = txtStartDate.Text.Replace("-", "/");
                ChequeDate = DateTime.Parse(ConvertDate.British_To_American(StartDate));
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
        #endregion

        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            #region For Purchase
            if (Hp.SelectedValue != "2")
            {
                #region For Purchase Transections
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
                    DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" &&
                {
                    if (CheckOpenChqPay())
                    {
                        string type = "";
                        if (DrpAccountType.SelectedItem.Value.ToString() == "44")
                        {
                            type = "a Pay Order.";
                        }
                        else
                        {
                            type = "an Open Cheque.";
                        }
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select " + type + "');", true);
                        return;
                    }
                }
                #endregion
            }
            else
            {
                #region For Installments
                if (CheckInvoiceInstallment())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an Installment');", true);
                    return;
                }
                #endregion
            }
            #endregion
        }
        else // For Purchase Return
        {
            #region Purchase Return
            if (Hp.SelectedValue != "2")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    if (CheckInvoiceReturn())
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please Select An Invoice');", true);
                        return;
                    }
                    if (CrossCheckInvoice())
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please Select Cross Invoice');", true);
                        return;
                    }
                }
                else
                {
                    if (CheckInvoiceReturn())
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please Select An Invoice');", true);
                        return;
                    }
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
            if (btnSave.Text == "Save")
            {
                #region Save
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
                {
                    if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 2 || DrpAccountType.SelectedIndex == 5 || DrpAccountType.SelectedIndex == 6)//on Cash Realization check Invoice Selection
                    {
                        #region Cash Realization
                        foreach (GridViewRow dr in GrdCredit.Rows)
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            if (chRelized.Checked == true)
                            {
                                InvoiceCount++;
                                Session["ITEM_NAME"] = dr.Cells[2].Text;
                                Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                                Session["CrossITEM_NAME"] = "";
                                Session["CrossVendor_Name"] = "";
                                break;
                            }
                        }
                        if (InvoiceCount == Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                        {
                            Session["ITEM_NAME"] = "";
                            Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                            Session["CrossITEM_NAME"] = "";
                            Session["CrossVendor_Name"] = "";
                            if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase Only
                            {
                                CashAdvance(2);
                                //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                                //{
                                //    InsertGL2(null);
                                //}
                                Session.Remove("VoucherNo");
                            }
                        }
                        else if (InvoiceCount != Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
                        {
                            ChequeRealization(2); // Used as All Cash, Online, Cheque
                            //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                            //{
                            //    string ChequeNo = null;
                            //    ChequeNo = ChequeNO();
                            //    InsertGL2(ChequeNo);
                            //}
                            Session.Remove("VoucherNo");
                            SelectCreditInvoice();
                        }
                        #endregion
                    }
                    else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4) //on Cash Realization check Invoice Selection
                    {
                        #region Cheque Realization
                        string Ids = "";
                        if (Session["ChequeIDs"] == null)
                        {
                            Ids = "";
                        }
                        else
                        {
                            Ids = Session["ChequeIDs"].ToString();
                        }
                        checkDuplication();
                        string ChequeNo = null;
                        ChequeNo = ChequeNO();
                        int Customer_ID = 0;
                        int Vendor_ID = 0;
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase
                        {
                            Customer_ID = 0;
                            Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                        }
                        else if (ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Sale Return
                        {
                            Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                            Vendor_ID = 0;
                        }
                        if (DrpStatus.SelectedIndex == 0)
                        {
                            string Favouring = txtFavouring.Text;
                            int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
                            HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, ChequeNo, txtBankName.Text, ChequeDate,
                                Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now, int.Parse(Session["UserId"].ToString())
                                , Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1, Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value)
                                , Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Favouring, ChequeType, 0, Ids);

                            foreach (GridViewRow dr in GrdCredit.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 2) // For Purchase
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                    }
                                    else // For Sales Return
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                    }
                                }
                            }
                            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
                            {
                                foreach (GridViewRow dr in GridChqPayOrder.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        CController.InsertOpenChequePayOrder(long.Parse(HFChqueProcessId.Value), long.Parse(dr.Cells[1].Text), int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
                                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                        {
                                            con.Open();
                                            using (SqlCommand command3 = new SqlCommand("UPDATE CHEQUE_PROCESS set ISSUED =" + 1 + " where CHEQUE_PROCESS_ID=" + Convert.ToInt64(dr.Cells[1].Text), con))
                                            {
                                                command3.ExecuteNonQuery();
                                            }
                                            con.Close();
                                        }
                                    }
                                }
                            }
                            //PrintChequeVoucher(HFChqueProcessId.Value);
                        }
                        SelectCreditInvoice();
                        LoadCheque(2);
                        txtChequeNumber.Text = "";
                        #endregion
                    }
                }
                else // For Purchase Return
                {
                    #region Purchase Return
                    if (DrpAccountType.SelectedIndex == 1 || DrpAccountType.SelectedIndex == 2 || DrpAccountType.SelectedIndex == 4)// On Cash Receipt, Online Transfer & Other Bank Transfer
                    {
                        foreach (GridViewRow dr in GrdCreditReturn.Rows)
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            if (chRelized.Checked == true)
                            {
                                InvoiceCount++;
                                Session["ITEM_NAME"] = dr.Cells[2].Text;
                                Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
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
                            //string ChequeNo = null;
                            //ChequeNo = ChequeNO();
                            ////if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3)
                            ////{
                            ////    ChequeNo = drpCheque.SelectedItem.Text;
                            ////}
                            ////else if (DrpAccountType.SelectedIndex == 3)
                            ////{
                            ////    ChequeNo = txtChequeNumber.Text;
                            ////}
                            //InsertGL2(ChequeNo);
                            ////}
                            ////}

                            Session.Remove("VoucherNo");
                            SelectCreditInvoice();
                            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                            {
                                //LoadCrossSaleInvoices(int.Parse(DrpBankAccount.SelectedItem.Value.ToString()));
                                CrossPayments();
                            }
                        }
                    }
                    else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3) // On Open Cheque & Pay Order
                    {
                        checkDuplication();
                        string ChequeNo = null;
                        ChequeNo = ChequeNO();
                        //if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3)
                        //{
                        //    ChequeNo = drpCheque.SelectedItem.Text;
                        //}
                        //else if (DrpAccountType.SelectedIndex == 3)
                        //{
                        //    ChequeNo = txtChequeNumber.Text;
                        //}
                        if (DrpStatus.SelectedIndex == 0)
                        {
                            string Favouring = txtFavouring.Text;
                            int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
                            //HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, ChequeNo, txtBankName.Text, ChequeDate,
                            //    Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                            //    , Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1
                            //    , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtTax.Text))
                            //    , Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Favouring, ChequeType, 0, Session["ChequeIDs"].ToString());
                            HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, txtChequeNumber.Text, txtBankName.Text, ChequeDate,
                                        Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                                        , Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1
                                        , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), short.Parse(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtTax.Text))
                                        , Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Favouring, ChequeType, 0, "");
                            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                            {
                                //////// For Main Invoice ///////////////
                                foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                    }
                                }
                                //////// For Cross Invoice ///////////////
                                foreach (GridViewRow dr in CrossGrdCredit.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
                                        { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"])); }
                                        else
                                        { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"]), Constants.LongNullValue); }
                                    }
                                }
                            }
                            else
                            {
                                foreach (GridViewRow dr in GrdCreditReturn.Rows)
                                {
                                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                    if (chRelized.Checked == true)
                                    {
                                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                    }
                                }
                            }

                            //PrintChequeVoucher(HFChqueProcessId.Value);
                        }
                        txtChequeNumber.Text = "";
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
                {
                    string ChequeNo = null;
                    if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3 || DrpAccountType.SelectedIndex == 4)
                    {
                        ChequeNo = ChequeNO();
                        if (DrpAccountType.SelectedItem.Value.ToString() == "18") //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
                        {

                        }
                        else if (DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" && 
                        {

                        }
                        else
                        {
                            DataTable dt = _bnkController.SelectCheckBook(Convert.ToInt32(drpCheque.SelectedItem.Value), Constants.IntNullValue, null, null, null, true, Constants.DateNullValue, 4);
                            if (dt.Rows.Count > 0)
                            {

                                if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) == 0 && DrpStatus.SelectedIndex != 0)
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected cheque is not issued yet');", true);
                                    return;
                                }
                            }
                        }
                    }
                    string Ids = "";
                    if (Session["ChequeIDs"] == null)
                    {
                        Ids = "";
                    }
                    else
                    {
                        Ids = Session["ChequeIDs"].ToString();
                    }
                    if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == txtTransactionDate.Text)
                    {
                        if (Session["ChequeIDs"].ToString() != Session["ChequeProccessIDs"].ToString())
                        {
                            string ChequeProccessIDs = Session["ChequeProccessIDs"].ToString();
                            string[] IDs = ChequeProccessIDs.Split(',');
                            foreach (string ID in IDs)
                            {
                                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                {
                                    con.Open();
                                    using (SqlCommand command3 = new SqlCommand("update CHEQUE_PROCESS set ISSUED =" + 0 + " where CHEQUE_PROCESS_ID=" + Convert.ToInt64(ID), con))
                                    {
                                        command3.ExecuteNonQuery();
                                    }
                                    con.Close();
                                }
                            }

                        }
                        int Customer_ID = 0;
                        int Vendor_ID = 0;
                        if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase
                        {
                            Customer_ID = 0;
                            Vendor_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                        }
                        else if (ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Sale Return
                        {
                            Customer_ID = int.Parse(DrpVendor.SelectedItem.Value.ToString());
                            Vendor_ID = 0;
                        }
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), Vendor_ID, Customer_ID, ChequeNo, txtBankName.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString())
                           , txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value)
                           , Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Ids);

                        CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                        foreach (GridViewRow dr in GrdCredit.Rows)
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            if (chRelized.Checked == true)
                            {
                                if (Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]) == 2) // For Purchase
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                }
                                else // For Sales Return
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                }
                            }
                        }
                        //if (DrpBankAccount.SelectedItem.Value.ToString() == "84" && DrpAccountType.SelectedItem.Value.ToString() == "18" ||
                        //        DrpBankAccount.SelectedItem.Value.ToString() == "85" && DrpAccountType.SelectedItem.Value.ToString() == "44")
                        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
                        {
                            foreach (GridViewRow dr in GridChqPayOrder.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    CController.InsertOpenChequePayOrder(long.Parse(HFChqueProcessId.Value), long.Parse(dr.Cells[1].Text), int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command3 = new SqlCommand("update CHEQUE_PROCESS set ISSUED =" + 1 + " where CHEQUE_PROCESS_ID=" + Convert.ToInt64(dr.Cells[1].Text), con))
                                        {
                                            command3.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                }
                            }
                        }
                    }

                    else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                    {
                        ChequeNo = ChequeNO();
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, ChequeNo, null, Constants.DateNullValue, Constants.DateNullValue
                            , Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString())
                            , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue
                            , Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Ids);
                    }
                    else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                    {

                        ChequeNo = ChequeNO();
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, ChequeNo, null, Constants.DateNullValue, Constants.DateNullValue
                            , Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString())
                            , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue
                            , Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Ids);
                        CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                        Session["CHEQUE_PROCESS_IDs"] = HFChqueProcessId.Value;
                        foreach (GridViewRow dr in GrdCredit.Rows)
                        {
                            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            if (chRelized.Checked == true)
                            {
                                int TRANSACTION_TYPE_ID = Convert.ToInt32(dr.Cells[9].Text); //Convert.ToInt32(GrdCredit.DataKeys[dr.RowIndex].Values["TRANSACTION_TYPE_ID"]);
                                if (TRANSACTION_TYPE_ID == 2) // For Purchase
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.IntNullValue);
                                }
                                else // For Sales Return
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["PURCHASE_DETAIL_ID"]), Constants.LongNullValue, Constants.IntNullValue);
                                }
                            }
                        }
                        //if (DrpBankAccount.SelectedItem.Value.ToString() == "84" && DrpAccountType.SelectedItem.Value.ToString() == "18" ||
                        //        DrpBankAccount.SelectedItem.Value.ToString() == "85" && DrpAccountType.SelectedItem.Value.ToString() == "44")
                        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
                        {
                            foreach (GridViewRow dr in GridChqPayOrder.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked != true)
                                {
                                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                                    {
                                        con.Open();
                                        using (SqlCommand command3 = new SqlCommand("update CHEQUE_PROCESS set ISSUED =" + 0 + " where CHEQUE_PROCESS_ID=" + Convert.ToInt64(dr.Cells[1].Text), con))
                                        {
                                            command3.ExecuteNonQuery();
                                        }
                                        con.Close();
                                    }
                                }
                            }
                        }
                        Session["ITEM_NAME"] = "";
                        Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
                        Session["CrossITEM_NAME"] = "";
                        Session["CrossVendor_Name"] = "";
                        ChequeRealization(1);
                        //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
                        //{
                        //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                        //{
                        //    InsertGL();
                        //}
                        //}

                        Session.Remove("VoucherNo");
                        SelectCreditInvoice();
                    }
                }
                else // For Purchase Return
                {
                    DataTable dt = _bnkController.SelectCheckBook(Convert.ToInt32(txtChequeNumber.Text), Constants.IntNullValue, null, null, null, true, Constants.DateNullValue, 4);
                    if (dt.Rows.Count > 0)
                    {
                        //if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) == 0 && DrpStatus.SelectedIndex != 0)
                        //{
                        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected cheque is not issued yet');", true);
                        //    return;
                        //}
                    }
                    if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == txtTransactionDate.Text)
                    {
                        //CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, txtChequeNumber.Text, txtBankName.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue,
                        //   Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                        //   , txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                        //   , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtTax.Text))
                        //   , Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Session["ChequeIDs"].ToString());
                        short chkDiscount = 0;
                        if (ChkIsDiscount.Checked)
                        {
                            chkDiscount = 1;
                        }
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, txtChequeNumber.Text, txtBankName.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue
                            , int.Parse(Session["UserId"].ToString()), txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), chkDiscount
                            , Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlAccountHead.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), "");

                        if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                        {
                            //////// For Main Invoice ///////////////
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                }
                            }
                            //////// For Cross Invoice ///////////////
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 3);
                            foreach (GridViewRow dr in CrossGrdCredit.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
                                    { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"])); }
                                    else
                                    { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"]), Constants.LongNullValue); }
                                }
                            }
                        }
                        else
                        {
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                }
                            }
                        }

                    }
                    else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
                    {
                        //CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date,
                        //                                 Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                        //                                 , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                        //                                 , Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue
                        //                                 , int.Parse(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Session["ChequeIDs"].ToString());
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString())
                            , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");
                    }
                    else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
                    {
                        //CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date,
                        //                                 Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString()) //DrpAccountType.SelectedIndex
                        //                                 , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
                        //                                 , Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue
                        //                                 , int.Parse(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value), Session["ChequeIDs"].ToString());
                        CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, txtChequeNumber.Text, null, Constants.DateNullValue, Constants.DateNullValue, Constants.DateNullValue, Date, Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, int.Parse(Session["UserId"].ToString())
                            , txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), Constants.DecimalNullValue, Constants.ShortNullValue, Constants.DecimalNullValue, Constants.IntNullValue, Convert.ToString(hfChequeNo.Value), "");
                        CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                        Session["CHEQUE_PROCESS_IDs"] = HFChqueProcessId.Value;
                        if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                        {
                            //////// For Main Invoice ///////////////
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                }
                            }
                            //////// For Cross Invoice ///////////////
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 3);
                            foreach (GridViewRow dr in CrossGrdCredit.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
                                    { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"])); }
                                    else
                                    { CController.InsertChequeEntryCrossInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(CrossGrdCredit.DataKeys[dr.RowIndex].Values["MASTER_ID"]), Constants.LongNullValue); }
                                }
                            }
                        }
                        else
                        {
                            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
                            foreach (GridViewRow dr in GrdCreditReturn.Rows)
                            {
                                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                                if (chRelized.Checked == true)
                                {
                                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue, Convert.ToInt64(GrdCreditReturn.DataKeys[dr.RowIndex].Values["PURCHASE_RETURN_ID"]), Constants.IntNullValue);
                                }
                            }
                        }
                        ChequeRealization(1);
                        //if (Convert.ToBoolean(Session["IsFinanceSetting"]))
                        //{
                        //    InsertGL();
                        //}
                        Session.Remove("VoucherNo");
                        SelectCreditInvoice();
                    }
                }
            }
        }
        else // Installments
        {
            #region Installment Code
            //if (btnSave.Text == "Save")
            //{
            //    if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Sale
            //    {
            //        if (DrpAccountType.SelectedIndex != 0)//on Cash Realization check Invoice Selection
            //        {
            //            //if (ddlPaymentDesc.SelectedItem.Value.ToString() == "1") // Just for Installments
            //            //{
            //            foreach (GridViewRow dr in GrdCreditInstallment.Rows)
            //            {
            //                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //                if (chRelized.Checked == true)
            //                {
            //                    InvoiceCount++;
            //                    Session["ITEM_NAME"] = ddlItem.SelectedItem.Text;
            //                    Session["Vendor_Name"] = DrpVendor.SelectedItem.Text;
            //                    break;
            //                }
            //            }
            //            if (InvoiceCount == Constants.IntNullValue)
            //            {
            //                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an installment');", true);
            //                return;
            //            }
            //            //}

            //            //if (InvoiceCount == Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
            //            //    {
            //            //        CashAdvance();
            //            //        //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
            //            //        //{
            //            //            if (Convert.ToBoolean(Session["IsFinanceSetting"]))
            //            //            {
            //            //                InsertGL2(null);
            //            //            }
            //            //        //}

            //            //        Session.Remove("VoucherNo");
            //            //    }
            //            //    else 
            //            if (InvoiceCount != Constants.IntNullValue && DrpAccountType.SelectedIndex != 0)
            //            {
            //                ChequeRealization();// used as All cash, online, cheque
            //                                    //if (ddlCrossPAyment.SelectedItem.Value.ToString() == "1")
            //                                    //{
            //                if (Convert.ToBoolean(Session["IsFinanceSetting"]))
            //                {
            //                    string ChequeNo = null;
            //                    if (DrpAccountType.SelectedIndex == 0)
            //                    {
            //                        ChequeNo = txtChequeNumber.Text;
            //                    }
            //                    else if (DrpAccountType.SelectedIndex == 3)
            //                    {
            //                        ChequeNo = txtChequeNumber.Text;
            //                    }
            //                    InsertGL2(ChequeNo);
            //                }
            //                //}
            //                Session.Remove("VoucherNo");
            //                SelectCreditInvoice();
            //            }
            //        }
            //        else if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3)//on Cash Realization check Invoice Selection
            //        {
            //            checkDuplication();
            //            string ChequeNo = null;
            //            if (DrpAccountType.SelectedIndex == 0)
            //            {
            //                ChequeNo = txtChequeNumber.Text;
            //            }
            //            else if (DrpAccountType.SelectedIndex == 3)
            //            {
            //                ChequeNo = txtChequeNumber.Text;
            //            }
            //            if (DrpStatus.SelectedIndex == 0)
            //            {
            //                string Favouring = txtFavouring.Text;
            //                int ChequeType = int.Parse(DrpAccountType.SelectedItem.Value.ToString());
            //                HFChqueProcessId.Value = CController.InsertChequeEntry(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, ChequeNo, txtBankName.Text, ChequeDate,
            //                    Date, Constants.DateNullValue, Constants.DateNullValue, Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), DateTime.Now, DrpAccountType.SelectedIndex, Hp.SelectedValue, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 1
            //                    , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Favouring, ChequeType);
            //                foreach (GridViewRow dr in GrdCredit.Rows)
            //                {
            //                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //                    if (chRelized.Checked == true)
            //                    {
            //                        CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue);
            //                    }
            //                }
            //                //PrintChequeVoucher(HFChqueProcessId.Value);
            //            }
            //            SelectCreditInvoice();
            //            txtChequeNumber.Text = "";
            //        }
            //    }
            //    else // For Sale Return
            //    {

            //    }

            //}
            //else
            //{
            //    if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase Installment Update
            //    {
            //        string ChequeNo = null;
            //        if (DrpAccountType.SelectedIndex == 0)
            //        {
            //            ChequeNo = txtChequeNumber.Text;
            //        }
            //        else if (DrpAccountType.SelectedIndex == 3)
            //        {
            //            ChequeNo = txtChequeNumber.Text;
            //        }
            //        DataTable dt = _bnkController.SelectCheckBook(Convert.ToInt32(ChequeNo), Constants.IntNullValue, null, null, null, true, Constants.DateNullValue, 4);
            //        if (dt.Rows.Count > 0)
            //        {
            //            if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) == 0 && DrpStatus.SelectedIndex != 0)
            //            {
            //                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected cheque is not issued yet');", true);
            //                return;
            //            }
            //        }
            //        if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Pending && txtReceivedDate.Text == DateTime.Parse(txtTransactionDate.Text).ToString("dd-MM-yyyy"))
            //        {

            //            CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), 0, ChequeNo, DrpBankAccount.SelectedItem.Text, ChequeDate, Date, Constants.DateNullValue, Constants.DateNullValue,
            //               Convert.ToDecimal(dc.chkNull_0(txtAmount.Text)), int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, DrpAccountType.SelectedIndex, txtRemarks.Text, int.Parse(DrpBankAccount.SelectedItem.Value.ToString())
            //               , Convert.ToDecimal(dc.chkNull_0(txtDiscount.Text)), Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtTax.Text)), Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value));
            //            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
            //            foreach (GridViewRow dr in GrdCredit.Rows)
            //            {
            //                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //                if (chRelized.Checked == true)
            //                {
            //                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue);
            //                }
            //            }
            //        }
            //        else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Bons || int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Cancel)
            //        {
            //            CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, ChequeNo, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
            //                                             Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, DrpAccountType.SelectedIndex, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
            //                                             , Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue, Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value));
            //        }
            //        else if (int.Parse(DrpStatus.SelectedItem.Value.ToString()) == Constants.Cheque_Clear)
            //        {
            //            CController.UpdateChequeEntry(long.Parse(HFChqueProcessId.Value), Constants.IntNullValue, Constants.IntNullValue, Constants.LongNullValue, ChequeNo, null, ChequeDate, Constants.DateNullValue, Constants.DateNullValue, Date,
            //                                             Constants.DecimalNullValue, int.Parse(DrpStatus.SelectedItem.Value.ToString()), Constants.DateNullValue, Hp.SelectedValue, DrpAccountType.SelectedIndex, txtRemarks.Text, long.Parse(DrpBankAccount.SelectedItem.Value.ToString())
            //                                             , Constants.DecimalNullValue, Convert.ToInt16(ddlTransactionType.SelectedItem.Value), Constants.DecimalNullValue, Convert.ToInt32(ddlCrossPAyment.SelectedItem.Value.ToString()), Convert.ToString(hfChequeNo.Value));
            //            CController.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 1);
            //            foreach (GridViewRow dr in GrdCredit.Rows)
            //            {
            //                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            //                if (chRelized.Checked == true)
            //                {
            //                    CController.InsertChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), Convert.ToInt64(GrdCredit.DataKeys[dr.RowIndex].Values["SALE_DETAIL_ID"]), Constants.LongNullValue, Constants.LongNullValue, Constants.LongNullValue);
            //                }
            //            }
            //            ChequeRealization();
            //            if (Convert.ToBoolean(Session["IsFinanceSetting"]))
            //            {
            //                InsertGL();
            //            }
            //            Session.Remove("VoucherNo");
            //            SelectCreditInvoice();
            //        }
            //    }
            //    else  // For Purchase Return
            //    {

            //    }
            //}
            #endregion

        }
        //}

        //}
        //catch (Exception ex)
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('" + ex.Message.ToString() + "')", true);
        //}
        LoadCheque(2);
        ClearAll();
        LoadReceviedCheque();
        LoadPaymentRecieved();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearAll();
        LoadCheque(2);
    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["dt"];
        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
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
        string Vendor_Name = Session["Vendor_Name"].ToString();
        string CrossItem_Name = Session["CrossITEM_NAME"].ToString();
        string CrossVendor_Name = Session["CrossVendor_Name"].ToString();
        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            string ChequeNo = null;
            if (DrpAccountType.SelectedIndex == 0 || DrpAccountType.SelectedIndex == 3)
            {
                //if (DrpBankAccount.SelectedItem.Value.ToString() == "84" && DrpAccountType.SelectedItem.Value.ToString() == "18" ||
                //                DrpBankAccount.SelectedItem.Value.ToString() == "85" && DrpAccountType.SelectedItem.Value.ToString() == "44")
                if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
                {
                    foreach (GridViewRow dv in GridChqPayOrder.Rows)
                    {
                        CheckBox chRelized = (CheckBox)dv.Cells[0].FindControl("ChbIsAssigned");
                        if (chRelized.Checked == true)
                        {
                            if (dv.Cells[4].Text == "&nbsp;" || dv.Cells[4].Text == "" || dv.Cells[4].Text == null)
                            {

                            }
                            else
                            {
                                ChequeNo += ", " + dv.Cells[4].Text.Replace("&nbsp;","0");
                            }
                        }
                        
                    }
                    if (ChequeNo != "" || ChequeNo == null)
                    {
                        ChequeNo = ChequeNo.Remove(0, 2);
                    }
                    else
                    {
                        ChequeNo = "";
                    }
                }
                else
                {
                    ChequeNo = drpCheque.SelectedItem.Text;
                }

            }
            else
            {
                ChequeNo = txtChequeNumber.Text;
            }
            DataTable dtVoucher = new DataTable();
            DataTable dtVoucherCrossPayment = new DataTable();
            dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
            dtVoucher.Columns.Add("DEBIT", typeof(decimal));
            dtVoucher.Columns.Add("CREDIT", typeof(decimal));
            dtVoucher.Columns.Add("REMARKS", typeof(string));
            dtVoucher.Columns.Add("Principal_Id", typeof(int));
            
            DataRow dr = dtVoucher.NewRow();
            dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
            dr["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Paid to " + DrpVendor.SelectedItem.Text;
            dr["DEBIT"] = 0;
            dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dtVoucher.Rows.Add(dr);
            //Debit Side Entry
            DataRow dr1 = dtVoucher.NewRow();
            dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
            dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
            dr1["CREDIT"] = 0;
            dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            dr1["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Paid to " + DrpVendor.SelectedItem.Text;
            dtVoucher.Rows.Add(dr1);
            //}

            //string MaxDocumentId = "";
            //string MaxDocumentIdForCross = "";
            //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Expanse_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
            string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
            LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);

            //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true);

            //PrintVoucher(MaxDocumentId);
        }
        else // For Purchase Return
        {
            //Configuration.GetAccountHead();
            DataTable dtVoucher = new DataTable();
            DataTable dtVoucherCrossPayment = new DataTable();
            dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
            dtVoucher.Columns.Add("DEBIT", typeof(decimal));
            dtVoucher.Columns.Add("CREDIT", typeof(decimal));
            dtVoucher.Columns.Add("REMARKS", typeof(string));
            dtVoucher.Columns.Add("Principal_Id", typeof(int));

            dtVoucherCrossPayment.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
            dtVoucherCrossPayment.Columns.Add("DEBIT", typeof(decimal));
            dtVoucherCrossPayment.Columns.Add("CREDIT", typeof(decimal));
            dtVoucherCrossPayment.Columns.Add("REMARKS", typeof(string));
            dtVoucherCrossPayment.Columns.Add("Principal_Id", typeof(int));


            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Supllier to Supplier
                {
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
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
                    //DataRow dr2 = dtVoucherCrossPayment.NewRow();
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
                    //dtVoucherCrossPayment.Rows.Add(dr3);
                    dtVoucher.Rows.Add(dr3);
                }
                else  // Supllier to Customer
                {
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
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
                    //DataRow dr2 = dtVoucherCrossPayment.NewRow();
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    //dtVoucherCrossPayment.Rows.Add(dr2);
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    //DataRow dr3 = dtVoucherCrossPayment.NewRow();
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr3);
                }
            }
            else
            {
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Return from " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["CREDIT"] = 0;
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                dr1["DEBIT"] = 0;
                dr1["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpBankAccount.SelectedItem.Text + " Return from " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
            }

            //string MaxDocumentId = "";
            //string MaxDocumentIdForCross = "";
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);

                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text, Constants.DateNullValue, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentIdForCross, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["CrossVoucherNo"].ToString(), txtRemarks.Text, Constants.DateNullValue, drpCheque.SelectedItem.Text, dtVoucherCrossPayment, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
            }
            else
            {
                string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                MaxDocumentId = LController.SelectMaxVoucherId(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), 0, MaxDocumentId, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, txtChequeNumber.Text, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true);
            }
        }
        Session["ITEM_NAME"] = null;
        Session["Vendor_Name"] = null;
        Session["CrossITEM_NAME"] = null;
        Session["CrossVendor_Name"] = null;
    }
    private void InsertGL2(string ChequeNo, string MaxDocumentId, IDbConnection mConnection, IDbTransaction mTransaction)
    {
        DateTime Date = Constants.DateNullValue;
        string TransactionDate = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(TransactionDate));
        DateTime ChequeDate = Constants.DateNullValue;
        string Item_Name = Session["ITEM_NAME"].ToString();
        string Vendor_Name = Session["Vendor_Name"].ToString();
        string CrossItem_Name = Session["CrossITEM_NAME"].ToString();
        string CrossVendor_Name = Session["CrossVendor_Name"].ToString();
        string Remarks = "";
        try
        {
            if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "33" || DrpAccountType.SelectedItem.Value.ToString() == "34")
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

        if (Vendor_Name != "")
        {
            Remarks = Remarks + Vendor_Name + ", ";
        }
        if (Item_Name != "")
        {
            Remarks = Remarks + Item_Name + ", " + txtRemarks.Text;
        }
        else
        {
            Remarks = Remarks + txtRemarks.Text;
        }

        //Configuration.GetAccountHead();
        DataTable dtVoucher = new DataTable();
        DataTable dtVoucherCrossPayment = new DataTable();

        dtVoucher.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        dtVoucher.Columns.Add("DEBIT", typeof(decimal));
        dtVoucher.Columns.Add("CREDIT", typeof(decimal));
        dtVoucher.Columns.Add("REMARKS", typeof(string));
        dtVoucher.Columns.Add("Principal_Id", typeof(int));

        //dtVoucherCrossPayment.Columns.Add("ACCOUNT_HEAD_ID", typeof(long));
        //dtVoucherCrossPayment.Columns.Add("DEBIT", typeof(decimal));
        //dtVoucherCrossPayment.Columns.Add("CREDIT", typeof(decimal));
        //dtVoucherCrossPayment.Columns.Add("REMARKS", typeof(string));
        //dtVoucherCrossPayment.Columns.Add("Principal_Id", typeof(int));


        if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14") // For Purchase & Sale Return
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Supllier to Supplier
                {
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);
                    //Credit Side Entry
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);

                    /////// 2nd Voucher (Cross Payment Voucher for Supplier)
                    //Debit Side Entry
                    DataRow dr3 = dtVoucherCrossPayment.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucherCrossPayment.Rows.Add(dr3);
                    //Credit Side Entry
                    DataRow dr2 = dtVoucherCrossPayment.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucherCrossPayment.Rows.Add(dr2);

                }
                else  // Supllier to Customer
                {
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);
                    //Credit Side Entry
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);

                    /////// 2nd Voucher  (Cross Payment Voucher for Customer)
                    //Debit Side Entry
                    DataRow dr3 = dtVoucherCrossPayment.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucherCrossPayment.Rows.Add(dr3);
                    //Credit Side Entry
                    DataRow dr2 = dtVoucherCrossPayment.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    dtVoucherCrossPayment.Rows.Add(dr2);

                }
            }
            else
            {
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Paid to " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = 0;
                dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["CREDIT"] = 0;
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Paid to " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
            }
            //string MaxDocumentId = "";
            string MaxDocumentIdForCross = "";
            //using chequeDate as Transfer Date
            if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "47")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(Constants.CashPayment_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);

                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), MaxDocumentIdForCross, Constants.CashPayment_Voucher, Date, Constants.Cash_Relization, Session["CrossVoucherNo"].ToString(), txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucherCrossPayment, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " and " + MaxDocumentIdForCross + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                }
                else
                {
                    if (DrpAccountType.SelectedItem.Value.ToString() == "47")
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    else
                    {
                        LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.CashPayment_Voucher, Date, Constants.Cash_Relization, Session["VoucherNo"].ToString(), Remarks, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                }
            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    // MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(Constants.Expanse_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);

                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true, mConnection, mTransaction);
                    //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), MaxDocumentIdForCross, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["CrossVoucherNo"].ToString(), txtRemarks.Text, ChequeDate, ChequeNo, dtVoucherCrossPayment, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " and " + MaxDocumentIdForCross + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                }
                else
                {
                    //MaxDocumentId = LController.SelectMaxVoucherId(Constants.Expanse_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                    LController.Add_VoucherWithSameEntry(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Expanse_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Remarks, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true, mConnection, mTransaction);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    //PrintVoucher(MaxDocumentId);
                }
            }
        }
        else // For Purchase Return
        {
            if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2") // Supllier to Supplier
                {
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr["DEBIT"] = 0;
                    dr["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr);
                    //Debit Side Entry
                    DataRow dr1 = dtVoucher.NewRow();
                    dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr1["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr1["CREDIT"] = 0;
                    dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                    dr1["REMARKS"] = "Received from " + DrpVendor.SelectedItem.Text + " Paid to " + DrpBankAccount.SelectedItem.Text;
                    dtVoucher.Rows.Add(dr1);

                    /////// 2nd Voucher (Cross Payment Voucher for Supplier)
                    //DataRow dr2 = dtVoucherCrossPayment.NewRow();
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    //dtVoucherCrossPayment.Rows.Add(dr2);
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    //DataRow dr3 = dtVoucherCrossPayment.NewRow();
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    //dtVoucherCrossPayment.Rows.Add(dr3);
                    dtVoucher.Rows.Add(dr3);
                }
                else  // Supllier to Customer
                {
                    DataRow dr = dtVoucher.NewRow();
                    dr["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
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
                    //DataRow dr2 = dtVoucherCrossPayment.NewRow();
                    DataRow dr2 = dtVoucher.NewRow();
                    dr2["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Receivables;
                    dr2["DEBIT"] = 0;
                    dr2["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr2["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr2["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    //dtVoucherCrossPayment.Rows.Add(dr2);
                    dtVoucher.Rows.Add(dr2);
                    //Debit Side Entry
                    //DataRow dr3 = dtVoucherCrossPayment.NewRow();
                    DataRow dr3 = dtVoucher.NewRow();
                    dr3["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Settlement_Account;
                    dr3["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                    dr3["CREDIT"] = 0;
                    dr3["Principal_Id"] = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
                    dr3["REMARKS"] = "Received from " + DrpBankAccount.SelectedItem.Text;
                    //dtVoucherCrossPayment.Rows.Add(dr3);
                    dtVoucher.Rows.Add(dr3);
                }
            }
            else
            {
                DataRow dr = dtVoucher.NewRow();
                dr["ACCOUNT_HEAD_ID"] = Convert.ToInt64(DrpBankAccount.SelectedItem.Value.ToString());
                dr["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return from " + DrpVendor.SelectedItem.Text;
                dr["DEBIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr["CREDIT"] = 0;
                dr["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dtVoucher.Rows.Add(dr);
                //Debit Side Entry
                DataRow dr1 = dtVoucher.NewRow();
                dr1["ACCOUNT_HEAD_ID"] = Constants.AccountHead_Trade_Payables;
                dr1["DEBIT"] = 0;
                dr1["CREDIT"] = Convert.ToDecimal(dc.chkNull_0(txtAmount.Text));
                dr1["Principal_Id"] = Convert.ToInt32(DrpVendor.SelectedItem.Value);
                dr1["REMARKS"] = DrpAccountType.SelectedItem.Text + " Return from " + DrpVendor.SelectedItem.Text;
                dtVoucher.Rows.Add(dr1);
            }


            //string MaxDocumentId = "";
            string MaxDocumentIdForCross = "";

            //using chequeDate as Transfer Date
            if (DrpAccountType.SelectedItem.Value.ToString() == "21")
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(Constants.Cash_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);

                    LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), MaxDocumentIdForCross, Constants.Cash_Voucher, Date, Constants.CashPayment_Voucher, Session["CrossVoucherNo"].ToString(), txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucherCrossPayment, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " and " + MaxDocumentIdForCross + " saved successfully');", true);
                    PrintVoucher(MaxDocumentId);
                }
                else
                {
                    MaxDocumentId = LController.SelectMaxVoucherId(Constants.Cash_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Cash_Voucher, Date, Constants.CashPayment_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, Constants.DateNullValue, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    PrintVoucher(MaxDocumentId);
                }

            }
            else
            {
                if (ddlCrossPAyment.SelectedItem.Value.ToString() != "1")
                {
                    MaxDocumentId = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    //MaxDocumentIdForCross = LController.SelectMaxVoucherForCross(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);

                    LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Journal_Voucher, Date, Constants.Journal_Voucher, Session["VoucherNo"].ToString(), Vendor_Name + " to " + CrossVendor_Name + ", " + Item_Name + " to " + CrossItem_Name + ", " + txtRemarks.Text, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    //LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpBankAccount.SelectedItem.Value.ToString()), MaxDocumentIdForCross, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["CrossVoucherNo"].ToString(), txtRemarks.Text, ChequeDate, ChequeNo, dtVoucherCrossPayment, Convert.ToInt32(Session["UserID"]), "-1", Constants.DateNullValue, true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " and " + MaxDocumentIdForCross + " saved successfully');", true);
                    PrintVoucher(MaxDocumentId);
                }
                else
                {
                    string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                    MaxDocumentId = LController.SelectMaxVoucherId(Constants.Bank_Voucher, Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), Date);
                    LController.Add_Voucher(Convert.ToInt32(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpVendor.SelectedItem.Value.ToString()), MaxDocumentId, Constants.Bank_Voucher, Date, Constants.Bank_Deposit, Session["VoucherNo"].ToString(), Vendor_Name + ", " + Item_Name + ", " + txtRemarks.Text, ChequeDate, ChequeNo, dtVoucher, Convert.ToInt32(Session["UserID"]), PROCESS_IDs, Constants.DateNullValue, true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('Voucher No : " + MaxDocumentId + " saved successfully');", true);
                    PrintVoucher(MaxDocumentId);
                }
            }
        }
        Session["ITEM_NAME"] = null;
        Session["Vendor_Name"] = null;
        Session["CrossITEM_NAME"] = null;
        Session["CrossVendor_Name"] = null;
    }
    private void PrintVoucher(string VoucherNo)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();
        int VoucherType = Constants.IntNullValue;
        string VoucherType2 = "";
        string pVoucherType = "";
        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
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
        else if (DrpAccountType.SelectedItem.Value.ToString() == "21" || DrpAccountType.SelectedItem.Value.ToString() == "47")
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
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
                        VoucherType = 24;
                        VoucherType2 = "Cash Payment Voucher";
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
                    VoucherType = 14;
                    VoucherType2 = "Cash Receipt Voucher";
                }
            }
        }
        else
        {
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2" || ddlTransactionType.SelectedItem.Value.ToString() == "14")
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
                    VoucherType = 15;
                    VoucherType2 = "Bank Receipt Voucher";
                }
            }
        }

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        //ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, Constants.IntNullValue);
        //CrpReport.SetDataSource(ds);
        //CrpReport.Refresh();
        if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44" || DrpAccountType.SelectedItem.Value.ToString() == "46")
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
                string PROCESS_IDs = Session["CHEQUE_PROCESS_IDs"].ToString();
                ds = RptAccountCtl.SelectUnpostVoucherForPrintWithInstrument(int.Parse(drpDistributor.SelectedItem.Value.ToString()), VoucherNo, VoucherType, PROCESS_IDs, 2);
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
        Session["CHEQUE_PROCESS_IDs"] = null;

    }
    private void PrintChequeVoucher(string ChequeProcessId)
    {

        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        CORNBusinessLayer.Reports.crpChequeVoucher CrpReport = new CORNBusinessLayer.Reports.crpChequeVoucher();


        string VoucherType2 = "";
        string pVoucherType = "";

        if (DrpAccountType.SelectedItem.Value.ToString() == "18")
        {
            pVoucherType = "Bank Voucher";
            VoucherType2 = "Bank Payment Voucher";
        }


        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributor.SelectedItem.Value.ToString()), null, Convert.ToInt32(ChequeProcessId), 0);
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
        //txtChequeNo.Text = "";
        txtAmount.Text = "";
        txtBankName.Text = "";
        txtStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        btnSave.Text = "Save";
        txtReceivedDate.Text = "";
        txtRemarks.Text = "";
        txtTax.Text = "";
        txtDiscount.Text = "";
        txtFavouring.Text = "";
        txtChequeNumber.Text = "";
        ChkIsDiscount.Checked = false;
        DrpStatus.SelectedIndex = 0;
        DrpStatus.Enabled = false;


        ddlTransactionType.Enabled = true;
        Hp.Enabled = true;
        DrpAccountType.Enabled = true;
        DrpVendor.Enabled = true;
        ddlItem.Enabled = true;
        drpDistributor.Enabled = true;
        ddlCrossPAyment.Enabled = false;
    }
    private bool IsDayClosed()
    {
        DistributorController DistrCtl = new DistributorController();
        try
        {
            DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["DISTRIBUTOR_ID"]), 3);
            if (dtDayClose.Rows.Count > 0)
            {
                //if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
                //{
                return false;
                //}
            }
            return true;
        }
        catch (Exception)
        {
            throw;
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
    protected void ddlTransactionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "2")
        {
            LoadData();
            PaymentType();
            VendorItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            toggleControls(DrpAccountType.SelectedItem.Value.ToString());
            PurchaseChqueStatus();
            //LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
            Panel3.Visible = false;
            CrossPanel1.Visible = false;
            CrossPanel2.Visible = false;
            //LoadAccountHead(1);
            if (ddlTransactionType.SelectedItem.Value.ToString() == "2") // For Purchase
            {
                CrossPayments();
            }
            else
            {
                // For Purchase Return

            }
            DrpBankAccount.SelectedIndex = 0;
            //ddlCrossPAyment.Enabled = true;
        }
        else
        {
            LoadData();
            PaymentType();
            VendorItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            CrossPayments();
            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = true;
            LoadAccountHead(1);
        }


    }
    protected void Hp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue != "2")
        {
            PaymentType();
            LoadData();
            SelectCreditInvoice();
            LoadReceviedCheque();
            PanelItem.Visible = false;
            CrossPayments();
        }
        else
        {
            PaymentType();
            LoadData();
            VendorItems();
            SelectCreditInvoice();
            LoadReceviedCheque();
            PanelItem.Visible = true;
            CrossPayments();
        }
    }
    protected void ddlCrossPAyment_SelectedIndexChanged(object sender, EventArgs e)
    {
        CrossPayments();
    }
    public void CrossPayments()
    {
        if (ddlCrossPAyment.SelectedItem.Value.ToString() == "2")
        {
            ///// Cross Payment Suppliers ////////
            int VendorID = Convert.ToInt32(DrpVendor.SelectedItem.Value);
            Vendor(VendorID);
            DrpBankAccount.SelectedIndex = 0;
            int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
            LoadCrossSupplierInvoices(ID);
            LoadReceviedCheque();
            CrossPanel1.Visible = true;
            CrossPanel2.Visible = true;

        }
        else if (ddlCrossPAyment.SelectedItem.Value.ToString() == "3")
        {
            ///// Cross Payment Customers ////////
            Customers();
            DrpBankAccount.SelectedIndex = 0;
            int ID = Convert.ToInt32(DrpBankAccount.SelectedItem.Value);
            LoadCrossSaleInvoices(ID);
            LoadReceviedCheque();
            CrossPanel1.Visible = true;
            CrossPanel2.Visible = true;

        }
        else
        {
            CrossPanel1.Visible = false;
            CrossPanel2.Visible = false;
            LoadAccountHead(1);
            LoadReceviedCheque();
            DrpBankAccount.SelectedIndex = 0;
        }
    }
    public void Vendor(int VendorID)
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 14);
        DataRow[] dtr = dt.Select("VENDOR_ID=" + VendorID);
        if (dtr.Any())
        {
            foreach (var drow in dtr)
            {
                drow.Delete();
            }
            dt.AcceptChanges();
        }
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
        //if (Hp.SelectedValue == "1")
        //{
        //    DbCalls db = new DbCalls();
        //    int LocationID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
        //    var query = (from k in db.VENDORS
        //                 //join PR in db.tbl_PURCHASE_MASTER on k.VENDOR_ID equals PR.VENDOR_ID
        //                 //join PD in db.tbl_PURCHASE_DETAIL on PR.PURCHASE_MASTER_ID equals PD.PURCHASE_MASTER_ID
        //                 select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.DISTRIBUTOR_ID,k.CUSTOMER_TYPE }).Where(x => x.TYPE_ID == Constants.Vendor && x.DISTRIBUTOR_ID == LocationID
        //                 || x.TYPE_ID == Constants.Customer && x.CUSTOMER_TYPE == 2 && x.DISTRIBUTOR_ID == LocationID).ToList();
        //    query.RemoveAll(x => x.VENDOR_ID == VendorID);
        //    if (query.Any())
        //    {
        //        DrpBankAccount.Items.Clear();
        //        DrpBankAccount.DataSource = query;
        //        DrpBankAccount.TextField = "VENDOR_NAME";
        //        DrpBankAccount.ValueField = "VENDOR_ID";
        //        DrpBankAccount.DataBind();
        //    }
        //    else
        //    {
        //        DrpBankAccount.Items.Clear();
        //        DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //        DrpBankAccount.SelectedIndex = 0;
        //    }
        //}
        //else
        //{
        //    /////// Installments Returns ....////////
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //    DrpBankAccount.SelectedIndex = 0;
        //}
    }
    public void Customers()
    {
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 31);
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
        //if (Hp.SelectedValue == "1")
        //{
        //    DbCalls db = new DbCalls();
        //    int LocationID = Convert.ToInt32(drpDistributor.SelectedItem.Value);
        //    var query = (from k in db.VENDORS
        //                 //join SR in db.tbl_SALE_RETURN on k.VENDOR_ID equals SR.VENDOR_ID
        //                 select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.DISTRIBUTOR_ID }).Where(x => x.TYPE_ID == Constants.Customer && x.DISTRIBUTOR_ID == LocationID).ToList();
        //    if (query.Any())
        //    {
        //        DrpBankAccount.Items.Clear();
        //        DrpBankAccount.DataSource = query;
        //        DrpBankAccount.TextField = "VENDOR_NAME";
        //        DrpBankAccount.ValueField = "VENDOR_ID";
        //        DrpBankAccount.DataBind();
        //    }
        //    else
        //    {
        //        DrpBankAccount.Items.Clear();
        //        DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //        DrpBankAccount.SelectedIndex = 0;
        //    }
        //}
        //else
        //{
        //    /////// Installments Returns ....////////
        //    DrpBankAccount.Items.Clear();
        //    DrpBankAccount.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        //    DrpBankAccount.SelectedIndex = 0;
        //}

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
        int DISTRIBUTOR_ID;
        int DOCUMENT_NO;
        int TRANSACTION_TYPE;
        int TYPE_ID;
        int VOUCHER_TYPE_ID;
        decimal DEBIT;
        decimal CREDIT;
        decimal Amount;
        int PRINCIPAL_ID;

        var ProID = ((LinkButton)sender).CommandArgument;
        Customer_Ledger_Logics Ser = new Customer_Ledger_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, data.VOUCHER_NO, 15);
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                DISTRIBUTOR_ID = int.Parse(row["DISTRIBUTOR_ID"].ToString());
                VOUCHER_NO = row["VOUCHER_NO"].ToString();
                DOCUMENT_NO = int.Parse(row["DOCUMENT_NO"].ToString());
                TRANSACTION_TYPE = int.Parse(row["TRANSACTION_TYPE"].ToString());
                TYPE_ID = int.Parse(row["TYPE_ID"].ToString());
                VOUCHER_TYPE_ID = int.Parse(row["VOUCHER_TYPE_ID"].ToString());
                DEBIT = decimal.Parse(row["DEBIT"].ToString());
                CREDIT = decimal.Parse(row["CREDIT"].ToString());
                PRINCIPAL_ID = int.Parse(row["PRINCIPAL_ID"].ToString());
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
                    if (TYPE_ID == Constants.Document_Purchase_Return)
                    {
                        #region Document_Purchase_Return
                        PurchaseReturnLogics Per = new PurchaseReturnLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.CREDIT_AMOUNT);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            if (DEBIT_AMOUNT > 0)
                            {
                                Amount = DEBIT_AMOUNT + Amount;
                                using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT =" + Amount + "where PURCHASE_RETURN_ID=" + Purchase.PURCHASE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT =" + Amount + ", IS_DELETED = " + 0 + " where PURCHASE_RETURN_ID=" + Purchase.PURCHASE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.PURCHASE_RETURN_ID) + "and TYPE_ID=" + TYPE_ID, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        #endregion

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
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO= '" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            //}
                            con.Close();
                        }
                        #endregion
                    }
                    else // For Advance Payments
                    {
                        if (PRINCIPAL_ID != 0)
                        {
                            #region Document_Advance For Supplier
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();

                                using (SqlCommand command = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(DOCUMENT_NO) + "and TYPE_ID=" + TYPE_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                                using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            #endregion
                        }
                        else
                        {
                            #region Document_Advance For Customer
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();

                                using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(DOCUMENT_NO) + "and TYPE_ID=" + TYPE_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                                using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            #endregion
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record deleted successfully.');", true);
                    SelectCreditInvoice();
                    LoadReceviedCheque();
                }
                else
                {
                    if (TYPE_ID == Constants.Document_Purchase || TYPE_ID == Constants.Document_Opening || TYPE_ID == Constants.InvoiceBooking)
                    {
                        #region Document_Purchase, Document_Opening, Invoice Booking & Inventory Update
                        if (TYPE_ID != 9)
                        {
                            PurchaseDetailLogics Per = new PurchaseDetailLogics();
                            var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                            decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);

                            #region Purcheses, Opening & Invoice Booking
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                if (DEBIT_AMOUNT > 0)
                                {
                                    Amount = DEBIT_AMOUNT + Amount;
                                    using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT =" + Amount + "where PURCHASE_DETAIL_ID=" + Purchase.PURCHASE_DETAIL_ID, con))
                                    {
                                        command3.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                    using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_DETAIL set DEBIT_AMOUNT =" + Amount + ", PURCHASE_CLOSED = " + 0 + " where PURCHASE_DETAIL_ID=" + Purchase.PURCHASE_DETAIL_ID, con))
                                    {
                                        command3.ExecuteNonQuery();
                                    }
                                }
                                using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.PURCHASE_DETAIL_ID) + "and TYPE_ID=" + TYPE_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                                using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            #endregion
                        }
                        else
                        {
                            InventoryUpdateDetailLogics Per = new InventoryUpdateDetailLogics();
                            var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                            decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);

                            #region Inventory Update Invoices
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                if (DEBIT_AMOUNT > 0)
                                {
                                    Amount = DEBIT_AMOUNT + Amount;
                                    using (SqlCommand command3 = new SqlCommand("UPDATE tbl_INVENTORY_UPDATE_DETAIL SET DEBIT_AMOUNT =" + Amount + " WHERE INVENTORY_UPDATE_DETAIL_ID=" + Purchase.INVENTORY_UPDATE_DETAIL_ID, con))
                                    {
                                        command3.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                    using (SqlCommand command3 = new SqlCommand("UPDATE tbl_INVENTORY_UPDATE_DETAIL SET DEBIT_AMOUNT =" + Amount + ", PURCHASE_CLOSED = " + 0 + " WHERE INVENTORY_UPDATE_DETAIL_ID=" + Purchase.INVENTORY_UPDATE_DETAIL_ID, con))
                                    {
                                        command3.ExecuteNonQuery();
                                    }
                                }
                                using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER SET IS_DELETED = 1 WHERE VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "AND VOUCHER_NO='" + VOUCHER_NO + "' AND DOCUMENT_NO=" + Convert.ToInt64(Purchase.INVENTORY_UPDATE_DETAIL_ID) + "AND TYPE_ID=" + TYPE_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                                using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else if (TYPE_ID == Constants.Document_Purchase_Installment)
                    {
                        #region Document_Purchase_Installment
                        DbCalls db = new DbCalls();
                        PurchaseInstallmentLogics Per = new PurchaseInstallmentLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.DEBIT_AMOUNT);
                        var qryy = (from k in db.tbl_PURCHASE_INS_DETAIL
                                    select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.IS_PAID, k.RECEIPT_NO }).Where(x => x.PURCHASE_INS_MASTER_ID == DOCUMENT_NO && x.RECEIPT_NO == VOUCHER_NO).ToList();
                        int ReceivedAmount = Convert.ToInt32(Amount);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();

                            int firstentry = 0;
                            bool check = false;
                            for (int i = 0; i <= qryy.Count - 1; i++)
                            {
                                var query = (from k in db.tbl_PURCHASE_INS_DETAIL
                                             select new { k.PURCHASE_INS_MASTER_ID, k.PURCHASE_INS_DETAIL_ID, k.DUE_AMOUNT, k.PAY_AMOUNT, k.OS_AMOUNT, k.IS_PAID, k.DUE_DATE, k.RECEIPT_NO }).Where(x => x.PURCHASE_INS_MASTER_ID == Purchase.PURCHASE_INS_MASTER_ID && x.RECEIPT_NO == VOUCHER_NO).ToList();
                                int SALE_DETAIL_ID = Convert.ToInt32(query.First().PURCHASE_INS_DETAIL_ID);
                                int DUE_AMOUNT = Convert.ToInt32(query.First().DUE_AMOUNT);
                                int OS_AMOUNT = Convert.ToInt32(query.First().OS_AMOUNT);
                                int REC_AMOUNT = Convert.ToInt32(query.First().PAY_AMOUNT);
                                int RECEIVED = REC_AMOUNT + OS_AMOUNT;
                                if (ReceivedAmount > REC_AMOUNT)
                                {
                                    if (firstentry != i)
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO='', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', PAY_AMOUNT=" + Convert.ToDecimal(0.00) + ", OS_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", IS_PAID=" + 0 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        check = true;
                                    }
                                    else
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(0.00) + ", RECEIPT_NO = '', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', PAY_AMOUNT=" + Convert.ToDecimal(0.00) + ", OS_AMOUNT=" + Convert.ToDecimal(RECEIVED) + ", IS_PAID=" + 0 + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
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
                                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set RECEIPT_AMOUNT = " + Convert.ToDecimal(ReceivedAmount) + ",RECEIPT_NO='', PAYMENT_DATE='" + Convert.ToDateTime(Constants.DateNullValue) + "', PAY_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", OS_AMOUNT=" + Convert.ToDecimal(recv) + ", IS_PAID=" + Paid + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_DETAIL set  RECEIPT_NO = '', PAYMENT_DATE ='" + Convert.ToDateTime(Constants.DateNullValue) + "', PAY_AMOUNT=" + Convert.ToDecimal(OS_Amount) + ", OS_AMOUNT=" + Convert.ToDecimal(recv) + ", IS_PAID=" + Paid + " where PURCHASE_INS_DETAIL_ID=" + SALE_DETAIL_ID, con))
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
                                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_MASTER set DEBIT_AMOUNT = " + Amount + " where PURCHASE_INS_MASTER_ID=" + Purchase.PURCHASE_INS_MASTER_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_INS_MASTER set DEBIT_AMOUNT = " + Amount + ", PURCHASE_CLOSED = " + 0 + " where PURCHASE_INS_MASTER_ID=" + Purchase.PURCHASE_INS_MASTER_ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.PURCHASE_INS_MASTER_ID) + "and TYPE_ID=" + TYPE_ID, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        #endregion
                        VendorItems();
                    }
                    else if (TYPE_ID == Constants.Document_Purchase_Return)
                    {
                        #region Document_Purchase_Return
                        PurchaseReturnLogics Per = new PurchaseReturnLogics();
                        var Purchase = Per.Get(Convert.ToInt32(DOCUMENT_NO));
                        decimal DEBIT_AMOUNT = Convert.ToDecimal(Purchase.CREDIT_AMOUNT);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            if (DEBIT_AMOUNT > 0)
                            {
                                Amount = DEBIT_AMOUNT + Amount;
                                using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT =" + Amount + "where PURCHASE_RETURN_ID=" + Purchase.PURCHASE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                Amount = DEBIT_AMOUNT + Amount - ADVANCE;
                                using (SqlCommand command3 = new SqlCommand("update tbl_PURCHASE_RETURN set CREDIT_AMOUNT =" + Amount + ", IS_DELETED = " + 0 + " where PURCHASE_RETURN_ID=" + Purchase.PURCHASE_RETURN_ID, con))
                                {
                                    command3.ExecuteNonQuery();
                                }
                            }
                            using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + VOUCHER_TYPE_ID + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(Purchase.PURCHASE_RETURN_ID) + "and TYPE_ID=" + TYPE_ID, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        #endregion

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
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
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
    protected void drpCheque_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DrpAccountType.SelectedItem.Value.ToString() == "18") //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
        {
            OpenChequesDetail(int.Parse(drpCheque.SelectedItem.Value.ToString()));
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" && 
        {
            PayOrdersDetail(int.Parse(drpCheque.SelectedItem.Value.ToString()));
        }
        else
        {
            txtStartDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            txtAmount.Text = "";
        }
    }
    protected void ChbIsAssigned_CheckedChanged(object sender, EventArgs e)
    {
        decimal totalamount = 0;
        string ChequeIDs = "";
        Session["ChequeIDs"] = null;
        foreach (GridViewRow row in GridChqPayOrder.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                bool Check = ((System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("ChbIsAssigned")).Checked;
                if (Check)
                {
                    totalamount += decimal.Parse(row.Cells[7].Text);
                    ChequeIDs += "," + row.Cells[1].Text;
                }
            }
        }
        if (ChequeIDs != "")
        {
            ChequeIDs = ChequeIDs.Remove(0, 1);
        }
        Session["ChequeIDs"] = ChequeIDs;
        txtAmount.Text = Math.Round(totalamount, 0).ToString();
        txtAmount.Enabled = false;
    }
}