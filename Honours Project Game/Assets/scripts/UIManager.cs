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
    GameObject CrewSelectionBar;

    [SerializeField]
    GameObject CrewMemberUIPrefab;

    [SerializeField]
    GameObject SelectedCrewMemberHighlight;

    private GameObject Selected_Crew_Sprite = null;

    private TwoWayDictionary<CrewMember> crewToUIsprite;

    // Use this for initialization
    void Start()
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

    public void SelectCrewMember(GameObject go)
    {
        if (GameController.INSTANCE.Game_State != GameState.IN_PLAY)
            return;

        if (go == null)
        {
            SelectedCrewMemberHighlight.SetActive(false);
            Selected_Crew_Sprite = null;
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
