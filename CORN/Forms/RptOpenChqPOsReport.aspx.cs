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

public partial class Forms_RptOpenChqPOsReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDistributor();
            LoadPrincipal();
            LoadType();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void LoadType()
    {
        DrpType.Items.Add(new DevExpress.Web.ListEditItem("All", "0"));
        DrpType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque", "1"));
        DrpType.Items.Add(new DevExpress.Web.ListEditItem("Pay Orders", "2"));
        DrpType.Items.Add(new DevExpress.Web.ListEditItem("Open Cheque Through Voucher Entry", "3"));
        DrpType.Items.Add(new DevExpress.Web.ListEditItem("Pay Orders Through Voucher Entry", "4"));
        DrpType.SelectedIndex = 0;
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
    private void LoadPrincipal()
    {
        DrpPrincipal.Items.Clear();
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 7, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        DrpPrincipal.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(this.DrpPrincipal, m_dt, 0, 1,false);
        //if (m_dt.Rows.Count > 0)
        //{
            DrpPrincipal.SelectedIndex = 0;
        //}

    }
    private void showReport(int reportType)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {
            ReportDocument CrpReport = new ReportDocument();

            ds = RptCustCtl.GetOpenChqAndPOz(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpPrincipal.SelectedItem.Value.ToString()),
              DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(DrpType.SelectedItem.Value.ToString()));
            
            if (DrpType.SelectedItem.Value.ToString() == "0")
            {
                //CrpReport = new CrpOpenChqAndPozForAll();
                CrpReport = new CrpOpenChqAndPozForAllNew();
            }
            else
            {
                //CrpReport = new CrpOpenChqAndPoz();
                CrpReport = new CrpOpenChqAndPozNew();
            }
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();

            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            CrpReport.SetParameterValue("ReportName", DrpType.SelectedItem.Text);
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            if (DrpType.SelectedItem.Value.ToString() == "0")
            {
                CrpReport.SetParameterValue("ChqPo", "Cheque/PO #");
                CrpReport.SetParameterValue("ChqPoDate", "Cheque/PO Date");
            }
            else if (DrpType.SelectedItem.Value.ToString() == "1" || DrpType.SelectedItem.Value.ToString() == "3")
            {
                CrpReport.SetParameterValue("ChqPo", "Cheque #");
                CrpReport.SetParameterValue("ChqPoDate", "Cheque Date");
            }
            else if (DrpType.SelectedItem.Value.ToString() == "2" || DrpType.SelectedItem.Value.ToString() == "4")
            {
                CrpReport.SetParameterValue("ChqPo", "PO #");
                CrpReport.SetParameterValue("ChqPoDate", "PO Date");
            }
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



    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPrincipal();
    }
}