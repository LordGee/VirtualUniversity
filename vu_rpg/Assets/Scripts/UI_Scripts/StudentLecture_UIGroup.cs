using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class StudentLecture_UIGroup : MonoBehaviour {

    [Header("Panels")]
    public GameObject lectureSelectionPanel;
    public GameObject lectureQuestionPanel;
    public GameObject lectureResultsPanel;

    [Header("Lecture")]
    public Camera lectureCamera;
    public VideoPlayer video;

    [Header("Lecture Selection")]
    public Transform lectureContent;
    public QuizSelectionSlot slotPrefab;

    [Header("Lecture Questions")]
    public Text questionText;
    public Button[] answerButtons;

    [Header("Lecture Results")]
    public Text resultsHeadingText;
    public Text resultsSubHeadingText;
    public Transform resultContent;
    public QuizResultSlot resultSlot;

    private int chosenLecture;
    private Player player;
    private List<Lecture> lectures;
    private int attend_id;
    private List<QuestionResults> results;
    private bool startLecture;
    private List<bool> breakComplete;
    private int currentBreakIndex;
    private List<int> answerIndexOrder;
    private int selectedAnswer;
    private bool isPaused, isQuestion;

    void OnApplicationFocus(bool hasFocus) { isPaused = !hasFocus; }

    void OnApplicationPause(bool pauseStatus) { isPaused = pauseStatus; }

    void OnApplicationQuit() { Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time)); }

    public async void InitStart() {
        chosenLecture = -1;
        lectureSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        lectures = new List<Lecture>();
        await Database.GetStudentLectures(lectures, FindObjectOfType<NetworkManagerMMO>().loginAccount, await Database.GetPlayerCourseName(FindObjectOfType<NetworkManagerMMO>().loginAccount));
        PopulateLectures();
        startLecture = false;
    }

    void Update() {
        if (startLecture) {
            if (video.isPlaying) {
                for (int i = 0; i < lectures[chosenLecture].break_points.Count; i++) {
                    // check for breakpoint questions
                    if (video.time >= lectures[chosenLecture].break_points[i].break_time && !breakComplete[i]) {
                        // pause video and display question
                        isQuestion = true;
                        BreakPoint(i);
                        breakComplete[i] = true;
                    }
                }
                if (isPaused) {
                    Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time));
                    video.Pause();
                }
            } else if (!isPaused && !isQuestion) {
                video.Play();
            }
            if (video.frame > 1 && video.frame == (long)video.frameCount) {
                EndLecture();
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

    private async void SelectedLecture() {
        if (await LoadChosenLecture()) {
            lectureCamera.gameObject.SetActive(true);
            breakComplete = new List<bool>();
            // Loop through number of breakpoints and set boolean value to determine if break has already been completed
            for (int i = 0; i < lectures[chosenLecture].break_points.Count; i++) {
                if (await Database.HasQuestionBeenAttempted(
                    lectures[chosenLecture].break_points[i].break_question.question_id, attend_id, true)) {
                    breakComplete.Add(true);
                } else {
                    breakComplete.Add(false);
                }
                
            }
            startLecture = true;
        }
    }

    private async Task<bool> LoadChosenLecture() {
        try {
            video.Stop();
            video.url = lectures[chosenLecture].lecture_url;
            if (lectures[chosenLecture].watch_time > 0) {
                video.time = (double)lectures[chosenLecture].watch_time;
            }
            lectureSelectionPanel.SetActive(false);
            isQuestion = false;
            video.Play();
            video.isLooping = false;
            results = new List<QuestionResults>();
            if (lectures[chosenLecture].attend_id >= 0) {
                attend_id = lectures[chosenLecture].attend_id;
            } else {
                attend_id = await Database.CreateNewLectureAttend(FindObjectOfType<NetworkManagerMMO>().loginAccount, lectures[chosenLecture].lecture_id);
            }
            return true;
        } catch (Exception e) {
            Debug.LogError("Lecture failed to load: " + e);
            return false;
        }
    }

    private void BreakPoint(int index) {
        currentBreakIndex = index;
        video.Pause();
        lectureCamera.gameObject.SetActive(false);
        lectureQuestionPanel.gameObject.SetActive(true);
        PrepareQuestion();
        SetupAnswerButton();
        Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time));
    }

    private void PrepareQuestion() {
        questionText.text = lectures[chosenLecture].break_points[currentBreakIndex].break_question.question;
        List<bool> hasAnswerBeenAllocated = new List<bool>();
        answerIndexOrder = new List<int>();
        for (int i = 0; i < lectures[chosenLecture].break_points[currentBreakIndex].break_question.answers.Count; i++) {
            hasAnswerBeenAllocated.Add(false);
        }
        while (!AllocationTest.HasAllocationFinished(hasAnswerBeenAllocated)) {
            int index = Random.Range(0, lectures[chosenLecture].break_points[currentBreakIndex].break_question.answers.Count);
            if (!hasAnswerBeenAllocated[index]) {
                answerIndexOrder.Add(index);
                hasAnswerBeenAllocated[index] = true;
            }
        }
    }

    private void SetupAnswerButton() {
        for (int i = 0; i < lectures[chosenLecture].break_points[currentBreakIndex].break_question.answers.Count; i++) {
            answerButtons[i].GetComponentInChildren<Text>().text = lectures[chosenLecture].break_points[currentBreakIndex]
                .break_question.answers[answerIndexOrder[i]].answer;
            int count = i;
            answerButtons[i].onClick.SetListener(() => {
                selectedAnswer = answerIndexOrder[count];
                ConfirmAnswer();
            });
        }
    }

    private void ConfirmAnswer() {
        QuestionResults result = new QuestionResults();
        result.fk_attend_id = attend_id; 
        result.fk_answer_id = lectures[chosenLecture].break_points[currentBreakIndex].break_question.answers[selectedAnswer].answer_id;
        result.fk_question_id = lectures[chosenLecture].break_points[currentBreakIndex].break_question.question_id;
        result.isCorrect = lectures[chosenLecture].break_points[currentBreakIndex].break_question.answers[selectedAnswer].isCorrect;
        results.Add(result);
        Database.UpdateResultsAfterQuestionAnswered(result, true);
        
        if (result.isCorrect == 1) {
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay("CORRECT");
        } else {
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Wrong");
        }

        ResumeLecture();
    }

    private void ResumeLecture() {
        lectureCamera.gameObject.SetActive(true);
        lectureQuestionPanel.gameObject.SetActive(false);
        isQuestion = false;
        video.Play();
    }

    private async void EndLecture() {
        lectureCamera.gameObject.SetActive(false);
        lectureResultsPanel.gameObject.SetActive(true);
        video.Stop();
        isQuestion = true;
        startLecture = false;
        resultsHeadingText.text = "Results for " + lectures[chosenLecture].lecture_title;

        // Calculate result as percentage
        int totalQuestions = lectures[chosenLecture].break_points.Count;
        int totalCorrect = await Database.GetTotalCorrectFromResults(attend_id, true);
        float percentage = 0;
        if (totalCorrect != 0 || totalQuestions != 0) {
            percentage = (float)(totalCorrect * 100) / totalQuestions;
        }
        resultsSubHeadingText.text = "Your Result is " + Math.Ceiling(percentage) + "%";

        // Show each question and define correct and wrong answers.
        UIUtils.BalancePrefabs(resultSlot.gameObject, totalQuestions, resultContent);
        for (int i = 0; i < totalQuestions; i++) {
            QuizResultSlot slot = resultContent.GetChild(i).GetComponent<QuizResultSlot>();
            slot.nameText.text = "Q" + (i + 1) + ". " + lectures[chosenLecture].break_points[i].break_question.question;
            slot.correctAnswerText.text = "Correct Answer: " + await Database.GetCorrectAnswer(lectures[chosenLecture].break_points[i].break_question.question_id);
            if (await Database.GetWasAnswerCorrect(attend_id, lectures[chosenLecture].break_points[i].break_question.question_id, true)) {
                slot.selectButton.GetComponentInChildren<Text>().text = "CORRECT";
                slot.selectButton.GetComponent<Image>().color = Color.green;
            } else {
                slot.selectButton.GetComponentInChildren<Text>().text = "Incorrect";
                slot.selectButton.GetComponent<Image>().color = Color.yellow;
                slot.wrongAnswerText.text = "You Answered: " +
                                            await Database.GetActualAnswer(await Database.GetStudentsAnswerId(attend_id,
                                                lectures[chosenLecture].break_points[i].break_question.question_id, true));
            }
        }

        // update lecture attend table to be completed.
        Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time));
        Database.UpdateLectureAttendToComplete(attend_id);
    }

    public void ExitQuizSelection() {
        lectureSelectionPanel.gameObject.SetActive(false);
    }

    public void ExitResultsPanel() {
        lectureResultsPanel.gameObject.SetActive(false);
        InitStart();
    }
}
