using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Item_Log_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_ITEM_LOG> Get()
        {
            var DbObj = db.tbl_ITEM_LOG.ToList();
            return DbObj;
        }
        public Classes.tbl_ITEM_LOG Get(long ID)
        {
            var DbObj = db.tbl_ITEM_LOG.Find(ID);
            return DbObj;
        }
        public long Save(Classes.tbl_ITEM_LOG SaveObj)
        {
            try
            {
                db.tbl_ITEM_LOG.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.ITEM_LOG_ID;
        }
        public bool Update(Classes.tbl_ITEM_LOG SaveObj)
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
