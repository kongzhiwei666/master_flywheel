using System;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Threading;

using Framework;
using Framework.WinGui.Utility;
using Framework.WinGui.Interfaces;

namespace Card.Core
{
    using Card.WinGui.Forms;
    /// <summary>
    /// FASApplication 的摘要说明。
    /// </summary>
    public class CardApplication : System.Windows.Forms.ApplicationContext
    {
        public const string applicationId = "Card";
        public const string applicationName = "飞轮测试平台";

        public static WinGuiMain guiMain;
        private static string appDataFolderPath;

        private CommandMediator cmdMediator = null;

        public CommandMediator Mediator
        {
            get { return this.cmdMediator; }
        }


        private static string ApplicationDataFolderFromEnv
        {
            get
            {
                if (StringHelper.EmptyOrNull(appDataFolderPath))
                {
                    appDataFolderPath = ConfigurationSettings.AppSettings["AppDataFolder"];
                }

                if (StringHelper.EmptyOrNull(appDataFolderPath))
                {
                    try
                    {	// once
                        appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CardApplication.Name);
                    }
                    catch
                    {
                        Application.Exit();
                    }
                }
                return appDataFolderPath;
            }
        }
        public static string Name { get { return applicationId; } }
        public static string GetUserPath()
        {
            string s = ApplicationDataFolderFromEnv;
            if (!Directory.Exists(s)) Directory.CreateDirectory(s);
            return s;
        }
        public static string GetShortcutSettingsFileName()
        {
            return Path.Combine(CardApplication.GetUserPath(), "shortcutsettings.xml");
        }


        static CardApplication()
        {
        }

        public CardApplication()
            : base()
        {

        }

        public void Init()
        {
            this.cmdMediator = new CommandMediator();
        }
        public void StartMainGui()
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(this.OnAppDomainUnload);
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            base.MainForm = guiMain = new WinGuiMain(this); // interconnect

            //enter_mainevent_loop:			
            try
            {

                Application.Run(this);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("严重错误:{0}", ex.Message), "系统提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Application.Exit();
            }
        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (guiMain != null && !guiMain.IsDisposed)
                guiMain.Close();
            guiMain = null;
        }

        private void OnAppDomainUnload(object sender, EventArgs e)
        {
            // forward to:
            OnApplicationExit(sender, e);
        }



        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>

        [STAThread]
        static void Main()
        {
            bool running = false;

            CardApplication appInstance = new CardApplication(); 

            if (!running)
            {
                appInstance.Init();
                appInstance.StartMainGui();
            }
        }


        #region 系统菜单和命令的调用

        public void CmdFormShow(ICommand sender)
        {
            guiMain.CmdFormShow(sender);
        }

        public void CmdExitApp(ICommand sender)
        {

            D2KDASK.D2K_Release_Card(0);
            Defs.Device1.acCanClose();
            Defs.Device2.acCanClose();
            Defs.brun1 = false;
            Defs.brun2 = false;
            Defs.Thread1.Abort();
            Defs.Thread2.Abort();
            Defs.Can1ReadStatus = false;
            Defs.Can2ReadStatus = false;
            if (guiMain != null && !guiMain.IsDisposed)
                guiMain.Close();

            guiMain = null;



            Application.Exit();
        }



        #endregion
    }


}
