using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Framework;


#region third party namespaces
using DocumentManager;
using DockingSuite;
using TD.SandBar;
#endregion

#region Framework namespaces
using Framework.WinGui.Utility;
using Framework.WinGui.Menus;
using Framework.WinGui.Tools;
using Framework.WinGui.Controls;
using Framework.Core;
using Framework.WinGui.Interfaces;
using Framework.WinGui.Utility.Keyboard;
#endregion

#region business namespaces
using Card.Core;
using Common;
using FlyWheel1;
using FlyWheel2;
using FlyWheel3;
using FlyWheel4;
using FlyWheel5;
using FlyWheel6;
#endregion


namespace Card.WinGui.Forms
{

    /// <summary>
    /// Form1 的摘要说明。
    /// </summary>
    /// 
    public class WinGuiMain : System.Windows.Forms.Form, Framework.WinGui.Interfaces.IMdiContainer
    {
        private TD.SandBar.MenuBar menuBarMain;
        private TD.SandBar.SandBarManager sandBarManager;
        private TD.SandBar.ToolBarContainer leftSandBarDock;
        private TD.SandBar.ToolBarContainer rightSandBarDock;
        private TD.SandBar.ToolBarContainer bottomSandBarDock;
        private TD.SandBar.ToolBarContainer topSandBarDock;
        public TD.SandBar.ToolBar toolBarMain;
        private System.Windows.Forms.StatusBar _status;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.StatusBarPanel _statusInfo;
        private System.ComponentModel.IContainer components;

        private string ReadTimeOutValue_can1 = string.Empty;
        private string ReadTimeOutValue_can2 = string.Empty;
        private string WriteTimeOutValue_can1 = string.Empty;
        private string WriteTimeOutValue_can2 = string.Empty;


        #region GUI main components:

        /// <summary>
        /// 工具条图片
        /// </summary>
        private ImageList _toolImages = null;

        /// <summary>
        /// 管理快捷方式
        /// </summary>
        private ShortcutManager _shortcuts;

        /// <summary>
        /// Mdi窗体管理
        /// </summary>
        private Card.WinGui.Utility.WindowManage _wm = null;
        private StatusBarPanel _statusTime;
        private Panel panelGround;
        private Panel panel1;
        private Label label2;
        private Label label1;

        /// <summary>
        /// 
        /// </summary>
        private CardApplication owner;

        #endregion

        #region 实现MdiContainer接口

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IMdiChild child)
        {
            _wm.Add(child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IMdiChild child)
        {
            _wm.Remove(child);
        }

        /// <summary>
        /// 
        /// </summary>
        public IMdiChild ActiveChild
        {
            get
            {
                return _wm.Active;
            }
            set
            {
                _wm.Active = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TD.SandBar.MenuBar MdiMainMenu
        {
            get
            {
                return this.menuBarMain;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TD.SandBar.SandBarManager MdiToolBarPanel
        {
            get
            {
                return this.sandBarManager;
            }
        }
        #endregion

        #region 实现IMessage接口

        /// <summary>
        /// 用消息对话框显示消息
        /// </summary>
        /// <param name="msg">用于显示在对话框中的消息</param>
        public void ShowMessage(string msg)
        {
            this.ShowMessage(msg, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 用消息对话框显示消息
        /// </summary>
        /// <param name="msg">用于显示在对话框中的消息</param>
        /// <param name="type">MessageBoxIcon枚举类型的参数</param>
        public void ShowMessage(string msg, MessageBoxIcon type)
        {
            switch (type)
            {
                case System.Windows.Forms.MessageBoxIcon.Information:
                    this._statusInfo.Text = msg;
                    break;
                case System.Windows.Forms.MessageBoxIcon.Error:
                    MessageBox.Show(msg);
                    break;
                default:
                    MessageBox.Show(msg);
                    break;
            }

        }

        #endregion

        /// <summary>
        /// 窗口属性
        /// </summary>
        public CardApplication GuiOwner
        {
            get { return owner; }
            set { owner = value; }
        }

        public WinGuiMain(CardApplication theGuiOwner)
        {
            InitializeComponent();
            GuiOwner = theGuiOwner;
            Init();


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
            this.components = new System.ComponentModel.Container();
            this.sandBarManager = new TD.SandBar.SandBarManager();
            this.bottomSandBarDock = new TD.SandBar.ToolBarContainer();
            this.leftSandBarDock = new TD.SandBar.ToolBarContainer();
            this.rightSandBarDock = new TD.SandBar.ToolBarContainer();
            this.topSandBarDock = new TD.SandBar.ToolBarContainer();
            this.toolBarMain = new TD.SandBar.ToolBar();
            this._status = new System.Windows.Forms.StatusBar();
            this._statusInfo = new System.Windows.Forms.StatusBarPanel();
            this._statusTime = new System.Windows.Forms.StatusBarPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.panelGround = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.topSandBarDock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._statusInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._statusTime)).BeginInit();
            this.panelGround.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sandBarManager
            // 
            this.sandBarManager.BottomContainer = this.bottomSandBarDock;
            this.sandBarManager.LeftContainer = this.leftSandBarDock;
            this.sandBarManager.OwnerForm = this;
            this.sandBarManager.RightContainer = this.rightSandBarDock;
            this.sandBarManager.TopContainer = this.topSandBarDock;
            // 
            // bottomSandBarDock
            // 
            this.bottomSandBarDock.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomSandBarDock.Location = new System.Drawing.Point(0, 750);
            this.bottomSandBarDock.Manager = this.sandBarManager;
            this.bottomSandBarDock.Name = "bottomSandBarDock";
            this.bottomSandBarDock.Size = new System.Drawing.Size(1260, 0);
            this.bottomSandBarDock.TabIndex = 2;
            // 
            // leftSandBarDock
            // 
            this.leftSandBarDock.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftSandBarDock.Location = new System.Drawing.Point(0, 23);
            this.leftSandBarDock.Manager = this.sandBarManager;
            this.leftSandBarDock.Name = "leftSandBarDock";
            this.leftSandBarDock.Size = new System.Drawing.Size(0, 727);
            this.leftSandBarDock.TabIndex = 0;
            // 
            // rightSandBarDock
            // 
            this.rightSandBarDock.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightSandBarDock.Location = new System.Drawing.Point(1260, 23);
            this.rightSandBarDock.Manager = this.sandBarManager;
            this.rightSandBarDock.Name = "rightSandBarDock";
            this.rightSandBarDock.Size = new System.Drawing.Size(0, 727);
            this.rightSandBarDock.TabIndex = 1;
            // 
            // topSandBarDock
            // 
            this.topSandBarDock.Controls.Add(this.toolBarMain);
            this.topSandBarDock.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSandBarDock.Location = new System.Drawing.Point(0, 0);
            this.topSandBarDock.Manager = this.sandBarManager;
            this.topSandBarDock.Name = "topSandBarDock";
            this.topSandBarDock.Size = new System.Drawing.Size(1260, 23);
            this.topSandBarDock.TabIndex = 3;
            // 
            // toolBarMain
            // 
            this.toolBarMain.DockLine = 1;
            this.toolBarMain.Guid = new System.Guid("46d02df3-0068-4bb3-a433-298ceabd4490");
            this.toolBarMain.Location = new System.Drawing.Point(2, 0);
            this.toolBarMain.Name = "toolBarMain";
            this.toolBarMain.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolBarMain.Size = new System.Drawing.Size(25, 23);
            this.toolBarMain.TabIndex = 1;
            this.toolBarMain.Text = "toolBar1";
            // 
            // _status
            // 
            this._status.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._status.Location = new System.Drawing.Point(0, 727);
            this._status.Name = "_status";
            this._status.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this._statusInfo,
            this._statusTime});
            this._status.ShowPanels = true;
            this._status.Size = new System.Drawing.Size(1260, 23);
            this._status.TabIndex = 4;
            // 
            // _statusInfo
            // 
            this._statusInfo.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this._statusInfo.Name = "_statusInfo";
            this._statusInfo.Text = "_statusInfo";
            this._statusInfo.Width = 621;
            // 
            // _statusTime
            // 
            this._statusTime.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this._statusTime.Name = "_statusTime";
            this._statusTime.Text = "_statusTime";
            this._statusTime.Width = 621;
            // 
            // panelGround
            // 
            this.panelGround.AutoScroll = true;
            this.panelGround.Controls.Add(this.panel1);
            this.panelGround.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGround.Location = new System.Drawing.Point(0, 23);
            this.panelGround.Name = "panelGround";
            this.panelGround.Size = new System.Drawing.Size(1260, 704);
            this.panelGround.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 704);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Image = global::Card.Properties.Resources.FlyWheelTest1;
            this.label1.Location = new System.Drawing.Point(16, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1232, 540);
            this.label1.TabIndex = 2;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(180, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(895, 35);
            this.label2.TabIndex = 1;
            this.label2.Text = "飞轮测试平台";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // WinGuiMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(1260, 750);
            this.Controls.Add(this.panelGround);
            this.Controls.Add(this._status);
            this.Controls.Add(this.leftSandBarDock);
            this.Controls.Add(this.rightSandBarDock);
            this.Controls.Add(this.bottomSandBarDock);
            this.Controls.Add(this.topSandBarDock);
            this.Name = "WinGuiMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinGuiMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.topSandBarDock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._statusInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._statusTime)).EndInit();
            this.panelGround.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        protected void Init()
        {
            InitResources();
            InitShortcutManager();
            //InitMenuBar();
            InitToolBars();
            InitStatusBar();
            InitContextMenus();
            InitTrayIcon();
            this._wm = new WinGui.Utility.WindowManage(this, this.panelGround);
            this.Text = CardApplication.applicationName;


            #region 数据采集卡初始化
            short m_dev = D2KDASK.D2K_Register_Card(D2KDASK.DAQ_2205, 0);
            if (m_dev < 0)
            {
                _statusInfo.Text = "Select Device First!";
                MessageBox.Show("Select Device First!");

            }
            else
            {
                short ret = D2KDASK.D2K_DIO_PortConfig(0, D2KDASK.Channel_P1A, D2KDASK.OUTPUT_PORT);
                if (ret < 0)
                {
                    _statusInfo.Text = "D2K_DIO_PortConfig error!";
                    MessageBox.Show("D2K_DIO_PortConfig error!");
                    D2KDASK.D2K_Release_Card(0);

                }

                D2KDASK.D2K_DO_WritePort(0, D2KDASK.Channel_P1A, Convert.ToUInt32(Defs.StateVector, 2));
            }
            #endregion

            string Commu = System.Configuration.ConfigurationSettings.AppSettings["CommunicationString1"];
            string[] para = Commu.Split(';');


            string[] com = para[0].Split('=');
            string can1 = com[1].Trim();


            string[] baud = para[1].Split('=');
            string baud1 = baud[1].Trim();

            com = para[2].Split('=');
            ReadTimeOutValue_can1 = com[1].Trim();
            com = para[3].Split('=');
            WriteTimeOutValue_can1 = com[1].Trim();



            Commu = System.Configuration.ConfigurationSettings.AppSettings["CommunicationString2"];
            para = Commu.Split(';');


            com = para[0].Split('=');
            string can2 = com[1].Trim();


            baud = para[1].Split('=');
            string baud2 = baud[1].Trim();


            com = para[2].Split('=');
            ReadTimeOutValue_can2 = com[1].Trim();


            com = para[3].Split('=');
            WriteTimeOutValue_can2 = com[1].Trim();

            #region can 总线配置
            bool syncflag = false;
            uint nWriteCount = 10;
            uint nReadCount = 10;
            
            int nRet1 = Defs.Device1.acCanOpen(can1, syncflag, nReadCount, nWriteCount);
            int nRet2 = Defs.Device2.acCanOpen(can2, syncflag, nReadCount, nWriteCount);

            if (nRet1 < 0 && nRet2 < 0)
            {
                _statusInfo.Text = "Failed to open the CAN1 and CAN2 port, please check the CAN1 and CAN2 port name!";
                MessageBox.Show("Failed to open the CAN1 and CAN2 port, please check the CAN1 and CAN2 port name!");
            }
            #endregion


            int nRet;

            #region can1 配置
            if (nRet1 >= 0)
            {
                nRet = Defs.Device1.acEnterResetMode();                                                                           
                if (nRet < 0)
                {
                    _statusInfo.Text = "Failed to stop opertion1!";
                    MessageBox.Show("Failed to stop opertion1!");
                    Defs.Device1.acCanClose();
                }
                else
                {
                    nRet = Defs.Device1.acSetBaud(Convert.ToUInt32(baud1));                                                                
                    if (nRet < 0)
                    {
                        _statusInfo.Text = "Failed to set baud 1!";
                        MessageBox.Show("Failed to set baud 1!");
                        Defs.Device1.acCanClose();
                    }
                    else
                    {
                        nRet = Defs.Device1.acSetTimeOut(Convert.ToUInt32(ReadTimeOutValue_can1), Convert.ToUInt32(WriteTimeOutValue_can1));                                       
                        if (nRet < 0)
                        {
                            _statusInfo.Text = "Failed to set Timeout 1!";
                            MessageBox.Show("Failed to set Timeout 1!");
                            Defs.Device1.acCanClose();
                        }
                        else
                        {
                            nRet = Defs.Device1.acEnterWorkMode();                                                                       
                            if (nRet < 0)
                            {
                                _statusInfo.Text = "Failed to restart operation!";
                                MessageBox.Show("Failed to restart operation!");
                                Defs.Device1.acCanClose();
                            }
                        }
                    }
                }
            }
            #endregion

            #region can2 配置
            if (nRet2 >= 0)
            {

                nRet = Defs.Device2.acEnterResetMode();                                                                             
                if (nRet < 0)
                {
                    _statusInfo.Text = "Failed to stop opertion2!";
                    MessageBox.Show("Failed to stop opertion2!");
                    Defs.Device2.acCanClose();
                }
                else
                {
                    nRet = Defs.Device2.acSetBaud(Convert.ToUInt32(baud2));                                                                 
                    if (nRet < 0)
                    {
                        _statusInfo.Text = "Failed to set baud 2!";
                        MessageBox.Show("Failed to set baud 2!");
                        Defs.Device2.acCanClose();
                    }
                    else
                    {
                        nRet = Defs.Device2.acSetTimeOut(Convert.ToUInt32(ReadTimeOutValue_can2), Convert.ToUInt32(WriteTimeOutValue_can2));                                        
                        if (nRet < 0)
                        {
                            _statusInfo.Text = "Failed to set Timeout 2!";
                            MessageBox.Show("Failed to set Timeout 2!");
                            Defs.Device2.acCanClose();
                        }
                        else
                        {
                            nRet = Defs.Device2.acEnterWorkMode();                                                                        
                            if (nRet < 0)
                            {
                                _statusInfo.Text = "Failed to restart operation!";
                                MessageBox.Show("Failed to restart operation!");
                                Defs.Device2.acCanClose();
                            }
                        }
                    }
                }

            }
            #endregion

           
        }


        /// <summary>
        /// 初始化事件响应处理函数和主窗口变量
        /// </summary>
        protected void InitResources()
        {
            FlyWheel1.FlyWheel1.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel2.FlyWheel2.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel3.FlyWheel3.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel4.FlyWheel4.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel5.FlyWheel5.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel6.FlyWheel6.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel7.FlyWheel7.ShowInformation += new StateMessageShow(SimulatorMessageShow);
            FlyWheel8.FlyWheel8.ShowInformation += new StateMessageShow(SimulatorMessageShow);
           
        }

        /// <summary>
        /// 初始化Shortcut
        /// </summary>
        protected void InitShortcutManager()
        {
            _shortcuts = new ShortcutManager();
            string settingsPath = CardApplication.GetShortcutSettingsFileName();
            if (File.Exists(settingsPath))
            {
                _shortcuts.LoadSettings(settingsPath);
            }
            else
            {
                using (Stream settingsStream = Framework.Core.Resource.Manager.GetStream("ShortcutSettings.xml"))
                {
                    _shortcuts.LoadSettings(settingsStream);
                }
            }
        }


        #region MenuBar init
        /// <summary>
        /// Creat the FirstMenu
        /// </summary>
        private void InitMenuBar()
        {
            // Init the MainMenuControl
            menuBarMain.SuspendLayout();
            menuBarMain.ImageList = _toolImages;
            // Create the top level Menu
            MenuBarItem top0 = new MenuBarItem(Framework.Core.Resource.Manager["RES_MainMenuFileCaption"]);
            top0.ToolTipText = Framework.Core.Resource.Manager["RES_MainMenuFileDesc"];

            MenuBarItem top1 = new MenuBarItem(Framework.Core.Resource.Manager["RES_MainMenuCustomCardCaption"]);
            top1.ToolTipText = Framework.Core.Resource.Manager["RES_MainMenuCustomCardDesc"];

            MenuBarItem top2 = new MenuBarItem(Framework.Core.Resource.Manager["RES_MainMenuTollCardCaption"]);
            top2.ToolTipText = Framework.Core.Resource.Manager["RES_MainMenuToolCardDesc"];


            MenuBarItem top4 = new MenuBarItem(Framework.Core.Resource.Manager["RES_MainMenuHelpCaption"]);
            top4.ToolTipText = Framework.Core.Resource.Manager["RES_MainMenuHelpDesc"];			// Create the submenus

            CreateFileMenu(top0);
            CreateCustomCardMenu(top1);
            CreateTollCardMenu(top2);
            CreateHelpMenu(top4);
            menuBarMain.Buttons.AddRange(new MenuItemBase[] { top0, top1, top2, top4 });

            menuBarMain.ResumeLayout(false);
        }


        #region 增加第一层菜单
        protected void CreateFileMenu(MenuBarItem mc)
        {
            
            AppMenuCommand stype1 = new AppMenuCommand("cmdExit",
                owner.Mediator, new ExecuteCommandHandler(owner.CmdExitApp),
                "RES_MenuExitCaption", "RES_MenuExitDesc", 3, _shortcuts);
            stype1.BeginGroup = true;

            mc.MenuItems.AddRange(new MenuButtonItem[] { stype1 });
        }

        protected void CreateCustomCardMenu(MenuBarItem mc)
        {
            AppMenuCommand stype1 = new AppMenuCommand("cmdMakeCustomCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeCustomCardCaption", "RES_MenuMakeCustomCardDesc", 0, _shortcuts);

            AppMenuCommand stype2 = new AppMenuCommand("cmdFillMoneyCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuFillMoneyCardCaption", "RES_MenuFillMoneyCardDesc", 1, _shortcuts);

            AppMenuCommand stype3 = new AppMenuCommand("cmdReplaceCustomCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuReplaceCustomCardCaption", "RES_MenuReplaceCustomCardDesc", 5, _shortcuts);

            mc.MenuItems.AddRange(new MenuButtonItem[] { stype1, stype2, stype3 });
        }

        protected void CreateTollCardMenu(MenuBarItem mc)
        {
            AppMenuCommand stype1 = new AppMenuCommand("cmdMakeParameterCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeParameterCardCaption", "RES_MenuMakeParameterCardDesc", 6, _shortcuts);


            AppMenuCommand stype2 = new AppMenuCommand("cmdMakeTimeCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeTimeCardCaption", "RES_MenuMakeTimeCardDesc", 7, _shortcuts);

            AppMenuCommand stype3 = new AppMenuCommand("cmdMakeGasCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeGasCardCaption", "RES_MenuMakeGasCardDesc", 13, _shortcuts);

            AppMenuCommand stype4 = new AppMenuCommand("cmdMakeDataCard",
               owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
               "RES_MenuMakeDataCardCaption", "RES_MenuMakeDataCardDesc", 14, _shortcuts);

            AppMenuCommand stype5 = new AppMenuCommand("cmdMakeCheckCard",
               owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
               "RES_MenuMakeCheckCardCaption", "RES_MenuMakeCheckCardDesc", 15, _shortcuts);


            mc.MenuItems.AddRange(new MenuButtonItem[] { stype1, stype2, stype3,stype4,stype5 });
        }

        protected void CreateHelpMenu(MenuBarItem mc)
        {
            AppMenuCommand stype2 = new AppMenuCommand("cmdAbout",
                owner.Mediator, new ExecuteCommandHandler(this.CmdAboutApp),
                "RES_MenuAboutCaption", "RES_MenuAboutDesc", 9, _shortcuts);
            AppMenuCommand stype3 = new AppMenuCommand("cmdRefresh",
                owner.Mediator, new ExecuteCommandHandler(this.CmdRefresh),
                "RES_MenuRefreshCaption", "RES_MenuRefreshDesc", 10, _shortcuts);




            mc.MenuItems.AddRange(new MenuButtonItem[] { stype2, stype3 });
        }

        protected void InitContextMenus()
        {

        }

        #endregion

        #endregion

        #region ToolBars 工具条
        private void InitToolBars()
        {

            toolBarMain.SuspendLayout();

            toolBarMain.ImageList = _toolImages;

            CreateMainToolbar(toolBarMain);

            toolBarMain.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(OnAnyToolBarButtonClick);
            toolBarMain.DockLine = 1;
            toolBarMain.Visible = true;
            toolBarMain.Dock = DockStyle.Left;


          
            toolBarMain.ResumeLayout(false);


        }

        private void CreateMainToolbar(TD.SandBar.ToolBar tb)
        {
            AppToolCommand tool1 = new AppToolCommand("cmdExit",
                owner.Mediator, new ExecuteCommandHandler(owner.CmdExitApp),
                "RES_MenuExitCaption", "RES_MenuExitDesc", 0);
            Bitmap il1 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("Exit.bmp"));
            tool1.Image = il1;
            tool1.ToolTipText = "退出";



            AppToolCommand tool2 = new AppToolCommand("cmdMakeCustomCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeCustomCardCaption1", "RES_MenuMakeCustomCardDesc1", 0);

            Bitmap il2 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel1.bmp"));
            tool2.Image = il2;
            tool2.ToolTipText = "飞轮1";




            tool2.BeginGroup = true;

            AppToolCommand tool3 = new AppToolCommand("cmdFillMoneyCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuFillMoneyCardCaption1", "RES_MenuFillMoneyCardDesc1", 1);

            Bitmap il3 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel2.bmp"));
            tool3.Image = il3;
            tool3.ToolTipText = "飞轮2";

            AppToolCommand tool4 = new AppToolCommand("cmdReplaceCustomCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuReplaceCustomCardCaption1", "RES_MenuReplaceCustomCardDesc1", 5);

            Bitmap il4 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel3.bmp"));
            tool4.Image = il4;
            tool4.ToolTipText = "飞轮3";


            AppToolCommand tool5 = new AppToolCommand("cmdMakeParameterCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeParameterCardCaption1", "RES_MenuMakeParameterCardDesc1", 6);

            Bitmap il5 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel4.bmp"));
            tool5.Image = il5;
            tool5.ToolTipText = "飞轮4";

            AppToolCommand tool6 = new AppToolCommand("cmdMakeTimeCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeTimeCardCaption1", "RES_MenuMakeTimeCardDesc1", 7);

            Bitmap il6 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel5.bmp"));
            tool6.Image = il6;
            tool6.ToolTipText = "飞轮5";

            tool6.BeginGroup = true;

            AppToolCommand tool7 = new AppToolCommand("cmdMakeGasCard",
                owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
                "RES_MenuMakeGasCardCaption1", "RES_MenuMakeGasCardDesc1", 13);

            Bitmap il7 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel6.bmp"));
            tool7.Image = il7;
            tool7.ToolTipText = "飞轮6";

            AppToolCommand tool8 = new AppToolCommand("cmdFlyWheel7",
               owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
               "RES_MenuMakeTimeCardCaption", "RES_MenuMakeTimeCardDesc", 13);

            Bitmap il8 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel7.bmp"));
            tool8.Image = il8;
            tool8.ToolTipText = "飞轮7";


            AppToolCommand tool9 = new AppToolCommand("cmdFlyWheel8",
              owner.Mediator, new ExecuteCommandHandler(this.CmdLoadChild),
              "RES_MenuMakeGasCardCaption", "RES_MenuMakeGasCardDesc", 13);

            Bitmap il9 = new Bitmap(Framework.Core.Resource.Manager.LoadBitmap("FlyWheel8.bmp"));
            tool9.Image = il9;
            tool9.ToolTipText = "飞轮8";


            tb.Buttons.AddRange(new ToolbarItemBase[] { tool1, tool2, tool3, tool4, tool5, tool6, tool7, tool8, tool9 });
            


        }

        #endregion

        #region StatusBar init 状态条
        private void InitStatusBar()
        {
            this._statusInfo.Text = string.Format("  准备  ");
            this._statusTime.Text = string.Format("  杭州电子科技大学  自动化学院  ");
        }
        #endregion

        #region TrayIcon Init
        private void InitTrayIcon()
        {
        }
        #endregion

        #region CmdImplement
        /// <summary>
        /// 菜单关于事件
        /// </summary>
        /// <param name="sender"></param>
        public void CmdAboutApp(ICommand sender)
        {
            AboutDialog dialog = new AboutDialog();
            string str = @"测试平台由杭州电子科技大学自动化学院研制";
            dialog.AboutText.Text = str;
            dialog.ShowDialog(this);
        }
        public void CmdRefresh(ICommand sender)
        {
           
        }


        public void CmdLoadChild(ICommand sender)
        {
            this.ShowMessage("当前状态：  .....加载飞轮测试板卡.....");

            Common.CommonGui  gui = null;
            if (this._wm.Contains(sender.ID))
            {
                this.ActiveChild = _wm[sender.ID];
                return;
            }

            switch (sender.ID)
            {
                case "cmdMakeCustomCard":
                    gui = new FlyWheel1.FlyWheel1(sender.ID, "");
                    gui.Caption = "飞轮1状态";
                    this.ShowMessage("当前状态：  .....飞轮1控制面板加载结束.....");
                    break;
                case "cmdFillMoneyCard":
                    gui = new FlyWheel2.FlyWheel2(sender.ID, "");
                    gui.Caption = "飞轮2状态";
                    this.ShowMessage("当前状态：  .....飞轮2控制面板加载结束.....");
                    break;
                case "cmdReplaceCustomCard":
                    gui = new FlyWheel3.FlyWheel3(sender.ID, "");
                    gui.Caption = "飞轮3状态";
                    this.ShowMessage("当前状态：  .....飞轮3控制面板加载结束.....");
                    break;
                case "cmdMakeParameterCard":
                    gui = new FlyWheel4.FlyWheel4(sender.ID, "");
                    gui.Caption = "飞轮4状态";
                    this.ShowMessage("当前状态：  .....飞轮4控制面板加载结束.....");
                    break;
                case "cmdMakeTimeCard":
                    gui = new FlyWheel5.FlyWheel5(sender.ID, "");
                    gui.Caption = "飞轮5状态";
                    this.ShowMessage("当前状态：  .....飞轮5控制面板加载结束.....");
                    break;
                case "cmdMakeGasCard":
                    gui = new FlyWheel6.FlyWheel6(sender.ID, "");
                    gui.Caption = "飞轮6状态";
                    this.ShowMessage("当前状态：  .....飞轮6控制面板加载结束.....");
                    break;
                case "cmdFlyWheel7":
                    gui = new FlyWheel7.FlyWheel7(sender.ID, "");
                    gui.Caption = "飞轮7状态";
                    this.ShowMessage("当前状态：  .....飞轮7控制面板加载结束.....");
                    break;

                case "cmdFlyWheel8":
                    gui = new FlyWheel8.FlyWheel8(sender.ID, "");
                    gui.Caption = "飞轮8状态";
                    this.ShowMessage("当前状态：  .....飞轮8控制面板加载结束.....");
                    break;
            }
           
            if (gui != null)
            {
                gui.Dock = DockStyle.Fill;
                gui.Visible = true;
                this.ActiveChild = gui;
                gui.SetFocus();
                
                this._wm.Add(gui);

            }
            
            this.panel1.Visible = false;
            
        }
        public void SimulatorMessageShow(string msg)
        {
            this.ShowMessage(string.Format("当前状态：  .....{0}....." , msg));

        }
        public void CmdFormShow(ICommand sender)
        {
            Common.CommonGui gui = null;
            //发现是否已经打开
            if (this._wm.Contains(sender.ID))
            {
                this.ActiveChild = _wm[sender.ID];
                return;
            }



        }
        public void CmdCloseMidChild(ICommand sender)
        {
            if (this._wm.Active != null)
            {
                this._wm.Remove(this._wm.Active);
                return;
            }
        }

        /// <summary>
        /// 菜单帮助事件
        /// </summary>
        /// <param name="sender"></param>
        public void CmdHelpDoc(ICommand sender)
        {
            //设置帮助文件
            string shelp = System.Configuration.ConfigurationSettings.AppSettings["Help"];
            shelp = Path.Combine(Application.StartupPath, shelp);
            this.helpProvider1.HelpNamespace = shelp;
            System.Windows.Forms.Help.ShowHelp(this, this.helpProvider1.HelpNamespace);
        }
        #endregion

        #region Command ID  全局变量的定义
        /// <summary>
        /// 全局变量的定义
        /// </summary>

        public const string CMD_SIMULATION = "cmdSimulation";  //移动机械臂仿真环境






        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnyToolBarButtonClick(object sender, ToolBarItemEventArgs e)
        {
            ICommand cmd = e.Item as ICommand;
            if (cmd != null)
            {
                cmd.Execute();
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 1, 0);
            }
        }

        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; // 鼠标左键按下  
        public const int MOUSEEVENTF_LEFTUP = 0x0004; // 鼠标左键抬起  


        [System.Runtime.InteropServices.DllImport("user32", SetLastError = true)]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo); 

        #region #Test
        private void Test(TD.SandBar.ToolBar toolClient)
        {
            // 
            // toolBarMain
            // 位置要改变,待定
            this.topSandBarDock.Controls.Add(toolClient);
            toolClient.DockLine = 1;
            toolClient.Guid = new System.Guid("46d02df3-0068-4bb3-a433-298ceabd4491");
            toolClient.Location = new System.Drawing.Point(2, 23);
            toolClient.Name = "toolClient";
            toolClient.Size = new System.Drawing.Size(2, 47);
            toolClient.TabIndex = 10;
            toolClient.Text = "toolBar2";

            toolClient.SuspendLayout();

            toolClient.ImageList = _toolImages;

            CreateMainToolbar(toolClient);

            toolClient.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(OnAnyToolClientButtonClick);

            toolClient.ResumeLayout(false);
        }
        private void OnAnyToolClientButtonClick(object sender, ToolBarItemEventArgs e)
        {
        }
        #endregion #Test

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            owner.CmdExitApp(null);
        }
    }
}
