using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class UINpcDialogue {

    [Header("Custom Options")]
    public Button quizButton;
    public Button lectureButton;
    public Button workshopButton;
    public Button adminButton;

    [Header("Custom Panels")]
    public GameObject StudentQuizPanel;

    private void UpdateButtons(Npc npc, Player player) {

        teleportButton.gameObject.SetActive(npc.teleportTo != null);
        if (npc.teleportTo != null)
            teleportButton.GetComponentInChildren<Text>().text = "Teleport: " + npc.teleportTo.name;
        teleportButton.onClick.SetListener(() => { player.CmdNpcTeleport(); });

        if (!player.IsAdmin()) {
            // quiz
            quizButton.gameObject.SetActive(true);
            quizButton.onClick.SetListener((() => {
                Hide();
                StudentQuizPanel.GetComponent<StudentQuiz_UIGroup>().InitStart();
            }));
            // lecture
            lectureButton.gameObject.SetActive(true);

            // workshop
            workshopButton.gameObject.SetActive(true);
        } else {
            quizButton.gameObject.SetActive(false);
            lectureButton.gameObject.SetActive(false);
            workshopButton.gameObject.SetActive(false);
        }

        // Admin Option
        if (player.IsAdmin()) {
            adminButton.gameObject.SetActive(true);
            adminButton.onClick.SetListener(() => { OpenAdminPanel(); });
        } else {
            adminButton.gameObject.SetActive(false);
        }
    }

    private void OpenAdminPanel() {
        FindObjectOfType<Administration>().BeginAdministrationUI();
        Hide();
    }

    public void Hide() { panel.SetActive(false); }
}
