using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Phase_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.PHASE> Get()
        {
            var DbObj = db.PHASEs.ToList();
            return DbObj;
        }
        public Classes.PHASE Get(int ID)
        {
            var DbObj = db.PHASEs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.PHASE SaveObj)
        {
            try
            {
                db.PHASEs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.PHASE_ID;
        }
        public bool Update(Classes.PHASE SaveObj)
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
