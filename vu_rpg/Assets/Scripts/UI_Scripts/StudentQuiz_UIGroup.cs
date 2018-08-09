using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentQuiz_UIGroup : MonoBehaviour {

    public GameObject quizSelectionPanel;

    private List<Quiz> quizzes;
    private Player player;

    private int choosenQuiz = -1;

    public void InitStart() {
        quizSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        quizzes = new List<Quiz>();
        Database.GetStudentQuizzes(ref quizzes, player.account, player.course);
    }

    private void PopulateQuizzes() {

    }
}
