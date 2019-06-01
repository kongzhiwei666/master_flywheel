using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Specialized;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;


namespace Framework.Data
{
    /// <summary>
    /// 实现SQL Server 数据库的访问
    /// </summary>
    public class DASQLServer : IDA
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string m_sConnectString = string.Empty;
        /// <summary>
        /// 缺省构造函数，从Config文件ConnectString中提取连接字符串
        /// </summary>
        public DASQLServer()
        {
            m_sConnectString = System.Configuration.ConfigurationSettings.AppSettings["ConnectString"];
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sConn">连接字符串</param>
        public DASQLServer(string sConn)
        {
            m_sConnectString = sConn;
        }


        #region 实现 IDA接口
        /// <summary>
        /// 实现IDA接口，取出sTables中的表的所有数据
        /// </summary>
        /// <param name="sTables">表字符串，以','分割符分隔</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回数据集，否则返回null</returns>
        public virtual DataSet GetData(string sTables, out string sInfo)
        {
            DataSet ds, dstmp;
            sInfo = "";
            string[] sT = sTables.Split(',');
            ds = new DataSet();
            for (int i = 0; i < sT.Length; i++)
            {
                dstmp = GetData(sT[i], string.Format("Select * From {0}", sT[i]), out sInfo);
                if (dstmp != null)
                    ds.Merge(dstmp);
                else
                {
                    ds = null;
                    break;
                }
            }
            return ds;
        }
        /// <summary>
        /// 实现IDA接口，以特定的SQL语句返回数据到sTable表中
        /// </summary>
        /// <param name="sTable">返回数据的表名称</param>
        /// <param name="sSQL">SQL语句</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回数据集，否则返回null</returns>
        public virtual DataSet GetData(string sTable, string sSQL, out string sInfo)
        {
            SqlCommand cmd = null;
            SqlDataAdapter adp = null;
            SqlConnection conn = null;
            DataSet ds = null;
            if (sSQL.Trim().ToUpper().IndexOf("SELECT") > 1)
            {
                sInfo = "请使用常询命令";
                return null;
            }

            try
            {
                adp = new SqlDataAdapter();
                conn = new SqlConnection(m_sConnectString);
                conn.Open();
                cmd = new SqlCommand(sSQL, conn);
                adp.SelectCommand = cmd;
                ds = new DataSet(sTable);
                adp.Fill(ds, sTable);
                sInfo = "";
            }
            catch (Exception ex)
            {
                ds = null;
                sInfo = ex.Message;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
            return ds;
        }
        /// <summary>
        /// 执行单条的命令
        /// </summary>
        /// <param name="sSQL">命令</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        public virtual bool Execute(string sSQL, out string sInfo)
        {

            sInfo = string.Empty;
            SqlCommand cmd = null;
            SqlConnection conn = null;
            bool bResult = true;
            try
            {
                conn = new SqlConnection(m_sConnectString);
                conn.Open();
                cmd = new SqlCommand(sSQL, conn);
                cmd.ExecuteNonQuery();
                sInfo = "";
            }
            catch (Exception ex)
            {
                bResult = false;
                sInfo = ex.Message;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
            return bResult;
        }

        /// <summary>
        /// 判断查询语句记录是否为空
        /// </summary>
        /// <param name="sSQL">查询语句</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        public virtual bool isEmptyRecord(string sSQL, out string sInfo)
        {
            sInfo = string.Empty;
            SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlDataReader sqlReader = null;
            bool bResult = true;
            try
            {
                conn = new SqlConnection(m_sConnectString);
                cmd = conn.CreateCommand();
                cmd.CommandText = sSQL;
                conn.Open();
                sqlReader = cmd.ExecuteReader();
                int i = 0;
                while (sqlReader.Read())
                    i = i + 1;
                if (i == 0)
                    bResult = false;//空记录
                sInfo = "";
            }
            catch (Exception ex)
            {
                bResult = false;
                sInfo = ex.Message;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
            return bResult;
        }

        /// <summary>
        /// 批量处理命令行
        /// </summary>
        /// <param name="sSQLs">命令行数组</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，否则返回false</returns>
        public virtual bool Execute(string[] sSQLs, out string sInfo)
        {
            sInfo = string.Empty;
            SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlTransaction tran = null;
            bool bResult = true;
            try
            {
                conn = new SqlConnection(m_sConnectString);
                conn.Open();
                tran = conn.BeginTransaction();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = tran;
                for (int i = 0; i < sSQLs.Length; i++)
                {
                    cmd.CommandText = sSQLs[i];
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                sInfo = "";
            }
            catch (Exception ex)
            {
                bResult = false;
                tran.Rollback();
                sInfo = ex.Message;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
            return bResult;
        }
        /// <summary>
        /// 保存DataSet中的数据，数据集中的数据表
        /// </summary>
        /// <param name="sTables">数据表字符串，以','分割</param>
        /// <param name="ds">数据集</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>如果成功返回true，如果失败返回false</returns>
        public virtual bool SaveData(string sTables, DataSet ds, out string sInfo)
        {
            sInfo = string.Empty;
            bool bResult = false;
            string[] sT = sTables.Split(',');
            SqlCommand cmd = null;
            SqlDataAdapter adp = null;
            SqlConnection conn = null;
            SqlTransaction myTran = null;
            sInfo = string.Empty;
            conn = new SqlConnection(m_sConnectString);
            conn.Open();
            myTran = conn.BeginTransaction();
            try
            {
                adp = new SqlDataAdapter();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = myTran;

                for (int i = 0; i < sT.Length; i++)
                {
                    cmd.CommandText = string.Format("SELECT * FROM {0}", sT[i]);
                    adp.RowUpdated += new SqlRowUpdatedEventHandler(this.AfterRow_Updated);
                    adp.SelectCommand = cmd;
                    SqlCommandBuilder m_cmdB = new SqlCommandBuilder(adp);
                    adp.Update(ds, sT[i]);
                }

                ds.AcceptChanges();
                myTran.Commit();
                bResult = true;
            }
            catch (Exception ex)
            {
                myTran.Rollback();
                sInfo = ex.Message;
                bResult = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return bResult;
        }

        /// <summary>
        /// 验证数据库字段,未实现
        /// </summary>
        /// <param name="sTables"></param>
        /// <param name="ds"></param>
        /// <param name="sInfo"></param>
        /// <returns></returns>
        public virtual bool CheckData(string sTables, DataSet ds, out string sInfo)
        {
            sInfo = string.Empty;
            return true;
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="sTable">表名称</param>
        /// <param name="sExec">执行文本</param>
        /// <param name="sPara">参数列表</param>
        /// <param name="sName">参数名称</param>
        /// <param name="sCount">参数取值大小</param>
        /// <param name="sProcedureName">存储过程名称</param>
        /// <param name="sInfo">返回字符串</param>
        /// <returns>返回结果集</returns>
        public virtual DataSet GetData(string sTable, string sPara, string sName, string sCount, string sProcedureName, out string sInfo)
        {
            sInfo = string.Empty;
            string[] sT = sName.Split('?');
            string[] sC = sCount.Split(',');
            string[] sP = sPara.Split(',');
            SqlCommand cmd = null;
            SqlDataAdapter adp = null;
            SqlConnection conn = null;
            SqlTransaction tran = null;
            sInfo = string.Empty;
            conn = new SqlConnection(m_sConnectString);

            adp = new SqlDataAdapter();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 60000;
            cmd.CommandText = sProcedureName;
            try
            {
                for (int i = 0; i < sT.Length; i++)
                {
                    cmd.Parameters.Add(sP[i], SqlDbType.VarChar, Convert.ToInt32(sC[i])).Value = sT[i];
                }
                conn.Open();
                tran = conn.BeginTransaction();
            }
            catch (Exception e)
            {
                sInfo = e.Message;

            }
            cmd.Transaction = tran;
            adp.SelectCommand = cmd;
            DataSet ds = new DataSet();

            try
            {

                adp.Fill(ds, sProcedureName);
                tran.Commit();

            }
            catch (Exception e)
            {
                sInfo = e.Message;
                tran.Rollback();

            }
            finally
            {
                conn.Close();
            }
            ds.Tables[0].TableName = sTable;
            return ds;


        }


        #endregion

        #region 属性
        public string ConnectString
        {
            get
            {
                return this.m_sConnectString;
            }
        }
        #endregion

        /// <summary>
        /// Identity数据表的自动更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void AfterRow_Updated(object sender, SqlRowUpdatedEventArgs args)
        {
            if (args.StatementType == StatementType.Insert)
            {
                if (args.Errors == null)
                {
                    // 没有错误，现在开始取得数据
                    string strGetID = "SELECT @@IDENTITY";
                    int newID = 0;
                    SqlCommand cmd = new SqlCommand(strGetID, args.Command.Connection);
                    if (args.Command.Transaction != null)
                        cmd.Transaction = args.Command.Transaction;
                    object obj = cmd.ExecuteScalar();
                    newID = Convert.ToInt32(obj);
                    args.Row[args.Row.Table.TableName + "ID"] = newID;
                }
            }
        }

    }

    /// <summary>
    /// 数据文件类
    /// </summary>
    public class SystemDataFromFile
    {
        public SystemDataFromFile() { }

        #region 文件操作方法
        /// <summary>
        /// 写文件
        /// </summary>
        public void WriteDataToFile(DataSet dsFile, string sTable)
        {
            //判断目录是否存在
            string StatDataDirectory = System.Configuration.ConfigurationSettings.AppSettings["XmlSystemData"];
            if (!System.IO.Directory.Exists(StatDataDirectory))
            {
                System.IO.Directory.CreateDirectory(StatDataDirectory);
            }
            try
            {
                string AcurrentPath = string.Format(@"{0}\{1}.xml", StatDataDirectory, sTable);
                DataSet ds = new DataSet(sTable);
                DataTable dt = dsFile.Tables[sTable].Copy();
                ds.Merge(dt);
                ds.WriteXml(AcurrentPath, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                throw new FileException("基金数据写文件失败" + ex.Message);
            }

        }
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="AcurrentPath">文件的绝对路径</param>
        /// <param name="sHeader">文件的题头</param>
        /// <param name="sExist">存在标识</param>
        public void CheckFile(string AcurrentPath, out bool sExist)
        {
            if (System.IO.File.Exists(AcurrentPath))
            {
                sExist = true;
            }
            else
            {
                sExist = false;

            }

        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="AcurrentPath">绝对路径</param>
        /// <returns></returns>
        public DataSet ReadDataFromFile(string AcurrentPath)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(AcurrentPath, XmlReadMode.ReadSchema);
            return ds;
        }
        #endregion
    }
    /// <summary>
    /// 文件异常类
    /// </summary>
    public class FileException : ApplicationException
    {
        public FileException(string message)
            : base(message)
        {
        }
        public FileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
