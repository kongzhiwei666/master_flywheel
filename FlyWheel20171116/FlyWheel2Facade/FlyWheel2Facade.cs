using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Business.FlyWheel2Business;

namespace Facade.FlyWheel2Facade
{
    public class FlyWheel2Facade
    {
        public FlyWheel2Facade()
        {
        }


        /// <summary>
        /// 打开飞轮1电源
        /// </summary>
        /// <returns></returns>
        public bool OpenElecSource(string state, out string error, string experiment_name, string experiment_memeber, string experiment_product)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            string er = string.Empty;
            bool results = business.OpenElecSource(state, out er, experiment_name, experiment_memeber, experiment_product);
            error = er;
            return results;
        }

        /// <summary>
        /// 恒速控制
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool WriteCardSpeedControl(string direction, string rotorspeed, out string error)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            bool results = business.WriteCardSpeedControl(direction, rotorspeed, out error);
            return results;
        }


        /// <summary>
        /// 力矩控制
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool WriteCardTorqueControl(string direction, string torque, out string error)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            bool results = business.WriteCardTorqueControl(direction, torque, out error);
            return results;
        }


        /// <summary>
        /// 读信息
        /// </summary>
        /// <returns></returns>
        public string[] GetDataFromCard(out string sInfo)
        {
            FlyWheel2Business business = new FlyWheel2Business();

            return business.GetDataFromCard(out sInfo);
        }


        /// <summary>
        /// 读信息can1
        /// </summary>
        /// <returns></returns>
        public bool GetDataFromCardcan(out string motionspeed, out string current, out string sInfo)
        {
            FlyWheel2Business business = new FlyWheel2Business();

            return business.GetDataFromCardcan(out motionspeed, out current, out sInfo);
        }

        /// <summary>
        /// 恒速数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="ConstantMoment"></param>
        /// <param name="ChangeMoment"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <param name="experiment_name"></param>
        /// <param name="experiment_memeber"></param>
        /// <param name="experiment_product"></param>
        /// <param name="real_deta_moment"></param>
        /// <returns></returns>
        public string WriteDataToDatabase(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
             string RotorPow, string ConstantMoment, string ChangeMoment, string RotorRevIde, string RotorRevRea, string datetime,
             string experiment_name, string experiment_memeber, string experiment_product, string real_deta_moment)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            return business.WriteDataToDatabase(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow, ConstantMoment,
                ChangeMoment, RotorRevIde, RotorRevRea, datetime, experiment_name, experiment_memeber, experiment_product, real_deta_moment);
        }

        /// <summary>
        /// 斜坡数据库
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
        /// <param name="ideal_torque"></param>
        /// <param name="experiment_name"></param>
        /// <param name="experiment_memeber"></param>
        /// <param name="experiment_product"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseSlope(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
    string RotorPow, string RotorRevRea, string MeanTorque, string datetime, string ideal_torque, string experiment_name, string experiment_memeber, string experiment_product)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            return business.WriteDataToDatabaseSlope(MotorID, Scheme, SouceVol, SouceCut, RotorVol,
                RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime, ideal_torque, experiment_name, experiment_memeber, experiment_product);
        }


        /// <summary>
        /// 正弦数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="SouceVol"></param>
        /// <param name="SouceCut"></param>
        /// <param name="RotorVol"></param>
        /// <param name="RotorCut"></param>
        /// <param name="RotorPow"></param>
        /// <param name="RotorRevIde"></param>
        /// <param name="RotorRevRea"></param>
        /// <param name="datetime"></param>
        /// <param name="experiment_name"></param>
        /// <param name="experiment_memeber"></param>
        /// <param name="experiment_product"></param>
        /// <param name="real_deta_moment"></param>
        /// <param name="max_deta_moment"></param>
        /// <param name="max_deta_motion"></param>
        /// <param name="mean_torque"></param>
        /// <param name="ideal_torque"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseSine(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut,
            string RotorPow, string RotorRevIde, string RotorRevRea, string datetime,
            string experiment_name, string experiment_memeber, string experiment_product,
             string real_deta_moment, string max_deta_moment, string max_deta_motion, string mean_torque, string ideal_torque)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            return business.WriteDataToDatabaseSine(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow,
                RotorRevIde, RotorRevRea, datetime, experiment_name, experiment_memeber, experiment_product, real_deta_moment, max_deta_moment, max_deta_motion, mean_torque, ideal_torque);
        }

        /// <summary>
        /// 力矩数据库
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
        /// <param name="ideal_torque"></param>
        /// <param name="experiment_name"></param>
        /// <param name="experiment_memeber"></param>
        /// <param name="experiment_product"></param>
        /// <param name="Scheme4_torque"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseTorque(string MotorID, string Scheme, string SouceVol, string SouceCut, string RotorVol, string RotorCut, string RotorPow,
            string RotorRevRea, string MeanTorque, string datetime, string ideal_torque, string experiment_name, string experiment_memeber, string experiment_product, string Scheme4_torque)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            return business.WriteDataToDatabaseTorque(MotorID, Scheme, SouceVol, SouceCut,
                RotorVol, RotorCut, RotorPow, RotorRevRea, MeanTorque, datetime, ideal_torque, experiment_name, experiment_memeber, experiment_product, Scheme4_torque);
        }

        /// <summary>
        /// 时间常数数据库
        /// </summary>
        /// <param name="MotorID"></param>
        /// <param name="Scheme"></param>
        /// <param name="TimeConstant"></param>
        /// <param name="datetime"></param>
        /// <param name="experiment_name"></param>
        /// <param name="experiment_memeber"></param>
        /// <param name="experiment_product"></param>
        /// <returns></returns>
        public string WriteDataToDatabaseTimeConstant(string MotorID, string Scheme, string TimeConstant,
            string datetime, string experiment_name, string experiment_memeber, string experiment_product)
        {
            FlyWheel2Business business = new FlyWheel2Business();
            return business.WriteDataToDatabaseTimeConstant(MotorID, Scheme, TimeConstant, datetime,
                experiment_name, experiment_memeber, experiment_product);
        }



    }
}
