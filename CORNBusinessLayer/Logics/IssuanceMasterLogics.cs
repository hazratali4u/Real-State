using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORNBusinessLayer.Logics
{
    public class IssuanceMasterLogics
    {
        DbCalls db = new DbCalls();
        public List<Classes.tbl_ISSUANCE_MASTER> Get()
        {
            var DbObj = db.tbl_ISSUANCE_MASTER.ToList();
            return DbObj;
        }
        public Classes.tbl_ISSUANCE_MASTER Get(int ID)
        {
            var DbObj = db.tbl_ISSUANCE_MASTER.Find(ID);
            return DbObj;
        }
        public long Save(Classes.tbl_ISSUANCE_MASTER SaveObj)
        {
            try
            {
                db.tbl_ISSUANCE_MASTER.Add(SaveObj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return SaveObj.ISSUANCE_MASTER_ID;
        }
        public bool Update(Classes.tbl_ISSUANCE_MASTER SaveObj)
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
