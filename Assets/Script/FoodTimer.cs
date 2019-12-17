using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodTimer : MonoBehaviour
{
    public Text timeText;
    float time ;
    GameObject  healthy; 
    GameObject [] unhealthy; 
    // Start is called before the first frame update
    void Start()
    {
        time = 15;
        setText();
        healthy = GameObject.FindGameObjectWithTag("healthy food");
        unhealthy = GameObject.FindGameObjectsWithTag("unhealthy food");
    }

    // Update is called once per frame
    void Update()
    {
        if(time >= 0)
        time = time - 0.01f;
        setText();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == healthy)
        {
            
            time = time + 5f;
            Destroy(healthy);
        }if(collision.gameObject.CompareTag("unhealthy Food") && time!= 0)
        {
            time = time - 5f; 
        }

    }
    void setText()
    {
        timeText.text = "Time : " + time; 
    }

}
