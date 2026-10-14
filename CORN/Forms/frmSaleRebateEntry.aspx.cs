using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNBusinessLayer.Reports;
using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;

public partial class Forms_frmSaleRebateEntry : System.Web.UI.Page
{
    DataTable PurchaseDetail, SearchTable;
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    IDbTransaction mTransaction;
    IDbConnection mConnection;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CreatTable();
            CreateSearchTable();
            LoadProject();
            LoadDistributor();
            LoadCategories();
            LoadSubCategories();
            LoadVendors();
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            ddlLocation.Enabled = true;
            ddlProject.Enabled = true;
            ddlCategory.Enabled = true;
            ddlSubCategory.Enabled = true;
            PanelItems.Visible = false;
            //HideGridColumns();
            PanelPerchase.Visible = false;
            panelgrid.Visible = true;
            LoadGrid("");
            Session["RebateMasterID"] = null;
        }


        //GridHeaderFilterMode headerFilterMode = true ? GridHeaderFilterMode.CheckedList : GridHeaderFilterMode.List;
        //foreach (GridViewDataColumn column in grid.Columns)
        //    column.SettingsHeaderFilter.Mode = headerFilterMode;

        //GridHeaderFilterMode headerFilterMode = GridHeaderFilterMode.CheckedList;
        //foreach (GridViewDataColumn column in grid.Columns)
        //    column.SettingsHeaderFilter.Mode = headerFilterMode;
        //LoadItems("");
    }

    #region Load Dropdowns
    protected void LoadProject()
    {
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.TextField = "PROJECT_NAME";
        ddlProject.ValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedIndex = 0;
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
        var Qry = DropDowns.CATEGORYDropDown();
        if (Qry.Any())
        {
            ddlCategory.DataSource = Qry;
            ddlCategory.TextField = "CATEGORY_NAME";
            ddlCategory.ValueField = "CATEGORY_ID";
            ddlCategory.DataBind();
            ddlCategory.SelectedIndex = 0;
        }
        else
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            ddlCategory.SelectedIndex = 0;
        }
    }
    public void LoadSubCategories()
    {
        int ID = Convert.ToInt32(ddlCategory.SelectedItem.Value);
        var Qry = DropDowns.SUB_CATEGORYDropDown().Where(x => x.CATEGORY_ID == ID);
        if (Qry.Any())
        {
            ddlSubCategory.DataSource = Qry;
            ddlSubCategory.TextField = "SUB_CATEGORY_NAME";
            ddlSubCategory.ValueField = "SUB_CATEGORY_ID";
            ddlSubCategory.DataBind();
            ddlSubCategory.SelectedIndex = 0;
        }
        else
        {
            ddlSubCategory.Items.Clear();
            ddlSubCategory.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            ddlSubCategory.SelectedIndex = 0;
        }
    }
    protected void LoadVendors()
    {
        //DbCalls db = new DbCalls();
        //var qry = (from k in db.VENDORS
        //           select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
        //ddlVendor.DataSource = qry;
        ddlVendor.DataSource = DropDowns.CustomerDropDown();
        ddlVendor.TextField = "VENDOR_NAME";
        ddlVendor.ValueField = "VENDOR_ID";
        ddlVendor.DataBind();
        ddlVendor.SelectedIndex = 0;
    }
    protected void LoadItems(string ItemID)
    {
        DbCalls db = new DbCalls();
        int PROJECTID = int.Parse(ddlProject.SelectedItem.Value.ToString());
        int CATEGORYID = int.Parse(ddlCategory.SelectedItem.Value.ToString());
        int SUBCATEGORYID = int.Parse(ddlSubCategory.SelectedItem.Value.ToString());
        var query2 = (
      from i in db.tbl_SALE_DETAIL
      join il in db.ITEM_INFORMATION on i.ITEM_INFO_ID equals il.ITEM_INFO_ID
      join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID
      join c in db.CATEGORies on i.CATEGORY_ID equals c.CATEGORY_ID
      join subc in db.SUB_CATEGORY on i.SUB_CATEGORY_ID equals subc.SUB_CATEGORY_ID
      join m in db.MEASURMENTs on il.MEASUREMENT equals m.MEASURMENT_ID

      from b in db.BLOCKs
           .Where(x => x.BLOCK_ID == il.BLOCK_ID)
           .DefaultIfEmpty() // <== makes join left join

      from s in db.STREETs
      .Where(x => x.STREET_ID == il.STREET_ID)
      .DefaultIfEmpty() // <== makes join left join

      from pr in db.PRECENTs
      .Where(x => x.PRECENT_ID == il.PRECENT_ID)
      .DefaultIfEmpty() // <== makes join left join

      from prs in db.PRECENTSTREETs
      .Where(x => x.PRECENTSTREET_ID == il.PRECENTSTREET_ID)
      .DefaultIfEmpty() // <== makes join left join

      from sc in db.SECTORs
      .Where(x => x.SECTOR_ID == il.SECTOR_ID)
      .DefaultIfEmpty() // <== makes join left join

          //from Il in db.tbl_ITEM_LOG
          //.Where(x => x.ItemID == i.ITEM_INFO_ID)
          //.DefaultIfEmpty() // <== makes join left join
          //from groups in db.STREET
          //       .Where(gruppe => gruppe.STREET_ID == mappings.USRGRP_GRP)
          //       .DefaultIfEmpty() // <== makes join left join

          // where users.USR_Name.Contains(keyword)
          // || mappings.USRGRP_USR.Equals(666)  
          // || mappings.USRGRP_USR == 666 
          // || groups.Name.Contains(keyword)

      select new
      {
          ITEM_INFO_ID = i.ITEM_INFO_ID,
          p.PROJECT_ID,
          p.PROJECT_NAME,
          ITEM_NAME = il.ITEM_NAME,
          il.OPEN_FROM_REGISTRATION,
          il.TOWER_NO,
          il.FLOOR_NO,
          MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
          BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
          STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
          PRECENT_NAME = pr.PRECENT_NAME == null ? "N/A" : pr.PRECENT_NAME,
          PRECENTSTREET_NAME = prs.PRECENTSTREET_NAME == null ? "N/A" : prs.PRECENTSTREET_NAME,
          SECTOR_NAME = sc.SECTOR_NAME == null ? "N/A" : sc.SECTOR_NAME,
          c.CATEGORY_ID,
          subc.SUB_CATEGORY_ID,
          i.SALE_CLOSED

          //Il.TypeID,// = Il.TypeID == null ? 0 : Il.TypeID,
          //Il.IsActive //= Il.IsActive == null ? true : Il.IsActive
      }
  );
        var xy = (query2).Where(x => x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID).ToList(); // && x.SALE_CLOSED == false
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, CATEGORYID, SUBCATEGORYID, null, 46);
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                xy.RemoveAll(x => x.ITEM_INFO_ID == int.Parse(dr["ITEM_INFO_ID"].ToString()));
            }
        }
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                xy.RemoveAll(x => x.ITEM_INFO_ID == int.Parse(row.Cells[0].Text));
            }
        }
        CreateSearchTable();
        foreach (var row in xy)
        {
            DataRow dr = SearchTable.NewRow();
            dr["ITEM_INFO_ID"] = row.ITEM_INFO_ID;
            dr["PROJECT_ID"] = row.PROJECT_ID;
            dr["PROJECT_NAME"] = row.PROJECT_NAME;
            dr["ITEM_NAME"] = row.ITEM_NAME;
            dr["OPEN_FROM_REGISTRATION"] = row.OPEN_FROM_REGISTRATION;
            dr["TOWER_NO"] = row.TOWER_NO;
            dr["FLOOR_NO"] = row.FLOOR_NO;
            dr["MEASURMENT"] = row.MEASURMENT;
            dr["BLOCK_NAME"] = row.BLOCK_NAME;
            dr["STREET_NAME"] = row.STREET_NAME;
            dr["PRECENT_NAME"] = row.PRECENT_NAME;
            dr["PRECENTSTREET_NAME"] = row.PRECENTSTREET_NAME;
            dr["SECTOR_NAME"] = row.SECTOR_NAME;
            dr["CATEGORY_ID"] = row.CATEGORY_ID;
            dr["SUB_CATEGORY_ID"] = row.SUB_CATEGORY_ID;
            SearchTable.Rows.Add(dr);
        }

        if (!ItemID.Equals(""))
        {
            SearchTable.DefaultView.RowFilter = "PROJECT_NAME LIKE '%" + txtSearch.Text + "%'  OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR MEASURMENT LIKE '%" + txtSearch.Text + "%' OR BLOCK_NAME LIKE '%" + txtSearch.Text + "%' OR STREET_NAME LIKE '%" + txtSearch.Text + "%' OR PRECENT_NAME LIKE '%" + txtSearch.Text + "%' OR PRECENTSTREET_NAME LIKE '%" + txtSearch.Text + "%' OR OPEN_FROM_REGISTRATION LIKE '%" + txtSearch.Text + "%' OR TOWER_NO LIKE '%" + txtSearch.Text + "%' OR FLOOR_NO LIKE '%" + txtSearch.Text + "%'";
            if (SearchTable.Rows.Count > 0)
            {
                grid.PageIndex = 0;
            }
            grid.DataSource = SearchTable;
            grid.DataBind();
        }
        else
        {
            grid.DataSource = SearchTable;
            grid.DataBind();
        }
    }
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetail.Columns.Add("SaleAmount", typeof(decimal));
        PurchaseDetail.Columns.Add("Rebate1", typeof(string));
        PurchaseDetail.Columns.Add("CommissionType", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_PER", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_VALUE", typeof(decimal));
        PurchaseDetail.Columns.Add("Commission", typeof(string));
        PurchaseDetail.Columns.Add("Balance", typeof(string));
        PurchaseDetail.Columns.Add("Rebate2", typeof(string));
        PurchaseDetail.Columns.Add("Rebate3", typeof(string));
        PurchaseDetail.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetail.Columns.Add("CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("SUB_CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("IS_INVENTORY", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_LOG_ID", typeof(int));
        PurchaseDetail.Columns.Add("ItemAmount", typeof(decimal));
        this.Session.Add("PurchaseDetail", PurchaseDetail);
    }
    public void CreateSearchTable()
    {
        Session["SearchTable"] = null;
        SearchTable = new DataTable();
        SearchTable.Columns.Add("ITEM_INFO_ID", typeof(string));
        SearchTable.Columns.Add("PROJECT_ID", typeof(int));
        SearchTable.Columns.Add("PROJECT_NAME", typeof(string));
        SearchTable.Columns.Add("ITEM_NAME", typeof(string));
        SearchTable.Columns.Add("OPEN_FROM_REGISTRATION", typeof(string));
        SearchTable.Columns.Add("TOWER_NO", typeof(string));
        SearchTable.Columns.Add("FLOOR_NO", typeof(string));
        SearchTable.Columns.Add("MEASURMENT", typeof(string));
        SearchTable.Columns.Add("BLOCK_NAME", typeof(string));
        SearchTable.Columns.Add("STREET_NAME", typeof(string));
        SearchTable.Columns.Add("PRECENT_NAME", typeof(string));
        SearchTable.Columns.Add("PRECENTSTREET_NAME", typeof(string));
        SearchTable.Columns.Add("SECTOR_NAME", typeof(string));
        SearchTable.Columns.Add("CATEGORY_ID", typeof(int));
        SearchTable.Columns.Add("SUB_CATEGORY_ID", typeof(int));
        this.Session.Add("SearchTable", SearchTable);
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
        ddlLocation.Enabled = true;
        ddlProject.Enabled = true;
        ddlCategory.Enabled = true;
        ddlSubCategory.Enabled = true;
        PanelItems.Visible = false;
        //HideGridColumns();
        txtTotalBalance.Text = "";
        txtTotalCommission.Text = "";
        txtTotalRebate1Amount.Text = "";
        txtTotalRebate2Amount.Text = "";
        txtTotalSaleAmount.Text = "";
        txtTotalRebate3Amount.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        Session["RebateDISTRIBUTOR_ID"] = null;


    }
    private bool CheckDublicate(int Item_ID)
    {
        //if (Convert.ToInt32(RowId.Value) >= 0)
        //{
        //    return true;
        //}
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        DataRow[] foundRows = PurchaseDetail.Select("ITEM_INFO_ID  = '" + Item_ID + "'");
        if (foundRows.Length == 0)
        {
            return true;
        }
        return false;
    }
    private void HideGridColumns()
    {
        decimal TotalSaleAmount = 0;
        //decimal TotalRebate1Amount = 0;
        //decimal TotalRebate2Amount = 0;
        //decimal TotalRebate3Amount = 0;
        //decimal TotalBalanceAmount = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                decimal Purchase = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text)), 0);
                TotalSaleAmount += Purchase;
                //decimal Rebate1 = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtRebate1")).Text)), 0);
                //TotalRebate1Amount += Rebate1;
                //decimal Rebate2 = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtRebate2")).Text)), 0);
                //TotalRebate2Amount += Rebate2;
                //decimal Rebate3 = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtRebate3")).Text)), 0);
                //TotalRebate3Amount += Rebate3;
                //decimal Balance = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[7].FindControl("txtBalance")).Text)), 0);
                //TotalBalanceAmount += Balance;
            }
        }
        ASPxGridView1.FooterRow.Cells[0].Visible = false;
        ASPxGridView1.FooterRow.Cells[2].Text = "Total";
        ASPxGridView1.FooterRow.Cells[2].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[3].Text = String.Format("{0:N0}", TotalSaleAmount);
        ASPxGridView1.FooterRow.Cells[3].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
        //ASPxGridView1.FooterRow.Cells[4].Text = String.Format("{0:N0}", TotalRebate1Amount);
        ASPxGridView1.FooterRow.Cells[4].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
        //ASPxGridView1.FooterRow.Cells[5].Text = String.Format("{0:N0}", TotalRebate2Amount);
        ASPxGridView1.FooterRow.Cells[5].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
        //ASPxGridView1.FooterRow.Cells[6].Text = String.Format("{0:N0}", TotalRebate3Amount);
        ASPxGridView1.FooterRow.Cells[6].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
        //ASPxGridView1.FooterRow.Cells[7].Text = String.Format("{0:N0}", TotalBalanceAmount);
        ASPxGridView1.FooterRow.Cells[7].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[8].Visible = false;
        ASPxGridView1.FooterRow.Cells[9].Visible = false;
        ASPxGridView1.FooterRow.Cells[10].Visible = false;
        ASPxGridView1.FooterRow.Cells[11].Visible = false;
        ASPxGridView1.FooterRow.Cells[12].Visible = false;
        ASPxGridView1.FooterRow.Cells[13].Visible = false;
        ASPxGridView1.FooterRow.Cells[14].Visible = false;
        ASPxGridView1.FooterRow.Cells[15].Visible = false;
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
        decimal TotalSaleAmount = 0;
        decimal TotalRebate1 = 0;
        decimal TotalCommission = 0;
        decimal TotalBalance = 0;
        decimal TotalRebate2 = 0;
        decimal TotalRebate3 = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                //decimal COMMISSION_PER;
                //decimal COMMISSION_VALUE;
                //decimal Commission;
                decimal Sale = Math.Round(Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text)), 0);
                TotalSaleAmount += Sale;
                TotalRebate1 += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtRebate1")).Text));
                TotalRebate2 += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtRebate2")).Text));
                TotalRebate3 += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtRebate3")).Text));
                //int CommissionType = Val(((DropDownList)row.FindControl("ddlCommissionType")).SelectedValue);
                //if (CommissionType == 1)
                //{
                //    COMMISSION_PER = decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                //    decimal TCommission = (decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text)) / 100) * decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                //    COMMISSION_VALUE = 0;
                //    Commission = TCommission;
                //}
                //else
                //{
                //    COMMISSION_PER = 0;
                //    decimal TCommission = decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                //    COMMISSION_VALUE = TCommission;
                //    Commission = TCommission;
                //}
                //TotalCommission += Convert.ToDecimal(Commission);
                TotalBalance += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text));
                //TotalRebate2 += Convert.ToDecimal(row.Cells[9].Text);
                //TotalRebate3 += Convert.ToDecimal(row.Cells[10].Text);
            }
        }
        txtTotalSaleAmount.Text = Convert.ToString(TotalSaleAmount);
        txtTotalRebate1Amount.Text = Convert.ToString(TotalRebate1);
        txtTotalCommission.Text = Convert.ToString(TotalCommission);
        txtTotalBalance.Text = Convert.ToString(TotalBalance);
        txtTotalRebate2Amount.Text = Convert.ToString(TotalRebate2);
        txtTotalRebate3Amount.Text = Convert.ToString(TotalRebate3);

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
            var qry = (from k in db.tbl_SALE_REBATE_MASTER
                           //join pd in db.tbl_PURCHASE_DETAIL on k.PURCHASE_MASTER_ID equals pd.PURCHASE_MASTER_ID
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       //join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
                       //join i in db.ITEM_INFORMATION on pd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       //join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
                       //join s in db.STREET on i.STREET_ID equals s.STREET_ID
                       join v in db.VENDORS on k.CUSTOMER_ID equals v.VENDOR_ID
                       select new
                       {
                           d.DISTRIBUTOR_NAME,
                           k.DATE,
                           //ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME,
                           v.VENDOR_NAME,
                           k.REBATE_MASTER_ID,
                           k.TOTAL_SALE_AMOUNT,
                           k.TOTAL_REBATE1,
                           k.TOTAL_REBATE2,
                           k.TOTAL_REBATE3,
                           BALANCE = k.TOTAL_SALE_AMOUNT - k.TOTAL_REBATE1 - k.TOTAL_REBATE2 - k.TOTAL_REBATE3,
                           Active = k.IS_DELETED == false ? "Active" : "Inactive",
                           k.IS_DELETED,
                           k.TYPE_ID
                       }).Where(x => x.Active == "Active" && x.TYPE_ID == 13).ToList();
            SaleMaster.DataSource = qry;
            SaleMaster.DataBind();
            int total = 0;
            int rebate1 = 0;
            int rebate2 = 0;
            int rebate3 = 0;
            int balance = 0;
            foreach (GridViewRow row in SaleMaster.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                total += amount;
                int Rebate1 = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                rebate1 += Rebate1;
                int Rebate2 = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[7].Text), 0));
                rebate2 += Rebate2;
                int Rebate3 = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[8].Text), 0));
                rebate3 += Rebate3;
                int Balance = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[9].Text), 0));
                balance += Balance;
            }
            if (qry.Count() > 0)
            {
                SaleMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", total);
                SaleMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", rebate1);
                SaleMaster.FooterRow.Cells[7].Text = string.Format("{0:n2}", rebate2);
                SaleMaster.FooterRow.Cells[8].Text = string.Format("{0:n2}", rebate3);
                SaleMaster.FooterRow.Cells[9].Text = string.Format("{0:n2}", balance);
                SaleMaster.FooterRow.Cells[3].Text = "Total";
                SaleMaster.FooterRow.Cells[3].Font.Bold = true;
                SaleMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[4].Font.Bold = true;
                SaleMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[5].Font.Bold = true;
                SaleMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[6].Font.Bold = true;
                SaleMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[7].Font.Bold = true;
                SaleMaster.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[8].Font.Bold = true;
                SaleMaster.FooterRow.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[9].Font.Bold = true;
                SaleMaster.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;
                SaleMaster.FooterRow.Cells[4].Visible = false;
                SaleMaster.FooterRow.Cells[10].Visible = false;
                SaleMaster.FooterRow.Cells[11].Visible = false;
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
            ////|| r.Field<Decimal>("SALE_AMOUNT").Contains(txtSearch.Text)
            //dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%' OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR Convert(PURCHASE_MASTER_ID, System.String)  LIKE '%" + txtSearch.Text + "%'";
            ////OR PURCHASE_MASTER_ID = '" + txtSearch.Text + "'
            ////OR SALE_AMOUNT LIKE '%" + txtSearch.Text + "%'
            //if (dt.Rows.Count > 0)
            //{
            //    SaleMaster.PageIndex = 0;
            //}
            //SaleMaster.DataSource = dt;
            //SaleMaster.DataBind();
            //int total = 0;
            //foreach (GridViewRow row in SaleMaster.Rows)
            //{
            //    int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
            //    total += amount;
            //}
            //SaleMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
            //SaleMaster.FooterRow.Cells[3].Text = "Total";
            //SaleMaster.FooterRow.Cells[3].Font.Bold = true;
            //SaleMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            //SaleMaster.FooterRow.Cells[4].Font.Bold = true;
            //SaleMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            //SaleMaster.FooterRow.Cells[7].Visible = false;
            //SaleMaster.FooterRow.Cells[8].Visible = false;

        }
    }
    #endregion

    #region Grid Opration
    protected void GrdItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }
    protected void grid_HeaderFilterFillItems(object sender, DevExpress.Web.ASPxGridViewHeaderFilterEventArgs e)
    {
        if (e.Column.FieldName == "Total")
            PrepareTotalFilterItems(e);
        if (e.Column.FieldName == "Quantity")
            PrepareQuantityFilterItems(e);
    }

    #endregion

    #region Click Oprations

    protected void btnSave_Click(object sender, EventArgs e)
    {
        decimal TotalRebate = 0;
        decimal TotalStockInHandRebate = 0;
        decimal TotalconsumptionRebate = 0;
        if (ddlLocation.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Location is required.');", true);
            ddlLocation.Focus();
            return;
        }
        if (ddlCategory.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
            ddlCategory.Focus();
            return;
        }
        if (ddlSubCategory.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
            ddlSubCategory.Focus();
            return;
        }
        if (ddlVendor.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ddlVendor.Focus();
            return;
        }
        if (ASPxGridView1.Rows.Count < 0)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Must select an item to purchase.');", true);
            return;
        }
        SetTotals();
        SaleRebateMasterLogics pml = new SaleRebateMasterLogics();
        tbl_SALE_REBATE_MASTER pm = new tbl_SALE_REBATE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
        pm.PROJECT_ID = 0; //Convert.ToInt32(ddlProject.SelectedValue);
        pm.CATEGORY_ID = 0; //Convert.ToInt32(ddlCategory.SelectedValue);
        pm.SUBCATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedValue);
        pm.CUSTOMER_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
        pm.COMMISSIONAGENT_ID = 0; //Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        pm.TOTAL_SALE_AMOUNT = Convert.ToDecimal(txtTotalSaleAmount.Text);
        pm.TOTAL_REBATE1 = Convert.ToDecimal(txtTotalRebate1Amount.Text);
        pm.TOTAL_REBATE2 = Convert.ToDecimal(txtTotalRebate2Amount.Text);
        pm.TOTAL_REBATE3 = Convert.ToDecimal(txtTotalRebate3Amount.Text);
        TotalRebate = Convert.ToDecimal(txtTotalRebate1Amount.Text) + Convert.ToDecimal(txtTotalRebate2Amount.Text) + Convert.ToDecimal(txtTotalRebate3Amount.Text);
        pm.TOTAL_BALANCE = Convert.ToDecimal(txtTotalSaleAmount.Text) - TotalRebate;
        pm.TOTAL_COMMISSION = Convert.ToDecimal(txtTotalCommission.Text);
        pm.REMARKS = txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        if (Session["RebateMasterID"] != null)
        {

            pm.TYPE_ID = 13;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pm.REBATE_MASTER_ID = Convert.ToInt32(Session["RebateMasterID"]);
            pml.Update(pm);
            DbCalls db = new DbCalls();
            int REBATE_MASTER_ID = pm.REBATE_MASTER_ID;
            SaleRebateDetailLogics pdl = new SaleRebateDetailLogics();
            tbl_SALE_REBATE_DETAIL pd = new tbl_SALE_REBATE_DETAIL();
            DataTable dt = LController.SelectBSI(pm.REBATE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 47);
            foreach (DataRow gvr in dt.Rows)
            {

                int IS_INVENTORY = int.Parse(gvr["IS_INVENTORY"].ToString());
                int ITEM_LOG_ID = int.Parse(gvr["ITEM_LOG_ID"].ToString());
                decimal ItemAmount = decimal.Parse(gvr["ItemAmount"].ToString());
                if (IS_INVENTORY != 1)
                {
                    decimal Amount = ItemAmount + decimal.Parse(dc.chkNull_0(gvr["Rebate1"].ToString())) + decimal.Parse(dc.chkNull_0(gvr["Rebate2"].ToString())) + decimal.Parse(dc.chkNull_0(gvr["Rebate3"].ToString()));
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET ItemAmount = " + Amount + " WHERE ITEM_LOG_ID=" + ITEM_LOG_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
            DataTable dt2 = LController.SelectBSI(pm.REBATE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 47);
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("DELETE tbl_REBATE_DETAIL where REBATE_MASTER_ID=" + REBATE_MASTER_ID, con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int ITEM_INFO_ID = Convert.ToInt32(row.Cells[0].Text);
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Rebate1 = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtRebate1")).Text;
                    string Rebate2 = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtRebate2")).Text;
                    string Rebate3 = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtRebate3")).Text;
                    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[7].FindControl("txtBalance")).Text;

                    int Project_ID = Convert.ToInt32(row.Cells[10].Text);
                    int Category_ID = Convert.ToInt32(row.Cells[11].Text);
                    int SubCategory_ID = Convert.ToInt32(row.Cells[12].Text);
                    int IS_INVENTORY = Convert.ToInt32(row.Cells[13].Text);
                    int ITEM_LOG_ID = Convert.ToInt32(row.Cells[14].Text);
                    DataRow[] foundRows = dt2.Select("ITEM_INFO_ID  = '" + int.Parse(ITEM_INFO_ID.ToString()) + "'");
                    decimal ItemAmount = decimal.Parse(foundRows[0]["ItemAmount"].ToString());

                    pd.REBATE_MASTER_ID = REBATE_MASTER_ID;
                    pd.ITEM_INFO_ID = ITEM_INFO_ID;
                    pd.PROJECT_ID = Project_ID;
                    pd.CATEGORY_ID = Category_ID;
                    pd.SUB_CATEGORY_ID = SubCategory_ID;
                    pd.SALE_AMOUNT = Convert.ToDecimal(SaleAmount);
                    pd.REBATE1 = Convert.ToDecimal(Rebate1);
                    pd.REBATE2 = Convert.ToDecimal(Rebate2);
                    pd.REBATE3 = Convert.ToDecimal(Rebate3);
                    pd.COMMISSION_PERCENT = 0;
                    pd.COMMISSION_VALUE = 0;
                    pd.COMMISSION = 0;
                    pd.BALANCE = Convert.ToDecimal(SaleAmount) - Convert.ToDecimal(Rebate1) - Convert.ToDecimal(Rebate2) - Convert.ToDecimal(Rebate3);
                    if (IS_INVENTORY != 1)
                    {
                        TotalStockInHandRebate += Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);
                        decimal Amount = ItemAmount - Convert.ToDecimal(Rebate1) - Convert.ToDecimal(Rebate2) - Convert.ToDecimal(Rebate3);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET ItemAmount = " + Amount + " WHERE ITEM_LOG_ID=" + ITEM_LOG_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                    else
                    {
                        TotalconsumptionRebate += Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);
                    }
                    pd.DEBIT_AMOUNT = Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);

                    pd.REBATE_CLOSED = Convert.ToBoolean(0);
                    pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                    pd.TIME_STEMP = DateTime.Now;
                    pd.IS_DELETED = Convert.ToBoolean(0);
                    pdl.Save(pd);
                }
            }
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                string VOUCHER = Session["RebateVoucheNo"].ToString();
                int Distributor_ID = Convert.ToInt32(Session["RebateDISTRIBUTOR_ID"]);
                con.Open();
                using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER SET IS_DELETED = 1 WHERE VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "AND VOUCHER_NO='" + VOUCHER + "' AND DISTRIBUTOR_ID=" + Convert.ToInt32(Distributor_ID) + "AND DOCUMENT_NO=" + Convert.ToInt64(pm.REBATE_MASTER_ID) + "AND DOCUMENT_TYPE_ID=" + Constants.Document_Sale_Rebate + "AND PAYMENT_MODE=" + Constants.Document_Sale_Rebate, con))
                {
                    command.ExecuteNonQuery();
                }
                using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(Distributor_ID), con))
                {
                    command1.ExecuteNonQuery();
                }
                using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER + "' AND DISTRIBUTOR_ID =" + Convert.ToInt32(Distributor_ID), con))
                {
                    command2.ExecuteNonQuery();
                }
                con.Close();
            }
            string remarks;
            if (txtRemarks.Equals(""))
            {
                remarks = txtRemarks.Text;
            }
            else
            {
                remarks = " / " + txtRemarks.Text;
            }
            string Info = //"Default Purchase / Ref.# " + pm.PURCHASE_MASTER_ID + 
                remarks; //+ " / " + ddlVendor.SelectedItem.Text; //"Ref.# " + pm.SALE_MASTER_ID; // + ", " + ItemInfo;
            //string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + " / " + txtRemarks.Text + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(TotalRebate)
               , pm.DATE, pm.CUSTOMER_ID, Constants.Document_Sale_Rebate
               , "", Convert.ToInt32(Session["UserID"]), pm.REBATE_MASTER_ID
               , Convert.ToString(pm.REBATE_MASTER_ID)
               , Convert.ToDecimal(0), Convert.ToDecimal(TotalStockInHandRebate), Convert.ToDecimal(TotalconsumptionRebate), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                string VOUCHER_NO = Session["VoucherNo2"].ToString();
                con.Open();
                using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_REBATE_MASTER SET VOUCHER_NO='" + VOUCHER_NO + "' WHERE REBATE_MASTER_ID=" + Convert.ToInt32(pm.REBATE_MASTER_ID), con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            Show_Voucher(hfVoucher_No.Value);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["RebateMasterID"] = null;
            Session["VoucherNo2"] = null;
            Session["RebateDISTRIBUTOR_ID"] = null;
        }
        else
        {
            pm.TYPE_ID = 13;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pml.Save(pm);
            int REBATE_MASTER_ID = pm.REBATE_MASTER_ID;
            SaleRebateDetailLogics pdl = new SaleRebateDetailLogics();
            tbl_SALE_REBATE_DETAIL pd = new tbl_SALE_REBATE_DETAIL();
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int ITEM_INFO_ID = Convert.ToInt32(row.Cells[0].Text);
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Rebate1 = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtRebate1")).Text;
                    string Rebate2 = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtRebate2")).Text;
                    string Rebate3 = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtRebate3")).Text;
                    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[7].FindControl("txtBalance")).Text;

                    int Project_ID = Convert.ToInt32(row.Cells[10].Text);
                    int Category_ID = Convert.ToInt32(row.Cells[11].Text);
                    int SubCategory_ID = Convert.ToInt32(row.Cells[12].Text);
                    int IS_INVENTORY = Convert.ToInt32(row.Cells[13].Text);
                    int ITEM_LOG_ID = Convert.ToInt32(row.Cells[14].Text);
                    decimal ItemAmount = Convert.ToDecimal(row.Cells[15].Text);

                    pd.REBATE_MASTER_ID = REBATE_MASTER_ID;
                    pd.ITEM_INFO_ID = ITEM_INFO_ID;
                    pd.PROJECT_ID = Project_ID;
                    pd.CATEGORY_ID = Category_ID;
                    pd.SUB_CATEGORY_ID = SubCategory_ID;
                    pd.SALE_AMOUNT = Convert.ToDecimal(SaleAmount);
                    pd.REBATE1 = Convert.ToDecimal(Rebate1);
                    pd.REBATE2 = Convert.ToDecimal(Rebate2);
                    pd.REBATE3 = Convert.ToDecimal(Rebate3);
                    pd.COMMISSION_PERCENT = 0;
                    pd.COMMISSION_VALUE = 0;
                    pd.COMMISSION = 0;
                    pd.BALANCE = Convert.ToDecimal(SaleAmount) - Convert.ToDecimal(Rebate1) - Convert.ToDecimal(Rebate2) - Convert.ToDecimal(Rebate3);
                    if (IS_INVENTORY != 1)
                    {
                        TotalStockInHandRebate += Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);
                        decimal Amount = ItemAmount - Convert.ToDecimal(Rebate1) - Convert.ToDecimal(Rebate2) - Convert.ToDecimal(Rebate3);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET ItemAmount = " + Amount + " WHERE ITEM_LOG_ID=" + ITEM_LOG_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                    else
                    {
                        TotalconsumptionRebate += Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);
                    }
                    pd.DEBIT_AMOUNT = Convert.ToDecimal(Rebate1) + Convert.ToDecimal(Rebate2) + Convert.ToDecimal(Rebate3);
                    pd.REBATE_CLOSED = Convert.ToBoolean(0);
                    pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                    pd.TIME_STEMP = DateTime.Now;
                    pd.IS_DELETED = Convert.ToBoolean(0);
                    pdl.Save(pd);

                }
            }

            string remarks;
            if (txtRemarks.Equals(""))
            {
                remarks = txtRemarks.Text;
            }
            else
            {
                remarks = " / " + txtRemarks.Text;
            }
            string Info = //"Default Purchase / Ref.# " + pm.PURCHASE_MASTER_ID + 
                remarks; //+ " / " + ddlVendor.SelectedItem.Text; //"Ref.# " + pm.SALE_MASTER_ID; // + ", " + ItemInfo;
            //string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + " / " + txtRemarks.Text + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(TotalRebate)
               , pm.DATE, pm.CUSTOMER_ID, Constants.Document_Sale_Rebate
               , "", Convert.ToInt32(Session["UserID"]), pm.REBATE_MASTER_ID
               , Convert.ToString(pm.REBATE_MASTER_ID)
               , Convert.ToDecimal(0), Convert.ToDecimal(TotalStockInHandRebate), Convert.ToDecimal(TotalconsumptionRebate), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                string VOUCHER_NO = Session["VoucherNo2"].ToString();
                con.Open();
                using (SqlCommand command = new SqlCommand("UPDATE tbl_SALE_REBATE_MASTER SET VOUCHER_NO='" + VOUCHER_NO + "' WHERE REBATE_MASTER_ID=" + Convert.ToInt32(pm.REBATE_MASTER_ID), con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            Show_Voucher(hfVoucher_No.Value);
            Session["VoucherNo2"] = null;
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
        , string p_MANUAL_DOCUMENT_NO, decimal p_Discount_Amount, decimal p_StockInHandRebate, decimal p_ConsumptionRebate, string p_Remarks, decimal p_Commisson)
    {

        try
        {

            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);

            #region GL Master, Detail
            Session["VoucherNo2"] = null;
            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);
            bool IsFinanceSetting = GetFinanceConfig();
            if (p_DOCUMENT_TYPE_ID == Constants.Document_Sale_Rebate)
            {
                decimal Purchases = p_Credit + p_Discount_Amount;
                decimal Rebate_Amount = p_StockInHandRebate + p_ConsumptionRebate;
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, 0, Rebate_Amount, p_Ledger_Date, "Default Sale Rebate / " + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.Document_Sale_Rebate, "Sale Rebate", 0, Constants.Document_Sale_Rebate);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, Rebate_Amount, 0, p_Ledger_Date, "Default Sale Rebate / " + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.Document_Sale_Rebate, "Sale Rebate", 0, Constants.Document_Sale_Rebate);
                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Sale_Rebate
                        , p_MANUAL_DOCUMENT_NO, "Rebate Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Rebate", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Commission_Charges, Rebate_Amount, 0, "Rebate Voucher", mTransaction, mConnection);
                    if (p_StockInHandRebate > 0)
                    {
                        LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, 0, p_StockInHandRebate, "Rebate Voucher", mTransaction, mConnection);
                    }

                    if (p_ConsumptionRebate > 0)
                    {
                        LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, 0, p_ConsumptionRebate, "Rebate Voucher", mTransaction, mConnection);
                    }
                    hfVoucher_No.Value = VoucherNo2;
                    Session["VoucherNo2"] = VoucherNo2;
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
    public void Show_Voucher(string Voucher_No)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        crpVoucherView CrpReport = new crpVoucherView();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(ddlLocation.SelectedItem.Value.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(ddlLocation.SelectedItem.Value.ToString()), Voucher_No, 16);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VoucherType", "Journal Voucher");
        CrpReport.SetParameterValue("VoucherSubType", "Journal Voucher");
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
        DoEmptyTextBox();
        PanelPerchase.Visible = false;
        panelgrid.Visible = true;
        mastergrid.Visible = true;
        LoadGrid("");
        CreatTable();
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
    }
    protected void lnkBtn_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        ItemLogics Ser = new ItemLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        if (!CheckDublicate(Convert.ToInt32(ProID)))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + data.ITEM_NAME + " is already exists.');", true);
            //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        var qry = (
                   from i in db.ITEM_INFORMATION
                   join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID
                   join c in db.CATEGORies on i.CATEGORY_ID equals c.CATEGORY_ID
                   join subc in db.SUB_CATEGORY on i.SUB_CATEGORY_ID equals subc.SUB_CATEGORY_ID
                   join m in db.MEASURMENTs on i.MEASUREMENT equals m.MEASURMENT_ID

                   from b in db.BLOCKs
                        .Where(x => x.BLOCK_ID == i.BLOCK_ID)
                        .DefaultIfEmpty() // <== makes left join

                   from s in db.STREETs
                    .Where(x => x.STREET_ID == i.STREET_ID)
                    .DefaultIfEmpty() // <== makes left join

                   select new
                   {
                       ITEM_INFO_ID = i.ITEM_INFO_ID,
                       ITEM_NAME = i.ITEM_NAME,
                       p.PROJECT_NAME,
                       c.CATEGORY_NAME,
                       subc.SUB_CATEGORY_NAME,
                       MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
                       BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
                       STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
                   }
                    );
        var xy = (qry).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID).ToList();
        DataRow dr = PurchaseDetail.NewRow();
        dr["ITEM_INFO_ID"] = data.ITEM_INFO_ID;
        dr["ITEM_INFO_NAME"] = xy.FirstOrDefault().ITEM_NAME + ", " + xy.FirstOrDefault().PROJECT_NAME + ", " + xy.FirstOrDefault().CATEGORY_NAME + ", " + xy.FirstOrDefault().SUB_CATEGORY_NAME + ", " + xy.FirstOrDefault().BLOCK_NAME + ", " + xy.FirstOrDefault().STREET_NAME + ", " + xy.FirstOrDefault().MEASURMENT;
        dr["PROJECT_ID"] = data.PROJECT_ID;
        dr["CATEGORY_ID"] = data.CATEGORY_ID;
        dr["SUB_CATEGORY_ID"] = data.SUB_CATEGORY_ID;
        dr["SaleAmount"] = "";
        dr["Discount"] = "";
        dr["CommissionType"] = 1;
        dr["COMMISSION_PER"] = 0;
        dr["COMMISSION_VALUE"] = 0;
        dr["Commission"] = "";
        dr["Balance"] = "";
        dr["TOKEN_AMOUNT"] = 0;
        dr["DOWN_PAYMENT"] = 0;
        PurchaseDetail.Rows.Add(dr);
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        HideGridColumns();

        this.Session.Add("PurchaseDetail", PurchaseDetail);
        //ddlLocation.Enabled = false;
        //ddlProject.Enabled = false;
        //ddlCategory.Enabled = false;
        //ddlSubCategory.Enabled = false;
        PanelItems.Visible = true;
        SetTotals();
        LoadItems(ProID);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            foreach (DataRow dr in PurchaseDetail.Rows)
            {
                if (int.Parse(dr["ITEM_INFO_ID"].ToString()) == int.Parse(row.Cells[0].Text))
                {
                    decimal Balance = 0;
                    decimal SaleAmount = decimal.Parse(dr["SaleAmount"].ToString());
                    string Rebate1 = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtRebate1")).Text;
                    string Rebate2 = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtRebate2")).Text;
                    string Rebate3 = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtRebate3")).Text;
                    Balance = SaleAmount - decimal.Parse(dc.chkNull_0(Rebate1)) - decimal.Parse(dc.chkNull_0(Rebate2)) - decimal.Parse(dc.chkNull_0(Rebate3));
                    dr["Rebate1"] = Rebate1;
                    dr["Rebate2"] = Rebate2;
                    dr["Rebate3"] = Rebate3;
                    dr["Balance"] = Balance;
                    break;
                }
            }

        }
        var ProID = ((LinkButton)sender).CommandArgument;
        var query = PurchaseDetail.AsEnumerable().Where(r => r.Field<int>("ITEM_INFO_ID") == int.Parse(ProID));
        foreach (var row in query.ToList())
            row.Delete();
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        HideGridColumns();
        this.Session.Add("PurchaseDetail", PurchaseDetail);
        SetTotals();
        LoadItems("");
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>Calculation();</script>", false);
    }
    #endregion

    #region Index Change
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        //GridHeaderFilterMode headerFilterMode = GridHeaderFilterMode.CheckedList;
        //foreach (GridViewDataColumn column in grid.Columns)
        //    column.SettingsHeaderFilter.Mode = headerFilterMode;
        LoadItems("");
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
    #endregion

    protected virtual void PrepareTotalFilterItems(ASPxGridViewHeaderFilterEventArgs e)
    {
        e.Values.Clear();
        if (e.Column.SettingsHeaderFilter.Mode == GridHeaderFilterMode.List)
            e.AddShowAll();
        int step = 100;
        for (int i = 0; i < 10; i++)
        {
            double start = step * i;
            double end = start + step - 0.01;
            e.AddValue(string.Format("from {0:c} to {1:c}", start, end), "", string.Format("[Total] >= {0} and [Total] <= {1}", start, end));
        }
        e.AddValue(string.Format("> {0:c}", 1000), "", "[Total] > 1000");
    }
    protected virtual void PrepareQuantityFilterItems(ASPxGridViewHeaderFilterEventArgs e)
    {
        int max = 0;
        for (int i = 0; i < e.Values.Count; i++)
        {
            int value;
            if (!int.TryParse(e.Values[i].Value, out value)) continue;
            if (value > max) max = value;
        }
        e.Values.Clear();
        if (e.Column.SettingsHeaderFilter.Mode == GridHeaderFilterMode.List)
            e.AddShowAll();
        int step = 10;
        for (int i = 0; i < max / step + 1; i++)
        {
            int start = step * i;
            int end = start + step - 1;
            e.AddValue(string.Format("from {0} to {1}", start, end), "", string.Format("[Quantity] >= {0} and [Quantity] <= {1}", start, end));
        }
    }


    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Session["PurchaseDetail"] = null;
        PanelPerchase.Visible = true;
        panelgrid.Visible = false;
        DoEmptyTextBox();
        CreatTable();
        LoadItems("");
    }

    protected void SaleMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        SaleMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["RebateMasterID"] = null;
        Session["RebateVoucheNo"] = null;
        Session["RebateDISTRIBUTOR_ID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleRebateMasterLogics Ser = new SaleRebateMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var dController = new DistributorController();
        //var dtt = dController.SelectPurchaseDetailPaymentInfo(Constants.Journal_Voucher, data.VENDOR_ID, data.PURCHASE_MASTER_ID, Constants.Document_Purchase, 0, 1);
        //if (dtt.Rows.Count > 0)
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
        LoadCategories();
        LoadSubCategories();
        LoadVendors();
        ddlVendor.Value = Convert.ToString(data.CUSTOMER_ID);
        txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
        CreatTable();
        DataTable dt = LController.SelectBSI(data.REBATE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 47);
        foreach (DataRow gvr in dt.Rows)
        {
            PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
            DataRow dr = PurchaseDetail.NewRow();
            dr["ITEM_INFO_ID"] = gvr["ITEM_INFO_ID"];
            dr["ITEM_INFO_NAME"] = gvr["ITEM_INFO_NAME"];
            dr["SaleAmount"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["SaleAmount"].ToString())), 0);
            dr["Rebate1"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Rebate1"].ToString())), 0);
            dr["Rebate2"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Rebate2"].ToString())), 0);
            dr["Rebate3"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Rebate3"].ToString())), 0);
            dr["Balance"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Balance"].ToString())), 0);
            dr["PROJECT_ID"] = gvr["PROJECT_ID"];
            dr["CATEGORY_ID"] = gvr["CATEGORY_ID"];
            dr["SUB_CATEGORY_ID"] = gvr["SUB_CATEGORY_ID"];
            dr["IS_INVENTORY"] = int.Parse(gvr["IS_INVENTORY"].ToString());
            dr["ITEM_LOG_ID"] = int.Parse(gvr["ITEM_LOG_ID"].ToString());
            dr["ItemAmount"] = decimal.Parse(gvr["ItemAmount"].ToString());
            PurchaseDetail.Rows.Add(dr);
        }
        txtRemarks.Text = data.REMARKS;
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        if (dt.Rows.Count > 0)
        {
            HideGridColumns();
        }

        int TotalSaleAmount = 0;
        int TotalRebate1Amount = 0;
        int TotalRebate2Amount = 0;
        int TotalRebate3Amount = 0;
        int TotalBalanceAmount = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
            string Rebate1 = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtRebate1")).Text;
            string Rebate2 = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtRebate2")).Text;
            string Rebate3 = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtRebate3")).Text;
            string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[7].FindControl("txtBalance")).Text;
            TotalSaleAmount += Convert.ToInt32(Math.Round(decimal.Parse(SaleAmount), 0));
            TotalRebate1Amount += Convert.ToInt32(Math.Round(decimal.Parse(Rebate1), 0));
            TotalRebate2Amount += Convert.ToInt32(Math.Round(decimal.Parse(Rebate2), 0));
            TotalRebate3Amount += Convert.ToInt32(Math.Round(decimal.Parse(Rebate3), 0));
            TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(Balance), 0));
        }
        ////HtmlControl control = FindControl("lblSaleAmount") as HtmlControl;
        ////control.Attributes["Class"] = TotalSaleAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[2].Text = TotalSaleAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[3].Text = TotalDiscountAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[5].Text = TotalCommissionAmount.ToString();
        //ASPxGridView1.FooterRow.Cells[6].Text = TotalBalanceAmount.ToString();
        this.Session.Add("PurchaseDetail", PurchaseDetail);
        SetTotals();
        LoadItems("");
        Session.Add("RebateMasterID", data.REBATE_MASTER_ID);
        Session.Add("RebateDISTRIBUTOR_ID", data.DISTRIBUTOR_ID);
        Session.Add("RebateVoucheNo", data.VOUCHER_NO);
        mastergrid.Visible = false;
        PanelPerchase.Visible = true;
        panelgrid.Visible = false;
        PanelItems.Visible = true;
        btnSave.Text = "Update";
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>Calculation();</script>", false);
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        //bool flag = false;
        //DbCalls db = new DbCalls();
        //var ProID = ((LinkButton)sender).CommandArgument;
        //SaleMasterLogics Ser = new SaleMasterLogics();
        //var data = Ser.Get(Convert.ToInt32(ProID));
        //bool Active = Convert.ToBoolean(data.IS_DELETED);
        //var dController = new DistributorController();
        //var dtt = dController.SelectPurchaseDetailPaymentInfo(Constants.Journal_Voucher, data.VENDOR_ID, data.PURCHASE_MASTER_ID, Constants.Document_Purchase, 0, 1);
        //if (dtt.Rows.Count > 0)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete. Some payments are issued against this purchase.');", true);
        //}
        //else
        //{
        //    try
        //    {
        //        //var qq = (from k in db.tbl_SALE_MASTER
        //        //          join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
        //        //          select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
        //        //if (qq.Any())
        //        //{
        //        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete because this item has been sold.');", true);
        //        //}
        //        //else
        //        //{
        //        var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
        //        if (!Active)
        //        {
        //            var qry = (from k in db.tbl_ITEM_LOG select k).Where(x => x.TransactionID == data.PURCHASE_MASTER_ID && x.TypeID == Constants.Document_Purchase && x.IsActive == true).ToList();
        //            int ItemLogID = Convert.ToInt32(qry.FirstOrDefault().ITEM_LOG_ID);
        //            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        //            {
        //                con.Open();
        //                using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_MASTER where PURCHASE_MASTER_ID =" + Convert.ToInt32(ProID), con))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //                using (SqlCommand command2 = new SqlCommand("DELETE tbl_PURCHASE_DETAIL where PURCHASE_MASTER_ID =" + Convert.ToInt32(ProID), con))
        //                {
        //                    command2.ExecuteNonQuery();
        //                }
        //                using (SqlCommand command4 = new SqlCommand("DELETE tbl_ITEM_LOG where ITEM_LOG_ID =" + Convert.ToInt32(ItemLogID), con))
        //                {
        //                    command4.ExecuteNonQuery();
        //                }
        //                if (q.Count > 0)
        //                {
        //                    string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
        //                    using (SqlCommand command3 = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale + "and TYPE_ID=" + Constants.Document_Purchase, con))
        //                    {
        //                        command3.ExecuteNonQuery();
        //                    }
        //                    using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
        //                    {
        //                        command1.ExecuteNonQuery();
        //                    }
        //                    using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
        //                    {
        //                        command2.ExecuteNonQuery();
        //                    }
        //                }
        //                con.Close();
        //            }
        //            flag = true;
        //            if (flag)
        //            {
        //                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
        //            }
        //        }
        //        //}
        //    }
        //    catch (Exception ex) { }
        //}
        //LoadGrid("");
    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadItems(txtSearch.Text);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int ItemCount = Constants.IntNullValue;
        foreach (GridViewRow r in grid.Rows)
        {
            System.Web.UI.WebControls.CheckBox ChkChecked = (System.Web.UI.WebControls.CheckBox)r.Cells[0].FindControl("ChbIsAssigned");
            if (ChkChecked.Checked == true)
            {
                ItemCount++;
                string ProID = r.Cells[11].Text;
                DbCalls db = new DbCalls();
                ItemLogics Ser = new ItemLogics();
                var data = Ser.Get(Convert.ToInt32(ProID));
                PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
                if (!CheckDublicate(Convert.ToInt32(ProID)))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + data.ITEM_NAME + " is already exists.');", true);
                    //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
                    return;
                }

                DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 48);
                if (dt.Rows.Count > 0)
                {
                    DataRow[] foundRows = dt.Select("ITEM_INFO_ID  = '" + int.Parse(ProID.ToString()) + "'");
                    if (foundRows.Length > 0)
                    {
                        DataRow dr = PurchaseDetail.NewRow();
                        dr["ITEM_INFO_ID"] = data.ITEM_INFO_ID;
                        dr["ITEM_INFO_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
                        dr["PROJECT_ID"] = data.PROJECT_ID;
                        dr["CATEGORY_ID"] = data.CATEGORY_ID;
                        dr["SUB_CATEGORY_ID"] = data.SUB_CATEGORY_ID;
                        decimal value = decimal.Parse(foundRows[0]["SALE_AMOUNT"].ToString());
                        dr["SaleAmount"] = Math.Round(value, 0);
                        dr["Rebate1"] = "0";
                        dr["CommissionType"] = 1;
                        dr["COMMISSION_PER"] = 0;
                        dr["COMMISSION_VALUE"] = 0;
                        dr["Commission"] = "";
                        dr["Balance"] = Math.Round(value, 0);
                        dr["Rebate2"] = "0";
                        dr["Rebate3"] = "0";
                        dr["IS_INVENTORY"] = int.Parse(foundRows[0]["IS_INVENTORY"].ToString());
                        dr["ITEM_LOG_ID"] = int.Parse(foundRows[0]["ITEM_LOG_ID"].ToString());
                        dr["ItemAmount"] = decimal.Parse(foundRows[0]["ItemAmount"].ToString());
                        PurchaseDetail.Rows.Add(dr);
                    }
                }



                //var qry = (
                //           from pd in db.tbl_PURCHASE_DETAIL
                //           join i in db.ITEM_INFORMATION on pd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                //           join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID
                //           join c in db.CATEGORies on i.CATEGORY_ID equals c.CATEGORY_ID
                //           join subc in db.SUB_CATEGORY on i.SUB_CATEGORY_ID equals subc.SUB_CATEGORY_ID
                //           join m in db.MEASURMENT on i.MEASUREMENT equals m.MEASURMENT_ID

                //           from b in db.BLOCK
                //                .Where(x => x.BLOCK_ID == i.BLOCK_ID)
                //                .DefaultIfEmpty() // <== makes left join

                //           from s in db.STREET
                //            .Where(x => x.STREET_ID == i.STREET_ID)
                //            .DefaultIfEmpty() // <== makes left join

                //           select new
                //           {
                //               ITEM_INFO_ID = i.ITEM_INFO_ID,
                //               ITEM_NAME = i.ITEM_NAME,
                //               p.PROJECT_NAME,
                //               c.CATEGORY_NAME,
                //               subc.SUB_CATEGORY_NAME,
                //               MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
                //               BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
                //               STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
                //               pd.SALE_AMOUNT,
                //               pd.PURCHASE_DETAIL_ID,
                //               pd.ITEM_LOG_ID
                //           }
                //            );
                //var xy = (qry).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID).ToList();

                ASPxGridView1.DataSource = PurchaseDetail;
                ASPxGridView1.DataBind();
                HideGridColumns();
                this.Session.Add("PurchaseDetail", PurchaseDetail);
                PanelItems.Visible = true;
                SetTotals();
                LoadItems("");
            }
        }
        if (ItemCount == Constants.IntNullValue)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an item first');", true);
            return;
        }
    }

    protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid.PageIndex = e.NewPageIndex;
        LoadItems("");
    }
}