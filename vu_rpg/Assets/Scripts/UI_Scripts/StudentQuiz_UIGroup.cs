using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentQuiz_UIGroup : MonoBehaviour {

    public GameObject quizSelectionPanel;
    public Transform quizContent;
    public QuizSelectionSlot slotPrefab;

    private List<Quiz> quizzes;
    private Player player;

    private int choosenQuiz = -1;
    private bool selectionQuiz = false;

    public void InitStart() {
        choosenQuiz = -1;
        quizSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        quizzes = new List<Quiz>();
        Database.GetStudentQuizzes(ref quizzes, player.account, player.course);
        PopulateQuizzes();
        selectionQuiz = true;
    }

    void Update() {
        if (selectionQuiz) {
            PopulateQuizzes();
            if (choosenQuiz != -1) {
                selectionQuiz = false;
            }
        }
    }

    private void PopulateQuizzes() {
        UIUtils.BalancePrefabs(slotPrefab.gameObject, quizzes.Count, quizContent);
        for (int i = 0; i < quizzes.Count; i++) {
            QuizSelectionSlot slot = quizContent.GetChild(i).GetComponent<QuizSelectionSlot>();
            slot.nameText.text = quizzes[i].QuizName;
            slot.selectButton.onClick.SetListener(() => { choosenQuiz = i; });
        }
    }
}
