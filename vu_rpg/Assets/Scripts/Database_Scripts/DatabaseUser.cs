﻿using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public partial class Database {
    static void Initialize_User() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS accounts (
                            name TEXT NOT NULL PRIMARY KEY,
                            password TEXT NOT NULL,
                            banned INTEGER NOT NULL DEFAULT 0,
                            account_type TEXT NOT NULL DEFAULT 'Student',
                            fk_course TEXT)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Enrolled (
                            enrolled_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            fk_account TEXT NOT NULL,
                            fk_subject TEXT NOT NULL)");
    }

    public static void RegisterUser(string account, string password, string course) {
        ExecuteNoReturn("INSERT INTO accounts (name, password, fk_course) VALUES (@name, @password, @course)", 
            new SqliteParameter("@name", account), 
            new SqliteParameter("@password", password),
            new SqliteParameter("@course", course));
    }

    public static string GetAccountType(string account) {
        object value = ExecuteScalar("SELECT account_type FROM accounts WHERE name = @value", new SqliteParameter("@value", account));
        string result = value.ToString();
        return result;
    }

    public static string GetCourseName(string course) {
        object value = ExecuteScalar("SELECT fk_course FROM accounts WHERE name = @value", new SqliteParameter("@value", course));
        string result = value.ToString();
        return result;
    }
}
