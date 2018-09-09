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
    }

    [Serializable]
    public class ModelLectureBreak {
        public int break_id;
    }

    [Serializable]
    public class ModelLectureAttend {
        public int attend_id;
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
}
