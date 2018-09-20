using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level {

    public int numMoves;
    public int targetScore;

    private int movesUsed = 0;

    void Start() {
        type = LevelType.MOVES;
        canvas.SetLevelType(type);
        canvas.SetScore(currentScore);
        canvas.SetTargetText(targetScore);
        canvas.SetRemaining(numMoves);
    }
    public override void OnMove() {
        base.OnMove();
        movesUsed++;
        canvas.SetRemaining(numMoves - movesUsed);
        if (numMoves - movesUsed == 0) {
            if (currentScore >= targetScore) {
                GameWin();
            } else {
                GameLose();
            }
        }
    }
}
