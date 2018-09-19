using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

/// <summary>
/// Extension of the Database class, dedicated to queries related
/// to the User functionality.
/// </summary>
public partial class Database {
    /// <summary>
    /// Creates if not exists the database structure
    /// Invoked by the main initialise.
    /// </summary>
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

    /// <summary>
    /// Registers a new user into the game server database
    /// Server only
    /// </summary>
    /// <param name="account">Account name of the new user</param>
    /// <param name="password">Encrypted password</param>
    /// <param name="course">Course that the new user will register to</param>
    public static void RegisterUser(string account, string password, string course) {
        ExecuteNoReturn("INSERT INTO accounts (name, password, fk_course) VALUES (@name, @password, @course)", 
            new SqliteParameter("@name", account), 
            new SqliteParameter("@password", password),
            new SqliteParameter("@course", course));
    }

    /// <summary>
    /// Get the type of account for a registered user from the game server database
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <returns>Returns the account type</returns>
    public static string GetAccountType(string account) {
        object value = ExecuteScalar("SELECT account_type FROM accounts WHERE name = @value", new SqliteParameter("@value", account));
        return value.ToString();
    }

    /// <summary>
    /// Gets the course that the given user is registered to
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <returns>Returns name of the course that the give user is registered to</returns>
    public static string GetCourseName(string account) {
        object value = ExecuteScalar("SELECT fk_course FROM accounts WHERE name = @value", new SqliteParameter("@value", account));
        return value.ToString();
    }

    /// <summary>
    /// Inserts new player details into custom database
    /// </summary>
    /// <param name="account">Account name of the new user</param>
    /// <param name="course">Course name that the user is registered too</param>
    public static void InsertPlayerDetails(string account, string course) {
        if (course != "") {
            string sql = "INSERT INTO Enrolled (fk_account, fk_course_name) VALUES (" + PrepareString(account) + ", " +
                         PrepareString(course) + ")";
            crud.DbCreate(sql);
        }
    }

    /// <summary>
    /// GGet the course name from the custom database
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <returns>Returns the course name for the give user</returns>
    public static async Task<string> GetPlayerCourseName(string account) {
        int selection = (int)Table.Enrolled;
        string sql = "SELECT fk_course_name FROM " + TableNames[selection] + " WHERE fk_account = " + PrepareString(account);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        return value.enrolledResult[0].fk_course_name;
    }

    /// <summary>
    /// Checks if the given account is of type admin
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <returns>Returns True if user is aan admin</returns>
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
