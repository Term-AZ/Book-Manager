using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;

namespace BookLibraryV1
{
    internal class FileReader
    {
        static Form1 form;
        static LoadingForm form2;
        static AuthorTableAccessor authorTableAccessor;
        static BookTableAccessor bookTableAccessor;
        GenreTableAccessor genreTableAccessor;
        static ImageTableAccessor imageTableAccessor;
        static Dictionary<String, String> authorList;
        static Dictionary<String, String> bookList;
        static XDocument doc;
        static XNamespace ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
        private static ManualResetEvent mre = new ManualResetEvent(false);
        private delegate void SafeCallDelegate(string text);
        public static List<String> failedFiles = new List<string>();
        public static int p = 0;
        static Thread pr;
        static int fileSize=0;

        int size = 0;

        public FileReader(Form1 f, AuthorTableAccessor dBAuthor, BookTableAccessor dBBooks, GenreTableAccessor dbGenre, ImageTableAccessor dbImage)
        {
            form = f;
            authorTableAccessor = dBAuthor;
            bookTableAccessor = dBBooks;
            genreTableAccessor = dbGenre;
            imageTableAccessor = dbImage;
        }
        public void populateTables(List<String> files)
        {
            failedFiles = new List<string>();
            fileSize = files.Count;
            Thread counterThreads = new Thread(new ParameterizedThreadStart(read.readBook));

            form2 = new LoadingForm(fileSize);
            form2.Name = "Loading";
            form2.Show();

            counterThreads.Start(files);
            form.Enabled = false;
        }
        public void editBook(List<String> ids, String directory)
        {
            foreach(String iD in ids)
            {
                Dictionary<String, String> bookDetails = bookTableAccessor.getBook(iD);
                List<String> authorDetails = authorTableAccessor.getAuthor(bookTableAccessor.getAuthorId(iD).Trim());
                String d = bookDetails["Directory"];
                if (d.Contains(".zip"))
                {
                    using (ZipArchive archive = ZipFile.OpenRead(d))
                    {
                        using (var stream = archive.Entries[Int32.Parse(bookDetails["zippedIndex"])].Open())
                        {
                            doc = XDocument.Load(stream);
                        }
                    }
                }
                else
                {
                    doc = XDocument.Load($"{bookDetails["Directory"]}");
                }
                IEnumerable<XElement> description = doc.Root.Element(ns + "description").Elements();
                IEnumerable<XElement> titleInfo = description.ElementAt(findTitleInfoIndex(description)).Elements();
                XElement t = description.ElementAt(0);
                int index = findAuthorInfoIndex(titleInfo, description);
                IEnumerable<XElement> authorInfo = titleInfo.ElementAt(index).Elements();

                foreach (XElement author in authorInfo)
                {
                    switch (author.Name.ToString())
                    {
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}first-name":
                            author.Value = authorDetails.ElementAt(0);
                            break;
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}last-name":
                            author.Value = authorDetails.ElementAt(2);
                            break;
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}middle-name":
                            author.Value = authorDetails.ElementAt(1);
                            break;
                        default:
                            break;
                    }
                }
                //garbage code to remove, needs to be changed later
                for (int i = titleInfo.Count() - 1; i >= 0; i--)
                {
                    switch (titleInfo.ElementAt(i).Name.ToString())
                    {
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}genre":
                            titleInfo.ElementAt(i).Remove();
                            break;
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}book-title":
                            titleInfo.ElementAt(i).Value = bookDetails["Title"];
                            break;
                        default: break;
                    }
                }
                foreach (String newGenre in bookDetails["Genre"].Trim().Split(' '))
                {
                    t.AddFirst(new XElement("Genre", newGenre));
                }

                doc.Save($"{directory}/{bookDetails["Title"]}.fb2");
            }
        }


        public static int findTitleInfoIndex(IEnumerable<XElement> l)
        {
            for (int i = 0; i < l.Count(); i++)
            {
                if (l.ElementAt(i).Name.ToString() == "{http://www.gribuser.ru/xml/fictionbook/2.0}title-info")
                {
                    return i;
                }
            }
            return -1;
        }
        public static int findAuthorInfoIndex(IEnumerable<XElement> l, IEnumerable<XElement> x)
        {
            for (int i = 0; i < l.Count(); i++)
            {
                if (l.ElementAt(i).Name.ToString() == "{http://www.gribuser.ru/xml/fictionbook/2.0}author")
                {
                    return i;
                }
            }
            for (int i = 0; i < x.Count(); i++)
            {
                if (l.ElementAt(i).Name.ToString() == "{http://www.gribuser.ru/xml/fictionbook/2.0}author")
                {
                    return i;
                }
            }
            return 0;
        }
        public static int findPublisherInfoIndex(IEnumerable<XElement> l)
        {
            for (int i = 0; i < l.Count(); i++)
            {
                if (l.ElementAt(i).Name.ToString() == "{http://www.gribuser.ru/xml/fictionbook/2.0}publish-info")
                {
                    return i;
                }
            }
            return 0;
        }


        class read
        {
            static int counter = 0;
            static LocalDataStoreSlot localSlot;
            static read()
            {
                localSlot = Thread.AllocateDataSlot();
            }
            public static void readBook(object l)
            {
                var books = ((IEnumerable)l).Cast<object>().ToList();
                
                foreach (string f in books)
                {
                    addingbook(f);
                }
                foreach(String s in failedFiles)
                {
                    form.FailedURLs.Invoke(new Action(() =>
                        form.FailedURLs.Text += $"{s}\n"
                ));
                }
                form2.progressLbl.Invoke(new Action(() =>
                    form2.progressLbl.Text = $"Successfully read {counter} files, application can now be closed"
                    ));
                form2.label1.Invoke(new Action(() =>
                    form2.label1.Hide()
                ));
                form.Invoke(new Action(() =>
                    form.Enabled = true
                ));
            }
            private static void addingbook(string f)
            {
                FileInfo fi = new FileInfo(f);
                String currentZippedPath = "";
                try
                {
                    String[] a = f.Split('\\').ToArray<String>();
                    if (fi.Extension == ".fb2" && !a[a.Length - 1].Contains(".zip"))
                    {
                        doc = XDocument.Load($"{f}"); //load file
                        addBooksToTable(doc, f);
                    }
                    else if (a[a.Length - 1].Contains(".zip"))
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(f))
                        {
                            int x = 0;
                            fileSize += archive.Entries.Count;
                            foreach (ZipArchiveEntry i in archive.Entries)
                            {
                                currentZippedPath = $"{f}\\{i.FullName}";
                                FileInfo nFi = new FileInfo(i.FullName);


                                if (nFi.Extension == ".zip")
                                {
                                    addingbook($"{f}\\{i.Name}");
                                }
                                else
                                {
                                    using (var stream = i.Open())
                                    {
                                        doc = XDocument.Load(stream);
                                        addBooksToTable(doc, $"{f}", x);
                                    }
                                    x++;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    failedFiles.Add(currentZippedPath);
                }
            }
            private static void addBooksToTable(XDocument doc, String f, int i = -1)
            {
                IEnumerable<XElement> description = Enumerable.Empty<XElement>();
                IEnumerable<XElement> titleInfo = Enumerable.Empty<XElement>();
                IEnumerable<XElement> publisherInfo = Enumerable.Empty<XElement>();
                IEnumerable<XElement> authorInfo = Enumerable.Empty<XElement>();
                IEnumerable<XElement> image = Enumerable.Empty<XElement>();
                int index = 0;
                String imageUrl = "";
                List<String> tags;

                StringBuilder sb = new StringBuilder("");
                authorList = new Dictionary<String, String>
                {
                    //create new list and new index
                    { "FirstNames", "" },
                    { "LastNames", "" },
                    { "MiddleNames", ""},
                    { "ID", "" }
                };

                bookList = new Dictionary<String, String>
                {
                    { "Title", "" },
                    { "AuthorId", "" },
                    { "Series", "" },
                    { "SeriesNum", "0" },
                    { "Directory", "" },
                    { "Genre", "" },
                    { "Keywords", "" },
                    { "Annotation", "" },
                    { "Publisher", "" },
                    { "ImageId", "" },
                    { "zippedIndex", i.ToString() }
                };
                try
                {
                    XElement elements = doc.Root.Element(ns + "description");//gets description node
                    image = doc.Root.Elements(ns + "binary");
                    description = elements.Elements(); //all elements under description
                    titleInfo = description.ElementAt(findTitleInfoIndex(description)).Elements();//opens title info element as it is the first node
                    publisherInfo = description.ElementAt(findPublisherInfoIndex(description)).Elements();
                    index = findAuthorInfoIndex(titleInfo, description);
                    authorInfo = titleInfo.ElementAt(index).Elements();

                    tags = new List<String>();
                    foreach (XElement authorElements in authorInfo)
                    {
                        switch (authorElements.Name.ToString())
                        {
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}first-name":
                                authorList["FirstNames"] = (authorElements.Value);
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}last-name":
                                if (authorElements.ElementsBeforeSelf().Count() < 2)
                                {
                                    authorList["MiddleNames"] = ("");
                                }
                                authorList["LastNames"] = (authorElements.Value);
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}middle-name":
                                authorList["MiddleNames"] = (authorElements.Value);
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}id":
                                authorList["ID"] = (authorElements.Value);
                                break;
                            default:
                                break;
                        }
                    }

                    foreach (XElement bookElements in titleInfo)
                    {
                        switch (bookElements.Name.ToString())
                        {
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}book-title":
                                bookList["Title"] = (bookElements.Value);
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}sequence":
                                if (bookElements.Attribute("name") != null)
                                {
                                    bookList["Series"] = bookElements.Attribute("name").Value;
                                }
                                if (bookElements.Attribute("number") != null)
                                {
                                    bookList["SeriesNum"] = bookElements.Attribute("number").Value;
                                }
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}genre":
                                sb.Append(bookElements.Value + " ");
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}keywords":
                                bookList["Keywords"] = (bookElements.Value);
                                break;
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}annotation":
                                bookList["Annotation"] = (bookElements.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (XElement publisherElement in publisherInfo)
                    {
                        switch (publisherElement.Name.ToString())
                        {
                            case "{http://www.gribuser.ru/xml/fictionbook/2.0}publisher":
                                bookList["Publisher"] = publisherElement.Value;
                                break;
                            default: break;
                        }
                    }
                    bookList["Directory"] = f;
                    bookList["Genre"] = sb.ToString();
                    foreach (XElement q in image)
                    {
                        if (q.Attribute("id").Value == "cover.jpg")
                        {
                            imageUrl = q.Value;
                            break;
                        }
                    }
                    authorTableAccessor.addToAuthorTable(authorList);
                    bookList["AuthorId"] = authorTableAccessor.checkAuthorLocation(authorList["ID"]).ToString();
                    imageTableAccessor.addToCoverTable(imageUrl);
                    bookList["ImageId"] = imageTableAccessor.getRecentAdded().ToString();
                    bookTableAccessor.addBook(bookList);
                    counter++;
                    form2.progressLbl.Invoke(new Action(() =>
                        form2.progressLbl.Text = $"Progress: {counter}/{fileSize}"
                        ));
                    form2.progressBar1.Invoke(new Action(() =>
                        form2.progressBar1.Value = counter
                        ));
                }
                catch
                {
                    failedFiles.Add(f);
                }
            }
        }
    }
}

