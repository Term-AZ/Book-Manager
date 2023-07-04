namespace BookLibraryV1
{
    partial class SelectAuthor
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
            this.AuthorList = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.aName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.selectAuthorBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AuthorList
            // 
            this.AuthorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.aName});
            this.AuthorList.Dock = System.Windows.Forms.DockStyle.Top;
            this.AuthorList.FullRowSelect = true;
            this.AuthorList.HideSelection = false;
            this.AuthorList.Location = new System.Drawing.Point(0, 0);
            this.AuthorList.Name = "AuthorList";
            this.AuthorList.Size = new System.Drawing.Size(522, 229);
            this.AuthorList.TabIndex = 0;
            this.AuthorList.UseCompatibleStateImageBehavior = false;
            this.AuthorList.View = System.Windows.Forms.View.Details;
            this.AuthorList.SelectedIndexChanged += new System.EventHandler(this.AuthorList_SelectedIndexChanged);
            // 
            // Id
            // 
            this.Id.Text = "ID";
            this.Id.Width = 100;
            // 
            // aName
            // 
            this.aName.Text = "Author Name";
            this.aName.Width = 415;
            // 
            // selectAuthorBtn
            // 
            this.selectAuthorBtn.Location = new System.Drawing.Point(193, 235);
            this.selectAuthorBtn.Name = "selectAuthorBtn";
            this.selectAuthorBtn.Size = new System.Drawing.Size(135, 23);
            this.selectAuthorBtn.TabIndex = 1;
            this.selectAuthorBtn.Text = "Select Author";
            this.selectAuthorBtn.UseVisualStyleBackColor = true;
            this.selectAuthorBtn.Click += new System.EventHandler(this.selectAuthorBtn_Click);
            // 
            // SelectAuthor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 307);
            this.Controls.Add(this.selectAuthorBtn);
            this.Controls.Add(this.AuthorList);
            this.MaximumSize = new System.Drawing.Size(538, 346);
            this.MinimumSize = new System.Drawing.Size(538, 346);
            this.Name = "SelectAuthor";
            this.Text = "SelectAuthor";
            this.Load += new System.EventHandler(this.SelectAuthor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView AuthorList;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader aName;
        private System.Windows.Forms.Button selectAuthorBtn;
    }
}