using System;
using System.Collections;
using System.Windows.Forms;

#region Card namespace
using Common;
using Card.Core;
#endregion

using Framework.WinGui.Menus;
using Framework.WinGui.Interfaces;
using Framework.WinGui.Tools;
using Card.WinGui.Forms;

namespace Card.WinGui.Utility
{
    /// <summary>
    /// 主要用于模拟MDI的效果,在这里CommomGui（实现了接口IMDiChild）或
    /// CommonGui的子类相当于子窗体,此类维护的是IMDiChild
    /// </summary>
    public class WindowManage
    {
        #region 声明或定义变量

        /// <summary>
        /// 主界面
        /// </summary>
        protected WinGuiMain guiMain;

        /// <summary>
        /// 与用存储子窗体的哈希表
        /// </summary>
        protected Hashtable _hash = null;

        /// <summary>
        /// 子窗体容器，用于存储子窗体中的控件
        /// </summary>
        protected System.Windows.Forms.Control _container;

        /// <summary>
        /// 当前激活的子窗体
        /// </summary>
        protected IMdiChild _active = null;

        #endregion 声明或定义变量

        #region 属性

        /// <summary>
        /// 设置子窗体的激活属性
        /// 若为true,则会重绘子界面
        /// 若为false,则子窗体不可见
        /// </summary>
        public IMdiChild Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active == value)
                    return;
                else
                    ActiveGui(value);
            }
        }

        /// <summary>
        /// 获驱主窗体的包含的子窗体数目
        /// </summary>
        public int Count
        {
            get
            {
                if (_hash == null)
                    return _hash.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 根据ID获取子窗体
        /// 若不存在此窗体，则返回空(NULL)
        /// </summary>
        public IMdiChild this[string id]
        {
            get
            {
                if (_hash.Contains(id))
                    return (IMdiChild)_hash[id];
                else
                    return null;
            }
        }

        /// <summary>
        /// 根据索引值获取子窗体
        /// </summary>
        public IMdiChild this[int index]
        {
            get
            {
                if (_hash == null)
                    return null;
                int i = 0;
                foreach (object key in _hash.Keys)
                {
                    if (i == index)
                        return (IMdiChild)_hash[key];
                    else
                        i++;
                }
                return null;
            }
        }

        #endregion 属性

        /// <summary>
        /// 设置子窗体为激活状态
        /// </summary>
        /// <param name="active">子窗体对象</param>
        private void ActiveGui(IMdiChild active)
        {
            Framework.WinGui.Menus.AppMenuCommand stype;
            //用于控制toolbar是否显示
            Framework.WinGui.Menus.AppMenuCommand toollistmenu;
            Framework.WinGui.Menus.AppMenuCommand mdilist;

            //实现关闭以前窗口
            //this._container.SuspendLayout();
            if (_active != null)
                this.DeactiveGui();
            //激活当前窗口,修改主窗口显示信息
            guiMain.Text = string.Format(CardApplication.applicationName + " - [{0}]", active.Caption);
            this._active = active;
            ((Control)_active).Capture = true;
            this._container.Controls.Add((Control)_active);
            ((Control)_active).Parent = this._container;
            // ((Control)_active).Focus();
            _active.OnMidChildActive();

            //必须在子控件中生成一个menu,并设置其属性IsToolBarList为true,否则默认条件下,下面的代码是不执行的
            toollistmenu = AppMenuCommand.ToolBarList;
            if (toollistmenu != null)
            {
                if (active.Tools != null)
                {
                    for (int i = 0; i < active.Tools.Length; i++)
                    {
                        for (int j = 0; j < toollistmenu.Parent.MenuItems.Count; j++)
                        {
                            if (toollistmenu.Parent.MenuItems[j] is AppMenuCommand)
                            {
                                stype = (AppMenuCommand)toollistmenu.Parent.MenuItems[j];
                                if (stype.ID == "RES_Menu" + active.ID + active.Tools[i].Name)
                                {
                                    stype.Checked = true;
                                    stype.Enabled = true;
                                    //									break;改动过的代码
                                }
                            }
                            //添加工具条到停靠
                            //把子窗体的toolbar添加至主窗体中
                            //							((IMdiContainer)guiMain).MdiToolBarPanel.AddToolbar(active.Tools[i]); 
                            active.Tools[i].Parent = guiMain.MdiToolBarPanel.TopContainer;
                            active.Tools[i].Show();
                        }
                    }
                }
            }


            //激活窗口菜单状态
            mdilist = AppMenuCommand.MidList;
            if (mdilist != null)
            {
                for (int j = 0; j < mdilist.Parent.MenuItems.Count; j++)
                {
                    if (mdilist.Parent.MenuItems[j] is AppMenuCommand)
                    {
                        stype = (AppMenuCommand)mdilist.Parent.MenuItems[j];
                        if (stype.ID == active.ID + "_MdiList")
                        {
                            stype.Checked = true;
                            break;
                        }
                    }
                }
            }

            //把子窗体中的菜单加入主窗体的菜单栏中
            if (active.Menus != null)
            {
                for (int j = 0; j < active.Menus.Length; j++)
                {
                    int pos = ((IMdiContainer)guiMain).MdiMainMenu.Buttons.Count - 1;
                    //刘李明删除的代码，菜单已经生成了，不过没有添加至菜单栏内
                    ((IMdiContainer)guiMain).MdiMainMenu.Buttons.Insert(pos + 1, active.Menus[j]);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DeactiveGui()
        {
            Framework.WinGui.Menus.AppMenuCommand stype;
            Framework.WinGui.Menus.AppMenuCommand toollistmenu;
            Framework.WinGui.Menus.AppMenuCommand mdilist;
            this._container.Focus();
            ((Control)_active).Capture = false;
            this._container.Controls.Remove((Control)_active);

            ((Control)_active).Parent = null;
            //改变工具条菜单状态
            toollistmenu = AppMenuCommand.ToolBarList;
            if (toollistmenu != null)
            {
                if (_active.Tools != null)
                {
                    for (int i = 0; i < _active.Tools.Length; i++)
                    {
                        for (int j = 0; j < toollistmenu.Parent.MenuItems.Count; j++)
                        {
                            if (toollistmenu.Parent.MenuItems[j] is AppMenuCommand)
                            {
                                stype = (AppMenuCommand)toollistmenu.Parent.MenuItems[j];
                                if (stype.ID == _active.ID + _active.Tools[i].Name)
                                {
                                    //去掉的代码
                                    //									stype.Checked = false;
                                    //									stype.Enabled = false;
                                    //									break;
                                }
                            }
                            //从停靠中移除工具条
                            ((IMdiContainer)guiMain).MdiToolBarPanel.RemoveToolbar(_active.Tools[i]);
                            _active.Tools[i].Parent = null;
                            _active.Tools[i].Close();
                        }
                    }
                }
            }

            //改变窗口菜单状态
            mdilist = AppMenuCommand.MidList;
            if (mdilist != null)
            {
                for (int j = 0; j < mdilist.Parent.MenuItems.Count; j++)
                {
                    if (mdilist.Parent.MenuItems[j] is AppMenuCommand)
                    {
                        stype = (AppMenuCommand)mdilist.Parent.MenuItems[j];
                        if (stype.ID == _active.ID + "_MdiList")
                        {
                            stype.Checked = false;
                            break;
                        }
                    }
                }
            }

            //移除子菜单项目
            if (_active.Menus != null)
            {
                for (int j = 0; j < _active.Menus.Length; j++)
                {
                    //刘李明删除的代码
                    ((IMdiContainer)guiMain).MdiMainMenu.Buttons.Remove(_active.Menus[j]);
                }
            }
            _active = null;
        }

        /// <summary>
        /// WindowManage的构造函数
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="container"></param>
        public WindowManage(WinGuiMain gui, System.Windows.Forms.Control container)
        {
            guiMain = gui;
            _container = container;
            _hash = new Hashtable(15);
        }

        /// <summary>
        /// 判断是否含有此子窗体
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return _hash.Contains(key);
        }

        /// <summary>
        /// 添加子窗体至主父窗体中
        /// </summary>
        /// <param name="gui"></param>
        public void Add(IMdiChild gui)
        {
            if (this._hash.ContainsKey(gui.ID))
                return;
            else
            {
                this._hash.Add(gui.ID, gui);
                //添加工具条菜单
                Framework.WinGui.Menus.AppMenuCommand toollistmenu = AppMenuCommand.ToolBarList;
                if (toollistmenu != null)
                {
                    if (gui.Tools != null && gui.Tools.Length != 0)
                    {
                        for (int i = 0; i < gui.Tools.Length; i++)
                        {
                            Framework.WinGui.Menus.AppMenuCommand stype;
                            stype = new AppMenuCommand(gui.ID + gui.Tools[i].Name, new ExecuteCommandHandler(this.OnMdiToolsBarSelectExecute), gui.Tools[i].Name);
                            stype.Text = gui.Tools[i].Text;
                            //默认为选择
                            stype.Checked = true;
                            gui.Tools[i].Closed += new EventHandler(OnToolsBarClose);
                            toollistmenu.Parent.MenuItems.Add(stype);
                        }
                    }
                }
                //添加窗体菜单
                Framework.WinGui.Menus.AppMenuCommand mdilist = AppMenuCommand.MidList;
                if (mdilist != null)
                {
                    Framework.WinGui.Menus.AppMenuCommand stype;
                    stype = new AppMenuCommand(gui.ID + "_MdiList", new ExecuteCommandHandler(this.OnMdiChildSelectExecute), gui.Caption);
                    if (this._hash.Count == 1)
                        stype.BeginGroup = true;
                    mdilist.Parent.MenuItems.Add(stype);
                }
            }
        }

        /// <summary>
        /// 删除某个子窗体
        /// </summary>
        /// <param name="gui"></param>
        public void Remove(IMdiChild gui)
        {
            if (_hash.ContainsValue(gui))
            {
                Framework.WinGui.Menus.AppMenuCommand stype;
                if (this._active == gui)
                    this.DeactiveGui();
                _hash.Remove(gui.ID);
                //移除工具条菜单
                Framework.WinGui.Menus.AppMenuCommand toollistmenu = AppMenuCommand.ToolBarList;
                if (toollistmenu != null)
                {
                    if (gui.Tools != null)
                    {
                        for (int i = 0; i < gui.Tools.Length; i++)
                        {
                            for (int j = 0; j < toollistmenu.Parent.MenuItems.Count; j++)
                            {
                                if (toollistmenu.Parent.MenuItems[j] is AppMenuCommand)
                                {
                                    stype = (AppMenuCommand)toollistmenu.Parent.MenuItems[j];
                                    if (stype.ID == gui.ID + gui.Tools[i].Name)
                                    {
                                        toollistmenu.Parent.MenuItems.RemoveAt(j);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                //移除窗口菜单
                Framework.WinGui.Menus.AppMenuCommand mdilist = AppMenuCommand.MidList;
                if (mdilist != null)
                {
                    for (int j = 0; j < mdilist.Parent.MenuItems.Count; j++)
                    {
                        if (mdilist.Parent.MenuItems[j] is AppMenuCommand)
                        {
                            stype = (AppMenuCommand)mdilist.Parent.MenuItems[j];
                            if (stype.ID == gui.ID + "_MdiList")
                            {
                                mdilist.Parent.MenuItems.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
                //释放窗体资源
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnMdiChildSelectExecute(ICommand sender)
        {
            if (sender is AppMenuCommand)
            {
                AppMenuCommand m = (AppMenuCommand)sender;
                string id = m.Tag.ToString().Substring(0, m.Tag.ToString().Length - 8);
                IMdiChild child = (IMdiChild)this._hash[id];
                if (child != null && this.Active != child)
                    this.ActiveGui(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnMdiToolsBarSelectExecute(ICommand sender)
        {
            //显示当前的工具条
            if (_active != null && _active.Tools != null && sender is AppMenuCommand)
            {
                AppMenuCommand stype = (AppMenuCommand)sender;
                for (int i = 0; i < _active.Tools.Length; i++)
                {
                    if (sender.ID == _active.ID + _active.Tools[i].Name)
                    {
                        if (stype.Checked)
                        {
                            ((IMdiContainer)guiMain).MdiToolBarPanel.RemoveToolbar(_active.Tools[i]);
                            _active.Tools[i].Parent = null;
                            _active.Tools[i].Close();
                        }
                        else
                        {
                            ((IMdiContainer)guiMain).MdiToolBarPanel.AddToolbar(_active.Tools[i]);
                            _active.Tools[i].Show();
                            _active.Tools[i].Parent = guiMain.MdiToolBarPanel.TopContainer;
                            stype.Checked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnToolsBarClose(object sender, System.EventArgs e)
        {
            //添加工具条菜单
            Framework.WinGui.Menus.AppMenuCommand stype;
            Framework.WinGui.Menus.AppMenuCommand toollistmenu = AppMenuCommand.ToolBarList;
            if (toollistmenu != null && sender is TD.SandBar.ToolBar)
            {
                for (int j = 0; j < toollistmenu.Parent.MenuItems.Count; j++)
                {
                    if (toollistmenu.Parent.MenuItems[j] is AppMenuCommand)
                    {
                        stype = (AppMenuCommand)toollistmenu.Parent.MenuItems[j];
                        if (stype.ID == _active.ID + ((TD.SandBar.ToolBar)sender).Name)
                        {
                            stype.Checked = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
