using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                            enrolled_id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                            fk_account VARCHAR(255) NOT NULL,
                            fk_course_name VARCHAR(255) NOT NULL)");
    }

    public static void RegisterUser(string account, string password, string course) {
        ExecuteNoReturn("INSERT INTO accounts (name, password, fk_course) VALUES (@name, @password, @course)", 
            new SqliteParameter("@name", account), 
            new SqliteParameter("@password", password),
            new SqliteParameter("@course", course));
    }

    public static string GetAccountType(string account) {
        object value = ExecuteScalar("SELECT account_type FROM accounts WHERE name = @value", new SqliteParameter("@value", account));
        return value.ToString();
    }

    public static string GetCourseName(string account) {
        object value = ExecuteScalar("SELECT fk_course FROM accounts WHERE name = @value", new SqliteParameter("@value", account));
        return value.ToString();
    }

    public static void InsertPlayerDetails(string account, string course) {
        if (course != "") {
            string sql = "INSERT INTO Enrolled (fk_account, fk_course_name) VALUES (" + PrepareString(account) + ", " +
                         PrepareString(course) + ")";
            crud.DbCreate(sql);
        }
    }

    public static async Task<string> GetPlayerCourseName(string account) {
        int selection = (int)Table.Enrolled;
        string sql = "SELECT fk_course_name FROM " + TableNames[selection] + " WHERE fk_account = " + PrepareString(account);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        return value.enrolledResult[0].fk_course_name;
    }

    public static async Task<bool> IsPlayerAdmin(string account) {
        int selection = (int) Table.Enrolled;
        string sql = "SELECT account_type FROM " + TableNames[selection] + " WHERE fk_account = " +
                     PrepareString(account);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.enrolledResult[0].account_type == "Admin") {
            return true;
        }
        return false;
    }
}
