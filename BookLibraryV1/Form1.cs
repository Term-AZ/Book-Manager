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

namespace BookLibraryV1
{
    public partial class Form1 : Form
    {
        AuthorTableAccessor authorTableAccessor;
        BookTableAccessor bookTableAccessor;
        FileReader fileReader;
        GenreTableAccessor genreTableAccessor;
        SQLiteConnection connection;
        private String URL = "";
        private string[] files;
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

            
            string workingDirectory = Environment.CurrentDirectory;
            URL= Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            

            if(!System.IO.File.Exists($"{URL}\\Library.db"))
            {
                SQLiteConnection.CreateFile($"{URL}\\Library.db");
            }

            connection = new SQLiteConnection($"data source = {URL}\\Library.db");
            connection.Open();
            authorTableAccessor = new AuthorTableAccessor(this, connection);
            bookTableAccessor = new BookTableAccessor(this, connection);
            genreTableAccessor = new GenreTableAccessor(this, URL, connection);
            fileReader = new FileReader(this, authorTableAccessor, bookTableAccessor, genreTableAccessor);

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
            SearchTypeComboBox.DropDownStyle= ComboBoxStyle.DropDownList;

            createTables();
            populateTreeView("default");
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {

/*
                foreach (string fileName in Directory.GetFiles("C:\\Users\\augus\\source\\repos\\BookLibraryV1\\BookLibraryV1\\Books"))
                {
                    booksTitles.Add(fileName);
                }
                DirectoryTextBox.Text = $"Files found: {booksTitles.Count()}";*/

                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    booksTitles = Directory.GetFiles(fbd.SelectedPath).ToList();
                    DirectoryTextBox.Text = $"Files found: {booksTitles.Count()}";
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

        private void AddBtn_Click(object sender, EventArgs e)
        {
            fileReader.populateTables(booksTitles);
            populateTreeView("default1");
        }
        private void createTables()
        {
            authorTableAccessor.createTable();
            bookTableAccessor.createTable();
            genreTableAccessor.createGenreTable();
            genreTableAccessor.populateGenreTable();
        }
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                populateTreeView("default");
                return;
            }
            switch (SearchTypeComboBox.Items[SearchTypeComboBox.SelectedIndex].ToString())
            {
                case "Search By Title":
                    mainNode.Text = "Titles";
                    populateTreeViewByBook(bookTableAccessor.searchBooksByTitle(SearchTextBox.Text));
                    break;
                case "Search By Author":
                    mainNode.Text = "Authors";
                    populateTreeView("byAuthor");
                    break;
                case "Search By Genre":
                    mainNode.Text = "Authors";
                    populateTreeView("byGenre");
                    break;
                case "Search By Keywords":
                    mainNode.Text = "Books";
                    populateTreeView("byKeywords");
                    break;
                case "Search By Publisher":
                    mainNode.Text = "Books";
                    populateTreeView("byPublisher");
                    break;
                case "Search By Series":
                    mainNode.Text = "Books";
                    populateTreeView("bySeries");
                    break;
            }
        }

        private void populateTreeView(String type)
        {
            Dictionary<String, TreeNode> authors = new Dictionary<string, TreeNode>();
            Dictionary<String, List<TreeNode>> books = new Dictionary<string, List<TreeNode>>();
            mainNode.Nodes.Clear();
            altMainNode.Nodes.Clear();
            switch (type)
            {
                case "default":
                    mainNode.Text = "Authors";
                    authors = authorTableAccessor.getAuthors();
                    books = bookTableAccessor.getBooks();
                    populateTreeViewByThreeLayers(authors, books);
                    break;
                case "default1":
                    mainNode.Text = "Authors";
                    authors = authorTableAccessor.getAuthors();
                    books = bookTableAccessor.getBooks();
                    populateTreeViewByThreeLayers(authors, books);
                    break;
                case "byAuthor":
                    mainNode.Text = "Authors";
                    List<String> s = new List<String>();
                    authors = authorTableAccessor.getAuthorsByName(SearchTextBox.Text);
                    foreach(KeyValuePair<String, TreeNode> t in authors)
                    {
                        s.Add(t.Key);
                    }
                    books = bookTableAccessor.searchByAuthorId(s);
                    populateTreeViewByThreeLayers(authors, books);
                    break;
                case "byGenre":
                    mainNode.Text = "Books";
                    books = bookTableAccessor.searchByGenre(SearchTextBox.Text);
                    populateTreeViewByBook(books);
                    break;
                case "byKeywords":
                    mainNode.Text = "Books";
                    books = bookTableAccessor.searchByKeywords(SearchTextBox.Text);
                    populateTreeViewByBook(books);
                    break;
                case "byPublisher":
                    mainNode.Text = "Books";
                    books = bookTableAccessor.searchByPublisher(SearchTextBox.Text);
                    populateTreeViewByBook(books);
                    break;
                case "bySeries":
                    books = bookTableAccessor.searchBySeries(SearchTextBox.Text);
                    populateTreeViewByBook(books);
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


        private void ViewBooks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            if (ViewBooks.SelectedNode.FirstNode == null && ViewBooks.SelectedNode.Parent!=null)
            {
                bookId = ViewBooks.SelectedNode.Name.Trim();
                DirectoryTextBox.Text = bookId;

                authorDetails = authorTableAccessor.getAuthor(ViewBooks.SelectedNode.Tag.ToString());
                bookDetails = bookTableAccessor.getBook(bookId);
                
                BookTitleTextBox.Text = ViewBooks.SelectedNode.Text;

                try
                {
                    AuthorFNTextBox.Text = authorDetails.ElementAt(0);
                    AuthorMNTextBox.Text = authorDetails.ElementAt(1);
                    AuthorLNTextBox.Text = authorDetails.ElementAt(2);
                }
                catch(Exception a)
                {
                    AuthorFNTextBox.Text = "";
                    AuthorMNTextBox.Text = "";
                    AuthorLNTextBox.Text = "";
                }


                AnnotationBox.Text = bookDetails["Annotation"];
                SeriesNameTextBox.Text = bookDetails["Series"];
                SeriesNumberTextBox.Text = bookDetails["SeriesNum"];
                populateGenreBox(bookDetails["Genre"]);
                CoverImage.Image = getCover(bookDetails["Image"]);
          
            }
        }
        private void populateGenreBox(String genres)
        {
            List<String> genre = genres.Split(' ').ToList();
            GenreListBox.DataSource = genre;

        }
        private Image getCover(String url)
        {
            if (url != "")
            {
                byte[] bytes = Convert.FromBase64String(url);      
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }
                return image;
            }
            return null;
        }

        private void UpdateBtnBook_Click(object sender, EventArgs e)
        {
            TreeNode currentNode = ViewBooks.SelectedNode;
            String id = ViewBooks.SelectedNode.Name.Trim();
            //TreeNode parentNode = currentNode.Parent;
            TreeNode updatedBook = bookTableAccessor.updateBook(id, BookTitleTextBox.Text);
            currentNode.Text = updatedBook.Text;
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
                    fileReader.editBook(ViewBooks.SelectedNode.Name.Trim(), fbd.SelectedPath);
                    DirectoryTextBox.Text = "File Saved";
                }

                //fileReader.editBook(ViewBooks.SelectedNode.Name.Trim(), saveFileDialog.FileName);
            }
        }

        private void updateBtnAuthor_Click(object sender, EventArgs e)
        {
            string iD = ViewBooks.SelectedNode.Parent.Name.Trim();
            ViewBooks.SelectedNode.Parent.Text = authorTableAccessor.updateAuthor(iD, AuthorFNTextBox.Text, AuthorMNTextBox.Text,AuthorLNTextBox.Text);
        }

        private void EditGenreBtn_Click(object sender, EventArgs e)
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
            bookTableAccessor.updateSeriesName(SeriesNameTextBox.Text, ViewBooks.SelectedNode.Parent.Text.Trim(),ViewBooks.SelectedNode.Parent.Tag.ToString());
            ViewBooks.SelectedNode.Parent.Text = SeriesNameTextBox.Text;
        }

        private void UpdateSeriesNumBtn_Click(object sender, EventArgs e)
        {         
            TreeNode cNode = ViewBooks.SelectedNode;
            TreeNode pNode = cNode.Parent;
            if(pNode.Tag != "Author")
            {
                bookTableAccessor.updateSeriesNum(SeriesNumberTextBox.Text, cNode.Name);
                cNode.Remove();
                pNode.Nodes.Insert(Int32.Parse(SeriesNumberTextBox.Text.Trim()), cNode);

            }
        }
    }
}
