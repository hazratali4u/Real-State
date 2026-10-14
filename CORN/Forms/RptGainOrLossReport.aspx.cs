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

public partial class Forms_RptGainOrLossReport : System.Web.UI.Page
{
    public string p_Customers = null, p_Categories = null, p_Streets = null, p_Measurments = null, p_Itemss = null;
    readonly LedgerController LController = new LedgerController();
    readonly VenderEntryController RptCustCtl = new VenderEntryController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SaleType();
            LoadDistributor();
            //DATA(int.Parse(ddlType.SelectedItem.Value.ToString()));
            LoadPrincipal();
            
            LoadProjects();

            LoadSubCategories(int.Parse(ddlType.SelectedItem.Value.ToString()));
            LoadMeasurmements(int.Parse(ddlType.SelectedItem.Value.ToString()));
            LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
            LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));

            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtStartDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            txtEndDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
        }
    }
    public void DATA(int P_Type)
    {
        this.Session["dt"] = null;
        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == Constants.IntNullValue)
        {
            P_Type = 3;
        }
        DataTable dt = RptCustCtl.GetLoadMeasurement(int.Parse(drpDistributor.SelectedItem.Value.ToString()), Constants.IntNullValue, p_Customers, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  Constants.DateNullValue, Constants.DateNullValue, P_Type);
        this.Session.Add("dt", dt);
    }
    public void LoadProjects()
    {
        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable()
                      group rows by new { BLOCK_ID = rows["PROJECT_ID"], BLOCK_NAME = rows["PROJECT_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                DrpProjects.DataSource = Blocks;
                DrpProjects.TextField = "PROJECT_NAME";
                DrpProjects.ValueField = "PROJECT_ID";
                DrpProjects.DataBind();
                DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
                myDefaultItem.Selected = true;
                DrpProjects.Items.Insert(0, myDefaultItem);
                //ddlBlock.SelectedIndex = 0;
            }
        }

        //DrpProjects.DataSource = DropDowns.PROJECTDropDown();
        //DrpProjects.TextField = "PROJECT_NAME";
        //DrpProjects.ValueField = "PROJECT_ID";
        //DrpProjects.DataBind();
        //DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        //myDefaultItem.Selected = true;
        //DrpProjects.Items.Insert(0, myDefaultItem);
    }
    public void LoadSubCategories(int P_Type)
    {
        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == Constants.IntNullValue)
        {
            P_Type = 3;
        }
        DataTable dt = RptCustCtl.GetLoadMeasurement(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()), p_Customers, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  Constants.DateNullValue, Constants.DateNullValue, P_Type);
        if (dt.Rows.Count > 0)
            {
                var qry = from rows in dt.AsEnumerable()
                          group rows by new { BLOCK_ID = rows["SUB_CATEGORY_ID"], BLOCK_NAME = rows["SUB_CATEGORY_NAME"] } into grp
                          select grp.First();
                if (qry.Any())
                {
                    var Blocks = qry.CopyToDataTable();
                    LstSubCategory.DataSource = Blocks;
                    LstSubCategory.DataTextField = "SUB_CATEGORY_NAME";
                    LstSubCategory.DataValueField = "SUB_CATEGORY_ID";
                    LstSubCategory.DataBind();
                }
            }
    }
    public void LoadMeasurmements(int P_Type)
    {
        //DataTable dt = (DataTable)this.Session["dt"];
        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == Constants.IntNullValue)
        {
            P_Type = 3;
        }
        for (int i = 0; i < LstSubCategory.Items.Count; i++)
        {
            if (LstSubCategory.Items[i].Selected == true)
            {
                p_Categories += LstSubCategory.Items[i].Value.ToString() + ",";
            }
        }
        DataTable dt = RptCustCtl.GetLoadMeasurement(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()), p_Customers, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  Constants.DateNullValue, Constants.DateNullValue, P_Type);
        if (dt.Rows.Count > 0)
            {
                var qry = from rows in dt.AsEnumerable()
                          group rows by new { BLOCK_ID = rows["MEASURMENT_ID"], BLOCK_NAME = rows["MEASURMENT"] } into grp
                          select grp.First();
                if (qry.Any())
                {
                    var Blocks = qry.CopyToDataTable();
                    LstMeasurement.DataSource = Blocks;
                    LstMeasurement.DataTextField = "MEASURMENT";
                    LstMeasurement.DataValueField = "MEASURMENT_ID";
                    LstMeasurement.DataBind();
                }
            }
    }
    
    public void LoadBlock(int P_Type)
    {

        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == Constants.IntNullValue)
        {
            P_Type = 3;
        }
        for (int i = 0; i < LstSubCategory.Items.Count; i++)
        {
            if (LstSubCategory.Items[i].Selected == true)
            {
                p_Categories += LstSubCategory.Items[i].Value.ToString() + ",";
            }
        }
        for (int i = 0; i < LstMeasurement.Items.Count; i++)
        {
            if (LstMeasurement.Items[i].Selected == true)
            {
                p_Measurments += LstMeasurement.Items[i].Value.ToString() + ",";
            }
        }
        DataTable dt = RptCustCtl.GetLoadMeasurement(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()), p_Customers, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  Constants.DateNullValue, Constants.DateNullValue, P_Type);
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable()
                      group rows by new { BLOCK_ID = rows["STREET_ID"], BLOCK_NAME = rows["STREET_NAME"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                LstBlock.DataSource = Blocks;
                LstBlock.DataTextField = "STREET_NAME";
                LstBlock.DataValueField = "STREET_ID";
                LstBlock.DataBind();
            }
        }
    }
    public void LoadPlots(int P_Type)
    {
        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == Constants.IntNullValue)
        {
            P_Type = 3;
        }
        for (int i = 0; i < LstSubCategory.Items.Count; i++)
        {
            if (LstSubCategory.Items[i].Selected == true)
            {
                p_Categories += LstSubCategory.Items[i].Value.ToString() + ",";
            }
        }
        for (int i = 0; i < LstMeasurement.Items.Count; i++)
        {
            if (LstMeasurement.Items[i].Selected == true)
            {
                p_Measurments += LstMeasurement.Items[i].Value.ToString() + ",";
            }
        }
        for (int i = 0; i < LstBlock.Items.Count; i++)
        {
            if (LstBlock.Items[i].Selected == true)
            {
                p_Streets += LstBlock.Items[i].Value.ToString() + ",";
            }
        }
        DataTable dt = RptCustCtl.GetLoadMeasurement(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()), p_Customers, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  Constants.DateNullValue, Constants.DateNullValue, P_Type);
        if (dt.Rows.Count > 0)
        {
            var qry = from rows in dt.AsEnumerable()
                      group rows by new { BLOCK_ID = rows["ITEM_INFO_ID"], BLOCK_NAME = rows["ITEM"] } into grp
                      select grp.First();
            if (qry.Any())
            {
                var Blocks = qry.CopyToDataTable();
                LstPlots.DataSource = Blocks;
                LstPlots.DataTextField = "ITEM";
                LstPlots.DataValueField = "ITEM_INFO_ID";
                LstPlots.DataBind();
            }
        }



        //DbCalls db = new DbCalls();
        //var qry = (from k in db.tbl_PURCHASE_MASTER
        //           join c in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals c.ITEM_INFO_ID
        //           select new { k.PURCHASE_MASTER_ID, k.PROJECT_ID, k.ITEM_INFO_ID, ITEM_NAME = "Ref.# " + k.PURCHASE_MASTER_ID + ", " + c.ITEM_NAME }).Where(x => x.PROJECT_ID == Project_ID).ToList();
        //LstPlots.DataSource = qry;
        //LstPlots.DataTextField = "ITEM_NAME";
        //LstPlots.DataValueField = "ITEM_INFO_ID";
        //LstPlots.DataBind();



    }
    public void SaleType()
    {
        DevExpress.Web.ListEditItem myDefaultItem = new DevExpress.Web.ListEditItem("All", Constants.IntNullValue.ToString());
        myDefaultItem.Selected = true;
        ddlType.Items.Insert(0, myDefaultItem);
    }
    private void LoadPrincipal()
    {
        //SKUPriceDetailController PController = new SKUPriceDetailController();
        //LstCustomer.Items.Clear();
        //if (int.Parse(ddlType.SelectedItem.Value.ToString()) == int.Parse(Constants.IntNullValue.ToString()))
        //{
        //    DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, 10, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        //    clsWebFormUtil.FillListBox(LstCustomer, m_dt, 0, 1, false);
        //    for (int i = 0; i < LstCustomer.Items.Count; i++)
        //    {
        //        LstCustomer.Items[i].Selected = true;
        //    }
        //    ChbAllCustomer.Checked = true;
        //}
        //else
        //{
        //    int TYPE;
        //    if (int.Parse(ddlType.SelectedItem.Value.ToString()) == 1)
        //    {
        //        TYPE = 8;
        //    }
        //    else
        //    {
        //        TYPE = 9;
        //    }
        //    DataTable m_dt = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(this.Session["UserId"].ToString()), Constants.IntNullValue, TYPE, DateTime.Parse(this.Session["CurrentWorkDate"].ToString()));
        //    clsWebFormUtil.FillListBox(LstCustomer, m_dt, 0, 1, false);
        //    for (int i = 0; i < LstCustomer.Items.Count; i++)
        //    {
        //        LstCustomer.Items[i].Selected = true;
        //    }
        //    ChbAllCustomer.Checked = true;
        //}
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

    private void showReport(int reportType)
    {
        //for (int i = 0; i < LstCustomer.Items.Count; i++)
        //{
        //    if (LstCustomer.Items[i].Selected == true)
        //    {
        //        p_Customers += LstCustomer.Items[i].Value.ToString() + ",";
        //    }
        //}
        //if (p_Customers == null)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Customer.');", true);
        //    return;
        //}
        for (int i = 0; i < LstSubCategory.Items.Count; i++)
        {
            if (LstSubCategory.Items[i].Selected == true)
            {
                p_Categories += LstSubCategory.Items[i].Value.ToString() + ",";
            }
        }
        if (p_Categories == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Category.');", true);
            return;
        }
        for (int i = 0; i < LstMeasurement.Items.Count; i++)
        {
            if (LstMeasurement.Items[i].Selected == true)
            {
                p_Measurments += LstMeasurement.Items[i].Value.ToString() + ",";
            }
        }
        if (p_Measurments == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Measurement.');", true);
            return;
        }

        for (int i = 0; i < LstBlock.Items.Count; i++)
        {
            if (LstBlock.Items[i].Selected == true)
            {
                p_Streets += LstBlock.Items[i].Value.ToString() + ",";
            }
        }
        if (p_Streets == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Block & Streets.');", true);
            return;
        }
        for (int i = 0; i < LstPlots.Items.Count; i++)
        {
            if (LstPlots.Items[i].Selected == true)
            {
                p_Itemss += LstPlots.Items[i].Value.ToString() + ",";
            }
        }
        if (p_Itemss == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Select Item.');", true);
            return;
        }
        DocumentPrintController DPrint = new DocumentPrintController();
        int Type;
        if (int.Parse(ddlType.SelectedItem.Value.ToString()) == int.Parse(Constants.IntNullValue.ToString()))
        {
            Type = 3;
        }
        else
        {
            Type = int.Parse(ddlType.SelectedItem.Value.ToString());
        }
       
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(drpDistributor.SelectedItem.Value.ToString()));
        {
            ReportDocument CrpReport = new ReportDocument();
            
                ds = RptCustCtl.GetGainOrLossOnSale(int.Parse(drpDistributor.SelectedItem.Value.ToString()), int.Parse(DrpProjects.SelectedItem.Value.ToString()),null, p_Categories, p_Streets, p_Measurments, p_Itemss,
                  DateTime.Parse(txtStartDate.Text + " 00:00:00"), DateTime.Parse(txtEndDate.Text + " 23:59:59"), Type);
                CrpReport = new RptGainOrLossOnSale();
                CrpReport.SetDataSource(ds);
                CrpReport.Refresh();

                CrpReport.SetParameterValue("FromDate", DateTime.Parse(txtStartDate.Text));
                CrpReport.SetParameterValue("To_date", DateTime.Parse(txtEndDate.Text));
                CrpReport.SetParameterValue("Location", drpDistributor.SelectedItem.Text);
                //CrpReport.SetParameterValue("Principal", LstCustomer.SelectedItem.Text);
                CrpReport.SetParameterValue("Project", DrpProjects.SelectedItem.Text);
                CrpReport.SetParameterValue("CompanyName", dt.Rows[0]["COMPANY_NAME"].ToString());
            CrpReport.SetParameterValue("USERNAME", Session["UserName"].ToString());


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
            //PanelItems.Visible = false;
            LoadPrincipal();
        }
        else
        {
            CustomerItems();
            PanelFromDate.Visible = false;
            PanelToDate.Visible = false;
            //PanelItems.Visible = true;
        }
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

    protected void DrpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        Hp_SelectedIndexChanged(null, null);
    }

    protected void DrpProjects_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSubCategories(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadMeasurmements(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }

    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        DATA(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadProjects();
        LoadSubCategories(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadMeasurmements(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }

    protected void ChbAllSubCategory_CheckedChanged(object sender, EventArgs e)
    {
        LoadMeasurmements(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }

    //protected void LstSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    LoadMeasurmements(int.Parse(ddlType.SelectedItem.Value.ToString()));
    //    LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
    //    LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    //}

    protected void ChbAllMeasurement_CheckedChanged(object sender, EventArgs e)
    {
        LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }

    //protected void LstMeasurement_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    LoadBlock(int.Parse(ddlType.SelectedItem.Value.ToString()));
    //    LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    //}

    protected void ChbAllBlock_CheckedChanged(object sender, EventArgs e)
    {
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }

    protected void LstBlock_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPlots(int.Parse(ddlType.SelectedItem.Value.ToString()));
    }
}