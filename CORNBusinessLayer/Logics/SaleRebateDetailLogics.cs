using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
    public class SaleRebateDetailLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_SALE_REBATE_DETAIL> Get()
        {
            var DbObj = db.tbl_SALE_REBATE_DETAIL.ToList();
            return DbObj;
        }
        public Classes.tbl_SALE_REBATE_DETAIL Get(int ID)
        {
            var DbObj = db.tbl_SALE_REBATE_DETAIL.Find(ID);
            return DbObj;
        }
        public int Save(Classes.tbl_SALE_REBATE_DETAIL SaveObj)
        {
            try
            {
                db.tbl_SALE_REBATE_DETAIL.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.REBATE_DETAIL_ID;
        }
        public bool Update(Classes.tbl_SALE_REBATE_DETAIL SaveObj)
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
