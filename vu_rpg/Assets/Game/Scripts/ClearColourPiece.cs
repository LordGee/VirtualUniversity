using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColourPiece : ClearablePiece {

    private ColourPiece.ColourType colour;

    public ColourPiece.ColourType Colour {
        get { return colour; }
        set { colour = value; }
    }
    public override void Clear() {
        base.Clear();
        piece.Grid.ClearColour(colour);
    }
}
