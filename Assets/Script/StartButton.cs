using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartButton : MonoBehaviour
{
    public Button start; 
    // Start is called before the first frame update
    void Start()
    {
        start.onClick.AddListener(WhenClick);
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void WhenClick()
    {
        SceneManager.LoadScene("game");
    }
}
