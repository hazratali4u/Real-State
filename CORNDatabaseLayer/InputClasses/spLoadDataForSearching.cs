using CORNCommon.Classes;
using CORNDataAccessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CORNDatabaseLayer.InputClasses
{
   public class spLoadDataForSearching
    {
        #region Private Members
        private string sp_Name = "spLoadDataForSearching";
        private IDbConnection m_connection;
        private IDbTransaction m_transaction;
        private int m_TYPEID;
        //private int m_DISTRIBUTOR_ID;
        //private int m_PROJECT_ID;
        //private int m_BLOCK_ID;
        //private int m_STREET_ID;

        #endregion
        #region Public Properties

        //public int DISTRIBUTOR_ID
        //{
        //    set
        //    {
        //        m_DISTRIBUTOR_ID = value;
        //    }
        //    get
        //    {
        //        return m_DISTRIBUTOR_ID;
        //    }
        //}
        public int TYPEID
        {
            set
            {
                m_TYPEID = value;
            }
            get
            {
                return m_TYPEID;
            }
        }
        //public int PROJECT_ID
        //{
        //    set
        //    {
        //        m_PROJECT_ID = value;
        //    }
        //    get
        //    {
        //        return m_PROJECT_ID;
        //    }
        //}

        //public int BLOCK_ID
        //{
        //    set
        //    {
        //        m_BLOCK_ID = value;
        //    }
        //    get
        //    {
        //        return m_BLOCK_ID;
        //    }
        //}
        //public int STREET_ID
        //{
        //    set
        //    {
        //        m_STREET_ID = value;
        //    }
        //    get
        //    {
        //        return m_STREET_ID;
        //    }
        //}

        public IDbConnection Connection
        {
            set
            {
                m_connection = value;
            }
            get
            {
                return m_connection;
            }
        }
        public IDbTransaction Transaction
        {
            set
            {
                m_transaction = value;
            }
            get
            {
                return m_transaction;
            }
        }
        #endregion
        #region Constructor
        public spLoadDataForSearching()
        {

        }
        #endregion
        #region public Methods
        public bool ExecuteQuery()
        {
            try
            {
                IDbCommand cmd = ProviderFactory.GetCommand(EnumProviders.SQLClient);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp_Name;
                cmd.Connection = m_connection;
                if (m_transaction != null)
                {
                    cmd.Transaction = m_transaction;
                }
                GetParameterCollection(ref cmd);
                cmd.ExecuteNonQuery();
                //m_GETCOUNT = (int)((IDataParameter)(cmd.Parameters["@GETCOUNT"])).Value;
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }
        }
        public IDataReader ExecuteReader()
        {
            try
            {
                IDbCommand command = ProviderFactory.GetCommand(EnumProviders.SQLClient);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sp_Name;
                command.Connection = m_connection;
                if (m_transaction != null)
                {
                    command.Transaction = m_transaction;
                }
                GetParameterCollection(ref command);
                IDataReader dr = command.ExecuteReader();
                return dr;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
            }
        }
        public DataTable ExecuteTable()
        {
            try
            {
                IDbCommand command = ProviderFactory.GetCommand(EnumProviders.SQLClient);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sp_Name;
                command.Connection = m_connection;
                if (m_transaction != null)
                {
                    command.Transaction = m_transaction;
                }
                GetParameterCollection(ref command);
                IDbDataAdapter da = ProviderFactory.GetAdapter(EnumProviders.SQLClient);
                da.SelectCommand = command;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
            }
        }
        public string ExecuteScalar()
        {
            try
            {
                IDbCommand command = ProviderFactory.GetCommand(EnumProviders.SQLClient);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sp_Name;
                command.Connection = m_connection;
                if (m_transaction != null)
                {
                    command.Transaction = m_transaction;
                }
                GetParameterCollection(ref command);
                object o;
                o = command.ExecuteScalar();


                return o.ToString();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
            }
        }

        public void GetParameterCollection(ref IDbCommand cmd)
        {
            IDataParameterCollection pparams = cmd.Parameters;
            IDataParameter parameter;
            parameter = ProviderFactory.GetParameter(EnumProviders.SQLClient);
            parameter.ParameterName = "@TYPEID";
            parameter.DbType = ProviderFactory.GetDBType(EnumProviders.SQLClient, EnumDBTypes.Int);
            if (m_TYPEID == Constants.IntNullValue)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = m_TYPEID;
            }
            pparams.Add(parameter);


            //parameter = ProviderFactory.GetParameter(EnumProviders.SQLClient);
            //parameter.ParameterName = "@DISTRIBUTOR_ID";
            //parameter.DbType = ProviderFactory.GetDBType(EnumProviders.SQLClient, EnumDBTypes.Int);
            //if (m_DISTRIBUTOR_ID == Constants.IntNullValue)
            //{
            //    parameter.Value = DBNull.Value;
            //}
            //else
            //{
            //    parameter.Value = m_DISTRIBUTOR_ID;
            //}
            //pparams.Add(parameter);

            //parameter = ProviderFactory.GetParameter(EnumProviders.SQLClient);
            //parameter.ParameterName = "@PROJECT_ID";
            //parameter.DbType = ProviderFactory.GetDBType(EnumProviders.SQLClient, EnumDBTypes.Int);
            //if (m_PROJECT_ID == Constants.IntNullValue)
            //{
            //    parameter.Value = DBNull.Value;
            //}
            //else
            //{
            //    parameter.Value = m_PROJECT_ID;
            //}
            //pparams.Add(parameter);

            //parameter = ProviderFactory.GetParameter(EnumProviders.SQLClient);
            //parameter.ParameterName = "@BLOCK_ID";
            //parameter.DbType = ProviderFactory.GetDBType(EnumProviders.SQLClient, EnumDBTypes.Int);
            //if (m_BLOCK_ID == Constants.IntNullValue)
            //{
            //    parameter.Value = DBNull.Value;
            //}
            //else
            //{
            //    parameter.Value = m_BLOCK_ID;
            //}
            //pparams.Add(parameter);


            //parameter = ProviderFactory.GetParameter(EnumProviders.SQLClient);
            //parameter.ParameterName = "@STREET_ID";
            //parameter.DbType = ProviderFactory.GetDBType(EnumProviders.SQLClient, EnumDBTypes.Int);
            //if (m_STREET_ID == Constants.IntNullValue)
            //{
            //    parameter.Value = DBNull.Value;
            //}
            //else
            //{
            //    parameter.Value = m_STREET_ID;
            //}
            //pparams.Add(parameter);
        }
        #endregion
    }
}
