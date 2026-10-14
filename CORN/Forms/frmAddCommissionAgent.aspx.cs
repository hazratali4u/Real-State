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

public partial class Forms_frmAddCommissionAgent : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");
        //=========================================================

        try
        {

            if (!IsPostBack)
            {
                btnSave.Attributes.Add("onclick", "return ValidateForm()");
                LoadGrid("");

                LoadDistributor();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    #region Load

    private void LoadDistributor()
    {
        DbCalls db = new DbCalls();
        try
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            ddDistributorId.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
            ddDistributorId.DataTextField = "DISTRIBUTOR_NAME";
            ddDistributorId.DataValueField = "DISTRIBUTOR_ID";
            ddDistributorId.DataBind();

        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }
    public void clear()
    {
        txtAddress.Text = "";
        txtCNIC.Text = "";
        txtContact.Text = "";
        txtContact2.Text = "";
        txtEmail.Text = "";
        txtName.Text = "";
        txtNTNNO.Text = "";
        txtRemarks.Text = "";
        txtSearch.Text = "";
        Session["CommissionAgentID"] = null;
    }
    private void LoadGrid(string pType)
    {
        DbCalls db = new DbCalls();
        try
        {
            if (pType.Equals(""))
            {
                var qry = (from k in db.VENDORS
                           join l in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals l.DISTRIBUTOR_ID
                           select new { l.DISTRIBUTOR_NAME, k.VENDOR_ID, k.VENDOR_NAME, k.PRIMARY_CONTACT, k.ADDRESS, Active = k.IS_DELETED == false ? "Active" : "Inactive", k.TYPE_ID }).Where(x => x.TYPE_ID == Constants.CommissionAgent).ToList();
                GrdCustomer.DataSource = qry;
                GrdCustomer.DataBind();
            }
            else
            {
                DataTable dt = LController.SelectDataForSearching(11);
                dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%'  OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR PRIMARY_CONTACT LIKE '%" + txtSearch.Text + "%' OR ADDRESS LIKE '%" + txtSearch.Text + "%'";
                if (dt.Rows.Count > 0)
                {
                    GrdCustomer.PageIndex = 0;
                }
                GrdCustomer.DataSource = dt;
                GrdCustomer.DataBind();
            }

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }



    }


    #endregion

    #region Click

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        mPopUpLocation.Show();
        clear();
        btnSave.Text = "Save";
    }
    //protected void btnImport_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        Page.MaintainScrollPositionOnPostBack = true;
    //        HttpFileCollection uploadedFiles = Request.Files;
    //        if (FupVendor.HasFile)
    //        {
    //            HttpPostedFile userPostedFile = uploadedFiles[0];
    //            if (userPostedFile.ContentLength > 0)
    //            {
    //                string fileName = Path.GetFileName(userPostedFile.FileName);
    //                var file = Path.Combine(Server.MapPath("~/images/"), fileName);
    //                userPostedFile.SaveAs(file);
    //                cController.ImportCustomer(0, Convert.ToInt32(Session["DISTRIBUTOR_ID"]), Server.MapPath("~/images/" + fileName));
    //                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Customers imported successfully.');", true);
    //            }
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('No file selected.');", true);
    //        }
    //        mPopUpLocation.Show();

    //    }
    //    catch (Exception ex)
    //    {
    //        ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "')", true);
    //        mPopUpLocation.Show();
    //        throw;
    //    }
    //}

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtName.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Name is required');", true);
                mPopUpLocation.Show();
                return;
            }
            if (txtContact.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Primary contact is required');", true);
                mPopUpLocation.Show();
                return;
            }
            if (txtAddress.Text.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Address is required');", true);
                mPopUpLocation.Show();
                return;
            }
            CustomerLogics Ser = new CustomerLogics();
            VENDOR vnd = new VENDOR();
            vnd.DISTRIBUTOR_ID = Convert.ToInt32(ddDistributorId.SelectedItem.Value);
            vnd.VENDOR_NAME = txtName.Text;
            vnd.PRIMARY_CONTACT = txtContact.Text;
            vnd.SECONDARY_CONTACT = txtContact2.Text;
            vnd.EMAIL = txtEmail.Text;
            vnd.CNIC = txtCNIC.Text;
            vnd.ADDRESS = txtAddress.Text;
            vnd.NTN = txtNTNNO.Text;
            vnd.REMARKS = txtRemarks.Text;
            vnd.TYPE_ID = Constants.CommissionAgent;
            vnd.USER_ID = Convert.ToInt32(Session["UserID"]);
            vnd.TIME_STEMP = DateTime.Now;
            vnd.IS_DELETED = false;
            if (Session["CommissionAgentID"] != null)
            {
                vnd.VENDOR_ID = Convert.ToInt32(Session["CommissionAgentID"]);
                Ser.Update(vnd);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
                Session["CommissionAgentID"] = null;
            }
            else
            {
                Ser.Save(vnd);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
            LoadGrid("");
            clear();
            mPopUpLocation.Hide();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
            mPopUpLocation.Show();
        }
    }


    protected void btnFilter_Click(object sender, EventArgs e)
    {
        LoadGrid("filter");

    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Session["CommissionAgentID"] = null;
        mPopUpLocation.Hide();
        clear();
    }
    protected void btnActive_Click(object sender, EventArgs e)
    {
        bool check = false;
        try
        {
            foreach (GridViewRow dr2 in GrdCustomer.Rows)
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
            foreach (GridViewRow dr in GrdCustomer.Rows)
            {
                var chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");


                if (chRelized.Checked)
                {
                    if (Convert.ToString(dr.Cells[5].Text) == "Active")
                    {
                        int ID = Convert.ToInt32(dr.Cells[6].Text);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update VENDORS set IS_DELETED = 1 where VENDOR_ID=" + ID, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        flag = true;
                    }
                    else
                    {
                        int ID = Convert.ToInt32(dr.Cells[6].Text);
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand("update VENDORS set IS_DELETED = 0 where VENDOR_ID=" + ID, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        flag = true;
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
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        btnSave.Text = "Save";
        myModalLabel.InnerText = "Add New Customer";
        mPopUpLocation.Show();
        clear();
        txtName.Focus();
        Session["CommissionAgentID"] = null;
    }

    #endregion

    private void ClearAll()
    {
        txtNTNNO.Text = "";
        txtName.Text = "";
        txtContact.Text = "";
        txtContact2.Text = "";
        txtAddress.Text = "";
        txtEmail.Text = "";
        txtCNIC.Text = "";
        txtRemarks.Text = "";
        lblErrorMsg.Text = "";
        btnSave.Text = "Save";
        myModalLabel.InnerText = "Add New Customer";


    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Session["CommissionAgentID"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        CustomerLogics Ser = new CustomerLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        txtName.Text = data.VENDOR_NAME;
        txtContact.Text = data.PRIMARY_CONTACT;
        txtContact2.Text = data.SECONDARY_CONTACT;
        txtEmail.Text = data.EMAIL;
        txtNTNNO.Text = data.NTN;
        txtCNIC.Text = data.CNIC;
        txtRemarks.Text = data.REMARKS;
        txtAddress.Text = data.ADDRESS;
        ddDistributorId.DataSource = DropDowns.DISTRIBUTORDropDown();
        ddDistributorId.DataTextField = "DISTRIBUTOR_NAME";
        ddDistributorId.DataValueField = "DISTRIBUTOR_ID";
        ddDistributorId.DataBind();
        ddDistributorId.SelectedValue = Convert.ToString(data.DISTRIBUTOR_ID);
        Session.Add("CommissionAgentID", data.VENDOR_ID);
        btnSave.Text = "Update";
        mPopUpLocation.Show();
        btnSave.Text = "Update";
    }


    protected void GrdCustomer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdCustomer.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }
}