using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is an extension of the UINpcDialogue class.
/// The following are custom aspects that have been implemented.
/// </summary>
public partial class UINpcDialogue {

    [Header("Custom Options")]
    public Button quizButton;
    public Button lectureButton;
    public Button workshopButton;
    public Button adminButton;

    [Header("Custom Panels")]
    public GameObject studentQuizPanel;
    public GameObject studentLecturePanel;
    public GameObject game;
    private GameObject tempGamePanel;

    private bool isAdmin, isChecked;

    /// <summary>
    /// Checks the user account to see if user is an Admin  account type
    /// </summary>
    async void checkAdminState() {
        if (!isChecked && panel.activeSelf) {
            isChecked = true; // must before next line due to the await command
            isAdmin = await Database.IsPlayerAdmin(FindObjectOfType<NetworkManagerMMO>().loginAccount);
        }
    }

    /// <summary>
    /// This function is executed from the Update() method and checks for button presses
    /// </summary>
    /// <param name="npc">The NPC character being visited</param>
    /// <param name="player">Player class</param>
    private void UpdateButtons(Npc npc, Player player) {
        checkAdminState();
        teleportButton.gameObject.SetActive(npc.teleportTo != null);
        if (npc.teleportTo != null)
            teleportButton.GetComponentInChildren<Text>().text = "Teleport: " + npc.teleportTo.name;
        teleportButton.onClick.SetListener(() => { player.CmdNpcTeleport(); });
        if (!isAdmin) {
            // quiz
            quizButton.gameObject.SetActive(true);
            quizButton.onClick.SetListener(() => {
                Hide();
                studentQuizPanel.GetComponent<StudentQuiz_UIGroup>().InitStart();
            });
            // lecture
            lectureButton.gameObject.SetActive(true);
            lectureButton.onClick.SetListener(() => {
                Hide();
                studentLecturePanel.GetComponent<StudentLecture_UIGroup>().InitStart();
            });
            // workshop
            workshopButton.gameObject.SetActive(true);
            workshopButton.onClick.SetListener(() => {
                Hide();
                tempGamePanel = Instantiate(game);
                tempGamePanel.SetActive(true);
            });
        } else {
            quizButton.gameObject.SetActive(false);
            lectureButton.gameObject.SetActive(false);
            workshopButton.gameObject.SetActive(false);
        }
        // Admin Option
        if (isAdmin) {
            adminButton.gameObject.SetActive(true);
            adminButton.onClick.SetListener(() => { OpenAdminPanel(); });
        } else {
            adminButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Opens the administration panel
    /// </summary>
    private void OpenAdminPanel() {
        FindObjectOfType<Administration>().BeginAdministrationUI();
        Hide();
    }

    /// <summary>
    /// Resets the current panel and deactivates
    /// </summary>
    public void Hide() {
        panel.SetActive(false);
        isChecked = false;
    }
}
