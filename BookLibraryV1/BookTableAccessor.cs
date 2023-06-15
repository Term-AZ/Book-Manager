﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BookLibraryV1
{
    internal class BookTableAccessor
    {
        Form1 form;
        SQLiteConnection connection;
        FileReader fileReader;
        public BookTableAccessor(Form1 forms, SQLiteConnection conn)
        {
            form = forms;
            connection = conn;
        }
        /// <summary>
        /// Creates book table
        /// </summary>
        public void createTable()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                String sql = "CREATE TABLE if not exists Books (Id INTEGER NOT NULL, Title TEXT NOT NULL,AuthorID TEXT NOT NULL,Series TEXT, SeriesNum TEXT, Directory TEXT NOT NULL,Genre TEXT NOT NULL,Keywords TEXT,Annotation TEXT NOT NULL,Publisher TEXT NOT NULL,Image TEXT,CONSTRAINT PK_Books PRIMARY KEY(Id AUTOINCREMENT), FOREIGN KEY(AuthorID) REFERENCES Authors(Id))";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
           
        }
        /// <summary>
        /// Resets table
        /// </summary>
        public void resetBookTable()
        {
            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                string strSql = "DELETE FROM Books";
                cmd.CommandText = strSql;
                cmd.ExecuteNonQuery();
            }
            
        }

        /// <summary>
        /// Adds book entry to the table. 
        /// </summary>
        /// <param name="book"></param>
        public void addBook(Dictionary<String, String> book)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO [Books] VALUES (@Id,@Title,@AuthorID,@Series,@SeriesNum,@Directory,@Genre,@Keywords,@Annotation,@Publisher,@Image)";
                command.Parameters.Add(new SQLiteParameter("@Id",0));
                command.Parameters.Add(new SQLiteParameter("@Title", book["Title"]));
                command.Parameters.Add(new SQLiteParameter("@AuthorID", book["AuthorID"]));
                command.Parameters.Add(new SQLiteParameter("@Series", book["Series"]));
                command.Parameters.Add(new SQLiteParameter("@SeriesNum", book["SeriesNum"]));
                command.Parameters.Add(new SQLiteParameter("@Directory", book["Directory"]));
                command.Parameters.Add(new SQLiteParameter("@Genre", book["Genre"]));
                command.Parameters.Add(new SQLiteParameter("@Keywords", book["Keywords"]));
                command.Parameters.Add(new SQLiteParameter("@Annotation", book["Annotation"]));
                command.Parameters.Add(new SQLiteParameter("@Publisher", book["Publisher"]));
                command.Parameters.Add(new SQLiteParameter("@Image", book["ImageURL"]));
                command.ExecuteNonQuery();
            }

        }
        /// <summary>
        /// gets all books from the table and adds them to a dictionary. Dictionary key is teh author id and the value is a list of books
        /// linked to the author. If the book is in a series, create a series treenode instead and add the book to the series treenode. 
        /// </summary>
        /// <returns>Dictrionaryy of treeNode Lists</returns>
        public Dictionary<String,List<TreeNode>> getBooks()
        {
            using(SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<string, string>>();
                command.CommandText = "SELECT Id, Title, AuthorId, Series FROM [Books] ORDER BY Series,SeriesNum";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bD.Add(new Dictionary<string, string>
                    {
                        {"Id", reader.GetInt32(0).ToString() },
                        {"Title", reader.GetString(1) },
                        {"AuthorId", reader.GetString(2) },
                        {"Series", reader.GetString(3) },
                    });
                }
                reader.Close();
                return formatBookNodes(bD);
            }

        }
        public Dictionary<String, List<TreeNode>> searchByAuthorId(List<String> ids)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<string, string>>();
                SQLiteDataReader reader;
                command.CommandText = "SELECT Id, Title, SERIES FROM [Books] WHERE AuthorId = @id ORDER BY Series,SeriesNum";
                command.Parameters.Add(new SQLiteParameter("@id", ""));
                foreach (String i in ids)
                {
                    command.Parameters["@id"].Value = i.Trim();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        bD.Add(new Dictionary<string, string>
                        {
                            {"Id", reader.GetInt32(0).ToString() },
                            {"Title", reader.GetString(1) },
                            {"AuthorId", i },
                            {"Series", reader.GetString(2) },
                        });
                    }
                    reader.Close();
                }
                return formatBookNodes(bD);
            }
        }
        public Dictionary<String, List<TreeNode>> searchByGenre(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<String, String>>();
                command.CommandText = "SELECT Id, Title, AuthorId, Series FROM Books WHERE Genre LIKE '%" + filter + "%' ORDER BY Series,SeriesNum";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bD.Add(new Dictionary<string, string>
                    {
                        { "Id", reader.GetInt32(0).ToString()},
                        { "Title", reader.GetString(1)},
                        { "AuthorId", reader.GetString(2)},
                        { "Series", reader.GetString(3)}
                    });
                }
                return formatBookNodes(bD);
            }
        }
        public Dictionary<String, List<TreeNode>> searchByKeywords(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<String, String>>();
                command.CommandText = "SELECT Id, Title, AuthorId, Series FROM Books WHERE Keywords LIKE '%" + filter + "%' ORDER BY Series,SeriesNum";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bD.Add(new Dictionary<string, string>
                    {
                        { "Id", reader.GetInt32(0).ToString()},
                        { "Title", reader.GetString(1)},
                        { "AuthorId", reader.GetString(2)},
                        { "Series", reader.GetString(3)}
                    });
                }
                return formatBookNodes(bD);
            }
        }
        public Dictionary<String, List<TreeNode>> searchByPublisher(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<String, String>>();
                command.CommandText = "SELECT Id, Title, AuthorId, Series FROM Books WHERE Publisher LIKE '%" + filter + "%' ORDER BY Series,SeriesNum";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bD.Add(new Dictionary<string, string>
                    {
                        { "Id", reader.GetInt32(0).ToString()},
                        { "Title", reader.GetString(1)},
                        { "AuthorId", reader.GetString(2)},
                        { "Series", reader.GetString(3)}
                    });
                }
                return formatBookNodes(bD);
            }
        }
        public Dictionary<String, List<TreeNode>> searchBySeries(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<Dictionary<String, String>> bD = new List<Dictionary<String, String>>();
                command.CommandText = "SELECT Id, Title, AuthorId, Series FROM Books WHERE Series LIKE '%" + filter + "%' ORDER BY Series,SeriesNum";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bD.Add(new Dictionary<string, string>
                    {
                        { "Id", reader.GetInt32(0).ToString()},
                        { "Title", reader.GetString(1)},
                        { "AuthorId", reader.GetString(2)},
                        { "Series", reader.GetString(3)}
                    });
                }
                return formatBookNodes(bD);
            }
        }

        public Dictionary<String, List<TreeNode>> formatBookNodes(List<Dictionary<String,String>> l)
        {
            Dictionary<String, List<TreeNode>> bookDetails = new Dictionary<String, List<TreeNode>>();
            foreach(Dictionary<String,String> d in l)
            {
                //check if author key already exists in dictionary, if not add it
                if (!bookDetails.ContainsKey(d["AuthorId"]))
                {
                    bookDetails[d["AuthorId"]] = new List<TreeNode>();
                }
                //check if book is in a series
                if (d["Series"] != "")
                {
                    Boolean found = false;
                    //go over the current treenodes linked to the author
                    foreach (TreeNode t in bookDetails[d["AuthorId"]])
                    {
                        //if series node is found, add the book to it
                        if (t.Text == d["Series"].Trim())
                        {
                            found = true;
                            t.Nodes.Add(new TreeNode()
                            {
                                Name = d["Id"],
                                Text = d["Title"],
                                Tag = d["AuthorId"]
                            });
                            break;
                        }
                    }
                    //if a node wasnt found, create a node and add the book to it
                    if (!found)
                    {
                        //create series treenode
                        bookDetails[d["AuthorId"]].Add(new TreeNode()
                        {
                            Name = d["AuthorId"],
                            Text = d["Series"],
                            Tag = d["AuthorId"]
                        });
                        //add book node to the series node
                        bookDetails[d["AuthorId"]].Last().Nodes.Add(new TreeNode()
                        {
                            Text = d["Title"],
                            Name = d["Id"],
                            Tag = d["AuthorId"]
                        });
                    }
                }
                else
                {//book isnt in a series, just add it to the author node
                    bookDetails[d["AuthorId"]].Add(new TreeNode()
                    {
                        Text = d["Title"],
                        Name = d["Id"],
                        Tag = d["AuthorId"]
                    });
                }
            }
            return bookDetails;
           
        }
        /// <summary>
        /// Gets book details to dispaly in the UI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<String, String> getBook(String id)
        {
            using( SQLiteCommand command = connection.CreateCommand()) 
            {
                Dictionary<String,String> bookDetails = new Dictionary<String, String>();
                command.CommandText = "SELECT Title, Series, Directory, Genre, Annotation, Publisher, Image, SeriesNum FROM Books WHERE Id=@Id";
                command.Parameters.Add(new SQLiteParameter("@Id", Int32.Parse(id)));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    bookDetails["Title"] = reader.GetString(0);
                    bookDetails["Series"] = reader.GetString(1);
                    bookDetails["Directory"] = reader.GetString(2);
                    bookDetails["Genre"] = reader.GetString(3);
                    bookDetails["Annotation"] = reader.GetString(4);
                    bookDetails["Publisher"] = reader.GetString(5);
                    bookDetails["Image"] = reader.GetString(6);
                    bookDetails["SeriesNum"] = reader.GetString(7);
                }
                return bookDetails;
            }
        }
        /// <summary>
        /// Updates book entry in table
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public TreeNode updateBook(String iD, String title)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Books SET Title=@title WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@title", title));
                command.Parameters.Add(new SQLiteParameter("@Id", Int32.Parse(iD)));
                command.ExecuteNonQuery();
                return new TreeNode
                {
                    Name = iD,
                    Text = title
                };
            }
        }
        /// <summary>
        /// gets directory of the book. used when saving a book.
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public String getDirectory(String iD)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Directory FROM Books WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
                return null;
            }
        }
        /// <summary>
        /// gets author ID from the entry
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public String getAuthorId(String iD)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT AuthorId FROM Books WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
                return "";
            }
        }
        /// <summary>
        /// Changes the genre name to a new name the user inputed.
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="nGenreName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public String editGenre(String iD, string nGenreName, int index)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<String> genres = new List<String>();
                StringBuilder sb = new StringBuilder();
                //get genres from table
                command.CommandText = "SELECT Genre FROM Books WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //split genres into array
                    genres = reader.GetString(0).Trim().Split(' ').ToList();
                }
                //change genre name
                genres[index]=nGenreName;
                //combine genres into single text and update book entry with new genres/
                for (int i = 0; i < genres.Count(); i++)
                {
                    sb.Append($"{genres[i]} ");
                }
                reader.Close();
                command.CommandText = "UPDATE Books SET Genre=@g WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@g", sb.ToString()));
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                command.ExecuteNonQuery();

                return sb.ToString();
            }
        }
        /// <summary>
        /// adds a genre to the book
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="nGenreName"></param>
        /// <returns></returns>
        public String addGenre(String iD, string nGenreName)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                command.CommandText = "SELECT Genre FROM Books WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                SQLiteDataReader reader = command.ExecuteReader();
                //appends new genre to the end of the genre text
                while (reader.Read())
                {
                    sb.Append(reader.GetString(0).Trim());
                }
                sb.Append($" {nGenreName}");
                reader.Close();
                //update genre entry
                command.CommandText = "UPDATE Books SET Genre=@g WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@g", sb.ToString()));
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                command.ExecuteNonQuery();

                return sb.ToString();
            }
        }
        /// <summary>
        /// deletes genre from genre entry
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public String deleteGenre(String iD, int index)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<String> genres = new List<string>();
                StringBuilder sb = new StringBuilder();
                //get all genres
                command.CommandText = "SELECT Genre FROM Books WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                SQLiteDataReader reader = command.ExecuteReader();
                //split string in list
                while (reader.Read())
                {
                    genres = reader.GetString(0).Trim().Split(' ').ToList();
                }
                //remove string at specified index
                genres.RemoveAt(index);
                //recombine string and update entry in table
                foreach (String genre in genres)
                {
                    sb.Append($"{genre} ");
                }
                reader.Close();
                command.CommandText = "UPDATE Books SET Genre=@g WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@g", sb.ToString()));
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(iD)));
                command.ExecuteNonQuery();

                return sb.ToString();
            }
        }
        public List<TreeNode> searchBooksByTitle(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                command.CommandText = "SELECT Id, Title, AuthorId FROM Books WHERE Title LIKE '%" + filter + "%'";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    treeNodes.Add(new TreeNode()
                    {
                        Name = reader.GetInt32(0).ToString(),
                        Text = reader.GetString(1),
                        Tag = reader.GetString(2)
                    });
                }
                return treeNodes;
            }
        }
        public void updateSeriesName(String nName, String cName, String authorId)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Books SET Series=@s WHERE Series=@oS AND AuthorId=@id";
                command.Parameters.Add(new SQLiteParameter("@s", nName));
                command.Parameters.Add(new SQLiteParameter("@oS", cName));
                command.Parameters.Add(new SQLiteParameter("@id", authorId));
                command.ExecuteNonQuery();
            }
        }
        public void updateSeriesNum(String nNum, String bookId)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Books SET SeriesNum=@nN WHERE Id=@id";
                command.Parameters.Add(new SQLiteParameter("@nN", nNum));
                command.Parameters.Add(new SQLiteParameter("@Id", Int32.Parse(bookId)));
                command.ExecuteNonQuery();
            }
        }
    }
}




/*//check if author key already exists in dictionary, if not add it
                    if (!bookDetails.ContainsKey(reader.GetString(2)))
                    {
                        bookDetails[reader.GetString(2)] = new List<TreeNode>();
                    }
                    //check if book is in a series
                    if (reader.GetString(3) != "")
                    {
                        Boolean found = false;
                        //go over the current treenodes linked to the author
                        foreach(TreeNode t in bookDetails[reader.GetString(2)])
                        {
                            //if series node is found, add the book to it
                            if(t.Text == reader.GetString(3).Trim())
                            {
                                found = true;
                                t.Nodes.Add(new TreeNode()
                                {
                                    Name = reader.GetInt32(0).ToString(),
                                    Text = reader.GetString(1),
                                    Tag = reader.GetString(2)
                                }) ;
                                break;
                            }
                        }
                        //if a node wasnt found, create a node and add the book to it
                        if (!found)
                        {
                            //create series treenode
                            bookDetails[reader.GetString(2)].Add(new TreeNode()
                            {
                                Name=reader.GetString(2),
                                Text = reader.GetString(3),
                                Tag = reader.GetString(2)
                            });
                            //add book node to the series node
                            bookDetails[reader.GetString(2)].Last().Nodes.Add(new TreeNode()
                            {
                                Text = reader.GetString(1),
                                Name = reader.GetInt32(0).ToString(),
                                Tag = reader.GetString(2)
                            });
                        }
                    }
                    else{//book isnt in a series, just add it to the author node
                        bookDetails[reader.GetString(2)].Add(new TreeNode()
                        {
                            Text = reader.GetString(1),
                            Name = reader.GetInt32(0).ToString(),
                            Tag = reader.GetString(2)
                        });
                    }*/



//OLD CODE TO CREATE TREES
/*                Dictionary<String, List<TreeNode>> bookDetails = new Dictionary<String, List<TreeNode>>();
                command.CommandText = "SELECT Id, Title, AuthorId FROM [Books]";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!bookDetails.ContainsKey(reader.GetString(2)))
                    {
                        bookDetails[reader.GetString(2)] = new List<TreeNode>();
                    }
                    bookDetails[reader.GetString(2)].Add(new TreeNode() {
                        Name = reader.GetInt32(0).ToString(),
                        Text = reader.GetString(1)
                    });
                }
                return bookDetails;  */