
public class LevelObstacles : Level {

    public int numMoves;
    public MainGrid.PieceType[] obstacleTypes;
    public int scoreForRemainingMoves;

    private int movesUsed = 0;
    private int numObstaclesLeft;

	void Start () {
	    type = LevelType.OBSTACLE;
	    
        for (int i = 0; i < obstacleTypes.Length; i++) {
	        numObstaclesLeft += grid.GetPiecesOfType(obstacleTypes[i]).Count;
	    }

	    canvas.SetLevelType(type);
	    canvas.SetScore(currentScore);
	    canvas.SetTargetText(numObstaclesLeft);
	    canvas.SetRemaining(numMoves);
    }
    public override void OnMove() {
        base.OnMove();
        movesUsed++;
        canvas.SetRemaining(numMoves - movesUsed);
        if (numMoves - movesUsed == 0 && numObstaclesLeft > 0) {
            GameLose();
        }
    }
    public override void OnPieceCleared(GamePiece piece) {
        base.OnPieceCleared(piece);
        for (int i = 0; i < obstacleTypes.Length; i++) {
            if (obstacleTypes[i] == piece.Type) {
                numObstaclesLeft--;
                canvas.SetTargetText(numObstaclesLeft);
                if (numObstaclesLeft == 0) {
                    currentScore += scoreForRemainingMoves * (numMoves - movesUsed);
                    canvas.SetScore(currentScore);
                    GameWin();
                }
            }
        }
    }
}
