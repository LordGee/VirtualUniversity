using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

/// <summary>
/// Extension of the Database Class, dedicated to holding helper methods
/// that can be reused throughout the class.
/// </summary>
public partial class Database : MonoBehaviour {

    /// <summary>
    /// Helper function to get the next available ID from a given table that does
    /// not autoincrement, this is for the Game Server database.
    /// </summary>
    /// <param name="tableName">Name of the target table</param>
    /// <param name="primaryKey">Primary Key for the target table</param>
    /// <returns>Next Available ID</returns>
    public static int GetNextID(string tableName, string primaryKey) {
        object result = ExecuteScalar("SELECT " + primaryKey + " FROM " + tableName + " ORDER BY " + primaryKey + " DESC LIMIT 1");
        return Convert.ToInt32(result) + 1;
    }

    /// <summary>
    /// Helper function to get the next available ID from a given table that does
    /// not autoincrement, this is for the Web API database.
    /// </summary>
    /// <param name="table">Enum table value.</param>
    /// <returns>Integer value of the next available ID for the given table</returns>
    public static async Task<int> GetNextID_Crud(Table table) {
        int selection = (int) table;
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " ORDER BY " +
                     PrimaryKeyID[selection] + " DESC LIMIT 1";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        int id;
        switch (table) {
            case Table.Answers:
                id = (value.answerResult.Count > 0) ? value.answerResult[0].answer_id : 0;
                break;
            case Table.CourseSubjects:
                id = (value.courseSubjectResult.Count > 0) ? value.courseSubjectResult[0].course_subject_id : 0;
                break;
            case Table.LectureAttend:
                id = (value.lectureAttendResult.Count > 0) ? value.lectureAttendResult[0].attend_id : 0;
                break;
            case Table.LectureBreakPoints:
                id = (value.lectureBreakResult.Count > 0) ? value.lectureBreakResult[0].break_id : 0;
                break;
            case Table.Lectures:
                id = (value.lectureResult.Count > 0) ? value.lectureResult[0].lecture_id : 0;
                break;
            case Table.Questions:
                id = (value.questionResult.Count > 0) ? value.questionResult[0].question_id : 0;
                break;
            case Table.Quizzes:
                id = (value.quizResult.Count > 0) ? value.quizResult[0].quiz_id : 0;
                break;
            case Table.ResultQA:
                id = (value.resultQaResult.Count > 0) ? value.resultQaResult[0].result_qa_id : 0;
                break;
            case Table.Results:
                id = (value.resultResult.Count > 0) ? value.resultResult[0].result_id : 0;
                break;
            default:
                id = 0;
                break;
        }
        return id + 1;
    }

    /// <summary>
    /// Prepends and appends an escaped quote for the given string.
    /// </summary>
    /// <param name="value">Sting value</param>
    /// <returns>Escaped quoted string</returns>
    public static string PrepareString(string value) {
        string result = "\"" + value + "\"";
        return result;
    }

    /// <summary>
    /// Added SQL query which takes no parameters, game server only.
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

    /// <summary>
    /// Instance of the DatabaseCrud class.
    /// </summary>
    public static DatabaseCrud crud;

    /// <summary>
    /// Instantiates the instance of the DatabaseCrud variable
    /// used throughout the Database class
    /// </summary>
    private static void InitCrud() {
        crud = FindObjectOfType<DatabaseCrud>();
    }
}