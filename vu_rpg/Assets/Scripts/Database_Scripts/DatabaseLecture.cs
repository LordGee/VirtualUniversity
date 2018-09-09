using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public partial class Database {

    static void Initialize_Lecture() {
        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Lectures (
                            lecture_id INTEGER NOT NULL PRIMARY KEY,
                            lecture_title VARCHAR(255) NOT NULL,
                            lecture_url VARCHAR(255),
                            lecture_owner VARCHAR(255),
                            fk_subject_name VARCHAR(255))");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS LectureAttend (
                            attend_id INTEGER NOT NULL PRIMARY KEY,
                            attend_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                            attend_value INTEGER DEFAULT 0,
                            has_attended INTEGER DEFAULT 0,
                            watch_time INTEGER DEFAULT 0,
                            fk_account VARCHAR(255) NOT NULL,
                            fk_lecture_id INTEGER NOT NULL)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS LectureBreakPoints (
                            break_id INTEGER NOT NULL PRIMARY KEY,
                            break_time INTEGER,
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
                List<List<object>> previousWatch = ExecuteReader(
                    "SELECT attend_id, watch_time FROM LectureAttend WHERE fk_account = @account AND fk_lecture_id = @lecture",
                    new SqliteParameter("@account", account), new SqliteParameter("@lecture", temp.lecture_id));
                if (previousWatch.Count > 0) {
                    temp.attend_id = Convert.ToInt32(previousWatch[0][0]);
                    temp.watch_time = Convert.ToInt32(previousWatch[0][1]);
                }
                temp.break_points = new List<LectureBreakPoint>();
                List<List<object>> point = ExecuteReader(
                    "SELECT break_id, break_time FROM LectureBreakPoints WHERE fk_lecture_id = @lecture",
                    new SqliteParameter("@lecture", temp.lecture_id)
                );
                if (point.Count > 0) {
                    for (int j = 0; j < point.Count; j++) {
                        LectureBreakPoint tempBreakPoint = new LectureBreakPoint();
                        tempBreakPoint.break_id = Convert.ToInt32(point[j][0]);
                        tempBreakPoint.break_time = Convert.ToInt32(point[j][1]);
                        tempBreakPoint.break_question = new Questions();
                        tempBreakPoint.break_question.answers = new List<Answers>();
                        tempBreakPoint.break_question = Database.GetQuestionsForChosenLecture(tempBreakPoint.break_id); // loc: DatabaseStudentQuiz.cs
                        temp.break_points.Add(tempBreakPoint);
                    }
                }
                lectures.Add(temp);
            }
        }
    }

    public static Questions GetQuestionsForChosenLecture(int break_id) {
        Questions questions = new Questions();
        List<List<object>> questionResults =
            ExecuteReader("SELECT question_id, question FROM Questions WHERE fk_break_id = @break_id",
                new SqliteParameter("@break_id", break_id));
        for (int i = 0; i < questionResults.Count; i++) {
            Questions q = new Questions();
            q.question_id = Convert.ToInt32(questionResults[i][0]);
            q.question = (string)questionResults[i][1];
            q.answers = new List<Answers>();
            List<List<object>> answerResults =
                ExecuteReader("SELECT answer_id, answer, is_correct FROM Answers WHERE fk_question_id = @question",
                    new SqliteParameter("@question", q.question_id));
            for (int j = 0; j < answerResults.Count; j++) {
                Answers a = new Answers();
                a.answer_id = Convert.ToInt32(answerResults[j][0]);
                a.answer = (string)answerResults[j][1];
                a.isCorrect = Convert.ToInt32(answerResults[j][2]);
                q.answers.Add(a);
            }
            questions = q;
        }
        return questions;
    }

    public static int CreateNewLectureAttend(string account, int lecture) {
        int attend_id = GetNextID("LectureAttend", "attend_id");
        ExecuteNoReturn(
            "INSERT INTO LectureAttend (attend_id, fk_account, fk_lecture_ID) VALUES (@attend, @account, @lecture)",
            new SqliteParameter("@attend", attend_id), new SqliteParameter("@account", account),
            new SqliteParameter("@lecture", lecture));
        return attend_id;
    }

    private static bool HasLectureBeenCompleted(string account, int lecture) {
        int count =
            Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM LectureAttend WHERE fk_lecture_id = @lecture AND fk_account = @account AND has_attended = 1",
                new SqliteParameter("@lecture", lecture), new SqliteParameter("@account", account)));
        if (count == 0) { return true; }
        return false;
    }

    public static void UpdateLectureAttendToComplete(int id) {
        ExecuteNoReturn("UPDATE LectureAttend SET has_attended = 1 WHERE attend_id = @id",
            new SqliteParameter("@id", id));
    }

    public static void UpdateLectureTime(int id, int time) {
        ExecuteNoReturn("UPDATE LectureAttend SET watch_time = @time WHERE attend_id = @id",
            new SqliteParameter("@time", time), new SqliteParameter("@id", id));
    }

    public static bool HasQuestionBeenAttempted(int question, int attend, bool isLecture) {
        int count = -1;
        if (!isLecture) {
            count = Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM ResultQA WHERE fk_question_id = @question AND fk_result_id = @attend",
                new SqliteParameter("@question", question), new SqliteParameter("@attend", attend)));
        } else {
            count = Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM ResultQA WHERE fk_question_id = @question AND fk_attend_id = @attend",
                new SqliteParameter("@question", question), new SqliteParameter("@attend", attend)));
        }
        if (count == 1) { return true; }
        return false;
    }
}