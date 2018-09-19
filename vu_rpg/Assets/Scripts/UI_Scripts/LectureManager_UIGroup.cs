using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Administration {
    public void Lecture_Admin() {
        DeactivateActivateGroup(groupAdmin);
    }
}

/// <summary>
/// The lecture manager class manages the implementation of new lectures
/// </summary>
public class LectureManager_UIGroup : MonoBehaviour {

    public GameObject inputBox;
    public Dropdown dropBox;
    public GameObject primaryButton;
    public GameObject secondaryButton;
    public GameObject backButton;

    private Administration admin;
    private List<string> content;

    private Lecture lecture;
    private LectureBreakPoint breakPoint;

    private int answerCount = 0;
    public enum UI_STATE {
        Begin,
        LectureName,
        SetCourse,
        SetSubject,
        SetURL,
        SetBreak,
        SetQuestion,
        SetAnswer,
        COUNT
    };

    private UI_STATE currentUI;

    void Start() {
        admin = FindObjectOfType<Administration>();
        currentUI = UI_STATE.Begin;
        BeginState();
    }

    /// <summary>
    /// Handles all destinations of the primary button press,
    /// depending on the current workflow (state)
    /// </summary>
    public async void PrimaryButton() {
        switch (currentUI) {
            case UI_STATE.LectureName:
                lecture = new Lecture();
                lecture.lecture_title = inputBox.GetComponent<InputField>().text;
                SetCourse();
                break;
            case UI_STATE.SetCourse:
                lecture.course_name = dropBox.GetComponent<Dropdown>().options[dropBox.GetComponent<Dropdown>().value].text;
                SetSubject();
                break;
            case UI_STATE.SetSubject:
                lecture.fk_subject_name = dropBox.GetComponent<Dropdown>().options[dropBox.GetComponent<Dropdown>().value].text;
                SetURL();
                break;
            case UI_STATE.SetURL:
                lecture.lecture_url = inputBox.GetComponent<InputField>().text;
                lecture.break_points = new List<LectureBreakPoint>();
                // add first step to database.
                lecture.lecture_id = await Database.CreateNewLectureInit(lecture, FindObjectOfType<Player>().account);
                Message("Lecture Details added to Database");
                SetBreak();
                break;
            case UI_STATE.SetBreak:
                breakPoint = new LectureBreakPoint();
                breakPoint.break_time = Convert.ToInt32(inputBox.GetComponent<InputField>().text);
                SetQuestion();
                break;
            case UI_STATE.SetQuestion:
                breakPoint.break_question = new Questions();
                breakPoint.break_question.question = inputBox.GetComponent<InputField>().text;
                breakPoint.break_question.answers = new List<Answers>();
                answerCount = 0;
                SetAnswer();
                break;
            case UI_STATE.SetAnswer:
                int isCorrect = (answerCount == 1) ? 1 : 0;
                Answers ans = new Answers();
                ans.answer = inputBox.GetComponent<InputField>().text;
                ans.isCorrect = isCorrect;
                breakPoint.break_question.answers.Add(ans);
                answerCount++;
                if (answerCount > 4) {
                    // add this step to database.
                    breakPoint = await Database.CreateNewBreakForLecture(breakPoint, lecture.lecture_id);
                    // Return to 
                    Message("Question and Answers Added to Database");
                    SetBreak();
                } else {
                    SetAnswer();
                }
                break;
        }
    }

    /// <summary>
    /// Handles all destinations of the secondary button press,
    /// depending on the current workflow (state)
    /// </summary>
    public void SecondaryButton() {
        switch (currentUI) {
            case UI_STATE.SetURL:
                Message("Feature Coming Soon.");
                break;
            case UI_STATE.SetBreak:
                Message("Lecture has been added");
                BeginState();
                break;
        }
    }

    /// <summary>
    /// Handles all destinations of the back button press,
    /// depending on the current workflow (state)
    /// </summary>
    public void BackButton() {
        switch (currentUI) {
            case UI_STATE.LectureName:
                admin.Lecture_Admin();
                break;
            case UI_STATE.SetCourse:
                currentUI = UI_STATE.LectureName;
                PrimaryButton();
                break;
            case UI_STATE.SetSubject:
                currentUI = UI_STATE.SetCourse;
                PrimaryButton();
                break;
            case UI_STATE.SetURL:
                currentUI = UI_STATE.SetSubject;
                PrimaryButton();
                break;
            default:
                Message("You must continue at this point");
                break;
        }
    }

    /// <summary>
    /// Sets the UI and variable to an initial starting state
    /// </summary>
    private void BeginState() {
        ActivateAllUi();
        dropBox.gameObject.SetActive(false);
        secondaryButton.SetActive(false);
        admin.SetHeadingText("Enter Title for Lecture");
        primaryButton.GetComponentInChildren<Text>().text = "Add\nLecture\nTitle";
        currentUI = UI_STATE.LectureName;
    }

    /// <summary>
    /// Sets the UI and variable to an set a course state
    /// </summary>
    private async void SetCourse() {
        ActivateAllUi();
        inputBox.SetActive(false);
        secondaryButton.SetActive(false);
        admin.SetHeadingText("Select a Course");
        content = new List<string>();
        content = await Database.GetCourseNames();
        PopulateDropbox.Run(ref dropBox, content, "Select Course");
        primaryButton.GetComponentInChildren<Text>().text = "Select\nCourse";
        currentUI = UI_STATE.SetCourse;
    }

    /// <summary>
    /// Sets the UI and variable to an set a subject state
    /// </summary>
    private async void SetSubject() {
        ActivateAllUi();
        inputBox.SetActive(false);
        secondaryButton.SetActive(false);
        admin.SetHeadingText("Select a Subject");
        content = new List<string>();
        content = await Database.GetSubjectsLinkedToCourse(lecture.course_name);
        PopulateDropbox.Run(ref dropBox, content, "Select Subject");
        primaryButton.GetComponentInChildren<Text>().text = "Select\nSubject";
        currentUI = UI_STATE.SetSubject;
    }

    /// <summary>
    /// Sets the UI and variable to an set a URL state
    /// </summary>
    private void SetURL() {
        ActivateAllUi();
        dropBox.gameObject.SetActive(false);
        admin.SetHeadingText("Enter URL to your Video (MP4/AVI)");
        primaryButton.GetComponentInChildren<Text>().text = "Add\nURL";
        secondaryButton.GetComponentInChildren<Text>().text = "Upload\nVideo";
        currentUI = UI_STATE.SetURL;
    }

    /// <summary>
    /// Sets the UI and variable to an set a break point state
    /// </summary>
    private void SetBreak() {
        ActivateAllUi();
        dropBox.gameObject.SetActive(false);
        admin.SetHeadingText("Set a Break Point in the Video");
        primaryButton.GetComponentInChildren<Text>().text = "Add\nBreak\nPoint";
        secondaryButton.GetComponentInChildren<Text>().text = "Skip\nBreak\nPoint";
        currentUI = UI_STATE.SetBreak;
    }

    /// <summary>
    /// Sets the UI and variable to an add a question state
    /// This function gets called numerous times depending
    /// on how many answers have been recorded
    /// </summary>
    private void SetQuestion() {
        ActivateAllUi();
        admin.SetHeadingText("Set your question");
        dropBox.gameObject.SetActive(false);
        secondaryButton.SetActive(false);
        inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter your question here...";
        primaryButton.GetComponentInChildren<Text>().text = "Add\nQuestion";
        currentUI = UI_STATE.SetQuestion;
    }

    /// <summary>
    /// Sets the UI and variable to an set an answer state
    /// </summary>
    private void SetAnswer() {
        ActivateAllUi();
        string heading = "";
        if (answerCount < 1) {
            heading = "Set the correct answer";
            answerCount = 1;
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter the correct answer here...";
        } else {
            heading = "Set " + answerCount + "/4 incorrect answer";
            inputBox.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Enter an incorrect answer here...";
        }
        admin.SetHeadingText(heading);
        dropBox.gameObject.SetActive(false);
        secondaryButton.SetActive(false);
        backButton.SetActive(false);
        string buttonName = "";
        if (answerCount > 1) {
            buttonName = answerCount + "/4 : Add\nIncorrect\nAnswer";
        } else {
            buttonName = "Add\nCorrect\nAnswer";
        }
        primaryButton.GetComponentInChildren<Text>().text = buttonName;
        currentUI = UI_STATE.SetAnswer;
    }

    // HELPER Functions Below

    /// <summary>
    /// Activates the UI to a default state 
    /// </summary>
    private void ActivateAllUi() {
        inputBox.SetActive(true);
        dropBox.gameObject.SetActive(true);
        primaryButton.SetActive(true);
        secondaryButton.SetActive(true);
        backButton.SetActive(true);
        backButton.GetComponentInChildren<Text>().text = "Back";
        inputBox.GetComponent<InputField>().contentType = InputField.ContentType.Standard;
        inputBox.GetComponent<InputField>().characterLimit = 255;
        inputBox.GetComponentInChildren<Text>().text = "";
        inputBox.GetComponent<InputField>().text = "";
    }

    /// <summary>
    /// Passes a user feedback message to be displayed
    /// </summary>
    /// <param name="message">The message to display</param>
    private void Message(string message) {
        FindObjectOfType<UISystemMessage>().NewTextAndDisplay(message);
    }
}
