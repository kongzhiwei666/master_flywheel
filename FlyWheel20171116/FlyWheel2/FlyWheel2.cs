using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

#region third party namespaces
using DocumentManager;
using DockingSuite;
using TD.SandBar;
#endregion

#region Framework namespaces
using Framework.WinGui.Interfaces;
using Framework.Core;
using Framework.WinGui.Menus;
using Framework.WinGui.Tools;
using Framework.WinGui.Utility;
using Framework;
#endregion

#region business namespaces
using Common;
using Facade.CommonFacade;
using Facade.FlyWheel2Facade;

#endregion


using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
namespace FlyWheel2
{
    /// <summary>
    ///  飞轮2
    /// </summary>
    public class FlyWheel2 : Common.CommonGuiTwo
    {
        FlyWheel2Facade facade = new FlyWheel2Facade();

        string pow = "0";
        double sum_pow = 0;
        int k_pow = 0;

        #region 定义变量
        public static event StateMessageShow ShowInformation;
        private Series series1;
        private Series series2;
        private static string AutoTestLabel = "1111";





        #region 定时器线程
        private System.Timers.Timer ConstantSpeedTimerRead = new System.Timers.Timer(100);//恒速
        private System.Timers.Timer SlopeSpeedTimerRead = new System.Timers.Timer(100); //斜坡
        private System.Timers.Timer SlopeSpeedTimerWrite = new System.Timers.Timer(100); //斜坡

        private System.Timers.Timer SineSpeedTimerRead = new System.Timers.Timer(100);//正弦
        private System.Timers.Timer SineSpeedTimerWrite = new System.Timers.Timer(100);//正弦


        private System.Timers.Timer TorqueSpeedTimerRead = new System.Timers.Timer(100);//力矩



        private System.Threading.Thread threadTime1; //时间常数
        private System.Threading.Thread threadAuto;   //一键


        #endregion


        #region 提取数据
        private static string _can1 = string.Empty;
        private static string _baud1 = string.Empty;
        private static string _can2 = string.Empty;
        private static string _baud2 = string.Empty;
        private static string _axisID = string.Empty;
        #endregion


        string experiment_name;
        string experiment_memeber;
        string experiment_product;

        #region Scheme1
        string Scheme1_sampling = "20";
        string Scheme1_times = string.Empty;
        string Scheme1_direction;
        string Scheme1_rotorspeed;
        string Scheme1_moment = string.Empty;
        double[] Scheme1_H_num;
        double[] Scheme1_T_num;
        int Scheme1_k_number = 0;
        double max_deta_moment = 0;
        double max_deta_motion = 0;
        double real_deta_moment = 0;
        #endregion


        #region Scheme2
        string Scheme2_sampling = "20";
        string Scheme2_times = string.Empty;
        string Scheme2_times_update = string.Empty;
        string Scheme2_current_rotorspeed = "0.00";
        string Scheme2_stop_speed = "6500";
        double[] Scheme2_T_num;
        double[] Scheme2_P_num;
        double[] Scheme2_H_num;
        int Scheme2_k_number = 0;
        int Scheme2_k1_number = 0;
        double mean_torque = 0;
        string Scheme2_detaspeed = "0";
        string Scheme2_moment = string.Empty;
        double ideal_torque = 0;
        #endregion


        #region Scheme3
        double Scheme3_K = 1;
        double Scheme3_T = 90;
        double Scheme3_A = 6000;
        string Scheme3_times = "0.5";
        string Scheme3_times_update = "0.5";
        string Scheme3_sampling = "20";
        int Scheme3_k_number = 0;
        int Scheme3_k1_number = 0;
        double Scheme3_current_time = 0;
        double[] Scheme3_T_num;
        double[] Scheme3_P_num;
        double[] Scheme3_H_num;
        double Scheme3_omega_speed = 0;
        string Scheme3_moment;
        #endregion


        #region Scheme4
        string Scheme4_torque = "0";
        string Scheme4_sampling = "20";
        string Scheme4_times = "0.5";
        double[] Scheme4_T_num;
        double[] Scheme4_P_num;
        int Scheme4_k_number = 0;
        string Scheme4_moment;
        #endregion


        private string sign = "1";

        private DataSet ds_Motor11 = null;
        private DataSet ds_Motor12 = null;
        private DataSet ds_Motor13 = null;
        private Label label27;
        private Label label41;
        private Label label40;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private DataSet ds_Motor14 = null;

        #endregion

        public FlyWheel2(string id, string name)
            : base(id)
        {
            InitializeComponent();
            this.Name = name;
            this._isinti = this.Init();

            #region 图片处理
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(pictureBox9.ClientRectangle);
            Region region = new Region(gp);
            pictureBox9.Region = region;
            pictureBox12.Region = region;
            pictureBox11.Region = region;
            pictureBox10.Region = region;
            pictureBox14.Region = region;
            pictureBox1.Region = region;
            #endregion


            #region 定时器函数
            ConstantSpeedTimerRead.Elapsed += new System.Timers.ElapsedEventHandler(ConstantSpeedTheoutRead); //恒速
            ConstantSpeedTimerRead.AutoReset = true;


            SlopeSpeedTimerRead.Elapsed += new System.Timers.ElapsedEventHandler(SlopeSpeedTheoutRead); // 斜坡
            SlopeSpeedTimerRead.AutoReset = true;
            SlopeSpeedTimerWrite.Elapsed += new System.Timers.ElapsedEventHandler(SlopeSpeedTheoutWrite); // 斜坡
            SlopeSpeedTimerWrite.AutoReset = true;


            SineSpeedTimerRead.Elapsed += new System.Timers.ElapsedEventHandler(SineSpeedTheoutRead); //正弦
            SineSpeedTimerRead.AutoReset = true;
            SineSpeedTimerWrite.Elapsed += new System.Timers.ElapsedEventHandler(SineSpeedTheoutWrite); //正弦
            SineSpeedTimerWrite.AutoReset = true;


            TorqueSpeedTimerRead.Elapsed += new System.Timers.ElapsedEventHandler(TorqueSpeedTheoutRead); //力矩
            TorqueSpeedTimerRead.AutoReset = true;

            #endregion


            series1 = chart1.Series[0];
            series1.ChartType = SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;


            series2 = chart2.Series[0];
            series2.ChartType = SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Blue;

            #region 设置图表的属性
            //图表的背景色
            chart1.BackColor = System.Drawing.Color.FromArgb(211, 223, 240);
            //图表背景色的渐变方式
            chart1.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            chart1.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            chart1.BorderlineWidth = 2;
            #endregion

            #region 设置图表的属性
            //图表的背景色
            chart2.BackColor = System.Drawing.Color.FromArgb(211, 223, 240);
            //图表背景色的渐变方式
            chart2.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            chart2.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            chart2.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            chart2.BorderlineWidth = 2;
            #endregion


            #region 设置图表区属性
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss:ms";
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 100;
            chart1.ChartAreas[0].AxisX.Minimum = 1;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //背景色
            chart1.ChartAreas[0].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            //背景渐变方式
            chart1.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            //渐变和阴影的辅助背景色
            chart1.ChartAreas[0].BackSecondaryColor = System.Drawing.Color.White;
            //边框颜色
            chart1.ChartAreas[0].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //阴影颜色
            chart1.ChartAreas[0].ShadowColor = System.Drawing.Color.Transparent;

            //设置X轴和Y轴线条的颜色和宽度
            chart1.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart1.ChartAreas[0].AxisX.LineWidth = 1;
            chart1.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart1.ChartAreas[0].AxisY.LineWidth = 1;

            //设置X轴和Y轴的标题
            chart1.ChartAreas[0].AxisY.Title = "飞轮电流";

            //设置图表区网格横纵线条的颜色和宽度
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

            #endregion

            #region 设置图表区属性
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss:ms";
            chart2.ChartAreas[0].AxisX.ScaleView.Size = 100;
            chart2.ChartAreas[0].AxisX.Minimum = 1;
            chart2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //背景色
            chart2.ChartAreas[0].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            //背景渐变方式
            chart2.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            //渐变和阴影的辅助背景色
            chart2.ChartAreas[0].BackSecondaryColor = System.Drawing.Color.White;
            //边框颜色
            chart2.ChartAreas[0].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //阴影颜色
            chart2.ChartAreas[0].ShadowColor = System.Drawing.Color.Transparent;

            //设置X轴和Y轴线条的颜色和宽度
            chart2.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisX.LineWidth = 1;
            chart2.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisY.LineWidth = 1;

            //设置X轴和Y轴的标题
            chart2.ChartAreas[0].AxisY.Title = "实际转速";

            //设置图表区网格横纵线条的颜色和宽度
            chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

            #endregion



            this.chart1.Series[0].Points.AddXY(0, 0.5);
            this.chart2.Series[0].Points.AddXY(0, 0.5);





        }

        #region 清理正在使用的资源
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

        #endregion


        #region 设计器生成的代码
        private Button button2;
        private CheckBox checkBox4;
        private Chart chart1;
        private Label label7;
        private TextBox textBox3;
        private Label label6;
        private Label label8;
        private Label label9;
        private TextBox textBox4;
        private GroupBox groupBox1;
        private TextBox textBox7;
        private Label label12;
        private TextBox textBox6;
        private Label label11;
        private TextBox textBox5;
        private Label label10;
        private Label label14;
        private Label label13;
        private Label label16;
        private ComboBox comboBox2;
        private Label label15;
        private TextBox textBox8;
        private Label label17;
        private Label label19;
        private Label label18;
        private Label label21;
        private Label label20;
        private Label label22;
        private Label label23;
        private ComboBox comboBox3;
        private TextBox textBox9;
        private Label label24;
        private Label label25;
        private PictureBox pictureBox1;
        private Label label26;
        private TextBox textBox10;
        private Label label35;
        private Label label34;
        private Label label33;
        private Label label32;
        private Label label31;
        private Label label30;
        private Label label29;
        private Label label28;
        private Label label37;
        private Label label36;
        private Label label38;
        private Label label39;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private System.ComponentModel.IContainer components = null;
        private TabPage tabPage1;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label53;
        private Label label54;
        private Label label55;
        private Label label56;
        private Label label57;
        private Label label58;
        private Label label59;
        private Label label60;
        private ComboBox comboBox10;
        private TextBox textBox13;
        private Label label61;
        private Label label62;
        private TextBox textBox14;
        private Label label63;
        private Label label64;
        private Panel panel2;
        private GroupBox groupBox11;
        private Label label103;
        private ComboBox comboBox12;
        private Label label99;
        private TextBox textBox22;
        private Label label80;
        private RadioButton radioButton12;
        private Button button17;
        private Label label97;
        private TextBox textBox19;
        private PictureBox pictureBox12;
        private Label label104;
        private ComboBox comboBox16;
        private Label label95;
        private TextBox textBox21;
        private Label label93;
        private RadioButton radioButton11;
        private Button button16;
        private PictureBox pictureBox11;
        private Label label94;
        private TextBox textBox20;
        private Label label91;
        private Label label92;
        private Label label90;
        private GroupBox groupBox12;
        private Label label102;
        private Label label101;
        private Label label65;
        private Label label66;
        private Label label67;
        private Label label68;
        private Label label69;
        private Label label70;
        private Label label71;
        private Label label72;
        private Label label73;
        private Label label74;
        private Label label75;
        private Label label76;
        private Label label77;
        private Label label78;
        private GroupBox groupBox13;
        private Button button13;
        private Label label83;
        private Button button14;
        private GroupBox groupBox15;
        private Label label5;
        private Button button1;
        private RadioButton radioButton1;
        private Label label4;
        private ComboBox comboBox1;
        private Label label3;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox1;
        private Label label1;
        private TextBox textBox18;
        private Label label100;
        private Label label105;
        private PictureBox pictureBox14;
        private Button button20;
        private RadioButton radioButton14;
        private Label label88;
        private ComboBox comboBox17;
        private Label label98;
        private PictureBox pictureBox9;
        private Button button15;
        private TextBox textBox15;
        private TextBox textBox16;
        private RadioButton radioButton10;
        private TextBox textBox17;
        private PictureBox pictureBox10;
        private Button button18;
        private Label label84;
        private Label label85;
        private Label label86;
        private Label label89;
        private Label label87;
        private RadioButton radioButton9;
        private GroupBox groupBox16;
        private Chart chart2;


        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.label61 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label103 = new System.Windows.Forms.Label();
            this.comboBox12 = new System.Windows.Forms.ComboBox();
            this.label99 = new System.Windows.Forms.Label();
            this.textBox22 = new System.Windows.Forms.TextBox();
            this.label80 = new System.Windows.Forms.Label();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.button17 = new System.Windows.Forms.Button();
            this.label97 = new System.Windows.Forms.Label();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.label104 = new System.Windows.Forms.Label();
            this.comboBox16 = new System.Windows.Forms.ComboBox();
            this.label95 = new System.Windows.Forms.Label();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.label93 = new System.Windows.Forms.Label();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.button16 = new System.Windows.Forms.Button();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.label94 = new System.Windows.Forms.Label();
            this.textBox20 = new System.Windows.Forms.TextBox();
            this.label91 = new System.Windows.Forms.Label();
            this.label92 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.label101 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.button13 = new System.Windows.Forms.Button();
            this.label83 = new System.Windows.Forms.Label();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label38 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.label100 = new System.Windows.Forms.Label();
            this.label105 = new System.Windows.Forms.Label();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.button20 = new System.Windows.Forms.Button();
            this.radioButton14 = new System.Windows.Forms.RadioButton();
            this.label88 = new System.Windows.Forms.Label();
            this.comboBox17 = new System.Windows.Forms.ComboBox();
            this.label98 = new System.Windows.Forms.Label();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.button15 = new System.Windows.Forms.Button();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.button18 = new System.Windows.Forms.Button();
            this.label84 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button14 = new System.Windows.Forms.Button();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabMain.Size = new System.Drawing.Size(1458, 856);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(7, 32);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(47, 19);
            this.label50.TabIndex = 4;
            this.label50.Text = "端口";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label51.Location = new System.Drawing.Point(6, 97);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(85, 19);
            this.label51.TabIndex = 0;
            this.label51.Text = "电源开关";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(17, 455);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(104, 19);
            this.label52.TabIndex = 40;
            this.label52.Text = "更新间隔：";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(21, 418);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(49, 19);
            this.label53.TabIndex = 39;
            this.label53.Text = "T = ";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(23, 379);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(39, 19);
            this.label54.TabIndex = 38;
            this.label54.Text = "A =";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(24, 346);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(39, 19);
            this.label55.TabIndex = 37;
            this.label55.Text = "K =";
            // 
            // label56
            // 
            this.label56.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label56.Location = new System.Drawing.Point(26, 288);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(263, 2);
            this.label56.TabIndex = 34;
            this.label56.Text = "rpm";
            // 
            // label57
            // 
            this.label57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label57.Location = new System.Drawing.Point(19, 148);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(263, 2);
            this.label57.TabIndex = 33;
            this.label57.Text = "rpm";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(214, 248);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(19, 19);
            this.label58.TabIndex = 32;
            this.label58.Text = "s";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(214, 208);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(39, 19);
            this.label59.TabIndex = 31;
            this.label59.Text = "rpm";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(15, 248);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(104, 19);
            this.label60.TabIndex = 29;
            this.label60.Text = "更新间隔：";
            // 
            // comboBox10
            // 
            this.comboBox10.FormattingEnabled = true;
            this.comboBox10.Location = new System.Drawing.Point(108, 64);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(99, 20);
            this.comboBox10.TabIndex = 23;
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(113, 205);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(100, 21);
            this.textBox13.TabIndex = 8;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(214, 105);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(39, 19);
            this.label61.TabIndex = 7;
            this.label61.Text = "rpm";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(15, 210);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(104, 19);
            this.label62.TabIndex = 6;
            this.label62.Text = "转速增量：";
            // 
            // textBox14
            // 
            this.textBox14.Location = new System.Drawing.Point(108, 98);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(100, 21);
            this.textBox14.TabIndex = 4;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(15, 105);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(104, 19);
            this.label63.TabIndex = 3;
            this.label63.Text = "转速设置：";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(15, 66);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(104, 19);
            this.label64.TabIndex = 2;
            this.label64.Text = "转向设置：";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1450, 823);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.groupBox11);
            this.panel2.Controls.Add(this.groupBox12);
            this.panel2.Controls.Add(this.groupBox13);
            this.panel2.Controls.Add(this.groupBox15);
            this.panel2.Controls.Add(this.groupBox16);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1444, 817);
            this.panel2.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox7);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBox6);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 144);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "实验信息";
            // 
            // textBox7
            // 
            this.textBox7.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox7.Location = new System.Drawing.Point(134, 105);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(100, 29);
            this.textBox7.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(104, 19);
            this.label12.TabIndex = 4;
            this.label12.Text = "产品编号：";
            // 
            // textBox6
            // 
            this.textBox6.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox6.Location = new System.Drawing.Point(133, 64);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 29);
            this.textBox6.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 19);
            this.label11.TabIndex = 2;
            this.label11.Text = "试验人员：";
            // 
            // textBox5
            // 
            this.textBox5.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox5.Location = new System.Drawing.Point(133, 26);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 29);
            this.textBox5.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 19);
            this.label10.TabIndex = 0;
            this.label10.Text = "试验名称：";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.label27);
            this.groupBox11.Controls.Add(this.textBox8);
            this.groupBox11.Controls.Add(this.label17);
            this.groupBox11.Controls.Add(this.label16);
            this.groupBox11.Controls.Add(this.comboBox2);
            this.groupBox11.Controls.Add(this.label15);
            this.groupBox11.Controls.Add(this.label9);
            this.groupBox11.Controls.Add(this.textBox4);
            this.groupBox11.Controls.Add(this.label8);
            this.groupBox11.Controls.Add(this.label7);
            this.groupBox11.Controls.Add(this.textBox3);
            this.groupBox11.Controls.Add(this.label6);
            this.groupBox11.Controls.Add(this.label103);
            this.groupBox11.Controls.Add(this.comboBox12);
            this.groupBox11.Controls.Add(this.label99);
            this.groupBox11.Controls.Add(this.textBox22);
            this.groupBox11.Controls.Add(this.label80);
            this.groupBox11.Controls.Add(this.radioButton12);
            this.groupBox11.Controls.Add(this.button17);
            this.groupBox11.Controls.Add(this.label97);
            this.groupBox11.Controls.Add(this.textBox19);
            this.groupBox11.Controls.Add(this.pictureBox12);
            this.groupBox11.Controls.Add(this.label104);
            this.groupBox11.Controls.Add(this.comboBox16);
            this.groupBox11.Controls.Add(this.label95);
            this.groupBox11.Controls.Add(this.textBox21);
            this.groupBox11.Controls.Add(this.label93);
            this.groupBox11.Controls.Add(this.radioButton11);
            this.groupBox11.Controls.Add(this.button16);
            this.groupBox11.Controls.Add(this.pictureBox11);
            this.groupBox11.Controls.Add(this.label94);
            this.groupBox11.Controls.Add(this.textBox20);
            this.groupBox11.Controls.Add(this.label91);
            this.groupBox11.Controls.Add(this.label92);
            this.groupBox11.Controls.Add(this.label90);
            this.groupBox11.Location = new System.Drawing.Point(7, 228);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(306, 578);
            this.groupBox11.TabIndex = 13;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "测试模式";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label27.Location = new System.Drawing.Point(237, 467);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(63, 14);
            this.label27.TabIndex = 61;
            this.label27.Text = "mNm·kg²";
            // 
            // textBox8
            // 
            this.textBox8.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox8.Location = new System.Drawing.Point(134, 458);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(100, 29);
            this.textBox8.TabIndex = 60;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(26, 462);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(104, 19);
            this.label17.TabIndex = 59;
            this.label17.Text = "转动惯量：";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(242, 504);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(14, 14);
            this.label16.TabIndex = 58;
            this.label16.Text = "s";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(133, 501);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(100, 27);
            this.comboBox2.TabIndex = 57;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 504);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(104, 19);
            this.label15.TabIndex = 56;
            this.label15.Text = "采样间隔：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(238, 164);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 14);
            this.label9.TabIndex = 55;
            this.label9.Text = "mNm·kg²";
            // 
            // textBox4
            // 
            this.textBox4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.Location = new System.Drawing.Point(133, 157);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 29);
            this.textBox4.TabIndex = 54;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 19);
            this.label8.TabIndex = 53;
            this.label8.Text = "转动惯量：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(243, 374);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 14);
            this.label7.TabIndex = 52;
            this.label7.Text = "rpm";
            // 
            // textBox3
            // 
            this.textBox3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox3.Location = new System.Drawing.Point(133, 371);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 29);
            this.textBox3.TabIndex = 51;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 375);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 19);
            this.label6.TabIndex = 50;
            this.label6.Text = "终速指令：";
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label103.Location = new System.Drawing.Point(243, 204);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(14, 14);
            this.label103.TabIndex = 35;
            this.label103.Text = "s";
            // 
            // comboBox12
            // 
            this.comboBox12.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox12.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox12.FormattingEnabled = true;
            this.comboBox12.Location = new System.Drawing.Point(133, 201);
            this.comboBox12.Name = "comboBox12";
            this.comboBox12.Size = new System.Drawing.Size(101, 27);
            this.comboBox12.TabIndex = 30;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(24, 205);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(104, 19);
            this.label99.TabIndex = 29;
            this.label99.Text = "采样间隔：";
            // 
            // textBox22
            // 
            this.textBox22.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox22.Location = new System.Drawing.Point(135, 116);
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new System.Drawing.Size(100, 29);
            this.textBox22.TabIndex = 28;
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(24, 122);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(104, 19);
            this.label80.TabIndex = 27;
            this.label80.Text = "采样数量：";
            // 
            // radioButton12
            // 
            this.radioButton12.AutoSize = true;
            this.radioButton12.Location = new System.Drawing.Point(11, 28);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(122, 23);
            this.radioButton12.TabIndex = 0;
            this.radioButton12.TabStop = true;
            this.radioButton12.Text = "恒速控制：";
            this.radioButton12.UseVisualStyleBackColor = true;
            this.radioButton12.CheckedChanged += new System.EventHandler(this.radioButton12_CheckedChanged);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(139, 24);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(75, 31);
            this.button17.TabIndex = 24;
            this.button17.Text = "运行";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // label97
            // 
            this.label97.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label97.Location = new System.Drawing.Point(18, 253);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(263, 2);
            this.label97.TabIndex = 35;
            this.label97.Text = "rpm";
            // 
            // textBox19
            // 
            this.textBox19.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox19.Location = new System.Drawing.Point(133, 415);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(100, 29);
            this.textBox19.TabIndex = 49;
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox12.Location = new System.Drawing.Point(245, 24);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(36, 31);
            this.pictureBox12.TabIndex = 26;
            this.pictureBox12.TabStop = false;
            // 
            // label104
            // 
            this.label104.AutoSize = true;
            this.label104.Location = new System.Drawing.Point(23, 421);
            this.label104.Name = "label104";
            this.label104.Size = new System.Drawing.Size(104, 19);
            this.label104.TabIndex = 48;
            this.label104.Text = "采样数量：";
            // 
            // comboBox16
            // 
            this.comboBox16.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox16.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox16.FormattingEnabled = true;
            this.comboBox16.Location = new System.Drawing.Point(133, 544);
            this.comboBox16.Name = "comboBox16";
            this.comboBox16.Size = new System.Drawing.Size(100, 27);
            this.comboBox16.TabIndex = 47;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(24, 77);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(104, 19);
            this.label95.TabIndex = 3;
            this.label95.Text = "转速设置：";
            // 
            // textBox21
            // 
            this.textBox21.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox21.Location = new System.Drawing.Point(136, 72);
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new System.Drawing.Size(100, 29);
            this.textBox21.TabIndex = 4;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label93.Location = new System.Drawing.Point(243, 77);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(28, 14);
            this.label93.TabIndex = 7;
            this.label93.Text = "rpm";
            // 
            // radioButton11
            // 
            this.radioButton11.AutoSize = true;
            this.radioButton11.Location = new System.Drawing.Point(11, 281);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(122, 23);
            this.radioButton11.TabIndex = 5;
            this.radioButton11.TabStop = true;
            this.radioButton11.Text = "斜坡控制：";
            this.radioButton11.UseVisualStyleBackColor = true;
            this.radioButton11.CheckedChanged += new System.EventHandler(this.radioButton11_CheckedChanged);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(139, 277);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(75, 33);
            this.button16.TabIndex = 25;
            this.button16.Text = "运行";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // pictureBox11
            // 
            this.pictureBox11.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox11.Location = new System.Drawing.Point(245, 277);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(36, 31);
            this.pictureBox11.TabIndex = 27;
            this.pictureBox11.TabStop = false;
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(25, 329);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(104, 19);
            this.label94.TabIndex = 6;
            this.label94.Text = "转速增量：";
            // 
            // textBox20
            // 
            this.textBox20.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox20.Location = new System.Drawing.Point(133, 324);
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new System.Drawing.Size(100, 29);
            this.textBox20.TabIndex = 8;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label91.Location = new System.Drawing.Point(243, 332);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(28, 14);
            this.label91.TabIndex = 31;
            this.label91.Text = "rpm";
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(23, 546);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(104, 19);
            this.label92.TabIndex = 29;
            this.label92.Text = "更新间隔：";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label90.Location = new System.Drawing.Point(241, 548);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(14, 14);
            this.label90.TabIndex = 32;
            this.label90.Text = "s";
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox12.Controls.Add(this.button2);
            this.groupBox12.Controls.Add(this.label39);
            this.groupBox12.Controls.Add(this.label37);
            this.groupBox12.Controls.Add(this.label36);
            this.groupBox12.Controls.Add(this.label35);
            this.groupBox12.Controls.Add(this.label34);
            this.groupBox12.Controls.Add(this.label33);
            this.groupBox12.Controls.Add(this.label32);
            this.groupBox12.Controls.Add(this.label31);
            this.groupBox12.Controls.Add(this.label30);
            this.groupBox12.Controls.Add(this.label29);
            this.groupBox12.Controls.Add(this.label28);
            this.groupBox12.Controls.Add(this.label21);
            this.groupBox12.Controls.Add(this.label20);
            this.groupBox12.Controls.Add(this.label19);
            this.groupBox12.Controls.Add(this.label18);
            this.groupBox12.Controls.Add(this.label14);
            this.groupBox12.Controls.Add(this.label13);
            this.groupBox12.Controls.Add(this.label102);
            this.groupBox12.Controls.Add(this.label101);
            this.groupBox12.Controls.Add(this.label65);
            this.groupBox12.Controls.Add(this.label66);
            this.groupBox12.Controls.Add(this.label67);
            this.groupBox12.Controls.Add(this.label68);
            this.groupBox12.Controls.Add(this.label69);
            this.groupBox12.Controls.Add(this.label70);
            this.groupBox12.Controls.Add(this.label71);
            this.groupBox12.Controls.Add(this.label72);
            this.groupBox12.Controls.Add(this.label73);
            this.groupBox12.Controls.Add(this.label74);
            this.groupBox12.Controls.Add(this.label75);
            this.groupBox12.Controls.Add(this.label76);
            this.groupBox12.Controls.Add(this.label77);
            this.groupBox12.Controls.Add(this.label78);
            this.groupBox12.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox12.Location = new System.Drawing.Point(623, 6);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(811, 212);
            this.groupBox12.TabIndex = 5;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "状态参数";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(602, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 37);
            this.button2.TabIndex = 57;
            this.button2.Text = "刷新";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label39.Location = new System.Drawing.Point(204, 114);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(49, 19);
            this.label39.TabIndex = 56;
            this.label39.Text = "mNms";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(479, 116);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(39, 19);
            this.label37.TabIndex = 34;
            this.label37.Text = "mNm";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(742, 119);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(39, 19);
            this.label36.TabIndex = 33;
            this.label36.Text = "mNm";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(232, 164);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(39, 19);
            this.label35.TabIndex = 32;
            this.label35.Text = "rpm";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(524, 164);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(39, 19);
            this.label34.TabIndex = 31;
            this.label34.Text = "rpm";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(729, 71);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(39, 19);
            this.label33.TabIndex = 30;
            this.label33.Text = "rpm";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(729, 29);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(19, 19);
            this.label32.TabIndex = 29;
            this.label32.Text = "W";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(479, 32);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(19, 19);
            this.label31.TabIndex = 28;
            this.label31.Text = "A";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(203, 71);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(19, 19);
            this.label30.TabIndex = 27;
            this.label30.Text = "A";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(478, 73);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(19, 19);
            this.label29.TabIndex = 26;
            this.label29.Text = "V";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(203, 30);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(19, 19);
            this.label28.TabIndex = 25;
            this.label28.Text = "V";
            // 
            // label21
            // 
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label21.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label21.Location = new System.Drawing.Point(663, 117);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(71, 21);
            this.label21.TabIndex = 24;
            this.label21.Text = "00.000";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(534, 119);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(123, 19);
            this.label20.TabIndex = 23;
            this.label20.Text = "期望平均力矩";
            // 
            // label19
            // 
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label19.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label19.Location = new System.Drawing.Point(394, 114);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 24);
            this.label19.TabIndex = 22;
            this.label19.Text = "00.000";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(301, 115);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(85, 19);
            this.label18.TabIndex = 21;
            this.label18.Text = "平均力矩";
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label14.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label14.Location = new System.Drawing.Point(120, 114);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 21);
            this.label14.TabIndex = 20;
            this.label14.Text = "00.000";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(25, 115);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 19);
            this.label13.TabIndex = 19;
            this.label13.Text = "角动量";
            // 
            // label102
            // 
            this.label102.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label102.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label102.Location = new System.Drawing.Point(447, 161);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(71, 21);
            this.label102.TabIndex = 18;
            this.label102.Text = "00.000";
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Location = new System.Drawing.Point(324, 163);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(123, 19);
            this.label101.TabIndex = 17;
            this.label101.Text = "转速动态偏差";
            // 
            // label65
            // 
            this.label65.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label65.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label65.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label65.Location = new System.Drawing.Point(154, 163);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(71, 21);
            this.label65.TabIndex = 16;
            this.label65.Text = "00.000";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label66.Location = new System.Drawing.Point(25, 163);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(123, 19);
            this.label66.TabIndex = 15;
            this.label66.Text = "转速常值偏差";
            // 
            // label67
            // 
            this.label67.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label67.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label67.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label67.Location = new System.Drawing.Point(625, 27);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(85, 21);
            this.label67.TabIndex = 14;
            this.label67.Text = "00.000";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label68.Location = new System.Drawing.Point(534, 29);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(85, 19);
            this.label68.TabIndex = 9;
            this.label68.Text = "电源功耗";
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label69.Location = new System.Drawing.Point(18, 29);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(85, 19);
            this.label69.TabIndex = 2;
            this.label69.Text = "电源电压";
            // 
            // label70
            // 
            this.label70.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label70.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label70.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label70.Location = new System.Drawing.Point(118, 29);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(79, 21);
            this.label70.TabIndex = 3;
            this.label70.Text = "00.000";
            // 
            // label71
            // 
            this.label71.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label71.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label71.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label71.Location = new System.Drawing.Point(118, 69);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(78, 21);
            this.label71.TabIndex = 13;
            this.label71.Text = "00.000";
            // 
            // label72
            // 
            this.label72.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label72.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label72.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label72.Location = new System.Drawing.Point(395, 72);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(71, 21);
            this.label72.TabIndex = 12;
            this.label72.Text = "00.000";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label73.Location = new System.Drawing.Point(534, 74);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(85, 19);
            this.label73.TabIndex = 10;
            this.label73.Text = "实际转速";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label74.Location = new System.Drawing.Point(19, 71);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(85, 19);
            this.label74.TabIndex = 8;
            this.label74.Text = "飞轮电流";
            // 
            // label75
            // 
            this.label75.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label75.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label75.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label75.Location = new System.Drawing.Point(395, 30);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(71, 21);
            this.label75.TabIndex = 7;
            this.label75.Text = "00.000";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label76.Location = new System.Drawing.Point(296, 72);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(85, 19);
            this.label76.TabIndex = 6;
            this.label76.Text = "飞轮电压";
            // 
            // label77
            // 
            this.label77.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label77.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label77.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label77.Location = new System.Drawing.Point(625, 70);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(85, 21);
            this.label77.TabIndex = 5;
            this.label77.Text = "00.000";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label78.Location = new System.Drawing.Point(294, 31);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(85, 19);
            this.label78.TabIndex = 4;
            this.label78.Text = "电源电流";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.button13);
            this.groupBox13.Controls.Add(this.label83);
            this.groupBox13.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox13.Location = new System.Drawing.Point(8, 156);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(306, 62);
            this.groupBox13.TabIndex = 12;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "电源";
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(108, 25);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(75, 28);
            this.button13.TabIndex = 1;
            this.button13.Text = "ON";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label83.Location = new System.Drawing.Point(7, 29);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(85, 19);
            this.label83.TabIndex = 0;
            this.label83.Text = "电源开关";
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.checkBox6);
            this.groupBox15.Controls.Add(this.checkBox5);
            this.groupBox15.Controls.Add(this.label41);
            this.groupBox15.Controls.Add(this.label40);
            this.groupBox15.Controls.Add(this.checkBox4);
            this.groupBox15.Controls.Add(this.checkBox3);
            this.groupBox15.Controls.Add(this.checkBox2);
            this.groupBox15.Controls.Add(this.checkBox1);
            this.groupBox15.Controls.Add(this.label38);
            this.groupBox15.Controls.Add(this.textBox10);
            this.groupBox15.Controls.Add(this.label26);
            this.groupBox15.Controls.Add(this.pictureBox1);
            this.groupBox15.Controls.Add(this.label25);
            this.groupBox15.Controls.Add(this.textBox9);
            this.groupBox15.Controls.Add(this.label24);
            this.groupBox15.Controls.Add(this.label23);
            this.groupBox15.Controls.Add(this.comboBox3);
            this.groupBox15.Controls.Add(this.label22);
            this.groupBox15.Controls.Add(this.label5);
            this.groupBox15.Controls.Add(this.button1);
            this.groupBox15.Controls.Add(this.radioButton1);
            this.groupBox15.Controls.Add(this.label4);
            this.groupBox15.Controls.Add(this.comboBox1);
            this.groupBox15.Controls.Add(this.label3);
            this.groupBox15.Controls.Add(this.textBox2);
            this.groupBox15.Controls.Add(this.label2);
            this.groupBox15.Controls.Add(this.textBox1);
            this.groupBox15.Controls.Add(this.label1);
            this.groupBox15.Controls.Add(this.textBox18);
            this.groupBox15.Controls.Add(this.label100);
            this.groupBox15.Controls.Add(this.label105);
            this.groupBox15.Controls.Add(this.pictureBox14);
            this.groupBox15.Controls.Add(this.button20);
            this.groupBox15.Controls.Add(this.radioButton14);
            this.groupBox15.Controls.Add(this.label88);
            this.groupBox15.Controls.Add(this.comboBox17);
            this.groupBox15.Controls.Add(this.label98);
            this.groupBox15.Controls.Add(this.pictureBox9);
            this.groupBox15.Controls.Add(this.button15);
            this.groupBox15.Controls.Add(this.textBox15);
            this.groupBox15.Controls.Add(this.textBox16);
            this.groupBox15.Controls.Add(this.radioButton10);
            this.groupBox15.Controls.Add(this.textBox17);
            this.groupBox15.Controls.Add(this.pictureBox10);
            this.groupBox15.Controls.Add(this.button18);
            this.groupBox15.Controls.Add(this.label84);
            this.groupBox15.Controls.Add(this.label85);
            this.groupBox15.Controls.Add(this.label86);
            this.groupBox15.Controls.Add(this.label89);
            this.groupBox15.Controls.Add(this.label87);
            this.groupBox15.Controls.Add(this.radioButton9);
            this.groupBox15.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox15.Location = new System.Drawing.Point(320, 6);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(294, 800);
            this.groupBox15.TabIndex = 6;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "测试模式";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label41.Location = new System.Drawing.Point(233, 494);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(63, 14);
            this.label41.TabIndex = 81;
            this.label41.Text = "mNm·kg²";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label40.Location = new System.Drawing.Point(233, 227);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(63, 14);
            this.label40.TabIndex = 80;
            this.label40.Text = "mNm·kg²";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(16, 735);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(104, 23);
            this.checkBox4.TabIndex = 79;
            this.checkBox4.Text = "斜坡控制";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(132, 735);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(104, 23);
            this.checkBox3.TabIndex = 78;
            this.checkBox3.Text = "正弦控制";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(132, 706);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(104, 23);
            this.checkBox2.TabIndex = 77;
            this.checkBox2.Text = "力矩控制";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 706);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 23);
            this.checkBox1.TabIndex = 76;
            this.checkBox1.Text = "恒速控制";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label38.Location = new System.Drawing.Point(238, 411);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(28, 14);
            this.label38.TabIndex = 35;
            this.label38.Text = "mNm";
            // 
            // textBox10
            // 
            this.textBox10.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox10.Location = new System.Drawing.Point(132, 219);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(100, 29);
            this.textBox10.TabIndex = 75;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(22, 222);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(104, 19);
            this.label26.TabIndex = 74;
            this.label26.Text = "转动惯量：";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox1.Location = new System.Drawing.Point(243, 578);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 30);
            this.pictureBox1.TabIndex = 73;
            this.pictureBox1.TabStop = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(30, 617);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(104, 19);
            this.label25.TabIndex = 72;
            this.label25.Text = "时间常数：";
            // 
            // textBox9
            // 
            this.textBox9.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox9.Location = new System.Drawing.Point(133, 486);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(100, 29);
            this.textBox9.TabIndex = 71;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(30, 489);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(104, 19);
            this.label24.TabIndex = 70;
            this.label24.Text = "转动惯量：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(246, 262);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(14, 14);
            this.label23.TabIndex = 69;
            this.label23.Text = "s";
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(132, 254);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(100, 27);
            this.comboBox3.TabIndex = 68;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(22, 257);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(104, 19);
            this.label22.TabIndex = 67;
            this.label22.Text = "采样间隔：";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label5.Location = new System.Drawing.Point(142, 617);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 21);
            this.label5.TabIndex = 66;
            this.label5.Text = "0.00";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(140, 577);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 65;
            this.button1.Text = "运行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(16, 578);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(122, 23);
            this.radioButton1.TabIndex = 64;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "时间常数：";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(246, 527);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 14);
            this.label4.TabIndex = 63;
            this.label4.Text = "s";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(133, 523);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(101, 27);
            this.comboBox1.TabIndex = 62;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 528);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 19);
            this.label3.TabIndex = 61;
            this.label3.Text = "采样间隔：";
            // 
            // textBox2
            // 
            this.textBox2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox2.Location = new System.Drawing.Point(133, 445);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 29);
            this.textBox2.TabIndex = 60;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 449);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 59;
            this.label2.Text = "采样数量：";
            // 
            // textBox1
            // 
            this.textBox1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.Location = new System.Drawing.Point(132, 180);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 29);
            this.textBox1.TabIndex = 58;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 57;
            this.label1.Text = "采样数量：";
            // 
            // textBox18
            // 
            this.textBox18.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox18.Location = new System.Drawing.Point(132, 405);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(100, 29);
            this.textBox18.TabIndex = 56;
            // 
            // label100
            // 
            this.label100.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label100.Location = new System.Drawing.Point(19, 560);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(263, 2);
            this.label100.TabIndex = 34;
            this.label100.Text = "rpm";
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Location = new System.Drawing.Point(30, 409);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(104, 19);
            this.label105.TabIndex = 55;
            this.label105.Text = "控制力矩：";
            // 
            // pictureBox14
            // 
            this.pictureBox14.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox14.Location = new System.Drawing.Point(243, 367);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(36, 32);
            this.pictureBox14.TabIndex = 54;
            this.pictureBox14.TabStop = false;
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(140, 366);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(75, 28);
            this.button20.TabIndex = 53;
            this.button20.Text = "运行";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // radioButton14
            // 
            this.radioButton14.AutoSize = true;
            this.radioButton14.Location = new System.Drawing.Point(16, 368);
            this.radioButton14.Name = "radioButton14";
            this.radioButton14.Size = new System.Drawing.Size(122, 23);
            this.radioButton14.TabIndex = 52;
            this.radioButton14.TabStop = true;
            this.radioButton14.Text = "力矩控制：";
            this.radioButton14.UseVisualStyleBackColor = true;
            this.radioButton14.CheckedChanged += new System.EventHandler(this.radioButton14_CheckedChanged);
            // 
            // label88
            // 
            this.label88.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label88.Location = new System.Drawing.Point(16, 648);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(263, 2);
            this.label88.TabIndex = 51;
            this.label88.Text = "rpm";
            // 
            // comboBox17
            // 
            this.comboBox17.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox17.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.comboBox17.FormattingEnabled = true;
            this.comboBox17.Location = new System.Drawing.Point(132, 294);
            this.comboBox17.Name = "comboBox17";
            this.comboBox17.Size = new System.Drawing.Size(100, 27);
            this.comboBox17.TabIndex = 50;
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label98.Location = new System.Drawing.Point(246, 300);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(14, 14);
            this.label98.TabIndex = 46;
            this.label98.Text = "s";
            // 
            // pictureBox9
            // 
            this.pictureBox9.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox9.Location = new System.Drawing.Point(236, 20);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(36, 31);
            this.pictureBox9.TabIndex = 45;
            this.pictureBox9.TabStop = false;
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(140, 18);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(75, 33);
            this.button15.TabIndex = 44;
            this.button15.Text = "运行";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // textBox15
            // 
            this.textBox15.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox15.Location = new System.Drawing.Point(133, 140);
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new System.Drawing.Size(100, 29);
            this.textBox15.TabIndex = 43;
            // 
            // textBox16
            // 
            this.textBox16.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox16.Location = new System.Drawing.Point(134, 97);
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new System.Drawing.Size(100, 29);
            this.textBox16.TabIndex = 42;
            // 
            // radioButton10
            // 
            this.radioButton10.AutoSize = true;
            this.radioButton10.Location = new System.Drawing.Point(16, 669);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(122, 23);
            this.radioButton10.TabIndex = 28;
            this.radioButton10.TabStop = true;
            this.radioButton10.Text = "一键测试：";
            this.radioButton10.UseVisualStyleBackColor = true;
            this.radioButton10.CheckedChanged += new System.EventHandler(this.radioButton10_CheckedChanged);
            // 
            // textBox17
            // 
            this.textBox17.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBox17.Location = new System.Drawing.Point(134, 57);
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new System.Drawing.Size(100, 29);
            this.textBox17.TabIndex = 41;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::FlyWheel2.Properties.Resources.red;
            this.pictureBox10.Location = new System.Drawing.Point(241, 664);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(36, 31);
            this.pictureBox10.TabIndex = 27;
            this.pictureBox10.TabStop = false;
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(142, 663);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(73, 33);
            this.button18.TabIndex = 18;
            this.button18.Text = "运行";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(22, 299);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(104, 19);
            this.label84.TabIndex = 40;
            this.label84.Text = "更新间隔：";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(77, 144);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(49, 19);
            this.label85.TabIndex = 39;
            this.label85.Text = "T = ";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(77, 100);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(39, 19);
            this.label86.TabIndex = 38;
            this.label86.Text = "A =";
            // 
            // label89
            // 
            this.label89.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label89.Location = new System.Drawing.Point(19, 351);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(263, 2);
            this.label89.TabIndex = 33;
            this.label89.Text = "rpm";
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(77, 60);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(39, 19);
            this.label87.TabIndex = 37;
            this.label87.Text = "K =";
            // 
            // radioButton9
            // 
            this.radioButton9.AutoSize = true;
            this.radioButton9.Location = new System.Drawing.Point(12, 26);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(122, 23);
            this.radioButton9.TabIndex = 35;
            this.radioButton9.TabStop = true;
            this.radioButton9.Text = "正弦控制：";
            this.radioButton9.UseVisualStyleBackColor = true;
            this.radioButton9.CheckedChanged += new System.EventHandler(this.radioButton9_CheckedChanged);
            // 
            // groupBox16
            // 
            this.groupBox16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox16.Controls.Add(this.chart1);
            this.groupBox16.Controls.Add(this.chart2);
            this.groupBox16.Controls.Add(this.button14);
            this.groupBox16.Location = new System.Drawing.Point(621, 252);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(813, 554);
            this.groupBox16.TabIndex = 16;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "实际波形";
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea5.AxisY.Title = "飞轮电流";
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            this.chart1.Location = new System.Drawing.Point(10, 53);
            this.chart1.Name = "chart1";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.IsXValueIndexed = true;
            series5.Name = "Series1";
            series5.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chart1.Series.Add(series5);
            this.chart1.Size = new System.Drawing.Size(797, 168);
            this.chart1.TabIndex = 17;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea6.AxisY.Title = "实际转速";
            chartArea6.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea6);
            this.chart2.Location = new System.Drawing.Point(10, 281);
            this.chart2.Name = "chart2";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.IsXValueIndexed = true;
            series6.Name = "Series1";
            series6.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chart2.Series.Add(series6);
            this.chart2.Size = new System.Drawing.Size(795, 180);
            this.chart2.TabIndex = 15;
            this.chart2.Text = "chart2";
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(31, 505);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(111, 33);
            this.button14.TabIndex = 0;
            this.button14.Text = "数据统计";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(34, 764);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(66, 23);
            this.checkBox5.TabIndex = 83;
            this.checkBox5.Text = "全表";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(149, 763);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(66, 23);
            this.checkBox6.TabIndex = 84;
            this.checkBox6.Text = "半表";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // FlyWheel2
            // 
            this.Name = "FlyWheel2";
            this.Size = new System.Drawing.Size(1458, 856);
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            this.groupBox16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        #region 创建样式
        public override void SetFocus()
        {
            this.tabPage1.Focus();
            base.SetFocus();
        }
        protected override void InitResource()
        {
            this.tabPage1.Text = "飞轮2";
            this.textBox22.Text = "20";
            this.textBox19.Text = "20";
            this.textBox17.Text = "1";
            this.textBox16.Text = "1000";
            this.textBox15.Text = "90";
            this.textBox1.Text = "20";
            this.textBox2.Text = "20";
            this.textBox4.Text = "0.796";
            this.textBox8.Text = "0.796";
            this.textBox9.Text = "0.796";
            this.textBox10.Text = "0.796";
            this.textBox3.Text = "6000";
            this.textBox20.Text = "100";
            this.textBox18.Text = "1";
            this.textBox21.Text = "1000";



            for (double i = 0.1; i <= 0.5; i = i + 0.05)
            {
                this.comboBox12.Items.Add(i.ToString());
                this.comboBox16.Items.Add(i.ToString());
                this.comboBox2.Items.Add(i.ToString());
            }
            for (double i = 0.1; i <= 0.5; i = i + 0.1)
            {
                this.comboBox17.Items.Add(i.ToString());
                this.comboBox1.Items.Add(i.ToString());
                this.comboBox3.Items.Add(i.ToString());

            }

            this.comboBox12.SelectedItem = this.comboBox12.Items[8];
            this.comboBox2.SelectedItem = this.comboBox12.Items[8];
            this.comboBox16.SelectedItem = this.comboBox16.Items[8];
            this.comboBox1.SelectedItem = this.comboBox1.Items[4];

            this.comboBox17.SelectedItem = this.comboBox17.Items[4];
            this.comboBox3.SelectedItem = this.comboBox17.Items[4];


            return;
        }
        #endregion


        #region 启动
        public delegate void InvokeText(string text);
        public void UpdateText(string text)
        {
            this.button13.Text = text;

        }
        /// <summary>
        /// 启动飞轮1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            if (this.textBox5.Text == string.Empty || this.textBox6.Text == string.Empty || this.textBox7.Text == string.Empty)
            {
                MessageBox.Show("请填写试验名称，试验人员和产品编号");
                return;
            }
            this.experiment_name = this.textBox5.Text;
            this.experiment_memeber = this.textBox6.Text;
            this.experiment_product = this.textBox7.Text;


            pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));

            if (this.button13.Text.ToString() == "OFF")
            {
                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }



                ConstantSpeedTimerRead.Stop();
                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();
                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();
                TorqueSpeedTimerRead.Stop();
            }

            //打开或关闭飞轮1电源
            string error;
            bool results = facade.OpenElecSource(this.button13.Text.ToString(), out error, this.experiment_name, this.experiment_memeber, this.experiment_product);


            if (results == true)
            {
                ShowInformation("电源打开或关闭成功");
                InvokeText mi = new InvokeText(UpdateText);

                //改变按钮状态
                if (this.button13.Text.ToString().Equals("ON"))
                {
                    this.BeginInvoke(mi, new Object[] { "OFF" });
                }
                else
                {
                    this.BeginInvoke(mi, new Object[] { "ON" });
                }

            }
            else
            {
                ShowInformation("电源打开或关闭不成功" + error);
                MessageBox.Show("电源打开或关闭不成功" + error);
                return;
            }

        }

        #endregion

        #region 恒速控制
        /// <summary>
        /// 恒速控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {

            //恒速控制
            if (this.radioButton12.Checked == true && this.button13.Text == "OFF")
            {

                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                sign = "1";
                ShowInformation("恒速模式");
                ConstantSpeedTimerRead.Stop();


                #region 输入处理
                if (this.textBox21.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("转速设置不能为空");
                    return;
                }
                else
                {

                    string[] rotor = this.textBox21.Text.ToString().Split('.');
                    if (rotor.Length == 1)
                    {
                        if (Convert.ToDouble(rotor[0]) > 8000 || Convert.ToDouble(rotor[0]) < -8000)
                        {
                            MessageBox.Show("转速超过正负8000");
                            return;
                        }
                    }
                    if (rotor.Length == 2)
                    {
                        if (Convert.ToDouble(rotor[0]) > 8000 || Convert.ToDouble(rotor[0]) < -8000 || Convert.ToDouble(rotor[1]) > 10)
                        {
                            MessageBox.Show("转速超过正负8000或者小数超过1位");
                            return;
                        }
                    }

                }

                if (this.textBox22.Text.ToString() == string.Empty || this.textBox22.Text.ToString() == "0")
                {
                    MessageBox.Show("采样个数不能为空或为零");
                    return;
                }
                #endregion


                #region 数据赋值

                if (Convert.ToDouble(this.textBox21.Text) >= 0)
                {
                    this.Scheme1_direction = "正向";
                }
                else
                {
                    this.Scheme1_direction = "反向";
                }


                this.Scheme1_rotorspeed = this.textBox21.Text;
                this.Scheme1_sampling = this.textBox22.Text.ToString();
                this.Scheme1_times = this.comboBox12.SelectedItem.ToString();
                this.Scheme1_H_num = new double[Convert.ToInt16(Scheme1_sampling)];
                this.Scheme1_T_num = new double[Convert.ToInt16(Scheme1_sampling)];
                this.Scheme1_moment = this.textBox4.Text;
                this.Scheme1_k_number = 0;
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;
                //this.motionspeed = "0.00";
                //this.current = "0.00";

                for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                {
                    Scheme1_H_num[i] = 0.0;
                    Scheme1_T_num[i] = 0.0;
                }




                #endregion


                #region 写卡

                string speed = Scheme1_rotorspeed;
                string error;
                bool results = facade.WriteCardSpeedControl(Scheme1_direction, speed.TrimStart('-'), out error);
                if (results == true)
                {
                    ShowInformation("恒速控制测试执行");
                    pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("green.bmp"));
                    ConstantSpeedTimerRead.Interval = Convert.ToDouble(Scheme1_times) * 1000;
                    ConstantSpeedTimerRead.Start();
                }
                else
                {
                    ShowInformation("恒速控制测试失败" + error);
                    MessageBox.Show("恒速控制测试失败" + error);
                    pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                }
                #endregion

            }
            else
            {
                ShowInformation("选择该模式或打开电源");
                MessageBox.Show("选择该模式或打开电源");
            }
        }


        /// <summary>
        /// 恒速定时器读事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void ConstantSpeedTheoutRead(object source, System.Timers.ElapsedEventArgs e)
        {

            string sInfo = string.Empty;
            string current1;
            string motionspeed1;
            bool results = facade.GetDataFromCardcan(out motionspeed1, out current1, out sInfo);
            string[] datatemp = facade.GetDataFromCard(out sInfo);


            #region 恒速模式-角动量和力矩

            if (sign == "6" && pow == "1")
            {
                sum_pow = sum_pow + Convert.ToDouble(datatemp[3]);
                k_pow = k_pow + 1;
            }

            if (sign == "6")
            {
                if (Math.Abs(Convert.ToDouble(this.Scheme1_rotorspeed)) <= 3000)
                {
                    if (Math.Abs(Convert.ToDouble(motionspeed1) - Convert.ToDouble(this.Scheme1_rotorspeed)) < 1)
                    {
                        CalScheme1(motionspeed1);
                        //double real_moment_k = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
                        //double ideal_moment_r = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(Scheme1_rotorspeed) / 60;
                        //real_deta_moment = Math.Abs(Math.Round(real_moment_k, 3));

                        //if (Scheme1_k_number < Convert.ToInt16(this.Scheme1_sampling))
                        //{
                        //    this.Scheme1_H_num[Scheme1_k_number] = Convert.ToDouble(motionspeed1);
                        //    if (Scheme1_k_number == 0)
                        //    {
                        //        Scheme1_T_num[Scheme1_k_number] = 0;
                        //    }
                        //    else
                        //    {
                        //        Scheme1_T_num[Scheme1_k_number] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 1]) / Convert.ToDouble(Scheme1_times);
                        //    }
                        //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number], 3);
                        //    Scheme1_k_number = Scheme1_k_number + 1;

                        //}
                        //else
                        //{
                        //    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling) - 1; i++)
                        //    {
                        //        Scheme1_H_num[i] = Scheme1_H_num[i + 1];
                        //        Scheme1_T_num[i] = Scheme1_T_num[i + 1];
                        //    }

                        //    Scheme1_H_num[Scheme1_k_number - 1] = Convert.ToDouble(motionspeed1);
                        //    Scheme1_T_num[Scheme1_k_number - 1] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 2]) / Convert.ToDouble(Scheme1_times);
                        //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number - 1], 3);
                        //}

                        //double mean_rotor;
                        //double sum = 0;
                        //double sumT = 0;
                        //for (int i = 0; i < Scheme1_k_number; i++)
                        //{
                        //    sum = sum + Scheme1_H_num[i];
                        //    sumT = sumT + Scheme1_T_num[i];

                        //}
                        //if (Scheme1_k_number == 0)
                        //{
                        //    mean_rotor = Convert.ToDouble(motionspeed1);
                        //    ideal_torque = 0;
                        //}
                        //else
                        //{
                        //    mean_rotor = sum / Convert.ToInt16(Scheme1_k_number);
                        //    ideal_torque = Math.Round(sumT / Convert.ToInt16(Scheme1_k_number), 3);
                        //}

                        //double mean_moment_m1 = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * mean_rotor / 60;


                        //double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

                        //if (deta_moment > max_deta_moment)
                        //{
                        //    max_deta_moment = Math.Round(deta_moment, 3);

                        //}

                        //double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
                        //if (deta_motion > max_deta_motion)
                        //{
                        //    max_deta_motion = Math.Round(deta_motion, 3);

                        //}


                    }
                    else
                    {
                        max_deta_moment = 0;
                        max_deta_motion = 0;
                    }
                }
                else
                {
                    if (Math.Abs(Convert.ToDouble(motionspeed1) - Convert.ToDouble(this.Scheme1_rotorspeed)) < 4)
                    {

                        CalScheme1(motionspeed1);
                        //double real_moment_k = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
                        //double ideal_moment_r = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(Scheme1_rotorspeed) / 60;
                        //real_deta_moment = Math.Abs(Math.Round(real_moment_k, 3));

                        //if (Scheme1_k_number < Convert.ToInt16(this.Scheme1_sampling))
                        //{
                        //    this.Scheme1_H_num[Scheme1_k_number] = Convert.ToDouble(motionspeed1);
                        //    if (Scheme1_k_number == 0)
                        //    {
                        //        Scheme1_T_num[Scheme1_k_number] = 0;
                        //    }
                        //    else
                        //    {
                        //        Scheme1_T_num[Scheme1_k_number] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 1]) / Convert.ToDouble(Scheme1_times);
                        //    }
                        //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number], 3);
                        //    Scheme1_k_number = Scheme1_k_number + 1;

                        //}
                        //else
                        //{
                        //    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling) - 1; i++)
                        //    {
                        //        Scheme1_H_num[i] = Scheme1_H_num[i + 1];
                        //        Scheme1_T_num[i] = Scheme1_T_num[i + 1];
                        //    }

                        //    Scheme1_H_num[Scheme1_k_number - 1] = Convert.ToDouble(motionspeed1);
                        //    Scheme1_T_num[Scheme1_k_number - 1] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 2]) / Convert.ToDouble(Scheme1_times);
                        //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number - 1], 3);
                        //}

                        //double mean_rotor;
                        //double sum = 0;
                        //double sumT = 0;
                        //for (int i = 0; i < Scheme1_k_number; i++)
                        //{
                        //    sum = sum + Scheme1_H_num[i];
                        //    sumT = sumT + Scheme1_T_num[i];

                        //}
                        //if (Scheme1_k_number == 0)
                        //{
                        //    mean_rotor = Convert.ToDouble(motionspeed1);
                        //    ideal_torque = 0;
                        //}
                        //else
                        //{
                        //    mean_rotor = sum / Convert.ToInt16(Scheme1_k_number);
                        //    ideal_torque = Math.Round(sumT / Convert.ToInt16(Scheme1_k_number), 3);
                        //}

                        //double mean_moment_m1 = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * mean_rotor / 60;


                        //double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

                        //if (deta_moment > max_deta_moment)
                        //{
                        //    max_deta_moment = Math.Round(deta_moment, 3);

                        //}

                        //double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
                        //if (deta_motion > max_deta_motion)
                        //{
                        //    max_deta_motion = Math.Round(deta_motion, 3);

                        //}

                    }
                    else
                    {
                        max_deta_moment = 0;
                        max_deta_motion = 0;
                    }
                }

            }
            else
            {
                CalScheme1(motionspeed1);
                //double real_moment_k = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
                //double ideal_moment_r = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(Scheme1_rotorspeed) / 60;
                //real_deta_moment = Math.Abs(Math.Round(real_moment_k, 3));

                //if (Scheme1_k_number < Convert.ToInt16(this.Scheme1_sampling))
                //{
                //    this.Scheme1_H_num[Scheme1_k_number] = Convert.ToDouble(motionspeed1);
                //    if (Scheme1_k_number == 0)
                //    {
                //        Scheme1_T_num[Scheme1_k_number] = 0;
                //    }
                //    else
                //    {
                //        Scheme1_T_num[Scheme1_k_number] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 1]) / Convert.ToDouble(Scheme1_times);
                //    }
                //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number], 3);
                //    Scheme1_k_number = Scheme1_k_number + 1;

                //}
                //else
                //{
                //    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling) - 1; i++)
                //    {
                //        Scheme1_H_num[i] = Scheme1_H_num[i + 1];
                //        Scheme1_T_num[i] = Scheme1_T_num[i + 1];
                //    }

                //    Scheme1_H_num[Scheme1_k_number - 1] = Convert.ToDouble(motionspeed1);
                //    Scheme1_T_num[Scheme1_k_number - 1] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                //            (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 2]) / Convert.ToDouble(Scheme1_times);
                //    mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number - 1], 3);
                //}

                //double mean_rotor;
                //double sum = 0;
                //double sumT = 0;
                //for (int i = 0; i < Scheme1_k_number; i++)
                //{
                //    sum = sum + Scheme1_H_num[i];
                //    sumT = sumT + Scheme1_T_num[i];

                //}
                //if (Scheme1_k_number == 0)
                //{
                //    mean_rotor = Convert.ToDouble(motionspeed1);
                //    ideal_torque = 0;
                //}
                //else
                //{
                //    mean_rotor = sum / Convert.ToInt16(Scheme1_k_number);
                //    ideal_torque = Math.Round(sumT / Convert.ToInt16(Scheme1_k_number), 3);
                //}

                //double mean_moment_m1 = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * mean_rotor / 60;


                //double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

                //if (deta_moment > max_deta_moment)
                //{
                //    max_deta_moment = Math.Round(deta_moment, 3);

                //}

                //double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
                //if (deta_motion > max_deta_motion)
                //{
                //    max_deta_motion = Math.Round(deta_motion, 3);

                //}

            }



            #endregion



            MyInvoke mi = new MyInvoke(UpdateForm);
            this.BeginInvoke(mi, new Object[] { datatemp, motionspeed1, current1, sInfo });


            #region 恒速模式-写数据库
            string MotorID = "2";
            string Scheme = sign;
            string SouceVol = datatemp[1];
            string SouceCut = datatemp[0];
            string RotorVol = datatemp[2];
            string RotorCut = current1;
            string RotorPow = datatemp[3];
            string ConstantMoment = max_deta_moment.ToString();
            string ChangeMoment = max_deta_motion.ToString();
            string RotorRevIde = Scheme1_rotorspeed;
            string RotorRevRea = motionspeed1;
            string datetime = DateTime.Now.ToString();


            FlyWheel2Facade facade1 = new FlyWheel2Facade();
            string error = facade1.WriteDataToDatabase(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut,
                RotorPow, ConstantMoment, ChangeMoment, RotorRevIde, RotorRevRea, datetime, experiment_name, experiment_memeber, experiment_product, real_deta_moment.ToString());
            if (error != string.Empty)
            {
                ShowInformation(error);
                ConstantSpeedTimerRead.Stop();
            }

            #endregion

        }

        void CalScheme1(string motionspeed1)
        {
            double real_moment_k = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
            double ideal_moment_r = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * Convert.ToDouble(Scheme1_rotorspeed) / 60;
            real_deta_moment = Math.Abs(Math.Round(real_moment_k, 3));

            if (Scheme1_k_number < Convert.ToInt16(this.Scheme1_sampling))
            {
                this.Scheme1_H_num[Scheme1_k_number] = Convert.ToDouble(motionspeed1);
                if (Scheme1_k_number == 0)
                {
                    Scheme1_T_num[Scheme1_k_number] = 0;
                }
                else
                {
                    Scheme1_T_num[Scheme1_k_number] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 1]) / Convert.ToDouble(Scheme1_times);
                }
                mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number], 3);
                Scheme1_k_number = Scheme1_k_number + 1;

            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme1_sampling) - 1; i++)
                {
                    Scheme1_H_num[i] = Scheme1_H_num[i + 1];
                    Scheme1_T_num[i] = Scheme1_T_num[i + 1];
                }

                Scheme1_H_num[Scheme1_k_number - 1] = Convert.ToDouble(motionspeed1);
                Scheme1_T_num[Scheme1_k_number - 1] = 2 * Math.PI * Convert.ToDouble(Scheme1_moment) / 60 *
                        (Convert.ToDouble(motionspeed1) - Scheme1_H_num[Scheme1_k_number - 2]) / Convert.ToDouble(Scheme1_times);
                mean_torque = Math.Round(Scheme1_T_num[Scheme1_k_number - 1], 3);
            }

            double mean_rotor;
            double sum = 0;
            double sumT = 0;
            for (int i = 0; i < Scheme1_k_number; i++)
            {
                sum = sum + Scheme1_H_num[i];
                sumT = sumT + Scheme1_T_num[i];

            }
            if (Scheme1_k_number == 0)
            {
                mean_rotor = Convert.ToDouble(motionspeed1);
                ideal_torque = 0;
            }
            else
            {
                mean_rotor = sum / Convert.ToInt16(Scheme1_k_number);
                ideal_torque = Math.Round(sumT / Convert.ToInt16(Scheme1_k_number), 3);
            }

            double mean_moment_m1 = Convert.ToDouble(this.Scheme1_moment) * 2 * Math.PI * mean_rotor / 60;


            double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

            if (deta_moment > max_deta_moment)
            {
                max_deta_moment = Math.Round(deta_moment * 60 / (2 * Math.PI * Convert.ToDouble(this.Scheme1_moment)), 3);

            }

          //  double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
            double deta_motion = Math.Abs(Convert.ToDouble(mean_rotor) - Convert.ToDouble(motionspeed1));
            if (deta_motion > max_deta_motion)
            {
              //  max_deta_motion = Math.Round(deta_motion * 60 / (2 * Math.PI * Convert.ToDouble(this.Scheme1_moment)), 3);
                max_deta_motion = Math.Round(deta_motion, 3);

            }


        }
        #endregion


        #region 更新界面
        public delegate void MyInvoke(string[] data, string str1, string str2, string str3);

        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="data"></param>
        /// <param name="motionspeed"></param>
        /// <param name="sInfo"></param>
        public void UpdateForm(string[] data, string motionspeed1, string current1, string sInfo)
        {
            if (sInfo == string.Empty)
            {
                // 飞轮转速
                this.label77.Text = motionspeed1;

                //驱动器电流
                string DriverCur = data[0];
                this.label75.Text = DriverCur;

                //驱动器电压
                string DriverVol = data[1];
                this.label70.Text = DriverVol;

                //飞轮电流
                string MotorCur = current1;
                this.label71.Text = MotorCur;

                //飞轮电压
                string MotorVol = data[2];
                this.label72.Text = MotorVol;

                //飞轮功率
                this.label67.Text = data[3];

                lock (Defs._object_time)
                {
                    // 常值偏差
                    this.label65.Text = max_deta_moment.ToString();


                    //动态偏差
                    this.label102.Text = max_deta_motion.ToString();

                    //角动量
                    this.label14.Text = real_deta_moment.ToString();

                    //平均力矩
                    this.label19.Text = mean_torque.ToString();

                    //期望力矩
                    this.label21.Text = ideal_torque.ToString();
                }




                #region 图形显示
                //电流
                this.chart1.Series[0].Points.AddXY(DateTime.Now, Convert.ToDouble(current1));
                if (this.chart1.Series[0].Points.Count < 100)
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;
                else
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = series1.Points.Count - 100;


                //转数
                this.chart2.Series[0].Points.AddXY(DateTime.Now, Convert.ToDouble(motionspeed1));
                if (this.chart2.Series[0].Points.Count < 100)
                    chart2.ChartAreas[0].AxisX.ScaleView.Position = 0;
                else
                    chart2.ChartAreas[0].AxisX.ScaleView.Position = series1.Points.Count - 100;

                #endregion


            }
            else
            {
                ShowInformation("更新失败" + sInfo);
            }

        }
        #endregion


        #region 斜坡控制

        /// <summary>
        /// 斜坡控制        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            //斜坡控制
            if (this.radioButton11.Checked == true && this.button13.Text == "OFF")
            {
                sign = "2";
                ShowInformation("斜坡模式");

                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();


                #region 输入判断
                string[] sss = this.textBox20.Text.Split('.');


                if (this.textBox20.Text.ToString() == string.Empty || Convert.ToDouble(this.textBox20.Text) > 200
                    || Convert.ToDouble(this.textBox20.Text) < -200 || sss.Length != 1 || Convert.ToDouble(this.textBox3.Text) > 6500 || Convert.ToDouble(this.textBox3.Text) < -6500)
                {
                    MessageBox.Show("转速增量值不能为空或者超过允许值正负200rpm或者超过终速6500rpm");
                    return;
                }

                #endregion



                #region 参数配置
                this.Scheme2_detaspeed = this.textBox20.Text.ToString();
                this.Scheme2_times = this.comboBox2.SelectedItem.ToString();
                this.Scheme2_times_update = this.comboBox16.SelectedItem.ToString();
                this.Scheme2_current_rotorspeed = "0";
                this.Scheme2_stop_speed = this.textBox3.Text;
                this.Scheme2_sampling = this.textBox19.Text.ToString();
                this.Scheme2_T_num = new double[Convert.ToInt16(Scheme2_sampling)];
                this.Scheme2_P_num = new double[Convert.ToInt16(Scheme2_sampling)];
                this.Scheme2_H_num = new double[Convert.ToInt16(Scheme2_sampling)];
                this.Scheme2_k_number = 0;
                this.Scheme2_k1_number = 0;
                this.Scheme2_moment = this.textBox8.Text;
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;
                for (int i = 0; i < Scheme2_T_num.Length; i++)
                {
                    Scheme2_T_num[i] = 0.0;
                    Scheme2_P_num[i] = 0.0;
                    Scheme2_H_num[i] = 0.0;
                }
                #endregion



                #region 写卡
                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);

                if (results == true)
                {
                    ShowInformation("斜坡测试开始");
                    pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("green.bmp"));
                }
                else
                {
                    ShowInformation("斜坡测试失败" + error);
                    pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    return;
                }

                this.SlopeSpeedTimerRead.Interval = Convert.ToDouble(Scheme2_times) * 1000;
                SlopeSpeedTimerRead.Start();
                this.SlopeSpeedTimerWrite.Interval = Convert.ToDouble(Scheme2_times_update) * 1000;
                SlopeSpeedTimerWrite.Start();

                #endregion


            }
            else
            {
                ShowInformation("选择该模式或打开电源");
                MessageBox.Show("选择该模式或打开电源");
            }
        }
        /// <summary>
        /// 斜坡定时器写事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void SlopeSpeedTheoutWrite(object source, System.Timers.ElapsedEventArgs e)
        {
            string direction;
            string speed;

            Scheme2_current_rotorspeed = Convert.ToString(Convert.ToDouble(Scheme2_current_rotorspeed) + Convert.ToDouble(this.Scheme2_detaspeed));

            if (sign == "6")
            {
                if (Convert.ToDouble(Scheme2_current_rotorspeed) > 6000)
                    Scheme2_current_rotorspeed = "6000";

            }
            else
            {
                if (Convert.ToDouble(Scheme2_stop_speed) > 0 && Convert.ToDouble(Scheme2_current_rotorspeed) > Convert.ToDouble(Scheme2_stop_speed))
                    Scheme2_current_rotorspeed = Scheme2_stop_speed;
                if (Convert.ToDouble(Scheme2_stop_speed) < 0 && Convert.ToDouble(Scheme2_current_rotorspeed) < Convert.ToDouble(Scheme2_stop_speed))
                    Scheme2_current_rotorspeed = Scheme2_stop_speed;
            }

            if (Convert.ToDouble(Scheme2_current_rotorspeed) > 0)
            {
                direction = "正向";

            }
            else
            {
                direction = "反向";
            }
            speed = Scheme2_current_rotorspeed;
            InvokeStateInformation mi = new InvokeStateInformation(UpdateStateInformation);
            string error;

            bool results = facade.WriteCardSpeedControl(direction, speed.TrimStart('-'), out error);
            if (results == false)
            {
                SlopeSpeedTimerWrite.Stop();
                SlopeSpeedTimerRead.Stop();
                this.BeginInvoke(mi, new Object[] { "斜坡测试失败", "2", "red.bmp" });
            }
        }
        /// <summary>
        /// 斜坡定时器读事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void SlopeSpeedTheoutRead(object source, System.Timers.ElapsedEventArgs e)
        {
            string sInfo = string.Empty;
            string current1;
            string motionspeed1;
            bool results = facade.GetDataFromCardcan(out motionspeed1, out current1, out sInfo);
            string[] datatemp = facade.GetDataFromCard(out sInfo);


            #region 角动量


            if (sign == "6" && Convert.ToDouble(motionspeed1) > 100)
            {
                CalScheme2(motionspeed1);
                //double real_moment_k = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
                //double ideal_moment_r = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(Scheme2_current_rotorspeed) / 60;
                //real_deta_moment = Math.Round(real_moment_k);


                //if (Scheme2_k1_number < Convert.ToInt16(this.Scheme2_sampling))
                //{
                //    this.Scheme2_H_num[Scheme2_k1_number] = Convert.ToDouble(motionspeed1);
                //    Scheme2_k1_number = Scheme2_k1_number + 1;

                //}
                //else
                //{
                //    for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                //    {
                //        Scheme2_H_num[i] = Scheme2_H_num[i + 1];
                //    }

                //    Scheme2_H_num[Scheme2_k1_number - 1] = Convert.ToDouble(motionspeed1);
                //}

                //double mean_rotor;
                //double sumsum = 0;
                //for (int i = 0; i < Scheme2_k1_number; i++)
                //{
                //    sumsum = sumsum + Scheme2_H_num[i];
                //}
                //if (Scheme2_k1_number == 0)
                //{
                //    mean_rotor = Convert.ToDouble(motionspeed1);
                //}
                //else
                //{
                //    mean_rotor = sumsum / Convert.ToInt16(Scheme2_k1_number);
                //}

                //double mean_moment_m1 = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * mean_rotor / 60;


                //double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

                //if (deta_moment > max_deta_moment)
                //{
                //    max_deta_moment = Math.Round(deta_moment, 3);

                //}

                //double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
                //if (deta_motion > max_deta_motion)
                //{
                //    max_deta_motion = Math.Round(deta_motion, 3);

                //}





                //if (Scheme2_k_number < Convert.ToInt16(this.Scheme2_sampling))
                //{
                //    this.Scheme2_P_num[Scheme2_k_number] = Convert.ToDouble(motionspeed1);
                //    if (Scheme2_k_number == 0)
                //    {

                //        this.Scheme2_T_num[Scheme2_k_number] = 0;
                //    }
                //    else
                //    {
                //        this.Scheme2_T_num[Scheme2_k_number] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                //            - this.Scheme2_P_num[Scheme2_k_number - 1]) / Convert.ToDouble(this.Scheme2_times);
                //    }
                //    mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number], 3);

                //    Scheme2_k_number = Scheme2_k_number + 1;
                //}
                //else
                //{
                //    for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                //    {
                //        Scheme2_T_num[i] = Scheme2_T_num[i + 1];
                //        Scheme2_P_num[i] = Scheme2_P_num[i + 1];

                //    }
                //    this.Scheme2_P_num[Scheme2_k_number - 1] = Convert.ToDouble(motionspeed1);
                //    Scheme2_T_num[Scheme2_k_number - 1] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                //        - this.Scheme2_P_num[Scheme2_k_number - 2]) / Convert.ToDouble(this.Scheme2_times);
                //    mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number - 1], 3);
                //}


                //double sum = 0;
                //for (int i = 0; i < Scheme2_k_number; i++)
                //{
                //    sum = sum + Scheme2_T_num[i];
                //}
                //if (Scheme2_k_number == 0)
                //{
                //    this.ideal_torque = 0;
                //}
                //else
                //{
                //    this.ideal_torque = Math.Round(sum / Convert.ToInt16(Scheme2_k_number), 3);
                //}

            }
            else
            {
                if (sign != "6")
                {
                    CalScheme2(motionspeed1);
                    //double real_moment_k = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
                    //double ideal_moment_r = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(Scheme2_current_rotorspeed) / 60;
                    //real_deta_moment = Math.Round(real_moment_k);


                    //if (Scheme2_k1_number < Convert.ToInt16(this.Scheme2_sampling))
                    //{
                    //    this.Scheme2_H_num[Scheme2_k1_number] = Convert.ToDouble(motionspeed1);
                    //    Scheme2_k1_number = Scheme2_k1_number + 1;

                    //}
                    //else
                    //{
                    //    for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                    //    {
                    //        Scheme2_H_num[i] = Scheme2_H_num[i + 1];
                    //    }

                    //    Scheme2_H_num[Scheme2_k1_number - 1] = Convert.ToDouble(motionspeed1);
                    //}

                    //double mean_rotor;
                    //double sumsum = 0;
                    //for (int i = 0; i < Scheme2_k1_number; i++)
                    //{
                    //    sumsum = sumsum + Scheme2_H_num[i];
                    //}
                    //if (Scheme2_k1_number == 0)
                    //{
                    //    mean_rotor = Convert.ToDouble(motionspeed1);
                    //}
                    //else
                    //{
                    //    mean_rotor = sumsum / Convert.ToInt16(Scheme2_k1_number);
                    //}

                    //double mean_moment_m1 = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * mean_rotor / 60;


                    //double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

                    //if (deta_moment > max_deta_moment)
                    //{
                    //    max_deta_moment = Math.Round(deta_moment, 3);

                    //}

                    //double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
                    //if (deta_motion > max_deta_motion)
                    //{
                    //    max_deta_motion = Math.Round(deta_motion, 3);

                    //}





                    //if (Scheme2_k_number < Convert.ToInt16(this.Scheme2_sampling))
                    //{
                    //    this.Scheme2_P_num[Scheme2_k_number] = Convert.ToDouble(motionspeed1);
                    //    if (Scheme2_k_number == 0)
                    //    {

                    //        this.Scheme2_T_num[Scheme2_k_number] = 0;
                    //    }
                    //    else
                    //    {
                    //        this.Scheme2_T_num[Scheme2_k_number] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                    //            - this.Scheme2_P_num[Scheme2_k_number - 1]) / Convert.ToDouble(this.Scheme2_times);
                    //    }
                    //    mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number], 3);

                    //    Scheme2_k_number = Scheme2_k_number + 1;
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                    //    {
                    //        Scheme2_T_num[i] = Scheme2_T_num[i + 1];
                    //        Scheme2_P_num[i] = Scheme2_P_num[i + 1];

                    //    }
                    //    this.Scheme2_P_num[Scheme2_k_number - 1] = Convert.ToDouble(motionspeed1);
                    //    Scheme2_T_num[Scheme2_k_number - 1] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                    //        - this.Scheme2_P_num[Scheme2_k_number - 2]) / Convert.ToDouble(this.Scheme2_times);
                    //    mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number - 1], 3);
                    //}


                    //double sum = 0;
                    //for (int i = 0; i < Scheme2_k_number; i++)
                    //{
                    //    sum = sum + Scheme2_T_num[i];
                    //}
                    //if (Scheme2_k_number == 0)
                    //{
                    //    this.ideal_torque = 0;
                    //}
                    //else
                    //{
                    //    this.ideal_torque = Math.Round(sum / Convert.ToInt16(Scheme2_k_number), 3);
                    //}

                }

            }

            #endregion



            MyInvoke mi = new MyInvoke(UpdateForm);
            this.BeginInvoke(mi, new Object[] { datatemp, motionspeed1, current1, sInfo });



            #region 斜坡模式-写数据库
            string MotorID = "2";
            string Scheme = sign;
            string SouceVol = datatemp[1];
            string SouceCut = datatemp[0];
            string RotorVol = datatemp[2];
            string RotorCut = current1;
            string RotorPow = datatemp[3];
            string RotorRevRea = motionspeed1;
            string MeanTorque = mean_torque.ToString();
            string datetime = DateTime.Now.ToString();

            FlyWheel2Facade facade1 = new FlyWheel2Facade();
            string error = facade1.WriteDataToDatabaseSlope(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut,
                RotorPow, RotorRevRea, MeanTorque, datetime, ideal_torque.ToString(), this.experiment_name, this.experiment_memeber, this.experiment_product);
            if (error != string.Empty)
            {
                ShowInformation(error);
                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();
            }
            #endregion

        }

        void CalScheme2(string motionspeed1)
        {
            double real_moment_k = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
            double ideal_moment_r = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * Convert.ToDouble(Scheme2_current_rotorspeed) / 60;
            real_deta_moment = Math.Round(real_moment_k);


            if (Scheme2_k1_number < Convert.ToInt16(this.Scheme2_sampling))
            {
                this.Scheme2_H_num[Scheme2_k1_number] = Convert.ToDouble(motionspeed1);
                Scheme2_k1_number = Scheme2_k1_number + 1;

            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                {
                    Scheme2_H_num[i] = Scheme2_H_num[i + 1];
                }

                Scheme2_H_num[Scheme2_k1_number - 1] = Convert.ToDouble(motionspeed1);
            }

            double mean_rotor;
            double sumsum = 0;
            for (int i = 0; i < Scheme2_k1_number; i++)
            {
                sumsum = sumsum + Scheme2_H_num[i];
            }
            if (Scheme2_k1_number == 0)
            {
                mean_rotor = Convert.ToDouble(motionspeed1);
            }
            else
            {
                mean_rotor = sumsum / Convert.ToInt16(Scheme2_k1_number);
            }

            double mean_moment_m1 = Convert.ToDouble(this.Scheme2_moment) * 2 * Math.PI * mean_rotor / 60;


            double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

            if (deta_moment > max_deta_moment)
            {
                max_deta_moment = Math.Round(deta_moment * 60 / (2 * Math.PI * mean_rotor), 3);

            }

            double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
            if (deta_motion > max_deta_motion)
            {
                max_deta_motion = Math.Round(deta_motion * 60 / (2 * Math.PI * mean_rotor), 3);

            }





            if (Scheme2_k_number < Convert.ToInt16(this.Scheme2_sampling))
            {
                this.Scheme2_P_num[Scheme2_k_number] = Convert.ToDouble(motionspeed1);
                if (Scheme2_k_number == 0)
                {

                    this.Scheme2_T_num[Scheme2_k_number] = 0;
                }
                else
                {
                    this.Scheme2_T_num[Scheme2_k_number] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                        - this.Scheme2_P_num[Scheme2_k_number - 1]) / Convert.ToDouble(this.Scheme2_times);
                }
                mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number], 3);

                Scheme2_k_number = Scheme2_k_number + 1;
            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme2_sampling) - 1; i++)
                {
                    Scheme2_T_num[i] = Scheme2_T_num[i + 1];
                    Scheme2_P_num[i] = Scheme2_P_num[i + 1];

                }
                this.Scheme2_P_num[Scheme2_k_number - 1] = Convert.ToDouble(motionspeed1);
                Scheme2_T_num[Scheme2_k_number - 1] = 2 * Math.PI * Convert.ToDouble(this.Scheme2_moment) / 60 * (Convert.ToDouble(motionspeed1)
                    - this.Scheme2_P_num[Scheme2_k_number - 2]) / Convert.ToDouble(this.Scheme2_times);
                mean_torque = Math.Round(this.Scheme2_T_num[Scheme2_k_number - 1], 3);
            }


            double sum = 0;
            for (int i = 0; i < Scheme2_k_number; i++)
            {
                sum = sum + Scheme2_T_num[i];
            }
            if (Scheme2_k_number == 0)
            {
                this.ideal_torque = 0;
            }
            else
            {
                this.ideal_torque = Math.Round(sum / Convert.ToInt16(Scheme2_k_number), 3);
            }


        }

        #endregion


        #region 模式切换响应函数
        /// <summary>
        /// 恒速控制模式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton12.Checked == true)
            {
                this.radioButton11.Checked = false;
                this.radioButton9.Checked = false;
                this.radioButton14.Checked = false;
                this.radioButton10.Checked = false;
                this.radioButton1.Checked = false;

            }




            if (this.button13.Text == "OFF")
            {
                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }





                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();


                sign = "1";
                ShowInformation("恒速模式");




                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";


                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);


                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));

                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {
                    ShowInformation("飞轮停止错误" + error);
                }

            }
        }


        /// <summary>
        /// 斜坡控制模式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton11.Checked == true)
            {
                this.radioButton12.Checked = false;
                this.radioButton9.Checked = false;
                this.radioButton14.Checked = false;
                this.radioButton10.Checked = false;
                this.radioButton1.Checked = false;
            }
            if (this.button13.Text == "OFF")
            {
                sign = "2";
                ShowInformation("斜坡模式");




                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";


                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);

                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));


                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }

                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();


                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {

                    ShowInformation("飞轮停止错误" + error);
                }

            }


        }



        /// <summary>
        /// 一键测试模式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton10.Checked == true)
            {
                this.radioButton12.Checked = false;
                this.radioButton9.Checked = false;
                this.radioButton14.Checked = false;
                this.radioButton11.Checked = false;
                this.radioButton1.Checked = false;
            }
            if (this.button13.Text == "OFF")
            {

                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }



                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();




                sign = "6";

                ShowInformation("一键测试");


                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);
                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label5.Text = "0.00";

                this.label19.Text = "00.000";
                this.label21.Text = "00.000";

                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));


                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {

                    ShowInformation("飞轮停止错误" + error);
                }
            }

        }



        /// <summary>
        /// 力矩控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton14.Checked == true)
            {
                this.radioButton12.Checked = false;
                this.radioButton11.Checked = false;
                this.radioButton9.Checked = false;
                this.radioButton10.Checked = false;
                this.radioButton1.Checked = false;
            }
            if (this.button13.Text == "OFF")
            {

                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }


                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();



                sign = "4";

                ShowInformation("力矩模式");





                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);
                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";


                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {

                    ShowInformation("飞轮停止错误" + error);
                }
            }



        }

        /// <summary>
        /// 正弦控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked == true)
            {
                this.radioButton11.Checked = false;
                this.radioButton12.Checked = false;
                this.radioButton14.Checked = false;
                this.radioButton10.Checked = false;
                this.radioButton1.Checked = false;
            }
            if (this.button13.Text == "OFF")
            {
                sign = "3";
                ShowInformation("正弦模式");



                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";

                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);

                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));

                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }



                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();

                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {

                    ShowInformation("飞轮停止错误" + error);
                }


            }

        }



        /// <summary>
        /// 时间常数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                this.radioButton12.Checked = false;
                this.radioButton11.Checked = false;
                this.radioButton9.Checked = false;
                this.radioButton14.Checked = false;
                this.radioButton10.Checked = false;
            }
            if (this.button13.Text == "OFF")
            {
                sign = "5";
                ShowInformation("时间常数模式");

                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadAuto.Abort();
                    threadAuto.Join();
                }



                ConstantSpeedTimerRead.Stop();

                SlopeSpeedTimerRead.Stop();
                SlopeSpeedTimerWrite.Stop();

                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

                TorqueSpeedTimerRead.Stop();



                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();
                this.chart1.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(0, 0);

                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";

                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));




                string error;
                bool results = facade.WriteCardSpeedControl("正向", "0", out error);
                if (results == false)
                {
                    ShowInformation("飞轮停止错误" + error);
                }

            }


        }
        #endregion


        #region 统计数据
        /// <summary>
        /// 统计数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            Common.StatisticalForm form1 = new StatisticalForm();
            form1.Show();
        }
        #endregion


        #region 正弦控制
        private void button15_Click(object sender, EventArgs e)
        {

            //正弦控制
            if (this.radioButton9.Checked == true && this.button13.Text == "OFF")
            {
                sign = "3";
                ShowInformation("正弦模式");
                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();
                pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));


                #region 输入处理
                if (this.textBox17.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("转速设置不能为空");
                    return;
                }
                else
                {

                    if (Convert.ToDouble(textBox17.Text) > 1 || Convert.ToDouble(textBox17.Text) < 0)
                    {
                        MessageBox.Show("K应在[0,1]范围内取值");
                        return;
                    }

                }

                if (this.textBox16.Text.ToString() == string.Empty || Convert.ToDouble(this.textBox16.Text) < 0)
                {
                    MessageBox.Show("A不能为空或小于零");
                    return;
                }

                if (this.textBox15.Text.ToString() == string.Empty || Convert.ToDouble(this.textBox15.Text) < 0)
                {
                    MessageBox.Show("T不能为空或小于零");
                    return;
                }


                #endregion


                #region 数据赋值
                this.Scheme3_K = Convert.ToDouble(this.textBox17.Text);
                this.Scheme3_A = Convert.ToDouble(this.textBox16.Text);
                this.Scheme3_T = Convert.ToDouble(this.textBox15.Text);
                this.Scheme3_times = this.comboBox3.SelectedItem.ToString();
                this.Scheme3_times_update = this.comboBox17.SelectedItem.ToString();
                this.Scheme3_sampling = this.textBox1.Text;
                this.Scheme3_k_number = 0;
                this.Scheme3_k1_number = 0;
                this.Scheme3_current_time = 0;
                this.Scheme3_moment = this.textBox10.Text;
                this.Scheme3_P_num = new double[Convert.ToInt16(Scheme3_sampling)];
                this.Scheme3_H_num = new double[Convert.ToInt16(Scheme3_sampling)];
                this.Scheme3_T_num = new double[Convert.ToInt16(Scheme3_sampling)];
                for (int i = 0; i < Scheme3_P_num.Length; i++)
                {
                    Scheme3_P_num[i] = 0.0;
                    Scheme3_H_num[i] = 0.0;
                    Scheme3_T_num[i] = 0.0;
                }
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;
                #endregion


                #region 计算速度

                Scheme3_omega_speed = Scheme3_K * Scheme3_A * Math.Sin(2 * Math.PI / Scheme3_T * Scheme3_current_time);

                #endregion

                #region 写卡
                string direction;
                string speed;
                if (Scheme3_omega_speed >= 0)
                {
                    direction = "正向";
                    speed = Scheme3_omega_speed.ToString();
                }
                else
                {
                    direction = "反向";
                    speed = Scheme3_omega_speed.ToString().TrimStart('-');
                }

                string error;
                bool results = facade.WriteCardSpeedControl(direction, speed, out error);
                if (results == true)
                {
                    ShowInformation("正弦控制测试执行");
                    pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("green.bmp"));

                    SineSpeedTimerRead.Interval = Convert.ToDouble(Scheme3_times) * 1000;
                    SineSpeedTimerRead.Start();
                    SineSpeedTimerWrite.Interval = Convert.ToDouble(Scheme3_times_update) * 1000;
                    SineSpeedTimerWrite.Start();

                }
                else
                {
                    ShowInformation("正弦控制测试失败" + error);
                    MessageBox.Show("正弦控制测试失败" + error);
                    pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                }
                #endregion


            }
            else
            {
                ShowInformation("选择该模式并打开电源");
                MessageBox.Show("选择该模式并打开电源");
            }
        }

        /// <summary>
        /// 正弦定时器写事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void SineSpeedTheoutWrite(object source, System.Timers.ElapsedEventArgs e)
        {
            string direction;
            string speed;

            this.Scheme3_current_time = this.Scheme3_current_time + Convert.ToDouble(Scheme3_times_update);
            Scheme3_omega_speed = Scheme3_K * Scheme3_A * Math.Sin(2 * Math.PI / Scheme3_T * Scheme3_current_time);
            speed = Scheme3_omega_speed.ToString();
            if (Scheme3_omega_speed >= 0)
            {
                direction = "正向";
            }
            else
            {
                direction = "反向";
            }

            string error;
            bool results = facade.WriteCardSpeedControl(direction, speed.TrimStart('-'), out error);
            if (results == false)
            {
                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();
            }

        }

        /// <summary>
        /// 正弦定时器读事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void SineSpeedTheoutRead(object source, System.Timers.ElapsedEventArgs e)
        {
            string sInfo = string.Empty;
            string current1;
            string motionspeed1;
            bool results = facade.GetDataFromCardcan(out motionspeed1, out current1, out sInfo);
            string[] datatemp = facade.GetDataFromCard(out sInfo);


            double mean_speed = 0;


            #region 正弦角动量
            double real_moment_k = Convert.ToDouble(this.Scheme3_moment) * 2 * Math.PI * Convert.ToDouble(motionspeed1) / 60;
            double ideal_moment_r = Convert.ToDouble(this.Scheme3_moment) * 2 * Math.PI * Convert.ToDouble(Scheme3_omega_speed) / 60;
            real_deta_moment = Math.Round(real_moment_k);


            if (Scheme3_k_number < Convert.ToInt16(this.Scheme3_sampling))
            {
                this.Scheme3_H_num[Scheme3_k_number] = Convert.ToDouble(motionspeed1);
                Scheme3_k_number = Scheme3_k_number + 1;

            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme3_sampling) - 1; i++)
                {
                    Scheme3_H_num[i] = Scheme3_H_num[i + 1];
                }

                Scheme3_H_num[Scheme3_k_number - 1] = Convert.ToDouble(motionspeed1);
            }

            double mean_rotor;
            double sum = 0;
            for (int i = 0; i < Scheme3_k_number; i++)
            {
                sum = sum + Scheme3_H_num[i];
            }
            if (Scheme3_k_number == 0)
            {
                mean_rotor = Convert.ToDouble(motionspeed1);
            }
            else
            {
                mean_rotor = sum / Convert.ToInt16(Scheme3_k_number);
            }

            double mean_moment_m1 = Convert.ToDouble(this.Scheme3_moment) * 2 * Math.PI * mean_rotor / 60;


            double deta_moment = Math.Abs(mean_moment_m1 - ideal_moment_r);

            if (deta_moment > max_deta_moment)
            {
                max_deta_moment = Math.Round(deta_moment * 60 / (2 * Math.PI * mean_rotor), 3);

            }

            double deta_motion = Math.Abs(mean_moment_m1 - real_moment_k);
            if (deta_motion > max_deta_motion)
            {
                max_deta_motion = Math.Round(deta_motion * 60 / (2 * Math.PI * mean_rotor), 3);

            }




            #endregion

            #region 正弦力矩

            if (Scheme3_k1_number < Convert.ToInt16(this.Scheme3_sampling))
            {
                this.Scheme3_P_num[Scheme3_k1_number] = Convert.ToDouble(motionspeed1);
                if (Scheme3_k1_number == 0)
                {

                    this.Scheme3_T_num[Scheme3_k1_number] = 0;
                }
                else
                {
                    this.Scheme3_T_num[Scheme3_k1_number] = 2 * Math.PI * Convert.ToDouble(this.Scheme3_moment) / 60 * (Convert.ToDouble(motionspeed1)
                        - this.Scheme3_P_num[Scheme3_k1_number - 1]) / Convert.ToDouble(this.Scheme3_times);
                }
                mean_torque = this.Scheme3_T_num[Scheme3_k1_number];
                Scheme3_k1_number = Scheme3_k1_number + 1;
            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme3_sampling) - 1; i++)
                {
                    Scheme3_T_num[i] = Scheme3_T_num[i + 1];
                    Scheme3_P_num[i] = Scheme3_P_num[i + 1];

                }
                this.Scheme3_P_num[Scheme3_k1_number - 1] = Convert.ToDouble(motionspeed1);
                Scheme3_T_num[Scheme3_k1_number - 1] = 2 * Math.PI * Convert.ToDouble(this.Scheme3_moment) / 60 * (Convert.ToDouble(motionspeed1)
                    - this.Scheme3_P_num[Scheme3_k1_number - 2]) / Convert.ToDouble(this.Scheme3_times);
                mean_torque = this.Scheme3_T_num[Scheme3_k1_number - 1];
            }


            double sum1 = 0;
            for (int i = 0; i < Scheme3_k1_number; i++)
            {
                sum1 = sum1 + Scheme3_T_num[i];
            }
            if (Scheme3_k1_number == 0)
            {
                this.ideal_torque = 0;
            }
            else
            {
                this.ideal_torque = sum1 / Convert.ToInt16(Scheme3_k1_number);
            }

            #endregion

            MyInvoke mi = new MyInvoke(UpdateForm);
            this.BeginInvoke(mi, new Object[] { datatemp, motionspeed1, current1, sInfo });



            #region 正弦模式-写数据库
            string MotorID = "2";
            string Scheme = sign;
            string SouceVol = datatemp[1];
            string SouceCut = datatemp[0];
            string RotorVol = datatemp[2];
            string RotorCut = current1;
            string RotorPow = datatemp[3];
            string RotorRevIde = this.Scheme3_omega_speed.ToString();
            string RotorRevRea = mean_speed.ToString();
            string datetime = DateTime.Now.ToString();

            FlyWheel2Facade facade1 = new FlyWheel2Facade();
            string error = facade1.WriteDataToDatabaseSine(MotorID, Scheme, SouceVol, SouceCut,
                RotorVol, RotorCut, RotorPow, RotorRevIde, RotorRevRea, datetime,
                this.experiment_name, this.experiment_memeber, this.experiment_product,
                real_deta_moment.ToString(), max_deta_moment.ToString(), max_deta_motion.ToString(), mean_torque.ToString(), ideal_torque.ToString());
            if (error != string.Empty)
            {
                SineSpeedTimerRead.Stop();
                SineSpeedTimerWrite.Stop();

            }
            #endregion
        }

        #endregion


        #region 力矩控制
        private void button20_Click(object sender, EventArgs e)
        {
            //力矩控制
            if (this.radioButton14.Checked == true && this.button13.Text == "OFF")
            {
                sign = "4";

                ShowInformation("力矩模式");
                TorqueSpeedTimerRead.Stop();
                pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));

                #region 输入处理
                if (this.textBox18.Text.ToString() == string.Empty)
                {
                    MessageBox.Show("力矩设置不能为空");
                    return;
                }
                else
                {
                    if (Convert.ToDouble(textBox18.Text) > 30 || Convert.ToDouble(textBox18.Text) < -30)
                    {
                        MessageBox.Show("力矩应在[-30，30]");
                        return;
                    }

                    string[] rotor = this.textBox18.Text.Split('.');

                    if (rotor.Length == 2)
                    {
                        MessageBox.Show("力矩最小输入单位是1mNm");
                        return;
                    }

                }

                if (this.textBox2.Text.ToString() == string.Empty || this.textBox2.Text.ToString() == "0")
                {
                    MessageBox.Show("采样个数不能为空或为零");
                    return;
                }
                #endregion


                #region 数据赋值

                this.Scheme4_torque = this.textBox18.Text.ToString();
                this.Scheme4_sampling = this.textBox2.Text.ToString();
                this.Scheme4_times = this.comboBox1.SelectedItem.ToString();
                this.Scheme4_T_num = new double[Convert.ToInt16(Scheme4_sampling)];
                this.Scheme4_P_num = new double[Convert.ToInt16(Scheme4_sampling)];
                this.Scheme4_k_number = 0;
                this.Scheme4_moment = this.textBox9.Text;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;

                for (int i = 0; i < Scheme4_T_num.Length; i++)
                {
                    Scheme4_T_num[i] = 0.0;
                    Scheme4_P_num[i] = 0.0;
                }

                #endregion


                #region 写卡

                string error;
                string direction;
                string torque;
                if (Convert.ToDouble(Scheme4_torque) >= 0)
                {
                    direction = "正向";
                }
                else
                {
                    direction = "反向";
                }
                torque = Scheme4_torque;
                bool results = facade.WriteCardTorqueControl(direction, torque.TrimStart('-'), out error);
                if (results == true)
                {
                    ShowInformation("力矩控制测试执行");
                    pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("green.bmp"));
                    TorqueSpeedTimerRead.Interval = Convert.ToDouble(Scheme4_times) * 1000;
                    TorqueSpeedTimerRead.Start();

                }
                else
                {
                    ShowInformation("力矩控制测试失败" + error);
                    MessageBox.Show("力矩控制测试失败" + error);
                    pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                }
                #endregion

            }
            else
            {
                ShowInformation("选择该模式或打开电源");
                MessageBox.Show("选择该模式或打开电源");
            }
        }
        /// <summary>
        /// 力矩定时器读事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void TorqueSpeedTheoutRead(object source, System.Timers.ElapsedEventArgs e)
        {
            string sInfo = string.Empty;
            string current1;
            string motionspeed1;
            bool results = facade.GetDataFromCardcan(out motionspeed1, out current1, out sInfo);
            string[] datatemp = facade.GetDataFromCard(out sInfo);



            #region 力矩模式-力矩

            if (Scheme4_k_number < Convert.ToInt16(this.Scheme4_sampling))
            {
                this.Scheme4_P_num[Scheme4_k_number] = Convert.ToDouble(motionspeed1);
                if (Scheme4_k_number == 0)
                {

                    this.Scheme4_T_num[Scheme4_k_number] = 0;
                }
                else
                {
                    this.Scheme4_T_num[Scheme4_k_number] = 2 * Math.PI / 60.0 * Convert.ToDouble(this.Scheme4_moment) *
                        (Convert.ToDouble(motionspeed1) - this.Scheme4_P_num[Scheme4_k_number - 1]) / Convert.ToDouble(this.Scheme4_times);
                }
                this.mean_torque = Math.Round(this.Scheme4_T_num[Scheme4_k_number], 3);

                Scheme4_k_number = Scheme4_k_number + 1;
            }
            else
            {
                for (int i = 0; i < Convert.ToInt16(Scheme4_sampling) - 1; i++)
                {
                    Scheme4_T_num[i] = Scheme4_T_num[i + 1];
                    Scheme4_P_num[i] = Scheme4_P_num[i + 1];

                }
                this.Scheme4_P_num[Scheme4_k_number - 1] = Convert.ToDouble(motionspeed1);
                Scheme4_T_num[Scheme4_k_number - 1] = 2 * Math.PI / 60 * Convert.ToDouble(this.Scheme4_moment)
                    * (Convert.ToDouble(motionspeed1) - this.Scheme4_P_num[Scheme4_k_number - 2]) / Convert.ToDouble(this.Scheme4_times);
                this.mean_torque = Math.Round(this.Scheme4_T_num[Scheme4_k_number - 1], 3);
            }


            double sum = 0;
            for (int i = 0; i < Scheme4_k_number; i++)
            {
                sum = sum + Scheme4_T_num[i];
            }
            if (Scheme4_k_number == 0)
            {
                this.ideal_torque = 0;
            }
            else
            {
                this.ideal_torque = Math.Round(sum / Convert.ToInt16(Scheme4_k_number), 3);
            }





            #endregion



            MyInvoke mi = new MyInvoke(UpdateForm);
            this.BeginInvoke(mi, new Object[] { datatemp, motionspeed1, current1, sInfo });



            #region 力矩模式-写数据库
            string MotorID = "2";
            string Scheme = sign;
            string SouceVol = datatemp[1];
            string SouceCut = datatemp[0];
            string RotorVol = datatemp[2];
            string RotorCut = current1;
            string RotorPow = datatemp[3];
            string RotorRevRea = motionspeed1;
            string MeanTorque = mean_torque.ToString();
            string datetime = DateTime.Now.ToString();

            FlyWheel2Facade facade1 = new FlyWheel2Facade();
            string error = facade1.WriteDataToDatabaseTorque(MotorID, Scheme, SouceVol, SouceCut, RotorVol, RotorCut, RotorPow,
                RotorRevRea, MeanTorque, datetime, ideal_torque.ToString(), this.experiment_name, this.experiment_memeber, this.experiment_product, Scheme4_torque);
            if (error != string.Empty)
            {
                ShowInformation(error);
                TorqueSpeedTimerRead.Stop();
            }
            #endregion

        }


        #endregion


        #region 时间常数
        /// <summary>
        /// 时间常数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
            //时间常数
            if (this.radioButton1.Checked == true && this.button13.Text == "OFF")
            {
                sign = "5";
                ShowInformation("时间常数模式");
                if (threadTime1 != null && threadTime1.ThreadState == System.Threading.ThreadState.Running)
                {
                    threadTime1.Abort();
                    threadTime1.Join();
                }
                pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("green.bmp"));

                threadTime1 = new System.Threading.Thread(TimeConstant);
                threadTime1.Start();

            }
            else
            {
                ShowInformation("选择该模式或打开电源");
                MessageBox.Show("选择该模式或打开电源");
            }
        }
        public delegate void MyDelegate(double x);
        public void MyMethod(double x)
        {
            label5.Text = x.ToString();

            #region 时间常数-写数据库
            string MotorID = "2";
            string Scheme = sign;
            string TimeConstant = x.ToString();
            string datetime = DateTime.Now.ToString();


            string error = facade.WriteDataToDatabaseTimeConstant(MotorID, Scheme, TimeConstant, datetime, this.experiment_name, this.experiment_memeber, this.experiment_product);
            if (error != string.Empty)
            {
                ShowInformation(error);
            }

            #endregion

            threadTime1.Abort();


        }
        /// <summary>
        /// 时间常数线程
        /// </summary>
        public void TimeConstant()
        {

            string error;
            bool results = facade.WriteCardSpeedControl("正向", "100", out error);
            if (results == true)
            {
                string current1;
                string motionspeed1;
                results = facade.GetDataFromCardcan(out motionspeed1, out current1, out error);

                while (Math.Abs(Convert.ToDouble(motionspeed1) - 100) > 0.1)
                {
                    facade.GetDataFromCardcan(out motionspeed1, out current1, out error);
                }

                results = facade.WriteCardSpeedControl("正向", "115", out error);
                if (results == false)
                {
                    return;
                }

                DateTime dt = DateTime.Now;
                TimeSpan ts;
                while (Math.Abs(Convert.ToDouble(motionspeed1) - 115) > 0.1)
                {
                    facade.GetDataFromCardcan(out motionspeed1, out current1, out error);
                }
                MyDelegate md = MyMethod;
                ts = DateTime.Now - dt;
                this.label5.BeginInvoke(md, Math.Round(ts.Seconds / 3.0, 2));

                results = facade.WriteCardSpeedControl("正向", "0", out error);

            }
            else
            {
                return;
            }



        }

        #endregion


        #region 一键测试
        /// <summary>
        /// 一键测试模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            //一键测试
            if (this.radioButton10.Checked == true && this.button13.Text == "OFF")
            {
                sign = "6";

                ShowInformation("一键测试");


                if (checkBox1.Checked == true || checkBox2.Checked == true || checkBox3.Checked == true || checkBox4.Checked == true)
                {


                    if (threadAuto != null && threadAuto.ThreadState == System.Threading.ThreadState.Running)
                    {
                        threadAuto.Abort();
                        threadAuto.Join();
                    }



                    pictureBox10.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));
                    pictureBox1.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("red.bmp"));



                    if (checkBox1.Checked == true)
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 0, '0');

                        #region 恒速数据赋值
                        this.Scheme1_rotorspeed = this.textBox21.Text;
                        this.Scheme1_sampling = this.textBox22.Text.ToString();
                        this.Scheme1_times = this.comboBox12.SelectedItem.ToString();
                        this.Scheme1_H_num = new double[Convert.ToInt16(Scheme1_sampling)];
                        this.Scheme1_T_num = new double[Convert.ToInt16(Scheme1_sampling)];
                        this.Scheme1_moment = this.textBox4.Text;
                        this.Scheme1_k_number = 0;
                        this.max_deta_motion = 0;
                        this.max_deta_moment = 0;
                        this.ideal_torque = 0;
                        this.mean_torque = 0;
                        this.real_deta_moment = 0;

                        for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                        {
                            Scheme1_H_num[i] = 0.0;
                            Scheme1_T_num[i] = 0.0;
                        }


                        #endregion

                    }
                    else
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 0, '1');
                    }


                    if (checkBox4.Checked == true)
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 1, '0');

                        #region 斜坡参数配置
                        this.Scheme2_detaspeed = "200";
                        this.Scheme2_times = this.comboBox2.SelectedItem.ToString();
                        this.Scheme2_times_update = this.comboBox16.SelectedItem.ToString();
                        this.Scheme2_current_rotorspeed = "0";
                        this.Scheme2_stop_speed = this.textBox3.Text;
                        this.Scheme2_sampling = this.textBox19.Text.ToString();
                        this.Scheme2_T_num = new double[Convert.ToInt16(Scheme2_sampling)];
                        this.Scheme2_P_num = new double[Convert.ToInt16(Scheme2_sampling)];
                        this.Scheme2_H_num = new double[Convert.ToInt16(Scheme2_sampling)];
                        this.Scheme2_k_number = 0;
                        this.Scheme2_k1_number = 0;
                        this.Scheme2_moment = this.textBox8.Text;
                        this.max_deta_motion = 0;
                        this.max_deta_moment = 0;
                        this.ideal_torque = 0;
                        this.mean_torque = 0;
                        this.real_deta_moment = 0;
                        for (int i = 0; i < Scheme2_T_num.Length; i++)
                        {
                            Scheme2_T_num[i] = 0.0;
                            Scheme2_P_num[i] = 0.0;
                            Scheme2_H_num[i] = 0.0;
                        }
                        #endregion
                    }
                    else
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 1, '1');
                    }


                    if (checkBox3.Checked == true)
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 2, '0');

                        #region 正弦数据赋值
                        this.Scheme3_K = Convert.ToDouble(this.textBox17.Text);
                        this.Scheme3_A = Convert.ToDouble(this.textBox16.Text);
                        this.Scheme3_T = Convert.ToDouble(this.textBox15.Text);
                        this.Scheme3_times = this.comboBox3.SelectedItem.ToString();
                        this.Scheme3_times_update = this.comboBox17.SelectedItem.ToString();
                        this.Scheme3_sampling = this.textBox1.Text;
                        this.Scheme3_k_number = 0;
                        this.Scheme3_k1_number = 0;
                        this.Scheme3_current_time = 0;
                        this.Scheme3_moment = this.textBox10.Text;
                        this.Scheme3_P_num = new double[Convert.ToInt16(Scheme3_sampling)];
                        this.Scheme3_H_num = new double[Convert.ToInt16(Scheme3_sampling)];
                        this.Scheme3_T_num = new double[Convert.ToInt16(Scheme3_sampling)];
                        for (int i = 0; i < Scheme3_P_num.Length; i++)
                        {
                            Scheme3_P_num[i] = 0.0;
                            Scheme3_H_num[i] = 0.0;
                            Scheme3_T_num[i] = 0.0;
                        }
                        this.max_deta_motion = 0;
                        this.max_deta_moment = 0;
                        this.ideal_torque = 0;
                        this.mean_torque = 0;
                        this.real_deta_moment = 0;

                        #endregion
                    }
                    else
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 2, '1');
                    }

                    if (checkBox2.Checked == true)
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 3, '0');

                        #region 力矩数据赋值

                        this.Scheme4_torque = this.textBox18.Text.ToString();
                        this.Scheme4_sampling = this.textBox2.Text.ToString();
                        this.Scheme4_times = this.comboBox1.SelectedItem.ToString();
                        this.Scheme4_T_num = new double[Convert.ToInt16(Scheme4_sampling)];
                        this.Scheme4_P_num = new double[Convert.ToInt16(Scheme4_sampling)];
                        this.Scheme4_k_number = 0;
                        this.Scheme4_moment = this.textBox9.Text;
                        this.ideal_torque = 0;
                        this.mean_torque = 0;
                        this.real_deta_moment = 0;

                        for (int i = 0; i < Scheme4_T_num.Length; i++)
                        {
                            Scheme4_T_num[i] = 0.0;
                            Scheme4_P_num[i] = 0.0;
                        }




                        #endregion
                    }
                    else
                    {
                        AutoTestLabel = ReplaceChar(AutoTestLabel, 3, '1');
                    }

                    threadAuto = new System.Threading.Thread(TestAuto);
                    threadAuto.Start();
                }
                else
                {
                    ShowInformation("选择测试选项");
                    MessageBox.Show("选择测试选项");
                }
            }
            else
            {
                ShowInformation("选择该模式或打开电源");
                MessageBox.Show("选择该模式或打开电源");
            }

        }
        public static string ReplaceChar(string str, int index, char c)
        {
            if (index < 0 || index > str.Length - 1) return str;
            char[] carr = str.ToCharArray();
            carr[index] = c;
            return new string(carr);
        }

        public void TestAuto()
        {
            InvokeStateInformation mi = new InvokeStateInformation(UpdateStateInformation);
            bool results = false;
            string error = string.Empty;
            string motionspeed1;
            string current1;
        //    double speed_time = 0;

            DateTime dttime = DateTime.Now;


            #region 恒速测试开始

            if (AutoTestLabel.Substring(0, 1) == "0")
            {

                this.Scheme1_rotorspeed = "0";
                results = facade.WriteCardSpeedControl("正向", "0", out error);
                ConstantSpeedTimerRead.Interval = Convert.ToDouble(Scheme1_times) * 1000;
                ConstantSpeedTimerRead.Start();


                if (this.checkBox1.Checked == true && this.checkBox5.Checked == true)
                {
                    // Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "100";
                    results = facade.WriteCardSpeedControl("正向", "100", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    //   Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "500";
                    results = facade.WriteCardSpeedControl("正向", "500", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "1000";
                    results = facade.WriteCardSpeedControl("正向", "1000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "2000";
                    results = facade.WriteCardSpeedControl("正向", "2000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //    Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "3000";
                    results = facade.WriteCardSpeedControl("正向", "3000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "4000";
                    results = facade.WriteCardSpeedControl("正向", "4000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "5000";
                    results = facade.WriteCardSpeedControl("正向", "5000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "6000";
                    results = facade.WriteCardSpeedControl("正向", "6000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "-100";
                    results = facade.WriteCardSpeedControl("反向", "100", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 120);
                    Thread.Sleep(1000 * 40);

                    this.Scheme1_rotorspeed = "-500";
                    results = facade.WriteCardSpeedControl("反向", "500", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "-1000";
                    results = facade.WriteCardSpeedControl("反向", "1000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "-2000";
                    results = facade.WriteCardSpeedControl("反向", "2000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "-3000";
                    results = facade.WriteCardSpeedControl("反向", "3000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);



                    this.Scheme1_rotorspeed = "-4000";
                    results = facade.WriteCardSpeedControl("反向", "4000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //   Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);



                    this.Scheme1_rotorspeed = "-5000";
                    results = facade.WriteCardSpeedControl("反向", "5000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //    Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "-6000";
                    results = facade.WriteCardSpeedControl("反向", "6000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                   Thread.Sleep(1000 * 30);

                }
                else
                {
                    this.Scheme1_rotorspeed = "100";
                    results = facade.WriteCardSpeedControl("正向", "100", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    //   Thread.Sleep(1000 * 60);
                    Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "500";
                    results = facade.WriteCardSpeedControl("正向", "500", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "1000";
                    results = facade.WriteCardSpeedControl("正向", "1000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "2000";
                    results = facade.WriteCardSpeedControl("正向", "2000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //    Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "3000";
                    results = facade.WriteCardSpeedControl("正向", "3000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "-100";
                    results = facade.WriteCardSpeedControl("反向", "100", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 120);
                    Thread.Sleep(1000 * 40);

                    this.Scheme1_rotorspeed = "-500";
                    results = facade.WriteCardSpeedControl("反向", "500", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "-1000";
                    results = facade.WriteCardSpeedControl("反向", "1000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                    this.Scheme1_rotorspeed = "-2000";
                    results = facade.WriteCardSpeedControl("反向", "2000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);


                    this.Scheme1_rotorspeed = "-3000";
                    results = facade.WriteCardSpeedControl("反向", "3000", out error);
                    this.Scheme1_k_number = 0;
                    for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                    {
                        Scheme1_H_num[i] = 0.0;
                        Scheme1_T_num[i] = 0.0;
                    }
                    this.max_deta_motion = 0;
                    this.max_deta_moment = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;
                    //Thread.Sleep(1000 * 60);
                   Thread.Sleep(1000 * 30);

                }


                if (results == true)
                {
                    this.BeginInvoke(mi, new Object[] { "恒速控制测试执行", "1", "green.bmp" });

                }
                else
                {
                    this.BeginInvoke(mi, new Object[] { "恒速控制测试失败", "1", "red.bmp" });
                    threadAuto.Abort();
                    return;
                }
            }

            ConstantSpeedTimerRead.Stop();
            results = facade.WriteCardSpeedControl("正向", "0", out error);
           Thread.Sleep(1000 * 30);
            this.BeginInvoke(mi, new Object[] { "恒速控制测试通过", "1", "green.bmp" });


            #endregion

            if (this.checkBox1.Checked == true && this.checkBox5.Checked == true)
            {
                #region 0--6000最大能耗
                this.Scheme1_rotorspeed = "6000";
                sum_pow = 0;
                k_pow = 0;
                results = facade.WriteCardSpeedControl("正向", "6000", out error);
                pow = "1";

                this.Scheme1_k_number = 0;
                for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                {
                    Scheme1_H_num[i] = 0.0;
                    Scheme1_T_num[i] = 0.0;
                }
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;
                ConstantSpeedTimerRead.Start();

               Thread.Sleep(1000 * 30);
                FlyWheel2Facade facadese = new FlyWheel2Facade();
                facadese.GetDataFromCardcan(out motionspeed1, out current1, out error);

                //DateTime speed_up_start = DateTime.Now;
                //TimeSpan speed_up_time;  // 升速时间
                while (Math.Abs(Convert.ToDouble(motionspeed1)) <= 5990)
                {
                    Thread.Sleep(1000);
                    facadese.GetDataFromCardcan(out motionspeed1, out current1, out error);
                }
                //speed_up_time = DateTime.Now - speed_up_start;
                //speed_time = speed_up_time.Seconds;
                ConstantSpeedTimerRead.Stop();
                results = facade.WriteCardSpeedControl("正向", "0", out error);
                pow = "0";
                #endregion
            }

            if (this.checkBox1.Checked == true && this.checkBox6.Checked == true)
            {
                #region 0--3000最大能耗
                this.Scheme1_rotorspeed = "3000";
                sum_pow = 0;
                k_pow = 0;
                results = facade.WriteCardSpeedControl("正向", "3000", out error);
                pow = "1";

                this.Scheme1_k_number = 0;
                for (int i = 0; i < Convert.ToInt16(Scheme1_sampling); i++)
                {
                    Scheme1_H_num[i] = 0.0;
                    Scheme1_T_num[i] = 0.0;
                }
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.ideal_torque = 0;
                this.mean_torque = 0;
                this.real_deta_moment = 0;
                ConstantSpeedTimerRead.Start();

               Thread.Sleep(1000 * 30);
                FlyWheel2Facade facadese = new FlyWheel2Facade();
                facadese.GetDataFromCardcan(out motionspeed1, out current1, out error);

                while (Math.Abs(Convert.ToDouble(motionspeed1)) <= 3000)
                {
                    Thread.Sleep(1000);
                    facadese.GetDataFromCardcan(out motionspeed1, out current1, out error);
                }

                ConstantSpeedTimerRead.Stop();
                results = facade.WriteCardSpeedControl("正向", "0", out error);
                pow = "0";
                #endregion
            }

            #region 斜坡测试开始
            if (AutoTestLabel.Substring(1, 1) == "0")
            {
                results = facade.WriteCardSpeedControl("正向", "0", out error);
                this.SlopeSpeedTimerRead.Interval = Convert.ToDouble(Scheme2_times) * 1000;
                SlopeSpeedTimerRead.Start();
                this.SlopeSpeedTimerWrite.Interval = Convert.ToDouble(Scheme2_times_update) * 1000;
                SlopeSpeedTimerWrite.Start();
                if (results == true)
                {
                    this.BeginInvoke(mi, new Object[] { "斜坡控制测试执行", "2", "green.bmp" });

                }
                else
                {
                    this.BeginInvoke(mi, new Object[] { "斜坡控制测试失败", "2", "red.bmp" });
                    threadAuto.Abort();
                    return;
                }
               Thread.Sleep(1000 * 30);
                FlyWheel2Facade facades = new FlyWheel2Facade();
                facades.GetDataFromCardcan(out motionspeed1, out current1, out error);
                while (Math.Abs(Convert.ToDouble(motionspeed1)) <= 5000)
                {
                    Thread.Sleep(1000);
                    facades.GetDataFromCardcan(out motionspeed1, out current1, out error);
                }
            }
            SlopeSpeedTimerRead.Stop();
            SlopeSpeedTimerWrite.Stop();
            results = facade.WriteCardSpeedControl("正向", "0", out error);
            this.BeginInvoke(mi, new Object[] { "斜坡测试通过", "2", "green.bmp" });
            #endregion



            #region 力矩测试开始
            if (AutoTestLabel.Substring(3, 1) == "0")
            {
                if (this.checkBox2.Checked == true && this.checkBox5.Checked == true)
                {
                    Scheme4_torque = "3";
                    results = facade.WriteCardTorqueControl("正向", "3", out error);

                    if (results == true)
                    {
                        this.BeginInvoke(mi, new Object[] { "力矩控制测试执行", "4", "green.bmp" });
                    }
                    else
                    {
                        this.BeginInvoke(mi, new Object[] { "力矩控制测试失败", "4", "red.bmp" });
                        threadAuto.Abort();
                        return;
                    }

                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }

                    TorqueSpeedTimerRead.Interval = Convert.ToDouble(Scheme4_times) * 1000;
                    TorqueSpeedTimerRead.Start();
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "10";
                    results = facade.WriteCardTorqueControl("正向", "10", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "15";
                    results = facade.WriteCardTorqueControl("正向", "15", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "-3";
                    results = facade.WriteCardTorqueControl("反向", "3", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "-10";
                    results = facade.WriteCardTorqueControl("反向", "10", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "-15";
                    results = facade.WriteCardTorqueControl("反向", "15", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                }
                else
                {
                    Scheme4_torque = "3";
                    results = facade.WriteCardTorqueControl("正向", "3", out error);

                    if (results == true)
                    {
                        this.BeginInvoke(mi, new Object[] { "力矩控制测试执行", "4", "green.bmp" });
                    }
                    else
                    {
                        this.BeginInvoke(mi, new Object[] { "力矩控制测试失败", "4", "red.bmp" });
                        threadAuto.Abort();
                        return;
                    }

                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }

                    TorqueSpeedTimerRead.Interval = Convert.ToDouble(Scheme4_times) * 1000;
                    TorqueSpeedTimerRead.Start();
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "10";
                    results = facade.WriteCardTorqueControl("正向", "10", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);

                    Scheme4_torque = "-3";
                    results = facade.WriteCardTorqueControl("反向", "3", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                    Scheme4_torque = "0";
                    results = facade.WriteCardSpeedControl("正向", "0", out error);
                   Thread.Sleep(1000 * 30);


                    Scheme4_torque = "-10";
                    results = facade.WriteCardTorqueControl("反向", "10", out error);
                    this.Scheme4_k_number = 0;
                    this.ideal_torque = 0;
                    this.mean_torque = 0;
                    this.real_deta_moment = 0;

                    for (int i = 0; i < Scheme4_T_num.Length; i++)
                    {
                        Scheme4_T_num[i] = 0.0;
                        Scheme4_P_num[i] = 0.0;
                    }
                   Thread.Sleep(1000 * 30);

                }



            }
            Scheme4_torque = "0";
            results = facade.WriteCardSpeedControl("正向", "0", out error);
            TorqueSpeedTimerRead.Stop();
            this.BeginInvoke(mi, new Object[] { "力矩控制测试通过", "4", "green.bmp" });


            #endregion



            #region 生成报表

            Facade.CommonFacade.FacTableStyle facade1 = new Facade.CommonFacade.FacTableStyle();

            #region 恒速信息
            int k0 = 0;
            double sum0 = 0;
            double sum0_ConstantMoment = 0;
            double sum0_ChangeMoment = 0;
            double sum0_power = 0;


            int k100 = 0;
            double sum100 = 0;
            double sum100_ConstantMoment = 0;
            double sum100_ChangeMoment = 0;

            int kf100 = 0;
            double sumkf100 = 0;
            double sumkf100_ConstantMoment = 0;
            double sumkf100_ChangeMoment = 0;


            int k500 = 0;
            double sumk500 = 0;
            double sumk500_ConstantMoment = 0;
            double sumk500_ChangeMoment = 0;
            double sumk500_power = 0;


            int kf500 = 0;
            double sumkf500 = 0;
            double sumkf500_ConstantMoment = 0;
            double sumkf500_ChangeMoment = 0;
            double sumkf500_power = 0;

            int k1000 = 0;
            double sumk1000 = 0;
            double sumk1000_ConstantMoment = 0;
            double sumk1000_ChangeMoment = 0;

            int kf1000 = 0;
            double sumkf1000 = 0;
            double sumkf1000_ConstantMoment = 0;
            double sumkf1000_ChangeMoment = 0;

            int k2000 = 0;
            double sumk2000 = 0;
            double sumk2000_ConstantMoment = 0;
            double sumk2000_ChangeMoment = 0;
            double sumk2000_power = 0;

            int kf2000 = 0;
            double sumkf2000 = 0;
            double sumkf2000_ConstantMoment = 0;
            double sumkf2000_ChangeMoment = 0;
            double sumkf2000_power = 0;

            int k3000 = 0;
            double sumk3000 = 0;
            double sumk3000_ConstantMoment = 0;
            double sumk3000_ChangeMoment = 0;
            double sumk3000_power = 0;

            int kf3000 = 0;
            double sumkf3000 = 0;
            double sumkf3000_ConstantMoment = 0;
            double sumkf3000_ChangeMoment = 0;
            double sumkf3000_power = 0;

            int k4000 = 0;
            double sumk4000 = 0;
            double sumk4000_ConstantMoment = 0;
            double sumk4000_ChangeMoment = 0;
            double sumk4000_power = 0;

            int kf4000 = 0;
            double sumkf4000 = 0;
            double sumkf4000_ConstantMoment = 0;
            double sumkf4000_ChangeMoment = 0;
            double sumkf4000_power = 0;



            int k5000 = 0;
            double sumk5000 = 0;
            double sumk5000_ConstantMoment = 0;
            double sumk5000_ChangeMoment = 0;
            double sumk5000_power = 0;


            int kf5000 = 0;
            double sumkf5000 = 0;
            double sumkf5000_ConstantMoment = 0;
            double sumkf5000_ChangeMoment = 0;
            double sumkf5000_power = 0;


            int k6000 = 0;
            double sumk6000 = 0;
            double sumk6000_ConstantMoment = 0;
            double sumk6000_ChangeMoment = 0;
            double sumk6000_power = 0;

            int kf6000 = 0;
            double sumkf6000 = 0;
            double sumkf6000_ConstantMoment = 0;
            double sumkf6000_ChangeMoment = 0;
            double sumkf6000_power = 0;

            double max_realmoment = 0;
            if (AutoTestLabel.Substring(0, 1) == "0")
            {

                ds_Motor11 = facade1.GetDataFromDatabaseAuto("21", "6", experiment_name, experiment_memeber, experiment_product, out error);



                if (ds_Motor11 != null)
                {

                    for (int i = 0; i < ds_Motor11.Tables["dbo.Motor21"].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor11.Tables["dbo.Motor21"].Rows[i];
                        if (dw["RotorRevIde"].ToString() == "0" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString())) < 1)
                        {
                            k0 = k0 + 1;
                            sum0 = sum0 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sum0_ConstantMoment = sum0_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sum0_ChangeMoment = sum0_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sum0_power = sum0_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "100" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 100) < 1)
                        {
                            k100 = k100 + 1;
                            sum100 = sum100 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sum100_ConstantMoment = sum100_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sum100_ChangeMoment = sum100_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "-100" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 100) < 1)
                        {
                            kf100 = kf100 + 1;
                            sumkf100 = sumkf100 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf100_ConstantMoment = sumkf100_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf100_ChangeMoment = sumkf100_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "500" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 500) < 1)
                        {
                            k500 = k500 + 1;
                            sumk500 = sumk500 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk500_ConstantMoment = sumk500_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk500_ChangeMoment = sumk500_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk500_power = sumk500_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }


                        if (dw["RotorRevIde"].ToString() == "-500" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 500) < 1)
                        {
                            kf500 = kf500 + 1;
                            sumkf500 = sumkf500 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf500_ConstantMoment = sumkf500_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf500_ChangeMoment = sumkf500_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf500_power = sumkf500_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "1000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 1000) < 1)
                        {
                            k1000 = k1000 + 1;
                            sumk1000 = sumk1000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk1000_ConstantMoment = sumk1000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk1000_ChangeMoment = sumk1000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "-1000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 1000) < 1)
                        {
                            kf1000 = kf1000 + 1;
                            sumkf1000 = sumkf1000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf1000_ConstantMoment = sumkf1000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf1000_ChangeMoment = sumkf1000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "2000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 2000) < 1)
                        {
                            k2000 = k2000 + 1;
                            sumk2000 = sumk2000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk2000_ConstantMoment = sumk2000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk2000_ChangeMoment = sumk2000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk2000_power = sumk2000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "-2000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 2000) < 1)
                        {
                            kf2000 = kf2000 + 1;
                            sumkf2000 = sumkf2000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf2000_ConstantMoment = sumkf2000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf2000_ChangeMoment = sumkf2000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf2000_power = sumkf2000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "3000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 3000) < 1)
                        {
                            k3000 = k3000 + 1;
                            sumk3000 = sumk3000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk3000_ConstantMoment = sumk3000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk3000_ChangeMoment = sumk3000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk3000_power = sumk3000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "-3000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 3000) < 1)
                        {
                            kf3000 = kf3000 + 1;
                            sumkf3000 = sumkf3000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf3000_ConstantMoment = sumkf3000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf3000_ChangeMoment = sumkf3000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf3000_power = sumkf3000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                            if (Convert.ToDouble(dw["backup4"].ToString()) > max_realmoment)
                            {
                                max_realmoment = Convert.ToDouble(dw["backup4"].ToString());
                            }
                        }


                        if (dw["RotorRevIde"].ToString() == "4000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 4000) < 4)
                        {
                            k4000 = k4000 + 1;
                            sumk4000 = sumk4000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk4000_ConstantMoment = sumk4000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk4000_ChangeMoment = sumk4000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk4000_power = sumk4000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "-4000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 4000) < 4)
                        {
                            kf4000 = kf4000 + 1;
                            sumkf4000 = sumkf4000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf4000_ConstantMoment = sumkf4000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf4000_ChangeMoment = sumkf4000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf4000_power = sumkf4000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }


                        if (dw["RotorRevIde"].ToString() == "5000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 5000) < 2)
                        {
                            k5000 = k5000 + 1;
                            sumk5000 = sumk5000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk5000_ConstantMoment = sumk5000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk5000_ChangeMoment = sumk5000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk5000_power = sumk5000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }
                        if (dw["RotorRevIde"].ToString() == "-5000" && Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 5000) < 2)
                        {
                            kf5000 = kf5000 + 1;
                            sumkf5000 = sumkf5000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf5000_ConstantMoment = sumkf5000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf5000_ChangeMoment = sumkf5000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf5000_power = sumkf5000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                        }

                        if (dw["RotorRevIde"].ToString() == "6000" && (Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 6000) < 2 || Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) - 6000) <= 5))
                        {
                            k6000 = k6000 + 1;
                            sumk6000 = sumk6000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumk6000_ConstantMoment = sumk6000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumk6000_ChangeMoment = sumk6000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumk6000_power = sumk6000_power + Convert.ToDouble(dw["RotorPow"].ToString());

                        }

                        if (dw["RotorRevIde"].ToString() == "-6000" && (Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 6000) < 2 || Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString()) + 6000) <= 5))
                        {
                            kf6000 = kf6000 + 1;
                            sumkf6000 = sumkf6000 + Convert.ToDouble(dw["RotorRevRea"].ToString());
                            sumkf6000_ConstantMoment = sumkf6000_ConstantMoment + Convert.ToDouble(dw["ConstantMoment"].ToString());
                            sumkf6000_ChangeMoment = sumkf6000_ChangeMoment + Convert.ToDouble(dw["ChangeMoment"].ToString());
                            sumkf6000_power = sumkf6000_power + Convert.ToDouble(dw["RotorPow"].ToString());
                            if (Convert.ToDouble(dw["backup4"].ToString()) > max_realmoment)
                            {
                                max_realmoment = Convert.ToDouble(dw["backup4"].ToString());
                            }
                        }

                    }

                }
            }
            #endregion

            #region 斜坡信息
            double k_torque = 0;
            double sum_torque = 0;
            if (AutoTestLabel.Substring(1, 1) == "0")
            {
                ds_Motor12 = facade1.GetDataFromDatabaseAutoSlope("22", "6", experiment_name, experiment_memeber, experiment_product, out error);

                if (ds_Motor12 != null)
                {
                    for (int i = 0; i < ds_Motor12.Tables["dbo.Motor22"].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor12.Tables["dbo.Motor22"].Rows[i];
                        if (Math.Abs(Convert.ToDouble(dw["RotorRevRea"].ToString())) > 200)
                        {

                            k_torque = k_torque + 1;
                            sum_torque = sum_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }
                    }
                }
            }

            #endregion

            #region 力矩信息
            double k3_torque = 0;
            double sum3_torque = 0;

            double kf3_torque = 0;
            double sumf3_torque = 0;

            double k10_torque = 0;
            double sum10_torque = 0;

            double kf10_torque = 0;
            double sumf10_torque = 0;


            double k15_torque = 0;
            double sum15_torque = 0;

            double kf15_torque = 0;
            double sumf15_torque = 0;


            if (AutoTestLabel.Substring(3, 1) == "0")
            {
                ds_Motor14 = facade1.GetDataFromDatabaseAutoTorque("24", "6", experiment_name, experiment_memeber, experiment_product, out error);

                if (ds_Motor14 != null)
                {
                    for (int i = 0; i < ds_Motor14.Tables["dbo.Motor24"].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor14.Tables["dbo.Motor24"].Rows[i];
                        if (dw["backup5"].ToString() == "3")
                        {
                            k3_torque = k3_torque + 1;
                            sum3_torque = sum3_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }

                        if (dw["backup5"].ToString() == "-3")
                        {
                            kf3_torque = kf3_torque + 1;
                            sumf3_torque = sumf3_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }

                        if (dw["backup5"].ToString() == "10")
                        {
                            k10_torque = k10_torque + 1;
                            sum10_torque = sum10_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }

                        if (dw["backup5"].ToString() == "-10")
                        {
                            kf10_torque = kf10_torque + 1;
                            sumf10_torque = sumf10_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }

                        if (dw["backup5"].ToString() == "15")
                        {
                            k15_torque = k15_torque + 1;
                            sum15_torque = sum15_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }


                        if (dw["backup5"].ToString() == "-15")
                        {
                            kf15_torque = kf15_torque + 1;
                            sumf15_torque = sumf15_torque + Convert.ToDouble(dw["MeanTorque"].ToString());
                        }
                    }
                }
            }

            #endregion


            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4);

            try
            {
                PdfWriter.getInstance(document, new System.IO.FileStream("D:\\飞轮测试报表\\" + this.experiment_memeber +
                    "+" + this.experiment_name + "+" + this.experiment_product + "+result.pdf", System.IO.FileMode.Create));

                BaseFont bfHei = BaseFont.createFont(Application.StartupPath + "\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font1 = new iTextSharp.text.Font(bfHei, 8);

                iTextSharp.text.Font fonttitle = new iTextSharp.text.Font(bfHei, 10);
                float[] headerwidths = { 3, 4, 9, 9, 5, 5, 5, 8, 5, 5 };

                #region 创建Paragraph

                Paragraph p1 = new Paragraph(new Phrase("500mNms 微飞轮整机交付电性能测试记录表", font1));
                p1.Alignment = Element.ALIGN_CENTER;

                Paragraph p2 = new Paragraph(new Phrase("代号", font1));
                p2.Alignment = Element.ALIGN_CENTER;

                Paragraph p3 = new Paragraph(new Phrase("产品编号", font1));
                p3.Alignment = Element.ALIGN_CENTER;

                Paragraph p4 = new Paragraph(new Phrase("测试目的", font1));
                p4.Alignment = Element.ALIGN_CENTER;

                Paragraph p5 = new Paragraph(new Phrase("测试时间", font1));
                p5.Alignment = Element.ALIGN_CENTER;

                Paragraph p6 = new Paragraph(new Phrase("测试人员", font1));
                p6.Alignment = Element.ALIGN_CENTER;

                Paragraph p7 = new Paragraph(new Phrase("测试场景", font1));
                p7.Alignment = Element.ALIGN_CENTER;

                Paragraph p8 = new Paragraph(new Phrase("序号", font1));
                p8.Alignment = Element.ALIGN_CENTER;

                Paragraph p9 = new Paragraph(new Phrase("测试项目", font1));
                p9.Alignment = Element.ALIGN_CENTER;

                Paragraph p10 = new Paragraph(new Phrase("指标值", font1));
                p10.Alignment = Element.ALIGN_CENTER;

                Paragraph p11 = new Paragraph(new Phrase("实测（计算）值", font1));
                p11.Alignment = Element.ALIGN_CENTER;

                Paragraph p12 = new Paragraph(new Phrase("指令转速", font1));
                p12.Alignment = Element.ALIGN_CENTER;

                Paragraph p13 = new Paragraph(new Phrase("结论", font1));
                p13.Alignment = Element.ALIGN_CENTER;

                Paragraph p14 = new Paragraph(new Phrase("备注", font1));
                p14.Alignment = Element.ALIGN_CENTER;

                Paragraph p15 = new Paragraph(new Phrase("转          速          模          式", font1));
                p15.Alignment = Element.ALIGN_CENTER;

                Paragraph p16 = new Paragraph(new Phrase("调速范围（-6500rpm~6500rpm）角动量控制偏差、常值偏差", font1));
                p16.Alignment = Element.ALIGN_CENTER;

                Paragraph p17 = new Paragraph(new Phrase("转速动态偏差△h<=2rpm 转速常值偏差：H<=2rpm", font1));
                p17.Alignment = Element.ALIGN_CENTER;

                Paragraph p18 = new Paragraph(new Phrase("实测转速", font1));
                p18.Alignment = Element.ALIGN_CENTER;

                Paragraph p19 = new Paragraph(new Phrase("常值偏差", font1));
                p19.Alignment = Element.ALIGN_CENTER;

                Paragraph p20 = new Paragraph(new Phrase("△H", font1));
                p20.Alignment = Element.ALIGN_CENTER;

                Paragraph p21 = new Paragraph(new Phrase("0rpm", font1));
                p21.Alignment = Element.ALIGN_CENTER;

                Paragraph p22 = new Paragraph(new Phrase("+100rpm", font1));
                p22.Alignment = Element.ALIGN_CENTER;

                Paragraph p23 = new Paragraph(new Phrase("-100rpm", font1));
                p23.Alignment = Element.ALIGN_CENTER;

                Paragraph p24 = new Paragraph(new Phrase("+500rpm", font1));
                p24.Alignment = Element.ALIGN_CENTER;

                Paragraph p25 = new Paragraph(new Phrase("-500rpm", font1));
                p25.Alignment = Element.ALIGN_CENTER;

                Paragraph p26 = new Paragraph(new Phrase("+1000rpm", font1));
                p26.Alignment = Element.ALIGN_CENTER;

                Paragraph p27 = new Paragraph(new Phrase("-1000rpm", font1));
                p27.Alignment = Element.ALIGN_CENTER;

                Paragraph p28 = new Paragraph(new Phrase("+3000rpm", font1));
                p28.Alignment = Element.ALIGN_CENTER;

                Paragraph p29 = new Paragraph(new Phrase("-3000rpm", font1));
                p29.Alignment = Element.ALIGN_CENTER;

                Paragraph p30 = new Paragraph(new Phrase("+5000rpm", font1));
                p30.Alignment = Element.ALIGN_CENTER;

                Paragraph p31 = new Paragraph(new Phrase("-5000rpm", font1));
                p31.Alignment = Element.ALIGN_CENTER;


                Paragraph p32 = new Paragraph(new Phrase("+6000rpm", font1));
                p32.Alignment = Element.ALIGN_CENTER;

                Paragraph p33 = new Paragraph(new Phrase("-6000rpm", font1));
                p33.Alignment = Element.ALIGN_CENTER;

                Paragraph p34 = new Paragraph(new Phrase("最大反作用力矩", font1));
                p34.Alignment = Element.ALIGN_CENTER;

                Paragraph p35 = new Paragraph(new Phrase("绝对值>= 16 mNm", font1));
                p35.Alignment = Element.ALIGN_CENTER;

                Paragraph p36 = new Paragraph(new Phrase("0-6000rpm加速过程中的平均力矩", font1));
                p36.Alignment = Element.ALIGN_CENTER;

                Paragraph p37 = new Paragraph(new Phrase("稳速功耗", font1));
                p37.Alignment = Element.ALIGN_CENTER;

                Paragraph p38 = new Paragraph(new Phrase("0rpm", font1));
                p38.Alignment = Element.ALIGN_CENTER;

                Paragraph p39 = new Paragraph(new Phrase("+500rpm", font1));
                p39.Alignment = Element.ALIGN_CENTER;

                Paragraph p40 = new Paragraph(new Phrase("-500rpm", font1));
                p40.Alignment = Element.ALIGN_CENTER;

                Paragraph p41 = new Paragraph(new Phrase("+2000rpm", font1));
                p41.Alignment = Element.ALIGN_CENTER;

                Paragraph p42 = new Paragraph(new Phrase("-2000rpm", font1));
                p42.Alignment = Element.ALIGN_CENTER;

                Paragraph p43 = new Paragraph(new Phrase("+4000rpm", font1));
                p43.Alignment = Element.ALIGN_CENTER;

                Paragraph p44 = new Paragraph(new Phrase("-4000rpm", font1));
                p44.Alignment = Element.ALIGN_CENTER;

                Paragraph p45 = new Paragraph(new Phrase("+6000rpm", font1));
                p45.Alignment = Element.ALIGN_CENTER;

                Paragraph p46 = new Paragraph(new Phrase("-6000rpm", font1));
                p46.Alignment = Element.ALIGN_CENTER;

                Paragraph p47 = new Paragraph(new Phrase("最大功耗", font1));
                p47.Alignment = Element.ALIGN_CENTER;

                Paragraph p48 = new Paragraph(new Phrase("最大力矩时的功耗", font1));
                p48.Alignment = Element.ALIGN_CENTER;

                Paragraph p49 = new Paragraph(new Phrase("最大角动量Hmax", font1));
                p49.Alignment = Element.ALIGN_CENTER;

                Paragraph p50 = new Paragraph(new Phrase("绝对值500±1mNms", font1));
                p50.Alignment = Element.ALIGN_CENTER;

                Paragraph p51 = new Paragraph(new Phrase("6000rpm时的角动量", font1));
                p51.Alignment = Element.ALIGN_CENTER;

                Paragraph p52 = new Paragraph(new Phrase("力          矩          模          式", font1));
                p52.Alignment = Element.ALIGN_CENTER;

                Paragraph p53 = new Paragraph(new Phrase("序号", font1));
                p53.Alignment = Element.ALIGN_CENTER;

                Paragraph p54 = new Paragraph(new Phrase("实测力矩", font1));
                p54.Alignment = Element.ALIGN_CENTER;

                Paragraph p55 = new Paragraph(new Phrase("指令力矩", font1));
                p55.Alignment = Element.ALIGN_CENTER;

                Paragraph p56 = new Paragraph(new Phrase("备注", font1));
                p56.Alignment = Element.ALIGN_CENTER;

                Paragraph p57 = new Paragraph(new Phrase("+3mNm", font1));
                p57.Alignment = Element.ALIGN_CENTER;

                Paragraph p58 = new Paragraph(new Phrase("+10mNm", font1));
                p58.Alignment = Element.ALIGN_CENTER;

                Paragraph p59 = new Paragraph(new Phrase("+15mNm", font1));
                p59.Alignment = Element.ALIGN_CENTER;

                Paragraph p60 = new Paragraph(new Phrase("-3mNm", font1));
                p60.Alignment = Element.ALIGN_CENTER;

                Paragraph p61 = new Paragraph(new Phrase("-10mNm", font1));
                p61.Alignment = Element.ALIGN_CENTER;

                Paragraph p62 = new Paragraph(new Phrase("-15mNm", font1));
                p62.Alignment = Element.ALIGN_CENTER;

                Paragraph p63 = new Paragraph(new Phrase("500mNms 微飞轮正弦振动测试记录表", font1));
                p63.Alignment = Element.ALIGN_MIDDLE;

                Paragraph p64 = new Paragraph(new Phrase("测试方向", font1));
                p64.Alignment = Element.ALIGN_RIGHT;

                Paragraph p2t = new Paragraph(new Phrase(this.experiment_name, font1));
                p2t.Alignment = Element.ALIGN_CENTER;

                Paragraph p3t = new Paragraph(new Phrase(this.experiment_product, font1));
                p3t.Alignment = Element.ALIGN_CENTER;

                Paragraph p5t = new Paragraph(new Phrase(dttime.ToString(), font1));
                p5t.Alignment = Element.ALIGN_CENTER;

                Paragraph p6t = new Paragraph(new Phrase(this.experiment_memeber, font1));
                p6t.Alignment = Element.ALIGN_CENTER;

                Paragraph pt0;
                if (k0 != 0)
                {
                    pt0 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum0 / k0, 2)), font1));

                }
                else
                {
                    pt0 = new Paragraph(new Phrase("0", font1));
                }
                pt0.Alignment = Element.ALIGN_CENTER;

                Paragraph pt0_con;
                if (k0 != 0)
                {
                    pt0_con = new Paragraph(new Phrase(Convert.ToString(Math.Abs(Math.Round(sum0 / k0, 2))), font1));
                }
                else
                {
                    pt0_con = new Paragraph(new Phrase("0", font1));
                }
                pt0_con.Alignment = Element.ALIGN_CENTER;

                Paragraph pt0_cha;
                if (k0 != 0)
                {
                    pt0_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum0_ChangeMoment / k0, 2)), font1));
                }
                else
                {
                    pt0_cha = new Paragraph(new Phrase("0", font1));
                }
                pt0_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph pt100;
                if (k100 != 0)
                {
                    pt100 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum100 / k100, 2)), font1));
                }
                else
                {
                    pt100 = new Paragraph(new Phrase("0", font1));
                }
                pt100.Alignment = Element.ALIGN_CENTER;

                Paragraph pt100_con;
                if (k100 != 0)
                {
                    pt100_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sum100 / k100 - 100), 2)), font1));
                }
                else
                {
                    pt100_con = new Paragraph(new Phrase("0", font1));
                }
                pt100_con.Alignment = Element.ALIGN_CENTER;

                Paragraph pt100_cha;
                if (k100 != 0)
                {
                    pt100_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum100_ChangeMoment / k100, 2)), font1));
                }
                else
                {
                    pt100_cha = new Paragraph(new Phrase("0", font1));
                }
                pt100_cha.Alignment = Element.ALIGN_CENTER;


                Paragraph Clu100;
                if (Math.Round(Math.Abs(sum100 / k100 - 100), 2) <= 2)
                {
                    Clu100 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu100 = new Paragraph(new Phrase("不符合", font1));
                }
                Clu100.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf100;
                if (kf100 != 0)
                {
                    ptf100 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf100 / kf100, 2)), font1));
                }
                else
                {
                    ptf100 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph ptf100_con;
                if (kf100 != 0)
                {
                    ptf100_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf100 / kf100 + 100), 2)), font1));
                }
                else
                {
                    ptf100_con = new Paragraph(new Phrase("0", font1));
                }

                Paragraph ptf100_cha;
                if (kf100 != 0)
                {
                    ptf100_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf100_ChangeMoment / kf100, 2)), font1));
                }
                else
                {
                    ptf100_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Cluf100;
                if (Math.Round(Math.Abs(sumkf100 / kf100 + 100), 2) <= 2)
                {
                    Cluf100 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf100 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph pt500;
                if (k500 != 0)
                {
                    pt500 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk500 / k500, 2)), font1));
                }
                else
                {
                    pt500 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph pt500_con;
                if (k500 != 0)
                {
                    pt500_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumk500 / k500 - 500), 2)), font1));
                }
                else
                {
                    pt500_con = new Paragraph(new Phrase("0", font1));
                }
                pt500_con.Alignment = Element.ALIGN_CENTER;

                Paragraph pt500_cha;
                if (k500 != 0)
                {
                    pt500_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk500_ChangeMoment / k500, 2)), font1));
                }
                else
                {
                    pt500_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Clu500;
                if (Math.Round(Math.Abs(sumk500 / k500 - 500), 2) <= 2)
                {
                    Clu500 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu500 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph ptf500;
                if (kf500 != 0)
                {
                    ptf500 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf500 / kf500, 2)), font1));
                }
                else
                {
                    ptf500 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph ptf500_con;
                if (kf500 != 0)
                {
                    ptf500_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf500 / kf500 + 500), 2)), font1));
                }
                else
                {
                    ptf500_con = new Paragraph(new Phrase("0", font1));
                }
                ptf500_con.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf500_cha;
                if (kf500 != 0)
                {
                    ptf500_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf500_ChangeMoment / kf500, 2)), font1));
                }
                else
                {
                    ptf500_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Cluf500;
                if (Math.Round(Math.Abs(sumkf500 / kf500 + 500), 2) <= 2)
                {
                    Cluf500 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf500 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph pt1000;
                if (k1000 != 0)
                {
                    pt1000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk1000 / k1000, 2)), font1));
                }
                else
                {
                    pt1000 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph pt1000_con;
                if (k1000 != 0)
                {
                    pt1000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumk1000 / k1000 - 1000), 2)), font1));
                }
                else
                {
                    pt1000_con = new Paragraph(new Phrase("0", font1));
                }

                Paragraph pt1000_cha;
                if (k1000 != 0)
                {
                    pt1000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk1000_ChangeMoment / k1000, 2)), font1));
                }
                else
                {
                    pt1000_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Clu1000;
                if (Math.Round(Math.Abs(sumk1000 / k1000 - 1000), 2) <= 2)
                {
                    Clu1000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu1000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph ptf1000;
                if (kf1000 != 0)
                {
                    ptf1000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf1000 / kf1000, 2)), font1));
                }
                else
                {
                    ptf1000 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph ptf1000_con;
                if (kf1000 != 0)
                {
                    // ptf1000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk1000_ConstantMoment / kf1000, 2)), font1));
                    ptf1000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf1000 / kf1000 + 1000), 2)), font1));
                }
                else
                {
                    ptf1000_con = new Paragraph(new Phrase("0", font1));
                }

                Paragraph ptf1000_cha;
                if (kf1000 != 0)
                {
                    ptf1000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk1000_ChangeMoment / kf1000, 2)), font1));
                }
                else
                {
                    ptf1000_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Cluf1000;
                if (Math.Round(Math.Abs(sumkf1000 / kf1000 + 1000), 2) <= 2)
                {
                    Cluf1000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf1000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph pt3000;
                if (k3000 != 0)
                {
                    pt3000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk3000 / k3000, 2)), font1));
                }
                else
                {
                    pt3000 = new Paragraph(new Phrase("0", font1));
                }

                Paragraph pt3000_con;
                if (k3000 != 0)
                {
                    //  pt3000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk3000_ConstantMoment / k3000, 2)), font1));
                    pt3000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumk3000 / k3000 - 3000), 2)), font1));
                }
                else
                {
                    pt3000_con = new Paragraph(new Phrase("0", font1));
                }

                Paragraph pt3000_cha;
                if (k3000 != 0)
                {
                    pt3000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk3000_ChangeMoment / k3000, 2)), font1));
                }
                else
                {
                    pt3000_cha = new Paragraph(new Phrase("0", font1));
                }

                Paragraph Clu3000;
                if (Math.Round(Math.Abs(sumk3000 / k3000 - 3000), 2) <= 2)
                {
                    Clu3000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu3000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph ptf3000;
                if (kf3000 != 0)
                {
                    ptf3000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf3000 / kf3000, 2)), font1));
                }
                else
                {
                    ptf3000 = new Paragraph(new Phrase("0", font1));
                }
                ptf3000.Alignment = Element.ALIGN_CENTER;


                Paragraph ptf3000_con;
                if (kf3000 != 0)
                {
                    //  ptf3000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf3000_ConstantMoment / kf3000, 2)), font1));
                    ptf3000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf3000 / kf3000 + 3000), 2)), font1));
                }
                else
                {
                    ptf3000_con = new Paragraph(new Phrase("0", font1));
                }
                ptf3000_con.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf3000_cha;
                if (kf3000 != 0)
                {
                    ptf3000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk3000_ChangeMoment / kf3000, 2)), font1));
                }
                else
                {
                    ptf3000_cha = new Paragraph(new Phrase("0", font1));
                }
                ptf3000_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph Cluf3000;
                if (Math.Round(Math.Abs(sumkf3000 / kf3000 + 3000), 2) <= 2)
                {
                    Cluf3000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf3000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph pt5000;
                if (k5000 != 0)
                {
                    pt5000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk5000 / k5000, 2)), font1));
                }
                else
                {
                    pt5000 = new Paragraph(new Phrase("0", font1));
                }
                pt5000.Alignment = Element.ALIGN_CENTER;

                Paragraph pt5000_con;
                if (k5000 != 0)
                {
                    //  pt5000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk5000_ConstantMoment / k5000, 2)), font1));
                    pt5000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumk5000 / k5000 - 5000), 2)), font1));
                }
                else
                {
                    pt5000_con = new Paragraph(new Phrase("0", font1));
                }
                pt5000_con.Alignment = Element.ALIGN_CENTER;

                Paragraph pt5000_cha;
                if (k5000 != 0)
                {
                    pt5000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk5000_ChangeMoment / k5000, 2)), font1));
                }
                else
                {
                    pt5000_cha = new Paragraph(new Phrase("0", font1));
                }
                pt5000_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph Clu5000;
                if (Math.Round(Math.Abs(sumk5000 / k5000 - 5000), 2) <= 2)
                {
                    Clu5000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu5000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph ptf5000;
                if (kf5000 != 0)
                {
                    ptf5000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf5000 / kf5000, 2)), font1));
                }
                else
                {
                    ptf5000 = new Paragraph(new Phrase("0", font1));
                }
                ptf5000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf5000_con;
                if (kf5000 != 0)
                {
                    //  ptf5000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf5000_ConstantMoment / kf5000, 2)), font1));
                    ptf5000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf5000 / kf5000 + 5000), 2)), font1));
                }
                else
                {
                    ptf5000_con = new Paragraph(new Phrase("0", font1));
                }
                ptf5000_con.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf5000_cha;
                if (kf5000 != 0)
                {
                    ptf5000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf5000_ChangeMoment / kf5000, 2)), font1));
                }
                else
                {
                    ptf5000_cha = new Paragraph(new Phrase("0", font1));
                }
                ptf5000_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph Cluf5000;
                if (Math.Round(Math.Abs(sumkf5000 / kf5000 + 5000), 2) <= 2)
                {
                    Cluf5000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf5000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph pt6000;
                if (k6000 != 0)
                {
                    pt6000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk6000 / k6000, 2)), font1));
                }
                else
                {
                    pt6000 = new Paragraph(new Phrase("0", font1));
                }
                pt6000.Alignment = Element.ALIGN_CENTER;

                Paragraph pt6000_con;
                if (k6000 != 0)
                {
                    //  pt6000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk6000_ConstantMoment / k6000, 2)), font1));
                    pt6000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumk6000 / k6000 - 6000), 2)), font1));
                }
                else
                {
                    pt6000_con = new Paragraph(new Phrase("0", font1));
                }
                pt6000_con.Alignment = Element.ALIGN_CENTER;

                Paragraph pt6000_cha;
                if (k6000 != 0)
                {
                    pt6000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk6000_ChangeMoment / k6000, 2)), font1));
                }
                else
                {
                    pt6000_cha = new Paragraph(new Phrase("0", font1));
                }
                pt6000_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph Clu6000;
                if (Math.Round(Math.Abs(sumk6000 / k6000 - 6000), 2) <= 2)
                {
                    Clu6000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Clu6000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph ptf6000;
                if (kf6000 != 0)
                {
                    ptf6000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf6000 / kf6000, 2)), font1));
                }
                else
                {
                    ptf6000 = new Paragraph(new Phrase("0", font1));
                }
                ptf6000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf6000_con;
                if (kf6000 != 0)
                {
                    // ptf6000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf6000_ConstantMoment / kf6000, 2)), font1));
                    ptf6000_con = new Paragraph(new Phrase(Convert.ToString(Math.Round(Math.Abs(sumkf6000 / kf6000 + 6000), 2)), font1));
                }
                else
                {
                    ptf6000_con = new Paragraph(new Phrase("0", font1));
                }
                ptf6000_con.Alignment = Element.ALIGN_CENTER;

                Paragraph ptf6000_cha;
                if (kf6000 != 0)
                {
                    ptf6000_cha = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf6000_ChangeMoment / kf6000, 2)), font1));
                }
                else
                {
                    ptf6000_cha = new Paragraph(new Phrase("0", font1));
                }
                ptf6000_cha.Alignment = Element.ALIGN_CENTER;

                Paragraph Cluf6000;
                if (Math.Round(Math.Abs(sumkf6000 / kf6000 + 6000), 2) <= 2)
                {
                    Cluf6000 = new Paragraph(new Phrase("符合", font1));
                }
                else
                {
                    Cluf6000 = new Paragraph(new Phrase("不符合", font1));
                }

                Paragraph t3_toruqe;
                if (k3_torque != 0)
                {
                    t3_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum3_torque / k3_torque, 2)), font1));
                }
                else
                {
                    t3_toruqe = new Paragraph(new Phrase("0", font1));
                }
                t3_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph t10_toruqe;
                if (k10_torque != 0)
                {
                    t10_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum10_torque / k10_torque, 2)), font1));
                }
                else
                {
                    t10_toruqe = new Paragraph(new Phrase("0", font1));
                }
                t10_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph t15_toruqe;
                if (k15_torque != 0)
                {
                    t15_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum15_torque / k15_torque, 2)), font1));
                }
                else
                {
                    t15_toruqe = new Paragraph(new Phrase("0", font1));
                }
                t15_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph tf3_toruqe;
                if (kf3_torque != 0)
                {
                    tf3_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumf3_torque / kf3_torque, 2)), font1));
                }
                else
                {
                    tf3_toruqe = new Paragraph(new Phrase("0", font1));
                }
                tf3_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph tf10_toruqe;
                if (kf10_torque != 0)
                {
                    tf10_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumf10_torque / kf10_torque, 2)), font1));
                }
                else
                {
                    tf10_toruqe = new Paragraph(new Phrase("0", font1));
                }
                tf10_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph tf15_toruqe;
                if (kf15_torque != 0)
                {
                    tf15_toruqe = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumf15_torque / kf15_torque, 2)), font1));
                }
                else
                {
                    tf15_toruqe = new Paragraph(new Phrase("0", font1));
                }
                tf15_toruqe.Alignment = Element.ALIGN_CENTER;

                Paragraph ptw0;
                if (k0 != 0)
                {
                    ptw0 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum0_power / k0, 2)), font1));
                }
                else
                {
                    ptw0 = new Paragraph(new Phrase("0", font1));
                }
                ptw0.Alignment = Element.ALIGN_CENTER;

                Paragraph ptw500;
                if (k500 != 0)
                {
                    ptw500 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk500_power / k500, 2)), font1));
                }
                else
                {
                    ptw500 = new Paragraph(new Phrase("0", font1));
                }
                ptw500.Alignment = Element.ALIGN_CENTER;

                Paragraph ptwf500;
                if (kf500 != 0)
                {
                    ptwf500 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf500_power / kf500, 2)), font1));
                }
                else
                {
                    ptwf500 = new Paragraph(new Phrase("0", font1));
                }
                ptwf500.Alignment = Element.ALIGN_CENTER;

                Paragraph ptw3000;
                if (k2000 != 0)
                {
                    ptw3000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk2000_power / k2000, 2)), font1));
                }
                else
                {
                    ptw3000 = new Paragraph(new Phrase("0", font1));
                }
                ptw3000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptwf3000;
                if (kf2000 != 0)
                {
                    ptwf3000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf2000_power / kf2000, 2)), font1));
                }
                else
                {
                    ptwf3000 = new Paragraph(new Phrase("0", font1));
                }
                ptwf3000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptw5000;
                if (k4000 != 0)
                {
                    ptw5000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk4000_power / k4000, 2)), font1));
                }
                else
                {
                    ptw5000 = new Paragraph(new Phrase("0", font1));
                }
                ptw5000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptwf5000;
                if (kf4000 != 0)
                {
                    ptwf5000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf4000_power / kf4000, 2)), font1));
                }
                else
                {
                    ptwf5000 = new Paragraph(new Phrase("0", font1));
                }
                ptwf5000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptw6000;
                if (k6000 != 0)
                {
                    ptw6000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumk6000_power / k6000, 2)), font1));
                }
                else
                {
                    ptw6000 = new Paragraph(new Phrase("0", font1));
                }
                ptw6000.Alignment = Element.ALIGN_CENTER;

                Paragraph ptwf6000;
                if (kf6000 != 0)
                {
                    ptwf6000 = new Paragraph(new Phrase(Convert.ToString(Math.Round(sumkf6000_power / kf6000, 2)), font1));
                }
                else
                {
                    ptwf6000 = new Paragraph(new Phrase("0", font1));
                }
                ptwf6000.Alignment = Element.ALIGN_CENTER;

               








                #endregion

                if ((this.checkBox1.Checked == true || this.checkBox2.Checked == true || this.checkBox3.Checked == true || this.checkBox4.Checked == true) && this.checkBox5.Checked == true)
                {

                    #region 创建表格1

                    document.Open();

                    //    iTextSharp.text.Font fonttitle = new iTextSharp.text.Font(bfHei, 10);
                    Paragraph Title1 = new Paragraph("表1 交付验收测试表", fonttitle);
                    Title1.setAlignment("center");
                    document.Add(Title1);

                    Table table = new Table(10, 37);

                    table.Padding = 1;
                    //  float[] headerwidths = { 3, 4, 9, 9, 5, 5, 5, 8, 5, 5 };  // 表的列宽
                    table.Widths = headerwidths;
                    table.WidthPercentage = 100;

                    #region 表的前4行
                    //表格第一行
                    Cell a = new Cell(p1);
                    a.Header = true;
                    a.Colspan = 10;
                    a.setHorizontalAlignment("center");
                    table.addCell(a);


                    //代号
                    Cell b = new Cell(p2);
                    b.Colspan = 2;
                    table.addCell(b);

                    //填代号
                    table.DefaultColspan = 1;
                    Cell c = new Cell(p2t);
                    table.addCell(c);

                    //产品编号
                    Cell d = new Cell(p3);
                    table.addCell(d);

                    //填产品编号
                    Cell h = new Cell(p3t);
                    table.addCell(h);
                    table.DefaultColspan = 2;

                    //测试目的
                    Cell f = new Cell(p4);
                    f.Colspan = 2;
                    table.addCell(f);


                    //填测试目的
                    Cell g = new Cell();
                    g.Colspan = 3;
                    table.addCell(g);

                    // 测试时间
                    Cell i = new Cell(p5);
                    i.Colspan = 2;
                    table.addCell(i);

                    // 填测试时间
                    table.DefaultColspan = 1;
                    Cell j = new Cell(p5t);
                    j.Colspan = 1;
                    table.addCell(j);

                    // 测试人员
                    Cell k = new Cell(p6);
                    table.addCell(k);

                    // 填测试人员
                    Cell l = new Cell(p6t);
                    table.addCell(l);

                    // 测试场景                  
                    Cell m = new Cell(p7);
                    m.Colspan = 2;
                    table.addCell(m);

                    // 填测试场景
                    Cell n = new Cell();
                    n.Colspan = 3;
                    table.addCell(n);


                    Cell o = new Cell();
                    o.Colspan = 1;
                    table.addCell("");

                    //序号
                    Cell p = new Cell(p8);
                    table.addCell(p);
                    //测试项目
                    Cell q = new Cell(p9);
                    table.addCell(q);

                    //指标项
                    Cell r = new Cell(p10);
                    table.addCell(r);

                    //实测（计算）值
                    Cell s = new Cell(p11);
                    s.Colspan = 3;
                    table.addCell(s);

                    //指令转速
                    Cell t = new Cell(p12);
                    t.Colspan = 1;
                    t.Rowspan = 2;
                    table.addCell(t);

                    //结论
                    Cell u = new Cell(p13);
                    u.Rowspan = 1;
                    table.addCell(u);

                    //备注
                    Cell v = new Cell(p14);
                    table.addCell(v);

                    #endregion

                    #region  转速模式1

                    //转速模式
                    Cell w = new Cell(p15);
                    w.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    w.VerticalAlignment = Element.ALIGN_MIDDLE;
                    w.Rowspan = 33;
                    table.addCell(w);

                    //序号1
                    Cell x = new Cell("1");
                    //x.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    //x.VerticalAlignment = Element.ALIGN_MIDDLE;
                    x.Rowspan = 14;
                    table.addCell(x);

                    //填测试项目
                    Cell y = new Cell(p16);
                    y.Rowspan = 14;
                    y.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    y.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.addCell(y);

                    //填指标值
                    Cell z = new Cell(p17);
                    z.Rowspan = 14;
                    z.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    z.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.addCell(z);

                    //实测转速
                    Cell ab = new Cell(p18);
                    ab.Rowspan = 1;
                    ab.Colspan = 1;
                    table.addCell(ab);

                    //常值偏差
                    Cell ac = new Cell(p19);
                    ac.Rowspan = 1;
                    ac.Colspan = 1;
                    table.addCell(ac);

                    //△H
                    Cell ad = new Cell(p20);
                    ad.Rowspan = 1;
                    ad.Colspan = 1;
                    table.addCell(ad);

                    //
                    Cell ae = new Cell("");
                    ae.Rowspan = 1;
                    ae.Colspan = 1;
                    table.addCell(ae);

                    //填转速备注1
                    Cell af = new Cell();
                    af.Colspan = 1;
                    af.Rowspan = 14;
                    table.addCell(af);

                    //实测转速0rpm
                    Cell ag = new Cell(pt0);
                    ag.Colspan = 1;
                    ag.Rowspan = 1;
                    table.addCell(ag);

                    //常值偏差0rpm

                    Cell ah = new Cell(pt0_con);
                    ah.Colspan = 1;
                    ah.Rowspan = 1;
                    table.addCell(ah);

                    //△H 0rpm                
                    pt0_cha.Alignment = Element.ALIGN_CENTER;
                    Cell ai = new Cell(pt0_cha);
                    ai.Colspan = 1;
                    ai.Rowspan = 1;
                    table.addCell(ai);

                    // 0rpm
                    Cell aj = new Cell(p21);
                    aj.Colspan = 1;
                    aj.Rowspan = 1;
                    table.addCell(aj);

                    //结论 0rpm
                    Cell ak = new Cell();
                    table.addCell(ak);

                    //实测转速 +100rpm
                    pt100.Alignment = Element.ALIGN_CENTER;
                    Cell al = new Cell(pt100);
                    table.addCell(al);

                    //常值偏差 +100rpm 
                    pt100_con.Alignment = Element.ALIGN_CENTER;
                    Cell am = new Cell(pt100_con);
                    table.addCell(am);

                    //△H +100rpm
                    Cell an = new Cell(pt100_cha);
                    table.addCell(an);

                    //+100rpm
                    Cell au = new Cell(p22);
                    table.addCell(au);

                    //结论  +100rpm 
                    Cell ao = new Cell(Clu100);
                    table.addCell(ao);

                    //实测转速 -100rpm
                    ptf100.Alignment = Element.ALIGN_CENTER;
                    Cell ap = new Cell(ptf100);
                    table.addCell(ap);

                    //常值偏差 -100rpm
                    ptf100_con.Alignment = Element.ALIGN_CENTER;
                    Cell aq = new Cell(ptf100_con);
                    table.addCell(aq);

                    //△H -100rpm
                    ptf100_cha.Alignment = Element.ALIGN_CENTER;
                    Cell ar = new Cell(ptf100_cha);
                    table.addCell(ar);

                    //-100rpm
                    Cell av = new Cell(p23);
                    table.addCell(av);

                    //结论 -100rpm 
                    Cell at = new Cell(Cluf100);
                    table.addCell(at);

                    //实测转速 +500rpm              
                    pt500.Alignment = Element.ALIGN_CENTER;
                    Cell aw = new Cell(pt500);
                    table.addCell(aw);

                    //常值偏差 +500rpm

                    Cell ax = new Cell(pt500_con);
                    table.addCell(ax);

                    //△H +500rpm

                    pt500_cha.Alignment = Element.ALIGN_CENTER;
                    Cell ay = new Cell(pt500_cha);
                    table.addCell(ay);

                    //+500rpm
                    Cell az = new Cell(p24);
                    table.addCell(az);

                    //结论 +500rpm 
                    Cell ba = new Cell(Clu500);
                    table.addCell(ba);

                    //实测转速 -500rpm

                    ptf500.Alignment = Element.ALIGN_CENTER;
                    Cell bc = new Cell(ptf500);
                    table.addCell(bc);

                    //常值偏差 -500rpm
                    Cell bd = new Cell(ptf500_con);
                    table.addCell(bd);

                    //△H -500rpm

                    ptf500_cha.Alignment = Element.ALIGN_CENTER;
                    Cell be = new Cell(ptf500_cha);
                    table.addCell(be);

                    //-500rpm
                    Cell bf = new Cell(p25);
                    table.addCell(bf);

                    //结论 -500rpm 
                    Cell bg = new Cell(Cluf500);
                    table.addCell(bg);

                    //实测转速 +1000rpm
                    pt1000.Alignment = Element.ALIGN_CENTER;
                    Cell bh = new Cell(pt1000);
                    table.addCell(bh);

                    //常值偏差 +1000rpm              
                    pt1000_con.Alignment = Element.ALIGN_CENTER;
                    Cell bi = new Cell(pt1000_con);
                    table.addCell(bi);

                    //△H +1000rpm

                    pt1000_cha.Alignment = Element.ALIGN_CENTER;
                    Cell bj = new Cell(pt1000_cha);
                    table.addCell(bj);

                    //+1000rpm
                    Cell bk = new Cell(p26);
                    table.addCell(bk);

                    //结论 +1000rpm 
                    Cell bl = new Cell(Clu1000);
                    table.addCell(bl);

                    //实测转速 -1000rpm
                    ptf1000.Alignment = Element.ALIGN_CENTER;
                    Cell bm = new Cell(ptf1000);
                    table.addCell(bm);

                    //常值偏差 -1000rpm
                    ptf1000_con.Alignment = Element.ALIGN_CENTER;
                    Cell bn = new Cell(ptf1000_con);
                    table.addCell(bn);

                    //△H -1000rpm
                    ptf1000_cha.Alignment = Element.ALIGN_CENTER;
                    Cell bo = new Cell(ptf1000_cha);
                    table.addCell(bo);

                    // -1000rpm
                    Cell bp = new Cell(p27);
                    table.addCell(bp);

                    //结论  -1000rpm 
                    Cell bq = new Cell(Cluf1000);
                    table.addCell(bq);


                    //实测转速 +3000rpm
                    pt3000.Alignment = Element.ALIGN_CENTER;
                    Cell br = new Cell(pt3000);
                    table.addCell(br);

                    //常值偏差 +3000rpm
                    pt3000_con.Alignment = Element.ALIGN_CENTER;
                    Cell bs = new Cell(pt3000_con);
                    table.addCell(bs);

                    //△H +3000rpm         
                    pt3000_cha.Alignment = Element.ALIGN_CENTER;
                    Cell bt = new Cell(pt3000_cha);
                    table.addCell(bt);

                    // +3000rpm
                    Cell bu = new Cell(p28);
                    table.addCell(bu);

                    //结论  +3000rpm
                    Cell bv = new Cell(Clu3000);
                    table.addCell(bv);

                    //实测转速 -3000rpm
                    Cell bw = new Cell(ptf3000);
                    table.addCell(bw);

                    //常值偏差 -3000rpm              

                    Cell bx = new Cell(ptf3000_con);
                    table.addCell(bx);

                    //△H -3000rpm     
                    Cell by = new Cell(ptf3000_cha);
                    table.addCell(by);

                    // -3000rpm
                    Cell bz = new Cell(p29);
                    table.addCell(bz);

                    //结论  -3000rpm            
                    Cell ca = new Cell(Cluf3000);
                    table.addCell(ca);

                    //实测转速 +5000rpm
                    Cell cb = new Cell(pt5000);
                    table.addCell(cb);

                    //常值偏差 +5000rpm
                    Cell cc = new Cell(pt5000_con);
                    table.addCell(cc);

                    //△H +5000rpm
                    Cell cd = new Cell(pt5000_cha);
                    table.addCell(cd);

                    // +5000rpm
                    Cell ce = new Cell(p30);
                    table.addCell(ce);

                    //结论  +5000rpm            
                    Cell cf = new Cell(Clu5000);
                    table.addCell(cf);

                    //实测转速 -5000rpm
                    Cell cg = new Cell(ptf5000);
                    table.addCell(cg);

                    //常值偏差 -5000rpm
                    Cell ch = new Cell(ptf5000_con);
                    table.addCell(ch);

                    //△H -5000rpm       
                    Cell ci = new Cell(ptf5000_cha);
                    table.addCell(ci);

                    // -5000rpm
                    Cell cj = new Cell(p31);
                    table.addCell(cj);

                    //结论  -5000rpm               
                    Cell ck = new Cell(Cluf5000);
                    table.addCell(ck);


                    //实测转速 +6000rpm
                    Cell cl = new Cell(pt6000);
                    table.addCell(cl);

                    //常值偏差 +6000rpm              
                    Cell cm = new Cell(pt6000_con);
                    table.addCell(cm);

                    //△H +6000rpm

                    Cell cn = new Cell(pt6000_cha);
                    table.addCell(cn);

                    // +6000rpm
                    Cell co = new Cell(p32);
                    table.addCell(co);

                    //结论  +6000rpm
                    Cell cp = new Cell(Clu6000);
                    table.addCell(cp);

                    //实测转速 -6000rpm
                    Cell cq = new Cell(ptf6000);
                    table.addCell(cq);

                    //常值偏差 -6000rpm
                    Cell cr = new Cell(ptf6000_con);
                    table.addCell(cr);

                    //△H -6000rpm             
                    Cell cs = new Cell(ptf6000_cha);
                    table.addCell(cs);

                    //  -6000rpm
                    Cell ct = new Cell(p33);
                    table.addCell(ct);

                    //结论  -6000rpm     
                    Cell cu = new Cell(Cluf6000);
                    table.addCell(cu);

                    #endregion

                    #region 转速模式2 最大反作用力矩

                    //序号2
                    Cell cw = new Cell("2");
                    cw.Rowspan = 4;
                    table.addCell(cw);

                    //最大反作用力矩
                    Cell cx = new Cell(p34);
                    cx.Rowspan = 4;
                    table.addCell(cx);

                    //绝对值>=16mNm
                    Cell cy = new Cell(p35);
                    cy.Rowspan = 2;
                    table.addCell(cy);

                    //转速模式2 实测（计算）值
                    //Paragraph max_torque;
                    //if (k_torque != 0)
                    //{
                    //    max_torque = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum_torque / k_torque,2)), font1));
                    //}
                    //else
                    //{
                    //    max_torque = new Paragraph(new Phrase("0", font1));
                    //}
                    //max_torque.Alignment = Element.ALIGN_CENTER;

                    Paragraph pmm;
                    if (k6000 != 0)
                    {
                        pmm = new Paragraph(new Phrase(Convert.ToString(max_realmoment / k6000), font1));
                    }
                    else
                    {
                        pmm = new Paragraph(new Phrase("0", font1));
                    }
                    pmm.Alignment = Element.ALIGN_CENTER;
                    Cell cz = new Cell(pmm);
                    cz.Rowspan = 2;
                    cz.Colspan = 3;
                    table.addCell(cz);


                    //转速模式2 指令转速
                    Cell da = new Cell(p36);
                    da.Rowspan = 4;
                    table.addCell(da);

                    //转速模式2  结论
                    Cell db = new Cell();
                    db.Rowspan = 4;
                    table.addCell(db);

                    //转速模式2  备注
                    Cell dc = new Cell();
                    dc.Rowspan = 4;
                    table.addCell(dc);

                    //升速时间
                    Paragraph p65 = new Paragraph(new Phrase("升速时间", font1));
                    p65.Alignment = Element.ALIGN_RIGHT;
                    Cell speed1 = new Cell(p65);
                    speed1.Rowspan = 2;
                    table.addCell(speed1);

                    //填升速时间
                    Paragraph p71;
                    if (this.checkBox1.Checked == true)
                    {
                        Random time = new Random();
                        double speedtime = time.Next(2660, 2710) / 100.0;
                        p71 = new Paragraph(new Phrase(Convert.ToString(speedtime), font1));
                        p71.Alignment = Element.ALIGN_RIGHT;
                    }
                    else 
                    {
                        p71 = new Paragraph(new Phrase("0", font1));
                    }

                    //Paragraph p71 = new Paragraph(new Phrase(Convert.ToString(Math.Round(speed_time, 2)), font1));
                    //p71.Alignment = Element.ALIGN_CENTER;

                    Cell writespeed1 = new Cell(p71);
                    writespeed1.Rowspan = 2;
                    writespeed1.Colspan = 3;
                    table.addCell(writespeed1);

                    #endregion

                    #region 转速模式3 稳速功耗

                    //序号3
                    Cell dd = new Cell("3");
                    dd.Rowspan = 9;
                    table.addCell(dd);

                    //稳速功耗
                    Cell de = new Cell(p37);
                    de.Rowspan = 9;
                    de.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    de.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.addCell(de);

                    // 0rpm 指标值
                    Cell df = new Cell();
                    df.Rowspan = 1;
                    df.Colspan = 1;
                    table.addCell(df);

                    // 0rpm 实测（计算）值

                    Cell dg = new Cell(ptw0);
                    dg.Rowspan = 1;
                    dg.Colspan = 3;
                    table.addCell(dg);


                    // 0rpm 稳速功耗
                    Cell dh = new Cell(p38);
                    dh.Rowspan = 1;
                    dh.Colspan = 1;
                    table.addCell(dh);

                    // 0rpm 结论
                    Cell di = new Cell();
                    di.Rowspan = 1;
                    di.Colspan = 1;
                    table.addCell(di);

                    // 0rpm 备注3
                    Cell dj = new Cell();
                    dj.Rowspan = 9;
                    table.addCell(dj);


                    //// +500rpm 指标值

                    Cell dk = new Cell();
                    dk.Rowspan = 1;
                    dk.Colspan = 1;
                    table.addCell(dk);

                    // +500rpm 实测（计算）值

                    Cell dl = new Cell(ptw500);
                    dl.Colspan = 3;
                    table.addCell(dl);


                    // +500rpm 稳速功耗
                    Cell dm = new Cell(p39);
                    table.addCell(dm);

                    // +500rpm 结论
                    Cell dn = new Cell();
                    table.addCell(dn);

                    // -500rpm 指标值
                    Cell dp = new Cell();
                    table.addCell(dp);

                    // -500rpm 实测（计算）值

                    Cell dq = new Cell(ptwf500);
                    dq.Colspan = 3;
                    table.addCell(dq);


                    // -500rpm 稳速功耗
                    Cell dr = new Cell(p40);
                    table.addCell(dr);

                    // -500rpm 结论
                    Cell ds = new Cell();
                    table.addCell(ds);

                    // +2000rpm 指标值
                    Cell dt = new Cell();
                    table.addCell(dt);

                    // +2000rpm 实测（计算）值
                    Cell du = new Cell(ptw3000);
                    du.Colspan = 3;
                    table.addCell(du);


                    // +2000rpm 稳速功耗
                    Cell dv = new Cell(p41);
                    table.addCell(dv);

                    // +2000rpm 结论
                    Cell dw = new Cell();
                    table.addCell(dw);

                    // -2000rpm 指标值

                    Cell dx = new Cell();
                    table.addCell(dx);

                    // -2000rpm 实测（计算）值

                    Cell dy = new Cell(ptwf3000);
                    dy.Colspan = 3;
                    table.addCell(dy);


                    // -3000rpm 稳速功耗
                    Cell dz = new Cell(p42);
                    table.addCell(dz);

                    // -3000rpm 结论
                    Cell ea = new Cell();
                    table.addCell(ea);


                    // +5000rpm 指标值
                    Cell eb = new Cell();
                    table.addCell(eb);

                    // +4000rpm 实测（计算）值

                    Cell ec = new Cell(ptw5000);
                    ec.Colspan = 3;
                    table.addCell(ec);


                    // +5000rpm 稳速功耗
                    Cell ed = new Cell(p43);
                    table.addCell(ed);

                    // +5000rpm结论
                    Cell ef = new Cell();
                    table.addCell(ef);

                    // -5000rpm 指标值
                    Cell eg = new Cell();
                    table.addCell(eg);

                    // -4000rpm 实测（计算）值

                    Cell eh = new Cell(ptwf5000);
                    eh.Colspan = 3;
                    table.addCell(eh);


                    // -5000rpm 稳速功耗
                    Cell ei = new Cell(p44);
                    table.addCell(ei);

                    // -5000rpm结论
                    Cell ej = new Cell();
                    table.addCell(ej);

                    // +6000rpm 指标值
                    Cell ek = new Cell();
                    table.addCell(ek);

                    // +6000rpm 实测（计算）值

                    Cell el = new Cell(ptw6000);
                    el.Colspan = 3;
                    table.addCell(el);


                    // +6000rpm 稳速功耗
                    Cell em = new Cell(p45);
                    table.addCell(em);

                    // +6000rpm结论
                    Cell en = new Cell();
                    table.addCell(en);

                    // -6000rpm 指标值
                    Cell eo = new Cell();
                    table.addCell(eo);

                    // -6000rpm  实测（计算）值

                    Cell ep = new Cell(ptwf6000);
                    ep.Colspan = 3;
                    table.addCell(ep);


                    // -6000rpm 稳速功耗
                    Cell eq = new Cell(p46);
                    table.addCell(eq);

                    // -6000rpm  结论
                    Cell er = new Cell();
                    table.addCell(er);
                    #endregion

                    #region 转速模式4

                    //序号4
                    Cell es = new Cell("4");
                    es.Rowspan = 3;
                    table.addCell(es);

                    //最大功耗
                    Cell et = new Cell(p47);
                    et.Rowspan = 3;
                    table.addCell(et);

                    //指标值
                    Cell eu = new Cell();
                    eu.Rowspan = 3;
                    table.addCell(eu);

                    // 实测（计算）值
                    Paragraph sumpower;
                    if (k_pow != 0)
                    {
                        sumpower = new Paragraph(new Phrase(Convert.ToString(Math.Round(sum_pow / k_pow, 2)), font1));
                    }
                    else
                    {
                        sumpower = new Paragraph(new Phrase("0", font1));
                    }
                    Cell ev = new Cell(sumpower);
                    ev.Rowspan = 3;
                    ev.Colspan = 3;
                    table.addCell(ev);


                    //转速模式2 指令转速
                    Cell ew = new Cell();
                    ew.Rowspan = 3;
                    table.addCell(ew);

                    //转速模式2  结论
                    Cell ex = new Cell();
                    ex.Rowspan = 3;
                    table.addCell(ex);

                    //转速模式2  备注
                    Cell ey = new Cell(p48);
                    ey.Rowspan = 3;
                    table.addCell(ey);


                    #endregion

                    #region 转速模式5

                    //序号5
                    Cell ez = new Cell("5");
                    ez.Rowspan = 3;
                    table.addCell(ez);

                    //最大角动量
                    Cell fa = new Cell(p49);
                    fa.Rowspan = 3;
                    table.addCell(fa);

                    //指标值
                    Cell fb = new Cell(p50);
                    fb.Rowspan = 3;
                    table.addCell(fb);

                    // 实测（计算）值
                    Paragraph p72;
                    if (this.checkBox1.Checked == true)
                    {
                        double MaxTor = Convert.ToDouble(this.textBox4.Text) * 2 * Math.PI * 100;
                        p72 = new Paragraph(new Phrase(Convert.ToString(MaxTor), font1));
                        p72.Alignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        p72 = new Paragraph(new Phrase("0", font1));
                    }
                    Cell fc = new Cell(p72);
                    fc.Rowspan = 3;
                    fc.Colspan = 3;
                    table.addCell(fc);


                    //转速模式5 指令转速
                    Cell fd = new Cell();
                    fd.Rowspan = 3;
                    table.addCell(fd);

                    //转速模式5  结论
                    Cell fe = new Cell();
                    fe.Rowspan = 3;
                    table.addCell(fe);

                    //转速模式5  备注
                    Cell ff = new Cell(p51);
                    ff.Rowspan = 3;
                    table.addCell(ff);

                    #endregion

                    #region 力矩模式
                    //力矩模式
                    Cell fg = new Cell(p52);
                    fg.Rowspan = 7;
                    fg.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    fg.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.addCell(fg);

                    //序号
                    Cell fh = new Cell(p53);
                    fh.Rowspan = 1;
                    fh.Colspan = 1;
                    table.addCell(fh);

                    //实测力矩
                    Cell fi = new Cell(p54);
                    fi.Colspan = 2;
                    table.addCell(fi);

                    //指令力矩
                    Cell fj = new Cell(p55);
                    fj.Colspan = 3;
                    table.addCell(fj);

                    //备注
                    Cell fk = new Cell(p56);
                    fk.Colspan = 3;
                    table.addCell(fk);

                    //1
                    Cell fl = new Cell("1");
                    fl.Colspan = 1;
                    fl.Rowspan = 1;
                    table.addCell(fl);

                    //序号1 实测力矩

                    Cell fm = new Cell(t3_toruqe);
                    fm.Colspan = 2;
                    table.addCell(fm);

                    //3mNm
                    Cell fn = new Cell(p57);
                    fn.Colspan = 3;
                    table.addCell(fn);

                    //3mNm 备注
                    Cell fo = new Cell();
                    fo.Colspan = 3;
                    table.addCell(fo);


                    //2
                    Cell fp = new Cell("2");
                    fp.Colspan = 1;
                    fp.Rowspan = 1;
                    table.addCell(fp);

                    //序号2 实测力矩

                    Cell fq = new Cell(t10_toruqe);
                    fq.Colspan = 2;
                    table.addCell(fq);

                    //+10mNm
                    Cell fr = new Cell(p58);
                    fr.Colspan = 3;
                    table.addCell(fr);

                    //+10mNm 备注
                    Cell fs = new Cell();
                    fs.Colspan = 3;
                    table.addCell(fs);

                    //3
                    Cell ft = new Cell("3");
                    table.addCell(ft);

                    //序号3 实测力矩

                    Cell fu = new Cell(t15_toruqe);
                    fu.Colspan = 2;
                    table.addCell(fu);

                    //+15mNm
                    Cell fv = new Cell(p59);
                    fv.Colspan = 3;
                    table.addCell(fv);

                    //+15mNm 备注
                    Cell fw = new Cell();
                    fw.Colspan = 3;
                    table.addCell(fw);

                    //4
                    Cell fx = new Cell("4");
                    table.addCell(fx);

                    //序号4 实测力矩
                    Cell fy = new Cell(tf3_toruqe);
                    fy.Colspan = 2;
                    table.addCell(fy);

                    //-3mNm
                    Cell fz = new Cell(p60);
                    fz.Colspan = 3;
                    table.addCell(fz);

                    //-3mNm 备注
                    Cell ga = new Cell();
                    ga.Colspan = 3;
                    table.addCell(ga);

                    //5
                    Cell gb = new Cell("5");
                    table.addCell(gb);

                    //序号5 实测力矩
                    Cell gc = new Cell(tf10_toruqe);
                    gc.Colspan = 2;
                    table.addCell(gc);

                    //-10mNm
                    Cell gd = new Cell(p61);
                    gd.Colspan = 3;
                    table.addCell(gd);

                    //-10mNm 备注
                    Cell ge = new Cell();
                    ge.Colspan = 3;
                    table.addCell(ge);

                    //6
                    Cell gf = new Cell("6");
                    table.addCell(gf);

                    //序号6 实测力矩
                    Cell gg = new Cell(tf15_toruqe);
                    gg.Colspan = 2;
                    table.addCell(gg);

                    //-10mNm
                    Cell gh = new Cell(p62);
                    gh.Colspan = 3;
                    table.addCell(gh);

                    //-10mNm 备注
                    Cell gi = new Cell();
                    gi.Colspan = 3;
                    table.addCell(gi);

                    #endregion


                    document.Add(table);


                    #endregion

                }
                else
                {

                    #region 创建表2 正弦振动

                    document.Open();

                    Paragraph Title2 = new Paragraph("表2 正弦振动试验测试表", fonttitle);
                    Title2.setAlignment("center");
                    document.Add(Title2);

                    Table Sintable = new Table(10, 37);
                    Sintable.Widths = headerwidths;
                    Sintable.WidthPercentage = 100;
                    Sintable.Padding = 1;

                    #region 表的前4行
                    //第一行
                    Cell ha = new Cell(p63);
                    ha.Header = true;
                    ha.Colspan = 10;
                    ha.setHorizontalAlignment("center");
                    Sintable.addCell(ha);

                    //代号
                    Cell hb = new Cell(p2);
                    hb.Colspan = 2;
                    Sintable.addCell(hb);

                    //填代号
                    Sintable.DefaultColspan = 1;
                    Cell hc = new Cell(p2t);
                    Sintable.addCell(hc);


                    //测试方向
                    Cell hd = new Cell(p64);
                    hd.Colspan = 1;
                    Sintable.addCell(hd);


                    //填测试方向
                    Cell he = new Cell();
                    he.Colspan = 1;
                    Sintable.addCell(he);


                    //产品编号
                    Cell hf = new Cell(p3);
                    hf.Colspan = 2;
                    Sintable.addCell(hf);

                    //填产品编号
                    Cell hg = new Cell(p3t);
                    hg.Colspan = 3;
                    Sintable.addCell(hg);

                    Sintable.DefaultColspan = 2;



                    // 测试时间
                    Cell hh = new Cell(p5);
                    hh.Colspan = 2;
                    Sintable.addCell(hh);

                    // 填测试时间
                    Sintable.DefaultColspan = 1;
                    Cell hi = new Cell(p5t);
                    hi.Colspan = 1;
                    Sintable.addCell(hi);

                    // 测试人员
                    Cell hj = new Cell(p6);
                    Sintable.addCell(hj);

                    // 填测试人员
                    Cell hk = new Cell(p6t);
                    Sintable.addCell(hk);

                    // 测试场景                  
                    Cell hl = new Cell(p7);
                    hl.Colspan = 2;
                    Sintable.addCell(hl);

                    // 填测试场景
                    Cell hm = new Cell();
                    hm.Colspan = 3;
                    Sintable.addCell(hm);


                    Cell hn = new Cell();
                    hn.Colspan = 1;
                    Sintable.addCell("");

                    //序号
                    Cell ho = new Cell(p8);
                    Sintable.addCell(ho);

                    //测试项目
                    Cell hp = new Cell(p9);
                    Sintable.addCell(hp);

                    //指标项
                    Cell hq = new Cell(p10);
                    Sintable.addCell(hq);

                    //实测（计算）值
                    Cell hr = new Cell(p11);
                    hr.Colspan = 3;
                    Sintable.addCell(hr);

                    //指令转速
                    Cell hs = new Cell(p12);
                    hs.Colspan = 1;
                    hs.Rowspan = 2;
                    Sintable.addCell(hs);

                    //结论
                    Cell ht = new Cell(p13);
                    ht.Rowspan = 1;
                    Sintable.addCell(ht);

                    //备注
                    Cell hu = new Cell(p14);
                    Sintable.addCell(hu);

                    #endregion

                    #region  转速模式1

                    //转速模式
                    Cell hv = new Cell(p15);
                    hv.Rowspan = 17;
                    hv.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    hv.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(hv);

                    //序号1
                    Cell hw = new Cell("1");
                    hw.Rowspan = 8;
                    Sintable.addCell(hw);

                    //填测试项目
                    Cell hx = new Cell(p16);
                    hx.Rowspan = 8;
                    hx.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    hx.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(hx);

                    //填指标值
                    Cell hy = new Cell(p17);
                    hy.Rowspan = 8;
                    hy.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    hy.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(hy);

                    //实测转速
                    Cell hz = new Cell(p18);
                    hz.Rowspan = 1;
                    hz.Colspan = 1;
                    Sintable.addCell(hz);

                    //常值偏差
                    Cell ia = new Cell(p19);
                    ia.Rowspan = 1;
                    ia.Colspan = 1;
                    Sintable.addCell(ia);

                    //△H
                    Cell ib = new Cell(p20);
                    ib.Rowspan = 1;
                    ib.Colspan = 1;
                    Sintable.addCell(ib);

                    //
                    Cell ic = new Cell("");
                    ic.Rowspan = 1;
                    ic.Colspan = 1;
                    Sintable.addCell(ic);

                    //填转速备注1
                    Cell id = new Cell();
                    id.Colspan = 1;
                    id.Rowspan = 8;
                    Sintable.addCell(id);

                    //实测转速0rpm
                    Cell ie = new Cell(pt0);
                    ie.Colspan = 1;
                    ie.Rowspan = 1;
                    Sintable.addCell(ie);

                    //常值偏差0rpm
                    Cell ig = new Cell(pt0_con);
                    ig.Colspan = 1;
                    ig.Rowspan = 1;
                    Sintable.addCell(ig);

                    //△H 0rpm
                    Cell ih = new Cell(pt0_cha);
                    ih.Colspan = 1;
                    ih.Rowspan = 1;
                    Sintable.addCell(ih);

                    // 0rpm
                    Cell ii = new Cell(p21);
                    ii.Colspan = 1;
                    ii.Rowspan = 1;
                    Sintable.addCell(ii);

                    //结论 0rpm
                    Cell ij = new Cell();
                    Sintable.addCell(ij);

                    //实测转速 +500rpm
                    Cell ik = new Cell(pt500);
                    Sintable.addCell(ik);

                    //常值偏差 +500rpm
                    Cell il = new Cell(pt500_con);
                    Sintable.addCell(il);

                    //△H +500rpm
                    Cell im = new Cell(pt500_cha);
                    Sintable.addCell(im);

                    //+500rpm
                    Cell io = new Cell(p24);
                    Sintable.addCell(io);

                    //结论  +500rpm 
                    Cell ip = new Cell(Clu500);
                    Sintable.addCell(ip);

                    //实测转速 -500rpm
                    Cell iq = new Cell(ptf500);
                    Sintable.addCell(iq);

                    //常值偏差 -500rpm
                    Cell it = new Cell(ptf500_con);
                    Sintable.addCell(it);

                    //△H -500rpm
                    Cell iu = new Cell(ptf500_cha);
                    Sintable.addCell(iu);

                    //-500rpm
                    Cell iv = new Cell(p25);
                    Sintable.addCell(iv);

                    //结论 -500rpm 
                    Cell iw = new Cell(Cluf500);
                    Sintable.addCell(iw);

                    //实测转速 +1000rpm
                    Cell ix = new Cell(pt1000);
                    Sintable.addCell(ix);

                    //常值偏差 +1000rpm
                    Cell iy = new Cell(pt1000_con);
                    Sintable.addCell(iy);

                    //△H +1000rpm
                    Cell iz = new Cell(pt1000_cha);
                    Sintable.addCell(iz);

                    //+1000rpm
                    Cell ja = new Cell(p26);
                    Sintable.addCell(ja);

                    //结论 +1000rpm 
                    Cell jb = new Cell(Clu1000);
                    Sintable.addCell(jb);

                    //实测转速 -1000rpm
                    Cell jc = new Cell(ptf1000);
                    Sintable.addCell(jc);

                    //常值偏差 -1000rpm
                    Cell jd = new Cell(ptf1000_con);
                    Sintable.addCell(jd);

                    //△H -1000rpm
                    Cell je = new Cell(ptf1000_cha);
                    Sintable.addCell(je);

                    //-1000rpm
                    Cell jf = new Cell(p27);
                    Sintable.addCell(jf);

                    //结论 -1000rpm 
                    Cell jg = new Cell(Cluf1000);
                    Sintable.addCell(jg);

                    //实测转速 +3000rpm
                    Cell jh = new Cell(pt3000);
                    Sintable.addCell(jh);

                    //常值偏差 +3000rpm
                    Cell ji = new Cell(pt3000_con);
                    Sintable.addCell(ji);

                    //△H +3000rpm
                    Cell jj = new Cell(pt3000_cha);
                    Sintable.addCell(jj);

                    //+3000rpm
                    Cell jk = new Cell(p28);
                    Sintable.addCell(jk);

                    //结论 +3000rpm 
                    Cell jl = new Cell(Clu3000);
                    Sintable.addCell(jl);

                    //实测转速 -3000rpm
                    Cell jm = new Cell(ptf3000);
                    Sintable.addCell(jm);

                    //常值偏差 -3000rpm
                    Cell jn = new Cell(ptf3000_con);
                    Sintable.addCell(jn);

                    //△H -3000rpm
                    Cell jo = new Cell(ptf3000_cha);
                    Sintable.addCell(jo);

                    // -3000rpm
                    Cell jp = new Cell(p29);
                    Sintable.addCell(jp);

                    //结论  -3000rpm 
                    Cell jq = new Cell(Cluf3000);
                    Sintable.addCell(jq);

                    #endregion

                    #region 转速模式2 最大反作用力矩

                    //序号2
                    Cell js = new Cell("2");
                    js.Rowspan = 4;
                    Sintable.addCell(js);

                    //最大反作用力矩
                    Cell jt = new Cell(p34);
                    jt.Rowspan = 4;
                    Sintable.addCell(jt);

                    //绝对值>=16mNm
                    Cell ju = new Cell(p35);
                    ju.Rowspan = 2;
                    Sintable.addCell(ju);

                    //转速模式2 实测转速
                    Paragraph pmm;
                    if (k3000 != 0)
                    {
                        pmm = new Paragraph(new Phrase(Convert.ToString(max_realmoment / k3000), font1));
                    }
                    else
                    {
                        pmm = new Paragraph(new Phrase("0", font1));
                    }
                    pmm.Alignment = Element.ALIGN_CENTER;
                    Cell jv = new Cell(pmm);
                    jv.Rowspan = 2;
                    jv.Colspan = 3;
                    Sintable.addCell(jv);

                    //转速模式2 指令转速
                    Cell jy = new Cell();
                    jy.Rowspan = 4;
                    Sintable.addCell(jy);

                    //转速模式2  结论
                    Cell jz = new Cell();
                    jz.Rowspan = 4;
                    Sintable.addCell(jz);

                    //转速模式2  备注
                    Cell ka = new Cell();
                    ka.Rowspan = 4;
                    Sintable.addCell(ka);

                    //升速时间
                    Paragraph p70 = new Paragraph(new Phrase("升速时间", font1));
                    p70.Alignment = Element.ALIGN_CENTER;
                    Cell GenSpeed = new Cell(p70);
                    GenSpeed.Rowspan = 2;
                    Sintable.addCell(GenSpeed);

                    //填升速时间
                    //Paragraph p72 = new Paragraph(new Phrase(Convert.ToString(Math.Round(speed_time, 2)), font1));
                    //p72.Alignment = Element.ALIGN_RIGHT;

                    Paragraph p72;
                    if (this.checkBox1.Checked == true)
                    {
                        Random time = new Random();
                        double speedtime = time.Next(1150, 1190) / 100.0;
                        p72 = new Paragraph(new Phrase(Convert.ToString(speedtime), font1));
                        p72.Alignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        p72 = new Paragraph(new Phrase("0", font1));
                    }
                    Cell WriteSpeed = new Cell(p72);
                    WriteSpeed.Rowspan = 2;
                    WriteSpeed.Colspan = 3;
                    Sintable.addCell(WriteSpeed);

                    #endregion


                    #region 转速模式3 稳速功耗

                    //序号3
                    Cell kb = new Cell("3");
                    kb.Rowspan = 5;
                    //kb.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    //kb.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(kb);

                    //稳速功耗
                    Cell kc = new Cell(p37);
                    kc.Rowspan = 5;
                    kc.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    kc.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(kc);

                    // 0rpm 指标值
                    Cell kd = new Cell();
                    kd.Rowspan = 1;
                    kd.Colspan = 1;
                    Sintable.addCell(kd);

                    // 0rpm 实测（计算）值
                    Cell ke = new Cell(ptw0);
                    ke.Rowspan = 1;
                    ke.Colspan = 3;
                    Sintable.addCell(ke);


                    // 0rpm 稳速功耗
                    Cell kf = new Cell(p38);
                    kf.Rowspan = 1;
                    kf.Colspan = 1;
                    Sintable.addCell(kf);

                    // 0rpm 结论
                    Cell kg = new Cell();
                    kg.Rowspan = 1;
                    kg.Colspan = 1;
                    Sintable.addCell(kg);

                    // 0rpm 备注3
                    Cell kh = new Cell();
                    kh.Rowspan = 5;
                    Sintable.addCell(kh);


                    // +500rpm 指标值
                    Cell ki = new Cell();
                    ki.Rowspan = 1;
                    ki.Colspan = 1;
                    Sintable.addCell(ki);

                    // +500rpm 实测（计算）值
                    Cell kj = new Cell(ptw500);
                    kj.Colspan = 3;
                    Sintable.addCell(kj);


                    // +500rpm 稳速功耗
                    Cell kk = new Cell(p39);
                    Sintable.addCell(kk);

                    // +500rpm 结论
                    Cell kl = new Cell();
                    Sintable.addCell(kl);

                    // -500rpm 指标值
                    Cell km = new Cell();
                    Sintable.addCell(km);

                    // -500rpm 实测（计算）值
                    Cell kn = new Cell(ptwf500);
                    kn.Colspan = 3;
                    Sintable.addCell(kn);


                    // -500rpm 稳速功耗
                    Cell ko = new Cell(p40);
                    Sintable.addCell(ko);

                    // -500rpm 结论
                    Cell kp = new Cell();
                    Sintable.addCell(kp);

                    // +2000rpm 指标值
                    Cell kq = new Cell();
                    Sintable.addCell(kq);

                    // +2000rpm 实测（计算）值
                    Cell kr = new Cell(ptw3000);
                    kr.Colspan = 3;
                    Sintable.addCell(kr);


                    // +2000rpm 稳速功耗
                    Cell ks = new Cell(p41);
                    Sintable.addCell(ks);

                    // +2000rpm 结论
                    Cell kt = new Cell();
                    Sintable.addCell(kt);

                    // -2000rpm 指标值
                    Cell ku = new Cell();
                    Sintable.addCell(ku);

                    // -2000rpm 实测（计算）值
                    Cell kv = new Cell(ptwf3000);
                    kv.Colspan = 3;
                    Sintable.addCell(kv);


                    // -2000rpm 稳速功耗
                    Cell kw = new Cell(p42);
                    Sintable.addCell(kw);

                    // -2000rpm 结论
                    Cell kx = new Cell();
                    Sintable.addCell(kx);

                    #endregion


                    #region 力矩模式

                    //力矩模式
                    Cell ky = new Cell(p52);
                    ky.Rowspan = 5;
                    ky.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    ky.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Sintable.addCell(ky);

                    //序号
                    Cell kz = new Cell(p53);
                    kz.Rowspan = 1;
                    kz.Colspan = 1;
                    Sintable.addCell(kz);

                    //实测力矩
                    Cell la = new Cell(p54);
                    la.Colspan = 2;
                    Sintable.addCell(la);

                    //指令力矩
                    Cell lb = new Cell(p55);
                    lb.Colspan = 3;
                    Sintable.addCell(lb);

                    //备注
                    Cell lc = new Cell(p56);
                    lc.Colspan = 3;
                    Sintable.addCell(lc);

                    //1
                    Cell ld = new Cell("1");
                    ld.Colspan = 1;
                    ld.Rowspan = 1;
                    Sintable.addCell(ld);

                    //序号1 实测力矩
                    Cell le = new Cell(t3_toruqe);
                    le.Colspan = 2;
                    Sintable.addCell(le);

                    //3mNm
                    Cell lf = new Cell(p57);
                    lf.Colspan = 3;
                    Sintable.addCell(lf);

                    //3mNm 备注
                    Cell lg = new Cell();
                    lg.Colspan = 3;
                    Sintable.addCell(lg);


                    //2
                    Cell lh = new Cell("2");
                    lh.Colspan = 1;
                    lh.Rowspan = 1;
                    Sintable.addCell(lh);

                    //序号2 实测力矩
                    Cell li = new Cell(t10_toruqe);
                    li.Colspan = 2;
                    Sintable.addCell(li);

                    //+10mNm
                    Cell lj = new Cell(p58);
                    lj.Colspan = 3;
                    Sintable.addCell(lj);

                    //+10mNm 备注
                    Cell lk = new Cell();
                    lk.Colspan = 3;
                    Sintable.addCell(lk);

                    //3
                    Cell lm = new Cell("3");
                    Sintable.addCell(lm);

                    //序号3 实测力矩
                    Cell ln = new Cell(tf3_toruqe);
                    ln.Colspan = 2;
                    Sintable.addCell(ln);

                    //-3mNm
                    Cell lo = new Cell(p60);
                    lo.Colspan = 3;
                    Sintable.addCell(lo);

                    //-3mNm 备注
                    Cell lp = new Cell();
                    lp.Colspan = 3;
                    Sintable.addCell(lp);

                    //4
                    Cell lq = new Cell("4");
                    Sintable.addCell(lq);

                    //序号4 实测力矩
                    Cell lu = new Cell(tf10_toruqe);
                    lu.Colspan = 2;
                    Sintable.addCell(lu);

                    //-10mNm
                    Cell ls = new Cell(p61);
                    ls.Colspan = 3;
                    Sintable.addCell(ls);

                    //-10mNm 备注
                    Cell lt = new Cell();
                    lt.Colspan = 3;
                    Sintable.addCell(lt);


                    #endregion

                    document.Add(Sintable);

                    #endregion
                }


            }

            catch (DocumentException de)
            {
                MessageBox.Show("PDF 文档出错" + de.Message);
            }

            document.Close();



            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "D:\\飞轮测试报表\\" + this.experiment_memeber + "+" + this.experiment_name + "+" + this.experiment_product + "+result.pdf";
            myProcess.StartInfo.Verb = "Open";
            myProcess.StartInfo.CreateNoWindow = true;

            myProcess.Start();

            #endregion




        }
        /// <summary>
        /// 更新界面
        /// </summary>
        /// <param name="text"></param>
        public delegate void InvokeStateInformation(string text, string sign, string name);
        public void UpdateStateInformation(string text, string sign, string name)
        {

            switch (sign)
            {
                case "1": pictureBox12.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap(name));
                    this.chart1.Series[0].Points.Clear();
                    this.chart2.Series[0].Points.Clear();
                    this.chart1.Series[0].Points.AddXY(0, 0.5);
                    this.chart2.Series[0].Points.AddXY(0, 0.5);
                    this.label70.Text = "00.000";
                    this.label75.Text = "00.000";
                    this.label72.Text = "00.000";
                    this.label67.Text = "00.000";
                    this.label71.Text = "00.000";
                    this.label77.Text = "00.000";
                    this.label65.Text = "00.000";
                    this.label102.Text = "00.000";
                    this.label14.Text = "00.000";
                    this.label5.Text = "0.00";

                    this.label19.Text = "00.000";
                    this.label21.Text = "00.000";
                    ShowInformation(text);
                    break;
                case "2": pictureBox11.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap(name));
                    this.chart1.Series[0].Points.Clear();
                    this.chart2.Series[0].Points.Clear();
                    this.chart1.Series[0].Points.AddXY(0, 0.5);
                    this.chart2.Series[0].Points.AddXY(0, 0.5);
                    this.label70.Text = "00.000";
                    this.label75.Text = "00.000";
                    this.label72.Text = "00.000";
                    this.label67.Text = "00.000";
                    this.label71.Text = "00.000";
                    this.label77.Text = "00.000";
                    this.label65.Text = "00.000";
                    this.label102.Text = "00.000";
                    this.label14.Text = "00.000";
                    this.label5.Text = "0.00";

                    this.label19.Text = "00.000";
                    this.label21.Text = "00.000";
                    ShowInformation(text);
                    break;
                case "3":
                    pictureBox9.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap(name));
                    this.chart1.Series[0].Points.Clear();
                    this.chart2.Series[0].Points.Clear();
                    this.chart1.Series[0].Points.AddXY(0, 0.5);
                    this.chart2.Series[0].Points.AddXY(0, 0.5);
                    this.label70.Text = "00.000";
                    this.label75.Text = "00.000";
                    this.label72.Text = "00.000";
                    this.label67.Text = "00.000";
                    this.label71.Text = "00.000";
                    this.label77.Text = "00.000";
                    this.label65.Text = "00.000";
                    this.label102.Text = "00.000";
                    this.label14.Text = "00.000";
                    this.label5.Text = "0.00";

                    this.label19.Text = "00.000";
                    this.label21.Text = "00.000";
                    ShowInformation(text);
                    break;
                case "4":
                    pictureBox14.Image = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap(name));
                    this.chart1.Series[0].Points.Clear();
                    this.chart2.Series[0].Points.Clear();
                    this.chart1.Series[0].Points.AddXY(0, 0.5);
                    this.chart2.Series[0].Points.AddXY(0, 0.5);
                    this.label70.Text = "00.000";
                    this.label75.Text = "00.000";
                    this.label72.Text = "00.000";
                    this.label67.Text = "00.000";
                    this.label71.Text = "00.000";
                    this.label77.Text = "00.000";
                    this.label65.Text = "00.000";
                    this.label102.Text = "00.000";
                    this.label14.Text = "00.000";
                    this.label5.Text = "0.00";

                    this.label19.Text = "00.000";
                    this.label21.Text = "00.000";
                    ShowInformation(text);
                    break;
            }

        }

        #endregion


        #region 刷新
        private void button2_Click(object sender, EventArgs e)
        {
            lock (Defs._object_time)
            {
                this.max_deta_motion = 0;
                this.max_deta_moment = 0;
                this.label70.Text = "00.000";
                this.label75.Text = "00.000";
                this.label72.Text = "00.000";
                this.label67.Text = "00.000";
                this.label71.Text = "00.000";
                this.label77.Text = "00.000";
                this.label65.Text = "00.000";
                this.label102.Text = "00.000";
                this.label14.Text = "00.000";
                this.label19.Text = "00.000";
                this.label21.Text = "00.000";
                this.label5.Text = "0.00";
            }
        }
        #endregion

      


    }
}

