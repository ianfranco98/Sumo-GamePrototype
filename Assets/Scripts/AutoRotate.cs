using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    float t, y;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        t += Time.deltaTime;
        y += Time.deltaTime * 10;
        float x = Mathf.Sin(t) * 3;

        Vector3 v = new Vector3(x, y, 0);
        transform.eulerAngles = v;
    }
}
