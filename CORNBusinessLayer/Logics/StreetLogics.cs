using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class StreetLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.STREET> Get()
        {
            var DbObj = db.STREETs.ToList();
            return DbObj;
        }
        public Classes.STREET Get(int ID)
        {
            var DbObj = db.STREETs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.STREET SaveObj)
        {
            try
            {
                db.STREETs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.STREET_ID;
        }
        public bool Update(Classes.STREET SaveObj)
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
