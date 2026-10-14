using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class FacingTypeLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.FACING_TYPE> Get()
        {
            var DbObj = db.FACING_TYPE.ToList();
            return DbObj;
        }
        public Classes.FACING_TYPE Get(int ID)
        {
            var DbObj = db.FACING_TYPE.Find(ID);
            return DbObj;
        }
        public int Save(Classes.FACING_TYPE SaveObj)
        {
            try
            {
                db.FACING_TYPE.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.FACING_TYPE_ID;
        }
        public bool Update(Classes.FACING_TYPE SaveObj)
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
