using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public LayerMask layer;
    public bool eatFood;
    public enum Direction {Front, Left, Right };

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(nameof(Snake))) {
            transform.parent.gameObject.GetComponent<Snake>().CollisionDetected();
        }
    }

    public bool CheckObstacle(Direction direction) {
        Vector3 dir = transform.forward;
        if (direction == Direction.Left) {
            dir = -transform.right;
        }
        else if (direction == Direction.Right) {
            dir = transform.right;
        }
        var ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1, layer)) {
            return true;
        }
        return false;
    }
}
