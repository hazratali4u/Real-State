using CORNBusinessLayer.Classes;
using CORNBusinessLayer.Logics;
using CORNBusinessLayer.Reports;
using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forms_frmAddSaleReturn : System.Web.UI.Page
{
    private static int RowNo;
    DataTable SalesReturn, PurchaseDetailUpdate;
    readonly DataControl dc = new DataControl();
    readonly LedgerController LController = new LedgerController();
    readonly BankController _bnkController = new BankController();
    IDbTransaction mTransaction;
    IDbConnection mConnection;
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.AppendHeader("pragma", "no-cache");

        if (!Page.IsPostBack)
        {
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            DATA(6);
            CreatTable();
            CreatUpdateTable();
            LoadDate();
            LoadProject();
            LoadDistributor();
            LoadCustomer();
            LoadItems();
            txtSale_Amount();
            LoadGrid("");
            
            //DoEmptyTextBox();
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            txtAmountReturnable.Attributes.Add("readonly", "readonly");
            mastergrid.Visible = true;
            Panel3.Visible = true;
        }
    }
    public void DATA(int Type)
    {
        if (Type != 7)
        {
            this.Session["dt"] = null;
            DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
            this.Session.Add("dt", dt);
        }
        else
        {
            this.Session["dtItem"] = null;
            DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
            this.Session.Add("dtItem", dt);
        }
    }
    public void Items(int Type)
    {
        this.Session["Items"] = null;
        DataTable dt = LController.SelectBSI(Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, Constants.IntNullValue, null, Type);
        this.Session.Add("Items", dt);

    }
    public void EditedItems()
    {
        DataTable dt = (DataTable)this.Session["Items"];
        if (dt.Rows.Count > 0)
        {
            var qry = dt.AsEnumerable();
            if (qry.Any())
            {
                var Items = qry.CopyToDataTable();
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ITEM_NAME";
                ddlItem.DataValueField = "ITEM_INFO_ID";
                ddlItem.DataBind();
                ddlItem.SelectedIndex = 0;
            }
            else
            {
                ddlItem.Items.Clear();
                ddlItem.Items.Add(new ListItem("", "0"));
                //ddlItem.Items.Add(new ListItem("", "0"));
                ddlItem.SelectedIndex = 0;
            }
        }
        else
        {
            ddlItem.Items.Clear();
            ddlItem.Items.Add(new ListItem("", "0"));
            ddlItem.SelectedIndex = 0;
        }
    }
    protected void LoadProject()
    {
        ddlProject.DataSource = DropDowns.PROJECTDropDown();
        ddlProject.DataTextField = "PROJECT_NAME";
        ddlProject.DataValueField = "PROJECT_ID";
        ddlProject.DataBind();
        ddlProject.SelectedIndex = 0;
    }
    protected void LoadDistributor()
    {
        DataTable dt = (DataTable)Session["dt"];
        if (dt.Rows.Count > 0)
        {
            ddlLocation.DataSource = dt.AsEnumerable().GroupBy(c => c.Field<int>("DISTRIBUTOR_ID")).Select(y => y.FirstOrDefault()).CopyToDataTable(); ;
            ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
            ddlLocation.DataValueField = "DISTRIBUTOR_ID";
            ddlLocation.DataBind();
            ddlLocation.SelectedIndex = 0;
        }
        else
        {
            ddlLocation.Items.Add(new ListItem("", "0"));
        }
        //int UserID = Convert.ToInt32(Session["UserID"]);
        //ddlLocation.DataSource = DropDowns.DISTRIBUTORDropDown().Where(x => x.USER_ID == UserID);
        //ddlLocation.DataTextField = "DISTRIBUTOR_NAME";
        //ddlLocation.DataValueField = "DISTRIBUTOR_ID";
        //ddlLocation.DataBind();
    }
    protected void LoadDate()
    {
        //DbCalls db = new DbCalls();
        //var qry = (from k in db.tbl_SALE_MASTER
        //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
        //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
        //           //join l in db.CUSTOMER_LEDGER on k.VENDOR_ID equals l.CUSTOMER_ID
        //           select new { k.SALE_MASTER_ID,k.IS_DELETED,k.SALE_AMOUNT,v.VENDOR_ID,v.VENDOR_NAME,i.ITEM_INFO_ID,i.ITEM_NAME,v.TYPE_ID,k.PROJECT_ID,k.DISCOUNT,k.TOKEN_AMOUNT,k.DOWN_PAYMENT,k.DEBIT_AMOUNT,k.BALANCE}).Where(x => x.IS_DELETED == false).ToList();
        ////decimal DebitAmount = qry.Sum(x => x.DEBIT);
        //if (qry.Any())
        //{
        //    foreach (var item in qry)
        //    {
        //        SalesReturn = (DataTable)this.Session["SalesReturn"];
        //        DataRow dr = SalesReturn.NewRow();
        //        dr["SALE_MASTER_ID"] = item.SALE_MASTER_ID;
        //        dr["VENDOR_ID"] = item.VENDOR_ID;
        //        dr["VENDOR_NAME"] = item.VENDOR_NAME;
        //        dr["ITEM_INFO_ID"] = item.ITEM_INFO_ID;
        //        dr["ITEM_NAME"] = item.ITEM_NAME;
        //        dr["PROJECT_ID"] = item.PROJECT_ID;
        //        dr["SALE_AMOUNT"] = item.SALE_AMOUNT;
        //        dr["DISCOUNT"] = item.DISCOUNT;
        //        dr["TOKEN_AMOUNT"] = item.TOKEN_AMOUNT;
        //        dr["DOWN_PAYMENT"] = item.DOWN_PAYMENT;
        //        dr["DEBIT_AMOUNT"] = item.DEBIT_AMOUNT;
        //        dr["BALANCE"] = item.BALANCE;
        //        SalesReturn.Rows.Add(dr);
        //    }
        //    this.Session.Add("SalesReturn", SalesReturn);
        //}
    }
    public void DoEmptyTextBox()
    {
        Session["SaleReturnID"] = null;
        txtAmountReturnable.Text = "";
        txtDifference.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtPurchaseDate.Attributes.Add("readonly", "readonly");
        txtAmountReturnable.Attributes.Add("readonly", "readonly");
        mastergrid.Visible = true;
        Session["SalesReturn"] = null;
        CreatTable();
        LoadDate();
        LoadItems();
        txtSale_Amount();
    }
    protected void LoadCustomer()
    {
        DataTable dt = (DataTable)Session["dt"];
        int DISTRIBUTOR_ID = int.Parse(ddlLocation.SelectedItem.Value.ToString());
        DataRow[] dr = dt.Select("DISTRIBUTOR_ID = '" + (int)DISTRIBUTOR_ID + "'");
        if (dt.Rows.Count > 0)
        {
            ddlCustomer.DataSource = dr.AsEnumerable().GroupBy(c => c.Field<int>("VENDOR_ID")).Select(y => y.FirstOrDefault()).CopyToDataTable();
            ddlCustomer.DataTextField = "VENDOR_NAME";
            ddlCustomer.DataValueField = "VENDOR_ID";
            ddlCustomer.DataBind();
            ddlCustomer.SelectedIndex = 0;
        }
        else
        {
            ddlCustomer.Items.Add(new ListItem("", "0"));
        }
    }
    protected void LoadItems()
    {
        int VENDOR_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
        DataTable dt = (DataTable)this.Session["dt"];
        if (dt.Rows.Count > 0)
        {
            var qry = dt.AsEnumerable()
                  .Where(r => r.Field<int>("VENDOR_ID") == int.Parse(VENDOR_ID.ToString()));
            if (qry.Any())
            {
                var Items = qry.CopyToDataTable();
                ddlItem.DataSource = Items;
                ddlItem.DataTextField = "ItemInfo";
                ddlItem.DataValueField = "ITEM_INFO_ID";
                ddlItem.DataBind();
                ddlItem.SelectedIndex = 0;
            }
            else
            {
                ddlItem.Items.Clear();
                ddlItem.Items.Add(new ListItem("", "0"));
                ddlItem.SelectedIndex = 0;
            }
        }
        else
        {
            ddlItem.Items.Clear();
            ddlItem.Items.Add(new ListItem("", "0"));
            ddlItem.SelectedIndex = 0;
        }
    }
    private void CreatTable()
    {
        SalesReturn = new DataTable();
        SalesReturn.Columns.Add("SALE_MASTER_ID", typeof(long));
        SalesReturn.Columns.Add("VENDOR_ID", typeof(int));
        SalesReturn.Columns.Add("VENDOR_NAME", typeof(string));
        SalesReturn.Columns.Add("ITEM_INFO_ID", typeof(int));
        SalesReturn.Columns.Add("ITEM_NAME", typeof(string));
        SalesReturn.Columns.Add("PROJECT_ID", typeof(int));
        SalesReturn.Columns.Add("SALE_AMOUNT", typeof(decimal));
        SalesReturn.Columns.Add("DISCOUNT", typeof(decimal));
        SalesReturn.Columns.Add("TOKEN_AMOUNT", typeof(decimal));
        SalesReturn.Columns.Add("DOWN_PAYMENT", typeof(decimal));
        SalesReturn.Columns.Add("DEBIT_AMOUNT", typeof(decimal));
        SalesReturn.Columns.Add("BALANCE", typeof(decimal));
        SalesReturn.Columns.Add("RefID", typeof(long));
        this.Session.Add("SalesReturn", SalesReturn);
    }
    private void CreatUpdateTable()
    {
        PurchaseDetailUpdate = new DataTable();
        PurchaseDetailUpdate.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("ITEM_INFO_NAME", typeof(string));
        PurchaseDetailUpdate.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("REF_ID", typeof(int));
        PurchaseDetailUpdate.Columns.Add("RefID", typeof(long));
        this.Session.Add("PurchaseDetailUpdate", PurchaseDetailUpdate);
    }
    protected void LoadGrid(string pType)
    {
        if (pType.Equals(""))
        {
            DbCalls db = new DbCalls();
            //var qry = (from k in db.tbl_SALE_RETURN
            //           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
            //           join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
            //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
            //           join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on i.STREET_ID equals s.STREET_ID
            //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
            //           select new {k.REF_ID, d.DISTRIBUTOR_NAME, p.PROJECT_NAME, ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME, v.VENDOR_NAME, k.SALE_RETURN_ID, k.SALE_AMOUNT, k.SALE_RETURN_AMOUNT, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
            DataTable dt = LController.SelectDataForSearching(5);
            GridSaleMaster.DataSource = dt;
            GridSaleMaster.DataBind();
            int totalSaleAmount = 0;
            int totalReturnableAmount = 0;
            foreach (GridViewRow row in GridSaleMaster.Rows)
            {
                int saleamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                totalSaleAmount += saleamount;
                int returnableamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[7].Text), 0));
                totalReturnableAmount += returnableamount;
            }
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", totalSaleAmount);
                GridSaleMaster.FooterRow.Cells[7].Text = string.Format("{0:n2}", totalReturnableAmount);
                GridSaleMaster.FooterRow.Cells[5].Text = "Total";
                GridSaleMaster.FooterRow.Cells[5].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[6].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[7].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[8].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[10].Visible = false;
                GridSaleMaster.FooterRow.Cells[11].Visible = false;
            }
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(5);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%'  OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%' OR Convert(REF_ID, System.String)  LIKE '%" + txtSearch.Text + "%'";
            //OR SALE_AMOUNT = " + txtSearch.Text + " OR SALE_RETURN_AMOUNT = " + txtSearch.Text + "
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.PageIndex = 0;
            }
            GridSaleMaster.DataSource = dt;
            GridSaleMaster.DataBind();
            int totalSaleAmount = 0;
            int totalReturnableAmount = 0;
            foreach (GridViewRow row in GridSaleMaster.Rows)
            {
                int saleamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                totalSaleAmount += saleamount;
                int returnableamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[7].Text), 0));
                totalReturnableAmount += returnableamount;
            }
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", totalSaleAmount);
                GridSaleMaster.FooterRow.Cells[7].Text = string.Format("{0:n2}", totalReturnableAmount);
                GridSaleMaster.FooterRow.Cells[5].Text = "Total";
                GridSaleMaster.FooterRow.Cells[5].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[6].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[7].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[8].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[10].Visible = false;
                GridSaleMaster.FooterRow.Cells[11].Visible = false;
            }
        }
        
    }
    private void txtSale_Amount() {
        DataRow[] dr = null;
        DataTable dt = (DataTable)Session["dt"];
        if (dt.Rows.Count > 0)
        {
            dr = dt.Select("ITEM_INFO_ID =" + Convert.ToInt64(ddlItem.SelectedItem.Value) + "");
            if (dr.Any())
            {
                double SALE_AMOUNT = Convert.ToDouble(dr[0][14].ToString());
                lblSaleAmount.Text = string.Format("{0:N}", SALE_AMOUNT);

                decimal DISCOUNT = Convert.ToDecimal(dr[0][15].ToString());
                lblDiscount.Text = string.Format("{0:N}", DISCOUNT);

                decimal LESS_DISCOUNT = Convert.ToDecimal(dr[0][24].ToString());
                lblLessDiscount.Text = string.Format("{0:N}", LESS_DISCOUNT);

                double NETAMOUNT = Convert.ToDouble(SALE_AMOUNT + Convert.ToDouble(DISCOUNT) - Convert.ToDouble(LESS_DISCOUNT));
                lblNetAmount.Text = string.Format("{0:N}", NETAMOUNT);

                decimal TOKENAMOUNT = Convert.ToDecimal(dr[0][16].ToString());
                lblTokenAmount.Text = string.Format("{0:N}", TOKENAMOUNT);

                decimal DOWNPAYMENT = Convert.ToDecimal(dr[0][17].ToString());
                lblDownPayment.Text = string.Format("{0:N}", DOWNPAYMENT);

                decimal Payments = Convert.ToDecimal(dr[0][19].ToString()) - Convert.ToDecimal(dr[0][18].ToString());

                lblAmountReceived.Text = string.Format("{0:N}", Payments);

                decimal TotalPaidAmount = Payments + TOKENAMOUNT + DOWNPAYMENT;
                lblTotalPaidAmount.Text = string.Format("{0:N}", TotalPaidAmount);

                decimal BALANCE = Convert.ToDecimal(dr[0][18].ToString());
                lblBalance.Text = string.Format("{0:N}", BALANCE);

                txtAmountReturnable.Text = string.Format("{0:N}", NETAMOUNT);
                hfRefID.Value = dr[0][22].ToString();
            }
            else
            {
                lblSaleAmount.Text = string.Format("{0:N}", 0.00);
                lblDiscount.Text = string.Format("{0:N}", 0.00);
                lblLessDiscount.Text = string.Format("{0:N}", 0.00);
                lblNetAmount.Text = string.Format("{0:N}", 0.00);
                lblTokenAmount.Text = string.Format("{0:N}", 0.00);
                lblDownPayment.Text = string.Format("{0:N}", 0.00);
                lblAmountReceived.Text = string.Format("{0:N}", 0.00);
                lblTotalPaidAmount.Text = string.Format("{0:N}", 0.00);
                lblBalance.Text = string.Format("{0:N}", 0.00);
                txtAmountReturnable.Text = "";
                ddlItem.Items.Add(new ListItem("", "0"));
            }
            
        }
        else
        {
            lblSaleAmount.Text = string.Format("{0:N}", 0.00);
            lblDiscount.Text = string.Format("{0:N}", 0.00);
            lblLessDiscount.Text = string.Format("{0:N}", 0.00);
            lblNetAmount.Text = string.Format("{0:N}", 0.00);
            lblTokenAmount.Text = string.Format("{0:N}", 0.00);
            lblDownPayment.Text = string.Format("{0:N}", 0.00);
            lblAmountReceived.Text = string.Format("{0:N}", 0.00);
            lblTotalPaidAmount.Text = string.Format("{0:N}", 0.00);
            txtAmountReturnable.Text = "";
            lblBalance.Text = string.Format("{0:N}", 0.00);
        }
        
    }
    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Panelgrid.Visible = false;
        PanelSale.Visible = true;
        Panel3.Visible = false;
        DATA(6);
        LoadCustomer();
        LoadItems();
        txtSale_Amount();
    }

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlCustomer.SelectedItem.Value.Equals("") || ddlCustomer.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Customer is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCustomer);
            return;
        }
        if (ddlItem.SelectedItem.Value.Equals("") || ddlItem.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        if (txtAmountReturnable.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Returnable amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtAmountReturnable);
            return;
        }
        decimal RBAmount = Convert.ToDecimal(txtAmountReturnable.Text);
        decimal BAmount = Convert.ToDecimal(lblNetAmount.Text);
        if (RBAmount > BAmount)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Returnable Amount should be less then or equal to Net Amount.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtAmountReturnable);
            return;
        }

        decimal Consumption;
        string ItemInfo = "";
        int Item_ID;
        DataTable dtt = (DataTable)this.Session["dt"];
        DataRow[] foundRows = dtt.Select("ITEM_INFO_ID  = " + int.Parse(ddlItem.SelectedItem.Value.ToString()));
        if (foundRows.Length > 0)
        {
            Consumption = Convert.ToDecimal(foundRows[0]["Consumption"]);
            ItemInfo = Convert.ToString(foundRows[0]["ItemInfo"]);
            Item_ID = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"]);
        }
        else
        {
            Consumption = 0;
            ItemInfo = "";
            Item_ID = 0;
        }
        int VENDOR_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
        int ITEM_INFO_ID = Item_ID; //Convert.ToInt32(ddlItem.SelectedValue);
        DbCalls db = new DbCalls();
       
        DataRow[] dr = null;
        DataTable dt = (DataTable)Session["dt"];
        dr = dt.Select("ITEM_INFO_ID =" + Convert.ToInt64(ddlItem.SelectedItem.Value) + "");
        if (dr.Length > 0)
        {
            int vendorid = Convert.ToInt32(dr[0][12].ToString());
            int itemid = Convert.ToInt32(dr[0][7].ToString());
            int projectid = Convert.ToInt32(dr[0][1].ToString());
            int SALEID = Convert.ToInt32(dr[0][20].ToString());
            int REFID = Convert.ToInt32(dr[0][21].ToString());
            int PEV_SALE_MASTER_ID = Convert.ToInt32(Session["PEV_SALE_MASTER_ID"]);

            SaleReturnLogics srl = new SaleReturnLogics();
            tbl_SALE_RETURN sr = new tbl_SALE_RETURN();
            sr.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
            sr.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
            sr.PROJECT_ID = projectid;
            sr.VENDOR_ID = vendorid;
            sr.ITEM_INFO_ID = itemid;
            sr.SALE_AMOUNT = Convert.ToDecimal(lblSaleAmount.Text);
            sr.SALE_RETURN_AMOUNT = Convert.ToDecimal(txtAmountReturnable.Text);
            
            decimal Diff = Convert.ToDecimal(sr.SALE_AMOUNT - sr.SALE_RETURN_AMOUNT);
            sr.DIFFERENCE = Diff;
            sr.DISCOUNT = Convert.ToDecimal(lblDiscount.Text);
            sr.LESS_DISCOUNT = Convert.ToDecimal(lblLessDiscount.Text);
            sr.TOKENAMOUNT = Convert.ToDecimal(lblTokenAmount.Text);
            sr.DOWNPAYMENT = Convert.ToDecimal(lblDownPayment.Text);
            sr.RECEIVED = Convert.ToDecimal(lblAmountReceived.Text);
            sr.BALANCE = Convert.ToDecimal(lblBalance.Text);
            sr.REMARKS = txtRemarks.Text;
            //sr.SR_NO = txtSR_NO.Text;
            sr.USER_ID = Convert.ToInt32(Session["UserID"]);
            sr.REF_ID = REFID;
            //sr.SALE_REF_ID = SALEID;
            sr.TIME_STEMP = DateTime.Now;
            if (Session["SaleReturnID"] != null)
            {

                sr.SALE_RETURN_ID = Convert.ToInt32(Session["SaleReturnID"]);
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("DELETE tbl_ITEM_LOG where TransactionID=" + sr.SALE_RETURN_ID + "AND TypeID = 14", con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }
                DataTable dttt = (DataTable)Session["PurchaseDetailUpdate"];
                if (dttt.Rows.Count > 0)
                {
                    foreach (DataRow drr in dttt.Rows)
                    {
                        using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 1 where ITEM_LOG_ID=" + long.Parse(drr["RefID"].ToString()), con))
                            {
                                command2.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                }

                //int Def = Convert.ToInt32(Session["SR_Amount"]) - Convert.ToInt32(Session["DBT_Amount"]);
                //if (Convert.ToDecimal(Diff) > Convert.ToDecimal(Session["SR_Amount"]))
                //{
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtAmountReturnable.Text) - Convert.ToDecimal(Def));
                //}
                //else if (Convert.ToDecimal(Diff) < Convert.ToDecimal(Session["SR_Amount"]))
                //{
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtAmountReturnable.Text) - Convert.ToDecimal(Def));
                //}
                //else
                //{
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(txtAmountReturnable.Text);
                //}
                sr.CREDIT_AMOUNT = Convert.ToDecimal(lblAmountReceived.Text);
                if (Convert.ToDecimal(lblAmountReceived.Text) > 0)
                {
                    sr.RETURN_CLOSED = false;
                }
                else
                {
                    sr.RETURN_CLOSED = true;
                }
                sr.IS_DELETED = false;
                srl.Update(sr);

                

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ITEM_LOG_ID=" + hfRefID.Value, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }

                Item_Log_Logics Log = new Item_Log_Logics();
                tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                ILog.TransactionID = sr.SALE_RETURN_ID;
                ILog.TransactionDetailID = sr.SALE_RETURN_ID;
                ILog.TypeID = 14;
                ILog.RefID = long.Parse(hfRefID.Value);
                ILog.ProjectID = projectid;
                ILog.CategoryID = 0;
                ILog.SubCategoryID = 0;
                ILog.ItemID = ITEM_INFO_ID;
                ILog.ItemAmount = Convert.ToDecimal(txtAmountReturnable.Text);
                ILog.UserID = Convert.ToInt32(Session["UserID"]);
                ILog.IsActive = Convert.ToBoolean(1);
                ILog.TimeStamp = DateTime.Now;
                Log.Save(ILog);

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_SALE_RETURN SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where SALE_RETURN_ID=" + sr.SALE_RETURN_ID, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command1 = new SqlCommand("UPDATE tbl_SALE_DETAIL set SALE_CLOSED = 1 where SALE_DETAIL_ID =" + SALEID + "", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    if (SALEID != PEV_SALE_MASTER_ID)
                    {
                        using (SqlCommand command1 = new SqlCommand("UPDATE tbl_SALE_DETAIL set SALE_CLOSED = 0 where SALE_DETAIL_ID =" + PEV_SALE_MASTER_ID + "", con))
                        {
                            command1.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                }
                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == sr.SALE_RETURN_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleReturn && x.IS_DELETED == 0).ToList();
                if (q.Count > 0)
                {
                    string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                    int DISTRIBUTOR_ID = Convert.ToInt32(q.FirstOrDefault().DISTRIBUTOR_ID);
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and VOUCHER_NO='" + VOUCHER_NO + "' and DOCUMENT_TYPE_ID=" + Constants.Document_SaleReturn + "and DOCUMENT_NO=" + Convert.ToInt64(sr.SALE_RETURN_ID) + "and PAYMENT_MODE=" + Constants.CreditSale + "and PAYEE_NAME='" + "Sale Return" + "'", con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + DISTRIBUTOR_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                LedgerEntries(sr.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(sr.SALE_RETURN_AMOUNT)
                    , sr.DATE, sr.VENDOR_ID, Constants.Document_SaleReturn
                    , "", Convert.ToInt32(Session["UserID"]), sr.SALE_RETURN_ID
                    , Convert.ToString(sr.SALE_RETURN_ID)
                    , Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
                Show_Voucher(hfVoucher_No.Value);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
                Session["SaleReturnID"] = null;
                Session["SR_Amount"] = null;
                Session["DBT_Amount"] = null;
                Session["PEV_SALE_MASTER_ID"] = null;
            }
            else
            {
                var qq = (from k in db.tbl_SALE_RETURN select k).Where(x => x.VENDOR_ID == VENDOR_ID && x.ITEM_INFO_ID == ITEM_INFO_ID && x.IS_DELETED == false && x.CREDIT_AMOUNT > 0).ToList();
                if (qq.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Sale Return already added against " + ddlCustomer.SelectedItem.Text + " for " + ddlItem.SelectedItem.Text + ".');", true);
                    ScriptManager.GetCurrent(Page).SetFocus(ddlCustomer);
                    return;
                }
                //sr.CREDIT_AMOUNT= Convert.ToDecimal(txtAmountReturnable.Text);
                sr.CREDIT_AMOUNT = Convert.ToDecimal(lblAmountReceived.Text);
                if (Convert.ToDecimal(lblAmountReceived.Text) > 0)
                {
                    sr.RETURN_CLOSED = false;
                }
                else
                {
                    sr.RETURN_CLOSED = true;
                }
                sr.IS_DELETED = false;
                srl.Save(sr);

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_ITEM_LOG SET IsActive = 0 where ITEM_LOG_ID=" + hfRefID.Value, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }

                Item_Log_Logics Log = new Item_Log_Logics();
                tbl_ITEM_LOG ILog = new tbl_ITEM_LOG();
                ILog.Date = Convert.ToDateTime(txtPurchaseDate.Text);
                ILog.LocationID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
                ILog.TransactionID = sr.SALE_RETURN_ID;
                ILog.TransactionDetailID = sr.SALE_RETURN_ID;
                ILog.TypeID = 14;
                ILog.RefID = long.Parse(hfRefID.Value);
                ILog.ProjectID = projectid;
                ILog.CategoryID = 0;
                ILog.SubCategoryID = 0;
                ILog.ItemID = ITEM_INFO_ID;
                ILog.ItemAmount = Convert.ToDecimal(txtAmountReturnable.Text);
                ILog.UserID = Convert.ToInt32(Session["UserID"]);
                ILog.IsActive = Convert.ToBoolean(1);
                ILog.TimeStamp = DateTime.Now;
                Log.Save(ILog);

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_SALE_RETURN SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where SALE_RETURN_ID=" + sr.SALE_RETURN_ID, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command1 = new SqlCommand("UPDATE tbl_SALE_DETAIL set SALE_CLOSED = 1 where SALE_DETAIL_ID =" + SALEID + "", con))
                    {
                        
                        command1.ExecuteNonQuery();
                    }
                    con.Close();
                }
                LedgerEntries(sr.DISTRIBUTOR_ID, Consumption, Convert.ToDecimal(sr.SALE_RETURN_AMOUNT)
                    , sr.DATE, sr.VENDOR_ID, Constants.Document_SaleReturn
                    , "", Convert.ToInt32(Session["UserID"]), sr.SALE_RETURN_ID
                    , Convert.ToString(sr.SALE_RETURN_ID)
                    , Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
                Show_Voucher(hfVoucher_No.Value);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
            LoadGrid("");
            DoEmptyTextBox();
            mastergrid.Visible = true;
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            Panel3.Visible = true;
            DATA(6);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        DoEmptyTextBox();
        Panel3.Visible = true;
        DATA(6);
        LoadCustomer();
        LoadItems();
        txtSale_Amount();
    }

    protected void GridSaleMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridSaleMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }

    protected void txtSaleAmount_TextChanged(object sender, EventArgs e)
    {

    }

    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["SaleReturnID"] = null;
        Session["SalesReturn"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        SaleReturnLogics Ser = new SaleReturnLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_RETURN_ID && x.DOCUMENT_TYPE_ID == Constants.Document_SaleReturn && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected sale return could not update. Some payments are issued against this sale return.');", true);
        }
        else
        {
            //Items(3);
            DataTable dt = new DataTable();
            DataTable dtItem = new DataTable();
            DATA(6);
            DATA(7);
            dtItem = (DataTable)this.Session["dtItem"];
            DataRow[] foundRows = dtItem.Select("ITEM_INFO_ID  = '" + data.ITEM_INFO_ID + "'");
            if (foundRows.Length > 0)
            {
                Session["PEV_SALE_MASTER_ID"] = null;
                dt = (DataTable)this.Session["dt"];
                DataRow dr = dt.NewRow();
                dr["DISTRIBUTOR_ID"] = Convert.ToInt32(foundRows[0]["DISTRIBUTOR_ID"].ToString());
                dr["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
                dr["PROJECT_NAME"] = foundRows[0]["PROJECT_NAME"].ToString();
                dr["BLOCK_ID"] = Convert.ToInt32(foundRows[0]["BLOCK_ID"].ToString());
                dr["BLOCK_NAME"] = foundRows[0]["BLOCK_NAME"].ToString();
                dr["STREET_ID"] = Convert.ToInt32(foundRows[0]["STREET_ID"].ToString());
                dr["STREET_NAME"] = foundRows[0]["STREET_NAME"].ToString();
                dr["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
                dr["ITEM_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
                dr["Consumption"] = foundRows[0]["Consumption"].ToString();
                dr["DATE"] = Convert.ToDateTime(foundRows[0]["DATE"].ToString());
                dr["ItemInfo"] = foundRows[0]["ItemInfo"].ToString();
                dr["VENDOR_ID"] = Convert.ToInt32(foundRows[0]["VENDOR_ID"].ToString());
                dr["VENDOR_NAME"] = foundRows[0]["VENDOR_NAME"].ToString();
                dr["SALE_AMOUNT"] = foundRows[0]["SALE_AMOUNT"].ToString();
                dr["DISCOUNT"] = foundRows[0]["DISCOUNT"].ToString();
                dr["TOKEN_AMOUNT"] = foundRows[0]["TOKEN_AMOUNT"].ToString();
                dr["DOWN_PAYMENT"] = foundRows[0]["DOWN_PAYMENT"].ToString();
                dr["DEBIT_AMOUNT"] = foundRows[0]["DEBIT_AMOUNT"].ToString();
                dr["BALANCE"] = foundRows[0]["BALANCE"].ToString();
                dr["SALE_MASTER_ID"] = Convert.ToInt32(foundRows[0]["SALE_MASTER_ID"].ToString());
                dr["REF_ID"] = Convert.ToInt32(foundRows[0]["REF_ID"].ToString());
                dr["RefID"] = Convert.ToInt32(foundRows[0]["RefID"].ToString());
                dt.Rows.Add(dr);
                Session["PEV_SALE_MASTER_ID"] = foundRows[0]["SALE_MASTER_ID"].ToString();
                Session["dt"] = dt;

                PurchaseDetailUpdate = (DataTable)this.Session["PurchaseDetailUpdate"];
                DataRow drUp = PurchaseDetailUpdate.NewRow();
                drUp["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
                drUp["ITEM_INFO_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
                drUp["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
                drUp["REF_ID"] = Convert.ToInt32(foundRows[0]["SALE_MASTER_ID"].ToString());
                drUp["RefID"] = foundRows[0]["RefID"].ToString();
                PurchaseDetailUpdate.Rows.Add(drUp);

                hfRefID.Value = foundRows[0]["RefID"].ToString();
            }
            CreatTable();
            LoadDate();
            LoadCustomer();
            ddlCustomer.SelectedValue = Convert.ToString(data.VENDOR_ID);
            LoadItems();
            
            ddlItem.SelectedValue = Convert.ToString(data.ITEM_INFO_ID);
            txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            decimal saleamount = decimal.Parse(data.SALE_AMOUNT.ToString());
            lblSaleAmount.Text = string.Format("{0:N}", saleamount);
            LoadDate();
            decimal R_Amount = decimal.Parse(data.SALE_RETURN_AMOUNT.ToString());
            txtAmountReturnable.Text = string.Format("{0:N}", R_Amount);

            decimal DISCOUNT = decimal.Parse(data.DISCOUNT.ToString());
            decimal Discount = Convert.ToDecimal(Math.Round(DISCOUNT, 2));
            lblDiscount.Text = string.Format("{0:N}", DISCOUNT);

            decimal LESS_DISCOUNT = decimal.Parse(data.LESS_DISCOUNT.ToString());
            decimal Less_Discount = Convert.ToDecimal(Math.Round(LESS_DISCOUNT, 2));
            lblLessDiscount.Text = string.Format("{0:N}", LESS_DISCOUNT);

            decimal NETAMOUNT = Convert.ToDecimal(data.SALE_AMOUNT + data.DISCOUNT - data.LESS_DISCOUNT);
            lblNetAmount.Text = string.Format("{0:N}", NETAMOUNT);

            decimal TOKENAMOUNT = decimal.Parse(data.TOKENAMOUNT.ToString());
            lblTokenAmount.Text = string.Format("{0:N}", TOKENAMOUNT);

            decimal DOWNPAYMENT = decimal.Parse(data.DOWNPAYMENT.ToString());
            lblDownPayment.Text = string.Format("{0:N}", DOWNPAYMENT);

            decimal RECEIVED = decimal.Parse(data.RECEIVED.ToString());
            lblAmountReceived.Text = string.Format("{0:N}", RECEIVED);

            decimal TotalPaidAmount = RECEIVED + TOKENAMOUNT + DOWNPAYMENT;
            lblTotalPaidAmount.Text = string.Format("{0:N}", TotalPaidAmount);

            decimal BALANCE = decimal.Parse(data.BALANCE.ToString());
            lblBalance.Text = string.Format("{0:N}", BALANCE);
            //txtSR_NO.Text = data.SR_NO;
            txtRemarks.Text = data.REMARKS;
            Session.Add("SR_Amount", data.SALE_RETURN_AMOUNT);
            Session.Add("DBT_Amount", data.CREDIT_AMOUNT);
            Session.Add("SaleReturnID", data.SALE_RETURN_ID);
            mastergrid.Visible = false;
            Panelgrid.Visible = false;
            PanelSale.Visible = true;
            btnSave.Text = "Update";
            Panel3.Visible = false;
        }
        
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool flag = false;
        var ProID = ((LinkButton)sender).CommandArgument;
        DbCalls db = new DbCalls();
        SaleReturnLogics Ser = new SaleReturnLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        //int SALE_REF_ID = Convert.ToInt32(data.SALE_REF_ID);
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_RETURN_ID && x.DOCUMENT_TYPE_ID == Constants.Document_SaleReturn && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected sale return could not be deleted. Some payments are issued against this sale return.');", true);
        }
        else
        {
            try
            {
                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.SALE_RETURN_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_SaleReturn && x.IS_DELETED == 0).ToList();
                if (!Active)
                {
                    //using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    //{
                    //    con.Open();
                    //    using (SqlCommand command1 = new SqlCommand("UPDATE tbl_SALE_MASTER set SALE_CLOSED = 0, TYPE_ID = 13 where SALE_MASTER_ID =" + SALE_REF_ID + "", con))
                    //    {
                    //        command1.ExecuteNonQuery();
                    //    }
                    //    using (SqlCommand command = new SqlCommand("DELETE tbl_SALE_RETURN where SALE_RETURN_ID=" + Convert.ToInt32(ProID), con))
                    //    {
                    //        command.ExecuteNonQuery();
                    //    }
                    //    if (q.Count > 0)
                    //    {
                    //        string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                    //        using (SqlCommand command3 = new SqlCommand("UPDATE CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and CUSTOMER_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.SALE_RETURN_ID) + "and DOCUMENT_TYPE_ID=" + Constants.Document_SaleReturn + "and PAYMENT_MODE=" + Constants.CreditSale, con))
                    //        {
                    //            command3.ExecuteNonQuery();
                    //        }
                    //        using (SqlCommand command1 = new SqlCommand("UPDATE GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                    //        {
                    //            command1.ExecuteNonQuery();
                    //        }
                    //        using (SqlCommand command2 = new SqlCommand("UPDATE GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                    //        {
                    //            command2.ExecuteNonQuery();
                    //        }
                    //    }
                    //    con.Close();
                    //}
                    //flag = true;
                    //if (flag)
                    //{
                    //    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    //}
                }
            }
            catch (Exception ex) { }
        }
        LoadGrid("");
    }

    protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems();
        txtSale_Amount();
        ddlItem_SelectedIndexChanged(null, null);
    }

    private void LedgerEntries(int p_Distributor_Id
     , decimal p_Debit, decimal p_Credit, DateTime p_Ledger_Date, int p_PRINCIPAL_ID
     , int p_DOCUMENT_TYPE_ID, string pChequeNo, int p_UserId, long p_DOCUMENT_NO
     , string p_MANUAL_DOCUMENT_NO, decimal p_TokenAmount, decimal p_DownPayment, string p_Remarks)
    {

        try
        {

            mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
            mConnection.Open();
            mTransaction = ProviderFactory.GetTransaction(mConnection);

            #region Vendor Ledger

            LedgerController LController = new LedgerController();
            // Configuration.GetAccountHead();

            string VoucherNo = LController.SelectLedgerMaxDocumentId(Constants.Journal_Voucher, p_Distributor_Id, 1);

            #endregion

            #region GL Master, Detail

            string VoucherNo2 = LController.SelectMaxVoucherId(Constants.Journal_Voucher, p_Distributor_Id, p_Ledger_Date);

            bool IsFinanceSetting = GetFinanceConfig();

            if (p_DOCUMENT_TYPE_ID == Constants.Document_SaleReturn)
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales_Return, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Sale Return-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return", 0, Constants.Document_SaleReturn);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, p_Credit, 0,  p_Ledger_Date, "Sale Return-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return", 0, Constants.Document_SaleReturn);

                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_TokenAmount, 0, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return");
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_TokenAmount, p_Ledger_Date, "Token Amount-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return");

                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PurchaseAccount, p_Distributor_Id, p_DownPayment, 0, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return");
                //LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, PayableAccount, p_Distributor_Id, 0, p_DownPayment, p_Ledger_Date, "Down Paymnet-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Sale Return");
                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_SaleReturn
                        , p_MANUAL_DOCUMENT_NO, "Sale Return Voucher / " + ddlCustomer.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Sale Return", mTransaction, mConnection);
                    //decimal Sale_Return = p_Credit - p_di
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Sales_Return, p_Credit, 0, "Sale Return Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Receivables, 0, p_Credit, "Sale Return Voucher", mTransaction, mConnection);

                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Purchases, 0, p_Debit, "Sale Return (Consumptions) Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Debit, 0, "Sale Return (Consumptions) Voucher", mTransaction, mConnection);
                    hfVoucher_No.Value = VoucherNo2;
                }
            }
            #endregion
            mTransaction.Commit();
            //return mPurchaseMaster.PURCHASE_MASTER_ID;
        }
        catch (Exception exp)
        {
            ExceptionPublisher.PublishException(exp);
            mTransaction.Rollback();
            //return Constants.LongNullValue;
        }
        finally
        {
            if (mConnection != null && mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
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

    private bool GetFinanceConfig()
    {
        try
        {
            ConfigurationController _cController = new ConfigurationController();
            DataTable dt = _cController.SelectAppConfiguration(1, (int)Enums.AppSettingMaster.FinanceSetting, null, Constants.IntNullValue);

            if (dt.Rows.Count > 0)
            {
                DataRow[] dr = dt.Select("CODE = '" + (int)Enums.AppSetting.IsFinanceIntegrate + "'");
                if (dr.Length > 0)
                {
                    return Convert.ToInt32(dr[0][2]) == 1 ? true : false;
                }
            }
            return false;
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Error in Financial Setting!');", true);
            throw;
        }
    }

    protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtSale_Amount();
        Panelgrid.Visible = false;
        PanelSale.Visible = true;
        Panel3.Visible = false;
    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        LoadGrid(txtSearch.Text);
    }
    public void Show_Voucher(string Voucher_No)
    {
        DocumentPrintController DPrint = new DocumentPrintController();
        RptAccountController RptAccountCtl = new RptAccountController();
        crpVoucherView CrpReport = new crpVoucherView();
        DataSet ds = null;
        DataTable dt = DPrint.SelectReportTitle(int.Parse(ddlLocation.SelectedItem.Value.ToString()));
        ds = RptAccountCtl.SelectUnpostVoucherForPrint(int.Parse(ddlLocation.SelectedItem.Value.ToString()), Voucher_No, 16);
        CrpReport.SetDataSource(ds);
        CrpReport.Refresh();
        CrpReport.SetParameterValue("Company_Name", dt.Rows[0]["COMPANY_NAME"].ToString());
        CrpReport.SetParameterValue("DISTRIBUTOR_NAME", dt.Rows[0]["DISTRIBUTOR_NAME"].ToString());
        CrpReport.SetParameterValue("VoucherType", "Journal Voucher");
        CrpReport.SetParameterValue("VoucherSubType", "Journal Voucher");
        Session.Add("CrpReport", CrpReport);
        Session.Add("ReportType", 0);
        const string url = "'Default.aspx'";
        const string script = "<script language='JavaScript' type='text/javascript'> window.open(" + url + ",\"Link\",\"toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600,left=10,top=10\");</script>";
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
        cs.RegisterStartupScript(cstype, "OpenWindow", script);
    }

    protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCustomer();
        LoadItems();
        txtSale_Amount();
        ddlItem_SelectedIndexChanged(null, null);
    }
}