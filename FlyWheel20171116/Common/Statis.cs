using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Framework.WinGui.Interfaces;

namespace Common
{
    public partial class StatisticalForm : Form
    {
        public static event StateMessageShow ShowInformation;

        private DataSet ds_Motor1 = null;
        private DataSet ds_Motor2 = null;
        private DataSet ds_Motor3 = null;
        private DataSet ds_Motor4 = null;
        private DataSet ds_Motor5 = null;
        private DataSet ds_Motor6 = null;
        private DataSet ds_Motor7 = null;
        private DataSet ds_Motor8 = null;

        public StatisticalForm()
        {
            InitializeComponent();
            this.comboBox1.SelectedItem = this.comboBox1.Items[0];

          

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

            #region 设置图表的属性
            //图表的背景色
            chart3.BackColor = System.Drawing.Color.FromArgb(211, 223, 240);
            //图表背景色的渐变方式
            chart3.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            chart3.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            chart3.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            chart3.BorderlineWidth = 2;
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
            chart2.ChartAreas[0].AxisY.Title = "电流";

            //设置图表区网格横纵线条的颜色和宽度
            chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;


            chart2.Series[0].Color = System.Drawing.Color.Red;
            #endregion

            #region 设置图表区属性
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss:ms";
            chart3.ChartAreas[0].AxisX.ScaleView.Size = 100;
            chart3.ChartAreas[0].AxisX.Minimum = 1;
            chart3.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart3.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //背景色
            chart3.ChartAreas[0].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            //背景渐变方式
            chart3.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            //渐变和阴影的辅助背景色
            chart3.ChartAreas[0].BackSecondaryColor = System.Drawing.Color.White;
            //边框颜色
            chart3.ChartAreas[0].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //阴影颜色
            chart3.ChartAreas[0].ShadowColor = System.Drawing.Color.Transparent;

            //设置X轴和Y轴线条的颜色和宽度
            chart3.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart3.ChartAreas[0].AxisX.LineWidth = 1;
            chart3.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart3.ChartAreas[0].AxisY.LineWidth = 1;

            //设置X轴和Y轴的标题
            chart3.ChartAreas[0].AxisY.Title = "转速";

            //设置图表区网格横纵线条的颜色和宽度
            chart3.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart3.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            chart3.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart3.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

            chart3.Series[0].Color = System.Drawing.Color.Red;
            #endregion

            this.chart2.Series[0].Points.AddXY(0, 0.5);
            this.chart3.Series[0].Points.AddXY(0, 0.5);


        }

        #region 显示
        /// <summary>
        /// 画图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string startime = this.dateTimePicker1.Value.ToString();
            string endtime = this.dateTimePicker2.Value.ToString();
            string sInfo = string.Empty;
            string scheme = "1";

            switch (this.comboBox1.SelectedItem.ToString())
            {
                case "恒速模式": scheme = "1";
                    break;
                case "斜坡模式": scheme = "2";
                    break;
                case "正弦模式": scheme = "3";
                    break;
                case "力矩模式": scheme = "4";
                    break;
                case "浪涌模式": scheme = "6";
                    break;
            }


            //Legend legend2 = new Legend();
            //legend2.Alignment = StringAlignment.Center;
            //legend2.Docking = Docking.Bottom;
            //chart2.Legends.Add(legend2);

            //Legend legend3 = new Legend();
            //legend3.Alignment = StringAlignment.Center;
            //legend3.Docking = Docking.Bottom;
            //chart3.Legends.Add(legend3);

            #region 获取电机1的数据
            if (this.checkBox2.Checked == true || this.checkBox3.Checked == true)
            {

                Facade.CommonFacade.FacTableStyle facade1 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor1 = facade1.GetDataFromDatabase("1"+scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor1 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
              
                if (this.checkBox2.Checked == true)
                {
                    for (int i = 0; i < ds_Motor1.Tables["dbo.Motor1"+scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor1.Tables["dbo.Motor1"+scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }


                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机1";
                }
                if (scheme != "6")
                {
                    if (this.checkBox3.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor1.Tables["dbo.Motor1" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor1.Tables["dbo.Motor1" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }

                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机1";

                    }
                }

            }
            #endregion

            #region 获取电机2的数据

            if (this.checkBox4.Checked == true || this.checkBox5.Checked == true )
            {
                Facade.CommonFacade.FacTableStyle facade2 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor2 = facade2.GetDataFromDatabase("2"+scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor2 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox4.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor2.Tables["dbo.Motor2" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor2.Tables["dbo.Motor2" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机2";


                    }
                }
               
                if (this.checkBox5.Checked == true)
                {
                    for (int i = 0; i < ds_Motor2.Tables["dbo.Motor2" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor2.Tables["dbo.Motor2" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机2";

                }
                
               

            }
            #endregion

            #region 获取电机3的数据

            if (this.checkBox8.Checked == true || this.checkBox7.Checked == true)
            {
                Facade.CommonFacade.FacTableStyle facade3 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor3 = facade3.GetDataFromDatabase("3" + scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor3 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox7.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor3.Tables["dbo.Motor3" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor3.Tables["dbo.Motor3" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机3";


                    }
                }
                
                if (this.checkBox8.Checked == true)
                {
                    for (int i = 0; i < ds_Motor3.Tables["dbo.Motor3" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor3.Tables["dbo.Motor3" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机3";
                }
               
               

            }
            #endregion

            #region 获取电机4的数据

            if (this.checkBox10.Checked == true || this.checkBox11.Checked == true )
            {
                Facade.CommonFacade.FacTableStyle facade4 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor4 = facade4.GetDataFromDatabase("4" + scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor4 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox10.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor4.Tables["dbo.Motor4" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor4.Tables["dbo.Motor4" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机4";


                    }
                }
                
                if (this.checkBox11.Checked == true)
                {

                    for (int i = 0; i < ds_Motor4.Tables["dbo.Motor4" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor4.Tables["dbo.Motor4" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机4";
                }
                
               

            }
            #endregion

            #region 获取电机5的数据

            if (this.checkBox13.Checked == true || this.checkBox14.Checked == true )
            {
                Facade.CommonFacade.FacTableStyle facade5 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor5 = facade5.GetDataFromDatabase("5" + scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor5 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox13.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor5.Tables["dbo.Motor5" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor5.Tables["dbo.Motor5" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机5";

                    }
                }
               
                if (this.checkBox14.Checked == true)
                {

                    for (int i = 0; i < ds_Motor5.Tables["dbo.Motor5" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor5.Tables["dbo.Motor5" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机5";
                }

               

            }
            #endregion

            #region 获取电机6的数据

            if (this.checkBox16.Checked == true || this.checkBox17.Checked == true)
            {
                Facade.CommonFacade.FacTableStyle facade6 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor6 = facade6.GetDataFromDatabase("6"+scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor6 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox16.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor6.Tables["dbo.Motor6" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor6.Tables["dbo.Motor6" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机6";

                    }
                }
                if (this.checkBox17.Checked == true)
                {
                    for (int i = 0; i < ds_Motor6.Tables["dbo.Motor6" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor6.Tables["dbo.Motor6" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机6";
                }

               

            }
            #endregion

            #region 获取电机7的数据

            if (this.checkBox19.Checked == true || this.checkBox20.Checked == true )
            {
                Facade.CommonFacade.FacTableStyle facade7 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor7 = facade7.GetDataFromDatabase("7", scheme, startime, endtime, out sInfo);
                if (ds_Motor7 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox19.Checked == true)
                    {

                        for (int i = 0; i < ds_Motor7.Tables["dbo.Motor7"].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor7.Tables["dbo.Motor7"].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }
                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机7";


                    }
                }
                if (this.checkBox20.Checked == true)
                {
                    for (int i = 0; i < ds_Motor7.Tables["dbo.Motor7"].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor7.Tables["dbo.Motor7"].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机7";
                }

               

            }
            #endregion

            #region 获取电机8的数据

            if (this.checkBox22.Checked == true || this.checkBox23.Checked == true)
            {
                Facade.CommonFacade.FacTableStyle facade8 = new Facade.CommonFacade.FacTableStyle();
                ds_Motor8 = facade8.GetDataFromDatabase("8" + scheme, scheme, startime, endtime, out sInfo);
                if (ds_Motor8 == null)
                {
                    ShowInformation(sInfo);
                    return;
                }
                if (scheme != "6")
                {
                    if (this.checkBox22.Checked == true)
                    {
                        for (int i = 0; i < ds_Motor8.Tables["dbo.Motor8" + scheme].Rows.Count; i = i + 1)
                        {
                            DataRow dw = ds_Motor8.Tables["dbo.Motor8" + scheme].Rows[i];
                            this.chart3.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorRevRea"]));
                        }

                        this.chart3.Series[0].Color = System.Drawing.Color.Blue;
                        this.chart3.Series[0].Name = "电机8";


                    }
                }
                if (this.checkBox23.Checked == true)
                {
                    for (int i = 0; i < ds_Motor8.Tables["dbo.Motor8" + scheme].Rows.Count; i = i + 1)
                    {
                        DataRow dw = ds_Motor8.Tables["dbo.Motor8" + scheme].Rows[i];
                        this.chart2.Series[0].Points.AddXY(dw["DateTime"], Convert.ToDouble(dw["RotorCut"]));
                    }
                    this.chart2.Series[0].Color = System.Drawing.Color.Blue;
                    this.chart2.Series[0].Name = "电机8";

                }

              

            }
            #endregion

        }
        #endregion


        #region PDF

        /// <summary>
        /// 生成PDF文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string localFilePath = String.Empty;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "pdf files(*.pdf)|*.pdf";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                localFilePath = saveFileDialog1.FileName.ToString();
                Document document = new Document(PageSize.A4);

                PdfWriter.getInstance(document, new System.IO.FileStream(localFilePath, System.IO.FileMode.Create));
                document.Open();

                BaseFont bfHei = BaseFont.createFont(Application.StartupPath + "\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font1 = new iTextSharp.text.Font(bfHei, 16);
                Paragraph p2 = new Paragraph(new Phrase("多电机测试报表", font1));
                p2.Alignment = Element.ALIGN_CENTER;
                document.Add(p2);

                iTextSharp.text.Font font2 = new iTextSharp.text.Font(bfHei, 10);
                
                Paragraph ptx4 = new Paragraph(new Phrase("1. 电流结果图", font2));
                ptx4.Alignment = Element.ALIGN_LEFT;
                document.Add(ptx4);
                Paragraph ptx5 = new Paragraph(new Phrase("       ", font2));
                document.Add(ptx5);
                Paragraph ptx6 = new Paragraph(new Phrase("以下图形显示电机电流测试结果，每个图形显示100个数据点，用折线连接，按时间顺序排列。", font2));
                ptx6.Alignment = Element.ALIGN_LEFT;
                document.Add(ptx6);
                int k2 = 1;
                for (int i = 1; i <= chart2.Series[0].Points.Count; i = i + 2 + (int)chart2.ChartAreas[0].AxisX.ScaleView.Size)
                {

                    chart2.ChartAreas[0].AxisX.ScaleView.Position = i;
                    chart2.SaveImage(Application.StartupPath + "\\Chart2TempFile" + i.ToString() + ".jpg", ChartImageFormat.Jpeg);
                    iTextSharp.text.Image image2 = iTextSharp.text.Image.getInstance(Application.StartupPath + "\\Chart2TempFile" + i.ToString() + ".jpg");
                    image2.scaleAbsoluteHeight(PageSize.A4.Height / 10);
                    image2.scaleAbsoluteWidth(PageSize.A4.Width - 80);


                    document.Add(image2);
                    string name = string.Format("图2.{0} 电流区间图", k2);
                    Paragraph pt = new Paragraph(new Phrase(name, font2));
                    pt.Alignment = Element.ALIGN_CENTER;
                    document.Add(pt);
                    k2 = k2 + 1;

                }
                chart2.ChartAreas[0].AxisX.ScaleView.Position = 1;

                document.newPage();



                Paragraph ptx7 = new Paragraph(new Phrase("1. 转速结果图", font2));
                ptx7.Alignment = Element.ALIGN_LEFT;
                document.Add(ptx7);
                Paragraph ptx8 = new Paragraph(new Phrase("       ", font2));
                document.Add(ptx8);
                Paragraph ptx9 = new Paragraph(new Phrase("以下图形显示电机转速测试结果，每个图形显示50个数据点，用折线连接，按时间顺序排列。", font2));
                ptx9.Alignment = Element.ALIGN_LEFT;
                document.Add(ptx9);
                int k3 = 1;
                for (int i = 1; i <= chart3.Series[0].Points.Count; i = i + 2 + (int)chart3.ChartAreas[0].AxisX.ScaleView.Size)
                {

                    chart3.ChartAreas[0].AxisX.ScaleView.Position = i;
                    chart3.SaveImage(Application.StartupPath + "\\Chart3TempFile" + i.ToString() + ".jpg", ChartImageFormat.Jpeg);
                    iTextSharp.text.Image image3 = iTextSharp.text.Image.getInstance(Application.StartupPath + "\\Chart3TempFile" + i.ToString() + ".jpg");
                    image3.scaleAbsoluteHeight(PageSize.A4.Height / 10);
                    image3.scaleAbsoluteWidth(PageSize.A4.Width - 80);


                    document.Add(image3);
                    string name = string.Format("图3.{0} 转速区间图", k3);
                    Paragraph pt = new Paragraph(new Phrase(name, font2));
                    pt.Alignment = Element.ALIGN_CENTER;
                    document.Add(pt);
                    k3 = k3 + 1;

                }
                chart3.ChartAreas[0].AxisX.ScaleView.Position = 1;

                document.Close();



                //删除临时文件
                for (int i = 1; i <= chart2.Series[0].Points.Count; i = i + 2 + (int)chart2.ChartAreas[0].AxisX.ScaleView.Size)
                {
                    System.IO.File.Delete(Application.StartupPath + "\\Chart2TempFile" + i.ToString() + ".jpg");
                }
                for (int i = 1; i <= chart3.Series[0].Points.Count; i = i + 2 + (int)chart3.ChartAreas[0].AxisX.ScaleView.Size)
                {
                    System.IO.File.Delete(Application.StartupPath + "\\Chart3TempFile" + i.ToString() + ".jpg");
                }

            }

        }
        #endregion


        #region 选择电机1
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;


        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }
        #endregion


        #region 选择电机2
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        #endregion


        #region 选择电机3
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        #endregion


        #region 选择电机4
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }
        #endregion


        #region 选择电机5
        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }
        #endregion


        #region 选择电机6
        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }
        #endregion


        #region 选择电机7
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox22.Checked = false;
            this.checkBox23.Checked = false;
        }

        #endregion


        #region 选择电机8
        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;

            this.checkBox4.Checked = false;
            this.checkBox5.Checked = false;

            this.checkBox7.Checked = false;
            this.checkBox8.Checked = false;

            this.checkBox10.Checked = false;
            this.checkBox11.Checked = false;

            this.checkBox13.Checked = false;
            this.checkBox14.Checked = false;

            this.checkBox16.Checked = false;
            this.checkBox17.Checked = false;

            this.checkBox19.Checked = false;
            this.checkBox20.Checked = false;
        }
        #endregion

       



    }
}
