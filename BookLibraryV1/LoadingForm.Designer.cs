namespace BookLibraryV1
{
    partial class LoadingForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(49, 146);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(470, 23);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // progressLbl
            // 
            this.progressLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLbl.Location = new System.Drawing.Point(46, 222);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(473, 13);
            this.progressLbl.TabIndex = 1;
            this.progressLbl.Text = "label1";
            this.progressLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(49, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(470, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Reading files, please do not close the application";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 278);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressLbl);
            this.Controls.Add(this.progressBar1);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.MaximumSize = new System.Drawing.Size(591, 317);
            this.MinimumSize = new System.Drawing.Size(591, 317);
            this.Name = "LoadingForm";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label progressLbl;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label label1;
    }
}