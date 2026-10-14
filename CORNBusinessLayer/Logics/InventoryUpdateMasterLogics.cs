using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
   public class InventoryUpdateMasterLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_INVENTORY_UPDATE_MASTER> Get()
        {
            var DbObj = db.tbl_INVENTORY_UPDATE_MASTER.ToList();
            return DbObj;
        }
        public Classes.tbl_INVENTORY_UPDATE_MASTER Get(int ID)
        {
            var DbObj = db.tbl_INVENTORY_UPDATE_MASTER.Find(ID);
            return DbObj;
        }
        public int Save(Classes.tbl_INVENTORY_UPDATE_MASTER SaveObj)
        {
            try
            {
                db.tbl_INVENTORY_UPDATE_MASTER.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.INVENTORY_UPDATE_MASTER_ID;
        }
        public bool Update(Classes.tbl_INVENTORY_UPDATE_MASTER SaveObj)
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
