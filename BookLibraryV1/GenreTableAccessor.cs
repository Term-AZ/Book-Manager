using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Windows.Forms;

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
            xmlURL = $"Genres.xml";
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
        public List<String> getGenres()
        {
            List<String> s = new List<String>();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Genres ORDER BY Genre";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    s.Add(reader.GetString(1));
                }
                return s;
            }
        }
        public void addGenre(String s)
        {
            using(SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Genres VALUES(@Id,@Genre)";
                command.Parameters.Add(new SQLiteParameter("@Id", 0));
                command.Parameters.Add(new SQLiteParameter("@Genre", s));
                command.ExecuteNonQuery();
            }
        }
        public List<String> searchGenre(String filter)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                List<String> genres = new List<String>();
                command.CommandText = "SELECT Genre FROM Genres WHERE Genre LIKE '%"+filter+"%'";
                command.Parameters.Add(new SQLiteParameter("@filter", filter));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    genres.Add(reader.GetString(0));
                }
                return genres;
            }
        }
        public void editGenre(String oldName, String newName)
        {
            SQLiteDataReader reader;
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Genres WHERE Genre = @newName";
                command.Parameters.Add(new SQLiteParameter("@newName", newName));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32(0) > 0)
                    {
                        ErrorForm f = new ErrorForm("Genre with this name already exists");
                        f.ShowDialog();
                        return;
                    }
                }
                reader.Close();
                command.CommandText = "UPDATE Genres SET Genre = @newName WHERE Genre=@oldName";
                command.Parameters.Add(new SQLiteParameter("@oldName", oldName));
                command.ExecuteNonQuery();
            }
        }
        public void addCustomGenre(String name)
        {
            SQLiteDataReader reader;
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Genres WHERE Genre = @name";
                command.Parameters.Add(new SQLiteParameter("@name", name));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32(0) > 0)
                    {
                        ErrorForm f = new ErrorForm("Genre with this name already exists");
                        f.ShowDialog();
                        return;
                    }
                }
                reader.Close();
                command.CommandText = "INSERT INTO Genres VALUES(@id, @name)";
                command.Parameters.Add(new SQLiteParameter("@id", 0));
                command.ExecuteNonQuery();
            }
        }
        public void deleteGenre(String name)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Genres WHERE Genre = @name";
                command.Parameters.Add(new SQLiteParameter("@name", name));
                command.ExecuteNonQuery();
            }
        }
    }
}
