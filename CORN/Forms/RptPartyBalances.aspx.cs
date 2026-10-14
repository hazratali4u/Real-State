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

public partial class Forms_RptPartyBalances : System.Web.UI.Page
{
    public string p_Projects = null, p_Categories = null, p_SubCategories = null, p_Projects_Name = null, p_Categories_Name = null, p_SubCategories_Name = null, p_Customers = null;
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
            // All Parties
            lblCustomer.Text = "All Parties";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 7);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }
        }
        else if (ddlType.SelectedIndex == 1)
        {
            // Customer
            lblCustomer.Text = "Customer";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 1);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }
        }
        else if (ddlType.SelectedIndex == 2)
        {
            // Vendors
            lblCustomer.Text = "Vendor";
            DataTable dt = DController.SelectCustomerInfo(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 2);
            ddlCustomer.Items.Add("All", Constants.IntNullValue);
            clsWebFormUtil.FillDxComboBoxList(ddlCustomer, dt, 0, 1, false);
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.SelectedIndex = 0;
            }
        }
        else if (ddlType.SelectedIndex == 3)
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
        if (int.Parse(ddlCustomer.SelectedItem.Value.ToString()) != Constants.IntNullValue)
        {
            p_Customers += ddlCustomer.SelectedItem.Value;
        }
        else
        {
            for (int i = 0; i < ddlCustomer.Items.Count; i++)
            {
                //if (int.Parse(ddlCustomer.SelectedItem.Value.ToString()) != Constants.IntNullValue)
                //{
                    p_Customers += ddlCustomer.Items[i].Value.ToString() + ",";
                //}
            }
        }

        //for (int i = 0; i < LstProjects.Items.Count; i++)
        //{
        //    if (LstProjects.Items[i].Selected == true)
        //    {
        //        p_Projects += LstProjects.Items[i].Value.ToString() + ",";
        //        p_Projects_Name += LstProjects.Items[i].Text.ToString() + ",";
        //    }
        //}
        //if (p_Projects == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Project.');", true);
        //    return;
        //}
        //for (int i = 0; i < LstCategory.Items.Count; i++)
        //{
        //    if (LstCategory.Items[i].Selected == true)
        //    {
        //        p_Categories += LstCategory.Items[i].Value.ToString() + ",";
        //        p_Categories_Name += LstCategory.Items[i].Text.ToString() + ",";
        //    }
        //}
        //if (p_Categories == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
        //    return;
        //}
        //for (int i = 0; i < LstSubCategory.Items.Count; i++)
        //{
        //    if (LstSubCategory.Items[i].Selected == true)
        //    {
        //        p_SubCategories += LstSubCategory.Items[i].Value.ToString() + ",";
        //        p_SubCategories_Name += LstSubCategory.Items[i].Text.ToString() + ",";
        //    }
        //}
        //if (p_SubCategories == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Sub Category.');", true);
        //    return;
        //}
        DocumentPrintController DPrint = new DocumentPrintController();
        DataSet ds = null;
        int TYPE_ID = 0;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {
            ReportDocument CrpReport = new ReportDocument();
            if (ChkClosingBalance.Checked)
            {
                CrpReport = new RptPartyBalanceReport();
                TYPE_ID = 1;
            }
            else
            {
                CrpReport = new RptPartyBalanceWithoutZeroReport();
                TYPE_ID = 2;
            }
            ds = RptCustCtl.GetPartyBalances(int.Parse(drpDistributor.SelectedItem.Value.ToString()), p_Customers, p_Projects, p_Categories, p_SubCategories
                , DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), TYPE_ID);
            
            
            CrpReport.SetDataSource(ds);
            CrpReport.Refresh();
            CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
            CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
            CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
            if (ddlType.SelectedIndex == 0)
            {
                CrpReport.SetParameterValue("Report_Name", "All Parties Balance Summary Report");
            }
            else if (ddlType.SelectedIndex == 1)
            {
                CrpReport.SetParameterValue("Report_Name", "Customer Balance Summary Report");
            }
            else if (ddlType.SelectedIndex == 2)
            {
                CrpReport.SetParameterValue("Report_Name", "Vendor Balance Summary Report");
            }
            else if (ddlType.SelectedIndex == 3)
            {
                CrpReport.SetParameterValue("Report_Name", "Customer / Vendor Balance Summary Report");
            }
            CrpReport.SetParameterValue("PROJECT_NAME", "");
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
    protected void btnViewPDF_Click(object sender, EventArgs e)
    {
        ShowReport(0);
    }
    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        ShowReport(1);
    }

    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPrincipal();
    }
}