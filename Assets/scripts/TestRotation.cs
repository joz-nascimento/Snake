using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        float angle = Quaternion.Angle(transform.rotation, go.transform.rotation);
        Debug.Log("angle: " + angle);
        angle = Vector3.SignedAngle(transform.eulerAngles, go.transform.eulerAngles, Vector3.forward);
        Debug.Log("angle: " + angle);
        angle = transform.eulerAngles.y;
        float angle2 = go.transform.eulerAngles.y;
        Debug.Log("angle: " + angle);
        Debug.Log("angle2: " + angle2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
