using UnityEngine;

public partial class Database {

    public enum Table {
        Quizzes,
        Questions,
        Lectures,
        LectureBreakPoints,
        LectureAttend,
        Courses,
        CourseSubjects,

        COUNT
    };

    public static int[] NextID = {
        0, 0, 0, 0, 0, 0, 0,
    };
    protected static string[] PrimaryKeyID = {
        "quiz_id",
        "question_id",
        "lecture_id",
        "break_id",
        "attend_id",
        "course_name",
        "course_subject_id",
    };
    protected static string[] TableNames = {
        "Quizzes",
        "Questions",
        "Lectures",
        "LectureBreakPoints",
        "LectureAttend",
        "Courses",
        "CourseSubjects",
    };
    protected static string[] ModelNames = {
        "quizResult",
        "questionResult",
        "lectureResult",
        "lectureBreakResult",
        "lectureAttendResult",
        "courseResult",
        "courseSubjectResult",
    };

}
