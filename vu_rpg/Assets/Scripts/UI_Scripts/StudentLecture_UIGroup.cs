using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StudentLecture_UIGroup : MonoBehaviour {

    [Header("Panels")]
    public GameObject lectureSelectionPanel;
    public GameObject lectureQuestionPanel;

    [Header("Lecture")]
    public Camera lectureCamera;
    public VideoPlayer video;

    [Header("Lecture Selection")]
    public Transform lectureContent;
    public QuizSelectionSlot slotPrefab;
    public Text lectureHeading;

    [Header("Lecture Questions")]
    public Text questionSubHeading;
    public Text questionText;
    public Button[] answerButtons;

    private int chosenLecture;
    private float currentTime;
    private Player player;
    private List<Lecture> lectures;
    private bool startLecture;
    private List<bool> breakComplete;

    public void InitStart() {
        chosenLecture = -1;
        lectureSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        lectures = new List<Lecture>();
        Database.GetStudentLectures(ref lectures, player.account, player.course);
        PopulateLectures();
        startLecture = false;
        currentTime = 0.0f; 
    }

    void Update() {
        if (startLecture) {
            if (video.isPlaying) {
                Debug.Log("Video time is : " + video.time);
                for (int i = 0; i < lectures[chosenLecture].break_points.Count; i++) {
                    // check for breakpoint questions
                    if (video.time >= lectures[chosenLecture].break_points[i].break_time && !breakComplete[i]) {
                        // pause video and display question
                        Debug.Log("Question Time: " + i);
                        breakComplete[i] = true;
                    }
                }
            } else {
                Debug.Log("Video is NOT Playing");
                // switch off lecture camera and return to UI with results
            }
        }
    }

    private void PopulateLectures() {
        UIUtils.BalancePrefabs(slotPrefab.gameObject, lectures.Count, lectureContent);
        for (int i = 0; i < lectures.Count; i++) {
            QuizSelectionSlot slot = lectureContent.GetChild(i).GetComponent<QuizSelectionSlot>();
            slot.nameText.text = lectures[i].lecture_title;
            int count = i;
            slot.selectButton.onClick.SetListener(() => {
                chosenLecture = count;
                SelectedLecture();
            });
        }
    }

    private void SelectedLecture() {
        if (LoadChosenLecture()) {
            lectureCamera.gameObject.SetActive(true);
            breakComplete = new List<bool>();
            // Loop through number of breakpoints and set boolean value to determine if break has already been completed
            for (int i = 0; i < lectures[chosenLecture].break_points.Count; i++) {
                breakComplete.Add(false);
            }
            startLecture = true;
        }
    }

    private bool LoadChosenLecture() {
        try {
            video.Stop();
            video.url = lectures[chosenLecture].lecture_url;
            video.Play();
            video.isLooping = false;
            return true;
        } catch (Exception e) {
            Debug.LogError("Lecture failed to load: " + e);
            return false;
        }
    }
}
