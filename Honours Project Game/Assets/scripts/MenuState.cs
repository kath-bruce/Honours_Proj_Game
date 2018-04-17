using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : MonoBehaviour
{
    //public static MenuState INSTANCE { get; private set; }

    public GameDifficulty ChosenDifficulty { get; set; }

    // Use this for initialization
    void Start()
    {
        //if (INSTANCE == null)
        //{
        //    INSTANCE = this;
        //}
        //else
        //{
        //    Debug.LogError("MORE THAN ONE MENU STATE!!!!");
        //    Destroy(gameObject);
        //}

        DontDestroyOnLoad(gameObject);
    }
    
}
