using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCourse_UIGroup : MonoBehaviour {

    public GameObject newCourseText;

    public string GetNewCourse() {
        return newCourseText.GetComponent<InputField>().text;
    }
}
