using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

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

    public static async Task<int> CreateNewLectureInit(Lecture lecture, string account) {
        int id = await GetNextID_Crud(Table.Lectures);
        crud.DbCreate(
            "INSERT INTO Lectures (lecture_id, lecture_title, lecture_url, lecture_owner, fk_subject_name) VALUES (" +
            id + ", " + PrepareString(lecture.lecture_title) + ", " + PrepareString(lecture.lecture_url) + ", " + PrepareString(account) + ", " +
            PrepareString(lecture.fk_subject_name) + ")");
        return id;
    }

    public static async Task<LectureBreakPoint> CreateNewBreakForLecture(LectureBreakPoint point, int lectureId) {
        point.break_id = await GetNextID_Crud(Table.LectureBreakPoints);
        crud.DbCreate("INSERT INTO LectureBreakPoints (break_id, break_time, fk_lecture_id) VALUES (" + point.break_id + ", " +
                      point.break_time + ", " + lectureId + ")");
        point.break_question.question_id = await GetNextID_Crud(Table.Questions);
        crud.DbCreate("INSERT INTO Questions (question_id, question, fk_break_id) VALUES (" +
                      point.break_question.question_id + ", " + PrepareString(point.break_question.question) + ", " + point.break_id +
                      ")");
        for (int i = 0; i < point.break_question.answers.Count; i++) {
            crud.DbCreate("INSERT INTO Answers (answer, is_correct, fk_question_id) VALUES (" +
                          PrepareString(point.break_question.answers[i].answer) + ", " + point.break_question.answers[i].isCorrect +
                          ", " + point.break_question.question_id + ")");
        }
        return point;
    }

    public static async Task<List<Lecture>> GetStudentLectures(List<Lecture> lectures, string account, string course) {
        int selection = (int) Table.Lectures;
        string sql = "SELECT " + PrimaryKeyID[selection] + ", lecture_title, lecture_url, Lectures.fk_subject_name FROM " +
                     TableNames[selection] + ", Subjects, CourseSubjects WHERE Lectures.fk_subject_name = Subjects.subject_name AND " +
                     "Subjects.subject_name = CourseSubjects.fk_subject_name AND CourseSubjects.fk_course_name = " +
                     PrepareString(course) + " GROUP BY lecture_title ORDER BY lecture_title";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        for (int i = 0; i < value.lectureResult.Count; i++) {
            if (await HasLectureBeenCompleted(account, value.lectureResult[i].lecture_id)) {
                Lecture temp = new Lecture();
                temp.lecture_id = value.lectureResult[i].lecture_id;
                temp.lecture_title = value.lectureResult[i].lecture_title;
                temp.lecture_url = value.lectureResult[i].lecture_url;
                temp.course_name = await GetCourseNameFromSubject(value.lectureResult[i].fk_subject_name);
                temp.fk_subject_name = value.lectureResult[i].fk_subject_name;
                selection = (int) Table.LectureAttend;
                sql = "SELECT " + PrimaryKeyID[selection] + ", watch_time FROM " + TableNames[selection] +
                      " WHERE fk_account = " + PrepareString(account) + " AND fk_lecture_id = " + temp.lecture_id;
                json = (string)await crud.Read(sql, ModelNames[selection]);
                DatabaseCrud.JsonResult previousWatch = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
                if (previousWatch.lectureAttendResult.Count > 0) {
                    temp.attend_id = previousWatch.lectureAttendResult[0].attend_id;
                    temp.watch_time = previousWatch.lectureAttendResult[0].watch_time;
                }
                temp.break_points = new List<LectureBreakPoint>();
                selection = (int)Table.LectureBreakPoints;
                sql = "SELECT " + PrimaryKeyID[selection] + ", break_time FROM " + TableNames[selection] +
                      " WHERE fk_lecture_id = " + temp.lecture_id;
                json = (string)await crud.Read(sql, ModelNames[selection]);
                DatabaseCrud.JsonResult point = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
                if (point.lectureBreakResult.Count > 0) {
                    for (int j = 0; j < point.lectureBreakResult.Count; j++) {
                        LectureBreakPoint tempBreakPoint = new LectureBreakPoint();
                        tempBreakPoint.break_id = point.lectureBreakResult[j].break_id;
                        tempBreakPoint.break_time = point.lectureBreakResult[j].break_time;
                        tempBreakPoint.break_question = new Questions();
                        tempBreakPoint.break_question.answers = new List<Answers>();
                        tempBreakPoint.break_question = await GetQuestionsForChosenLecture(tempBreakPoint.break_id);
                        temp.break_points.Add(tempBreakPoint);
                    }
                }
                lectures.Add(temp);
            }
        }
        return lectures;
    }

    public static async Task<Questions> GetQuestionsForChosenLecture(int break_id) {
        Questions questions = new Questions();
        int selection = (int) Table.Questions;
        string sql = "SELECT " + PrimaryKeyID[selection] + ", question FROM " + TableNames[selection] +
                     " WHERE fk_break_id = " + break_id;
        string json = (string)await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult questionResults = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        for (int i = 0; i < questionResults.questionResult.Count; i++) {
            Questions q = new Questions();
            q.question_id = questionResults.questionResult[i].question_id;
            q.question = questionResults.questionResult[i].question;
            q.answers = new List<Answers>();
            selection = (int) Table.Answers;
            sql = "SELECT " + PrimaryKeyID[selection] + ", answer, is_correct FROM " + TableNames[selection] +
                  " WHERE fk_question_id = " + q.question_id;
            json = (string) await crud.Read(sql, ModelNames[selection]);
            DatabaseCrud.JsonResult answerResults = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
            for (int j = 0; j < answerResults.answerResult.Count; j++) {
                Answers a = new Answers();
                a.answer_id = answerResults.answerResult[j].answer_id;
                a.answer = answerResults.answerResult[j].answer;
                a.isCorrect = answerResults.answerResult[j].is_correct;
                q.answers.Add(a);
            }
            questions = q;
        }
        return questions;
    }

    public static async Task<int> CreateNewLectureAttend(string account, int lecture) {
        int attend_id = await GetNextID_Crud(Table.LectureAttend);
        crud.DbCreate("INSERT INTO LectureAttend (attend_id, fk_account, fk_lecture_ID) VALUES (" + attend_id + ", " +
                      PrepareString(account) + ", " + lecture + ")");
        return attend_id;
    }

    private static async Task<bool> HasLectureBeenCompleted(string account, int lecture) {
        int selection = (int)Table.LectureAttend;
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] +
                     " WHERE fk_lecture_id = " + lecture + " AND fk_account = " + PrepareString(account) + " AND has_attended = 1";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.lectureAttendResult.Count == 0) { return true; }
        return false;
    }

    public static void UpdateLectureAttendToComplete(int id) {
        crud.DbCreate("UPDATE LectureAttend SET has_attended = 1 WHERE attend_id = " + id);
    }

    public static void UpdateLectureTime(int id, int time) {
        crud.DbCreate("UPDATE LectureAttend SET watch_time = " + time + " WHERE attend_id = " + id);
    }

    public static async Task<bool> HasQuestionBeenAttempted(int question, int id, bool isLecture) {
        int selection = (int) Table.ResultQA;
        string selectValue;
        if (!isLecture) {
            selectValue = "fk_result_id";
        } else {
            selectValue = "fk_attend_id";
        }
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] +
                     " WHERE fk_question_id = " + question + " AND " + selectValue + " = " + id;
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.resultQaResult.Count == 1) { return true; }
        return false;
    }
}