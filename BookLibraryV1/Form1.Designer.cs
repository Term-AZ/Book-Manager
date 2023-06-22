namespace BookLibraryV1
{
    partial class Form1
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Title", System.Windows.Forms.HorizontalAlignment.Left);
            this.SelectFile = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            this.BookTitleTextBox = new System.Windows.Forms.TextBox();
            this.AuthorFNTextBox = new System.Windows.Forms.TextBox();
            this.AuthorMNTextBox = new System.Windows.Forms.TextBox();
            this.AuthorLNTextBox = new System.Windows.Forms.TextBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.AFNLabel = new System.Windows.Forms.Label();
            this.AMNLabel = new System.Windows.Forms.Label();
            this.ALNLabel = new System.Windows.Forms.Label();
            this.UpdateBtnBook = new System.Windows.Forms.Button();
            this.GenreLabel = new System.Windows.Forms.Label();
            this.CoverImage = new System.Windows.Forms.PictureBox();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ViewBooks = new System.Windows.Forms.TreeView();
            this.AnnotationBox = new System.Windows.Forms.RichTextBox();
            this.SaveBook = new System.Windows.Forms.Button();
            this.updateBtnAuthor = new System.Windows.Forms.Button();
            this.GenreListBox = new System.Windows.Forms.ListBox();
            this.EditGenreBtn = new System.Windows.Forms.Button();
            this.genreText = new System.Windows.Forms.TextBox();
            this.EditGenresComboBox = new System.Windows.Forms.ComboBox();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchTypeComboBox = new System.Windows.Forms.ComboBox();
            this.FailedURLs = new System.Windows.Forms.RichTextBox();
            this.SeriesNameTextBox = new System.Windows.Forms.TextBox();
            this.SeriesNumberTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.showInFileExplorer = new System.Windows.Forms.Button();
            this.ViewInBrowserBtn = new System.Windows.Forms.Button();
            this.UpdateSeriesName = new System.Windows.Forms.Button();
            this.UpdateSeriesNumBtn = new System.Windows.Forms.Button();
            this.ProgressLbl = new System.Windows.Forms.Label();
            this.updateBtn = new System.Windows.Forms.Button();
            this.ViewBooksListView = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Author = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Series = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ViewSelection = new System.Windows.Forms.ComboBox();
            this.ChangeViewBtn = new System.Windows.Forms.Button();
            this.UpdateAllBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CoverImage)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectFile
            // 
            this.SelectFile.Location = new System.Drawing.Point(270, 565);
            this.SelectFile.Name = "SelectFile";
            this.SelectFile.Size = new System.Drawing.Size(75, 23);
            this.SelectFile.TabIndex = 0;
            this.SelectFile.Text = "Select File(s)";
            this.SelectFile.UseVisualStyleBackColor = true;
            this.SelectFile.Click += new System.EventHandler(this.SelectFile_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Location = new System.Drawing.Point(351, 565);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(75, 23);
            this.AddBtn.TabIndex = 1;
            this.AddBtn.Text = "Add";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // BookTitleTextBox
            // 
            this.BookTitleTextBox.Location = new System.Drawing.Point(552, 69);
            this.BookTitleTextBox.Name = "BookTitleTextBox";
            this.BookTitleTextBox.Size = new System.Drawing.Size(237, 20);
            this.BookTitleTextBox.TabIndex = 3;
            // 
            // AuthorFNTextBox
            // 
            this.AuthorFNTextBox.Location = new System.Drawing.Point(552, 359);
            this.AuthorFNTextBox.Name = "AuthorFNTextBox";
            this.AuthorFNTextBox.Size = new System.Drawing.Size(139, 20);
            this.AuthorFNTextBox.TabIndex = 4;
            // 
            // AuthorMNTextBox
            // 
            this.AuthorMNTextBox.Location = new System.Drawing.Point(552, 400);
            this.AuthorMNTextBox.Name = "AuthorMNTextBox";
            this.AuthorMNTextBox.Size = new System.Drawing.Size(139, 20);
            this.AuthorMNTextBox.TabIndex = 5;
            // 
            // AuthorLNTextBox
            // 
            this.AuthorLNTextBox.Location = new System.Drawing.Point(552, 443);
            this.AuthorLNTextBox.Name = "AuthorLNTextBox";
            this.AuthorLNTextBox.Size = new System.Drawing.Size(139, 20);
            this.AuthorLNTextBox.TabIndex = 6;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(549, 53);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(30, 13);
            this.TitleLabel.TabIndex = 7;
            this.TitleLabel.Text = "Title:";
            // 
            // AFNLabel
            // 
            this.AFNLabel.AutoSize = true;
            this.AFNLabel.Location = new System.Drawing.Point(552, 343);
            this.AFNLabel.Name = "AFNLabel";
            this.AFNLabel.Size = new System.Drawing.Size(94, 13);
            this.AFNLabel.TabIndex = 8;
            this.AFNLabel.Text = "Author First Name:";
            // 
            // AMNLabel
            // 
            this.AMNLabel.AutoSize = true;
            this.AMNLabel.Location = new System.Drawing.Point(552, 386);
            this.AMNLabel.Name = "AMNLabel";
            this.AMNLabel.Size = new System.Drawing.Size(103, 13);
            this.AMNLabel.TabIndex = 9;
            this.AMNLabel.Text = "Author Middle Name";
            // 
            // ALNLabel
            // 
            this.ALNLabel.AutoSize = true;
            this.ALNLabel.Location = new System.Drawing.Point(552, 427);
            this.ALNLabel.Name = "ALNLabel";
            this.ALNLabel.Size = new System.Drawing.Size(92, 13);
            this.ALNLabel.TabIndex = 10;
            this.ALNLabel.Text = "Author Last Name";
            // 
            // UpdateBtnBook
            // 
            this.UpdateBtnBook.Location = new System.Drawing.Point(552, 95);
            this.UpdateBtnBook.Name = "UpdateBtnBook";
            this.UpdateBtnBook.Size = new System.Drawing.Size(237, 23);
            this.UpdateBtnBook.TabIndex = 11;
            this.UpdateBtnBook.Text = "Update";
            this.UpdateBtnBook.UseVisualStyleBackColor = true;
            this.UpdateBtnBook.Click += new System.EventHandler(this.UpdateBtnBook_Click);
            // 
            // GenreLabel
            // 
            this.GenreLabel.AutoSize = true;
            this.GenreLabel.Location = new System.Drawing.Point(555, 198);
            this.GenreLabel.Name = "GenreLabel";
            this.GenreLabel.Size = new System.Drawing.Size(41, 13);
            this.GenreLabel.TabIndex = 12;
            this.GenreLabel.Text = "Genres";
            // 
            // CoverImage
            // 
            this.CoverImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CoverImage.Location = new System.Drawing.Point(842, 12);
            this.CoverImage.Name = "CoverImage";
            this.CoverImage.Size = new System.Drawing.Size(256, 353);
            this.CoverImage.TabIndex = 14;
            this.CoverImage.TabStop = false;
            // 
            // DirectoryTextBox
            // 
            this.DirectoryTextBox.Location = new System.Drawing.Point(22, 565);
            this.DirectoryTextBox.Name = "DirectoryTextBox";
            this.DirectoryTextBox.Size = new System.Drawing.Size(242, 20);
            this.DirectoryTextBox.TabIndex = 15;
            // 
            // ViewBooks
            // 
            this.ViewBooks.Location = new System.Drawing.Point(22, 50);
            this.ViewBooks.Name = "ViewBooks";
            this.ViewBooks.Size = new System.Drawing.Size(521, 509);
            this.ViewBooks.TabIndex = 16;
            this.ViewBooks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ViewBooks_AfterSelect);
            // 
            // AnnotationBox
            // 
            this.AnnotationBox.Location = new System.Drawing.Point(842, 371);
            this.AnnotationBox.Name = "AnnotationBox";
            this.AnnotationBox.Size = new System.Drawing.Size(261, 164);
            this.AnnotationBox.TabIndex = 18;
            this.AnnotationBox.Text = "";
            // 
            // SaveBook
            // 
            this.SaveBook.Location = new System.Drawing.Point(969, 567);
            this.SaveBook.Name = "SaveBook";
            this.SaveBook.Size = new System.Drawing.Size(134, 23);
            this.SaveBook.TabIndex = 19;
            this.SaveBook.Text = "Save Book";
            this.SaveBook.UseVisualStyleBackColor = true;
            this.SaveBook.Click += new System.EventHandler(this.SaveBook_Click);
            // 
            // updateBtnAuthor
            // 
            this.updateBtnAuthor.Location = new System.Drawing.Point(552, 469);
            this.updateBtnAuthor.Name = "updateBtnAuthor";
            this.updateBtnAuthor.Size = new System.Drawing.Size(139, 23);
            this.updateBtnAuthor.TabIndex = 21;
            this.updateBtnAuthor.Text = "Update For Book";
            this.updateBtnAuthor.UseVisualStyleBackColor = true;
            this.updateBtnAuthor.Click += new System.EventHandler(this.updateBtnAuthor_Click);
            // 
            // GenreListBox
            // 
            this.GenreListBox.FormattingEnabled = true;
            this.GenreListBox.Location = new System.Drawing.Point(555, 214);
            this.GenreListBox.Name = "GenreListBox";
            this.GenreListBox.Size = new System.Drawing.Size(120, 108);
            this.GenreListBox.TabIndex = 22;
            // 
            // EditGenreBtn
            // 
            this.EditGenreBtn.Location = new System.Drawing.Point(682, 299);
            this.EditGenreBtn.Name = "EditGenreBtn";
            this.EditGenreBtn.Size = new System.Drawing.Size(140, 23);
            this.EditGenreBtn.TabIndex = 23;
            this.EditGenreBtn.Text = "Update";
            this.EditGenreBtn.UseVisualStyleBackColor = true;
            this.EditGenreBtn.Click += new System.EventHandler(this.EditGenreBtn_Click);
            // 
            // genreText
            // 
            this.genreText.Location = new System.Drawing.Point(682, 214);
            this.genreText.Name = "genreText";
            this.genreText.Size = new System.Drawing.Size(140, 20);
            this.genreText.TabIndex = 25;
            // 
            // EditGenresComboBox
            // 
            this.EditGenresComboBox.FormattingEnabled = true;
            this.EditGenresComboBox.Items.AddRange(new object[] {
            "Edit Selected Genre",
            "Delete Selected Genre",
            "Add New Genre"});
            this.EditGenresComboBox.Location = new System.Drawing.Point(682, 240);
            this.EditGenresComboBox.Name = "EditGenresComboBox";
            this.EditGenresComboBox.Size = new System.Drawing.Size(140, 21);
            this.EditGenresComboBox.TabIndex = 26;
            // 
            // SearchBtn
            // 
            this.SearchBtn.Location = new System.Drawing.Point(397, 21);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(75, 23);
            this.SearchBtn.TabIndex = 27;
            this.SearchBtn.Text = "Search";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Location = new System.Drawing.Point(22, 21);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(242, 20);
            this.SearchTextBox.TabIndex = 28;
            // 
            // SearchTypeComboBox
            // 
            this.SearchTypeComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "Search By Title",
            "Search By Author",
            "Search By Genre",
            "Search by Keywords",
            "Search By Publisher",
            "Search By Series"});
            this.SearchTypeComboBox.FormattingEnabled = true;
            this.SearchTypeComboBox.Items.AddRange(new object[] {
            "Search By Title",
            "Search By Author",
            "Search By Genre",
            "Search By Keywords",
            "Search By Publisher",
            "Search By Series"});
            this.SearchTypeComboBox.Location = new System.Drawing.Point(270, 20);
            this.SearchTypeComboBox.Name = "SearchTypeComboBox";
            this.SearchTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.SearchTypeComboBox.TabIndex = 29;
            // 
            // FailedURLs
            // 
            this.FailedURLs.Location = new System.Drawing.Point(22, 638);
            this.FailedURLs.Name = "FailedURLs";
            this.FailedURLs.Size = new System.Drawing.Size(521, 49);
            this.FailedURLs.TabIndex = 30;
            this.FailedURLs.Text = "";
            // 
            // SeriesNameTextBox
            // 
            this.SeriesNameTextBox.Location = new System.Drawing.Point(555, 147);
            this.SeriesNameTextBox.Name = "SeriesNameTextBox";
            this.SeriesNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.SeriesNameTextBox.TabIndex = 31;
            // 
            // SeriesNumberTextBox
            // 
            this.SeriesNumberTextBox.Location = new System.Drawing.Point(689, 147);
            this.SeriesNumberTextBox.Name = "SeriesNumberTextBox";
            this.SeriesNumberTextBox.Size = new System.Drawing.Size(100, 20);
            this.SeriesNumberTextBox.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(555, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Series Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(689, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Series Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(19, 614);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 18);
            this.label3.TabIndex = 35;
            this.label3.Text = "Failed to Load";
            // 
            // showInFileExplorer
            // 
            this.showInFileExplorer.Location = new System.Drawing.Point(831, 541);
            this.showInFileExplorer.Name = "showInFileExplorer";
            this.showInFileExplorer.Size = new System.Drawing.Size(272, 23);
            this.showInFileExplorer.TabIndex = 36;
            this.showInFileExplorer.Text = "Open in File Explorer";
            this.showInFileExplorer.UseVisualStyleBackColor = true;
            this.showInFileExplorer.Click += new System.EventHandler(this.showInFileExplorer_Click);
            // 
            // ViewInBrowserBtn
            // 
            this.ViewInBrowserBtn.Location = new System.Drawing.Point(831, 567);
            this.ViewInBrowserBtn.Name = "ViewInBrowserBtn";
            this.ViewInBrowserBtn.Size = new System.Drawing.Size(132, 23);
            this.ViewInBrowserBtn.TabIndex = 37;
            this.ViewInBrowserBtn.Text = "Export to Browser";
            this.ViewInBrowserBtn.UseVisualStyleBackColor = true;
            // 
            // UpdateSeriesName
            // 
            this.UpdateSeriesName.Location = new System.Drawing.Point(555, 173);
            this.UpdateSeriesName.Name = "UpdateSeriesName";
            this.UpdateSeriesName.Size = new System.Drawing.Size(100, 23);
            this.UpdateSeriesName.TabIndex = 38;
            this.UpdateSeriesName.Text = "Update";
            this.UpdateSeriesName.UseVisualStyleBackColor = true;
            this.UpdateSeriesName.Click += new System.EventHandler(this.UpdateSeriesName_Click);
            // 
            // UpdateSeriesNumBtn
            // 
            this.UpdateSeriesNumBtn.Location = new System.Drawing.Point(689, 172);
            this.UpdateSeriesNumBtn.Name = "UpdateSeriesNumBtn";
            this.UpdateSeriesNumBtn.Size = new System.Drawing.Size(100, 23);
            this.UpdateSeriesNumBtn.TabIndex = 40;
            this.UpdateSeriesNumBtn.Text = "Update";
            this.UpdateSeriesNumBtn.UseVisualStyleBackColor = true;
            this.UpdateSeriesNumBtn.Click += new System.EventHandler(this.UpdateSeriesNumBtn_Click);
            // 
            // ProgressLbl
            // 
            this.ProgressLbl.AutoSize = true;
            this.ProgressLbl.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressLbl.Location = new System.Drawing.Point(128, 588);
            this.ProgressLbl.Name = "ProgressLbl";
            this.ProgressLbl.Size = new System.Drawing.Size(35, 13);
            this.ProgressLbl.TabIndex = 41;
            this.ProgressLbl.Text = "label4";
            // 
            // updateBtn
            // 
            this.updateBtn.Location = new System.Drawing.Point(432, 565);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(111, 23);
            this.updateBtn.TabIndex = 42;
            this.updateBtn.Text = "Update View";
            this.updateBtn.UseVisualStyleBackColor = true;
            this.updateBtn.Click += new System.EventHandler(this.updateBtn_Click);
            // 
            // ViewBooksListView
            // 
            this.ViewBooksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Title,
            this.Author,
            this.Series});
            listViewGroup1.Header = "Title";
            listViewGroup1.Name = "Title";
            this.ViewBooksListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
            this.ViewBooksListView.HideSelection = false;
            this.ViewBooksListView.Location = new System.Drawing.Point(22, 50);
            this.ViewBooksListView.Name = "ViewBooksListView";
            this.ViewBooksListView.Size = new System.Drawing.Size(521, 509);
            this.ViewBooksListView.TabIndex = 44;
            this.ViewBooksListView.UseCompatibleStateImageBehavior = false;
            this.ViewBooksListView.View = System.Windows.Forms.View.Details;
            this.ViewBooksListView.SelectedIndexChanged += new System.EventHandler(this.ViewBooksListView_SelectedIndexChanged);
            // 
            // Id
            // 
            this.Id.Text = "Id";
            this.Id.Width = 50;
            // 
            // Title
            // 
            this.Title.Text = "Title";
            this.Title.Width = 150;
            // 
            // Author
            // 
            this.Author.Text = "Author";
            this.Author.Width = 180;
            // 
            // Series
            // 
            this.Series.Text = "Series";
            this.Series.Width = 150;
            // 
            // ViewSelection
            // 
            this.ViewSelection.FormattingEnabled = true;
            this.ViewSelection.Items.AddRange(new object[] {
            "Tree View",
            "List View"});
            this.ViewSelection.Location = new System.Drawing.Point(270, 611);
            this.ViewSelection.Name = "ViewSelection";
            this.ViewSelection.Size = new System.Drawing.Size(156, 21);
            this.ViewSelection.TabIndex = 45;
            // 
            // ChangeViewBtn
            // 
            this.ChangeViewBtn.Location = new System.Drawing.Point(432, 609);
            this.ChangeViewBtn.Name = "ChangeViewBtn";
            this.ChangeViewBtn.Size = new System.Drawing.Size(111, 23);
            this.ChangeViewBtn.TabIndex = 46;
            this.ChangeViewBtn.Text = "Change View";
            this.ChangeViewBtn.UseVisualStyleBackColor = true;
            this.ChangeViewBtn.Click += new System.EventHandler(this.ChangeViewBtn_Click);
            // 
            // UpdateAllBtn
            // 
            this.UpdateAllBtn.Location = new System.Drawing.Point(552, 498);
            this.UpdateAllBtn.Name = "UpdateAllBtn";
            this.UpdateAllBtn.Size = new System.Drawing.Size(139, 23);
            this.UpdateAllBtn.TabIndex = 47;
            this.UpdateAllBtn.Text = "Update For All";
            this.UpdateAllBtn.UseVisualStyleBackColor = true;
            this.UpdateAllBtn.Click += new System.EventHandler(this.UpdateAllBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 700);
            this.Controls.Add(this.UpdateAllBtn);
            this.Controls.Add(this.ChangeViewBtn);
            this.Controls.Add(this.ViewSelection);
            this.Controls.Add(this.ViewBooksListView);
            this.Controls.Add(this.updateBtn);
            this.Controls.Add(this.ProgressLbl);
            this.Controls.Add(this.UpdateSeriesNumBtn);
            this.Controls.Add(this.UpdateSeriesName);
            this.Controls.Add(this.ViewInBrowserBtn);
            this.Controls.Add(this.showInFileExplorer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SeriesNumberTextBox);
            this.Controls.Add(this.SeriesNameTextBox);
            this.Controls.Add(this.FailedURLs);
            this.Controls.Add(this.SearchTypeComboBox);
            this.Controls.Add(this.SearchTextBox);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.EditGenresComboBox);
            this.Controls.Add(this.genreText);
            this.Controls.Add(this.EditGenreBtn);
            this.Controls.Add(this.GenreListBox);
            this.Controls.Add(this.updateBtnAuthor);
            this.Controls.Add(this.SaveBook);
            this.Controls.Add(this.AnnotationBox);
            this.Controls.Add(this.ViewBooks);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.CoverImage);
            this.Controls.Add(this.GenreLabel);
            this.Controls.Add(this.UpdateBtnBook);
            this.Controls.Add(this.ALNLabel);
            this.Controls.Add(this.AMNLabel);
            this.Controls.Add(this.AFNLabel);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.AuthorLNTextBox);
            this.Controls.Add(this.AuthorMNTextBox);
            this.Controls.Add(this.AuthorFNTextBox);
            this.Controls.Add(this.BookTitleTextBox);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.SelectFile);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.CoverImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectFile;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.TextBox BookTitleTextBox;
        private System.Windows.Forms.TextBox AuthorFNTextBox;
        private System.Windows.Forms.TextBox AuthorMNTextBox;
        private System.Windows.Forms.TextBox AuthorLNTextBox;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label AFNLabel;
        private System.Windows.Forms.Label AMNLabel;
        private System.Windows.Forms.Label ALNLabel;
        private System.Windows.Forms.Button UpdateBtnBook;
        private System.Windows.Forms.Label GenreLabel;
        private System.Windows.Forms.PictureBox CoverImage;
        public System.Windows.Forms.TextBox DirectoryTextBox;
        private System.Windows.Forms.TreeView ViewBooks;
        private System.Windows.Forms.RichTextBox AnnotationBox;
        private System.Windows.Forms.Button SaveBook;
        private System.Windows.Forms.Button updateBtnAuthor;
        private System.Windows.Forms.ListBox GenreListBox;
        private System.Windows.Forms.Button EditGenreBtn;
        private System.Windows.Forms.TextBox genreText;
        private System.Windows.Forms.ComboBox EditGenresComboBox;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.ComboBox SearchTypeComboBox;
        private System.Windows.Forms.RichTextBox FailedURLs;
        private System.Windows.Forms.TextBox SeriesNameTextBox;
        private System.Windows.Forms.TextBox SeriesNumberTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button showInFileExplorer;
        private System.Windows.Forms.Button ViewInBrowserBtn;
        private System.Windows.Forms.Button UpdateSeriesName;
        private System.Windows.Forms.Button UpdateSeriesNumBtn;
        public System.Windows.Forms.Label ProgressLbl;
        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.ListView ViewBooksListView;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.ColumnHeader Author;
        private System.Windows.Forms.ColumnHeader Series;
        private System.Windows.Forms.ComboBox ViewSelection;
        private System.Windows.Forms.Button ChangeViewBtn;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.Button UpdateAllBtn;
    }
}

