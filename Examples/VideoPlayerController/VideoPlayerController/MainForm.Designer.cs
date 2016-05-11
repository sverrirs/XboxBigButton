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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cbUsingVLCMediaPlayer = new System.Windows.Forms.CheckBox();
            this.cbUsingNetflix = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbUsingVLCMediaPlayer
            // 
            this.cbUsingVLCMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUsingVLCMediaPlayer.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbUsingVLCMediaPlayer.Checked = true;
            this.cbUsingVLCMediaPlayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUsingVLCMediaPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUsingVLCMediaPlayer.Location = new System.Drawing.Point(12, 12);
            this.cbUsingVLCMediaPlayer.Name = "cbUsingVLCMediaPlayer";
            this.cbUsingVLCMediaPlayer.Size = new System.Drawing.Size(328, 63);
            this.cbUsingVLCMediaPlayer.TabIndex = 0;
            this.cbUsingVLCMediaPlayer.Text = "VLC Media Player";
            this.cbUsingVLCMediaPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbUsingVLCMediaPlayer.UseVisualStyleBackColor = true;
            this.cbUsingVLCMediaPlayer.CheckedChanged += new System.EventHandler(this.cbUsingVLCMediaPlayer_CheckedChanged);
            // 
            // cbUsingNetflix
            // 
            this.cbUsingNetflix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUsingNetflix.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbUsingNetflix.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUsingNetflix.Location = new System.Drawing.Point(12, 90);
            this.cbUsingNetflix.Name = "cbUsingNetflix";
            this.cbUsingNetflix.Size = new System.Drawing.Size(328, 63);
            this.cbUsingNetflix.TabIndex = 0;
            this.cbUsingNetflix.Text = "Netflix";
            this.cbUsingNetflix.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbUsingNetflix.UseVisualStyleBackColor = true;
            this.cbUsingNetflix.CheckedChanged += new System.EventHandler(this.cbUsingNetflix_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 168);
            this.Controls.Add(this.cbUsingNetflix);
            this.Controls.Add(this.cbUsingVLCMediaPlayer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "VLC/Netflix Remote";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUsingVLCMediaPlayer;
        private System.Windows.Forms.CheckBox cbUsingNetflix;
    }
}

