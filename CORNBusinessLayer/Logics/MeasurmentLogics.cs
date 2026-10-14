using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class MeasurmentLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.MEASURMENT> Get()
        {
            var DbObj = db.MEASURMENTs.ToList();
            return DbObj;
        }
        public Classes.MEASURMENT Get(int ID)
        {
            var DbObj = db.MEASURMENTs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.MEASURMENT SaveObj)
        {
            try
            {
                db.MEASURMENTs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.MEASURMENT_ID;
        }
        public bool Update(Classes.MEASURMENT SaveObj)
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
