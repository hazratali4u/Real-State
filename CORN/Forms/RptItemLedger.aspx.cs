using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNBusinessLayer.Reports;
using CORNCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_RptItemLedger : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ProjectLoad();
            LoadCategories();
            LoadSubCategories();
            ItemLoad();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void ProjectLoad()
    {
        ddlProject.Items.Clear();
        var qry = DropDowns.PROJECTDropDown();
        if (qry.Any())
        {

            ddlProject.DataSource = qry;
            ddlProject.DataTextField = "PROJECT_NAME";
            ddlProject.DataValueField = "PROJECT_ID";
            ddlProject.DataBind();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlProject.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlProject.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlProject.Items.Insert(0, myDefaultItem);
        }
    }
    private void LoadDistributor()
    {
        //DistributorController DController = new DistributorController();
        //DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        //clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, true);
        //if (dt.Rows.Count > 0)
        //{
        //    DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        //    myDefaultItem.Selected = true;
        //    drpDistributor.Items.Insert(0, myDefaultItem);
        //    drpDistributor.SelectedIndex = 0;
        //}
    }
    public void LoadCategories()
    {
        ddlCategory.Items.Clear();
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        //ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
        //myDefaultItem.Selected = true;
        //ddlCategory.Items.Insert(0, myDefaultItem);
        ddlCategory.SelectedIndex = 0;


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
            //ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            //myDefaultItem.Selected = true;
            //ddlSubCategory.Items.Insert(0, myDefaultItem);
            ddlSubCategory.SelectedIndex = 0;
        }
        else
        {
            ListItem myDefaultItem = new ListItem("", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSubCategory.Items.Insert(0, myDefaultItem);
        }
    }
    public void ItemLoad()
    {
        int P = Convert.ToInt32(ddlProject.SelectedValue);
        int C = Convert.ToInt32(ddlCategory.SelectedValue);
        int SC = Convert.ToInt32(ddlSubCategory.SelectedValue);
        if (ddlProject.SelectedValue == Constants.IntNullValue.ToString())
        {
            var Items = DropDowns.ITEMDropDown().Where(x => x.CATEGORY_ID == C && x.SUB_CATEGORY_ID == SC).ToList();
            if (Items.Any())
            {
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ITEM_NAME";
                ddlItem.DataValueField = "ITEM_INFO_ID";
                ddlItem.DataBind();
                ddlItem.SelectedIndex = 0;
            }
            else
            {
                ListItem myDefaultItem = new ListItem("", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlItem.Items.Insert(0, myDefaultItem);
            }
        }
        else
        {
            var Items = DropDowns.ITEMDropDown().Where(x => x.PROJECT_ID == P && x.CATEGORY_ID == C && x.SUB_CATEGORY_ID == SC).ToList();
            if (Items.Any())
            {
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ITEM_NAME";
                ddlItem.DataValueField = "ITEM_INFO_ID";
                ddlItem.DataBind();
                ddlItem.SelectedIndex = 0;
            }
            else
            {
                ListItem myDefaultItem = new ListItem("", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlItem.Items.Insert(0, myDefaultItem);
            }
        }
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        ItemLoad();
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
        ItemLoad();
    }
    protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ItemLoad();
    }
    private void showReport(int ReportType)
    {
        DocumentPrintController mController = new DocumentPrintController();
        RptInventoryController RptInventoryCtl = new RptInventoryController();
        ReportDocument CrpReport = new ReportDocument();
        CrpReport = new CrpSKULedgerWithValue();
        DataTable dt = mController.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));
        DataSet ds = null;
        
        ds = RptInventoryCtl.GetSKULedgerData(Convert.ToInt32(ddlItem.SelectedValue), Convert.ToInt32(ddlProject.SelectedValue), Convert.ToInt32(ddlCategory.SelectedValue), Convert.ToInt32(ddlSubCategory.SelectedValue), Convert.ToDateTime(txtStartDate.Text), Convert.ToDateTime(txtEndDate.Text + " 23:59:59"));
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("Project", ddlProject.SelectedItem.Text);
        CrpReport.SetParameterValue("Category", ddlCategory.SelectedItem.Text);
        CrpReport.SetParameterValue("SubCategory", ddlSubCategory.SelectedItem.Text);
        CrpReport.SetParameterValue("FromDate", txtStartDate.Text);
        CrpReport.SetParameterValue("To_date", txtEndDate.Text);
        CrpReport.SetParameterValue("USERNAME", Session["UserName"].ToString());
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", ReportType);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        showReport(0);
    }
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        showReport(1);
    }
}