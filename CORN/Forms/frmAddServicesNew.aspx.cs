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
public partial class Forms_frmAddServicesNew : System.Web.UI.Page
{
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
            PanelServices.Visible = false;
            PanelGrid.Visible = true;
            LoadProjects();
            Distributor();
            CommissionAgent();
            //DATA(24);
            //LoadBlock("");
            //LoadStreet();
            //LoadItemInfo();
            LoadGrid("");

            DoEmptyTextBox();
            //txtLogId.Attributes.Add("autocomplete", "off");
            //txtPswd.Attributes.Add("autocomplete", "off");
            txtServiceDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
            RbChange();
            Panel1.Visible = true;
        }
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
        Session["ServiceID"] = null;
        LoadProjects();
        Distributor();
        //txtBlockNo.Text = "";
        txtCommisionAmount.Text = "";
        txtCommissionValue.Text = "";
        txtFloorNo.Text = "";
        txtItemAmount.Text = "";
        //txtItemName.Text = "";
        txtMeasurement.Text = "";
        txtOwnerAddress.Text = "";
        txtOwnerCNIC.Text = "";
        txtExtraArea.Text = "";
        txtOwnerContact.Text = "";
        txtOwnerEmail.Text = "";
        txtOwnerName.Text = "";
        txtOwnerRemarks.Text = "";
        txtPurchaserAddress.Text = "";
        txtPurchaserCNIC.Text = "";
        txtPurchaserContact.Text = "";
        txtPurchaserEmail.Text = "";
        txtPurchaserName.Text = "";
        txtPurchaserRemarks.Text = "";
        txtRemarks.Text = "";
        txtServiceDate.Text = DateTime.Now.ToString("dd/MMM/yyyy");
        //txtStreetNo.Text = "";
        //txtSearch.Text = "";
        Corner.Checked = false;
        FacingCommercial.Checked = false;
        FacingMosque.Checked = false;
        FacingPark.Checked = false;
        MainRoad.Checked = false;
        Other.Checked = false;
        btnSave.Text = "Save";
        ddlCommissionPercent.SelectedIndex = 0;
    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            //DbCalls db = new DbCalls();
            //var qry = (from k in db.SERVICES
            //           join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
            //           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
            //           join v in db.VENDORS on k.ITEM_NAME equals v.VENDOR_ID
            //           select new { d.DISTRIBUTOR_NAME, p.PROJECT_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive", k.ITEM_AMOUNT, k.OWNER_NAME, k.PURCHASER_NAME, k.SERVICE_ID }).Where(X => X.Active == "Active").ToList();
            DataTable dt = LController.SelectDataForSearching(50);
            Grid_users.DataSource = dt;
            Grid_users.DataBind();
            int total = 0;
            foreach (GridViewRow row in Grid_users.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                total += amount;
            }
            if (dt.Rows.Count > 0)
            {
                Grid_users.FooterRow.Cells[5].Text = string.Format("{0:n2}", total);
                Grid_users.FooterRow.Cells[4].Text = "Total";
                Grid_users.FooterRow.Cells[4].Font.Bold = true;
                Grid_users.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                Grid_users.FooterRow.Cells[5].Font.Bold = true;
                Grid_users.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                Grid_users.FooterRow.Cells[6].Visible = false;
                Grid_users.FooterRow.Cells[7].Visible = false;
            }
            
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(50);
            dt.DefaultView.RowFilter = "VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR CONVERT(ITEM_AMOUNT, System.String) LIKE '%" + txtSearch.Text + "%' OR CONVERT(DATE, System.String) LIKE '%" + txtSearch.Text + "%'";
            //OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR OWNER_NAME LIKE '%" + txtSearch.Text + "%' OR PURCHASER_NAME LIKE '%" + txtSearch.Text + "%' OR CONVERT(ITEM_AMOUNT, System.String) LIKE " + txtSearch.Text + "";
            if (dt.Rows.Count > 0)
            {
                Grid_users.PageIndex = 0;
            }
            Grid_users.DataSource = dt;
            Grid_users.DataBind();
            int total = 0;
            foreach (GridViewRow row in Grid_users.Rows)
            {
                int amount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                total += amount;
            }
            if (dt.Rows.Count > 0)
            {
                Grid_users.FooterRow.Cells[5].Text = string.Format("{0:n2}", total);
                Grid_users.FooterRow.Cells[4].Text = "Total";
                Grid_users.FooterRow.Cells[4].Font.Bold = true;
                Grid_users.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                Grid_users.FooterRow.Cells[5].Font.Bold = true;
                Grid_users.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                Grid_users.FooterRow.Cells[6].Visible = false;
                Grid_users.FooterRow.Cells[7].Visible = false;
            }

        }
    }
    protected void ClearControls()
    {
        try
        {
            DoEmptyTextBox();
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    public void LoadProjects()
    {
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedIndex = 0;
    }
    public void Distributor()
    {
        int UserID = Convert.ToInt32(Session["UserID"]);
        ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
        ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
        ddlLocation.DataValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
        ddlLocation.SelectedIndex = 0;
    }
    public void CommissionAgent()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.VENDORS
                   select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID }).Where(x => x.TYPE_ID == 3).ToList();
        DrpCommissionAgent.DataSource = qry;
        DrpCommissionAgent.DataTextField = "VENDOR_NAME";
        DrpCommissionAgent.DataValueField = "VENDOR_ID";
        DrpCommissionAgent.DataBind();
        DrpCommissionAgent.SelectedIndex = 0;
    }
    #region Removed
    //protected void LoadBlock(string Val)
    //{
    //    DataTable dt = (DataTable)this.Session["dt"];
    //    if (dt.Rows.Count > 0)
    //    {
    //        var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
    //                  group rows by new { BLOCK_ID = rows["BLOCK_ID"], BLOCK_NAME = rows["BLOCK_NAME"] } into grp
    //                  select grp.First();
    //        if (qry.Any())
    //        {
    //            var Blocks = qry.CopyToDataTable();
    //            ddlBlock.DataSource = Blocks;
    //            ddlBlock.DataTextField = "BLOCK_NAME";
    //            ddlBlock.DataValueField = "BLOCK_ID";
    //            ddlBlock.DataBind();
    //            //ddlBlock.SelectedIndex = 0;
    //        }
    //        else
    //        {
    //            ddlBlock.Items.Clear();
    //            ddlBlock.Items.Add(new ListItem("", "0"));
    //            ddlBlock.SelectedIndex = 0;
    //        }
    //    }
    //    else
    //    {
    //        ddlBlock.Items.Clear();
    //        ddlBlock.Items.Add(new ListItem("", "0"));
    //        ddlBlock.SelectedIndex = 0;
    //    }

    //}
    //protected void LoadStreet()
    //{
    //    DataTable dt = (DataTable)this.Session["dt"];
    //    if (dt.Rows.Count > 0)
    //    {
    //        var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue) && x.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue))
    //                  group rows by new { STREET_ID = rows["STREET_ID"], STREET_NAME = rows["STREET_NAME"] } into grp
    //                  select grp.First();
    //        if (qry.Any())
    //        {
    //            var Streets = qry.CopyToDataTable();
    //            ddlStreet.DataSource = Streets;
    //            ddlStreet.DataTextField = "STREET_NAME";
    //            ddlStreet.DataValueField = "STREET_ID";
    //            ddlStreet.DataBind();
    //            //ddlStreet.SelectedIndex = 0;
    //        }
    //        else
    //        {
    //            ddlStreet.Items.Clear();
    //            ddlStreet.Items.Add(new ListItem("", "0"));
    //            ddlStreet.SelectedIndex = 0;
    //        }
    //    }
    //    else
    //    {
    //        ddlStreet.Items.Clear();
    //        ddlStreet.Items.Add(new ListItem("", "0"));
    //        ddlStreet.SelectedIndex = 0;
    //    }

    //}
    //private void LoadItemInfo()
    //{
    //    DataTable dt = (DataTable)this.Session["dt"];
    //    if (dt.Rows.Count > 0)
    //    {
    //        var qry = dt.AsEnumerable()
    //              .Where(r => r.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue) && r.Field<int>("BLOCK_ID") == int.Parse(ddlBlock.SelectedValue) && r.Field<int>("STREET_ID") == int.Parse(ddlStreet.SelectedValue));
    //        if (qry.Any())
    //        {
    //            var Items = qry.CopyToDataTable();
    //            ddlItem.DataSource = Items;
    //            ddlItem.DataTextField = "ITEM";
    //            ddlItem.DataValueField = "REF_ID";
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
    //public void DATA(int Type)
    //{
    //    if (Type != 25)
    //    {
    //        this.Session["dt"] = null;
    //        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
    //        this.Session.Add("dt", dt);
    //    }
    //    else
    //    {
    //        this.Session["dtItem"] = null;
    //        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
    //        this.Session.Add("dtItem", dt);
    //    }

    //}
    #endregion

    #endregion

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearch.Text);
    }

    //protected void btnAdd_Click(object sender, EventArgs e)
    //{
    //    mPopUpLocation.Show();
    //    DoEmptyTextBox();
    //}

    //protected void btnActive_Click(object sender, EventArgs e)
    //{

    //}

    //protected void btnClose_ServerClick(object sender, EventArgs e)
    //{
    //    ClearControls();
    //    mPopUpLocation.Hide();
    //}

    protected void btnSave_Click(object sender, EventArgs e)
    {
        //if (ddlBlock.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
        //    return;
        //}
        //if (ddlStreet.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlStreet);
        //    return;
        //}
        //if (ddlItem.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
        //    return;
        //}
        if (txtCommisionAmount.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter commission amount.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtCommisionAmount);
            return;
        }
        //if (txtOwnerName.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter owner name.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtOwnerName);
        //    return;
        //}
        //if (txtOwnerCNIC.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter owner CNIC.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtOwnerCNIC);
        //    return;
        //}
        //if (txtOwnerContact.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter owner contact no.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtOwnerContact);
        //    return;
        //}
        //if (txtOwnerAddress.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter owner address.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtOwnerAddress);
        //    return;
        //}
        //if (txtPurchaserName.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter Purchaser name.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaserName);
        //    return;
        //}
        //if (txtPurchaserCNIC.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchaser CNIC.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaserCNIC);
        //    return;
        //}
        //if (txtPurchaserContact.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchaser contact no.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaserContact);
        //    return;
        //}
        //if (txtPurchaserAddress.Text.Equals(""))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter purchaser address.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(txtPurchaserAddress);
        //    return;
        //}
        if (txtCommisionAmount.Text.Equals(""))
        {
            txtCommisionAmount.Text = "0";
        }

        ServicesLogics Ser = new ServicesLogics();
        SERVICE s = new SERVICE();
        s.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
        s.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedItem.Value);
        s.DATE = Convert.ToDateTime(txtServiceDate.Text);
        s.ITEM_NAME = Convert.ToInt32(DrpCommissionAgent.SelectedItem.Value);
        s.STREET_NO = 0; //Convert.ToInt32(ddlStreet.SelectedValue);
        s.EXTRA_AREA = ""; //txtExtraArea.Text;
        s.BLOCK_NO = 0; //Convert.ToInt32(ddlBlock.SelectedValue);
        s.FLOOR_NO = ""; //txtFloorNo.Text;
        s.MEASUREMENT = ""; //txtMeasurement.Text;
        s.COMMISSION_RESOURCE = Convert.ToInt32(ddlCommissionResource.SelectedValue);
        s.ITEM_AMOUNT = decimal.Parse(txtCommisionAmount.Text);
        //s.comission = Convert.ToDecimal(txtItemAmount.Text);
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    s.COMMISSION_PERCENT = Convert.ToInt32(ddlCommissionPercent.SelectedValue);
        //    s.COMMISSION_VALUE = 0;
        //}
        //else
        //{
        //    s.COMMISSION_PERCENT = 0;
        //    s.COMMISSION_VALUE = Convert.ToInt32(txtCommissionValue.Text);
        //}

        s.COMMISSION_TYPE = Convert.ToInt32(ddlCommisionType.SelectedValue);
        s.REMARKS = txtRemarks.Text;
        s.OWNER_NAME = txtOwnerName.Text;
        s.OWNER_CNIC = txtOwnerCNIC.Text;
        s.OWNER_CONTACT = txtOwnerContact.Text;
        s.OWNER_EMAIL = txtOwnerEmail.Text;
        s.OWNER_ADDRESS = txtOwnerAddress.Text;
        s.OWNER_REMARKS = txtOwnerRemarks.Text;
        s.PURCHASER_ADDRESS = txtPurchaserAddress.Text;
        s.PURCHASER_CNIC = txtPurchaserCNIC.Text;
        s.PURCHASER_CONTACT = txtPurchaserContact.Text;
        s.PURCHASER_EMAIL = txtPurchaserEmail.Text;
        s.PURCHASER_NAME = txtPurchaserName.Text;
        s.PURCHASER_REMARKS = txtPurchaserRemarks.Text;
        s.Corner = false; //Corner.Checked;
        s.Commercial = false; //FacingCommercial.Checked;
        s.Mosque = false; //FacingMosque.Checked;
        s.Park = false; //FacingPark.Checked;
        s.Road = false; //MainRoad.Checked;
        s.Other = false; //Other.Checked;
        s.USER_ID = Convert.ToInt32(Session["UserID"]);
        s.TIME_STEMP = DateTime.Now;
        s.IS_DELETED = false;
        if (Session["ServiceID"] != null)
        {
            string VoucherNo = Convert.ToString(Session["VoucherNo"]);
            s.SERVICE_ID = Convert.ToInt32(Session["ServiceID"]);
            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(ddlLocation.SelectedItem.Value), Convert.ToDateTime(txtServiceDate.Text));
            s.VOUCHER_NO = VoucherNo2;
            Ser.Update(s);
            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
            {
                con.Open();
                using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VoucherNo + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                {
                    command1.ExecuteNonQuery();
                }
                using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VoucherNo + "' AND DISTRIBUTOR_ID ='" + Convert.ToInt32(ddlLocation.SelectedItem.Value) + "'", con))
                {
                    command2.ExecuteNonQuery();
                }
                con.Close();
            }

            LedgerEntries(s.DISTRIBUTOR_ID, 0, Convert.ToDecimal(s.ITEM_AMOUNT)
            , s.DATE, 0, Constants.Document_Acess
            , "", Convert.ToInt32(Session["UserID"]), s.SERVICE_ID
            , Convert.ToString(s.SERVICE_ID)
            , Convert.ToDecimal(0), Convert.ToDecimal(0), Convert.ToDecimal(0), s.REMARKS, Convert.ToDecimal(txtCommisionAmount.Text), s.VOUCHER_NO);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["ServiceID"] = null;
            Session["VoucherNo"] = null;
        }
        else
        {
            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, Convert.ToInt32(ddlLocation.SelectedItem.Value), Convert.ToDateTime(txtServiceDate.Text));
            s.VOUCHER_NO = VoucherNo2;
            Ser.Save(s);
            LedgerEntries(s.DISTRIBUTOR_ID, 0, Convert.ToDecimal(s.ITEM_AMOUNT)
                , s.DATE, 0, Constants.Document_Acess
                , "", Convert.ToInt32(Session["UserID"]), s.SERVICE_ID
                , Convert.ToString(s.SERVICE_ID)
                , Convert.ToDecimal(0), Convert.ToDecimal(0), Convert.ToDecimal(0), s.REMARKS, Convert.ToDecimal(txtCommisionAmount.Text), s.VOUCHER_NO);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
        }
        LoadGrid("");
        DoEmptyTextBox();
        PanelServices.Visible = false;
        PanelGrid.Visible = true;
        Panel1.Visible = true;
        btnSave.Text = "Save";

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        PanelServices.Visible = false;
        PanelGrid.Visible = true;
        //mPopUpLocation.Show();
        //LoadUser();
        ClearControls();
        Panel1.Visible = true;
        btnSave.Text = "Save";

    }

    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        Grid_users.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }


    protected void RbtCommissionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        RbChange();
    }
    public void RbChange()
    {
        if (RbtCommissionType.SelectedValue == "1")
        {
            txtCommissionValue.ReadOnly = true;
            ddlCommissionPercent.Enabled = true;
            ddlCommissionPercent.SelectedIndex = 0;
            txtCommissionValue.Text = "0";
            txtCommisionAmount.Text = "0";
        }
        else
        {
            ddlCommissionPercent.SelectedIndex = 0;
            txtCommissionValue.ReadOnly = false;
            ddlCommissionPercent.Enabled = false;
            txtCommisionAmount.Text = "0";
        }
    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Session["ServiceID"] = null;
        Session["VoucherNo"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        ServicesLogics Ser = new ServicesLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
        ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown();
        ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
        ddlLocation.DataValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
        ddlLocation.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);

        txtServiceDate.Text = data.DATE.ToString("dd-MMM-yyyy");


        //DataTable dt = new DataTable();
        //DataTable dtItem = new DataTable();
        //DATA(24);
        //DATA(25);
        //dtItem = (DataTable)this.Session["dtItem"];
        //DataRow[] foundRows = dtItem.Select("REF_ID  = '" + data.ITEM_NAME + "'");
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
        //    dr["ITEM"] = foundRows[0]["ITEM"].ToString();
        //    dr["Consumption"] = foundRows[0]["Consumption"].ToString();
        //    dr["REF_ID"] = foundRows[0]["REF_ID"].ToString();
        //    dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
        //    dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
        //    dt.Rows.Add(dr);
        //    Session["dt"] = dt;
        //}
        //LoadBlock("");
        //ddlBlock.SelectedValue = Convert.ToString(data.BLOCK_NO);
        //LoadStreet();
        //ddlStreet.SelectedValue = Convert.ToString(data.STREET_NO);
        //LoadItemInfo();
        //ddlItem.SelectedValue = Convert.ToString(data.ITEM_NAME);
        ddlCommissionResource.SelectedValue = Convert.ToString(data.COMMISSION_RESOURCE);
        //txtFloorNo.Text = data.FLOOR_NO;
        //txtMeasurement.Text = data.MEASUREMENT;
        //txtExtraArea.Text = data.EXTRA_AREA;
        txtRemarks.Text = data.REMARKS;
        //decimal ItemAmount = Convert.ToDecimal(data.ITEM_AMOUNT);
        //txtItemAmount.Text = Convert.ToString(Math.Round(ItemAmount, 2));

        //ddlCommissionPercent.SelectedValue = Convert.ToString(data.COMMISSION_PERCENT);
        //if (ddlCommissionPercent.SelectedValue != "0")
        //{
        //    txtCommissionValue.Text = "0";
        //    decimal amount = Convert.ToDecimal(data.ITEM_AMOUNT / 100 * data.COMMISSION_PERCENT);
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

        txtCommisionAmount.Text = Convert.ToString(data.ITEM_AMOUNT);
        DrpCommissionAgent.SelectedValue = Convert.ToString(data.ITEM_NAME);
        ddlCommisionType.SelectedValue = Convert.ToString(data.COMMISSION_TYPE);
        txtOwnerAddress.Text = data.OWNER_ADDRESS;
        txtOwnerCNIC.Text = data.OWNER_CNIC;
        txtOwnerContact.Text = data.OWNER_CONTACT;
        txtOwnerEmail.Text = data.OWNER_EMAIL;
        txtOwnerName.Text = data.OWNER_NAME;
        txtOwnerRemarks.Text = data.OWNER_REMARKS;

        txtPurchaserAddress.Text = data.PURCHASER_ADDRESS;
        txtPurchaserCNIC.Text = data.PURCHASER_CNIC;
        txtPurchaserContact.Text = data.PURCHASER_CONTACT;
        txtPurchaserEmail.Text = data.PURCHASER_EMAIL;
        txtPurchaserName.Text = data.PURCHASER_NAME;
        txtPurchaserRemarks.Text = data.PURCHASER_REMARKS;

        //Corner.Checked = Convert.ToBoolean(data.Corner);
        //FacingCommercial.Checked = Convert.ToBoolean(data.Commercial);
        //FacingMosque.Checked = Convert.ToBoolean(data.Mosque);
        //FacingPark.Checked = Convert.ToBoolean(data.Park);
        //MainRoad.Checked = Convert.ToBoolean(data.Road);
        //Other.Checked = Convert.ToBoolean(data.Other);
        Session["VoucherNo"] = data.VOUCHER_NO;
        Session.Add("ServiceID", data.SERVICE_ID);
        PanelServices.Visible = true;
        PanelGrid.Visible = false;
        Panel1.Visible = false;
        btnSave.Text = "Update";
    }

    protected void ddlCommissionPercent_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    if (txtItemAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter item amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtItemAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
        //        decimal itemamount = Convert.ToDecimal(txtItemAmount.Text);
        //        txtCommissionValue.Text = "0";
        //        decimal Amount = itemamount / 100 * Percent;
        //        txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
        //        ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
        //    }
        //}
        //else
        //{
        //    if (txtItemAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter item amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtItemAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        txtCommissionValue_TextChanged(null, null);
        //    }
        //}
    }

    protected void txtCommissionValue_TextChanged(object sender, EventArgs e)
    {
        //string Commission = txtCommissionValue.Text;
        //txtCommisionAmount.Text = Commission;
        //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
    }

    protected void txtItemAmount_TextChanged(object sender, EventArgs e)
    {
        //if (RbtCommissionType.SelectedValue == "1")
        //{
        //    if (txtItemAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter item amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtItemAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        decimal Percent = Convert.ToDecimal(ddlCommissionPercent.SelectedValue);
        //        decimal itemamount = Convert.ToDecimal(txtItemAmount.Text);
        //        txtCommissionValue.Text = "0";
        //        decimal Amount = itemamount / 100 * Percent;
        //        txtCommisionAmount.Text = Math.Round(Amount, 2).ToString();
        //        ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
        //    }
        //}
        //else
        //{
        //    if (txtItemAmount.Text.Equals(""))
        //    {
        //        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Please enter item amount first.');", true);
        //        ScriptManager.GetCurrent(Page).SetFocus(txtItemAmount);
        //        ddlCommissionPercent.SelectedValue = "0";
        //        return;
        //    }
        //    else
        //    {
        //        if (txtCommissionValue.Text.Equals(""))
        //        {
        //            txtCommissionValue.Text = "0";
        //            txtCommisionAmount.Text = "0";
        //            //ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
        //        }
        //        else
        //        {
        //            string Commission = txtCommissionValue.Text;
        //            txtCommisionAmount.Text = Commission;
        //            ScriptManager.GetCurrent(Page).SetFocus(txtRemarks);
        //        }

        //    }
        //}
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        ServicesLogics Ser = new ServicesLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        string VoucherNo = Convert.ToString(data.VOUCHER_NO);
        int DISTRIBUTOR_ID = Convert.ToInt32(data.DISTRIBUTOR_ID);
        try
        {
            if (!Active)
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update SERVICES set IS_DELETED = 1 where SERVICE_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VoucherNo + "' AND DISTRIBUTOR_ID ='" + DISTRIBUTOR_ID + "'", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL SET IS_DELETED = 1 WHERE VOUCHER_NO ='" + VoucherNo + "' AND DISTRIBUTOR_ID ='" + DISTRIBUTOR_ID + "'", con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }

            }
        }
        catch (Exception ex) { }
        LoadGrid("");
    }

    //protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    LoadItems();
    //}

    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        PanelServices.Visible = true;
        PanelGrid.Visible = false;
        Panel1.Visible = false;
    }

    private void LedgerEntries(int p_Distributor_Id
      , decimal p_Debit, decimal p_Credit, DateTime p_Ledger_Date, int p_PRINCIPAL_ID
      , int p_DOCUMENT_TYPE_ID, string pChequeNo, int p_UserId, long p_DOCUMENT_NO
      , string p_MANUAL_DOCUMENT_NO, decimal p_Discount_Amount, decimal p_TokenAmount, decimal p_DownPayment, string p_Remarks, decimal p_Commisson, string VoucherNo2)
    {
        try
        {
            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);
            #region GL Master, Detail

            LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Acess
                , p_MANUAL_DOCUMENT_NO, "Service Voucher / Seller: " + txtOwnerName.Text + ",  Purchaser: " + txtPurchaserName.Text + ", " + p_Remarks, p_UserId, "Service", mTransaction, mConnection);
            decimal Sales = p_Credit - p_Discount_Amount;

            if (ddlCommisionType.SelectedValue == "0")
            {
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sale_Of_Services, 0, p_Commisson, "Service Voucher Commission Receivable", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Commission_Receivable, p_Commisson, 0, "Service Voucher Commission Receivable", mTransaction, mConnection);
            }
            else
            {
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Commission_Charges, p_Commisson, 0, "Service Voucher Commission Payable", mTransaction, mConnection);
                LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, 0, p_Commisson, "Service Voucher Commission Payable", mTransaction, mConnection);
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


    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadStreet();
        //LoadItemInfo();
        //ddlStreet.Focus();
    }

    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadItemInfo();
        //ddlItem.Focus();
    }
}