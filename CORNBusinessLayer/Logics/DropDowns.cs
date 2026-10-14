using CORNBusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CORNCommon.Classes;

namespace CORNBusinessLayer.Logics
{
   public class DropDowns
    {
       
        public static List<Classes.PROJECT> PROJECTDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.PROJECTs.Where(x=>x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.BLOCK> BLOCKDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.BLOCKs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.STREET> STREETDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.STREETs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.CATEGORY> CATEGORYDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.CATEGORies.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.SUB_CATEGORY> SUB_CATEGORYDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.SUB_CATEGORY.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.MEASURMENT> MEASURMENTDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.MEASURMENTs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.BALANCE_TYPE> BALANCE_TYPEDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.BALANCE_TYPE.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<DISTRIBUTOR> DISTRIBUTORDropDown()
        {
            DbCalls db = new DbCalls();
            List<DISTRIBUTOR> list = new List<DISTRIBUTOR>();
            var Obj = (from k in db.DISTRIBUTORs
                       join d in db.DISTRIBUTOR_ASSIGNMENT on k.DISTRIBUTOR_ID equals d.DISTRIBUTOR_ID
                       select new {k.DISTRIBUTOR_ID,k.DISTRIBUTOR_NAME,k.ISDELETED,d.USER_ID }).Where(x => x.ISDELETED == false).ToList();
            foreach (var i in Obj)
            {
                list.Add(new DISTRIBUTOR
                {
                    DISTRIBUTOR_ID = i.DISTRIBUTOR_ID,
                    DISTRIBUTOR_NAME = i.DISTRIBUTOR_NAME,
                    ISDELETED = i.ISDELETED,
                    USER_ID = i.USER_ID
                });
            }
            return list;
        }
       public class DISTRIBUTOR
        {
            public int DISTRIBUTOR_ID { get; set; }
            public string DISTRIBUTOR_NAME { get; set; }
            public bool? ISDELETED { get; set; }
            public int USER_ID { get; set; }
        }
        public static List<Classes.VENDOR> VendorDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.VENDORS.Where(x => x.IS_DELETED == false && x.TYPE_ID == Constants.Vendor).ToList();
            return Obj;
        }
        public static List<Classes.VENDOR> CustomerDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.VENDORS.Where(x => x.IS_DELETED == false && x.TYPE_ID == Constants.Customer).ToList();
            return Obj;
        }
        public static List<Classes.VENDOR> CommissionAgentDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.VENDORS.Where(x => x.IS_DELETED == false && x.TYPE_ID == Constants.CommissionAgent).OrderBy(x=>x.VENDOR_NAME).ToList();
            return Obj;
        }
        public static List<Classes.ITEM_INFORMATION> ITEMDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.ITEM_INFORMATION.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        //public static List<Classes.ITEM_INFORMATION> Sale_ITEMDropDown(string Val)
        //{
        //    DbCalls db = new DbCalls();
        //    List<ITEM_INFORMATION> List = new List<ITEM_INFORMATION>();
        //    var Items = db.tbl_PURCHASE_MASTER.Select(x => x.ITEM_INFO_ID).ToArray();
        //    if (Val.Equals(""))
        //    {
                
        //        List = db.ITEM_INFORMATION.Where(x => Items.Contains(x.ITEM_INFO_ID)).ToList();
        //    }
        //    else
        //    {
        //        List = db.ITEM_INFORMATION.Where(x => !Items.Contains(x.ITEM_INFO_ID)).ToList();
        //    }
        //    return List;
        //}
        //public static List<Classes.ITEM_INFORMATION> Purchase_ITEMDropDown(string Val)
        //{
        //    DbCalls db = new DbCalls();
        //    List<ITEM_INFORMATION> List = new List<ITEM_INFORMATION>();
        //    var SaleItems = db.tbl_PURCHASE_MASTER.Select(x => x.ITEM_INFO_ID).ToArray();
        //    if (Val.Equals(""))
        //    {
                
        //        List = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
        //    }
        //    else
        //    {
        //        List = db.ITEM_INFORMATION.Where(x => SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
        //    }
        //    return List;
        //}
        public class Itm_Info {
             public int ITEM_INFO_ID { get; set; }
        public string ITEM_NAME { get; set; }
        }
       
        public static List<Classes.ITEM_INFORMATION> SALE_RETURN_ITEMDropDown()
        {
            DbCalls db = new DbCalls();

            var SaleItems = db.tbl_SALE_RETURN.Select(x => x.ITEM_INFO_ID).ToArray();
            var Items = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
            return Items;
        }
        //public static List<Classes.ITEM_INFORMATION> PURCHASE_ITEMDropDown()
        //{
        //    DbCalls db = new DbCalls();

        //    var SaleItems = db.tbl_PURCHASE_MASTER.Select(x => x.ITEM_INFO_ID).ToArray();
        //    var Items = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
        //    return Items;
        //}
        public static List<Classes.ITEM_INFORMATION> PURCHASE_ITEM_InstallmentDropDown(string Val)
        {
            DbCalls db = new DbCalls();
            List<ITEM_INFORMATION> List = new List<ITEM_INFORMATION>();
            if (Val.Equals(""))
            {
                var SaleItems = db.tbl_PURCHASE_INS_MASTER.Select(x => x.ITEM_INFO_ID).ToArray();
                List = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
            }
            else
            {
                List = db.ITEM_INFORMATION.Where(x => x.IS_DELETED == false).ToList();
            }
            return List;


        }
        public static List<Classes.ITEM_INFORMATION> SALE_ITEM_InstallmentDropDown(string Val)
        {
            DbCalls db = new DbCalls();
            List<ITEM_INFORMATION> List = new List<ITEM_INFORMATION>();
            var SaleItems = db.tbl_SALE_INS_MASTER.Select(x => x.ITEM_INFO_ID).ToArray();
            if (Val.Equals(""))
            {
                List = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
            }
            else
            {
                List = db.ITEM_INFORMATION.Where(x => SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
            }
            return List;


        }
        public static List<Classes.ITEM_INFORMATION> PURCHASE_RETURN_ITEMDropDown()
        {
            DbCalls db = new DbCalls();

            var SaleItems = db.tbl_PURCHASE_RETURN.Select(x => x.ITEM_INFO_ID).ToArray();
            var Items = db.ITEM_INFORMATION.Where(x => !SaleItems.Contains(x.ITEM_INFO_ID)).ToList();
            return Items;
        }
        public static List<SaleReturnCustomer> SaleReturnCustomerDropDown()
        {
            List<SaleReturnCustomer> List = new List<SaleReturnCustomer>();
            DbCalls db = new DbCalls();
            //var Obj = (from k in db.tbl_SALE_MASTER
            //          join v in db.VENDORS on k.VENDOR_ID equals v.VENDOR_ID
            //          select new {k.SALE_MASTER_ID, k.IS_DELETED,v.VENDOR_ID,v.VENDOR_NAME,v.TYPE_ID })
            //          .Where(x => x.IS_DELETED == false && x.TYPE_ID == Constants.Customer).ToList();
            //foreach (var item in Obj)
            //{
            //    SaleReturnCustomer SRC = new SaleReturnCustomer();
            //    SRC.SALE_MASTER_ID = item.SALE_MASTER_ID;
            //    SRC.IS_DELETED = item.IS_DELETED;
            //    SRC.VENDOR_ID = item.VENDOR_ID;
            //    SRC.VENDOR_NAME = item.VENDOR_NAME;
            //    List.Add(SRC);
            //}
            return List;
        }
      
        public class SaleReturnCustomer
        {
            public int SALE_MASTER_ID { get; set; }
            public bool IS_DELETED { get; set; }
            public long VENDOR_ID { get; set; }
            public string VENDOR_NAME { get; set; }
            public int TYPE_ID { get; set; }
        }

        public static List<Classes.TOWN> TOWNDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.TOWNs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.INVENTORYTYPE> INVENTORYTYPEDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.INVENTORYTYPEs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.VILLATYPE> VILLATYPEDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.VILLATYPEs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.PHASE> PHASEDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.PHASEs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.SECTOR> SECTORDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.SECTORs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.PRECENT> PRECENTDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.PRECENTs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.DETAIL_CATEGORY> DETAIL_CATEGORYDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.DETAIL_CATEGORY.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }
        public static List<Classes.PRECENTSTREET> PRECENTSTREETDropDown()
        {
            DbCalls db = new DbCalls();
            var Obj = db.PRECENTSTREETs.Where(x => x.IS_DELETED == false).ToList();
            return Obj;
        }



        //public static List<Classes.ITEM_INFORMATION> Purchase_RETURN_VendersDropDown()
        //{
        //    DbCalls db = new DbCalls();

        //    //var SaleItems = db.tbl_PURCHASE_RETURN.Select(x => x.VENDOR_ID).ToArray();
        //    //var Items = db.VENDORS.Where(x => SaleItems.Contains(x.VENDOR_ID)).ToList();
        //    //return Items;
        //}
    }
}
