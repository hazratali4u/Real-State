using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Purchase_Ins_Detail_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_PURCHASE_INS_DETAIL> Get()
        {
            var DbObj = db.tbl_PURCHASE_INS_DETAIL.ToList();
            return DbObj;
        }
        public Classes.tbl_PURCHASE_INS_DETAIL Get(int ID)
        {
            var DbObj = db.tbl_PURCHASE_INS_DETAIL.Find(ID);
            return DbObj;
        }
        public int Save(Classes.tbl_PURCHASE_INS_DETAIL SaveObj)
        {
            try
            {
                db.tbl_PURCHASE_INS_DETAIL.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PURCHASE_INS_MASTER_ID;
        }
        public bool Update(Classes.tbl_PURCHASE_INS_DETAIL SaveObj)
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
        public void PurchaseInsDetail( int p_PURCHASE_INS_MASTER_ID, string p_PAYMENT_DESC, int p_INST_NO, DateTime p_DUE_DATE, decimal p_DUE_AMOUNT
            , decimal p_PAY_AMOUNT, decimal p_OS_AMOUNT, string p_RECEIPT_NO, decimal p_RECEIPT_AMOUNT, DateTime p_PAYMENT_DATE
            , int p_USER_ID, DateTime p_TIME_STEMP, bool p_IS_PAID)
        {
            try
            {
                tbl_PURCHASE_INS_DETAIL P = new tbl_PURCHASE_INS_DETAIL();
                P.PURCHASE_INS_MASTER_ID = p_PURCHASE_INS_MASTER_ID;
                P.PAYMENT_DESC = p_PAYMENT_DESC;
                P.INST_NO = p_INST_NO;
                P.DUE_DATE = p_DUE_DATE;
                P.DUE_AMOUNT = p_DUE_AMOUNT;
                P.PAY_AMOUNT = p_PAY_AMOUNT;
                P.OS_AMOUNT = p_OS_AMOUNT;
                P.RECEIPT_NO = p_RECEIPT_NO;
                P.RECEIPT_AMOUNT = p_RECEIPT_AMOUNT;
                P.PAYMENT_DATE = p_PAYMENT_DATE;
                P.USER_ID = p_USER_ID;
                P.TIME_STEMP = p_TIME_STEMP;
                P.IS_PAID = p_IS_PAID;
                db.tbl_PURCHASE_INS_DETAIL.Add(P);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
