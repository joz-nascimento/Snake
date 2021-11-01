using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckObstacle();
        Debug.DrawRay(transform.position, transform.forward, Color.green);

    }

    public bool CheckObstacle() {
        Vector3 dir = transform.forward;
        var ray = new Ray(transform.position, dir);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 1, -1)) {
            Debug.Log("hit");
            return true;
        }
        return false;
    }
}
