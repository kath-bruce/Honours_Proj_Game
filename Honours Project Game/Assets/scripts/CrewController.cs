using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HonsProj;

public class CrewController : MonoBehaviour
{
    public static CrewController INSTANCE { get; private set; }

    [SerializeField]
    int sizeOfCrew = 3;

    [SerializeField]
    GameObject crewPrefab;

    TwoWayDictionary<CrewMember> crew = new TwoWayDictionary<CrewMember>();

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

        //add crew with random roles //temp - will use xml doc to populate crew
        CrewMember maddy = new CrewMember("Maddy", (CrewMemberRole)Random.Range(0, System.Enum.GetNames(typeof(CrewMemberRole)).Length));
        CrewMember declan = new CrewMember("Declan", (CrewMemberRole)Random.Range(0, System.Enum.GetNames(typeof(CrewMemberRole)).Length));
        CrewMember meshal = new CrewMember("Meshal", (CrewMemberRole)Random.Range(0, System.Enum.GetNames(typeof(CrewMemberRole)).Length));

        GameObject maddyGO = Instantiate(crewPrefab, transform);
        maddyGO.name = maddy.Crew_Member_Name;

        GameObject declanGO = Instantiate(crewPrefab, transform);
        declanGO.name = declan.Crew_Member_Name;

        GameObject meshalGO = Instantiate(crewPrefab, transform);
        meshalGO.name = meshal.Crew_Member_Name;

        crew.Add(maddy, maddyGO);
        crew.Add(declan, declanGO);
        crew.Add(meshal, meshalGO);

        maddy.SetCrewMemberPosCallBack += SetCrewMemberPos;
        declan.SetCrewMemberPosCallBack+= SetCrewMemberPos;
        meshal.SetCrewMemberPosCallBack += SetCrewMemberPos;

        maddy.GetCrewMemberPosCallBack += GetCrewMemberPos;
        declan.GetCrewMemberPosCallBack += GetCrewMemberPos;
        meshal.GetCrewMemberPosCallBack += GetCrewMemberPos;

        maddy.MoveCrewMemberCallBack += MoveCrewMember;
        declan.MoveCrewMemberCallBack += MoveCrewMember;
        meshal.MoveCrewMemberCallBack += MoveCrewMember;

        Room maddyRoom = ShipController.INSTANCE.GetRandomRoom();
        Room declanRoom = ShipController.INSTANCE.GetRandomRoom();
        Room meshalRoom = ShipController.INSTANCE.GetRandomRoom();

        maddy.SetPos(maddyRoom.Room_Info.X, maddyRoom.Room_Info.Y);
        declan.SetPos(declanRoom.Room_Info.X, declanRoom.Room_Info.Y);
        meshal.SetPos(meshalRoom.Room_Info.X, meshalRoom.Room_Info.Y);

        foreach (GameObject memberGo in crew.GetGOs().ToList())
        {
            SpriteRenderer s_rend = memberGo.GetComponent<SpriteRenderer>();

            //todo null checking
            
            switch (crew.GetfType(memberGo).Crew_Member_Role)
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
                crew_member.Current_Task.DoWork(Time.deltaTime);
            }
        }
    }
}
