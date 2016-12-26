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
            this.btnCurrentPlayer = new System.Windows.Forms.Button();
            this.lblPlayerWindowStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCurrentPlayer
            // 
            this.btnCurrentPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCurrentPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCurrentPlayer.Location = new System.Drawing.Point(12, 12);
            this.btnCurrentPlayer.Name = "btnCurrentPlayer";
            this.btnCurrentPlayer.Size = new System.Drawing.Size(394, 65);
            this.btnCurrentPlayer.TabIndex = 0;
            this.btnCurrentPlayer.Text = "VLC Media Player";
            this.btnCurrentPlayer.UseVisualStyleBackColor = true;
            this.btnCurrentPlayer.Click += new System.EventHandler(this.btnCurrentPlayer_Click);
            // 
            // lblPlayerWindowStatus
            // 
            this.lblPlayerWindowStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlayerWindowStatus.Location = new System.Drawing.Point(12, 84);
            this.lblPlayerWindowStatus.Name = "lblPlayerWindowStatus";
            this.lblPlayerWindowStatus.Size = new System.Drawing.Size(394, 13);
            this.lblPlayerWindowStatus.TabIndex = 1;
            this.lblPlayerWindowStatus.Text = "Ready, click button to switch players.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 106);
            this.Controls.Add(this.lblPlayerWindowStatus);
            this.Controls.Add(this.btnCurrentPlayer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Video Player Remote";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCurrentPlayer;
        private System.Windows.Forms.Label lblPlayerWindowStatus;
    }
}

