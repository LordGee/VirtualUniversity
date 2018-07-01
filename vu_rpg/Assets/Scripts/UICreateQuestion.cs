using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

public class UICreateQuestion : MonoBehaviour {

    public NetworkManagerMMO manager;
    private List<string> courses;
    private QuizCreation quiz;

    [Header("Quiz Insert")]
    public GameObject quizCreationPanel;
    public Dropdown courseDropdown;
    public Text quizName;

    [Header("Question Insert")]
    public GameObject questionInsertPanel;
    public Text question;

    [Header("Answer Insert")]
    public Text[] answers;
    public Toggle[] results;

	void Start () {
	    courses = Database.GetCourseNames();
        PopulateCourseData();
	}

    public void PopulateCourseData() {
        courseDropdown.ClearOptions();
        courseDropdown.AddOptions(courses);
        courseDropdown.captionText.text = "Select a Subject";
    }

    public void btn_CreateNewQuiz() {
        quiz = new QuizCreation(
            courseDropdown.options[courseDropdown.value].text,
            quizName.text,
            "LordGee");
        // todo: add account name in place of LordGee
        // could be player.name or player.account
        quizCreationPanel.SetActive(false);
        questionInsertPanel.SetActive(true);
    }

    public void AddQuestion() {
        string[] answers = {this.answers[0].text, this.answers[1].text, this.answers[2].text};
        int result = -1;
        for (int i = 0; i < results.Length; i++) {
            if (results[i].isOn) {
                result = i;
            }
        }
        if (result == -1) {
            Debug.LogWarning("No correct answer");
            return;
        } 
        Database.AddNewQuestionAndAnswer(question.text, answers, result);
    }

    public void Hide() { questionInsertPanel.SetActive(false); }
    public void Show() { questionInsertPanel.SetActive(true); }
    public bool IsVisible() { return questionInsertPanel.activeSelf; }
}
