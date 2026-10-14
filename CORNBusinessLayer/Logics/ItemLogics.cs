using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class ItemLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.ITEM_INFORMATION> Get()
        {
            var DbObj = db.ITEM_INFORMATION.ToList();
            return DbObj;
        }
        public Classes.ITEM_INFORMATION Get(int ID)
        {
            var DbObj = db.ITEM_INFORMATION.Find(ID);
            return DbObj;
        }
        public int Save(Classes.ITEM_INFORMATION SaveObj)
        {
            try
            {
                db.ITEM_INFORMATION.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.ITEM_INFO_ID;
        }
        public bool Update(Classes.ITEM_INFORMATION SaveObj)
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
