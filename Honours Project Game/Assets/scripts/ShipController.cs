using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using HonsProj;

public class ShipController : MonoBehaviour
{
    public static ShipController INSTANCE { get; protected set; }
    
    private float hull_integrity;
    public float Hull_Integrity
    {
        get
        {
            return hull_integrity;
        }

        set
        {
            hull_integrity = value;
            UIManager.INSTANCE.UpdateHullIntegrityDisplay(value);
        }
    }

    private float shield_capacity;
    public float Shield_Capacity
    {
        get
        {
            return shield_capacity;
        }

        set
        {
            shield_capacity = value;
            UIManager.INSTANCE.UpdateShieldCapacityDisplay(value);
        }
    }

    private float life_support_efficiency;
    public float Life_Support_Efficiency
    {
        get
        {
            return life_support_efficiency;
        }

        set
        {
            life_support_efficiency = value;
            UIManager.INSTANCE.UpdateLifeSupportEfficiencyDisplay(value);
        }
    }
    
    private float crew_stress;
    public float Crew_Stress
    {
        get
        {
            return crew_stress;
        }

        set
        {
            crew_stress = value;
            UIManager.INSTANCE.UpdateCrewStressDisplay(value);
        }
    }

    public float Ship_Speed { get; private set; }
    
    [SerializeField]
    GameObject RoomPrefab;
    [SerializeField]
    GameObject TaskPrefab;
    [SerializeField]
    GameObject NodePrefab;
    [SerializeField]
    GameObject NodeParent;

    private TwoWayDictionary<Node> clickableNodesGoDict;
    private TwoWayDictionary<Room> roomGoDict;
    private TwoWayDictionary<Task> taskGoDict = new TwoWayDictionary<Task>();
    private List<Task> currentTasks = new List<Task>(); //note needed? since taskGoDict.GetFs() returns the same

    private const float taskGenerationTimer = 4.0f;
    private float timeTilNextTaskGeneration = taskGenerationTimer;

    private Graph ship_graph;

    private Dictionary<RoomType, List<TaskType>> tasks_for_room_type;
    private Dictionary<CrewMemberRole, List<TaskType>> tasks_for_roles;

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

        //DontDestroyOnLoad(gameObject);

    }

    void RestartShip()
    {
        //clear things
        taskGoDict.Clear();

        roomGoDict = null;

        ship_graph.Clear();
        ship_graph = null;

        clickableNodesGoDict.Clear();
        clickableNodesGoDict = null;

        currentTasks.Clear();

        tasks_for_roles.Clear();
        tasks_for_roles = null;

        tasks_for_room_type.Clear();
        tasks_for_room_type = null;

        timeTilNextTaskGeneration = taskGenerationTimer;
        
        InitialiseShip();
    }

    void Start()
    {
        GameController.INSTANCE.OnRestartGame += RestartShip;

        InitialiseShip();

    }

    void InitialiseShip()
    {
        //instantiate prefabs and create rooms
        //rooms then start periodically spawning tasks

        //taskGoDict = new TwoWayDictionary<Task>();

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

        //currentTasks = new List<Task>();

#if DEBUG
        tasks_for_room_type = XmlDataLoader.GetTasksForRoomType(@"Assets/xml files/tasks_for_room_type.xml");
        tasks_for_roles = XmlDataLoader.GetTasksForRoles(@"Assets/xml files/tasks_for_roles.xml");
#else
        tasks_for_room_type = XmlDataLoader.GetTasksForRoomType(@"xml files/tasks_for_room_type.xml");
        tasks_for_roles = XmlDataLoader.GetTasksForRoles(@"xml files/tasks_for_roles.xml");
#endif
        
        Hull_Integrity = 100.0f;
        Shield_Capacity = 100.0f;
        Life_Support_Efficiency = 100.0f;
        Crew_Stress = 0.0f;
        Ship_Speed = 10.0f;
    }

    public Room GetRandomRoom()
    {
        GameObject go = roomGoDict.GetGOs()[Random.Range(0, roomGoDict.GetGOs().Length - 1)];

        return roomGoDict.GetfType(go);
    }

    public Node GetRandomNodeInRoom(Room rm)
    {
        return ship_graph.GetNodesInRoom(rm)[Random.Range(0, ship_graph.GetNodesInRoom(rm).Count)];
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

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

            //todo potentially move to a separate controller script
#region generate task in random room
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
    }

    public void OnNodeClick(GameObject n_go)
    {
        Node clickedNode = clickableNodesGoDict.GetfType(n_go);

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
                    Debug.LogError("Could not set path for " + crew_member.Crew_Member_Name);
            }
        }
    }

    public void OnTaskClick(GameObject t_go)
    {
        Task t = taskGoDict.GetfType(t_go);

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
        Hull_Integrity -= timeDecay;

        if (Hull_Integrity <= 0.0f)
        {
            Hull_Integrity = 0.0f;
            GameController.INSTANCE.LostHullIntegrity();
        }
    }

    private void DecreaseShieldCapacity(float timeDecay)
    {
        Shield_Capacity -= timeDecay;

        if (Shield_Capacity <= 0.0f)
        {
            Shield_Capacity = 0.0f;
        }
    }

    private void DecreaseLifeSupportEfficiency(float timeDecay)
    {
        Life_Support_Efficiency -= timeDecay;

        if (Life_Support_Efficiency <= 0.0f)
        {
            Life_Support_Efficiency = 0.0f;
            GameController.INSTANCE.LostLifeSupport();
        }
    }

    private void IncreaseStress(float timeDecay)
    {
        Crew_Stress += timeDecay;

        if (Crew_Stress >= 500.0f)
        {
            Crew_Stress = 500.0f;
            GameController.INSTANCE.LostSanity();
        }
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

                Shield_Capacity += t.Work * 2;
                if (Shield_Capacity > 100.0f)
                {
                    Shield_Capacity = 100.0f;
                }

                break;
            case TaskType.REPAIR:

                Hull_Integrity += t.Work * 2;
                if (Hull_Integrity > 100.0f)
                {
                    Hull_Integrity = 100.0f;
                }

                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:

                Life_Support_Efficiency += t.Work * 2;
                if (Life_Support_Efficiency > 100.0f)
                {
                    Life_Support_Efficiency = 100.0f;
                }

                break;
            default:

                Crew_Stress -= t.Work * 2;
                if (Crew_Stress < 0.0f)
                {
                    Crew_Stress = 0.0f;
                }

                break;
        }
    }
}
