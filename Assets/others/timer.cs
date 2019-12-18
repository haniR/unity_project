using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : MonoBehaviour
{
    // Start is called before the first frame update
    float starttime;
    // Start is called before the first frame update
    void Start()
    {
        starttime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((Time.time - starttime).ToString("00:00.00"));

    }
}
