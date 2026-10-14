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

public partial class Forms_RptInvoicesSummaryReport : System.Web.UI.Page
{
    public string p_Projects = null, p_Categories = null, p_SubCategories = null, p_Projects_Name = null, p_Categories_Name = null, p_SubCategories_Name = null;
    readonly LedgerController LController = new LedgerController();
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    readonly DistributorController DController = new DistributorController();
    readonly DbCalls db = new DbCalls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ddlType.SelectedIndex = 0;
            LoadDistributor();
            LoadPrincipal();
            LoadProjects();
            LoadCategories();
            LoadSubCategories();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void LoadProjects()
    {
        //DataTable dt = (DataTable)this.Session["dt"];
        //if (dt.Rows.Count > 0)
        //{
        //    var qry = from rows in dt.AsEnumerable()
        //              group rows by new { BLOCK_ID = rows["PROJECT_ID"], BLOCK_NAME = rows["PROJECT_NAME"] } into grp
        //              select grp.First();
        //    if (qry.Any())
        //    {
        //        var Blocks = qry.CopyToDataTable();
        //        DrpProjects.DataSource = Blocks;
        //        DrpProjects.TextField = "PROJECT_NAME";
        //        DrpProjects.ValueField = "PROJECT_ID";
        //        DrpProjects.DataBind();
        //        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        //        myDefaultItem.Selected = true;
        //        DrpProjects.Items.Insert(0, myDefaultItem);
        //        //ddlBlock.SelectedIndex = 0;
        //    }
        //}

        LstProjects.DataSource = DropDowns.PROJECTDropDown();
        LstProjects.DataTextField = "PROJECT_NAME";
        LstProjects.DataValueField = "PROJECT_ID";
        LstProjects.DataBind();
        //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        //myDefaultItem.Selected = true;
        //LstProjects.Items.Insert(0, myDefaultItem);
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
            //var qry = from rows in dt.AsEnumerable()
            //          group rows by new { BLOCK_ID = rows["MEASURMENT_ID"], BLOCK_NAME = rows["MEASURMENT"] } into grp
            //          select grp.First();
            //if (qry.Any())
            //{
            LstSubCategory.DataSource = dt;
            LstSubCategory.DataTextField = "SUB_CATEGORY_NAME";
            LstSubCategory.DataValueField = "SUB_CATEGORY_ID";
            LstSubCategory.DataBind();
            //}
        }
    }
    private void LoadPrincipal()
    {
        ddlCustomer.Items.Clear();
        if (ddlType.SelectedIndex == 0)
        {
            // Customer
            lblCustomer.Text = "Customer";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 4); //1);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }

            //var qry = (from k in db.VENDORS select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.V_TYPE, k.CUSTOMER_TYPE }).Where(x=>x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 1).ToList();
            //ddlCustomer.DataSource = qry;
            //ddlCustomer.TextField = "VENDOR_NAME";
            //ddlCustomer.ValueField = "VENDOR_ID";
            //ddlCustomer.DataBind();
            //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            //myDefaultItem.Selected = true;
            //ddlCustomer.Items.Insert(0, myDefaultItem);
        }
        else if (ddlType.SelectedIndex == 1)
        {
            // Vendors
            lblCustomer.Text = "Vendor";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 5);  //2);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }
            //var qry = (from k in db.VENDORS select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.V_TYPE, k.CUSTOMER_TYPE }).Where(x => x.TYPE_ID == 2 && x.CUSTOMER_TYPE == null).ToList();
            //ddlCustomer.DataSource = qry;
            //ddlCustomer.TextField = "VENDOR_NAME";
            //ddlCustomer.ValueField = "VENDOR_ID";
            //ddlCustomer.DataBind();
            //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            //myDefaultItem.Selected = true;
            //ddlCustomer.Items.Insert(0, myDefaultItem);
        }
        else if (ddlType.SelectedIndex == 2)
        {
            // Customers / Vendors
            lblCustomer.Text = "Customer / Vendor";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 3);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }
            //var qry = (from k in db.VENDORS select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID, k.V_TYPE, k.CUSTOMER_TYPE }).Where(x => x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
            //ddlCustomer.DataSource = qry;
            //ddlCustomer.TextField = "VENDOR_NAME";
            //ddlCustomer.ValueField = "VENDOR_ID";
            //ddlCustomer.DataBind();
            //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
            //myDefaultItem.Selected = true;
            //ddlCustomer.Items.Insert(0, myDefaultItem);
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
    public void CustomerItems()
    {
        //if (LstCustomer.Items.Count > 0)
        //{
        //    //int CustomerID = Convert.ToInt32(LstCustomer.SelectedItem.Value);
        //    //DbCalls db = new DbCalls();
        //    //var qry = (from k in db.tbl_SALE_INS_MASTER
        //    //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
        //    //           join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
        //    //           join s in db.STREET on i.STREET_ID equals s.STREET_ID
        //    //           select new { k.SALE_INS_MASTER_ID, k.VENDOR_ID, k.ITEM_INFO_ID, ITEM_NAME = i.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME, k.SALE_CLOSED }).Where(x => x.SALE_CLOSED == false && x.VENDOR_ID == CustomerID).ToList();
        //    ////if (qry.Count != 0)
        //    ////{
        //    //ddlItems.Items.Clear();
        //    //ddlItems.DataSource = qry;
        //    //ddlItems.TextField = "ITEM_NAME";
        //    //ddlItems.ValueField = "SALE_INS_MASTER_ID";
        //    //ddlItems.DataBind();

        //    //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        //    //myDefaultItem.Selected = true;
        //    //ddlItems.Items.Insert(0, myDefaultItem);
        //    ////}
        //    ////else
        //    ////{
        //    ////    ddlItems.Items.Clear();
        //    ////    ddlItems.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        //    ////}
        //    ////ddlItems.SelectedIndex = 0;
        //}

    }
    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPrincipal();
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
        int TYPE_ID = 0;
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
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {
            ReportDocument CrpReport = new ReportDocument();
            ReportDocument SubReport = new ReportDocument();
            if (ddlType.SelectedIndex == 0)
            {
                if (ChkInvoices.Checked)
                {
                    TYPE_ID = 10;
                }
                else
                {
                    TYPE_ID = int.Parse(ddlType.SelectedItem.Value.ToString());
                }
                ds = RptCustCtl.GetPartyInvoicesSummary(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlCustomer.SelectedItem.Value.ToString()), p_Projects, p_Categories, p_SubCategories
                , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
                CrpReport = new RptInvoicesSummaryReport();
            }
            else if (ddlType.SelectedIndex == 1)
            {
                if (ChkInvoices.Checked)
                {
                    TYPE_ID = 20;
                }
                else
                {
                    TYPE_ID = int.Parse(ddlType.SelectedItem.Value.ToString());
                }
                ds = RptCustCtl.GetPartyInvoicesSummary(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlCustomer.SelectedItem.Value.ToString()), p_Projects, p_Categories, p_SubCategories
                , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
                CrpReport = new RptVendorInvoicesSummaryReport();
            }
            else if (ddlType.SelectedIndex == 2)
            {
                if (ChkInvoices.Checked)
                {
                    TYPE_ID = 30;
                    ds = RptCustCtl.GetPartyCommonInvoicesSummaryWithInvoices(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlCustomer.SelectedItem.Value.ToString())
                        , p_Projects, p_Categories, p_SubCategories, DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
                }
                else
                {
                    TYPE_ID = int.Parse(ddlType.SelectedItem.Value.ToString());
                    ds = RptCustCtl.GetPartyCommonInvoicesSummaryWithoutInvoices(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlCustomer.SelectedItem.Value.ToString())
                        , p_Projects, p_Categories, p_SubCategories, DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
                }
                CrpReport = new RptCommonSummaryReport();
                SubReport = CrpReport.OpenSubreport("SubReport");
            }
            CrpReport.SetDataSource(ds);
            if (ddlType.SelectedIndex == 2)
            {
                SubReport.SetDataSource(ds);
            }
            CrpReport.Refresh();
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            if (ddlType.SelectedIndex == 0)
            {
                CrpReport.SetParameterValue("Report_Name", "Customer Invoices Summary Report");
            }
            else if (ddlType.SelectedIndex == 1)
            {
                CrpReport.SetParameterValue("Report_Name", "Vendor Invoices Summary Report");
            }
            else
            {
                CrpReport.SetParameterValue("Report_Name", "Customer / Vendor Invoices Summary Report");
            }
            CrpReport.SetParameterValue("PROJECT_NAME", p_Projects_Name);
            CrpReport.SetParameterValue("CATEGORY_NAME", p_Categories_Name);
            CrpReport.SetParameterValue("SUB_CATEGORY_NAME", p_SubCategories_Name);

            CrpReport.SetParameterValue("TYPE_ID", ddlType.SelectedItem.Value.ToString());
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("USERNAME", Session["UserName"].ToString());
            Session.Add("CrpReport", CrpReport);
            Session.Add("ReportType", Report_Type);
            const string url = "'Default.aspx'";
            const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
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