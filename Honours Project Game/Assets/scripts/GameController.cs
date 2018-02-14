using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HonsProj;

//note not sure about this
public enum GameState { IN_PLAY, PAUSED, EVENT }//, COMBAT }

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE { get; private set; }

    public GameState Game_State { get; protected set; }

    // Use this for initialization
    void Start()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE GAME CONTROLLER!!!!");
            Destroy(gameObject);
        }

        Game_State = GameState.IN_PLAY;
    }

    public void InEvent()
    {
        Game_State = GameState.EVENT;
    }

    public void FinishedEvent()
    {
        Game_State = GameState.IN_PLAY;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
