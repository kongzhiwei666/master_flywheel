using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;


namespace Common
{

    public class CommonGuiTwo : Common.CommonGui
    {
        private System.ComponentModel.IContainer components = null;
        protected System.Windows.Forms.TabControl tabMain;
        public CommonGuiTwo()
        {
            InitializeComponent();

        }
        public CommonGuiTwo(string id)
            : base(id)
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

        #region 设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(721, 478);
            this.tabMain.TabIndex = 0;
            // 
            // CommonGuiTwo
            // 
            this.Controls.Add(this.tabMain);
            this.Name = "CommonGuiTwo";
            this.Size = new System.Drawing.Size(721, 478);
            this.ResumeLayout(false);

        }
        #endregion

        #region 初始化
        protected override bool Init()
        {
            bool bResult = true;
            try
            {
             
                InitResource();
                InitShortcutManager();
                InitMenuBar();
                InitToolBars();
                InitContextMenus();
             
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                bResult = false;
            }
            return bResult;
        }



        #endregion







    }

}

