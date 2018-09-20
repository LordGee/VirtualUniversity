using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public enum LevelType {
        TIMER,
        OBSTACLE,
        MOVES
    };

    public MainGrid grid;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    public hud canvas;

    public AudioClip[] soundclips;

    protected LevelType type;

    public LevelType Type {
        get { return type; }
    }

    private AudioSource audio;

    protected int currentScore;
    protected int multiplier = 1;

    private float lastTime, timer;

    protected bool didWin;

    public int CurrentScore {
        get { return currentScore; }
    }

    void Awake() {
        audio = GetComponent<AudioSource>();
    }

    void Start() {
        canvas.SetScore(currentScore);
        canvas.SetMultiplier(multiplier);
    }

    public virtual void GameWin() {
        grid.GameOver();
        didWin = true;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose() {
        grid.GameOver();
        didWin = false;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void OnMove() {
        timer = Time.timeSinceLevelLoad;
        if (timer - lastTime < 3f) {
            multiplier++;
            if (multiplier % 10 == 1) {
                audio.clip = soundclips[Random.Range(0, soundclips.Length)];
                audio.Play();
            }
        } else {
            multiplier = 1;
        }
        canvas.SetMultiplier(multiplier);
        grid.statistics.moved++;
        lastTime = Time.timeSinceLevelLoad;
    }

    public virtual void OnPieceCleared(GamePiece piece) {
        if (!grid.JustStarting) {
            currentScore += piece.scoreValue * multiplier;
            canvas.SetScore(currentScore);
        }
    }

    protected virtual IEnumerator WaitForGridFill() {
        while (grid.IsFilling) {
            yield return 0;
        }
        if (didWin) {
            canvas.OnGameWin(currentScore);
            // GetComponent<DB_AddStats>().InsertNewStats(grid.statistics.moved, grid.statistics.three, grid.statistics.four, grid.statistics.five, currentScore);
        }
        else {
            canvas.OnGameLose(currentScore);
        }
    }
}
