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

    public static void GetStudentLectures(ref List<Lecture> lectures, string account, string course) {
        List<List<object>> result = ExecuteReader(
            "SELECT lecture_id, lecture_title, lecture_url, Lectures.fk_subject_name " +
            "FROM Lectures, Subjects, CourseSubjects WHERE Lectures.fk_subject_name = Subjects.subject_name AND " +
            "Subjects.subject_name = CourseSubjects.fk_subject_name AND CourseSubjects.fk_course_name = @course " +
            "GROUP BY lecture_title ORDER BY lecture_title",
            new SqliteParameter("@course", course));
        for (int i = 0; i < result.Count; i++) {
            if (HasLectureBeenCompleted(account, Convert.ToInt32(result[i][0]))) {
                Lecture temp = new Lecture();
                temp.lecture_id = Convert.ToInt32(result[i][0]);
                temp.lecture_title = (string)result[i][1];
                temp.lecture_url = (string)result[i][2];
                temp.course_name = GetCourseNameFromSubject((string)result[i][3]);
                temp.fk_subject_name = (string)result[i][3];
                lectures.Add(temp);
            }
        }
    }

    private static bool HasLectureBeenCompleted(string account, int lecture) {
        int count =
            Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM LectureAttend WHERE fk_lecture_id = @lecture AND fk_account = @account AND has_attended = 1",
                new SqliteParameter("@lecture", lecture), new SqliteParameter("@account", account)));
        if (count == 0) { return true; }
        return false;
    }


}