using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmItemInformation : System.Web.UI.Page
{
    public bool chk = false;
    public int Possession;
    readonly LedgerController LController = new LedgerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            LoadProjects();
            LoadGrid("");

            DoEmptyTextBox();
            LoadCategories();
            LoadSubCategories();
            LoadMeausrment();
            Blocks(27);
            LoadBlock("");
            StreetLoad();
            PhaseLoad();
            SectorLoad();
            PrecentLoad();
            DetailCategoryLoad();
            InventoryTypeLoad();
            PrecentStreetLoad();
            FacingType();
            //txtLogId.Attributes.Add("autocomplete", "off");
            //txtPswd.Attributes.Add("autocomplete", "off");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
            ChkPossession.Checked = false;
            ChkNonPossession.Checked = false;

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
        txtSearch.Text = "";
        //txtBlockNo.Text = "";
        txtNo_Of_Floor.Text = "";
        txtItemName.Text = "";
        txtExtraArea.Text = "";
        txtOpenRegistration.Text = "Not Applicable";
        txtFormNo.Text = "Not Applicable";
        txtTowerNo.Text = "Not Applicable";
        txtFloorNo.Text = "Not Applicable";
        txtNo_Of_Floor.Text = "Not Applicable";
        txtNo_Of_Beds.Text = "Not Applicable";
        txtExtraArea.Text = "Not Applicable";
        txtRegisteredName.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        FacingType();
        //Corner.Checked = false;
        //FacingCommercial.Checked = false;
        //FacingMosque.Checked = false;
        //FacingPark.Checked = false;
        //MainRoad.Checked = false;
        //Other.Checked = false;
        LoadProjects();
        LoadCategories();
        LoadSubCategories();
        LoadMeausrment();

        ddlProject.Enabled = true;
        ddlCategory.Enabled = true;
        ddlSubCategory.Enabled = true;
        ddlBlock.Enabled = true;
        ddlStreet.Enabled = true;
        ddlInventoryType.Enabled = true;
        ddlPhase.Enabled = true;
        ddlSector.Enabled = true;
        ddlDetailCategory.Enabled = true;

        //txtFormNo.Text = "";
        //txtFloorNo.Text = "";
        //txtTowerNo.Text = "";
        //txtNo_Of_Beds.Text = "";
        ChkPossession.Checked = false;
        ChkNonPossession.Checked = false;
        ddlVillaType.SelectedValue = "1";

        PhaseLoad();
        SectorLoad();
        PrecentLoad();
        Session["Item"] = null;
        Session["FORM_NO"] = null;
        Session["OPEN_FROM_REGISTRATION"] = null;
    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {

            //////////////// FOR HORIZON ITEMS ///////////////////////

            DbCalls db = new DbCalls();
            var qry = (from k in db.ITEM_INFORMATION
                       join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
                       join c in db.CATEGORies on k.CATEGORY_ID equals c.CATEGORY_ID
                       join m in db.MEASURMENTs on k.MEASUREMENT equals m.MEASURMENT_ID
                       join sc in db.SUB_CATEGORY on k.SUB_CATEGORY_ID equals sc.SUB_CATEGORY_ID
                       join dc in db.DETAIL_CATEGORY on k.DETAIL_CATEGORY_ID equals dc.DETAIL_CATEGORY_ID
                       join it in db.INVENTORYTYPEs on k.INVENTORY_TYPE equals it.INVENTORYTYPE_ID
                       from b in db.BLOCKs
                            .Where(x => x.BLOCK_ID == k.BLOCK_ID)
                            .DefaultIfEmpty() // <== makes join left join

                       from s in db.STREETs
                           .Where(x => x.STREET_ID == k.STREET_ID)
                           .DefaultIfEmpty() // <== makes join left join

                           //join b in db.BLOCK on k.BLOCK_ID equals b.BLOCK_ID
                           //join s in db.STREET on k.STREET_ID equals s.STREET_ID
                       select new { p.PROJECT_NAME, c.CATEGORY_NAME, sc.SUB_CATEGORY_NAME, dc.DETAIL_CATEGORY_NAME, it.INVENTORYTYPE_NAME, k.ITEM_INFO_ID, k.ITEM_NAME,
                           //BLOCK_NO = b.BLOCK_NAME,
                           //STREET_NO = s.STREET_NAME,
                           BLOCK_NO = b.BLOCK_NAME == null ? "Not Applicable" : b.BLOCK_NAME,
                           STREET_NO = s.STREET_NAME == null ? "Not Applicable" : s.STREET_NAME,
                           MEASUREMENT = m.MEASURMENT1, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
            Grid_users.DataSource = qry;
            Grid_users.DataBind();


            //////////////// FOR ATHAR ITEMS ///////////////////////

            //      DbCalls db = new DbCalls();
            //      var query2 = (
            //    from i in db.ITEM_INFORMATION
            //    join p in db.PROJECTs on i.PROJECT_ID equals p.PROJECT_ID
            //    join c in db.CATEGORies on i.CATEGORY_ID equals c.CATEGORY_ID
            //    join m in db.MEASURMENT on i.MEASUREMENT equals m.MEASURMENT_ID
            //    join sc in db.SUB_CATEGORY on i.SUB_CATEGORY_ID equals sc.SUB_CATEGORY_ID

            //    from b in db.BLOCK
            //         .Where(x => x.BLOCK_ID == i.BLOCK_ID)
            //         .DefaultIfEmpty() // <== makes join left join

            //from s in db.STREET
            //    .Where(x => x.STREET_ID == i.STREET_ID)
            //    .DefaultIfEmpty() // <== makes join left join

            ////from pr in db.PRECENTs
            ////    .Where(x => x.PRECENT_ID == i.PRECENT_ID)
            ////    .DefaultIfEmpty() // <== makes join left join

            ////from prs in db.PRECENTSTREETs
            ////    .Where(x => x.PRECENTSTREET_ID == i.PRECENTSTREET_ID)
            ////    .DefaultIfEmpty() // <== makes join left join

            ////from sc in db.SECTORs
            ////    .Where(x => x.SECTOR_ID == i.SECTOR_ID)
            ////    .DefaultIfEmpty() // <== makes join left join

            //select new
            //    {
            //        p.PROJECT_NAME,
            //        c.CATEGORY_NAME,
            //        sc.SUB_CATEGORY_NAME,
            //        i.ITEM_INFO_ID,
            //        i.ITEM_NAME,
            //        p.PROJECT_ID,
            //    BLOCK_NO = b.BLOCK_NAME == null ? "Not Applicable" : b.BLOCK_NAME,
            //    STREET_NO = s.STREET_NAME == null ? "Not Applicable" : s.STREET_NAME,
            //        //PRECENT_NAME = pr.PRECENT_NAME == null ? "N/A" : pr.PRECENT_NAME,
            //        //PRECENTSTREET_NAME = prs.PRECENTSTREET_NAME == null ? "N/A" : prs.PRECENTSTREET_NAME,
            //        //SECTOR_NAME = sc.SECTOR_NAME == null ? "N/A" : sc.SECTOR_NAME,
            //        //c.CATEGORY_ID,
            //        //sc.SUB_CATEGORY_ID
            //        MEASUREMENT = m.MEASURMENT1,
            //        Active = i.IS_DELETED == false ? "Active" : "Inactive"
            //}
            //);
            //      //var xy = (query2).Where(x => x.PROJECT_ID == PROJECTID && x.CATEGORY_ID == CATEGORYID && x.SUB_CATEGORY_ID == SUBCATEGORYID).ToList();
            //      var xy = (query2).ToList();
            //      Grid_users.DataSource = xy;
            //      Grid_users.DataBind();
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(8);
            dt.DefaultView.RowFilter = "PROJECT_NAME LIKE '%" + txtSearch.Text + "%'  OR CATEGORY_NAME LIKE '%" + txtSearch.Text + "%' OR SUB_CATEGORY_NAME LIKE '%" + txtSearch.Text + "%' OR DETAIL_CATEGORY_NAME LIKE '%" + txtSearch.Text + "%' OR INVENTORYTYPE_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR BLOCK_NO LIKE '%" + txtSearch.Text + "%' OR STREET_NO LIKE '%" + txtSearch.Text + "%' OR MEASUREMENT LIKE '%" + txtSearch.Text + "%'";
            if (dt.Rows.Count > 0)
            {
                Grid_users.PageIndex = 0;
            }
            Grid_users.DataSource = dt;
            Grid_users.DataBind();
        }
    }
    public void Blocks(int Type)
    {
         this.Session["blocks"] = null;
         DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
         this.Session.Add("blocks", dt);
        
    }
    protected void LoadBlock(string Val)
    {
        DataTable dt = (DataTable)this.Session["blocks"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable().Where(x => x.Field<int>("PROJECT_ID") == int.Parse(ddlProject.SelectedValue))
                      group rows by new { BLOCK_ID = rows["BLOCK_ID"], BLOCK_NAME = rows["BLOCK_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                ddlBlock.Items.Clear();
                ddlBlock.DataSource = Blocks;
                ddlBlock.DataTextField = "BLOCK_NAME";
                ddlBlock.DataValueField = "BLOCK_ID";
                ddlBlock.DataBind();
                ListItem myDefaultItem = new ListItem("Not Applicable", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlBlock.Items.Insert(0, myDefaultItem);
            }
            else
            {
                ddlBlock.Items.Clear();
                ddlBlock.Items.Add(new ListItem("Not Applicable", "0"));
                ddlBlock.SelectedIndex = 0;
            }
        }
        else
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Add(new ListItem("Not Applicable", "0"));
            ddlBlock.SelectedIndex = 0;
        }
    }
    protected void ClearControls()
    {
        try
        {

            
            txtSearch.Text = "";
            //txtBlockNo.Text = "";
            txtNo_Of_Floor.Text = "";
            txtItemName.Text = "";
            //txtMeasurement.Text = "";
            //txtStreetNo.Text = "";
            txtRemarks.Text = "";
            btnSave.Text = "Save";
            FacingType();
            txtOpenRegistration.Text = "Not Applicable";
            txtFormNo.Text = "Not Applicable";
            txtTowerNo.Text = "Not Applicable";
            txtFloorNo.Text = "Not Applicable";
            txtNo_Of_Floor.Text = "Not Applicable";
            txtNo_Of_Beds.Text = "Not Applicable";
            txtExtraArea.Text = "Not Applicable";
            //Corner.Checked = false;
            //FacingCommercial.Checked = false;
            //FacingMosque.Checked = false;
            //FacingPark.Checked = false;
            //MainRoad.Checked = false;
            //Other.Checked = false;
            ddlProject.Enabled = true;
            ddlCategory.Enabled = true;
            ddlSubCategory.Enabled = true;
            ddlBlock.Enabled = true;
            ddlStreet.Enabled = true;
            ddlInventoryType.Enabled = true;
            ddlPhase.Enabled = true;
            ddlSector.Enabled = true;
            ddlDetailCategory.Enabled = true;
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    public void LoadProjects()
    {
        ddlProject.Items.Clear();
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        //ddlProject.Items.Add(new ListItem("", "0"));
        //ddlProject.SelectedIndex = 0;
    }
    public void LoadCategories()
    {
        ddlCategory.Items.Clear();
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        //ddlCategory.Items.Add(new ListItem("", "0"));
        //ddlCategory.SelectedIndex = 0;
        
        

    }
    public void LoadSubCategories()
    {
        int ID = Convert.ToInt32(ddlCategory.SelectedValue);
        var Qry = DropDowns.SUB_CATEGORYDropDown().Where(x => x.CATEGORY_ID == ID);
        


        if (Qry.Any())
        {
            ddlSubCategory.DataSource = Qry;
            ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
            ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
            ddlSubCategory.DataBind();
        }
        else
        {
            //ddlSubCategory.Items.Clear();
            //ddlSubCategory.Items.Add(new ListItem("", "0"));
            //ddlSubCategory.SelectedIndex = 0;
        }
    }
    public void LoadMeausrment()
    {
        //int ID = Convert.ToInt32(ddlSubCategory.SelectedValue);
        ddlMeasurment.DataSource = DropDowns.MEASURMENTDropDown();
        ddlMeasurment.DataTextField = "MEASURMENT1";
        ddlMeasurment.DataValueField = "MEASURMENT_ID";
        ddlMeasurment.DataBind();
        //ddlMeasurment.Items.Add(new ListItem("--Select Measurment--", "0"));
    }
    //public void BlockLoad()
    //{
    //    int ProID = Convert.ToInt32(ddlProject.SelectedValue);
    //    var qry = DropDowns.BLOCKDropDown().Where(x => x.PROJECT_ID == ProID);
    //    if (qry.Any())
    //    {
    //        ddlBlock.DataSource = qry;
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
    public void StreetLoad()
    {
        int BlockID = Convert.ToInt32(ddlBlock.SelectedValue);
        var qry = DropDowns.STREETDropDown().Where(x => x.BLOCK_ID == BlockID);
       
        if (qry.Any())
        {
            ddlStreet.DataSource = qry;
            ddlStreet.DataTextField = "STREET_NAME";
            ddlStreet.DataValueField = "STREET_ID";
            ddlStreet.DataBind();
        }
        else
        {
            ddlStreet.Items.Clear();
            ddlStreet.Items.Add(new ListItem("Not Applicable", "0"));
            ddlStreet.SelectedIndex = 0;
        }
    }

    public void PhaseLoad()
    {
        var qry = DropDowns.PHASEDropDown();
        if (qry.Any())
        {
            ddlPhase.DataSource = qry;
            ddlPhase.DataTextField = "PHASE_NAME";
            ddlPhase.DataValueField = "PHASE_ID";
            ddlPhase.DataBind();
        }
        else
        {
            ddlPhase.Items.Clear();
            ddlPhase.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPhase.SelectedIndex = 0;
        }
    }
    public void SectorLoad()
    {
        var qry = DropDowns.SECTORDropDown();
        if (qry.Any())
        {
            ddlSector.DataSource = qry;
            ddlSector.DataTextField = "SECTOR_NAME";
            ddlSector.DataValueField = "SECTOR_ID";
            ddlSector.DataBind();
            ListItem myDefaultItem = new ListItem("Not Applicable", "0");
            myDefaultItem.Selected = true;
            ddlSector.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlSector.Items.Clear();
            ListItem myDefaultItem = new ListItem("Not Applicable", "0");
            myDefaultItem.Selected = true;
            ddlSector.Items.Insert(0, myDefaultItem);
        }
    }
    public void PrecentLoad()
    {
        var qry = DropDowns.PRECENTDropDown();
        if (qry.Any())
        {
            ddlPrecent.DataSource = qry;
            ddlPrecent.DataTextField = "PRECENT_NAME";
            ddlPrecent.DataValueField = "PRECENT_ID";
            ddlPrecent.DataBind();
            ListItem myDefaultItem = new ListItem("Not Applicable", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPrecent.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlPrecent.Items.Clear();
            ddlPrecent.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecent.SelectedIndex = 0;
        }
    }
    public void PrecentStreetLoad()
    {
        var qry = DropDowns.PRECENTSTREETDropDown();
        if (qry.Any())
        {
            ddlPrecentStreet.DataSource = qry;
            ddlPrecentStreet.DataTextField = "PRECENTSTREET_NAME";
            ddlPrecentStreet.DataValueField = "PRECENTSTREET_ID";
            ddlPrecentStreet.DataBind();
            ListItem myDefaultItem = new ListItem("Not Applicable", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPrecentStreet.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlPrecentStreet.Items.Clear();
            ddlPrecentStreet.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecentStreet.SelectedIndex = 0;
        }
    }
    public void DetailCategoryLoad()
    {
        var qry = DropDowns.DETAIL_CATEGORYDropDown();
        if (qry.Any())
        {
            ddlDetailCategory.DataSource = qry;
            ddlDetailCategory.DataTextField = "DETAIL_CATEGORY_NAME";
            ddlDetailCategory.DataValueField = "DETAIL_CATEGORY_ID";
            ddlDetailCategory.DataBind();
        }
        else
        {
            //ddlDetailCategory.Items.Clear();
            //ddlDetailCategory.Items.Add(new ListItem("Not Applicable", "0"));
            //ddlDetailCategory.SelectedIndex = 0;
        }
    }
    public void InventoryTypeLoad()
    {
        var qry = DropDowns.INVENTORYTYPEDropDown();
        if (qry.Any())
        {
            ddlItemType.DataSource = qry;
            ddlItemType.DataTextField = "INVENTORYTYPE_NAME";
            ddlItemType.DataValueField = "INVENTORYTYPE_ID";
            ddlItemType.DataBind();
        }
        else
        {
            //ddlItemType.Items.Clear();
            //ddlItemType.Items.Add(new ListItem("Not Applicable", "0"));
            //ddlItemType.SelectedIndex = 0;
        }
    }

    public void FacingType()
    {
        lstFacingType.Items.Clear();
        DbCalls db = new DbCalls();
        var qry = (from k in db.FACING_TYPE
                   select new { k.FACING_TYPE_ID, k.FACING_TYPE_NAME ,k.IS_DELETED }).Where(x => x.IS_DELETED == false).ToList();
        lstFacingType.DataSource = qry.OrderBy(x => x.FACING_TYPE_NAME);
        lstFacingType.DataTextField = "FACING_TYPE_NAME";
        lstFacingType.DataValueField = "FACING_TYPE_ID";
        lstFacingType.DataBind();
    }

    #endregion
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearch.Text);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        DoEmptyTextBox();
    }

    protected void btnActive_Click(object sender, EventArgs e)
    {
        bool check = false;
        try
        {
            foreach (GridViewRow dr2 in Grid_users.Rows)
            {
                var chRelized2 = (CheckBox)dr2.Cells[0].FindControl("ChbIsAssigned");

                if (chRelized2.Checked)
                {
                    check = true;
                    break;
                }

            }
            if (!check)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select record first');", true);
                return;
            }

            bool flag = false;
            foreach (GridViewRow dr in Grid_users.Rows)
            {
                var chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");


                if (chRelized.Checked)
                {
                    if (Convert.ToString(dr.Cells[8].Text) == "Active")
                    {
                        int ID = Convert.ToInt32(dr.Cells[9].Text);
                        try
                        {
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update ITEM_INFORMATION set IS_DELETED = 1 where ITEM_INFO_ID=" + ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
                        }

                    }
                    else
                    {
                        int ID = Convert.ToInt32(dr.Cells[9].Text);
                        try
                        {
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update ITEM_INFORMATION set IS_DELETED = 0 where ITEM_INFO_ID=" + ID, con))
                                {
                                    command.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
                        }

                    }
                }
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
                this.LoadGrid("");
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        ClearControls();
        mPopUpLocation.Hide();
        Session["Item"] = null;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {

        if (ddlProject.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlProject);
            mPopUpLocation.Show();
            return;
        }
        if (ddlCategory.SelectedItem.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCategory);
            mPopUpLocation.Show();
            return;
        }
        if (ddlSubCategory.SelectedItem.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlSubCategory);
            mPopUpLocation.Show();
            return;
        }
        if (ddlMeasurment.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Measurment is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlMeasurment);
            mPopUpLocation.Show();
            return;
        }
        //if (ddlBlock.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlBlock);
        //    mPopUpLocation.Show();
        //    return;
        //}
        //if (ddlStreet.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlStreet);
        //    mPopUpLocation.Show();
        //    return;
        //}
        //if (ddlPhase.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Phase is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlPhase);
        //    mPopUpLocation.Show();
        //    return;
        //}
        //if (ddlSector.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sector is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlSector);
        //    mPopUpLocation.Show();
        //    return;
        //}
        //if (ddlPrecent.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Precent is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlPrecent);
        //    mPopUpLocation.Show();
        //    return;
        //}
        //if (ddlPrecentStreet.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Precent Street is required.');", true);
        //    ScriptManager.GetCurrent(Page).SetFocus(ddlPrecentStreet);
        //    mPopUpLocation.Show();
        //    return;
        //}
        

        DbCalls db = new DbCalls();
        int PROJECT_ID = Convert.ToInt32(ddlProject.SelectedValue);
        if (ddlDetailCategory.SelectedItem.Text == "Open Forms")
        {
            if (!txtOpenRegistration.Text.Equals(""))
            {
                if (Session["OPEN_FROM_REGISTRATION"] != null)
                {
                    if (Session["OPEN_FROM_REGISTRATION"].ToString() == txtOpenRegistration.Text)
                    {

                    }
                    else
                    {
                        if (txtOpenRegistration.Text == "Not Applicable")
                        {

                        }
                        else
                        {
                            var chek = (from k in db.ITEM_INFORMATION select k).Where(x => x.OPEN_FROM_REGISTRATION == txtOpenRegistration.Text);
                            if (chek.Any())
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Open Form Registration # " + txtOpenRegistration.Text + " already added. Please add an other number.');", true);
                                mPopUpLocation.Show();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (txtOpenRegistration.Text == "Not Applicable")
                    {

                    }
                    else
                    {
                        var chek = (from k in db.ITEM_INFORMATION select k).Where(x => x.OPEN_FROM_REGISTRATION == txtOpenRegistration.Text);
                        if (chek.Any())
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Open Form Registration # " + txtOpenRegistration.Text + " already added. Please add an other number.');", true);
                            mPopUpLocation.Show();
                            return;
                        }
                    }
                }
            }
        }
        if (Session["FORM_NO"] != null)
        {
            if (Session["FORM_NO"].ToString() == txtFormNo.Text)
            {

            }
            else
            {
                var chek = (from k in db.ITEM_INFORMATION select k).Where(x => x.FORM_NO == txtFormNo.Text);
                if (chek.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Form # " + txtFormNo.Text + " already added. Please add an other number.');", true);
                    mPopUpLocation.Show();
                    return;
                }
            }
        }
        if (ChkPossession.Checked)
        {
            Possession = 1;
        }
        else if (ChkNonPossession.Checked)
        {
            Possession = 2;
        }
        else
        {
            Possession = 0;
        }
        ChkPossession.Checked = true;
        ItemLogics Ser = new ItemLogics();
        ITEM_INFORMATION i = new ITEM_INFORMATION();
        i.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedItem.Value);
        i.CATEGORY_ID = Convert.ToInt32(ddlCategory.SelectedItem.Value);
        i.SUB_CATEGORY_ID = Convert.ToInt32(ddlSubCategory.SelectedItem.Value);
        i.INVENTORY_TYPE = Convert.ToInt32(ddlInventoryType.SelectedItem.Value);
        i.PHASE_ID = Convert.ToInt32(ddlPhase.SelectedItem.Value);
        i.SECTOR_ID = Convert.ToInt32(ddlSector.SelectedItem.Value);
        i.PRECENT_ID = Convert.ToInt32(ddlPrecent.SelectedItem.Value);
        i.PAID_INSTALLMENT = Convert.ToInt32(ddlPaidInstallment.SelectedItem.Value);
        i.DETAIL_CATEGORY_ID = Convert.ToInt32(ddlDetailCategory.SelectedItem.Value);
        i.ITEM_TYPE_ID = Convert.ToInt32(ddlItemType.SelectedItem.Value);
        i.PRECENTSTREET_ID = Convert.ToInt32(ddlPrecentStreet.SelectedItem.Value);
        i.OPEN_FROM_REGISTRATION = txtOpenRegistration.Text;
        i.ITEM_NAME = txtItemName.Text;
        i.STREET_ID = Convert.ToInt32(ddlStreet.SelectedItem.Value);
        i.BLOCK_ID = Convert.ToInt32(ddlBlock.SelectedItem.Value);
        i.NO_OF_FLOORS = txtNo_Of_Floor.Text;
        i.MEASUREMENT = Convert.ToInt32(ddlMeasurment.SelectedValue);
        i.EXTRA_AREA = txtExtraArea.Text;
        i.REGISTERED_NAME = txtRegisteredName.Text;
        i.REMARKS = txtRemarks.Text;
        i.USER_ID = Convert.ToInt32(Session["UserID"]);
        i.IS_DELETED = false;
        i.TIME_STEMP = DateTime.Now;
        //i.Corner = Corner.Checked;
        //i.Commercial = FacingCommercial.Checked;
        //i.Mosque = FacingMosque.Checked;
        //i.Park = FacingPark.Checked;
        //i.Road = MainRoad.Checked;
        //i.Other = Other.Checked;

        i.VILLA_TYPE = Convert.ToInt32(ddlVillaType.SelectedValue);
        i.FORM_NO = txtFormNo.Text;
        i.FLOOR_NO = txtFloorNo.Text;
        i.TOWER_NO = txtTowerNo.Text;
        i.NO_OF_BEDS= txtNo_Of_Beds.Text;
        i.POSSESSION = Possession;

        if (Session["Item"] != null)
        {
            i.ITEM_INFO_ID = Convert.ToInt32(Session["Item"]);
            Ser.Update(i);
            DeleteItemFacing(i.ITEM_INFO_ID);
            ItemFacing(i.ITEM_INFO_ID);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["Item"] = null;
        }
        else
        {
            if (txtOpenRegistration.Text == "Not Applicable")
            {
                Ser.Save(i);
                ItemFacing(i.ITEM_INFO_ID);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
            else
            {
                var chek = (from k in db.ITEM_INFORMATION select k).Where(x => x.ITEM_NAME == txtItemName.Text && x.PROJECT_ID == PROJECT_ID);
                if (chek.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item Name " + txtItemName.Text + " already added for " + ddlProject.SelectedItem.Text + ". Please add an other Item.');", true);
                    mPopUpLocation.Show();
                    return;
                }
                else
                {
                    Ser.Save(i);
                    ItemFacing(i.ITEM_INFO_ID);
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
                }
            }
            
        }
        LoadGrid("");
        DoEmptyTextBox();
        mPopUpLocation.Show();
    }


    public void ItemFacing(int ITEM_INFO_ID)
    {
        List<ListItem> selected = new List<ListItem>();
        foreach (ListItem item in lstFacingType.Items)
        {
            if (item.Selected)
            {
                ItemFacingLogics Ifl = new ItemFacingLogics();
                ITEM_FACING sa = new ITEM_FACING();
                sa.FACING_TYPE_ID = Convert.ToInt32(item.Value);
                sa.ITEM_INFO_ID = ITEM_INFO_ID;
                Ifl.Save(sa);
            }
        }
    }
    public void DeleteItemFacing(int ProID)
    {
        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand("DELETE ITEM_FACING where ITEM_INFO_ID =" + Convert.ToInt32(ProID), con))
            {
                command.ExecuteNonQuery();
            }
            con.Close();
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        //LoadUser();
        ClearControls();
        Session["Item"] = null;
        LoadCategories();
        LoadSubCategories();
        LoadMeausrment();
        LoadBlock("");
        StreetLoad();

    }

    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        Grid_users.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

  
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
        LoadMeausrment();
        mPopUpLocation.Show();
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Session["Item"] = null;
        Session["FORM_NO"] = null;
        Session["OPEN_FROM_REGISTRATION"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        ItemLogics Ser = new ItemLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        int Pro_ID = Convert.ToInt32(ProID);
        DbCalls db = new DbCalls();
        var chk = (from k in db.tbl_PURCHASE_DETAIL select k).Where(x => x.ITEM_INFO_ID == Pro_ID && x.IS_DELETED == false).ToList();
        if (chk.Any())
        {
            lblMsg.Visible = true;
            lblMsg.Text = "Item can't update because it is used in other tables.";
            //ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Item can't update because it is used in other tables.');", true);
            return;
        }
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
        ddlProject.Enabled = true;

        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        ddlCategory.SelectedValue = Convert.ToString(data.CATEGORY_ID);
        ddlCategory.Enabled = true;

        ddlSubCategory.DataSource = DropDowns.SUB_CATEGORYDropDown().Where(x=>x.CATEGORY_ID == data.CATEGORY_ID);
        ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
        ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
        ddlSubCategory.DataBind();
        ddlSubCategory.SelectedValue = Convert.ToString(data.SUB_CATEGORY_ID);
        ddlSubCategory.Enabled = true;

        ddlMeasurment.DataSource = DropDowns.MEASURMENTDropDown();
        ddlMeasurment.DataTextField = "MEASURMENT1";
        ddlMeasurment.DataValueField = "MEASURMENT_ID";
        ddlMeasurment.DataBind();

        if (data.BLOCK_ID != 0)
        {
            var Qry = DropDowns.BLOCKDropDown().Where(x => x.PROJECT_ID == data.PROJECT_ID);
            if (Qry.Any())
            {
                ddlBlock.DataSource = Qry;
                ddlBlock.DataTextField = "BLOCK_NAME";
                ddlBlock.DataValueField = "BLOCK_ID";
                ddlBlock.DataBind();
            }
        }
        else
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Add(new ListItem("Not Applicable", "0"));
            ddlBlock.SelectedIndex = 0;
            ddlBlock.Enabled = true;
        }
        if (data.BLOCK_ID.ToString() != "-2147483648")
        {
            ddlBlock.SelectedValue = Convert.ToString(data.BLOCK_ID);
            ddlBlock.Enabled = true;
        }
        else
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Add(new ListItem("Not Applicable", "0"));
            ddlBlock.SelectedIndex = 0;
            ddlBlock.Enabled = true;
        }
        if (data.STREET_ID != 0)
        {
            ddlStreet.DataSource = DropDowns.STREETDropDown().Where(x => x.BLOCK_ID == data.BLOCK_ID && x.PROJECT_ID == data.PROJECT_ID);
            ddlStreet.DataTextField = "STREET_NAME";
            ddlStreet.DataValueField = "STREET_ID";
            ddlStreet.DataBind();
        }
        else
        {
            ddlStreet.Items.Clear();
            ddlStreet.Items.Add(new ListItem("Not Applicable", "0"));
            ddlStreet.SelectedIndex = 0;
        }
        

        ddlStreet.SelectedValue = Convert.ToString(data.STREET_ID);
        ddlStreet.Enabled = true;

        ddlInventoryType.SelectedValue = Convert.ToString(data.INVENTORY_TYPE);
        ddlInventoryType.Enabled = true;
        

        if (data.PHASE_ID != 0)
        {
            PhaseLoad();
        }
        else
        {
            ddlPhase.Items.Clear();
            ddlPhase.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPhase.SelectedIndex = 0;
        }
        ddlPhase.SelectedValue = Convert.ToString(data.PHASE_ID);
        ddlPhase.Enabled = true;

        if (data.SECTOR_ID != 0)
        {
            SectorLoad();
        }
        else
        {
            ddlSector.Items.Clear();
            ddlSector.Items.Add(new ListItem("Not Applicable", "0"));
            ddlSector.SelectedIndex = 0;
        }
        ddlSector.SelectedValue = Convert.ToString(data.SECTOR_ID);
        ddlSector.Enabled = true;

        if (data.PRECENT_ID != 0)
        {
            if (data.PRECENT_ID.ToString() != "-2147483648")
            {
                PrecentLoad();
            }
            else
            {
                ddlPrecent.Items.Clear();
                ddlPrecent.Items.Add(new ListItem("Not Applicable", "0"));
                ddlPrecent.SelectedIndex = 0;
            }
        }
        else
        {
            ddlPrecent.Items.Clear();
            ddlPrecent.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecent.SelectedIndex = 0;
        }
        if (data.PRECENT_ID.ToString() != "-2147483648")
        {
            ddlPrecent.SelectedValue = Convert.ToString(data.PRECENT_ID);
            ddlPrecent.Enabled = true;
        }
        else
        {
            ddlPrecent.Items.Clear();
            ddlPrecent.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecent.SelectedIndex = 0;
        }


        if (data.DETAIL_CATEGORY_ID != 0)
        {
            DetailCategoryLoad();
        }
        else
        {
            ddlDetailCategory.Items.Clear();
            ddlDetailCategory.Items.Add(new ListItem("Not Applicable", "0"));
            ddlDetailCategory.SelectedIndex = 0;
        }
        ddlDetailCategory.SelectedValue = Convert.ToString(data.DETAIL_CATEGORY_ID);
        ddlDetailCategory.Enabled = true;
        if (data.ITEM_TYPE_ID != 0)
        {
            InventoryTypeLoad();
        }
        else
        {
            ddlItemType.Items.Clear();
            ddlItemType.Items.Add(new ListItem("Not Applicable", "0"));
            ddlItemType.SelectedIndex = 0;
        }
        ddlItemType.SelectedValue = Convert.ToString(data.ITEM_TYPE_ID);

        ddlPaidInstallment.SelectedValue = Convert.ToString(data.PAID_INSTALLMENT);

        if (data.PRECENTSTREET_ID != 0)
        {
            if (data.PRECENTSTREET_ID.ToString() != "-2147483648")
            {
                PrecentStreetLoad();
            }
            else
            {
                ddlPrecentStreet.Items.Clear();
                ddlPrecentStreet.Items.Add(new ListItem("Not Applicable", "0"));
                ddlPrecentStreet.SelectedIndex = 0;
            }
        }
        else
        {
            ddlPrecentStreet.Items.Clear();
            ddlPrecentStreet.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecentStreet.SelectedIndex = 0;
        }
        if (data.PRECENTSTREET_ID.ToString() != "-2147483648")
        {
            ddlPrecentStreet.SelectedValue = Convert.ToString(data.PRECENTSTREET_ID);
        }
        else
        {
            ddlPrecentStreet.Items.Clear();
            ddlPrecentStreet.Items.Add(new ListItem("Not Applicable", "0"));
            ddlPrecentStreet.SelectedIndex = 0;
        }

        txtOpenRegistration.Text = data.OPEN_FROM_REGISTRATION;

        txtItemName.Text = data.ITEM_NAME;
        txtNo_Of_Floor.Text = data.NO_OF_FLOORS;
        ddlMeasurment.SelectedValue = Convert.ToString(data.MEASUREMENT);
        txtExtraArea.Text = data.EXTRA_AREA;
        txtRegisteredName.Text = data.REGISTERED_NAME;
        txtRemarks.Text = data.REMARKS;
        
        var qry = (from k in db.ITEM_FACING select k).Where(x => x.ITEM_INFO_ID == data.ITEM_INFO_ID).ToList();

        for (var i = 0; i < qry.Count; i++)
        {
            for (int B = 0; B < lstFacingType.Items.Count; B++)
            {
                if (int.Parse(lstFacingType.Items[B].Value) == qry[i].FACING_TYPE_ID)
                {
                    lstFacingType.Items[B].Selected = true;
                    break;
                }
            }
        }

        //FacingCommercial.Checked = Convert.ToBoolean(data.Commercial);
        //FacingMosque.Checked = Convert.ToBoolean(data.Mosque);
        //FacingPark.Checked = Convert.ToBoolean(data.Park);
        //Corner.Checked = Convert.ToBoolean(data.Corner);
        //MainRoad.Checked = Convert.ToBoolean(data.Road);
        //Other.Checked = Convert.ToBoolean(data.Other);

        ddlVillaType.SelectedValue = Convert.ToString(data.VILLA_TYPE);
        txtFormNo.Text = data.FORM_NO;
        txtFloorNo.Text = data.FLOOR_NO;
        txtTowerNo.Text = data.TOWER_NO;
        txtNo_Of_Beds.Text = data.NO_OF_BEDS;
        if (data.POSSESSION == 1)
        {
            ChkPossession.Checked = true;
            ChkNonPossession.Checked = false;
        }
        else if (data.POSSESSION == 2)
        {
            ChkPossession.Checked = false;
            ChkNonPossession.Checked = true;
        }
        else
        {
            ChkPossession.Checked = false;
            ChkNonPossession.Checked = false;
        }
        Session.Add("Item", data.ITEM_INFO_ID);
        Session.Add("OPEN_FROM_REGISTRATION", data.OPEN_FROM_REGISTRATION);
        Session.Add("FORM_NO", data.FORM_NO);
        btnSave.Text = "Update";
        mPopUpLocation.Show();
    }

    protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadMeausrment();
        mPopUpLocation.Show();
    }

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
        StreetLoad();
        mPopUpLocation.Show();
    }

    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        StreetLoad();
        mPopUpLocation.Show();
    }



    protected void ChkPossession_CheckedChanged(object sender, EventArgs e)
    {
        if (ChkPossession.Checked)
        {
            if (ChkNonPossession.Checked)
            {
                //ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('You can checked only Possession or Non Possession at a time.');", true);
                ChkNonPossession.Checked = false;
                mPopUpLocation.Show();
                //return;
            }
            
            mPopUpLocation.Show();
        }
        else
        {
            mPopUpLocation.Show();
        }
    }

    protected void ChkNonPossession_CheckedChanged(object sender, EventArgs e)
    {
        if (ChkNonPossession.Checked)
        {
            if (ChkPossession.Checked)
            {
                //ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('You can checked only Possession or Non Possession at a time.');", true);
                ChkPossession.Checked = false;
                mPopUpLocation.Show();
                //return;
            }
            
            mPopUpLocation.Show();
        }
        else
        {
            mPopUpLocation.Show();
        }
    }
}
