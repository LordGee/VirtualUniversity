using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{

    private GamePiece piece;
    private IEnumerator moveCoroutine;

    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void Move(int _newX, int _newY, float _time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(_newX, _newY, _time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int _newX, int _newY, float _time)
    {
        piece.X = _newX;
        piece.Y = _newY;
        piece.name = "Piece(" + _newX + "," + _newY + ")";

        Vector3 startPosition = transform.position;
        Vector3 endPosition = piece.Grid.GetWorldPosition(_newX, _newY);
        for (float t = 0; t <= 1 * _time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPosition, endPosition, t / _time);
            yield return 0;
        }

        piece.transform.position = endPosition;
    }
}
