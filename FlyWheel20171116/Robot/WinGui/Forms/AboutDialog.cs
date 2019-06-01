using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;


namespace Card.WinGui.Forms
{
    class AboutDialog : Form
    {
        // Fields
        internal RichTextBox AboutText;
        private IContainer components;
        private PictureBox IconPicture;
        private Button OKButton;

        // Methods
        internal AboutDialog()
        {
            this.InitializeComponent();


        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);

        }
        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(AboutDialog));
            this.AboutText = new RichTextBox();
            this.IconPicture = new PictureBox();
            this.OKButton = new Button();
            ((ISupportInitialize)this.IconPicture).BeginInit();
            base.SuspendLayout();
            this.AboutText.BackColor = SystemColors.Window;
            this.AboutText.BorderStyle = BorderStyle.None;
            this.AboutText.Location = new Point(0x5f, 0x11);
            this.AboutText.Name = "AboutText";
            this.AboutText.ReadOnly = true;
            this.AboutText.Size = new Size(0x11c, 0xc3);
            this.AboutText.TabIndex = 0;
            this.AboutText.Text = "";
            //this.IconPicture.Image = (Image)manager.GetObject("Log_m");
            this.IconPicture.Location = new Point(12, 0x11);
            this.IconPicture.Name = "IconPicture";
            this.IconPicture.Size = new Size(0x40, 0x40);
            this.IconPicture.TabIndex = 1;
            this.IconPicture.TabStop = false;
            this.OKButton.DialogResult = DialogResult.OK;
            this.OKButton.Location = new Point(0xa7, 0xe4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new Size(0x43, 0x18);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            base.AcceptButton = this.OKButton;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Window;
            // this.BackgroundImage = (Image)manager.GetObject("$this.BackgroundImage");
            base.ClientSize = new Size(400, 300);
            base.ControlBox = false;
            base.Controls.Add(this.OKButton);
            base.Controls.Add(this.IconPicture);
            base.Controls.Add(this.AboutText);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            // base.Icon = (Icon)manager.GetObject("$this.Icon");
            base.Name = "AboutDialog";
            base.ShowInTaskbar = false;
            this.Text = "About Visual Simulation Environment for Mobile Arm";
            ((ISupportInitialize)this.IconPicture).EndInit();
            base.ResumeLayout(false);

        }

    }
}
