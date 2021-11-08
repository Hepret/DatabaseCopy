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
        public void CopySchema()
        {
            // Expression that will create tables with columns, primary keys and foreign keys
            StringBuilder sqlExpression = new StringBuilder();
            CreateTables();
            SetPrimaryKeys();
            SetForeignKeys();
            SqlCommand sqlCommand = new SqlCommand(sqlExpression.ToString(), connToNewDb);
            sqlCommand.ExecuteNonQuery();

            void SetPrimaryKeys()
            {
                foreach (TableNode node in tableQueue)
                {
                    string name = node.Name;
                    string[] restriction = new string[4];
                    restriction[2] = name;
                    DataTable indexColumnsTable = conn.GetSchema("IndexColumns", restriction);
                    string columnName = string.Empty;
                    string constraintName = string.Empty;

                    foreach (DataRow row in indexColumnsTable.Rows)
                    {
                        if (row[2].ToString().Contains("PK"))
                        {
                            columnName = row[6].ToString();
                            constraintName = row[2].ToString();
                            break;
                        }

                    }
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        sqlExpression.AppendFormat( $"\nALTER TABLE [dbo].[{name}]\n ADD CONSTRAINT {constraintName} PRIMARY KEY CLUSTERED({columnName})");

                        /*SqlCommand  = new SqlCommand(cmdStr, connToNewDb);

                        sqlCmd.ExecuteNonQuery();*/
                    }
                }
            }
            void SetForeignKeys()
            {
                string getRelation = @"
           SELECT
                OBJECT_NAME(fk.parent_object_id) 'Parent table',
                c1.name 'Parent column',
                OBJECT_NAME(fk.referenced_object_id) 'Referenced table',
                c2.name 'Referenced column',
                fk.name 'Constraint name'
            FROM 
                sys.foreign_keys fk
            INNER JOIN 
                sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
            INNER JOIN	
                sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id
            INNER JOIN
                sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id";
                SqlCommand cmd = new SqlCommand(getRelation, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {


                    while (reader.Read())
                    {
                        string ParentTable = reader.GetValue(0).ToString();
                        string ParentColumn = reader.GetValue(1).ToString();
                        string ReferencedTable = reader.GetValue(2).ToString();
                        string ReferencedColumn = reader.GetValue(3).ToString();
                        string ConstraintName = reader.GetValue(4).ToString();

                        string SetFKSql = $@"
                    ALTER TABLE [dbo].[{ParentTable}]
                    ADD CONSTRAINT {ConstraintName} FOREIGN KEY ([{ParentColumn}])
                    REFERENCES [dbo].[{ReferencedTable}] ([{ReferencedColumn}])";

                      sqlExpression.AppendFormat(SetFKSql);

                    }
                }

                reader.Close();
            }
            void CreateTables()
            {

                foreach (TableNode node in tableQueue)
                {
                    string table = node.Name;
                    string[] restriction = new string[4];
                    restriction[2] = table;


                    DataTable Columns = conn.GetSchema("Columns", restriction);

                    string[] ColumnsArr = new string[Columns.Rows.Count];

                    for (int i = 0; i < Columns.Rows.Count; i++)
                    {
                        ColumnsArr[i] = GetCreateString.Get(Columns.Rows[i]);
                    }

                    string columnsStr = string.Join(",", ColumnsArr);

                    sqlExpression.AppendFormat($"\n CREATE TABLE {table}({columnsStr})");
                }
            }
        }

    }
}
