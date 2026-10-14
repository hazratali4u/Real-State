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

public partial class Forms_RptInHandInventory : System.Web.UI.Page
{
    readonly LedgerController LController = new LedgerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDistributor();
            ProjectLoad();
            LoadCategories();
            LoadSubCategories();
            LoadMeausrment();
            Blocks(27);
            LoadBlock("");
            InventoryTypeLoad();
            PhaseLoad();
            SectorLoad();
            PrecentLoad();
            DetailCategoryLoad();
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
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, true);
        if (dt.Rows.Count > 0)
        {
            DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            drpDistributor.Items.Insert(0, myDefaultItem);
            drpDistributor.SelectedIndex = 0;
        }
    }
    public void LoadCategories()
    {
        ddlCategory.Items.Clear();
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.DataTextField = "CATEGORY_NAME";
        ddlCategory.DataValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlCategory.Items.Insert(0, myDefaultItem);



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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSubCategory.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSubCategory.Items.Insert(0, myDefaultItem);
        }
    }
    public void LoadMeausrment()
    {
        ddlMeasurment.DataSource = DropDowns.MEASURMENTDropDown();
        ddlMeasurment.DataTextField = "MEASURMENT1";
        ddlMeasurment.DataValueField = "MEASURMENT_ID";
        ddlMeasurment.DataBind();
        ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlMeasurment.Items.Insert(0, myDefaultItem);
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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPhase.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlPhase.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPhase.Items.Insert(0, myDefaultItem);
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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSector.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlSector.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPrecent.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlPrecent.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlPrecent.Items.Insert(0, myDefaultItem);
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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlDetailCategory.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlDetailCategory.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlDetailCategory.Items.Insert(0, myDefaultItem);
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
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlItemType.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlItemType.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlItemType.Items.Insert(0, myDefaultItem);
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
                ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlBlock.Items.Insert(0, myDefaultItem);
            }
            else
            {
                ddlBlock.Items.Clear();
                ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlBlock.Items.Insert(0, myDefaultItem);
            }
        }
        else
        {
            ddlBlock.Items.Clear();
            ListItem myDefaultItem = new ListItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlBlock.Items.Insert(0, myDefaultItem);
        }
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBlock("");
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
        LoadMeausrment();
    }
    protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadMeausrment();
    }
    private void showReport(int reportType)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {
            ReportDocument CrpReport = new ReportDocument();

            ds = RptCustCtl.GetInventoryInHandReport(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlProject.SelectedItem.Value.ToString()),int.Parse(ddlCategory.SelectedItem.Value.ToString()), int.Parse(ddlSubCategory.SelectedItem.Value.ToString()),
                int.Parse(ddlDetailCategory.SelectedItem.Value.ToString()), int.Parse(ddlItemType.SelectedItem.Value.ToString()), int.Parse(ddlMeasurment.SelectedItem.Value.ToString()),
                int.Parse(ddlPhase.SelectedItem.Value.ToString()), int.Parse(ddlBlock.SelectedItem.Value.ToString()), int.Parse(ddlSector.SelectedItem.Value.ToString()),
                int.Parse(ddlPrecent.SelectedItem.Value.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1);
            if (RbtReportType.SelectedValue.ToString() == "1")
            {
                CrpReport = new CrpInventoryInHandReport();
            }
            else
            {
                CrpReport = new CrpInventoryInHandReportWithOutValues();
            }
            
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            if (RbtReportType.SelectedValue.ToString() == "1")
            {
                CrpReport.SetParameterValue("ReportName", " With Values");
            }
            else
            {
                CrpReport.SetParameterValue("ReportName", " With out Values");
            }
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("ProjectName", ddlProject.SelectedItem.Text);
            CrpReport.SetParameterValue("CategoryName", ddlProject.SelectedItem.Text);
            CrpReport.SetParameterValue("SubCategoryName", ddlProject.SelectedItem.Text);
            CrpReport.SetParameterValue("DetailCategoryName", ddlProject.SelectedItem.Text);
            CrpReport.SetParameterValue("ItemTypeName", ddlProject.SelectedItem.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());

            Session.Add("CrpReport", CrpReport);
            Session.Add("ReportType", reportType);
            const string url = "'Default.aspx'";
            const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
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