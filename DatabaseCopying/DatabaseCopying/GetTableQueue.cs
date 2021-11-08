using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopying
{
    partial class DatabaseCopy
    {

        void getTableQueue()
        {
            tableQueue = TableNode.getQueue(getRelations());

            List<TableNode> getRelations()
            {
                Dictionary<string, TableNode> tableNames = getTableNames();
                foreach (TableNode node in tableNames.Values)
                {
                    string sqlExpression = $@"
SELECT
    OBJECT_NAME(fk.parent_object_id) 'Parent table',
    OBJECT_NAME(fk.referenced_object_id) 'Referenced table'
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
INNER JOIN	
    sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id
INNER JOIN
    sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id
WHERE OBJECT_NAME(fk.parent_object_id) = '{node.Name}'
";
                    SqlCommand SqlExpression = new SqlCommand(sqlExpression, conn);
                    SqlDataReader reader = SqlExpression.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string referenced = reader.GetValue(1).ToString();
                            node.AddParent(tableNames[referenced]);

                        }
                    }

                    reader.Close();
                }

                return tableNames.Values.ToList();
                Dictionary<string, TableNode> getTableNames()
                {
                    tableNames = new Dictionary<string, TableNode>();

                    DataTable schema = conn.GetSchema("Tables");
                    foreach (DataRow row in schema.Rows)
                    {
                        if (row[2].ToString() != "sysdiagrams")
                        {
                            tableNames.Add(row[2].ToString(), new TableNode(row[2].ToString())); // Table Names
                        }
                    }
                    return tableNames;
                }
            }
        }
    }
}
