using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the select subject / course UI
/// within the subject administration
/// </summary>
public class SelectSubject_UIGroup : MonoBehaviour {

    public Dropdown courseDropdown;
    private List<string> courses;

    void Start() {
        UpdateCourseData();
    }

    /// <summary>
    /// Gets the latest course data from the database
    /// </summary>
    public async void UpdateCourseData() {
        courses = new List<string>();
        courses = await Database.GetCourseNames();
        PopulateCourseData();
    }

    /// <summary>
    /// Populates the dropdown with all the course data
    /// </summary>
    private void PopulateCourseData() {
        courseDropdown.ClearOptions();
        courseDropdown.AddOptions(courses);
    }

    /// <summary>
    /// Returns the selected course from the dropdown options
    /// </summary>
    /// <returns>Returns selected course</returns>
    public string GetSelectedCourse() {
        return courseDropdown.options[courseDropdown.value].text;
    }
}