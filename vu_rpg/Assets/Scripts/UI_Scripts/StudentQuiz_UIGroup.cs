﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class StudentQuiz_UIGroup : MonoBehaviour {

    [Header("Panels")]
    public GameObject quizSelectionPanel;
    public GameObject quizQuestionPanel;

    [Header("Quiz Selection")]
    public Transform quizContent;
    public QuizSelectionSlot slotPrefab;
    public Text questionHeading;

    [Header("Quiz Questions")]
    public Text questionSubHeading;
    public Text questionText;
    public Button[] answerButtons;

    private List<Quiz> quizzes;
    private Player player;

    private int choosenQuiz = -1;
    private bool selectionQuiz = false;
    private int results_id;

    private bool startQuiz = false;
    private float quizTimer, currentTime, lastUpdate;
    private const float QUIZ_TIMER = 300.0f;
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
        quizTimer = QUIZ_TIMER;
        lastUpdate = QUIZ_TIMER;
        currentTime = 0.0f;
    }

    void Update() {
        if (selectionQuiz) {
            PopulateQuizzes();
            if (choosenQuiz != -1) {
                selectionQuiz = false;

            }
        } else if (startQuiz) {
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
        Debug.Log("Ending quiz");
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
        return "Timer: " + (int)quizTimer;
    }
}
