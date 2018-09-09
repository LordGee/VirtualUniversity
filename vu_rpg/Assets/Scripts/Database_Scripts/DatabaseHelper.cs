using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking;

public partial class Database : MonoBehaviour {

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

    /// <summary>
    /// Added SQL query which takes no parameters
    /// </summary>
    /// <param name="sql">SQL QUERY</param>
    /// <returns>Double Array of type Object</returns>
    public static List<List<object>> ExecuteReaderNoParams(string sql) {
        List<List<object>> result = new List<List<object>>();
        using (SqliteCommand command = new SqliteCommand(sql, connection)) {
            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    object[] buffer = new object[reader.FieldCount];
                    reader.GetValues(buffer);
                    result.Add(buffer.ToList());
                }
            }
        }
        return result;
    }

    public static void UpdateNextID() {
        for (int i = 0; i < 5; i++) {
            string sql = "SELECT " + PrimaryKeyID[i] + " FROM " + TableNames[i] + " ORDER BY " + PrimaryKeyID[i] +
                         " DESC LIMIT 1";
            crud.UpdateID(sql, i, ModelNames[i]);
        }
    }

    public static DatabaseCrud crud;

    private static void InitCrud() {
        crud = FindObjectOfType<DatabaseCrud>();
    }


}

