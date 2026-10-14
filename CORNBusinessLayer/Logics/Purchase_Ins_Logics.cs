using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Purchase_Ins_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_PURCHASE_INS> Get()
        {
            var DbObj = db.tbl_PURCHASE_INS.ToList();
            return DbObj;
        }
        public Classes.tbl_PURCHASE_INS Get(int ID)
        {
            var DbObj = db.tbl_PURCHASE_INS.Find(ID);
            return DbObj;
        }
        public int Save(int p_PURCHASE_INS_MASTER_ID,int p_INS_TYPE,string p_INS_TYPE_NAME, int p_NO_INS,decimal p_INS_AMOUNT,DateTime p_FROM_MONTH,decimal p_TOTAL_INS_AMOUNT)
        {
            tbl_PURCHASE_INS pi = new tbl_PURCHASE_INS();
            try
            {
                pi.PURCHASE_INS_MASTER_ID = p_PURCHASE_INS_MASTER_ID;
                pi.INS_TYPE = p_INS_TYPE;
                pi.INS_TYPE_NAME = p_INS_TYPE_NAME;
                pi.NO_INS = p_NO_INS;
                pi.INS_AMOUNT = p_INS_AMOUNT;
                pi.FROM_MONTH = p_FROM_MONTH;
                pi.TOTAL_INS_AMOUNT = p_TOTAL_INS_AMOUNT;
                db.tbl_PURCHASE_INS.Add(pi);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return pi.PURCHASE_INS_MASTER_ID;
        }
        public bool Update(Classes.tbl_PURCHASE_INS SaveObj)
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
