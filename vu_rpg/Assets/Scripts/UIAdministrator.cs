using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAdministrator : MonoBehaviour {

    [Header("Course")]
    public GameObject courseManagementPanel;

    [Header("Quiz")]
    public GameObject quizCreationPanel;
    public Text newSubjectText;
    public Dropdown existingCourses;
    private List<string> courses;


    void Start() {
        courses = Database.GetCourseNames();
        PopulateCourseData();
    }

    public void PopulateCourseData() {
        existingCourses.ClearOptions();
        existingCourses.AddOptions(courses);
        existingCourses.captionText.text = "Existing Subjects";
    }

    public void btn_AddNewCourse() {
        if (newSubjectText.text.Length > 1) {
            Database.AddNewCourse(newSubjectText.text);
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Subject added successfully");
        } else {
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Failed to add new subject");
        }
    }

    public void OpenCoursePanel() {
        courseManagementPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OpenQuizPanel() {
        quizCreationPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
