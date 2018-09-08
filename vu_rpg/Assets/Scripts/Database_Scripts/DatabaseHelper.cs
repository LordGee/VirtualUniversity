using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public static int GetNextID_Crud(string tableName, string primaryKey) {
        string result = crud.DbRead("SELECT " + primaryKey + " FROM " + tableName + " ORDER BY " +
                                                primaryKey + "");
        Debug.Log("JSON: " + result);
        int z = 0;
        return 1;
    }

    public static Crud crud;

    private static void InitCrud() {
        crud = FindObjectOfType<Crud>();
    }
}

