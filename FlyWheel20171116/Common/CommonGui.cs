using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

#region third party namespaces
using DocumentManager;
using DockingSuite;
using TD.SandBar;
#endregion

using Framework.WinGui.Interfaces;
using Framework.Core;
using Framework.WinGui.Menus;
using Framework.WinGui.Tools;


namespace Common
{
    /// <summary>
    /// 所有窗口的基类，是一个MdiChild的实例
    /// </summary>
    public class CommonGui : System.Windows.Forms.UserControl, Framework.WinGui.Interfaces.IMdiChild, Framework.WinGui.Interfaces.IMessage
    {
        #region 保护属性
        protected static DataSet m_dsstyle = null;

        /// <summary>
        /// 
        /// </summary>
        public DataSet StyleDataSet
        {
            get
            {
                return m_dsstyle;
            }
        }

        /// <summary>
        /// 业务数据
        /// </summary>
        protected DataSet _ds = null;


        /// <summary>
        /// 界面样式数据
        /// </summary>
        protected DataSet _dscode = null;

        /// <summary>
        /// 窗体标示
        /// </summary>
        protected string _id = string.Empty;

        /// <summary>
        /// 窗体显示标题
        /// </summary>
        protected string _caption = string.Empty;

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        protected bool _isinti = false;

        /// <summary>
        /// 主窗口
        /// </summary>
        protected IMdiContainer _main = null;

        #endregion

        #region 界面
        protected ImageList _toolImages = null;
        protected ImageList _menuImages = null;
        protected ImageList _treeImages = null;
        protected ImageList _gridImages = null;
        protected TD.SandBar.MenuBarItem[] _menuBars = null;
        protected TD.SandBar.ToolBar[] _toolBars = null;
        #endregion

        #region 实现 mdi 框架函数
        /// <summary>
        /// 控件标示
        /// </summary>
        public string ID
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// 窗体名称
        /// </summary>
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }
        /// <summary>
        /// 设置主窗体，IMdiContainer接口
        /// </summary>
        public IMdiContainer MainForm
        {
            get
            {
                return _main;
            }
            set
            {
                this._main = value;
                if (value == null)
                    this._main.RemoveChild(this);
                else
                    this._main.AddChild(this);
            }
        }
        /// <summary>
        /// 返回子窗口菜单项目数组
        /// </summary>
        public TD.SandBar.MenuBarItem[] Menus
        {
            get
            {
                return _menuBars;
            }
        }
        /// <summary>
        /// 返回工具条数组
        /// </summary>
        public TD.SandBar.ToolBar[] Tools
        {
            get
            {
                return this._toolBars;
            }
        }
        public void OnMidChildActive()
        {
            try
            {
                this.OnActive();
            }
            catch (Exception ex)
            {
                MessageBox.Show("严重错误:" + ex.ToString(), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        #endregion

        #region 实现 IMessage 函数
        public void ShowMessage(string msg)
        {

            if (this._main is IMessage)
                ((IMessage)_main).ShowMessage(msg);
            else
                MessageBox.Show(msg);
        }
        public void ShowMessage(string msg, System.Windows.Forms.MessageBoxIcon type)
        {
            if (this._main is IMessage)
                ((IMessage)_main).ShowMessage(msg, type);
            else
                MessageBox.Show(msg, this.Caption, System.Windows.Forms.MessageBoxButtons.OK, type);
        }
        #endregion

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// 
        /// </summary>
        public CommonGui()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        /// <param name="id"></param>
        public CommonGui(string id)
            : this()
        {
            _id = id;
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                this._ds = null;
                this._dscode = null;
                this._main = null;
            }
            base.Dispose(disposing);
        }


        #region 组件设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器 
        /// 修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CommonGui
            // 
            this.Name = "CommonGui";
            this.Size = new System.Drawing.Size(668, 414);
            this.ResumeLayout(false);

        }
        #endregion

        //base->访问基类的成员:base关键字用于在一个派生类中访问基类的成员：
        //在基类上调用一个以被其它方法重写了的方法、在建立派生类的一个实例的时候指定
        //哪一个基类构造器应该被调用。对基类的访问只允许出现在构造器、
        //实例方法或实例属性访问器中

        //this->调用一个方法的当前对象
        //调用一个方法的当前实例。静态成员函数中没有this指针。
        //this关键字只能用于构造器、实例方法和实例属性访问器中

        //C#中接口interface的说明
        //interface只提供方法规约 不提供方法主体
        //public interface Iperson
        //{void getName() //有方法 但是没有方法体}
        //interface中的方法不能用public abstract等修饰 无字段变量 无构造函数
        //interface的继承必须实现接口中的所有方法


        //C#中抽象类abstract的说明
        //抽象方法所在类必须为抽象类
        //抽象类不能直接实例化，必须由其派生类实现
        //抽象方法不包含方法主体，必须由派生类以override方式实现此方法

        //C#中虚方法virtual的说明
        //可在派生类中以override覆盖此方法
        //不覆盖也可由对象调用
        //无此标记的方法，重写时需要new隐藏原方法
        //virtual方法主要用于重写

        #region 初始化界面

        /// <summary>
        /// 初始化窗体，菜单，窗体等
        /// </summary>
        protected virtual bool Init() { return true; }

        /// <summary>
        /// 设置焦点
        /// </summary>
        public virtual void SetFocus(){}

        /// <summary>
        /// 初始化资源文件,包扩菜单和工具条的
        /// </summary>
        protected virtual void InitResource() { return; }

        /// <summary>
        /// 初始化菜单的快捷方式
        /// </summary>
        protected virtual void InitShortcutManager() { return; }

        /// <summary>
        /// 初始化主菜单栏
        /// </summary>
        protected virtual void InitMenuBar() { return; }

        /// <summary>
        /// 初始化工具栏
        /// </summary>
        protected virtual void InitToolBars() { return; }

        /// <summary>
        /// 初始化弹出菜单
        /// </summary>
        protected virtual void InitContextMenus() { return; }

        #endregion


        #region 在界面上处理信息


        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnActive()
        {
        }

        #endregion
    }
}
