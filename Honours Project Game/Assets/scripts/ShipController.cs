using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using HonsProj;

public class ShipController : MonoBehaviour
{
    //rooms are 3x3
    //increments of 0.5
    //tasks - pure c#? have method OnTick()? that is called from TaskController obj?

    //make in editor snappable ship? - LATER!!!!

    public static ShipController INSTANCE { get; protected set; }

    public float HullIntegrity { get; private set; }
    public float ShieldCapacity { get; private set; }
    public float LifeSupportEfficiency { get; private set; }

    //todo serialize all the fields
    [SerializeField]
    GameObject RoomPrefab;
    [SerializeField]
    GameObject TaskPrefab;
    [SerializeField]
    GameObject NodePrefab;
    [SerializeField]
    GameObject NodeParent;

    private TwoWayDictionary<Node> clickableNodesGoDict;

    //temp - use a UI manager script
    public Text HullIntegrityDisplay;
    public Text ShieldCapacityDisplay;
    public Text LifeSupportEfficiencyDisplay;
    public Text ShipStressDisplay;

    float ShipStress = 0.0f; //temp

    //public GameObject PlayerPrefab;

    private TwoWayDictionary<Room> roomGoDict;

    private TwoWayDictionary<Task> taskGoDict;
    private List<Task> currentTasks; //note needed? since taskGoDict.GetFs() returns the same

    private const float taskGenerationTimer = 4.0f;
    private float timeTilNextTaskGeneration = taskGenerationTimer;

    private Graph ship_graph;

    private Dictionary<RoomType, List<TaskType>> tasks_for_room_type;
    private Dictionary<CrewMemberRole, List<TaskType>> tasks_for_roles;

    //mark nodes in scene with empty game object
    //get game objects and add x,y to list of nodes
    //edges??? - draw with line renderers??? then do the same thing 

    // Use this for initialization
    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE SHIP CONTROLLER!!!!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //instantiate prefabs and create rooms
        //rooms then start periodically spawning tasks

        taskGoDict = new TwoWayDictionary<Task>();

        roomGoDict = new TwoWayDictionary<Room>();

        //temp - get room game objects already instantiated
        #region get rooms
        foreach (Transform child in transform)
        {
            SpriteRenderer s_rend = child.gameObject.GetComponent<SpriteRenderer>();

            RoomInfo r_info;
            r_info.X = child.position.x;
            r_info.Y = child.position.y;
            r_info.width = s_rend.size.x;
            r_info.height = s_rend.size.y;

            Room _room
                = new Room(
                    (RoomType)Random.Range(0, System.Enum.GetNames(typeof(RoomType)).Length), //random room type
                    r_info
                    );

            _room.AddTaskCallBack += AddTask;
            _room.RemoveTaskCallBack += RemoveTask;

            child.gameObject.name = _room.ToString();

            //change colour of room sprites - temp - will have different sprites

            switch (_room.Room_Type)
            {
                case RoomType.NAVIGATION:
                    s_rend.color = Color.magenta;
                    break;
                case RoomType.TORPEDO_LAUNCHER:
                    s_rend.color = Color.blue;
                    break;
                case RoomType.LIFE_SUPPORT:
                    s_rend.color = Color.green;
                    break;
                case RoomType.SHIELD_CHARGER:
                    s_rend.color = Color.red;
                    break;
                case RoomType.COMMS:
                    s_rend.color = Color.yellow;
                    break;
                case RoomType.MED_BAY:
                    s_rend.color = Color.cyan;
                    break;
                default:
                    break;
            }

            //todo replace with text mesh
            TextMesh rm_text = child.gameObject.GetComponentInChildren<TextMesh>();

            rm_text.text = _room.ToString();

            roomGoDict.Add(_room, child.gameObject);
        }

        Debug.Log("found all rooms on ship: " + roomGoDict.GetCount() + " rooms");
        #endregion

        ship_graph = GraphPlotter.CreateGraph(roomGoDict.GetFs().ToList());

        #region create clickable nodes

        clickableNodesGoDict = new TwoWayDictionary<Node>();

        foreach (Node node in ship_graph.GetNodes())
        {
            GameObject nodeGo = Instantiate(NodePrefab, NodeParent.transform);
            nodeGo.transform.position = new Vector3((float)node.X, (float)node.Y, 0);
            clickableNodesGoDict.Add(node, nodeGo);
        }

        #endregion

        currentTasks = new List<Task>();

        tasks_for_room_type = XmlDataLoader.GetTasksForRoomType(@"Assets/xml files/tasks_for_room_type.xml");
        tasks_for_roles = XmlDataLoader.GetTasksForRoles(@"Assets/xml files/tasks_for_roles.xml");

        //set player in random room
        //Room player_room = GetRandomRoom();
        //Player.INSTANCE.SetPlayerPos(player_room.Room_Info.X, player_room.Room_Info.Y);

        //Node playerpos;// Player.INSTANCE.GetPlayerPos();
        //playerpos.X = 0.0f; //temp
        //playerpos.Y = 0.0f; //temp

        //Debug.Log("Player pos: " + playerpos.X + ", " + playerpos.Y);

        HullIntegrity = 100.0f;
        ShieldCapacity = 100.0f;
        LifeSupportEfficiency = 100.0f;
    }

    //private Node[] GetRandomPath()
    //{
    //    ship_graph.SetStartAndEnd(GetRandomRoom(), GetRandomRoom());

    //    return ship_graph.FindPath();
    //}

    public Room GetRandomRoom()
    {
        GameObject go = roomGoDict.GetGOs()[Random.Range(0, roomGoDict.GetGOs().Length - 1)];

        return roomGoDict.GetfType(go);
    }

    // Update is called once per frame
    void Update()
    {
        //todo move this to room? and loop through rooms to OnTick()?
        //  is it alright if there are two separate loops for rooms and tasks??
        //  i guess because it would allow for the separation of dealing with tasks
        //  and rooms generating tasks??
        if (timeTilNextTaskGeneration > 0f)
        {
            timeTilNextTaskGeneration -= Time.deltaTime;
        }
        else
        {
            timeTilNextTaskGeneration = taskGenerationTimer;

            #region generate task in random room
            //temp - only one task per room
            //todo when completing task, cooldown on room before a task can respawn
            List<GameObject> r_go_w_no_task = new List<GameObject>();
            foreach (GameObject r_go in roomGoDict.GetGOs())
            {
                Room temp_rm = roomGoDict.GetfType(r_go);
                if (temp_rm.NumberOfTasks() < 2)
                    r_go_w_no_task.Add(r_go);
            }

            if (r_go_w_no_task.Count > 0)
            {
                GameObject random_room_w_no_task = r_go_w_no_task[Random.Range(0, r_go_w_no_task.Count)];

                Room rm = roomGoDict.GetfType(random_room_w_no_task);

                List<TaskType> task_type_list = new List<TaskType>();
                tasks_for_room_type.TryGetValue(rm.Room_Type, out task_type_list);

                TaskType task_type = task_type_list[Random.Range(0, task_type_list.Count)];


                Node n = ship_graph.GetNodesInRoom(rm)[Random.Range(0, ship_graph.GetNodesInRoom(rm).Count)];

                //temp random task
                Task task =
                    new Task(
                        //(TaskType)Random.Range(0, System.Enum.GetNames(typeof(TaskType)).Length), //random task type
                        task_type,
                        Random.Range(1, 4),                                                       //random work needed
                        rm,                                                                         //the random room
                        n
                        );

                ship_graph.AddTaskToNode(n, task);
                
                rm.AddTask(task, n);
            }
            #endregion
        }

        foreach (Task t in currentTasks)
        {
            t.OnTick(Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0)) //temp - handling this here
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out hit))
            {
                GameObject go = hit.transform.gameObject;

                Debug.Log("clicked on " + go.name);

                Task t = taskGoDict.GetfType(go);

                if (t != null)
                {
                    //find path from player current node to task node
                    CrewMember crew_member;

                    if (CrewController.INSTANCE.Selected_Crew_Member == null)
                    {
                        crew_member = CrewController.INSTANCE.GetRandomCrewMember();
                    }
                    else
                    {
                        crew_member = CrewController.INSTANCE.Selected_Crew_Member;
                    }

                    //check if crew member can do task
                    List<TaskType> tasks = new List<TaskType>();

                    tasks_for_roles.TryGetValue(crew_member.Crew_Member_Role, out tasks);

                    if (tasks.Contains(t.Task_Type))
                    {
                        if (ship_graph.SetStartAndEnd(crew_member.GetPrevNode(), t))
                            crew_member.SetPathAndTask(ship_graph.FindPath().ToList(), t);
                        else
                            Debug.LogError("Could not set path for " + crew_member.Crew_Member_Name);
                    }
                    else
                    {
                        Debug.Log("wrong task type for crew member " + crew_member.Crew_Member_Name);
                    }
                }
                else
                {
                    Node clickedNode = clickableNodesGoDict.GetfType(go);

                    if (!clickedNode.IsNull())
                    {
                        //Debug.Log("clicked node");
                        CrewMember crew_member;

                        if (CrewController.INSTANCE.Selected_Crew_Member != null)
                        {
                            crew_member = CrewController.INSTANCE.Selected_Crew_Member;

                            if (ship_graph.SetStartAndEnd(crew_member.GetPrevNode(), clickedNode))
                                crew_member.SetPath(ship_graph.FindPath().ToList());
                            else
                                Debug.LogError("Could not set path for "+crew_member.Crew_Member_Name);
                        }
                    }
                }
            }
        }

    }

    public bool IsTaskInList(Task t)
    {
        return currentTasks.Contains(t);
    }

    private void AddTask(Task t, Node n)
    {
        currentTasks.Add(t);

        // instantiate task display prefab

        GameObject go = roomGoDict.GetGO(t.Parent_Room);
        GameObject t_go = Instantiate(TaskPrefab, go.transform, false);

        t_go.name = t.Task_Type.ToString();
        t_go.transform.position = new Vector3((float)n.X, (float)n.Y, 0.0f);

        //change colour of task sprites - temp - will have different sprites
        SpriteRenderer s_rend = t_go.GetComponent<SpriteRenderer>();

        switch (t.Task_Type)
        {
            case TaskType.LAUNCH_TORPEDO:
                s_rend.color = Color.magenta;
                break;
            case TaskType.REPAIR:
                s_rend.color = Color.blue;
                break;
            case TaskType.STEER_SHIP:
                s_rend.color = Color.green;
                break;
            case TaskType.CHARGE_SHIELDS:
                s_rend.color = Color.red;
                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:
                s_rend.color = Color.yellow;
                break;
            case TaskType.HEAL_CREW_MEMBER:
                s_rend.color = Color.cyan;
                break;
            default:
                break;
        }

        //todo replace with text mesh
        TextMesh t_text = t_go.GetComponentInChildren<TextMesh>();

        t_text.text = t.ToStringTaskType();

        //switch case based on task type for which stress cb
        //maybe have a bool prop in this script
        //that is checked every update on whether or not to 
        //increase or decrease ship aspects
        //maybe enum to allow different ways to change ship aspects????

        switch (t.Task_Type)
        {
            case TaskType.CHARGE_SHIELDS:
                t.IncreaseStressCallBack += DecreaseShieldCapacity;
                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:
                t.IncreaseStressCallBack += DecreaseLifeSupportEfficiency;
                break;
            case TaskType.REPAIR:
                //if (t.Parent_Room.Room_Type == RoomType.LIFE_SUPPORT)
                //t.IncreaseStressCallBack += DecreaseLifeSupportEfficiency;
                //else 
                t.IncreaseStressCallBack += DecreaseShipHullIntegrity;
                break;
            default:
                t.IncreaseStressCallBack += IncreaseStress;
                break;
        }

        taskGoDict.Add(t, t_go);
    }

    private void DecreaseShipHullIntegrity(float timeDecay)
    {
        HullIntegrity -= timeDecay;

        if (HullIntegrity < 0.0f)
        {
            HullIntegrity = 0.0f;
        }

        //update text
        HullIntegrityDisplay.text = "Hull Integrity: " + HullIntegrity.ToString("0") + "%";
    }

    private void DecreaseShieldCapacity(float timeDecay)
    {
        ShieldCapacity -= timeDecay;

        if (ShieldCapacity < 0.0f)
        {
            ShieldCapacity = 0.0f;
        }

        //update text
        ShieldCapacityDisplay.text = "Shield Capacity: " + ShieldCapacity.ToString("0") + "%";
    }

    private void DecreaseLifeSupportEfficiency(float timeDecay)
    {
        LifeSupportEfficiency -= timeDecay;

        if (LifeSupportEfficiency < 0.0f)
        {
            LifeSupportEfficiency = 0.0f;
        }

        //update text
        LifeSupportEfficiencyDisplay.text = "Life Support Efficiency: " + LifeSupportEfficiency.ToString("0") + "%";
    }

    private void IncreaseStress(float timeDecay)
    {
        ShipStress += timeDecay;
        ShipStressDisplay.text = "Ship stress: " + ShipStress.ToString("0.0");
    }

    private void RemoveTask(Task t)
    {
        currentTasks.Remove(t);
        ship_graph.RemoveTaskFromNode(t.Task_Node);

        Destroy(taskGoDict.GetGO(t));

        taskGoDict.RemovefType(t);

        //todo remove callbacks????
        switch (t.Task_Type)
        {
            case TaskType.CHARGE_SHIELDS:

                ShieldCapacity += t.Work * 2;
                if (ShieldCapacity > 100.0f)
                {
                    ShieldCapacity = 100.0f;
                }
                ShieldCapacityDisplay.text = "Shield Capacity: " + ShieldCapacity.ToString("0") + "%";

                break;
            case TaskType.REPAIR:

                HullIntegrity += t.Work * 2;
                if (HullIntegrity > 100.0f)
                {
                    HullIntegrity = 100.0f;
                }
                HullIntegrityDisplay.text = "Hull Integrity: " + HullIntegrity.ToString("0") + "%";

                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:

                LifeSupportEfficiency += t.Work * 2;
                if (LifeSupportEfficiency > 100.0f)
                {
                    LifeSupportEfficiency = 100.0f;
                }
                LifeSupportEfficiencyDisplay.text = "Life Support Efficiency: " + LifeSupportEfficiency.ToString("0") + "%";

                break;
            default:

                ShipStress -= t.Work * 2;
                if (ShipStress < 0.0f)
                {
                    ShipStress = 0.0f;
                }
                ShipStressDisplay.text = "Ship Stress: " + ShipStress.ToString("0.0");

                break;
        }
    }
}
