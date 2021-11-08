using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopying
{
    public partial class DatabaseCopy
    {
        public void CopyData()
        {

            foreach (TableNode table in tableQueue)
            {
                string sql = $"SELECT * FROM  {table.Name}";

                // Создаем объект DataAdapter
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                // Создаем объект Dataset
                DataSet ds = new DataSet();
                // Заполняем Dataset
                adapter.Fill(ds);

                DataTable dataTable = ds.Tables[0];
                // Create column string
                List<string> tableColumnsList = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    tableColumnsList.Add(column.ColumnName);
                }
                string tableColumns = string.Join(",", tableColumnsList);

                string sqlExpression = "INSERT INTO\n"
                    + $"{table.Name}({tableColumns})\n"
                    + "VALUES\n";

                foreach (DataRow row in dataTable.Rows)
                {

                    List<string> rowList = new List<string>();
                    foreach (object item in row.ItemArray)
                    {
                        if (item is System.DBNull) rowList.Add("NULL");
                        else if (item is System.String)
                        {
                            string itemStr = item.ToString();
                            if (itemStr.Contains("'"))
                                itemStr = itemStr.Replace("'", "''");
                            rowList.Add($"N'{itemStr}'");

                        }
                        else if (item is System.Boolean)
                        {
                            if ((bool)item == true) rowList.Add("1");
                            else rowList.Add("0");
                        }
                        else rowList.Add(item.ToString());
                    }
                    string rowString = string.Join(",", rowList);
                    sqlExpression += $"({rowString}),\n";
                }
                char[] trimChar = { ',', '\n' };
                sqlExpression = sqlExpression.TrimEnd(trimChar);
                sqlExpression += ";";
                SqlCommand sqlCommand = new SqlCommand(sqlExpression, connToNewDb);
                sqlCommand.ExecuteNonQuery();
            }

        }




    }
}
