﻿using UnityEngine;

public partial class Database {

    public enum Table {
        Quizzes,
        Questions,
        Lectures,
        LectureBreakPoints,
        LectureAttend,
        Courses,
        CourseSubjects,
        Subjects,
        Results,
        Answers,
        ResultQA,

        COUNT
    };

    protected static string[] PrimaryKeyID = {
        "quiz_id",
        "question_id",
        "lecture_id",
        "break_id",
        "attend_id",
        "course_name",
        "course_subject_id",
        "subject_name",
        "result_id",
        "answer_id",
        "result_qa_id",

    };
    protected static string[] TableNames = {
        "Quizzes",
        "Questions",
        "Lectures",
        "LectureBreakPoints",
        "LectureAttend",
        "Courses",
        "CourseSubjects",
        "Subjects",
        "Results",
        "Answers",
        "ResultQA",

    };
    protected static string[] ModelNames = {
        "quizResult",
        "questionResult",
        "lectureResult",
        "lectureBreakResult",
        "lectureAttendResult",
        "courseResult",
        "courseSubjectResult",
        "subjectResult",
        "resultResult",
        "answerResult",
        "resultQaResult",

    };

}
