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

    public void UpdateShipSpeedDisplay(float ship_speed)
    {
        Current_Ship_Speed_Display.text = "Warp Speed: \n<color=#00ffff>" +ship_speed+ "</color>";
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
    }

    public void ShowLossDisplay(GameState state)
    {
        Loss_Display.SetActive(true);

        TMPro.TextMeshProUGUI loss_text = Loss_Display.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        switch (state)
        {
            case GameState.LOST_HULL:
                loss_text.text =
                    "Hull Destroyed!\n\nThe <color=white>ship hull integrity</color> has reached 0%!\n\nPress 'r' to try again";
                break;
            case GameState.LOST_LIFE_SUPPORT:
                loss_text.text =
                    "Life Support Failure!\n\nThe <color=white>life support efficiency</color> has reached 0%!\n\nPress 'r' to try again";
                break;
            case GameState.LOST_STRESSED:
                loss_text.text =
                    "Crew Too Stressed!\n\nThe <color=white>crew stress</color> has reached 500!\n\nPress 'r' to try again";
                break;
        }

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
        Crew_Stress_Display.text = "Crew Stress: " + new_value.ToString("0.0");
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

        crewToUIsprite.Add(cm, crewUIGo);

        RectTransform rect = CrewSelectionBar.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(crewToUIsprite.GetCount() * CrewMemberUIPrefab.GetComponent<RectTransform>().rect.width,
                                        CrewMemberUIPrefab.GetComponent<RectTransform>().rect.height);
    }
}
