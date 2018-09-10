using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

public partial class Database {
    static void Initialize_Quiz() {
        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Courses (
                            course_name VARCHAR(255) NOT NULL PRIMARY KEY)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS CourseSubjects (
                            course_subject_id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                            fk_course_name VARCHAR(255) NOT NULL,
                            fk_subject_name VARCHAR(255) NOT NULL)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Subjects (
                            subject_name VARCHAR(255) NOT NULL PRIMARY KEY)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Quizzes (
                            quiz_id INTEGER NOT NULL PRIMARY KEY,
                            quiz_name VARCHAR(255) NOT NULL,
                            quiz_timer INTEGER default 1,
                            creation_date DATETIME default CURRENT_TIMESTAMP,
                            quiz_owner VARCHAR(255) NOT NULL,                            
                            fk_subject_name VARCHAR(255) NOT NULL
                            )");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Questions (
                            question_id INTEGER NOT NULL PRIMARY KEY,
                            question VARCHAR(255) NOT NULL,
                            fk_quiz_id INTEGER NOT NULL DEFAULT -1,
                            fk_break_id INTEGER NOT NULL DEFAULT -1)");


        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Answers (
                            answer_id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                            answer VARCHAR(255) NOT NULL,
                            is_correct INTEGER NOT NULL,
                            fk_question_id INTEGER NOT NULL)");
    }

    /// <summary>
    /// Retrieve all available course names from the database
    /// Used for selection within the application, prevents typos
    /// </summary>
    /// <returns>Returns list of strings of each course</returns>
    public static async Task<List<string>> GetCourseNames() {
        int selection = (int) Table.Courses;
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " ORDER BY " +
                     PrimaryKeyID[selection] + " ASC";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        List<string> result = new List<string>();
        for (int i = 0; i < value.courseResult.Count; i++) {
            result.Add(value.courseResult[i].course_name);
        }
        return result;
    }

    public static async Task<List<string>> GetQuizNames() {
        int selection = (int)Table.Quizzes;
        //SELECT quiz_name FROM Quizzes ORDER BY quiz_name ASC
        string sql = "SELECT quiz_name FROM " + TableNames[selection] + " ORDER BY quiz_name ASC";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        List<string> result = new List<string>();
        for (int i = 0; i < value.quizResult.Count; i++) {
            result.Add(value.quizResult[i].quiz_name);
        }
        return result;
    }

    public static async Task<List<string>> GetSubjectsLinkedToCourse(string course) {
        int selection = (int) Table.CourseSubjects;
        //SELECT fk_subject_name FROM CourseSubjects WHERE fk_course_name = @course ORDER BY fk_subject_name ASC
        string sql  = "SELECT fk_subject_name FROM " + TableNames[selection] + " WHERE fk_course_name = " + PrepareString(course) + " ORDER BY fk_subject_name ASC";
        string json = (string)await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        List<string> result = new List<string>();
        for (int i = 0; i < value.courseSubjectResult.Count; i++) {
            result.Add(value.courseSubjectResult[i].fk_subject_name);
        }
        return result;
    }

 
    public static void AddNewCourse(string course) {
        crud.DbCreate("INSERT INTO Courses (course_name) VALUES (" + PrepareString(course) + ")") ;
    }

    public static void AddNewSubject(string subject) {
        crud.DbCreate("INSERT INTO Subjects (subject_name) VALUES (" + PrepareString(subject) + ")");
    }

    public static void AddCourseSubjects(string course, string subject) {
        crud.DbCreate("INSERT INTO CourseSubjects (fk_course_name, fk_subject_name) VALUES (" + PrepareString(course) + ", " +
                      PrepareString(subject) + ")");
    }

    public static async Task<bool> CheckCourseExists(string course) {
        int selection = (int) Table.Courses;
        // "SELECT course_name FROM Courses WHERE course_name = @course"
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " WHERE " +
                     PrimaryKeyID[selection] + " = " + PrepareString(course);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.courseResult.Count == 0) {
            return false;
        }
        return true;
    }

    public static async Task<bool> CheckSubjectExists(string subject) {
        int selection = (int)Table.Subjects;
        // "SELECT subject_name FROM Subjects WHERE subject_name = @subject"
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " WHERE " +
                     PrimaryKeyID[selection] + " = " + PrepareString(subject);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.subjectResult.Count == 0) {
            return false;
        }
        return true;
    }

    public static async Task<bool> CheckSubjectLinkedToCourseExists(string subject, string course) {
        int selection = (int)Table.CourseSubjects;
        // "SELECT fk_subject_name, fk_course_name FROM CourseSubjects
        // WHERE fk_subject_name = @subject AND fk_course_name = @course"
        string sql = "SELECT fk_subject_name, fk_course_name FROM " + TableNames[selection] +
                     " WHERE fk_subject_name = " + PrepareString(subject) + " AND fk_course_name = " + PrepareString(course);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.courseSubjectResult.Count == 0) {
            return false;
        }
        return true;
    }

    public static async Task<bool> CheckQuizExists(string quiz) {
        int selection = (int)Table.Quizzes;
        // "SELECT quiz_name FROM Quizzes WHERE quiz_name = @quiz"
        string sql = "SELECT quiz_name FROM " + TableNames[selection] + " WHERE quiz_name = " + PrepareString(quiz);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.quizResult.Count == 0) {
            return true;
        }
        return false;
    }

    public static void CreateNewQuiz(int quiz, string name, int number, string owner, string subject) {
        crud.DbCreate("INSERT INTO Quizzes (quiz_id, quiz_name, quiz_timer, quiz_owner, fk_subject_name) VALUES (" +
                      quiz + ", " + PrepareString(name) + ", " + number + ", " + PrepareString(owner) + ", " + PrepareString(subject) + ")");
    }

    public static void AddQuestionToQuiz(int id, string question, int quiz) {
        crud.DbCreate("INSERT INTO Questions (question_id, question, fk_quiz_id) VALUES (" + id + ", " + PrepareString(question) +
                      ", " + quiz + ")");
    }

    public static void AddAnswerToQuestion(string answer, int isCorrect, int question) {
        crud.DbCreate("INSERT INTO Answers (answer, is_correct, fk_question_id) VALUES (" + PrepareString(answer) + ", " + isCorrect +
                      ", " + question + ")");
    }
    /// <summary>
    /// Adds question and answers to the database
    /// </summary>
    /// <param name="question">Single string question</param>
    /// <param name="answers">Array of string answers (max 3)</param>
    /// <param name="correct">Correct answer 1 - 3</param>
    //public static void AddNewQuestionAndAnswer(string question, string[] answers, int correct) {
    //    int QuestionID = NextID[(int)Table.Questions];
    //    // todo: need to get quiz_id for this insert statement
    //    crud.DbCreate("INSERT INTO Questions (question_id, question, fk_quiz_id) VALUES (" + QuestionID + ", " +
    //                  question + ", 1)");
    //    for (int i = 0; i < answers.Length; i++) {
    //        crud.DbCreate("INSERT INTO Answers (answer, is_correct, fk_question_id) VALUES (" + answers[i] + ", " +
    //                      correct + ", " + QuestionID + ")");
    //    }
    //}
}
