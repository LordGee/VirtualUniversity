using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private Player player;

    private int choosenQuiz = -1;
    private bool selectionQuiz = false;
    private int results_id;

    private bool startQuiz = false;
    private const int SECONDS_IN_MINUTE = 60;
    private float quizTimer, currentTime, lastUpdate;
    private int currentQuestion = 0;

    private List<int> questionIndexOrder;
    private List<bool> hasQuestionBeenAllocated;

    private List<int> answerIndexOrder;
    private List<bool> hasAnswerBeenAllocated;

    private int selectedAnswer = -1;

    private List<QuestionResults> results;

    public void InitStart() {
        choosenQuiz = -1;
        quizSelectionPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        quizzes = new List<Quiz>();
        Database.GetStudentQuizzes(ref quizzes, player.account, player.course);
        PopulateQuizzes();
        selectionQuiz = true;
        startQuiz = false;
        currentTime = 0.0f;
    }

    void Update() {
        if (selectionQuiz) {
            PopulateQuizzes();
            if (choosenQuiz != -1) {
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
            }
            quizTimer -= Time.timeSinceLevelLoad - currentTime;
            currentTime = Time.timeSinceLevelLoad;
            SetupAnswerButton();
        }
    }

    private void PopulateQuizzes() {
        UIUtils.BalancePrefabs(slotPrefab.gameObject, quizzes.Count, quizContent);
        for (int i = 0; i < quizzes.Count; i++) {
            QuizSelectionSlot slot = quizContent.GetChild(i).GetComponent<QuizSelectionSlot>();
            slot.nameText.text = quizzes[i].QuizName;
            int count = i;
            slot.selectButton.onClick.SetListener(() => {
                choosenQuiz = count;
                results = new List<QuestionResults>();
                results_id = Database.CreateNewResultsForChoosenQuiz(player.account, quizzes[choosenQuiz].QuizId);
                SelectedQuiz();
            });
        }
    }

    private void SelectedQuiz() {
        quizSelectionPanel.gameObject.SetActive(false);
        quizQuestionPanel.gameObject.SetActive(true);
        questionHeading.text = UpdateHeading();
        currentTime = Time.timeSinceLevelLoad;
        currentQuestion = 0;
        PrepareQuestionsAndAnswers();
    }

    private void PrepareQuestionsAndAnswers() {
        quizzes[choosenQuiz].Questions = Database.GetQuestionsForChoosenQuiz(quizzes[choosenQuiz].QuizId);
        hasQuestionBeenAllocated = new List<bool>();
        questionIndexOrder = new List<int>();
        for (int i = 0; i < quizzes[choosenQuiz].Questions.Count; i++) {
            hasQuestionBeenAllocated.Add(false);
        }
        while (!HasAllocationFinished(hasQuestionBeenAllocated)) {
            int index = Random.Range(0, quizzes[choosenQuiz].Questions.Count);
            if (!hasQuestionBeenAllocated[index]) {
                questionIndexOrder.Add(index);
                hasQuestionBeenAllocated[index] = true;
            }
        }
        quizTimer = quizzes[choosenQuiz].QuizTimer * SECONDS_IN_MINUTE;
        lastUpdate = quizzes[choosenQuiz].QuizTimer * SECONDS_IN_MINUTE;
        startQuiz = true;
        NextQuestion();
    }

    private void NextQuestion() {
        questionText.text = quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].question;
        hasAnswerBeenAllocated = new List<bool>();
        answerIndexOrder = new List<int>();
        for (int i = 0; i < quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count; i++) {
            hasAnswerBeenAllocated.Add(false);
        }
        while (!HasAllocationFinished(hasAnswerBeenAllocated)) {
            int index = Random.Range(0, quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count);
            if (!hasAnswerBeenAllocated[index]) {
                answerIndexOrder.Add(index);
                hasAnswerBeenAllocated[index] = true;
            }
        }
    }

    private void SetupAnswerButton() {
        for (int i = 0; i < quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers.Count; i++) {
            answerButtons[i].GetComponentInChildren<Text>().text = quizzes[choosenQuiz]
                .Questions[questionIndexOrder[currentQuestion]].answers[answerIndexOrder[i]].answer;
            int count = i;
            answerButtons[i].onClick.SetListener(() => {
                selectedAnswer = answerIndexOrder[count];
                ConfirmAnswer();
            });
        }
    }

    private void ConfirmAnswer() {
        QuestionResults result = new QuestionResults();
        result.fk_results_id = results_id;
        result.fk_answer_id = quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers[selectedAnswer].answer_id;
        result.fk_question_id = quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].question_id;
        result.isCorrect = quizzes[choosenQuiz].Questions[questionIndexOrder[currentQuestion]].answers[selectedAnswer].isCorrect;
        results.Add(result);
        Database.UpdateResultsAfterQuestionAnswered(result);
        if (currentQuestion < quizzes[choosenQuiz].Questions.Count - 1) {
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

    private void EndQuiz() {
        // Update Database with result is_completed
        Database.UpdateResultsToIsCompleted(results_id); 

        // Display Result Panel
        startQuiz = false;
        quizQuestionPanel.gameObject.SetActive(false);
        quizResultsPanel.gameObject.SetActive(true);
        resultsHeadingText.text = "Results for " + quizzes[choosenQuiz].QuizName;

        // Calculate result as percentage
        int totalQuestions = quizzes[choosenQuiz].Questions.Count;
        int totalCorrect = Database.GetTotalCorrectFromResults(results_id);
        float percentage = 0;
        if (totalCorrect != 0 || totalQuestions != 0) {
            percentage = (float) (totalCorrect * 100) / totalQuestions;
        }
        resultsSubHeadingText.text = "Your Result is " + Math.Ceiling(percentage) + "%";

        // Show each question and define correct and wrong answers.
        UIUtils.BalancePrefabs(resultSlot.gameObject, totalQuestions, resultContent);
        for (int i = 0; i < totalQuestions; i++) {
            QuizResultSlot slot = resultContent.GetChild(i).GetComponent<QuizResultSlot>();
            slot.nameText.text = "Q" + (i + 1) + ". " + quizzes[choosenQuiz].Questions[i].question;
            slot.correctAnswerText.text = "Correct Answer: " + Database.GetCorrectAnswer(quizzes[choosenQuiz].Questions[i].question_id);
            if (Database.GetWasAnswerCorrect(results_id, quizzes[choosenQuiz].Questions[i].question_id)) {
                slot.selectButton.GetComponentInChildren<Text>().text = "CORRECT";
                slot.selectButton.GetComponent<Image>().color = Color.green;
            } else {
                slot.selectButton.GetComponentInChildren<Text>().text = "Incorrect";
                slot.selectButton.GetComponent<Image>().color = Color.yellow;
                slot.wrongAnswerText.text = "You Answered: " +
                                            Database.GetActualAnswer(Database.GetStudentsAnswerId(results_id,
                                                quizzes[choosenQuiz].Questions[i].question_id));
            }
        }
    }

    private bool HasAllocationFinished(List<bool> test) {
        for (int i = 0; i < test.Count; i++) {
            if (!test[i]) { return false; }
        }
        return true;
    }

    private string UpdateHeading() {
        return quizzes[choosenQuiz].QuizName + " - " + (currentQuestion + 1) + "/" + quizzes[choosenQuiz].Questions.Count;
    }

    private string UpdateSubHeading() {
        // todo Reference: https://answers.unity.com/questions/45676/making-a-timer-0000-minutes-and-seconds.html
        float minutes = Mathf.Floor(quizTimer / SECONDS_IN_MINUTE);
        float seconds = quizTimer % SECONDS_IN_MINUTE;
        string zero = (seconds < 10) ? "0" : "";
        return "Timer: " + minutes + ":" + zero + (int)seconds;
    }

    public void ExitQuizSelection() {
        quizSelectionPanel.gameObject.SetActive(false);
    }

    public void ExitResultsPanel() {
        quizResultsPanel.gameObject.SetActive(false);
        InitStart();
    }
}
