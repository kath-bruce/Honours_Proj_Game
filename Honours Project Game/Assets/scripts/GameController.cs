using System.Collections;
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

    private int no_of_tries = 0;
    public int No_Of_Tries
    {
        get
        {
            return no_of_tries;
        }

        protected set
        {
            no_of_tries = value;
        }
    }

    public const int MAX_NO_OF_TRIES = 3;

    private bool has_finished_tutorial;

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

        Current_Game_Phase = GamePhase.FIRST_PHASE;
        Current_Game_Difficulty = GameDifficulty.EASY;

        Distance_To_Earth = EASY_DISTANCE;
        initial_distance = Distance_To_Earth;

        //TODO!!!! - remember to uncomment this
        Pause();
        UIManager.INSTANCE.ViewTutorial();
        has_finished_tutorial = false;
        //has_finished_tutorial = true;
    }

    public void FinishedTutorial()
    {
        has_finished_tutorial = true;
    }

    public bool HasFinishedTutorial()
    {
        return has_finished_tutorial;
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
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State, ++No_Of_Tries);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void LostLifeSupport()
    {
        Current_Game_State = GameState.LOST_LIFE_SUPPORT;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State, ++No_Of_Tries);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void LostSanity()
    {
        Current_Game_State = GameState.LOST_STRESSED;
        UIManager.INSTANCE.ShowLossDisplay(Current_Game_State, ++No_Of_Tries);
        AudioController.INSTANCE.PlayLoseAudio();
    }

    public void InEvent()
    {
        Current_Game_State = GameState.EVENT;
    }

    public void FinishedEvent()
    {
        finished_event = true;
        //Current_Game_State = GameState.IN_PLAY;
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

        OnRestartGame();
        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = EASY_DISTANCE;
        initial_distance = Distance_To_Earth;

        No_Of_Tries = 0;
    }

    public void RestartInMediumDifficulty()
    {
        Current_Game_Difficulty = GameDifficulty.MEDIUM;

        OnRestartGame();
        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = MED_DISTANCE;
        initial_distance = Distance_To_Earth;

        No_Of_Tries = 0;
    }

    public void RestartInHardDifficulty()
    {
        Current_Game_Difficulty = GameDifficulty.HARD;

        OnRestartGame();
        Current_Game_State = GameState.IN_PLAY;
        Current_Game_Phase = GamePhase.FIRST_PHASE;

        Distance_To_Earth = HARD_DISTANCE;
        initial_distance = Distance_To_Earth;

        No_Of_Tries = 0;
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
        if (finished_event) //note is this necessary?? - yes apparently
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
