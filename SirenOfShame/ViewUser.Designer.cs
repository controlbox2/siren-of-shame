﻿namespace SirenOfShame
{
    partial class ViewUser
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this._userName = new System.Windows.Forms.Label();
            this._closeButton = new System.Windows.Forms.PictureBox();
            this._achievementsText = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._unobtainedTemplate = new System.Windows.Forms.Label();
            this._obtainedTemplate = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._changeAvatar = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._reputation = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._achievementCount = new System.Windows.Forms.Label();
            this.avatar1 = new SirenOfShame.Avatar();
            ((System.ComponentModel.ISupportInitialize)(this._closeButton)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _userName
            // 
            this._userName.AutoSize = true;
            this._userName.BackColor = System.Drawing.Color.Transparent;
            this._userName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._userName.Location = new System.Drawing.Point(11, 2);
            this._userName.Name = "_userName";
            this._userName.Size = new System.Drawing.Size(70, 26);
            this._userName.TabIndex = 0;
            this._userName.Text = "label1";
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._closeButton.Image = global::SirenOfShame.Properties.Resources.CloseButton2;
            this._closeButton.Location = new System.Drawing.Point(612, 9);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(12, 12);
            this._closeButton.TabIndex = 1;
            this._closeButton.TabStop = false;
            this._closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // _achievementsText
            // 
            this._achievementsText.AutoSize = true;
            this._achievementsText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._achievementsText.Location = new System.Drawing.Point(18, 0);
            this._achievementsText.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this._achievementsText.Name = "_achievementsText";
            this._achievementsText.Size = new System.Drawing.Size(93, 16);
            this._achievementsText.TabIndex = 2;
            this._achievementsText.Text = "Achievements";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this._unobtainedTemplate);
            this.flowLayoutPanel1.Controls.Add(this._obtainedTemplate);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(76, 68);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(559, 215);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // _unobtainedTemplate
            // 
            this._unobtainedTemplate.AccessibleDescription = "";
            this._unobtainedTemplate.AccessibleName = "";
            this._unobtainedTemplate.AutoSize = true;
            this._unobtainedTemplate.BackColor = System.Drawing.Color.LightGray;
            this._unobtainedTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._unobtainedTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._unobtainedTemplate.ForeColor = System.Drawing.Color.Gray;
            this._unobtainedTemplate.Location = new System.Drawing.Point(3, 3);
            this._unobtainedTemplate.Margin = new System.Windows.Forms.Padding(3);
            this._unobtainedTemplate.Name = "_unobtainedTemplate";
            this._unobtainedTemplate.Size = new System.Drawing.Size(141, 18);
            this._unobtainedTemplate.TabIndex = 5;
            this._unobtainedTemplate.Text = "Unobtained Template";
            this._unobtainedTemplate.Visible = false;
            // 
            // _obtainedTemplate
            // 
            this._obtainedTemplate.AutoSize = true;
            this._obtainedTemplate.BackColor = System.Drawing.SystemColors.WindowFrame;
            this._obtainedTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._obtainedTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._obtainedTemplate.ForeColor = System.Drawing.SystemColors.Window;
            this._obtainedTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._obtainedTemplate.Location = new System.Drawing.Point(150, 3);
            this._obtainedTemplate.Margin = new System.Windows.Forms.Padding(3);
            this._obtainedTemplate.Name = "_obtainedTemplate";
            this._obtainedTemplate.Size = new System.Drawing.Size(143, 18);
            this._obtainedTemplate.TabIndex = 4;
            this._obtainedTemplate.Text = "Obtained Template";
            this._obtainedTemplate.Visible = false;
            // 
            // _changeAvatar
            // 
            this._changeAvatar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._changeAvatar.Location = new System.Drawing.Point(13, 98);
            this._changeAvatar.Name = "_changeAvatar";
            this._changeAvatar.Size = new System.Drawing.Size(50, 19);
            this._changeAvatar.TabIndex = 8;
            this._changeAvatar.TabStop = true;
            this._changeAvatar.Text = "change";
            this._changeAvatar.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._changeAvatar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ChangeAvatarLinkClicked);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::SirenOfShame.Properties.Resources.gradient33;
            this.panel1.Controls.Add(this._userName);
            this.panel1.Controls.Add(this._closeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 30);
            this.panel1.TabIndex = 9;
            // 
            // _reputation
            // 
            this._reputation.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._reputation.Location = new System.Drawing.Point(5, 120);
            this._reputation.Name = "_reputation";
            this._reputation.Size = new System.Drawing.Size(67, 27);
            this._reputation.TabIndex = 10;
            this._reputation.Text = "1000";
            this._reputation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this._achievementCount);
            this.flowLayoutPanel2.Controls.Add(this._achievementsText);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(73, 49);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(232, 19);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // _achievementCount
            // 
            this._achievementCount.AutoSize = true;
            this._achievementCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._achievementCount.Location = new System.Drawing.Point(3, 0);
            this._achievementCount.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this._achievementCount.Name = "_achievementCount";
            this._achievementCount.Size = new System.Drawing.Size(15, 16);
            this._achievementCount.TabIndex = 0;
            this._achievementCount.Text = "8";
            // 
            // avatar1
            // 
            this.avatar1.BackColor = System.Drawing.Color.Transparent;
            this.avatar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.avatar1.ImageIndex = -1;
            this.avatar1.Location = new System.Drawing.Point(13, 50);
            this.avatar1.Name = "avatar1";
            this.avatar1.Size = new System.Drawing.Size(50, 50);
            this.avatar1.TabIndex = 7;
            this.avatar1.Click += new System.EventHandler(this.Avatar1Click);
            // 
            // ViewUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this._reputation);
            this.Controls.Add(this._changeAvatar);
            this.Controls.Add(this.avatar1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "ViewUser";
            this.Size = new System.Drawing.Size(633, 281);
            ((System.ComponentModel.ISupportInitialize)(this._closeButton)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _userName;
        private System.Windows.Forms.PictureBox _closeButton;
        private System.Windows.Forms.Label _achievementsText;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label _obtainedTemplate;
        private System.Windows.Forms.Label _unobtainedTemplate;
        private System.Windows.Forms.ToolTip toolTip1;
        private Avatar avatar1;
        private System.Windows.Forms.LinkLabel _changeAvatar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _reputation;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label _achievementCount;
    }
}
