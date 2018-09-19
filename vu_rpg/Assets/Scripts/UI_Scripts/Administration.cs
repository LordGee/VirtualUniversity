using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A struct to combine to data types for use in the unity inspector
/// </summary>
[System.Serializable]
public struct UIAdminGroups {
    public GameObject groupObject;
    public string title;
}

/// <summary>
/// This class handles the navigation of all elements within the Administration panel
/// </summary>
public partial class Administration : MonoBehaviour {

    public GameObject backPanel;
    public Text headingText;

    [Header("UI Groups")]
    public UIAdminGroups groupAdmin; 
    public UIAdminGroups groupSubject;
    public UIAdminGroups groupQuiz;
    public UIAdminGroups groupLecture;

    private UIAdminGroups current;
    private Player player;

    /// <summary>
    /// Sets the UI to the start position
    /// </summary>
    public void BeginAdministrationUI() {
        backPanel.SetActive(true);
        current = groupAdmin;
        current.groupObject.SetActive(true);
        SetHeadingText(current.title);
        player = FindObjectOfType<Player>();
    }

    /// <summary>
    /// Exits the administration and takes the user back to the game world
    /// </summary>
    public void ExitAdministrationUI() {
        backPanel.SetActive(false);
    }

    /// <summary>
    /// Sets the heading text in the UI to a new value
    /// </summary>
    /// <param name="value">New heading text</param>
    public void SetHeadingText(string value) {
        headingText.text = value;
    }

    /// <summary>
    /// Not used anymore
    /// </summary>
    /// <param name="value">new message to display</param>
    public void DisplayUserMessage(string value) {
        FindObjectOfType<UISystemMessage>().NewTextAndDisplay(value);
    }

    /// <summary>
    /// Deactivates the current group of UI and activates the new defined group
    /// </summary>
    /// <param name="open">UI Group to activate next</param>
    private void DeactivateActivateGroup(UIAdminGroups open) {
        current.groupObject.SetActive(false);
        current = open;
        current.groupObject.SetActive(true);
        SetHeadingText(current.title);
    }

    /// <summary>
    /// Deactivates the current UI group
    /// </summary>
    private void DeactivateGroup() {
        current.groupObject.SetActive(false);
    }

    /// <summary>
    /// Needed to store in database at various points of the administration process.
    /// Presume this is the primary key in the player table.
    /// </summary>
    /// <returns>Player Account Name</returns>
    public string GetPlayerName() {
        return FindObjectOfType<NetworkManagerMMO>().loginAccount;
    }
}