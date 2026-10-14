using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_RptItemWiseInventoryReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDistributor();
            LoadProject();
            //LoadCustomer();
            //CustomerItems(1);
            LoadBlock();
            LoadStreet();
            PanelBlock.Visible = false;
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    #region Load
    private void LoadDistributor()
    {
        try
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            drpDistributor.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
            drpDistributor.DataTextField = "DISTRIBUTOR_NAME";
            drpDistributor.DataValueField = "DISTRIBUTOR_ID";
            drpDistributor.DataBind();
            drpDistributor.Items.Add(new ListItem("All", "0"));
            drpDistributor.SelectedValue = "0";
        }
        catch (Exception ex)
        {
            ExceptionPublisher.PublishException(ex);
            ScriptManager.RegisterStartupScript(this, typeof(Page), "CatchMsg", "alert('" + ex.Message.ToString() + "');", true);
        }
    }
    protected void LoadProject()
    {
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.Items.Add(new ListItem("All", "0"));
        ddlProject.SelectedValue = "0";
    }
    protected void LoadBlock()
    {
        int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
        ddlBlock.DataSource = DropDowns.BLOCKDropDown().Where(x => x.PROJECT_ID == ProjectID).ToList();
        ddlBlock.DataTextField = "BLOCK_NAME";
        ddlBlock.DataValueField = "BLOCK_ID";
        ddlBlock.DataBind();
        ddlBlock.Items.Add(new ListItem("All", "0"));
        ddlBlock.SelectedValue = "0";
    }
    protected void LoadStreet()
    {
        int ProjectID = Convert.ToInt32(ddlProject.SelectedValue);
        int BlockID = Convert.ToInt32(ddlBlock.SelectedValue);
        ddlStreet.DataSource = DropDowns.STREETDropDown().Where(x => x.PROJECT_ID == ProjectID && x.BLOCK_ID == BlockID).ToList();
        ddlStreet.DataTextField = "STREET_NAME";
        ddlStreet.DataValueField = "STREET_ID";
        ddlStreet.DataBind();
        ddlStreet.Items.Add(new ListItem("All", "0"));
        ddlStreet.SelectedValue = "0";
    }
    public void IndexChange()
    {
        if (drpDistributor.SelectedValue == "0")
        {
            if (ddlProject.SelectedValue == "0")
            {
                PanelBlock.Visible = false;
            }
            else
            {
                PanelBlock.Visible = true;
                LoadBlock();
                LoadStreet();
            }
        }
        else
        {
            if (ddlProject.SelectedValue == "0")
            {
                PanelBlock.Visible = false;
            }
            else
            {
                PanelBlock.Visible = true;
                LoadBlock();
                LoadStreet();
            }
        }
    }
    #endregion
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        IndexChange();
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        IndexChange();
    }

    protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadStreet();
    }

    protected void ddlStreet_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnViewPDF_Click(object sender, EventArgs e)
    {

    }

    protected void btnViewExcel_Click(object sender, EventArgs e)
    {

    }

  
}