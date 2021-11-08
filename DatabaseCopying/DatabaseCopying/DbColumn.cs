using System.Data;

namespace DatabaseCopying
{
    public partial class DatabaseCopy
    { 
            private static class GetCreateString
            {
                public static string Get(DataRow column)
                {

                    string name;
                    string type;
                    string notNull;
                    string column_default;
                    string max_length;
                    notNull = column[6].ToString() == "NO" ? "NOT NULL" : "NULL";
                    name = column[3].ToString();
                    type = column[7].ToString();
                    column_default = string.IsNullOrEmpty(column[5].ToString()) ? string.Empty : $"DEFAULT {column[5].ToString()}";
                    max_length = getMaxLength(column, type);
                    if (string.IsNullOrEmpty(max_length))
                        return $"{name} {type} {notNull} {column_default}";
                    else
                        return $"{name} {type}({max_length}) {notNull} {column_default}";

                }

                static string getMaxLength(DataRow column, string type)
                {
                    // Approximate numbers
                    if (type == "real" || type == "float")
                        return $"{column[10].ToString()},{column[11].ToString()}";
                    // Datatime
                    if (!string.IsNullOrEmpty(column[13].ToString()))
                        return string.IsNullOrEmpty(column[13].ToString()) ? column[13].ToString() : string.Empty;
                    // Text
                    if (column[8].ToString() == "-1") return "max";
                    return string.IsNullOrEmpty(column[8].ToString()) ? column[8].ToString() : string.Empty;

                }
            }
    }
}

