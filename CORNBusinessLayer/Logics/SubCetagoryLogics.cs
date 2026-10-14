using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class SubCetagoryLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.SUB_CATEGORY> Get()
        {
            var DbObj = db.SUB_CATEGORY.ToList();
            return DbObj;
        }
        public Classes.SUB_CATEGORY Get(int ID)
        {
            var DbObj = db.SUB_CATEGORY.Find(ID);
            return DbObj;
        }
        public int Save(Classes.SUB_CATEGORY SaveObj)
        {
            try
            {
                db.SUB_CATEGORY.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.SUB_CATEGORY_ID;
        }
        public bool Update(Classes.SUB_CATEGORY SaveObj)
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
