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

public partial class Forms_frmReceivingEntry : System.Web.UI.Page
{
    DataTable PurchaseDetail;
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    IDbTransaction mTransaction;
    IDbConnection mConnection;
    DbCalls db = new DbCalls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CreatTable();
            LoadProject();
            LoadDistributor();
            LoadIssuedItems(1);
            LoadCategories();
            LoadSubCategories();
            LoadVendors();
            //LoadItems("");
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            PanelPerchase.Visible = false;
            panelgrid.Visible = true;
            LoadGrid("");
            Session["IssuanceMasterID"] = null;
            Session["UpdateData"] = 0;
        }
    }

    #region Load Dropdowns

    protected void LoadProject()
    {
        //ddlProject.Items.Clear();
        //DataTable dt = (DataTable)this.Session["Idt"];
        //if (dt.Rows.Count > 0)
        //{
        //    DataTable category = dt.AsEnumerable().GroupBy(c => c.Field<int>("ISSUANCE_MASTER_ID")).Select(y => y.FirstOrDefault()).CopyToDataTable();
        //    ddlProject.DataSource = category;
        //    ddlProject.TextField = "ITEM_INFO_NAME";
        //    ddlProject.ValueField = "ISSUANCE_MASTER_ID";
        //    ddlProject.DataBind();
        //    ddlProject.SelectedIndex = 0;
        //}
        //else
        //{
        //    ddlProject.Items.Add(new ListItem("", "0"));
        //}
        //ddlProject.DataSource = DropDowns.PROJECTDropDown();
        //ddlProject.TextField = "PROJECT_NAME";
        //ddlProject.ValueField = "PROJECT_ID";
        //ddlProject.DataBind();
        //ddlProject.SelectedIndex = 0;
    }
    protected void LoadDistributor()
    {
        int UserID = Convert.ToInt32(Session["UserID"]);
        ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
        ddlLocation.TextField = "DISTRIBUTOR_NAME";
        ddlLocation.ValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
        ddlLocation.SelectedIndex = 0;
    }
    public void LoadCategories()
    {
        ddlCategory.Items.Clear();
        DataTable dt = (DataTable)this.Session["Idt"];
        if (dt.Rows.Count > 0)
        {
            DataTable category = dt.AsEnumerable().GroupBy(c => c.Field<int>("CATEGORY_ID")).Select(y => y.FirstOrDefault()).CopyToDataTable();
            ddlCategory.DataSource = category;
            ddlCategory.TextField = "CATEGORY_NAME";
            ddlCategory.ValueField = "CATEGORY_ID";
            ddlCategory.DataBind();
            ddlCategory.SelectedIndex = 0;
        }
        else
        {
            ddlCategory.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        }
        //var Qry = DropDowns.CATEGORYDropDown();
        //if (Qry.Any())
        //{
        //    ddlCategory.DataSource = Qry;
        //    ddlCategory.TextField = "CATEGORY_NAME";
        //    ddlCategory.ValueField = "CATEGORY_ID";
        //    ddlCategory.DataBind();
        //    ddlSubCategory.SelectedIndex = 0;
        //}
        //else
        //{
        //    ddlCategory.Items.Clear();
        //    ddlCategory.Items.Add(new ListItem("", "0"));
        //    ddlCategory.SelectedIndex = 0;
        //}
    }
    public void LoadSubCategories()
    {
        ddlSubCategory.Items.Clear();
        int ID = Convert.ToInt32(ddlCategory.SelectedItem.Value);
        DataTable dt = (DataTable)this.Session["Idt"];
        if (dt.Rows.Count > 0)
        {
            DataTable subcategory = dt.AsEnumerable().Where(x => x.Field<int>("CATEGORY_ID") == ID).GroupBy(c => c.Field<int>("SUB_CATEGORY_ID")).Select(y => y.FirstOrDefault()).CopyToDataTable();
            ddlSubCategory.DataSource = subcategory;
            ddlSubCategory.TextField = "SUB_CATEGORY_NAME";
            ddlSubCategory.ValueField = "SUB_CATEGORY_ID";
            ddlSubCategory.DataBind();
            ddlSubCategory.SelectedIndex = 0;
        }
        else
        {
            ddlSubCategory.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        }
        //var Qry = DropDowns.SUB_CATEGORYDropDown().Where(x => x.CATEGORY_ID == ID);
        //if (Qry.Any())
        //{
        //    ddlSubCategory.DataSource = Qry;
        //    ddlSubCategory.TextField = "SUB_CATEGORY_NAME";
        //    ddlSubCategory.ValueField = "SUB_CATEGORY_ID";
        //    ddlSubCategory.DataBind();
        //    ddlSubCategory.SelectedIndex = 0;
        //}
        //else
        //{
        //    ddlSubCategory.Items.Clear();
        //    ddlSubCategory.Items.Add(new ListItem("", "0"));
        //    ddlSubCategory.SelectedIndex = 0;
        //}
    }
    protected void LoadVendors()
    {
        ddlVendor.Items.Clear();
        DataTable dt = LController.SelectIssuedForms(Constants.LongNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 2);
        if (dt.Rows.Count > 0)
        {
            ddlVendor.DataSource = dt;
            ddlVendor.TextField = "VENDOR_NAME";
            ddlVendor.ValueField = "VENDOR_ID";
            ddlVendor.DataBind();
            ddlVendor.SelectedIndex = 0;
        }
        else
        {
            ddlVendor.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
        }
        //DbCalls db = new DbCalls();
        //var qry = (from k in db.VENDORS
        //           select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
        ////ddlVendor.DataSource = DropDowns.VendorDropDown();
        //ddlVendor.DataSource = qry;
        //ddlVendor.TextField = "VENDOR_NAME";
        //ddlVendor.ValueField = "VENDOR_ID";
        //ddlVendor.DataBind();
    }
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetail.Columns.Add("PurchaseAmount", typeof(decimal));
        PurchaseDetail.Columns.Add("Discount", typeof(decimal));
        PurchaseDetail.Columns.Add("CommissionType", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_PER", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_VALUE", typeof(decimal));
        PurchaseDetail.Columns.Add("Commission", typeof(decimal));
        PurchaseDetail.Columns.Add("Balance", typeof(decimal));
        PurchaseDetail.Columns.Add("TOKEN_AMOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("DOWN_PAYMENT", typeof(decimal));
        PurchaseDetail.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetail.Columns.Add("CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("SUB_CATEGORY_ID", typeof(int));
        this.Session.Add("PurchaseDetail", PurchaseDetail);
    }
    protected void LoadItems(string ItemID)
    {
        int VendorID = int.Parse(ddlVendor.SelectedItem.Value.ToString());
        int CATEGORYID = int.Parse(ddlCategory.SelectedItem.Value.ToString());
        int SUBCATEGORYID = int.Parse(ddlSubCategory.SelectedItem.Value.ToString());
        DataTable dt = (DataTable)this.Session["Idt"];
        DataRow[] results = dt.Select("CATEGORY_ID = " + CATEGORYID + " AND SUB_CATEGORY_ID = " + SUBCATEGORYID + " AND VENDOR_ID = " + VendorID);
        if (results.Count() > 0)
        {
            var Items = results.CopyToDataTable();
            ASPxGridView1.DataSource = Items;
            ASPxGridView1.DataBind();
            //HideGridColumns();
            ddlProject.DataSource = Items;
            ddlProject.TextField = "ITEM_INFO_NAME";
            ddlProject.ValueField = "ISSUANCE_MASTER_ID";
            ddlProject.DataBind();
            ddlProject.SelectedIndex = 0;

            PanelItems.Visible = true;
        }
        else
        {
            ASPxGridView1.DataSource = null;
            ASPxGridView1.DataBind();
            PanelItems.Visible = true;
        }
        
    }
    private void HideGridColumns()
    {
        //ASPxGridView1.FooterRow.Cells[0].Visible = false;
        ASPxGridView1.FooterRow.Cells[1].Visible = false;
        ASPxGridView1.FooterRow.Cells[2].Text = "Total";
        ASPxGridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[3].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[4].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[5].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
        //ASPxGridView1.FooterRow.Cells[6].Font.Bold = true;
        //ASPxGridView1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[6].Visible = false;
        ASPxGridView1.FooterRow.Cells[7].Visible = false;
        ASPxGridView1.FooterRow.Cells[8].Visible = false;
        ASPxGridView1.FooterRow.Cells[9].Visible = false;
        ASPxGridView1.FooterRow.Cells[10].Visible = false;
        ASPxGridView1.FooterRow.Cells[11].Visible = false;
        ASPxGridView1.FooterRow.Cells[12].Visible = false;
        //ASPxGridView1.FooterRow.Cells[13].Visible = false;
    }
    private void DoEmptyTextBox()
    {
        LoadProject();
        LoadDistributor();
        LoadCategories();
        LoadSubCategories();
        LoadVendors();
        txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtPurchaseDate.Attributes.Add("readonly", "readonly");
        txtTotalBalance.Text = "";
        txtTotalCommission.Text = "";
        txtTotalDiscountAmount.Text = "";
        txtTotalDownPayment.Text = "";
        txtTotalPurchaseAmount.Text = "";
        txtTotalTokenAmount.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
    }
    public int Val(string Val)
    {
        try
        {
            return Convert.ToInt32(Val);
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
    private void SetTotals()
    {
        decimal TotalPurchaseAmount = 0;
        decimal TotalDiscount = 0;
        decimal TotalCommission = 0;
        decimal TotalBalance = 0;
        decimal TotalTokenAmount = 0;
        decimal TotalDownPayment = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            CheckBox ChChecked = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ChChecked.Checked == true)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    TotalPurchaseAmount += Convert.ToDecimal(row.Cells[3].Text); //Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtPurchaseAmount")).Text);
                    TotalDiscount += Convert.ToDecimal(row.Cells[4].Text); //Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text);
                    TotalBalance += Convert.ToDecimal(row.Cells[5].Text); //Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtBalance")).Text);
                    TotalTokenAmount += Convert.ToDecimal(row.Cells[9].Text);
                    TotalDownPayment += Convert.ToDecimal(row.Cells[10].Text);
                }
            }
        }
        txtTotalPurchaseAmount.Text = Convert.ToString(TotalPurchaseAmount);
        txtTotalDiscountAmount.Text = Convert.ToString(TotalDiscount);
        txtTotalCommission.Text = Convert.ToString(TotalCommission);
        txtTotalBalance.Text = Convert.ToString(TotalBalance);
        txtTotalTokenAmount.Text = Convert.ToString(TotalTokenAmount);
        txtTotalDownPayment.Text = Convert.ToString(TotalDownPayment);

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
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_ISSUANCE_MASTER
                       join i in db.tbl_ISSUANCE_DETAIL on k.ISSUANCE_MASTER_ID equals i.ISSUANCE_MASTER_ID 
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
                       select new
                       {
                           d.DISTRIBUTOR_NAME,
                           k.DATE,
                           //ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME,
                           v.VENDOR_NAME,
                           PURCHASE_MASTER_ID = k.ISSUANCE_MASTER_ID,
                           TOTAL_PURCHASE_AMOUNT = i.ITEM_AMOUNT,
                           Active = k.IS_DELETED == false ? "Active" : "Inactive",
                           k.IS_DELETED,
                           k.TYPE_ID,
                           i.ISSUANCE_CLOSED 
                       }).Where(x => x.Active == "Active" && x.TYPE_ID == 26 && x.ISSUANCE_CLOSED == true).ToList();
            PurchaseMaster.DataSource = qry;
            PurchaseMaster.DataBind();
            int total = 0;
            foreach (GridViewRow row in PurchaseMaster.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                total += amount;
            }
            if (qry.Count() > 0)
            {
                PurchaseMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
                PurchaseMaster.FooterRow.Cells[3].Text = "Total";
                PurchaseMaster.FooterRow.Cells[3].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[4].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[7].Visible = false;
                PurchaseMaster.FooterRow.Cells[8].Visible = false;
            }
        }
        else
        {
            //DataTable dt = LController.SelectDataForSearching(1);
            ////        var filtered = dt.AsEnumerable()
            ////.Where(r => r.Field<String>("DISTRIBUTOR_NAME").Contains(txtSearch.Text)
            ////       || r.Field<String>("PROJECT_NAME").Contains(txtSearch.Text)
            ////       || r.Field<String>("ITEM_NAME").Contains(txtSearch.Text)
            ////       || r.Field<String>("VENDOR_NAME").Contains(txtSearch.Text)
            ////       );
            ////|| r.Field<Decimal>("PURCHASE_AMOUNT").Contains(txtSearch.Text)
            //dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR Convert(PURCHASE_MASTER_ID, System.String)  LIKE '%" + txtSearch.Text + "%'";
            ////OR PURCHASE_MASTER_ID = '" + txtSearch.Text + "'
            ////OR PURCHASE_AMOUNT LIKE '%" + txtSearch.Text + "%'
            //if (dt.Rows.Count > 0)
            //{
            //    PurchaseMaster.PageIndex = 0;
            //}
            //PurchaseMaster.DataSource = dt;
            //PurchaseMaster.DataBind();
            //int total = 0;
            //foreach (GridViewRow row in PurchaseMaster.Rows)
            //{
            //    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
            //    total += amount;
            //}
            //PurchaseMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
            //PurchaseMaster.FooterRow.Cells[3].Text = "Total";
            //PurchaseMaster.FooterRow.Cells[3].Font.Bold = true;
            //PurchaseMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            //PurchaseMaster.FooterRow.Cells[4].Font.Bold = true;
            //PurchaseMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            //PurchaseMaster.FooterRow.Cells[7].Visible = false;
            //PurchaseMaster.FooterRow.Cells[8].Visible = false;

        }
    }
    protected void LoadIssuedItems(int type)
    {
        //if (type == 3)
        //{
        //    Session["UpdateData"] = 1;
        //}
        //else
        //{
        //    Session["UpdateData"] = 0;
        //}
        DataTable dt = LController.SelectIssuedForms(Constants.LongNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, type);
        this.Session.Add("Idt", dt);
    }

    #endregion

    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Session["IssuanceMasterID"] = null;
        Session["UpdateData"] = 0;
        PanelPerchase.Visible = true;
        panelgrid.Visible = false;
        LoadIssuedItems(1);
        LoadItems("");
        DoEmptyTextBox();

        //CreatTable();
    }


    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
        LoadItems("");
    }

    protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
    }

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        bool itemcheck = false;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            CheckBox ChChecked = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ChChecked.Checked == true)
            {
                int ReceivedItem = int.Parse(((System.Web.UI.WebControls.DropDownList)row.Cells[6].FindControl("ddlReceivedItem")).SelectedValue);
                if (ReceivedItem == 0)
                {
                    itemcheck = true;
                    break;
                }
                else
                {
                    itemcheck = false;
                }
            }
        }
        if (itemcheck)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please select received item for select row.');", true);
            return;
        }
        if (ddlLocation.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Location is required.');", true);
            ddlLocation.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlLocation);
            return;
        }
        if (ddlCategory.SelectedItem.Value.Equals("") || ddlCategory.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
            ddlCategory.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (ddlSubCategory.SelectedItem.Value.Equals("") || ddlSubCategory.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
            ddlSubCategory.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (ddlVendor.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ddlVendor.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        SetTotals();
        IssuanceMasterLogics pml = new IssuanceMasterLogics();
        tbl_ISSUANCE_MASTER pm = new tbl_ISSUANCE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
        pm.PROJECT_ID = 0; //Convert.ToInt32(ddlProject.SelectedValue);
        pm.CATEGORY_ID = 0; //Convert.ToInt32(ddlCategory.SelectedValue);
        pm.SUBCATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedValue);
        pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
        pm.COMMISSIONAGENT_ID = 0; //Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtTotalPurchaseAmount.Text);
        pm.TOTAL_DISCOUNT = Convert.ToDecimal(txtTotalDiscountAmount.Text);
        pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(txtTotalTokenAmount.Text);
        pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(txtTotalDownPayment.Text);
        pm.TOTAL_BALANCE = Convert.ToDecimal(txtTotalBalance.Text);
        pm.TOTAL_COMMISSION = Convert.ToDecimal(txtTotalCommission.Text);
        pm.REMARKS = txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        if (Session["IssuanceMasterID"] != null)
        {

            pm.TYPE_ID = 26;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pm.ISSUANCE_MASTER_ID = Convert.ToInt32(Session["IssuanceMasterID"]);
            pml.Update(pm);
            DbCalls db = new DbCalls();
            int ISSUANCE_MASTER_ID = pm.ISSUANCE_MASTER_ID;
            IssuanceDetailLogics pdl = new IssuanceDetailLogics();
            tbl_ISSUANCE_DETAIL pd = new tbl_ISSUANCE_DETAIL();
            DataTable dtt = LController.SelectBSI(ISSUANCE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 32);
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                foreach (DataRow dr in dtt.Rows)
                {
                    int IssuedRefID = int.Parse(dr["REC_REF_ID"].ToString());
                    int IssuancDetailID = int.Parse(dr["ISSUANCE_DETAIL_ID"].ToString());
                    using (SqlCommand command = new SqlCommand("UPDATE tbl_ISSUANCE_DETAIL SET REC_ITEM_LOG_ID = " + "" + ",REC_REF_ID = " + "" + ",ISSUANCE_CLOSED = 0 where ISSUANCE_DETAIL_ID=" + IssuancDetailID, con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command1 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 1 where ITEM_LOG_ID=" + IssuedRefID, con))
                    {
                        command1.ExecuteNonQuery();
                    }

                }
                using (SqlCommand command2 = new SqlCommand("DELETE tbl_ITEM_LOG where TransactionID=" + ISSUANCE_MASTER_ID + "AND TypeID = 26", con))
                {
                    command2.ExecuteNonQuery();
                }
                con.Close();
            }
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                CheckBox ChChecked = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
                if (ChChecked.Checked == true)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int ITEM_INFO_ID = Convert.ToInt32(row.Cells[1].Text);
                        string PurchaseAmount = row.Cells[3].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtPurchaseAmount")).Text;
                        string Discount = row.Cells[4].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                        //string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtBalance")).Text;
                        int ReceivedItem = int.Parse(((System.Web.UI.WebControls.DropDownList)row.Cells[6].FindControl("ddlReceivedItem")).SelectedValue);

                        decimal TOKEN_AMOUNT = Convert.ToDecimal(row.Cells[9].Text);
                        decimal DOWN_PAYMENT = Convert.ToDecimal(row.Cells[10].Text);

                        int Project_ID = Convert.ToInt32(row.Cells[11].Text);
                        int Category_ID = Convert.ToInt32(row.Cells[12].Text);
                        int SubCategory_ID = Convert.ToInt32(row.Cells[13].Text);
                        int ISSUANCE_DETAIL_ID = Convert.ToInt32(row.Cells[14].Text);
                        int ITEM_LOG_ID = Convert.ToInt32(row.Cells[15].Text);
                        int REC_ITEM_LOG_ID = Convert.ToInt32(row.Cells[16].Text);
                        

                        pd.ISSUANCE_MASTER_ID = ISSUANCE_MASTER_ID;
                        pd.ISSUANCE_DETAIL_ID = ISSUANCE_DETAIL_ID;
                        pd.ISSUED_ITEM_ID = ITEM_INFO_ID;
                        pd.RECEIVED_ITEM_ID = ReceivedItem;
                        pd.RECEIVED_DATE = Convert.ToDateTime(txtPurchaseDate.Text);
                        pd.PROJECT_ID = Project_ID;
                        pd.CATEGORY_ID = Category_ID;
                        pd.SUB_CATEGORY_ID = SubCategory_ID;
                        pd.ITEM_AMOUNT = Convert.ToDecimal(PurchaseAmount);
                        pd.DISCOUNT = Convert.ToDecimal(Discount);
                        pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                        pd.DOWN_PAYMENT = DOWN_PAYMENT;
                        pd.COMMISSION_PERCENT = 0; //COMMISSION_PER;
                        pd.COMMISSION_VALUE = 0; //COMMISSION_VALUE;
                        pd.COMMISSION = 0; //Commission;
                        pd.BALANCE = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        pd.DEBIT_AMOUNT = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        pd.ISSUANCE_CLOSED = Convert.ToBoolean(1);
                        pd.ITEM_LOG_ID = ITEM_LOG_ID;
                        pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        pd.TIME_STEMP = DateTime.Now;
                        pd.IS_DELETED = Convert.ToBoolean(0);
                        pdl.Update(pd);

                        Item_Log_Logics Log = new Item_Log_Logics();
                        tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                        ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                        ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                        ILog.TransactionID = ISSUANCE_MASTER_ID;
                        ILog.TransactionDetailID = pd.ISSUANCE_DETAIL_ID;
                        ILog.TypeID = 26;
                        ILog.ProjectID = Project_ID;
                        ILog.CategoryID = Category_ID;
                        ILog.SubCategoryID = SubCategory_ID;
                        ILog.ItemID = ReceivedItem;
                        ILog.ItemAmount = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        ILog.UserID = Convert.ToInt32(Session["UserID"]);
                        ILog.IsActive = Convert.ToBoolean(1);
                        ILog.TimeStamp = DateTime.Now;
                        Log.Save(ILog);

                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            var item = (from k in db.tbl_ITEM_LOG select k).Where(x => x.ItemID == ReceivedItem && x.TypeID == 25 && x.IsActive == true).ToList();
                            if (item.Any())
                            {
                                long Item_LogID = item.FirstOrDefault().ITEM_LOG_ID;
                                using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ISSUANCE_DETAIL SET REC_ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + ",REC_REF_ID =" + Item_LogID + "  where ISSUANCE_DETAIL_ID=" + pd.ISSUANCE_DETAIL_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ItemID=" + ReceivedItem + "AND TypeID = 25 AND IsActive = 1", con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ISSUANCE_DETAIL SET REC_ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where ISSUANCE_DETAIL_ID=" + pd.ISSUANCE_DETAIL_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ItemID=" + ReceivedItem + "AND TypeID = 25 AND IsActive = 1", con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                            }
                            con.Close();
                        }
                    }
                }
            }

            var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == pm.ISSUANCE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Issuance && x.IS_DELETED == 0).ToList();
            if (q.Count > 0)
            {
                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.ISSUANCE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Issuance + "and PAYMENT_MODE=" + Constants.CreditSale, con))
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
            string Info = txtRemarks.Text; //"Ref.# " + pm.ISSUANCE_MASTER_ID + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
               , pm.DATE, pm.VENDOR_ID, Constants.Document_Received
               , "", Convert.ToInt32(Session["UserID"]), pm.ISSUANCE_MASTER_ID
               , Convert.ToString(pm.ISSUANCE_MASTER_ID)
               , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            Session["DBT_Amount"] = null;
            Session["BAL_Amount"] = null;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["IssuanceMasterID"] = null;
            //mastergrid.Visible = true;
        }
        else
        {
            pm.TYPE_ID = 26;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pm.ISSUANCE_MASTER_ID = Convert.ToInt32(ddlProject.SelectedItem.Value);
            pml.Update(pm);
            int ISSUANCE_MASTER_ID = pm.ISSUANCE_MASTER_ID;
            IssuanceDetailLogics pdl = new IssuanceDetailLogics();
            tbl_ISSUANCE_DETAIL pd = new tbl_ISSUANCE_DETAIL();
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                CheckBox ChChecked = (CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
                if (ChChecked.Checked == true)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int ITEM_INFO_ID = Convert.ToInt32(row.Cells[1].Text);
                        string PurchaseAmount = row.Cells[3].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtPurchaseAmount")).Text;
                        string Discount = row.Cells[4].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                        //string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtBalance")).Text;
                        int ReceivedItem = int.Parse(((System.Web.UI.WebControls.DropDownList)row.Cells[6].FindControl("ddlReceivedItem")).SelectedValue);

                        decimal TOKEN_AMOUNT = Convert.ToDecimal(row.Cells[9].Text);
                        decimal DOWN_PAYMENT = Convert.ToDecimal(row.Cells[10].Text);

                        int Project_ID = Convert.ToInt32(row.Cells[11].Text);
                        int Category_ID = Convert.ToInt32(row.Cells[12].Text);
                        int SubCategory_ID = Convert.ToInt32(row.Cells[13].Text);
                        int ISSUANCE_DETAIL_ID = Convert.ToInt32(row.Cells[14].Text);
                        int ITEM_LOG_ID = Convert.ToInt32(row.Cells[15].Text);

                        pd.ISSUANCE_MASTER_ID = ISSUANCE_MASTER_ID;
                        pd.ISSUANCE_DETAIL_ID = ISSUANCE_DETAIL_ID;
                        pd.ISSUED_ITEM_ID = ITEM_INFO_ID;
                        pd.RECEIVED_ITEM_ID = ReceivedItem;
                        pd.RECEIVED_DATE = Convert.ToDateTime(txtPurchaseDate.Text);
                        pd.PROJECT_ID = Project_ID;
                        pd.CATEGORY_ID = Category_ID;
                        pd.SUB_CATEGORY_ID = SubCategory_ID;
                        pd.ITEM_AMOUNT = Convert.ToDecimal(PurchaseAmount);
                        pd.DISCOUNT = Convert.ToDecimal(Discount);
                        pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                        pd.DOWN_PAYMENT = DOWN_PAYMENT;
                        pd.COMMISSION_PERCENT = 0; //COMMISSION_PER;
                        pd.COMMISSION_VALUE = 0; //COMMISSION_VALUE;
                        pd.COMMISSION = 0; //Commission;
                        pd.BALANCE = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        pd.DEBIT_AMOUNT = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        pd.ISSUANCE_CLOSED = Convert.ToBoolean(1);
                        pd.ITEM_LOG_ID = ITEM_LOG_ID;
                        pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        pd.TIME_STEMP = DateTime.Now;
                        pd.IS_DELETED = Convert.ToBoolean(0);
                        pdl.Update(pd);

                        Item_Log_Logics Log = new Item_Log_Logics();
                        tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                        ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                        ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                        ILog.TransactionID = ISSUANCE_MASTER_ID;
                        ILog.TransactionDetailID = pd.ISSUANCE_DETAIL_ID;
                        ILog.TypeID = 26;
                        ILog.ProjectID = Project_ID;
                        ILog.CategoryID = Category_ID;
                        ILog.SubCategoryID = SubCategory_ID;
                        ILog.ItemID = ReceivedItem;
                        ILog.ItemAmount = Convert.ToDecimal(PurchaseAmount) - Convert.ToDecimal(Discount);
                        ILog.UserID = Convert.ToInt32(Session["UserID"]);
                        ILog.IsActive = Convert.ToBoolean(1);
                        ILog.TimeStamp = DateTime.Now;
                        Log.Save(ILog);

                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            var item = (from k in db.tbl_ITEM_LOG select k).Where(x => x.ItemID == ReceivedItem && x.TypeID == 25 && x.IsActive == true).ToList();
                            if (item.Any())
                            {
                                long Item_LogID = item.FirstOrDefault().ITEM_LOG_ID;
                                using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ISSUANCE_DETAIL SET REC_ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + ",REC_REF_ID =" + Item_LogID + "  where ISSUANCE_DETAIL_ID=" + pd.ISSUANCE_DETAIL_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ItemID=" + ReceivedItem + "AND TypeID = 25 AND IsActive = 1", con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ISSUANCE_DETAIL SET REC_ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where ISSUANCE_DETAIL_ID=" + pd.ISSUANCE_DETAIL_ID, con))
                                {
                                    command2.ExecuteNonQuery();
                                }
                                using (SqlCommand command1 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ItemID=" + ReceivedItem + "AND TypeID = 25 AND IsActive = 1", con))
                                {
                                    command1.ExecuteNonQuery();
                                }
                            }
                            con.Close();
                        }
                    }
                }

            }

            string Info = txtRemarks.Text; //"Ref.# " + pm.ISSUANCE_MASTER_ID + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
                , pm.DATE, pm.VENDOR_ID, Constants.Document_Received
                , "", Convert.ToInt32(Session["UserID"]), pm.ISSUANCE_MASTER_ID
                , Convert.ToString(pm.ISSUANCE_MASTER_ID)
                , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadGrid("");
        DoEmptyTextBox();
        LoadItems("");
        mastergrid.Visible = true;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
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

            //DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            //DataRow[] drConfig = null;

            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.Inventoryatstore + "'");
            //long PurchaseAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());


            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.AccountPayable + "'");
            //long PayableAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            //drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.PurchaseDiscount + "'");
            //long PurchaseDiscount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            if (p_DOCUMENT_TYPE_ID == Constants.Document_Received)
            {
                //LController.PostingPrinvipalInvoiceAccount(
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Received-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Received", 0, Constants.Document_Received);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Received-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Received", 0, Constants.Document_Received);

                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Received
                        , p_MANUAL_DOCUMENT_NO, "Received Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Received", mTransaction, mConnection);
                    decimal Purchases = p_Credit - p_Discount_Amount;
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, Purchases, 0, "Received Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, 0, Purchases, "Received Voucher", mTransaction, mConnection);
                }
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
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        DoEmptyTextBox();
        PanelPerchase.Visible = false;
        panelgrid.Visible = true;
        mastergrid.Visible = true;
        LoadGrid("");
        CreatTable();
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        Session["UpdateData"] = null;
    }

    protected void PurchaseMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        PurchaseMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {

    }

    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["IssuanceMasterID"] = null;
        Session["UpdateData"] = 1;
        var ProID = ((LinkButton)sender).CommandArgument;
        IssuanceMasterLogics Ser = new IssuanceMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        //var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.ISSUANCE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Issuance && x.IS_DELETED == 0).ToList();
        //if (qry.Any())
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update. Some payments are issued against this purchase.');", true);
        //}
        //else
        //{
        ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
        ddlLocation.TextField = "DISTRIBUTOR_NAME";
        ddlLocation.ValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
        ddlLocation.Value = Convert.ToString(data.DISTRIBUTOR_ID);
        LoadIssuedItems(3);
        LoadCategories();
        LoadSubCategories();
        LoadVendors();
        ddlVendor.Value = Convert.ToString(data.VENDOR_ID);

        txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
        //    CreatTable();
        //    DataTable dt = LController.SelectBSI(data.ISSUANCE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 32);
        //    foreach (DataRow gvr in dt.Rows)
        //    {
        //        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        //        DataRow dr = PurchaseDetail.NewRow();
        //        dr["ITEM_INFO_ID"] = gvr["ITEM_INFO_ID"];
        //        dr["ITEM_INFO_NAME"] = gvr["ITEM_INFO_NAME"];
        //        dr["PurchaseAmount"] = Math.Round(decimal.Parse(gvr["PurchaseAmount"].ToString()), 0);
        //        dr["Discount"] = Math.Round(decimal.Parse(gvr["Discount"].ToString()), 0);
        //    //decimal Commission_Per = decimal.Parse(gvr["COMMISSION_PER"].ToString());
        //    //if (Math.Round(Commission_Per, 0) != 0)
        //    //{
        //    //    dr["COMMISSION_VALUE"] = "0";
        //    //    decimal amount = Convert.ToDecimal(gvr["Balance"]) / 100 * decimal.Parse(gvr["COMMISSION_PER"].ToString());
        //    //    dr["Commission"] = Math.Round(decimal.Parse(gvr["COMMISSION_PER"].ToString()), 0);
        //    //    dr["COMMISSION_VALUE"] = Convert.ToString(Math.Round(amount, 2));
        //    //    dr["CommissionType"] = 1;
        //    //}
        //    //else
        //    //{
        //    //    dr["CommissionType"] = 2;
        //    //    dr["Commission"] = Math.Round(decimal.Parse(gvr["COMMISSION_VALUE"].ToString()), 0);
        //    //    dr["COMMISSION_VALUE"] = Convert.ToDecimal(gvr["COMMISSION_VALUE"]);
        //    //}

        //        dr["COMMISSION_PER"] = gvr["COMMISSION_PER"];
        //        dr["COMMISSION_VALUE"] = gvr["COMMISSION_VALUE"];
        //        dr["Commission"] = gvr["Commission"];
        //        dr["Balance"] = Math.Round(decimal.Parse(gvr["Balance"].ToString()), 0);
        //        dr["TOKEN_AMOUNT"] = gvr["TOKEN_AMOUNT"];
        //        dr["DOWN_PAYMENT"] = gvr["DOWN_PAYMENT"];
        //        dr["PROJECT_ID"] = gvr["PROJECT_ID"];
        //        dr["CATEGORY_ID"] = gvr["CATEGORY_ID"];
        //        dr["SUB_CATEGORY_ID"] = gvr["SUB_CATEGORY_ID"];
        //        PurchaseDetail.Rows.Add(dr);
        //    }
        //ASPxGridView1.DataSource = PurchaseDetail;
        //ASPxGridView1.DataBind();
        txtRemarks.Text = data.REMARKS;
        LoadItems("");
        DataTable dtt = LController.SelectIssuedForms(Constants.LongNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 4);
        foreach (GridViewRow dr in ASPxGridView1.Rows)
        {
            foreach (DataRow dbr in dtt.Rows)
            {
                if (Convert.ToInt32(ASPxGridView1.DataKeys[dr.RowIndex].Values["ITEM_INFO_ID"]) == Convert.ToInt32(dbr["ITEM_INFO_ID"]))
                {
                    CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                    chRelized.Checked = true;
                    ((System.Web.UI.WebControls.DropDownList)dr.Cells[6].FindControl("ddlReceivedItem")).SelectedValue = dbr["RECEIVED_ITEM_ID"].ToString();
                    //((System.Web.UI.WebControls.TextBox)dr.Cells[3].FindControl("txtPurchaseAmount")).Text = Math.Round(decimal.Parse(dbr["PurchaseAmount"].ToString()), 0).ToString();
                    //((System.Web.UI.WebControls.TextBox)dr.Cells[4].FindControl("txtDiscount")).Text = Math.Round(decimal.Parse(dbr["Discount"].ToString()), 0).ToString();
                    //((System.Web.UI.WebControls.TextBox)dr.Cells[5].FindControl("txtBalance")).Text = Math.Round(decimal.Parse(dbr["Balance"].ToString()), 0).ToString();
                }
            }
        }
        //HideGridColumns();
        int TotalPurchaseAmount = 0;
        int TotalDiscountAmount = 0;
        int TotalBalanceAmount = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            string PurchaseAmount = row.Cells[3].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtPurchaseAmount")).Text;
            string Discount = row.Cells[4].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
            string Balance = row.Cells[5].Text; //((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtBalance")).Text;
            TotalPurchaseAmount += Convert.ToInt32(Math.Round(decimal.Parse(PurchaseAmount), 0));
            TotalDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(Discount), 0));
            TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(Balance), 0));
        }
        //HtmlControl control = FindControl("lblPurchaseAmount") as HtmlControl;
        //control.Attributes["Class"] = TotalPurchaseAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[3].Text = TotalPurchaseAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[4].Text = TotalDiscountAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[5].Text = TotalBalanceAmount.ToString();
        this.Session.Add("PurchaseDetail", PurchaseDetail);
        SetTotals();
        //LoadItems("");
        Session.Add("IssuanceMasterID", data.ISSUANCE_MASTER_ID);
        mastergrid.Visible = false;
        PanelPerchase.Visible = true;
        panelgrid.Visible = false;
        PanelItems.Visible = true;
        btnSave.Text = "Update";
        //AddPopUp_Click(null, null);

        //}
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {

    }

    protected void ddlVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
    }

    protected void ASPxGridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            SqlConnection con = new SqlConnection(Configuration.ConnectionString);
            con.Open();
            DevExpress.Web.ASPxComboBox DDL = (e.Row.FindControl("ddlReceivedItem") as DevExpress.Web.ASPxComboBox);

            SqlCommand cmd = new SqlCommand((@"SELECT DISTINCT ITEM_INFO_ID,
            I.ITEM_NAME + ', ' + P.PROJECT_NAME + ', ' + ISNULL(B.BLOCK_NAME,'N/A') + ', ' + ISNULL(S.STREET_NAME,'N/A') + ', ' + ISNULL(M.MEASURMENT,'N/A') AS ITEM_INFO_NAME
            FROM ITEM_INFORMATION I
            INNER JOIN PROJECT P ON I.PROJECT_ID = p.PROJECT_ID
            LEFT OUTER JOIN BLOCK B ON I.BLOCK_ID = B.BLOCK_ID
            LEFT OUTER JOIN STREET S ON I.STREET_ID = S.STREET_ID
            LEFT OUTER JOIN MEASURMENT M ON I.MEASUREMENT = M.MEASURMENT_ID
            LEFT OUTER JOIN tbl_ITEM_LOG IL ON I.ITEM_INFO_ID = IL.ItemID AND IL.TypeID = 2 AND IL.IsActive = 0 
            WHERE DETAIL_CATEGORY_ID IN (2,3) 
            AND I.ITEM_INFO_ID NOT IN (SELECT ItemID FROM tbl_ITEM_LOG WHERE TypeID = 26 AND IsActive = 1)
            OR I.ITEM_INFO_ID IN (SELECT ItemID FROM tbl_ITEM_LOG WHERE TypeID = 25 AND IsActive = 1)"), con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            if (int.Parse(Session["UpdateData"].ToString()) == 1)
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                DataTable dtt = LController.SelectIssuedForms(Constants.LongNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 4);
                foreach (DataRow gvr in dtt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["ITEM_INFO_ID"] = gvr["RECEIVED_ITEM_ID"];
                    dr["ITEM_INFO_NAME"] = gvr["ITEM_INFO_NAME2"];
                    dt.Rows.Add(dr);
                }
                
                
                DDL.DataSource = dt;
                DDL.TextField = "ITEM_INFO_NAME";
                DDL.ValueField = "ITEM_INFO_ID";
                DDL.DataBind();
                DDL.Items.Insert(0, new DevExpress.Web.ListEditItem("--Select Received Form--", "0"));
            }
            else
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                DDL.DataSource = dt;
                DDL.TextField = "ITEM_INFO_NAME";
                DDL.ValueField = "ITEM_INFO_ID";
                DDL.DataBind();
                DDL.Items.Insert(0, new DevExpress.Web.ListEditItem("--Select Received Form--", "0"));
            }
            
        }
    }
}