using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public partial class Database {
    static void Initialize_User() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS accounts (
                            name TEXT NOT NULL PRIMARY KEY,
                            password TEXT NOT NULL,
                            banned INTEGER NOT NULL DEFAULT 0,
                            account_type SET ('Admin', 'Student') NOT NULL,
                            fk_course TEXT)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Enrolled (
                            enrolled_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            fk_account TEXT NOT NULL,
                            fk_subject TEXT NOT NULL)");
    }

    public static void RegisterUser(string account, string password, string type, string course) {
        ExecuteNoReturn("INSERT INTO accounts (name, password, account_type, fk_course) VALUES (@name, @password, @type, @course)", 
            new SqliteParameter("@name", account), 
            new SqliteParameter("@password", password),
            new SqliteParameter("@type", type),
            new SqliteParameter("@course", course));
    }
}
