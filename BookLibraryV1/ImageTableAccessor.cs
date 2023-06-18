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
    internal class ImageTableAccessor
    {
        Form1 form;
        SQLiteConnection connection;
        public ImageTableAccessor(Form1 f, SQLiteConnection conn)
        {
            form = f;
            connection = conn;
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
            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                string strSql = "DELETE FROM Covers";
                cmd.CommandText = strSql;
                cmd.ExecuteNonQuery();
            }
        }
        public void addToCoverTable(String imageCode)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                byte[] image = Convert.FromBase64String(imageCode);
                //SqliteBlob sqliteBlob = new SqliteBlob(connection, "Covers", "ImageCode",2);
               
                command.CommandText = "INSERT INTO [Covers] VALUES(@Id, @ImageCode)";
                command.Parameters.Add(new SQLiteParameter("@Id", 0));
                command.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@ImageCode",
                    Value = image,
                    DbType = System.Data.DbType.Binary
                });

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
                command.Parameters.Add(new SQLiteParameter("@id", Int32.Parse(id)));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    image = (byte[])reader.GetValue(0);
                }
                return image;
            }
        }

    }
}
