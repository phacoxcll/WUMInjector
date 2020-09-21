namespace WUMInjector
{
    partial class WUMInjectorGUI
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WUMInjectorGUI));
            this.panelLoadedBase = new System.Windows.Forms.Panel();
            this.labelBase = new System.Windows.Forms.Label();
            this.buttonLoadBase = new System.Windows.Forms.Button();
            this.panelCommonKey = new System.Windows.Forms.Panel();
            this.textBoxCommonKey = new System.Windows.Forms.TextBox();
            this.labelCommonKey = new System.Windows.Forms.Label();
            this.buttonInjectPack = new System.Windows.Forms.Button();
            this.buttonInjectNotPack = new System.Windows.Forms.Button();
            this.textBoxShortName = new System.Windows.Forms.TextBox();
            this.buttonChoose = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.panelImages = new System.Windows.Forms.Panel();
            this.buttonDrcPreview = new System.Windows.Forms.Button();
            this.buttonTvPreview = new System.Windows.Forms.Button();
            this.buttonIconFrame = new System.Windows.Forms.Button();
            this.buttonTvBackground = new System.Windows.Forms.Button();
            this.pictureBoxBootTv = new System.Windows.Forms.PictureBox();
            this.buttonDrcBackground = new System.Windows.Forms.Button();
            this.buttonIconBackground = new System.Windows.Forms.Button();
            this.pictureBoxMenuIcon = new System.Windows.Forms.PictureBox();
            this.buttonIconPreview = new System.Windows.Forms.Button();
            this.pictureBoxBootDrc = new System.Windows.Forms.PictureBox();
            this.labelId = new System.Windows.Forms.Label();
            this.labelSize = new System.Windows.Forms.Label();
            this.labelShortName = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.panelImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBootTv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMenuIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBootDrc)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLoadedBase
            // 
            this.panelLoadedBase.BackgroundImage = global::WUMInjector.Properties.Resources.x_mark_16;
            this.panelLoadedBase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelLoadedBase.Location = new System.Drawing.Point(93, 376);
            this.panelLoadedBase.Name = "panelLoadedBase";
            this.panelLoadedBase.Size = new System.Drawing.Size(20, 20);
            this.panelLoadedBase.TabIndex = 21;
            // 
            // labelBase
            // 
            this.labelBase.AutoSize = true;
            this.labelBase.Location = new System.Drawing.Point(119, 380);
            this.labelBase.Name = "labelBase";
            this.labelBase.Size = new System.Drawing.Size(67, 13);
            this.labelBase.TabIndex = 20;
            this.labelBase.Text = "Base invalid!";
            // 
            // buttonLoadBase
            // 
            this.buttonLoadBase.Location = new System.Drawing.Point(12, 375);
            this.buttonLoadBase.Name = "buttonLoadBase";
            this.buttonLoadBase.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadBase.TabIndex = 19;
            this.buttonLoadBase.Text = "Load base";
            this.buttonLoadBase.UseVisualStyleBackColor = true;
            this.buttonLoadBase.Click += new System.EventHandler(this.buttonLoadBase_Click);
            // 
            // panelCommonKey
            // 
            this.panelCommonKey.BackgroundImage = global::WUMInjector.Properties.Resources.x_mark_16;
            this.panelCommonKey.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelCommonKey.Location = new System.Drawing.Point(341, 345);
            this.panelCommonKey.Name = "panelCommonKey";
            this.panelCommonKey.Size = new System.Drawing.Size(20, 20);
            this.panelCommonKey.TabIndex = 18;
            // 
            // textBoxCommonKey
            // 
            this.textBoxCommonKey.Location = new System.Drawing.Point(119, 345);
            this.textBoxCommonKey.Name = "textBoxCommonKey";
            this.textBoxCommonKey.Size = new System.Drawing.Size(216, 20);
            this.textBoxCommonKey.TabIndex = 17;
            this.textBoxCommonKey.TextChanged += new System.EventHandler(this.textBoxCommonKey_TextChanged);
            // 
            // labelCommonKey
            // 
            this.labelCommonKey.AutoSize = true;
            this.labelCommonKey.Location = new System.Drawing.Point(12, 348);
            this.labelCommonKey.Name = "labelCommonKey";
            this.labelCommonKey.Size = new System.Drawing.Size(101, 13);
            this.labelCommonKey.TabIndex = 16;
            this.labelCommonKey.Text = "Wii U Common Key:";
            // 
            // buttonInjectPack
            // 
            this.buttonInjectPack.Location = new System.Drawing.Point(537, 40);
            this.buttonInjectPack.Name = "buttonInjectPack";
            this.buttonInjectPack.Size = new System.Drawing.Size(75, 23);
            this.buttonInjectPack.TabIndex = 15;
            this.buttonInjectPack.Text = "Do pack";
            this.buttonInjectPack.UseVisualStyleBackColor = true;
            this.buttonInjectPack.Click += new System.EventHandler(this.buttonInjectPack_Click);
            // 
            // buttonInjectNotPack
            // 
            this.buttonInjectNotPack.Location = new System.Drawing.Point(456, 40);
            this.buttonInjectNotPack.Name = "buttonInjectNotPack";
            this.buttonInjectNotPack.Size = new System.Drawing.Size(75, 23);
            this.buttonInjectNotPack.TabIndex = 14;
            this.buttonInjectNotPack.Text = "Do not pack";
            this.buttonInjectNotPack.UseVisualStyleBackColor = true;
            this.buttonInjectNotPack.Click += new System.EventHandler(this.buttonInjectNotPack_Click);
            // 
            // textBoxShortName
            // 
            this.textBoxShortName.Location = new System.Drawing.Point(57, 42);
            this.textBoxShortName.Name = "textBoxShortName";
            this.textBoxShortName.Size = new System.Drawing.Size(393, 20);
            this.textBoxShortName.TabIndex = 13;
            // 
            // buttonChoose
            // 
            this.buttonChoose.Location = new System.Drawing.Point(12, 12);
            this.buttonChoose.Name = "buttonChoose";
            this.buttonChoose.Size = new System.Drawing.Size(75, 23);
            this.buttonChoose.TabIndex = 12;
            this.buttonChoose.Text = "Chosse";
            this.buttonChoose.UseVisualStyleBackColor = true;
            this.buttonChoose.Click += new System.EventHandler(this.buttonChoose_Click);
            // 
            // panelImages
            // 
            this.panelImages.Controls.Add(this.buttonDrcPreview);
            this.panelImages.Controls.Add(this.buttonTvPreview);
            this.panelImages.Controls.Add(this.buttonIconFrame);
            this.panelImages.Controls.Add(this.buttonTvBackground);
            this.panelImages.Controls.Add(this.pictureBoxBootTv);
            this.panelImages.Controls.Add(this.buttonDrcBackground);
            this.panelImages.Controls.Add(this.buttonIconBackground);
            this.panelImages.Controls.Add(this.pictureBoxMenuIcon);
            this.panelImages.Controls.Add(this.buttonIconPreview);
            this.panelImages.Controls.Add(this.pictureBoxBootDrc);
            this.panelImages.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelImages.Location = new System.Drawing.Point(12, 86);
            this.panelImages.Name = "panelImages";
            this.panelImages.Size = new System.Drawing.Size(603, 254);
            this.panelImages.TabIndex = 22;
            // 
            // buttonDrcPreview
            // 
            this.buttonDrcPreview.Location = new System.Drawing.Point(4, 61);
            this.buttonDrcPreview.Name = "buttonDrcPreview";
            this.buttonDrcPreview.Size = new System.Drawing.Size(117, 27);
            this.buttonDrcPreview.TabIndex = 16;
            this.buttonDrcPreview.Text = "GamePad Preview";
            this.buttonDrcPreview.UseVisualStyleBackColor = true;
            this.buttonDrcPreview.Click += new System.EventHandler(this.buttonDrcPreview_Click);
            // 
            // buttonTvPreview
            // 
            this.buttonTvPreview.Location = new System.Drawing.Point(4, 32);
            this.buttonTvPreview.Name = "buttonTvPreview";
            this.buttonTvPreview.Size = new System.Drawing.Size(117, 27);
            this.buttonTvPreview.TabIndex = 15;
            this.buttonTvPreview.Text = "TV Preview";
            this.buttonTvPreview.UseVisualStyleBackColor = true;
            this.buttonTvPreview.Click += new System.EventHandler(this.buttonTvPreview_Click);
            // 
            // buttonIconFrame
            // 
            this.buttonIconFrame.BackColor = System.Drawing.SystemColors.Control;
            this.buttonIconFrame.FlatAppearance.BorderSize = 0;
            this.buttonIconFrame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.buttonIconFrame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.buttonIconFrame.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonIconFrame.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonIconFrame.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonIconFrame.Location = new System.Drawing.Point(4, 90);
            this.buttonIconFrame.Name = "buttonIconFrame";
            this.buttonIconFrame.Size = new System.Drawing.Size(117, 27);
            this.buttonIconFrame.TabIndex = 13;
            this.buttonIconFrame.Text = "Icon Frame";
            this.buttonIconFrame.UseVisualStyleBackColor = false;
            this.buttonIconFrame.Click += new System.EventHandler(this.buttonIconFrame_Click);
            // 
            // buttonTvBackground
            // 
            this.buttonTvBackground.BackColor = System.Drawing.SystemColors.Control;
            this.buttonTvBackground.FlatAppearance.BorderSize = 0;
            this.buttonTvBackground.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.buttonTvBackground.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.buttonTvBackground.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTvBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonTvBackground.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonTvBackground.Location = new System.Drawing.Point(361, 16);
            this.buttonTvBackground.Name = "buttonTvBackground";
            this.buttonTvBackground.Size = new System.Drawing.Size(117, 27);
            this.buttonTvBackground.TabIndex = 12;
            this.buttonTvBackground.Text = "TV Background";
            this.buttonTvBackground.UseVisualStyleBackColor = false;
            this.buttonTvBackground.Click += new System.EventHandler(this.buttonTvBackground_Click);
            // 
            // pictureBoxBootTv
            // 
            this.pictureBoxBootTv.BackColor = System.Drawing.Color.White;
            this.pictureBoxBootTv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBootTv.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxBootTv.Location = new System.Drawing.Point(240, 49);
            this.pictureBoxBootTv.Name = "pictureBoxBootTv";
            this.pictureBoxBootTv.Size = new System.Drawing.Size(360, 202);
            this.pictureBoxBootTv.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBootTv.TabIndex = 2;
            this.pictureBoxBootTv.TabStop = false;
            // 
            // buttonDrcBackground
            // 
            this.buttonDrcBackground.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDrcBackground.FlatAppearance.BorderSize = 0;
            this.buttonDrcBackground.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.buttonDrcBackground.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.buttonDrcBackground.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDrcBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDrcBackground.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonDrcBackground.Location = new System.Drawing.Point(482, 16);
            this.buttonDrcBackground.Name = "buttonDrcBackground";
            this.buttonDrcBackground.Size = new System.Drawing.Size(117, 27);
            this.buttonDrcBackground.TabIndex = 11;
            this.buttonDrcBackground.Text = "GamePad Background";
            this.buttonDrcBackground.UseVisualStyleBackColor = false;
            this.buttonDrcBackground.Click += new System.EventHandler(this.buttonDrcBackground_Click);
            // 
            // buttonIconBackground
            // 
            this.buttonIconBackground.BackColor = System.Drawing.SystemColors.Control;
            this.buttonIconBackground.FlatAppearance.BorderSize = 0;
            this.buttonIconBackground.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.buttonIconBackground.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.buttonIconBackground.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonIconBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonIconBackground.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonIconBackground.Location = new System.Drawing.Point(240, 16);
            this.buttonIconBackground.Name = "buttonIconBackground";
            this.buttonIconBackground.Size = new System.Drawing.Size(117, 27);
            this.buttonIconBackground.TabIndex = 10;
            this.buttonIconBackground.Text = "Icon Background";
            this.buttonIconBackground.UseVisualStyleBackColor = false;
            this.buttonIconBackground.Click += new System.EventHandler(this.buttonIconBackground_Click);
            // 
            // pictureBoxMenuIcon
            // 
            this.pictureBoxMenuIcon.BackColor = System.Drawing.Color.White;
            this.pictureBoxMenuIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxMenuIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxMenuIcon.Location = new System.Drawing.Point(124, 3);
            this.pictureBoxMenuIcon.Name = "pictureBoxMenuIcon";
            this.pictureBoxMenuIcon.Size = new System.Drawing.Size(114, 114);
            this.pictureBoxMenuIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMenuIcon.TabIndex = 5;
            this.pictureBoxMenuIcon.TabStop = false;
            // 
            // buttonIconPreview
            // 
            this.buttonIconPreview.Location = new System.Drawing.Point(4, 3);
            this.buttonIconPreview.Name = "buttonIconPreview";
            this.buttonIconPreview.Size = new System.Drawing.Size(117, 27);
            this.buttonIconPreview.TabIndex = 14;
            this.buttonIconPreview.Text = "Icon Preview";
            this.buttonIconPreview.UseVisualStyleBackColor = true;
            this.buttonIconPreview.Click += new System.EventHandler(this.buttonIconPreview_Click);
            // 
            // pictureBoxBootDrc
            // 
            this.pictureBoxBootDrc.BackColor = System.Drawing.Color.White;
            this.pictureBoxBootDrc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBootDrc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxBootDrc.Location = new System.Drawing.Point(3, 119);
            this.pictureBoxBootDrc.Name = "pictureBoxBootDrc";
            this.pictureBoxBootDrc.Size = new System.Drawing.Size(235, 132);
            this.pictureBoxBootDrc.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBootDrc.TabIndex = 6;
            this.pictureBoxBootDrc.TabStop = false;
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.Location = new System.Drawing.Point(206, 17);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(44, 13);
            this.labelId.TabIndex = 24;
            this.labelId.Text = "Title ID:";
            // 
            // labelSize
            // 
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(93, 17);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(30, 13);
            this.labelSize.TabIndex = 25;
            this.labelSize.Text = "Size:";
            // 
            // labelShortName
            // 
            this.labelShortName.AutoSize = true;
            this.labelShortName.Location = new System.Drawing.Point(13, 45);
            this.labelShortName.Name = "labelShortName";
            this.labelShortName.Size = new System.Drawing.Size(38, 13);
            this.labelShortName.TabIndex = 26;
            this.labelShortName.Text = "Name:";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Location = new System.Drawing.Point(12, 404);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(603, 95);
            this.richTextBoxLog.TabIndex = 27;
            this.richTextBoxLog.Text = "";
            // 
            // WUMInjectorGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 511);
            this.Controls.Add(this.richTextBoxLog);
            this.Controls.Add(this.labelShortName);
            this.Controls.Add(this.labelSize);
            this.Controls.Add(this.labelId);
            this.Controls.Add(this.panelImages);
            this.Controls.Add(this.panelLoadedBase);
            this.Controls.Add(this.labelBase);
            this.Controls.Add(this.buttonLoadBase);
            this.Controls.Add(this.panelCommonKey);
            this.Controls.Add(this.textBoxCommonKey);
            this.Controls.Add(this.labelCommonKey);
            this.Controls.Add(this.buttonInjectPack);
            this.Controls.Add(this.buttonInjectNotPack);
            this.Controls.Add(this.textBoxShortName);
            this.Controls.Add(this.buttonChoose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(643, 550);
            this.MinimumSize = new System.Drawing.Size(643, 550);
            this.Name = "WUMInjectorGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WUM Injector";
            this.Load += new System.EventHandler(this.WUMInjectorGUI_Load);
            this.panelImages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBootTv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMenuIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBootDrc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelLoadedBase;
        private System.Windows.Forms.Label labelBase;
        private System.Windows.Forms.Button buttonLoadBase;
        private System.Windows.Forms.Panel panelCommonKey;
        private System.Windows.Forms.TextBox textBoxCommonKey;
        private System.Windows.Forms.Label labelCommonKey;
        private System.Windows.Forms.Button buttonInjectPack;
        private System.Windows.Forms.Button buttonInjectNotPack;
        private System.Windows.Forms.TextBox textBoxShortName;
        private System.Windows.Forms.Button buttonChoose;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Panel panelImages;
        private System.Windows.Forms.Button buttonIconFrame;
        private System.Windows.Forms.Button buttonTvBackground;
        private System.Windows.Forms.Button buttonIconBackground;
        private System.Windows.Forms.PictureBox pictureBoxBootTv;
        private System.Windows.Forms.Button buttonDrcBackground;
        private System.Windows.Forms.PictureBox pictureBoxMenuIcon;
        private System.Windows.Forms.PictureBox pictureBoxBootDrc;
        private System.Windows.Forms.Button buttonDrcPreview;
        private System.Windows.Forms.Button buttonTvPreview;
        private System.Windows.Forms.Button buttonIconPreview;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.Label labelShortName;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
    }
}

