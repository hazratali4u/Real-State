using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
    public class BlockLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.BLOCK> Get()
        {
            var DbObj = db.BLOCKs.ToList();
            return DbObj;
        }
        public Classes.BLOCK Get(int ID)
        {
            var DbObj = db.BLOCKs.Find(ID);
            return DbObj;
        }
        public int Save(Classes.BLOCK SaveObj)
        {
            try
            {
                db.BLOCKs.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.BLOCK_ID;
        }
        public bool Update(Classes.BLOCK SaveObj)
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
