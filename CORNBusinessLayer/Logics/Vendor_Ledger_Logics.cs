using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Vendor_Ledger_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.VENDOR_LEDGER> Get()
        {
            var DbObj = db.VENDOR_LEDGER.ToList();
            return DbObj;
        }
        public Classes.VENDOR_LEDGER Get(int ID)
        {
            var DbObj = db.VENDOR_LEDGER.Find(ID);
            return DbObj;
        }
        public int Save(Classes.VENDOR_LEDGER SaveObj)
        {
            try
            {
                db.VENDOR_LEDGER.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Convert.ToInt32(SaveObj.VENDOR_LEDGER_ID);
        }
        public bool Update(Classes.VENDOR_LEDGER SaveObj)
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
