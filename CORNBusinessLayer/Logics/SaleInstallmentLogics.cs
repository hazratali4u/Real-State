using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class SaleInstallmentLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_SALE_INS_MASTER> Get()
        {
            var DbObj = db.tbl_SALE_INS_MASTER.ToList();
            return DbObj;
        }
        public Classes.tbl_SALE_INS_MASTER Get(int ID)
        {
            var DbObj = db.tbl_SALE_INS_MASTER.Find(ID);
            return DbObj;
        }
        public long Save(Classes.tbl_SALE_INS_MASTER SaveObj)
        {
            try
            {
                db.tbl_SALE_INS_MASTER.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.SALE_INS_MASTER_ID;
        }
        public bool Update(Classes.tbl_SALE_INS_MASTER SaveObj)
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
