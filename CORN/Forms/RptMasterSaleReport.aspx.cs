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

public partial class Forms_RptMasterSaleReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadPrincipal();
            LoadDistributor();
            LoadProjects();
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void LoadProjects()
    {
        DrpProjects.DataSource = DropDowns.PROJECTDropDown();
        DrpProjects.TextField = "PROJECT_NAME";
        DrpProjects.ValueField = "PROJECT_ID";
        DrpProjects.DataBind();
        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        DrpProjects.Items.Insert(0, myDefaultItem);
    }
    private void LoadPrincipal()
    {
        SKUPriceDetailController PController = new SKUPriceDetailController();
        DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 6, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        DrpPrincipal.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
        clsWebFormUtil.FillDxComboBoxList(this.DrpPrincipal, m_dt, 0, 1, false);


        if (m_dt.Rows.Count > 0)
        {
            DrpPrincipal.SelectedIndex = 0;
        }

    }
    private void LoadDistributor()
    {
        DistributorController DController = new DistributorController();

        DataTable dt = DController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["CompanyId"].ToString()));

        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2, true);

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
                      DateTime.Parse(txtStartDate.Text + " 00:00:00"), 1);

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
        DocumentPrintController DPrint = new DocumentPrintController();
        VenderEntryController RptCustCtl = new VenderEntryController();

        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {

            

            ReportDocument CrpReport = new ReportDocument();
            if (Hp.SelectedValue == "1")
            {
                ds = RptCustCtl.GetMasterSale(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()), int.Parse(DrpPrincipal.SelectedItem.Value.ToString()),
                  DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), int.Parse(Hp.SelectedValue));
                CrpReport = new CrpMasterSale();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
                CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
                CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
                CrpReport.SetParameterValue("Principal", DrpPrincipal.SelectedItem.Text);
                CrpReport.SetParameterValue("Project", DrpProjects.SelectedItem.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            }
            else
            {
                ds = RptCustCtl.GetCustomerInstallmentDetail(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()),
                     int.Parse(DrpPrincipal.SelectedItem.Value.ToString()), int.Parse(ddlItems.SelectedItem.Value.ToString()), int.Parse(Hp.SelectedValue));
                CrpReport = new CrpCustomerInstallments();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                //CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
                //CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
                CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
                CrpReport.SetParameterValue("Project", DrpProjects.SelectedItem.Text);
                //CrpReport.SetParameterValue("Op_Balance", LoadVendoerOpBalance());
                //CrpReport.SetParameterValue("opType", opType.Value);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
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
    protected void Hp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Hp.SelectedValue == "1")
        {
            PanelFromDate.Visible = true;
            PanelToDate.Visible = true;
            PanelItems.Visible = false;
            LoadPrincipal();
        }
        else
        {
            CustomerItems();
            PanelFromDate.Visible = false;
            PanelToDate.Visible = false;
            PanelItems.Visible = true;
        }
    }
    public void CustomerItems()
    {
        if (DrpPrincipal.Items.Count > 0)
        {
            int CustomerID = Convert.ToInt32(DrpPrincipal.SelectedItem.Value);
            DbCalls db = new DbCalls();
            var qry = (from k in db.tbl_SALE_INS_MASTER
                       join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
                       join b in db.BLOCKs on i.BLOCK_ID equals b.BLOCK_ID
                       join s in db.STREETs on i.STREET_ID equals s.STREET_ID
                       select new { k.SALE_INS_MASTER_ID, k.VENDOR_ID, k.ITEM_INFO_ID, ITEM_NAME = i.ITEM_NAME + ", " + b.BLOCK_NAME + ", " + s.STREET_NAME, k.SALE_CLOSED }).Where(x => x.SALE_CLOSED == false && x.VENDOR_ID == CustomerID).ToList();
            //if (qry.Count != 0)
            //{
                ddlItems.Items.Clear();
                ddlItems.DataSource = qry;
                ddlItems.TextField = "ITEM_NAME";
                ddlItems.ValueField = "SALE_INS_MASTER_ID";
                ddlItems.DataBind();

                DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                ddlItems.Items.Insert(0, myDefaultItem);
            //}
            //else
            //{
            //    ddlItems.Items.Clear();
            //    ddlItems.Items.Add(new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString()));
            //}
            //ddlItems.SelectedIndex = 0;
        }

    }
    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        Hp_SelectedIndexChanged(null, null);
    }
}