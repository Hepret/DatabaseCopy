using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopying
{
    public partial class DatabaseCopy : ICopyDatabase
    {
        SqlConnection conn;
        SqlConnection connToNewDb;

        public string DbName { private set; get; }
        public string NewDbName { private set; get; }

        Queue<TableNode> tableQueue; // Order of the tables

        public DatabaseCopy(string ConnectionString, string connStrToNewDb = null, string DBName = null, string NewDbName = null)
        {

            conn = new SqlConnection(ConnectionString);

            conn.Open();

            this.DbName = DbName ?? conn.Database;

            if (conn.Database != this.DbName)
                conn.ChangeDatabase(this.DbName);
            if (connStrToNewDb is null)
            {
                this.NewDbName = NewDbName ?? String.Format($"{this.DbName}Copy");
                CreateNewDb();
            }
            else
            {
                connToNewDb = new SqlConnection(connStrToNewDb);
                this.NewDbName = conn.Database;
                connToNewDb.Open();
            }

            getTableQueue();
        }

        public void Copy()
        {
            CopySchema();
            CopyData();
        }

        
        public void CloseConnetion()
        {
            conn.Close();
            connToNewDb.Close();
            Console.WriteLine("Success");
        }


        void CreateNewDb()
        {
            String sqlExpression = string.Format($@"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{this.NewDbName}')
            BEGIN
            CREATE DATABASE {this.NewDbName};
            END ");

            SqlCommand commCreateDataBase = new SqlCommand(sqlExpression, conn);
            commCreateDataBase.ExecuteNonQuery();

            connToNewDb = new SqlConnection(conn.ConnectionString);
            connToNewDb.Open();
            connToNewDb.ChangeDatabase(this.NewDbName);
        }

        void ICopyDatabase.CopyData()
        {
            throw new NotImplementedException();
        }
    }
}
