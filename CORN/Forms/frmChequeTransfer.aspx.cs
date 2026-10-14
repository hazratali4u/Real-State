using CORNBusinessLayer.Classes;
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

public partial class Forms_frmChequeTransfer : System.Web.UI.Page
{
    readonly DataControl _dc = new DataControl();
    readonly ChequeEntryController CController = new ChequeEntryController();
    readonly LedgerController LController = new LedgerController();
    IDbTransaction mTransaction;
    IDbConnection mConnection;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtTransactionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            LoadDistributorFrom();
            LoadDistributorTo();
            PaymentType();
            DrpAccountType_SelectedIndexChanged(null, null);
            LoadCheque();

        }
    }
    private void LoadAccountHead(int p_Parent)
    {
        ///////////////////// For Fourth Level //////////////////
        DataTable dtHead = new DataTable();
        int TypeID;
        if (p_Parent == 18)
        {
            TypeID = 46; //TypeID = p_Parent;
        }
        else if (p_Parent == 44)
        {
            TypeID = 46; //TypeID = p_Parent;
        }
        else if (p_Parent == 46)
        {
            TypeID = p_Parent;
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
        ///////////////////// END Fourth Level //////////////////
    }
    private void LoadDistributorFrom()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDxComboBoxList(drpDistributorFrom, dt, 0, 2, true);

        if (dt.Rows.Count > 0)
        {
            drpDistributorFrom.SelectedIndex = 0;
        }
    }
    private void LoadDistributorTo()
    {
        DistributorController DController = new DistributorController();
        drpDistributorTo.Items.Clear();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["CompanyId"].ToString()));
        if (dt.Rows.Count > 0)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                if (int.Parse(dr["DISTRIBUTOR_ID"].ToString()) == int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()))
                    dr.Delete();
            }
            dt.AcceptChanges();
            if (dt.Rows.Count > 0)
            {
                clsWebFormUtil.FillDxComboBoxList(drpDistributorTo, dt, 0, 2, true);
                drpDistributorTo.SelectedIndex = 0;
            }
            else
            {
                drpDistributorTo.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
                drpDistributorTo.SelectedIndex = 0;
            }
        }
    }
    public void PaymentType()
    {
        DrpAccountType.Items.Clear();
        DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
        //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cash Payment", "21"));
        //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Online Transfer", "33"));
        DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
        //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Own Cheque", "46"));
        //DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Cross Cheque", "45"));
        DrpAccountType.SelectedIndex = 0;
    }
    public void LoadCheque()
    {
        // Open Cheques In Hand
        if (DrpAccountType.SelectedItem.Value.ToString() == "18") //DrpBankAccount.SelectedItem.Value.ToString() == "84" && 
        {
            OpenChequeGrid("");
            //txtAmount.Enabled = false;
            //txtAmount.Text = "";
        }
        else if (DrpAccountType.SelectedItem.Value.ToString() == "44") //DrpBankAccount.SelectedItem.Value.ToString() == "85" && 
        {
            // Pay Orders In Hand
            PayOrderGrid("");
            //txtAmount.Enabled = false;
            //txtAmount.Text = "";
        }
    }
    public void OpenChequeGrid(string Type)
    {
        string UpdateChequeProccessIDs = "";
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        if (Type == "Update")
        {
            int ReferenceID = int.Parse(Session["REFERENCE_ID"].ToString());
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
            dt2 = CController.SelectOpenChequePayorder(530, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), ReferenceID, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
            if (dt2.Rows.Count > 0)
            {
                foreach (DataRow row in dt2.Rows)
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
                    UpdateChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
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
                GridChqPayOrder.FooterRow.Cells[9].Visible = false;
                GridChqPayOrder.FooterRow.Cells[10].Visible = false;
                GridChqPayOrder.FooterRow.Cells[11].Visible = false;
                GridChqPayOrder.FooterRow.Cells[12].Visible = false;
                GridChqPayOrder.FooterRow.Cells[13].Visible = false;
                GridChqPayOrder.FooterRow.Cells[14].Visible = false;
                GridChqPayOrder.FooterRow.Cells[15].Visible = false;
                GridChqPayOrder.FooterRow.Cells[16].Visible = false;
                GridChqPayOrder.FooterRow.Cells[17].Visible = false;
                GridChqPayOrder.FooterRow.Cells[18].Visible = false;
                GridChqPayOrder.FooterRow.Cells[19].Visible = false;
                GridChqPayOrder.FooterRow.Cells[20].Visible = false;
                GridChqPayOrder.FooterRow.Cells[21].Visible = false;
                //lblChequeNo.Visible = false;
                //drpCheque.Visible = false;
            }
            if (UpdateChequeProccessIDs != "")
            {
                UpdateChequeProccessIDs = UpdateChequeProccessIDs.Remove(0, 1);
            }
            Session["UpdateChequeProccessIDs"] = UpdateChequeProccessIDs;
            string[] IDs = UpdateChequeProccessIDs.Split(',');
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
            Session["REFERENCE_ID"] = null;
        }
        else
        {
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
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
                GridChqPayOrder.FooterRow.Cells[9].Visible = false;
                GridChqPayOrder.FooterRow.Cells[10].Visible = false;
                GridChqPayOrder.FooterRow.Cells[11].Visible = false;
                GridChqPayOrder.FooterRow.Cells[12].Visible = false;
                GridChqPayOrder.FooterRow.Cells[13].Visible = false;
                GridChqPayOrder.FooterRow.Cells[14].Visible = false;
                GridChqPayOrder.FooterRow.Cells[15].Visible = false;
                GridChqPayOrder.FooterRow.Cells[16].Visible = false;
                GridChqPayOrder.FooterRow.Cells[17].Visible = false;
                GridChqPayOrder.FooterRow.Cells[18].Visible = false;
                GridChqPayOrder.FooterRow.Cells[19].Visible = false;
                GridChqPayOrder.FooterRow.Cells[20].Visible = false;
                GridChqPayOrder.FooterRow.Cells[21].Visible = false;
                //lblChequeNo.Visible = false;
                //drpCheque.Visible = false;
            }
        }
    }
    public void PayOrderGrid(string Type)
    {
        string UpdateChequeProccessIDs = "";
        GridChqPayOrder.DataSource = null;
        GridChqPayOrder.DataBind();
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        if (Type == "Update")
        {
            int ReferenceID = int.Parse(Session["REFERENCE_ID"].ToString());
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
            dt2 = CController.SelectOpenChequePayorder(530, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), ReferenceID, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
            if (dt2.Rows.Count > 0)
            {
                foreach (DataRow row in dt2.Rows)
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
                    UpdateChequeProccessIDs += "," + row["CHEQUE_PROCESS_ID"].ToString();
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
                GridChqPayOrder.FooterRow.Cells[9].Visible = false;
                GridChqPayOrder.FooterRow.Cells[10].Visible = false;
                GridChqPayOrder.FooterRow.Cells[11].Visible = false;
                GridChqPayOrder.FooterRow.Cells[12].Visible = false;
                GridChqPayOrder.FooterRow.Cells[13].Visible = false;
                GridChqPayOrder.FooterRow.Cells[14].Visible = false;
                GridChqPayOrder.FooterRow.Cells[15].Visible = false;
                GridChqPayOrder.FooterRow.Cells[16].Visible = false;
                GridChqPayOrder.FooterRow.Cells[17].Visible = false;
                GridChqPayOrder.FooterRow.Cells[18].Visible = false;
                GridChqPayOrder.FooterRow.Cells[19].Visible = false;
                GridChqPayOrder.FooterRow.Cells[20].Visible = false;
                GridChqPayOrder.FooterRow.Cells[21].Visible = false;
                //lblChequeNo.Visible = false;
                //drpCheque.Visible = false;
            }

            if (UpdateChequeProccessIDs != "")
            {
                UpdateChequeProccessIDs = UpdateChequeProccessIDs.Remove(0, 1);
            }
            Session["UpdateChequeProccessIDs"] = UpdateChequeProccessIDs;
            string[] IDs = UpdateChequeProccessIDs.Split(',');
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
            Session["REFERENCE_ID"] = null;

        }
        else
        {
            dt = CController.SelectOpenChequePayorder(529, int.Parse(DrpAccountType.SelectedItem.Value.ToString()), int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 2);
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
                GridChqPayOrder.FooterRow.Cells[9].Visible = false;
                GridChqPayOrder.FooterRow.Cells[10].Visible = false;
                GridChqPayOrder.FooterRow.Cells[11].Visible = false;
                GridChqPayOrder.FooterRow.Cells[12].Visible = false;
                GridChqPayOrder.FooterRow.Cells[13].Visible = false;
                GridChqPayOrder.FooterRow.Cells[14].Visible = false;
                GridChqPayOrder.FooterRow.Cells[15].Visible = false;
                GridChqPayOrder.FooterRow.Cells[16].Visible = false;
                GridChqPayOrder.FooterRow.Cells[17].Visible = false;
                GridChqPayOrder.FooterRow.Cells[18].Visible = false;
                GridChqPayOrder.FooterRow.Cells[19].Visible = false;
                GridChqPayOrder.FooterRow.Cells[20].Visible = false;
                GridChqPayOrder.FooterRow.Cells[21].Visible = false;
                //lblChequeNo.Visible = false;
                //drpCheque.Visible = false;
            }
        }
    }
    private void LoadReceviedCheque()
    {

        GrdCheque.DataSource = null;
        GrdCheque.DataBind();

        //GrdCO.DataSource = null;
        //GrdCO.DataBind();
        DateTime Date = Constants.DateNullValue;
        string abd = txtTransactionDate.Text.Replace("-", "/");
        Date = DateTime.Parse(ConvertDate.British_To_American(abd));
        decimal chqAmount = 0;

        //if (DrpStatus.SelectedItem.Value.ToString() != Constants.Cheque_Clear.ToString())
        //{
        DataTable dt = null;
        dt = CController.SelectTransferChequeEntry(4, Date, int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), int.Parse(_dc.chkNull_0(drpDistributorTo.SelectedItem.Value.ToString())), Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
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
            GrdCheque.FooterRow.Cells[2].Visible = false;
            GrdCheque.FooterRow.Cells[8].Text = string.Format("{0:n2}", chqAmount);
            GrdCheque.FooterRow.Cells[7].Text = "Total";
            GrdCheque.FooterRow.Cells[7].Font.Bold = true;
            GrdCheque.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
            GrdCheque.FooterRow.Cells[8].Font.Bold = true;
            GrdCheque.FooterRow.Cells[8].HorizontalAlign = HorizontalAlign.Right;
            GrdCheque.FooterRow.Cells[9].Visible = false;
            GrdCheque.FooterRow.Cells[10].Visible = false;
            GrdCheque.FooterRow.Cells[11].Visible = false;
            GrdCheque.FooterRow.Cells[12].Visible = false;
            GrdCheque.FooterRow.Cells[13].Visible = false;
            GrdCheque.FooterRow.Cells[14].Visible = false;
            GrdCheque.FooterRow.Cells[15].Visible = false;
            GrdCheque.FooterRow.Cells[16].Visible = false;
            GrdCheque.FooterRow.Cells[17].Visible = false;
            GrdCheque.FooterRow.Cells[18].Visible = false;
            GrdCheque.FooterRow.Cells[19].Visible = false;
        }
        //lblTotalAmount.Text = string.Format("{0:n2}", chqAmount);

        //foreach (GridViewRow gvr in GrdCredit.Rows)
        //{
        //    CheckBox row = gvr.FindControl("ChbIsAssigned") as CheckBox;
        //    if (row.Enabled)
        //    {
        //        break;
        //    }
        //    else
        //    {
        //        row.Enabled = true;
        //    }
        //}



        //}


    }
    protected void drpDistributorFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadDistributorTo();
        LoadCheque();
        LoadReceviedCheque();
    }
    protected void DrpAccountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
        LoadCheque();
        LoadReceviedCheque();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int InvoiceCount = Constants.IntNullValue;
        int ISSUED = 0;
        if (btnSave.Text != "Transferred")
        {
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("DELETE CHEQUE_PROCESS WHERE CHEQUE_PROCESS_ID =" + Convert.ToInt64(Session["CHEQUE_PROCESS_ID"]), con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            //Session["CHEQUE_PROCESS_ID"] = null;
        }
        decimal TransferedAmount = 0;
        string TransferedChequeNos = "";
        string ChqueProcessIds = "";
        foreach (GridViewRow dr in GridChqPayOrder.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                if (Session["CHEQUE_PROCESS_ID"] == null)
                {
                    ISSUED = int.Parse(dr.Cells[21].Text);
                }
                string ChqDate = txtTransactionDate.Text.Replace("-", "/");
                DateTime Cheque_Date = DateTime.Parse(ConvertDate.British_To_American(ChqDate));
                string RecDate = dr.Cells[12].Text.Replace("-", "/");
                DateTime Receive_Date = DateTime.Parse(ConvertDate.British_To_American(RecDate));
                HFChqueProcessId.Value = CController.ChequeTransferOut(int.Parse(dr.Cells[1].Text), int.Parse(_dc.chkNull_0(drpDistributorTo.SelectedItem.Value.ToString())), int.Parse(drpDistributorFrom.SelectedItem.Value.ToString())
                    , int.Parse(dr.Cells[9].Text), int.Parse(dr.Cells[10].Text), dr.Cells[4].Text, dr.Cells[3].Text, Cheque_Date, Receive_Date, Constants.DateNullValue, Receive_Date, decimal.Parse(dr.Cells[7].Text)
                    , int.Parse(dr.Cells[14].Text), DateTime.Now, int.Parse(Session["UserId"].ToString()), dr.Cells[15].Text, dr.Cells[16].Text, long.Parse(dr.Cells[11].Text), int.Parse(dr.Cells[17].Text), 0
                    , short.Parse(dr.Cells[18].Text), 0, int.Parse(dr.Cells[19].Text), dr.Cells[6].Text, int.Parse(dr.Cells[20].Text), ISSUED, int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()));
                TransferedAmount += decimal.Parse(dr.Cells[7].Text);
                TransferedChequeNos += dr.Cells[4].Text + ",";
                ChqueProcessIds += dr.Cells[1].Text + ",";
            }
        }
        if (InvoiceCount != Constants.IntNullValue)
        {
            TransferedChequeNos = TransferedChequeNos.TrimEnd(',');
            ChqueProcessIds = ChqueProcessIds.TrimEnd(',');
            string ChqDate = txtTransactionDate.Text.Replace("-", "/");
            DateTime Cheque_Date = DateTime.Parse(ConvertDate.British_To_American(ChqDate));
            InsertGL(int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), TransferedAmount, Cheque_Date, 0, TransferedChequeNos);
            PrintVoucher(hfVoucher_No.Value, ChqueProcessIds);
            LoadCheque();
            LoadReceviedCheque();
            drpDistributorFrom.Enabled = true;
            DrpAccountType.Enabled = true;
            Session["CHEQUE_PROCESS_ID"] = null;
            btnSave.Text = "Transferred";
            
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select " + DrpAccountType.SelectedItem.Text + " first.');", true);
            return;
        }
    }
    private void InsertGL(int p_Distributor_Id, decimal TransferedAmount, DateTime p_Ledger_Date, int p_PRINCIPAL_ID, string pChequeNo)
    {
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            #region GL Master, Detail

            string Remarks = "Default Instrument Transfer Out Voucher / " + drpDistributorFrom.SelectedItem.Text + " to " + drpDistributorTo.SelectedItem.Text;
            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);
            {
                LController.PostingGLMasterForTransfer(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                    , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Transfer_Out
                    , "Transfer Out Voucher", Remarks, int.Parse(Session["UserId"].ToString()), "Instrument Transfer Out", pChequeNo, mTransaction, mConnection);

                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, long.Parse(DrpBankAccount.SelectedItem.Value.ToString()), 0, TransferedAmount, "Instrument Transfer Out Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, 253, TransferedAmount, 0, "Instrument Transfer Out Voucher", mTransaction, mConnection);
                hfVoucher_No.Value = VoucherNo2;
            }
            #endregion
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
    private void PrintVoucher(string VoucherNo, string ChqueProcessIds)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        ReportDocument CrpReport = new ReportDocument();

        int VoucherType = Constants.IntNullValue;
        string VoucherType2 = "";
        string pVoucherType = "";
        
                pVoucherType = "Journal Voucher";
                VoucherType = 16;
                VoucherType2 = "Journal Voucher";

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()));

                //ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), VoucherNo, VoucherType, Constants.IntNullValue);
                //CrpReport = new CORNBusinessLayer.Reports.crpVoucherViewSupplier();
                //CrpReport.SetDataSource(ds);
                //CrpReport.Refresh();

        ds = RptAccountCtl.SelectUnpostVoucherForPrintWithInstrument(int.Parse(drpDistributorFrom.SelectedItem.Value.ToString()), VoucherNo, VoucherType, ChqueProcessIds, 2);
        CrpReport = new CORNBusinessLayer.Reports.crpVoucherViewWithInstrument();
        ReportDocument SubReport = CrpReport.OpenSubreport("RptInstrument");
        CrpReport.SetDataSource(ds);
        SubReport.SetDataSource(ds);
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
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session["CHEQUE_PROCESS_ID"] = null;
        btnSave.Text = "Transferred";
        DrpAccountType.Enabled = true;
        drpDistributorFrom.Enabled = true;
        LoadDistributorTo();
        LoadCheque();
        LoadReceviedCheque();
    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {

    }
    protected void GrdCheque_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Session["REFERENCE_ID"] = null;
        Session["CHEQUE_PROCESS_ID"] = null;
        ChequeEntryController Ccontroller = new ChequeEntryController();
        HFChqueProcessId.Value = GrdCheque.Rows[e.NewEditIndex].Cells[0].Text;
        //LoadDistributorFrom();
        drpDistributorFrom.SelectedItem.Value = GrdCheque.Rows[e.NewEditIndex].Cells[17].Text;
        drpDistributorFrom.Enabled = false;
        LoadDistributorTo();
        //drpDistributorTo.SelectedIndex = 0;
        //drpDistributorTo.SelectedItem.Value = GrdCheque.Rows[e.NewEditIndex].Cells[16].Text;
        
        drpDistributorTo.SelectedItem.Text = GrdCheque.Rows[e.NewEditIndex].Cells[3].Text;
        int ReferenceID = int.Parse(GrdCheque.Rows[e.NewEditIndex].Cells[18].Text);
        Session["REFERENCE_ID"] = ReferenceID;

        if (GrdCheque.Rows[e.NewEditIndex].Cells[19].Text == "Open Cheque")
        {
            // Open Cheque
            DrpAccountType.Items.Clear();
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
            DrpAccountType.SelectedIndex = 0;
            DrpAccountType.Enabled = false;
            LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
            DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[10].Text;
            DrpBankAccount.Enabled = false;
            OpenChequeGrid("Update");
        }
        else
        {
            // Pay Order
            DrpAccountType.Items.Clear();
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "18"));
            DrpAccountType.Items.Add(new DevExpress.Web.ListEditItem("Pay Order", "44"));
            DrpAccountType.SelectedIndex = 1;
            DrpAccountType.Enabled = false;
            LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
            DrpBankAccount.Value = GrdCheque.Rows[e.NewEditIndex].Cells[10].Text;
            DrpBankAccount.Enabled = false;
            PayOrderGrid("Update");
        }
        //LoadAccountHead(int.Parse(DrpAccountType.SelectedItem.Value.ToString()));
        Session["CHEQUE_PROCESS_ID"] = GrdCheque.Rows[e.NewEditIndex].Cells[0].Text;
        btnSave.Text = "Update";
        //DrpBankAccount.SelectedItem.Value = GrdCheque.Rows[e.NewEditIndex].Cells[9].Text;

        //if (DrpAccountType.SelectedItem.Value.ToString() == "18" || DrpAccountType.SelectedItem.Value.ToString() == "44")
        //{
        //    DataTable dt = Ccontroller.SelectChequeEntryInvoice(long.Parse(HFChqueProcessId.Value), 0);
        //    foreach (GridViewRow dr in GridChqPayOrder.Rows)
        //    {
        //        foreach (DataRow dbr in dt.Rows)
        //        {
        //            if (Convert.ToInt64(GridChqPayOrder.DataKeys[dr.RowIndex].Values["CHEQUE_PROCESS_ID"]) == Convert.ToInt64(dbr["CHEQUE_PROCESS_ID"]))
        //            {
        //                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
        //                chRelized.Checked = true;
        //            }
        //        }
        //    }
        //}

    }
    protected void GrdCheque_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand("DELETE CHEQUE_PROCESS WHERE CHEQUE_PROCESS_ID =" + Convert.ToInt64(GrdCheque.Rows[e.RowIndex].Cells[0].Text), con))
            {
                command.ExecuteNonQuery();
            }
            using (SqlCommand command1 = new SqlCommand("update CHEQUE_PROCESS set ISSUED =" + 0 + " where CHEQUE_PROCESS_ID=" + Convert.ToInt64(GrdCheque.Rows[e.RowIndex].Cells[18].Text), con))
            {
                command1.ExecuteNonQuery();
            }
            con.Close();
        }
        LoadCheque();
        LoadReceviedCheque();
    }
    protected void ChbIsAssigned_CheckedChanged(object sender, EventArgs e)
    {

    }
    protected void drpDistributorTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Session["CHEQUE_PROCESS_ID"] == null)
        {
            LoadReceviedCheque();
        }
    }

    protected void DrpBankAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCheque();
    }
}