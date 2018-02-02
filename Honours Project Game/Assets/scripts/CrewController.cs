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

        //add crew //temp - will use xml doc to populate crew
        CrewMember maddy = new CrewMember("Maddy");
        CrewMember declan = new CrewMember("Declan");
        CrewMember meshal = new CrewMember("Meshal");

        GameObject maddyGO = Instantiate(crewPrefab, transform);
        maddyGO.name = maddy.Crew_Member_Name;

        GameObject declanGO = Instantiate(crewPrefab, transform);
        declanGO.name = declan.Crew_Member_Name;

        GameObject meshalGO = Instantiate(crewPrefab, transform);
        meshalGO.name = meshal.Crew_Member_Name;

        crew.Add(maddy, maddyGO);
        crew.Add(declan, declanGO);
        crew.Add(meshal, meshalGO);

        maddy.SetPos_RegisterCallback(SetCrewMemberPos);
        declan.SetPos_RegisterCallback(SetCrewMemberPos);
        meshal.SetPos_RegisterCallback(SetCrewMemberPos);

        maddy.GetPos_RegisterCallback(GetCrewMemberPos);
        declan.GetPos_RegisterCallback(GetCrewMemberPos);
        meshal.GetPos_RegisterCallback(GetCrewMemberPos);

        maddy.Move_RegisterCallback(MoveCrewMember);
        declan.Move_RegisterCallback(MoveCrewMember);
        meshal.Move_RegisterCallback(MoveCrewMember);

        Room maddyRoom = ShipController.INSTANCE.GetRandomRoom();
        Room declanRoom = ShipController.INSTANCE.GetRandomRoom();
        Room meshalRoom = ShipController.INSTANCE.GetRandomRoom();

        maddy.SetPos(maddyRoom.Room_Info.X, maddyRoom.Room_Info.Y);
        declan.SetPos(declanRoom.Room_Info.X, declanRoom.Room_Info.Y);
        meshal.SetPos(meshalRoom.Room_Info.X, meshalRoom.Room_Info.Y);
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
