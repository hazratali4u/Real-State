using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class DetailCategory_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.DETAIL_CATEGORY> Get()
        {
            var DbObj = db.DETAIL_CATEGORY.ToList();
            return DbObj;
        }
        public Classes.DETAIL_CATEGORY Get(int ID)
        {
            var DbObj = db.DETAIL_CATEGORY.Find(ID);
            return DbObj;
        }
        public int Save(Classes.DETAIL_CATEGORY SaveObj)
        {
            try
            {
                db.DETAIL_CATEGORY.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.DETAIL_CATEGORY_ID;
        }
        public bool Update(Classes.DETAIL_CATEGORY SaveObj)
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
