using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class PrecentStreet_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.PRECENTSTREET> Get()
        {
            var DbObj = db.PRECENTSTREETs.ToList();
            return DbObj;
        }
        public Classes.PRECENTSTREET Get(int ID)
        {
            var DbObj = db.PRECENTSTREETs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.PRECENTSTREET SaveObj)
        {
            try
            {
                db.PRECENTSTREETs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PRECENTSTREET_ID;
        }
        public bool Update(Classes.PRECENTSTREET SaveObj)
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
