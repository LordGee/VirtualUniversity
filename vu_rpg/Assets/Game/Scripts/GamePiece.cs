using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

    public int scoreValue;

    private int x, y;
    private MainGrid.PieceType type;
    private MainGrid grid;
    private MovablePiece movableComponent;
    private ColourPiece colourComponent;
    private ClearablePiece clearableComponent;

    public int X {
        get { return x; }
        set { if (IsMovable()) { x = value; } }
    }

    public int Y {
        get { return y; }
        set { if (IsMovable()) { y = value; } }
    }

    public MainGrid.PieceType Type {
        get { return type; }
    }

    public MainGrid Grid {
        get { return grid; }
    }

    public MovablePiece MovableComponent {
        get { return movableComponent; }
    }

    public ColourPiece ColourComponent {
        get { return colourComponent; }
    }

    public ClearablePiece ClearableComponent {
        get { return clearableComponent; }
    }

    void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colourComponent = GetComponent<ColourPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }

    public void Init(int _x, int _y, MainGrid _grid, MainGrid.PieceType _type) {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    void OnMouseEnter() {
        grid.EnterPiece(this);
    }

    void OnMouseDown() {
        if (!grid.JustStarting) {
            grid.PressPiece(this);
        }
    }

    void OnMouseUp() {
        grid.ReleasePress();
    }

    public bool IsMovable() {
        return movableComponent != null;
    }

    public bool IsColoured() {
        return colourComponent != null;
    }

    public bool IsClearable() {
        return clearableComponent != null;
    }
}
