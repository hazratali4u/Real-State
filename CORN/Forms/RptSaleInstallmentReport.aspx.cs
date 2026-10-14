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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Forms_RptSaleInstallmentReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            LoadDistributor();
            LoadProject();
            LoadCustomer();
            CustomerItems();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    private void LoadDistributor()
    {
        try
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            drpDistributor.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
            drpDistributor.TextField = "DISTRIBUTOR_NAME";
            drpDistributor.ValueField = "DISTRIBUTOR_ID";
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
        ddlProject.TextField = "PROJECT_NAME";
        ddlProject.ValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedIndex = 0;
    }
    private void LoadCustomer()
    {
        DbCalls db = new DbCalls();
        var query = (from k in db.VENDORS
                     select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID }).Where(x => x.TYPE_ID == Constants.Customer).ToList();
        ddlCustomer.DataSource = query;
        ddlCustomer.TextField = "VENDOR_NAME";
        ddlCustomer.ValueField = "VENDOR_ID";
        ddlCustomer.DataBind();
        if (query.Any())
        {
            ddlCustomer.SelectedIndex = 0;
        }
    }
    private void LoadSupplier()
    {
        DbCalls db = new DbCalls();
        var query = (from k in db.VENDORS
                     select new { k.VENDOR_ID, k.VENDOR_NAME, k.TYPE_ID }).Where(x => x.TYPE_ID == Constants.Vendor).ToList();
        ddlCustomer.DataSource = query;
        ddlCustomer.TextField = "VENDOR_NAME";
        ddlCustomer.ValueField = "VENDOR_ID";
        ddlCustomer.DataBind();
        if (query.Any())
        {
            ddlCustomer.SelectedIndex = 0;
        }
    }

    public void CustomerItems()
    {
        if (ddlCustomer.Items.Count > 0)
        {
            int CustomerID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_SALE_INS_MASTER
                       join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       select new { k.SALE_INS_MASTER_ID, k.VENDOR_ID, k.ITEM_INFO_ID, i.ITEM_NAME, k.SALE_CLOSED }).Where(x => x.SALE_CLOSED == false && x.VENDOR_ID == CustomerID).ToList();
            if (qry.Count != 0)
            {
                ddlItem.Items.Clear();
                ddlItem.DataSource = qry;
                ddlItem.TextField = "ITEM_NAME";
                ddlItem.ValueField = "SALE_INS_MASTER_ID";
                ddlItem.DataBind();
            }
            else
            {
                ddlItem.Items.Clear();
                ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            }
            ddlItem.SelectedIndex = 0;
        }

    }

    public void SupplierItems()
    {
        if (ddlCustomer.Items.Count > 0)
        {
            int CustomerID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_PURCHASE_INS_MASTER
                       join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       select new { k.PURCHASE_INS_MASTER_ID, k.VENDOR_ID, k.ITEM_INFO_ID, i.ITEM_NAME, k.PURCHASE_CLOSED }).Where(x => x.PURCHASE_CLOSED == false && x.VENDOR_ID == CustomerID).ToList();
            if (qry.Count != 0)
            {
                ddlItem.Items.Clear();
                ddlItem.DataSource = qry;
                ddlItem.TextField = "ITEM_NAME";
                ddlItem.ValueField = "PURCHASE_INS_MASTER_ID";
                ddlItem.DataBind();
            }
            else
            {
                ddlItem.Items.Clear();
                ddlItem.Items.Add(new DevExpress.Web.ListEditItem("", "0"));
            }
            ddlItem.SelectedIndex = 0;
        }
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCustomer();
    }

    protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ReportType.SelectedValue == "1")
        {
            CustomerItems();
        }
        else
        {
            SupplierItems();
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
    private void showReport(int reportType)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();

        DataSet ds = null;

        {
            DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));

            ds = RptCustCtl.GetCustomerInstallmentDetail(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(ddlProject.SelectedItem.Value.ToString()),
                      int.Parse(ddlCustomer.SelectedItem.Value.ToString()), int.Parse(ddlItem.SelectedItem.Value.ToString()), int.Parse(ReportType.SelectedValue));
            ReportDocument CrpReport = new ReportDocument();
            if (ReportType.SelectedValue != "1")
            {
                CrpReport = new CrpSupplierInstallments();
            }
            else
            {
                CrpReport = new CrpCustomerInstallments();
            }
            
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            //CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            //CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("Project", ddlProject.SelectedItem.Text);
            //CrpReport.SetParameterValue("Op_Balance", LoadVendoerOpBalance());
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

    protected void ReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ReportType.SelectedValue == "1")
        {
            //HtmlControl control = FindControl("Label1") as HtmlControl;
            //control.Attributes["class"] = "fa-caret-right";
            Label1.InnerText = "Customer";
            LoadCustomer();
            CustomerItems();
        }
        else
        {
            //HtmlControl control = FindControl("Label1") as HtmlControl;
            //control.Attributes["class"] = "fa-caret-right";
            Label1.InnerText = "Supplier";
            LoadSupplier();
            SupplierItems();
        }
    }
}