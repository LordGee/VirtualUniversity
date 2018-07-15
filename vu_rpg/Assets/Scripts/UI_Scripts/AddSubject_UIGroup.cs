using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddSubject_UIGroup : MonoBehaviour {

    public GameObject newSubjectText;

	void Start () {
	    newSubjectText.GetComponent<InputField>().text = "";
	}

    public string GetNewSubject() {
        return newSubjectText.GetComponent<InputField>().text;
    }
	
}
