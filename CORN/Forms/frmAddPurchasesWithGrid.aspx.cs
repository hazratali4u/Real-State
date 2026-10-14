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

public partial class Forms_frmAddPurchasesWithGrid : System.Web.UI.Page
{
    private static int RowNo;
    DataTable PurchaseDetail;
   
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
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
            //ASPxGridView1.SearchPanelFilter.
            //SearchPanelFilter
            panelgrid.Visible = true;
            PanelPerchase.Visible = true;
            Items(2);
            LoadProject();
            LoadDistributor();
            LoadCategories();
            LoadSubCategories();
            DetailCategoryLoad();
            LoadBlock("");
            LoadStreet();
            
            LoadVendors();
            LoadCommissionAgent();
            LoadGrid("");
            DoEmptyTextBox();
            RbChange();
            CreatTable();
            txtBalance.Attributes.Add("readonly", "readonly");
            //txtPswd.Attributes.Add("autocomplete", "off");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            mastergrid.Visible = true;
            GetCOAConfiguration();
            GetFinanceConfig();
            //Panel3.Visible = true;
        }
        LoadItems("");
    }

    #region Load Events
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
    public void DoEmptyTextBox()
    {
        Session["PurchasMasterID"] = null;
        txtBalance.Text = "";
        //txtDownPayment.Text = "";
        txtPurchaseAmount.Text = "";
        txtDiscount.Text = "";
        //txtPurchaseDate.Text = 
        //txtTokenAmount.Text = "";
        //txtSearch.Text = "";
        //txtCommissionValue.Text = "";
        ////txtRemarks.Text = "";
        txtDiscount.Text = "";
        btnSave.Text = "Save";
        txtPurchaseAmount.Text = "";
        //ddlPaymentMode.SelectedValue = "0";
        //txtCommisionAmount.Text = "";
        //ddlCommissionAgent.Items.Add(new DevExpress.Web.ListEditItem("--Select--", "0"));
        ////ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
        //ddlCommissionPercent.SelectedIndex = 0;
        ////ddlCommissionAgent.SelectedValue = "0";
        mastergrid.Visible = true;

    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_PURCHASE_MASTER
                           //join pd in db.tbl_PURCHASE_DETAIL on k.PURCHASE_MASTER_ID equals pd.PURCHASE_MASTER_ID
                       join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
                       //join i in db.ITEM_INFORMATION on pd.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       //join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
                       //join s in db.STREET on i.STREET_ID equals s.STREET_ID
                       join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
                       select new
                       {
                           d.DISTRIBUTOR_NAME,
                           p.PROJECT_NAME,
                           //ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME,
                           v.VENDOR_NAME,
                           k.PURCHASE_MASTER_ID,
                           k.TOTAL_PURCHASE_AMOUNT,
                           Active = k.IS_DELETED == false ? "Active" : "Inactive",
                           k.IS_DELETED,
                           k.TYPE_ID
                       }).Where(x => x.Active == "Active" && x.TYPE_ID == 2).ToList();
            PurchaseMaster.DataSource = qry;
            PurchaseMaster.DataBind();
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

        }

    }
    private void CreatTable()
    {
        PurchaseDetail = new DataTable();
        PurchaseDetail.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetail.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetail.Columns.Add("PURCHASE_AMOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("DISCOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_PER", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION_VALUE", typeof(decimal));
        PurchaseDetail.Columns.Add("COMMISSION", typeof(decimal));
        PurchaseDetail.Columns.Add("BALANCE", typeof(decimal));
        PurchaseDetail.Columns.Add("TOKEN_AMOUNT", typeof(decimal));
        PurchaseDetail.Columns.Add("DOWN_PAYMENT", typeof(decimal));
        this.Session.Add("PurchaseDetail", PurchaseDetail);
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

    public void Items(int Type)
    {
        if (Type != 8)
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
    protected void LoadItems(string Val)
    {
        DbCalls db = new DbCalls();
        int PROJECTID = int.Parse(ddlProject.SelectedValue);
        var query2 = (
      from i in db.ITEM_INFORMATION
      join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID

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
        ITEM_INFO_ID = i.ITEM_INFO_ID,
        p.PROJECT_ID,
        p.PROJECT_NAME,
        ITEM_NAME = i.ITEM_NAME,
        //BLOCK_NAME = b.BLOCK_NAME,
        //STREET_NAME = s.STREET_NAME,
        //pr.PRECENT_NAME,
        //prs.PRECENTSTREET_NAME,
        //sc.SECTOR_NAME,
        BLOCK_NAME = b.BLOCK_NAME == null ? "Not Applicable" : b.BLOCK_NAME,
        STREET_NAME = s.STREET_NAME == null ? "Not Applicable" : s.STREET_NAME,
        PRECENT_NAME = pr.PRECENT_NAME == null ? "Not Applicable" : pr.PRECENT_NAME,
        PRECENTSTREET_NAME = prs.PRECENTSTREET_NAME == null ? "Not Applicable" : prs.PRECENTSTREET_NAME,
        SECTOR_NAME = sc.SECTOR_NAME == null ? "Not Applicable" : sc.SECTOR_NAME

          //UserGroupId = groups.ID
          //,
          //GroupName = groups.Name
      }

  );


        var xy = (query2).Where(x=>x.PROJECT_ID == PROJECTID).ToList();

        grid.DataSource = xy;
        grid.DataBind();

        //var qry = from k in db.ITEM_INFORMATION

        //          select new { k.ITEM_INFO_ID,k.ITEM_NAME }
        //if (Val.Equals(""))
        //{
        //    DataTable dt = (DataTable)this.Session["dt"];
        //    if (dt.Rows.Count > 0)
        //    {
        //        var qry = dt.AsEnumerable()
        //              .Where(r => r.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue)
        //              && r.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue)
        //              && r.Field<int>("STREET_ID") == int.Parse(ddlStreet.SelectedValue)
        //              && r.Field<int>("CATEGORY_ID") == int.Parse(ddlCategory.SelectedValue)
        //              && r.Field<int>("SUB_CATEGORY_ID") == int.Parse(ddlSubCategory.SelectedValue)
        //              && r.Field<int>("DETAIL_CATEGORY_ID") == int.Parse(ddlDetailCategory.SelectedValue));
        //        if (qry.Any())
        //        {
        //            var Items = qry.CopyToDataTable();
        //            ddlItem.DataSource = Items;
        //            ddlItem.DataTextField = "ITEM_NAME";
        //            ddlItem.DataValueField = "ITEM_INFO_ID";
        //            ddlItem.DataBind();
        //            ddlItem.SelectedIndex = 0;
        //        }
        //        else
        //        {
        //            ddlItem.Items.Clear();
        //            ddlItem.Items.Add(new ListItem("", "0"));
        //            ddlItem.SelectedIndex = 0;
        //        }
        //    }
        //    else
        //    {
        //        ddlItem.Items.Clear();
        //        ddlItem.Items.Add(new ListItem("", "0"));
        //        ddlItem.SelectedIndex = 0;
        //    }
        //}
        //else
        //{

        //}

    }
    protected void LoadBlock(string Val)
    {
        //DataTable dt = (DataTable)this.Session["dt"];
        //if (dt.Rows.Count > 0)
        //{
        //    var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue)
        //              && x.Field<int>("DETAIL_CATEGORY_ID") == int.Parse(ddlDetailCategory.SelectedValue))
        //              group rows by new { BLOCK_ID = rows["BLOCK_ID"], BLOCK_NAME = rows["BLOCK_NAME"] } into grp
        //              select grp.First();
        //    if (qry.Any())
        //    {
        //        var Blocks = qry.CopyToDataTable();
        //        ddlBlock.DataSource = Blocks;
        //        ddlBlock.DataTextField = "BLOCK_NAME";
        //        ddlBlock.DataValueField = "BLOCK_ID";
        //        ddlBlock.DataBind();
        //    }
        //    else
        //    {
        //        ddlBlock.Items.Clear();
        //        ddlBlock.Items.Add(new ListItem("", "0"));
        //        ddlBlock.SelectedIndex = 0;
        //    }
        //}
        //else
        //{
        //    ddlBlock.Items.Clear();
        //    ddlBlock.Items.Add(new ListItem("", "0"));
        //    ddlBlock.SelectedIndex = 0;
        //}
    }
    protected void LoadStreet()
    {
        //DataTable dt = (DataTable)this.Session["dt"];
        //if (dt.Rows.Count > 0)
        //{
        //    var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue)
        //              && x.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue)
        //              && x.Field<int>("DETAIL_CATEGORY_ID") == int.Parse(ddlDetailCategory.SelectedValue))
        //              group rows by new { STREET_ID = rows["STREET_ID"], STREET_NAME = rows["STREET_NAME"] } into grp
        //              select grp.First();
        //    if (qry.Any())
        //    {
        //        var Streets = qry.CopyToDataTable();
        //        ddlStreet.DataSource = Streets;
        //        ddlStreet.DataTextField = "STREET_NAME";
        //        ddlStreet.DataValueField = "STREET_ID";
        //        ddlStreet.DataBind();
        //    }
        //    else
        //    {
        //        ddlStreet.Items.Clear();
        //        ddlStreet.Items.Add(new ListItem("", "0"));
        //        ddlStreet.SelectedIndex = 0;
        //    }
        //}
        //else
        //{
        //    ddlStreet.Items.Clear();
        //    ddlStreet.Items.Add(new ListItem("", "0"));
        //    ddlStreet.SelectedIndex = 0;
        //}
    }
    protected void LoadVendors()
    {
        ddlVendor.DataSource = DropDowns.VendorDropDown();
        ddlVendor.DataTextField = "VENDOR_NAME";
        ddlVendor.DataValueField = "VENDOR_ID";
        ddlVendor.DataBind();
    }
    protected void LoadCommissionAgent()
    {
        //ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
        //ddlCommissionAgent.DataTextField = "VENDOR_NAME";
        //ddlCommissionAgent.DataValueField = "VENDOR_ID";
        //ddlCommissionAgent.DataBind();
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
    #endregion
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        //LoadGrid(txtSearch.Text);
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
        //if (ddlBlock.SelectedValue.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
        //    ddlBlock.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
        //    return;
        //}
        //if (ddlStreet.SelectedValue.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street is required.');", true);
        //    ddlStreet.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
        //    return;
        //}
        //if (ddlItem.SelectedValue.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
        //    ddlItem.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
        //    return;
        //}
        if (ddlVendor.SelectedValue.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ddlVendor.Focus();
            //ScriptManager.GetCurrent(Page).SetFocus(ddlVendor);
            return;
        }
        //if (txtPurchaseAmount.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount is required.');", true);
        //    txtPurchaseAmount.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
        //    return;
        //}
        //if (txtPurchaseAmount.Text.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount could not be zero.');", true);
        //    txtPurchaseAmount.Focus();
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
        //    return;
        //}
        //if (txtPurchaseAmount.Text.Equals("0.00"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Amount could not be zero.');", true);
        //    txtPurchaseAmount.Focus();
        //    //ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
        //    return;
        //}
        //if (txtCommisionAmount.Text.Equals(""))
        //{
        //    txtCommisionAmount.Text = "0";
        //}
        string ItemInfo = "";
        DataTable dt = (DataTable)this.Session["dt"];
        DataRow[] foundRows = dt.Select("ITEM_INFO_ID  = " + int.Parse(ddlItem.SelectedValue));
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
        //string myStr = "ABCDEPER 343573";
        //string subStr = myStr.Right(6);
        PurchaseMasterLogics pml = new PurchaseMasterLogics();
        tbl_PURCHASE_MASTER pm = new tbl_PURCHASE_MASTER();
        pm.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
        pm.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedValue);
        pm.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedValue);
        pm.VENDOR_ID = Convert.ToInt32(ddlVendor.SelectedValue);
        pm.COMMISSIONAGENT_ID = 0; //Convert.ToInt32(ddlCommissionAgent.SelectedValue);
        pm.TOTAL_PURCHASE_AMOUNT = Convert.ToDecimal(txtTotalPurchaseAmount.Text);
        pm.TOTAL_DISCOUNT = Convert.ToDecimal(txtTotalDiscountAmount.Text);
        pm.TOTAL_TOKEN_AMOUNT = Convert.ToDecimal(txtTotalTokenAmount.Text);
        pm.TOTAL_DOWN_PAYMENT = Convert.ToDecimal(txtTotalDownPayment.Text);
        pm.TOTAL_BALANCE = Convert.ToDecimal(txtTotalBalance.Text);
        pm.TOTAL_COMMISSION = Convert.ToDecimal(txtTotalCommission.Text);
        pm.REMARKS = ""; // txtRemarks.Text;
        pm.USER_ID = Convert.ToInt32(Session["UserID"]);
        pm.TIME_STEMP = DateTime.Now;
        if (Session["PurchasMasterID"] != null)
        {

            pm.TYPE_ID = 2;
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
                using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_DETAIL where PURCHASE_MASTER_ID=" + PURCHASE_MASTER_ID, con))
                {
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            foreach (GridViewRow gvr in GrdItems.Rows)
            {
                pd.PURCHASE_MASTER_ID = PURCHASE_MASTER_ID;
                pd.ITEM_INFO_ID = Convert.ToInt32(gvr.Cells[0].Text);
                pd.PURCHASE_AMOUNT = Convert.ToDecimal(gvr.Cells[2].Text);
                pd.DISCOUNT = Convert.ToDecimal(gvr.Cells[3].Text);
                pd.TOKEN_AMOUNT = Convert.ToDecimal(gvr.Cells[8].Text);
                pd.DOWN_PAYMENT = Convert.ToDecimal(gvr.Cells[9].Text);
                pd.COMMISSION_PERCENT = Convert.ToDecimal(gvr.Cells[6].Text);
                pd.COMMISSION_VALUE = Convert.ToDecimal(gvr.Cells[7].Text);
                pd.COMMISSION = Convert.ToDecimal(gvr.Cells[4].Text);
                pd.BALANCE = Convert.ToDecimal(gvr.Cells[5].Text);
                pd.DEBIT_AMOUNT = Convert.ToDecimal(0.00);
                pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
                pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                pd.TIME_STEMP = DateTime.Now;
                pd.IS_DELETED = Convert.ToBoolean(0);
                pdl.Save(pd);
            }


            var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == pm.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
            if (q.Count > 0)
            {
                string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(pm.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale, con))
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
            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
               , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
               , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
               , Convert.ToString(pm.PURCHASE_MASTER_ID)
               , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            Session["DBT_Amount"] = null;
            Session["BAL_Amount"] = null;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PurchasMasterID"] = null;
            mastergrid.Visible = true;
        }
        else
        {
            pm.TYPE_ID = 2;
            pm.VOUCHER_NO = "";
            pm.IS_DELETED = false;
            pml.Save(pm);
            int PURCHASE_MASTER_ID = pm.PURCHASE_MASTER_ID;
            PurchaseDetailLogics pdl = new PurchaseDetailLogics();
            tbl_PURCHASE_DETAIL pd = new tbl_PURCHASE_DETAIL();
            foreach (GridViewRow gvr in GrdItems.Rows)
            {
                pd.PURCHASE_MASTER_ID = PURCHASE_MASTER_ID;
                pd.ITEM_INFO_ID = Convert.ToInt32(gvr.Cells[0].Text);
                pd.PURCHASE_AMOUNT = Convert.ToDecimal(gvr.Cells[2].Text);
                pd.DISCOUNT = Convert.ToDecimal(gvr.Cells[3].Text);
                pd.TOKEN_AMOUNT = Convert.ToDecimal(gvr.Cells[8].Text);
                pd.DOWN_PAYMENT = Convert.ToDecimal(gvr.Cells[9].Text);
                pd.COMMISSION_PERCENT = Convert.ToDecimal(gvr.Cells[6].Text);
                pd.COMMISSION_VALUE = Convert.ToDecimal(gvr.Cells[7].Text);
                pd.COMMISSION = Convert.ToDecimal(gvr.Cells[4].Text);
                pd.BALANCE = Convert.ToDecimal(gvr.Cells[5].Text);
                pd.DEBIT_AMOUNT = Convert.ToDecimal(0.00);
                pd.PURCHASE_CLOSED = Convert.ToBoolean(0);
                pd.USER_ID = Convert.ToInt32(Session["UserID"]);
                pd.TIME_STEMP = DateTime.Now;
                pd.IS_DELETED = Convert.ToBoolean(0);
                pdl.Save(pd);
            }

            string Info = "Ref.# " + pm.PURCHASE_MASTER_ID + ", " + ItemInfo;
            LedgerEntries(pm.DISTRIBUTOR_ID, 0, Convert.ToDecimal(pm.TOTAL_PURCHASE_AMOUNT)
                , pm.DATE, pm.VENDOR_ID, Constants.Document_Purchase
                , "", Convert.ToInt32(Session["UserID"]), pm.PURCHASE_MASTER_ID
                , Convert.ToString(pm.PURCHASE_MASTER_ID)
                , Convert.ToDecimal(pm.TOTAL_DISCOUNT), Convert.ToDecimal(pm.TOTAL_TOKEN_AMOUNT), Convert.ToDecimal(pm.TOTAL_DOWN_PAYMENT), Info, Convert.ToDecimal(txtTotalCommission.Text)); //Convert.ToDecimal(txtCommisionAmount.Text));
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadGrid("");
        DoEmptyTextBox();
        mastergrid.Visible = true;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
        //Panel3.Visible = true;
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        panelgrid.Visible = true;
        mastergrid.Visible = true;
        PanelPerchase.Visible = false;
        //Panel3.Visible = true;
        DoEmptyTextBox();
        Items(2);
        LoadBlock("");
        LoadStreet();
        LoadItems("");
    }
    protected void RbtCommissionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (ddlCommissionAgent.SelectedValue == "0")
        //{
        //    RbtCommissionType.SelectedValue = "1";
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please select commission agent first.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlCommissionAgent);
        //    return;
        //}
        //RbChange();
    }
    public void RbChange()
    {
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    txtCommissionValue.ReadOnly = true;
        //    ddlCommissionPercent.Enabled = true;
        //    ddlCommissionPercent.SelectedIndex = 0;
        //    txtCommissionValue.Text = "0";
        //    txtCommisionAmount.Text = "0";
        //    ddlCommissionPercent.Focus();
        //}
        //else
        //{
        //    ddlCommissionPercent.SelectedIndex = 0;
        //    txtCommissionValue.ReadOnly = false;
        //    ddlCommissionPercent.Enabled = false;
        //    txtCommisionAmount.Text = "0";
        //    txtCommissionValue.Focus();
        //}
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
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update. Some payments are issued against this purchase.');", true);
        }
        else
        {
            //var q = (from k in db.tbl_SALE_MASTER
            //         join si in db.tbl_SALE_INS_MASTER on k.DISTRIBUTOR_ID equals si.DISTRIBUTOR_ID
            //         select new { k.ITEM_INFO_ID, k.SALE_CLOSED, SALE_INS_CLOSED = si.SALE_CLOSED }).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID && x.SALE_CLOSED == false).ToList();
            //if (q.Any())
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not update because this item has been sold.');", true);
            //}
            //else
            //{
            //DataTable dt = new DataTable();
            //DataTable dtItem = new DataTable();
            //Items(2);
            //Items(8);
            //dtItem = (DataTable)this.Session["dtItem"];
            //DataRow[] foundRows = dtItem.Select("ITEM_INFO_ID  = '" + data.ITEM_INFO_ID + "'");
            //if (foundRows.Length > 0)
            //{
            //    dt = (DataTable)this.Session["dt"];
            //    DataRow dr = dt.NewRow();
            //    dr["DISTRIBUTOR_ID"] = Convert.ToInt32(foundRows[0]["DISTRIBUTOR_ID"].ToString());
            //    dr["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
            //    dr["PROJECT_NAME"] = foundRows[0]["PROJECT_NAME"].ToString();
            //    dr["BLOCK_ID"] = Convert.ToInt32(foundRows[0]["BLOCK_ID"].ToString());
            //    dr["BLOCK_NAME"] = foundRows[0]["BLOCK_NAME"].ToString();
            //    dr["STREET_ID"] = Convert.ToInt32(foundRows[0]["STREET_ID"].ToString());
            //    dr["STREET_NAME"] = foundRows[0]["STREET_NAME"].ToString();
            //    dr["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
            //    dr["ITEM_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
            //    dr["Consumption"] = foundRows[0]["Consumption"].ToString();
            //    //dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
            //    dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
            //    dt.Rows.Add(dr);
            //    Session["dt"] = dt;
            //}
            ddlProject.DataSource = DropDowns.PROJECTDropDown();
            ddlProject.DataTextField = "PROJECT_NAME";
            ddlProject.DataValueField = "PROJECT_ID";
            ddlProject.DataBind();
            ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
            ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
            ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
            ddlLocation.DataValueField = "DISTRIBUTOR_ID";
            ddlLocation.DataBind();

            ddlVendor.DataSource = DropDowns.VendorDropDown();
            ddlVendor.DataTextField = "VENDOR_NAME";
            ddlVendor.DataValueField = "VENDOR_ID";
            ddlVendor.DataBind();
            ddlVendor.SelectedValue = Convert.ToString(data.VENDOR_ID);
            //ddlCommissionAgent.DataSource = DropDowns.CommissionAgentDropDown();
            //ddlCommissionAgent.DataTextField = "VENDOR_NAME";
            //ddlCommissionAgent.DataValueField = "VENDOR_ID";
            //ddlCommissionAgent.DataBind();
            //ddlCommissionAgent.Items.Add(new ListItem("--Select--", "0"));
            //ddlCommissionAgent.SelectedValue = Convert.ToString(data.COMMISSIONAGENT_ID);

            txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            //decimal purchaseamount = decimal.Parse(data.PURCHASE_AMOUNT.ToString());
            //txtPurchaseAmount.Text = Convert.ToString(Math.Round(purchaseamount, 2));

            //decimal discountamount = decimal.Parse(data.DISCOUNT.ToString());
            //txtDiscount.Text = Convert.ToString(Math.Round(discountamount, 2));

            //decimal tokenamount = decimal.Parse(data.TOKEN_AMOUNT.ToString());
            ////txtTokenAmount.Text = Convert.ToString(Math.Round(tokenamount, 2));

            //decimal downpayment = decimal.Parse(data.DOWN_PAYMENT.ToString());
            ////txtDownPayment.Text = Convert.ToString(Math.Round(downpayment, 2));

            //decimal balance = decimal.Parse(data.BALANCE.ToString());
            //txtBalance.Text = Convert.ToString(Math.Round(balance, 2));

            //ddlCommissionPercent.SelectedValue = Convert.ToString(data.COMMISSION_PERCENT);
            //if (ddlCommissionPercent.SelectedValue != "0")
            //{
            //    txtCommissionValue.Text = "0";
            //    decimal amount = Convert.ToDecimal(data.BALANCE / 100 * data.COMMISSION_PERCENT);
            //    txtCommisionAmount.Text = Convert.ToString(Math.Round(amount, 2));
            //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
            //    RbtCommissionType.SelectedValue = "1";
            //}
            //else
            //{
            //    RbtCommissionType.SelectedValue = "2";
            //    txtCommissionValue.Text = Convert.ToString(data.COMMISSION_VALUE);
            //    txtCommisionAmount.Text = Convert.ToString(data.COMMISSION_VALUE);
            //}
            //txtRemarks.Text = data.REMARKS;
            DataTable dt = LController.SelectBSI(data.PURCHASE_MASTER_ID, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, 26);
            //var query = (from k in db.tbl_PURCHASE_DETAIL
            //             join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
            //             join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
            //             join s in db.STREET on i.STREET_ID equals s.STREET_ID
            //             select new { k.PURCHASE_MASTER_ID,k.ITEM_INFO_ID, ITEM_INFO_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME,
            //             k.PURCHASE_AMOUNT,k.DISCOUNT,
            //                 COMMISSION_PER = k.COMMISSION_PERCENT,k.COMMISSION_VALUE,k.COMMISSION,k.BALANCE,k.TOKEN_AMOUNT,k.DOWN_PAYMENT}).Where(X => X.PURCHASE_MASTER_ID == data.PURCHASE_MASTER_ID).ToList();
            //PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
            //query.CopyToDataTable(PurchaseDetail, LoadOption.PreserveChanges);
            GrdItems.DataSource = dt;
            GrdItems.DataBind();
            this.Session.Add("PurchaseDetail", dt);
            decimal TotalPurchaseAmount = 0;
            decimal TotalDiscount = 0;
            decimal TotalCommission = 0;
            decimal TotalBalance = 0;
            decimal TotalTokenAmount = 0;
            decimal TotalDownPayment = 0;
            foreach (GridViewRow gvr in GrdItems.Rows)
            {
                TotalPurchaseAmount += Convert.ToDecimal(gvr.Cells[2].Text);
                TotalDiscount += Convert.ToDecimal(gvr.Cells[3].Text);
                TotalCommission += Convert.ToDecimal(gvr.Cells[4].Text);
                TotalBalance += Convert.ToDecimal(gvr.Cells[5].Text);
                TotalTokenAmount += Convert.ToDecimal(gvr.Cells[8].Text);
                TotalDownPayment += Convert.ToDecimal(gvr.Cells[9].Text);
            }
            txtTotalPurchaseAmount.Text = Convert.ToString(TotalPurchaseAmount);
            txtTotalDiscountAmount.Text = Convert.ToString(TotalDiscount);
            txtTotalCommission.Text = Convert.ToString(TotalCommission);
            txtTotalBalance.Text = Convert.ToString(TotalBalance);
            txtTotalTokenAmount.Text = Convert.ToString(TotalTokenAmount);
            txtTotalDownPayment.Text = Convert.ToString(TotalDownPayment);


            Session.Add("PurchasMasterID", data.PURCHASE_MASTER_ID);
            mastergrid.Visible = false;
            panelgrid.Visible = false;
            PanelPerchase.Visible = true;
            btnSave.Text = "Update";
            //Panel3.Visible = false;
            //}
        }

    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool flag = false;
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseMasterLogics Ser = new PurchaseMasterLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase could not be deleted. Some payments are issued against this purchase.');", true);
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
                var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_MASTER_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase && x.IS_DELETED == 0).ToList();
                if (!Active)
                {
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
                        if (q.Count > 0)
                        {
                            string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                            using (SqlCommand command3 = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and PRINCIPAL_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_MASTER_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase + "and PAYMENT_MODE=" + Constants.CreditSale, con))
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

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
        LoadStreet();
        LoadItems("");
        //ddlBlock.Focus();
        //ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
    }

    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        panelgrid.Visible = false;
        PanelPerchase.Visible = true;
        //lblToketAmount.InnerText = "Token Amount";
        //txtDownPayment.Enabled = true;
        txtDiscount.Text = "";
        DoEmptyTextBox();
        LoadVendors();
        Items(2);
        LoadProject();
        LoadBlock("");
        LoadStreet();
        LoadItems("");
        //LoadGrid("");
        //Panel3.Visible = false;
    }
    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        Session["PURCHASE_MASTER_ID"] = null;
        Session["VENDOR_ID"] = null;
        Session["BALANCE"] = null;
        Session["DIST_ID"] = null;
        Session["Purchase_Date"] = null;
        panelgrid.Visible = true;
        PanelPerchase.Visible = false;
        //Panel3.Visible = true;
        Items(2);
        LoadProject();
        LoadBlock("");
        LoadStreet();
        LoadItems("");
    }
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void DrpVendor_SelectedIndexChanged(object sender, EventArgs e)
    {

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

            DataTable dtConfig = GetCOAConfiguration();
            bool IsFinanceSetting = GetFinanceConfig();
            DataRow[] drConfig = null;

            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.Inventoryatstore + "'");
            long PurchaseAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());


            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.AccountPayable + "'");
            long PayableAccount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            drConfig = dtConfig.Select("CODE = '" + (int)Enums.COAMapping.PurchaseDiscount + "'");
            long PurchaseDiscount = Convert.ToInt64(drConfig[0]["VALUE"].ToString());

            if (p_DOCUMENT_TYPE_ID == Constants.Document_Purchase)
            {

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Purchase-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_Discount_Amount, 0, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Discount_Amount, p_Ledger_Date, "Discount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);
                LController.PostingPrinvipalInvoiceAccount(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase", 0, Constants.Document_Purchase);

                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Purchase
                        , p_MANUAL_DOCUMENT_NO, "Purchase Voucher / " + ddlVendor.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Purchase", mTransaction, mConnection);
                    decimal Purchases = p_Credit - p_Discount_Amount;
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, Purchases, 0, "Purchase Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, 0, Purchases, "Purchase Voucher", mTransaction, mConnection);
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

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        if (txtBalance.Text.Equals("") || txtBalance.Text.Equals("0") || decimal.Parse(txtBalance.Text) < 0)
        {
            if (txtPurchaseAmount.Text.Equals("") || txtPurchaseAmount.Text.Equals("0"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter Purchase Amount first.');", true);
                ScriptManager.GetCurrent(Page).SetFocus(txtPurchaseAmount);
                return;
            }
            else
            {
                if (txtBalance.Text.Equals("0") || int.Parse(txtBalance.Text) < 0)
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
        if (!CheckDublicate())
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + ddlItem.SelectedItem.Text + " is already exists.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        if (Convert.ToInt32(RowId.Value) >= 0)
        {

            DataRow dr = PurchaseDetail.Rows[Convert.ToInt32(RowId.Value)];
            dr["ITEM_INFO_ID"] = int.Parse(ddlItem.SelectedValue);
            dr["ITEM_INFO_NAME"] = ddlItem.SelectedItem.Text;
            dr["PURCHASE_AMOUNT"] = decimal.Parse(txtPurchaseAmount.Text);
            dr["DISCOUNT"] = decimal.Parse(txtDiscount.Text);
            if (RbtCommissionType.SelectedValue == "1")
            {
                dr["COMMISSION_PER"] = decimal.Parse(txtCommissionValue.Text);
                decimal Commission = (decimal.Parse(txtBalance.Text) / 100) * decimal.Parse(txtCommissionValue.Text);
                dr["COMMISSION_VALUE"] = 0;
                dr["COMMISSION"] = Commission;
            }
            else
            {
                dr["COMMISSION_PER"] = 0;
                dr["COMMISSION_VALUE"] = decimal.Parse(txtCommissionValue.Text);
                dr["COMMISSION"] = decimal.Parse(txtCommissionValue.Text);
            }
            dr["BALANCE"] = decimal.Parse(txtBalance.Text);
            dr["TOKEN_AMOUNT"] = 0;
            dr["DOWN_PAYMENT"] = 0;
        }
        else
        {
            DataRow dr = PurchaseDetail.NewRow();
            dr["ITEM_INFO_ID"] = int.Parse(ddlItem.SelectedValue);
            dr["ITEM_INFO_NAME"] = ddlItem.SelectedItem.Text;
            dr["PURCHASE_AMOUNT"] = decimal.Parse(txtPurchaseAmount.Text);
            dr["DISCOUNT"] = decimal.Parse(txtDiscount.Text);
            if (RbtCommissionType.SelectedValue == "1")
            {
                dr["COMMISSION_PER"] = decimal.Parse(txtCommissionValue.Text);
                decimal Commission = (decimal.Parse(txtBalance.Text) / 100) * decimal.Parse(txtCommissionValue.Text);
                dr["COMMISSION_VALUE"] = 0;
                dr["COMMISSION"] = Commission;
            }
            else
            {
                dr["COMMISSION_PER"] = 0;
                dr["COMMISSION_VALUE"] = decimal.Parse(txtCommissionValue.Text);
                dr["COMMISSION"] = decimal.Parse(txtCommissionValue.Text);
            }
            dr["BALANCE"] = decimal.Parse(txtBalance.Text);
            dr["TOKEN_AMOUNT"] = 0;
            dr["DOWN_PAYMENT"] = 0;
            PurchaseDetail.Rows.Add(dr);
        }
        Clear();
        GrdItems.DataSource = PurchaseDetail;
        GrdItems.DataBind();
        btnAdd.Text = "Add";
        this.Session.Add("PurchaseDetail", PurchaseDetail);
        ScriptManager.GetCurrent(Page).SetFocus(ddlItem);

        //TotalTable.Visible = true;
        decimal TotalPurchaseAmount = 0;
        decimal TotalDiscount = 0;
        decimal TotalCommission = 0;
        decimal TotalBalance = 0;
        decimal TotalTokenAmount = 0;
        decimal TotalDownPayment = 0;
        foreach (GridViewRow gvr in GrdItems.Rows)
        {
            TotalPurchaseAmount += Convert.ToDecimal(gvr.Cells[2].Text);
            TotalDiscount += Convert.ToDecimal(gvr.Cells[3].Text);
            TotalCommission += Convert.ToDecimal(gvr.Cells[4].Text);
            TotalBalance += Convert.ToDecimal(gvr.Cells[5].Text);
            TotalTokenAmount += Convert.ToDecimal(gvr.Cells[8].Text);
            TotalDownPayment += Convert.ToDecimal(gvr.Cells[9].Text);
        }
        txtTotalPurchaseAmount.Text = Convert.ToString(TotalPurchaseAmount);
        txtTotalDiscountAmount.Text = Convert.ToString(TotalDiscount);
        txtTotalCommission.Text = Convert.ToString(TotalCommission);
        txtTotalBalance.Text = Convert.ToString(TotalBalance);
        txtTotalTokenAmount.Text = Convert.ToString(TotalTokenAmount);
        txtTotalDownPayment.Text = Convert.ToString(TotalDownPayment);
        ddlItem.Enabled = true;
    }
    public void Clear()
    {
        #region Clear Txtbox
        txtPurchaseAmount.Text = "";
        txtDiscount.Text = "";
        txtCommissionValue.Text = "";
        txtBalance.Text = "";
        RowId.Value = "-1";
        #endregion
    }
    private bool CheckDublicate()
    {
        if (Convert.ToInt32(RowId.Value) >= 0)
        {
            return true;
        }
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        DataRow[] foundRows = PurchaseDetail.Select("ITEM_INFO_ID  = '" + ddlItem.SelectedValue + "'");
        if (foundRows.Length == 0)
        {
            return true;
        }
        return false;
    }
    protected void GrdItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GrdItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        PurchaseDetail = (DataTable)this.Session["PurchaseDetail"];
        PurchaseDetail.Rows.RemoveAt(e.RowIndex);
        GrdItems.DataSource = PurchaseDetail;
        GrdItems.DataBind();
        RowId.Value = "-1";
        btnAdd.Text = "Add";
        TotalTable.Visible = true;
        decimal TotalPurchaseAmount = 0;
        decimal TotalDiscount = 0;
        decimal TotalCommission = 0;
        decimal TotalBalance = 0;
        decimal TotalTokenAmount = 0;
        decimal TotalDownPayment = 0;
        foreach (GridViewRow gvr in GrdItems.Rows)
        {
            TotalPurchaseAmount += Convert.ToDecimal(gvr.Cells[2].Text);
            TotalDiscount += Convert.ToDecimal(gvr.Cells[3].Text);
            TotalCommission += Convert.ToDecimal(gvr.Cells[4].Text);
            TotalBalance += Convert.ToDecimal(gvr.Cells[5].Text);
            TotalTokenAmount += Convert.ToDecimal(gvr.Cells[8].Text);
            TotalDownPayment += Convert.ToDecimal(gvr.Cells[9].Text);
        }
        txtTotalPurchaseAmount.Text = Convert.ToString(TotalPurchaseAmount);
        txtTotalDiscountAmount.Text = Convert.ToString(TotalDiscount);
        txtTotalCommission.Text = Convert.ToString(TotalCommission);
        txtTotalBalance.Text = Convert.ToString(TotalBalance);
        txtTotalTokenAmount.Text = Convert.ToString(TotalTokenAmount);
        txtTotalDownPayment.Text = Convert.ToString(TotalDownPayment);
        ddlItem.Enabled = true;
    }

    protected void GrdItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        RowId.Value = e.NewEditIndex.ToString();
        //LoadItems("");
        DataTable dt = new DataTable();
        dt = new DataTable();
        dt.Columns.Add("ITEM_INFO_ID", typeof(int));
        dt.Columns.Add("ITEM_NAME", typeof(string));
        DataRow dr = dt.NewRow();
        dr["ITEM_INFO_ID"] = int.Parse(GrdItems.Rows[e.NewEditIndex].Cells[0].Text);
        dr["ITEM_NAME"] = GrdItems.Rows[e.NewEditIndex].Cells[1].Text;
        dt.Rows.Add(dr);

        ddlItem.Items.Clear();
        var qry = dt.AsEnumerable();
        if (qry.Any())
        {
            var Items = qry.CopyToDataTable();
            ddlItem.DataSource = Items;
            ddlItem.DataTextField = "ITEM_NAME";
            ddlItem.DataValueField = "ITEM_INFO_ID";
            ddlItem.DataBind();
            ddlItem.SelectedIndex = 0;
        }
        ddlItem.Enabled = false;
        txtPurchaseAmount.Text = GrdItems.Rows[e.NewEditIndex].Cells[2].Text.Replace(",", "");
        txtDiscount.Text = GrdItems.Rows[e.NewEditIndex].Cells[3].Text.Replace(",", "");
        decimal Commision = decimal.Parse(GrdItems.Rows[e.NewEditIndex].Cells[6].Text);
        if (Commision > 0)
        {
            decimal CommisionValue = Convert.ToDecimal(GrdItems.Rows[e.NewEditIndex].Cells[4].Text);
            decimal Balance = Convert.ToDecimal(GrdItems.Rows[e.NewEditIndex].Cells[5].Text);
            decimal Percent = (CommisionValue / Balance) * 100;
            txtCommissionValue.Text = Math.Round(Percent, 2).ToString().Replace(",", "");
            RbtCommissionType.SelectedValue = "1";
        }
        else
        {
            txtCommissionValue.Text = GrdItems.Rows[e.NewEditIndex].Cells[4].Text.Replace(",", "");
            RbtCommissionType.SelectedValue = "2";
        }
        txtBalance.Text = GrdItems.Rows[e.NewEditIndex].Cells[5].Text.Replace(",", "");
        btnAdd.Text = "Update";
        TotalTable.Visible = true;
    }


    public void LoadCategories()
    {
        //ddlCategory.Items.Clear();
        //ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        //ddlCategory.DataTextField = "CATEGORY_NAME";
        //ddlCategory.DataValueField = "CATEGORY_ID";
        //ddlCategory.DataBind();
        //ddlCategory.Items.Add(new ListItem("", "0"));
        //ddlCategory.SelectedIndex = 0;

        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
                      group rows by new { CATEGORY_ID = rows["CATEGORY_ID"], CATEGORY_NAME = rows["CATEGORY_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                ddlCategory.DataSource = Blocks;
                ddlCategory.DataTextField = "CATEGORY_NAME";
                ddlCategory.DataValueField = "CATEGORY_ID";
                ddlCategory.DataBind();
            }
            else
            {
                ddlCategory.Items.Clear();
                ddlCategory.Items.Add(new ListItem("", "0"));
                ddlCategory.SelectedIndex = 0;
            }
        }
        else
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("", "0"));
            ddlCategory.SelectedIndex = 0;
        }



    }
    public void LoadSubCategories()
    {
        //int ID = Convert.ToInt32(ddlCategory.SelectedValue);
        //var Qry = DropDowns.SUB_CATEGORYDropDown().Where(x => x.CATEGORY_ID == ID);
        //if (Qry.Any())
        //{
        //    ddlSubCategory.DataSource = Qry;
        //    ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
        //    ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
        //    ddlSubCategory.DataBind();
        //}
        //else
        //{
        //    ddlSubCategory.Items.Clear();
        //    ddlSubCategory.Items.Add(new ListItem("", "0"));
        //    ddlSubCategory.SelectedIndex = 0;
        //}
        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
                      group rows by new { SUB_CATEGORY_ID = rows["SUB_CATEGORY_ID"], SUB_CATEGORY_NAME = rows["SUB_CATEGORY_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                ddlSubCategory.DataSource = Blocks;
                ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
                ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
                ddlSubCategory.DataBind();
            }
            else
            {
                ddlSubCategory.Items.Clear();
                ddlSubCategory.Items.Add(new ListItem("", "0"));
                ddlSubCategory.SelectedIndex = 0;
            }
        }
        else
        {
            ddlSubCategory.Items.Clear();
            ddlSubCategory.Items.Add(new ListItem("", "0"));
            ddlSubCategory.SelectedIndex = 0;
        }
    }
    public void DetailCategoryLoad()
    {
        //var Qry = DropDowns.DETAIL_CATEGORYDropDown();
        //if (Qry.Any())
        //{
        //    ddlDetailCategory.DataSource = Qry;
        //    ddlDetailCategory.DataTextField = "DETAIL_CATEGORY_NAME";
        //    ddlDetailCategory.DataValueField = "DETAIL_CATEGORY_ID";
        //    ddlDetailCategory.DataBind();
        //}
        //else
        //{
        //    ddlDetailCategory.Items.Clear();
        //    ddlDetailCategory.Items.Add(new ListItem("", "0"));
        //    ddlDetailCategory.SelectedIndex = 0;
        //}
        ////DataTable dt = (DataTable)this.Session["dt"];
        ////if (dt.Rows.Count > 0)
        ////{
        ////    var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
        ////              group rows by new { DETAIL_CATEGORY_ID = rows["DETAIL_CATEGORY_ID"], DETAIL_CATEGORY_NAME = rows["DETAIL_CATEGORY_NAME"] } into grp
        ////              select grp.First();
        ////    if (qry.Any())
        ////    {
        ////        var Blocks = qry.CopyToDataTable();
        ////        ddlDetailCategory.DataSource = Blocks;
        ////        ddlDetailCategory.DataTextField = "DETAIL_CATEGORY_NAME";
        ////        ddlDetailCategory.DataValueField = "DETAIL_CATEGORY_ID";
        ////        ddlDetailCategory.DataBind();
        ////    }
        ////    else
        ////    {
        ////        ddlDetailCategory.Items.Clear();
        ////        ddlDetailCategory.Items.Add(new ListItem("", "0"));
        ////        ddlDetailCategory.SelectedIndex = 0;
        ////    }
        ////}
        ////else
        ////{
        ////    ddlDetailCategory.Items.Clear();
        ////    ddlDetailCategory.Items.Add(new ListItem("", "0"));
        ////    ddlDetailCategory.SelectedIndex = 0;
        ////}
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

    protected void ddlDetailCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
        LoadStreet();
        LoadItems("");
    }

    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadStreet();
        LoadItems("");
    }

    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
    }

    protected void lnkBtn_Click(object sender, EventArgs e)
    {

    }

    protected void grid_HeaderFilterFillItems(object sender, DevExpress.Web.ASPxGridViewHeaderFilterEventArgs e)
    {

    }
}