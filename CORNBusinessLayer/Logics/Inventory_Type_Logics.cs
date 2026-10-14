using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class Inventory_Type_Logics
    {
        DbCalls db = new DbCalls();
        public List<Classes.INVENTORYTYPE> Get()
        {
            var DbObj = db.INVENTORYTYPEs.ToList();
            return DbObj;
        }
        public Classes.INVENTORYTYPE Get(int ID)
        {
            var DbObj = db.INVENTORYTYPEs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.INVENTORYTYPE SaveObj)
        {
            try
            {
                db.INVENTORYTYPEs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.INVENTORYTYPE_ID;
        }
        public bool Update(Classes.INVENTORYTYPE SaveObj)
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
