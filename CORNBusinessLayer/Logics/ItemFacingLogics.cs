using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class ItemFacingLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.ITEM_FACING> Get()
        {
            var DbObj = db.ITEM_FACING.ToList();
            return DbObj;
        }
        public Classes.ITEM_FACING Get(int ID)
        {
            var DbObj = db.ITEM_FACING.Find(ID);
            return DbObj;
        }
        public int Save(Classes.ITEM_FACING SaveObj)
        {
            try
            {
                db.ITEM_FACING.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.ITEM_FACING_ID;
        }
        public bool Update(Classes.ITEM_FACING SaveObj)
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
