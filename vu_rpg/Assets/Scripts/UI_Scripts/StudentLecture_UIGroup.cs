using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

/// <summary>
/// The StudentLecture_UIGroup handles all the functionality from a
/// student selecting a lecture through to completion
/// </summary>
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
    private List<Lecture> lectures;
    private int attend_id;
    private List<QuestionResults> results;
    private bool startLecture;
    private List<bool> breakComplete;
    private int currentBreakIndex;
    private List<int> answerIndexOrder;
    private int selectedAnswer;
    private bool isPaused, isQuestion;

    /// <summary>
    /// If the application loses focus the lecture will pause
    /// </summary>
    /// <param name="hasFocus">Focus status</param>
    void OnApplicationFocus(bool hasFocus) { isPaused = !hasFocus; }

    /// <summary>
    /// If the application pauses this ensures the lecture also pauses
    /// </summary>
    /// <param name="pauseStatus">Pause status</param>
    void OnApplicationPause(bool pauseStatus) { isPaused = pauseStatus; }

    /// <summary>
    /// Upon application quiz this ensures that the current watch time is recorded
    /// </summary>
    void OnApplicationQuit() { Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time)); }

    /// <summary>
    /// Sets the initial UI for the Lecture Selection screen
    /// </summary>
    public async void InitStart() {
        chosenLecture = -1;
        lectureSelectionPanel.SetActive(true);
        lectures = new List<Lecture>();
        await Database.GetStudentLectures(lectures, FindObjectOfType<NetworkManagerMMO>().loginAccount, await Database.GetPlayerCourseName(FindObjectOfType<NetworkManagerMMO>().loginAccount));
        PopulateLectures();
        startLecture = false;
    }

    /// <summary>
    /// Update function handles the video timers
    /// </summary>
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

    /// <summary>
    /// All available lectures are obtained from the
    /// database and loaded into the UI for lecture selection.
    /// </summary>
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

    /// <summary>
    /// Once the lecture has been selected the UI is prepared
    /// </summary>
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

    /// <summary>
    /// Once the lecture has been selected the lecture video
    /// is then loaded into the video player component
    /// </summary>
    /// <returns>Returns true if load is successful</returns>
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

    /// <summary>
    /// Handles break point preparation by pausing
    /// video and preparing question
    /// </summary>
    /// <param name="index">Break Point Index value</param>
    private void BreakPoint(int index) {
        currentBreakIndex = index;
        video.Pause();
        lectureCamera.gameObject.SetActive(false);
        lectureQuestionPanel.gameObject.SetActive(true);
        PrepareQuestion();
        SetupAnswerButton();
        Database.UpdateLectureTime(attend_id, Mathf.FloorToInt((float)video.time));
    }

    /// <summary>
    /// Prepares the question and answers are loaded into memory.
    /// </summary>
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

    /// <summary>
    /// Once answer buttons have been allocated the following
    /// sets up the button listeners and sets the answer value
    /// </summary>
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

    /// <summary>
    /// Handles the process after a question has been answered by the user.
    /// These value are recorded and the database is updated.
    /// Feedback is given to the user whether correct or wrong.
    /// </summary>
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

    /// <summary>
    /// Resumes the lecture after a break point
    /// </summary>
    private void ResumeLecture() {
        lectureCamera.gameObject.SetActive(true);
        lectureQuestionPanel.gameObject.SetActive(false);
        isQuestion = false;
        video.Play();
    }

    /// <summary>
    /// Once the lecture has concluded the results are
    /// calculated and the lecture results UI is displayed
    /// </summary>
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

    /// <summary>
    /// Deactivated lecture selection panel
    /// </summary>
    public void ExitQuizSelection() {
        lectureSelectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivated quiz results panel and sets the starting values for quiz selection
    /// </summary>
    public void ExitResultsPanel() {
        lectureResultsPanel.gameObject.SetActive(false);
        InitStart();
    }
}
