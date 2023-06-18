using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

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
                if (imageCode == "")
                {
                    command.CommandText = "INSERT INTO COVER VALUES(@Id,@ImageCode)";
                    command.Parameters.Add(new SQLiteParameter("@Id", 0));
                    command.Parameters.Add(new SQLiteParameter("@ImageCode", imageCode));
                    command.ExecuteNonQuery();
                    return;
                }
                byte[] image = Convert.FromBase64String(imageCode);


                command.CommandText = "INSERT INTO COVER VALUES(@Id,@ImageCode)";
                command.Parameters.Add(new SQLiteParameter("@Id",0));
                command.Parameters.Add(new SQLiteParameter("@ImageCode",DbType.Binary, 20).Value = image);
                command.ExecuteNonQuery();
            }
        }

    }
}
