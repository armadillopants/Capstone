using System.Collections;
using UnityEngine;
 
class GridMove : MonoBehaviour {
    private float moveSpeed = 3f;
    private float gridSize = 0.3f;
    private bool allowDiagonals = true;
    private bool correctDiagonalSpeed = true;
    private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;
 
    public void Update(){
        if (!isMoving) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (!allowDiagonals) {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                    input.y = 0;
                } else {
                    input.x = 0;
                }
            }
 
            if (input != Vector2.zero) {
                StartCoroutine(move(transform));
            }
        }
    }
 
    public IEnumerator move(Transform trans){
        isMoving = true;
        startPosition = trans.position;
        t = 0;
 
		endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
			startPosition.y, startPosition.z + System.Math.Sign(input.y) * gridSize);
 
        if(allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0) {
            factor = 0.7071f;
        } else {
            factor = 1f;
        }
 
        while (t < 1f) {
            t += Time.deltaTime * (moveSpeed/gridSize) * factor;
            trans.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
 
        isMoving = false;
        yield return 0;
    }
}
