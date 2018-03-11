using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if left click
        //if in event
        //return
        //else - raycast 
        //if raycast hit
        //switch case on tag

        if (Input.GetMouseButtonDown(0))
        {
            if (GameController.INSTANCE.Current_Game_State == GameState.EVENT || GameController.INSTANCE.Current_Game_State == GameState.PAUSED)
                return;

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out hit))
            {
                GameObject go = hit.transform.gameObject;

                Debug.Log("clicked on " + go.name);

                switch (go.tag)
                {
                    case "Task":
                        ShipController.INSTANCE.OnTaskClick(go);
                        break;

                    case "ClickableNode":
                        ShipController.INSTANCE.OnNodeClick(go);
                        break;

                    case "CrewMember":
                        CrewController.INSTANCE.OnCrewMemberClick(go);
                        break;

                    case "CrewMemberUISprite":
                        break;

                    default:
                        break;
                }
            }
        }

        //if right click 
        //probably just wanting to deselect crew member - call crew controller function
        //note possible bug where crew member isn't properly deselected??? - probably not but be on lookout
        if (Input.GetMouseButtonDown(1))
        {
            if (GameController.INSTANCE.Current_Game_State == GameState.EVENT || GameController.INSTANCE.Current_Game_State == GameState.PAUSED)
                return;

            CrewController.INSTANCE.DeselectCrewMember();
        }
        
        ///////////////////////////////

        //if (Input.GetKeyDown(KeyCode.I))
        //    GameController.INSTANCE.LostHullIntegrity();

        //if (Input.GetKeyDown(KeyCode.L))
            //GameController.INSTANCE.LostLifeSupport();

        //if (Input.GetKeyDown(KeyCode.S))
        //    GameController.INSTANCE.LostSanity();

        //if (Input.GetKeyDown(KeyCode.W))
        //    GameController.INSTANCE.ChangeDistanceToEarth(-50000.0f);

        ///////////////////////////

        if (GameController.INSTANCE.Current_Game_State == GameState.LOST_HULL 
            || GameController.INSTANCE.Current_Game_State == GameState.LOST_LIFE_SUPPORT 
            || GameController.INSTANCE.Current_Game_State == GameState.LOST_STRESSED)
        {
            if ((GameController.INSTANCE.No_Of_Tries < GameController.MAX_NO_OF_TRIES) 
                || (GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    GameController.INSTANCE.RestartInCurrentDifficulty();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (GameController.INSTANCE.Current_Game_Difficulty)
                {
                    case GameDifficulty.EASY:
                        GameController.INSTANCE.RestartInMediumDifficulty();
                        break;
                    case GameDifficulty.MEDIUM:
                        GameController.INSTANCE.RestartInHardDifficulty();
                        break;
                    default:
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
                GameController.INSTANCE.Menu();
        }
        else if (GameController.INSTANCE.Current_Game_State == GameState.WON)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (GameController.INSTANCE.Current_Game_Difficulty)
                {
                    case GameDifficulty.EASY:
                        GameController.INSTANCE.RestartInMediumDifficulty();
                        break;
                    case GameDifficulty.MEDIUM:
                        GameController.INSTANCE.RestartInHardDifficulty();
                        break;
                    default:
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
            GameController.INSTANCE.Menu();

        if (Input.GetKeyDown(KeyCode.Tab) && GameController.INSTANCE.HasFinishedTutorial())
        {
            if (GameController.INSTANCE.Current_Game_State != GameState.PAUSED)
            {
                GameController.INSTANCE.Pause();
            }
            else
            {
                GameController.INSTANCE.Unpause();
            }
        }
    }
}
