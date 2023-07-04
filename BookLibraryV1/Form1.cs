using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Compression;
using System.ComponentModel.Design.Serialization;

namespace BookLibraryV1
{
    public partial class Form1 : Form
    {
        AuthorTableAccessor authorTableAccessor;
        BookTableAccessor bookTableAccessor;
        FileReader fileReader;
        GenreTableAccessor genreTableAccessor;
        ImageTableAccessor imageTableAccessor;
        SQLiteConnection connection;
        Search searcher;
        private String URL = "";
        public List<string> files = new List<string>();
        public List<string> booksTitles = new List<string>();
        TreeNode mainNode;
        TreeNode altMainNode;
        TreeNode savedNode;

        String bookId;
        String authorId;
        List<String> authorDetails;
        Dictionary<String, String> bookDetails;

        bool genreUpdated = false;

        List<Rectangle> rectangleList = new List<Rectangle>();
        List<Control> controlList = new List<Control>();

        Rectangle originalFormSize;
        Rectangle selectFileBtn;
        Rectangle addFileBtn;
        Rectangle updateViewBtn;
        Rectangle changeViewBtn;
        Rectangle resetTablesBtn;
        Rectangle addSelectedGenreBtn;
        Rectangle deleteSelectedGenreBtn;
        Rectangle manageGeneresBtn;
        Rectangle updateBookBtn;
        Rectangle openInFileExplorerBtn;
        Rectangle saveBookBtn;
        Rectangle changeImageBtn;
        Rectangle searchBtn;
        Rectangle deleteBtn;

        Rectangle ListViewRectangle;
        Rectangle TreeViewRectangle;
        Rectangle GenreView;

        Rectangle failedToLoadBox;
        Rectangle annotationBox;

        Rectangle searchBox;
        Rectangle directoryTextBox;
        Rectangle bookTitleTextBox;
        Rectangle authorFNTextBox;
        Rectangle authorMNTextBox;
        Rectangle authorLNTextBox;
        Rectangle authorTableId;
        Rectangle authorBookId;
        Rectangle seriesName;
        Rectangle seriesNumber;

        Rectangle coverImageBox;

        Rectangle searchDropDown;
        Rectangle viewDropDown;
        Rectangle genreDropDown;

        Rectangle rLabel1;
        Rectangle rLabel2;
        Rectangle rLabel3;
        Rectangle rLabel4;
        Rectangle rLabel5;
        Rectangle rLabel6;
        Rectangle rLabel7;
        Rectangle rLabel8;
        Rectangle rLabel9;
        Rectangle rLabel10;

        Image image;
        public Form1()
        {
            InitializeComponent();
            this.Text = "Book Manager";
            ViewBooksListView.FullRowSelect = true;
            ViewBooksListView.Visible = false;
            ListOfGenresComboBox.Enabled = false;
            EditGenreBtn.Enabled = false;
            DeleteSelectedGenreBtn.Enabled = false;
            try
            {
                string workingDirectory = Environment.CurrentDirectory;
                URL = Directory.GetParent(workingDirectory).Parent.Parent.FullName;


                if (!System.IO.File.Exists($"{URL}\\Library.db"))
                {
                    SQLiteConnection.CreateFile($"{URL}\\Library.db");
                }
                connection = new SQLiteConnection($"data source = {URL}\\Library.db");
                connection.Open();
                imageTableAccessor = new ImageTableAccessor(this, connection, URL);
                authorTableAccessor = new AuthorTableAccessor(this, connection);
                bookTableAccessor = new BookTableAccessor(this, connection, authorTableAccessor);
                genreTableAccessor = new GenreTableAccessor(this, URL, connection);
                fileReader = new FileReader(this, authorTableAccessor, bookTableAccessor, genreTableAccessor, imageTableAccessor);
                searcher = new Search();

                //altMainNode = new TreeNode();
                //altMainNode.Text = "Books Without Authors";
                //altMainNode.Name = "Authorless";
                mainNode = new TreeNode();
                mainNode.Name = "main";
                mainNode.Text = "Authors";
                mainNode.ExpandAll();
                this.ViewBooks.Nodes.Add(mainNode);
                //this.ViewBooks.Nodes.Add(altMainNode);


                CoverImage.SizeMode = PictureBoxSizeMode.StretchImage;
                SearchTypeComboBox.SelectedIndex = 0;
                SearchTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                ViewSelection.SelectedIndex = 0;

                createTables();

                populateGenres();
                populateView("default1");
            }
            catch (Exception ex)
            {
                ErrorForm er = new ErrorForm($"Unable to access and/or create database: {ex}");
                er.ShowDialog();
            }


        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            files.Clear();
            booksTitles.Clear();
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    files = Directory.GetFileSystemEntries(fbd.SelectedPath, "*", SearchOption.AllDirectories).ToList();
                    allBooks(files);
                    DirectoryTextBox.Text = $"In files: {files.Count}      In BooksTitles: {booksTitles.Count}";
                    //DirectoryTextBox.Text = $"Files found: {booksTitles.Count()}";
                }
                else
                {
                    DirectoryTextBox.Text = "No files found";
                }
            }
        }
        private void allBooks(List<String> l)
        {
            foreach(String f in l)
            {
                FileInfo fi = new FileInfo(f);
                FileAttributes fa = File.GetAttributes(f);
/*                if (fa != FileAttributes.Directory)
                {
                    booksTitles.Add(f);
                    //allBooks(Directory.GetFiles(f).ToList()); TO DO IT RECURSIVELY, METHOD GetFileSystemEntries gets all files and directories already, not needed to search through each folder
                }
                else if((fa & FileAttributes.Compressed) == FileAttributes.Compressed)
                {
                    DirectoryTextBox.Text = "ZIP!!!!!";
                }*/

                if(fi.Extension == ".fb2")
                {
                    booksTitles.Add(f);
                }
                else if(fi.Extension == ".zip")
                {
                    booksTitles.Add(f);
                }

            }
            return; 
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (booksTitles.Count ==0)
            {
                return;
            }
            fileReader.populateTables(booksTitles);
            populateView("default1");
        }
        private void createTables()
        {
            authorTableAccessor.createTable();
            bookTableAccessor.createTable();
            genreTableAccessor.createGenreTable();
            genreTableAccessor.populateGenreTable();
            imageTableAccessor.createTable();
        }
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                populateView("default");  
                return;
            }
            if (ViewBooks.Visible == true)
            {
                Dictionary<String, TreeNode> authors = new Dictionary<string, TreeNode>();
                mainNode.Nodes.Clear();
                //altMainNode.Nodes.Clear();
                switch (SearchTypeComboBox.Items[SearchTypeComboBox.SelectedIndex].ToString())
                {
                    case "Search By Title":
                        mainNode.Text = "Titles";
                        populateTreeViewByBook(bookTableAccessor.searchBooksByTitle(SearchTextBox.Text));
                        break;
                    case "Search By Author":
                        mainNode.Text = "Authors";
                        List<String> s = new List<String>();
                        authors = authorTableAccessor.getAuthorsByNameTree(SearchTextBox.Text);
                        foreach (KeyValuePair<String, TreeNode> t in authors)
                        {
                            s.Add(t.Key);
                        }
                        populateTreeViewByThreeLayers(authors, bookTableAccessor.searchByAuthorIdTree(s));
                        break;
                    case "Search By Genre":
                        mainNode.Text = "Authors";
                        populateTreeViewByBook(bookTableAccessor.searchByGenreTree(SearchTextBox.Text));
                        break;
                    case "Search By Keywords":
                        mainNode.Text = "Books";
                        populateTreeViewByBook(bookTableAccessor.searchByKeywordsTree(SearchTextBox.Text));
                        break;
                    case "Search By Publisher":
                        mainNode.Text = "Books";
                        populateTreeViewByBook(bookTableAccessor.searchByPublisherTree(SearchTextBox.Text));
                        break;
                    case "Search By Series":
                        mainNode.Text = "Books";
                        populateTreeViewByBook(bookTableAccessor.searchBySeriesTree(SearchTextBox.Text));
                        break;
                }
            }
            else
            {
                switch (SearchTypeComboBox.Items[SearchTypeComboBox.SelectedIndex].ToString())
                {
                    case "Search By Title":
                        populateListViewByBook(bookTableAccessor.searchByTitleList(SearchTextBox.Text));  
                        break;
                    case "Search By Author":
                        populateListViewByBook(bookTableAccessor.searchByAuthorList(authorTableAccessor.getAuthorByNameList(SearchTextBox.Text)));
                        break;
                    case "Search By Genre":
                        populateListViewByBook(bookTableAccessor.searchByGenreList(SearchTextBox.Text));
                        break;
                    case "Search By Keywords":
                        populateListViewByBook(bookTableAccessor.searchByKeywordsList(SearchTextBox.Text));
                        break;
                    case "Search By Publisher":
                        populateListViewByBook(bookTableAccessor.searchByPublisherList(SearchTextBox.Text));
                        break;
                    case "Search By Series":
                        populateListViewByBook(bookTableAccessor.searchBySeriesList(SearchTextBox.Text));
                        break;
                }
            }    
        }

        private void populateView(String type)
        {
            mainNode.Nodes.Clear();
            //altMainNode.Nodes.Clear();
            switch (type)
            {
                case "default":
                    if (ViewBooks.Visible == true)
                    {
                        mainNode.Text = "Authors";
                        populateTreeViewByThreeLayers(authorTableAccessor.getAuthors(), bookTableAccessor.getBooksTree());
                    }
                    else
                    {
                        populateListViewByBook(bookTableAccessor.getBooksList());
                    }
                    break;
                case "default1":
                    mainNode.Text = "Authors";
                    populateTreeViewByThreeLayers(authorTableAccessor.getAuthors(), bookTableAccessor.getBooksTree());
                    populateListViewByBook(bookTableAccessor.getBooksList());
                    break;
                case "reset":
                    if (ViewBooks.Visible == true)
                    {
                            populateListViewByBook(bookTableAccessor.getBooksList());
                    }
                    else
                    {
                        mainNode.Text = "Authors";
                        populateTreeViewByThreeLayers(authorTableAccessor.getAuthors(), bookTableAccessor.getBooksTree());
                    }
                    break;
                case "tree":
                    mainNode.Text = "Authors";
                    populateTreeViewByThreeLayers(authorTableAccessor.getAuthors(), bookTableAccessor.getBooksTree());
                    break;
            }
        }
        /// <summary>
        /// Used for searches that will not result in the base tree structure
        /// </summary>
        /// <param name="l"></param>
        private void populateTreeViewByBook(List<TreeNode> l)
        {
            mainNode.Nodes.Clear();
            //altMainNode.Nodes.Clear();
            foreach (TreeNode t in l)
            {
                if (t.Tag == "-1")
                {
                    altMainNode.Nodes.Add(t);
                    continue;
                }
                mainNode.Nodes.Add(t);
            }
            mainNode.Expand();
        }
        private void populateTreeViewByBook(Dictionary<String, List<TreeNode>> l)
        {
            foreach(KeyValuePair<String,List<TreeNode>> i in l)
            {
                foreach(TreeNode t in i.Value)
                {
/*                    if(t.Tag == "-1")
                    {
                        altMainNode.Nodes.Add(t);
                        continue;
                    }*/
                    if (t.Nodes.Count==0)
                    {
                        mainNode.Nodes.Insert(mainNode.Nodes.Count, t);
                        continue;
                    }
                    mainNode.Nodes.Insert(0,t);
                }
            }
            mainNode.Expand();
        }
        private void populateListViewByBook(List<Dictionary<String,String>> books)
        {
            ViewBooksListView.Items.Clear();
            foreach (Dictionary<String, String> entry in books)
            {
                ViewBooksListView.Items.Add(new ListViewItem(new String[] { entry["Id"], entry["Title"], entry["Author"], entry["Series"] }));
            }
        }

        private void populateTreeViewByThreeLayers(Dictionary<String, TreeNode> a, Dictionary<String, List<TreeNode>> b)
        {
            foreach (KeyValuePair<String, List<TreeNode>> pair in b)
            {
                if (!a.ContainsKey(pair.Key))
                {
                    continue;
                }
                foreach (TreeNode d in pair.Value)
                {
/*                    if (d.Tag.ToString() == "")
                    {
                        altMainNode.Nodes.Add(d);
                        continue;
                    }*/
                    a[pair.Key].Nodes.Add(d);
                }
            }

            foreach (KeyValuePair<string, TreeNode> entry in a)
            {
                if (entry.Value.LastNode == null)
                {
                    continue;
                }
                mainNode.Nodes.Add(entry.Value);
            }
            mainNode.Expand();
        }

        private void populateGenreBox(String genres)
        {
            List<String> genre = genres.Split(' ').ToList();
            GenreListBox.DataSource = genre;

        }
        private Image createCover(byte[] s)
        {
            using (var ms = new MemoryStream(s))
            {
                return Image.FromStream(ms);
            }
        }
        private void SaveBook_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if(ViewBooks.Visible == true)
                    {
                        fileReader.editBook(new List<String>{ ViewBooks.SelectedNode.Name.Trim()}, fbd.SelectedPath);
                    }
                    else
                    {
                        if (ViewBooksListView.SelectedItems.Count == 0)
                        {
                            return;
                        }
                        var item = ViewBooksListView.SelectedItems[0];
                        List<String> s = new List<String>();
                        for(int i =0;i< ViewBooksListView.SelectedItems.Count; i++)
                        {
                            s.Add(ViewBooksListView.SelectedItems[i].SubItems[0].Text);
                        }

                        fileReader.editBook(s, fbd.SelectedPath);
                    }
                    
                    DirectoryTextBox.Text = "File Saved";
                }
                //fileReader.editBook(ViewBooks.SelectedNode.Name.Trim(), saveFileDialog.FileName);
            }
        }
        private void showInFileExplorer_Click(object sender, EventArgs e)
        {
            if (ViewBooks.SelectedNode != null)
            {
                if (ViewBooks.SelectedNode.Nodes.Count == 0)
                {
                    String folderPath = bookTableAccessor.getDirectory(ViewBooks.SelectedNode.Name);
                    DirectoryTextBox.Text = $"In file explorer: url is: {folderPath}";
                    if (folderPath != null)
                    {
                        Process.Start("explorer.exe", "/select, \"" + folderPath + "\"");
                    }
                }
            }
        }
        private void updateBtn_Click(object sender, EventArgs e)
        {
            populateView("default1");
        }

        private void ChangeViewBtn_Click(object sender, EventArgs e)
        {
            if (ViewSelection.Items[ViewSelection.SelectedIndex].ToString() == "Tree View")
            {
                ViewBooks.Visible = true;
                ViewBooksListView.Visible = false;
            }
            else
            {
                ViewBooks.Visible = false;
                ViewBooksListView.Visible = true;
            }
            populateView("default1");
        }

        private void ViewBooksListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ViewBooksListView.SelectedItems.Count == 0)
            {
                return;
            }
            var item = ViewBooksListView.SelectedItems[0];
            bookId = item.SubItems[0].Text;
            var bookDetails = bookTableAccessor.getBook(bookId);
            var authorDetails = authorTableAccessor.getAuthor(bookDetails["AuthorId"]);
            ListOfGenresComboBox.Enabled = true;
            populateInfo(authorDetails, bookDetails);
            BookTitleTextBox.Tag = BookTitleTextBox.Text;
            SeriesNameTextBox.Tag = SeriesNameTextBox.Text;
            SeriesNumberTextBox.Tag = SeriesNumberTextBox.Text;
            AuthorFNTextBox.Tag = AuthorFNTextBox.Text;
            AuthorMNTextBox.Tag = AuthorMNTextBox.Text;
            AuthorLNTextBox.Tag = AuthorLNTextBox.Text;

        }
        private void ViewBooks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (ViewBooks.SelectedNode.Tag)
            {
                case "Author":
                    populateInfo(authorTableAccessor.getAuthor(ViewBooks.SelectedNode.Name.ToString()));
                    EditGenreBtn.Enabled = false;
                    UploadImageBtn.Enabled = false;
                    ListOfGenresComboBox.Enabled = false;
                    EditGenreBtn.Enabled = false;
                    DeleteSelectedGenreBtn.Enabled = false;
                    break;
                case "Series":
                    populateInfo(authorTableAccessor.getAuthor(ViewBooks.SelectedNode.Parent.Name.ToString()),ViewBooks.SelectedNode.Text);
                    EditGenreBtn.Enabled = false;
                    UploadImageBtn.Enabled = false;
                    ListOfGenresComboBox.Enabled = false;
                    EditGenreBtn.Enabled = false;
                    DeleteSelectedGenreBtn.Enabled = false;
                    break;
                default:
                    if (ViewBooks.SelectedNode.FirstNode == null && ViewBooks.SelectedNode.Parent != null)
                    {
                        bookId = ViewBooks.SelectedNode.Name.Trim();
                        authorDetails = authorTableAccessor.getAuthor(ViewBooks.SelectedNode.Tag.ToString());
                        bookDetails = bookTableAccessor.getBook(bookId);
                        populateInfo(authorDetails, bookDetails);
                        EditGenreBtn.Enabled = true;
                        UploadImageBtn.Enabled = true;
                        ListOfGenresComboBox.Enabled = true;
                        EditGenreBtn.Enabled = true;
                        DeleteSelectedGenreBtn.Enabled = true;
                    }
                    break;
            }
            BookTitleTextBox.Tag = BookTitleTextBox.Text;
            SeriesNameTextBox.Tag = SeriesNameTextBox.Text;
            SeriesNumberTextBox.Tag = SeriesNumberTextBox.Text;
            AuthorFNTextBox.Tag = AuthorFNTextBox.Text;
            AuthorMNTextBox.Tag = AuthorMNTextBox.Text;
            AuthorLNTextBox.Tag = AuthorLNTextBox.Text;
        }
        private void populateInfo(List<String> authorDetails, Dictionary<String,String> bookDetails)
        {
            try
            {
                BookTitleTextBox.Text = bookDetails["Title"];
                AuthorFNTextBox.Text = authorDetails.ElementAt(0);
                AuthorMNTextBox.Text = authorDetails.ElementAt(1);
                AuthorLNTextBox.Text = authorDetails.ElementAt(2);
                authorTableIdTextBox.Text = authorDetails.ElementAt(3);
                authorIdTextBox.Text = authorDetails.ElementAt(4);
                AnnotationBox.Text = bookDetails["Annotation"];
            }
            catch (Exception a)
            {
                AuthorFNTextBox.Text = "";
                AuthorMNTextBox.Text = "";
                AuthorLNTextBox.Text = "";
                AnnotationBox.Text = "";
                
            }
            try { CoverImage.Image = createCover(imageTableAccessor.getCover(bookDetails["ImageId"])); } catch { }
            

            SeriesNameTextBox.Text = bookDetails["Series"];
            SeriesNumberTextBox.Text = bookDetails["SeriesNum"];
            populateGenreBox(bookDetails["Genre"]);
           
        }
        private void populateInfo(List<String> authorDetails)
        {
            AuthorFNTextBox.Text = authorDetails.ElementAt(0);
            AuthorMNTextBox.Text = authorDetails.ElementAt(1);
            AuthorLNTextBox.Text = authorDetails.ElementAt(2);
            authorTableIdTextBox.Text = authorDetails.ElementAt(3);
            authorIdTextBox.Text = authorDetails.ElementAt(4);
            SeriesNumberTextBox.Text = "";
            AnnotationBox.Text = "";
            BookTitleTextBox.Text = "";
            GenreListBox.DataSource = new List<String>();
            CoverImage.Image = null;
        }
        private void populateInfo(List<String> authorDetails, String sName)
        {
            AuthorFNTextBox.Text = authorDetails.ElementAt(0);
            AuthorMNTextBox.Text = authorDetails.ElementAt(1);
            AuthorLNTextBox.Text = authorDetails.ElementAt(2);
            authorTableIdTextBox.Text = authorDetails.ElementAt(3);
            authorIdTextBox.Text = authorDetails.ElementAt(4);
            SeriesNameTextBox.Text = sName;
            SeriesNumberTextBox.Text = "";
            AnnotationBox.Text = "";
            BookTitleTextBox.Text = "";
            GenreListBox.DataSource = new List<String>();
            CoverImage.Image = null;
        }

        private void UpdateAllBtn_Click(object sender, EventArgs e)
        {
/*            var item = ViewBooksListView.SelectedItems[0];
            var name = item.SubItems[2].Text;
            authorTableAccessor.updateAuthor(bookTableAccessor.getAuthorId(item.SubItems[0].Text), AuthorFNTextBox.Text, AuthorMNTextBox.Text, AuthorLNTextBox.Text);
            populateView("default1");*/
        }

        private void UploadImageBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select File";
            dlg.InitialDirectory = $"{URL}";
            dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";
            DialogResult d = dlg.ShowDialog();

            if (d == DialogResult.OK)
            {
                if (ViewBooks.Visible == true)
                {
                    imageTableAccessor.updateCover(dlg.FileName, bookTableAccessor.getImageId(ViewBooks.SelectedNode.Name));
                }
                else
                {
                    imageTableAccessor.updateCover(dlg.FileName, bookTableAccessor.getImageId(ViewBooksListView.SelectedItems[0].SubItems[0].Text));
                }
                CoverImage.Image = createCover(imageTableAccessor.getCover(bookDetails["ImageId"]));
            }
        }

        private void UpdateAllFieldsBtn_Click(object sender, EventArgs e)
        {
            String currentSeriesName = SeriesNameTextBox.Text;
            StringBuilder sb = new StringBuilder();
            bool moveingAuthorsSeries = false;
            int newAuthorId = 0;
            String oldAuthorId = "";
            String result="";
            if (ViewBooks.Visible == true)
            {
                if (!ViewBooks.SelectedNode.Tag.Equals("Author") && !ViewBooks.SelectedNode.Tag.Equals("Series") && !BookTitleTextBox.Tag.Equals(BookTitleTextBox.Text))
                {
                    String id = ViewBooks.SelectedNode.Name.Trim();
                    //TreeNode parentNode = currentNode.Parent;
                    ViewBooks.SelectedNode.Text = bookTableAccessor.updateBook(id, BookTitleTextBox.Text).Text;
                }
                if(!AuthorFNTextBox.Tag.Equals(AuthorFNTextBox.Text) || !AuthorMNTextBox.Tag.Equals(AuthorMNTextBox.Text) ||!AuthorLNTextBox.Tag.Equals(AuthorLNTextBox.Text))
                {
                    String fullName = "";
                    if (AuthorMNTextBox.Text == "")
                    {
                        fullName = sb.Append($"{AuthorFNTextBox.Text.Trim()} ").Append(AuthorLNTextBox.Text.Trim()).ToString().Trim();
                    }
                    else
                    {
                        fullName = sb.Append($"{AuthorFNTextBox.Text.Trim()} ").Append($"{AuthorMNTextBox.Text.Trim()} ").Append(AuthorLNTextBox.Text.Trim()).ToString().Trim();
                    }

                    if (ViewBooks.SelectedNode.Tag == "Author")
                    {
                        ViewBooks.SelectedNode.Text = fullName;
                        authorTableAccessor.updateAuthor(ViewBooks.SelectedNode.Name, AuthorFNTextBox.Text.Trim(), AuthorMNTextBox.Text.Trim(), AuthorLNTextBox.Text.Trim());
                        return;

                    }else if(ViewBooks.SelectedNode.Tag == "Series")
                    {
                        moveingAuthorsSeries = true;
                        String i = ViewBooks.SelectedNode.Parent.Name;
                        String seriesName = ViewBooks.SelectedNode.Text;
                        oldAuthorId = ViewBooks.SelectedNode.Parent.Name;
                        TreeNode copy = (TreeNode)ViewBooks.SelectedNode.Clone();
                        result = authorTableAccessor.findAuthorsByFullName(fullName);                       
                        if (result==null)
                        {
                            newAuthorId = createNewAuthor(fullName);
                            insertAuthorNode(fullName, newAuthorId);
                            bookTableAccessor.updateSeriesAuthor(seriesName, i, newAuthorId.ToString());
                            removeAuthorNode(oldAuthorId);
                            ViewBooks.SelectedNode.Remove();
                            searchAndUpdate(newAuthorId.ToString(), copy);
                        }
                        else
                        {
                            bookTableAccessor.updateSeriesAuthor(seriesName, i, result);
                            removeAuthorNode(oldAuthorId);
                            ViewBooks.SelectedNode.Remove();
                            searchAndUpdate(result, copy);
                        }
                        for(int q=0; q < copy.GetNodeCount(false); q++)
                        {
                            copy.Nodes[q].Tag = result;
                        }
                    }
                    else
                    {
                        String bookId = ViewBooks.SelectedNode.Name;
                        result = authorTableAccessor.findAuthorsByFullName(fullName);
                        if(ViewBooks.SelectedNode.Parent.Tag == "Series")
                        {
                            oldAuthorId = ViewBooks.SelectedNode.Parent.Name;
                        }
                        else
                        {
                            oldAuthorId = ViewBooks.SelectedNode.Parent.Parent.Name;
                        }
                        if (result == null)
                        {
                            newAuthorId = createNewAuthor(fullName);
                            ViewBooks.SelectedNode.Tag = newAuthorId.ToString();
                            bookTableAccessor.updateBookAuthorId(newAuthorId, Int32.Parse(bookId));
                            TreeNode copy = (TreeNode)ViewBooks.SelectedNode.Clone();
                            insertAuthorNode(fullName, newAuthorId);
                            removeAuthorNode(oldAuthorId);
                            ViewBooks.SelectedNode.Remove();
                            searchAndUpdate(newAuthorId.ToString(), copy);
                        }
                        else
                        {
                            bookTableAccessor.updateBookAuthorId(Int32.Parse(result), Int32.Parse(bookId));
                            TreeNode copy = (TreeNode)ViewBooks.SelectedNode.Clone();
                            copy.Tag = result.ToString();
                            removeAuthorNode(oldAuthorId);
                            ViewBooks.SelectedNode.Remove();
                            searchAndUpdate(result, copy);
                        }                        
                    }
                }

                if(!SeriesNameTextBox.Tag.Equals(currentSeriesName))
                {
                    if (ViewBooks.SelectedNode.Tag == "Series")
                    {
                        bookTableAccessor.updateSeriesName(currentSeriesName, ViewBooks.SelectedNode.Text, ViewBooks.SelectedNode.Parent.Name);
                        ViewBooks.SelectedNode.Text = currentSeriesName;
                        if (moveingAuthorsSeries)
                        {
                            TreeNode copy = (TreeNode)ViewBooks.SelectedNode.Clone();
                            ViewBooks.SelectedNode.Parent.Parent.Nodes.Find(result, false)[0].Nodes.Add(copy);
                            removeSeriesNode();
                            if (ViewBooks.SelectedNode.Parent.GetNodeCount(false) == 1)
                            {
                                ViewBooks.SelectedNode.Parent.Remove();
                            }
                            ViewBooks.SelectedNode.Remove();
                        }
                    }
                    else
                    {
                        String bookId = ViewBooks.SelectedNode.Name;
                        String authorId = bookTableAccessor.getAuthorId(bookId);
                        TreeNode copy = (TreeNode)ViewBooks.SelectedNode.Clone();
                        String seriesName = SeriesNameTextBox.Text.Trim();
                        if (seriesName.Equals(""))
                        {
                            if(ViewBooks.SelectedNode.Parent.Tag == "Series")
                            {
                                bookTableAccessor.updateSeriesForBook("", bookId);
                                if (ViewBooks.SelectedNode.Parent.GetNodeCount(true) == 1)
                                {
                                    ViewBooks.SelectedNode.Parent.Remove();
                                    ViewBooks.SelectedNode.Parent.Nodes.Add(copy);
                                    ViewBooks.SelectedNode.Remove();
                                }
                                else
                                {
                                    ViewBooks.SelectedNode.Parent.Parent.Nodes.Add(copy);
                                    ViewBooks.SelectedNode.Remove();
                                }
                                ViewBooks.SelectedNode = copy;
                            }
                        }
                        else
                        {
                            if (ViewBooks.SelectedNode.Parent.Tag == "Series")
                            {
                                try
                                {
                                    ViewBooks.SelectedNode.Parent.Parent.Nodes.Find(seriesName, true)[0].Nodes.Add(copy);
                                    removeSeriesNode();
                                    ViewBooks.SelectedNode.Remove();
                                }
                                catch
                                {
                                    ViewBooks.SelectedNode.Parent.Parent.Nodes.Add(new TreeNode
                                    {
                                        Name = seriesName,
                                        Text = seriesName,
                                        Tag = "Series"
                                    });
                                    ViewBooks.SelectedNode.Parent.Parent.Nodes.Find(seriesName, true)[0].Nodes.Add(copy);
                                    bookTableAccessor.updateSeriesForBook(seriesName, bookId);
                                    removeSeriesNode();
                                    ViewBooks.SelectedNode.Remove();
                                }
                                ViewBooks.SelectedNode = copy;
                            }
                            else
                            {
                                bookTableAccessor.updateSeriesForBook(seriesName, bookId);
                                try
                                {
                                    ViewBooks.SelectedNode.Parent.Nodes.Find(seriesName, true)[0].Nodes.Add(copy);
                                    removeSeriesNode();
                                    ViewBooks.SelectedNode.Remove();
                                }
                                catch
                                {
                                    ViewBooks.SelectedNode.Parent.Nodes.Add(new TreeNode
                                    {
                                        Name = seriesName,
                                        Text = seriesName,
                                        Tag = "Series"
                                    });
                                    ViewBooks.SelectedNode.Parent.Nodes.Find(seriesName, true)[0].Nodes.Add(copy);
                                    removeSeriesNode();
                                    ViewBooks.SelectedNode.Remove();
                                }
                                ViewBooks.SelectedNode = copy;
                            }
                        }
                    }
                }
                if(!SeriesNumberTextBox.Tag.Equals(SeriesNumberTextBox.Text) && !ViewBooks.SelectedNode.Tag.Equals("Author"))
                {
                    TreeNode cNode = ViewBooks.SelectedNode;
                    TreeNode pNode = cNode.Parent;
                    if (!pNode.Tag.Equals("Author"))
                    {
                        bookTableAccessor.updateSeriesNum(SeriesNumberTextBox.Text, cNode.Name);
                        cNode.Remove();
                        pNode.Nodes.Insert(Int32.Parse(SeriesNumberTextBox.Text.Trim()), cNode);
                    }
                }
                if (genreUpdated)
                {
                    genreUpdated = false;
                }
            }
            else
            {
                if (!BookTitleTextBox.Tag.Equals(BookTitleTextBox.Text))
                {
                    if (ViewBooksListView.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    if (ViewBooksListView.SelectedItems.Count == 1)
                    {
                        var items = ViewBooksListView.SelectedItems[0];
                        ViewBooksListView.SelectedItems[0].SubItems[1].Text = bookTableAccessor.updateBook(items.SubItems[0].Text, BookTitleTextBox.Text).Text;
                    }
                    else
                    {
                        for (int i = 0; i < ViewBooksListView.SelectedItems.Count; i++)
                        {
                            bookTableAccessor.updateBook(ViewBooksListView.SelectedItems[i].SubItems[0].Text, BookTitleTextBox.Text);
                            ViewBooksListView.SelectedItems[i].SubItems[1].Text = BookTitleTextBox.Text;
                        }
                    }

                }
                if (!AuthorFNTextBox.Tag.Equals(AuthorFNTextBox.Text) || !AuthorMNTextBox.Tag.Equals(AuthorMNTextBox.Text) || !AuthorLNTextBox.Tag.Equals(AuthorLNTextBox.Text))
                {
                    String fullName = "";
                    if (AuthorMNTextBox.Text == "")
                    {
                        fullName = sb.Append($"{AuthorFNTextBox.Text.Trim()} ").Append(AuthorLNTextBox.Text.Trim()).ToString().Trim();
                    }
                    else
                    {
                        fullName = sb.Append($"{AuthorFNTextBox.Text.Trim()} ").Append($"{AuthorMNTextBox.Text.Trim()} ").Append(AuthorLNTextBox.Text.Trim()).ToString().Trim();
                    }

                    if (ViewBooksListView.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    if (ViewBooksListView.SelectedItems.Count == 1)
                    {
                        var item = ViewBooksListView.SelectedItems[0];
                        String currentId = bookTableAccessor.getAuthorId(item.SubItems[0].Text);
                        result = authorTableAccessor.findAuthorsByFullName(fullName);
                        if (result == null)
                        {
                            newAuthorId = authorTableAccessor.addCustomAuthor(new Dictionary<string, string>
                            {
                                {"Id", "Custom"},
                                {"FirstName",AuthorFNTextBox.Text.Trim() },
                                {"MiddleName",AuthorMNTextBox.Text.Trim() },
                                {"LastName",AuthorLNTextBox.Text.Trim() },
                                {"FullName", fullName }
                            });
                            bookTableAccessor.updateBookAuthorId(newAuthorId, Int32.Parse(item.SubItems[0].Text));
                            ViewBooksListView.SelectedItems[0].SubItems[0].Text = newAuthorId.ToString();
                            ViewBooksListView.SelectedItems[0].SubItems[2].Text = fullName;
                            if (!bookTableAccessor.checkHowManyBooksAuthorHas(currentId))
                            {
                                authorTableAccessor.deleteAuthor(currentId);
                            }
                        }
                        else
                        {
                            bookTableAccessor.updateBookAuthorId(Int32.Parse(result), Int32.Parse(item.SubItems[0].Text));
                            if (!bookTableAccessor.checkHowManyBooksAuthorHas(currentId))
                            {
                                authorTableAccessor.deleteAuthor(currentId);
                            }
                        }
                    }
                    else
                    {
                        result = authorTableAccessor.findAuthorsByFullName(fullName);
                        if (result == null)
                        {
                            newAuthorId = authorTableAccessor.addCustomAuthor(new Dictionary<string, string>
                            {
                                {"Id", "Custom"},
                                {"FirstName",AuthorFNTextBox.Text.Trim() },
                                {"MiddleName",AuthorMNTextBox.Text.Trim() },
                                {"LastName",AuthorLNTextBox.Text.Trim() },
                                {"FullName", fullName }
                            });
                            for (int i = 0; i < ViewBooksListView.SelectedItems.Count; i++)
                            {
                                bookTableAccessor.updateBookAuthorId(newAuthorId, Int32.Parse(ViewBooksListView.SelectedItems[i].SubItems[0].Text));
                                ViewBooksListView.SelectedItems[i].SubItems[0].Text = newAuthorId.ToString();
                                ViewBooksListView.SelectedItems[i].SubItems[2].Text = fullName;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ViewBooksListView.SelectedItems.Count; i++)
                            {
                                bookTableAccessor.updateBookAuthorId(Int32.Parse(result), Int32.Parse(ViewBooksListView.SelectedItems[i].SubItems[0].Text));
                            }
                        }
                    }
                }
                if (!SeriesNameTextBox.Tag.Equals(SeriesNameTextBox.Text))
                {
                    if (ViewBooksListView.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    if(ViewBooksListView.SelectedItems.Count == 1)
                    {
                        var item = ViewBooksListView.SelectedItems[0];
                        bookTableAccessor.updateSeriesName(SeriesNameTextBox.Text, item.SubItems[3].Text, bookTableAccessor.getAuthorId(item.SubItems[0].Text));
                        ViewBooksListView.SelectedItems[0].SubItems[3].Text = SeriesNameTextBox.Text;
                    }
                    else
                    {
                        for (int i = 0; i < ViewBooksListView.SelectedItems.Count; i++)
                        {
                            var item = ViewBooksListView.SelectedItems[i];
                            bookTableAccessor.updateSeriesName(SeriesNameTextBox.Text, item.SubItems[3].Text, bookTableAccessor.getAuthorId(item.SubItems[0].Text));
                            ViewBooksListView.SelectedItems[i].SubItems[3].Text = SeriesNameTextBox.Text;
                        }
                    }
                }
                if (!SeriesNumberTextBox.Tag.Equals(SeriesNumberTextBox.Text))
                {
                    if (ViewBooksListView.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    var item = ViewBooksListView.SelectedItems[0];
                    bookTableAccessor.updateSeriesNum(SeriesNumberTextBox.Text, item.SubItems[0].Text);
                }
            }
        }
        public void populateGenres()
        {
            ListOfGenresComboBox.DataSource = genreTableAccessor.getGenres();
        }

        private void EditGenreBtn_Click(object sender, EventArgs e)
        {
            String id = "";
            if (ViewBooks.Visible == true)
            {
                id = ViewBooks.SelectedNode.Name;
                bookTableAccessor.addGenre(id, ListOfGenresComboBox.SelectedItem.ToString());
            }
            else
            {
                id = ViewBooksListView.SelectedItems[0].SubItems[0].Text;
                bookTableAccessor.addGenre(id, ListOfGenresComboBox.SelectedItem.ToString());
            }
            populateGenreBox(bookTableAccessor.getBookGenre(id));
        }
        private void DeleteSelectedGenreBtn_Click(object sender, EventArgs e)
        {
            String id="";
            int index = GenreListBox.SelectedIndex;
            if (ViewBooks.Visible == true)
            {
                id = ViewBooks.SelectedNode.Name;
            }
            else
            {
                id = ViewBooksListView.SelectedItems[0].SubItems[0].Text;
            }
            bookTableAccessor.deleteGenre(id, index);
            populateGenreBox(bookTableAccessor.getBookGenre(id));
        }

        private void ManageGenreBtn_Click(object sender, EventArgs e)
        {
           ManageGenres manageGenreForm = new ManageGenres(genreTableAccessor);
           manageGenreForm.ShowDialog();
           populateGenres();
        }
        private void searchAndUpdate(String name, TreeNode copyNode)
        {
            mainNode.Nodes.Find(name.Trim(), true)[0].Nodes.Add(copyNode);
            ViewBooks.SelectedNode = copyNode;
        }
        private void searchAndUpdateWithoutSelection(String name, TreeNode copyNode)
        {
            mainNode.Nodes.Find(name.Trim(), true)[0].Nodes.Add(copyNode);
        }
        private int createNewAuthor(String fullName)
        {
            return authorTableAccessor.addCustomAuthor(new Dictionary<string, string>
            {
                {"Id", "Custom"},
                {"FirstName",AuthorFNTextBox.Text.Trim() },
                {"MiddleName",AuthorMNTextBox.Text.Trim() },
                {"LastName",AuthorLNTextBox.Text.Trim() },
                {"FullName", fullName }
            });
        }
        private void removeAuthorNode(String oldId)
        {
            if (SeriesNameTextBox.Text.Equals(""))
            {
                if (ViewBooks.SelectedNode.Parent.GetNodeCount(false) == 1)
                {
                    ViewBooks.SelectedNode.Parent.Remove();
                    authorTableAccessor.deleteAuthor(oldId);
                }
            }
            else
            {
                if (ViewBooks.SelectedNode.Parent.Parent.GetNodeCount(false) == 1)
                {
                    ViewBooks.SelectedNode.Parent.Remove();
                    authorTableAccessor.deleteAuthor(oldId);
                }
            }
        }
        private void removeSeriesNode()
        {
            if (ViewBooks.SelectedNode.Parent.GetNodeCount(true) == 1)
            {
                ViewBooks.SelectedNode.Parent.Remove();
            }
        }
        private void insertAuthorNode(String fullName, int newAuthorId)
        {
            mainNode.Nodes.Insert(searcher.searchTree(mainNode.Nodes, fullName), new TreeNode
            {
                Name = newAuthorId.ToString(),
                Text = fullName,
                Tag = "Author"
            });
        }

        private void ResetTablesBtn_Click(object sender, EventArgs e)
        {
            ResetConfirmer rS = new ResetConfirmer();
            rS.ShowDialog();

            if (rS.returnValue)
            {
                authorTableAccessor.resetAuthorTable();
                bookTableAccessor.resetBookTable();
                imageTableAccessor.resetCoverTable();
                populateView("default1");
            }
        }
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if(ViewBooks.Visible == true)
            {
                int result=2;
                if(ViewBooks.SelectedNode.Tag == "Author")
                {
                    ConfirmDelete cd = new ConfirmDelete(0);
                    cd.ShowDialog();
                    if (cd.returnValue == 1)
                    {
                        authorTableAccessor.deleteAuthor(ViewBooks.SelectedNode.Name);
                        bookTableAccessor.deleteBooksByAuthorId(ViewBooks.SelectedNode.Name);
                        ViewBooks.SelectedNode.Remove();
                    }
                    else if (cd.returnValue == 0)
                    {
                        authorTableAccessor.deleteAuthor(ViewBooks.SelectedNode.Name);
                        foreach(TreeNode t in ViewBooks.SelectedNode.Nodes)
                        {
                            TreeNode copy = (TreeNode) t.Clone();
                            searchAndUpdateWithoutSelection("-1", copy);
                        }
                        ViewBooks.SelectedNode.Remove();
                    }
                    return;
                    
                }else if(ViewBooks.SelectedNode.Tag == "Series")
                {
                    ConfirmDelete cd = new ConfirmDelete(1);
                    cd.ShowDialog();
                    if (cd.returnValue == 1)
                    {
                        bookTableAccessor.deleteBooksBySeries(ViewBooks.SelectedNode.Parent.Name, ViewBooks.SelectedNode.Text.Trim());
                        ViewBooks.SelectedNode.Remove();
                    }
                    else if (cd.returnValue == 0)
                    {
                        bookTableAccessor.updateSeriesName("",ViewBooks.SelectedNode.Text.Trim(), ViewBooks.SelectedNode.Parent.Name);
                        foreach(TreeNode t in ViewBooks.SelectedNode.Nodes)
                        {
                            TreeNode copy = (TreeNode)t.Clone();
                            ViewBooks.SelectedNode.Parent.Nodes.Add(copy);
                        }
                        ViewBooks.SelectedNode.Remove();
                    }
                    return;
                }
                else
                {
                    ConfirmDelete cd = new ConfirmDelete(2);
                    cd.ShowDialog();
                    if (cd.returnValue == 1)
                    {
                        bookTableAccessor.deleteBook(ViewBooks.SelectedNode.Name);
                        ViewBooks.SelectedNode.Remove();
                    }
                    return;
                }
            }
            else
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
/*            foreach(Control ctrl in this.Controls)
            {
                rectangleList.Add(new Rectangle(ctrl.Location.X, ctrl.Location.Y, ctrl.Width, ctrl.Height));
                controlList.Add(ctrl);
            }*/



            originalFormSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);

            selectFileBtn = new Rectangle(SelectFile.Location.X, SelectFile.Location.Y, SelectFile.Width, SelectFile.Height);
            addFileBtn = new Rectangle(AddBtn.Location.X, AddBtn.Location.Y, AddBtn.Width, AddBtn.Height);
            updateViewBtn = new Rectangle(updateBtn.Location.X, updateBtn.Location.Y, updateBtn.Width, updateBtn.Height);
            changeViewBtn = new Rectangle(ChangeViewBtn.Location.X, ChangeViewBtn.Location.Y, ChangeViewBtn.Width, ChangeViewBtn.Height);
            resetTablesBtn = new Rectangle(ResetTablesBtn.Location.X, ResetTablesBtn.Location.Y, ResetTablesBtn.Width, ResetTablesBtn.Height);
            addSelectedGenreBtn = new Rectangle(EditGenreBtn.Location.X, EditGenreBtn.Location.Y, EditGenreBtn.Width, EditGenreBtn.Height);
            deleteSelectedGenreBtn = new Rectangle(DeleteSelectedGenreBtn.Location.X, DeleteSelectedGenreBtn.Location.Y, DeleteSelectedGenreBtn.Width, DeleteSelectedGenreBtn.Height);
            manageGeneresBtn = new Rectangle(ManageGenreBtn.Location.X, ManageGenreBtn.Location.Y, ManageGenreBtn.Width, ManageGenreBtn.Height);
            updateBookBtn = new Rectangle(UpdateAllFieldsBtn.Location.X, UpdateAllFieldsBtn.Location.Y, UpdateAllFieldsBtn.Width, UpdateAllFieldsBtn.Height);
            openInFileExplorerBtn = new Rectangle(showInFileExplorer.Location.X, showInFileExplorer.Location.Y, showInFileExplorer.Width, showInFileExplorer.Height);
            saveBookBtn = new Rectangle(SaveBook.Location.X, SaveBook.Location.Y, SaveBook.Width, SaveBook.Height);
            changeImageBtn = new Rectangle(UploadImageBtn.Location.X, UploadImageBtn.Location.Y, UploadImageBtn.Width, UploadImageBtn.Height);
            searchBtn = new Rectangle(SearchBtn.Location.X, SearchBtn.Location.Y, SearchBtn.Width, SearchBtn.Height);
            deleteBtn = new Rectangle(DeleteBtn.Location.X, DeleteBtn.Location.Y, DeleteBtn.Width,DeleteBtn.Height);

            ListViewRectangle = new Rectangle(ViewBooksListView.Location.X, ViewBooksListView.Location.Y, ViewBooksListView.Width, ViewBooksListView.Height);
            TreeViewRectangle = new Rectangle(ViewBooks.Location.X, ViewBooks.Location.Y, ViewBooks.Width, ViewBooks.Height);

            searchBox = new Rectangle(SearchTextBox.Location.X, SearchTextBox.Location.Y, SearchTextBox.Width, SearchTextBox.Height);
            directoryTextBox = new Rectangle(DirectoryTextBox.Location.X, DirectoryTextBox.Location.Y, DirectoryTextBox.Width, DirectoryTextBox.Height);
            bookTitleTextBox = new Rectangle(BookTitleTextBox.Location.X, BookTitleTextBox.Location.Y, BookTitleTextBox.Width, BookTitleTextBox.Height);
            authorFNTextBox = new Rectangle(AuthorFNTextBox.Location.X, AuthorFNTextBox.Location.Y, AuthorFNTextBox.Width, AuthorFNTextBox.Height);
            authorMNTextBox = new Rectangle(AuthorMNTextBox.Location.X, AuthorMNTextBox.Location.Y, AuthorMNTextBox.Width, AuthorMNTextBox.Height);
            authorLNTextBox = new Rectangle(AuthorLNTextBox.Location.X, AuthorLNTextBox.Location.Y, AuthorLNTextBox.Width, AuthorLNTextBox.Height);
            authorTableId = new Rectangle(authorTableIdTextBox.Location.X, authorTableIdTextBox.Location.Y, authorTableIdTextBox.Width, authorTableIdTextBox.Height);
            authorBookId = new Rectangle(authorIdTextBox.Location.X, authorIdTextBox.Location.Y, authorIdTextBox.Width, authorIdTextBox.Height);
            seriesName = new Rectangle(SeriesNameTextBox.Location.X, SeriesNameTextBox.Location.Y, SeriesNameTextBox.Width, SeriesNameTextBox.Height);
            seriesNumber = new Rectangle(SeriesNumberTextBox.Location.X, SeriesNumberTextBox.Location.Y, SeriesNumberTextBox.Width, SeriesNumberTextBox.Height);

            GenreView = new Rectangle(GenreListBox.Location.X, GenreListBox.Location.Y, GenreListBox.Width, GenreListBox.Height);
            failedToLoadBox = new Rectangle(FailedURLs.Location.X, FailedURLs.Location.Y, FailedURLs.Width, FailedURLs.Height);
            annotationBox = new Rectangle(AnnotationBox.Location.X, AnnotationBox.Location.Y, AnnotationBox.Width, AnnotationBox.Height);

            coverImageBox = new Rectangle(CoverImage.Location.X, CoverImage.Location.Y, CoverImage.Width, CoverImage.Height);

            searchDropDown = new Rectangle(SearchTypeComboBox.Location.X, SearchTypeComboBox.Location.Y, SearchTypeComboBox.Width, SearchTypeComboBox.Height);
            viewDropDown = new Rectangle(ViewSelection.Location.X, ViewSelection.Location.Y, ViewSelection.Width, ViewSelection.Height);
            genreDropDown = new Rectangle(ListOfGenresComboBox.Location.X, ListOfGenresComboBox.Location.Y, ListOfGenresComboBox.Width, ListOfGenresComboBox.Height);

            rLabel1 = new Rectangle(TitleLabel.Location.X, TitleLabel.Location.Y, TitleLabel.Width, TitleLabel.Height);
            rLabel2 = new Rectangle(label1.Location.X, label1.Location.Y, label1.Width, label1.Height);
            rLabel3 = new Rectangle(label3.Location.X, label3.Location.Y, label3.Width, label3.Height);
            rLabel4 = new Rectangle(label2.Location.X, label2.Location.Y, label2.Width, label2.Height);
            rLabel5 = new Rectangle(GenreLabel.Location.X, GenreLabel.Location.Y, GenreLabel.Width, GenreLabel.Height);
            rLabel6 = new Rectangle(AFNLabel.Location.X, AFNLabel.Location.Y, AFNLabel.Width, AFNLabel.Height);
            rLabel7 = new Rectangle(AMNLabel.Location.X, AMNLabel.Location.Y, AMNLabel.Width, AMNLabel.Height);
            rLabel8 = new Rectangle(ALNLabel.Location.X, ALNLabel.Location.Y, ALNLabel.Width, ALNLabel.Height);
            rLabel9 = new Rectangle(label4.Location.X, label4.Location.Y, label4.Width, label4.Height);
            rLabel1 = new Rectangle(label5.Location.X, label5.Location.Y, label5.Width, label5.Height);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            /*            for (int i=0; i < controlList.Count; i++)
                        {
                            ResizeControl(rectangleList.ElementAt(i), controlList.ElementAt(i));
                        }
            */

            ResizeControl(selectFileBtn, SelectFile);
            ResizeControl(addFileBtn, AddBtn);
            ResizeControl(updateViewBtn, updateBtn);
            ResizeControl(changeViewBtn, ChangeViewBtn);
            ResizeControl(resetTablesBtn, ResetTablesBtn);
            ResizeControl(addSelectedGenreBtn, EditGenreBtn);
            ResizeControl(deleteSelectedGenreBtn, DeleteSelectedGenreBtn);
            ResizeControl(manageGeneresBtn, ManageGenreBtn);
            ResizeControl(updateBookBtn, UpdateAllFieldsBtn);
            ResizeControl(openInFileExplorerBtn, showInFileExplorer);
            ResizeControl(saveBookBtn, SaveBook);
            ResizeControl(changeImageBtn, UploadImageBtn);
            ResizeControl(searchBtn, SearchBtn);
            ResizeControl(deleteBtn, DeleteBtn);

            ResizeControl(ListViewRectangle, ViewBooksListView);
            ResizeControl(TreeViewRectangle, ViewBooks);

            ResizeControl(searchBox, SearchTextBox);
            ResizeControl(directoryTextBox, DirectoryTextBox);
            ResizeControl(bookTitleTextBox, BookTitleTextBox);
            ResizeControl(authorFNTextBox, AuthorFNTextBox);
            ResizeControl(authorMNTextBox, AuthorMNTextBox);
            ResizeControl(authorLNTextBox, AuthorLNTextBox);
            ResizeControl(authorTableId, authorTableIdTextBox);
            ResizeControl(authorBookId, authorIdTextBox);
            ResizeControl(seriesName, SeriesNameTextBox);
            ResizeControl(seriesNumber, SeriesNumberTextBox);

            ResizeControl(GenreView, GenreListBox);
            ResizeControl(failedToLoadBox, FailedURLs);
            ResizeControl(annotationBox, AnnotationBox);

            ResizeControl(coverImageBox, CoverImage);

            ResizeControl(searchDropDown, SearchTypeComboBox);
            ResizeControl(viewDropDown, ViewSelection);
            ResizeControl(genreDropDown, ListOfGenresComboBox);

            ResizeControl(rLabel1,TitleLabel);
            ResizeControl(rLabel2,label1);
            ResizeControl(rLabel3,label3);
            ResizeControl(rLabel4,label2);
            ResizeControl(rLabel5,GenreLabel);
            ResizeControl(rLabel6,AFNLabel);
            ResizeControl(rLabel7,AMNLabel);
            ResizeControl(rLabel8,ALNLabel);
            ResizeControl(rLabel9,label4);
            ResizeControl(rLabel1,label5);


        }
        private void ResizeControl(Rectangle r, Control c)
        {
            float xRatio = (float)this.Width / (float)originalFormSize.Width;
            float yRatio = (float)this.Height / (float)originalFormSize.Height;

            int newX = (int)(r.Location.X * xRatio);
            int newY = (int)(r.Location.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }

    }
}
