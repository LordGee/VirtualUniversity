using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add new subject UI
/// </summary>
public class AddSubject_UIGroup : MonoBehaviour {

    public GameObject newSubjectText;

	void Start () {
	    newSubjectText.GetComponent<InputField>().text = "";
	}

    /// <summary>
    /// Returns the new subject enter in the input field as a string
    /// </summary>
    /// <returns>Returns the subject</returns>
    public string GetNewSubject() {
        return newSubjectText.GetComponent<InputField>().text;
    }
}
