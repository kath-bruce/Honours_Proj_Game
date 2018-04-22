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
        //deselect crew member - call crew controller function
        if (Input.GetMouseButtonDown(1))
        {
            if (GameController.INSTANCE.Current_Game_State == GameState.EVENT || GameController.INSTANCE.Current_Game_State == GameState.PAUSED)
                return;

            CrewController.INSTANCE.DeselectCrewMember();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
            GameController.INSTANCE.Menu();

        if (GameController.INSTANCE.Current_Game_State == GameState.LOST_HULL
            || GameController.INSTANCE.Current_Game_State == GameState.LOST_LIFE_SUPPORT
            || GameController.INSTANCE.Current_Game_State == GameState.LOST_STRESSED
            || GameController.INSTANCE.Current_Game_State == GameState.WON)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameController.INSTANCE.RestartInCurrentDifficulty();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
            GameController.INSTANCE.Menu();

        if (Input.GetKeyDown(KeyCode.Tab))
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
