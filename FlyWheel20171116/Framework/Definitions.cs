using System;
using System.Threading;

namespace Framework
{
    public class Defs
    {
        public static Thread Thread1 = new Thread(ThreadMethod1);
        public static Thread Thread2 = new Thread(ThreadMethod2);

        public static ReaderWriterLock rwl = new ReaderWriterLock();


       
        public static object _object1 = new object();
        public static object _object2 = new object();
        public static object _object_time = new object();

        public static AdvCANIO Device1 = new AdvCANIO();
        public static AdvCANIO Device2 = new AdvCANIO();

        public static string Motor1ID = string.Empty;
        public static string Motor2ID = string.Empty;
        public static string Motor3ID = string.Empty;
        public static string Motor4ID = string.Empty;
        public static string Motor5ID = string.Empty;
        public static string Motor6ID = string.Empty;
        public static string Motor7ID = string.Empty; 
        public static string Motor8ID = string.Empty;


        public static string[] speed   = new string[8] { "0", "0", "0", "0", "0", "0", "0", "0" };
        public static string[] current = new string[8] { "0", "0", "0", "0", "0", "0", "0", "0" };

        public static string speed0 = "0";
        public static string current0 = "0";



        public static bool Motor1value = false;

        public static bool Motor1RSuceess = false;
        public static bool Motor2RSuceess = false;
        public static bool Motor3RSuceess = false;
        public static bool Motor4RSuceess = false;
        public static bool Motor5RSuceess = false;
        public static bool Motor6RSuceess = false;
        public static bool Motor7RSuceess = false;
        public static bool Motor8RSuceess = false;


        public static bool Can1ReadStatus = false;
        public static bool Can2ReadStatus = false;


        public static bool brun2 = false;
        public static bool brun1 = false;


        public static string Motor1Num = string.Empty;
        public static string Motor2Num = string.Empty;
        public static string Motor3Num = string.Empty;
        public static string Motor4Num = string.Empty;
        public static string Motor5Num = string.Empty;
        public static string Motor6Num = string.Empty;
        public static string Motor7Num = string.Empty;
        public static string Motor8Num = string.Empty;
        /// <summary>
        /// 读can1
        /// </summary>
        private static void ThreadMethod1()
        {
            string sInfo = string.Empty;
            brun1 = true;

            while (brun1)
            {

                #region 数据反馈 can
                AdvCan.canmsg_t[] msgReadT;
                uint pulNumberofRead;
                bool Resan1 = false;
                AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[1];
                msgWrite[0].data = new byte[8];
                uint pulNumberofWritten;
                string IDRC;
                lock (Defs._object1)
                {
                    Resan1 = ReadCan1RecogInfor(out msgReadT, 1, out pulNumberofRead, out sInfo);
                }

                #region 识别电机
                IDRC = (msgReadT[0].id).ToString("X8");

                if (Resan1 == true && pulNumberofRead == 1 && IDRC.Substring(0, 2) == "01")
                {
                    if (Motor1RSuceess == false && StateVector.Substring(7, 1) == "0" && Motor1ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor1ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor1RSuceess = true;
                            Motor1Num = IDRC;
                        }
                    }
                    if (Motor1RSuceess == true && Motor1Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor1ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                    if (Motor2RSuceess == false && StateVector.Substring(6, 1) == "0" && Motor2ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor2ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor2RSuceess = true;
                            Motor2Num = IDRC;
                        }
                    }
                    if (Motor2RSuceess == true && Motor2Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor2ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                    if (Motor3RSuceess == false && StateVector.Substring(5, 1) == "0" && Motor3ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor3ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor3RSuceess = true;
                            Motor3Num = IDRC;
                        }
                    }
                    if (Motor3RSuceess == true && Motor3Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor3ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }


                    if (Motor4RSuceess == false && StateVector.Substring(4, 1) == "0" && Motor4ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor4ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor4RSuceess = true;
                            Motor4Num = IDRC;
                        }
                    }
                    if (Motor4RSuceess == true && Motor4Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor4ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor5RSuceess == false && StateVector.Substring(3, 1) == "0" && Motor5ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor5ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor5RSuceess = true;
                            Motor5Num = IDRC;
                        }
                    }
                    if (Motor5RSuceess == true && Motor5Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor5ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor6RSuceess == false && StateVector.Substring(2, 1) == "0" && Motor6ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor6ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor6RSuceess = true;
                            Motor6Num = IDRC;
                        }
                    }
                    if (Motor6RSuceess == true && Motor6Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor6ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor7RSuceess == false && StateVector.Substring(1, 1) == "0" && Motor7ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor7ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor7RSuceess = true;
                            Motor7Num = IDRC;
                        }
                    }
                    if (Motor7RSuceess == true && Motor7Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor7ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor8RSuceess == false && StateVector.Substring(0, 1) == "0" && Motor8ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor8ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor8RSuceess = true;
                            Motor8Num = IDRC;
                        }
                    }
                    if (Motor8RSuceess == true && Motor8Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor8ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                }
                #endregion



                #region 电机1
                string ID1H = "19" + "F0" + "0" + Motor1ID + "00";
                string ID1T = "19" + "F0" + "0" + Motor1ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID1T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try 
                    {
                    
                        speed[0] = motorspeed;
                        current[0] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }

                }

                #endregion

                #region 电机2
                string ID2H = "19" + "F0" + "0" + Motor2ID + "00";
                string ID2T = "19" + "F0" + "0" + Motor2ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID2T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[1] = motorspeed;
                        current[1] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }

                }

                #endregion

                #region 电机3
                string ID3H = "19" + "F0" + "0" + Motor3ID + "00";
                string ID3T = "19" + "F0" + "0" + Motor3ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID3T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[2] = motorspeed;
                        current[2] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion

                #region 电机4
                string ID4H = "19" + "F0" + "0" + Motor4ID + "00";
                string ID4T = "19" + "F0" + "0" + Motor4ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID4T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[3] = motorspeed;
                        current[3] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机5
                string ID5H = "19" + "F0" + "0" + Motor5ID + "00";
                string ID5T = "19" + "F0" + "0" + Motor5ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID5T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[4] = motorspeed;
                        current[4] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机6
                string ID6H = "19" + "F0" + "0" + Motor6ID + "00";
                string ID6T = "19" + "F0" + "0" + Motor6ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID6T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[5] = motorspeed;
                        current[5] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion

                #region 电机7
                string ID7H = "19" + "F0" + "0" + Motor7ID + "00";
                string ID7T = "19" + "F0" + "0" + Motor7ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID7T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[6] = motorspeed;
                        current[6] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机8
                string ID8H = "19" + "F0" + "0" + Motor8ID + "00";
                string ID8T = "19" + "F0" + "0" + Motor8ID + "FF";

                if (Resan1 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID8T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan1(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[7] = motorspeed;
                        current[7] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion



                #endregion
              

               
            }
        }
        /// <summary>
        /// 计算验证码
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private static byte computeSNP(byte[] bs)
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
        /// 获得can1数据
        /// </summary>
        /// <param name="msgReadT"></param>
        /// <param name="motorspeed"></param>
        /// <param name="motorcurrent"></param>
        private static void GetDataFromCan1(AdvCan.canmsg_t msgReadT, out string motorspeed,out string motorcurrent)
        {
            string Binary1 = Convert.ToString(msgReadT.data[0], 2);
            string Binary2 = Convert.ToString(msgReadT.data[1], 2);
            string Binary3 = Convert.ToString(msgReadT.data[2], 2);
            string Binary = Binary1.PadLeft(8, '0') + Binary2.PadLeft(8, '0') + Binary3.PadLeft(8, '0');

            if (Binary.Substring(0, 1) == "1")
            {

                Int32 tempp = Convert.ToInt32(Binary, 2) - 1;
                string Binary_temp = Convert.ToString(tempp, 2);

                string S = string.Empty;
                int[] Data = new int[Binary.Length];
                for (int i = 0; i < Binary.Length; i++)
                {
                    Data[i] = Convert.ToInt32(Binary.Substring(i, 1));

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

                double sp = Math.Round(Convert.ToInt32(S, 2) * (-0.2), 3);
                motorspeed = sp.ToString();
                
            }
            else
            {
                double sp = Math.Round(Convert.ToInt32(Binary, 2) * 0.2, 3);
                motorspeed = sp.ToString();
            }

            string Binary4 = Convert.ToString(msgReadT.data[3], 2);
            string Binary5 = Convert.ToString(msgReadT.data[4], 2);
            string Binary6 = Binary4.PadLeft(8, '0') + Binary5.PadLeft(8, '0');

            double sp1 = Math.Round(Convert.ToInt32(Binary6, 2) * 0.0022, 3);
            motorcurrent = sp1.ToString();
            

        }
       

        /// <summary>
        /// 读can2
        /// </summary>
        private static void ThreadMethod2()
        {
            string sInfo = string.Empty;
            brun2 = true;

            while (brun2)
            {

                #region 数据反馈 can
                AdvCan.canmsg_t[] msgReadT;
                uint pulNumberofRead;
                bool Resan2 = false;
                AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[1];
                msgWrite[0].data = new byte[8];
                uint pulNumberofWritten;
                string IDRC;
                lock (Defs._object2)
                {
                    Resan2 = ReadCan2RecogInfor(out msgReadT, 1, out pulNumberofRead, out sInfo);
                }
                #region 识别电机
                IDRC = (msgReadT[0].id).ToString("X8");

                if (Resan2 == true && pulNumberofRead == 1 && IDRC.Substring(0, 2) == "01")
                {
                    if (Motor1RSuceess == false && StateVector.Substring(7, 1) == "0" && Motor1ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor1ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor1RSuceess = true;
                            Motor1Num = IDRC;
                        }
                    }
                    if (Motor1RSuceess == true && Motor1Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor1ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                    if (Motor2RSuceess == false && StateVector.Substring(6, 1) == "0" && Motor2ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor2ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor2RSuceess = true;
                            Motor2Num = IDRC;
                        }
                    }
                    if (Motor2RSuceess == true && Motor2Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor2ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                    if (Motor3RSuceess == false && StateVector.Substring(5, 1) == "0" && Motor3ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor3ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor3RSuceess = true;
                            Motor3Num = IDRC;
                        }
                    }
                    if (Motor3RSuceess == true && Motor3Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor3ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }


                    if (Motor4RSuceess == false && StateVector.Substring(4, 1) == "0" && Motor4ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor4ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor4RSuceess = true;
                            Motor4Num = IDRC;
                        }
                    }
                    if (Motor4RSuceess == true && Motor4Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor4ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor5RSuceess == false && StateVector.Substring(3, 1) == "0" && Motor5ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor5ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor5RSuceess = true;
                            Motor5Num = IDRC;
                        }
                    }
                    if (Motor5RSuceess == true && Motor5Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor5ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor6RSuceess == false && StateVector.Substring(2, 1) == "0" && Motor6ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor6ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor6RSuceess = true;
                            Motor6Num = IDRC;
                        }
                    }
                    if (Motor6RSuceess == true && Motor6Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor6ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor7RSuceess == false && StateVector.Substring(1, 1) == "0" && Motor7ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor7ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor7RSuceess = true;
                            Motor7Num = IDRC;
                        }
                    }
                    if (Motor7RSuceess == true && Motor7Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor7ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }
                    if (Motor8RSuceess == false && StateVector.Substring(0, 1) == "0" && Motor8ID != string.Empty)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor8ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        if (WritE1 == true || WritE2 == true)
                        {
                            Motor8RSuceess = true;
                            Motor8Num = IDRC;
                        }
                    }
                    if (Motor8RSuceess == true && Motor8Num == IDRC)
                    {
                        // 发送识别允许帧
                        msgWrite[0].flags = AdvCan.MSG_EXT;
                        msgWrite[0].cob = 0;
                        msgWrite[0].id = Convert.ToUInt32("02" + IDRC.Substring(2, 6), 16);
                        msgWrite[0].length = 4;
                        msgWrite[0].data[0] = msgReadT[0].data[0];
                        msgWrite[0].data[1] = msgReadT[0].data[1];
                        msgWrite[0].data[2] = msgReadT[0].data[2];
                        msgWrite[0].data[3] = Convert.ToByte(Motor8ID);
                        bool WritE1 = WriteCan1RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                        bool WritE2 = WriteCan2RecogInfor(msgWrite, 1, out pulNumberofWritten, out sInfo);
                    }

                }
                #endregion



                #region 电机1
                string ID1H = "19" + "F0" + "0" + Motor1ID + "00";
                string ID1T = "19" + "F0" + "0" + Motor1ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID1T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[0] = motorspeed;
                        current[0] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }

                }

                #endregion

                #region 电机2
                string ID2H = "19" + "F0" + "0" + Motor2ID + "00";
                string ID2T = "19" + "F0" + "0" + Motor2ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID2T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[1] = motorspeed;
                        current[1] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }

                }

                #endregion

                #region 电机3
                string ID3H = "19" + "F0" + "0" + Motor3ID + "00";
                string ID3T = "19" + "F0" + "0" + Motor3ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID3T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[2] = motorspeed;
                        current[2] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion

                #region 电机4
                string ID4H = "19" + "F0" + "0" + Motor4ID + "00";
                string ID4T = "19" + "F0" + "0" + Motor4ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID4T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[3] = motorspeed;
                        current[3] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机5
                string ID5H = "19" + "F0" + "0" + Motor5ID + "00";
                string ID5T = "19" + "F0" + "0" + Motor5ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID5T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[4] = motorspeed;
                        current[4] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机6
                string ID6H = "19" + "F0" + "0" + Motor6ID + "00";
                string ID6T = "19" + "F0" + "0" + Motor6ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID6T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[5] = motorspeed;
                        current[5] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion

                #region 电机7
                string ID7H = "19" + "F0" + "0" + Motor7ID + "00";
                string ID7T = "19" + "F0" + "0" + Motor7ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID7T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[6] = motorspeed;
                        current[6] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion


                #region 电机8
                string ID8H = "19" + "F0" + "0" + Motor8ID + "00";
                string ID8T = "19" + "F0" + "0" + Motor8ID + "FF";

                if (Resan2 == true && pulNumberofRead == 1 && msgReadT[0].id.ToString() == Convert.ToUInt32(ID8T, 16).ToString())
                {
                    string motorspeed = string.Empty;
                    string motorcurrent = string.Empty;
                    GetDataFromCan2(msgReadT[0], out motorspeed, out motorcurrent);
                    rwl.AcquireWriterLock(100);
                    try
                    {

                        speed[7] = motorspeed;
                        current[7] = motorcurrent;
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }

                #endregion



                #endregion
            }
        }

        /// <summary>
        /// 获得can2数据
        /// </summary>
        /// <param name="msgReadT"></param>
        /// <param name="motorspeed"></param>
        /// <param name="motorcurrent"></param>
        private static void GetDataFromCan2(AdvCan.canmsg_t msgReadT, out string motorspeed, out string motorcurrent)
        {
            string Binary1 = Convert.ToString(msgReadT.data[0], 2);
            string Binary2 = Convert.ToString(msgReadT.data[1], 2);
            string Binary3 = Convert.ToString(msgReadT.data[2], 2);
            string Binary = Binary1.PadLeft(8, '0') + Binary2.PadLeft(8, '0') + Binary3.PadLeft(8, '0');

            if (Binary.Substring(0, 1) == "1")
            {

                Int32 tempp = Convert.ToInt32(Binary, 2) - 1;
                string Binary_temp = Convert.ToString(tempp, 2);

                string S = string.Empty;
                int[] Data = new int[Binary.Length];
                for (int i = 0; i < Binary.Length; i++)
                {
                    Data[i] = Convert.ToInt32(Binary.Substring(i, 1));

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

                double sp = Math.Round(Convert.ToInt32(S, 2) * (-0.2), 3);
                motorspeed = sp.ToString();

            }
            else
            {
                double sp = Math.Round(Convert.ToInt32(Binary, 2) * 0.2, 3);
                motorspeed = sp.ToString();
            }

            string Binary4 = Convert.ToString(msgReadT.data[3], 2);
            string Binary5 = Convert.ToString(msgReadT.data[4], 2);
            string Binary6 = Binary4.PadLeft(8, '0') + Binary5.PadLeft(8, '0');

            double sp1 = Math.Round(Convert.ToInt32(Binary6, 2) * 0.0022, 3);
            motorcurrent = sp1.ToString();


        }

        /// <summary>
        /// 读Can1数据
        /// </summary>
        /// <param name="msgRead"></param>
        /// <param name="nReadCount"></param>
        /// <param name="pulNumberofRead"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ReadCan1RecogInfor(out AdvCan.canmsg_t[] msgRead, uint nReadCount, out uint pulNumberofRead, out string error)
        {
            error = string.Empty;
            pulNumberofRead = 0;
            msgRead = new AdvCan.canmsg_t[nReadCount];

            for (int i = 0; i < nReadCount; i++)
            {
                msgRead[i].data = new byte[8];
            }
            
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
        public static bool ReadCan2RecogInfor(out AdvCan.canmsg_t[] msgRead, uint nReadCount, out uint pulNumberofRead, out string error)
        {
            error = string.Empty;
            pulNumberofRead = 0;
            msgRead = new AdvCan.canmsg_t[nReadCount];
            for (int i = 0; i < nReadCount; i++)
            {
                msgRead[i].data = new byte[8];
            }
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
        public static bool WriteCan1RecogInfor(AdvCan.canmsg_t[] msgWrite, uint nWriteCount, out uint pulNumberofWritten, out string error)
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
        public static bool WriteCan2RecogInfor(AdvCan.canmsg_t[] msgWrite, uint nWriteCount, out uint pulNumberofWritten, out string error)
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



        #region 状态修正
        public static string StateVector = "11111111";
        public static void ModifyState(string ID,char state)
        {
            switch (ID)
            {
              case "1": StateVector = ReplaceChar(StateVector, 7, state);
                          break;
              case "2": StateVector = ReplaceChar(StateVector, 6, state);
                          break;
              case "3": StateVector = ReplaceChar(StateVector, 5, state);
                          break;
              case "4": StateVector = ReplaceChar(StateVector, 4, state);
                          break;
              case "5": StateVector = ReplaceChar(StateVector, 3, state);
                          break;
              case "6": StateVector = ReplaceChar(StateVector, 2, state);
                          break;
              case "7": StateVector = ReplaceChar(StateVector, 1, state);
                          break;
              case "8": StateVector = ReplaceChar(StateVector, 0, state);
                          break;
            }

        }
        public static string ReplaceChar(string str, int index, char c)
        {
            if (index < 0 || index > str.Length - 1) return str;
            char[] carr = str.ToCharArray();
            carr[index] = c;
            return new string(carr);
        }
        #endregion



    }

}
