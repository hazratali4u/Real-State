using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
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

public partial class Forms_frmOpeningInventory : System.Web.UI.Page
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
            //PanelItems.Visible = true;
            //HideGridColumns();

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
        DbCalls db = new DbCalls();
        var qry = (from k in db.VENDORS
                   select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
        //ddlVendor.DataSource = DropDowns.VendorDropDown();
        ddlVendor.DataSource = qry;
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
      from i in db.ITEM_INFORMATION
          //join il in db.tbl_ITEM_LOG on i.ITEM_INFO_ID equals il.ItemID
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
          PurchaseAmount = ""
          

          //Il.TypeID,// = Il.TypeID == null ? 0 : Il.TypeID,
          //Il.IsActive //= Il.IsActive == null ? true : Il.IsActive
      }
  );
        var xy = (query2).Where(x => x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID).ToList();
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, CATEGORYID, SUBCATEGORYID, null, 28);
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                xy.RemoveAll(x => x.ITEM_INFO_ID == int.Parse(dr["ITEM_INFO_ID"].ToString()));
            }
        }
        //foreach (GridViewRow row in ASPxGridView1.Rows)
        //{
        //    if (row.RowType == DataControlRowType.DataRow)
        //    {
        //        xy.RemoveAll(x => x.ITEM_INFO_ID == int.Parse(row.Cells[0].Text));
        //    }
        //}
        DataTable PurchaseDetail = (DataTable)Session["PurchaseDetail"];
        foreach (var item in xy)
        {
            DataRow dr = PurchaseDetail.NewRow();
            dr["ITEM_INFO_ID"] = item.ITEM_INFO_ID;
            dr["ITEM_NAME"] = item.ITEM_NAME;
            dr["PROJECT_ID"] = item.PROJECT_ID;
            dr["PROJECT_NAME"] = item.PROJECT_NAME;
            dr["OPEN_FROM_REGISTRATION"] = item.OPEN_FROM_REGISTRATION;
            dr["PurchaseAmount"] = item.PurchaseAmount;
            dr["TOWER_NO"] = item.TOWER_NO;
            dr["FLOOR_NO"] = item.FLOOR_NO;
            dr["MEASURMENT"] = item.MEASURMENT;
            dr["BLOCK_NAME"] = item.BLOCK_NAME;
            dr["STREET_NAME"] = item.STREET_NAME;
            dr["PRECENT_NAME"] = item.PRECENT_NAME;
            dr["PRECENTSTREET_NAME"] = item.PRECENTSTREET_NAME;
            dr["SECTOR_NAME"] = item.SECTOR_NAME;
            dr["CATEGORY_ID"] = item.CATEGORY_ID;
            dr["SUB_CATEGORY_ID"] = item.SUB_CATEGORY_ID;
            PurchaseDetail.Rows.Add(dr);
        }
        CreateSearchTable();
        foreach (var row in xy)
        {
            DataRow dr = SearchTable.NewRow();
            dr["ITEM_INFO_ID"] = row.ITEM_INFO_ID;
            dr["PROJECT_ID"] = row.PROJECT_ID;
            dr["PROJECT_NAME"] = row.PROJECT_NAME;
            dr["PurchaseAmount"] = row.PurchaseAmount;
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
        //ASPxGridView1.DataSource = PurchaseDetail;
        //ASPxGridView1.DataBind();
        if (!ItemID.Equals(""))
        {
            SearchTable.DefaultView.RowFilter = "PROJECT_NAME LIKE '%" + txtSearch.Text + "%'  OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR MEASURMENT LIKE '%" + txtSearch.Text + "%' OR BLOCK_NAME LIKE '%" + txtSearch.Text + "%' OR STREET_NAME LIKE '%" + txtSearch.Text + "%' OR PRECENT_NAME LIKE '%" + txtSearch.Text + "%' OR PRECENTSTREET_NAME LIKE '%" + txtSearch.Text + "%' OR OPEN_FROM_REGISTRATION LIKE '%" + txtSearch.Text + "%' OR TOWER_NO LIKE '%" + txtSearch.Text + "%' OR FLOOR_NO LIKE '%" + txtSearch.Text + "%'";
            if (SearchTable.Rows.Count > 0)
            {
                ASPxGridView1.PageIndex = 0;
            }
            ASPxGridView1.DataSource = SearchTable;
            ASPxGridView1.DataBind();
        }
        else
        {
            ASPxGridView1.DataSource = SearchTable;
            ASPxGridView1.DataBind();
        }
        HideGridColumns();
    }
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_NAME", typeof(string));
        PurchaseDetail.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetail.Columns.Add("PROJECT_NAME", typeof(string));
        PurchaseDetail.Columns.Add("OPEN_FROM_REGISTRATION", typeof(string));
        PurchaseDetail.Columns.Add("PurchaseAmount", typeof(string));
        PurchaseDetail.Columns.Add("TOWER_NO", typeof(string));
        PurchaseDetail.Columns.Add("FLOOR_NO", typeof(string));
        PurchaseDetail.Columns.Add("MEASURMENT", typeof(string));
        PurchaseDetail.Columns.Add("BLOCK_NAME", typeof(string));
        PurchaseDetail.Columns.Add("STREET_NAME", typeof(string));
        PurchaseDetail.Columns.Add("PRECENT_NAME", typeof(string));
        PurchaseDetail.Columns.Add("PRECENTSTREET_NAME", typeof(string));
        PurchaseDetail.Columns.Add("SECTOR_NAME", typeof(string));
        PurchaseDetail.Columns.Add("CATEGORY_ID", typeof(int));
        PurchaseDetail.Columns.Add("SUB_CATEGORY_ID", typeof(int));
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
        SearchTable.Columns.Add("PurchaseAmount", typeof(string));
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
        //PanelItems.Visible = false;
        //HideGridColumns();
        txtTotalBalance.Text = "";
        txtTotalCommission.Text = "";
        txtTotalDiscountAmount.Text = "";
        txtTotalDownPayment.Text = "";
        txtTotalPurchaseAmount.Text = "";
        txtTotalTokenAmount.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";


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
        if (ASPxGridView1.Rows.Count > 0)
        {
            ASPxGridView1.FooterRow.Cells[0].Visible = false;
            ASPxGridView1.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[2].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[3].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[5].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[6].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[8].Visible = true;
            ASPxGridView1.FooterRow.Cells[9].Visible = true;
            ASPxGridView1.FooterRow.Cells[10].Visible = true;
            ASPxGridView1.FooterRow.Cells[11].Text = "Total";
            ASPxGridView1.FooterRow.Cells[11].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[11].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[12].Visible = true;
            ASPxGridView1.FooterRow.Cells[12].Font.Bold = true;
            ASPxGridView1.FooterRow.Cells[12].HorizontalAlign = HorizontalAlign.Right;
            ASPxGridView1.FooterRow.Cells[12].Visible = true;
            ASPxGridView1.FooterRow.Cells[13].Visible = false;
            ASPxGridView1.FooterRow.Cells[14].Visible = false;
            ASPxGridView1.FooterRow.Cells[15].Visible = false;
        }
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
            System.Web.UI.WebControls.CheckBox ChChecked = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
            if (ChChecked.Checked == true)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    decimal COMMISSION_PER;
                    decimal COMMISSION_VALUE;
                    decimal Commission;
                    TotalPurchaseAmount += Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text);
                    TotalDiscount += 0; //Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtDiscount")).Text);
                    int CommissionType = 2; //Val(((DropDownList)row.FindControl("ddlCommissionType")).SelectedValue);
                    if (CommissionType == 1)
                    {
                        COMMISSION_PER = decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text);
                        decimal TCommission = (decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text) / 100) * decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text);
                        COMMISSION_VALUE = 0;
                        Commission = TCommission;
                    }
                    else
                    {
                        COMMISSION_PER = 0;
                        decimal TCommission = 0; //decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text);
                        COMMISSION_VALUE = TCommission;
                        Commission = TCommission;
                    }
                    TotalCommission += Convert.ToDecimal(Commission);
                    TotalBalance += Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text); //Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text);
                    TotalTokenAmount += 0; //Convert.ToDecimal(row.Cells[9].Text);
                    TotalDownPayment += 0; //Convert.ToDecimal(row.Cells[10].Text);
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
            var qry = (from k in db.tbl_PURCHASE_MASTER
                           //join pd in db.tbl_PURCHASE_DETAIL on k.PURCHASE_MASTER_ID equals pd.PURCHASE_MASTER_ID
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       //join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
                       //join i in db.ITEM_INFORMATION on pd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       //join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
                       //join s in db.STREET on i.STREET_ID equals s.STREET_ID
                       join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
                       select new
                       {
                           d.DISTRIBUTOR_NAME,
                           k.DATE,
                           //ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME,
                           v.VENDOR_NAME,
                           k.PURCHASE_MASTER_ID,
                           k.TOTAL_PURCHASE_AMOUNT,
                           k.TOTAL_DISCOUNT,
                           BALANCE = k.TOTAL_PURCHASE_AMOUNT + k.TOTAL_DISCOUNT,
                           Active = k.IS_DELETED == false ? "Active" : "Inactive",
                           k.IS_DELETED,
                           k.TYPE_ID
                       }).Where(x => x.Active == "Active" && x.TYPE_ID == 7).ToList();
            PurchaseMaster.DataSource = qry;
            PurchaseMaster.DataBind();
            int total = 0;
            int discount = 0;
            int balance = 0;
            foreach (GridViewRow row in PurchaseMaster.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[4].Text), 0));
                total += amount;
                int Discount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                discount += Discount;
                int Balance = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                balance += Balance;
            }
            if (qry.Count() > 0)
            {
                PurchaseMaster.FooterRow.Cells[4].Text = string.Format("{0:n2}", total);
                PurchaseMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", discount);
                PurchaseMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", balance);
                PurchaseMaster.FooterRow.Cells[3].Text = "Total";
                PurchaseMaster.FooterRow.Cells[3].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[4].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[5].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                PurchaseMaster.FooterRow.Cells[6].Font.Bold = true;
                PurchaseMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
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
    public bool CheckInvoice()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in ASPxGridView1.Rows)
        {
            System.Web.UI.WebControls.CheckBox chRelized = (System.Web.UI.WebControls.CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
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
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ddlVendor.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        if (CheckInvoice())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an item first');", true);
            return;
        }
        SetTotals();
        PurchaseMasterLogics pml = new PurchaseMasterLogics();
        tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
        pm.PROJECT_ID = 0; //Convert.ToInt32(ddlProject.SelectedValue);
        pm.CATEGORY_ID = 0; //Convert.ToInt32(ddlCategory.SelectedValue);
        pm.SUBCATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedValue);
        pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedItem.Value);
        pm.COMMISSIONAGENT_ID = 0; //Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtTotalPurchaseAmount.Text);
        pm.TOTAL_DISCOUNT = 0; //Convert.ToDecimal(txtTotalDiscountAmount.Text);
        pm.TOTAL_TOKEN_AMOUNT = 0; //Convert.ToDecimal(txtTotalTokenAmount.Text);
        pm.TOTAL_DOWN_PAYMENT = 0; //Convert.ToDecimal(txtTotalDownPayment.Text);
        pm.TOTAL_BALANCE = Convert.ToDecimal(txtTotalPurchaseAmount.Text);
        pm.TOTAL_COMMISSION = 0; //Convert.ToDecimal(txtTotalCommission.Text);
        pm.REMARKS = txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        if (Session["PurchasMasterID"] != null)
        {
            string VOUCHER_NO = Session["OpeningVoucherNo"].ToString();
            pm.TYPE_ID = 7;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pm.PURCHASE_MASTER_ID = Convert.ToInt32(Session["PurchasMasterID"]);
            pml.Update(pm);
            DbCalls db = new DbCalls();
            int PURCHASE_MASTER_ID = pm.PURCHASE_MASTER_ID;
            PurchaseDetailLogics pdl = new PurchaseDetailLogics();
            tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_DETAIL WHERE PURCHASE_MASTER_ID=" + PURCHASE_MASTER_ID, con))
                {
                    command.ExecuteNonQuery();
                }
                using (SqlCommand command1 = new SqlCommand("DELETE tbl_ITEM_LOG WHERE TransactionID=" + PURCHASE_MASTER_ID + "AND TypeID = 7", con))
                {
                    command1.ExecuteNonQuery();
                }
                using (SqlCommand command2 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                {
                    command2.ExecuteNonQuery();
                }
                using (SqlCommand command3 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                {
                    command3.ExecuteNonQuery();
                }
                con.Close();
            }
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                System.Web.UI.WebControls.CheckBox ChChecked = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
                if (ChChecked.Checked == true)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        decimal COMMISSION_PER;
                        decimal COMMISSION_VALUE;
                        decimal Commission;
                        int ITEM_INFO_ID = Convert.ToInt32(row.Cells[1].Text);
                        string PurchaseAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text;
                        string Discount = "0.00";//((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtDiscount")).Text;
                        int CommissionType = 2; //Val(((DropDownList)row.FindControl("ddlCommissionType")).SelectedValue);
                        if (CommissionType == 1)
                        {
                            COMMISSION_PER = decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                            decimal TCommission = (decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text)) / 100) * decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                            COMMISSION_VALUE = 0;
                            Commission = TCommission;
                        }
                        else
                        {
                            COMMISSION_PER = 0;
                            decimal TCommission = 0;//decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text);
                            COMMISSION_VALUE = TCommission;
                            Commission = TCommission;
                        }
                        string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text;//((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                        decimal TOKEN_AMOUNT = 0; //Convert.ToDecimal(row.Cells[9].Text);
                        decimal DOWN_PAYMENT = 0; //Convert.ToDecimal(row.Cells[10].Text);

                        int Project_ID = Convert.ToInt32(row.Cells[13].Text);
                        int Category_ID = Convert.ToInt32(row.Cells[14].Text);
                        int SubCategory_ID = Convert.ToInt32(row.Cells[15].Text);

                        pd.PURCHASE_MASTER_ID = PURCHASE_MASTER_ID;
                        pd.ITEM_INFO_ID = ITEM_INFO_ID;
                        pd.PROJECT_ID = Project_ID;
                        pd.CATEGORY_ID = Category_ID;
                        pd.SUB_CATEGORY_ID = SubCategory_ID;
                        pd.PURCHASE_AMOUNT = Convert.ToDecimal(PurchaseAmount);
                        pd.DISCOUNT = Convert.ToDecimal(Discount);
                        pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                        pd.DOWN_PAYMENT = DOWN_PAYMENT;
                        pd.COMMISSION_PERCENT = COMMISSION_PER;
                        pd.COMMISSION_VALUE = COMMISSION_VALUE;
                        pd.COMMISSION = Commission;
                        pd.BALANCE = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        pd.DEBIT_AMOUNT = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
                        pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        pd.TIME_STEMP = DateTime.Now;
                        pd.IS_DELETED = Convert.ToBoolean(0);
                        pdl.Save(pd);

                        Item_Log_Logics Log = new Item_Log_Logics();
                        tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                        ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                        ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                        ILog.TransactionID = PURCHASE_MASTER_ID;
                        ILog.TransactionDetailID = pd.PURCHASE_DETAIL_ID;
                        ILog.TypeID = 7;
                        ILog.ProjectID = Project_ID;
                        ILog.CategoryID = Category_ID;
                        ILog.SubCategoryID = SubCategory_ID;
                        ILog.ItemID = ITEM_INFO_ID;
                        ILog.ItemAmount = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        ILog.UserID = Convert.ToInt32(Session["UserID"]);
                        ILog.IsActive = Convert.ToBoolean(1);
                        ILog.TimeStamp = DateTime.Now;
                        Log.Save(ILog);

                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command2 = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where PURCHASE_DETAIL_ID=" + pd.PURCHASE_DETAIL_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }


                    }
                }
            }
            

            //var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == pm.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Opening && x.IS_DELETED == 0).ToList();
            //if (q.Count > 0)
            //{
            //    string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
            //    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            //    {
            //        con.Open();
            //        using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Opening + "and PAYMENT_MODE=" + Constants.CreditSale, con))
            //        {
            //            command.ExecuteNonQuery();
            //        }
            //        using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
            //        {
            //            command1.ExecuteNonQuery();
            //        }
            //        using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
            //        {
            //            command2.ExecuteNonQuery();
            //        }
            //        con.Close();
            //    }
            //}
            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + " / " + txtRemarks.Text + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            string Vou_No = LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
               , pm.DATE, pm.VENDOR_ID, Constants.Document_Opening
               , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
               , Convert.ToString(pm.PURCHASE_MASTER_ID)
               , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_MASTER set VOUCHER_NO = '" + Vou_No + "' where PURCHASE_MASTER_ID=" + Convert.ToInt64(pm.PURCHASE_MASTER_ID) , con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            Session["DBT_Amount"] = null;
            Session["BAL_Amount"] = null;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PurchasMasterID"] = null;
            //mastergrid.Visible = true;
        }
        else
        {
            pm.TYPE_ID = 7;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pml.Save(pm);
            int PURCHASE_MASTER_ID = pm.PURCHASE_MASTER_ID;
            PurchaseDetailLogics pdl = new PurchaseDetailLogics();
            tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();
            foreach (GridViewRow row in ASPxGridView1.Rows)
            {
                System.Web.UI.WebControls.CheckBox ChChecked = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("ChbIsAssigned");
                if (ChChecked.Checked == true)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        decimal COMMISSION_PER;
                        decimal COMMISSION_VALUE;
                        decimal Commission;
                        int ITEM_INFO_ID = Convert.ToInt32(row.Cells[1].Text);
                        string PurchaseAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text;
                        string Discount = "0.00";//((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtDiscount")).Text;
                        int CommissionType = 2; //Val(((DropDownList)row.FindControl("ddlCommissionType")).SelectedValue);
                        if (CommissionType == 1)
                        {
                            COMMISSION_PER = decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                            decimal TCommission = (decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text)) / 100) * decimal.Parse(dc.chkNull_0(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text));
                            COMMISSION_VALUE = 0;
                            Commission = TCommission;
                        }
                        else
                        {
                            COMMISSION_PER = 0;
                            decimal TCommission = 0;//decimal.Parse(((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text);
                            COMMISSION_VALUE = TCommission;
                            Commission = TCommission;
                        }
                        string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[12].FindControl("txtPurchaseAmount")).Text;//((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                        decimal TOKEN_AMOUNT = 0; //Convert.ToDecimal(row.Cells[9].Text);
                        decimal DOWN_PAYMENT = 0; //Convert.ToDecimal(row.Cells[10].Text);

                        int Project_ID = Convert.ToInt32(row.Cells[13].Text);
                        int Category_ID = Convert.ToInt32(row.Cells[14].Text);
                        int SubCategory_ID = Convert.ToInt32(row.Cells[15].Text);

                        pd.PURCHASE_MASTER_ID = PURCHASE_MASTER_ID;
                        pd.ITEM_INFO_ID = ITEM_INFO_ID;
                        pd.PROJECT_ID = Project_ID;
                        pd.CATEGORY_ID = Category_ID;
                        pd.SUB_CATEGORY_ID = SubCategory_ID;
                        pd.PURCHASE_AMOUNT = Convert.ToDecimal(PurchaseAmount);
                        pd.DISCOUNT = Convert.ToDecimal(Discount);
                        pd.TOKEN_AMOUNT = TOKEN_AMOUNT;
                        pd.DOWN_PAYMENT = DOWN_PAYMENT;
                        pd.COMMISSION_PERCENT = COMMISSION_PER;
                        pd.COMMISSION_VALUE = COMMISSION_VALUE;
                        pd.COMMISSION = Commission;
                        pd.BALANCE = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        pd.DEBIT_AMOUNT = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
                        pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                        pd.TIME_STEMP = DateTime.Now;
                        pd.IS_DELETED = Convert.ToBoolean(0);
                        pdl.Save(pd);

                        Item_Log_Logics Log = new Item_Log_Logics();
                        tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                        ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                        ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                        ILog.TransactionID = PURCHASE_MASTER_ID;
                        ILog.TransactionDetailID = pd.PURCHASE_DETAIL_ID;
                        ILog.TypeID = 7;
                        ILog.ProjectID = Project_ID;
                        ILog.CategoryID = Category_ID;
                        ILog.SubCategoryID = SubCategory_ID;
                        ILog.ItemID = ITEM_INFO_ID;
                        ILog.ItemAmount = Convert.ToDecimal(PurchaseAmount) + Convert.ToDecimal(Discount);
                        ILog.UserID = Convert.ToInt32(Session["UserID"]);
                        ILog.IsActive = Convert.ToBoolean(1);
                        ILog.TimeStamp = DateTime.Now;
                        Log.Save(ILog);

                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command2 = new SqlCommand("UPDATE tbl_PURCHASE_DETAIL SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where PURCHASE_DETAIL_ID=" + pd.PURCHASE_DETAIL_ID, con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                }
            }

            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + " / " + txtRemarks.Text + " / " + ddlVendor.SelectedItem.Text; // + ", " + ItemInfo;
            string Vou_No = LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
                , pm.DATE, pm.VENDOR_ID, Constants.Document_Opening
                , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
                , Convert.ToString(pm.PURCHASE_MASTER_ID)
                , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("update tbl_PURCHASE_MASTER set VOUCHER_NO = '" + Vou_No + "' where PURCHASE_MASTER_ID=" + Convert.ToInt64(pm.PURCHASE_MASTER_ID), con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadGrid("");
        DoEmptyTextBox();
        LoadItems("");
        mastergrid.Visible = true;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
    }
    private string LedgerEntries(int p_Distributor_Id
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

            if (p_DOCUMENT_TYPE_ID == Constants.Document_Opening)
            {
                //LController.PostingPrinvipalInvoiceAccount(
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Opening-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Opening-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);

                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);

                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);

                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Opening", 0, Constants.Document_Opening);

                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Opening
                        , p_MANUAL_DOCUMENT_NO, "Opening Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Opening", mTransaction, mConnection);
                    decimal Purchases = p_Credit + p_Discount_Amount;
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, Purchases, 0, "Opening Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Opening_Balances, 0, Purchases, "Opening Voucher", mTransaction, mConnection);

                    
                }
            }
            
            #endregion
            mTransaction.Commit();
            return VoucherNo2;
        }
        catch (Exception exp)
        {
            ExceptionPublisher.PublishException(exp);
            mTransaction.Rollback();
            return "";
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
        dr["PurchaseAmount"] = 0;
        dr["Discount"] = 0;
        dr["CommissionType"] = 1;
        dr["COMMISSION_PER"] = 0;
        dr["COMMISSION_VALUE"] = 0;
        dr["Commission"] = 0;
        dr["Balance"] = 0;
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
        //PanelItems.Visible = true;
        SetTotals();
        LoadItems(ProID);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        var ProID = ((LinkButton)sender).CommandArgument;
        var query = PurchaseDetail.AsEnumerable().Where(r => r.Field<int>("ITEM_INFO_ID") == int.Parse(ProID));
        foreach (var row in query.ToList())
            row.Delete();
        ASPxGridView1.DataSource = PurchaseDetail;
        ASPxGridView1.DataBind();
        this.Session.Add("PurchaseDetail", PurchaseDetail);
        SetTotals();
        LoadItems("");
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

    protected void PurchaseMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        PurchaseMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["PurchasMasterID"] = null;
        Session["OpeningVoucherNo"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var dController = new DistributorController();

        DataTable dt = LController.SelectBSI(data.PURCHASE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 43);
        DataTable dtitem = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 29);
        if (dtitem.Rows.Count > 0)
        {
            foreach (DataRow dr in dtitem.Rows)
            {
                var query = dt.AsEnumerable().Where(r => r.Field<int>("ITEM_INFO_ID") == int.Parse(dr["ITEM_INFO_ID"].ToString()));
                foreach (var row in query.ToList())
                    row.Delete();
                dt.AcceptChanges();
            }
        }
        if (dt.Rows.Count > 0)
        {
            var dtt = dController.SelectPurchaseDetailPaymentInfo(Constants.Journal_Voucher, data.VENDOR_ID, data.PURCHASE_MASTER_ID, Constants.Document_Opening, 0, 1);
            if (dtt.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update. Some payments are issued against this purchase.');", true);
            }
            else
            {
                var IUD = (from k in db.tbl_PURCHASE_DETAIL select k).Where(x => x.PURCHASE_MASTER_ID == data.PURCHASE_MASTER_ID).ToList();

                ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
                ddlLocation.TextField = "DISTRIBUTOR_NAME";
                ddlLocation.ValueField = "DISTRIBUTOR_ID";
                ddlLocation.DataBind();
                ddlLocation.Value = Convert.ToString(data.DISTRIBUTOR_ID);

                LoadCategories();
                ddlCategory.Value = Convert.ToString(IUD.FirstOrDefault().CATEGORY_ID);

                LoadSubCategories();
                ddlSubCategory.Value = Convert.ToString(IUD.FirstOrDefault().SUB_CATEGORY_ID);

                LoadVendors();
                ddlVendor.Value = Convert.ToString(data.VENDOR_ID);
                txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
                CreatTable();
                LoadItems("");
                DataTable dt2 = LController.SelectBSI(data.PURCHASE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 34);
                foreach (DataRow gvr in dt2.Rows)
                {
                    PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
                    DataRow dr = PurchaseDetail.NewRow();
                    dr["ITEM_INFO_ID"] = gvr["ITEM_INFO_ID"];
                    dr["ITEM_NAME"] = gvr["ITEM_NAME"];
                    dr["PurchaseAmount"] = Math.Round(decimal.Parse(dc.chkNull_0(gvr["PurchaseAmount"].ToString())), 0);
                    dr["PROJECT_ID"] = int.Parse(gvr["PROJECT_ID"].ToString());
                    dr["PROJECT_NAME"] = gvr["PROJECT_NAME"];
                    dr["OPEN_FROM_REGISTRATION"] = gvr["OPEN_FROM_REGISTRATION"];
                    dr["TOWER_NO"] = gvr["TOWER_NO"];
                    dr["FLOOR_NO"] = gvr["FLOOR_NO"];
                    dr["MEASURMENT"] = gvr["MEASURMENT"];
                    dr["BLOCK_NAME"] = gvr["BLOCK_NAME"];
                    dr["STREET_NAME"] = gvr["STREET_NAME"];
                    dr["PRECENT_NAME"] = gvr["PRECENT_NAME"];
                    dr["PRECENTSTREET_NAME"] = gvr["PRECENTSTREET_NAME"];
                    dr["SECTOR_NAME"] = gvr["SECTOR_NAME"];
                    dr["CATEGORY_ID"] = gvr["CATEGORY_ID"];
                    dr["SUB_CATEGORY_ID"] = gvr["SUB_CATEGORY_ID"];
                    PurchaseDetail.Rows.Add(dr);
                }
                txtRemarks.Text = data.REMARKS;
                ASPxGridView1.DataSource = PurchaseDetail;
                ASPxGridView1.DataBind();
                foreach (GridViewRow dr in ASPxGridView1.Rows)
                {
                    foreach (DataRow dbr in dt2.Rows)
                    {
                        if (Convert.ToInt32(ASPxGridView1.DataKeys[dr.RowIndex].Values["ITEM_INFO_ID"]) == Convert.ToInt32(dbr["ITEM_INFO_ID"]))
                        {
                            System.Web.UI.WebControls.CheckBox chRelized = (System.Web.UI.WebControls.CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                            chRelized.Checked = true;
                        }
                    }
                }
                HideGridColumns();
                //int TotalPurchaseAmount = 0;
                //int TotalDiscountAmount = 0;
                //int TotalCommissionAmount = 0;
                //int TotalBalanceAmount = 0;
                //foreach (GridViewRow row in ASPxGridView1.Rows)
                //{
                //    string PurchaseAmount = ((System.Web.UI.WebControls.TextBox)row.Cells[2].FindControl("txtPurchaseAmount")).Text;
                //    string Discount = "0"; //((System.Web.UI.WebControls.TextBox)row.Cells[3].FindControl("txtDiscount")).Text;
                //    string Commission = "0"; //((System.Web.UI.WebControls.TextBox)row.Cells[5].FindControl("txtCommission")).Text;
                //    string Balance = ((System.Web.UI.WebControls.TextBox)row.Cells[2].FindControl("txtPurchaseAmount")).Text; //((System.Web.UI.WebControls.TextBox)row.Cells[6].FindControl("txtBalance")).Text;
                //    TotalPurchaseAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(PurchaseAmount)), 0));
                //    TotalDiscountAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(Discount)), 0));
                //    TotalCommissionAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(Commission)), 0));
                //    TotalBalanceAmount += Convert.ToInt32(Math.Round(decimal.Parse(dc.chkNull_0(Balance)), 0));
                //}
                this.Session.Add("PurchaseDetail", PurchaseDetail);
                this.Session.Add("OpeningVoucherNo", data.VOUCHER_NO);
                SetTotals();
                Session.Add("PurchasMasterID", data.PURCHASE_MASTER_ID);
                mastergrid.Visible = false;
                PanelPerchase.Visible = true;
                panelgrid.Visible = false;
                btnSave.Text = "Update";
    }
}
        else
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('You can't Update Inventory Update Transaction. Some items are saled.');", true);
}

        
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        bool flag = false;
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        var dController = new DistributorController();
        var dtt = dController.SelectPurchaseDetailPaymentInfo(Constants.Journal_Voucher, data.VENDOR_ID, data.PURCHASE_MASTER_ID, Constants.Document_Opening, 0, 1);
        if (dtt.Rows.Count > 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete. Some payments are issued against this purchase.');", true);
        }
        else
        {
            try
            {
                //var qq = (from k in db.tbl_SALE_MASTER
                //          join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
                //          select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
                //if (qq.Any())
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not delete because this item has been sold.');", true);
                //}
                //else
                //{
                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Opening && x.TYPE_ID == Constants.Document_Opening && x.IS_DELETED == 0).ToList();
                if (!Active)
                {
                    var qry = (from k in db.tbl_ITEM_LOG select k).Where(x => x.TransactionID == data.PURCHASE_MASTER_ID && x.TypeID == Constants.Document_Opening && x.IsActive == true).ToList();
                    int ItemLogID = Convert.ToInt32(qry.FirstOrDefault().ITEM_LOG_ID);
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_MASTER where PURCHASE_MASTER_ID =" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("DELETE tbl_PURCHASE_DETAIL where PURCHASE_MASTER_ID =" + Convert.ToInt32(ProID), con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        using (SqlCommand command4 = new SqlCommand("DELETE tbl_ITEM_LOG where ITEM_LOG_ID =" + Convert.ToInt32(ItemLogID), con))
                        {
                            command4.ExecuteNonQuery();
                        }
                        if (q.Count > 0)
                        {
                            string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                            using (SqlCommand command3 = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Opening + "and PAYMENT_MODE=" + Constants.CreditSale + "and TYPE_ID=" + Constants.Document_Opening, con))
                            {
                                command3.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                            {
                                command2.ExecuteNonQuery();
                            }
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                    }
                }
                //}
            }
            catch (Exception ex) { }
        }
        LoadGrid("");
    }

    //protected void grid_DataBound(object sender, EventArgs e)
    //{
    //    grid.Columns["ITEM_INFO_ID"].Visible = false;
    //    grid.Columns["PROJECT_ID"].Visible = false;
    //    grid.Columns["CATEGORY_ID"].Visible = false;
    //    grid.Columns["SUB_CATEGORY_ID"].Visible = false;
    //}

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadItems(txtSearch.Text);
    }
}