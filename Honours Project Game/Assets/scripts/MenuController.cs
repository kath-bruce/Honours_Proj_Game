using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Play()
    {
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("_spaceship");
    }

    public void Exit()
    {
        GetComponent<AudioSource>().Play();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
