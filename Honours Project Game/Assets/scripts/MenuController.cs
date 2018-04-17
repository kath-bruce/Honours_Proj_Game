using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayEasy()
    {
        GetComponent<AudioSource>().Play();
        FindObjectOfType<MenuState>().ChosenDifficulty = GameDifficulty.EASY;
        SceneManager.LoadScene("_spaceship");
    }

    public void PlayMedium()
    {
        GetComponent<AudioSource>().Play();
        FindObjectOfType<MenuState>().ChosenDifficulty = GameDifficulty.MEDIUM;
        SceneManager.LoadScene("_spaceship");
    }

    public void PlayHard()
    {
        GetComponent<AudioSource>().Play();
        FindObjectOfType<MenuState>().ChosenDifficulty = GameDifficulty.HARD;
        SceneManager.LoadScene("_spaceship");
    }

    public void Tutorial()
    {
        GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("_tutorial");
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
