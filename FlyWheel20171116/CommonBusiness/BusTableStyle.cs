using System;
using System.Data;

using Framework.Data;
using Framework;

namespace Business.CommonBusiness
{
    /// <summary>
    /// Class1 的摘要说明。
    /// </summary>
    public class BusTableStyle
    {
        public BusTableStyle() { }


        public static string Motor1_Speed;
        public static string Motor1_Curre;

        public static string Motor2_Speed;
        public static string Motor2_Curre;
        
        public static string Motor3_Speed;
        public static string Motor3_Curre;

        public static string Motor4_Speed;
        public static string Motor4_Curre;

        public static string Motor5_Speed;
        public static string Motor5_Curre;

        public static string Motor6_Speed;
        public static string Motor6_Curre;

        public static string Motor7_Speed;
        public static string Motor7_Curre;

        public static string Motor8_Speed;
        public static string Motor8_Curre;

        /// <summary>
        /// 读Can1数据
        /// </summary>
        /// <param name="msgRead"></param>
        /// <param name="nReadCount"></param>
        /// <param name="pulNumberofRead"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadCan1RecogInfor(out AdvCan.canmsg_t[] msgRead, uint nReadCount, out uint pulNumberofRead, out string error)
        {
            error = string.Empty;
            pulNumberofRead = 0;
            msgRead = new AdvCan.canmsg_t[1];
            msgRead[0].data = new byte[8];

            int nRet;

            bool success1 = true;
            nRet = Defs.Device1.acCanRead(msgRead, nReadCount, ref pulNumberofRead);
            if (nRet == AdvCANIO.TIME_OUT)
            {
                error = "receiving timeout!";
                success1 = false;

            }
            else if (nRet == AdvCANIO.OPERATION_ERROR)
            {
                error = " receiving error!";
                success1 = false;
            }
            else
            {

                if (msgRead[0].id == AdvCan.ERRORID)
                {
                    error = "is a incorrect package";
                    success1 = false;
                }
            }
           
            return success1;

        }


        /// <summary>
        /// 读Can2数据
        /// </summary>
        /// <param name="msgRead"></param>
        /// <param name="nReadCount"></param>
        /// <param name="pulNumberofRead"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadCan2RecogInfor(out AdvCan.canmsg_t[] msgRead, uint nReadCount, out uint pulNumberofRead, out string error)
        {
            error = string.Empty;
            pulNumberofRead = 0;
            msgRead = new AdvCan.canmsg_t[1];
            msgRead[0].data = new byte[8];
            int nRet;
            bool success2 = true;

            nRet = Defs.Device2.acCanRead(msgRead, nReadCount, ref pulNumberofRead);
            if (nRet == AdvCANIO.TIME_OUT)
            {
                error = "receiving timeout!";
                success2 = false;

            }
            else if (nRet == AdvCANIO.OPERATION_ERROR)
            {
                error = " receiving error!";
                success2 = false;
            }
            else
            {

                if (msgRead[0].id == AdvCan.ERRORID)
                {
                    error = "is a incorrect package";
                    success2 = false;
                }
            }

            return success2;

        }


        /// <summary>
        /// can1 写数据
        /// </summary>
        /// <param name="msgWrite"></param>
        /// <param name="nWriteCount"></param>
        /// <param name="pulNumberofWritten"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteCan1RecogInfor(AdvCan.canmsg_t[] msgWrite, uint nWriteCount, out uint pulNumberofWritten, out string error)
        {
            error = string.Empty;
            pulNumberofWritten = 0;

            bool success1 = true;
            int nRet = Defs.Device1.acCanWrite(msgWrite, nWriteCount, ref pulNumberofWritten);

            if (nRet == AdvCANIO.TIME_OUT)
            {
                error = " sending timeout!";
                success1 = false;
            }
            else if (nRet == AdvCANIO.OPERATION_ERROR)
            {
                error = " sending error!";
                success1 = false;
            }
            else
            {
                error = " sending ok!";
                success1 = false;
            }

           
            return success1;

        }


        /// <summary>
        /// can2 写数据
        /// </summary>
        /// <param name="msgWrite"></param>
        /// <param name="nWriteCount"></param>
        /// <param name="pulNumberofWritten"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteCan2RecogInfor(AdvCan.canmsg_t[] msgWrite, uint nWriteCount, out uint pulNumberofWritten, out string error)
        {
            error = string.Empty;
            pulNumberofWritten = 0;

            bool success2 = true;
            int nRet = Defs.Device2.acCanWrite(msgWrite, nWriteCount, ref pulNumberofWritten);

            if (nRet == AdvCANIO.TIME_OUT)
            {
                error = " sending timeout!";
                success2 = false;
            }
            else if (nRet == AdvCANIO.OPERATION_ERROR)
            {
                error = " sending error!";
                success2 = false;
            }
            else
            {
                error = " sending ok!";
                success2 = false;
            }


            return success2;

        }



        


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="scheme"></param>
        /// <param name="startttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataSet GetDataFromDatabase(string MotorID,string scheme, string startttime, string endtime, out string sInfo)
        {
            sInfo = string.Empty;
            Framework.Data.IDA ida = new Framework.Data.DASQLServer();

            string sql = string.Format("select * from  dbo.Motor{0}  where Scheme  = '{1}' and DateTime >= '{2}' and DateTime <= '{3}'",
                MotorID,scheme, startttime, endtime);

            DataSet ds = new DataSet();
            string tableName = string.Format("dbo.Motor{0}", MotorID);
            ds = ida.GetData(tableName, sql, out sInfo);
            return ds;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="scheme"></param>
        /// <param name="startttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataSet GetDataFromDatabaseAuto(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            sInfo = string.Empty;
            Framework.Data.IDA ida = new Framework.Data.DASQLServer();

            
            string sql = string.Format("select * from  dbo.Motor{0}  where Scheme  = '{1}' and backup1 = '{2}' and backup2 = '{3}' and backup3 = '{4}'",
                TableID,scheme, name, memeber, product);

            DataSet ds = new DataSet();
            string tableName = string.Format("dbo.Motor{0}", TableID);
            ds = ida.GetData(tableName, sql, out sInfo);
            return ds;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="scheme"></param>
        /// <param name="startttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataSet GetDataFromDatabaseAutoSlope(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            sInfo = string.Empty;
            Framework.Data.IDA ida = new Framework.Data.DASQLServer();


            string sql = string.Format("select * from  dbo.Motor{0}  where Scheme  = '{1}' and backup2 = '{2}' and backup3 = '{3}' and backup4 = '{4}'",
                TableID, scheme, name, memeber, product);

            DataSet ds = new DataSet();
            string tableName = string.Format("dbo.Motor{0}", TableID);
            ds = ida.GetData(tableName, sql, out sInfo);
            return ds;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="scheme"></param>
        /// <param name="startttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public DataSet GetDataFromDatabaseAutoTorque(string TableID, string scheme, string name, string memeber, string product, out string sInfo)
        {
            sInfo = string.Empty;
            Framework.Data.IDA ida = new Framework.Data.DASQLServer();


            string sql = string.Format("select * from  dbo.Motor{0}  where Scheme  = '{1}' and backup2 = '{2}' and backup3 = '{3}' and backup4 = '{4}'",
                TableID, scheme, name, memeber, product);

            DataSet ds = new DataSet();
            string tableName = string.Format("dbo.Motor{0}", TableID);
            ds = ida.GetData(tableName, sql, out sInfo);
            return ds;
        }
    }
}
