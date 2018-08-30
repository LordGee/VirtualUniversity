using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class Database {
    /// <summary>
    /// Helper function to get the next available ID from a given table that does not autoincrement
    /// </summary>
    /// <param name="tableName">Name of the target table</param>
    /// <param name="primaryKey">Primary Key for the target table</param>
    /// <returns>Next Available ID</returns>
    public static int GetNextID(string tableName, string primaryKey) {
        object result = ExecuteScalar("SELECT " + primaryKey + " FROM " + tableName + " ORDER BY " + primaryKey + " DESC LIMIT 1");
        return Convert.ToInt32(result) + 1;
    }
}
