using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class ProjectLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.PROJECT> Get()
        {
            var DbObj = db.PROJECTs.ToList();
            return DbObj;
        }
        public Classes.PROJECT Get(int ID)
        {
            var DbObj = db.PROJECTs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.PROJECT SaveObj)
        {
            try
            {
                db.PROJECTs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
           
            return SaveObj.PROJECT_ID;
        }
        public bool Update(Classes.PROJECT SaveObj)
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


