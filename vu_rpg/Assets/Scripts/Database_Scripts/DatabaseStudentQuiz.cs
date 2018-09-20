using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Extension of the Database class, dedicated to queries related
/// to the Student Quiz functionality.
/// </summary>
public partial class Database {
    /// <summary>
    /// Creates if not exists the database structure
    /// Invoked by the main initialise.
    /// </summary>
    static void Initialize_StudentQuiz() {
        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS Results (
                            result_id INTEGER NOT NULL PRIMARY KEY,
                            result_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                            result_value INTEGER DEFAULT 0,
                            is_completed INTEGER DEFAULT 0,
                            time_elapsed INTEGER DEFAULT 0,
                            fk_account VARCHAR(255) NOT NULL,
                            fk_quiz_id INTEGER NOT NULL)");

        crud.DbCreate(@"CREATE TABLE IF NOT EXISTS ResultQA (
                            result_qa_id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                            fk_result_id INTEGER NOT NULL DEFAULT -1,
                            fk_attend_id INTEGER NOT NULL DEFAULT -1,
                            fk_question_id INTEGER NOT NULL,
                            fk_answer_id INTEGER NOT NULL)");
    }

    /// <summary>
    /// Get all incomplete quizzes for a given student, based on the course the student
    /// is registered on. If partial complete then get information to allow continuation.
    /// </summary>
    /// <param name="quiz">List of current quiz data</param>
    /// <param name="account">Account name of the user</param>
    /// <param name="course">The course of the current user</param>
    /// <returns>Complete list of appropriate quizzes</returns>
    public static async Task<List<Quiz>> GetStudentQuizzes(List<Quiz> quiz, string account, string course) {
        int selection = (int)Table.Quizzes;
        // "SELECT quiz_id, quiz_name, quiz_timer, creation_date, quiz_owner, Quizzes.fk_subject_name " +
        // "FROM Quizzes, Subjects, CourseSubjects WHERE Quizzes.fk_subject_name = Subjects.subject_name AND " +
        // "Subjects.subject_name = CourseSubjects.fk_subject_name AND CourseSubjects.fk_course_name = @course " +
        // "GROUP BY quiz_name ORDER BY quiz_name"
        string sql = "SELECT " + PrimaryKeyID[selection] + ", quiz_name, quiz_timer, creation_date, quiz_owner, Quizzes.fk_subject_name " +
                     "FROM " + TableNames[selection] + ", Subjects, CourseSubjects WHERE Quizzes.fk_subject_name = Subjects.subject_name AND " +
                     "Subjects.subject_name = CourseSubjects.fk_subject_name AND CourseSubjects.fk_course_name = " + PrepareString(await GetPlayerCourseName(account)) +
                     " GROUP BY quiz_name ORDER BY quiz_name";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        for (int i = 0; i < value.quizResult.Count; i++) {
            if (await HasQuizBeenCompleted(account, value.quizResult[i].quiz_id)) {
                Quiz temp = new Quiz();
                temp.QuizId = value.quizResult[i].quiz_id;
                temp.QuizName = value.quizResult[i].quiz_name;
                temp.QuizTimer = value.quizResult[i].quiz_timer;
                temp.CourseName = await GetCourseNameFromSubject(value.quizResult[i].fk_subject_name);
                temp.SubjectName = value.quizResult[i].fk_subject_name;
                selection = (int) Table.Results;
                sql = "SELECT " + PrimaryKeyID[selection] + ", time_elapsed FROM " + TableNames[selection] +
                      " WHERE fk_account = " + PrepareString(account) + " AND fk_quiz_id = " + temp.QuizId;
                string previousAttemptJson = (string) await crud.Read(sql, ModelNames[selection]);
                DatabaseCrud.JsonResult previousAttempt = JsonUtility.FromJson<DatabaseCrud.JsonResult>(previousAttemptJson);
                if (previousAttempt.resultResult.Count > 0) {
                    temp.result_id = previousAttempt.resultResult[0].result_id;
                    temp.time_elapsed = previousAttempt.resultResult[0].time_elapsed;
                }
                quiz.Add(temp);
            }
        }
        return quiz;
    }

    /// <summary>
    /// Checks if a given quiz has already been completed by the user
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <param name="quiz">Quiz ID</param>
    /// <returns>Returns true if quiz has NOT been completed</returns>
    private static async Task<bool> HasQuizBeenCompleted(string account, int quiz) {
        int selection = (int)Table.Results;
        // "SELECT COUNT(*) FROM Results WHERE fk_quiz_id = @quiz AND fk_account = @account AND is_completed = 1"
        string sql = "SELECT " + PrimaryKeyID[selection] + " FROM " + TableNames[selection] + " WHERE fk_quiz_id = " +
                     quiz + " AND fk_account = " + PrepareString(account) + " AND is_completed = 1";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.resultResult.Count > 0) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gets the linked course name for a given subject
    /// </summary>
    /// <param name="subject">Subject name</param>
    /// <returns>Course name linked to subject</returns>
    private static async Task<string> GetCourseNameFromSubject(string subject) {
        int selection = (int)Table.CourseSubjects;
        // "SELECT fk_course_name FROM CourseSubjects WHERE fk_subject_name = @subject"
        string sql = "SELECT fk_course_name FROM " + TableNames[selection] + " WHERE fk_subject_name = " + PrepareString(subject);
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        return value.courseSubjectResult[0].fk_course_name;
    }

    /// <summary>
    /// Create a new result entry when the quiz begins
    /// </summary>
    /// <param name="account">Account name of the user</param>
    /// <param name="quiz">Quiz ID</param>
    /// <returns>Returns the new ID for the results entry</returns>
    public static async Task<int> CreateNewResultsForChosenQuiz(string account, int quiz) {
        int selection = (int)Table.Results;
        int id = await GetNextID_Crud(Table.Results);
        // "INSERT INTO Results (result_id, fk_account, fk_quiz_id) VALUES (@id, @account, @quiz)"
        crud.DbCreate("INSERT INTO " + TableNames[selection] + " (" + PrimaryKeyID[selection] +
                      ", fk_account, fk_quiz_id" + ") VALUES (" + id + ", " + PrepareString(account) 
                      + ", " + quiz + ")");
        return id;
    }

    /// <summary>
    /// Get all questions and their answers for the chosen quiz
    /// </summary>
    /// <param name="quiz">Quiz ID</param>
    /// <returns>Returns a list of questions and their respective answers</returns>
    public static async Task<List<Questions>> GetQuestionsForChosenQuiz(int quiz) {
        int selection = (int)Table.Questions;
        // "SELECT question_id, question FROM Questions WHERE fk_quiz_id = @quiz"
        string sql = "SELECT " + PrimaryKeyID[selection] + ", question FROM " + TableNames[selection] +
                     " WHERE fk_quiz_id = " + quiz;
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        List<Questions> questions = new List<Questions>();
        for (int i = 0; i < value.questionResult.Count; i++) {
            Questions q = new Questions();
            q.question_id = value.questionResult[i].question_id;
            q.question = value.questionResult[i].question;
            q.answers = new List<Answers>();
            selection = (int)Table.Answers;
            // "SELECT answer_id, answer, is_correct FROM Answers WHERE fk_question_id = @question"
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
            questions.Add(q);
        }
        return questions;
    }

    /// <summary>
    /// After question completion the results are inserts into the ResultsQA table
    /// and the Results or LecturesAttend table is updated with the users current score
    /// </summary>
    /// <param name="result">Copy of the QuestionResults class</param>
    /// <param name="isLecture">Is this a lecture</param>
    public static async void UpdateResultsAfterQuestionAnswered(QuestionResults result, bool isLecture) {
        if (!isLecture) {
            if (result.isCorrect == 1) {
                int selection = (int) Table.Results;
                string sql =
                    "SELECT result_value FROM Results WHERE result_id = " + result.fk_results_id;
                string                  json        = (string) await crud.Read(sql, ModelNames[selection]);
                DatabaseCrud.JsonResult value       = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
                int                     resultValue = value.resultResult[0].result_value + 1;
                crud.DbCreate("UPDATE Results SET result_value = " + resultValue + " WHERE result_id = " +
                              result.fk_results_id);
            }
            crud.DbCreate(
                "INSERT INTO ResultQA (fk_result_id, fk_question_id, fk_answer_id) VALUES (" + result.fk_results_id +
                ", " + result.fk_question_id + ", " + result.fk_answer_id + ")");
        } else {
            if (result.isCorrect == 1) {
                int selection = (int) Table.LectureAttend;
                string sql =
                    "SELECT attend_value FROM LectureAttend WHERE attend_id = " + result.fk_attend_id;
                string                  json        = (string) await crud.Read(sql, ModelNames[selection]);
                DatabaseCrud.JsonResult value       = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
                int                     attendValue = value.lectureAttendResult[0].attend_value + 1;
                crud.DbCreate("UPDATE LectureAttend SET attend_value = " + attendValue + " WHERE attend_id = " +
                              result.fk_attend_id);
            }
            crud.DbCreate(
                "INSERT INTO ResultQA (fk_attend_id, fk_question_id, fk_answer_id) VALUES (" + result.fk_attend_id +
                ", " + result.fk_question_id + ", " + result.fk_answer_id + ")");
        }
    }

    /// <summary>
    /// Once completed the results table is set to complete
    /// </summary>
    /// <param name="result">Result ID</param>
    public static void UpdateResultsToIsCompleted(int result) {
        crud.DbCreate("UPDATE Results SET is_completed = 1 WHERE result_id = " + result);
    }

    /// <summary>
    /// Get the overall results from the database
    /// </summary>
    /// <param name="id">Results ID or LectureAttend ID</param>
    /// <param name="isLecture">Is this a lecture</param>
    /// <returns></returns>
    public static async Task<int> GetTotalCorrectFromResults(int id, bool isLecture) {
        string returnValue;
        int selection;
        if (!isLecture) {
            selection = (int)Table.Results;
            returnValue = "result_value";
            // "SELECT result_value FROM Results WHERE result_id = @id"
        } else {
            selection = (int)Table.LectureAttend;
            returnValue = "attend_value";
            // "SELECT attend_value FROM LectureAttend WHERE attend_id = @id"
        }
        string sql = "SELECT " + returnValue + " FROM " + TableNames[selection] +
                     " WHERE " + PrimaryKeyID[selection] + " = " + id;
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (!isLecture) {
            return value.resultResult[0].result_value;
        }
        return value.lectureAttendResult[0].attend_value;
    }

    /// <summary>
    /// Check is an answer to a question is correct or not
    /// </summary>
    /// <param name="id">Result or LectureAttend ID</param>
    /// <param name="question">Question ID</param>
    /// <param name="isLecture">Is this a lecture</param>
    /// <returns>Returns true is the question was answered correctly</returns>
    public static async Task<bool> GetWasAnswerCorrect(int id, int question, bool isLecture) {
        int selection = (int)Table.Answers;
        string selectValue;
        if (!isLecture) {
            selectValue = "fk_result_id";
            // "SELECT is_correct FROM Answers, ResultQA WHERE ResultQA.fk_result_id = @result AND " +
            // "ResultQA.fk_question_id = @question AND ResultQA.fk_answer_id = Answers.answer_id",
        } else {
            selectValue = "fk_attend_id";
            // "SELECT is_correct FROM Answers, ResultQA WHERE ResultQA.fk_attend_id = @result AND " +
            // "ResultQA.fk_question_id = @question AND ResultQA.fk_answer_id = Answers.answer_id",
        }
        string sql = "SELECT is_correct FROM " + TableNames[selection] + ", ResultQA WHERE ResultQA." + selectValue +
                     " = " + id + " AND ResultQA.fk_question_id = " + question +
                     " AND ResultQA.fk_answer_id = Answers.answer_id";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        if (value.answerResult[0].is_correct == 1) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get the correct answer for a given question
    /// </summary>
    /// <param name="question">Question ID</param>
    /// <returns>Returns the correct answer</returns>
    public static async Task<string> GetCorrectAnswer(int question) {
        int selection = (int)Table.Answers;
        // "SELECT answer FROM Answers WHERE fk_question_id = @id AND is_correct = 1"
        string sql = "SELECT answer FROM " + TableNames[selection] + " WHERE fk_question_id = " + question +
                     " AND is_correct = 1";
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        return value.answerResult[0].answer;
    }

    /// <summary>
    /// Get the answer the user gave for a given question
    /// </summary>
    /// <param name="result">Result ID</param>
    /// <param name="question">Question ID</param>
    /// <param name="isLecture">Is this a lecture</param>
    /// <returns></returns>
    public static async Task<int> GetStudentsAnswerId(int result, int question, bool isLecture) {
        int selection = (int)Table.ResultQA;
        string selectValue;
        if (!isLecture) {
            selectValue = "fk_result_id";
           // "SELECT fk_answer_id FROM ResultQA WHERE fk_result_id = @result AND fk_question_id = @question",
        } else {
            selectValue = "fk_attend_id";
           // "SELECT fk_answer_id FROM ResultQA WHERE fk_attend_id = @result AND fk_question_id = @question",
        }
        string sql = "SELECT fk_answer_id FROM " + TableNames[selection] + " WHERE " + selectValue + " = " + result +
                     " AND fk_question_id = " + question;
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        return value.resultQaResult[0].fk_answer_id;
    }

    /// <summary>
    /// Get the actual answer of a given Answer ID
    /// </summary>
    /// <param name="answer">Answer ID</param>
    /// <returns>Returns the actual answer</returns>
    public static async Task<string> GetActualAnswer(int answer) {
        int selection = (int) Table.Answers;
        // "SELECT answer FROM Answers WHERE answer_id = @id"
        string sql = "SELECT answer FROM Answers WHERE answer_id = " + answer;
        string json = (string) await crud.Read(sql, ModelNames[selection]);
        DatabaseCrud.JsonResult value = JsonUtility.FromJson<DatabaseCrud.JsonResult>(json);
        string text = value.answerResult[0].answer;
        return (text == null) ? "" : text;
    }

    /// <summary>
    /// Update the time elapsed during a quiz
    /// </summary>
    /// <param name="result">Result ID</param>
    /// <param name="time">Time Elapsed</param>
    public static void UpdateTimeElapsed(int result, int time) {
        crud.DbCreate("UPDATE Results SET time_elapsed = " + time + " WHERE result_id = " + result);
    }
}