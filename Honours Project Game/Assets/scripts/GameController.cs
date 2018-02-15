using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HonsProj;

//note not sure about this
public enum GameState { IN_PLAY, PAUSED, EVENT, WON, LOST_HULL, LOST_LIFE_SUPPORT, LOST_STRESSED }//, COMBAT }

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE { get; private set; }

    public GameState Game_State { get; protected set; }
    
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
        Game_State = GameState.IN_PLAY;
        Distance_To_Earth = 5000.0f; //light years
    }

    public void LostHullIntegrity()
    {
        Game_State = GameState.LOST_HULL;
    }

    public void LostLifeSupport()
    {
        Game_State = GameState.LOST_LIFE_SUPPORT;
    }

    public void LostSanity()
    {
        Game_State = GameState.LOST_STRESSED;
    }

    public void InEvent()
    {
        Game_State = GameState.EVENT;
    }

    public void FinishedEvent()
    {
        finished_event = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (finished_event)
        {
            Game_State = GameState.IN_PLAY;
            finished_event = false;
        }
        
        Distance_To_Earth -= Time.deltaTime * ShipController.INSTANCE.Ship_Speed;

        if (Distance_To_Earth <= 0f)
        {
            Game_State = GameState.WON;
        }
    }
}
