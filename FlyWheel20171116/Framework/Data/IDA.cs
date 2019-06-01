using System;
using System.Data;

namespace Framework.Data
{
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public interface IDA
    {
        string ConnectString { get;}

        /// <summary>
        /// 取得多张表中所有的数据
        /// </summary>
        /// <param name="sTables">表名称</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回DataSet，否则返回null</returns>
        DataSet GetData(string sTables, out string sInfo);
        /// <summary>
        /// 执行sSQL返回返回数据集
        /// </summary>
        /// <param name="sTable">数据表名称</param>
        /// <param name="sSQL">执行命令行</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回DataSet，否则返回null</returns>
        DataSet GetData(string sTable, string sSQL, out string sInfo);
        /// <summary>
        /// 执行单条SQL语句
        /// </summary>
        /// <param name="sSQL">SQL语句</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        bool Execute(string sSQL, out string sInfo);
        /// <summary>
        /// 检查记录是否为空
        /// </summary>
        /// <param name="sSQL">查询语句</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        bool isEmptyRecord(string sSQL, out string sInfo);
        /// <summary>
        /// 执行多条SQL语句
        /// </summary>
        /// <param name="sSQLs">SQL语句数组</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        bool Execute(string[] sSQLs, out string sInfo);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sTables">表名称</param>
        /// <param name="ds">表数据</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        bool SaveData(string sTables, DataSet ds, out string sInfo);
        /// <summary>
        /// 检查数据是否正确
        /// </summary>
        /// <param name="sTables">表名称</param>
        /// <param name="ds">表数据</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        bool CheckData(string sTables, DataSet ds, out string sInfo);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="sTable">表名称</param>
        /// <param name="sExec">代码</param>
        /// <param name="sName">参数名称</param>
        /// <param name="sCount">参数个数</param>
        /// <param name="sCondition">条件</param>
        /// <param name="sProcedureName">存储过程名称</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>结果数据集</returns>
        DataSet GetData(string sTable, string sPara, string sName, string sCount, string sProcedureName, out string sInfo);
    }
}
