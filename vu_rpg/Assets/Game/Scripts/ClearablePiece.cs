using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    public AnimationClip clearAnimation;
    public GameObject explosion;

    private bool isBeingCleared = false;

    public bool IsBeingCleared {
        get { return isBeingCleared; }
    }

    protected GamePiece piece;

    void Awake() {
        piece = GetComponent<GamePiece>();
    }

    public virtual void Clear() {
        piece.Grid.level.OnPieceCleared(piece);
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine() {
        GameObject explode = Instantiate(explosion,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -0.1f), Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play(true);
        Debug.Log(explosion.transform.position);
        Animator animator = GetComponent<Animator>();
        if (animator) {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(explode);
            Destroy(gameObject);
        }
    }
}
