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

public partial class Forms_RptDailyClosing : System.Web.UI.Page
{
    readonly DistributorController DController = new DistributorController();
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {   
            LoadDistributor();
            LoadAccountHead();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    private void LoadDistributor()
    {
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        LstLocations.DataSource = dt;
        LstLocations.DataTextField = "DISTRIBUTOR_NAME";
        LstLocations.DataValueField = "DISTRIBUTOR_ID";
        LstLocations.DataBind();

        //drpDistributor.Items.Add("All", Constants.IntNullValue);
        //clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, false);
        //if (dt.Rows.Count > 0)
        //{
        //    drpDistributor.SelectedIndex = 0;
        //}
    }

    public void LoadAccountHead()
    {
        AccountHeadController mAccountController = new AccountHeadController();
        DataTable dtHead = mAccountController.SelectAccountHead(Constants.AC_AccountHeadId, Constants.LongNullValue, 4);
        if (dtHead.Rows.Count > 0)
        {
            LstAccountHeads.DataSource = dtHead;
            LstAccountHeads.DataTextField = "ACCOUNT_DETAIL";
            LstAccountHeads.DataValueField = "ACCOUNT_HEAD_ID";
            LstAccountHeads.DataBind();
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
    private void ShowReport(int Report_Type)
    {
        string p_Locations = "";
        string p_Locations_Name = "";
        string p_AccountHeads = "";
        string p_AccountHeads_Name = "";
        int TYPE_ID = 0;
        for (int i = 0; i < LstLocations.Items.Count; i++)
        {
            if (LstLocations.Items[i].Selected == true)
            {
                p_Locations += LstLocations.Items[i].Value.ToString() + ",";
                p_Locations_Name += LstLocations.Items[i].Text.ToString() + ",";
            }
        }
        if (p_Locations == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Location.');", true);
            return;
        }

        for (int i = 0; i < LstAccountHeads.Items.Count; i++)
        {
            if (LstAccountHeads.Items[i].Selected == true)
            {
                p_AccountHeads += LstAccountHeads.Items[i].Value.ToString() + ",";
                p_AccountHeads_Name += LstAccountHeads.Items[i].Text.ToString() + ",";
            }
        }
        //if (p_AccountHeads == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Account Head.');", true);
        //    return;
        //}
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(Session["DISTRIBUTOR_ID"].ToString()));
        {
            decimal Opening_Balance = 0;
            DataTable dt_Opening = new DataTable();
            ReportDocument CrpReport = new ReportDocument();
            ReportDocument SubReport = new ReportDocument();
            ReportDocument SubReport2 = new ReportDocument();
            ds = RptCustCtl.GetDailyClosing(Constants.IntNullValue, Constants.IntNullValue, p_Locations, p_AccountHeads, ""
               , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
            dt_Opening = RptCustCtl.GetDailyOpening(Constants.IntNullValue, Constants.IntNullValue, p_Locations, p_AccountHeads, ""
               , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), 2);
            if(dt_Opening.Rows.Count > 0)
            {
                Opening_Balance = decimal.Parse(dt_Opening.Rows[0]["OP_BALANCE"].ToString());
            }
            else
            {
                Opening_Balance = 0;
            }
            CrpReport = new RptDailyClosingReport();
            SubReport = CrpReport.OpenSubreport("SubReport");
            SubReport2 = CrpReport.OpenSubreport("SubReport2");
            CrpReport.SetDataSource(ds);
            SubReport.SetDataSource(ds);
            SubReport2.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", "");
            CrpReport.SetParameterValue("Report_Name", "Daily Closing Report");
            CrpReport.SetParameterValue("PROJECT_NAME", "");
            CrpReport.SetParameterValue("CATEGORY_NAME", "");
            CrpReport.SetParameterValue("SUB_CATEGORY_NAME", "");
            CrpReport.SetParameterValue("TYPE_ID", ddlType.SelectedItem.Value.ToString());
            CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("USERNAME", Session["UserName"].ToString());
            CrpReport.SetParameterValue("OP_BALANCE", Opening_Balance);
            Session.Add("CrpReport", CrpReport);
            Session.Add("ReportType", Report_Type);
            const string url = "'Default.aspx'";
            const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "OpenWindow", script);
        }
    }
}