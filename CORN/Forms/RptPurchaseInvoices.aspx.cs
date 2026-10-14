using CORNBusinessLayer.Classes;
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

public partial class Forms_RptPurchaseInvoices : System.Web.UI.Page
{
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDistributor();
            LoadPrincipal();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    private void LoadPrincipal()
    {
        DbCalls db = new DbCalls();
        DrpPrincipal.Items.Clear();
        var qry = (from k in db.VENDORS
                   select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
        DrpPrincipal.DataSource = qry;
        DrpPrincipal.TextField = "VENDOR_NAME";
        DrpPrincipal.ValueField = "VENDOR_ID";
        DrpPrincipal.DataBind();
        DrpPrincipal.SelectedIndex = 0;
        //DrpPrincipal.Items.Clear();
        //SKUPriceDetailController PController = new SKUPriceDetailController();
        //DataTable m_dt = PController.SelectDataPrice(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 7, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        //clsWebFormUtil.FillDxComboBoxList(this.DrpPrincipal, m_dt, 0, 1);


        //if (m_dt.Rows.Count > 0)
        //{
        //    DrpPrincipal.SelectedIndex = 0;
        //}

    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        drpDistributor.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, false);
        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    private decimal LoadVendoerOpBalance()
    {
        if (drpDistributor.Items.Count > 0 && DrpPrincipal.Items.Count > 0)
        {
            VenderEntryController mController = new VenderEntryController();
            DataTable dt = mController.GetVendorOpening(int.Parse(DrpPrincipal.SelectedItem.Value.ToString()), int.Parse(drpDistributor.SelectedItem.Value.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), 0);

            if (decimal.Parse(dt.Rows[0][0].ToString()) > 0)
            {
                opType.Value = "DR";

            }
            else
            {
                opType.Value = "CR";
            }


            return decimal.Parse(dt.Rows[0][0].ToString());
        }
        return 0;
    }
    private void showReport(int reportType)
    {
        string INVOICE_IDs = "";
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        CrpInvoice CrpReport = new CrpInvoice();
        if (GrdInvoices.Rows.Count > 0)
        {
            if (CheckInvoice())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select an invoice first to print.');", true);
                return;
            }
            foreach (GridViewRow dr in GrdInvoices.Rows)
            {
                CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
                if (chRelized.Checked == true)
                {
                    INVOICE_IDs += dr.Cells[1].Text + ",";
                }
            }
            ds = RptCustCtl.GetInvoices(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpPrincipal.SelectedItem.Value.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), INVOICE_IDs, 10);
        }
        else
        {
            ds = RptCustCtl.GetInvoices(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpPrincipal.SelectedItem.Value.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), null, int.Parse(RbtReportType.SelectedValue));
        }
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
        CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
        CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
        if (int.Parse(RbtReportType.SelectedValue) == 1 || int.Parse(RbtReportType.SelectedValue) == 3)
        {
            CrpReport.SetParameterValue("Net_Amount", "Net Receivable");
        }
        else if (int.Parse(RbtReportType.SelectedValue) == 2 || int.Parse(RbtReportType.SelectedValue) == 4)
        {
            CrpReport.SetParameterValue("Net_Amount", "Net Payable");
        }
        CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", reportType);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
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
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadPrincipal();
    }

    protected void btnGetData_Click(object sender, EventArgs e)
    {
        DataTable dt = RptCustCtl.GetInvoicesData(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpPrincipal.SelectedItem.Value.ToString()),
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 8);
        if (dt.Rows.Count > 0)
        {
            GrdInvoices.DataSource = dt;
            GrdInvoices.DataBind();
        }
        else
        {
            GrdInvoices.DataSource = null;
            GrdInvoices.DataBind();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('There is no invoice exist in the selected time period.');", true);
        }
    }
    public bool CheckInvoice()
    {
        int InvoiceCount = Constants.IntNullValue;
        bool flag;
        foreach (GridViewRow dr in GrdInvoices.Rows)
        {
            CheckBox chRelized = (CheckBox)dr.Cells[0].FindControl("ChbIsAssigned");
            if (chRelized.Checked == true)
            {
                InvoiceCount++;
                break;
            }
        }
        if (InvoiceCount == Constants.IntNullValue)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }
        return flag;

    }
}