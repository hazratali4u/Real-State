using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
public    class PurchaseReturnLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_PURCHASE_RETURN> Get()
        {
            var DbObj = db.tbl_PURCHASE_RETURN.ToList();
            return DbObj;
        }
        public Classes.tbl_PURCHASE_RETURN Get(int ID)
        {
            var DbObj = db.tbl_PURCHASE_RETURN.Find(ID);
            return DbObj;
        }
        public int Save(Classes.tbl_PURCHASE_RETURN SaveObj)
        {
            try
            {
                db.tbl_PURCHASE_RETURN.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PURCHASE_RETURN_ID;
        }
        public bool Update(Classes.tbl_PURCHASE_RETURN SaveObj)
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
