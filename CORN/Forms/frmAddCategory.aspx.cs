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

public partial class Forms_frmAddCategory : System.Web.UI.Page
{
    EmployeController _empCtrl = new EmployeController();
    /// <summary>
    /// Page_Load Function Populates All Grids On The Page
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
           
            clear();
            LoadCategories();
            dropdown();
            LoadSubCategories();
            ProjectLoad();
            Categoryformeasurment();
            LoadCategoriesMeasument();
            this.LoadSubCategoriesforMeasurment();
            LoadDetailCategoryGrid();


            LoadBlocks();
            LoadProjectStreet();
            LoadBlockStreet();
            LoadStreets();
            LoadInventoryType();
            MeasurmentInventoryType();
            MeasurmentVillaType();
            LoadPhaseGrid();
            LoadSectorGrid();
            LoadPrecentGrid();
            PrecentDropdown();
            LoadPrecentStreetGrid();
            LoadFacingGrid();
        }
    }
    public void dropdown()
    {
        
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
    }
    public void Categoryformeasurment()
    {
        //ddlCatgoryMeasurment.Items.Clear();
        //ddlCatgoryMeasurment.DataSource = DropDowns.CATEGORYDropDown();
        //ddlCatgoryMeasurment.DataTextField = "CATEGORY_NAME";
        //ddlCatgoryMeasurment.DataValueField = "CATEGORY_ID";
        //ddlCatgoryMeasurment.DataBind();
        //ddlCatgoryMeasurment.Items.Add(new ListItem("", "0"));
        //ddlCatgoryMeasurment.SelectedIndex = 0;
    }
    public void LoadCategories()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.CATEGORies select new { k.CATEGORY_NAME,k.REMARKS,k.CATEGORY_ID,Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x=>x.Active == "Active").ToList();
        grdChannelData.DataSource = qry;
        grdChannelData.DataBind();
    }
    public void LoadSubCategories()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.SUB_CATEGORY
                   join c in db.CATEGORies on k.CATEGORY_ID equals c.CATEGORY_ID
                   select new { c.CATEGORY_NAME, k.REMARKS, k.SUB_CATEGORY_ID,k.SUB_CATEGORY_NAME,Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
        GrdBusType.DataSource = qry;
        GrdBusType.DataBind();
    }
    public void LoadSubCategoriesforMeasurment()
    {
        //DbCalls db = new DbCalls();
        //int CategoryID = Convert.ToInt32(ddlCatgoryMeasurment.SelectedValue);
        //var qry = (from k in db.SUB_CATEGORY
        //           join c in db.CATEGORies on k.CATEGORY_ID equals c.CATEGORY_ID
        //           select new {k.CATEGORY_ID, k.SUB_CATEGORY_ID, k.SUB_CATEGORY_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active" && x.CATEGORY_ID == CategoryID).ToList();
       

        //if (qry.Any())
        //{
        //    ddlSubCategory.DataSource = qry;
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
    }
    #region Designation Tab



    public void clear()
    {
        ScriptManager.GetCurrent(Page).SetFocus(txtCategoryName);

        //txtCategoryName.Focus();
        txtCategoryName.Text = "";
        txtRemarksCategory.Text = "";
        txtRemarksSubCategory.Text = "";
        txtSubCategoryName.Text = "";
        txtMeasurement.Text = "";
        txtRemarksMeasurment.Text = "";
        //txtMeasurementNo_ofBeds.Text = "";
        btnSaveChannelType.Text = "Save";
        btnSaveBusType.Text = "Save";
        BtnSaveMeasurment.Text = "Save";
        BtnBlockSave.Text = "Save";
        txtBlock.Text = "";
        txtBlockRemarks.Text = "";
        txtStreetName.Text = "";
        txtRemarksStreet.Text = "";
        BtnStreetSave.Text = "Save";
    }
   
   
    protected void btnSaveChannelType_Click(object sender, EventArgs e)
    {
        if (txtCategoryName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtCategoryName);
            return;
            
        }
        CategoryLogics Ser = new CategoryLogics();
        CORNBusinessLayer.Classes.CATEGORY pro = new CORNBusinessLayer.Classes.CATEGORY();
        pro.CATEGORY_NAME = txtCategoryName.Text;
        pro.REMARKS = txtRemarksCategory.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["Category"] != null)
        {
            pro.CATEGORY_ID = Convert.ToInt32(Session["Category"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["Category"] = null;
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.CATEGORies select k).Where(x => x.CATEGORY_NAME == txtCategoryName.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category Name " + txtCategoryName.Text + " already added. Please add an other category.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadCategories();
        clear();
        dropdown();
        LoadSubCategories();
        Categoryformeasurment();
        LoadCategoriesMeasument();
        this.LoadSubCategoriesforMeasurment();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session["SubCategory"] = null;
        Session["Category"] = null;
        btnSaveChannelType.Text = "Save";
        btnSaveBusType.Text = "Save";
        lblErrorMsg.Text = "";
        lblErrorMsg.Visible = false;
        txtCategoryName.Text = "";
        txtRemarksCategory.Text = "";
    }

    #endregion

    #region Department Type Tab

   
   
    protected void GrdBusType_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdBusType.PageIndex = e.NewPageIndex;
        this.LoadSubCategories();
    }

   
  
    protected void btnSaveBusType_Click(object sender, EventArgs e)
    {
        if (txtSubCategoryName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
            return;
        }
        SubCetagoryLogics Ser = new SubCetagoryLogics();
        CORNBusinessLayer.Classes.SUB_CATEGORY pro = new CORNBusinessLayer.Classes.SUB_CATEGORY();
        pro.CATEGORY_ID = Convert.ToInt32(ddlCategory.SelectedItem.Value);
        pro.SUB_CATEGORY_NAME = txtSubCategoryName.Text;
        pro.REMARKS = txtRemarksSubCategory.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["SubCategory"] != null)
        {
            pro.SUB_CATEGORY_ID = Convert.ToInt32(Session["SubCategory"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["SubCategory"] = null;
            btnSaveBusType.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            int CategoryID = Convert.ToInt32(ddlCategory.SelectedValue);
            var chek = (from k in db.SUB_CATEGORY select k).Where(x => x.SUB_CATEGORY_NAME == txtSubCategoryName.Text && x.CATEGORY_ID == CategoryID);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category Name " + txtSubCategoryName.Text + " already added. Please add an other sub-category.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadSubCategories();
        clear();
        Categoryformeasurment();
        LoadCategoriesMeasument();
        this.LoadSubCategoriesforMeasurment();
    }


    #endregion


   

    private string GetAutoCode(string PreeFix, int CodeType, long CValue)
    {
        SETTINGS_TABLE_Controller AutoCode = new SETTINGS_TABLE_Controller();
        return AutoCode.GetAutoCode(PreeFix, CodeType, CValue);
    }
    protected void grdChannelData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdChannelData.PageIndex = e.NewPageIndex;
        LoadCategories();


    }
   
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Session["Category"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        CategoryLogics Ser = new CategoryLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtCategoryName.Text = data.CATEGORY_NAME;
        txtRemarksCategory.Text = data.REMARKS;
        Session.Add("Category", data.CATEGORY_ID);
        btnSaveChannelType.Text = "Update";
        //mPopUpLocation.Show();
    }

    protected void btnEditSubCategory_Click(object sender, EventArgs e)
    {
        Session["SubCategory"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        SubCetagoryLogics Ser = new SubCetagoryLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        ddlCategory.SelectedValue = Convert.ToString(data.CATEGORY_ID);
        txtSubCategoryName.Text = data.SUB_CATEGORY_NAME;
        txtRemarksSubCategory.Text = data.REMARKS;
        Session.Add("SubCategory", data.SUB_CATEGORY_ID);
        btnSaveBusType.Text = "Update";
    }

   

    protected void btnDeleteCategory_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        CategoryLogics Ser = new CategoryLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.MEASURMENTs select new { k.SUB_CATEGORY_ID,k.IS_DELETED }).Where(x => x.SUB_CATEGORY_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This category is used in Sub Category. To remove this you have to remove sub category first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update CATEGORY set IS_DELETED = 1 where CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }
                
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update CATEGORY set IS_DELETED = 0 where CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            this.LoadCategories();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }

    }

    protected void btnDeleteSubCategory_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        SubCetagoryLogics Ser = new SubCetagoryLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.MEASURMENTs select new { k.SUB_CATEGORY_ID, k.IS_DELETED }).Where(x => x.SUB_CATEGORY_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This sub category is used in Measurment. To remove this you have to remove Measurment first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update SUB_CATEGORY set IS_DELETED = 1 where SUB_CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update SUB_CATEGORY set IS_DELETED = 0 where SUB_CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            this.LoadCategories();
            this.LoadSubCategories();
            LoadSubCategoriesforMeasurment();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    protected void btncancelDestype_Click(object sender, EventArgs e)
    {
        Session["SubCategory"] = null;
        txtRemarksSubCategory.Text="";
        txtSubCategoryName.Text = "";
        btnSaveBusType.Text = "Save";
        dropdown();
    }

    #region MEASURMENT


    public void MeasurmentInventoryType()
    {
        //ddlInventoryType.DataSource = DropDowns.INVENTORYTYPEDropDown();
        //ddlInventoryType.DataTextField = "INVENTORYTYPE_NAME";
        //ddlInventoryType.DataValueField = "INVENTORYTYPE_ID";
        //ddlInventoryType.DataBind();
    }

    public void MeasurmentVillaType()
    {
        //ddlVillaType.DataSource = DropDowns.VILLATYPEDropDown();
        //ddlVillaType.DataTextField = "VILLATYPE_NAME";
        //ddlVillaType.DataValueField = "VILLATYPE_ID";
        //ddlVillaType.DataBind();
    }


    public void LoadCategoriesMeasument()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.MEASURMENTs
                   //join c in db.CATEGORies on k.CATEGORY_ID equals c.CATEGORY_ID
                   //join sc in db.SUB_CATEGORY on k.SUB_CATEGORY_ID equals sc.SUB_CATEGORY_ID
                   select new { k.MEASURMENT_ID,  k.REMARKS, MEASURMENT = k.MEASURMENT1, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
        GridMeasurment.DataSource = qry;
        GridMeasurment.DataBind();
    }
    protected void BtnSaveMeasurment_Click(object sender, EventArgs e)
    {
        if (txtMeasurement.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Measurment is required.');", true);
            return;
        }
        MeasurmentLogics Ser = new MeasurmentLogics();
        CORNBusinessLayer.Classes.MEASURMENT pro = new CORNBusinessLayer.Classes.MEASURMENT();
        pro.CATEGORY_ID = 0; //Convert.ToInt32(ddlCatgoryMeasurment.SelectedItem.Value);
        pro.SUB_CATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedItem.Value);
        pro.INVENTORYTYPEID = 0; //Convert.ToInt32(ddlInventoryType.SelectedItem.Value);
        pro.VILLA_TYPE_ID = 0; //Convert.ToInt32(ddlVillaType.SelectedItem.Value);
        pro.NO_OF_BEDS = "0"; //txtMeasurementNo_ofBeds.Text;
        pro.MEASURMENT1 = txtMeasurement.Text;
        pro.REMARKS = txtRemarksMeasurment.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["MEASURMENT"] != null)
        {
            pro.MEASURMENT_ID = Convert.ToInt32(Session["MEASURMENT"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["MEASURMENT"] = null;
            btnSaveBusType.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            //int CategoryID = Convert.ToInt32(ddlCatgoryMeasurment.SelectedValue);
            //int SubCategoryID = Convert.ToInt32(ddlSubCategory.SelectedValue);
            var chek = (from k in db.MEASURMENTs select k).Where(x => x.MEASURMENT1 == txtMeasurement.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Measurment " + txtMeasurement.Text + " already added. Please add an other Measurment.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        Categoryformeasurment();
        LoadSubCategoriesforMeasurment();
        LoadCategoriesMeasument();
        MeasurmentInventoryType();
        MeasurmentVillaType();
        clear();
    }

    protected void BtnCancelMeasurment_Click(object sender, EventArgs e)
    {
        Session["SubCategory"] = null;
        Session["Category"] = null;
        Session["MEASURMENT"] = null;
        btnSaveChannelType.Text = "Save";
        btnSaveBusType.Text = "Save";
        BtnSaveMeasurment.Text = "Save";
        lblErrorMsg.Text = "";
        txtMeasurement.Text = "";
        txtRemarksMeasurment.Text = "";
        txtSubCategoryName.Text = "";
        lblErrorMsg.Visible = false;
        txtCategoryName.Text = "";
        txtRemarksCategory.Text = "";
        //txtMeasurementNo_ofBeds.Text = "";
        MeasurmentInventoryType();
        MeasurmentVillaType();
    }

    protected void btnEditMeasurment_Click(object sender, EventArgs e)
    {
        Session["MEASURMENT"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        MeasurmentLogics Ser = new MeasurmentLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        //ddlCatgoryMeasurment.DataSource = DropDowns.CATEGORYDropDown();
        //ddlCatgoryMeasurment.DataTextField = "CATEGORY_NAME";
        //ddlCatgoryMeasurment.DataValueField = "CATEGORY_ID";
        //ddlCatgoryMeasurment.DataBind();
        //ddlSubCategory.DataSource = DropDowns.SUB_CATEGORYDropDown();
        //ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
        //ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
        //ddlSubCategory.DataBind();
        //ddlCatgoryMeasurment.SelectedValue = Convert.ToString(data.CATEGORY_ID);
        //ddlSubCategory.SelectedValue = Convert.ToString(data.SUB_CATEGORY_ID);
        //string INVENTORYTYPEID = data.INVENTORYTYPEID.ToString().Replace("&nbsp", "");
        //if (INVENTORYTYPEID.Equals(""))
        //{
        //    MeasurmentInventoryType();
        //}
        //else
        //{
        //    ddlInventoryType.SelectedValue = Convert.ToString(data.INVENTORYTYPEID);
        //}
        //string VILLA_TYPE_ID = data.VILLA_TYPE_ID.ToString().Replace("&nbsp", "");
        //if (VILLA_TYPE_ID.Equals(""))
        //{
        //    MeasurmentVillaType();
        //}
        //else
        //{
        //    ddlVillaType.SelectedValue = Convert.ToString(data.VILLA_TYPE_ID);
        //}
        //string NO_OF_BEDS = data.NO_OF_BEDS.ToString().Replace("&nbsp", "");
        //if (NO_OF_BEDS.Equals(""))
        //{
        //    txtMeasurementNo_ofBeds.Text = "";
        //}
        //else
        //{
        //    txtMeasurementNo_ofBeds.Text = NO_OF_BEDS;
        //}
        
        
        txtMeasurement.Text = data.MEASURMENT1;
        txtRemarksMeasurment.Text = data.REMARKS;
        Session.Add("MEASURMENT", data.MEASURMENT_ID);
        BtnSaveMeasurment.Text = "Update";
    }

    protected void btnDeleteMeasurment_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        MeasurmentLogics Ser = new MeasurmentLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.MEASUREMENT, k.IS_DELETED }).Where(x => x.MEASUREMENT == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Measurment is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update MEASURMENT set IS_DELETED = 1 where MEASURMENT_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update MEASURMENT set IS_DELETED = 0 where MEASURMENT_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadCategoriesMeasument();
        }
        catch (Exception ex)
        { }
    }

    protected void ddlCatgoryMeasurment_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategoriesforMeasurment();
        //UpdatePanel3.Update();
    }



    protected void GridMeasurment_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridMeasurment.PageIndex = e.NewPageIndex;
        LoadCategoriesMeasument();
    }
    #endregion

    #region Block

    protected void BtnBlockSave_Click(object sender, EventArgs e)
    {
        if (ddlProject.SelectedItem.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
            return;
        }
        if (txtBlock.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block Name is required.');", true);
            return;
        }
        BlockLogics Ser = new BlockLogics();
        CORNBusinessLayer.Classes.BLOCK pro = new CORNBusinessLayer.Classes.BLOCK();
        pro.PROJECT_ID = Convert.ToInt32(ddlProject.SelectedItem.Value);
        pro.BLOCK_NAME = txtBlock.Text;
        pro.REMARKS = txtBlockRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STAMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["Block"] != null)
        {
            pro.BLOCK_ID = Convert.ToInt32(Session["Block"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["Block"] = null;
            btnSaveBusType.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
            var chek = (from k in db.BLOCKs select k).Where(x => x.BLOCK_NAME == txtBlock.Text && x.PROJECT_ID == ProjectID);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtBlock.Text + " already added. Please add an other Block Name.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadBlocks();
        Categoryformeasurment();
        LoadSubCategoriesforMeasurment();
        LoadCategoriesMeasument();
        clear();
        ProjectLoad();
        LoadBlockStreet();
    }

    protected void BtnBlockCencal_Click(object sender, EventArgs e)
    {
        Session["Block"] = null;
        txtBlock.Text = "";
        txtBlockRemarks.Text = "";
        BtnBlockSave.Text = "Save";
        ProjectLoad();
    }
    public void ProjectLoad()
    {
        ddlProject.Items.Clear();
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.Items.Add(new ListItem("", "0"));
        ddlProject.SelectedIndex = 0;
    }
    public void LoadBlocks()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.BLOCKs
                   join c in db.PROJECTs on k.PROJECT_ID equals c.PROJECT_ID
                   select new { k.IS_DELETED,k.BLOCK_ID, c.PROJECT_NAME, k.REMARKS, k.BLOCK_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
        GridViewBlock.DataSource = qry;
        GridViewBlock.DataBind();
    }

    protected void btnEditBlock_Click(object sender, EventArgs e)
    {
        Session["Block"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        BlockLogics Ser = new BlockLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();

        ddlProject.SelectedValue = Convert.ToString(data.PROJECT_ID);
        txtBlock.Text = data.BLOCK_NAME;
        txtBlockRemarks.Text = data.REMARKS;
        Session.Add("Block", data.BLOCK_ID);
        BtnBlockSave.Text = "Update";
    }

    protected void btnDeleteBlock_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        BlockLogics Ser = new BlockLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.BLOCK_ID, k.IS_DELETED }).Where(x => x.BLOCK_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Block is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update BLOCK set IS_DELETED = 1 where BLOCK_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update BLOCK set IS_DELETED = 0 where BLOCK_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            this.LoadCategories();
            this.LoadSubCategories();
            LoadSubCategoriesforMeasurment();
            ProjectLoad();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    protected void GridViewBlock_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridViewBlock.PageIndex = e.NewPageIndex;
        LoadBlocks();
    }

    #endregion

    #region STREET 

    public void LoadProjectStreet()
    {
        ddlProjectStreet.Items.Clear();
        ddlProjectStreet.DataSource = DropDowns.PROJECTDropDown();
        ddlProjectStreet.DataTextField = "PROJECT_NAME";
        ddlProjectStreet.DataValueField = "PROJECT_ID";
        ddlProjectStreet.DataBind();
        ddlProjectStreet.Items.Add(new ListItem("","0"));
        ddlProjectStreet.SelectedIndex = 0;
    }
    public void LoadBlockStreet()
    {
        int ProjectID = Convert.ToInt32(ddlProjectStreet.SelectedValue);
        ddlBlockStreet.DataSource = DropDowns.BLOCKDropDown().Where(x=>x.PROJECT_ID == ProjectID);
        ddlBlockStreet.DataTextField = "BLOCK_NAME";
        ddlBlockStreet.DataValueField = "BLOCK_ID";
        ddlBlockStreet.DataBind();
    }
    public void LoadStreets()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.STREETs
                   join c in db.PROJECTs on k.PROJECT_ID equals c.PROJECT_ID
                   join b in db.BLOCKs on k.BLOCK_ID equals b.BLOCK_ID
                   select new { k.STREET_ID, b.BLOCK_NAME, c.PROJECT_NAME, k.REMARKS, k.STREET_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
        GridViewStreet.DataSource = qry;
        GridViewStreet.DataBind();
    }
    protected void ddlProjectStreet_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlockStreet();
    }

    protected void BtnStreetSave_Click(object sender, EventArgs e)
    {
        if (ddlProjectStreet.SelectedItem.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project is required.');", true);
            return;
        }
        if (ddlBlockStreet.SelectedItem.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Block is required.');", true);
            return;
        }
        if (txtStreetName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street Name is required.');", true);
            return;
        }
        StreetLogics Ser = new StreetLogics();
        CORNBusinessLayer.Classes.STREET pro = new CORNBusinessLayer.Classes.STREET();
        pro.PROJECT_ID = Convert.ToInt32(ddlProjectStreet.SelectedItem.Value);
        pro.BLOCK_ID = Convert.ToInt32(ddlBlockStreet.SelectedItem.Value);
        pro.STREET_NAME = txtStreetName.Text;
        pro.REMARKS = txtRemarksStreet.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["Street"] != null)
        {
            pro.STREET_ID = Convert.ToInt32(Session["Street"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["Street"] = null;
            BtnStreetSave.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            int ProjectID = Convert.ToInt32(ddlProjectStreet.SelectedValue);
            int BlockID = Convert.ToInt32(ddlBlockStreet.SelectedValue);
            var chek = (from k in db.STREETs select k).Where(x => x.STREET_NAME == txtStreetName.Text && x.PROJECT_ID == ProjectID && x.BLOCK_ID == BlockID);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtStreetName.Text + " already added. Please add an other Street Name.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadStreets();
        Categoryformeasurment();
        LoadSubCategoriesforMeasurment();
        LoadCategoriesMeasument();
        clear();
        LoadProjectStreet();
        LoadBlockStreet();
    }

    protected void BtnStreetCancel_Click(object sender, EventArgs e)
    {
        Session["Street"] = null;
        txtStreetName.Text = "";
        txtRemarksStreet.Text = "";
        BtnStreetSave.Text = "Save";
        LoadProjectStreet();
        LoadBlockStreet();
    }

    protected void GridViewStreet_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridViewStreet.PageIndex = e.NewPageIndex;
        LoadStreets();
    }

    protected void btnEditStreet_Click(object sender, EventArgs e)
    {
        Session["Street"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        StreetLogics Ser = new StreetLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        ddlProjectStreet.DataSource = DropDowns.PROJECTDropDown();
        ddlProjectStreet.DataTextField = "PROJECT_NAME";
        ddlProjectStreet.DataValueField = "PROJECT_ID";
        ddlProjectStreet.DataBind();
        ddlProjectStreet.SelectedValue = Convert.ToString(data.PROJECT_ID);
        ddlBlockStreet.DataSource = DropDowns.BLOCKDropDown().Where(x=>x.PROJECT_ID == data.PROJECT_ID);
        ddlBlockStreet.DataTextField = "BLOCK_NAME";
        ddlBlockStreet.DataValueField = "BLOCK_ID";
        ddlBlockStreet.DataBind();
        ddlBlockStreet.SelectedValue = Convert.ToString(data.BLOCK_ID);
        txtStreetName.Text = data.STREET_NAME;
        txtRemarksStreet.Text = data.REMARKS;
        Session.Add("Street", data.STREET_ID);
        BtnStreetSave.Text = "Update";
    }

    protected void btnDeleteStreet_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region INVENTORY TYPE

    protected void LoadInventoryType()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.INVENTORYTYPEs
                   select new { k.INVENTORYTYPE_ID, k.INVENTORYTYPE_NAME, k.REMARKS, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        //.Where(x => x.Active == "Active")
        GrdInventoryType.DataSource = qry;
        GrdInventoryType.DataBind();
    }

    protected void clearInventoryType()
    {
        Session["INVENTORYTYPE"] = null;
        txtInventoryType.Text = "";
        txtInventoryTypeRemarks.Text = "";
        BtnSaveInventoryType.Text = "Save";
    }
    protected void BtnSaveInventoryType_Click(object sender, EventArgs e)
    {
       
        if (txtInventoryType.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Type is required.');", true);
            txtInventoryType.Focus();
            return;
        }
        Inventory_Type_Logics Ser = new Inventory_Type_Logics();
        CORNBusinessLayer.Classes.INVENTORYTYPE pro = new CORNBusinessLayer.Classes.INVENTORYTYPE();
        pro.INVENTORYTYPE_NAME = txtInventoryType.Text;
        pro.REMARKS = txtInventoryTypeRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["INVENTORYTYPE"] != null)
        {
            pro.INVENTORYTYPE_ID = Convert.ToInt32(Session["INVENTORYTYPE"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["INVENTORYTYPE"] = null;
            BtnSaveInventoryType.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.INVENTORYTYPEs select k).Where(x => x.INVENTORYTYPE_NAME == txtInventoryType.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtInventoryType.Text + " already added. Please add an other type.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        MeasurmentInventoryType();
        LoadInventoryType();
        clearInventoryType();
    }

    protected void BtnCancelInventoryType_Click(object sender, EventArgs e)
    {
        clearInventoryType();
    }

    protected void GrdInventoryType_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdInventoryType.PageIndex = e.NewPageIndex;
        LoadInventoryType();
    }

    protected void btnEditINVENTORYTYPE_Click(object sender, EventArgs e)
    {
        Session["INVENTORYTYPE"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        Inventory_Type_Logics Ser = new Inventory_Type_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtInventoryType.Text = data.INVENTORYTYPE_NAME;
        txtInventoryTypeRemarks.Text = data.REMARKS;
        Session.Add("INVENTORYTYPE", data.INVENTORYTYPE_ID);
        BtnSaveInventoryType.Text = "Update";
    }

    protected void btnDeleteINVENTORYTYPE_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        Inventory_Type_Logics Ser = new Inventory_Type_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                //var check = (from k in db.ITEM_INFORMATION select new { k.INVENTORYTYPEID, k.IS_DELETED }).Where(x => x.INVENTORYTYPEID == id && x.IS_DELETED == false);
                //if (check.Any())
                //{
                //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Inventory Type is used in Items. To remove this you have to remove item first.');", true);
                //    return;
                //}
                //else
                //{
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update INVENTORYTYPE set IS_DELETED = 1 where INVENTORYTYPE_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                //}

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update INVENTORYTYPE set IS_DELETED = 0 where INVENTORYTYPE_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadInventoryType();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region PHASE

    protected void LoadPhaseGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.PHASEs
                   select new { k.PHASE_ID, k.PHASE_NAME, k.REMARKS, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        //.Where(x => x.Active == "Active")
        GrdPhase.DataSource = qry;
        GrdPhase.DataBind();
    }
    protected void clearPhase()
    {
        Session["PHASE"] = null;
        txtPhaseName.Text = "";
        txtPhaseRemarks.Text = "";
        BtnSavePhase.Text = "Save";
    }
    protected void BtnSavePhase_Click(object sender, EventArgs e)
    {
        if (txtPhaseName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Phase Name is required.');", true);
            txtPhaseName.Focus();
            return;
        }
        Phase_Logics Ser = new Phase_Logics();
        CORNBusinessLayer.Classes.PHASE pro = new CORNBusinessLayer.Classes.PHASE();
        pro.PHASE_NAME = txtPhaseName.Text;
        pro.REMARKS = txtPhaseRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["PHASE"] != null)
        {
            pro.PHASE_ID = Convert.ToInt32(Session["PHASE"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PHASE"] = null;
            BtnSavePhase.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.PHASEs select k).Where(x => x.PHASE_NAME == txtPhaseName.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtPhaseName.Text + " already added. Please add an other Phase.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadPhaseGrid();
        clearPhase();
    }

    protected void BtnCancelPhase_Click(object sender, EventArgs e)
    {
        clearPhase();
    }

    protected void GrdPhase_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdPhase.PageIndex = e.NewPageIndex;
        LoadPhaseGrid();
    }

    protected void btnEditPhase_Click(object sender, EventArgs e)
    {
        Session["PHASE"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        Phase_Logics Ser = new Phase_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtPhaseName.Text = data.PHASE_NAME;
        txtPhaseRemarks.Text = data.REMARKS;
        Session.Add("PHASE", data.PHASE_ID);
        BtnSavePhase.Text = "Update";
    }

    protected void btnDeletePhase_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        Phase_Logics Ser = new Phase_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.PHASE_ID, k.IS_DELETED }).Where(x => x.PHASE_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Phase is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update PHASE set IS_DELETED = 1 where PHASE_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update PHASE set IS_DELETED = 0 where PHASE_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadPhaseGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region SECTOR

    protected void LoadSectorGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.SECTORs
                   select new { k.SECTOR_ID, k.SECTOR_NAME, k.REMARKS, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        //.Where(x => x.Active == "Active")
        GrdSector.DataSource = qry;
        GrdSector.DataBind();
    }
    protected void clearSector()
    {
        Session["SECTOR"] = null;
        txtSectorName.Text = "";
        txtSectorRemarks.Text = "";
        BtnSaveSector.Text = "Save";
    }

    protected void BtnSaveSector_Click(object sender, EventArgs e)
    {
        if (txtSectorName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sector Name is required.');", true);
            txtSectorName.Focus();
            return;
        }
        Sector_Logics Ser = new Sector_Logics();
        CORNBusinessLayer.Classes.SECTOR pro = new CORNBusinessLayer.Classes.SECTOR();
        pro.SECTOR_NAME = txtSectorName.Text;
        pro.REMARKS = txtSectorRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["SECTOR"] != null)
        {
            pro.SECTOR_ID = Convert.ToInt32(Session["SECTOR"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["SECTOR"] = null;
            BtnSaveSector.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.SECTORs select k).Where(x => x.SECTOR_NAME == txtSectorName.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtSectorName.Text + " already added. Please add an other Sector.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadSectorGrid();
        clearSector();
    }

    protected void BtnCancelSector_Click(object sender, EventArgs e)
    {
        clearSector();
    }

    protected void GrdSector_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdSector.PageIndex = e.NewPageIndex;
        LoadSectorGrid();
    }

    protected void btnEditSector_Click(object sender, EventArgs e)
    {
        Session["SECTOR"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        Sector_Logics Ser = new Sector_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtSectorName.Text = data.SECTOR_NAME;
        txtSectorRemarks.Text = data.REMARKS;
        Session.Add("SECTOR", data.SECTOR_ID);
        BtnSaveSector.Text = "Update";
    }

    protected void btnDeleteSector_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        Sector_Logics Ser = new Sector_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.SECTOR_ID, k.IS_DELETED }).Where(x => x.SECTOR_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Sector is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update SECTOR set IS_DELETED = 1 where SECTOR_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update SECTOR set IS_DELETED = 0 where SECTOR_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadSectorGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region PRECENT

    protected void LoadPrecentGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.PRECENTs
                   select new { k.PRECENT_ID, k.PRECENT_NAME, k.REMARKS, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        //.Where(x => x.Active == "Active")
        GrdPrecent.DataSource = qry;
        GrdPrecent.DataBind();
    }
    protected void clearPrecent()
    {
        Session["PRECENT"] = null;
        txtPrecentName.Text = "";
        txtPrecentRemarks.Text = "";
        BtnSavePrecent.Text = "Save";
    }

    protected void BtnSavePrecent_Click(object sender, EventArgs e)
    {
        if (txtPrecentName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Precent Name is required.');", true);
            txtPrecentName.Focus();
            return;
        }
        Precent_Logics Ser = new Precent_Logics();
        CORNBusinessLayer.Classes.PRECENT pro = new CORNBusinessLayer.Classes.PRECENT();
        pro.PRECENT_NAME = txtPrecentName.Text;
        pro.REMARKS = txtPrecentRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["PRECENT"] != null)
        {
            pro.PRECENT_ID = Convert.ToInt32(Session["PRECENT"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PRECENT"] = null;
            BtnSavePrecent.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.PRECENTs select k).Where(x => x.PRECENT_NAME == txtPrecentName.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('" + txtPrecentName.Text + " already added. Please add an other Precent.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadPrecentGrid();
        PrecentDropdown();
        clearPrecent();
    }

    protected void BtnCancelPrecent_Click(object sender, EventArgs e)
    {
        clearPrecent();
    }

    protected void GrdPrecent_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdPrecent.PageIndex = e.NewPageIndex;
        LoadPrecentGrid();
    }

    protected void btnEditPrecent_Click(object sender, EventArgs e)
    {
        Session["PRECENT"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        Precent_Logics Ser = new Precent_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtPrecentName.Text = data.PRECENT_NAME;
        txtPrecentRemarks.Text = data.REMARKS;
        Session.Add("PRECENT", data.PRECENT_ID);
        BtnSavePrecent.Text = "Update";
    }

    protected void btnDeletePrecent_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        Precent_Logics Ser = new Precent_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.PRECENT_ID, k.IS_DELETED }).Where(x => x.PRECENT_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Precent is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update PRECENT set IS_DELETED = 1 where PRECENT_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update PRECENT set IS_DELETED = 0 where PRECENT_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadPrecentGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region DETAIL CATEGORY

    protected void LoadDetailCategoryGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.DETAIL_CATEGORY
                   //join c in db.CATEGORies on k.CATEGORY_ID equals c.CATEGORY_ID
                   //join sc in db.SUB_CATEGORY on k.SUB_CATEGORY_ID equals sc.SUB_CATEGORY_ID
                   select new { k.DETAIL_CATEGORY_ID,
                       //c.CATEGORY_NAME,
                       //sc.SUB_CATEGORY_NAME,
                       k.REMARKS, k.DETAIL_CATEGORY_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        GrdDetailCategory.DataSource = qry;
        GrdDetailCategory.DataBind();
    }
    protected void clearDetailCategory()
    {
        Session["DETAIL_CATEGORY_ID"] = null;
        txtDetailCategoryName.Text = "";
        txtDetailCategoryRemarks.Text = "";
        btnSaveDetailCategory.Text = "Save";
    }
    protected void btnSaveDetailCategory_Click(object sender, EventArgs e)
    {
        //if (ddlCatgoryMeasurment.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Category is required.');", true);
        //    ddlCatgoryMeasurment.Focus();
        //    return;
        //}
        //if (ddlSubCategory.SelectedValue.Equals("0"))
        //{
        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sub Category is required.');", true);
        //    ddlSubCategory.Focus();
        //    return;
        //}
        if (txtDetailCategoryName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Detail Category Name is required.');", true);
            txtDetailCategoryName.Focus();
            return;
        }
        DetailCategory_Logics Ser = new DetailCategory_Logics();
        CORNBusinessLayer.Classes.DETAIL_CATEGORY pro = new CORNBusinessLayer.Classes.DETAIL_CATEGORY();
        pro.CATEGORY_ID = 0; //Convert.ToInt32(ddlCatgoryMeasurment.SelectedValue);
        pro.SUB_CATEGORY_ID = 0; //Convert.ToInt32(ddlSubCategory.SelectedValue);
        pro.DETAIL_CATEGORY_NAME = txtDetailCategoryName.Text;
        pro.REMARKS = txtDetailCategoryRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["DETAIL_CATEGORY_ID"] != null)
        {
            pro.DETAIL_CATEGORY_ID = Convert.ToInt32(Session["DETAIL_CATEGORY_ID"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["DETAIL_CATEGORY_ID"] = null;
            btnSaveDetailCategory.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            //int CategoryID = Convert.ToInt32(ddlCatgoryMeasurment.SelectedValue);
            //int SubCategoryID = Convert.ToInt32(ddlSubCategory.SelectedValue);
            var chek = (from k in db.DETAIL_CATEGORY select k).Where(x => x.DETAIL_CATEGORY_NAME == txtDetailCategoryName.Text); //&& x.CATEGORY_ID == CategoryID && x.SUB_CATEGORY_ID == SubCategoryID);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Detail Category " + txtDetailCategoryName.Text + " already added. Please add an other Detail Category.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadDetailCategoryGrid();
        Categoryformeasurment();
        LoadSubCategoriesforMeasurment();
        clearDetailCategory();
    }
    protected void btnCancelDetailCategory_Click(object sender, EventArgs e)
    {
        clearDetailCategory();
    }
    protected void GrdDetailCategory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdDetailCategory.PageIndex = e.NewPageIndex;
        LoadDetailCategoryGrid();
    }
    protected void btnEditDtailCategory_Click(object sender, EventArgs e)
    {
        Session["DETAIL_CATEGORY_ID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        DetailCategory_Logics Ser = new DetailCategory_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));

        //ddlCatgoryMeasurment.DataSource = DropDowns.CATEGORYDropDown();
        //ddlCatgoryMeasurment.DataTextField = "CATEGORY_NAME";
        //ddlCatgoryMeasurment.DataValueField = "CATEGORY_ID";
        //ddlCatgoryMeasurment.DataBind();
        //ddlSubCategory.DataSource = DropDowns.SUB_CATEGORYDropDown();
        //ddlSubCategory.DataTextField = "SUB_CATEGORY_NAME";
        //ddlSubCategory.DataValueField = "SUB_CATEGORY_ID";
        //ddlSubCategory.DataBind();
        //ddlCatgoryMeasurment.SelectedValue = Convert.ToString(data.CATEGORY_ID);
        //ddlSubCategory.SelectedValue = Convert.ToString(data.SUB_CATEGORY_ID);

        txtDetailCategoryName.Text = data.DETAIL_CATEGORY_NAME;
        txtDetailCategoryRemarks.Text = data.REMARKS;
        Session.Add("DETAIL_CATEGORY_ID", data.DETAIL_CATEGORY_ID);
        btnSaveDetailCategory.Text = "Update";
    }
    protected void btnDeleteDeatilCategory_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        DetailCategory_Logics Ser = new DetailCategory_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.DETAIL_CATEGORY_ID, k.IS_DELETED }).Where(x => x.DETAIL_CATEGORY_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Detail Category is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update DETAIL_CATEGORY set IS_DELETED = 1 where DETAIL_CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update DETAIL_CATEGORY set IS_DELETED = 0 where DETAIL_CATEGORY_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadDetailCategoryGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region PRECENT STREET

    public void PrecentDropdown()
    {
        ddlPrecent.Items.Clear();
        ddlPrecent.DataSource = DropDowns.PRECENTDropDown();
        ddlPrecent.DataTextField = "PRECENT_NAME";
        ddlPrecent.DataValueField = "PRECENT_ID";
        ddlPrecent.DataBind();
        ddlPrecent.Items.Add(new ListItem("", "0"));
        ddlPrecent.SelectedIndex = 0;
    }
    protected void LoadPrecentStreetGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.PRECENTSTREETs
                   join c in db.PRECENTs on k.PRECENT_ID equals c.PRECENT_ID
                   select new { k.PRECENTSTREET_ID, c.PRECENT_NAME, k.REMARKS, k.PRECENTSTREET_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        GrdPrecentStreet.DataSource = qry;
        GrdPrecentStreet.DataBind();
    }
    protected void clearPrecentStreet()
    {
        Session["PRECENTSTREET_ID"] = null;
        txtPrecentStreet.Text = "";
        txtPrecentStreetRemarks.Text = "";
        BtnSavePrecentStreet.Text = "Save";
    }
    protected void BtnSavePrecentStreet_Click(object sender, EventArgs e)
    {
        if (ddlPrecent.SelectedValue.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Precent is required.');", true);
            ddlPrecent.Focus();
            return;
        }
        if (txtPrecentStreet.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Street Name is required.');", true);
            txtPrecentStreet.Focus();
            return;
        }
        PrecentStreet_Logics Ser = new PrecentStreet_Logics();
        CORNBusinessLayer.Classes.PRECENTSTREET pro = new CORNBusinessLayer.Classes.PRECENTSTREET();
        pro.PRECENT_ID = Convert.ToInt32(ddlPrecent.SelectedValue);
        pro.PRECENTSTREET_NAME = txtPrecentStreet.Text;
        pro.REMARKS = txtPrecentStreetRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["PRECENTSTREET_ID"] != null)
        {
            pro.PRECENTSTREET_ID = Convert.ToInt32(Session["PRECENTSTREET_ID"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PRECENTSTREET_ID"] = null;
            BtnSavePrecentStreet.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            int PrecentID = Convert.ToInt32(ddlPrecent.SelectedValue);
            var chek = (from k in db.PRECENTSTREETs select k).Where(x => x.PRECENTSTREET_NAME == txtPrecentStreet.Text && x.PRECENT_ID == PrecentID);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Precent Street " + txtPrecentStreet.Text + " already added. Please add an other Precent Street.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadPrecentStreetGrid();
        clearPrecentStreet();
    }
    protected void BtnSavePrecentCencel_Click(object sender, EventArgs e)
    {
        clearPrecentStreet();
    }
    protected void GrdPrecentStreet_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdPrecentStreet.PageIndex = e.NewPageIndex;
        LoadPrecentStreetGrid();
    }
    protected void btnEditPrecentStreet_Click(object sender, EventArgs e)
    {
        Session["PRECENTSTREET_ID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        PrecentStreet_Logics Ser = new PrecentStreet_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));

        PrecentDropdown();
        ddlPrecent.SelectedValue = Convert.ToString(data.PRECENT_ID);
        txtPrecentStreet.Text = data.PRECENTSTREET_NAME;
        txtPrecentStreetRemarks.Text = data.REMARKS;
        Session.Add("PRECENTSTREET_ID", data.PRECENTSTREET_ID);
        BtnSavePrecentStreet.Text = "Update";
    }

    protected void btnDeletePrecentStreet_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        PrecentStreet_Logics Ser = new PrecentStreet_Logics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.PRECENTSTREET_ID, k.IS_DELETED }).Where(x => x.PRECENTSTREET_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Precent Street is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update PRECENTSTREET set IS_DELETED = 1 where PRECENTSTREET_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update PRECENTSTREET set IS_DELETED = 0 where PRECENTSTREET_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadPrecentStreetGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

    #region Facing Type

    protected void LoadFacingGrid()
    {
        DbCalls db = new DbCalls();
        var qry = (from k in db.FACING_TYPE
                   select new { k.FACING_TYPE_ID, k.REMARKS, k.FACING_TYPE_NAME, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        GrdFacing.DataSource = qry;
        GrdFacing.DataBind();
    }
    protected void clearFacing()
    {
        Session["FACING_TYPE_ID"] = null;
        txtFacingType.Text = "";
        txtFacingRemarks.Text = "";
        btnFacingType.Text = "Save";
    }

    protected void btnFacingType_Click(object sender, EventArgs e)
    {
        
        if (txtFacingType.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Type Name is required.');", true);
            txtFacingType.Focus();
            return;
        }
        FacingTypeLogics Ser = new FacingTypeLogics();
        CORNBusinessLayer.Classes.FACING_TYPE pro = new CORNBusinessLayer.Classes.FACING_TYPE();
        pro.FACING_TYPE_NAME = txtFacingType.Text;
        pro.REMARKS = txtFacingRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["FACING_TYPE_ID"] != null)
        {
            pro.FACING_TYPE_ID = Convert.ToInt32(Session["FACING_TYPE_ID"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["FACING_TYPE_ID"] = null;
            btnFacingType.Text = "Save";
        }
        else
        {
            DbCalls db = new DbCalls();
            var chek = (from k in db.FACING_TYPE select k).Where(x => x.FACING_TYPE_NAME == txtFacingType.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Facing Type " + txtFacingType.Text + " already added. Please add an other type.');", true);
                //mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
        }
        LoadFacingGrid();
        clearFacing();
    }

    protected void btnFacingTypeCancel_Click(object sender, EventArgs e)
    {
        clearFacing();
    }

    protected void GrdFacing_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdFacing.PageIndex = e.NewPageIndex;
        LoadFacingGrid();
    }

    protected void btnEditFacing_Click(object sender, EventArgs e)
    {
        Session["FACING_TYPE_ID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        FacingTypeLogics Ser = new FacingTypeLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));

        txtFacingType.Text = data.FACING_TYPE_NAME;
        txtFacingRemarks.Text = data.REMARKS;
        Session.Add("FACING_TYPE_ID", data.FACING_TYPE_ID);
        btnFacingType.Text = "Update";
    }

    protected void btnDeleteFacing_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        FacingTypeLogics Ser = new FacingTypeLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        try
        {
            if (!Active)
            {
                DbCalls db = new DbCalls();
                int id = Convert.ToInt32(ProID);
                var check = (from k in db.ITEM_INFORMATION select new { k.PRECENTSTREET_ID, k.IS_DELETED }).Where(x => x.PRECENTSTREET_ID == id && x.IS_DELETED == false);
                if (check.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('This Facing Type is used in Items. To remove this you have to remove item first.');", true);
                    return;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update FACING_TYPE set IS_DELETED = 1 where FACING_TYPE_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("update FACING_TYPE set IS_DELETED = 0 where FACING_TYPE_ID=" + Convert.ToInt32(ProID), con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                flag = true;
                if (flag)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record updated successfully');", true);
                }
            }
            LoadFacingGrid();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }

    #endregion

}