using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentLecture_UIGroup : MonoBehaviour {

    [Header("Panels")]
    public GameObject lectureSelectionPanel;
    public GameObject lectureQuestionPanel;

    [Header("Lecture Selection")]
    public Transform lectureContent;
    public QuizSelectionSlot slotPrefab;
    public Text lectureHeading;

    [Header("Lecture Questions")]
    public Text questionSubHeading;
    public Text questionText;
    public Button[] answerButtons;

    private int choosenLecture;
    private float currentTime;
    private Player player;
    private List<Lecture> lectures;
    private bool startLecture;

    public void InitStart() {
        choosenLecture = -1;
        lectureSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        lectures = new List<Lecture>();
        Database.GetStudentLectures(ref lectures, player.account, player.course);
        PopulateLectures();
        startLecture = false;
        currentTime = 0.0f; 
    }

    private void PopulateLectures() {
        UIUtils.BalancePrefabs(slotPrefab.gameObject, lectures.Count, lectureContent);
        for (int i = 0; i < lectures.Count; i++) {
            QuizSelectionSlot slot = lectureContent.GetChild(i).GetComponent<QuizSelectionSlot>();
            slot.nameText.text = lectures[i].lecture_title;
            int count = i;
            slot.selectButton.onClick.SetListener(() => {
                choosenLecture = count;
                SelectedLecture();
            });
        }
    }

    private void SelectedLecture() {

    }
}
