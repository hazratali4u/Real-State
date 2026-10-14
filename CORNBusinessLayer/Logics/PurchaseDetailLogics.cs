using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class PurchaseDetailLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_PURCHASE_DETAIL> Get()
        {
            var DbObj = db.tbl_PURCHASE_DETAIL.ToList();
            return DbObj;
        }
        public Classes.tbl_PURCHASE_DETAIL Get(int ID)
        {
            var DbObj = db.tbl_PURCHASE_DETAIL.Find(ID);
            return DbObj;
        }
        public long Save(Classes.tbl_PURCHASE_DETAIL SaveObj)
        {
            try
            {
                db.tbl_PURCHASE_DETAIL.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PURCHASE_DETAIL_ID;
        }
        public bool Update(Classes.tbl_PURCHASE_DETAIL SaveObj)
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
