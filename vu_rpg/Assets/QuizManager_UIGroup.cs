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
    public string answer;
    public int isCorrect; // was boolean, stored as int in database.
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
    private int answerCount = 0;

    private const int MAX_ANSWER = 4;

    public enum UI_STATE {
        Begin,
        QuestionManager,
        SelectCourse,
        SelectSubject,
        NameQuiz,
        NumberOfQuestions,
        AddQuestion,
        AddAnswer,
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
                AddQuizToDatabase();
                currentUI = UI_STATE.AddQuestion;
                SetQuestion();
                break;
            case UI_STATE.AddQuestion:
                currentUI = UI_STATE.AddAnswer;
                answerCount = -1;
                AddAnswer();
                break;
            case UI_STATE.AddAnswer:
                Debug.Log("Count : " + answerCount);
                if (answerCount <= 4) {
                    if (answerCount >= 1) {
                        SetAnswer();
                        AddAnswer();
                    }
                    if (answerCount == 4) {
                        AddQuestionToDatabase();
                        AddAnswersToDatabase();
                        answerCount = -1;
                        Message("Add another question?");
                        currentUI = UI_STATE.AddQuestion;
                        SetQuestion();
                    }
                } else {
                   
                } 
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
        // moved to add quiz to database function
        Message("Number of Questions Added");

        string heading = (isNew) ? "Set your question" : "Update this question";
        admin.SetHeadingText(heading);

        ActivateAllUi();
        dropBox.SetActive(false);
        secondaryButton.SetActive(false);

        
        if (isNew) {
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter your question here...";
        }

        string buttonName = (isNew) ? "Add\nQuestion" : "Update\nQuestion";
        primaryButton.GetComponentInChildren<Text>().text = buttonName;
    }

    private void AddAnswer() {
        string heading = "";
        if (answerCount < 1) {
            tempQuestion = new Questions();
            tempQuestion.answers = new List<Answers>();
            tempQuestion.question = inputBox.GetComponent<InputField>().text;
            Message("Question Added");
            heading = (isNew) ? "Set the correct answer" : "Update the correct answer";
            answerCount = 1;
        } else {
            Message("Answer Added");
            heading = (isNew) ? "Set " + answerCount + "/4 incorrect answer" : "Update " + answerCount + "/4 incorrect answer";
        }

        admin.SetHeadingText(heading);

        ActivateAllUi();
        dropBox.SetActive(false);
        secondaryButton.SetActive(false);

        inputBox.GetComponentInChildren<Text>().text = "";

        string buttonName = "";
        if (isNew) {
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter the correct answer here...";
            if (answerCount > 1) {
                buttonName = answerCount + "/4 : Add\nIncorrect\nAnswer";
            } else {
                buttonName = "Add\nCorrect\nAnswer";
            }
        } else {
            if (answerCount > 1) {
                buttonName = answerCount + "/4 : Update\nIncorrect\nAnswer";
            } else {
                buttonName = "Update\nCorrect\nAnswer";
            }
        }        

        primaryButton.GetComponentInChildren<Text>().text = buttonName;
    }

    private void SetAnswer() {
        Answers answer = new Answers();
        answer.answer = inputBox.GetComponent<InputField>().text;
        if (answerCount == 1) {
            answer.isCorrect = 1;
        } else {
            answer.isCorrect = 0;
        }
        tempQuestion.answers.Add(answer);
        answerCount++;
        Message("Answer Added");
    }

    private void AddQuizToDatabase() {
        quiz.NumberQuestions = Int32.Parse(inputBox.GetComponent<InputField>().text);
        Debug.Log("Nuber of questions = " + quiz.NumberQuestions);
        quiz.QuizId = Database.GetNewIDForQuiz();
        Database.CreateNewQuiz(quiz.QuizId, quiz.QuizName, quiz.NumberQuestions, "Gordon", quiz.SubjectName);
        // todo change to user account name:
    }

    private void AddQuestionToDatabase() {
        tempQuestion.question_id = Database.GetNewIDForQuestion();
        Database.AddQuestionToQuiz(tempQuestion.question_id, tempQuestion.question, quiz.QuizId);
    }

    private void AddAnswersToDatabase() {
        for (int i = 0; i < tempQuestion.answers.Count; i++) {
            Database.AddAnswerToQuestion(tempQuestion.answers[i].answer, tempQuestion.answers[i].isCorrect, tempQuestion.question_id);
        }
    }

    /// <summary>
    /// HELPER Functions Below
    /// </summary>

    private void ActivateAllUi() {
        inputBox.SetActive(true);
        dropBox.SetActive(true);
        primaryButton.SetActive(true);
        secondaryButton.SetActive(true);
        inputBox.GetComponent<InputField>().contentType = InputField.ContentType.Standard;
        inputBox.GetComponent<InputField>().characterLimit = 255;
        inputBox.GetComponentInChildren<Text>().text = "";
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
