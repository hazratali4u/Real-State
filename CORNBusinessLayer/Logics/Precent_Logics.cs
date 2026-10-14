using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Precent_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.PRECENT> Get()
        {
            var DbObj = db.PRECENTs.ToList();
            return DbObj;
        }
        public Classes.PRECENT Get(int ID)
        {
            var DbObj = db.PRECENTs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.PRECENT SaveObj)
        {
            try
            {
                db.PRECENTs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PRECENT_ID;
        }
        public bool Update(Classes.PRECENT SaveObj)
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
