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
    public GameObject studentQuizPanel;
    public GameObject studentLecturePanel;

    private bool isAdmin, isChecked;


    async void checkAdminState() {
        if (!isChecked && panel.activeSelf) {
            isChecked = true;
            isAdmin = await Database.IsPlayerAdmin(FindObjectOfType<NetworkManagerMMO>().loginAccount);
        }
    }

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

    private void OpenAdminPanel() {
        FindObjectOfType<Administration>().BeginAdministrationUI();
        Hide();
    }

    public void Hide() {
        panel.SetActive(false);
        isChecked = false;
    }
}
