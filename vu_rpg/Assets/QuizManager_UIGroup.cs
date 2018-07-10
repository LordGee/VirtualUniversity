using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Administration {
    public void Quiz_Admin() {
        DeactivateActivateGroup(groupAdmin);
    }
}

public class QuizManager_UIGroup : MonoBehaviour {

    public GameObject inputBox;
    public GameObject dropBox;
    public GameObject primaryButton;
    public GameObject secondaryButton;

    private List<string> content;

    public enum UI_STATE {
        Begin,
        QuestionManager,
        SelectCourse,
        SelectSubject,
        NameQuiz,
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
        }
    }

    public void PrimaryButton() {
        switch (currentUI) {
            case UI_STATE.Begin:
                
                break;
        }
    }

    public void SecondaryButton() {
        switch (currentUI) {
            case UI_STATE.Begin:

                break;
        }
    }

    private void BeginState() {
        inputBox.SetActive(false);
        inputBox.GetComponent<InputField>().text = "";

        dropBox.SetActive(true);
        content = new List<string>();
        content = Database.GetQuizNames();
        PopulateDropboxData("Select Quiz");

        primaryButton.SetActive(true);
        primaryButton.GetComponentInChildren<Text>().text = "Manage\nSelected\nQuiz";

        secondaryButton.SetActive(true);
        secondaryButton.GetComponentInChildren<Text>().text = "Create\nNew\nQuiz";

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
