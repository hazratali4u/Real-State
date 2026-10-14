using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Sector_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.SECTOR> Get()
        {
            var DbObj = db.SECTORs.ToList();
            return DbObj;
        }
        public Classes.SECTOR Get(int ID)
        {
            var DbObj = db.SECTORs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.SECTOR SaveObj)
        {
            try
            {
                db.SECTORs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.SECTOR_ID;
        }
        public bool Update(Classes.SECTOR SaveObj)
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
