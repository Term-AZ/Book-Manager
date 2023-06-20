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
        


        Image image;
        public Form1()
        {
            InitializeComponent();
            ViewBooksListView.FullRowSelect = true;
            ViewBooksListView.Visible = false;
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
                imageTableAccessor = new ImageTableAccessor(this, connection);
                authorTableAccessor = new AuthorTableAccessor(this, connection);
                bookTableAccessor = new BookTableAccessor(this, connection);
                genreTableAccessor = new GenreTableAccessor(this, URL, connection);
                fileReader = new FileReader(this, authorTableAccessor, bookTableAccessor, genreTableAccessor, imageTableAccessor);

                altMainNode = new TreeNode();
                altMainNode.Text = "Books Without Authors";
                altMainNode.Name = "Authorless";
                mainNode = new TreeNode();
                mainNode.Name = "main";
                mainNode.Text = "Authors";
                mainNode.ExpandAll();
                this.ViewBooks.Nodes.Add(mainNode);
                this.ViewBooks.Nodes.Add(altMainNode);


                CoverImage.SizeMode = PictureBoxSizeMode.StretchImage;

                EditGenresComboBox.SelectedIndex = 0;
                EditGenresComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                SearchTypeComboBox.SelectedIndex = 0;
                SearchTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                ViewSelection.SelectedIndex = 0;

                createTables();
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
            /*            OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Title = "Select File";
                        dlg.InitialDirectory = $"{URL}";
                        dlg.ShowDialog();
                        DirectoryTextBox.Text = dlg.FileName;
                        URL = dlg.FileName;*/ 
        }
        private void allBooks(List<String> l)
        {
            foreach(String f in l)
            {
                FileAttributes fa = File.GetAttributes(f);
                if (fa != FileAttributes.Directory)
                {
                    booksTitles.Add(f);
                    //allBooks(Directory.GetFiles(f).ToList()); TO DO IT RECURSIVELY, METHOD GetFileSystemEntries gets all files and directories already, not needed to search through each folder
                }
            }
            return; 
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            fileReader.populateTables(booksTitles);
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
                Dictionary<String, List<TreeNode>> books = new Dictionary<string, List<TreeNode>>();
                mainNode.Nodes.Clear();
                altMainNode.Nodes.Clear();
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
                        books = bookTableAccessor.searchByAuthorIdTree(s);
                        populateTreeViewByThreeLayers(authors, books);
                        break;
                    case "Search By Genre":
                        mainNode.Text = "Authors";
                        books = bookTableAccessor.searchByGenreTree(SearchTextBox.Text);
                        populateTreeViewByBook(books);
                        break;
                    case "Search By Keywords":
                        mainNode.Text = "Books";
                        books = bookTableAccessor.searchByKeywordsTree(SearchTextBox.Text);
                        populateTreeViewByBook(books);
                        break;
                    case "Search By Publisher":
                        mainNode.Text = "Books";
                        books = bookTableAccessor.searchByPublisherTree(SearchTextBox.Text);
                        populateTreeViewByBook(books);
                        break;
                    case "Search By Series":
                        mainNode.Text = "Books";
                        books = bookTableAccessor.searchBySeriesTree(SearchTextBox.Text);
                        populateTreeViewByBook(books);
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
            Dictionary<String, TreeNode> authors = new Dictionary<string, TreeNode>();
            Dictionary<String, List<TreeNode>> books = new Dictionary<string, List<TreeNode>>();
            mainNode.Nodes.Clear();
            altMainNode.Nodes.Clear();
            switch (type)
            {
                case "default":
                    if (ViewBooks.Visible == true)
                    {
                        mainNode.Text = "Authors";
                        authors = authorTableAccessor.getAuthors();
                        books = bookTableAccessor.getBooksTree();
                        populateTreeViewByThreeLayers(authors, books);
                    }
                    else
                    {
                        populateListViewByBook(bookTableAccessor.getBooksList());
                    }
                    break;
                case "default1":
                    mainNode.Text = "Authors";
                    authors = authorTableAccessor.getAuthors();
                    books = bookTableAccessor.getBooksTree();
                    populateTreeViewByThreeLayers(authors, books);
                    populateListViewByBook(bookTableAccessor.getBooksList());
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
            altMainNode.Nodes.Clear();
            foreach (TreeNode t in l)
            {
                if (t.Tag == "")
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
                    if(t.Tag == "")
                    {
                        altMainNode.Nodes.Add(t);
                        continue;
                    }
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
                foreach (TreeNode d in pair.Value)
                {
                    if (d.Tag.ToString() == "")
                    {
                        altMainNode.Nodes.Add(d);
                        continue;
                    }

                    if (!a.ContainsKey(pair.Key))
                    {
                        continue;
                    }
                    a[pair.Key].Nodes.Add(d);
                }
            }

            foreach (KeyValuePair<string, TreeNode> entry in a)
            {
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
        public void populateFailed(List<String> s)
        {
            foreach(String i in s)
            {
                FailedURLs.Text += $"{i}\n";
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

        private void UpdateSeriesName_Click(object sender, EventArgs e)
        {
            if (ViewBooks.Visible == true)
            {
                bookTableAccessor.updateSeriesName(SeriesNameTextBox.Text, ViewBooks.SelectedNode.Parent.Text.Trim(), ViewBooks.SelectedNode.Parent.Tag.ToString());
                ViewBooks.SelectedNode.Parent.Text = SeriesNameTextBox.Text;
            }
            else
            {
                if (ViewBooksListView.SelectedItems.Count == 0)
                {
                    return;
                }
                var item = ViewBooksListView.SelectedItems[0];
                bookTableAccessor.updateSeriesName(SeriesNameTextBox.Text, item.SubItems[3].Text, bookTableAccessor.getAuthorId(item.SubItems[0].Text));
                ViewBooksListView.SelectedItems[0].SubItems[3].Text = SeriesNameTextBox.Text;
            } 
        }

        private void UpdateSeriesNumBtn_Click(object sender, EventArgs e)
        {
            if (ViewBooks.Visible == true)
            {
                TreeNode cNode = ViewBooks.SelectedNode;
                TreeNode pNode = cNode.Parent;
                if (pNode.Tag != "Author")
                {
                    bookTableAccessor.updateSeriesNum(SeriesNumberTextBox.Text, cNode.Name);
                    cNode.Remove();
                    pNode.Nodes.Insert(Int32.Parse(SeriesNumberTextBox.Text.Trim()), cNode);
                }
            }
            else
            {
                if (ViewBooksListView.SelectedItems.Count == 0)
                {
                    return;
                }
                var item = ViewBooksListView.SelectedItems[0];
                bookTableAccessor.updateSeriesNum(SeriesNumberTextBox.Text, item.SubItems[0].Text);
            }
        }
        private void updateBtnAuthor_Click(object sender, EventArgs e)
        {
            if (ViewBooks.Visible == true)
            {
                string iD = ViewBooks.SelectedNode.Parent.Name.Trim();
                ViewBooks.SelectedNode.Parent.Text = authorTableAccessor.updateAuthor(iD, AuthorFNTextBox.Text, AuthorMNTextBox.Text, AuthorLNTextBox.Text);
            }
            else
            {
                if (ViewBooksListView.SelectedItems.Count == 0)
                {
                    return;
                }
                var item = ViewBooksListView.SelectedItems[0];
                ViewBooksListView.SelectedItems[0].SubItems[3].Text = authorTableAccessor.updateAuthor(bookTableAccessor.getAuthorId(item.SubItems[0].Text), AuthorFNTextBox.Text, AuthorMNTextBox.Text, AuthorLNTextBox.Text);
            }
        }
        private void UpdateBtnBook_Click(object sender, EventArgs e)
        {
            if (ViewBooks.Visible == true)
            {
                TreeNode currentNode = ViewBooks.SelectedNode;
                String id = ViewBooks.SelectedNode.Name.Trim();
                //TreeNode parentNode = currentNode.Parent;
                currentNode.Text = bookTableAccessor.updateBook(id, BookTitleTextBox.Text).Text;
            }
            else
            {
                if (ViewBooksListView.SelectedItems.Count == 0)
                {
                    return;
                }
                var items = ViewBooksListView.SelectedItems[0];
                ViewBooksListView.SelectedItems[0].SubItems[1].Text = bookTableAccessor.updateBook(items.SubItems[0].Text,BookTitleTextBox.Text).Text;
            }
        }
        private void EditGenreBtn_Click(object sender, EventArgs e)
        {
            if (ViewBooks.Visible == true)
            {
                switch (EditGenresComboBox.Items[EditGenresComboBox.SelectedIndex].ToString())
                {
                    case "Edit Selected Genre":
                        populateGenreBox(bookTableAccessor.editGenre(ViewBooks.SelectedNode.Name.Trim(), genreText.Text, GenreListBox.SelectedIndex));
                        break;
                    case "Delete Selected Genre":
                        populateGenreBox(bookTableAccessor.deleteGenre(ViewBooks.SelectedNode.Name.Trim(), GenreListBox.SelectedIndex));
                        break;
                    case "Add New Genre":
                        populateGenreBox(bookTableAccessor.addGenre(ViewBooks.SelectedNode.Name.Trim(), genreText.Text));
                        break;
                }
            }
            else
            {
                if (ViewBooksListView.SelectedItems.Count == 0)
                {
                    return;
                }
                var item = ViewBooksListView.SelectedItems[0];
                switch (EditGenresComboBox.Items[EditGenresComboBox.SelectedIndex].ToString())
                {
                    case "Edit Selected Genre":
                        populateGenreBox(bookTableAccessor.editGenre(item.SubItems[0].Text, genreText.Text, GenreListBox.SelectedIndex));
                        break;
                    case "Delete Selected Genre":
                        populateGenreBox(bookTableAccessor.deleteGenre(item.SubItems[0].Text, GenreListBox.SelectedIndex));
                        break;
                    case "Add New Genre":
                        populateGenreBox(bookTableAccessor.addGenre(item.SubItems[0].Text, genreText.Text));
                        break;
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
            populateInfo(authorDetails, bookDetails);

        }
        private void ViewBooks_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (ViewBooks.SelectedNode.FirstNode == null && ViewBooks.SelectedNode.Parent != null)
            {
                bookId = ViewBooks.SelectedNode.Name.Trim();
                DirectoryTextBox.Text = bookId;

                authorDetails = authorTableAccessor.getAuthor(ViewBooks.SelectedNode.Tag.ToString());
                bookDetails = bookTableAccessor.getBook(bookId);
                populateInfo(authorDetails, bookDetails);

            }
        }
        private void populateInfo(List<String> authorDetails, Dictionary<String,String> bookDetails)
        {

            BookTitleTextBox.Text = bookDetails["Title"];

            try
            {
                AuthorFNTextBox.Text = authorDetails.ElementAt(0);
                AuthorMNTextBox.Text = authorDetails.ElementAt(1);
                AuthorLNTextBox.Text = authorDetails.ElementAt(2);
                AnnotationBox.Text = bookDetails["Annotation"];
                CoverImage.Image = createCover(imageTableAccessor.getCover(bookId));
            }
            catch (Exception a)
            {
                AuthorFNTextBox.Text = "";
                AuthorMNTextBox.Text = "";
                AuthorLNTextBox.Text = "";
                AnnotationBox.Text = "";
            }

            SeriesNameTextBox.Text = bookDetails["Series"];
            SeriesNumberTextBox.Text = bookDetails["SeriesNum"];
            populateGenreBox(bookDetails["Genre"]);
           
        }
    }
}
