using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
  public  class CategoryLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.CATEGORY> Get()
        {
            var DbObj = db.CATEGORies.ToList();
            return DbObj;
        }
        public Classes.CATEGORY Get(int ID)
        {
            var DbObj = db.CATEGORies.Find(ID);
            return DbObj;
        }
        public int Save(Classes.CATEGORY SaveObj)
        {
            try
            {
                db.CATEGORies.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.CATEGORY_ID;
        }
        public bool Update(Classes.CATEGORY SaveObj)
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
