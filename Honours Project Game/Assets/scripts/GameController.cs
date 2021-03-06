﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HonsProj;
using UnityEngine.SceneManagement;

public enum GameState { IN_PLAY, PAUSED, EVENT, WON, LOST_HULL, LOST_LIFE_SUPPORT, LOST_STRESSED }
public enum GamePhase { FIRST_PHASE, MIDDLE_PHASE, FINAL_PHASE }
public enum GameDifficulty { EASY, MEDIUM, HARD }

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE { get; private set; }

    public delegate void RestartGame();
    public event RestartGame OnRestartGame;

    public GameState Current_Game_State { get; protected set; }
    private GameState prev_game_state;

    private GamePhase current_game_phase;
    public GamePhase Current_Game_Phase
    {
        get
        {
            return current_game_phase;
        }

        protected set
        {
            current_game_phase = value;
            UIManager.INSTANCE.UpdatePhaseDisplay(current_game_phase);
        }
    }

    private GameDifficulty current_game_difficulty;
    public GameDifficulty Current_Game_Difficulty
    {
        get
        {
            return current_game_difficulty;
        }

        protected set
        {
            current_game_difficulty = value;
            UIManager.INSTANCE.UpdateDifficultyDisplay(current_game_difficulty);
        }
    }

    private float distance_to_earth;
    public float Distance_To_Earth
    {
        get
        {
            return distance_to_earth;
        }

        protected set
        {
            distance_to_earth = value;
            UIManager.INSTANCE.UpdateDistanceToEarth(value);
        }
    }

    private bool finished_event = false;

    private const float EASY_DISTANCE = 2500.0f;
    private const float MED_DISTANCE = 3000.0f;
    private const float HARD_DISTANCE = 3500.0f;

    private float initial_distance;

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
        switch(FindObjectOfType<MenuState>().ChosenDifficulty)
        {
            case GameDifficulty.EASY:
                RestartInEasyDifficulty();
                break;
            case GameDifficulty.MEDIUM:
                RestartInMediumDifficulty();
                break;
            case GameDifficulty.HARD:
                RestartInHardDifficulty();
                break;
            default:
                break;
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("_menu");
    }

    public void ChangeDistanceToEarth(float deltaDistance)
    {
        Distance_To_Earth += deltaDistance;

        if (Distance_To_Earth <= initial_distance * (1.0f / 3.0f))
        {
            Current_Game_Phase = GamePhase.FINAL_PHASE;
        }
        else if (Distance_To_Earth <= initial_distance * (2.0f / 3.0f))
        {
            Current_Game_Phase = GamePhase.MIDDLE_PHASE;
        }

        if (Distance_To_Earth <= 0)
        {
            Distance_To_Earth = 0.0f;
            Current_Game_State = GameState.WON;
            UIManager.INSTANCE.ShowWinDisplay();
            AudioController.INSTANCE.PlayWinAudio();
        }
    }

    public void LostHullIntegrity()
    {
        Current_Game_State = GameState.LOST_HULL;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void LostLifeSupport()
    {
        Current_Game_State = GameState.LOST_LIFE_SUPPORT;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void LostSanity()
    {
        Current_Game_State = GameState.LOST_STRESSED;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void InEvent()
    {
        Current_Game_State = GameState.EVENT;
    }

    public void FinishedEvent()
    {
        finished_event = true;
    }

    public void Pause()
    {
        prev_game_state = Current_Game_State;
        Current_Game_State = GameState.PAUSED;
        UIManager.INSTANCE.UpdatePauseDisplay(true);
    }

    public void Unpause()
    {
        Current_Game_State = prev_game_state;
        UIManager.INSTANCE.UpdatePauseDisplay(false);
    }

    public void RestartInEasyDifficulty()
    {
        Current_Game_Difficulty = GameDifficulty.EASY;

        if (OnRestartGame != null)
            OnRestartGame();

        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = EASY_DISTANCE;
        initial_distance = Distance_To_Earth;
        
    }

    public void RestartInMediumDifficulty()
    {
        Current_Game_Difficulty = GameDifficulty.MEDIUM;

        if (OnRestartGame != null)
            OnRestartGame();

        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = MED_DISTANCE;
        initial_distance = Distance_To_Earth;
        
    }

    public void RestartInHardDifficulty()
    {
        Current_Game_Difficulty = GameDifficulty.HARD;

        if (OnRestartGame != null)
            OnRestartGame();

        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = HARD_DISTANCE;
        initial_distance = Distance_To_Earth;
        
    }

    public void RestartInCurrentDifficulty()
    {
        OnRestartGame();
        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = initial_distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (finished_event && Current_Game_State != GameState.LOST_HULL && Current_Game_State 
            != GameState.LOST_LIFE_SUPPORT && Current_Game_State != GameState.LOST_STRESSED)
        {
            Current_Game_State = GameState.IN_PLAY;
            finished_event = false;
        }

        if (Current_Game_State == GameState.IN_PLAY)
        {
            ChangeDistanceToEarth(-Time.deltaTime * ShipController.INSTANCE.Ship_Speed);
        }
    }
}
