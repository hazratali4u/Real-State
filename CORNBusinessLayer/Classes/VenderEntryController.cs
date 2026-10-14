using System;
using System.Data;
using CORNCommon.Classes;
using CORNDatabaseLayer.Classes;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections;
using CORNDataAccessLayer.Classes;
using CORNDatabaseLayer.ReportClasses;
using CORNDatabaseLayer.InputClasses;

namespace CORNBusinessLayer.Classes
{
   public  class VenderEntryController
    {


       #region Constructor

        /// <summary>
        /// Constructor for OrderEntryController
        /// </summary>
       public VenderEntryController()
		{
			//
			// TODO: Add constructor logic here
			//
		}
        #endregion

        #region Select

        #region Vendoer Ledger

        public DataSet GetVendorLedger(int p_PRINCIPAL_ID, int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
               Reports.DsReport ds = new Reports.DsReport();

                uspGetVendorLedgerReport mLedger = new uspGetVendorLedgerReport();

                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();
                
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptCustomerLedgerView"].ImportRow(dr);

                }
                
                spSelectCHEQUE_PROCESS2 mLedgerSub = new spSelectCHEQUE_PROCESS2();
                mLedgerSub.Connection = mConnection;
                mLedgerSub.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedgerSub.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedgerSub.CUSTOMER_ID = Convert.ToInt32(p_PRINCIPAL_ID);
                mLedgerSub.STATUS_ID = 500001;// Recieved and deposit Cheques

                DataTable dtPro = mLedgerSub.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["spSelectCHEQUE_PROCESS2"].ImportRow(dr);
                }

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetMasterInventory(int p_DISTRIBUTOR_ID, int p_PROJECT_ID, int p_PRINCIPAL_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                uspGetMasterInventoryReport mLedger = new uspGetMasterInventoryReport();

                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetMasterInventoryReport"].ImportRow(dr);

                }
                //spSelectCHEQUE_PROCESS2 mLedgerSub = new spSelectCHEQUE_PROCESS2();
                //mLedgerSub.Connection = mConnection;
                //mLedgerSub.PRINCIPAL_ID = p_PRINCIPAL_ID;
                //mLedgerSub.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                //mLedgerSub.CUSTOMER_ID = Convert.ToInt32(p_PRINCIPAL_ID);
                //mLedgerSub.STATUS_ID = 527528;// Recieved and deposit Cheques
                //DataTable dtPro = mLedgerSub.ExecuteTable();
                //foreach (DataRow dr in dtPro.Rows)
                //{
                //    ds.Tables["spSelectCHEQUE_PROCESS2"].ImportRow(dr);
                //}

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetInventoryDetail(int p_DISTRIBUTOR_ID, string p_PROJECT_ID, string p_MainCategory_ID, string p_SubCategory_ID, string p_Facing_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_InventoryType_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                uspGetInventoryDetailReport mLedger = new uspGetInventoryDetailReport();

                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_IDs = p_PROJECT_ID;
                mLedger.Categories_IDs = p_MainCategory_ID;
                mLedger.SubCategories_IDs = p_SubCategory_ID;
                mLedger.Facing_IDs = p_Facing_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.INVENTORYTYPE_ID = p_InventoryType_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetInventoryDetailReport"].ImportRow(dr);

                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetMasterSale(int p_DISTRIBUTOR_ID, int p_PROJECT_ID, int p_PRINCIPAL_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetMasterSaleReport mLedger = new uspGetMasterSaleReport();
                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetMasterSaleReport"].ImportRow(dr);

                }
                //spSelectCHEQUE_PROCESS2 mLedgerSub = new spSelectCHEQUE_PROCESS2();
                //mLedgerSub.Connection = mConnection;
                //mLedgerSub.PRINCIPAL_ID = p_PRINCIPAL_ID;
                //mLedgerSub.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                //mLedgerSub.CUSTOMER_ID = Convert.ToInt32(p_PRINCIPAL_ID);
                //mLedgerSub.STATUS_ID = 527528;// Recieved and deposit Cheques
                //DataTable dtPro = mLedgerSub.ExecuteTable();
                //foreach (DataRow dr in dtPro.Rows)
                //{
                //    ds.Tables["spSelectCHEQUE_PROCESS2"].ImportRow(dr);
                //}

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetOpenChqAndPOz(int p_DISTRIBUTOR_ID, int p_PRINCIPAL_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetOpenChqPOzReport mLedger = new uspGetOpenChqPOzReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetOpenChqPOzReport"].ImportRow(dr);

                }
                //spSelectCHEQUE_PROCESS2 mLedgerSub = new spSelectCHEQUE_PROCESS2();
                //mLedgerSub.Connection = mConnection;
                //mLedgerSub.PRINCIPAL_ID = p_PRINCIPAL_ID;
                //mLedgerSub.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                //mLedgerSub.CUSTOMER_ID = Convert.ToInt32(p_PRINCIPAL_ID);
                //mLedgerSub.STATUS_ID = 527528;// Recieved and deposit Cheques
                //DataTable dtPro = mLedgerSub.ExecuteTable();
                //foreach (DataRow dr in dtPro.Rows)
                //{
                //    ds.Tables["spSelectCHEQUE_PROCESS2"].ImportRow(dr);
                //}

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetPartyReport(int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetPartyReport mLedger = new uspGetPartyReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetPartyReport"].ImportRow(dr);

                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetPropertySellerReport(int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetPropertySellerReport mLedger = new uspGetPropertySellerReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetPropertySellerReport"].ImportRow(dr);

                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetProfitReport(int p_DISTRIBUTOR_ID, int p_PARTY_ID, int p_VENDOR_ID, string p_Projects, string p_Categories, string p_SubCategories, string p_DetailCategories, string p_ItemTypes, string p_Measurements, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetProfitReport mLedger = new uspGetProfitReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.DETAIL_CATEGORY_ID = p_DetailCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                mLedger.ITEM_TYPE_ID = p_ItemTypes;
                mLedger.MEASUREMENT_ID = p_Measurements;
                mLedger.VENDOR_ID = p_VENDOR_ID;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetProfitReport"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetInventoryReport(int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Project_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetInventoryReport mLedger = new uspGetInventoryReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.Project_ID = p_Project_ID;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetInventoryReport"].ImportRow(dr);

                }

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetGainOrLossOnSale(int p_DISTRIBUTOR_ID, int p_PROJECT_ID, string p_Customers, string p_Categories, string p_Streets, string p_Measurements, string p_Itemss, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspGainOrLossOnSale mLedger = new UspGainOrLossOnSale();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.CUSTOMER_ID = p_Customers;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.STREET_ID = p_Streets;
                mLedger.MEASURMENT_ID = p_Measurements;
                mLedger.ITEM_ID = p_Itemss;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspGainOrLossOnSale"].ImportRow(dr);

                }
               
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataTable GetLoadMeasurement(int p_DISTRIBUTOR_ID, int p_PROJECT_ID, string p_Customers, string p_Categories, string p_Streets, string p_Measurements, string p_Itemss, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                spLoadSalesData mLedger = new spLoadSalesData();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.CUSTOMER_ID = p_Customers;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.STREET_ID = p_Streets;
                mLedger.MEASURMENT_ID = p_Measurements;
                mLedger.ITEM_ID = p_Itemss;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }


        public DataTable GetLoadDropDowns(int p_DISTRIBUTOR_ID, string p_Projects, string p_Customers, string p_Categories, string p_Streets, string p_Measurements, string p_Itemss, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                spLoadDropDowns mLedger = new spLoadDropDowns();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CUSTOMER_ID = p_Customers;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.STREET_ID = p_Streets;
                mLedger.MEASURMENT_ID = p_Measurements;
                mLedger.ITEM_ID = p_Itemss;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetCustomerLedger(int p_PRINCIPAL_ID, int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_id)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                uspGetVendorLedgerReport mLedger = new uspGetVendorLedgerReport();

                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TypeID = p_Type_id;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["RptCustomerLedgerView"].ImportRow(dr);

                }

                spSelectCHEQUE_PROCESS2 mLedgerSub = new spSelectCHEQUE_PROCESS2();

                mLedgerSub.Connection = mConnection;
                mLedgerSub.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedgerSub.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedgerSub.CUSTOMER_ID = Convert.ToInt32(p_PRINCIPAL_ID);
                mLedgerSub.STATUS_ID = 500000; //526528;// Recieved and deposit Cheques

                DataTable dtPro = mLedgerSub.ExecuteTable();

                foreach (DataRow dr in dtPro.Rows)
                {
                    ds.Tables["spSelectCHEQUE_PROCESS2"].ImportRow(dr);
                }

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetCustomerInstallmentDetail( int p_DISTRIBUTOR_ID, int p_PROJECT_ID, int p_CUSTOMER_ID, int p_MASTER_ID, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                uspGetCustomerInstallments mLedger = new uspGetCustomerInstallments();

                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.CUSTOMER_ID = p_CUSTOMER_ID;
                mLedger.MASTER_ID = p_MASTER_ID;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetCustomerInstallments"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetSupplierWise(int p_DISTRIBUTOR_ID, int p_PROJECT_ID, int p_SUPPLIER_ID, int p_MASTER_ID, int p_Type_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                uspGetCustomerInstallments mLedger = new uspGetCustomerInstallments();

                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.PROJECT_ID = p_PROJECT_ID;
                mLedger.CUSTOMER_ID = p_SUPPLIER_ID;
                mLedger.MASTER_ID = p_MASTER_ID;
                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetCustomerInstallments"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataTable GetVendorOpening(int p_PRINCIPAL_ID, int p_DISTRIBUTOR_ID, DateTime p_FROM_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
              

                uspGetVendorLedgerOpening mLedger = new uspGetVendorLedgerOpening();

                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetSupplierClosingSummary(int p_PRINCIPAL_ID, int p_DISTRIBUTOR_ID, DateTime p_TO_DATE)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DSReportNew ds = new Reports.DSReportNew();

                uspGetSupplierClosingSummary mLedger = new uspGetSupplierClosingSummary();

                mLedger.Connection = mConnection;
                mLedger.PRINCIPAL_ID = p_PRINCIPAL_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.TO_DATE = p_TO_DATE;

                DataTable DT = mLedger.ExecuteTable();

                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetSupplierClosingSummary"].ImportRow(dr);

                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetCommissionDetail(int p_Distributor_ID, int p_Principal_Id, DateTime p_FromDate, DateTime p_To_Date, int p_UserId, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                uspGetCommissionDetail ObjPrint = new uspGetCommissionDetail();
                CORNBusinessLayer.Reports.DsReport ds = new CORNBusinessLayer.Reports.DsReport();
                ObjPrint.Connection = mConnection;
                ObjPrint.DISTRIBUTOR_ID = p_Distributor_ID;
                ObjPrint.PRINCIPAL_ID = p_Principal_Id;
                ObjPrint.FROM_DATE = p_FromDate;
                ObjPrint.TO_DATE = p_To_Date;
                ObjPrint.USER_ID = p_UserId;
                ObjPrint.TYPE_ID = p_TYPE_ID;
                DataTable dt = ObjPrint.ExecuteTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ds.Tables["uspGetCommissionDetail"].ImportRow(dr);
                }

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetPartyClosing(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspPartyClosing mLedger = new UspPartyClosing();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspPartyClosing"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetPartyBalances(int p_DISTRIBUTOR_ID, string p_Parties, string p_Projects, string p_Categories, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspPartyBalances mLedger = new UspPartyBalances();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_Parties;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspPartyBalances"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetPartyInvoicesSummary(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspInvoicesSummary mLedger = new UspInvoicesSummary();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspInvoicesSummary"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetInvoiceWiseProfitability(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, string p_DetailCategories, string p_ItemTypes, string p_Measurements, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspInvoiceWiseProfitability mLedger = new UspInvoiceWiseProfitability();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                mLedger.DETAIL_CATEGORY_ID = p_DetailCategories;
                mLedger.ITEM_TYPE_ID = p_ItemTypes;
                mLedger.MEASUREMENT_ID = p_Measurements;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspInvoiceWiseProfitability"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetPartyCommonInvoicesSummaryWithoutInvoices(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                // For Invoices
                UspInvoicesSummary mLedger = new UspInvoicesSummary();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = 1;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspInvoicesSummary"].ImportRow(dr);
                }
                // For Purchases
                UspInvoicesSummary mLedger2 = new UspInvoicesSummary();
                mLedger2.Connection = mConnection;
                mLedger2.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger2.CUSTOMER_ID = p_PARTY_ID;
                mLedger2.PROJECT_ID = p_Projects;
                mLedger2.CATEGORY_ID = p_Categories;
                mLedger2.SUB_CATEGORY_ID = p_SubCategories;
                mLedger2.FROM_DATE = p_FROM_DATE;
                mLedger2.TO_DATE = p_TO_DATE;
                mLedger2.TYPE_ID = 2;
                DataTable DT2 = mLedger2.ExecuteTable();
                foreach (DataRow dr in DT2.Rows)
                {
                    ds.Tables["UspInvoicesSummary1"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetPartyCommonInvoicesSummaryWithInvoices(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();

                // For Invoices
                UspInvoicesSummary mLedger = new UspInvoicesSummary();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = 10;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspInvoicesSummary"].ImportRow(dr);
                }
                // For Purchases
                UspInvoicesSummary mLedger2 = new UspInvoicesSummary();
                mLedger2.Connection = mConnection;
                mLedger2.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger2.CUSTOMER_ID = p_PARTY_ID;
                mLedger2.PROJECT_ID = p_Projects;
                mLedger2.CATEGORY_ID = p_Categories;
                mLedger2.SUB_CATEGORY_ID = p_SubCategories;
                mLedger2.FROM_DATE = p_FROM_DATE;
                mLedger2.TO_DATE = p_TO_DATE;
                mLedger2.TYPE_ID = 20;
                DataTable DT2 = mLedger2.ExecuteTable();
                foreach (DataRow dr in DT2.Rows)
                {
                    ds.Tables["UspInvoicesSummary1"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetCommonProfitability(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Projects, string p_Categories, string p_SubCategories, string p_DetailCategories, string p_ItemTypes, string p_Measurements, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                // For Sales Invoices
                UspInvoiceWiseProfitability mLedger = new UspInvoiceWiseProfitability();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Projects;
                mLedger.CATEGORY_ID = p_Categories;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = 1;
                mLedger.DETAIL_CATEGORY_ID = p_DetailCategories;
                mLedger.ITEM_TYPE_ID = p_ItemTypes;
                mLedger.MEASUREMENT_ID = p_Measurements;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspInvoiceWiseProfitability"].ImportRow(dr);
                }
                // For Purchase Invoices
                UspInvoiceWiseProfitability mLedger2 = new UspInvoiceWiseProfitability();
                mLedger2.Connection = mConnection;
                mLedger2.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger2.CUSTOMER_ID = p_PARTY_ID;
                mLedger2.PROJECT_ID = p_Projects;
                mLedger2.CATEGORY_ID = p_Categories;
                mLedger2.SUB_CATEGORY_ID = p_SubCategories;
                mLedger2.FROM_DATE = p_FROM_DATE;
                mLedger2.TO_DATE = p_TO_DATE;
                mLedger2.TYPE_ID = 2;
                DataTable DT2 = mLedger2.ExecuteTable();
                foreach (DataRow dr in DT2.Rows)
                {
                    ds.Tables["UspInvoiceWiseProfitability1"].ImportRow(dr);
                }

                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        #endregion

        public DataSet GetInventoryInHandReport(int p_DISTRIBUTOR_ID, int p_Project_ID, int p_Category_ID, int p_Sub_Category_ID, int p_Detail_Category_ID, int p_Item_Type_ID
            , int p_Measurement_ID, int p_Phase_ID, int p_Block_ID, int p_Sector_ID, int p_Precent_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_Type_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                uspGetInventoryInHandReport mLedger = new uspGetInventoryInHandReport();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.Project_ID = p_Project_ID;
                mLedger.CATEGORY_ID = p_Category_ID;
                mLedger.SUB_CATEGORY_ID = p_Sub_Category_ID;
                mLedger.DETAIL_CATEGORY_ID = p_Detail_Category_ID;
                mLedger.ITEM_TYPE_ID = p_Item_Type_ID;
                mLedger.MEASUREMENT_ID = p_Measurement_ID;
                mLedger.PHASE_ID = p_Phase_ID;
                mLedger.BLOCK_ID = p_Block_ID;
                mLedger.SECTOR_ID = p_Sector_ID;
                mLedger.PRECENT_ID = p_Precent_ID;
                mLedger.TYPE_ID = p_Type_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["uspGetInventoryInHandReport"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetDailyClosing(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Locations, string p_AccountHeads, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspDailyClosing mLedger = new UspDailyClosing();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Locations;
                mLedger.CATEGORY_ID = p_AccountHeads;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = 0;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspDailyClosing"].ImportRow(dr);
                }

                UspDailyClosing mLedger1 = new UspDailyClosing();
                mLedger1.Connection = mConnection;
                mLedger1.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger1.CUSTOMER_ID = p_PARTY_ID;
                mLedger1.PROJECT_ID = p_Locations;
                mLedger1.CATEGORY_ID = p_AccountHeads;
                mLedger1.SUB_CATEGORY_ID = p_SubCategories;
                mLedger1.FROM_DATE = p_FROM_DATE;
                mLedger1.TO_DATE = p_TO_DATE;
                mLedger1.TYPE_ID = 1;
                DataTable DT1 = mLedger1.ExecuteTable();
                foreach (DataRow dr in DT1.Rows)
                {
                    ds.Tables["UspDailyClosing1"].ImportRow(dr);
                }

                UspOutstandingInstuments mLedger2 = new UspOutstandingInstuments();
                mLedger2.Connection = mConnection;
                mLedger2.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger2.CUSTOMER_ID = p_PARTY_ID;
                mLedger2.PROJECT_ID = p_Locations;
                mLedger2.CATEGORY_ID = p_AccountHeads;
                mLedger2.SUB_CATEGORY_ID = p_SubCategories;
                mLedger2.FROM_DATE = p_FROM_DATE;
                mLedger2.TO_DATE = p_TO_DATE;
                mLedger2.TYPE_ID = 4;
                DataTable DT2 = mLedger2.ExecuteTable();
                foreach (DataRow dr in DT2.Rows)
                {
                    ds.Tables["UspFundPosition"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataTable GetDailyOpening(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Locations, string p_AccountHeads, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspDailyClosing mLedger = new UspDailyClosing();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Locations;
                mLedger.CATEGORY_ID = p_AccountHeads;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                //foreach (DataRow dr in DT.Rows)
                //{
                //    ds.Tables["UspDailyClosing"].ImportRow(dr);
                //}
                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetFundPositionReport(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Locations, string p_AccountHeads, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspFundPosition mLedger = new UspFundPosition();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Locations;
                mLedger.CATEGORY_ID = p_AccountHeads;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = 0;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspFundPosition"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetRptOutstandingInstumentsReport(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Locations, string p_AccountHeads, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspOutstandingInstuments mLedger = new UspOutstandingInstuments();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Locations;
                mLedger.CATEGORY_ID = p_AccountHeads;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspFundPosition"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataTable DeleteVoucher(int p_DISTRIBUTOR_ID, int p_VOUCHER_TYPE_ID, string p_VoucherNos, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                DataTable DT = new DataTable();
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspDeleteVoucher mLedger = new UspDeleteVoucher();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.VOUCHER_TYPE_ID = p_VOUCHER_TYPE_ID;
                mLedger.VOUCHER_NOs = p_VoucherNos;
                mLedger.TYPE_ID = p_TYPE_ID;
                if (p_TYPE_ID == 0)
                {
                    DT = mLedger.ExecuteTable();
                }
                else
                {
                    mLedger.ExecuteQuery();
                }
                //foreach (DataRow dr in DT.Rows)
                //{
                //    ds.Tables["UspDailyClosing"].ImportRow(dr);
                //}
                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public bool DeleteVouchers(int p_DISTRIBUTOR_ID, int p_VOUCHER_TYPE_ID, string p_VoucherNos, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspDeleteVoucher mLedger = new UspDeleteVoucher();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.VOUCHER_TYPE_ID = p_VOUCHER_TYPE_ID;
                mLedger.VOUCHER_NOs = p_VoucherNos;
                mLedger.TYPE_ID = p_TYPE_ID;
                    mLedger.ExecuteQuery();
                //foreach (DataRow dr in DT.Rows)
                //{
                //    ds.Tables["UspDailyClosing"].ImportRow(dr);
                //}
                return true;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return false;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataSet GetRptSales_PurchaseBalancesReport(int p_DISTRIBUTOR_ID, int p_PARTY_ID, string p_Locations, string p_AccountHeads, string p_SubCategories, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspSalesPurchaseBalances mLedger = new UspSalesPurchaseBalances();
                mLedger.Connection = mConnection;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.CUSTOMER_ID = p_PARTY_ID;
                mLedger.PROJECT_ID = p_Locations;
                mLedger.CATEGORY_ID = p_AccountHeads;
                mLedger.SUB_CATEGORY_ID = p_SubCategories;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspSalesPurchaseBalances"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }
        public DataSet GetInvoices(int p_DISTRIBUTOR_ID, int p_PARTY_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, string p_INVOICE_IDs, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspGetInvoices mLedger = new UspGetInvoices();
                mLedger.Connection = mConnection;
                mLedger.PARTY_ID = p_PARTY_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.INVOICE_IDs = p_INVOICE_IDs;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                foreach (DataRow dr in DT.Rows)
                {
                    ds.Tables["UspGetInvoices"].ImportRow(dr);
                }
                return ds;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        public DataTable GetInvoicesData(int p_DISTRIBUTOR_ID, int p_PARTY_ID, DateTime p_FROM_DATE, DateTime p_TO_DATE, int p_TYPE_ID)
        {
            IDbConnection mConnection = null;
            try
            {
                mConnection = ProviderFactory.GetConnection(Configuration.ConnectionString, EnumProviders.SQLClient);
                mConnection.Open();
                Reports.DsReport ds = new Reports.DsReport();
                UspGetInvoices mLedger = new UspGetInvoices();
                mLedger.Connection = mConnection;
                mLedger.PARTY_ID = p_PARTY_ID;
                mLedger.DISTRIBUTOR_ID = p_DISTRIBUTOR_ID;
                mLedger.FROM_DATE = p_FROM_DATE;
                mLedger.TO_DATE = p_TO_DATE;
                mLedger.TYPE_ID = p_TYPE_ID;
                DataTable DT = mLedger.ExecuteTable();
                return DT;
            }
            catch (Exception exp)
            {
                ExceptionPublisher.PublishException(exp);
                return null;
            }
            finally
            {
                if (mConnection != null && mConnection.State == ConnectionState.Open)
                {
                    mConnection.Close();
                }
            }
        }

        #endregion


    }
}
