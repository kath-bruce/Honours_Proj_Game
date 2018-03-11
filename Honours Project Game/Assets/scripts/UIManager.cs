using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HonsProj;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE { get; protected set; }

    [SerializeField]
    GameObject Win_Display;

    [SerializeField]
    GameObject Loss_Display;

    [SerializeField]
    Text Hull_Integrity_Display;
    [SerializeField]
    Text Shield_Capacity_Display;
    [SerializeField]
    Text Life_Support_Efficiency_Display;
    [SerializeField]
    Text Crew_Stress_Display;

    [SerializeField]
    Text Distance_To_Earth_Display;

    [SerializeField]
    GameObject CrewSelectionBar;

    [SerializeField]
    GameObject CrewMemberUIPrefab;

    [SerializeField]
    GameObject SelectedCrewMemberHighlight;

    [SerializeField]
    TMPro.TextMeshProUGUI Current_Phase_Display;

    [SerializeField]
    TMPro.TextMeshProUGUI Current_Difficulty_Display;

    [SerializeField]
    TMPro.TextMeshProUGUI Current_Ship_Speed_Display;

    [SerializeField]
    GameObject Pause_Display;

    [SerializeField]
    GameObject Crew_Legend_Display;

    [SerializeField]
    GameObject Crew_Legend_Entry_Prefab;

    [SerializeField]
    GameObject Tutorial;

    private GameObject Selected_Crew_Sprite = null;

    private TwoWayDictionary<CrewMember> crewToUIsprite = new TwoWayDictionary<CrewMember>();

    // Use this for initialization
    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE UI MANAGER!!!!");
            Destroy(gameObject);
        }

    }

    void Start()
    {
        GameController.INSTANCE.OnRestartGame += RestartUI;

        InitialiseUI();
    }

    void InitialiseUI()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            UpdateCrewSelectionBar(cm);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        //
    }

    void RestartUI()
    {
        Win_Display.SetActive(false);
        Loss_Display.SetActive(false);
        Selected_Crew_Sprite = null;
        SelectedCrewMemberHighlight.SetActive(false);

        crewToUIsprite.Clear();

        InitialiseUI();
    }

    public void UpdatePauseDisplay(bool isPaused)
    {
        Pause_Display.SetActive(isPaused);
        Pause_Display.transform.SetAsLastSibling();

        if (isPaused)
            UpdateCrewLegend(CrewController.INSTANCE.GetCrewMembers(), ShipController.INSTANCE.GetTasksForRoles());
        else
        {
            ClearCrewLegend();
            Tutorial.SetActive(false);
        }
    }

    public void UpdateShipSpeedDisplay(float ship_speed)
    {
        Current_Ship_Speed_Display.text = "Warp Speed: \n<color=#00ffff>" + ship_speed + "</color>";
    }

    public void UpdatePhaseDisplay(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.FIRST_PHASE:
                Current_Phase_Display.text = "Phase: <color=green>1/3</color>";
                break;
            case GamePhase.MIDDLE_PHASE:
                Current_Phase_Display.text = "Phase: <color=yellow>2/3</color>";
                break;
            case GamePhase.FINAL_PHASE:
                Current_Phase_Display.text = "Phase: <color=red>3/3</color>";
                break;
            default:
                break;
        }
    }

    public void UpdateDifficultyDisplay(GameDifficulty diff)
    {
        switch (diff)
        {
            case GameDifficulty.EASY:
                Current_Difficulty_Display.text = "Difficulty: <color=green>Easy</color>";
                break;
            case GameDifficulty.MEDIUM:
                Current_Difficulty_Display.text = "Difficulty: <color=yellow>Med</color>";
                break;
            case GameDifficulty.HARD:
                Current_Difficulty_Display.text = "Difficulty: <color=red>Hard</color>";
                break;
            default:
                break;
        }
    }

    public void ShowWinDisplay()
    {
        Win_Display.SetActive(true);

        if (GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
        {
            TMPro.TextMeshProUGUI win_text = Win_Display.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            win_text.text = "<color=white>Reached Earth!<size=30>\n\nYou have made it back in one piece!</size></color>\n\n"+
                "<size=20>Press 'r' to try again or Escape to go back to the menu</size>";
        }

        Win_Display.transform.SetAsLastSibling();
    }

    public void ShowLossDisplay(GameState state, int no_of_tries)
    {
        Loss_Display.SetActive(true);

        TMPro.TextMeshProUGUI[] loss_texts = Loss_Display.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        foreach(TMPro.TextMeshProUGUI loss_text in loss_texts)
        {
            if (loss_text.gameObject.name == "Tries")
            {
                if (GameController.INSTANCE.Current_Game_Difficulty != GameDifficulty.HARD)
                    loss_text.text = "Tries: " + no_of_tries + "/" + GameController.MAX_NO_OF_TRIES;
                else
                    loss_text.text = "";
            }
            else
            {
                switch (state)
                {
                    case GameState.LOST_HULL:
                        loss_text.text =
                            "Hull Destroyed!\n\n<size=30>The<color=white> ship hull integrity</color> has reached 0%!</size>\n\n";

                        if (GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or Escape to go back to the menu</size>";
                        }
                        else if (no_of_tries < GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or space to advance to the next difficulty level</size>";
                        }
                        else if (no_of_tries >= GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press space to advance to the next difficulty level</size>";
                        }
                        break;
                    case GameState.LOST_LIFE_SUPPORT:
                        loss_text.text =
                            "Life Support Failure!\n\n<size=30>The<color=white> life support efficiency</color> has reached 0%!</size>\n\n";

                        if (GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or Escape to go back to the menu</size>";
                        }
                        else if (no_of_tries < GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or space to advance to the next difficulty level</size>";
                        }
                        else if (no_of_tries >= GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press space to advance to the next difficulty level</size>";
                        }

                        break;
                    case GameState.LOST_STRESSED:
                        loss_text.text =
                            "Crew Too Stressed!\n\n<size=30>The <color=white>crew stress</color> has reached 300!</size>\n\n";

                        if (GameController.INSTANCE.Current_Game_Difficulty == GameDifficulty.HARD)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or Escape to go back to the menu</size>";
                        }
                        else if (no_of_tries < GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press 'r' to try again or space to advance to the next difficulty level</size>";
                        }
                        else if (no_of_tries >= GameController.MAX_NO_OF_TRIES)
                        {
                            loss_text.text += "<size=20>Press space to advance to the next difficulty level</size>";
                        }

                        break;
                }
            }
        }

        Loss_Display.transform.SetAsLastSibling();
    }

    public void UpdateHullIntegrityDisplay(float new_value)
    {
        Hull_Integrity_Display.text = "Hull Integrity: " + new_value.ToString("0") + "%";
    }

    public void UpdateShieldCapacityDisplay(float new_value)
    {
        Shield_Capacity_Display.text = "Shield Capacity: " + new_value.ToString("0") + "%";
    }

    public void UpdateLifeSupportEfficiencyDisplay(float new_value)
    {
        Life_Support_Efficiency_Display.text = "Life Support Efficiency: " + new_value.ToString("0") + "%";
    }

    public void UpdateCrewStressDisplay(float new_value)
    {
        Crew_Stress_Display.text = "Crew Stress: " + new_value.ToString("0") + "/300";
    }

    public void UpdateDistanceToEarth(float new_value)
    {
        Distance_To_Earth_Display.text = "Distance To Earth: " + '\n' + new_value.ToString("0.00") + '\n' + "light years";
    }

    public void SelectCrewMember(GameObject go)
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        if (go == null)
        {
            SelectedCrewMemberHighlight.SetActive(false);
            Selected_Crew_Sprite = null;
            return;
        }

        if (crewToUIsprite.ContainsGO(go))
        {
            CrewMember cm = crewToUIsprite.GetfType(go);

            Selected_Crew_Sprite = go;
            SelectedCrewMemberHighlight.SetActive(true);
            SelectedCrewMemberHighlight.GetComponent<RectTransform>().SetPositionAndRotation(Selected_Crew_Sprite.transform.position, Quaternion.identity);

            CrewController.INSTANCE.SelectCrewMember(cm);

        }
    }

    public void SelectCrewMember(CrewMember cm)
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        if (cm == null)
        {
            SelectedCrewMemberHighlight.SetActive(false);
            Selected_Crew_Sprite = null;
            return;
        }

        if (crewToUIsprite.ContainsF(cm))
        {
            GameObject go = crewToUIsprite.GetGO(cm);

            Selected_Crew_Sprite = go;
            SelectedCrewMemberHighlight.SetActive(true);
            SelectedCrewMemberHighlight.GetComponent<RectTransform>().SetPositionAndRotation(Selected_Crew_Sprite.transform.position, Quaternion.identity);

            CrewController.INSTANCE.SelectCrewMember(cm);

        }
    }

    public void UpdateCrewLegend(List<CrewMember> crew, Dictionary<CrewMemberRole, List<TaskType>> tasks_for_roles)
    {
        foreach (CrewMember cm in crew)
        {
            GameObject current_legend_entry = Instantiate(Crew_Legend_Entry_Prefab, Crew_Legend_Display.transform);

            foreach (Transform child in current_legend_entry.transform)
            {
                switch (child.tag)
                {
                    case "CrewLegendSprite":

                        Image crewImage = child.GetComponent<Image>();

                        //note could potentially have a method for this?
                        switch (cm.Crew_Member_Role)
                        {
                            case CrewMemberRole.CAPTAIN:
                                crewImage.color = Color.blue;
                                break;
                            case CrewMemberRole.COMMS_OFFICER:
                                crewImage.color = Color.cyan;
                                break;
                            case CrewMemberRole.ENGINEER:
                                crewImage.color = Color.green;
                                break;
                            case CrewMemberRole.FIRST_OFFICER:
                                crewImage.color = Color.magenta;
                                break;
                            case CrewMemberRole.PILOT:
                                crewImage.color = Color.red;
                                break;
                            case CrewMemberRole.SHIP_MEDIC:
                                crewImage.color = Color.yellow;
                                break;
                            case CrewMemberRole.WEAPONS_OFFICER:
                                crewImage.color = Color.white;
                                break;
                            default:
                                crewImage.color = Color.black;
                                break;
                        }

                        crewImage.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Lvl " + cm.Crew_Member_Level;

                        break;

                    case "CrewLegendTitleAndName":

                        TMPro.TextMeshProUGUI titleAndName = child.GetComponent<TMPro.TextMeshProUGUI>();

                        switch (cm.Crew_Member_Role)
                        {
                            case CrewMemberRole.CAPTAIN:
                                titleAndName.text = "<color=blue>Captain</color> " + cm.Crew_Member_Name;// Color.blue;
                                break;
                            case CrewMemberRole.COMMS_OFFICER:
                                titleAndName.text = "<color=#00ffff>Comms Officer</color> " + cm.Crew_Member_Name;// Color.cyan;
                                break;
                            case CrewMemberRole.ENGINEER:
                                titleAndName.text = "<color=green>Engineer</color> " + cm.Crew_Member_Name;// Color.green;
                                break;
                            case CrewMemberRole.FIRST_OFFICER:
                                titleAndName.text = "<color=#ff00ff>First Officer</color> " + cm.Crew_Member_Name;// Color.magenta;
                                break;
                            case CrewMemberRole.PILOT:
                                titleAndName.text = "<color=red>Pilot</color> " + cm.Crew_Member_Name;// Color.red;
                                break;
                            case CrewMemberRole.SHIP_MEDIC:
                                titleAndName.text = "<color=yellow>Medic</color> " + cm.Crew_Member_Name;// Color.yellow;
                                break;
                            case CrewMemberRole.WEAPONS_OFFICER:
                                titleAndName.text = "<color=white>Weapons Officer</color> " + cm.Crew_Member_Name;// Color.white;
                                break;
                            default:
                                titleAndName.text = "WOOPS!";
                                break;
                        }

                        break;

                    case "CrewLegendTasks":

                        if (cm.Crew_Member_Role == CrewMemberRole.CAPTAIN)
                        {
                            child.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "> All tasks";
                            break;
                        }

                        for (int i = 0; i < tasks_for_roles[cm.Crew_Member_Role].Count; i++)
                        {
                            TMPro.TextMeshProUGUI taskName = child.GetChild(i).GetComponent<TMPro.TextMeshProUGUI>();

                            TaskType t = tasks_for_roles[cm.Crew_Member_Role][i];

                            switch (t)
                            {
                                case TaskType.TORPEDO_ASTEROIDS:
                                    taskName.text = "> <color=#c300c3>Torpedo asteroids</color>";// Color.magenta; //#c300c3 //#ff00ff
                                    break;
                                case TaskType.REPAIR:
                                    taskName.text = "> <color=#0000c3>Repair</color>";// Color.blue; //#0000c3 //blue
                                    break;
                                case TaskType.STEER_SHIP:
                                    taskName.text = "> <color=#00c300>Steer ship</color>";// Color.green; //#00c300 //green
                                    break;
                                case TaskType.CHARGE_SHIELDS:
                                    taskName.text = "> <color=#c30000>Charge shields</color>";// Color.red; //#c30000 //red
                                    break;
                                case TaskType.MAINTAIN_LIFE_SUPPORT:
                                    taskName.text = "> <color=#c3b403>Maintain life support</color>";// Color.yellow; //#c3b403 //yellow
                                    break;
                                case TaskType.HEAL_CREW_MEMBER:
                                    taskName.text = "> <color=#00c3c3>Heal crew member</color>";// Color.cyan; //#00c3c3 //#00ffff
                                    break;
                                case TaskType.MAINTAIN_COMMS:
                                    taskName.text = "> <color=black>Maintain comms</color>";// Color.black;
                                    break;
                                case TaskType.TALK_TO_OTHER_SHIP:
                                    taskName.text = "> <color=#959595>Talk to other ship</color>";//grey //#C3C3C3FF //#959595
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;

                    default:
                        break;
                }

            }
        }
    }

    public void ClearCrewLegend()
    {
        foreach (Transform child in Crew_Legend_Display.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateCrewSelectionBar(CrewMember cm)
    {
        GameObject crewUIParent = CrewSelectionBar.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
        GameObject crewUIGo = Instantiate(CrewMemberUIPrefab, crewUIParent.transform);

        Image crewImage = crewUIGo.GetComponent<Image>();

        switch (cm.Crew_Member_Role)
        {
            case CrewMemberRole.CAPTAIN:
                crewImage.color = Color.blue;
                break;
            case CrewMemberRole.COMMS_OFFICER:
                crewImage.color = Color.cyan;
                break;
            case CrewMemberRole.ENGINEER:
                crewImage.color = Color.green;
                break;
            case CrewMemberRole.FIRST_OFFICER:
                crewImage.color = Color.magenta;
                break;
            case CrewMemberRole.PILOT:
                crewImage.color = Color.red;
                break;
            case CrewMemberRole.SHIP_MEDIC:
                crewImage.color = Color.yellow;
                break;
            case CrewMemberRole.WEAPONS_OFFICER:
                crewImage.color = Color.white;
                break;
            default:
                crewImage.color = Color.black;
                break;
        }

        foreach (Transform child in crewUIGo.transform)
        {
            switch (child.tag)
            {
                case "CrewMemberUILevel":
                    child.GetComponent<TMPro.TextMeshProUGUI>().text = cm.Crew_Member_Level.ToString();

                    break;

                case "CrewMemberUITitle":
                    TMPro.TextMeshProUGUI titleText = child.GetComponent<TMPro.TextMeshProUGUI>();

                    switch (cm.Crew_Member_Role)
                    {
                        case CrewMemberRole.CAPTAIN:
                            titleText.text = "<color=blue>Cpt.</color>";// Color.blue;
                            break;
                        case CrewMemberRole.COMMS_OFFICER:
                            titleText.text = "<color=#00ffff>Comms.</color>";// Color.cyan;
                            break;
                        case CrewMemberRole.ENGINEER:
                            titleText.text = "<color=green>Eng.</color>";// Color.green;
                            break;
                        case CrewMemberRole.FIRST_OFFICER:
                            titleText.text = "<color=#ff00ff>1st O.</color>";// Color.magenta;
                            break;
                        case CrewMemberRole.PILOT:
                            titleText.text = "<color=red>Pilot</color>";// Color.red;
                            break;
                        case CrewMemberRole.SHIP_MEDIC:
                            titleText.text = "<color=yellow>Med.</color>";// Color.yellow;
                            break;
                        case CrewMemberRole.WEAPONS_OFFICER:
                            titleText.text = "<color=white>W Off.</color>";// Color.white;
                            break;
                        default:
                            titleText.text = "WOOPS!";
                            break;
                    }


                    break;

                default:
                    break;
            }
        }

        crewToUIsprite.Add(cm, crewUIGo);

        RectTransform rect = CrewSelectionBar.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(crewToUIsprite.GetCount() * CrewMemberUIPrefab.GetComponent<RectTransform>().rect.width,
                                        CrewMemberUIPrefab.GetComponent<RectTransform>().rect.height + 20);
    }

    public void UpdateCrewMemberUI(CrewMember cm)
    {
        if (crewToUIsprite.ContainsF(cm))
        {
            GameObject uiObj = crewToUIsprite.GetGO(cm);

            foreach (Transform child in uiObj.transform)
            {
                if (child.tag == "CrewMemberUILevel")
                {
                    child.GetComponent<TMPro.TextMeshProUGUI>().text = cm.Crew_Member_Level.ToString();
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void ViewTutorial()
    {
        AudioController.INSTANCE.PlayButtonSelectAudio();
        Tutorial.SetActive(true);
        Tutorial.transform.SetAsLastSibling();
    }
}
