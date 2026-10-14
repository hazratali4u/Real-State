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

public partial class Forms_RptInventoryDetailReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadPrincipal();
            LoadDistributor();
            LoadProjects();
            FacingType();
            LoadCategories();
            LoadSubCategories();
            LoadInventoryType();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }

    public void LoadProjects()
    {
        DrpProjects.DataSource = DropDowns.PROJECTDropDown();
        DrpProjects.TextField = "PROJECT_NAME";
        DrpProjects.ValueField = "PROJECT_ID";
        DrpProjects.DataBind();
        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        DrpProjects.Items.Insert(0, myDefaultItem);
    }
    public void LoadCategories()
    {
        ddlCategory.Items.Clear();
        ddlCategory.DataSource = DropDowns.CATEGORYDropDown();
        ddlCategory.TextField = "CATEGORY_NAME";
        ddlCategory.ValueField = "CATEGORY_ID";
        ddlCategory.DataBind();
        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlCategory.Items.Insert(0, myDefaultItem);



    }
    public void LoadSubCategories()
    {
        int ID = Convert.ToInt32(ddlCategory.SelectedItem.Value);
        var Qry = DropDowns.SUB_CATEGORYDropDown().Where(x => x.CATEGORY_ID == ID);
        if (Qry.Any())
        {
            ddlSubCategory.DataSource = Qry;
            ddlSubCategory.TextField = "SUB_CATEGORY_NAME";
            ddlSubCategory.ValueField = "SUB_CATEGORY_ID";
            ddlSubCategory.DataBind();
            DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSubCategory.Items.Insert(0, myDefaultItem);
        }
        else
        {
            ddlSubCategory.Items.Clear();
            DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            myDefaultItem.Selected = true;
            ddlSubCategory.Items.Insert(0, myDefaultItem);
        }
    }
    public void FacingType()
    {
        lstFacingType.Items.Clear();
        DbCalls db = new DbCalls();
        var qry = (from k in db.FACING_TYPE
                   select new { k.FACING_TYPE_ID, k.FACING_TYPE_NAME, k.IS_DELETED }).Where(x => x.IS_DELETED == false).ToList();
        lstFacingType.DataSource = qry.OrderBy(x => x.FACING_TYPE_NAME);
        lstFacingType.DataTextField = "FACING_TYPE_NAME";
        lstFacingType.DataValueField = "FACING_TYPE_ID";
        lstFacingType.DataBind();
    }
    private void LoadInventoryType()
    {
        ddlInventoryType.Items.Clear();
        ddlInventoryType.Items.Add(new DevExpress.Web.ListEditItem("Own Inventory", "2"));
        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("Available for Sale", "1");
        myDefaultItem.Selected = true;
        ddlInventoryType.Items.Insert(0, myDefaultItem);
    }
    private void LoadPrincipal()
    {
        //SKUPriceDetailController PController = new SKUPriceDetailController();
        //DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        //DrpPrincipal.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        //clsWebFormUtil.FillDxComboBoxList(this.DrpPrincipal, m_dt, 0, 1, false);


        //if (m_dt.Rows.Count > 0)
        //{
        //    DrpPrincipal.SelectedIndex = 0;
        //}

    }

    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();

        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));

        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, true);

        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    //private decimal LoadVendoerOpBalance()
    //{
    //    if (drpDistributor.Items.Count > 0 && DrpPrincipal.Items.Count > 0)
    //    {
    //        VenderEntryController mController = new VenderEntryController();
    //        DataTable dt = mController.GetVendorOpening(int.Parse(DrpPrincipal.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()),
    //                  DateTime.Parse(txtStartDate.Text + " 00:00:00"), 1);

    //        if (decimal.Parse(dt.Rows[0][0].ToString()) > 0)
    //        {
    //            opType.Value = "DR";

    //        }
    //        else
    //        {
    //            opType.Value = "CR";
    //        }


    //        return decimal.Parse(dt.Rows[0][0].ToString());
    //    }
    //    return 0;
    //}

    private void showReport(int reportType)
    {
        string Project_IDs = null;
        string Category_IDs = null;
        string SubCategory_IDs = null;
        string FacingList_IDs = null;
        for (int i = 0; i < lstFacingType.Items.Count; i++)
        {
            if (lstFacingType.Items[i].Selected == true)
            {
                FacingList_IDs += lstFacingType.Items[i].Value.ToString() + ",";
            }
        }
        if (FacingList_IDs == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
            return;
        }
        if (DrpProjects.SelectedItem.Text == "All")
        {
            for (int i = 0; i < DrpProjects.Items.Count; i++)
            {
                Project_IDs += null;
            }
        }
        else
        {
            Project_IDs = DrpProjects.SelectedItem.Value.ToString() + ",";
        }
        if (ddlCategory.SelectedItem.Text == "All")
        {
            for (int i = 0; i < ddlCategory.Items.Count; i++)
            {
                Category_IDs += null;
            }
        }
        else
        {
            Category_IDs = ddlCategory.SelectedItem.Value.ToString() + ",";
        }
        if (ddlSubCategory.SelectedItem.Text == "All")
        {
            for (int i = 0; i < ddlSubCategory.Items.Count; i++)
            {
                SubCategory_IDs += null;
            }
        }
        else
        {
            SubCategory_IDs = ddlSubCategory.SelectedItem.Value.ToString() + ",";
        }

        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();
        DataSet ds = null;
        {
            ds = RptCustCtl.GetInventoryDetail(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Project_IDs, Category_IDs, SubCategory_IDs, FacingList_IDs,
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(ddlInventoryType.SelectedItem.Value.ToString()));

            DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));

            CrpInventoryDetailReport CrpReport = new CrpInventoryDetailReport();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("Principal", "");
            CrpReport.SetParameterValue("Project", DrpProjects.SelectedItem.Text);
            //CrpReport.SetParameterValue("opType", opType.Value);
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

    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
    }
}