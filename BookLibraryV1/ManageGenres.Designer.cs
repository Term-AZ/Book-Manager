namespace BookLibraryV1
{
    partial class ManageGenres
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
            this.GenreList = new System.Windows.Forms.ListView();
            this.Genres = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DeleteGenre = new System.Windows.Forms.Button();
            this.EditGenre = new System.Windows.Forms.Button();
            this.AddNewGenre = new System.Windows.Forms.Button();
            this.newGenreTextBox = new System.Windows.Forms.TextBox();
            this.SearchGenreTextBox = new System.Windows.Forms.TextBox();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GenreList
            // 
            this.GenreList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Genres});
            this.GenreList.HideSelection = false;
            this.GenreList.Location = new System.Drawing.Point(12, 12);
            this.GenreList.Name = "GenreList";
            this.GenreList.Size = new System.Drawing.Size(265, 394);
            this.GenreList.TabIndex = 0;
            this.GenreList.UseCompatibleStateImageBehavior = false;
            this.GenreList.View = System.Windows.Forms.View.Details;
            // 
            // Genres
            // 
            this.Genres.Text = "Genres";
            this.Genres.Width = 260;
            // 
            // DeleteGenre
            // 
            this.DeleteGenre.Location = new System.Drawing.Point(308, 383);
            this.DeleteGenre.Name = "DeleteGenre";
            this.DeleteGenre.Size = new System.Drawing.Size(136, 23);
            this.DeleteGenre.TabIndex = 1;
            this.DeleteGenre.Text = "Delete Selected Genre";
            this.DeleteGenre.UseVisualStyleBackColor = true;
            this.DeleteGenre.Click += new System.EventHandler(this.DeleteGenre_Click);
            // 
            // EditGenre
            // 
            this.EditGenre.Location = new System.Drawing.Point(308, 354);
            this.EditGenre.Name = "EditGenre";
            this.EditGenre.Size = new System.Drawing.Size(136, 23);
            this.EditGenre.TabIndex = 2;
            this.EditGenre.Text = "Edit Selected Genre";
            this.EditGenre.UseVisualStyleBackColor = true;
            this.EditGenre.Click += new System.EventHandler(this.EditGenre_Click);
            // 
            // AddNewGenre
            // 
            this.AddNewGenre.Location = new System.Drawing.Point(308, 325);
            this.AddNewGenre.Name = "AddNewGenre";
            this.AddNewGenre.Size = new System.Drawing.Size(136, 23);
            this.AddNewGenre.TabIndex = 3;
            this.AddNewGenre.Text = "Add New Genre";
            this.AddNewGenre.UseVisualStyleBackColor = true;
            this.AddNewGenre.Click += new System.EventHandler(this.AddNewGenre_Click);
            // 
            // newGenreTextBox
            // 
            this.newGenreTextBox.Location = new System.Drawing.Point(308, 299);
            this.newGenreTextBox.Name = "newGenreTextBox";
            this.newGenreTextBox.Size = new System.Drawing.Size(136, 20);
            this.newGenreTextBox.TabIndex = 5;
            this.newGenreTextBox.TextChanged += new System.EventHandler(this.newGenreTextBox_TextChanged);
            // 
            // SearchGenreTextBox
            // 
            this.SearchGenreTextBox.Location = new System.Drawing.Point(308, 12);
            this.SearchGenreTextBox.Name = "SearchGenreTextBox";
            this.SearchGenreTextBox.Size = new System.Drawing.Size(136, 20);
            this.SearchGenreTextBox.TabIndex = 6;
            // 
            // SearchBtn
            // 
            this.SearchBtn.Location = new System.Drawing.Point(308, 38);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(136, 23);
            this.SearchBtn.TabIndex = 7;
            this.SearchBtn.Text = "Search";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // ManageGenres
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 418);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.SearchGenreTextBox);
            this.Controls.Add(this.newGenreTextBox);
            this.Controls.Add(this.AddNewGenre);
            this.Controls.Add(this.EditGenre);
            this.Controls.Add(this.DeleteGenre);
            this.Controls.Add(this.GenreList);
            this.MaximumSize = new System.Drawing.Size(545, 457);
            this.MinimumSize = new System.Drawing.Size(545, 457);
            this.Name = "ManageGenres";
            this.Text = "ManageGenres";
            this.Load += new System.EventHandler(this.ManageGenres_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView GenreList;
        private System.Windows.Forms.Button DeleteGenre;
        private System.Windows.Forms.Button EditGenre;
        private System.Windows.Forms.Button AddNewGenre;
        private System.Windows.Forms.TextBox newGenreTextBox;
        private System.Windows.Forms.ColumnHeader Genres;
        private System.Windows.Forms.TextBox SearchGenreTextBox;
        private System.Windows.Forms.Button SearchBtn;
    }
}