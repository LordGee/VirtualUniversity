using System;
using System.Collections.Generic;

/// <summary>
/// Extension of the Crud class dedicated to storing the models used in the database.
/// </summary>
public partial class DatabaseCrud {

    /// <summary>
    /// The Json Result combines all the models into one class.
    /// These can then be selected when required.
    /// This sounds extreme but reduces the amount of class that need to be written
    /// to hold each model.
    /// </summary>
    [Serializable]
    public class JsonResult {
        public List<ModelQuestion> questionResult;
        public List<ModelQuiz> quizResult;
        public List<ModelLecture> lectureResult;
        public List<ModelLectureBreak> lectureBreakResult;
        public List<ModelLectureAttend> lectureAttendResult;
        public List<ModelCourses> courseResult;
        public List<ModelCourseSubject> courseSubjectResult;
        public List<ModelSubject> subjectResult;
        public List<ModelResults> resultResult;
        public List<ModelAnswer> answerResult;
        public List<ModelResultQA> resultQaResult;
        public List<ModelEnrolled> enrolledResult;
    }

    /// <summary>
    /// Model of the Questions table
    /// </summary>
    [Serializable]
    public class ModelQuestion {
        public int question_id;
        public string question;
        public int fk_quiz_id;
        public int fk_break_id;
    }

    /// <summary>
    /// Model of the Quizzes table
    /// </summary>
    [Serializable]
    public class ModelQuiz {
        public int quiz_id;
        public string quiz_name;
        public int quiz_timer;
        public DateTime creation_date;
        public string quiz_owner;
        public string fk_subject_name;
    }

    /// <summary>
    /// Model of the Lecture table
    /// </summary>
    [Serializable]
    public class ModelLecture {
        public int lecture_id;
        public string lecture_title;
        public string lecture_url;
        public string lecture_owner;
        public string fk_subject_name;
    }

    /// <summary>
    /// Model of the LectureBreakPoint table
    /// </summary>
    [Serializable]
    public class ModelLectureBreak {
        public int break_id;
        public int break_time;
        public int fk_lecture_id;
    }

    /// <summary>
    /// Model of the LectureAttend table
    /// </summary>
    [Serializable]
    public class ModelLectureAttend {
        public int attend_id;
        public DateTime attend_date;
        public int attend_value;
        public int has_attended;
        public int watch_time;
        public string fk_account;
        public int fk_lecture_id;
    }

    /// <summary>
    /// Model of the Courses table
    /// </summary>
    [Serializable]
    public class ModelCourses {
        public string course_name;
    }

    /// <summary>
    /// Model of the CourseSubject table
    /// </summary>
    [Serializable]
    public class ModelCourseSubject {
        public int course_subject_id;
        public string fk_course_name;
        public string fk_subject_name;
    }

    /// <summary>
    /// Model of the Subject table
    /// </summary>
    [Serializable]
    public class ModelSubject {
        public string subject_name;
    }

    /// <summary>
    /// Model of the Results table
    /// </summary>
    [Serializable]
    public class ModelResults {
        public int result_id;
        public DateTime result_date;
        public int result_value;
        public int is_completed;
        public int time_elapsed;
        public string fk_account;
        public int fk_quiz_id;
    }

    /// <summary>
    /// Model of the Answers table
    /// </summary>
    [Serializable]
    public class ModelAnswer {
        public int answer_id;
        public string answer;
        public int is_correct;
        public int fk_question_id;
    }

    /// <summary>
    /// Model of the ResultQA table
    /// </summary>
    [Serializable]
    public class ModelResultQA {
        public int result_qa_id;
        public int fk_result_id;
        public int fk_attend_id;
        public int fk_question_id;
        public int fk_answer_id;
    }

    /// <summary>
    /// Model of the Enrolled table
    /// </summary>
    [Serializable]
    public class ModelEnrolled {
        public int enrolled_id;
        public string fk_account;
        public string fk_course_name;
        public string account_type;
    }
}
