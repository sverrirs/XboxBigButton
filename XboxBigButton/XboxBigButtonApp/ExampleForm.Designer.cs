namespace XboxBigButtonApp
{
    partial class ExampleForm
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
            this.btnControllerGreen = new System.Windows.Forms.Button();
            this.btnControllerRed = new System.Windows.Forms.Button();
            this.btnControllerBlue = new System.Windows.Forms.Button();
            this.btnControllerYellow = new System.Windows.Forms.Button();
            this.tbControllerGreen = new System.Windows.Forms.TextBox();
            this.tbControllerRed = new System.Windows.Forms.TextBox();
            this.tbControllerBlue = new System.Windows.Forms.TextBox();
            this.tbControllerYellow = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnControllerGreen
            // 
            this.btnControllerGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnControllerGreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControllerGreen.Location = new System.Drawing.Point(12, 12);
            this.btnControllerGreen.Name = "btnControllerGreen";
            this.btnControllerGreen.Size = new System.Drawing.Size(33, 33);
            this.btnControllerGreen.TabIndex = 0;
            this.btnControllerGreen.Text = "G";
            this.btnControllerGreen.UseVisualStyleBackColor = false;
            // 
            // btnControllerRed
            // 
            this.btnControllerRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnControllerRed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControllerRed.Location = new System.Drawing.Point(220, 12);
            this.btnControllerRed.Name = "btnControllerRed";
            this.btnControllerRed.Size = new System.Drawing.Size(33, 33);
            this.btnControllerRed.TabIndex = 0;
            this.btnControllerRed.Text = "R";
            this.btnControllerRed.UseVisualStyleBackColor = false;
            // 
            // btnControllerBlue
            // 
            this.btnControllerBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnControllerBlue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControllerBlue.Location = new System.Drawing.Point(428, 12);
            this.btnControllerBlue.Name = "btnControllerBlue";
            this.btnControllerBlue.Size = new System.Drawing.Size(33, 33);
            this.btnControllerBlue.TabIndex = 0;
            this.btnControllerBlue.Text = "B";
            this.btnControllerBlue.UseVisualStyleBackColor = false;
            // 
            // btnControllerYellow
            // 
            this.btnControllerYellow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnControllerYellow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControllerYellow.Location = new System.Drawing.Point(636, 12);
            this.btnControllerYellow.Name = "btnControllerYellow";
            this.btnControllerYellow.Size = new System.Drawing.Size(33, 33);
            this.btnControllerYellow.TabIndex = 0;
            this.btnControllerYellow.Text = "Y";
            this.btnControllerYellow.UseVisualStyleBackColor = false;
            // 
            // tbControllerGreen
            // 
            this.tbControllerGreen.Location = new System.Drawing.Point(13, 52);
            this.tbControllerGreen.Multiline = true;
            this.tbControllerGreen.Name = "tbControllerGreen";
            this.tbControllerGreen.Size = new System.Drawing.Size(194, 478);
            this.tbControllerGreen.TabIndex = 1;
            // 
            // tbControllerRed
            // 
            this.tbControllerRed.Location = new System.Drawing.Point(220, 51);
            this.tbControllerRed.Multiline = true;
            this.tbControllerRed.Name = "tbControllerRed";
            this.tbControllerRed.Size = new System.Drawing.Size(194, 478);
            this.tbControllerRed.TabIndex = 1;
            // 
            // tbControllerBlue
            // 
            this.tbControllerBlue.Location = new System.Drawing.Point(428, 52);
            this.tbControllerBlue.Multiline = true;
            this.tbControllerBlue.Name = "tbControllerBlue";
            this.tbControllerBlue.Size = new System.Drawing.Size(194, 478);
            this.tbControllerBlue.TabIndex = 1;
            // 
            // tbControllerYellow
            // 
            this.tbControllerYellow.Location = new System.Drawing.Point(636, 51);
            this.tbControllerYellow.Multiline = true;
            this.tbControllerYellow.Name = "tbControllerYellow";
            this.tbControllerYellow.Size = new System.Drawing.Size(194, 478);
            this.tbControllerYellow.TabIndex = 1;
            // 
            // ExampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 544);
            this.Controls.Add(this.tbControllerYellow);
            this.Controls.Add(this.tbControllerBlue);
            this.Controls.Add(this.tbControllerRed);
            this.Controls.Add(this.tbControllerGreen);
            this.Controls.Add(this.btnControllerYellow);
            this.Controls.Add(this.btnControllerBlue);
            this.Controls.Add(this.btnControllerRed);
            this.Controls.Add(this.btnControllerGreen);
            this.Name = "ExampleForm";
            this.Text = "ExampleForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnControllerGreen;
        private System.Windows.Forms.Button btnControllerRed;
        private System.Windows.Forms.Button btnControllerBlue;
        private System.Windows.Forms.Button btnControllerYellow;
        private System.Windows.Forms.TextBox tbControllerGreen;
        private System.Windows.Forms.TextBox tbControllerRed;
        private System.Windows.Forms.TextBox tbControllerBlue;
        private System.Windows.Forms.TextBox tbControllerYellow;
    }
}

