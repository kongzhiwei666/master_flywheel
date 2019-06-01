using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Framework.Data;
using System.Data.SqlClient;
using Common;
using System.Configuration;
using System.IO;
using System.Threading;


using Framework.WinGui.Utility;
using Framework.WinGui.Interfaces;


namespace Card.WinGui.Forms
{
    public delegate void WorkProcessEventHandler(object sender, WorkProgressEventArgs e);
    /// <summary>
    /// SplashScreen 的摘要说明。
    /// </summary>
    public class SplashScreen : System.Windows.Forms.Form
    {
        //资源载入结束标示变量
        private System.Windows.Forms.Label ShowMessage;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ProgressBar progressBar;
        //
        private System.Threading.Thread m_processthread = null;
        private System.Windows.Forms.Panel panel1;

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.Container components = null;

        public SplashScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.label = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ShowMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.ForeColor = System.Drawing.Color.Red;
            this.label.Location = new System.Drawing.Point(12, 258);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(60, 16);
            this.label.TabIndex = 5;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(496, 24);
            this.progressBar.TabIndex = 7;
            this.progressBar.Visible = false;
            // 
            // ShowMessage
            // 
            this.ShowMessage.BackColor = System.Drawing.Color.Transparent;
            this.ShowMessage.ForeColor = System.Drawing.Color.Red;
            this.ShowMessage.Location = new System.Drawing.Point(80, 258);
            this.ShowMessage.Name = "ShowMessage";
            this.ShowMessage.Size = new System.Drawing.Size(128, 16);
            this.ShowMessage.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Location = new System.Drawing.Point(0, 288);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(496, 16);
            this.panel1.TabIndex = 9;
            this.panel1.Visible = false;
            // 
            // SplashScreen
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(496, 288);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ShowMessage);
            this.Controls.Add(this.label);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region 初始化资源并启动线程
        public void InitSysResource()
        {
            //进度条初始化
            progressBar.Minimum = 1;
            progressBar.Maximum = 9;
            progressBar.Value = 1;
            //启动线程
            this.m_processthread = new System.Threading.Thread(new System.Threading.ThreadStart(this.LoadData));
            this.m_processthread.IsBackground = true;
            this.m_processthread.Start();
            //显示对话框
            //this.ShowDialog();


        }
        #endregion

        #region 数据载入
        protected void LoadData()
        {
            //try
            //{

            //    const int sleeptime = 0;//防止太快 进行认为的时间延时
            //    //服务器连接测试
            //    //				SqlConnection conn = null;
            //    //				string m_sConnectString = System.Configuration.ConfigurationSettings.AppSettings["ConnectString"]+" ;Connect TimeOut=24";
            //    //				try
            //    //				{
            //    //					ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}",SYSTEM_DATA_CONNECTION)],0,1,0);
            //    //					conn = new SqlConnection(m_sConnectString);
            //    //					conn.Open();
            //    //				}
            //    //				catch(Exception ex)
            //    //				{
            //    //					MessageBox.Show(ex.Message);
            //    //					ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}",SYSTEM_DATA_CONNECTION_ERROR)],0,0,0);
            //    //					throw new SysDataException(Framework.Core.Resource.Manager[string.Format("RES_{0}",SYSTEM_DATA_CONNECTION_ERROR)]);
            //    //					
            //    //				}
            //    //				finally
            //    //				{
            //    //					conn.Close();
            //    //				}
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_CONNECTION_SUCCESS)], 0, 1, 0);

            //    //正在加载产品信息
            //    Business.CommonBusiness.BusSystemData bsd = new BusSystemData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_PRODUCT)], 0, 1, 1);
            //    bsd.GetSelectData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_PRODUCT)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //加载数据
            //    CommonGui.InitCode();
            //    //加载基金统计结果数据文件
            //    Business.FundBusiness.BusFundStat sdff = new Business.FundBusiness.BusFundStat();

            //    //正在加载JJ_ZT_TZZH_JJCCJG文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_CCJG)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_JJCCJG";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_CCJG)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_TZZH_HYFB文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_HYFB)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_HYFB";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_HYFB)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_TZZH_HYFB2001文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_HYFB2001)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_HYFB2001";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_HYFB2001)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_TZZH_CYQK文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_CYJG)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_CYQK";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_CYJG)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_TZZH_ZCZH文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_ZCZH)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_ZCZH";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_ZCZH)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_TZZH_JJZC文件
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_JJZC)], 0, 1, 1);
            //    sdff.sTable = "JJ_ZT_TZZH_JJZC";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_JJZC)], 1, 1, 1);
            //    System.Threading.Thread.Sleep(sleeptime);

            //    //正在加载JJ_ZT_JZFX
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_JZFX)], 0, 1, 1);
            //    //判断文件是否过期
            //    string StatDataDirectory = System.Configuration.ConfigurationSettings.AppSettings["XmlSystemData"];
            //    if (!System.IO.Directory.Exists(StatDataDirectory))
            //    {
            //        System.IO.Directory.CreateDirectory(StatDataDirectory);
            //    }
            //    string AcurrentPath = string.Format(@"{0}\{1}.xml", StatDataDirectory, "JJ_ZT_JZFX");
            //    if (System.IO.File.Exists(AcurrentPath))
            //    {
            //        FileInfo fi = new FileInfo(AcurrentPath);
            //        if (fi.CreationTime.DayOfYear != System.DateTime.Now.DayOfYear)
            //        {
            //            fi.Delete();

            //        }

            //    }
            //    sdff.sTable = "JJ_ZT_JZFX";
            //    sdff.GetData();
            //    ShowProgress(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_FUND_STAT_JZFX)], 1, 0, 1);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    throw new SysDataException(Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_CONNECTION_ERROR1)]);
            //}

        }
        private void UpdateProgress(int i)
        {
            if (i == 1)
                this.progressBar.Value++;
            else
                this.progressBar.Value = this.progressBar.Value;

        }
        public void ShowProgress(string msg, int donepercent, int progressid, int init)
        {
            WorkProgressEventArgs e = new WorkProgressEventArgs(msg, donepercent, progressid, init);
            object[] paramslist = { this, e };
            this.BeginInvoke(new WorkProcessEventHandler(UpdateProgress), paramslist);
        }
        private void UpdateProgress(object sender, WorkProgressEventArgs e)
        {
            if (e.Init == 0)
            {
                label.Text = Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_INIT)];

            }
            else
            {
                label.Text = Framework.Core.Resource.Manager[string.Format("RES_{0}", SYSTEM_DATA_READ)];

            }
            ShowMessage.Text = e.Message;
            UpdateProgress(e.DonePercent);
            if (e.ProgressID == 0)
                this.Close();
        }
        #endregion

        #region 资源
        public const string SYSTEM_DATA_PRODUCT = "System_Data_Product";               //产品信息
        public const string SYSTEM_DATA_FUND_STAT_CCJG = "System_Data_Fund_Stat_CCJG";        //基金持仓结构文件
        public const string SYSTEM_DATA_FUND_STAT_HYFB = "System_Data_Fund_Stat_HYFB";        //基金行业分布文件
        public const string SYSTEM_DATA_FUND_STAT_HYFB2001 = "System_Data_Fund_Stat_HYFB2001";    //2001年基金行业分布文件
        public const string SYSTEM_DATA_FUND_STAT_CYJG = "System_Data_Fund_Stat_CYJG";        //基金股票持有结构文件
        public const string SYSTEM_DATA_FUND_STAT_ZCZH = "System_Data_Fund_Stat_ZCZH";        //基金资产组合文件
        public const string SYSTEM_DATA_FUND_STAT_JJZC = "System_Data_Fund_Stat_JJZC";        //基金重仓股文件
        public const string SYSTEM_DATA_INIT = "System_Data_Init";                  //初始化：
        public const string SYSTEM_DATA_READ = "System_Data_Read";                  //正在载入：
        public const string SYSTEM_DATA_CONNECTION = "System_Data_Connection";            // 正在连接服务器
        public const string SYSTEM_DATA_CONNECTION_SUCCESS = "System_Data_Connection_Success";    //服务器连接成功
        public const string SYSTEM_DATA_CONNECTION_ERROR = "System_Data_Connection_Error";      //服务器正在重新启动，请稍后在试
        public const string SYSTEM_DATA_CONNECTION_ERROR1 = "System_Data_Connection_Error1";     //初始化系统数据失败
        public const string SYSTEM_DATA_FUND_STAT_JZFX = "System_Data_Fund_Stat_JZFX";        //基金净值文件
        #endregion

    }
    /// <summary>
    /// 异常类
    /// </summary>
    public class SysDataException : ApplicationException
    {
        public SysDataException(string message)
            : base(message)
        {
        }
        public SysDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 事件通知类
    /// </summary>
    public class WorkProgressEventArgs : EventArgs
    {
        private string m_msg = string.Empty;//信息
        private int m_donepercent = -1;//进度
        private int m_progressid = -1;//结束
        private int m_init = -1;//标识

        public int ProgressID
        {
            get
            {
                return m_progressid;
            }
        }
        public string Message
        {
            get
            {
                return m_msg;
            }
        }
        public int DonePercent
        {
            get
            {
                return m_donepercent;
            }
        }
        public int Init
        {
            get
            {
                return m_init;
            }
        }


        public WorkProgressEventArgs(string msg, int donepercent, int progressid, int init)
        {
            this.m_msg = msg;
            this.m_donepercent = donepercent;
            this.m_progressid = progressid;
            this.m_init = init;
        }

    }

}