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
using System.Web.UI.WebControls;

public partial class Forms_frmAddSaleEntry : System.Web.UI.Page
{
    DataTable PurchaseDetail, PurchaseDetailUpdate, SearchTable;
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
            CreatUpdateTable();
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
            LoadItems("");
            PanelSearch.Visible = true;
            PanelPerchase.Visible = false;
            panelgrid.Visible = true;
            LoadGrid("");
            Session["PurchasMasterID"] = null;
        }


        //GridHeaderFilterMode headerFilterMode = true ? GridHeaderFilterMode.CheckedList : GridHeaderFilterMode.List;
        //foreach (GridViewDataColumn column in grid.Columns)
        //    column.SettingsHeaderFilter.Mode = headerFilterMode;

        //GridHeaderFilterMode headerFilterMode = GridHeaderFilterMode.CheckedList;
        //foreach (GridViewDataColumn column in grid.Columns)
        //    column.SettingsHeaderFilter.Mode = headerFilterMode;
        
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
      from k in db.tbl_ITEM_LOG
      join i in db.ITEM_INFORMATION on k.ItemID equals i.ITEM_INFO_ID
      join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID
      join c in db.CATEGORies on i.CATEGORY_ID equals c.CATEGORY_ID
      join subc in db.SUB_CATEGORY on i.SUB_CATEGORY_ID equals subc.SUB_CATEGORY_ID
      join m in db.MEASURMENTs on i.MEASUREMENT equals m.MEASURMENT_ID

      from b in db.BLOCKs
           .Where(x => x.BLOCK_ID == i.BLOCK_ID)
           .DefaultIfEmpty() // <== makes join left join

      from s in db.STREETs
      .Where(x => x.STREET_ID == i.STREET_ID)
      .DefaultIfEmpty() // <== makes join left join

      from pr in db.PRECENTs
      .Where(x => x.PRECENT_ID == i.PRECENT_ID)
      .DefaultIfEmpty() // <== makes join left join

      from prs in db.PRECENTSTREETs
      .Where(x => x.PRECENTSTREET_ID == i.PRECENTSTREET_ID)
      .DefaultIfEmpty() // <== makes join left join

      from sc in db.SECTORs
      .Where(x => x.SECTOR_ID == i.SECTOR_ID)
      .DefaultIfEmpty() // <== makes join left join

          //from groups in db.STREET
          //       .Where(gruppe => gruppe.STREET_ID == mappings.USRGRP_GRP)
          //       .DefaultIfEmpty() // <== makes join left join

          // where users.USR_Name.Contains(keyword)
          // || mappings.USRGRP_USR.Equals(666)  
          // || mappings.USRGRP_USR == 666 
          // || groups.Name.Contains(keyword)

      select new
      {
          REF_ID = k.TransactionID,
          ITEM_INFO_ID = i.ITEM_INFO_ID,
          p.PROJECT_ID,
          p.PROJECT_NAME,
          ITEM_NAME = i.ITEM_NAME,
          i.OPEN_FROM_REGISTRATION,
          i.TOWER_NO,
          i.FLOOR_NO,
          MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
          BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
          STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
          PRECENT_NAME = pr.PRECENT_NAME == null ? "N/A" : pr.PRECENT_NAME,
          PRECENTSTREET_NAME = prs.PRECENTSTREET_NAME == null ? "N/A" : prs.PRECENTSTREET_NAME,
          SECTOR_NAME = sc.SECTOR_NAME == null ? "N/A" : sc.SECTOR_NAME,
          c.CATEGORY_ID,
          subc.SUB_CATEGORY_ID,
          PURCHASE_DETAIL_ID = k.TransactionDetailID,
          RefID = k.ITEM_LOG_ID,
          k.TypeID,
          k.IsActive,
          k.ITEM_LOG_ID
      }
  );
        var xy = (query2).Where(x => x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID && x.TypeID == 2 && x.IsActive == true 
        || x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID && x.TypeID == 26 && x.IsActive == true 
        || x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID && x.TypeID == 14 && x.IsActive == true
        || x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID && x.TypeID == 7 && x.IsActive == true).ToList();
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, CATEGORYID, SUBCATEGORYID, null, 29);
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
            dr["REF_ID"] = row.REF_ID;
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
            dr["PURCHASE_DETAIL_ID"] = row.PURCHASE_DETAIL_ID;
            dr["RefID"] = row.RefID;
            dr["TypeID"] = row.TypeID;
            dr["IsActive"] = row.IsActive;
            dr["ITEM_LOG_ID"] = row.ITEM_LOG_ID;
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
        Session["PurchaseDetail"] = null;
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetail.Columns.Add("SaleAmount", typeof(string));
        PurchaseDetail.Columns.Add("Discount", typeof(string));
        PurchaseDetail.Columns.Add("LessDiscount", typeof(string));
        PurchaseDetail.Columns.Add("CommissionType", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_PER", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_VALUE", typeof(decimal));
        PurchaseDetail.Columns.Add("Commission", typeof(string));
        PurchaseDetail.Columns.Add("Balance", typeof(string));
        PurchaseDetail.Columns.Add("TOKEN_AMOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("DOWN_PAYMENT", typeof(decimal));
        PurchaseDetail.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetail.Columns.Add("CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("SUB_CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("REF_ID", typeof(int));
        PurchaseDetail.Columns.Add("Consumption", typeof(decimal));
        PurchaseDetail.Columns.Add("REF_DETAIL_ID", typeof(int));
        PurchaseDetail.Columns.Add("RefID", typeof(long));
        this.Session.Add("PurchaseDetail", PurchaseDetail);



        //dr["REF_ID"] = row.REF_ID;
        //dr["ITEM_INFO_ID"] = row.ITEM_INFO_ID;
        //dr["PROJECT_ID"] = row.PROJECT_ID;
        //dr["PROJECT_NAME"] = row.PROJECT_NAME;
        //dr["ITEM_NAME"] = row.ITEM_NAME;
        //dr["OPEN_FROM_REGISTRATION"] = row.OPEN_FROM_REGISTRATION;
        //dr["TOWER_NO"] = row.TOWER_NO;
        //dr["FLOOR_NO"] = row.FLOOR_NO;
        //dr["MEASURMENT"] = row.MEASURMENT;
        //dr["BLOCK_NAME"] = row.BLOCK_NAME;
        //dr["STREET_NAME"] = row.STREET_NAME;
        //dr["PRECENT_NAME"] = row.PRECENT_NAME;
        //dr["PRECENTSTREET_NAME"] = row.PRECENTSTREET_NAME;
        //dr["SECTOR_NAME"] = row.SECTOR_NAME;
        //dr["CATEGORY_ID"] = row.CATEGORY_ID;
        //dr["SUB_CATEGORY_ID"] = row.SUB_CATEGORY_ID;
        //dr["PURCHASE_DETAIL_ID"] = row.PURCHASE_DETAIL_ID;
        //dr["RefID"] = row.RefID;
        //dr["TypeID"] = row.TypeID;
        //dr["IsActive"] = row.IsActive;
        //dr["ITEM_LOG_ID"] = row.ITEM_LOG_ID;
    }
    public void CreateSearchTable()
    {
        Session["SearchTable"] = null;
        SearchTable = new DataTable();
        SearchTable.Columns.Add("REF_ID", typeof(int));
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
        SearchTable.Columns.Add("PURCHASE_DETAIL_ID", typeof(int));
        SearchTable.Columns.Add("RefID", typeof(long));
        SearchTable.Columns.Add("TypeID", typeof(int));
        SearchTable.Columns.Add("IsActive", typeof(bool));
        SearchTable.Columns.Add("ITEM_LOG_ID", typeof(long));
        this.Session.Add("SearchTable", SearchTable);
    }
    private void CreatUpdateTable()
    {
        PurchaseDetailUpdate = new DataTable();
        PurchaseDetailUpdate.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetailUpdate.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("CATEGORY_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("SUB_CATEGORY_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("REF_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("Consumption", typeof(decimal));
        PurchaseDetailUpdate.Columns.Add("REF_DETAIL_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("RefID", typeof(long));
        this.Session.Add("PurchaseDetailUpdate", PurchaseDetailUpdate);
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
        ASPxGridView1.DataSource = null;
        ASPxGridView1.DataBind();
        txtTotalBalance.Text = "";
        txtTotalCommission.Text = "";
        txtTotalDiscountAmount.Text = "";
        txtTotalLessDiscountAmount.Text = "";
        txtTotalDownPayment.Text = "";
        txtTotalPurchaseAmount.Text = "";
        txtTotalTokenAmount.Text = "";
        txtRemarks.Text = "";

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
        ASPxGridView1.FooterRow.Cells[0].Visible = false;
        ASPxGridView1.FooterRow.Cells[2].Text = "Total";
        ASPxGridView1.FooterRow.Cells[2].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[3].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[4].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[5].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[6].Font.Bold = true;
        ASPxGridView1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
        ASPxGridView1.FooterRow.Cells[7].Visible = false;
        ASPxGridView1.FooterRow.Cells[8].Visible = false;
        ASPxGridView1.FooterRow.Cells[9].Visible = false;
        ASPxGridView1.FooterRow.Cells[10].Visible = false;
        ASPxGridView1.FooterRow.Cells[11].Visible = false;
        ASPxGridView1.FooterRow.Cells[12].Visible = false;
        ASPxGridView1.FooterRow.Cells[13].Visible = false;
        ASPxGridView1.FooterRow.Cells[14].Visible = false;
        ASPxGridView1.FooterRow.Cells[15].Visible = false;
        ASPxGridView1.FooterRow.Cells[16].Visible = false;
        ASPxGridView1.FooterRow.Cells[17].Visible = false;
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
        decimal TotalLessDiscount = 0;
        decimal TotalCommission = 0;
        decimal TotalBalance = 0;
        decimal TotalTokenAmount = 0;
        decimal TotalDownPayment = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                TotalPurchaseAmount += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text));
                TotalDiscount += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text));
                TotalLessDiscount += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text));
                TotalBalance += Convert.ToDecimal(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text));
                TotalTokenAmount += Convert.ToDecimal(row.Cells[9].Text);
                TotalDownPayment += Convert.ToDecimal(row.Cells[10].Text);
            }
        }
        txtTotalPurchaseAmount.Text = Convert.ToString(TotalPurchaseAmount);
        txtTotalDiscountAmount.Text = Convert.ToString(TotalDiscount);
        txtTotalLessDiscountAmount.Text = Convert.ToString(TotalLessDiscount);
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
            var qry = (from k in db.tbl_SALE_MASTER
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       join v in db.VENDORS on k.CUSTOMER_ID equals v.VENDOR_ID
                       select new
                       {
                           d.DISTRIBUTOR_NAME,
                           k.DATE,
                           v.VENDOR_NAME,
                           k.SALE_MASTER_ID,
                           k.TOTAL_SALE_AMOUNT,
                           k.TOTAL_DISCOUNT,
                           k.TOTAL_LESS_DISCOUNT,
                           BALANCE = k.TOTAL_SALE_AMOUNT + k.TOTAL_DISCOUNT - k.TOTAL_LESS_DISCOUNT,
                           Active = k.IS_DELETED == false ? "Active" : "Inactive",
                           k.IS_DELETED,
                           k.TYPE_ID,
                       }).Where(x => x.Active == "Active" && x.TYPE_ID == 13).ToList();
            PurchaseMaster.DataSource = qry;
            PurchaseMaster.DataBind();
            int total = 0;
            int discount = 0;
            int lessdiscount = 0;
            int balance = 0;
            foreach (GridViewRow row in PurchaseMaster.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                total += amount;
                int disc = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                discount += disc;
                int lessdisc = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                lessdiscount += lessdisc;
                int balanceamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[7].Text), 0));
                balance += balanceamount;
            }
            if (qry.Count() > 0)
            {
                PurchaseMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
                PurchaseMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", discount);
                PurchaseMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", lessdiscount);
                PurchaseMaster.FooterRow.Cells[7].Text = string.Format("{0:n2}", balance);
                PurchaseMaster.FooterRow.Cells[3].Text = "Total";
                PurchaseMaster.FooterRow.Cells[3].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[4].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[5].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[6].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[7].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[8].Visible = false;
                PurchaseMaster.FooterRow.Cells[9].Visible = false;
            }
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(4);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearchMasterGrid.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearchMasterGrid.Text + "%' OR Convert(SALE_MASTER_ID, System.String)  LIKE '%" + txtSearchMasterGrid.Text + "%' OR Convert(DATE, System.String)  LIKE '%" + txtSearchMasterGrid.Text + "%'";
            if (dt.Rows.Count > 0)
            {
                PurchaseMaster.PageIndex = 0;
            }
            PurchaseMaster.DataSource = dt;
            PurchaseMaster.DataBind();
            int total = 0;
            int discount = 0;
            int lessdiscount = 0;
            int balance = 0;
            foreach (GridViewRow row in PurchaseMaster.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                total += amount;
                int disc = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                discount += disc;
                int lessdisc = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                lessdiscount += lessdisc;
                int balanceamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[7].Text), 0));
                balance += balanceamount;
            }
            if (dt.Rows.Count > 0)
            {
                PurchaseMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
                PurchaseMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", discount);
                PurchaseMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", lessdiscount);
                PurchaseMaster.FooterRow.Cells[7].Text = string.Format("{0:n2}", balance);
                PurchaseMaster.FooterRow.Cells[3].Text = "Total";
                PurchaseMaster.FooterRow.Cells[3].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[4].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[5].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[6].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[7].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[8].Visible = false;
                PurchaseMaster.FooterRow.Cells[9].Visible = false;
            }
        }
    }
    #endregion

    #region Grid Opration
    protected void GrdItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }
    #endregion

    #region Click Oprations
    public decimal Consumption;
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlLocation.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Location is required.');", true);
            ddlLocation.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlLocation);
            return;
        }
        //if (ddlProject.SelectedValue.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
        //    ddlProject.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
        //    return;
        //}
        if (ddlCategory.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
            ddlCategory.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (ddlSubCategory.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
            ddlSubCategory.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (ddlVendor.SelectedItem.Value.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Customer is required.');", true);
            ddlVendor.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        
        SetTotals();
        SaleMasterLogics pml = new SaleMasterLogics();
        tbl_SALE_MASTER pm = new tbl_SALE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
        pm.PROJECT_ID = 0; //Convert.ToInt32(ddlProject.SelectedValue);
        pm.CATEGORY_ID = 0; //Convert.ToInt32(ddlCategory.SelectedValue);
        pm.SUBCATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedValue);
        pm.CUSTOMER_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
        pm.COMMISSIONAGENT_ID = 0; //Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        pm.TOTAL_SALE_AMOUNT = Convert.ToDecimal(txtTotalPurchaseAmount.Text);
        pm.TOTAL_DISCOUNT = Convert.ToDecimal(dc.chkNull_0(txtTotalDiscountAmount.Text));
        pm.TOTAL_LESS_DISCOUNT = Convert.ToDecimal(dc.chkNull_0(txtTotalLessDiscountAmount.Text));
        pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(txtTotalTokenAmount.Text);
        pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(txtTotalDownPayment.Text);
        pm.TOTAL_BALANCE = Convert.ToDecimal(txtTotalBalance.Text);
        pm.TOTAL_COMMISSION = Convert.ToDecimal(txtTotalCommission.Text);
        pm.REMARKS = txtRemarks.Text;
        //pm.SR_NO = txtSR_NO.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        if (Session["SALE_MASTER_ID"] != null)
        {

            pm.TYPE_ID = 13;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pm.SALE_MASTER_ID = Convert.ToInt32(Session["SALE_MASTER_ID"]);
            pml.Update(pm);
            DbCalls db = new DbCalls();
            int SALE_MASTER_ID = pm.SALE_MASTER_ID;
            SaleDetailLogics pdl = new SaleDetailLogics();
            tbl_SALE_DETAIL pd = new tbl_SALE_DETAIL();
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_DETAIL where SALE_MASTER_ID=" + SALE_MASTER_ID, con))
                {
                    command.ExecuteNonQuery();
                }
                using (SqlCommand command2 = new SqlCommand("DELETE tbl_ITEM_LOG where TransactionID=" + SALE_MASTER_ID + "AND TypeID = 13", con))
                {
                    command2.ExecuteNonQuery();
                }
                con.Close();
            }
            DataTable dt = (DataTable)Session["PurchaseDetailUpdate"];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 1 where ITEM_LOG_ID=" + long.Parse(dr["RefID"].ToString()), con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int ITEM_INFO_ID = Convert.ToInt32(row.Cells[0].Text);
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                    string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
                    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                    decimal TOKEN_AMOUNT = Convert.ToDecimal(row.Cells[9].Text);
                    decimal DOWN_PAYMENT = Convert.ToDecimal(row.Cells[10].Text);

                    int Project_ID = Convert.ToInt32(row.Cells[11].Text);
                    int Category_ID = Convert.ToInt32(row.Cells[12].Text);
                    int SubCategory_ID = Convert.ToInt32(row.Cells[13].Text);
                    int REF_ID = Convert.ToInt32(row.Cells[14].Text);
                    decimal Consptn = Convert.ToDecimal(row.Cells[15].Text);
                    int REF_DETAIL_ID = Convert.ToInt32(row.Cells[16].Text);
                    int RefID = Convert.ToInt32(row.Cells[17].Text);

                    pd.SALE_MASTER_ID = SALE_MASTER_ID;
                    pd.ITEM_INFO_ID = ITEM_INFO_ID;
                    pd.PROJECT_ID = Project_ID;
                    pd.CATEGORY_ID = Category_ID;
                    pd.SUB_CATEGORY_ID = SubCategory_ID;
                    pd.SALE_AMOUNT = Convert.ToDecimal(SaleAmount);
                    pd.DISCOUNT = Convert.ToDecimal(Discount);
                    pd.LESS_DISCOUNT = Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                    pd.DOWN_PAYMENT = DOWN_PAYMENT;
                    pd.COMMISSION_PERCENT = 0;
                    pd.COMMISSION_VALUE = 0;
                    pd.COMMISSION = 0;
                    pd.BALANCE = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.DEBIT_AMOUNT = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.SALE_CLOSED = Convert.ToBoolean(0);
                    pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                    pd.TIME_STEMP = DateTime.Now;
                    pd.IS_DELETED = Convert.ToBoolean(0);
                    pd.REF_ID = REF_ID;
                    pdl.Save(pd);
                    Consumption += Consptn;

                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ITEM_LOG_ID=" + RefID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                    Item_Log_Logics Log = new Item_Log_Logics();
                    tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                    ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                    ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                    ILog.TransactionID = SALE_MASTER_ID;
                    ILog.TransactionDetailID = pd.SALE_DETAIL_ID;
                    ILog.TypeID = 13;
                    ILog.RefID = RefID;
                    ILog.ProjectID = Project_ID;
                    ILog.CategoryID = Category_ID;
                    ILog.SubCategoryID = SubCategory_ID;
                    ILog.ItemID = ITEM_INFO_ID;
                    ILog.ItemAmount = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    ILog.UserID = Convert.ToInt32(Session["UserID"]);
                    ILog.IsActive = Convert.ToBoolean(1);
                    ILog.TimeStamp = DateTime.Now;
                    Log.Save(ILog);

                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_SALE_DETAIL SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where SALE_DETAIL_ID=" + pd.SALE_DETAIL_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }

            var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == pm.SALE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
            if (q.Count > 0)
            {
                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.SALE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Invoice + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
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
            string Info = "Default Sale" + remarks; //+ " / " + ddlVendor.SelectedItem.Text; //"Ref.# " + pm.SALE_MASTER_ID; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(pm.TOTAL_SALE_AMOUNT)
               , pm.DATE, pm.CUSTOMER_ID, Constants.Document_SaleInvoice
               , "", Convert.ToInt32(Session["UserID"]), pm.SALE_MASTER_ID
               , Convert.ToString(pm.SALE_MASTER_ID)
               , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_LESS_DISCOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            //Show_Voucher(hfVoucher_No.Value);
            showReport(pm.SALE_MASTER_ID);
            Session["DBT_Amount"] = null;
            Session["BAL_Amount"] = null;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["SALE_MASTER_ID"] = null;
            //mastergrid.Visible = true;
        }
        else
        {
            pm.TYPE_ID = 13;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pml.Save(pm);
            int SALE_MASTER_ID = pm.SALE_MASTER_ID;
            SaleDetailLogics pdl = new SaleDetailLogics();
            tbl_SALE_DETAIL pd = new tbl_SALE_DETAIL();
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int ITEM_INFO_ID = Convert.ToInt32(row.Cells[0].Text);
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                    string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
                    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                    decimal TOKEN_AMOUNT = Convert.ToDecimal(row.Cells[9].Text);
                    decimal DOWN_PAYMENT = Convert.ToDecimal(row.Cells[10].Text);

                    int Project_ID = Convert.ToInt32(row.Cells[11].Text);
                    int Category_ID = Convert.ToInt32(row.Cells[12].Text);
                    int SubCategory_ID = Convert.ToInt32(row.Cells[13].Text);
                    int REF_ID = Convert.ToInt32(row.Cells[14].Text);
                    decimal Consptn = Convert.ToDecimal(row.Cells[15].Text);
                    int REF_DETAIL_ID = Convert.ToInt32(row.Cells[16].Text);
                    int RefID = Convert.ToInt32(row.Cells[17].Text);

                    pd.SALE_MASTER_ID = SALE_MASTER_ID;
                    pd.ITEM_INFO_ID = ITEM_INFO_ID;
                    pd.PROJECT_ID = Project_ID;
                    pd.CATEGORY_ID = Category_ID;
                    pd.SUB_CATEGORY_ID = SubCategory_ID;
                    pd.SALE_AMOUNT = Convert.ToDecimal(dc.chkNull_0(SaleAmount));
                    pd.DISCOUNT = Convert.ToDecimal(dc.chkNull_0(Discount));
                    pd.LESS_DISCOUNT = Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                    pd.DOWN_PAYMENT = DOWN_PAYMENT;
                    pd.COMMISSION_PERCENT = 0;
                    pd.COMMISSION_VALUE = 0;
                    pd.COMMISSION = 0;
                    pd.BALANCE = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.DEBIT_AMOUNT = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    pd.SALE_CLOSED = Convert.ToBoolean(0);
                    pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                    pd.TIME_STEMP = DateTime.Now;
                    pd.IS_DELETED = Convert.ToBoolean(0);
                    pd.REF_ID = REF_ID;
                    pdl.Save(pd);
                    Consumption += Consptn;

                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ITEM_LOG_ID=" + RefID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                    Item_Log_Logics Log = new Item_Log_Logics();
                    tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                    ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                    ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                    ILog.TransactionID = SALE_MASTER_ID;
                    ILog.TransactionDetailID = pd.SALE_DETAIL_ID;
                    ILog.TypeID = 13;
                    ILog.RefID = RefID;
                    ILog.ProjectID = Project_ID;
                    ILog.CategoryID = Category_ID;
                    ILog.SubCategoryID = SubCategory_ID;
                    ILog.ItemID = ITEM_INFO_ID;
                    ILog.ItemAmount = Convert.ToDecimal(dc.chkNull_0(SaleAmount)) + Convert.ToDecimal(dc.chkNull_0(Discount)) - Convert.ToDecimal(dc.chkNull_0(LessDiscount));
                    ILog.UserID = Convert.ToInt32(Session["UserID"]);
                    ILog.IsActive = Convert.ToBoolean(1);
                    ILog.TimeStamp = DateTime.Now;
                    Log.Save(ILog);

                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command2 = new SqlCommand("UPDATE tbl_SALE_DETAIL SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where SALE_DETAIL_ID=" + pd.SALE_DETAIL_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }

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
            string Info = "Default Sale" + remarks; //+ " / " + ddlVendor.SelectedItem.Text; //"Ref.# " + pm.SALE_MASTER_ID; // + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(pm.TOTAL_SALE_AMOUNT)
                , pm.DATE, pm.CUSTOMER_ID, Constants.Document_SaleInvoice
                , "", Convert.ToInt32(Session["UserID"]), pm.SALE_MASTER_ID
                , Convert.ToString(pm.SALE_MASTER_ID)
                , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_LESS_DISCOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            //Show_Voucher(hfVoucher_No.Value);
            showReport(pm.SALE_MASTER_ID);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        
        LoadGrid("");
        DoEmptyTextBox();
        LoadItems("");
        PanelSearch.Visible = true;
        mastergrid.Visible = true;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
    }
    private void LedgerEntries(int p_Distributor_Id
      , decimal p_Debit, decimal p_Credit, DateTime p_Ledger_Date, int p_PRINCIPAL_ID
      , int p_DOCUMENT_TYPE_ID, string pChequeNo, int p_UserId, long p_DOCUMENT_NO
      , string p_MANUAL_DOCUMENT_NO, decimal p_Discount_Amount, decimal p_Less_Discount_Amount, decimal p_DownPayment, string p_Remarks, decimal p_Commisson)
    {
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);

            #region GL Master, Detail

            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);


            if (p_DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice)
            {
                decimal Sales = p_Credit + p_Discount_Amount - p_Less_Discount_Amount;
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales, p_Distributor_Id, Sales, 0, p_Ledger_Date, p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, Sales, p_Ledger_Date, p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale", 0, Constants.Document_SaleInvoice);


                LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                    , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_SaleInvoice
                    , p_MANUAL_DOCUMENT_NO, "Sale Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Sale", mTransaction, mConnection);
                //decimal Sales = p_Credit + p_Discount_Amount;
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales, 0, Sales, "Sale Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, Sales, 0, "Sale Voucher", mTransaction, mConnection);
                
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchases, p_Debit, 0, "Sale (Consumptions) Voucher", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, 0, p_Debit, "Sale (Consumptions) Voucher", mTransaction, mConnection);
                
                hfVoucher_No.Value = VoucherNo2;
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
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        DoEmptyTextBox();
        PanelSearch.Visible = true;
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
        long itemlogid = long.Parse(ProID);
        var q = (from k in db.tbl_ITEM_LOG select k).Where(x => x.ITEM_LOG_ID == itemlogid).FirstOrDefault();
        int ItemID = q.ItemID;
        //PurchaseDetailLogics Ser = new PurchaseDetailLogics();
        ItemLogics Ser2 = new ItemLogics();
        //var data = Ser.Get(Convert.ToInt32(ProID));
        //int ITEM_ID = data.ITEM_INFO_ID;
        decimal Consumption = decimal.Parse(q.ItemAmount.ToString());
        var dataa = Ser2.Get(Convert.ToInt32(ItemID));
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        if (!CheckDublicate(ItemID))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + dataa.ITEM_NAME + " is already exists.');", true);
            //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        int Ref_ID = q.TransactionID;
        var qry = (
                   //from k in db.tbl_PURCHASE_DETAIL
                   //join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                   from k in db.tbl_ITEM_LOG
                   join i in db.ITEM_INFORMATION on k.ItemID equals i.ITEM_INFO_ID
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
                       REF_ID = k.TransactionID,
                       REF_DETAIL_ID = k.TransactionDetailID,
                       ITEM_INFO_ID = i.ITEM_INFO_ID,
                       ITEM_NAME = i.ITEM_NAME,
                       p.PROJECT_NAME,
                       c.CATEGORY_NAME,
                       subc.SUB_CATEGORY_NAME,
                       MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
                       BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
                       STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
                       RefID = k.ITEM_LOG_ID,
                       k.TypeID,
                       k.IsActive
                   }
                    );
        var xy = (qry).Where(x => x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 2 && x.IsActive == true
        || x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 26 && x.IsActive == true).ToList();
        DataRow dr = PurchaseDetail.NewRow();
        dr["ITEM_INFO_ID"] = q.ItemID;
        dr["ITEM_INFO_NAME"] = xy.FirstOrDefault().ITEM_NAME + ", " + xy.FirstOrDefault().PROJECT_NAME + ", " + xy.FirstOrDefault().CATEGORY_NAME + ", " + xy.FirstOrDefault().SUB_CATEGORY_NAME + ", " + xy.FirstOrDefault().BLOCK_NAME + ", " + xy.FirstOrDefault().STREET_NAME + ", " + xy.FirstOrDefault().MEASURMENT;
        dr["PROJECT_ID"] = q.ProjectID;
        dr["CATEGORY_ID"] = q.CategoryID;
        dr["SUB_CATEGORY_ID"] = q.SubCategoryID;
        dr["SaleAmount"] = 0;
        dr["Discount"] = 0;
        dr["LessDiscount"] = 0;
        dr["CommissionType"] = 1;
        dr["COMMISSION_PER"] = 0;
        dr["COMMISSION_VALUE"] = 0;
        dr["Commission"] = 0;
        dr["Balance"] = 0;
        dr["TOKEN_AMOUNT"] = 0;
        dr["DOWN_PAYMENT"] = 0;
        dr["REF_ID"] = xy.FirstOrDefault().REF_ID;
        dr["Consumption"] = Consumption;
        dr["REF_DETAIL_ID"] = xy.FirstOrDefault().REF_DETAIL_ID;
        dr["RefID"] = xy.FirstOrDefault().RefID;
        PurchaseDetail.Rows.Add(dr);
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        HideGridColumns();
        int TotalSaleAmount = 0;
        int TotalDiscountAmount = 0;
        int TotalLessDiscountAmount = 0;
        int TotalBalanceAmount = 0;
        foreach (GridViewRow row in ASPxGridView1.Rows)
        {
            string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
            string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
            string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
            string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
            TotalSaleAmount += Convert.ToInt32(Math.Round(decimal.Parse(SaleAmount), 0));
            TotalDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(Discount), 0));
            TotalLessDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(LessDiscount), 0));
            TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(Balance), 0));
        }
        this.Session.Add("PurchaseDetail", PurchaseDetail);
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
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                    string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
                    Balance = decimal.Parse(dc.chkNull_0(SaleAmount)) + decimal.Parse(dc.chkNull_0(Discount));
                    dr["SaleAmount"] = SaleAmount;
                    dr["Discount"] = Discount;
                    dr["LessDiscount"] = LessDiscount;
                    dr["Balance"] = Balance.ToString();
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
        PanelSearch.Visible = false;
        PanelPerchase.Visible = true;
        panelgrid.Visible = false;
        CreatTable();
        DoEmptyTextBox();
        LoadItems("");
    }
    protected void PurchaseMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        PurchaseMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }
    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["SALE_MASTER_ID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleMasterLogics Ser = new SaleMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Sele could not update. Some payments are received against this sale.');", true);
        }
        else
        {
            ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
            ddlLocation.TextField = "DISTRIBUTOR_NAME";
            ddlLocation.ValueField = "DISTRIBUTOR_ID";
            ddlLocation.DataBind();
            ddlLocation.Value = Convert.ToString(data.DISTRIBUTOR_ID);

            LoadCategories();
            LoadSubCategories();

            ddlVendor.DataSource = DropDowns.CustomerDropDown();
            ddlVendor.TextField = "VENDOR_NAME";
            ddlVendor.ValueField = "VENDOR_ID";
            ddlVendor.DataBind();
            ddlVendor.Value = Convert.ToString(data.CUSTOMER_ID);
            
            txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            CreatTable();
            DataTable dt = LController.SelectBSI(data.SALE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 30);
            //DataTable dt_Item_Log = LController.SelectBSI(data.SALE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 32);
            foreach (DataRow gvr in dt.Rows)
            {
                PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
                PurchaseDetailUpdate = (DataTable)this.Session["PurchaseDetailUpdate"];
                DataRow dr = PurchaseDetail.NewRow();
                dr["ITEM_INFO_ID"] = gvr["ITEM_INFO_ID"];
                dr["ITEM_INFO_NAME"] = gvr["ITEM_INFO_NAME"];
                dr["SaleAmount"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["SaleAmount"].ToString())), 0);
                dr["Discount"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Discount"].ToString())), 0);
                dr["LessDiscount"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["LessDiscount"].ToString())), 0);
                decimal Commission_Per = decimal.Parse(dc.chkNull_0(gvr["COMMISSION_PER"].ToString()));
                if (Math.Round(Commission_Per, 0) != 0)
                {
                    dr["COMMISSION_VALUE"] = "0";
                    decimal amount = Convert.ToDecimal(dc.chkNull_0(gvr["Balance"].ToString())) / 100 * decimal.Parse(dc.chkNull_0(gvr["COMMISSION_PER"].ToString()));
                    dr["Commission"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["COMMISSION_PER"].ToString())), 0);
                    dr["COMMISSION_VALUE"] = Convert.ToString(Math.Round(amount, 2));
                    dr["CommissionType"] = 1;
                }
                else
                {
                    dr["CommissionType"] = 2;
                    dr["Commission"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["COMMISSION_VALUE"].ToString())), 0);
                    dr["COMMISSION_VALUE"] = Convert.ToDecimal(gvr["COMMISSION_VALUE"]);
                }

                //dr["COMMISSION_PER"] = gvr["COMMISSION_PER"];
                //dr["COMMISSION_VALUE"] = gvr["COMMISSION_VALUE"];
                //dr["Commission"] = gvr["Commission"];
                dr["Balance"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["Balance"].ToString())), 0);
                dr["TOKEN_AMOUNT"] = gvr["TOKEN_AMOUNT"];
                dr["DOWN_PAYMENT"] = gvr["DOWN_PAYMENT"];
                dr["PROJECT_ID"] = gvr["PROJECT_ID"];
                dr["CATEGORY_ID"] = gvr["CATEGORY_ID"];
                dr["SUB_CATEGORY_ID"] = gvr["SUB_CATEGORY_ID"];
                dr["REF_ID"] = gvr["REF_ID"];
                dr["RefID"] = gvr["RefID"]; 
                int Item_ID = int.Parse(gvr["ITEM_INFO_ID"].ToString());
                int Ref_ID = int.Parse(gvr["REF_ID"].ToString());
                dr["REF_DETAIL_ID"] = int.Parse(gvr["REF_DETAIL_ID"].ToString());
                var conm = (from k in db.tbl_PURCHASE_DETAIL
                            select new { k.BALANCE, k.ITEM_INFO_ID, k.PURCHASE_MASTER_ID }).Where(x => x.PURCHASE_MASTER_ID == Ref_ID && x.ITEM_INFO_ID == Item_ID).ToList();
                dr["Consumption"] = decimal.Parse(conm.FirstOrDefault().BALANCE.ToString());
                PurchaseDetail.Rows.Add(dr);

                DataRow drUp = PurchaseDetailUpdate.NewRow();
                drUp["ITEM_INFO_ID"] = gvr["ITEM_INFO_ID"];
                drUp["ITEM_INFO_NAME"] = gvr["ITEM_INFO_NAME"];
                drUp["PROJECT_ID"] = gvr["PROJECT_ID"];
                drUp["CATEGORY_ID"] = gvr["CATEGORY_ID"];
                drUp["SUB_CATEGORY_ID"] = gvr["SUB_CATEGORY_ID"];
                drUp["REF_ID"] = gvr["REF_ID"];
                drUp["REF_DETAIL_ID"] = int.Parse(gvr["REF_DETAIL_ID"].ToString());
                drUp["RefID"] = gvr["RefID"];
                var conmp = (from k in db.tbl_PURCHASE_DETAIL
                            select new { k.BALANCE, k.ITEM_INFO_ID, k.PURCHASE_MASTER_ID }).Where(x => x.PURCHASE_MASTER_ID == Ref_ID && x.ITEM_INFO_ID == Item_ID).ToList();
                drUp["Consumption"] = decimal.Parse(conmp.FirstOrDefault().BALANCE.ToString());
                PurchaseDetailUpdate.Rows.Add(drUp);

            }
            txtRemarks.Text = data.REMARKS;
            //txtSR_NO.Text = data.SR_NO;
            ASPxGridView1.DataSource = PurchaseDetail;
            ASPxGridView1.DataBind();
            HideGridColumns();
            int TotalSaleAmount = 0;
            int TotalDiscountAmount = 0;
            int TotalLessDiscountAmount = 0;
            int TotalBalanceAmount = 0;
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
                string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                TotalSaleAmount += Convert.ToInt32(Math.Round(decimal.Parse(SaleAmount), 0));
                TotalDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(Discount), 0));
                TotalLessDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(LessDiscount), 0));
                TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(Balance), 0));
            }
            this.Session.Add("PurchaseDetail", PurchaseDetail);
            
            this.Session.Add("PurchaseDetailUpdate", PurchaseDetailUpdate);
            SetTotals();
            LoadItems("");
            Session.Add("SALE_MASTER_ID", data.SALE_MASTER_ID);
            PanelSearch.Visible = false;
            mastergrid.Visible = false;
            PanelPerchase.Visible = true;
            panelgrid.Visible = false;
            PanelItems.Visible = true;
            btnSave.Text = "Update";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>Calculation();</script>", false);
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        bool flag = false;
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleMasterLogics Ser = new SaleMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        var dController = new DistributorController();
        var dtt = dController.SelectPurchaseDetailPaymentInfo(Constants.Journal_Voucher, data.CUSTOMER_ID, data.SALE_MASTER_ID, Constants.Document_Invoice, 0, 2);
        if (dtt.Rows.Count > 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected Sale could not be deleted. Some payments are received against this sale.');", true);
        }
        else
        {
            try
            {

                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.CUSTOMER_ID && x.DOCUMENT_NO == data.SALE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleInvoice && x.IS_DELETED == 0).ToList();
                if (!Active)
                {
                    var qry = (from k in db.tbl_ITEM_LOG select k).Where(x => x.TransactionID == data.SALE_MASTER_ID && x.TypeID == Constants.Document_SaleInvoice && x.IsActive == true).ToList();
                    
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_MASTER where SALE_MASTER_ID =" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("DELETE tbl_SALE_DETAIL where SALE_MASTER_ID =" + Convert.ToInt32(ProID), con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        foreach (var item in qry)
                        {
                            int RefID = Convert.ToInt32(item.RefID);
                            int ItemLogID = Convert.ToInt32(item.ITEM_LOG_ID);
                            using (SqlCommand command4 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 1 where ITEM_LOG_ID =" + Convert.ToInt32(RefID), con))
                            {
                                command4.ExecuteNonQuery();
                            }
                            using (SqlCommand command4 = new SqlCommand("DELETE tbl_ITEM_LOG where ITEM_LOG_ID =" + Convert.ToInt32(ItemLogID), con))
                            {
                                command4.ExecuteNonQuery();
                            }
                        }
                        if (q.Count > 0)
                        {
                            string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                            using (SqlCommand command3 = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.CUSTOMER_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.SALE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_SaleInvoice + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                            {
                                command3.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(data.DISTRIBUTOR_ID) + "'", con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(data.DISTRIBUTOR_ID) + "'", con))
                            {
                                command2.ExecuteNonQuery();
                            }
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }
                //}
            }
            catch (Exception ex) { }
        }
        CreatTable();
        DoEmptyTextBox();
        LoadItems("");
        LoadGrid("");
    }
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadItems(txtSearch.Text);
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int ItemCount = Constants.IntNullValue;
        decimal Consumption = 0;
        foreach (GridViewRow r in grid.Rows)
        {
            CheckBox ChkChecked = (CheckBox)r.Cells[0].FindControl("ChbIsAssigned");
            if (ChkChecked.Checked == true)
            {
                ItemCount++;
                long itemlogid = long.Parse(r.Cells[11].Text);
                DbCalls db = new DbCalls();
                //var ProID = ((LinkButton)sender).CommandArgument;
                //long itemlogid = long.Parse(ProID);
                var q = (from k in db.tbl_ITEM_LOG select k).Where(x => x.ITEM_LOG_ID == itemlogid).FirstOrDefault();
                int ItemID = q.ItemID;
                if (q.TypeID == 2 || q.TypeID == 7)
                {
                    Consumption = decimal.Parse(q.ItemAmount.ToString());
                }
                else
                {
                    var qq = (from k in db.tbl_ITEM_LOG select k).Where(x => x.ItemID == ItemID && x.TypeID == 2 || x.ItemID == ItemID && x.TypeID == 7).FirstOrDefault();
                    Consumption = decimal.Parse(qq.ItemAmount.ToString());
                }
                ItemLogics Ser2 = new ItemLogics();
                var dataa = Ser2.Get(Convert.ToInt32(ItemID));
                PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
                if (!CheckDublicate(ItemID))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + dataa.ITEM_NAME + " is already exists.');", true);
                    return;
                }
                int Ref_ID = q.TransactionID;
                var qry = (
                           from k in db.tbl_ITEM_LOG
                           join i in db.ITEM_INFORMATION on k.ItemID equals i.ITEM_INFO_ID
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
                               REF_ID = k.TransactionID,
                               REF_DETAIL_ID = k.TransactionDetailID,
                               ITEM_INFO_ID = i.ITEM_INFO_ID,
                               ITEM_NAME = i.ITEM_NAME,
                               p.PROJECT_NAME,
                               c.CATEGORY_NAME,
                               subc.SUB_CATEGORY_NAME,
                               MEASURMENT = m.MEASURMENT1 == null ? "N/A" : m.MEASURMENT1,
                               BLOCK_NAME = b.BLOCK_NAME == null ? "N/A" : b.BLOCK_NAME,
                               STREET_NAME = s.STREET_NAME == null ? "N/A" : s.STREET_NAME,
                               RefID = k.ITEM_LOG_ID,
                               k.TypeID,
                               k.IsActive
                           }
                            );
                var xy = (qry).Where(x => x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 2 && x.IsActive == true
                || x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 26 && x.IsActive == true
                || x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 7 && x.IsActive == true
                || x.ITEM_INFO_ID == q.ItemID && x.REF_ID == Ref_ID && x.TypeID == 14 && x.IsActive == true).ToList();
                DataRow dr = PurchaseDetail.NewRow();
                dr["ITEM_INFO_ID"] = q.ItemID;
                dr["ITEM_INFO_NAME"] = xy.FirstOrDefault().ITEM_NAME + ", " + xy.FirstOrDefault().PROJECT_NAME + ", " + xy.FirstOrDefault().CATEGORY_NAME + ", " + xy.FirstOrDefault().SUB_CATEGORY_NAME + ", " + xy.FirstOrDefault().BLOCK_NAME + ", " + xy.FirstOrDefault().STREET_NAME + ", " + xy.FirstOrDefault().MEASURMENT;
                dr["PROJECT_ID"] = q.ProjectID;
                dr["CATEGORY_ID"] = q.CategoryID;
                dr["SUB_CATEGORY_ID"] = q.SubCategoryID;
                dr["SaleAmount"] = "";
                dr["Discount"] = 0;
                dr["LessDiscount"] = 0;
                dr["CommissionType"] = 1;
                dr["COMMISSION_PER"] = 0;
                dr["COMMISSION_VALUE"] = 0;
                dr["Commission"] = "";
                dr["Balance"] = "";
                dr["TOKEN_AMOUNT"] = 0;
                dr["DOWN_PAYMENT"] = 0;
                dr["REF_ID"] = xy.FirstOrDefault().REF_ID;
                dr["Consumption"] = Consumption;
                dr["REF_DETAIL_ID"] = xy.FirstOrDefault().REF_DETAIL_ID;
                dr["RefID"] = xy.FirstOrDefault().RefID;
                PurchaseDetail.Rows.Add(dr);
                ASPxGridView1.DataSource = PurchaseDetail;
                ASPxGridView1.DataBind();
                HideGridColumns();
                int TotalSaleAmount = 0;
                int TotalDiscountAmount = 0;
                int TotalLessDiscountAmount = 0;
                int TotalBalanceAmount = 0;
                foreach (GridViewRow row in ASPxGridView1.Rows)
                {
                    string SaleAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtSaleAmount")).Text;
                    string Discount = ((System.Web.UI.WebControls.TextBox)row.Cells[4].FindControl("txtDiscount")).Text;
                    string LessDiscount = ((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtLessDiscount")).Text;
                    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                    TotalSaleAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(SaleAmount)), 0));
                    TotalDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(Discount)), 0));
                    TotalLessDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(LessDiscount)), 0));
                    TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(Balance)), 0));
                }
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

    private void showReport(int p_Invoice_ID)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();
        DataSet ds = null;
        {
            DataTable dt = DPrint.SelectReportTitle(int.Parse(ddlLocation.SelectedItem.Value.ToString()));
            CrpInvoice CrpReport = new CrpInvoice();
            ds = RptCustCtl.GetInvoices(p_Invoice_ID, int.Parse(ddlVendor.SelectedItem.Value.ToString()),
                      DateTime.Parse(txtPurchaseDate.Text + " 00:00:00"), DateTime.Parse(txtPurchaseDate.Text + " 23:59:59"), null, 5);
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtPurchaseDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtPurchaseDate.Text));
            CrpReport.SetParameterValue("Location", ddlLocation.SelectedItem.Text);
            CrpReport.SetParameterValue("Net_Amount", "Net Receivable");
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            Session.Add("CrpReport", CrpReport);
            Session.Add("ReportType", 0);
            const string url = "'Default.aspx'";
            const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }
    protected void BtnMasterGridSearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearchMasterGrid.Text);
    }
}