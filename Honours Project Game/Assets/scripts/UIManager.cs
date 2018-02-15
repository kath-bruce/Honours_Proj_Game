using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HonsProj;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE { get; protected set; }

    //temp - use a UI manager script
    [SerializeField]
    Text Hull_Integrity_Display;
    [SerializeField]
    Text Shield_Capacity_Display;
    [SerializeField]
    Text Life_Support_Efficiency_Display;
    [SerializeField]
    Text Ship_Stress_Display;

    [SerializeField]
    Text Distance_To_Earth_Display;

    [SerializeField]
    GameObject CrewSelectionBar;

    [SerializeField]
    GameObject CrewMemberUIPrefab;

    [SerializeField]
    GameObject SelectedCrewMemberHighlight;

    private GameObject Selected_Crew_Sprite = null;

    private TwoWayDictionary<CrewMember> crewToUIsprite;

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
        crewToUIsprite = new TwoWayDictionary<CrewMember>();

        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            UpdateCrewSelectionBar(cm);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Game_State != GameState.IN_PLAY)
            return;


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

    public void UpdateShipStressDisplay(float new_value)
    {
        Ship_Stress_Display.text = "Ship stress: " + new_value.ToString("0.0");
    }

    public void UpdateDistanceToEarth(float new_value)
    {
        Distance_To_Earth_Display.text = "Distance To Earth: " + '\n' + new_value.ToString("0.00") + '\n' + "light years";
    }

    public void SelectCrewMember(GameObject go)
    {
        if (GameController.INSTANCE.Game_State != GameState.IN_PLAY)
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
        if (GameController.INSTANCE.Game_State != GameState.IN_PLAY)
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
