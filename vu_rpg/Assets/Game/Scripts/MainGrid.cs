using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGrid : MonoBehaviour {
    public enum PieceType {
        EMPTY,
        NORMAL,
        BRICK,
        ROW_SPECIAL,
        COLUMN_SPECIAL,
        RAINBOW,
        COUNT
    };

    private const float spacing = 1.1f;

    [System.Serializable]
    public struct PiecePrefab {
        public PieceType type;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct PiecePositions {
        public PieceType type;
        public int x;
        public int y;
    };

    public int xDimension, yDimension;
    public float fillTime;

    public Level level;

    public PiecePrefab[] piecePrefabs;

    public PiecePositions[] initialPositions;

    private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GamePiece[,] pieces;

    private bool inverse = false;

    private GamePiece pressedPiece, enteredPiece;

    private bool gameOver = false;

    private bool isFilling = false;
    public bool IsFilling {
        get { return isFilling; }
    }

    private bool justStarting = false;
    public bool JustStarting {
        get { return justStarting; }
    }

    public struct Stats {
        public int moved;
        public int three;
        public int four;
        public int five;
    }

    public Stats statistics;

    void Awake() {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++) {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)) {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        pieces = new GamePiece[xDimension, yDimension];

        for (int i = 0; i < initialPositions.Length; i++) {
            if (initialPositions[i].x >= 0 && initialPositions[i].x < xDimension && initialPositions[i].y >= 0 &&
                initialPositions[i].y < yDimension) {
                SpawnNewPiece(initialPositions[i].x, initialPositions[i].y, initialPositions[i].type);
            }
        }

        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                if (pieces[x, y] == null) {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }
        justStarting = true;
        StartCoroutine(Fill());
        statistics = new Stats();
    }

    public IEnumerator Fill() {
        bool needsRefill = true;
        isFilling = true;
        while (needsRefill) {
            yield return new WaitForSeconds(fillTime);
            while (FillStep()) {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches();
        }
        isFilling = false;
        justStarting = false;
        ShuffleGrid();
    }

    public bool FillStep() {
        bool movedPiece = false;
        for (int y = yDimension - 2; y >= 0; y--) {
            for (int loopX = 0; loopX < xDimension; loopX++) {
                int x = loopX;
                if (inverse) {
                    x = xDimension - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable()) {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY) {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else {
                        for (int diag = -1; diag <= 1; diag++) {
                            if (diag != 0) {
                                int diagX = x + diag;
                                if (inverse) {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDimension) {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY) {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--) {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable()) {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY) {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove) {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < xDimension; x++) {
            GamePiece pieceBelow = pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY) {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece =
                    Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;
                newPiece.name = "Piece(" + x + "," + 0 + ")";
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColourComponent
                    .SetColour((ColourPiece.ColourType) Random.Range(0, pieces[x, 0].ColourComponent.NumColours));
                movedPiece = true;
            }
        }
        return movedPiece;
    }

    public Vector3 GetWorldPosition(float _x, float _y) {
        _x = _x * spacing;
        _y = _y * spacing;
        return new Vector3(transform.position.x - xDimension / 2.0f + _x,
            transform.position.y + yDimension / 2.0f - _y, 0f);
    }

    public GamePiece SpawnNewPiece(int _x, int _y, PieceType _type) {
        GameObject newPiece = Instantiate(piecePrefabDict[_type], GetWorldPosition(_x, _y), Quaternion.identity);
        newPiece.transform.parent = transform;
        newPiece.name = "Piece(" + _x + "," + _y + ")";
        pieces[_x, _y] = newPiece.GetComponent<GamePiece>();
        pieces[_x, _y].Init(_x, _y, this, _type);
        return pieces[_x, _y];
    }

    public bool IsAdjacent(GamePiece _p1, GamePiece _p2) {
        return (_p1.X == _p2.X && (int) Mathf.Abs(_p1.Y - _p2.Y) == 1) ||
               (_p1.Y == _p2.Y && (int) Mathf.Abs(_p1.X - _p2.X) == 1);
    }

    public void SwapPieces(GamePiece _p1, GamePiece _p2) {
        if (gameOver) {
            return;
        }
        if (_p1.IsMovable() && _p2.IsMovable()) {
            pieces[_p1.X, _p1.Y] = _p2;
            pieces[_p2.X, _p2.Y] = _p1;
            if (GetMatch(_p1, _p2.X, _p2.Y) != null || GetMatch(_p2, _p1.X, _p1.Y) != null ||
                _p1.Type == PieceType.RAINBOW || _p2.Type == PieceType.RAINBOW) {
                int p1X = _p1.X;
                int p1Y = _p1.Y;
                _p1.MovableComponent.Move(_p2.X, _p2.Y, fillTime);
                _p2.MovableComponent.Move(p1X, p1Y, fillTime);

                if (_p1.Type == PieceType.RAINBOW && _p1.IsClearable() && _p2.IsColoured()) {
                    ClearColourPiece clearColour = _p1.GetComponent<ClearColourPiece>();
                    if (clearColour) {
                        clearColour.Colour = _p2.ColourComponent.Colour;
                    }
                    ClearPiece(_p1.X, _p1.Y);
                }
                if (_p2.Type == PieceType.RAINBOW && _p2.IsClearable() && _p1.IsColoured()) {
                    ClearColourPiece clearColour = _p2.GetComponent<ClearColourPiece>();
                    if (clearColour) {
                        clearColour.Colour = _p1.ColourComponent.Colour;
                    }
                    ClearPiece(_p2.X, _p2.Y);
                }

                ClearAllValidMatches();

                if (_p1.Type == PieceType.ROW_SPECIAL || _p1.Type == PieceType.COLUMN_SPECIAL) {
                    ClearPiece(_p1.X, _p1.Y);
                }
                if (_p2.Type == PieceType.ROW_SPECIAL || _p2.Type == PieceType.COLUMN_SPECIAL) {
                    ClearPiece(_p2.X, _p2.Y);
                }

                pressedPiece = null;
                enteredPiece = null;

                StartCoroutine(Fill());

                level.OnMove();
            }
            else {
                pieces[_p1.X, _p1.Y] = _p1;
                pieces[_p2.X, _p2.Y] = _p2;
            }
        }
    }

    public void PressPiece(GamePiece _piece) {
        pressedPiece = _piece;
    }

    public void EnterPiece(GamePiece _piece) {
        enteredPiece = _piece;
        if (pressedPiece) {
            if (IsAdjacent(pressedPiece, enteredPiece)) {
                SwapPieces(pressedPiece, enteredPiece);
            }
        }
    }

    public void ReleasePress() {
        pressedPiece = null;
        enteredPiece = null;
    }

    public List<GamePiece> GetMatch(GamePiece _piece, int _newX, int _newY) {
        if (_piece.IsColoured()) {
            ColourPiece.ColourType colour           = _piece.ColourComponent.Colour;
            List<GamePiece>        horizontalPieces = new List<GamePiece>();
            List<GamePiece>        verticalPieces   = new List<GamePiece>();
            List<GamePiece>        matchingPieces   = new List<GamePiece>();

            horizontalPieces.Add(_piece);
            for (int direction = 0; direction <= 1; direction++) {
                for (int xOffset = 1; xOffset < xDimension; xOffset++) {
                    int x;
                    if (direction == 0) {
                        x = _newX - xOffset;
                    } else {
                        x = _newX + xOffset;
                    }
                    if (x < 0 || x >= xDimension) {
                        break;
                    }
                    if (pieces[x, _newY].IsColoured() && pieces[x, _newY].ColourComponent.Colour == colour) {
                        horizontalPieces.Add(pieces[x, _newY]);
                    }
                    else {
                        break;
                    }
                }
            }
            if (horizontalPieces.Count >= 3) {
                for (int i = 0; i < horizontalPieces.Count; i++) {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            if (horizontalPieces.Count >= 3) {
                for (int i = 0; i < horizontalPieces.Count; i++) {
                    for (int direction = 0; direction <= 1; direction++) {
                        for (int yOffset = 1; yOffset < yDimension; yOffset++) {
                            int y;
                            if (direction == 0) {
                                y = _newY - yOffset;
                            } else {
                                y = _newY + yOffset;
                            }
                            if (y < 0 || y >= yDimension) {
                                break;
                            }
                            if (pieces[horizontalPieces[i].X, y].IsColoured() &&
                                pieces[horizontalPieces[i].X, y].ColourComponent.Colour == colour) {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            } else {
                                break;
                            }
                        }
                    }
                    if (verticalPieces.Count < 2) {
                        verticalPieces.Clear();
                    } else {
                        for (int j = 0; j < verticalPieces.Count; j++) {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3) {
                if (!JustStarting) {
                    if (matchingPieces.Count == 3) {
                        statistics.three++;
                    }
                    else if (matchingPieces.Count == 4) {
                        statistics.four++;
                    }
                    else {
                        statistics.five++;
                    }
                }
                return matchingPieces;
            }

            horizontalPieces.Clear();
            verticalPieces.Clear();

            verticalPieces.Add(_piece);
            for (int direction = 0; direction <= 1; direction++) {
                for (int yOffset = 1; yOffset < yDimension; yOffset++) {
                    int y;
                    if (direction == 0) {
                        y = _newY - yOffset;
                    } else {
                        y = _newY + yOffset;
                    }
                    if (y < 0 || y >= yDimension) {
                        break;
                    }
                    if (pieces[_newX, y].IsColoured() && pieces[_newX, y].ColourComponent.Colour == colour) {
                        verticalPieces.Add(pieces[_newX, y]);
                    } else {
                        break;
                    }
                }
            }
            if (verticalPieces.Count >= 3) {
                for (int i = 0; i < verticalPieces.Count; i++) {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }
            if (verticalPieces.Count >= 3) {
                for (int i = 0; i < verticalPieces.Count; i++) {
                    for (int direction = 0; direction <= 1; direction++) {
                        for (int xOffset = 1; xOffset < xDimension; xOffset++) {
                            int x;
                            if (direction == 0) {
                                x = _newX - xOffset;
                            } else {
                                x = _newX + xOffset;
                            }
                            if (x < 0 || x >= xDimension) {
                                break;
                            }
                            if (pieces[x, verticalPieces[i].Y].IsColoured() &&
                                pieces[x, verticalPieces[i].Y].ColourComponent.Colour == colour) {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            } else {
                                break;
                            }
                        }
                    }
                    if (horizontalPieces.Count < 2) {
                        horizontalPieces.Clear();
                    }
                    else {
                        for (int j = 0; j < horizontalPieces.Count; j++) {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3) {
                if (!JustStarting) {
                    if (matchingPieces.Count == 3) {
                        statistics.three++;
                    } else if (matchingPieces.Count == 4) {
                        statistics.four++;
                    } else {
                        statistics.five++;
                    }
                }
                return matchingPieces;
            }
        }
        return null;
    }

    public bool ClearAllValidMatches() {
        bool needsRefill = false;
        for (int y = 0; y < yDimension; y++) {
            for (int x = 0; x < xDimension; x++) {
                if (pieces[x, y].IsClearable()) {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null) {
                        PieceType specialPiece  = PieceType.COUNT;
                        GamePiece randomPiece   = match[Random.Range(0, match.Count)];
                        int       specialPieceX = randomPiece.X;
                        int       specialPieceY = randomPiece.Y;
                        if (match.Count == 4) {
                            if (pressedPiece == null || enteredPiece == null) {
                                specialPiece = (PieceType) Random.Range((int) PieceType.ROW_SPECIAL,
                                    (int) PieceType.COLUMN_SPECIAL);
                            } else if (pressedPiece.Y == enteredPiece.Y) {
                                specialPiece = PieceType.ROW_SPECIAL;
                            } else {
                                specialPiece = PieceType.COLUMN_SPECIAL;
                            }
                        } else if (match.Count >= 5) {
                            specialPiece = PieceType.RAINBOW;
                        }
                        for (int i = 0; i < match.Count; i++) {
                            if (ClearPiece(match[i].X, match[i].Y)) {
                                needsRefill = true;
                                if (match[i] == pressedPiece || match[i] == enteredPiece) {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }
                        if (specialPiece != PieceType.COUNT) {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPiece);
                            if (specialPiece == PieceType.ROW_SPECIAL || specialPiece == PieceType.COLUMN_SPECIAL &&
                                newPiece.IsColoured() && match[0].IsColoured()) {
                                newPiece.ColourComponent.SetColour(match[0].ColourComponent.Colour);
                            } else if (specialPiece == PieceType.RAINBOW && newPiece.IsColoured()) {
                                newPiece.ColourComponent.SetColour(ColourPiece.ColourType.ANY);
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }

    public bool ClearPiece(int x, int y) {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared) {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }
        return false;
    }

    public void ClearObstacles(int x, int y) {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++) {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDimension) {
                if (pieces[adjacentX, y].Type == PieceType.BRICK && pieces[adjacentX, y].IsClearable()) {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++) {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDimension) {
                if (pieces[x, adjacentY].Type == PieceType.BRICK && pieces[x, adjacentY].IsClearable()) {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int y) {
        for (int x = 0; x < xDimension; x++) {
            ClearPiece(x, y);
        }
    }

    public void ClearColumn(int x) {
        for (int y = 0; y < yDimension; y++) {
            ClearPiece(x, y);
        }
    }

    public void ClearColour(ColourPiece.ColourType colour) {
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                if (pieces[x, y].IsColoured() && pieces[x, y].ColourComponent.Colour == colour ||
                    colour == ColourPiece.ColourType.ANY) {
                    ClearPiece(x, y);
                }
            }
        }
    }

    public void GameOver() {
        gameOver = true;
    }

    public List<GamePiece> GetPiecesOfType(PieceType type) {
        List<GamePiece> piecesOfType = new List<GamePiece>();
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                if (pieces[x, y].Type == type) {
                    piecesOfType.Add(pieces[x, y]);
                }
            }
        }
        return piecesOfType;
    }

    public bool IsValidMatchPossible() {
        bool validMatch = false;
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                if (x >= 0 && x < xDimension && y >= 0 && y < yDimension) {
                    for (int i = -1; i < 2; i++) {
                        for (int j = -1; j < 2; j++) {
                            if (i == 0 || j == 0 && i != j) {
                                if (x + i >= 0 && x + i < xDimension && y + j >= 0 && y + j < yDimension) {
                                    if (CheckNewPosition(pieces[x, y], pieces[x + i, y + j])) {
                                        validMatch = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (validMatch) {
                    return validMatch;
                }
            }
        }
       
        return validMatch;
    }

    private bool CheckNewPosition(GamePiece g1, GamePiece g2) {
        if (g1.IsMovable() && g2.IsMovable()) {
            pieces[g1.X, g1.Y] = g2;
            pieces[g2.X, g2.Y] = g1;
            if (GetMatch(g1, g2.X, g2.Y) != null || GetMatch(g2, g1.X, g1.Y) != null ||
                g1.Type == PieceType.RAINBOW || g2.Type == PieceType.RAINBOW) {
                pieces[g1.X, g1.Y] = g1;
                pieces[g2.X, g2.Y] = g2;
                return true;
            } 
            pieces[g1.X, g1.Y] = g1;
            pieces[g2.X, g2.Y] = g2;
        }
        return false;
    }

    private void ShuffleGrid() {
        while (!IsValidMatchPossible()) {
            // TODO: Add Shuffling animation
            for (int x = 0; x < xDimension; x++) {
                for (int y = 0; y < yDimension; y++) {
                    if (pieces[x, y].Type == PieceType.NORMAL) {
                        ClearPiece(x, y);
                        SpawnNewPiece(x, y, PieceType.NORMAL);
                        pieces[x, y].ColourComponent.SetColour((ColourPiece.ColourType)Random.Range(0, pieces[x, y].ColourComponent.NumColours));
                    }
                }
            }
            ClearAllValidMatches();
            StartCoroutine(Fill());
        }
        
        
    }
}
