using System;
using System.Data;

using Business.CommonBusiness;

namespace Facade.CommonFacade
{
    /// <summary>
    /// Class1 的摘要说明。
    /// </summary>
    public class FacTableStyle
    {
        public FacTableStyle() { }



        public DataSet GetDataFromDatabaseAutoSlope(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            BusTableStyle bts = new BusTableStyle();
            return bts.GetDataFromDatabaseAutoSlope(TableID, scheme, name, memeber, product, out sInfo);

        }
        public DataSet GetDataFromDatabaseAutoTorque(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            BusTableStyle bts = new BusTableStyle();
            return bts.GetDataFromDatabaseAutoTorque(TableID, scheme, name, memeber, product, out sInfo);

        }
        public DataSet GetDataFromDatabaseAuto(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            BusTableStyle bts = new BusTableStyle();
            return bts.GetDataFromDatabaseAuto(TableID, scheme, name, memeber, product, out sInfo);

        }
        public DataSet GetDataFromDatabase(string MotorID, string scheme, string startttime, string endtime, out string sInfo)
        {
            BusTableStyle bts = new BusTableStyle();
            return bts.GetDataFromDatabase(MotorID, scheme, startttime, endtime, out sInfo);

        }
    }
}
