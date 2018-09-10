using System;
using System.Collections.Generic;

public partial class DatabaseCrud {

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
    }

    [Serializable]
    public class ModelQuestion {
        public int question_id;
        public string question;
        public int fk_quiz_id;
        public int fk_break_id;
    }

    [Serializable]
    public class ModelQuiz {
        public int quiz_id;
        public string quiz_name;
        public int quiz_timer;
        public DateTime creation_date;
        public string quiz_owner;
        public string fk_subject_name;
    }

    [Serializable]
    public class ModelLecture {
        public int lecture_id;
        public string lecture_title;
        public string lecture_url;
        public string lecture_owner;
        public string fk_subject_name;
    }

    [Serializable]
    public class ModelLectureBreak {
        public int break_id;
        public int break_time;
        public int fk_lecture_id;
    }

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

    [Serializable]
    public class ModelCourses {
        public string course_name;
    }

    [Serializable]
    public class ModelCourseSubject {
        public int course_subject_id;
        public string fk_course_name;
        public string fk_subject_name;
    }

    [Serializable]
    public class ModelSubject {
        public string subject_name;
    }

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

    [Serializable]
    public class ModelAnswer {
        public int answer_id;
        public string answer;
        public int is_correct;
        public int fk_question_id;
    }

    [Serializable]
    public class ModelResultQA {
        public int result_qa_id;
        public int fk_result_id;
        public int fk_attend_id;
        public int fk_question_id;
        public int fk_answer_id;
    }
}
