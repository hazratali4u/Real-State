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

public partial class Forms_frmAddPurchaseReturn : System.Web.UI.Page
{

    private static int RowNo;
    DataTable PurchaseReturn, PurchaseDetailUpdate;
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
            Items(3);
            CreatTable();
            CreatUpdateTable();
            LoadDate();
            LoadProject();
            LoadDistributor();
            LoadCustomer();
            LoadItems("");
            txtSale_Amount();
            LoadGrid("");
            btnCancel.Attributes.Add("onclick", "return ValidateForm();");
            //DoEmptyTextBox();
            txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            txtPurchaseDate.Attributes.Add("readonly", "readonly");
            txtAmountReceiveable.Attributes.Add("readonly", "readonly");
            mastergrid.Visible = true;

            //Panelgrid.Visible = false;
            //PanelSale.Visible = true;
            //Panel3.Visible = false;

        }
        Panel3.Visible = true;
    }
    public void Items(int Type)
    {
        if (Type != 4)
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
        //var qry = (from k in db.tbl_PURCHASE_MASTER
        //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
        //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
        //           select new { k.PURCHASE_MASTER_ID, k.IS_DELETED, k.PURCHASE_AMOUNT, v.VENDOR_ID, v.VENDOR_NAME, i.ITEM_INFO_ID, i.ITEM_NAME, v.TYPE_ID, k.PROJECT_ID, k.DISCOUNT, k.TOKEN_AMOUNT, k.DOWN_PAYMENT, k.BALANCE, k.DEBIT_AMOUNT }).Where(x => x.IS_DELETED == false).ToList();
        //if (qry.Any())
        //{
        //    foreach (var item in qry)
        //    {
        //        PurchaseReturn = (DataTable)this.Session["PurchaseReturn"];
        //        DataRow dr = PurchaseReturn.NewRow();
        //        dr["PURCHASE_MASTER_ID"] = item.PURCHASE_MASTER_ID;
        //        dr["VENDOR_ID"] = item.VENDOR_ID;
        //        dr["VENDOR_NAME"] = item.VENDOR_NAME;
        //        dr["ITEM_INFO_ID"] = item.ITEM_INFO_ID;
        //        dr["ITEM_NAME"] = item.ITEM_NAME;
        //        dr["PROJECT_ID"] = item.PROJECT_ID;
        //        dr["PURCHASE_AMOUNT"] = item.PURCHASE_AMOUNT;
        //        dr["DISCOUNT"] = item.DISCOUNT;
        //        dr["TOKEN_AMOUNT"] = item.TOKEN_AMOUNT;
        //        dr["DOWN_PAYMENT"] = item.DOWN_PAYMENT;
        //        dr["DEBIT_AMOUNT"] = item.DEBIT_AMOUNT;
        //        dr["BALANCE"] = item.BALANCE;
        //        PurchaseReturn.Rows.Add(dr);
        //    }
        //    this.Session.Add("PurchaseReturn", PurchaseReturn);
        //}
    }
    public void DoEmptyTextBox()
    {
        Session["PurchaseReturnID"] = null;
        Session["PEV_PURCHASE_MASTER_ID"] = null;
        txtAmountReceiveable.Text = "";
        txtDifference.Text = "";
        txtRemarks.Text = "";
        btnSave.Text = "Save";
        txtPurchaseDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
        txtPurchaseDate.Attributes.Add("readonly", "readonly");
        txtAmountReceiveable.Attributes.Add("readonly", "readonly");
        mastergrid.Visible = true;
        Session["PurchaseReturn"] = null;
        CreatTable();
        Items(3);
        LoadDate();
        LoadItems("");
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
    protected void LoadItems(string Val)
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
    public void EdItems()
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
        PurchaseReturn = new DataTable();
        PurchaseReturn.Columns.Add("PURCHASE_MASTER_ID", typeof(long));
        PurchaseReturn.Columns.Add("VENDOR_ID", typeof(int));
        PurchaseReturn.Columns.Add("VENDOR_NAME", typeof(string));
        PurchaseReturn.Columns.Add("ITEM_INFO_ID", typeof(int));
        PurchaseReturn.Columns.Add("ITEM_NAME", typeof(string));
        PurchaseReturn.Columns.Add("PROJECT_ID", typeof(int));
        PurchaseReturn.Columns.Add("PURCHASE_AMOUNT", typeof(decimal));
        PurchaseReturn.Columns.Add("DISCOUNT", typeof(decimal));
        PurchaseReturn.Columns.Add("TOKEN_AMOUNT", typeof(decimal));
        PurchaseReturn.Columns.Add("DOWN_PAYMENT", typeof(decimal));
        PurchaseReturn.Columns.Add("DEBIT_AMOUNT", typeof(decimal));
        PurchaseReturn.Columns.Add("BALANCE", typeof(decimal));
        PurchaseReturn.Columns.Add("RefID", typeof(long));
        this.Session.Add("PurchaseReturn", PurchaseReturn);
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
            //DbCalls db = new DbCalls();
            //var qry = (from k in db.tbl_PURCHASE_RETURN
            //           join d in db.DISTRIBUTORs on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
            //           join p in db.PROJECTs on k.PROJECT_ID equals p.PROJECT_ID
            //           join i in db.ITEM_INFORMATION on k.ITEM_INFO_ID equals i.ITEM_INFO_ID
            //           join b in db.BLOCK on i.BLOCK_ID equals b.BLOCK_ID
            //           join s in db.STREET on i.STREET_ID equals s.STREET_ID
            //           join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
            //           select new { d.DISTRIBUTOR_NAME, p.PROJECT_NAME, ITEM_NAME = i.ITEM_NAME + " , " + b.BLOCK_NAME + ", " + s.STREET_NAME, v.VENDOR_NAME, k.PURCHASE_RETURN_ID, k.PURCHASE_AMOUNT, k.PURCHASE_RETURN_AMOUNT, Active = k.IS_DELETED == false ? "Active" : "Inactive" }).Where(x => x.Active == "Active").ToList();
            DataTable dt = LController.SelectDataForSearching(2);
            GridSaleMaster.DataSource = dt;
            GridSaleMaster.DataBind();
            int totalSaleAmount = 0;
            int totalReturnableAmount = 0;
            foreach (GridViewRow row in GridSaleMaster.Rows)
            {
                int saleamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                totalSaleAmount += saleamount;
                int returnableamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                totalReturnableAmount += returnableamount;
            }
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", totalSaleAmount);
                GridSaleMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", totalReturnableAmount);
                GridSaleMaster.FooterRow.Cells[4].Text = "Total";
                GridSaleMaster.FooterRow.Cells[4].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[5].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[6].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[9].Visible = false;
                GridSaleMaster.FooterRow.Cells[10].Visible = false;
            }
        }
        else
        {
            DataTable dt = LController.SelectDataForSearching(2);
            dt.DefaultView.RowFilter = "DISTRIBUTOR_NAME LIKE '%" + txtSearch.Text + "%'  OR PROJECT_NAME LIKE '%" + txtSearch.Text + "%' OR ITEM_NAME LIKE '%" + txtSearch.Text + "%' OR VENDOR_NAME LIKE '%" + txtSearch.Text + "%'";
            //OR PURCHASE_AMOUNT = " + txtSearch.Text + " OR PURCHASE_RETURN_AMOUNT = " + txtSearch.Text + "
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
                int saleamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[5].Text), 0));
                totalSaleAmount += saleamount;
                int returnableamount = Convert.ToInt32(Math.Round(decimal.Parse(row.Cells[6].Text), 0));
                totalReturnableAmount += returnableamount;
            }
            if (dt.Rows.Count > 0)
            {
                GridSaleMaster.FooterRow.Cells[5].Text = string.Format("{0:n2}", totalSaleAmount);
                GridSaleMaster.FooterRow.Cells[6].Text = string.Format("{0:n2}", totalReturnableAmount);
                GridSaleMaster.FooterRow.Cells[4].Text = "Total";
                GridSaleMaster.FooterRow.Cells[4].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[5].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[6].Font.Bold = true;
                GridSaleMaster.FooterRow.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                GridSaleMaster.FooterRow.Cells[9].Visible = false;
                GridSaleMaster.FooterRow.Cells[10].Visible = false;
            }
        }
        
    }
    private void txtSale_Amount()
    {
        DataRow[] dr = null;
        DataTable dt = (DataTable)Session["dt"];
        //DataTable dtT = (DataTable)this.Session["PurchaseReturn"];
        if (dt.Rows.Count > 0)
        {
            dr = dt.Select("ITEM_INFO_ID =" + Convert.ToInt64(ddlItem.SelectedItem.Value) + "");
            if (dr.Any())
            {
                decimal SALE_AMOUNT = Convert.ToDecimal(dr[0][14].ToString());
                lblPurchaseAmount.Text = string.Format("{0:N}", SALE_AMOUNT);

                decimal DISCOUNT = Convert.ToDecimal(dr[0][15].ToString());
                lblDiscount.Text = string.Format("{0:N}", DISCOUNT);

                decimal LESS_DISCOUNT = Convert.ToDecimal(dr[0][23].ToString());
                lblLessDiscount.Text = string.Format("{0:N}", LESS_DISCOUNT);

                decimal REBATE_AMOUNT = Convert.ToDecimal(dr[0][24].ToString());
                lblRebate.Text = string.Format("{0:N}", REBATE_AMOUNT);

                decimal NETAMOUNT = Convert.ToDecimal(SALE_AMOUNT + DISCOUNT - LESS_DISCOUNT - REBATE_AMOUNT);
                lblNetAmount.Text = string.Format("{0:N}", NETAMOUNT);

                decimal TOKENAMOUNT = Convert.ToDecimal(dr[0][16].ToString());
                lblTokenAmount.Text = string.Format("{0:N}", TOKENAMOUNT);

                decimal DOWNPAYMENT = Convert.ToDecimal(dr[0][17].ToString());
                lblDownPayment.Text = string.Format("{0:N}", DOWNPAYMENT);

                decimal Payments = Convert.ToDecimal(dr[0][19].ToString()) - Convert.ToDecimal(dr[0][18].ToString());

                lblAmountReceived.Text = string.Format("{0:N}", Payments);

                decimal TotalPaidAmount = Payments + TOKENAMOUNT + DOWNPAYMENT;
                lblTotalPaidAmount.Text = string.Format("{0:N}", TotalPaidAmount);

                decimal BALANCE = Convert.ToDecimal(dr[0][18].ToString()) - Convert.ToDecimal(dr[0][24].ToString());
                lblBalance.Text = string.Format("{0:N}", BALANCE);

                txtAmountReceiveable.Text = string.Format("{0:N}", NETAMOUNT);
                hfRefID.Value = dr[0][21].ToString();
            }
            else
            {
                lblPurchaseAmount.Text = string.Format("{0:N}", 0.00);
                lblDiscount.Text = string.Format("{0:N}", 0.00);
                lblLessDiscount.Text = string.Format("{0:N}", 0.00);
                lblNetAmount.Text = string.Format("{0:N}", 0.00);
                lblTokenAmount.Text = string.Format("{0:N}", 0.00);
                lblDownPayment.Text = string.Format("{0:N}", 0.00);
                lblAmountReceived.Text = string.Format("{0:N}", 0.00);
                lblTotalPaidAmount.Text = string.Format("{0:N}", 0.00);
                lblBalance.Text = string.Format("{0:N}", 0.00);
            }
            
        }
        else
        {
            lblPurchaseAmount.Text = string.Format("{0:N}", 0.00);
            lblDiscount.Text = string.Format("{0:N}", 0.00);
            lblLessDiscount.Text = string.Format("{0:N}", 0.00);
            lblNetAmount.Text = string.Format("{0:N}", 0.00);
            lblTokenAmount.Text = string.Format("{0:N}", 0.00);
            lblDownPayment.Text = string.Format("{0:N}", 0.00);
            lblAmountReceived.Text = string.Format("{0:N}", 0.00);
            lblTotalPaidAmount.Text = string.Format("{0:N}", 0.00);
            lblBalance.Text = string.Format("{0:N}", 0.00);
        }
        

    }
    protected void AddPopUp_Click(object sender, ImageClickEventArgs e)
    {
        Panelgrid.Visible = false;
        PanelSale.Visible = true;
        Panel3.Visible = false;
        Items(3);
        LoadCustomer();
        LoadItems("");
        txtSale_Amount();
    }

    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlCustomer.SelectedItem.Value.Equals("") || ddlCustomer.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Vendor is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlCustomer);
            return;
        }
        if (ddlItem.SelectedItem.Value.Equals("") || ddlItem.SelectedItem.Value.Equals("0"))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Item is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(ddlItem);
            return;
        }
        if (txtAmountReceiveable.Text.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Returnable amount is required.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtAmountReceiveable);
            return;

        }
        
        decimal RBAmount = Convert.ToDecimal(txtAmountReceiveable.Text);
        decimal BAmount = Convert.ToDecimal(lblNetAmount.Text);
        if (RBAmount > BAmount)
        {
            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Receiveable Amount should be less then or equal to Net Amount.');", true);
            ScriptManager.GetCurrent(Page).SetFocus(txtAmountReceiveable);
            return;
        }
        int VENDOR_ID = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
        int ITEM_INFO_ID = Convert.ToInt32(ddlItem.SelectedItem.Value);
        DbCalls db = new DbCalls();
        string ItemInfo = "";
        DataRow[] dr = null;
        DataTable dt = (DataTable)Session["dt"];
        dr = dt.Select("ITEM_INFO_ID =" + Convert.ToInt64(ddlItem.SelectedItem.Value) + "");
        if (dr.Length > 0)
        {
            ItemInfo = Convert.ToString(dr[0]["ItemInfo"]);
            int vendorid = Convert.ToInt32(dr[0][12].ToString());
            int itemid = Convert.ToInt32(dr[0][7].ToString());
            int projectid = Convert.ToInt32(dr[0][1].ToString());
            int PurchaseID = Convert.ToInt32(dr[0][20].ToString());
            int PEV_PURCHASE_MASTER_ID = Convert.ToInt32(Session["PEV_PURCHASE_MASTER_ID"]);

            PurchaseReturnLogics srl = new PurchaseReturnLogics();
            tbl_PURCHASE_RETURN sr = new tbl_PURCHASE_RETURN();
            sr.DATE = Convert.ToDateTime(txtPurchaseDate.Text);
            sr.DISTRIBUTOR_ID = Convert.ToInt32(ddlLocation.SelectedItem.Value);
            sr.PROJECT_ID = projectid;
            sr.VENDOR_ID = vendorid;
            sr.ITEM_INFO_ID = itemid;
            sr.PURCHASE_AMOUNT = Convert.ToDecimal(lblPurchaseAmount.Text);
            sr.PURCHASE_RETURN_AMOUNT = Convert.ToDecimal(txtAmountReceiveable.Text);
            decimal Diff = Convert.ToDecimal(sr.PURCHASE_AMOUNT - sr.PURCHASE_RETURN_AMOUNT);
            sr.DIFFERENCE = Diff;
            sr.DISCOUNT = Convert.ToDecimal(lblDiscount.Text);
            sr.LESS_DISCOUNT = Convert.ToDecimal(lblLessDiscount.Text);
            sr.REBATE_AMOUNT = Convert.ToDecimal(lblRebate.Text);
            sr.TOKENAMOUNT = Convert.ToDecimal(lblTokenAmount.Text);
            sr.DOWNPAYMENT = Convert.ToDecimal(lblDownPayment.Text);
            sr.RECEIVED = Convert.ToDecimal(lblAmountReceived.Text);
            sr.BALANCE = Convert.ToDecimal(lblBalance.Text);
            sr.REMARKS = txtRemarks.Text;
            //sr.SR_NO = txtSR_NO.Text;
            sr.USER_ID = Convert.ToInt32(Session["UserID"]);
            sr.TIME_STEMP = DateTime.Now;
            if (Session["PurchaseReturnID"] != null)
            {

                sr.PURCHASE_RETURN_ID = Convert.ToInt32(Session["PurchaseReturnID"]);
                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("DELETE tbl_ITEM_LOG where TransactionID=" + sr.PURCHASE_RETURN_ID + "AND TypeID = 3", con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }
                DataTable dtt = (DataTable)Session["PurchaseDetailUpdate"];
                if (dtt.Rows.Count > 0)
                {
                    foreach (DataRow drr in dtt.Rows)
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
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtAmountReceiveable.Text) - Convert.ToDecimal(Def));
                //}
                //else if (Convert.ToDecimal(Diff) < Convert.ToDecimal(Session["SR_Amount"]))
                //{
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(Convert.ToDecimal(txtAmountReceiveable.Text) - Convert.ToDecimal(Def));
                //}
                //else
                //{
                //    sr.CREDIT_AMOUNT = Convert.ToDecimal(txtAmountReceiveable.Text);
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
                ILog.TransactionID = sr.PURCHASE_RETURN_ID;
                ILog.TransactionDetailID = sr.PURCHASE_RETURN_ID;
                ILog.TypeID = 3;
                ILog.RefID = long.Parse(hfRefID.Value);
                ILog.ProjectID = projectid;
                ILog.CategoryID = 0;
                ILog.SubCategoryID = 0;
                ILog.ItemID = ITEM_INFO_ID;
                ILog.ItemAmount = Convert.ToDecimal(txtAmountReceiveable.Text);
                ILog.UserID = Convert.ToInt32(Session["UserID"]);
                ILog.IsActive = Convert.ToBoolean(1);
                ILog.TimeStamp = DateTime.Now;
                Log.Save(ILog);

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_PURCHASE_RETURN SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where PURCHASE_RETURN_ID=" + sr.PURCHASE_RETURN_ID, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command1 = new SqlCommand("update tbl_PURCHASE_DETAIL set PURCHASE_CLOSED = 1 where PURCHASE_DETAIL_ID =" + PurchaseID + "", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    if (PurchaseID != PEV_PURCHASE_MASTER_ID)
                    {
                        using (SqlCommand command1 = new SqlCommand("update tbl_PURCHASE_DETAIL set PURCHASE_CLOSED = 0 where PURCHASE_DETAIL_ID =" + PEV_PURCHASE_MASTER_ID + "", con))
                        {
                            command1.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                }

                var q = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.DOCUMENT_NO == sr.PURCHASE_RETURN_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase_Return && x.IS_DELETED == 0).ToList();
                if (q.Count > 0)
                {
                    string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                    int Distributor_ID = q.FirstOrDefault().DISTRIBUTOR_ID;
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("update CUSTOMER_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase_Return + "and VOUCHER_NO ='" + VOUCHER_NO + "' and DOCUMENT_NO=" + Convert.ToInt64(sr.PURCHASE_RETURN_ID) + "and PAYMENT_MODE=" + Constants.CreditSale + "and PAYEE_NAME='" + "Purchase Return" + "'", con))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + Distributor_ID, con))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "' AND DISTRIBUTOR_ID =" + Distributor_ID, con))
                        {
                            command2.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                LedgerEntries(sr.DISTRIBUTOR_ID, 0, Convert.ToDecimal(sr.PURCHASE_RETURN_AMOUNT)
                    , sr.DATE, sr.VENDOR_ID, Constants.Document_Purchase_Return
                    , "", Convert.ToInt32(Session["UserID"]), sr.PURCHASE_RETURN_ID
                    , Convert.ToString(sr.PURCHASE_RETURN_ID)
                    , Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
                //Show_Voucher(hfVoucher_No.Value);
                //ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record updated successfully.');", true);
                Session["PurchaseReturnID"] = null;
                Session["SR_Amount"] = null;
                Session["DBT_Amount"] = null;
            }
            else
            {
                var qq = (from k in db.tbl_PURCHASE_RETURN select k).Where(x => x.VENDOR_ID == VENDOR_ID && x.ITEM_INFO_ID == ITEM_INFO_ID && x.CREDIT_AMOUNT > 0).ToList();
                if (qq.Any())
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Purchase Return already added against " + ddlCustomer.SelectedItem.Text + " for " + ddlItem.SelectedItem.Text + ".');", true);
                    ScriptManager.GetCurrent(Page).SetFocus(ddlCustomer);
                    return;
                }
                //sr.CREDIT_AMOUNT = Convert.ToDecimal(txtAmountReceiveable.Text);
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
                ILog.TransactionID = sr.PURCHASE_RETURN_ID;
                ILog.TransactionDetailID = sr.PURCHASE_RETURN_ID;
                ILog.TypeID = 3;
                ILog.RefID = long.Parse(hfRefID.Value);
                ILog.ProjectID = projectid;
                ILog.CategoryID = 0;
                ILog.SubCategoryID = 0;
                ILog.ItemID = ITEM_INFO_ID;
                ILog.ItemAmount = Convert.ToDecimal(txtAmountReceiveable.Text);
                ILog.UserID = Convert.ToInt32(Session["UserID"]);
                ILog.IsActive = Convert.ToBoolean(1);
                ILog.TimeStamp = DateTime.Now;
                Log.Save(ILog);

                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command2 = new SqlCommand("UPDATE tbl_PURCHASE_RETURN SET ITEM_LOG_ID =" + ILog.ITEM_LOG_ID + " where PURCHASE_RETURN_ID=" + sr.PURCHASE_RETURN_ID, con))
                    {
                        command2.ExecuteNonQuery();
                    }
                    con.Close();
                }


                using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand command1 = new SqlCommand("update tbl_PURCHASE_DETAIL set PURCHASE_CLOSED = 1 where PURCHASE_DETAIL_ID =" + PurchaseID + "", con))
                    {
                        command1.ExecuteNonQuery();
                    }
                    con.Close();
                }
                LedgerEntries(sr.DISTRIBUTOR_ID, 0, Convert.ToDecimal(sr.PURCHASE_RETURN_AMOUNT)
                    , sr.DATE, sr.VENDOR_ID, Constants.Document_Purchase_Return
                    , "", Convert.ToInt32(Session["UserID"]), sr.PURCHASE_RETURN_ID
                    , Convert.ToString(sr.PURCHASE_RETURN_ID)
                    , Constants.DecimalNullValue, Constants.DecimalNullValue, ItemInfo);
                
                //ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "alert('Record saved successfully.');", true);
            }
            //Items(3);
            //LoadGrid("");
            //DoEmptyTextBox();
            //mastergrid.Visible = true;
            //Panelgrid.Visible = true;
            //PanelSale.Visible = false;
            //Panel3.Visible = true;
            Show_Voucher(hfVoucher_No.Value);
            LoadGrid("");
            DoEmptyTextBox();
            mastergrid.Visible = true;
            Panelgrid.Visible = true;
            PanelSale.Visible = false;
            Panel3.Visible = true;
            Items(3);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Panelgrid.Visible = true;
        PanelSale.Visible = false;
        DoEmptyTextBox();
        Panel3.Visible = true;
        Items(3);
        LoadCustomer();
        LoadItems("");
    }

    protected void GridSaleMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridSaleMaster.PageIndex = e.NewPageIndex;
        LoadGrid("");
    }
    protected void btnEditMaster_Click(object sender, EventArgs e)
    {
        DbCalls db = new DbCalls();
        Session["PurchaseReturnID"] = null;
        Session["PurchaseReturn"] = null;
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseReturnLogics Ser = new PurchaseReturnLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        var qry = (from k in db.CUSTOMER_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.CUSTOMER_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_RETURN_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase_Return && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase return could not update. Some payments received against this purchase return.');", true);
        }
        else
        {
            DataTable dt = new DataTable();
            DataTable dtItem = new DataTable();
            Items(3);
            Items(4);
            dtItem = (DataTable)this.Session["dtItem"];
            DataRow[] foundRows = dtItem.Select("ITEM_INFO_ID  = '" + data.ITEM_INFO_ID + "'");
            if (foundRows.Length > 0)
            {
                Session["PEV_PURCHASE_MASTER_ID"] = null;
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
                dr["PURCHASE_AMOUNT"] = foundRows[0]["PURCHASE_AMOUNT"].ToString();
                dr["DISCOUNT"] = foundRows[0]["DISCOUNT"].ToString();
                dr["TOKEN_AMOUNT"] = foundRows[0]["TOKEN_AMOUNT"].ToString();
                dr["DOWN_PAYMENT"] = foundRows[0]["DOWN_PAYMENT"].ToString();
                dr["DEBIT_AMOUNT"] = foundRows[0]["DEBIT_AMOUNT"].ToString();
                dr["BALANCE"] = foundRows[0]["BALANCE"].ToString();
                dr["PURCHASE_MASTER_ID"] = Convert.ToInt32(foundRows[0]["PURCHASE_MASTER_ID"].ToString());
                dr["RefID"] = Convert.ToInt32(foundRows[0]["RefID"].ToString());
                dr["LESS_DISCOUNT"] = foundRows[0]["LESS_DISCOUNT"].ToString();
                dr["REBATE_AMOUNT"] = foundRows[0]["REBATE_AMOUNT"].ToString();
                dt.Rows.Add(dr);
                Session["PEV_PURCHASE_MASTER_ID"] = foundRows[0]["PURCHASE_MASTER_ID"].ToString();
                Session["dt"] = dt;

                PurchaseDetailUpdate = (DataTable)this.Session["PurchaseDetailUpdate"];
                DataRow drUp = PurchaseDetailUpdate.NewRow();
                drUp["ITEM_INFO_ID"] = Convert.ToInt32(foundRows[0]["ITEM_INFO_ID"].ToString());
                drUp["ITEM_INFO_NAME"] = foundRows[0]["ITEM_NAME"].ToString();
                drUp["PROJECT_ID"] = Convert.ToInt32(foundRows[0]["PROJECT_ID"].ToString());
                drUp["REF_ID"] = Convert.ToInt32(foundRows[0]["PURCHASE_MASTER_ID"].ToString());
                drUp["RefID"] = foundRows[0]["RefID"].ToString();
                PurchaseDetailUpdate.Rows.Add(drUp);

                hfRefID.Value = foundRows[0]["RefID"].ToString();
            }

            //EditedItems(4);
            CreatTable();
            LoadDate();
            LoadCustomer();
            ddlCustomer.SelectedValue = Convert.ToString(data.VENDOR_ID);
            LoadItems("");
            ddlItem.SelectedValue = Convert.ToString(data.ITEM_INFO_ID);
            txtPurchaseDate.Text = data.DATE.ToString("dd-MMM-yyyy");
            decimal saleamount = decimal.Parse(data.PURCHASE_AMOUNT.ToString());
            lblPurchaseAmount.Text = string.Format("{0:N}", saleamount); 

            decimal DISCOUNT = decimal.Parse(data.DISCOUNT.ToString());
            decimal Discount = Convert.ToDecimal(Math.Round(DISCOUNT, 2));
            lblDiscount.Text = string.Format("{0:N}", DISCOUNT);

            decimal LESS_DISCOUNT = decimal.Parse(data.LESS_DISCOUNT.ToString());
            decimal Less_Discount = Convert.ToDecimal(Math.Round(LESS_DISCOUNT, 2));
            lblLessDiscount.Text = string.Format("{0:N}", LESS_DISCOUNT);

            decimal REBATE_AMOUNT = decimal.Parse(data.REBATE_AMOUNT.ToString());
            decimal Rebate_Amount = Convert.ToDecimal(Math.Round(REBATE_AMOUNT, 2));
            lblRebate.Text = string.Format("{0:N}", REBATE_AMOUNT);

            decimal NETAMOUNT = Convert.ToDecimal(data.PURCHASE_AMOUNT + data.DISCOUNT - data.LESS_DISCOUNT - data.REBATE_AMOUNT);
            lblNetAmount.Text = string.Format("{0:N}", NETAMOUNT);

            decimal TOKENAMOUNT = decimal.Parse(data.TOKENAMOUNT.ToString());
            lblTokenAmount.Text = string.Format("{0:N}", TOKENAMOUNT);

            decimal DOWNPAYMENT = decimal.Parse(data.DOWNPAYMENT.ToString());
            lblDownPayment.Text = string.Format("{0:N}", DOWNPAYMENT);

            //decimal saleamount = decimal.Parse(data.PURCHASE_AMOUNT.ToString());
            //txtSaleAmount.Text = Convert.ToString(Math.Round(saleamount, 2));

            decimal R_Amount = decimal.Parse(data.PURCHASE_RETURN_AMOUNT.ToString());
            txtAmountReceiveable.Text = Convert.ToString(Math.Round(R_Amount, 2));

            decimal RECEIVED = decimal.Parse(data.RECEIVED.ToString());
            lblAmountReceived.Text = string.Format("{0:N}", RECEIVED);

            decimal TotalPaidAmount = RECEIVED + TOKENAMOUNT + DOWNPAYMENT;
            lblTotalPaidAmount.Text = string.Format("{0:N}", TotalPaidAmount);

            decimal BALANCE = decimal.Parse(data.BALANCE.ToString()) - decimal.Parse(data.REBATE_AMOUNT.ToString());
            lblBalance.Text = string.Format("{0:N}", BALANCE);

            txtRemarks.Text = data.REMARKS;
            //txtSR_NO.Text = data.SR_NO;
            //txtReceiptAmount.Text = Convert.ToString(data.DEBIT_AMOUNT);
            Session.Add("SR_Amount", data.PURCHASE_RETURN_AMOUNT);
            Session.Add("DBT_Amount", data.CREDIT_AMOUNT);

            Session.Add("PurchaseReturnID", data.PURCHASE_RETURN_ID);
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
        DbCalls db = new DbCalls();
        var ProID = ((LinkButton)sender).CommandArgument;
        PurchaseReturnLogics Ser = new PurchaseReturnLogics();
        var data = Ser.Get(Convert.ToInt32(ProID));
        bool Active = Convert.ToBoolean(data.IS_DELETED);
        var qry = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID != Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_RETURN_ID && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase_Return && x.IS_DELETED == 0).ToList();
        if (qry.Any())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Selected purchase return could not be deleted. Some payments received against this purchase return.');", true);
        }
        else
        {
            try
            {
                var q = (from k in db.VENDOR_LEDGER select k).Where(x => x.VOUCHER_TYPE_ID == Constants.Journal_Voucher && x.PRINCIPAL_ID == data.VENDOR_ID && x.DOCUMENT_NO == data.PURCHASE_RETURN_ID && x.PAYMENT_MODE == Constants.CreditSale && x.DOCUMENT_TYPE_ID == Constants.Document_Purchase_Return && x.IS_DELETED == 0).ToList();
                if (!Active)
                {
                    using (SqlConnection con = new SqlConnection(Configuration.ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE tbl_PURCHASE_RETURN where PURCHASE_RETURN_ID=" + Convert.ToInt32(ProID), con))
                        {
                            command.ExecuteNonQuery();
                        }
                        if (q.Count > 0)
                        {
                            string VOUCHER_NO = q.FirstOrDefault().VOUCHER_NO;
                            using (SqlCommand command = new SqlCommand("update VENDOR_LEDGER set IS_DELETED = 1 where VOUCHER_TYPE_ID=" + Constants.Journal_Voucher + "and DOCUMENT_TYPE_ID=" + Constants.Document_Purchase_Return + "and PRINCIPAL_ID=" + data.VENDOR_ID + "and DOCUMENT_NO=" + Convert.ToInt64(data.PURCHASE_RETURN_ID) + "and PAYMENT_MODE=" + Constants.CreditSale + "and PAYEE_NAME='" + "Purchase Return" + "'", con))
                            {
                                command.ExecuteNonQuery();
                            }
                            using (SqlCommand command1 = new SqlCommand("update GL_MASTER set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                            {
                                command1.ExecuteNonQuery();
                            }
                            using (SqlCommand command2 = new SqlCommand("update GL_DETAIL set IS_DELETED = 1 where VOUCHER_NO='" + VOUCHER_NO + "'", con))
                            {
                                command2.ExecuteNonQuery();
                            }
                        }
                        
                        con.Close();
                    }
                    flag = true;
                    if (flag)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Record deleted successfully');", true);
                    }
                }
            }
            catch (Exception ex) { }
        }
        LoadGrid("");
    }

    protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItems("");
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


            if (p_DOCUMENT_TYPE_ID == Constants.Document_Purchase_Return)
            {
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, p_Distributor_Id, p_Credit, 0, p_Ledger_Date, "Purchase Return-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Return",0,Constants.Document_Purchase_Return);
                LController.PostingPrinvipalInvoiceAccountforCustomer(Constants.Journal_Voucher, VoucherNo2, Constants.AccountReceivable, p_Distributor_Id, 0, p_Credit, p_Ledger_Date, "Purchase Return-" + p_Remarks, DateTime.Now, p_PRINCIPAL_ID, p_DOCUMENT_NO, p_MANUAL_DOCUMENT_NO, p_DOCUMENT_TYPE_ID, p_UserId, mTransaction, mConnection, Constants.CreditSale, "Purchase Return", 0, Constants.Document_Purchase_Return);
                if (IsFinanceSetting)
                {
                    LController.PostingGLMaster(p_Distributor_Id, p_PRINCIPAL_ID, VoucherNo2
                        , Constants.Journal_Voucher, p_Ledger_Date, Constants.Document_Purchase_Return
                        , p_MANUAL_DOCUMENT_NO, "Purchase Return Voucher / " + ddlCustomer.SelectedItem.Text + ", " + p_Remarks, p_UserId, "Purchase Return", mTransaction, mConnection);

                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Trade_Payables, p_Credit, 0, "Purchase Return Voucher", mTransaction, mConnection);
                    LController.PostingGLDetail(p_Distributor_Id, p_PRINCIPAL_ID, Constants.Journal_Voucher, VoucherNo2, Constants.AccountHead_Stock_in_Trade, 0, p_Credit, "Purchase Return Voucher", mTransaction, mConnection);
                    hfVoucher_No.Value = VoucherNo2;
                }
            }
            #endregion
            mTransaction.Commit();
        }
        catch (Exception exp)
        {
            ExceptionPublisher.PublishException(exp);
            mTransaction.Rollback();
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
    protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCustomer();
        LoadItems("");
        txtSale_Amount();
        ddlItem_SelectedIndexChanged(null, null);

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
}