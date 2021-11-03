using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Coroutine a = StartCoroutine(MyCoroutine());
        //StopAllCoroutines();
        StopCoroutine(a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MyCoroutine() {
        Debug.Log("start coroutine");
        yield return new WaitForSeconds(2);
        Debug.Log("coroutine after 2 seconds");
    }
}
