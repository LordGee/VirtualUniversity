using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;

public partial class Database {

    static void Initialize_StudentQuiz() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Results (
                            result_id INTEGER NOT NULL PRIMARY KEY,
                            result_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                            result_value INTEGER DEFAULT 0,
                            is_completed INTEGER DEFAULT 0, 
                            fk_account TEXT NOT NULL,
                            fk_quiz_id INTEGER NOT NULL)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS ResultQA (
                            result_qa_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            fk_result_id INTEGER NOT NULL,
                            fk_question_id INTEGER NOT NULL,
                            fk_answer_id INTEGER NOT NULL)");
    }

    // get all quizes by course... narrow to subject later
    public static void GetStudentQuizzes(ref List<Quiz> quiz, string account, string course) {
        List<List<object>> result = ExecuteReader(
            "SELECT quiz_id, quiz_name, number_questions, creation_date, quiz_owner, Quizes.fk_subject_name " +
            "FROM Quizes, Subjects, CourseSubjects WHERE Quizes.fk_subject_name = Subjects.subject_name AND " +
            "Subjects.subject_name = CourseSubjects.fk_subject_name AND CourseSubjects.fk_course_name = @course " +
            "GROUP BY quiz_name ORDER BY quiz_name",
            new SqliteParameter("@course", course));
        for (int i = 0; i < result.Count; i++) {
            if (HasQuizBeenCompleted(account, Convert.ToInt32(result[i][0]))) {
                Quiz temp = new Quiz();
                temp.QuizId = Convert.ToInt32(result[i][0]);
                temp.QuizName = (string) result[i][1];
                temp.NumberQuestions = Convert.ToInt32(result[i][2]);
                temp.CourseName = GetCourseNameFromSubject((string)result[i][5]);
                temp.SubjectName = (string)result[i][5];
                quiz.Add(temp);
            }
        }
    }

    private static bool HasQuizBeenCompleted(string account, int quiz) {
        int count =
            Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM Results WHERE fk_quiz_id = @quiz AND fk_account = @account AND is_completed = 1",
                new SqliteParameter("@quiz", quiz), new SqliteParameter("@account", account)));
        if (count == 0) {
            return true;
        } else {
            return false;
        }
    }

    private static string GetCourseNameFromSubject(string subject) {
        return (string) ExecuteScalar("SELECT fk_course_name FROM CourseSubjects WHERE fk_subject_name = @subject",
            new SqliteParameter("@subject", subject));
    }

    public static int CreateNewResultsForChoosenQuiz(string account, int quiz) {
        object id = ExecuteScalar("SELECT result_id FROM Results ORDER BY result_id DESC LIMIT 1");
        int result = Convert.ToInt32(id) + 1;
        ExecuteNoReturn("INSERT INTO Results (result_id, fk_account, fk_quiz_id) VALUES (@id, @account, @quiz)",
            new SqliteParameter("@id", result), new SqliteParameter("@account", account),
            new SqliteParameter("@quiz", quiz));
        return result;
    }

    public static List<Questions> GetQuestionsForChoosenQuiz(int quiz) {
        List<Questions> questions = new List<Questions>();

        List<List<object>> questionResults = ExecuteReader("SELECT question_id, question FROM Questions WHERE fk_quiz_id = @quiz", new SqliteParameter("@quiz", quiz));
        for (int i = 0; i < questionResults.Count; i++) {
            Questions q = new Questions();
            q.question_id = Convert.ToInt32(questionResults[i][0]);
            q.question = (string) questionResults[i][1];
            q.answers = new List<Answers>();
            List<List<object>> answerResults =
                ExecuteReader("SELECT answer_id, answer, is_correct FROM Answers WHERE fk_question_id = @question",
                    new SqliteParameter("@question", q.question_id));
            for (int j = 0; j < answerResults.Count; j++) {
                Answers a = new Answers();
                a.answer_id = Convert.ToInt32(answerResults[j][0]);
                a.answer = (string) answerResults[j][1];
                a.isCorrect = Convert.ToInt32(answerResults[j][2]);
                q.answers.Add(a);
            }
            questions.Add(q);
        }

        return questions;
    }

    public static void UpdateResultsAfterQuestionAnswered(QuestionResults result) {
        if (result.isCorrect == 1) {
            ExecuteNoReturn("UPDATE Results SET result_value = result_value + 1 WHERE result_id = @id", new SqliteParameter("@id", result.fk_results_id));
        }
        ExecuteNoReturn(
            "INSERT INTO ResultQA (fk_result_id, fk_question_id, fk_answer_id) VALUES (@result, @question, @answer)",
            new SqliteParameter("@result", result.fk_results_id),
            new SqliteParameter("@question", result.fk_question_id),
            new SqliteParameter("@answer", result.fk_answer_id));
    }

    public static void UpdateResultsToIsCompleted(int result) {
        ExecuteNoReturn("UPDATE Results SET is_completed = 1 WHERE result_id = @id",
            new SqliteParameter("@id", result));
    }

    public static int GetTotalCorrectFromResults(int result) {
        object value = ExecuteScalar("SELECT result_value FROM Results WHERE result_id = @id",
            new SqliteParameter("@id", result));
        return Convert.ToInt32(value);
    }

    public static bool GetWasAnswerCorrect(int result, int question) {
        object value = ExecuteScalar(
            "SELECT is_correct FROM Answers, ResultQA WHERE ResultQA.fk_result_id = @result AND " +
            "ResultQA.fk_question_id = @question AND ResultQA.fk_answer_id = Answers.answer_id",
            new SqliteParameter("@result", result), new SqliteParameter("@question", question));
        if (Convert.ToInt32(value) == 1) {
            return true;
        }
        return false;
    }

    public static string GetCorrectAnswer(int question) {
        object text = ExecuteScalar("SELECT answer FROM Answers WHERE fk_question_id = @id AND is_correct = 1",
            new SqliteParameter("@id", question));
        return text.ToString();
    }

    public static int GetStudentsAnswerId(int result, int question) {
        object id = ExecuteScalar(
            "SELECT fk_answer_id FROM ResultQA WHERE fk_result_id = @result AND fk_question_id = @question",
            new SqliteParameter("@result", result), new SqliteParameter("@question", question));
        return Convert.ToInt32(id);
    }

    public static string GetActualAnswer(int answer) {
        object text = ExecuteScalar("SELECT answer FROM Answers WHERE answer_id = @id",
            new SqliteParameter("@id", answer));
        return text.ToString();
    }
}