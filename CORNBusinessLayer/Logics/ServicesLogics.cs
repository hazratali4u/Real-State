using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class ServicesLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.SERVICE> Get()
        {
            var DbObj = db.SERVICES.ToList();
            return DbObj;
        }
        public Classes.SERVICE Get(int ID)
        {
            var DbObj = db.SERVICES.Find(ID);
            return DbObj;
        }
        public int Save(Classes.SERVICE SaveObj)
        {
            try
            {
                db.SERVICES.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.SERVICE_ID;
        }
        public bool Update(Classes.SERVICE SaveObj)
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
