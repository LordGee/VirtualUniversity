using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSubject_UIGroup : MonoBehaviour {

    public Dropdown courseDropdown;
    private List<string> courses;

    void Start() {
        UpdateCourseData();
    }

    public void UpdateCourseData() {
        courses = new List<string>();
        courses = Database.GetCourseNames();
        PopulateCourseData();
    }

    private void PopulateCourseData() {
        courseDropdown.ClearOptions();
        courseDropdown.AddOptions(courses);
        courseDropdown.captionText.text = "Existing Subjects";
    }

    public string GetSelectedCourse() {
        return courseDropdown.options[courseDropdown.value].text;
    }


}
