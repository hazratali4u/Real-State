using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CORNBusinessLayer.Classes;
using CORNCommon.Classes;
using System.Web;

/// <summary>
/// Form For Day Close
/// </summary>
public partial class Forms_frmDayCloseStatus : System.Web.UI.Page
{
    DistributorController mController = new DistributorController();
    DataControl DC = new DataControl();
    DataSet ds = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            LastClosedDay(int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["Distributor_Id"].ToString()));
            if (Grid_Hierarchy.Rows.Count > 1)
            {
                Grid_Hierarchy.UseAccessibleHeader = true;
                Grid_Hierarchy.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            bool chk = bool.Parse(Session["IS_CanReverseDayClose"].ToString());
            if (chk)
            {
                btnReverseDayClose.Visible = true;
            }
        }
    }

    
    private DataTable SelectPendingBills()
    {

        OrderEntryController mSkuController = new OrderEntryController();
        DataTable dtSkus = mSkuController.SelectPendingBills(Constants.LongNullValue,
            DateTime.Parse(Session["CurrentWorkDate"].ToString()), int.Parse(Session["DISTRIBUTOR_ID"].ToString()), int.Parse(HttpContext.Current.Session["UserID"].ToString()), Constants.IntNullValue,2);

        return dtSkus;
    }
    /// <summary>
    /// Gets Location(s) Last Day Close(s)
    /// </summary>
    /// <param name="UserId">User</param>
    /// <param name="p_Distributor">Location</param>
    private void LastClosedDay(int UserId, int p_Distributor)
    {
        btnDayClose.Visible = true;
        if (System.Configuration.ConfigurationManager.AppSettings["IsSync"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["IsSync"].ToString() == "1")
            {
                btnSync.Visible = true;
            }
        }
        
        DistributorController mDayClose = new DistributorController();
        DataTable dt = mDayClose.SelectMaxDayClose(UserId, p_Distributor);
        if (dt.Rows.Count > 0)
        {
            this.Session.Add("CurrentWorkDate", DateTime.Parse(dt.Rows[0]["CLOSING_DATE"].ToString()));
        }
        else
        {
            this.Session.Add("CurrentWorkDate", DateTime.Now);
            rblDistributorTypes.Visible = true;
        }
        GetLastClosedDay(UserId, p_Distributor, 4);
    }

    /// <summary>
    /// Loads Location(s) Last Day Close(s) To Grid
    /// </summary>
    /// <param name="UserId">User</param>
    /// <param name="p_Distributor">Location</param>
    /// <param name="p_Status">Status</param>
    private void GetLastClosedDay(int UserId, int p_Distributor, int p_Status)
    {
        DataTable dtable = mController.MaxDayClose(int.Parse(this.Session["UserId"].ToString()), p_Status);
        Grid_Hierarchy.DataSource = dtable;
        Grid_Hierarchy.DataBind();
    }

    /// <summary>
    /// Loads Locations(Active/InActive/All) Last Day Close(s) To Grid
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void rblDistributorTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetLastClosedDay(int.Parse(this.Session["UserId"].ToString()), int.Parse(this.Session["Distributor_Id"].ToString()), Convert.ToInt32(rblDistributorTypes.SelectedValue));
        if (Grid_Hierarchy.Rows.Count > 1)
        {
            Grid_Hierarchy.UseAccessibleHeader = true;
            Grid_Hierarchy.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    /// <summary>
    /// Performs Following Tasks And LogOuts
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// Closes Day Transactions
    /// </item>
    /// <item>
    /// Inserts LogOut Time
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="sender">object</param>
    /// <param name="e">EventArgs</param>
    protected void btnDayClose_Click(object sender, EventArgs e)
    {
        try
        {
            if (btnDayClose.Visible == true)
            {
                DataTable dtOrders = SelectPendingBills();

                if (dtOrders.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Plz Close your Orders first');", true);

                }
                else
                {
                    DistributorController mDayClose = new DistributorController();

                    bool dt = mDayClose.UspDayClose(DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), int.Parse(this.Session["DISTRIBUTOR_ID"].ToString()), int.Parse(this.Session["UserID"].ToString()));

                    if (dt == false)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Some error in day Close Contact System Administrator');", true);
                    }

                    else
                    {
                        UserController mController = new UserController();
                        if (mController.InsertUserLogoutTime(Convert.ToInt64(Session["User_Log_ID"]), Convert.ToInt32(Session["UserID"])) == "Logout Time Inserted")
                        {
                            this.Session.Clear();
                            Response.Redirect("~/Login.aspx");
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Some error in day Close Contact System Administrator');", true);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    protected void btnReverseDayClose_Click(object sender, EventArgs e)
    {
        try
        {

            if (btnReverseDayClose.Visible == true)
            {
                DistributorController mDayClose = new DistributorController();

                bool dt = mDayClose.UspDayReverse(DateTime.Parse(this.Session["CurrentWorkDate"].ToString()), int.Parse(this.Session["DISTRIBUTOR_ID"].ToString()), int.Parse(this.Session["UserID"].ToString()));

                if (dt == false)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert(' Some error in Reverse Day Close Contact System Administrator');", true);
                }

                else
                {

                    Session.Clear();
                    Response.Redirect("~/Login.aspx");
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    protected void btnSync_Click(object sender, EventArgs e)
    {

    }   
}