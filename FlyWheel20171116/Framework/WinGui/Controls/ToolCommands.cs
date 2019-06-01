using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

//using RssComponents;
using Framework.WinGui.Interfaces;
using Framework.WinGui.Utility;
using Framework.Core;

namespace Framework.WinGui.Tools
{
    #region AppToolCommand class
    /// <summary>
    /// Colleage base Toolbar Command class, that is controlled by and talks to the mediator
    /// </summary>
    public class AppToolCommand : TD.SandBar.ButtonItem, ICommand, ICommandComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        protected CommandMediator _med;
        protected string _id = string.Empty;
        protected event ExecuteCommandHandler OnExecute;


        public string ID
        {
            get
            {
                return _id;
            }
        }

        public AppToolCommand()
        {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

        }

        public AppToolCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId)
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
            _id = cmdId;
            _med = mediator;
            OnExecute += executor;
            _med.RegisterCommand(cmdId, this);
        }
        public AppToolCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            this.ImageIndex = imageIndex;
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

    #region AppToolMenuCommand class
    /// <summary>
    /// Colleage base Menu class, that is controlled by and talks to the mediator
    /// </summary>
    public class AppToolMenuCommand : TD.SandBar.DropDownMenuItem, ICommand, ICommandComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        protected CommandMediator med;
        protected event ExecuteCommandHandler OnExecute;
        protected string _id = string.Empty;

        public string ID
        {
            get
            {
                return _id;
            }
        }

        public AppToolMenuCommand()
        {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

        }

        public AppToolMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId)
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
            _id = cmdId;
            OnExecute += executor;
            med = mediator;
            med.RegisterCommand(cmdId, this);
        }
        public AppToolMenuCommand(string cmdId, CommandMediator mediator, ExecuteCommandHandler executor, string captionResourceId, string descResourceId, int imageIndex)
            : this(cmdId, mediator, executor, captionResourceId, descResourceId)
        {
            this.ImageIndex = imageIndex;
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


        #endregion

        public CommandMediator Mediator
        {
            get { return med; }
            set { med = value; }
        }
    }

    #endregion

}
