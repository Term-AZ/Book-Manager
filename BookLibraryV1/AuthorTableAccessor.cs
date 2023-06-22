using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLibraryV1
{
    public class AuthorTableAccessor
    {
        Form1 form;
        String source = "";
        SQLiteConnection connection;
        public AuthorTableAccessor(Form1 f, SQLiteConnection conn) 
        {
            form = f;
            connection = conn;
        }

        //Creates table
        public void createTable()
        {

            using(SQLiteCommand command = connection.CreateCommand()) 
            {
                //check if table exists before creating it
                String sql = "CREATE TABLE if not exists Authors (AuthorId INTEGER, Id TEXT,FirstName TEXT NOT NULL, MiddleName TEXT, LastName TEXT NOT NULL, FullName TEXT NOT NULL, CONSTRAINT PK_Authors PRIMARY KEY(AuthorId AUTOINCREMENT))";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
        }
        //Resets the table to be empty
        public void resetAuthorTable()
        {
            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                string strSql = "DELETE FROM Authors";
                cmd.CommandText = strSql;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO [Authors] VALUES(@AuthorId,@Id, @FirstName, @MiddleName, @LastName, @FullName)";
                cmd.Parameters.Add(new SQLiteParameter("@AuthorId", -1));
                cmd.Parameters.Add(new SQLiteParameter("@Id", ""));
                cmd.Parameters.Add(new SQLiteParameter("@FirstName", "No"));
                cmd.Parameters.Add(new SQLiteParameter("@MiddleName", "Author"));
                cmd.Parameters.Add(new SQLiteParameter("@LastName", "Found"));
                cmd.Parameters.Add(new SQLiteParameter("@FullName", "No Author Found"));
                cmd.ExecuteNonQuery();
            } 
        }
        public void addToAuthorTable(Dictionary<String,String> authors)
        {
            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                for (int i=0; i< authors["ID"].Count();i++)//iterates over ever dictionary in the list
                {
                    //check if author already exists
                    cmd.CommandText = "SELECT Id FROM Authors WHERE Id=@Id";
                    cmd.Parameters.Add(new SQLiteParameter("@Id", authors["ID"]));
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    
                    if (!reader.Read()) //check if author already exists in table
                    {
                        sb.Clear();
                        sb.Append($"{authors["FirstNames"]} ").Append($"{authors["MiddleNames"]} ").Append($"{authors["LastNames"]}");
                        reader.Close();
                        //if not add author to table
                        cmd.CommandText = "INSERT INTO [Authors] VALUES(@AuthorId, @Id,@FirstName,@MiddleName,@LastName,@FullName)";
                        cmd.Parameters.Add(new SQLiteParameter("@AuthorId", 0));
                        cmd.Parameters.Add(new SQLiteParameter("@Id", authors["ID"]));
                        cmd.Parameters.Add(new SQLiteParameter("@FirstName", authors["FirstNames"]));
                        cmd.Parameters.Add(new SQLiteParameter("@MiddleName", authors["MiddleNames"]));
                        cmd.Parameters.Add(new SQLiteParameter("@LastName", authors["LastNames"]));
                        cmd.Parameters.Add(new SQLiteParameter("@FullName", sb.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();
                    }
                    /* else //if exists, check if there is a valid first name and last name
                     {
                         while (reader.NextResult())
                         {
                             if (reader.GetString(1) == "")//if first name is empty and this book has the author name, add it to the author profile
                             {
                                 cmd.CommandText = "UPDATE Books SET FirstName=@FirstName WHERE Id=@Id";
                                 cmd.Parameters.Add(new SQLiteParameter("@FirstName", authors["FirstName"].ElementAt(i)));
                                 cmd.Parameters.Add(new SQLiteParameter("@Id", authors["Id"].ElementAt(i)));
                             }
                             if (reader.GetString(3) == "")//if last name is empty and this book has the author name, add it to the author profile
                             {
                                 cmd.CommandText = "UPDATE Books SET LastName=@LastName WHERE Id=@Id";
                                 cmd.Parameters.Add(new SQLiteParameter("@LastName", authors["LastName"].ElementAt(i)));
                                 cmd.Parameters.Add(new SQLiteParameter("@Id", authors["Id"].ElementAt(i)));
                             }
                         }

                     }*/

                }

            }
                     
        }
        public Dictionary<String, TreeNode> getAuthors()
        {
            using(SQLiteCommand command = connection.CreateCommand())
            {
                Dictionary<String, TreeNode> authorTreeNodes = new Dictionary<String, TreeNode>();   
                command.CommandText = "SELECT AuthorId, FullName FROM Authors ORDER BY FirstName ASC";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    //combine authors full name into one string
                    //create treenode for author. 
                    authorTreeNodes[reader.GetInt32(0).ToString()]= new TreeNode()
                    {
                        Name = reader.GetInt32(0).ToString(),
                        Text = reader.GetString(1).Trim(),
                        Tag = "Author"
                    };
                }
                return authorTreeNodes;
            }
        }
        public Dictionary<String, TreeNode> getAuthorsByNameTree(string filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                //can be done with just a list, need to know performance
                Dictionary<String, TreeNode> authorTreeNodes = new Dictionary<String, TreeNode>();
                command.CommandText = "SELECT AuthorId, FullName FROM Authors WHERE FullName LIKE '%"+filter+"%'";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authorTreeNodes[reader.GetInt32(0).ToString()] = new TreeNode()
                    {
                        Name = reader.GetInt32(0).ToString(),
                        Text = reader.GetString(1).Trim(),
                        Tag = "Author"
                    };
                }
                form.DirectoryTextBox.Text = authorTreeNodes.Count.ToString();
                return authorTreeNodes;
            }
        }
        public List<String> getAuthorByNameList(String filter)
        {
            List<String> authors = new List<String>();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT AuthorId FROM Authors WHERE FullName LIKE '%"+filter+"%'";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authors.Add(reader.GetInt32(0).ToString());
                }
                return authors;
            }
        }

        //get author details and return them as a list
        public List<String> getAuthor(String id)
        {
            using(SQLiteCommand command = connection.CreateCommand())
            {
                List<String> authorDetails=new List<String>();
                command.CommandText = "SELECT FirstName, MiddleName, LastName FROM Authors WHERE AuthorId=@Id";
                command.Parameters.Add(new SQLiteParameter("@Id", id));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authorDetails.Add(reader.GetString(0));
                    authorDetails.Add(reader.GetString(1));
                    authorDetails.Add(reader.GetString(2));
                }
                return authorDetails;
            }
 
        }
        //change author name(s) in table based on user input
        public String updateAuthor(String iD, String nFName, String nMName, String nLName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{nFName} ").Append($"{nMName} ").Append($"{nLName} ");
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Authors SET FirstName=@fName, MiddleName=@mName, LastName=@lName, FullName=@fName WHERE AuthorId=@iD";
                command.Parameters.Add(new SQLiteParameter("@iD", iD));
                command.Parameters.Add(new SQLiteParameter("@fName", nFName));
                command.Parameters.Add(new SQLiteParameter("@mName", nMName));
                command.Parameters.Add(new SQLiteParameter("@lName", nLName));
                command.Parameters.Add(new SQLiteParameter("@fName", stringBuilder.ToString()));
                command.ExecuteNonQuery();
            }
            return stringBuilder.ToString();
        } 
        public int checkAuthorLocation(String id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT AuthorId FROM Authors WHERE Id = @id";
                command.Parameters.Add(new SQLiteParameter("@Id", id));  
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                return -1;
            }
        }
        public String getAuthorBookId(String id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id FROM Authors WHERE AuthorId=@id";
                command.Parameters.Add(new SQLiteParameter("@id", id));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
                return null;
            }
        }
        public int addCustomAuthor(Dictionary<String, String> author)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT AuthorId, FullName FROM Authors WHERE FullName=@id";
                command.Parameters.Add(new SQLiteParameter("@id", author["FullName"]));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1).Equals(author["FullName"])) {
                        return reader.GetInt32(0);
                    }
                }
                reader.Close();
                command.CommandText = "INSERT INTO [Authors] VALUES(@AuthorId, @Id,@FirstName,@MiddleName,@LastName,@FullName)";
                command.Parameters.Add(new SQLiteParameter("@AuthorId", 0));
                command.Parameters.Add(new SQLiteParameter("@Id", author["Id"]));
                command.Parameters.Add(new SQLiteParameter("@FirstName", author["FirstName"]));
                command.Parameters.Add(new SQLiteParameter("@MiddleName", author["MiddleName"]));
                command.Parameters.Add(new SQLiteParameter("@LastName", author["LastName"]));
                command.Parameters.Add(new SQLiteParameter("@FullName", author["FullName"]));
                command.ExecuteNonQuery();
                return (int)connection.LastInsertRowId;
            }
        }
        public void deleteAuthor(String id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Authors WHERE AuthorId=@id";
                command.Parameters.Add(new SQLiteParameter("@id", id));
                command.ExecuteNonQuery();
            }
        }
    }
}
