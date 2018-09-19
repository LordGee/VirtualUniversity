using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add New Course UI
/// </summary>
public class AddCourse_UIGroup : MonoBehaviour {
    
    public GameObject newCourseText;

    /// <summary>
    /// Gets the text from the input field and returns the string
    /// </summary>
    /// <returns>Returns text of the new course</returns>
    public string GetNewCourse() {
        return newCourseText.GetComponent<InputField>().text;
    }
}
