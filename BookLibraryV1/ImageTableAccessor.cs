using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace BookLibraryV1
{
    public class ImageTableAccessor
    {
        Form1 form;
        SQLiteConnection connection;
        String URL;
        public ImageTableAccessor(Form1 f, SQLiteConnection conn, String uRL)
        {
            form = f;
            connection = conn;
            URL = uRL;
        }
        public void createTable()
        {

            using (SQLiteCommand command = connection.CreateCommand())
            {
                //check if table exists before creating it
                String sql = "CREATE TABLE if not exists Covers (Id INTEGER NOT NULL, ImageCode BLOB,  CONSTRAINT PK_Covers PRIMARY KEY(Id AUTOINCREMENT))";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
        public void resetCoverTable()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                string strSql = "DELETE FROM Covers";
                command.CommandText = strSql;
                command.ExecuteNonQuery();

                byte[] image = File.ReadAllBytes($"{URL}\\defaultImage.jpg");

                command.CommandText = "INSERT INTO Covers VALUES (@Id, @ImageCode)";
                command.Parameters.Add(new SQLiteParameter("@Id", -1));
                command.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@ImageCode",
                    Value = image,
                    DbType = System.Data.DbType.Binary
                });
                command.ExecuteNonQuery();
            }
        }
        public void addToCoverTable(String imageCode)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                byte[] image = Convert.FromBase64String(imageCode);
                command.CommandText = "INSERT INTO [Covers] VALUES(@Id, @ImageCode)";
                command.Parameters.Add(new SQLiteParameter("@Id", 0));
                if (image.Length == 0)
                {
                    command.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@ImageCode",
                        Value = null,
                        DbType = System.Data.DbType.Binary
                    });
                }
                else
                {
                    command.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@ImageCode",
                        Value = image,
                        DbType = System.Data.DbType.Binary
                    });
                }

               /* SQLiteParameter param = new SQLiteParameter("@ImageCode", System.Data.DbType.Binary);
                param.Value = image;*/
                //command.Parameters.Add(param);
                //ommand.Parameters.Add(new SQLiteParameter("@ImageCode",DbType.Binary).Value = image);*/
                command.ExecuteNonQuery();
            }
        }
        public byte[] getCover(String id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                byte[] image = { };
                command.CommandText = "SELECT ImageCode FROM Covers WHERE Id=@id";
                SQLiteParameter parameter = new SQLiteParameter("@id",id);
                command.Parameters.Add(parameter);
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        return (byte[])reader.GetValue(0);
                    }
                    return image;
                }
                catch
                {
                    reader.Close();
                    command.Parameters[0].Value = -1;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return (byte[])reader.GetValue(0);
                    }
                    return image;
                }
            }
        }
        public int getRecentAdded()
        {
            return (int)connection.LastInsertRowId;
        }
        public void updateCover(String URL, int ID)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                byte[] image = File.ReadAllBytes(URL);
                command.CommandText = "UPDATE Covers SET ImageCode=@image WHERE Id=@ID";
                command.Parameters.Add(new SQLiteParameter("@ID", ID));
                command.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@image",
                    Value = image,
                    DbType = System.Data.DbType.Binary
                });
                command.ExecuteNonQuery();
            }
        }
    }
}
