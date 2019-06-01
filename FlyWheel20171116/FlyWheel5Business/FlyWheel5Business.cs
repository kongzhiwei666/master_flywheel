using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Framework;
using System.Threading;

namespace Business.FlyWheel5Business
{
    public class FlyWheel5Business
    {
        Framework.Data.IDA ida = null;

        private static string axis_IDnumber = string.Empty;
        private static double[] VolFilter = new double[500];
        private static int FilterNumber = 0;

        public FlyWheel5Business()
        {
            try
            {
                string sCon = System.Configuration.ConfigurationSettings.AppSettings["ConnectString"];
                ida = new Framework.Data.DASQLServer(sCon);

                //电机ID
                string AxisID = System.Configuration.ConfigurationSettings.AppSettings["Motor5AxisIDString"];
                string[] axisPara = AxisID.Split('=');
                axis_IDnumber = axisPara[1].Trim();
                Defs.Motor5ID = axis_IDnumber;
            }
            catch (Exception ex)
            {
                string issue = ex.Message.ToString();
                return;
            }



        }


        /// <summary>
        /// 打开或关闭飞轮1电源
        /// </summary>
        /// <returns></returns>
        public bool OpenElecSource(string state, out string error, string experiment_name,
            string experiment_memeber, string experiment_product)
        {
            bool results = true;
            error = string.Empty;

            if (state == "ON")
            {
                #region 数据采集卡

                Defs.ModifyState("5", '0');
                D2KDASK.D2K_DO_WritePort(0, D2KDASK.Channel_P1A, Convert.ToUInt32(Defs.StateVector, 2));
                DateTime dt = DateTime.Now;

                #region 采集卡获得浪涌电流
                ushort[] chans = new ushort[1];
                chans[0] = 1;
                ushort[] ranges = new ushort[1];
                ushort[] chan_data = new ushort[1];
                double temp = 0.0;
                double[] MotorCur = new double[1000]; //浪涌电流
                TimeSpan span;
                int length_wave = 0;

                DateTime[] dttime = new DateTime[1000];

                ranges[0] = D2KDASK.AD_B_10_V | D2KDASK.AI_DIFF;

                short ret1 = D2KDASK.D2K_AI_MuxScanSetup(0, 1, chans, ranges);
                if (ret1 < 0)
                {
                    error = "Wave current error!";
                    Defs.ModifyState("5", '1');
                    D2KDASK.D2K_DO_WritePort(0, D2KDASK.Channel_P1A, Convert.ToUInt32(Defs.StateVector, 2));
                    return false;
                }


                while (true)
                {
                    D2KDASK.D2K_AI_ReadMuxScan(0, chan_data);
                    D2KDASK.D2K_AI_VoltScale(0, D2KDASK.AD_B_10_V, (short)chan_data[0], out temp);
                    MotorCur[length_wave] = Math.Round(temp, 3);
                    if (length_wave == 1000)
                    {
                        break;
                    }
                    dttime[length_wave] = DateTime.Now;
                    length_wave = length_wave + 1;
                    span = DateTime.Now - dt;
                    if (span.TotalMilliseconds > 10)
                    {
                        break;
                    }
                }
                for (int i = 0; i < length_wave; i++)
                {
                    WriteDataToDatabaseWave("5", "7", MotorCur[i].ToString(), dttime[i].ToString(), experiment_name, experiment_memeber, experiment_product);
                }
                #endregion


                #endregion


              
                #region can线程启动
                if (Defs.Can1ReadStatus == false)
                {
                    Defs.Can1ReadStatus = true;
                    Defs.Thread1.Start();

                }
                if (Defs.Can2ReadStatus == false)
                {
                    Defs.Can2ReadStatus = true;
                    Defs.Thread2.Start();
                }
                #endregion

            }
            else
            {
                Defs.ModifyState("5", '1');
                D2KDASK.D2K_DO_WritePort(0, D2KDASK.Channel_P1A, Convert.ToUInt32(Defs.StateVector, 2));
                Defs.Motor5RSuceess = false;
            }
            return results;
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

            return success2;

        }



        /// <summary>
        /// 十进制转十六进制补码-转速
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public byte[] TransTentoHex(string direction, string rotorspeed)
        {

            int tem_speed = (int)Math.Round(Convert.ToDouble(rotorspeed) / 0.2);
            byte[] speed = new byte[3];

            if (direction == "正向")
            {
                string tem_speed1 = tem_speed.ToString("X6");
                speed[0] = Convert.ToByte(tem_speed1.Substring(0, 2), 16);
                speed[1] = Convert.ToByte(tem_speed1.Substring(2, 2), 16);
                speed[2] = Convert.ToByte(tem_speed1.Substring(4, 2), 16);
            }
            else
            {

                string binary_speed = Convert.ToString(tem_speed, 2);

                string binary_full_speed = binary_speed.PadLeft(24, '0');
                string S = string.Empty;
                int[] Data = new int[binary_full_speed.Length];
                for (int i = 0; i < binary_full_speed.Length; i++)
                {
                    Data[i] = Convert.ToInt32(binary_full_speed.Substring(i, 1));

                    if (Data[i] == 0)
                    {
                        Data[i] = 1;
                    }
                    else
                    {
                        Data[i] = 0;
                    }
                    S = S + Convert.ToString(Data[i]);
                }


                string b = Convert.ToString(Convert.ToInt32(S, 2) + 1, 2);



                speed[0] = Convert.ToByte(b.Substring(0, 8), 2);
                speed[1] = Convert.ToByte(b.Substring(8, 8), 2);
                speed[2] = Convert.ToByte(b.Substring(16, 8), 2);
            }
            return speed;
        }

        /// <summary>
        /// 计算验证码
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private byte computeSNP(byte[] bs)
        {
            int snp = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                snp = snp + bs[i];

            }
            snp = ~snp + 1;
            return (byte)(snp & 0xFF);
        }

        /// <summary>
        /// 恒速控制
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool WriteCardSpeedControl(string direction, string rotorspeed, out string error)
        {

            bool results = true;
            error = string.Empty;
            byte[] speed = new byte[3];
            speed = TransTentoHex(direction, rotorspeed);


            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[2];
            uint pulNumberofWritten;

            msgWrite[0].flags = AdvCan.MSG_EXT;
            msgWrite[0].cob = 0;
            string idz = "19" + "0" + axis_IDnumber + "F0" + "00";
            msgWrite[0].id = Convert.ToUInt32(idz, 16);
            msgWrite[0].length = 8;
            msgWrite[0].data = new byte[8];
            msgWrite[0].data[0] = Convert.ToByte("20", 16);
            msgWrite[0].data[1] = Convert.ToByte("33", 16);
            msgWrite[0].data[2] = Convert.ToByte("1F", 16);
            msgWrite[0].data[3] = Convert.ToByte("F0", 16);
            msgWrite[0].data[4] = Convert.ToByte("0" + axis_IDnumber, 16);
            msgWrite[0].data[5] = Convert.ToByte("FF", 16);
            msgWrite[0].data[6] = Convert.ToByte("FF", 16);
            msgWrite[0].data[7] = Convert.ToByte("00", 16);
            msgWrite[0].data[7] = computeSNP(msgWrite[0].data);

            msgWrite[1].flags = AdvCan.MSG_EXT;
            msgWrite[1].cob = 0;
            string idw = "19" + "0" + axis_IDnumber + "F0" + "FF";
            msgWrite[1].id = Convert.ToUInt32(idw, 16);
            msgWrite[1].length = 5;
            msgWrite[1].data = new byte[8];
            msgWrite[1].data[0] = Convert.ToByte("A5", 16);
            msgWrite[1].data[1] = Convert.ToByte("01", 16);
            msgWrite[1].data[2] = speed[0];
            msgWrite[1].data[3] = speed[1];
            msgWrite[1].data[4] = speed[2];

            bool WritE1;
            bool WritE2;
            lock (Defs._object1)
            {
                WritE1 = WriteCan1RecogInfor(msgWrite, 2, out pulNumberofWritten, out error);
            }

            lock (Defs._object2)
            {
                WritE2 = WriteCan2RecogInfor(msgWrite, 2, out pulNumberofWritten, out error);

            }
            if (WritE1 == false && WritE2 == false)
            {
                error = "恒速写错误";
                return false;

            }
            


            return results;
        }

        /// <summary>
        /// 力矩转码
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="torque"></param>
        /// <returns></returns>
        public byte[] TransTentoHexTorque(string direction, string torque)
        {

            int tem_speed = (int)Math.Round(Convert.ToDouble(torque) / 0.0572);
            byte[] speed = new byte[3];

            if (direction == "正向")
            {
                string tem_speed1 = tem_speed.ToString("X6");
                speed[0] = Convert.ToByte(tem_speed1.Substring(0, 2), 16);
                speed[1] = Convert.ToByte(tem_speed1.Substring(2, 2), 16);
                speed[2] = Convert.ToByte(tem_speed1.Substring(4, 2), 16);
            }
            else
            {

                string binary_speed = Convert.ToString(tem_speed, 2);

                string binary_full_speed = binary_speed.PadLeft(24, '0');
                string S = string.Empty;
                int[] Data = new int[binary_full_speed.Length];
                for (int i = 0; i < binary_full_speed.Length; i++)
                {
                    Data[i] = Convert.ToInt32(binary_full_speed.Substring(i, 1));

                    if (Data[i] == 0)
                    {
                        Data[i] = 1;
                    }
                    else
                    {
                        Data[i] = 0;
                    }
                    S = S + Convert.ToString(Data[i]);
                }


                string b = Convert.ToString(Convert.ToInt32(S, 2) + 1, 2);


                speed[0] = Convert.ToByte(b.Substring(0, 8), 2);
                speed[1] = Convert.ToByte(b.Substring(8, 8), 2);
                speed[2] = Convert.ToByte(b.Substring(16, 8), 2);
            }
            return speed;
        }

        /// <summary>
        /// 力矩模式
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="rotorspeed"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteCardTorqueControl(string direction, string torque, out string error)
        {

            bool results = true;
            error = string.Empty;
            byte[] torques = new byte[3];
            torques = TransTentoHexTorque(direction, torque);


            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[2];
            uint pulNumberofWritten;

            msgWrite[0].flags = AdvCan.MSG_EXT;
            msgWrite[0].cob = 0;
            string idz = "19" + "0" + axis_IDnumber + "F0" + "00";
            msgWrite[0].id = Convert.ToUInt32(idz, 16);
            msgWrite[0].length = 8;
            msgWrite[0].data = new byte[8];
            msgWrite[0].data[0] = Convert.ToByte("20", 16);
            msgWrite[0].data[1] = Convert.ToByte("33", 16);
            msgWrite[0].data[2] = Convert.ToByte("1F", 16);
            msgWrite[0].data[3] = Convert.ToByte("F0", 16);
            msgWrite[0].data[4] = Convert.ToByte("0" + axis_IDnumber, 16);
            msgWrite[0].data[5] = Convert.ToByte("FF", 16);
            msgWrite[0].data[6] = Convert.ToByte("FF", 16);
            msgWrite[0].data[7] = Convert.ToByte("00", 16);
            msgWrite[0].data[7] = computeSNP(msgWrite[0].data);

            msgWrite[1].flags = AdvCan.MSG_EXT;
            msgWrite[1].cob = 0;
            string idw = "19" + "0" + axis_IDnumber + "F0" + "FF";
            msgWrite[1].id = Convert.ToUInt32(idw, 16);
            msgWrite[1].length = 5;
            msgWrite[1].data = new byte[8];
            msgWrite[1].data[0] = Convert.ToByte("A5", 16);
            msgWrite[1].data[1] = Convert.ToByte("1C", 16);
            msgWrite[1].data[2] = torques[0];
            msgWrite[1].data[3] = torques[1];
            msgWrite[1].data[4] = torques[2];

            bool WritE1;
            bool WritE2;
            lock (Defs._object1)
            {
                WritE1 = WriteCan1RecogInfor(msgWrite, 2, out pulNumberofWritten, out error);
            }
            lock (Defs._object2)
            {
                WritE2 = WriteCan2RecogInfor(msgWrite, 2, out pulNumberofWritten, out error);
            }
            if (WritE1 == false && WritE2 == false)
            {
                error = "力矩写错误";
                return false;

            }
            

            return results;
        }



        /// <summary>
        /// 获取can1信息
        /// </summary>
        /// <returns></returns>
        public bool GetDataFromCardcan(out string speed, out string current, out string sInfo)
        {
            sInfo = string.Empty;
            speed = "0.00";
            current = "0.00";
            bool results = false;


            #region 数据请求
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[2];
            uint pulNumberofWritten;

            msgWrite[0].flags = AdvCan.MSG_EXT;
            msgWrite[0].cob = 0;
            string idz = "19" + "0" + axis_IDnumber + "F0" + "00";
            msgWrite[0].id = Convert.ToUInt32(idz, 16);
            msgWrite[0].length = 8;
            msgWrite[0].data = new byte[8];
            msgWrite[0].data[0] = Convert.ToByte("00100000", 2);
            msgWrite[0].data[1] = Convert.ToByte("01000011", 2);
            msgWrite[0].data[2] = Convert.ToByte("00011111", 2);
            msgWrite[0].data[3] = Convert.ToByte("F0", 16);
            msgWrite[0].data[4] = Convert.ToByte("0" + axis_IDnumber, 16);
            msgWrite[0].data[5] = Convert.ToByte("FF", 16);
            msgWrite[0].data[6] = Convert.ToByte("FF", 16);
            msgWrite[0].data[7] = Convert.ToByte("00", 16);
            msgWrite[0].data[7] = computeSNP(msgWrite[0].data);


            msgWrite[1].flags = AdvCan.MSG_EXT;
            msgWrite[1].cob = 0;
            string idw = "19" + "0" + axis_IDnumber + "F0" + "FF";
            msgWrite[1].id = Convert.ToUInt32(idw, 16);
            msgWrite[1].length = 8;
            msgWrite[1].data = new byte[8];
            msgWrite[1].data[0] = Convert.ToByte("3C", 16);
            msgWrite[1].data[1] = Convert.ToByte("AA", 16);
            msgWrite[1].data[2] = Convert.ToByte("01", 16);
            msgWrite[1].data[3] = Convert.ToByte("01", 16);
            msgWrite[1].data[4] = Convert.ToByte("55", 16);
            msgWrite[1].data[5] = Convert.ToByte("55", 16);
            msgWrite[1].data[6] = Convert.ToByte("55", 16);
            msgWrite[1].data[7] = Convert.ToByte("55", 16);

            bool WritE1;
            bool WritE2;

            lock (Defs._object1)
            {
                WritE1 = WriteCan1RecogInfor(msgWrite, 2, out pulNumberofWritten, out sInfo);
            }
            lock (Defs._object2)
            {
                WritE2 = WriteCan2RecogInfor(msgWrite, 2, out pulNumberofWritten, out sInfo);
            }
            if (WritE1 == true || WritE2 == true)
            {
                Defs.rwl.AcquireReaderLock(10);
                try
                {
                    speed = Defs.speed[4];
                    current = Defs.current[4];

                }
                finally
                {
                    Defs.rwl.ReleaseReaderLock();
                }
                results = true;
            }
            #endregion



            return results;
        }


        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public string[] GetDataFromCard(out string sInfo)
        {
            sInfo = string.Empty;
            string[] data = new string[4] { "0.00", "0.00", "0.00", "0.00" };

            lock (Defs._object2)
            {
                //采集卡获得其它数据
                ushort[] chans = new ushort[2];
                chans[0] = 0; //电压
                chans[1] = 5; //电流
                ushort[] ranges = new ushort[2];

                ranges[0] = D2KDASK.AD_B_10_V | D2KDASK.AI_DIFF;
                ranges[1] = D2KDASK.AD_B_10_V | D2KDASK.AI_DIFF;

                short ret = D2KDASK.D2K_AI_MuxScanSetup(0, 2, chans, ranges);
                if (ret < 0)
                {
                    sInfo = "D2K_AI_MuxScanSetup error!";
                }

                ushort[] chan_data = new ushort[2];
                double temp;
                double DriverCur = 0.0;//驱动器电流
                double DriverVol = 0.0;//驱动器电压
                double MotorCur = 0.0; //电机电流
                double MotorVol = 0.0; //电机电压

                short err = D2KDASK.D2K_AI_ReadMuxScan(0, chan_data);
                if (err == 0)
                {
                    D2KDASK.D2K_AI_VoltScale(0, D2KDASK.AD_B_10_V, (short)chan_data[1], out temp);
                    DriverCur = Math.Round(temp, 3);

                    D2KDASK.D2K_AI_VoltScale(0, D2KDASK.AD_B_10_V, (short)chan_data[0], out temp);
                  //  DriverVol = Math.Round(temp * 10, 3);

                    if (FilterNumber < VolFilter.Length)
                    {
                        VolFilter[FilterNumber] = Math.Round(temp * 10, 3);
                        FilterNumber++;
                    }
                    else
                    {
                        for (int i = 0; i < VolFilter.Length - 1; i++)
                        {
                            VolFilter[i] = VolFilter[i + 1];
                        }
                        VolFilter[VolFilter.Length - 1] = Math.Round(temp * 10, 3);
                    }

                    double sumsum = 0;
                    for (int i = 0; i < FilterNumber; i++)
                    {
                        sumsum = sumsum + VolFilter[i];

                    }
                    DriverVol = sumsum / FilterNumber;
                }
                else
                {
                    sInfo = "结束";
                    return data;
                }

                //电机功率
                double MotorPow = Math.Round(DriverCur * DriverVol, 3);




                data[0] = DriverCur.ToString();
                data[1] = DriverVol.ToString();
                data[2] = MotorVol.ToString();
                data[3] = MotorPow.ToString();
            }
            return data;
        }



        /// <summary>
        /// 写恒速数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorDire"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabase(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
             string RotorPow, string ConstantMoment, string ChangeMoment, string RotorRevIde,
            string RotorRevRea, string datetime, string experiment_name, string experiment_memeber, string experiment_product, string real_deta_moment)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor51(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, ConstantMoment, ChangeMoment, RotorRevIde, RotorRevRea, DateTime, backup1,backup2,backup3,backup4)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}','{15}'", MotorID, Scheme, SouceVol,
                SouceCut, RotorVol, RotorCut, RotorPow, ConstantMoment, ChangeMoment, RotorRevIde, RotorRevRea, datetime, experiment_name, experiment_memeber, experiment_product, real_deta_moment);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);
            return sInfo;
        }

        /// <summary>
        /// 写斜坡数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorDire"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseSlope(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
    string RotorPow, string RotorRevRea, string MeanTorque, string datetime, string ideal_torque, string experiment_name, string experiment_memeber, string experiment_product)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor52(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime,backup1,backup2,backup3,backup4)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}','{11}','{12}','{13}'", MotorID, Scheme,
                SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime, ideal_torque, experiment_name, experiment_memeber, experiment_product);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);
            return sInfo;
        }

        /// <summary>
        /// 写浪涌数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorDire"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseWave(string MotorID, string Scheme, string RotorCut, string datetime, string experiment_name, string experiment_memeber, string experiment_product)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor56(MotorID, Scheme, RotorCut,datetime,backup1,backup2,backup3)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}'", MotorID, Scheme,
               RotorCut, datetime, experiment_name, experiment_memeber, experiment_product);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);
            return sInfo;
        }

        /// <summary>
        /// 写正弦数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="MeanTorque"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseSine(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
            string RotorPow, string RotorRevIde, string RotorRevRea, string datetime, string experiment_name, string experiment_memeber, string experiment_product,
            string real_deta_moment, string max_deta_moment, string max_deta_motion, string mean_torque, string ideal_torque)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor53(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevIde, RotorRevRea, datetime, backup1,backup2,backup3,backup4,backup5,backup6,backup7,backup8)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}'", MotorID,
                Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevIde, RotorRevRea, datetime, experiment_name, experiment_memeber, experiment_product,
                 real_deta_moment, max_deta_moment, max_deta_motion, mean_torque, ideal_torque);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);
            return sInfo;
        }

        /// <summary>
        /// 写力矩数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorDire"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseTorque(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
    string RotorPow, string RotorRevRea, string MeanTorque, string datetime, string ideal_torque, string experiment_name, string experiment_memeber, string experiment_product, string Scheme4_torque)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor54(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime,backup1,backup2,backup3,backup4,backup5)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}','{11}','{12}','{13}','{14}'", MotorID, Scheme,
                SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime, ideal_torque, experiment_name, experiment_memeber, experiment_product, Scheme4_torque);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);
            return sInfo;
        }


        /// <summary>
        /// 写时间常数数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorDire"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseTimeConstant(string MotorID, string Scheme, string TimeConstant, string datetime,
            string experiment_name, string experiment_memeber, string experiment_product)
        {
            string sInfo = string.Empty;
            string sql = "insert into dbo.Motor55(MotorID, Scheme, TimeConstant, datetime,backup1,backup2,backup3)";
            string data = string.Format("select '{0}', '{1}',  '{2}', '{3}', '{4}', '{5}', '{6}'", MotorID, Scheme, TimeConstant, datetime, experiment_name, experiment_memeber, experiment_product);

            sql = string.Format("{0} {1}", sql, data);
            ida.Execute(sql, out sInfo);

            return sInfo;
        }

    }
}
