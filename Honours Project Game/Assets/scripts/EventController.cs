using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HonsProj;
using UnityEngine.UI;

public class EventController : MonoBehaviour
{
    //prefab
    //instantiate prefab with event text data
    //options are selectables - with own script with event member???? idk
    //destroyed when an option is selected
    //game is paused with events

    public static EventController INSTANCE { get; private set; }

    private const float event_timer = 10.0f;
    private float time_left_til_event = event_timer;

    [SerializeField]
    GameObject eventPrefab;

    private GameObject current_event_go;
    private HonsProj.Event current_event;

    // Use this for initialization
    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE EVENT CONTROLLER!!!!");
            Destroy(gameObject);
        }

    }

    void Start()
    {
        GameController.INSTANCE.OnRestartGame += RestartEvents;
    }

    void RestartEvents()
    {
        Destroy(current_event_go);
        current_event = null;
        time_left_til_event = event_timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        time_left_til_event -= Time.deltaTime;

        if (time_left_til_event <= 0.0f)
        {
            //generate event
            GenerateEvent();
            time_left_til_event = event_timer;
        }
    }

    void GenerateEvent()
    {
        GameController.INSTANCE.InEvent();

        current_event_go = Instantiate(eventPrefab, FindObjectOfType<Canvas>().transform);

        current_event = new HonsProj.Event("Crew levels up!");

        current_event.SetEventText("Level up the crew!");

        current_event.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        current_event.AddChoice(HonsProj.EventType.CONTINUE, LevelUpCrew, false, "+ (each crew member gains one level)");

        current_event_go.GetComponent<EventInfoSetter>().SetEventInfo(current_event);
    }

    void DestroyEvent(/*params object[] args*/)
    {
        Destroy(current_event_go);
        current_event = null;
        GameController.INSTANCE.FinishedEvent();
    }

    public void LevelUpCrew()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            cm.LevelUp();

            //Debug.Log(cm.Crew_Member_Name + " leveled up to " + cm.Crew_Member_Level + "!");
        }
    }

    public void LevelUpRandomCrewMember()
    {
        CrewController.INSTANCE.GetRandomCrewMember().LevelUp();
    }

    public void LevelUpCrewMember(CrewMemberRole role)
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            if (cm.Crew_Member_Role == role && cm.Crew_Member_Level < 10)
            {
                cm.LevelUp();
                break;
            }
        }
    }

    public void LevelDownCrew()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            cm.LevelDown();
        }
    }

    public void LevelDownRandomCrewMember()
    {
        CrewController.INSTANCE.GetRandomCrewMember().LevelDown();
    }

    public void LevelDownCrewMember(CrewMemberRole role)
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            if (cm.Crew_Member_Role == role && cm.Crew_Member_Level > 1)
            {
                cm.LevelDown();
                break;
            }
        }
    }

    public void GenerateTask(TaskType task_type)
    {
        Room r = ShipController.INSTANCE.GetRoomByTaskType(task_type);

        if (r != null)
        {
            ShipController.INSTANCE.AddTask(r, task_type);
        }
    }

    public void IncreaseShipHullIntegrity()
    {
        ShipController.INSTANCE.IncreaseShipHullIntegrity(20.0f);
    }

    public void DecreaseShipHullIntegrity()
    {
        ShipController.INSTANCE.DecreaseShipHullIntegrity(20.0f);
    }

    public void IncreaseShieldCapacity()
    {
        ShipController.INSTANCE.IncreaseShieldCapacity(20.0f);
    }

    public void DecreaseShieldCapacity()
    {
        ShipController.INSTANCE.DecreaseShieldCapacity(20.0f);
    }

    public void IncreaseLifeSupportEfficiency()
    {
        ShipController.INSTANCE.IncreaseLifeSupportEfficiency(20.0f);
    }

    public void DecreaseLifeSupportEfficiency()
    {
        ShipController.INSTANCE.DecreaseLifeSupportEfficiency(20.0f);
    }

    public void DecreaseCrewStress()
    {
        ShipController.INSTANCE.DecreaseCrewStress(20.0f);
    }

    public void IncreaseCrewStress()
    {
        ShipController.INSTANCE.IncreaseCrewStress(20.0f);
    }

    public void IncreaseShipSpeed()
    {
        ShipController.INSTANCE.ChangeShipSpeed(5.0f);
    }

    public void DecreaseShipSpeed()
    {
        ShipController.INSTANCE.ChangeShipSpeed(-5.0f);
    }


}
