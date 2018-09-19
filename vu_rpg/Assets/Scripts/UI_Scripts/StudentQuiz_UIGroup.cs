using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// The StudentQuiz_UIGroup handles all the functionality from a
/// student selecting a quiz through to completion
/// </summary>
public class StudentQuiz_UIGroup : MonoBehaviour {

    [Header("Panels")]
    public GameObject quizSelectionPanel;
    public GameObject quizQuestionPanel;
    public GameObject quizResultsPanel;

    [Header("Quiz Selection")]
    public Transform quizContent;
    public QuizSelectionSlot slotPrefab;
    public Text questionHeading;

    [Header("Quiz Questions")]
    public Text questionSubHeading;
    public Text questionText;
    public Button[] answerButtons;

    [Header("Quiz Results")]
    public Text resultsHeadingText;
    public Text resultsSubHeadingText;
    public Transform resultContent;
    public QuizResultSlot resultSlot;

    private List<Quiz> quizzes;

    private int chosenQuiz = -1;
    private bool selectionQuiz = false;
    private int results_id;

    private bool startQuiz = false;
    
    private float quizTimer, currentTime, lastUpdate;
    private int currentQuestion = 0;

    private List<int> questionIndexOrder;
    private List<bool> hasQuestionBeenAllocated;

    private List<int> answerIndexOrder;
    private List<bool> hasAnswerBeenAllocated;

    private int selectedAnswer = -1;

    private List<QuestionResults> results;

    /// <summary>
    /// Sets the initial UI for the Quiz Selection screen
    /// </summary>
    public async void InitStart() {
        chosenQuiz = -1;
        quizSelectionPanel.SetActive(true);
        quizzes = new List<Quiz>();
        hasQuestionBeenAllocated = new List<bool>();
        questionIndexOrder = new List<int>();
        quizzes = await Database.GetStudentQuizzes(quizzes, FindObjectOfType<NetworkManagerMMO>().loginAccount,
            await Database.GetPlayerCourseName(FindObjectOfType<NetworkManagerMMO>().loginAccount));
        PopulateQuizzes();
        selectionQuiz = true;
        startQuiz = false;
        currentTime = 0.0f;
    }

    /// <summary>
    /// Update function handles the quiz timers
    /// </summary>
    void Update() {
        if (selectionQuiz) {
            PopulateQuizzes();
            if (chosenQuiz != -1) {
                selectionQuiz = false;
            }
        } else if (startQuiz) {
            if (quizTimer < 0) {
                FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Time has run out");
                EndQuiz();
            }
            if (lastUpdate - quizTimer >= 1.0f) {
                questionSubHeading.text = UpdateSubHeading();
                lastUpdate = quizTimer;
                questionHeading.text = UpdateHeading();
                if (Random.Range(0,10) > 9.0f) {
                    Database.UpdateTimeElapsed(results_id, (quizzes[chosenQuiz].QuizTimer * _CONST.SECONDS_IN_MINUTE) - (int)quizTimer);
                }
            }
            quizTimer -= Time.timeSinceLevelLoad - currentTime;
            currentTime = Time.timeSinceLevelLoad;
            SetupAnswerButton();
        }
    }

    /// <summary>
    /// All available quizzes are obtained from the
    /// database and loaded into the UI for quiz selection.
    /// </summary>
    private void PopulateQuizzes() {
        UIUtils.BalancePrefabs(slotPrefab.gameObject, quizzes.Count, quizContent);
        for (int i = 0; i < quizzes.Count; i++) {
            QuizSelectionSlot slot = quizContent.GetChild(i).GetComponent<QuizSelectionSlot>();
            slot.nameText.text = quizzes[i].QuizName;
            int count = i;
            slot.selectButton.onClick.SetListener(async () => {
                chosenQuiz = count;
                results = new List<QuestionResults>();
                if (quizzes[chosenQuiz].result_id >= 0) {
                    results_id = quizzes[chosenQuiz].result_id;
                } else {
                    results_id = await Database.CreateNewResultsForChosenQuiz(FindObjectOfType<NetworkManagerMMO>().loginAccount, quizzes[chosenQuiz].QuizId);
                }
                SelectedQuiz();
            });
        }
    }

    /// <summary>
    /// Once the quiz has been selected the UI is prepared
    /// </summary>
    private void SelectedQuiz() {
        quizSelectionPanel.gameObject.SetActive(false);
        quizQuestionPanel.gameObject.SetActive(true);
        questionHeading.text = UpdateHeading();
        currentTime = Time.timeSinceLevelLoad;
        currentQuestion = 0;
        PrepareQuestionsAndAnswers();
    }

    /// <summary>
    /// Based on the quiz selection all of the questions and answers are loaded into memory.
    /// The questions are randomized
    /// </summary>
    private async void PrepareQuestionsAndAnswers() {
        quizzes[chosenQuiz].Questions = await Database.GetQuestionsForChosenQuiz(quizzes[chosenQuiz].QuizId);
        hasQuestionBeenAllocated = new List<bool>();
        questionIndexOrder = new List<int>();
        for (int i = 0; i < quizzes[chosenQuiz].Questions.Count; i++) {
            hasQuestionBeenAllocated.Add(false);
        }
        while (!AllocationTest.HasAllocationFinished(hasQuestionBeenAllocated)) {
            int index = Random.Range(0, quizzes[chosenQuiz].Questions.Count);
            if (!hasQuestionBeenAllocated[index]) {
                if (! await Database.HasQuestionBeenAttempted(quizzes[chosenQuiz].Questions[index].question_id, results_id, false)) {
                    questionIndexOrder.Add(index);
                }
                hasQuestionBeenAllocated[index] = true;
            }
        }
        if (quizzes[chosenQuiz].time_elapsed > 0) {
            quizTimer = (quizzes[chosenQuiz].QuizTimer * _CONST.SECONDS_IN_MINUTE) - quizzes[chosenQuiz].time_elapsed;
        } else {
            quizTimer = quizzes[chosenQuiz].QuizTimer * _CONST.SECONDS_IN_MINUTE;
        }
        lastUpdate = quizzes[chosenQuiz].QuizTimer * _CONST.SECONDS_IN_MINUTE;
        startQuiz = true;
        NextQuestion();
    }

    /// <summary>
    /// Prepares the next question to be asked a randomizes
    /// the answers before being allocated to a button
    /// </summary>
    private void NextQuestion() {
        questionText.text = quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].question;
        hasAnswerBeenAllocated = new List<bool>();
        answerIndexOrder = new List<int>();
        for (int i = 0; i < quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count; i++) {
            hasAnswerBeenAllocated.Add(false);
        }
        while (!AllocationTest.HasAllocationFinished(hasAnswerBeenAllocated)) {
            int index = Random.Range(0, quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count);
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
        for (int i = 0; i < quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count; i++) {
            answerButtons[i].GetComponentInChildren<Text>().text = quizzes[chosenQuiz]
                .Questions[questionIndexOrder[currentQuestion]].answers[answerIndexOrder[i]].answer;
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
        result.fk_results_id = results_id;
        result.fk_answer_id = quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers[selectedAnswer].answer_id;
        result.fk_question_id = quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].question_id;
        result.isCorrect = quizzes[chosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers[selectedAnswer].isCorrect;
        results.Add(result);
        Database.UpdateResultsAfterQuestionAnswered(result, false);
        Database.UpdateTimeElapsed(results_id, (quizzes[chosenQuiz].QuizTimer * _CONST.SECONDS_IN_MINUTE) - (int)quizTimer);
        if (currentQuestion < questionIndexOrder.Count - 1) {
            currentQuestion++;
            if (result.isCorrect == 1) {
                FindObjectOfType<UISystemMessage>().NewTextAndDisplay("CORRECT");
            } else {
                FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Wrong");
            }
            NextQuestion();
        } else {
            EndQuiz();
        }
    }

    /// <summary>
    /// Once the quiz has concluded the results are
    /// calculated and the quiz results UI is displayed
    /// </summary>
    private async void EndQuiz() {
        // Update Database with result is_completed
        // todo: make this call async as its currently messing with the final results
        Database.UpdateResultsToIsCompleted(results_id); 
        // Display Result Panel
        startQuiz = false;
        quizQuestionPanel.gameObject.SetActive(false);
        quizResultsPanel.gameObject.SetActive(true);
        resultsHeadingText.text = "Results for " + quizzes[chosenQuiz].QuizName;
        // Calculate result as percentage
        int totalQuestions = quizzes[chosenQuiz].Questions.Count;
        int totalCorrect = await Database.GetTotalCorrectFromResults(results_id, false);
        float percentage = 0;
        if (totalCorrect != 0 || totalQuestions != 0) {
            percentage = (float) (totalCorrect * 100) / totalQuestions;
        }
        resultsSubHeadingText.text = "Your Result is " + Math.Ceiling(percentage) + "%";
        // Show each question and define correct and wrong answers.
        UIUtils.BalancePrefabs(resultSlot.gameObject, totalQuestions, resultContent);
        for (int i = 0; i < totalQuestions; i++) {
            QuizResultSlot slot = resultContent.GetChild(i).GetComponent<QuizResultSlot>();
            slot.nameText.text = "Q" + (i + 1) + ". " + quizzes[chosenQuiz].Questions[i].question;
            slot.correctAnswerText.text = "Correct Answer: " + await Database.GetCorrectAnswer(quizzes[chosenQuiz].Questions[i].question_id);
            if (await Database.GetWasAnswerCorrect(results_id, quizzes[chosenQuiz].Questions[i].question_id, false)) {
                slot.selectButton.GetComponentInChildren<Text>().text = "CORRECT";
                slot.selectButton.GetComponent<Image>().color = Color.green;
            } else {
                slot.selectButton.GetComponentInChildren<Text>().text = "Incorrect";
                slot.selectButton.GetComponent<Image>().color = Color.yellow;
                slot.wrongAnswerText.text = "You Answered: " +
                                            await Database.GetActualAnswer(await Database.GetStudentsAnswerId(
                                                results_id, quizzes[chosenQuiz].Questions[i].question_id, false));
            }
        }
    }

    /// <summary>
    /// Update the main heading of the UI
    /// </summary>
    /// <returns>Returns the new updated heading text</returns>
    private string UpdateHeading() {
        return quizzes[chosenQuiz].QuizName + " - " + (currentQuestion + 1) + "/" + questionIndexOrder.Count;
    }

    /// <summary>
    /// Updates the sub heading / time
    /// </summary>
    /// <returns>Returns the updated text</returns>
    private string UpdateSubHeading() {
        // Reference: https://answers.unity.com/questions/45676/making-a-timer-0000-minutes-and-seconds.html
        float minutes = Mathf.Floor(quizTimer / _CONST.SECONDS_IN_MINUTE);
        float seconds = quizTimer % _CONST.SECONDS_IN_MINUTE;
        string zero = (seconds < 10) ? "0" : "";
        return "Timer: " + minutes + ":" + zero + (int)seconds;
    }

    /// <summary>
    /// Deactivated quiz selection panel
    /// </summary>
    public void ExitQuizSelection() {
        quizSelectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivated quiz results panel and sets the starting values for quiz selection
    /// </summary>
    public void ExitResultsPanel() {
        quizResultsPanel.gameObject.SetActive(false);
        InitStart();
    }
}
