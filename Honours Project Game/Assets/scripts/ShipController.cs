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

        private set
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

        private set
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

        private set
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

        private set
        {
            crew_stress = value;
            UIManager.INSTANCE.UpdateCrewStressDisplay(value);
        }
    }

    private float ship_speed;
    public float Ship_Speed
    {
        get
        {
            return ship_speed;
        }

        private set
        {
            ship_speed = value;
            UIManager.INSTANCE.UpdateShipSpeedDisplay(value);
        }
    }

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
    private List<Task> currentTasks = new List<Task>();

    private const float EASY_TASK_GENERATION_TIMER = 4.0f;
    private const float MED_TASK_GENERATION_TIMER = 3.5f;
    private const float HARD_TASK_GENERATION_TIMER = 3.5f;

    private float timeTilNextTaskGeneration;// = taskGenerationTimer;
    private float current_task_timer;

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

        //had a weird thing where if i reinstantiated the clickable nodes then the raycast on click
        //would hit the clickable node before a task that was there
        //no idea why that would be
        //instead i just amended the box colliders on the clickable node, task and crew member prefabs

        currentTasks.Clear();

        tasks_for_roles.Clear();
        tasks_for_roles = null;

        tasks_for_room_type.Clear();
        tasks_for_room_type = null;

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

        roomGoDict = new TwoWayDictionary<Room>();

        #region get rooms
        List<string> roomTypes = System.Enum.GetNames(typeof(RoomType)).ToList();

        foreach (Transform child in transform)
        {
            SpriteRenderer s_rend = child.gameObject.GetComponent<SpriteRenderer>();

            RoomInfo r_info;
            r_info.X = child.position.x;
            r_info.Y = child.position.y;
            r_info.width = s_rend.size.x;
            r_info.height = s_rend.size.y;

            int randomRoomTypeIndex = Random.Range(0, roomTypes.Count);

            Room _room
                = new Room(
                    (RoomType)System.Enum.Parse(typeof(RoomType), roomTypes[randomRoomTypeIndex], false),
                    r_info
                    );

            roomTypes.RemoveAt(randomRoomTypeIndex);

            _room.AddTaskCallBack += AddTask;
            _room.RemoveTaskCallBack += RemoveTask;

            child.gameObject.name = _room.ToString();

            //change colour of room sprites

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

#if DEBUG
        tasks_for_room_type = XmlDataLoader.GetTasksForRoomType(@"Assets/xml files/tasks_for_room_type.xml");
        tasks_for_roles = XmlDataLoader.GetTasksForRoles(@"Assets/xml files/tasks_for_roles.xml");
#else
        tasks_for_room_type = XmlDataLoader.GetTasksForRoomType(@"xml files/tasks_for_room_type.xml");
        tasks_for_roles = XmlDataLoader.GetTasksForRoles(@"xml files/tasks_for_roles.xml");
#endif

        switch (GameController.INSTANCE.Current_Game_Difficulty)
        {
            case GameDifficulty.EASY:

                Hull_Integrity = 100.0f;
                Shield_Capacity = 100.0f;
                Life_Support_Efficiency = 100.0f;
                Crew_Stress = 0.0f;
                current_task_timer = EASY_TASK_GENERATION_TIMER;

                break;
            case GameDifficulty.MEDIUM:

                Hull_Integrity = 60.0f;
                Shield_Capacity = 60.0f;
                Life_Support_Efficiency = 60.0f;
                Crew_Stress = 100.0f;
                current_task_timer = MED_TASK_GENERATION_TIMER;

                break;
            case GameDifficulty.HARD:

                Hull_Integrity = 50.0f;
                Shield_Capacity = 50.0f;
                Life_Support_Efficiency = 50.0f;
                Crew_Stress = 200.0f;
                current_task_timer = HARD_TASK_GENERATION_TIMER;

                break;
            default:
                break;
        }

        Ship_Speed = 10.0f;
        timeTilNextTaskGeneration = current_task_timer;

    }

    public Room GetRandomRoom(bool only_non_full_room)
    {
        if (only_non_full_room)
        {
            List<Room> non_full_rooms = new List<Room>();

            foreach (Room rm in roomGoDict.GetFs())
            {
                if (!rm.IsFull())
                {
                    non_full_rooms.Add(rm);
                }
            }

            if (non_full_rooms.Count == 0)
                return null;

            return non_full_rooms[Random.Range(0, non_full_rooms.Count)];
        }
        else
        {
            return roomGoDict.GetFs()[Random.Range(0, roomGoDict.GetFs().Length)];
        }
    }

    public Room GetRoom(RoomType type)
    {
        foreach (Room r in roomGoDict.GetFs())
        {
            if (r.Room_Type == type && !r.IsFull())
            {
                return r;
            }
        }

        return null;
    }

    public void HighlightTask(Task t, bool isHighlighted)
    {
        taskGoDict.GetGO(t).GetComponent<cakeslice.Outline>().enabled = isHighlighted;
    }

    public Node GetRandomNodeInRoom(Room rm, bool all_nodes)
    {
        List<Node> roomNodes = ship_graph.GetNodesInRoom(rm, all_nodes);

        if (roomNodes.Count > 0)
            return roomNodes[Random.Range(0, roomNodes.Count)];
        else
            return new Node(null, null);
    }

    public void ChangeShipSpeed(float deltaSpeed)
    {
        Ship_Speed += deltaSpeed;

        if (Ship_Speed <= 0.0f)
        {
            Ship_Speed = 1.0f; //!!!!!NOTE!!!!!! - if ship speed is zero something is wrong
        }
        else if (Ship_Speed > 50.0f)
        {
            Ship_Speed = 50.0f; //todo fix all the hard coded values
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        if (timeTilNextTaskGeneration > 0f)
        {
            timeTilNextTaskGeneration -= Time.deltaTime;
        }
        else
        {
            timeTilNextTaskGeneration = current_task_timer;

            #region generate task in random room

            Room rm = GetRandomRoom(true);

            List<TaskType> task_type_list = new List<TaskType>();
            tasks_for_room_type.TryGetValue(rm.Room_Type, out task_type_list);

            TaskType task_type = task_type_list[Random.Range(0, task_type_list.Count)];

            AddTask(rm, task_type);

            #endregion
        }

        foreach (Task t in currentTasks)
        {
            t.OnTick(Time.deltaTime);
        }
    }

    public Room GetRoomByTaskType(TaskType type)
    {
        foreach (RoomType r_type in tasks_for_room_type.Keys)
        {
            if (tasks_for_room_type[r_type].Contains(type))
            {
                // AddTask(GetRoom(r_type), type);
                return GetRoom(r_type);
            }
        }
        return null;
    }

    public Dictionary<CrewMemberRole, List<TaskType>> GetTasksForRoles()
    {
        return tasks_for_roles;
    }

    public void AddTask(Room room, TaskType type)
    {
        if (!tasks_for_room_type[room.Room_Type].Contains(type))
            return;

        if (room == null)
            return;

        Node n = GetRandomNodeInRoom(room, false);

        if (n.IsNull())
            return;

        int workNeeded = 1;

        switch (GameController.INSTANCE.Current_Game_Difficulty)
        {
            case GameDifficulty.EASY:
                workNeeded = Random.Range(2, 6);
                break;
            case GameDifficulty.MEDIUM:
                workNeeded = Random.Range(2, 7);
                break;
            case GameDifficulty.HARD:
                workNeeded = Random.Range(2, 7);
                break;
            default:
                break;
        }

        Task task = new Task(type, workNeeded, room, n);

        ship_graph.AddTaskToNode(n, task);

        room.AddTask(task, n);
    }

    public void OnNodeClick(GameObject n_go)
    {
        Node clickedNode = clickableNodesGoDict.GetfType(n_go);

        if (!clickedNode.IsNull() && CrewController.INSTANCE.Selected_Crew_Member != null)
        {
            if (ship_graph.SetStartAndEnd(CrewController.INSTANCE.Selected_Crew_Member.GetPrevNode(), clickedNode))
            {
                CrewController.INSTANCE.Selected_Crew_Member.SetPath(ship_graph.FindPath().ToList());
            }
            else
                Debug.LogError("Could not set path for " + CrewController.INSTANCE.Selected_Crew_Member.Crew_Member_Name);
        }
    }

    public void OnTaskClick(GameObject t_go)
    {
        Task t = taskGoDict.GetfType(t_go);

        if (t != null && CrewController.INSTANCE.Selected_Crew_Member != null)
        {
            //find path from player current node to task node

            //check if crew member can do task
            List<TaskType> tasks = new List<TaskType>();

            tasks_for_roles.TryGetValue(CrewController.INSTANCE.Selected_Crew_Member.Crew_Member_Role, out tasks);

            if (CrewController.INSTANCE.Selected_Crew_Member.Crew_Member_Role == CrewMemberRole.CAPTAIN || tasks.Contains(t.Task_Type))
            {
                if (ship_graph.SetStartAndEnd(CrewController.INSTANCE.Selected_Crew_Member.GetPrevNode(), t))
                {
                    CrewController.INSTANCE.Selected_Crew_Member.SetPathAndTask(ship_graph.FindPath().ToList(), t);
                }
                else
                    Debug.LogError("Could not set path for " + CrewController.INSTANCE.Selected_Crew_Member.Crew_Member_Name);
            }
            else
            {
                Debug.Log("wrong task type for crew member " + CrewController.INSTANCE.Selected_Crew_Member.Crew_Member_Name);
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

        //change colour of task sprites
        SpriteRenderer s_rend = t_go.GetComponent<SpriteRenderer>();

        switch (t.Task_Type)
        {
            case TaskType.TORPEDO_ASTEROIDS:
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
            case TaskType.MAINTAIN_COMMS:
                s_rend.color = Color.black;
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
                t.UncompletedCallBack += DecreaseShieldCapacity;
                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:
                t.UncompletedCallBack += DecreaseLifeSupportEfficiency; //to make maintain life support task more important
                t.UncompletedCallBack += DecreaseLifeSupportEfficiency;
                break;
            case TaskType.REPAIR:
                t.UncompletedCallBack += DecreaseShipHullIntegrity;
                break;
            case TaskType.TORPEDO_ASTEROIDS:
                t.UncompletedCallBack += AsteroidHittingShield;
                break;
            default:
                t.UncompletedCallBack += IncreaseCrewStress;
                break;
        }

        t.WorkedOnCallback += HighlightTask;

        taskGoDict.Add(t, t_go);
    }

    private void AsteroidHittingShield(float timeDecay)
    {
        Shield_Capacity -= timeDecay;

        if (Shield_Capacity <= 0.0f)
        {
            Shield_Capacity = 0.0f;
            DecreaseShipHullIntegrity(timeDecay);
        }
    }

    public void DecreaseShipHullIntegrity(float timeDecay)
    {
        Hull_Integrity -= timeDecay;

        if (Hull_Integrity <= 0.0f)
        {
            Hull_Integrity = 0.0f;
            GameController.INSTANCE.LostHullIntegrity();
        }
    }

    public void IncreaseShipHullIntegrity(float timeDecay)
    {
        Hull_Integrity += timeDecay;

        if (Hull_Integrity > 100.0f)
        {
            Hull_Integrity = 100.0f;
        }
    }

    public void DecreaseShieldCapacity(float timeDecay)
    {
        Shield_Capacity -= timeDecay;

        if (Shield_Capacity <= 0.0f)
        {
            Shield_Capacity = 0.0f;
        }
    }

    public void IncreaseShieldCapacity(float timeDecay)
    {
        Shield_Capacity += timeDecay;

        if (Shield_Capacity > 100.0f)
        {
            Shield_Capacity = 100.0f;
        }
    }

    public void DecreaseLifeSupportEfficiency(float timeDecay)
    {
        Life_Support_Efficiency -= timeDecay;

        if (Life_Support_Efficiency <= 0.0f)
        {
            Life_Support_Efficiency = 0.0f;
            GameController.INSTANCE.LostLifeSupport();
        }
    }

    public void IncreaseLifeSupportEfficiency(float timeDecay)
    {
        Life_Support_Efficiency += timeDecay;

        if (Life_Support_Efficiency > 100.0f)
        {
            Life_Support_Efficiency = 100.0f;
        }
    }

    public void IncreaseCrewStress(float timeDecay)
    {
        Crew_Stress += timeDecay;

        if (Crew_Stress >= 300.0f)
        {
            Crew_Stress = 300.0f;
            GameController.INSTANCE.LostSanity();
        }
    }

    public void DecreaseCrewStress(float timeDecay)
    {
        Crew_Stress -= timeDecay;

        if (Crew_Stress < 0.0f)
        {
            Crew_Stress = 0.0f;
        }
    }

    private void RemoveTask(Task t, CrewMember cm)
    {
        currentTasks.Remove(t);
        ship_graph.RemoveTaskFromNode(t.Task_Node);

        Destroy(taskGoDict.GetGO(t));

        taskGoDict.RemovefType(t);

        Debug.Log("Finished task: " + t.ToStringTaskType());

        switch (t.Task_Type)
        {
            case TaskType.CHARGE_SHIELDS:

                IncreaseShieldCapacity((t.Work * cm.Crew_Member_Level) * 1.5f);

                break;
            case TaskType.REPAIR:

                IncreaseShipHullIntegrity(t.Work * cm.Crew_Member_Level);

                break;
            case TaskType.MAINTAIN_LIFE_SUPPORT:

                IncreaseLifeSupportEfficiency(t.Work * cm.Crew_Member_Level);

                break;
            case TaskType.TORPEDO_ASTEROIDS:

                AddTask(GetRoomByTaskType(TaskType.CHARGE_SHIELDS), TaskType.CHARGE_SHIELDS);

                break;

            default:

                DecreaseCrewStress(t.Work * cm.Crew_Member_Level);

                break;
        }
    }
}
