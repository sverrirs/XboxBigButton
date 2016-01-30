namespace VideoPlayerController
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbUsingVLCMediaPlayer = new System.Windows.Forms.CheckBox();
            this.cbUsingNetflix = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbUsingVLCMediaPlayer
            // 
            this.cbUsingVLCMediaPlayer.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbUsingVLCMediaPlayer.Checked = true;
            this.cbUsingVLCMediaPlayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUsingVLCMediaPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUsingVLCMediaPlayer.Location = new System.Drawing.Point(12, 12);
            this.cbUsingVLCMediaPlayer.Name = "cbUsingVLCMediaPlayer";
            this.cbUsingVLCMediaPlayer.Size = new System.Drawing.Size(241, 63);
            this.cbUsingVLCMediaPlayer.TabIndex = 0;
            this.cbUsingVLCMediaPlayer.Text = "VLC Media Player";
            this.cbUsingVLCMediaPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbUsingVLCMediaPlayer.UseVisualStyleBackColor = true;
            this.cbUsingVLCMediaPlayer.CheckedChanged += new System.EventHandler(this.cbUsingVLCMediaPlayer_CheckedChanged);
            // 
            // cbUsingNetflix
            // 
            this.cbUsingNetflix.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbUsingNetflix.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUsingNetflix.Location = new System.Drawing.Point(12, 90);
            this.cbUsingNetflix.Name = "cbUsingNetflix";
            this.cbUsingNetflix.Size = new System.Drawing.Size(241, 63);
            this.cbUsingNetflix.TabIndex = 0;
            this.cbUsingNetflix.Text = "Netflix";
            this.cbUsingNetflix.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbUsingNetflix.UseVisualStyleBackColor = true;
            this.cbUsingNetflix.CheckedChanged += new System.EventHandler(this.cbUsingNetflix_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(260, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(72, 62);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 168);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cbUsingNetflix);
            this.Controls.Add(this.cbUsingVLCMediaPlayer);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUsingVLCMediaPlayer;
        private System.Windows.Forms.CheckBox cbUsingNetflix;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

