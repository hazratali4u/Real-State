using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNDatabaseLayer.Classes;
using CORNBusinessLayer.Logics;
using System.Data.SqlClient;

public partial class Forms_frmAddProject : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    DbCalls db = new DbCalls();
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
            //loadRecords();
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
    public void DoEmptyTextBox()
    {
        lblErrorMsg.Visible = false;
        lblErrorMsg.Text = "";
        txtProjectName.Text = "";
        txtContactPerson.Text = "";
        txtContactNo.Text = "";
        txtEmail.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        IsActive.Checked = true;
        txtSearch.Text = "";
    }
    protected void LoadGrid(string pType)
    {

        if (pType.Equals(""))
        {
            var qry = (from k in db.PROJECTs
                       select new { k.PROJECT_ID, k.PROJECT_NAME, k.CONTACT_PERSON, k.CONTACT_NUMBER, k.REMARKS, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
            Grid_users.DataSource = qry;
            Grid_users.DataBind();
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(12);
            dt.DefaultView.RowFilter = "PROJECT_NAME LIKE '%" + txtSearch.Text + "%'  OR CONTACT_PERSON LIKE '%" + txtSearch.Text + "%' OR CONTACT_NUMBER LIKE '%" + txtSearch.Text + "%' OR REMARKS LIKE '%" + txtSearch.Text + "%'";
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
            txtProjectName.Text = "";
            txtContactPerson.Text = "";
            txtContactNo.Text = "";
            txtEmail.Text = "";
            txtRemarks.Text = "";
            btnSave.Text = "Save";
            IsActive.Checked = true;
            txtSearch.Text = "";
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
    }
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid("filter");
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        DoEmptyTextBox();
        Session["PROJECT"] = null;
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
            UserController UserCntrl = new UserController();

            bool flag = false;
            foreach (GridViewRow dr in Grid_users.Rows)
            {
                var chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");


                if (chRelized.Checked)
                {
                    if (Convert.ToString(dr.Cells[5].Text) == "Active")
                    {
                        try
                        {
                            int ID = Convert.ToInt32(dr.Cells[6].Text);
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update PROJECT set IS_DELETED = 1 where PROJECT_ID=" + ID, con))
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
                        try
                        {
                            int ID = Convert.ToInt32(dr.Cells[6].Text);
                            using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("update PROJECT set IS_DELETED = 0 where PROJECT_ID=" + ID, con))
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
                    loadRecords();
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
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtProjectName.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project name is required.');", true);
            mPopUpLocation.Show();
            return;
        }
        if (txtContactPerson.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Contact Person is required.');", true);
            mPopUpLocation.Show();
            return;
        }
        if (txtContactNo.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Contact No. is required.');", true);
            mPopUpLocation.Show();
            return;
        }
        ProjectLogics Ser = new ProjectLogics();
        CORNBusinessLayer.Classes.PROJECT pro = new CORNBusinessLayer.Classes.PROJECT();
        pro.PROJECT_NAME = txtProjectName.Text;
        pro.CONTACT_PERSON = txtContactPerson.Text;
        pro.CONTACT_NUMBER = txtContactNo.Text;
        pro.EMAIL_ID = txtEmail.Text;
        pro.REMARKS = txtRemarks.Text;
        pro.USER_ID = Convert.ToInt32(Session["UserID"]);
        pro.TIME_STEMP = DateTime.Now;
        pro.IS_DELETED = false;
        if (Session["PROJECT"] != null)
        {
            pro.PROJECT_ID = Convert.ToInt32(Session["PROJECT"]);
            Ser.Update(pro);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
            Session["PROJECT"] = null;
        }
        else
        {
            var chek = (from k in db.PROJECTs select k).Where(x => x.PROJECT_NAME == txtProjectName.Text);
            if (chek.Any())
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Project Name "+txtProjectName.Text+" already added. Please add an other project.');", true);
                mPopUpLocation.Show();
                return;
            }
            else
            {
                Ser.Save(pro);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
          
        }
        loadRecords();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        //LoadUser();
        ClearControls();
        Session["PROJECT"] = null;
    }
    protected void btnClose_ServerClick(object sender, EventArgs e)
    {
        ClearControls();
        mPopUpLocation.Hide();
        Session["PROJECT"] = null;
    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Session["PROJECT"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        ProjectLogics Ser = new ProjectLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtProjectName.Text = data.PROJECT_NAME;
        txtContactPerson.Text = data.CONTACT_PERSON;
        txtContactNo.Text = data.CONTACT_NUMBER;
        txtEmail.Text = data.EMAIL_ID;
        txtRemarks.Text = data.REMARKS;
        Session.Add("PROJECT", data.PROJECT_ID);
        btnSave.Text = "Update";
        mPopUpLocation.Show();
    }
    public void loadRecords()
    {
        var qry = (from k in db.PROJECTs
                   select new { k.PROJECT_ID, k.PROJECT_NAME, k.CONTACT_PERSON, k.CONTACT_NUMBER, k.REMARKS,Active = k.IS_DELETED == false ? "Active" : "Inactive" }).ToList();
        Grid_users.DataSource = qry;
        Grid_users.DataBind();
    }
}
