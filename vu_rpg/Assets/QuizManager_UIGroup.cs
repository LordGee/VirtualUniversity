using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Administration {
    public void Quiz_Admin() {
        DeactivateActivateGroup(groupAdmin);
    }
}

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
    public int NumberQuestions
    {
        get { return number_questions; }
        set { number_questions = value; }
    }

    private string quiz_owner;

    private List<Questions> questions;
    public List<Questions> Questions
    {
        get { return questions; }
        set { questions = value; }
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
    public string answer;
    public bool isCorrect;
}

public class QuizManager_UIGroup : MonoBehaviour {

    public GameObject inputBox;
    public GameObject dropBox;
    public GameObject primaryButton;
    public GameObject secondaryButton;

    private List<string> content;
    private Quiz quiz;
    private Questions tempQuestion;

    private Administration admin;

    private bool isNew;

    public enum UI_STATE {
        Begin,
        QuestionManager,
        SelectCourse,
        SelectSubject,
        NameQuiz,
        NumberOfQuestions,
        AddQuestion,
        AddCorrectAnswer,
        AddWrongAnswer,
        EditQuestion,
        EditCorrectAnswer,
        EditWrongAnswer,

        Count
    };

    private UI_STATE currentUI;

    void Start() {
        admin = FindObjectOfType<Administration>();
        currentUI = UI_STATE.Begin;
        SetupUI();
    }

    public void SetupUI() {
        switch (currentUI) {
            case UI_STATE.Begin :
                BeginState();
                break;
        }
    }

    public void BackButton() {
        switch (currentUI) {
            case UI_STATE.Begin:
                FindObjectOfType<Administration>().Quiz_Admin();
                break;
            case UI_STATE.SelectCourse:
                currentUI = UI_STATE.Begin;
                BeginState();
                break;
            case UI_STATE.SelectSubject:
                currentUI = UI_STATE.SelectCourse;
                SelectCourse();
                break;
            case UI_STATE.NameQuiz:
                currentUI = UI_STATE.SelectSubject;
                break;
            default:
                Debug.LogWarning("This button is not assigned");
                break;
        }
    }

    public void PrimaryButton() {
        switch (currentUI) {
            case UI_STATE.Begin:
                currentUI = UI_STATE.QuestionManager;
                isNew = false;
                // Todo: add question manager function
                break;
            case UI_STATE.SelectCourse:
                currentUI = UI_STATE.SelectSubject;
                SelectSubject();
                break;
            case UI_STATE.SelectSubject:
                currentUI = UI_STATE.NameQuiz;
                NameQuiz();
                break;
            case UI_STATE.NameQuiz:
                currentUI = UI_STATE.NumberOfQuestions;
                NumberOfQuestions();
                break;
            case UI_STATE.NumberOfQuestions:
                currentUI = UI_STATE.AddQuestion;
                SetQuestion();
                break;
            case UI_STATE.AddQuestion:
                currentUI = UI_STATE.AddCorrectAnswer;
                AddCorrectAnswer();
                break;
            default:
                Debug.LogWarning("This button is not assigned");
                break;
        }
    }

    public void SecondaryButton() {
        switch (currentUI) {
            case UI_STATE.Begin:
                currentUI = UI_STATE.SelectCourse;
                isNew = true;
                SelectCourse();
                break;

            default:
                Debug.LogWarning("This button is not assigned");
                break;
        }
    }

    private void BeginState() {
        ActivateAllUi();
        inputBox.GetComponent<InputField>().text = "";
        inputBox.SetActive(false);
        
        admin.SetHeadingText("Add / Edit Quizes");

        content = new List<string>();
        content = Database.GetQuizNames();
        PopulateDropboxData("Select Quiz");

        primaryButton.GetComponentInChildren<Text>().text = "Manage\nSelected\nQuiz";

        secondaryButton.GetComponentInChildren<Text>().text = "Create\nNew\nQuiz";
    }

    private void SelectCourse() {
        ActivateAllUi();
        inputBox.SetActive(false);
        secondaryButton.SetActive(false);

        admin.SetHeadingText("Select a Course");

        content = new List<string>();
        content = Database.GetCourseNames();
        PopulateDropboxData("Select Course");

        primaryButton.GetComponentInChildren<Text>().text = "Select\nCourse";

        quiz = new Quiz();
    }

    private void SelectSubject() {
        quiz.CourseName = dropBox.GetComponent<Dropdown>().options[dropBox.GetComponent<Dropdown>().value].text;
        Message("Course Added");

        ActivateAllUi();
        inputBox.SetActive(false);
        secondaryButton.SetActive(false);

        admin.SetHeadingText("Select a Subject");

        content = new List<string>();
        content = Database.GetSubjectsLinkedToCourse(quiz.CourseName); 
        PopulateDropboxData("Select Subject");

        primaryButton.GetComponentInChildren<Text>().text = "Select\nSubject";
    }

    private void NameQuiz() {
        quiz.SubjectName = dropBox.GetComponent<Dropdown>().options[dropBox.GetComponent<Dropdown>().value].text;
        Message("Subject Added");

        string heading = (isNew) ? "Name your new quiz" : "Update the name of your quiz";
        admin.SetHeadingText(heading);

        ActivateAllUi();
        dropBox.SetActive(false);
        secondaryButton.SetActive(false);

        if (isNew) {
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Give your new quiz a name...";
        }

        string buttonName = (isNew) ? "Create\nQuiz" : "Update\nQuiz\nName";
        primaryButton.GetComponentInChildren<Text>().text = buttonName;
    }

    private void NumberOfQuestions() {
        quiz.QuizName = inputBox.GetComponent<InputField>().text;
        Message("Quiz Name Added");

        string heading = (isNew) ? "How many questions will be asked" : "Update how many questions will be asked";
        admin.SetHeadingText(heading);

        ActivateAllUi();
        dropBox.SetActive(false);
        secondaryButton.SetActive(false);

        if (isNew) {
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter number of questions to be asked...";
        }

        inputBox.GetComponentInChildren<Text>().text = "";
        inputBox.GetComponent<InputField>().text = "";
        inputBox.GetComponent<InputField>().contentType = InputField.ContentType.IntegerNumber;
        inputBox.GetComponent<InputField>().characterLimit = 3;
        string buttonName = (isNew) ? "Set\nNumber\nQuestions" : "Update\nNumber\nQuestions";
        primaryButton.GetComponentInChildren<Text>().text = buttonName;
    }

   

    private void SetQuestion()
    {
        quiz.NumberQuestions = Int32.Parse(inputBox.GetComponent<InputField>().text);
        Message("Number of Questions Added");

        string heading = (isNew) ? "Set you first question" : "Update this question";
        admin.SetHeadingText(heading);

        ActivateAllUi();
        dropBox.SetActive(false);
        secondaryButton.SetActive(false);

        inputBox.GetComponent<InputField>().contentType = InputField.ContentType.Standard;
        inputBox.GetComponentInChildren<Text>().text = "";
        if (isNew) {
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter your question here...";
        }

        string buttonName = (isNew) ? "Add\nQuestion" : "Update\nQuestion";
        primaryButton.GetComponentInChildren<Text>().text = buttonName;
    }

    private void AddCorrectAnswer() {
        tempQuestion = new Questions();
        tempQuestion.question = inputBox.GetComponent<InputField>().text;
        Message("Question Added");

        string heading = (isNew) ? "Set the correct answer" : "Update the correct answer";
        admin.SetHeadingText(heading);
    }

    /// <summary>
    /// HELPER Functions Below
    /// </summary>

    public void ConstrainInputNumber() {
        if (currentUI != UI_STATE.NumberOfQuestions) { return; }
        int value = Int32.Parse(inputBox.GetComponent<InputField>().text);
        if (value < 1) {
            inputBox.GetComponent<InputField>().text = "1";
        } else if (value > 500) {
            inputBox.GetComponent<InputField>().text = "500";
        }
    }

    private void ActivateAllUi() {
        inputBox.SetActive(true);
        dropBox.SetActive(true);
        primaryButton.SetActive(true);
        secondaryButton.SetActive(true);
    }

    private void PopulateDropboxData(string caption) {
        dropBox.GetComponent<Dropdown>().ClearOptions();
        dropBox.GetComponent<Dropdown>().AddOptions(content);
        dropBox.GetComponent<Dropdown>().captionText.text = caption;
    }

    private void Message(string message) {
        FindObjectOfType<UISystemMessage>().NewTextAndDisplay(message);
    }
}
