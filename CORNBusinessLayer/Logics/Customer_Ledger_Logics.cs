using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Customer_Ledger_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.CUSTOMER_LEDGER> Get()
        {
            var DbObj = db.CUSTOMER_LEDGER.ToList();
            return DbObj;
        }
        public Classes.CUSTOMER_LEDGER Get(int ID)
        {
            var DbObj = db.CUSTOMER_LEDGER.Find(ID);
            return DbObj;
        }
        public int Save(Classes.CUSTOMER_LEDGER SaveObj)
        {
            try
            {
                db.CUSTOMER_LEDGER.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Convert.ToInt32(SaveObj.CUSTOMER_LEDGER_ID);
        }
        public bool Update(Classes.CUSTOMER_LEDGER SaveObj)
        {
            try
            {
                db.Entry(SaveObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return false;
        }
    }
}
