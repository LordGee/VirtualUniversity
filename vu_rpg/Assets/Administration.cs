using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UIAdminGroups {
    public GameObject groupObject;
    public string title;
}

public partial class Administration : MonoBehaviour {

    public GameObject backPanel;
    public Text headingText;
    [Header("UI Groups")]
    public UIAdminGroups groupAdmin; 
    public UIAdminGroups groupSubject;
    public UIAdminGroups groupNewCourse;
    public UIAdminGroups groupNewSubject;

    public UIAdminGroups groupQuiz;

    private UIAdminGroups current;

    void Start() {
        current = groupAdmin;
        current.groupObject.SetActive(true);
        SetHeadingText(current.title);
    }

    public void SetHeadingText(string value) {
        headingText.text = value;
    }

    public void DisplayUserMessage(string value) {
        FindObjectOfType<UISystemMessage>().NewTextAndDisplay(value);
    }

    private void DeactivateActivateGroup(UIAdminGroups open) {
        current.groupObject.SetActive(false);
        current = open;
        current.groupObject.SetActive(true);
        SetHeadingText(current.title);
    }

    private void DeactivateGroup() {
        current.groupObject.SetActive(false);
        // todo exit administration as well
    }

    

}
