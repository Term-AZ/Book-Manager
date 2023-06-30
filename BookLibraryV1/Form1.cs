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
        


        Image image;
        public Form1()
        {
            InitializeComponent();
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
                DirectoryTextBox.Text = f;
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
                    //zipReader(f);
                }

            }
            return; 
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            fileReader.populateTables(booksTitles);
            /*            foreach (String f in files)
                        {
                            DirectoryTextBox.Text = f;
                            FileInfo fi = new FileInfo(f);
                            if (fi.Extension==".fb2")
                            {
                                booksTitles.Add(f);
                                List<String> s = new List<String>
                                {
                                    f
                                };
                                //allBooks(Directory.GetFiles(f).ToList()); TO DO IT RECURSIVELY, METHOD GetFileSystemEntries gets all files and directories already, not needed to search through each folder
                            }
                            else if (fi.Extension == ".zip")
                            {
                                DirectoryTextBox.Text = "ZIP!";
                                zipReader(f);
                            }
                        }*/

        }
        private void zipReader(String path)
        {
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach(ZipArchiveEntry i in archive.Entries)
                {
                    if (i.FullName.Contains(".zip"))
                    {

                    }
                    else
                    {
                        booksTitles.Add($"{path}");
                    }
                }
            }
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
            DirectoryTextBox.Text = books.Count.ToString();
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
/*          SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.Title = "Save Book";
            saveFileDialog.ShowDialog();*/
            using (var fbd = new FolderBrowserDialog())
            {
                
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if(ViewBooks.Visible == true)
                    {
                        fileReader.editBook(ViewBooks.SelectedNode.Name.Trim(), fbd.SelectedPath);
                    }
                    else
                    {
                        if (ViewBooksListView.SelectedItems.Count == 0)
                        {
                            return;
                        }
                        var item = ViewBooksListView.SelectedItems[0];
                        fileReader.editBook(item.SubItems[0].Text.Trim(), fbd.SelectedPath);
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
                        DirectoryTextBox.Text = bookId;
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
    }
}
