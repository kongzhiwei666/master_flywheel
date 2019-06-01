using System;
using System.Data;
using Business.CommonBusiness;

namespace Facade.CommonFacade
{
    /// <summary>
    /// Summary description for FacSystemData.
    /// </summary>
    public class FacSystemData
    {
        public FacSystemData() { }
        public DataSet GetCode(string Code, string Bourse, string tablenametag)
        {
            //Business.CommonBusiness.BusSystemData bts = new BusSystemData() ;
            //DataSet ds = bts.GetDataTable(Code,Bourse,tablenametag);
            DataSet ds = null;
            return ds;
        }
        /// <summary>
        /// GetCode
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Bourse"></param>
        /// <param name="tablenametag"></param>
        /// <param name="tablename">数据表结构</param>
        /// <returns></returns>
        public DataSet GetCode(string Code, string Bourse, string tablenametag, string tablename)
        {
            //			Business.CommonBusiness.BusSystemData bts = new BusSystemData() ;
            //			DataSet ds = bts.GetDataTable(Code,Bourse,tablenametag);
            DataSet ds = null;
            return ds;
        }

    }
}
