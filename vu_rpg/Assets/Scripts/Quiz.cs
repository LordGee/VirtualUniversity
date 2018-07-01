using System.Collections.Generic;
using UnityEngine;

struct Questions {
    /// <summary>
    /// Table: Questions
    /// </summary>
    public int question_id;
    public string question;
    /// <summary>
    /// Table: Answers
    /// </summary>
    public string[] answers;
    public int correct;
}

public class QuizCreation {
    /// <summary>
    /// Table: Course
    /// </summary>
    private string course_name;
    /// <summary>
    /// Table: Quizes
    /// </summary>
    private string quiz_name;
    private string quiz_owner;

    private List<Questions> questions;

    /// <summary>
    /// Get and Set
    /// </summary>
    public string CourseName {
        get { return course_name; }
    }
    public string QuizName {
        get { return quiz_name; }
    }   

    public QuizCreation(string cName, string qName, string owner) {
        // check if course name if valid
        if (!Database.CheckCourseExists(cName)) {
            // Todo: throw an error redo class construct
            Debug.Log("Quiz.cs say:  Failed Course");
        }
        // check if quiz name is valid
        if (!Database.CheckQuizExists(qName)) {
            // Todo: throw an error redo class construct
            Debug.Log("Quiz.cs say:  Failed Quiz");
        }
        course_name = cName;
        quiz_name = qName;
        // Set Quiz Owner to account name
        quiz_owner = owner;
        // declare new list of questions
        questions = new List<Questions>();
        Debug.Log("Quiz.cs say:  Created instance of quiz");
    }

    public void AddQuestion(string question, string ans1, string ans2, string ans3, int correct) {
        // check each question null or empty 
        // check each answer null or empty 
        // check correct result is within range
        // Add objects to question list.
    }


}

public class Quiz {
    /// <summary>
    /// Table: Course
    /// </summary>
    private string course_name;
    /// <summary>
    /// Table: Quizes
    /// </summary>
    private string quiz_name;
    private string quiz_owner;

    private List<Questions> questions;

}
