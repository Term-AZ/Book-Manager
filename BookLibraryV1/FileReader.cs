using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using System.Collections;

namespace BookLibraryV1
{
    internal class FileReader
    {
        static Form1 form;
        static AuthorTableAccessor authorTableAccessor;
        static BookTableAccessor bookTableAccessor;
        GenreTableAccessor genreTableAccessor;
        static Dictionary<String, String> authorList;
        static Dictionary<String , String> bookList;
        static XDocument doc;
        static XNamespace ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
        private static ManualResetEvent mre = new ManualResetEvent(false);
        private delegate void SafeCallDelegate(string text);
        static List<String> failedFiles = new List<string>();
        public static int i = 0;

        public FileReader(Form1 f, AuthorTableAccessor dBAuthor, BookTableAccessor dBBooks, GenreTableAccessor dbGenre) {
            form = f;
            authorTableAccessor = dBAuthor;
            bookTableAccessor = dBBooks;
            genreTableAccessor = dbGenre;
        } 
        public void populateTables(List<String> files)
        {
            failedFiles = new List<string>();
            Thread counterThreads = new Thread(new ParameterizedThreadStart(readBooks));
            Thread progressBar = new Thread(new ParameterizedThreadStart(progress));

            Form2 f = new Form2();
            f.Show();

            counterThreads.Start(files);
            //progressBar.Start(files.Count);
            counterThreads.Join();

            
        }
        public void editBook(String iD, String directory)
        {
            Dictionary<String, String> bookDetails = bookTableAccessor.getBook(iD);
            List<String> authorDetails = authorTableAccessor.getAuthor(bookTableAccessor.getAuthorId(iD).Trim());
            doc = XDocument.Load($"{bookDetails["Directory"]}");
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
            for(int i = titleInfo.Count()-1; i>=0;i--)
            {
                switch (titleInfo.ElementAt(i).Name.ToString())
                {
                    case "{http://www.gribuser.ru/xml/fictionbook/2.0}genre":
                        titleInfo.ElementAt(i).Remove();
                        break;
                    case "{http://www.gribuser.ru/xml/fictionbook/2.0}book-title":
                        titleInfo.ElementAt(i).Value = bookDetails["Title"];
                        break;
                    default : break;
                }
            }
            foreach(String newGenre in bookDetails["Genre"].Trim().Split(' '))
            { 
                t.AddFirst(new XElement("Genre", newGenre));
            }
            
            doc.Save($"{directory}/{bookDetails["Title"]}.fb2");
        }


        public static int findTitleInfoIndex(IEnumerable<XElement> l)
        {
            for(int i=0; i<l.Count();i++)
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
            for(int i=0;i < l.Count(); i++)
            {
                if(l.ElementAt(i).Name.ToString()== "{http://www.gribuser.ru/xml/fictionbook/2.0}author")
                {
                    return i;
                }
            }
            for(int i=0; i < x.Count(); i++)
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
        private static void readBooks(object l ){
            var books = ((IEnumerable)l).Cast<object>().ToList();
            i = 0;
            authorTableAccessor.resetAuthorTable();
            bookTableAccessor.resetBookTable();


            List<String> tags;
            foreach (string file in books)
            {
                mre.Reset();
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
                    { "AuthorID", "" },
                    { "Series", "" },
                    { "SeriesNum", "" },
                    { "Directory", "" },
                    { "Genre", "" },
                    { "Keywords", "" },
                    { "Annotation", "" },
                    { "Publisher", "" },
                    { "ImageURL", "" },
                };
                try
                {
                    doc = XDocument.Load($"{file}"); //load file
                }
                catch (Exception e)
                {
                    failedFiles.Add($"{file}");
                }

                XElement elements = doc.Root.Element(ns + "description");//gets description node
                IEnumerable<XElement> image = doc.Root.Elements(ns + "binary");

                //IEnumerable<XElement> image = (from el in doc.Elements("binary") where (string)el.Attribute("id") == "cover.jpg" select el);
                IEnumerable<XElement> description = elements.Elements(); //all elements under description
                IEnumerable<XElement> titleInfo = description.ElementAt(findTitleInfoIndex(description)).Elements();//opens title info element as it is the first node
                IEnumerable<XElement> publisherInfo = description.ElementAt(findPublisherInfoIndex(description)).Elements();
                int index = findAuthorInfoIndex(titleInfo, description);
                IEnumerable<XElement> authorInfo = titleInfo.ElementAt(index).Elements();


                tags = new List<String>();
                foreach (XElement authorElements in authorInfo)
                {
                    switch (authorElements.Name.ToString())
                    {
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}first-name":
                            authorList["FirstNames"] = (authorElements.Value);
                            //fFirstName = true;
                            break;
                        case "{http://www.gribuser.ru/xml/fictionbook/2.0}last-name":
                            if (authorElements.ElementsBeforeSelf().Count() < 2)
                            {
                                authorList["MiddleNames"] = ("");
                            }
                            authorList["LastNames"] = (authorElements.Value);
                            //fLastName = true;
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
                            else
                            {
                                bookList["SeriesNum"] = "0";
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
                bookList["Directory"] = file;
                if (authorList["ID"] == "")
                {
                    bookList["AuthorID"] = (authorList["LastNames"]);
                }
                else
                {
                    bookList["AuthorID"] = (authorList["ID"]);
                }
                bookList["Genre"] = sb.ToString();
                foreach (XElement i in image)
                {
                    if (i.Attribute("id").Value == "cover.jpg")
                    {
                        bookList["ImageURL"] = i.Value;
                        break;
                    }
                }
                i++;
                authorTableAccessor.addToAuthorTable(authorList);
                bookTableAccessor.addBook(bookList);
            }
        }
        private static void progress(object s)
        {
            int size = Convert.ToInt32(s);
            for (int i = 0; i <= size; i++)
            {
                form.ProgressLbl.Invoke(new Action(() =>
                    form.ProgressLbl.Text = $"Reading Books {i} / {s}"
                ));
                Thread.Sleep(100);
            }
        }
        
        private static void safeText(String text)
        {
            if (form.ProgressLbl.InvokeRequired)
            {
                var d = new SafeCallDelegate(safeText);
                form.ProgressLbl.Invoke(d, new Object[] { text });
            }
            else
            {
                form.ProgressLbl.Text = text;
            }
        }

    }


}
