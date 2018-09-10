using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static async Task<int> GetNextID_Crud(Table table) {
        int selection = (int) table;
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " ORDER BY " +
                     PrimaryKeyID[selection] + " DESC LIMIT 1";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        int id;
        switch (table) {
            case Table.Answers:
                id = value.answerResult[0].answer_id;
                break;
            case Table.CourseSubjects:
                id = value.courseSubjectResult[0].course_subject_id;
                break;
            case Table.LectureAttend:
                id = value.lectureAttendResult[0].attend_id;
                break;
            case Table.LectureBreakPoints:
                id = value.lectureBreakResult[0].break_id;
                break;
            case Table.Lectures:
                id = value.lectureResult[0].lecture_id;
                break;
            case Table.Questions:
                id = value.questionResult[0].question_id;
                break;
            case Table.Quizzes:
                id = value.quizResult[0].quiz_id;
                break;
            case Table.ResultQA:
                id = value.resultQaResult[0].result_qa_id;
                break;
            case Table.Results:
                id = value.resultResult[0].result_id;
                break;
            default:
                id = 1;
                break;
        }
        return id + 1;

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

    public static DatabaseCrud crud;

    private static void InitCrud() {
        crud = FindObjectOfType<DatabaseCrud>();
    }


}

