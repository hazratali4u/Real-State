using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class CustomerLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.VENDOR> Get()
        {
            var DbObj = db.VENDORS.ToList();
            return DbObj;
        }
        public Classes.VENDOR Get(int ID)
        {
            var DbObj = db.VENDORS.Find(ID);
            return DbObj;
        }
        public long Save(Classes.VENDOR SaveObj)
        {
            try
            {
                db.VENDORS.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.VENDOR_ID;
        }
        public bool Update(Classes.VENDOR SaveObj)
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
