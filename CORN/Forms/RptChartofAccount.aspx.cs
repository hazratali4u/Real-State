using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System.Web;

/// <summary>
/// Form For Chart of Account Report
/// </summary>
public partial class Forms_RptChartofAccount : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load Function
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");
        if (!Page.IsPostBack)
        {
            this.GetAccountType();
            this.GetSubAccountType();
            this.GetDetailAccountType();
        }
    }

    /// <summary>
    /// Loads Account Types To AccountType Combo
    /// </summary>
    private void GetAccountType()
    {
        DrpMainType.Items.Clear();
        AccountHeadController MController = new AccountHeadController();
        DataTable dt = MController.GetAccountHead(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, DrpAccountCategory.SelectedIndex, Constants.AC_MainTypeId);
        DrpMainType.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(DrpMainType, dt, 0, 1);
        if (dt.Rows.Count > 0)
        {
            DrpMainType.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Loads Account Sub Types To SubType Combo
    /// </summary>
    private void GetSubAccountType()
    {
        DrpSubType.Items.Clear();

        AccountHeadController MController = new AccountHeadController();
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

    /// <summary>
    /// Loads Account Detail Types To Detail Type Combo
    /// </summary>
    private void GetDetailAccountType()
    {
        DrpDetailType.Items.Clear();

        AccountHeadController MController = new AccountHeadController();
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
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }
    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetSubAccountType();
        this.GetDetailAccountType();
    }
    public void ShowReport(int p_TYPE_ID)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        DataTable dt = DPrint.SelectReportTitle(Constants.IntNullValue);
        DataSet ds = RptAccountCtl.SelectRptChartofAccount(DrpAccountCategory.SelectedIndex, int.Parse(DrpMainType.SelectedItem.Value.ToString()), int.Parse(DrpSubType.SelectedItem.Value.ToString()), int.Parse(DrpDetailType.SelectedItem.Value.ToString()));
        CORNBusinessLayer.Reports.CrpchartOfAccunt CrpReport = new CORNBusinessLayer.Reports.CrpchartOfAccunt();
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        this.Session.Add("CrpReport", CrpReport);
        this.Session.Add("ReportType", p_TYPE_ID);
        string url = "'Default.aspx'";
        string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }
    protected void DrpSubType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GetDetailAccountType();
    }
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }
    protected void DrpAccountCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetAccountType();
        GetSubAccountType();
        GetDetailAccountType();
    }
}
