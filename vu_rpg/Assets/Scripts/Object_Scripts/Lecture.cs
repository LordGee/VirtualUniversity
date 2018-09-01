﻿using System.Collections.Generic;

public class Lecture {
    public int lecture_id;
    public string lecture_title;
    public string lecture_url;
    public string course_name;
    public string fk_subject_name;
    public List<LectureBreakPoint> break_points;
}

public class LectureBreakPoint {
    public int break_id;
    public int break_time;
    public Questions break_question;
}