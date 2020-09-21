namespace WUMInjector
{
    partial class FFmpegWait
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelWait = new System.Windows.Forms.Label();
            this.pictureBoxWait = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).BeginInit();
            this.SuspendLayout();
            // 
            // labelWait
            // 
            this.labelWait.AutoSize = true;
            this.labelWait.Location = new System.Drawing.Point(12, 12);
            this.labelWait.Name = "labelWait";
            this.labelWait.Size = new System.Drawing.Size(160, 13);
            this.labelWait.TabIndex = 0;
            this.labelWait.Text = "Unzipping FFmpeg please wait...";
            // 
            // pictureBoxWait
            // 
            this.pictureBoxWait.Image = global::WUMInjector.Properties.Resources.Loading;
            this.pictureBoxWait.Location = new System.Drawing.Point(0, 28);
            this.pictureBoxWait.Name = "pictureBoxWait";
            this.pictureBoxWait.Size = new System.Drawing.Size(256, 90);
            this.pictureBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxWait.TabIndex = 1;
            this.pictureBoxWait.TabStop = false;
            // 
            // FFmpegWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 118);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxWait);
            this.Controls.Add(this.labelWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(256, 118);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(256, 118);
            this.Name = "FFmpegWait";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Downloading FFmpeg";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWait;
        public System.Windows.Forms.PictureBox pictureBoxWait;
    }
}