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

public partial class Forms_frmInstallmentPurchase : System.Web.UI.Page
{
    private static int RowNo;
    DataTable PurchaseDetail;
    DataTable InstallmentDetail;
    DataTable InsDetail;
    public int Percent = 1;
    public int Value = 2;
    public int PurchaseDefault = 1;
    public int PurchaseReturn = 2;
    readonly LedgerController LController = new LedgerController();
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

            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            Items(9);
            LoadProject();
            LoadDistributor();
            LoadVendors();
            LoadBlock();
            LoadStreet();
            LoadItems("");
            LoadCommissionAgent();
            LoadGrid("");
            DoEmptyTextBox();

            CreatInsTable();
            InsDetailTable();
            //txtBalance.Attributes.Add("readonly", "readonly");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");

            txtConfirmationDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            //txtConfirmationDate.Attributes.Add("readonly", "readonly");

            txtAllocationDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            //txtAllocationDate.Attributes.Add("readonly", "readonly");

            txtPurchaseAmount.Attributes.Add("autocomplete", "off");
            txtDiscount.Attributes.Add("autocomplete", "off");
            txtBookingPercent.Attributes.Add("autocomplete", "off");
            //txtBookingValue.Attributes.Add("autocomplete", "off");
            txtConfirmationPercent.Attributes.Add("autocomplete", "off");
            txtConfirmationValue.Attributes.Add("autocomplete", "off");
            txtAllocationPercent.Attributes.Add("autocomplete", "off");
            txtAllocationValue.Attributes.Add("autocomplete", "off");
            //txtCommissionPercent.Attributes.Add("autocomplete", "off");
            //txtCommissionValue.Attributes.Add("autocomplete", "off");
            Panel3.Visible = true;
            HFeild.Value = "0";
            HiddenPurchaseAmount.Value = "0";
            HiddenDiscount.Value = "0";
            HiddenBookingPercent.Value = "0";
            HiddenBookingValue.Value = "0";
            HiddenConfirmationPercent.Value = "0";
            HiddenConfirmationValue.Value = "0";
            HiddenAllocationPercent.Value = "0";
            HiddenAllocationValue.Value = "0";
            HiddenBalance.Value = "0";
        }
    }
    #region Load Events
    public void Items(int Type)
    {
        if (Type != 10)
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
    protected void LoadVendors()
    {
        ddlVendor.DataSource = DropDowns.VendorDropDown();
        ddlVendor.DataTextField = "VENDOR_NAME";
        ddlVendor.DataValueField = "VENDOR_ID";
        ddlVendor.DataBind();

    }
    protected void LoadBlock()
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
    protected void LoadStreet()
    {
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
    protected void LoadItems(string Val)
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
    protected void LoadCommissionAgent()
    {
        //ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
        //ddlCommissionAgent.DataTextField = "VENDOR_NAME";
        //ddlCommissionAgent.DataValueField = "VENDOR_ID";
        //ddlCommissionAgent.DataBind();
    }
    public void DoEmptyTextBox()
    {
        Session["SaleMasterID"] = null;
        txtBalance.Text = "";
        txtPurchaseAmount.Text = "";
        txtDiscount.Text = "";
        txtBookingPercent.Text = "";
        txtBookingValue.Text = "";
        txtConfirmationPercent.Text = "";
        txtConfirmationValue.Text = "";
        txtAllocationPercent.Text = "";
        txtAllocationValue.Text = "";

        txtRemarks.Text = "";
        btnSave.Text = "Save";
        //txtDiscount.Text = "";
        ddlInstallmentType.SelectedValue = "1";
        txtNoofInstallment.Text = "";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        //ddlCommissionAgent.Items.Add(new DevExpress.Web.ListEditItem("--Select--", "0"));
        //ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
        //ddlCommissionAgent.SelectedValue = "0";
        //ddlItem.SelectedIndex = 0;
        mastergrid.Visible = true;
        Session["PurchaseDetail"] = null;
        Session["PurchaseInstallment"] = null;
        PanelInstallments.Visible = false;
        TotalTable.Visible = false;

        CreatInsTable();
        btnAdd.Text = "Add";
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
    private void CreatInsTable()
    {
        InstallmentDetail = new DataTable();
        InstallmentDetail.Columns.Add("InstallmentType_ID", typeof(int));
        InstallmentDetail.Columns.Add("InstallmentType", typeof(string));
        InstallmentDetail.Columns.Add("NoOfInstallment", typeof(int));
        InstallmentDetail.Columns.Add("StartMonth", typeof(DateTime));
        InstallmentDetail.Columns.Add("InstallmentAmount", typeof(decimal));
        InstallmentDetail.Columns.Add("TotalInstallmentAmount", typeof(decimal));
        InstallmentDetail.Columns.Add("PreviousAmount", typeof(decimal));
        this.Session.Add("InstallmentDetail", InstallmentDetail);
    }
    private void InsDetailTable()
    {
        InsDetail = new DataTable();
        InsDetail.Columns.Add("PURCHASE_INS_DETAIL_ID", typeof(int));
        InsDetail.Columns.Add("PURCHASE_INS_MASTER_ID", typeof(int));
        InsDetail.Columns.Add("PAYMENT_DESC", typeof(string));
        InsDetail.Columns.Add("INST_NO", typeof(int));
        InsDetail.Columns.Add("DUE_DATE", typeof(DateTime));
        InsDetail.Columns.Add("DUE_AMOUNT", typeof(decimal));
        InsDetail.Columns.Add("REC_AMOUNT", typeof(decimal));
        InsDetail.Columns.Add("OS_AMOUNT", typeof(decimal));
        InsDetail.Columns.Add("RECEIPT_NO", typeof(string));
        InsDetail.Columns.Add("RECEIPT_AMOUNT", typeof(decimal));
        InsDetail.Columns.Add("PAYMENT_DATE", typeof(DateTime));
        InsDetail.Columns.Add("USER_ID", typeof(int));
        InsDetail.Columns.Add("TIME_STEMP", typeof(DateTime));
        InsDetail.Columns.Add("IS_PAID", typeof(bool));
        this.Session.Add("InsDetail", InsDetail);
    }
    protected void LoadGrid(string pType)
    {
        if (!pType.Equals(""))
        {
            DataTable dt = LController.SelectDataForSearching(3);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%'  OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%'";
            //OR PURCHASE_AMOUNT = " + txtSearch.Text + "
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.PageIndex = 0;
            }
            GridSaleMaster.DataSource = dt;
            GridSaleMaster.DataBind();
        }
        else
        {
            
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_PURCHASE_INS_MASTER
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
                       join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       join b in db.BLOCKs on i.BLOCK_ID equals b.BLOCK_ID
                       join s in db.STREETs on i.STREET_ID equals s.STREET_ID
                       join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
                       select new { d.DISTRIBUTOR_NAME, p.PROJECT_NAME, ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME, v.VENDOR_NAME, k.PURCHASE_INS_MASTER_ID, k.PURCHASE_AMOUNT, Active = k.IS_DELETED == false ? "Active" : "Inactive", k.PURCHASE_CLOSED }
                       ).Where(x => x.Active == "Active" && x.PURCHASE_CLOSED == false).ToList();
            GridSaleMaster.DataSource = qry;
            GridSaleMaster.DataBind();
        }
        
    }
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
    #endregion
    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Panelgrid.Visible = false;
        PanelSale.Visible = true;
        Items(9);
        LoadProject();
        LoadBlock();
        LoadStreet();
        LoadItems("");
        Panel3.Visible = false;
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock();
        LoadStreet();
        LoadItems("");
        ddlBlock.Focus();
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
        if (ddlItem.SelectedValue.Equals("0"))
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
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        if (txtPurchaseAmount.Text.Equals("0.00"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount could not be zero.');", true);
            txtPurchaseAmount.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
            return;
        }
        //if (txtCommisionAmount.Text.Equals(""))
        //{
        //    txtCommisionAmount.Text = "0";
        //}
        DateTime ConfDate = Convert.ToDateTime(txtConfirmationDate.Text);
        DateTime AllDate = Convert.ToDateTime(txtAllocationDate.Text);
        if (ConfDate > AllDate)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Allocation Date must be greater then Confirmation Date.');", true);
            txtAllocationDate.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(txtAllocationDate);
            return;
        }
        DataTable Ins = new DataTable();
        Ins = (DataTable)Session["InstallmentDetail"];
        if (Ins == null)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please add Installmet plan first.');", true);
            return;
        }
        if (Ins.Rows.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please add Installmet plan first.');", true);
            return;
        }
        string ItemInfo = "";
        DataTable dTt = (DataTable)this.Session["dt"];
        DataRow[] foundRows = dTt.Select("ITEM_INFO_ID  = " + int.Parse(ddlItem.SelectedValue));
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
        decimal Balance;
        PurchaseInstallmentLogics pml = new PurchaseInstallmentLogics();
        tbl_PURCHASE_INS_MASTER pm = new tbl_PURCHASE_INS_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.CONFIRMATION_DATE = Convert.ToDateTime(txtConfirmationDate.Text);
        pm.ALLOCATION_DATE = Convert.ToDateTime(txtAllocationDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedValue);
        pm.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedValue);
        pm.BLOCK_ID = Convert.ToInt32(ddlBlock.SelectedValue);
        pm.STREET_ID = Convert.ToInt32(ddlStreet.SelectedValue);
        pm.ITEM_INFO_ID = Convert.ToInt32(ddlItem.SelectedValue);
        pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedValue);
        pm.PURCHASE_AMOUNT = Convert.ToDecimal(txtPurchaseAmount.Text);
        pm.DISCOUNT = Convert.ToDecimal(txtDiscount.Text);
        Balance = Convert.ToDecimal(txtPurchaseAmount.Text) - Convert.ToDecimal(txtDiscount.Text);
        if (RbtBooking.SelectedValue != "1")
        {
            pm.BOOKING = Convert.ToDecimal(txtBookingValue.Text);
            pm.BOOKING_PER_VAL = Value;
        }
        else
        {
            decimal value = Balance / 100 * Convert.ToDecimal(txtBookingPercent.Text);
            pm.BOOKING = Convert.ToDecimal(value);
            pm.BOOKING_PER_VAL = Percent;
        }
        if (RbtConfirmation.SelectedValue != "1")
        {
            pm.CONFIRMATION = Convert.ToDecimal(txtConfirmationValue.Text);
            pm.CONFIRMATION_PER_VAL = Value;
        }
        else
        {
            decimal value = Balance / 100 * Convert.ToDecimal(txtConfirmationPercent.Text);
            pm.CONFIRMATION = Convert.ToDecimal(value);
            pm.CONFIRMATION_PER_VAL = Percent;
        }
        if (RbtAllocation.SelectedValue != "1")
        {
            pm.ALLOCATION = Convert.ToDecimal(txtAllocationValue.Text);
            pm.ALLOCATION_PER_VAL = Value;
        }
        else
        {
            decimal value = Balance / 100 * Convert.ToDecimal(txtAllocationPercent.Text);
            pm.ALLOCATION = Convert.ToDecimal(value);
            pm.ALLOCATION_PER_VAL = Percent;
        }
        pm.BALANCE = Balance;
        pm.REMARKS = txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        pm.PURCHASE_CLOSED = false;
        pm.DEBIT_AMOUNT = Balance;
        pm.IS_DELETED = false;
        pm.PURCHASE_TYPE = PurchaseDefault;
        if (Session["PurchaseInstallment"] != null)
        {
            pm.PURCHASE_INS_MASTER_ID = Convert.ToInt32(Session["PurchaseInstallment"]);
            pml.Update(pm);
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("delete tbl_PURCHASE_INS_DETAIL where PURCHASE_INS_MASTER_ID=" + pm.PURCHASE_INS_MASTER_ID, con))
                {
                    command.ExecuteNonQuery();
                }
                using (SqlCommand command2 = new SqlCommand("delete tbl_PURCHASE_INS where PURCHASE_INS_MASTER_ID=" + pm.PURCHASE_INS_MASTER_ID, con))
                {
                    command2.ExecuteNonQuery();
                }
                con.Close();
            }

            Purchase_Ins_Detail_Logics PIDL = new Purchase_Ins_Detail_Logics();
            Purchase_Ins_Logics PIL = new Purchase_Ins_Logics();
            tbl_PURCHASE_INS_DETAIL sd = new tbl_PURCHASE_INS_DETAIL();
            if (pm.BOOKING > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "BOOKING", 0, Convert.ToDateTime(txtPurchaseDate.Text), Convert.ToDecimal(pm.BOOKING), 0, Convert.ToDecimal(pm.BOOKING), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            if (pm.CONFIRMATION > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "CONFIRMATION", 1, DateTime.Parse(txtConfirmationDate.Text), Convert.ToDecimal(pm.CONFIRMATION), 0, Convert.ToDecimal(pm.CONFIRMATION), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            if (pm.ALLOCATION > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "ALLOCATION", 1, DateTime.Parse(txtAllocationDate.Text), Convert.ToDecimal(pm.ALLOCATION), 0, Convert.ToDecimal(pm.ALLOCATION), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            DataTable dt = new DataTable();
            dt = (DataTable)Session["InstallmentDetail"];
            int Ins_Type = Constants.IntNullValue;
            string Pay_Desc = "";
            int No_of_Inst = Constants.IntNullValue;
            decimal Inst_Amount = Constants.DecimalNullValue;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Ins_Type = int.Parse(row["InstallmentType_ID"].ToString());
                    Pay_Desc = row["InstallmentType"].ToString();
                    No_of_Inst = int.Parse(row["NoOfInstallment"].ToString());
                    Inst_Amount = decimal.Parse(row["InstallmentAmount"].ToString());
                    decimal Total_Ins_Amount = No_of_Inst * Inst_Amount;
                    DateTime dtFrom = new DateTime();
                    DateTime dtFromMonth = DateTime.Parse(row["StartMonth"].ToString());
                    dtFrom = new DateTime(dtFromMonth.Year, dtFromMonth.Month, 1);
                    dtFrom = dtFrom.AddMonths(1).AddDays(-1);
                    PIL.Save(pm.PURCHASE_INS_MASTER_ID, Ins_Type, Pay_Desc, No_of_Inst, Inst_Amount, dtFromMonth, Total_Ins_Amount);
                    int month = 0;
                    for (int i = 0; i <= No_of_Inst - 1; i++)
                    {
                        InsDetail = (DataTable)this.Session["InsDetail"];
                        DataRow dr = InsDetail.NewRow();
                        dr["PURCHASE_INS_MASTER_ID"] = Convert.ToInt32(pm.PURCHASE_INS_MASTER_ID);
                        //sd.PURCHASE_INS_MASTER_ID = pm.PURCHASE_INS_MASTER_ID;
                        if (Ins_Type == 1)
                        {
                            if (month != i)
                            {
                                month += 3;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 2)
                        {
                            if (month != i)
                            {
                                month += 6;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 3)
                        {
                            if (month != i)
                            {
                                month += 12;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 4)
                        {
                            //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                        }
                        else if (Ins_Type == 5)
                        {
                            dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                        }
                        dr["PAYMENT_DESC"] = Pay_Desc;
                        dr["INST_NO"] = i + 1;
                        dr["DUE_AMOUNT"] = Convert.ToDecimal(Inst_Amount);
                        dr["REC_AMOUNT"] = Convert.ToDecimal(0.00);
                        dr["OS_AMOUNT"] = Convert.ToDecimal(Inst_Amount);
                        dr["RECEIPT_NO"] = "";
                        dr["RECEIPT_AMOUNT"] = Convert.ToDecimal(0.00);
                        dr["PAYMENT_DATE"] = Constants.DateNullValue;
                        dr["IS_PAID"] = false;
                        dr["USER_ID"] = Convert.ToInt32(Session["UserID"]);
                        dr["TIME_STEMP"] = DateTime.Now;
                        InsDetail.Rows.Add(dr);

                        //sd.PAYMENT_DESC = Pay_Desc;
                        //sd.INST_NO = i + 1;
                        //sd.DUE_AMOUNT = Convert.ToDecimal(Inst_Amount);
                        //sd.PAY_AMOUNT = Convert.ToDecimal(0.00);
                        //sd.OS_AMOUNT = Convert.ToDecimal(Inst_Amount);
                        //sd.RECEIPT_NO = "";
                        //sd.RECEIPT_AMOUNT = Convert.ToDecimal(0.00);
                        //sd.PAYMENT_DATE = Constants.DateNullValue;
                        //sd.IS_PAID = false;
                        //sd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        //sd.TIME_STEMP = DateTime.Now;
                        //PIDL.Save(sd);
                    }
                }
                this.Session.Add("InsDetail", InsDetail);
                InsDetail = (DataTable)this.Session["InsDetail"];
                var SortedTable = InsDetail.AsEnumerable()
                  .OrderBy(r => r.Field<DateTime>("DUE_DATE"))
                  .CopyToDataTable();
                foreach (DataRow row in SortedTable.Rows)
                {
                    sd.PURCHASE_INS_MASTER_ID = Convert.ToInt32(row["PURCHASE_INS_MASTER_ID"].ToString());
                    sd.DUE_DATE = DateTime.Parse(row["DUE_DATE"].ToString());
                    sd.PAYMENT_DESC = row["PAYMENT_DESC"].ToString();
                    sd.INST_NO = int.Parse(row["INST_NO"].ToString());
                    sd.DUE_AMOUNT = Convert.ToDecimal(row["DUE_AMOUNT"].ToString());
                    sd.PAY_AMOUNT = Convert.ToDecimal(row["REC_AMOUNT"].ToString());
                    sd.OS_AMOUNT = Convert.ToDecimal(row["OS_AMOUNT"].ToString());
                    sd.RECEIPT_NO = row["RECEIPT_NO"].ToString();
                    sd.RECEIPT_AMOUNT = Convert.ToDecimal(row["RECEIPT_AMOUNT"].ToString());
                    sd.PAYMENT_DATE = DateTime.Parse(row["PAYMENT_DATE"].ToString());
                    sd.IS_PAID = bool.Parse(row["IS_PAID"].ToString());
                    sd.USER_ID = Convert.ToInt32(row["USER_ID"].ToString());
                    sd.TIME_STEMP = DateTime.Parse(row["TIME_STEMP"].ToString());
                    PIDL.Save(sd);
                }

            }
            DbCalls db = new DbCalls();
            var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == pm.PURCHASE_INS_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0 && x.PAYEE_NAME == "Purchase Installment").ToList();
            if (q.Count > 0)
            {
                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                string PAYEE_NAME = q.FirstOrDefault().PAYEE_NAME;
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID =" + Constants.Journal_Voucher + " and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.PURCHASE_INS_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale + "and PAYEE_NAME='" + PAYEE_NAME + "'", con))
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
            
            //var qry = (from k in db.ITEM_INFORMATION
            //           join m in db.MEASURMENT on k.MEASUREMENT equals m.MEASURMENT_ID
            //           join b in db.BLOCK on k.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on k.STREET_ID equals s.STREET_ID
            //           select new { k.ITEM_INFO_ID, ItemInfo = k.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME + ", " + m.MEASURMENT1 + ", " + k.EXTRA_AREA }).Where(x => x.ITEM_INFO_ID == pm.ITEM_INFO_ID).FirstOrDefault();
            //string ItemInfo = qry.ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.PURCHASE_AMOUNT)
                , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
                , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_INS_MASTER_ID
                , Convert.ToString(pm.PURCHASE_INS_MASTER_ID)
                , Convert.ToDecimal(pm.DISCOUNT), Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PurchaseInstallment"] = null;
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
        }
        else
        {
            pml.Save(pm);
            Purchase_Ins_Detail_Logics PIDL = new Purchase_Ins_Detail_Logics();
            Purchase_Ins_Logics PIL = new Purchase_Ins_Logics();
            tbl_PURCHASE_INS_DETAIL sd = new tbl_PURCHASE_INS_DETAIL();
            if (pm.BOOKING > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "BOOKING", 0, Convert.ToDateTime(txtPurchaseDate.Text), Convert.ToDecimal(pm.BOOKING), 0, Convert.ToDecimal(pm.BOOKING), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            if (pm.CONFIRMATION > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "CONFIRMATION", 1, DateTime.Parse(txtConfirmationDate.Text), Convert.ToDecimal(pm.CONFIRMATION), 0, Convert.ToDecimal(pm.CONFIRMATION), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            if (pm.ALLOCATION > 0)
            {
                PIDL.PurchaseInsDetail(pm.PURCHASE_INS_MASTER_ID, "ALLOCATION", 1, DateTime.Parse(txtAllocationDate.Text), Convert.ToDecimal(pm.ALLOCATION), 0, Convert.ToDecimal(pm.ALLOCATION), "", 0, Constants.DateNullValue, Convert.ToInt32(Session["UserID"]), DateTime.Now, false);
            }
            DataTable dt = new DataTable();
            dt = (DataTable)Session["InstallmentDetail"];
            int Ins_Type = Constants.IntNullValue;
            string Pay_Desc = "";
            int No_of_Inst = Constants.IntNullValue;
            decimal Inst_Amount = Constants.DecimalNullValue;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Ins_Type = int.Parse(row["InstallmentType_ID"].ToString());
                    Pay_Desc = row["InstallmentType"].ToString();
                    No_of_Inst = int.Parse(row["NoOfInstallment"].ToString());
                    Inst_Amount = decimal.Parse(row["InstallmentAmount"].ToString());
                    decimal Total_Ins_Amount = No_of_Inst * Inst_Amount;
                    DateTime dtFrom = new DateTime();
                    DateTime dtFromMonth = DateTime.Parse(row["StartMonth"].ToString());
                    dtFrom = new DateTime(dtFromMonth.Year, dtFromMonth.Month, 1);
                    dtFrom = dtFrom.AddMonths(1).AddDays(-1);
                    PIL.Save(pm.PURCHASE_INS_MASTER_ID, Ins_Type, Pay_Desc, No_of_Inst, Inst_Amount, dtFromMonth, Total_Ins_Amount);
                    int month = 0;
                    for (int i = 0; i <= No_of_Inst - 1; i++)
                    {
                        InsDetail = (DataTable)this.Session["InsDetail"];
                        DataRow dr = InsDetail.NewRow();
                        dr["PURCHASE_INS_MASTER_ID"] = Convert.ToInt32(pm.PURCHASE_INS_MASTER_ID);
                        //sd.PURCHASE_INS_MASTER_ID = pm.PURCHASE_INS_MASTER_ID;
                        if (Ins_Type == 1)
                        {
                            if (month != i)
                            {
                                month += 3;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 2)
                        {
                            if (month != i)
                            {
                                month += 6;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 3)
                        {
                            if (month != i)
                            {
                                month += 12;
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(month).ToShortDateString());
                            }
                            else
                            {
                                //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                                dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            }
                        }
                        else if (Ins_Type == 4)
                        {
                            //sd.DUE_DATE = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                            dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                        }
                        else if (Ins_Type == 5)
                        {
                            dr["DUE_DATE"] = Convert.ToDateTime(dtFrom.AddMonths(i).ToShortDateString());
                        }
                        dr["PAYMENT_DESC"] = Pay_Desc;
                        dr["INST_NO"] = i + 1;
                        dr["DUE_AMOUNT"] = Convert.ToDecimal(Inst_Amount);
                        dr["REC_AMOUNT"] = Convert.ToDecimal(0.00);
                        dr["OS_AMOUNT"] = Convert.ToDecimal(Inst_Amount);
                        dr["RECEIPT_NO"] = "";
                        dr["RECEIPT_AMOUNT"] = Convert.ToDecimal(0.00);
                        dr["PAYMENT_DATE"] = Constants.DateNullValue;
                        dr["IS_PAID"] = false;
                        dr["USER_ID"] = Convert.ToInt32(Session["UserID"]);
                        dr["TIME_STEMP"] = DateTime.Now;
                        InsDetail.Rows.Add(dr);

                        //sd.PAYMENT_DESC = Pay_Desc;
                        //sd.INST_NO = i + 1;
                        //sd.DUE_AMOUNT = Convert.ToDecimal(Inst_Amount);
                        //sd.PAY_AMOUNT = Convert.ToDecimal(0.00);
                        //sd.OS_AMOUNT = Convert.ToDecimal(Inst_Amount);
                        //sd.RECEIPT_NO = "";
                        //sd.RECEIPT_AMOUNT = Convert.ToDecimal(0.00);
                        //sd.PAYMENT_DATE = Constants.DateNullValue;
                        //sd.IS_PAID = false;
                        //sd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        //sd.TIME_STEMP = DateTime.Now;
                        //PIDL.Save(sd);
                    }
                }
                this.Session.Add("InsDetail", InsDetail);
                InsDetail = (DataTable)this.Session["InsDetail"];
                var SortedTable = InsDetail.AsEnumerable()
                  .OrderBy(r => r.Field<DateTime>("DUE_DATE"))
                  .CopyToDataTable();
                foreach (DataRow row in SortedTable.Rows)
                {
                    sd.PURCHASE_INS_MASTER_ID = Convert.ToInt32(row["PURCHASE_INS_MASTER_ID"].ToString());
                    sd.DUE_DATE = DateTime.Parse(row["DUE_DATE"].ToString());
                    sd.PAYMENT_DESC = row["PAYMENT_DESC"].ToString();
                    sd.INST_NO = int.Parse(row["INST_NO"].ToString());
                    sd.DUE_AMOUNT = Convert.ToDecimal(row["DUE_AMOUNT"].ToString());
                    sd.PAY_AMOUNT = Convert.ToDecimal(row["REC_AMOUNT"].ToString());
                    sd.OS_AMOUNT = Convert.ToDecimal(row["OS_AMOUNT"].ToString());
                    sd.RECEIPT_NO = row["RECEIPT_NO"].ToString();
                    sd.RECEIPT_AMOUNT = Convert.ToDecimal(row["RECEIPT_AMOUNT"].ToString());
                    sd.PAYMENT_DATE = DateTime.Parse(row["PAYMENT_DATE"].ToString());
                    sd.IS_PAID = bool.Parse(row["IS_PAID"].ToString());
                    sd.USER_ID = Convert.ToInt32(row["USER_ID"].ToString());
                    sd.TIME_STEMP = DateTime.Parse(row["TIME_STEMP"].ToString());
                    PIDL.Save(sd);
                }
            }
            //DbCalls db = new DbCalls();
            //var qry = (from k in db.ITEM_INFORMATION
            //           join m in db.MEASURMENT on k.MEASUREMENT equals m.MEASURMENT_ID
            //           join b in db.BLOCK on k.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on k.STREET_ID equals s.STREET_ID
            //           select new { k.ITEM_INFO_ID, ItemInfo = k.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME + ", " + m.MEASURMENT1 + ", " + k.EXTRA_AREA }).Where(x => x.ITEM_INFO_ID == pm.ITEM_INFO_ID).FirstOrDefault();
            //string ItemInfo = qry.ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.PURCHASE_AMOUNT)
                , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
                , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_INS_MASTER_ID
                , Convert.ToString(pm.PURCHASE_INS_MASTER_ID)
                , Convert.ToDecimal(pm.DISCOUNT), Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
        }
        Items(9);
        LoadGrid("");
        DoEmptyTextBox();
        LoadProject();
        LoadBlock();
        LoadStreet();
        LoadItems("");
        mastergrid.Visible = true;
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        Panel3.Visible = true;


    }
    private void LedgerEntries(int p_Distributor_Id
     , decimal p_Debit, decimal p_Credit, DateTime p_Ledger_Date, int p_PRINCIPAL_ID
     , int p_DOCUMENT_TYPE_ID, string pChequeNo, int p_UserId, long p_DOCUMENT_NO
     , string p_MANUAL_DOCUMENT_NO, decimal p_Discount_Amount, decimal p_TokenAmount, decimal p_DownPayment, string p_Remarks)
    {
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            #region GL Master, Detail
            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);
            //DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            //DataRow[] drConfig = null;
            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.CreditSales + "'");
            //long PurchaseAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());
            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.AccountPayable + "'");
            //long PayableAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());
            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.PurchaseDiscount + "'");
            //long PurchaseDiscount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            if (p_DOCUMENT_TYPE_ID == Constants.Document_Purchase)
            {
                //if (!Is_INS)
                //{
                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Sale-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");
                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Sale-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");

                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");
                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");

                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");
                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");

                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");
                //    LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale");
                //}
                //else
                //{
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Installment Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Installment",0,Constants.Document_Purchase_Installment);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Installment Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Installment", 0, Constants.Document_Purchase_Installment);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Installment Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Installment", 0, Constants.Document_Purchase_Installment);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Installment Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Installment", 0, Constants.Document_Purchase_Installment);
                //}
                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.CreditSale
                        , p_MANUAL_DOCUMENT_NO, "Purchase Installment Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Purchase Installment", mTransaction, mConnection);
                    decimal PurchaseInstallment = p_Credit - p_Discount_Amount;
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, PurchaseInstallment, 0, "Purchase Installment Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, 0, PurchaseInstallment, "Purchase Installment Voucher", mTransaction, mConnection);

                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Discount_Amount, 0, "Purchase Installment Discount Voucher", mTransaction, mConnection);
                    //LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchase_Discount, 0, p_Discount_Amount, "Purchase Installment Discount Voucher", mTransaction, mConnection);

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

    public class InstType
    {
        public int InstallmentType_ID { get; set; }
        public string InstallmentType { get; set; }
        public int NoOfInstallment { get; set; }
        public DateTime StartMonth { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public int TYPE_ID { get; set; }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        DoEmptyTextBox();
        Panel3.Visible = true;
    }
    protected void GridSaleMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridSaleMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }
    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        //decimal Balance;
        //int BOOKING_PER_VAL = Constants.IntNullValue;
        //int CONFIRMATION_PER_VAL = Constants.IntNullValue;
        //int ALLOCATION_PER_VAL = Constants.IntNullValue;
        //DbCalls db = new DbCalls();
        //Session["PurchaseInstallment"] = null;
        //Session["InsDetail"] = null;
        //InsDetailTable();
        //Session["Balance"] = null;
        //var ProID = ((LinkButton)sender).CommandArgument;
        //PurchaseInstallmentLogics Ser = new PurchaseInstallmentLogics();
        //var data = Ser.Get(Convert.ToInt32(ProID));
        //var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_INS_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0 && x.PAYEE_NAME == "Purchase Installment").ToList();
        //if (q.Any())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update. Some payments are issued against this purchase.');", true);
        //}
        //else
        //{
        //    var qq = (from k in db.tbl_SALE_MASTER
        //             join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
        //             select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
        //    if (qq.Any())
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update because this item has been sold.');", true);
        //    }
        //    else
        //    {
        //        DataTable dt = new DataTable();
        //        DataTable dtItem = new DataTable();
        //        Items(9);
        //        Items(10);
        //        dtItem = (DataTable)this.Session["dtItem"];
        //        DataRow[] foundRows = dtItem.Select("ITEM_INFO_ID  = '" + data.ITEM_INFO_ID + "'");
        //        if (foundRows.Length > 0)
        //        {
        //            dt = (DataTable)this.Session["dt"];
        //            DataRow dr = dt.NewRow();
        //            dr["DISTRIBUTOR_ID"] = Convert.ToInt32(foundRows[0]["DISTRIBUTOR_ID"].ToString());
        //            dr["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
        //            dr["PROJECT_NAME"] = foundRows[0]["PROJECT_NAME"].ToString();
        //            dr["BLOCK_ID"] = Convert.ToInt32(foundRows[0]["BLOCK_ID"].ToString());
        //            dr["BLOCK_NAME"] = foundRows[0]["BLOCK_NAME"].ToString();
        //            dr["STREET_ID"] = Convert.ToInt32(foundRows[0]["STREET_ID"].ToString());
        //            dr["STREET_NAME"] = foundRows[0]["STREET_NAME"].ToString();
        //            dr["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
        //            dr["ITEM_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
        //            dr["Consumption"] = foundRows[0]["Consumption"].ToString();
        //            //dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
        //            dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
        //            dt.Rows.Add(dr);
        //            Session["dt"] = dt;
        //        }
        //        LoadDistributor();
        //        LoadProject();
        //        ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
        //        LoadBlock();
        //        ddlBlock.SelectedValue = Convert.ToString(data.BLOCK_ID);

        //        LoadStreet();
        //        ddlStreet.SelectedValue = Convert.ToString(data.STREET_ID);

        //        LoadItems(ProID);
        //        LoadVendors();


        //        DateTime Date = data.DATE;
        //        txtPurchaseDate.Text = Date.ToString("dd-MMM-yyyy");

        //        DateTime CONDate = data.CONFIRMATION_DATE;
        //        txtConfirmationDate.Text = CONDate.ToString("dd-MMM-yyyy");

        //        DateTime AllDate = data.ALLOCATION_DATE;
        //        txtAllocationDate.Text = AllDate.ToString("dd-MMM-yyyy");


        //        ddlLocation.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);

        //        ddlItem.SelectedValue = Convert.ToString(data.ITEM_INFO_ID);
        //        ddlVendor.SelectedValue = Convert.ToString(data.VENDOR_ID);
        //        txtPurchaseAmount.Text = Math.Round(data.PURCHASE_AMOUNT).ToString();
        //        HiddenPurchaseAmount.Value = Math.Round(data.PURCHASE_AMOUNT).ToString();
        //        txtDiscount.Text = Math.Round(Convert.ToDecimal(data.DISCOUNT)).ToString();
        //        HiddenDiscount.Value = Math.Round(Convert.ToDecimal(data.DISCOUNT)).ToString();
        //        Balance = Convert.ToDecimal(data.PURCHASE_AMOUNT) - Convert.ToDecimal(data.DISCOUNT);
        //        BOOKING_PER_VAL = Convert.ToInt32(data.BOOKING_PER_VAL);
        //        CONFIRMATION_PER_VAL = Convert.ToInt32(data.CONFIRMATION_PER_VAL);
        //        ALLOCATION_PER_VAL = Convert.ToInt32(data.ALLOCATION_PER_VAL);
        //        if (BOOKING_PER_VAL != 1)
        //        {
        //            RbtBooking.SelectedValue = "2";
        //            txtBookingValue.Text = Math.Round(Convert.ToDecimal(data.BOOKING)).ToString();
        //            HiddenBookingValue.Value = Math.Round(Convert.ToDecimal(data.BOOKING)).ToString();
        //            txtBookingPercent.Text = "0";
        //            HiddenBookingPercent.Value = "0";
        //        }
        //        else
        //        {
        //            RbtBooking.SelectedValue = "1";
        //            txtBookingValue.Text = Math.Round(Convert.ToDecimal(data.BOOKING)).ToString();
        //            HiddenBookingValue.Value = Math.Round(Convert.ToDecimal(data.BOOKING)).ToString();
        //            decimal Percent = Convert.ToDecimal(data.BOOKING) / Balance * 100;
        //            txtBookingPercent.Text = Math.Round(Percent).ToString();
        //            HiddenBookingPercent.Value = Math.Round(Percent).ToString();
        //        }
        //        if (CONFIRMATION_PER_VAL != 1)
        //        {
        //            RbtConfirmation.SelectedValue = "2";
        //            txtConfirmationValue.Text = Math.Round(Convert.ToDecimal(data.CONFIRMATION)).ToString();
        //            HiddenConfirmationValue.Value = Math.Round(Convert.ToDecimal(data.CONFIRMATION)).ToString();
        //            txtConfirmationPercent.Text = "0";
        //            HiddenConfirmationPercent.Value = "0";
        //        }
        //        else
        //        {
        //            RbtConfirmation.SelectedValue = "1";
        //            txtConfirmationValue.Text = Math.Round(Convert.ToDecimal(data.CONFIRMATION)).ToString();
        //            HiddenConfirmationValue.Value = Math.Round(Convert.ToDecimal(data.CONFIRMATION)).ToString();
        //            decimal Percent = Convert.ToDecimal(data.CONFIRMATION) / Balance * 100;
        //            txtConfirmationPercent.Text = Math.Round(Percent).ToString();
        //            HiddenConfirmationPercent.Value = Math.Round(Percent).ToString();
        //        }
        //        if (ALLOCATION_PER_VAL != 1)
        //        {
        //            RbtAllocation.SelectedValue = "2";
        //            txtAllocationValue.Text = Math.Round(Convert.ToDecimal(data.ALLOCATION)).ToString();
        //            HiddenAllocationValue.Value = Math.Round(Convert.ToDecimal(data.ALLOCATION)).ToString();
        //            txtAllocationPercent.Text = "0";
        //            HiddenAllocationPercent.Value = "0";
        //        }
        //        else
        //        {
        //            RbtAllocation.SelectedValue = "1";
        //            txtAllocationValue.Text = Math.Round(Convert.ToDecimal(data.ALLOCATION)).ToString();
        //            HiddenAllocationValue.Value = Math.Round(Convert.ToDecimal(data.ALLOCATION)).ToString();
        //            decimal Percent = Convert.ToDecimal(data.ALLOCATION) / Balance * 100;
        //            txtAllocationPercent.Text = Math.Round(Percent).ToString();
        //            HiddenAllocationPercent.Value = Math.Round(Percent).ToString();
        //        }
        //        decimal BalanceAmount = Balance - Convert.ToDecimal(data.BOOKING) - Convert.ToDecimal(data.CONFIRMATION) - Convert.ToDecimal(data.ALLOCATION);
        //        txtBalance.Text = Math.Round(BalanceAmount).ToString();
        //        HiddenBalance.Value = Math.Round(BalanceAmount).ToString();
        //        Session["Balance"] = BalanceAmount;
        //        txtRemarks.Text = data.REMARKS;
        //        var qry = (from k in db.tbl_PURCHASE_INS select new { k.PURCHASE_INS_MASTER_ID, InstallmentType_ID = k.INS_TYPE, InstallmentType = k.INS_TYPE_NAME, StartMonth = k.FROM_MONTH, NoOfInstallment = k.NO_INS, InstallmentAmount = k.INS_AMOUNT, TotalInstallmentAmount = k.TOTAL_INS_AMOUNT, PreviousAmount = k.INS_AMOUNT }).Where(x => x.PURCHASE_INS_MASTER_ID == data.PURCHASE_INS_MASTER_ID).ToList();
        //        if (qry.Any())
        //        {
        //            GrdInstallment.DataSource = qry;
        //            GrdInstallment.DataBind();
        //            ///this.Session.Add("InstallmentDetail", qry.CopyToDataTable());
        //            foreach (var item in qry)
        //            {
        //                InstallmentDetail = (DataTable)this.Session["InstallmentDetail"];
        //                DataRow dr = InstallmentDetail.NewRow();
        //                dr["InstallmentType_ID"] = Convert.ToInt32(item.InstallmentType_ID);
        //                dr["InstallmentType"] = item.InstallmentType;
        //                dr["NoOfInstallment"] = Convert.ToInt32(item.NoOfInstallment);
        //                dr["InstallmentAmount"] = decimal.Parse(item.InstallmentAmount.ToString());
        //                dr["StartMonth"] = Convert.ToDateTime(item.StartMonth);
        //                dr["TotalInstallmentAmount"] = item.TotalInstallmentAmount;
        //                dr["PreviousAmount"] = item.PreviousAmount;
        //                InstallmentDetail.Rows.Add(dr);
        //            }
        //            this.Session.Add("InstallmentDetail", InstallmentDetail);
        //        }
        //        else
        //        {
        //            CreatInsTable();
        //        }

        //        decimal TotalAmount = 0;
        //        foreach (GridViewRow gvr in GrdInstallment.Rows)
        //        {
        //            TotalAmount += Convert.ToDecimal(gvr.Cells[5].Text);
        //        }
        //        decimal AMOUNT = Convert.ToDecimal(Balance) - TotalAmount - Convert.ToDecimal(data.BOOKING) - Convert.ToDecimal(data.CONFIRMATION) - Convert.ToDecimal(data.ALLOCATION);
        //        txtTotalRemainingAmount.Text = string.Format("{0:n2}", Math.Round(AMOUNT).ToString());
        //        btnSave.Text = "Update";
        //        Session.Add("PurchaseInstallment", data.PURCHASE_INS_MASTER_ID);
        //        Panelgrid.Visible = false;
        //        PanelSale.Visible = true;
        //        PanelInstallments.Visible = true;
        //        TotalTable.Visible = true;
        //        Panel3.Visible = false;
        //        HFeild.Value = "1";
        //    }

        //}

    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //DbCalls db = new DbCalls();
        //var ProID = ((LinkButton)sender).CommandArgument;
        //PurchaseInstallmentLogics Ser = new PurchaseInstallmentLogics();
        //var data = Ser.Get(Convert.ToInt32(ProID));
        //var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_INS_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0 && x.PAYEE_NAME == "Purchase Installment").ToList();
        //if (qry.Any())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not be deleted. Some payments are issued against this purchase.');", true);
        //}
        //else
        //{
        //    var qq = (from k in db.tbl_SALE_MASTER
        //             join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
        //             select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
        //    if (qq.Any())
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete because this item has been sold.');", true);
        //    }
        //    else
        //    {
        //        var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_INS_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0 && x.PAYEE_NAME == "Purchase Installment").ToList();
        //        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //        {
        //            con.Open();
        //            using (SqlCommand command1 = new SqlCommand("DELETE tbl_PURCHASE_INS_MASTER where PURCHASE_INS_MASTER_ID =" + data.PURCHASE_INS_MASTER_ID, con))
        //            {
        //                command1.ExecuteNonQuery();
        //            }
        //            using (SqlCommand command = new SqlCommand("delete tbl_PURCHASE_INS_DETAIL where PURCHASE_INS_MASTER_ID=" + data.PURCHASE_INS_MASTER_ID, con))
        //            {
        //                command.ExecuteNonQuery();
        //            }
        //            using (SqlCommand command2 = new SqlCommand("delete tbl_PURCHASE_INS where PURCHASE_INS_MASTER_ID=" + data.PURCHASE_INS_MASTER_ID, con))
        //            {
        //                command2.ExecuteNonQuery();
        //            }
        //            if (q.Count > 0)
        //            {
        //                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
        //                string PAYEE_NAME = q.FirstOrDefault().PAYEE_NAME;
        //                using (SqlCommand command3 = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and PRINCIPAL_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_INS_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale + "and PAYEE_NAME='" + PAYEE_NAME + "'", con))
        //                {
        //                    command3.ExecuteNonQuery();
        //                }
        //                using (SqlCommand command4 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
        //                {
        //                    command4.ExecuteNonQuery();
        //                }
        //                using (SqlCommand command5 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
        //                {
        //                    command5.ExecuteNonQuery();
        //                }
        //            }

        //            con.Close();
        //        }
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record Deleted successfully.');", true);
        //    }
        //}
        //LoadGrid("");
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //DataControl dc = new DataControl();
        InstallmentDetail = (DataTable)this.Session["InstallmentDetail"];
        if (txtNoofInstallment.Text.Equals("") || txtNoofInstallment.Text.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Number of installments is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtNoofInstallment);
            return;
        }
        if (ddlInstallmentType.SelectedValue.ToString() != "4")
        {
            if (txtInstallmentAmount.Text.Equals("") || txtInstallmentAmount.Text.Equals("0"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Installment amount is required.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtInstallmentAmount);
                return;
            }
        }
        if (txtBalance.Text.Equals("") || txtBalance.Text.Equals("0"))
        {
            if (txtPurchaseAmount.Text.Equals("") || txtPurchaseAmount.Text.Equals("0"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter Purchase Amount first.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                return;
            }
            else
            {
                if (txtBalance.Text.Equals("0"))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Balance should be greater than zero.');", true);
                    ScriptManager.GetCurrent(Page).SetFocus(txtBalance);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter Purchase Amount first.');", true);
                    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                    return;
                }

            }


        }
        
        var check = (from p in InstallmentDetail.AsEnumerable()
                    select p).ToList();
        if (check.Any())
        {
            if (txtTotalRemainingAmount.Text.Equals("") || txtTotalRemainingAmount.Text.Equals("0") || txtTotalRemainingAmount.Text.Equals("0.00"))
            {
                if (Session["PurchaseInstallment"] == null) { 
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Your remaining balance is zero. You can't add an other installment type.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                return;
                }
            }
            else
            {
                int no = Convert.ToInt32(txtNoofInstallment.Text);
                decimal ins_amount = decimal.Parse(txtInstallmentAmount.Text);
                decimal amount = no * ins_amount;
                if (amount > decimal.Parse(txtTotalRemainingAmount.Text))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtNoofInstallment.Text + " X " + txtInstallmentAmount.Text + " = " + amount + " should not be greater then Total Remaining Amount.');", true);
                    ScriptManager.GetCurrent(Page).SetFocus(txtInstallmentAmount);
                    return;
                }
            }
        }
        else
        {
            int no = Convert.ToInt32(txtNoofInstallment.Text);
            decimal ins_amount = decimal.Parse(txtInstallmentAmount.Text);
            decimal amount = no * ins_amount;
            if (amount > decimal.Parse(txtBalance.Text))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtNoofInstallment.Text + " X " + txtInstallmentAmount.Text + " = " + amount + " should not be greater then Balance.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtInstallmentAmount);
                return;
            }
        }
        if (!CheckDublicate())
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + ddlInstallmentType.SelectedItem.Text + " installments are already exists.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlInstallmentType);
            return;
        }
        decimal Ins_Amount;
        int NoIns = Convert.ToInt32(txtNoofInstallment.Text);
        if (Convert.ToInt32(RowId.Value) >= 0)
        {
           
                DataRow dr = InstallmentDetail.Rows[Convert.ToInt32(RowId.Value)];
                if (ddlInstallmentType.SelectedValue == "4")
                {
                    decimal MonthlyInsAmount = Convert.ToDecimal(Session["MonthlyInsAmount"]);
                    decimal Balance = MonthlyInsAmount;
                    Ins_Amount = Balance / NoIns;
                    dr["InstallmentAmount"] = Ins_Amount;
                    dr["InstallmentType_ID"] = Convert.ToInt32(ddlInstallmentType.SelectedValue);
                    dr["InstallmentType"] = ddlInstallmentType.SelectedItem.Text;
                    dr["NoOfInstallment"] = Convert.ToInt32(txtNoofInstallment.Text);
                    dr["StartMonth"] = Convert.ToDateTime(txtFromMonth.Text);
                    decimal InsAmount = NoIns * Ins_Amount;
                    dr["TotalInstallmentAmount"] = InsAmount;
                    Session["MonthlyInsAmount"] = null;
                }
                else
                {
                    decimal OldAmount = Convert.ToDecimal(Session["InsAmount"]);
                    dr["InstallmentAmount"] = decimal.Parse(txtInstallmentAmount.Text);
                    Ins_Amount = decimal.Parse(txtInstallmentAmount.Text);
                    dr["InstallmentType_ID"] = Convert.ToInt32(ddlInstallmentType.SelectedValue);
                    dr["InstallmentType"] = ddlInstallmentType.SelectedItem.Text;
                    dr["NoOfInstallment"] = Convert.ToInt32(txtNoofInstallment.Text);
                    dr["StartMonth"] = Convert.ToDateTime(txtFromMonth.Text);
                    decimal InsAmount = NoIns * Ins_Amount;
                    dr["TotalInstallmentAmount"] = InsAmount;
                    if (InsAmount != OldAmount)
                    {
                        var chek = (from p in InstallmentDetail.AsEnumerable()
                                    where p.Field<int>("InstallmentType_ID") == 4
                                    select p).ToList();
                        if (chek.Any())
                        {
                            var query = from p in InstallmentDetail.AsEnumerable()
                                        where p.Field<int>("InstallmentType_ID") == 4
                                        select new
                                        {
                                            NoOfInstallment = p.Field<int>("NoOfInstallment"),
                                            TotalInstallmentAmount = p.Field<decimal>("TotalInstallmentAmount")
                                        };
                            int noinst = Convert.ToInt32(query.FirstOrDefault().NoOfInstallment);
                            decimal instamount = Convert.ToDecimal(query.FirstOrDefault().TotalInstallmentAmount);
                            if (InsAmount > OldAmount)
                            {
                                decimal Diff = InsAmount - OldAmount;
                                decimal Amount = instamount - Diff;
                                decimal instmnts = Amount / noinst;
                                DataRow row = InstallmentDetail.AsEnumerable().Where(r => ((int)r["InstallmentType_ID"]).Equals(4)).First();
                                row["InstallmentAmount"] = instmnts;
                                row["TotalInstallmentAmount"] = Amount;
                            }
                            else
                            {
                                decimal Diff = OldAmount - InsAmount;
                                decimal Amount = instamount + Diff;
                                decimal instmnts = Amount / noinst;
                                DataRow row = InstallmentDetail.AsEnumerable().Where(r => ((int)r["InstallmentType_ID"]).Equals(4)).First();
                                row["InstallmentAmount"] = instmnts;
                                row["TotalInstallmentAmount"] = Amount;
                            }
                        }
                    }
                    Session["InsAmount"] = null;
                }

        }
        else
        {

            DataRow dr = InstallmentDetail.NewRow();
            if (ddlInstallmentType.SelectedValue == "4")
            {
                if (txtTotalRemainingAmount.Text.Equals(""))
                {
                    decimal Balance = decimal.Parse(txtBalance.Text);
                    Ins_Amount = Balance / NoIns;
                    dr["InstallmentAmount"] = Ins_Amount;
                }
                else
                {
                    decimal Balance = decimal.Parse(txtTotalRemainingAmount.Text);
                    Ins_Amount = Balance / NoIns;
                    dr["InstallmentAmount"] = Ins_Amount;
                }
                dr["InstallmentType_ID"] = Convert.ToInt32(ddlInstallmentType.SelectedValue);
                dr["InstallmentType"] = ddlInstallmentType.SelectedItem.Text;
                dr["NoOfInstallment"] = Convert.ToInt32(txtNoofInstallment.Text);
                dr["StartMonth"] = Convert.ToDateTime(txtFromMonth.Text);
                decimal InsAmount = NoIns * Ins_Amount;
                dr["TotalInstallmentAmount"] = InsAmount;
                InstallmentDetail.Rows.Add(dr);
            }
            else
            {
                dr["InstallmentAmount"] = decimal.Parse(txtInstallmentAmount.Text);
                Ins_Amount = decimal.Parse(txtInstallmentAmount.Text);
                dr["InstallmentType_ID"] = Convert.ToInt32(ddlInstallmentType.SelectedValue);
                dr["InstallmentType"] = ddlInstallmentType.SelectedItem.Text;
                dr["NoOfInstallment"] = Convert.ToInt32(txtNoofInstallment.Text);
                dr["StartMonth"] = Convert.ToDateTime(txtFromMonth.Text);
                decimal InsAmount = NoIns * Ins_Amount;
                dr["TotalInstallmentAmount"] = InsAmount;
                InstallmentDetail.Rows.Add(dr);
                var chek = (from p in InstallmentDetail.AsEnumerable()
                            where p.Field<int>("InstallmentType_ID") == 4
                            select p).ToList();
                if (chek.Any())
                {
                    var query = from p in InstallmentDetail.AsEnumerable()
                                where p.Field<int>("InstallmentType_ID") == 4
                                select new
                                {
                                    NoOfInstallment = p.Field<int>("NoOfInstallment"),
                                    TotalInstallmentAmount = p.Field<decimal>("TotalInstallmentAmount")
                                };
                    int noinst = Convert.ToInt32(query.FirstOrDefault().NoOfInstallment);
                    decimal instamount = Convert.ToDecimal(query.FirstOrDefault().TotalInstallmentAmount);
                    decimal Amount = instamount - InsAmount;
                    decimal instmnts = Amount / noinst;
                    DataRow row = InstallmentDetail.AsEnumerable().Where(r => ((int)r["InstallmentType_ID"]).Equals(4)).First();
                    row["InstallmentAmount"] = instmnts;
                    row["TotalInstallmentAmount"] = Amount;
                }
            }
            
        }

        Clear();
        GrdInstallment.DataSource = InstallmentDetail;
        GrdInstallment.DataBind();
        btnAdd.Text = "Add";
        this.Session.Add("InstallmentDetail", InstallmentDetail);
        ScriptManager.GetCurrent(Page).SetFocus(ddlInstallmentType);
        PanelInstallments.Visible = true;
        TotalTable.Visible = true;
        decimal TotalAmount = 0;
        foreach (GridViewRow gvr in GrdInstallment.Rows)
        {
            TotalAmount += Convert.ToDecimal(gvr.Cells[5].Text);
        }
        decimal AMOUNT = Convert.ToDecimal(txtBalance.Text) - TotalAmount;
        txtTotalRemainingAmount.Text = string.Format("{0:n2}", AMOUNT);
        HiddenPurchaseAmount.Value = "0";
        HiddenDiscount.Value = "0";
        HiddenBookingPercent.Value = "0";
        HiddenBookingValue.Value = "0";
        HiddenConfirmationPercent.Value = "0";
        HiddenConfirmationValue.Value = "0";
        HiddenAllocationPercent.Value = "0";
        HiddenAllocationValue.Value = "0";
        HiddenBalance.Value = "0";
    }
    public void Clear()
    {
        #region Clear Txtbox
        txtInstallmentAmount.Text = "";
        txtNoofInstallment.Text = "";
        RowId.Value = "-1";
        txtFromMonth.Text = DateTime.Now.ToString("MMM-yyyy");
        #endregion
    }
    private bool CheckDublicate()
    {
        if (Convert.ToInt32(RowId.Value) >= 0)
        {
            return true;
        }
        InstallmentDetail = (DataTable)this.Session["InstallmentDetail"];
        DataRow[] foundRows = InstallmentDetail.Select("InstallmentType_ID  = '" + ddlInstallmentType.SelectedValue + "'");
        if (foundRows.Length == 0)
        {
            return true;
        }
        return false;



    }
    protected void GrdInstallment_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void GrdInstallment_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        InstallmentDetail = (DataTable)this.Session["InstallmentDetail"];
        int InstType = int.Parse(GrdInstallment.Rows[e.RowIndex].Cells[0].Text);
        decimal Val = decimal.Parse(GrdInstallment.Rows[e.RowIndex].Cells[5].Text);
        InstallmentDetail.Rows.RemoveAt(e.RowIndex);
        if (InstallmentDetail.Rows.Count >= 0)
        {
            if (InstType != 4)
            {
                var chek = (from p in InstallmentDetail.AsEnumerable()
                            where p.Field<int>("InstallmentType_ID") == 4
                            select p).ToList();
                if (chek.Any())
                {
                    var query = from p in InstallmentDetail.AsEnumerable()
                                where p.Field<int>("InstallmentType_ID") == 4
                                select new
                                {
                                    NoOfInstallment = p.Field<int>("NoOfInstallment"),
                                    TotalInstallmentAmount = p.Field<decimal>("TotalInstallmentAmount")
                                };
                    int noinst = Convert.ToInt32(query.FirstOrDefault().NoOfInstallment);
                    decimal instamount = Convert.ToDecimal(query.FirstOrDefault().TotalInstallmentAmount);
                    decimal Amount = instamount + Val;
                    decimal instmnts = Amount / noinst;
                    DataRow row = InstallmentDetail.AsEnumerable().Where(r => ((int)r["InstallmentType_ID"]).Equals(4)).First();
                    row["InstallmentAmount"] = instmnts;
                    row["TotalInstallmentAmount"] = Amount;
                }
            }
            GrdInstallment.DataSource = InstallmentDetail;
            GrdInstallment.DataBind();
            RowId.Value = "-1";
            btnAdd.Text = "Add";
            PanelInstallments.Visible = true;
            TotalTable.Visible = true;
            decimal TotalAmount = 0;
            foreach (GridViewRow gvr in GrdInstallment.Rows)
            {
                TotalAmount += Convert.ToDecimal(gvr.Cells[5].Text);
            }
            decimal AMOUNT = Convert.ToDecimal(txtBalance.Text) - TotalAmount;
            txtTotalRemainingAmount.Text = string.Format("{0:n2}", AMOUNT);
        }
        else
        {
            GrdInstallment.DataSource = InstallmentDetail;
            GrdInstallment.DataBind();
            RowId.Value = "-1";
            btnAdd.Text = "Add";
            PanelInstallments.Visible = false;
            TotalTable.Visible = false;
        }

    }
    protected void GrdInstallment_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Session["MonthlyInsAmount"] = null;
        Session["InsAmount"] = null;
        RowId.Value = e.NewEditIndex.ToString();
        ddlInstallmentType.SelectedValue = GrdInstallment.Rows[e.NewEditIndex].Cells[0].Text;
        DateTime Month = Convert.ToDateTime(GrdInstallment.Rows[e.NewEditIndex].Cells[2].Text);
        txtFromMonth.Text = Month.ToString("MMM-yyyy");
        txtNoofInstallment.Text = GrdInstallment.Rows[e.NewEditIndex].Cells[3].Text;
        txtInstallmentAmount.Text = GrdInstallment.Rows[e.NewEditIndex].Cells[4].Text;
        if (ddlInstallmentType.SelectedValue == "4")
        {
            decimal MonthlyInstAmount = Convert.ToDecimal(GrdInstallment.Rows[e.NewEditIndex].Cells[5].Text);
            Session["MonthlyInsAmount"] = MonthlyInstAmount;
            txtInstallmentAmount.Enabled = false;
        }
        else
        {
            decimal InstAmount = Convert.ToDecimal(GrdInstallment.Rows[e.NewEditIndex].Cells[5].Text);
            Session["InsAmount"] = InstAmount;
        }
        btnAdd.Text = "Update";
        PanelInstallments.Visible = true;
        TotalTable.Visible = true;
    }
    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {  
        LoadStreet();
        LoadItems("");
        ddlStreet.Focus();
    }

    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
        ddlItem.Focus();
    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearch.Text);
    }
}