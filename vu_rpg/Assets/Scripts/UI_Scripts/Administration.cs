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
    public UIAdminGroups groupQuiz;

    private UIAdminGroups current;
    private Player player;

    public void BeginAdministrationUI() {
        current = groupAdmin;
        current.groupObject.SetActive(true);
        SetHeadingText(current.title);
        player = FindObjectOfType<Player>();
    }

    public void ExitAdministrationUI() {
        current.groupObject.SetActive(false);
        this.gameObject.SetActive(false);
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

    /// <summary>
    /// Needed to store in database at various points of the administration process.
    /// Presume this is the primary key in the player table.
    /// </summary>
    /// <returns>Player Account Name</returns>
    public string GetPlayerName() {
        return player.account;
    }

}
