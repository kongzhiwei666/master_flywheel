using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;


#region third party namespaces
using DocumentManager;
using DockingSuite;
#endregion


//using Framework.WinGui.Forms;
//using RssComponents;

namespace Framework.WinGui.Interfaces
{
    /// <summary>
    /// Form elements that can send commands have to implement ICommand
    /// </summary>
    public interface ICommand
    {
        string ID { get;}
        void Initialize();
        void Execute();
    }

    /// <summary>
    /// General GUI Command Abstraction (from Menubar, Toolbar, ...)
    /// </summary>
    public interface ICommandComponent
    {
        bool Checked { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
    }

    /// <summary>
    /// Delegate used to callback to mediator
    /// </summary>
    public delegate void ExecuteCommandHandler(ICommand sender);
    
    public delegate void StateMessageShow(string message);
    /// <summary>
    /// State of our tabbed view
    /// </summary>
    public interface ITabState
    {
        string Title { get; set; }
        string Url { get; set; }
        bool CanClose { get; set; }
        bool CanGoBack { get; set; }
        bool CanGoForward { get; set; }
    }


    public interface IMdiContainer
    {
        void AddChild(IMdiChild child);
        void RemoveChild(IMdiChild child);
        IMdiChild ActiveChild { get; set;}
        TD.SandBar.MenuBar MdiMainMenu { get;}
        TD.SandBar.SandBarManager MdiToolBarPanel { get;}
    }

    public interface IMdiChild
    {
        string Caption { get;set;}
        string ID { get;}
        IMdiContainer MainForm { get; set;}
        TD.SandBar.MenuBarItem[] Menus { get;}
        TD.SandBar.ToolBar[] Tools { get;}
        void OnMidChildActive();
    }


    /// <summary>
    /// 消息接口，实现消息的显示
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 显示一般的提示信息
        /// </summary>
        /// <param name="msg">消息内容</param>
        void ShowMessage(string msg);
        /// <summary>
        /// 实现特殊的提示消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="type">消息的类型</param>
        void ShowMessage(string msg, System.Windows.Forms.MessageBoxIcon type);
    }
}
