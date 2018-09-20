using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hud : MonoBehaviour {

    public Level level;
    public GameOver gameOver;

    public Text remainingText, remainingLabel;
    public Text targetText, targetLabel;
    public Text multiplerText;
    public Text scoreText;
    public Image[] stars;

    private int starIndex = 0;

    void Start() {
        for (int i = 0; i < stars.Length; i++) {
            if (i == starIndex) {
                stars[i].enabled = true;
            } else {
                stars[i].enabled = false;
            }
        }
    }

    public void SetScore(int score) {
        scoreText.text = score.ToString();
        int visableStar = 0;
        if (score >= level.score1Star && score < level.score2Star) {
            visableStar = 1;
        } else if (score >= level.score2Star && score < level.score3Star) {
            visableStar = 2;
        } else if (score >= level.score3Star) {
            visableStar = 3;
        }

        if (visableStar != starIndex) {
            for (int i = 0; i < stars.Length; i++) {
                if (i == visableStar) {
                    stars[i].enabled = true;
                } else {
                    stars[i].enabled = false;
                }
            }
        }
        starIndex = visableStar;
    }

    public void SetTargetText(int target) {
        targetText.text = target.ToString();
    }

    public void SetRemaining(int remaining) {
        remainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining) {
        remainingText.text = remaining;
    }

    public void SetMultiplier(int multi) {
        multiplerText.text = "x" + multi.ToString();
    }

    public void SetLevelType(Level.LevelType type) {
        if (type == Level.LevelType.MOVES) {
            remainingLabel.text = "moves\nremaining";
            targetLabel.text = "target\nscore";
        } else if (type == Level.LevelType.OBSTACLE) {
            remainingLabel.text = "moves\nremaining";
            targetLabel.text = "bricks\nremaining";
        } else if (type == Level.LevelType.TIMER) {
            remainingLabel.text = "time\nremaining";
            targetLabel.text = "target\nscore";
        }
    }

    public void OnGameWin(int score) {
        gameOver.ShowWin(score, starIndex);
        if (starIndex > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0)) {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, starIndex);
        }
        GetComponent<DB_AddScore>().InsertNewScore(level.CurrentScore, PlayerPrefs.GetInt("PlayerID"), SceneManager.GetActiveScene().buildIndex);
    }

    public void OnGameLose(int score) {
        gameOver.ShowLose();
    }
}
