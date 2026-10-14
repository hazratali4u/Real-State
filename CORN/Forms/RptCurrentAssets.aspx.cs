using System;
using System.Data;
using System.Web.UI;
using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using CORNBusinessLayer.Reports;

public partial class Forms_RptCurrentAssets : System.Web.UI.Page
{
    readonly AccountHeadController MController = new AccountHeadController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.LoadDistributor();
            GetAccountType();
            GetSubAccountType();
            GetDetailAccountType();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtStartDate.Attributes.Add("readonly", "readonly");
            txtEndDate.Attributes.Add("readonly", "readonly");
        }
    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        drpDistributor.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2);
        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    private void GetAccountType()
    {
        DrpMainType.Items.Clear();
        DataTable dt = MController.GetAccountHead(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, DrpAccountCategory.SelectedIndex, 95);
        //DrpMainType.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(DrpMainType, dt, 0, 1);
        if (dt.Rows.Count > 0)
        {
            DrpMainType.SelectedIndex = 0;
        }
    }
    private void GetSubAccountType()
    {
        DrpSubType.Items.Clear();
        if (DrpMainType.Items.Count > 0)
        {
            DataTable dt = MController.GetAccountHead(int.Parse(DrpMainType.SelectedItem.Value.ToString()), Constants.IntNullValue, Constants.IntNullValue, DrpAccountCategory.SelectedIndex, Constants.AC_SubTypeId);
            DrpSubType.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDxComboBoxList(DrpSubType, dt, 0, 1);
            if (dt.Rows.Count > 0)
            {
                DrpSubType.SelectedIndex = 0;
            }
        }
    }
    private void GetDetailAccountType()
    {
        DrpDetailType.Items.Clear();
        if (DrpSubType.Items.Count > 0)
        {
            DataTable dt = MController.GetAccountHead(int.Parse(DrpMainType.SelectedItem.Value.ToString()), int.Parse(DrpSubType.SelectedItem.Value.ToString()), Constants.IntNullValue, DrpAccountCategory.SelectedIndex, Constants.AC_DetailTypeId);
            DrpDetailType.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
            clsWebFormUtil.FillDxComboBoxList(DrpDetailType, dt, 0, 1);
            if (dt.Rows.Count > 0)
            {
                DrpDetailType.SelectedIndex = 0;
            }
        }
    }
    private void ShowReport(int pReportType)
    {
        DocumentPrintController dPrint = new DocumentPrintController();
        RptAccountController rptAccountCtl = new RptAccountController();
        DataTable dt = dPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        DataSet ds = rptAccountCtl.CurrentAssets(int.Parse(drpDistributor.SelectedItem.Value.ToString()), DrpAccountCategory.SelectedIndex, int.Parse(DrpMainType.SelectedItem.Value.ToString()), int.Parse(DrpSubType.SelectedItem.Value.ToString()), int.Parse(DrpDetailType.SelectedItem.Value.ToString()), DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"));

        ReportDocument CrpReport = new ReportDocument();
        if (ChkClosingBalance.Checked)
        {
            CrpReport = new CrpCurrentAssets();
        }
        else
        {
            CrpReport = new CrpCurrentAssetsWitoutZero();
        }
        //CORNBusinessLayer.Reports.CrpCurrentAssets CrpReport = new CORNBusinessLayer.Reports.CrpCurrentAssets();
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("From_Date", DateTime.Parse(txtStartDate.Text));
        CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
        CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
        CrpReport.SetParameterValue("AccountType", DrpMainType.SelectedItem.Text);
        CrpReport.SetParameterValue("SubAccountType", DrpSubType.SelectedItem.Text);
        CrpReport.SetParameterValue("AccountDetail", DrpDetailType.SelectedItem.Text);
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", pReportType);
        const string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
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

    protected void DrpMainType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetSubAccountType();
        this.GetDetailAccountType();
    }

    protected void DrpSubType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetDetailAccountType();
    }

    protected void DrpAccountCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetAccountType();
        GetSubAccountType();
        GetDetailAccountType();
    }
}