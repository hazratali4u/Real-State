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


public partial class Forms_RptProfitReport : System.Web.UI.Page
{

    public string p_Projects = null, p_Categories = null, p_SubCategories = null, p_DetailCategories = null
    , p_Projects_Name = null, p_Categories_Name = null, p_SubCategories_Name = null, p_DetailCategories_Name = null
    , p_ItemTypes_IDs = null, p_ItemTypes_Name = null, p_Measurement_IDs = null, p_Measurement_Name = null;
    readonly LedgerController LController = new LedgerController();
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    readonly DistributorController DController = new DistributorController();
    readonly DbCalls db = new DbCalls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDistributor();
            LoadPrincipal();
            LoadVendor();
            LoadProjects();
            LoadCategories();
            LoadSubCategories();
            LoadDetailCategories();
            InventoryTypeLoad();
            LoadMeasurement();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void LoadProjects()
    {
        LstProjects.DataSource = DropDowns.PROJECTDropDown();
        LstProjects.DataTextField = "PROJECT_NAME";
        LstProjects.DataValueField = "PROJECT_ID";
        LstProjects.DataBind();
    }
    public void LoadCategories()
    {
        var qry = (from k in db.CATEGORies select k).Where(x => x.IS_DELETED == false).ToList();
        LstCategory.DataSource = qry;
        LstCategory.DataTextField = "CATEGORY_NAME";
        LstCategory.DataValueField = "CATEGORY_ID";
        LstCategory.DataBind();
    }
    public void LoadSubCategories()
    {
        for (int i = 0; i < LstCategory.Items.Count; i++)
        {
            if (LstCategory.Items[i].Selected == true)
            {
                p_Categories += LstCategory.Items[i].Value.ToString() + ",";
            }
        }
        DataTable dt = RptCustCtl.GetLoadDropDowns(int.Parse(drpDistributor.SelectedItem.Value.ToString()), p_Projects, null, p_Categories, null, null, null,
                  Constants.DateNullValue, Constants.DateNullValue, 2);
        if (dt.Rows.Count > 0)
        {
            LstSubCategory.DataSource = dt;
            LstSubCategory.DataTextField = "SUB_CATEGORY_NAME";
            LstSubCategory.DataValueField = "SUB_CATEGORY_ID";
            LstSubCategory.DataBind();
        }
    }
    public void LoadDetailCategories()
    {
        var qry = (from k in db.DETAIL_CATEGORY select k).Where(x => x.IS_DELETED == false).ToList();
        LstDetailCategory.DataSource = qry;
        LstDetailCategory.DataTextField = "DETAIL_CATEGORY_NAME";
        LstDetailCategory.DataValueField = "DETAIL_CATEGORY_ID";
        LstDetailCategory.DataBind();
    }
    private void LoadPrincipal()
    {
        ddlCustomer.Items.Clear();
        lblCustomer.Text = "Vendor";
        DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 5);
        ddlCustomer.Items.Add("All", Constants.IntNullValue);
        clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
        if (dt.Rows.Count > 0)
        {
            ddlCustomer.SelectedIndex = 0;
        }
    }
    private void LoadVendor()
    {
        ddlVendor.Items.Clear();
        DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 5);
        ddlVendor.Items.Add("Not Applicable", Constants.IntNullValue);
        clsWebFormUtil.FillDxComboBoxList(ddlVendor, dt, 0, 1, false);
        if (dt.Rows.Count > 0)
        {
            ddlVendor.SelectedIndex = 0;
        }
    }
    private void LoadDistributor()
    {
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        drpDistributor.Items.Add("All", Constants.IntNullValue);
        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, false);
        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    public void InventoryTypeLoad()
    {
        var qry = DropDowns.INVENTORYTYPEDropDown();
        if (qry.Any())
        {
            LstItemTypes.DataSource = qry;
            LstItemTypes.DataTextField = "INVENTORYTYPE_NAME";
            LstItemTypes.DataValueField = "INVENTORYTYPE_ID";
            LstItemTypes.DataBind();
        }
        else
        {
            //ddlItemType.Items.Clear();
            //ddlItemType.Items.Add(new ListItem("Not Applicable", "0"));
            //ddlItemType.SelectedIndex = 0;
        }
    }
    public void LoadMeasurement()
    {
        LstMeasurement.DataSource = DropDowns.MEASURMENTDropDown();
        LstMeasurement.DataTextField = "MEASURMENT1";
        LstMeasurement.DataValueField = "MEASURMENT_ID";
        LstMeasurement.DataBind();
    }


    private decimal LoadVendoerOpBalance()
    {
        //if (drpDistributor.Items.Count > 0 && LstCustomer.Items.Count > 0)
        //{
        //    VenderEntryController mController = new VenderEntryController();
        //    DataTable dt = mController.GetVendorOpening(int.Parse(LstCustomer.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()),
        //              DateTime.Parse(txtStartDate.Text + " 00:00:00"), 1);

        //    if (decimal.Parse(dt.Rows[0][0].ToString()) > 0)
        //    {
        //        opType.Value = "DR";

        //    }
        //    else
        //    {
        //        opType.Value = "CR";
        //    }


        //    return decimal.Parse(dt.Rows[0][0].ToString());
        //}
        return 0;
    }
    protected void ChbAllCategory_CheckedChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
    }

    protected void LstCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories();
    }

    private void ShowReport(int Report_Type)
    {
        #region Projects
        for (int i = 0; i < LstProjects.Items.Count; i++)
        {
            if (LstProjects.Items[i].Selected == true)
            {
                p_Projects += LstProjects.Items[i].Value.ToString() + ",";
                p_Projects_Name += LstProjects.Items[i].Text.ToString() + ",";
            }
        }
        if (p_Projects == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Project.');", true);
            return;
        }
        #endregion

        #region Categories
        for (int i = 0; i < LstCategory.Items.Count; i++)
        {
            if (LstCategory.Items[i].Selected == true)
            {
                p_Categories += LstCategory.Items[i].Value.ToString() + ",";
                p_Categories_Name += LstCategory.Items[i].Text.ToString() + ",";
            }
        }
        if (p_Categories == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
            return;
        }
        #endregion

        #region Sub Categories
        for (int i = 0; i < LstSubCategory.Items.Count; i++)
        {
            if (LstSubCategory.Items[i].Selected == true)
            {
                p_SubCategories += LstSubCategory.Items[i].Value.ToString() + ",";
                p_SubCategories_Name += LstSubCategory.Items[i].Text.ToString() + ",";
            }
        }
        if (p_SubCategories == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Sub Category.');", true);
            return;
        }
        #endregion

        #region Detail Categoriees
        for (int i = 0; i < LstDetailCategory.Items.Count; i++)
        {
            if (LstDetailCategory.Items[i].Selected == true)
            {
                p_DetailCategories += LstDetailCategory.Items[i].Value.ToString() + ",";
                p_DetailCategories_Name += LstDetailCategory.Items[i].Text.ToString() + ",";
            }
        }
        if (p_DetailCategories == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Detail Category.');", true);
            return;
        }
        #endregion

        #region Item Types
        for (int i = 0; i < LstItemTypes.Items.Count; i++)
        {
            if (LstItemTypes.Items[i].Selected == true)
            {
                p_ItemTypes_IDs += LstItemTypes.Items[i].Value.ToString() + ",";
                p_ItemTypes_Name += LstItemTypes.Items[i].Text.ToString() + ",";
            }
        }
        if (p_ItemTypes_IDs == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Item Type.');", true);
            return;
        }
        #endregion

        #region Measurements
        for (int i = 0; i < LstMeasurement.Items.Count; i++)
        {
            if (LstMeasurement.Items[i].Selected == true)
            {
                p_Measurement_IDs += LstMeasurement.Items[i].Value.ToString() + ",";
                p_Measurement_Name += LstMeasurement.Items[i].Text.ToString() + ",";
            }
        }
        if (p_Measurement_IDs == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Measurement.');", true);
            return;
        }
        #endregion

        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));

        ReportDocument CrpReport = new ReportDocument();
        ds = RptCustCtl.GetProfitReport(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlCustomer.SelectedItem.Value.ToString()), int.Parse(ddlVendor.SelectedItem.Value.ToString()), p_Projects, p_Categories, p_SubCategories, p_DetailCategories, p_ItemTypes_IDs, p_Measurement_IDs
                                        , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 1);
        CrpReport = new CrpProfitReport();
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();

        CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
        CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
        CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
        CrpReport.SetParameterValue("CategoryName", p_Categories_Name);
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("PROJECT_NAME", p_Projects_Name);
        CrpReport.SetParameterValue("SUB_CATEGORY_NAME", p_SubCategories_Name);
        CrpReport.SetParameterValue("ItemType_Name", p_ItemTypes_Name);
        CrpReport.SetParameterValue("Measurement_Name", p_Measurement_Name);
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", Report_Type);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }
}