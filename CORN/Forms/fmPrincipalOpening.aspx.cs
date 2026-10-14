using System;
using System.Web.UI;
using CORNCommon.Classes;
using CORNBusinessLayer.Classes;
using System.Data;
using System.Linq;

public partial class Forms_fmPrincipalOpening : System.Web.UI.Page
{   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //if (IsDayClosed())
            //{
            //    UserController UserCtl = new UserController();

            //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
            //    Session.Clear();
            //    System.Web.Security.FormsAuthentication.SignOut();
            //    Response.Redirect("../Login.aspx");
            //}

            LoadDistributor();
            LoadPrincipal();
           
            Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
            txtOpeningDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");
            LoadOpeningInformation();
            txtOpeningDate.Attributes.Add("readonly", "readonly");
        }
    }
    private void LoadDistributor()
    {
        var dController = new DistributorController();
        var dt = dController.SelectDistributorInfo(Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), int.Parse(Session["CompanyId"].ToString()));
      

        clsWebFormUtil.FillDxComboBoxList(drpDistributor, dt, 0, 2);

        if (dt.Rows.Count > 0)
        {
            drpDistributor.SelectedIndex = 0;
        }
    }
    private void LoadPrincipal()
    {
        if (drpDistributor.Items.Count > 0)
        {
            DbCalls db = new DbCalls();
            var qry = (from k in db.VENDORS
                       select new { k.VENDOR_ID, k.VENDOR_NAME, k.IS_DELETED, k.TYPE_ID, k.CUSTOMER_TYPE }).Where(x => x.IS_DELETED == false && x.TYPE_ID == 2 || x.IS_DELETED == false && x.TYPE_ID == 1 && x.CUSTOMER_TYPE == 2).ToList();
            //ddlVendor.DataSource = DropDowns.VendorDropDown();
            drpPrincipal.DataSource = qry;
            drpPrincipal.TextField = "VENDOR_NAME";
            drpPrincipal.ValueField = "VENDOR_ID";
            drpPrincipal.DataBind();
            drpPrincipal.SelectedIndex = 0;
            //var PController = new SKUPriceDetailController();
            //DataTable dtVendor = PController.SelectDataPrice(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, int.Parse(Session["UserId"].ToString()), Constants.IntNullValue, 0, DateTime.Parse(Session["CurrentWorkDate"].ToString()));

            //clsWebFormUtil.FillDxComboBoxList(drpPrincipal, dtVendor, 0, 1, true);
            //if (dtVendor.Rows.Count > 0)
            //{
            //    drpPrincipal.SelectedIndex = 0;
            //}
        }
    }
   
    #region Opening Balance

    private void LoadOpeningInformation()
    {
        PurchaseController mPurchase = new PurchaseController();
        Configuration.SystemCurrentDateTime = (DateTime)this.Session["CurrentWorkDate"];
        txtOpeningDate.Text = Configuration.SystemCurrentDateTime.ToString("dd-MMM-yyyy");

        txtOpeningBalance.Text = "0";
        txtRemarks.Text = "";
        rblOpening.SelectedValue = "0";
        masterId.Value = "0";
        btnSaveOpeningBalance.Enabled = true;
        btnSaveOpeningBalance.Text = "Save";

        if (drpPrincipal.Items.Count > 0)
        {
            DataTable dtOpening = mPurchase.SelectPrincipalOpening(Convert.ToInt32(drpDistributor.SelectedItem.Value), Convert.ToInt32(drpPrincipal.SelectedItem.Value));
            if (dtOpening.Rows.Count > 0)
            {
                if (decimal.Parse(dtOpening.Rows[0]["BALANCE"].ToString()) == decimal.Parse(dtOpening.Rows[0]["DEBIT_AMOUNT"].ToString()))
                {
                    txtOpeningDate.Text = Convert.ToDateTime(dtOpening.Rows[0]["DATE"]).ToString("dd-MMM-yyyy");
                    decimal Balance = decimal.Parse(dtOpening.Rows[0]["BALANCE"].ToString());
                    txtOpeningBalance.Text = Math.Round(Balance,0).ToString();
                    txtRemarks.Text = dtOpening.Rows[0]["REMARKS"].ToString();
                    rblOpening.SelectedValue = dtOpening.Rows[0]["TYPE_ID"].ToString();
                    masterId.Value = dtOpening.Rows[0]["PURCHASE_MASTER_ID"].ToString();
                    btnSaveOpeningBalance.Enabled = true;
                    btnSaveOpeningBalance.Text = "Update";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Plz Delete Realization for Update.');", true);
                    btnSaveOpeningBalance.Enabled = false;
                }
            }
        }
    }

    protected void btnOpeningBalance_Click(object sender, EventArgs e)
    {
        //if (IsDayClosed())
        //{
        //    UserController UserCtl = new UserController();

        //    UserCtl.InsertUserLogoutTime(Convert.ToInt32(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"]));
        //    Session.Clear();
        //    System.Web.Security.FormsAuthentication.SignOut();
        //    Response.Redirect("../Login.aspx");
        //}

        DataControl dc = new DataControl();
        LedgerController ledgerCtl = new LedgerController();
        DateTime dtOpening = Constants.DateNullValue;
       
        if (txtOpeningDate.Text.Length > 0)
        {
            dtOpening = Convert.ToDateTime(txtOpeningDate.Text);
        }
        DataTable dtConfig = GetCOAConfiguration();
        long MasterID = Constants.LongNullValue;
        string PrincipalName = drpPrincipal.SelectedItem.Text;
        if (btnSaveOpeningBalance.Text == "Save" && masterId.Value=="0")
        {

            if (ledgerCtl.InsertVendorOpening(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, int.Parse(rblOpening.SelectedValue)
                , Convert.ToDateTime(txtOpeningDate.Text), Convert.ToInt32(drpPrincipal.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtOpeningBalance.Text))
                , int.Parse(this.Session["UserId"].ToString()), ref MasterID, dtConfig, PrincipalName))
            {
                LoadOpeningInformation();
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Opening Added Successfully.');", true);

                btnSaveOpeningBalance.Text = "Update";
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('some error occurred.');", true);
            }
        }
        else
        {
            if (ledgerCtl.DeletePrincipalOpening(Convert.ToInt32(drpDistributor.SelectedItem.Value), Convert.ToInt32(drpPrincipal.SelectedItem.Value),Convert.ToInt64(masterId.Value)))
            {
                if (ledgerCtl.InsertVendorOpening(Convert.ToInt32(drpDistributor.SelectedItem.Value), txtRemarks.Text, int.Parse(rblOpening.SelectedValue)
                    , Convert.ToDateTime(txtOpeningDate.Text), Convert.ToInt32(drpPrincipal.SelectedItem.Value), Convert.ToDecimal(dc.chkNull_0(txtOpeningBalance.Text))
                    , int.Parse(this.Session["UserId"].ToString()), ref MasterID, dtConfig, PrincipalName))
                {
                    LoadOpeningInformation();
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Opening Updated Successfully.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('some error occurred.');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('some error occurred.');", true);
            }
        }
    }
   
    #endregion
    protected void drpDistributor_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadOpeningInformation();
    }
    protected void drpPrincipal_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadOpeningInformation();
    }

    private bool IsDayClosed()
    {
        DistributorController DistrCtl = new DistributorController();
        try
        {
            DataTable dtDayClose = DistrCtl.MaxDayClose(Convert.ToInt32(Session["DISTRIBUTOR_ID"]), 3);
            if (dtDayClose.Rows.Count > 0)
            {
                if (Convert.ToDateTime(Session["CurrentWorkDate"]) == Convert.ToDateTime(dtDayClose.Rows[0]["DayClose"]))
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {

            throw;
        }
    }

    private DataTable GetCOAConfiguration()
    {
        try
        {
            COAMappingController _cController = new COAMappingController();
            DataTable dt = _cController.SelectCOAConfiguration(5, Constants.ShortNullValue, Constants.LongNullValue, "Level 4");
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg3", "alert('Error Occured: \n" + ex + "');", true);
            return null;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        txtOpeningBalance.Text = "0";
        txtRemarks.Text = "";
        btnSaveOpeningBalance.Text = "Save";
    }
}