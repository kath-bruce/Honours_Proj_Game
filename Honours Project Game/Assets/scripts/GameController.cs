using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HonsProj;

//note not sure about this
public enum GameState { IN_PLAY, PAUSED, EVENT, WON, LOST_HULL, LOST_LIFE_SUPPORT, LOST_STRESSED }//, COMBAT }

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE { get; private set; }

    public delegate void RestartGame();
    public event RestartGame OnRestartGame;

    public GameState Current_Game_State { get; protected set; }
    
    private float distance_to_earth;
    public float Distance_To_Earth
    {
        get
        {
            return distance_to_earth;
        }

        set
        {
            distance_to_earth = value;
            UIManager.INSTANCE.UpdateDistanceToEarth(value);
        }
    }

    private bool finished_event = false;

    // Use this for initialization
    void Awake()
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
    }

    void Start()
    { 
        Current_Game_State = GameState.IN_PLAY;
        Distance_To_Earth = 2000.0f; //light years - cause she's 2000 light years away
    }

    public void LostHullIntegrity()
    {
        Current_Game_State = GameState.LOST_HULL;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
    }

    public void LostLifeSupport()
    {
        Current_Game_State = GameState.LOST_LIFE_SUPPORT;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
    }

    public void LostSanity()
    {
        Current_Game_State = GameState.LOST_STRESSED;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
    }

    public void InEvent()
    {
        Current_Game_State = GameState.EVENT;
    }

    public void FinishedEvent()
    {
        finished_event = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //OnRestartGame();
        }

        if (finished_event)
        {
            Current_Game_State = GameState.IN_PLAY;
            finished_event = false;
        }

        if (Current_Game_State == GameState.IN_PLAY)
        {
            Distance_To_Earth -= Time.deltaTime * ShipController.INSTANCE.Ship_Speed;

            if (Distance_To_Earth <= 0f)
            {
                Current_Game_State = GameState.WON;
                UIManager.INSTANCE.ShowWinDisplay();
            }
        }
    }
}
