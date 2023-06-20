using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Xml.Linq;

namespace BookLibraryV1
{
    public class GenreTableAccessor
    {
        Form1 form;
        String xmlURL;
        SQLiteConnection connection;
        List<String> genres = new List<String>();
        public GenreTableAccessor(Form1 forms, string url, SQLiteConnection conn)
        {
            form = forms;
            connection = conn;
            xmlURL = $"{url}\\BookLibraryV1\\Genres.xml";
        }

        public void createGenreTable()
        {
            //create table
            using (SQLiteCommand command = connection.CreateCommand())
            {
                String sql = "CREATE TABLE if not exists Genres (Id INTEGER NOT NULL, Genre TEXT NOT NULL ,CONSTRAINT PK_Genre PRIMARY KEY('Id'))";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
        }

        public void populateGenreTable()
        {

            using(SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Genres";
                SQLiteDataReader rdr = command.ExecuteReader();
                //check if table is empty
                if(!rdr.Read()) 
                {
                    XDocument doc = XDocument.Load($"{xmlURL}");
                    int i = 0;
                    //if table is empty iterate over every entry in xml doc and add it to the table
                    XElement elements = doc.Root.Element("genre");
                    IEnumerable<XElement> genres = elements.Elements();
                    rdr.Close();
                    foreach (XElement genre in genres)
                    {
                        command.CommandText = "INSERT INTO [Genres] VALUES (@Id,@Genre)";
                        command.Parameters.Add(new SQLiteParameter("Id", i++));
                        command.Parameters.Add(new SQLiteParameter("@Genre", genre.Name.ToString()));
                        command.ExecuteNonQuery();
                    }
                }
            }
                
            
        }
        //Unnecessary code
        /*public HashSet<String> getGenreTags(List<String> tag)
        {
            using(SQLiteConnection connection = new SQLiteConnection($"{source}")) 
            {
                using( SQLiteCommand command = connection.CreateCommand())
                {
                    HashSet<String> tags = new HashSet<String>();
                    connection.Open();
                    foreach(String genre in tag)
                    {
                        command.CommandText = ("SELECT Genre FROM [Genres] WHERE Genre LIKE '%" + genre + "%'");
                        SQLiteDataReader rdr = command.ExecuteReader();
                        while(rdr.Read())
                        {
                            tags.Add(rdr.GetString(0));
                        }
                    }
                    connection.Close();
                    return tags;

                }
            }
        }*/
    }
}
