using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNDatabaseLayer;
public partial class Forms_frmAddSubCategory : System.Web.UI.Page
{
        
    
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            //LoadDISTRIBUTOR();
            //LoadUser();
            //LoadRole();
            LoadGrid("");
            DoEmptyTextBox();
            //txtLogId.Attributes.Add("autocomplete", "off");
            //txtPswd.Attributes.Add("autocomplete", "off");
            DataTable dtConfig = GetAppConfiguration();
            Session.Add("dtConfig", dtConfig);
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
        lblErrorMsg.Visible = false;
        lblErrorMsg.Text = "";
        txtSubCategoryName.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        IsActive.Checked = true;
    }
    protected void LoadGrid(string pType)
    {
        UserController UController = new UserController();
        Grid_users.DataSource = null;
        Grid_users.DataBind();
        DataTable dt = new DataTable();

        dt = UController.SelectSlashUser(null, null);
        DataRow[] dr = dt.Select("DISTRIBUTOR_ID = 1234");
        dt.Rows.Remove(dr[0]);
        dt.AcceptChanges();
        if (pType == "")
        {
            if (txtSearch.Text != "" || txtSearch.Text != string.Empty)
            {
                dt.DefaultView.RowFilter = "USER_CODE LIKE '%" + txtSearch.Text + "%' OR USER_NAME LIKE '%" + txtSearch.Text + "%'  OR LOGIN_ID LIKE '%" + txtSearch.Text + "%'  OR PASSWORD LIKE '%" + txtSearch.Text + "%'  OR role_name LIKE '%" + txtSearch.Text + "%' OR IS_ACTIVE LIKE '" + txtSearch.Text + "%'";
            }
            Grid_users.DataSource = dt;
            Grid_users.DataBind();
        }
        else
        {
            if (txtSearch.Text != "" || txtSearch.Text != string.Empty)
            {
                dt.DefaultView.RowFilter = "USER_CODE LIKE '%" + txtSearch.Text + "%' OR USER_NAME LIKE '%" + txtSearch.Text + "%'  OR LOGIN_ID LIKE '%" + txtSearch.Text + "%'  OR PASSWORD LIKE '%" + txtSearch.Text + "%'  OR role_name LIKE '%" + txtSearch.Text + "%' OR IS_ACTIVE LIKE '" + txtSearch.Text + "%'";
            }
            if (dt.Rows.Count > 0)
            {
                Grid_users.PageIndex = 0;
            }
            Grid_users.DataSource = dt;
            Grid_users.DataBind();
        }

    }
    protected void ClearControls()
    {
        try
        {

            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "";
            txtSearch.Text = "";
            txtSubCategoryName.Text = "";
            txtRemarks.Text = "";
            btnSave.Text = "Save";
            IsActive.Checked = true;
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    #endregion
    protected void btnsearch_Click(object sender, EventArgs e)
    {

    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        DoEmptyTextBox();
    }

    protected void btnActive_Click(object sender, EventArgs e)
    {

    }

    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        ClearControls();
        mPopUpLocation.Hide();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        //LoadUser();
        ClearControls();
    }

    protected void Grid_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        Grid_users.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void Grid_users_RowEditing(object sender, GridViewEditEventArgs e)
    {

    }
}