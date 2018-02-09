using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HonsProj;

public class CrewController : MonoBehaviour
{
    public static CrewController INSTANCE { get; private set; }

    //[SerializeField]
    //int sizeOfCrew = 3;

    [SerializeField]
    GameObject crewPrefab;

    public CrewMember Selected_Crew_Member { get; private set; }

    TwoWayDictionary<CrewMember> crew = new TwoWayDictionary<CrewMember>();

    List<CrewMember> crewFromXml;

    // Use this for initialization
    void Start()
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

        crewFromXml = XmlDataLoader.GetCrewFromXML(@"Assets/xml files/crew_members.xml");

        foreach (CrewMember crewMember in crewFromXml)
        {
            GameObject go = Instantiate(crewPrefab, transform);
            go.name = crewMember.Crew_Member_Name;

            crew.Add(crewMember, go);

            crewMember.SetCrewMemberPosCallBack += SetCrewMemberPos;
            crewMember.GetCrewMemberPosCallBack += GetCrewMemberPos;
            crewMember.MoveCrewMemberCallBack += MoveCrewMember;

            Room randomRoom = ShipController.INSTANCE.GetRandomRoom();

            crewMember.SetPos(randomRoom.Room_Info.X, randomRoom.Room_Info.Y);

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
                    break;
            }
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
            //Player.INSTANCE.DequeueFromPath();
            crewMember.DequeueFromPath();

            if (!crewMember.HasPath())
            {
                Debug.Log("crew member: " + crewMember.Crew_Member_Name + ", crew task: " + crewMember.Current_Task);
            }
        }
    }
    #endregion

    public CrewMember GetRandomCrewMember()
    {
        return crew.GetFs()[Random.Range(0, crew.GetCount())];
    }

    // Update is called once per frame
    void Update()
    {
        foreach (CrewMember crew_member in crew.GetFs())
        {
            if (crew_member.HasPath())
            {
                crew_member.Move();
            }

            if (crew_member.Current_Task != null && ShipController.INSTANCE.IsTaskInList(crew_member.Current_Task))
            {
                if (
                    crew_member.Current_Task.DoWork(Time.deltaTime, crew_member)
                    )
                {
                    //    Debug.Log("crew member: " + crew_member.Crew_Member_Name + " is doing " + crew_member.Current_Task + " at " + Time.deltaTime);
                }
            }
        }
        //todo make box colliders on sprite 2d???
        if (Input.GetMouseButtonDown(0)) //temp - handling this here
        {
            //if (Selected_Crew_Member != null)
            //{
            //    GameObject crewGo = crew.GetGO(Selected_Crew_Member);

            //    if (crewGo != null)
            //    {
            //        crewGo.GetComponent<cakeslice.Outline>().enabled = false;
            //        Selected_Crew_Member = null;
            //    }
            //}

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out hit))
            {
                GameObject go = hit.transform.gameObject;

                CrewMember selectedMember = crew.GetfType(go);

                if (selectedMember != null)
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

                    go.GetComponent<cakeslice.Outline>().enabled = true;
                    Selected_Crew_Member = selectedMember;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
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
        }
    }
}
