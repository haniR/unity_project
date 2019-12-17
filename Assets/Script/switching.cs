using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switching : MonoBehaviour
{
    public void ScreenLoader(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
