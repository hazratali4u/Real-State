using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class SaleReturnLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_SALE_RETURN> Get()
        {
            var DbObj = db.tbl_SALE_RETURN.ToList();
            return DbObj;
        }
        public Classes.tbl_SALE_RETURN Get(int ID)
        {
            var DbObj = db.tbl_SALE_RETURN.Find(ID);
            return DbObj;
        }
        public int Save(Classes.tbl_SALE_RETURN SaveObj)
        {
            try
            {
                db.tbl_SALE_RETURN.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.SALE_RETURN_ID;
        }
        public bool Update(Classes.tbl_SALE_RETURN SaveObj)
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
