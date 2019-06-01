using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using Framework.WinGui.Utility.Keyboard;
using Framework.WinGui.Interfaces;
using Framework.WinGui.Utility;
using Framework.Core;

namespace Framework.WinGui.Menus
{

    #region AppMenuCommand class
    /// <summary>
    /// Colleage base Menu class, that is controlled by and talks to the mediator
    /// </summary>
    public class AppMenuCommand : TD.SandBar.MenuButtonItem, ICommand, ICommandComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        protected CommandMediator _med;
        protected event ExecuteCommandHandler OnExecute;

        //for mdi form framework
        protected static AppMenuCommand _mdilist = null;
        protected static AppMenuCommand _toolbarlist = null;
        protected static AppMenuCommand _mergemenulist = null;
        protected bool _ismdilist = false;
        protected bool _istoolbarlist = false;
        protected bool _ismergemenulist = false;

        protected string _id = string.Empty;
        public string ID
        {
            get
            {
                return _id;
            }
        }
        public bool IsMdiList
        {
            get
            {
                return _ismdilist;
            }
            set
            {
                if (_ismdilist)
                {
                    if (value.Equals(true))
                        return;
                    else
                    {
                        AppMenuCommand._mdilist = null;
                        _ismdilist = false;
                    }
                }
                else
                {
                    if (AppMenuCommand._mdilist != null)
                        throw new Exception("Application as mdilist Menu already");
                    else
                    {
                        _ismdilist = true;
                        _mdilist = this;
                    }
                }
            }
        }

        public bool IsToolBarList
        {
            get
            {
                return _istoolbarlist;
            }
            set
            {
                if (_istoolbarlist)
                {
                    if (value.Equals(true))
                        return;
                    else
                    {
                        AppMenuCommand._toolbarlist = null;
                        _istoolbarlist = false;
                    }
                }
                else
                {
                    if (AppMenuCommand._toolbarlist != null)
                        throw new Exception("Application as toolbarlist Menu already");
                    else
                    {
                        _istoolbarlist = true;
                        _toolbarlist = this;
                    }
                }
            }
        }
        public bool IsMergeMenuList
        {
            get
            {
                return _ismergemenulist;
            }
            set
            {
                if (_ismergemenulist)
                {
                    if (value.Equals(true))
                        return;
                    else
                    {
                        AppMenuCommand._mergemenulist = null;
                        _ismergemenulist = false;
                    }
                }
                else
                {
                    if (AppMenuCommand._mergemenulist != null)
                        throw new Exception("Application as mergemenulist Menu already");
                    else
                    {
                        _ismergemenulist = true;
                        _mergemenulist = this;
                    }
                }
            }
        }
        public static AppMenuCommand MidList
        {
            get
            {
                return _mdilist;
            }
        }
        public static AppMenuCommand ToolBarList
        {
            get
            {
                return _toolbarlist;
            }
        }
        public static AppMenuCommand MergeMenuList
        {
            get
            {
                return _mergemenulist;
            }
        }
        //protected object _tag;

        public AppMenuCommand()
        {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //create default click handler
            this.Activate += new System.EventHandler(this.ClickHandler);
        }

        public AppMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, ShortcutManager shortcuts)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            SetShortcuts(cmdId, shortcuts);
        }
        public AppMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId)
            : this()
        {
            string t = Resource.Manager[captionResourceId];
            string d = Resource.Manager[descResourceId];

            if (t == null)
                t = captionResourceId;
            if (d == null)
                d = descResourceId;

            base.Text = t;
            base.ToolTipText = d;

            base.Tag = cmdId;
            this._id = cmdId;
            _med = mediator;
            OnExecute += executor;
            if (_med != null)
                _med.RegisterCommand(cmdId, this);
        }

        public AppMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            base.ImageIndex = imageIndex;
        }

        public AppMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex, ShortcutManager shortcuts)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId, imageIndex)
        {
            SetShortcuts(cmdId, shortcuts);
        }

        public AppMenuCommand(string cmdId, ExecuteCommandHandler executor, string caption)
            : this()
        {
            base.Text = caption;
            base.Tag = cmdId;
            this._id = cmdId;
            OnExecute += executor;
        }
        private void SetShortcuts(string cmdId, ShortcutManager shortcuts)
        {
            if (shortcuts != null)
            {
                this.Shortcut = shortcuts[cmdId];
            }
        }

        #region ICommandComponent implementation: abstract from the concrete Base class

        public new bool Checked
        {
            get { return base.Checked; }
            set { base.Checked = value; }
        }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        public new bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        #endregion

        public void ClickHandler(object obj, EventArgs e)
        {
            this.Execute();
        }

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Implementation of ICommand

        public virtual void Execute()
        {
            if (OnExecute != null)
                OnExecute(this);
        }

        public virtual void Initialize()
        {
            // empty here
        }

        #endregion

        public CommandMediator Mediator
        {
            get { return _med; }
            set { _med = value; }
        }

    }

    #endregion

    #region AppContextMenuCommand class
    /// <summary>
    /// Colleage base Menu class, that is controlled by and talks to the mediator
    /// </summary>
    public class AppContextMenuCommand : System.Windows.Forms.MenuItem, ICommand, ICommandComponent
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        protected CommandMediator _med;
        protected event ExecuteCommandHandler OnExecute;
        protected string _description = String.Empty;
        protected int _imageIndex;
        private string _id = string.Empty;

        public AppContextMenuCommand()
            : base()
        {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //create default click handler
            EventHandler evh = new EventHandler(this.ClickHandler);
            this.Click += evh;
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, ShortcutManager shortcuts)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            SetShortcuts(cmdId, shortcuts);
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId)
            : this()
        {
            string _text = Resource.Manager[captionResourceId];

            if (_text == null)
                _text = captionResourceId;
            base.Text = _text;

            _description = Resource.Manager[descResourceId];
            if (_description == null)
                _description = descResourceId;

            _id = cmdId;
            _med = mediator;
            OnExecute += executor;
            if (_med != null)
                _med.RegisterCommand(cmdId, this);
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex, ShortcutManager shortcuts)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            SetShortcuts(cmdId, shortcuts);
        }
        public AppContextMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            _imageIndex = imageIndex;
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, string caption, string desc, ShortcutManager shortcuts)
            : this()
        {
            SetShortcuts(cmdId, shortcuts);
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, string caption, string desc)
            : this()
        {
            base.Text = caption;
            _description = desc;
            _id = cmdId;
            _med = mediator;
            if (_med != null)
                _med.RegisterCommand(cmdId, this);
        }

        public AppContextMenuCommand(string cmdId, CommandMediator mediator, string caption, string desc, int imageIndex, ShortcutManager shortcuts)
            : this(cmdId, mediator, caption, desc, shortcuts)
        {
            _imageIndex = imageIndex;
        }

        private void SetShortcuts(string cmdId, ShortcutManager shortcuts)
        {
            if (shortcuts != null)
            {
                this.Shortcut = shortcuts[cmdId];
                this.ShowShortcut = shortcuts.IsDisplayed(cmdId);
            }
        }

        public string ID
        {
            get { return _id; }
        }

        #region ICommandComponent implementation: abstract from the concrete Base class

        public new bool Checked
        {
            get { return base.Checked; }
            set { base.Checked = value; }
        }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        public new bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        public void ClickHandler(object obj, EventArgs e)
        {
            this.Execute();
        }
        #endregion

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Implementation of ICommand

        public virtual void Execute()
        {
            if (OnExecute != null)
                OnExecute(this);
        }

        public virtual void Initialize()
        {
            // empty here
        }

        public CommandMediator Mediator
        {
            get { return _med; }
            set { _med = value; }
        }

        #endregion
    }

    #endregion

}
