using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public partial class Database {

    static void Initialize_Lecture() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Lectures (
                            lecture_id INTEGER NOT NULL PRIMARY KEY,
                            lecture_title TEXT NOT NULL,
                            lecture_url TEXT,
                            lecture_owner TEXT,
                            fk_subject_name TEXT)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS LectureAttend (
                            attend_id INTEGER NOT NULL PRIMARY KEY,
                            attend_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                            attend_value INTEGER DEFAULT 0,
                            has_attended INTEGER DEFAULT 0,
                            watch_time DATETIME,
                            fk_account TEXT NOT NULL,
                            fk_lecture_id INTEGER NOT NULL)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS LectureBreakPoints (
                            break_id INTEGER NOT NULL PRIMARY KEY,
                            break_time DATETIME,
                            fk_lecture_id INTEGER NOT NULL)");
    }

    public static int CreateNewLectureInit(Lecture lecture, string account) {
        int id = GetNextID("Lectures", "lecture_id");
        ExecuteNoReturn(
            "INSERT INTO Lectures (lecture_id, lecture_title, lecture_url, lecture_owner, fk_subject_name) VALUES (@id, @title, @url, @owner, @subject)",
            new SqliteParameter("@id", id), new SqliteParameter("@title", lecture.lecture_title),
            new SqliteParameter("@url", lecture.lecture_url), new SqliteParameter("@owner", account),
            new SqliteParameter("@subject", lecture.fk_subject_name));
        return id;
    }

    public static void CreateNewBreakForLecture(ref LectureBreakPoint point, int lectureId) {
        point.break_id = GetNextID("LectureBreakPoints", "break_id");
        ExecuteNoReturn(
            "INSERT INTO LectureBreakPoints (break_id, break_time, fk_lecture_id) VALUES (@point, @time, @lecture)",
            new SqliteParameter("@point", point.break_id), new SqliteParameter("@time", point.break_time),
            new SqliteParameter("@lecture", lectureId));
        point.break_question.question_id = GetNextID("Questions", "question_id");
        ExecuteNoReturn("INSERT INTO Questions (question_id, question, fk_break_id) VALUES (@id, @question, @point)",
            new SqliteParameter("@id", point.break_question.question_id),
            new SqliteParameter("@question", point.break_question.question),
            new SqliteParameter("@point", point.break_id));
        for (int i = 0; i < point.break_question.answers.Count; i++) {
            ExecuteNoReturn("INSERT INTO Answers (answer, is_correct, fk_question_id) VALUES (@ans, @correct, @id)",
                new SqliteParameter("@ans", point.break_question.answers[i].answer),
                new SqliteParameter("@correct", point.break_question.answers[i].isCorrect),
                new SqliteParameter("@id", point.break_question.question_id));
        }
    }

    /// <summary>
    /// Helper function to get the next available ID from a given table that does not autoincrement
    /// </summary>
    /// <param name="tableName">Name of the target table</param>
    /// <param name="primaryKey">Primary Key for the target table</param>
    /// <returns>Next Available ID</returns>
    private static int GetNextID(string tableName, string primaryKey) {
        object result = ExecuteScalar("SELECT " + primaryKey + " FROM " + tableName + " ORDER BY " + primaryKey + " DESC LIMIT 1");
        return Convert.ToInt32(result) + 1;
    }
}