using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : Level {

    public int timeInSeconds;
    public int targetScore;

    private float timerT;
    private bool gameFinished = false;

    void Start() {
        type = LevelType.TIMER;

        canvas.SetLevelType(type);
        canvas.SetScore(currentScore);
        canvas.SetTargetText(targetScore);
        canvas.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds / 60, timeInSeconds% 60));
    }

    void Update() {
        if (!gameFinished && !grid.JustStarting) {
            timerT += Time.deltaTime;
            canvas.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((timeInSeconds - timerT) / 60, 0), (int)Mathf.Max((timeInSeconds - timerT) % 60, 0)));
            if (timeInSeconds - timerT <= 0) {
                if (currentScore >= targetScore) {
                    GameWin();
                }
                else {
                    GameLose();
                }
                gameFinished = true;
            }
        }
    }

}
