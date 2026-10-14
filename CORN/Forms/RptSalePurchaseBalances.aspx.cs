using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNBusinessLayer.Reports;
using CORNCommon.Classes;
using CrystalDecisions.CrystalReports.Engine;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_RptSalePurchaseBalances : System.Web.UI.Page
{
    readonly DistributorController DController = new DistributorController();
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadDistributor();
            AllParty();
            LoadAccountHead();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    private void LoadDistributor()
    {
        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));
        ddlLocation.DataSource = dt;
        ddlLocation.TextField = "DISTRIBUTOR_NAME";
        ddlLocation.ValueField = "DISTRIBUTOR_ID";
        ddlLocation.DataBind();
        ListEditItem myDefaultItem = new ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlLocation.Items.Insert(0, myDefaultItem);
    }
    private void AllParty()
    {
        // Customers / Vendors
        ddlParty.Items.Clear();
        DataTable dt = DController.SelectCustomerInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 3);
        ddlParty.Items.Add("All", Constants.IntNullValue);
        clsWebFormUtil.FillDxComboBoxList(ddlParty, dt, 0, 1, false);
        ListEditItem myDefaultItem = new ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlParty.Items.Insert(0, myDefaultItem);
    }
    public void LoadAccountHead()
    {
        LstBalanceTypes.DataSource = DropDowns.BALANCE_TYPEDropDown();
        LstBalanceTypes.DataTextField = "BalanceTypeName";
        LstBalanceTypes.DataValueField = "BalanceTypeID";
        LstBalanceTypes.DataBind();
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
        //string p_Locations_Name = "";
        string p_BalanceTypes = "";
        string p_BalanceTypes_Name = "";
        int TYPE_ID = 0;
        //for (int i = 0; i < LstLocations.Items.Count; i++)
        //{
        //    if (LstLocations.Items[i].Selected == true)
        //    {
        //        p_Locations += LstLocations.Items[i].Value.ToString() + ",";
        //        p_Locations_Name += LstLocations.Items[i].Text.ToString() + ",";
        //    }
        //}
        //if (p_Locations == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Location.');", true);
        //    return;
        //}

        for (int i = 0; i < LstBalanceTypes.Items.Count; i++)
        {
            if (LstBalanceTypes.Items[i].Selected == true)
            {
                p_BalanceTypes += LstBalanceTypes.Items[i].Value.ToString() + ",";
                p_BalanceTypes_Name += LstBalanceTypes.Items[i].Text.ToString() + ",";
            }
        }
        if (p_BalanceTypes == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Balance Type.');", true);
            return;
        }
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(5);
        {
            ReportDocument CrpReport = new ReportDocument();
            ds = RptCustCtl.GetRptSales_PurchaseBalancesReport(Constants.IntNullValue, int.Parse(ddlParty.SelectedItem.Value.ToString()), p_Locations, p_BalanceTypes, ""
               , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(ddlType.SelectedItem.Value.ToString()));
            CrpReport = new RptBalanceRegister();
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", ddlLocation.SelectedItem.Text.ToString());
            CrpReport.SetParameterValue("Report_Name", ddlType.SelectedItem.Text + " Register");
            CrpReport.SetParameterValue("PROJECT_NAME", ddlParty.SelectedItem.Text);
            CrpReport.SetParameterValue("CATEGORY_NAME", "");
            CrpReport.SetParameterValue("SUB_CATEGORY_NAME", "");
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
}