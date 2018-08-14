using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;

public partial class Database {
    static void Initialize_Quiz() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Courses (
                            course_name TEXT NOT NULL PRIMARY KEY)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS CourseSubjects (
                            course_subject_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            fk_course_name TEXT NOT NULL,
                            fk_subject_name TEXT NOT NULL)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Subjects (
                            subject_name TEXT NOT NULL PRIMARY KEY)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Quizes (
                            quiz_id INTEGER NOT NULL PRIMARY KEY,
                            quiz_name TEXT NOT NULL,
                            quiz_timer INTEGER default 1,
                            creation_date DATETIME default CURRENT_TIMESTAMP,
                            quiz_owner TEXT NOT NULL,                            
                            fk_subject_name TEXT NOT NULL
                            )");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Questions (
                            question_id INTEGER NOT NULL PRIMARY KEY,
                            question TEXT NOT NULL,
                            fk_quiz_id INTEGER NOT NULL DEFAULT -1,
                            fk_lecture_id INTEGER NOT NULL DEFAULT -1)");


        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Answers (
                            answer_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            answer TEXT NOT NULL,
                            is_correct INTEGER NOT NULL,
                            fk_question_id INTEGER NOT NULL)");
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

    /// <summary>
    /// Rather then getting the last index of the question table, we get create
    /// the index so it can be reused when inserting the answers.
    /// todo: If questions get deleted or bd empty this may cause issues.
    /// </summary>
    /// <returns>Returns next available index value</returns>
    public static int GetNewIDForQuestion() {
        object id     = ExecuteScalar("SELECT question_id FROM Questions ORDER BY question_id DESC LIMIT 1");
        int    result = Convert.ToInt32(id);
        return result + 1;
    }

    public static int GetNewIDForQuiz() {
        object id     = ExecuteScalar("SELECT quiz_id FROM Quizes ORDER BY quiz_id DESC LIMIT 1");
        int    result = Convert.ToInt32(id);
        return result + 1;
    }

    /// <summary>
    /// Retrieve all avaiilable course names from the database
    /// Used for selection within the application, prevents typos
    /// </summary>
    /// <returns>Returns list of strings of each course</returns>
    public static List<string> GetCourseNames() {
        List<List<object>> results = new List<List<object>>();
        List<string> stringResults = new List<string>();
        results = ExecuteReaderNoParams("SELECT course_name FROM Courses ORDER BY course_name ASC");
        for (int i = 0; i < results.Count; i++) {
            stringResults.Add(results[i][0].ToString());   
        }
        return stringResults;
    }

    public static List<string> GetQuizNames() {
        List<List<object>> results       = new List<List<object>>();
        List<string>       stringResults = new List<string>();
        results = ExecuteReaderNoParams("SELECT quiz_name FROM Quizes ORDER BY quiz_name ASC");
        for (int i = 0; i < results.Count; i++) {
            stringResults.Add(results[i][0].ToString());
        }
        return stringResults;
    }

    public static List<string> GetSubjectsLinkedToCourse(string course) {
        List<List<object>> results = new List<List<object>>();
        List<string> stringResults = new List<string>();
        results = ExecuteReader("SELECT fk_subject_name FROM CourseSubjects WHERE fk_course_name = @course ORDER BY fk_subject_name ASC",
            new SqliteParameter("@course", course));
        for (int i = 0; i < results.Count; i++) {
            stringResults.Add(results[i][0].ToString());
        }
        return stringResults;
    }

    public static void AddNewCourse(string course) {
        ExecuteNoReturn("INSERT INTO Courses (course_name) VALUES (@course)",
            new SqliteParameter("@course", course));
    }

    public static void AddNewSubject(string subject) {
        ExecuteNoReturn("INSERT INTO Subjects (subject_name) VALUES (@subject)",
            new SqliteParameter("@subject", subject));
    }

    public static void AddCourseSubjects(string course, string subject) {
        ExecuteNoReturn("INSERT INTO CourseSubjects (fk_course_name, fk_subject_name) VALUES (@course, @subject)",
            new SqliteParameter("@course", course),
            new SqliteParameter("@subject", subject));
    }

    public static bool CheckCourseExists(string course) {
        object result = ExecuteScalar("SELECT course_name FROM Courses WHERE course_name = @course",
            new SqliteParameter("@course", course));
        if (result == null) {
            return false;
        }
        return true;
    }

    public static bool CheckSubjectExists(string subject) {
        object result = ExecuteScalar("SELECT subject_name FROM Subjects WHERE subject_name = @subject",
            new SqliteParameter("@subject", subject));
        if (result == null) {
            return false;
        }
        return true;
    }

    public static bool CheckSubjectLinkedToCourseExists(string subject, string course) {
        object result = ExecuteScalar("SELECT fk_subject_name, fk_course_name FROM CourseSubjects WHERE fk_subject_name = @subject AND fk_course_name = @course",
            new SqliteParameter("@subject", subject),
            new SqliteParameter("@course", course));
        if (result == null) {
            return false;
        }
        return true;
    }

    public static bool CheckQuizExists(string quiz) {
        bool dontExists = false;
        object result = ExecuteScalar("SELECT quiz_name FROM Quizes WHERE quiz_name = @quiz",
            new SqliteParameter("@quiz", quiz));
        if (result == null) {
            dontExists = true;
        }
        return dontExists;
    }

    public static void CreateNewQuiz(int quiz, string name, int number, string owner, string subject) {
        ExecuteNoReturn("INSERT INTO Quizes (quiz_id, quiz_name, number_questions, quiz_owner, fk_subject_name) VALUES (@quizid, @name ,@number, @owner, @subject)", 
            new SqliteParameter("@quizid", quiz), 
            new SqliteParameter("@name", name), 
            new SqliteParameter("@number", number),
            new SqliteParameter("@owner", owner),
            new SqliteParameter("@subject", subject));
    }

    public static void AddQuestionToQuiz(int id, string question, int quiz) {
        ExecuteNoReturn("INSERT INTO Questions (question_id, question, fk_quiz_id) VALUES (@id, @quest, @quiz)",
            new SqliteParameter("@id", id),
            new SqliteParameter("@quest", question),
            new SqliteParameter("@quiz", quiz));
    }

    public static void AddAnswerToQuestion(string answer, int isCorrect, int question) {
        ExecuteNoReturn("INSERT INTO Answers (answer, is_correct, fk_question_id) VALUES (@ans, @isCorrect, @question)",
            new SqliteParameter("@ans", answer),
            new SqliteParameter("@isCorrect", isCorrect),
            new SqliteParameter("@question", question));
    }
    /// <summary>
    /// Adds question and answers to the database
    /// todo: answers needs to be more dynamic, one answer per row.
    /// </summary>
    /// <param name="question">Single string question</param>
    /// <param name="answers">Array of string answers (max 3)</param>
    /// <param name="correct">Correct answer 1 - 3</param>
    public static void AddNewQuestionAndAnswer(string question, string[] answers, int correct) {
        int QuestionID = GetNewIDForQuestion();
        // todo: need to get quiz_id for this insert statement
        ExecuteNoReturn("INSERT INTO Questions (" +
                        "question_id, question, fk_quiz_id) VALUES (" +
                        "@id, @question, 1)",
            new SqliteParameter("@id", QuestionID),
            new SqliteParameter("@question", question));
        for (int i = 0; i < answers.Length; i++) {
            ExecuteNoReturn("INSERT INTO Answers (" +
                            "answer, is_correct, fk_question_id) VALUES (" +
                            "@ans, @correct, @id)",
                new SqliteParameter("@ans", answers[i]),
                new SqliteParameter("@correct", correct),
                new SqliteParameter("@id", QuestionID));
        }
    }
}
