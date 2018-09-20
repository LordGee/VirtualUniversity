﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    public GameObject screenParent;
    public GameObject scoreParent;
    public Text loseText;
    public Text scoreText;
    public Image[] stars;
    public GameObject gameO;

    void Start() {
        screenParent.SetActive(false);
        for (int i = 0; i < stars.Length; i++) {
            stars[i].enabled = false;
        }
    }

    public void ShowLose() {
        screenParent.SetActive(true);
        scoreParent.SetActive(false);
        Animator anim = GetComponent<Animator>();
        if (anim) {
            anim.Play("ShowGameOver");
        }
    }

    public void ShowWin(int score, int starCount) {
        screenParent.SetActive(true);
        loseText.enabled = false;
        scoreText.text = score.ToString();
        scoreText.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim) {
            anim.Play("ShowGameOver");
        }

        StartCoroutine(ShowWinStarsCoroutine(starCount));
    }

    private IEnumerator ShowWinStarsCoroutine(int starCount) {
        yield return new WaitForSeconds(0.5f);

        if (starCount < stars.Length) {
            for (int i = 0; i <= starCount; i++) {
                stars[i].enabled = true;
                if (i > 0) {
                    stars[i - 1].enabled = false;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        scoreText.enabled = true;
    }

    public void OnReplayClicked() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnDoneClicked() {
        Destroy(gameO);
    }
}