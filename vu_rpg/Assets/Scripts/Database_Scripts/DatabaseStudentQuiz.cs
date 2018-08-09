using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;

public partial class Database {

    static void Initialize_StudentQuiz() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Results (
                            result_id INTEGER NOT NULL PRIMARY KEY autoincrement,
                            result_date TIMESTAMP,
                            result_value INTEGER,
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
        int z = 0;
    }

    private static bool HasQuizBeenCompleted(string account, int quiz) {
        int count =
            Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM Results WHERE fk_quiz_id = @quiz AND fk_account = @account",
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


}