using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_RptCustomerWiseSalesReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDistributor();
            LoadProject();
            LoadCustomer();
            CustomerItems(1);
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
            drpDistributor.SelectedIndex = 0;
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
        ddlProject.SelectedIndex = 0;
    }
    private void LoadCustomer()
    {
        DbCalls db = new DbCalls();
        var query = (from k in db.VENDORS
                     select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID }).Where(x => x.TYPE_ID == Constants.Customer).ToList();
        ddlSupplier.DataSource = query;
        ddlSupplier.DataTextField = "VENDOR_NAME";
        ddlSupplier.DataValueField = "VENDOR_ID";
        ddlSupplier.DataBind();
        if (query.Any())
        {
            ddlSupplier.SelectedIndex = 0;
        }
    }
    public void CustomerItems(int P_Type)
    {
        if (ddlSupplier.Items.Count > 0)
        {
            int ProjectID = Convert.ToInt32(ddlProject.SelectedItem.Value);
            int VendorID = Convert.ToInt32(ddlSupplier.SelectedItem.Value);
            DbCalls db = new DbCalls();
            if (P_Type == 1)
            {
                var qry = (from k in db.tbl_SALE_DETAIL
                           join sm in db.tbl_SALE_MASTER on k.SALE_MASTER_ID equals sm.SALE_MASTER_ID
                           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                           select new { k.SALE_MASTER_ID, VENDOR_ID = sm.CUSTOMER_ID , k.ITEM_INFO_ID, i.ITEM_NAME, k.SALE_CLOSED, i.PROJECT_ID }).Where(x => x.SALE_CLOSED == false && x.PROJECT_ID == ProjectID && x.VENDOR_ID == VendorID).ToList();
                if (qry.Count != 0)
                {
                    ddlItem.Items.Clear();
                    ddlItem.DataSource = qry;
                    ddlItem.DataTextField = "ITEM_NAME";
                    ddlItem.DataValueField = "SALE_MASTER_ID";
                    ddlItem.DataBind();
                    ddlItem.Items.Add(new ListItem("All", "0"));
                    ddlItem.SelectedValue = "0";

                }
                else
                {
                    ddlItem.Items.Clear();
                    ddlItem.Items.Add(new ListItem("", "0"));
                    ddlItem.SelectedIndex = 0;
                }

            }
            else if (P_Type == 2)
            {
                var qry = (from k in db.tbl_SALE_RETURN
                           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                           select new { k.SALE_RETURN_ID, k.VENDOR_ID, k.ITEM_INFO_ID, i.ITEM_NAME, k.IS_DELETED, i.PROJECT_ID }).Where(x => x.IS_DELETED == false && x.PROJECT_ID == ProjectID && x.VENDOR_ID == VendorID).ToList();
                if (qry.Count != 0)
                {
                    ddlItem.Items.Clear();
                    ddlItem.DataSource = qry;
                    ddlItem.DataTextField = "ITEM_NAME";
                    ddlItem.DataValueField = "SALE_RETURN_ID";
                    ddlItem.DataBind();
                    ddlItem.Items.Add(new ListItem("All", "0"));
                    ddlItem.SelectedValue = "0";
                }
                else
                {
                    ddlItem.Items.Clear();
                    ddlItem.Items.Add(new ListItem("", "0"));
                    ddlItem.SelectedIndex = 0;
                }

            }
            else if (P_Type == 3)
            {
                var qry = (from k in db.tbl_SALE_INS_MASTER
                           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                           select new { k.SALE_INS_MASTER_ID, k.VENDOR_ID, k.ITEM_INFO_ID, i.ITEM_NAME, k.SALE_CLOSED, i.PROJECT_ID }).Where(x => x.SALE_CLOSED == false && x.PROJECT_ID == ProjectID && x.VENDOR_ID == VendorID).ToList();
                if (qry.Count != 0)
                {
                    ddlItem.Items.Clear();
                    ddlItem.DataSource = qry;
                    ddlItem.DataTextField = "ITEM_NAME";
                    ddlItem.DataValueField = "SALE_INS_MASTER_ID";
                    ddlItem.DataBind();
                    ddlItem.Items.Add(new ListItem("All", "0"));
                    ddlItem.SelectedValue = "0";
                }
                else
                {
                    ddlItem.Items.Clear();
                    ddlItem.Items.Add(new ListItem("", "0"));
                    ddlItem.SelectedIndex = 0;
                }
            }
        }
    }
    #endregion
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        int Type = Convert.ToInt32(ddlTransactionType.SelectedItem.Value);
        CustomerItems(Type);
    }

    protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
    {
        int Type = Convert.ToInt32(ddlTransactionType.SelectedItem.Value);
        CustomerItems(Type);
    }

    protected void ddlTransactionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        int Type = Convert.ToInt32(ddlTransactionType.SelectedItem.Value);
        CustomerItems(Type);
    }

    protected void btnViewPDF_Click(object sender, EventArgs e)
    {

    }

    protected void btnViewExcel_Click(object sender, EventArgs e)
    {

    }
}