﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HonsProj;

public class CrewController : MonoBehaviour
{
    public static CrewController INSTANCE { get; private set; }

    [SerializeField]
    GameObject crewPrefab;

    public CrewMember Selected_Crew_Member { get; private set; }

    TwoWayDictionary<CrewMember> crew = new TwoWayDictionary<CrewMember>();

    List<CrewMember> crewFromXml;

    // Use this for initialization
    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE CREW CONTROLLER!!!!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameController.INSTANCE.OnRestartGame += RestartCrew;

        InitialiseCrew();
    }

    void RestartCrew()
    {
        Selected_Crew_Member = null;

        crew.Clear();
        //crew = null;
        crewFromXml.Clear();
        crewFromXml = null;

        InitialiseCrew();
    }

    void InitialiseCrew()
    {
#if DEBUG
        crewFromXml = XmlDataLoader.GetCrewFromXML(@"Assets/xml files/crew_members.xml");
#else
        crewFromXml = XmlDataLoader.GetCrewFromXML(@"xml files/crew_members.xml");
#endif
        foreach (CrewMember crewMember in crewFromXml)
        {
            GameObject go = Instantiate(crewPrefab, transform);
            go.name = crewMember.Crew_Member_Name;

            switch (GameController.INSTANCE.Current_Game_Difficulty)
            {
                case GameDifficulty.EASY:
                    crewMember.SetLevel(7);
                    break;
                case GameDifficulty.MEDIUM:
                    crewMember.SetLevel(4);
                    break;
                case GameDifficulty.HARD:
                    crewMember.SetLevel(2);
                    break;
                default:
                    break;
            }


            crew.Add(crewMember, go);

            crewMember.SetCrewMemberPosCallBack += SetCrewMemberPos;
            crewMember.GetCrewMemberPosCallBack += GetCrewMemberPos;
            crewMember.MoveCrewMemberCallBack += MoveCrewMember;
            crewMember.UpdateUICallBack += UpdateCrewMemberUI;

            Node randomNode = ShipController.INSTANCE.GetRandomNodeInRoom(ShipController.INSTANCE.GetRandomRoom(false), true);

            crewMember.SetPos((float)randomNode.X, (float)randomNode.Y);

            SpriteRenderer s_rend = go.GetComponent<SpriteRenderer>();

            //todo null checking

            switch (crewMember.Crew_Member_Role)
            {
                case CrewMemberRole.CAPTAIN:
                    s_rend.color = Color.blue;
                    break;
                case CrewMemberRole.COMMS_OFFICER:
                    s_rend.color = Color.cyan;
                    break;
                case CrewMemberRole.ENGINEER:
                    s_rend.color = Color.green;
                    break;
                case CrewMemberRole.FIRST_OFFICER:
                    s_rend.color = Color.magenta;
                    break;
                case CrewMemberRole.PILOT:
                    s_rend.color = Color.red;
                    break;
                case CrewMemberRole.SHIP_MEDIC:
                    s_rend.color = Color.yellow;
                    break;
                case CrewMemberRole.WEAPONS_OFFICER:
                    s_rend.color = Color.white;
                    break;
                default:
                    s_rend.color = Color.black;
                    break;
            }
        }
    }

    public void UpdateCrewMemberUI(CrewMember cm)
    {
        UIManager.INSTANCE.UpdateCrewMemberUI(cm);
    }

    public void RandomiseCrewPositions()
    {
        foreach(CrewMember cm in crew.GetFs())
        {
            Node randomNode = ShipController.INSTANCE.GetRandomNodeInRoom(ShipController.INSTANCE.GetRandomRoom(false), true);

            cm.SetPos((float)randomNode.X, (float)randomNode.Y);
        }
    }

    public void SelectCrewMember(CrewMember cm)
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        if (crew.ContainsF(cm))
        {
            if (Selected_Crew_Member != null)
            {
                GameObject crewGo = crew.GetGO(Selected_Crew_Member);

                if (crewGo != null)
                {
                    crewGo.GetComponent<cakeslice.Outline>().enabled = false;
                    Selected_Crew_Member = null;
                }
            }

            crew.GetGO(cm).GetComponent<cakeslice.Outline>().enabled = true;
            Selected_Crew_Member = cm;
        }
    }

    //note all of this methods will become lambdas in start method
    #region temp methods
    void SetCrewMemberPos(CrewMember crewMember, float x, float y)
    {
        Vector3 vec = crew.GetGO(crewMember).transform.position;

        vec.x = x;
        vec.y = y;

        crew.GetGO(crewMember).transform.position = vec;
    }

    Node GetCrewMemberPos(CrewMember crewMember)
    {
        Vector3 vec = crew.GetGO(crewMember).transform.position;

        return new Node(vec.x, vec.y);
    }

    void MoveCrewMember(CrewMember crewMember, Node end)
    {
        Vector3 end_vec = new Vector3();
        end_vec.x = (float)end.X;
        end_vec.y = (float)end.Y;

        GameObject crewMemberGO = crew.GetGO(crewMember);

        crewMemberGO.transform.position = Vector3.LerpUnclamped(crewMemberGO.transform.position, end_vec, 15 * Time.deltaTime);

        if (crewMemberGO.transform.position == end_vec)
        {
            crewMember.DequeueFromPath();
        }
    }
    #endregion

    public CrewMember GetRandomCrewMember()
    {
        return crew.GetFs()[Random.Range(0, crew.GetCount())];
    }

    public List<CrewMember> GetCrewMembers()
    {
        //note return a copy????
        return crew.GetFs().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        foreach (CrewMember crew_member in crew.GetFs())
        {
            if (crew_member.HasPath())
            {
                crew_member.Move();
            }

            if (crew_member.Current_Task != null && ShipController.INSTANCE.IsTaskInList(crew_member.Current_Task))
            {
                if (crew_member.Current_Task.DoWork(Time.deltaTime * crew_member.GetWorkSpeed(), crew_member))
                {
                    //Debug.Log("crew member: " + crew_member.Crew_Member_Name + " is doing " + crew_member.Current_Task + " at " + Time.deltaTime);
                }
            }
        }
    }

    public void OnCrewMemberClick(GameObject cm_go)
    {
        CrewMember selectedMember = crew.GetfType(cm_go);

        if (selectedMember != null)
        {
            SelectCrewMember(selectedMember);
            UIManager.INSTANCE.SelectCrewMember(selectedMember);
        }
    }

    public void DeselectCrewMember()
    {
        if (Selected_Crew_Member != null)
        {
            GameObject crewGo = crew.GetGO(Selected_Crew_Member);

            if (crewGo != null)
            {
                crewGo.GetComponent<cakeslice.Outline>().enabled = false;
                Selected_Crew_Member = null;

                UIManager.INSTANCE.SelectCrewMember(Selected_Crew_Member);
            }
        }
    }
}
