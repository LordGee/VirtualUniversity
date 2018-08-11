using System.Collections.Generic;
using UnityEngine;

public class Quiz {
    /// <summary>
    /// Table: Course
    /// </summary>
    private string course_name;
    public string CourseName {
        get { return course_name; }
        set { course_name = value; }
    }

    private string subject_name;
    public string SubjectName {
        get { return subject_name; }
        set { subject_name = value; }
    }
    /// <summary>
    /// Table: Quizes
    /// </summary>
    private string quiz_name;
    public string QuizName {
        get { return quiz_name; }
        set { quiz_name = value; }
    }

    private int number_questions;
    public int NumberQuestions {
        get { return number_questions; }
        set { number_questions = value; }
    }

    private string quiz_owner;

    private List<Questions> questions;
    public List<Questions> Questions {
        get { return questions; }
        set { questions = value; }
    }

    private int quiz_id;
    public int QuizId {
        get { return quiz_id; }
        set { quiz_id = value; }
    }

    public Quiz() {
        questions = new List<Questions>();
    }
}

public class Questions {
    /// <summary>
    /// Table: Questions
    /// </summary>
    public int question_id;
    public string question;

    /// <summary>
    /// Table: Answers
    /// </summary>
    public List<Answers> answers;
}

public class Answers {
    public int answer_id;
    public string answer;
    public int isCorrect; // was boolean, stored as int in database.
}

public class QuestionResults {
    public int fk_results_id;
    public int fk_question_id;
    public int fk_answer_id;
    public int isCorrect;
}