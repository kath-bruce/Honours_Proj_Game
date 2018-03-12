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

    private List<HonsProj.Event> negative_events_low = new List<HonsProj.Event>();
    private List<HonsProj.Event> negative_events_med = new List<HonsProj.Event>();
    private List<HonsProj.Event> negative_events_high = new List<HonsProj.Event>();

    private List<HonsProj.Event> positive_events_low = new List<HonsProj.Event>();
    private List<HonsProj.Event> positive_events_med = new List<HonsProj.Event>();
    private List<HonsProj.Event> positive_events_high = new List<HonsProj.Event>();

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

        AddPositiveEvents();
        AddNegativeEvents();
    }

    private void AddPositiveEvents()
    {
        //-------------------------------low---------------------------------
        #region low positive events
        HonsProj.Event randomLevelUp = new HonsProj.Event("Crew member leveled up!", "A crew member gained a level, allowing them to complete tasks faster!");
        randomLevelUp.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        randomLevelUp.AddChoice(HonsProj.EventType.CONTINUE, () =>
        {
            CrewMember cm = CrewController.INSTANCE.GetRandomCrewMember();
            cm.LevelUp(); //todo have a pop up on who leveled up
        }, false);

        positive_events_low.Add(randomLevelUp);

        //

        HonsProj.Event wormholeFound = new HonsProj.Event("Wormhole found!", "Do you want to go through it or leave it be? " +
            "You will get closer to earth but will sustain <color=red>20 hull damage</color>");

        wormholeFound.AddChoice(HonsProj.EventType.FIRST_CHOICE, DestroyEvent, false);
        wormholeFound.AddChoice(HonsProj.EventType.FIRST_CHOICE, () =>
        {
            DecreaseDistanceToEarth();
            DecreaseShipHullIntegrity();
        }, false, "1. Go through wormhole (travel 500 LY)");

        wormholeFound.AddChoice(HonsProj.EventType.SECOND_CHOICE, DestroyEvent, false, "2. Leave it be");

        positive_events_low.Add(wormholeFound);
        
        #endregion

        //-------------------------medium------------------------------------
        #region medium positive events
        HonsProj.Event shipSpeedIncrease = new HonsProj.Event("Ship Engine Improved!",
            "A crew member has managed to improve the engine and the ship speed has increased!");

        shipSpeedIncrease.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        shipSpeedIncrease.AddChoice(HonsProj.EventType.CONTINUE, IncreaseShipSpeed, false, "+ (Ship speed has increased by 5)");

        positive_events_med.Add(shipSpeedIncrease);

        //

        HonsProj.Event resetShipAspectChoice = new HonsProj.Event("Extra components found!", "Someone has found some enough extra components to either repair" +
            " the hull by 30% or the life support system by 30%");

        resetShipAspectChoice.AddChoice(HonsProj.EventType.FIRST_CHOICE, DestroyEvent, false);
        resetShipAspectChoice.AddChoice(HonsProj.EventType.FIRST_CHOICE, () =>
        {
            ShipController.INSTANCE.IncreaseShipHullIntegrity(30.0f);
        }, false, "1. Repair hull by 30%");

        resetShipAspectChoice.AddChoice(HonsProj.EventType.SECOND_CHOICE, DestroyEvent, false);
        resetShipAspectChoice.AddChoice(HonsProj.EventType.SECOND_CHOICE, () =>
        {
            ShipController.INSTANCE.IncreaseLifeSupportEfficiency(30.0f);
        }, false, "2. Repair life support system by 30%");

        positive_events_med.Add(resetShipAspectChoice);
        
        #endregion

        //-------------------------high----------------------------------------
        #region highly positive events
        HonsProj.Event wholeCrewLevelsUp = new HonsProj.Event("Crew levels up!", "Level up the crew!");

        wholeCrewLevelsUp.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        wholeCrewLevelsUp.AddChoice(HonsProj.EventType.CONTINUE, LevelUpCrew, false, "+ (each crew member gains one level)");

        positive_events_high.Add(wholeCrewLevelsUp);

        //

        HonsProj.Event partyOrWork = new HonsProj.Event("Party or work","The crew want to have a party but also " +
            "think that they could do some work around the ship - what should they do?");

        partyOrWork.AddChoice(HonsProj.EventType.FIRST_CHOICE, DestroyEvent, false);
        partyOrWork.AddChoice(HonsProj.EventType.FIRST_CHOICE, DecreaseCrewStress, false, "1. Have a party (-20 stress)");

        partyOrWork.AddChoice(HonsProj.EventType.SECOND_CHOICE, DestroyEvent, false);
        partyOrWork.AddChoice(HonsProj.EventType.SECOND_CHOICE, () =>
        {
            ShipController.INSTANCE.IncreaseLifeSupportEfficiency(10.0f);
            ShipController.INSTANCE.IncreaseShieldCapacity(10.0f);
            ShipController.INSTANCE.IncreaseShipHullIntegrity(10.0f);
        }, false, "2. Work on ship (+10 to hull, shields and life support)");

        positive_events_high.Add(partyOrWork);
        #endregion
    }

    private void AddNegativeEvents()
    {
        //-------------------------------low---------------------------------
        #region low negative events
        HonsProj.Event repairNeeded = new HonsProj.Event("Repair Needed!", "Something has malfunctioned and needs repair");
        repairNeeded.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");

        repairNeeded.AddChoice(HonsProj.EventType.CONTINUE, () =>
        {
            Room rm = ShipController.INSTANCE.GetRandomRoom(true);
            ShipController.INSTANCE.AddTask(rm, TaskType.REPAIR);
        }, false, "+ (REPAIR task is generated)");

        negative_events_low.Add(repairNeeded);

        //

        HonsProj.Event shieldProblem = new HonsProj.Event("Problem with shields!", "An issue has been found with the shield" +
            " - either someone has to go outside the ship to fix it, causing stress, or the shield capacity will take a hit");

        shieldProblem.AddChoice(HonsProj.EventType.FIRST_CHOICE, DestroyEvent, false);
        shieldProblem.AddChoice(HonsProj.EventType.FIRST_CHOICE, IncreaseCrewStress, false, "1. send someone to fix the shield (+20 stress)");

        shieldProblem.AddChoice(HonsProj.EventType.SECOND_CHOICE, DestroyEvent, false);
        shieldProblem.AddChoice(HonsProj.EventType.SECOND_CHOICE, () =>
        {
            ShipController.INSTANCE.DecreaseShieldCapacity(10.0f);
        }, false, "2. ignore the problem (-10 shields)");

        negative_events_low.Add(shieldProblem);
        #endregion

        //-------------------------medium------------------------------------
        #region medium negative events
        HonsProj.Event fightBrokeOut = new HonsProj.Event("Fight broke out!", "Two crew members have had a fight and will need to be healed and leveled down");

        fightBrokeOut.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");

        fightBrokeOut.AddChoice(HonsProj.EventType.CONTINUE, () =>
        {
            Room med_bay = ShipController.INSTANCE.GetRoomByTaskType(TaskType.HEAL_CREW_MEMBER);

            CrewMember cm1 = CrewController.INSTANCE.GetRandomCrewMember();
            CrewMember cm2;

            List<CrewMember> crew = CrewController.INSTANCE.GetCrewMembers();
            crew.Remove(cm1);

            cm2 = crew[Random.Range(0, crew.Count)];

            cm1.LevelDown();
            cm2.LevelDown();

            ShipController.INSTANCE.AddTask(med_bay, TaskType.HEAL_CREW_MEMBER);
            ShipController.INSTANCE.AddTask(med_bay, TaskType.HEAL_CREW_MEMBER);
        }, false, "+ (HEAL CREW MEMBERS and level down 2 crew members)");

        negative_events_med.Add(fightBrokeOut);

        //

        HonsProj.Event asteroidHit = new HonsProj.Event("Mega Asteroid incoming!", "A mega asteroid is on course to hit the ship! "
            + "How much damage we take depends on how much the shields are charged");

        asteroidHit.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        asteroidHit.AddChoice(HonsProj.EventType.CONTINUE, () =>
        {
            if (GameController.INSTANCE.Current_Game_State != GameState.EVENT)
                return;

            ShipController.INSTANCE.DecreaseShipHullIntegrity((1 - (ShipController.INSTANCE.Shield_Capacity / 100.0f)) * 50.0f);
        }, false, "+ (take hull damage based on shield capacity - MAX 50)");

        negative_events_med.Add(asteroidHit);
        
        #endregion

        //-------------------------high----------------------------------------
        #region highly negative events
        HonsProj.Event engineMalfunction = new HonsProj.Event("Engine Malfunction!", "The engine has malfunctioned " +
           " - either the ship speed will need to decrease or an explosion will damage the hull");

        engineMalfunction.AddChoice(HonsProj.EventType.FIRST_CHOICE, DestroyEvent, false);
        engineMalfunction.AddChoice(HonsProj.EventType.FIRST_CHOICE, DecreaseShipSpeed, false, "1. Decrease the ship speed (-5)");

        engineMalfunction.AddChoice(HonsProj.EventType.SECOND_CHOICE, DestroyEvent, false);
        engineMalfunction.AddChoice(HonsProj.EventType.SECOND_CHOICE, DecreaseShipHullIntegrity, false, "2. An explosion damages the ship hull (-20)");

        negative_events_high.Add(engineMalfunction);

        //

        HonsProj.Event mindwormInfestation = new HonsProj.Event("Mindworm Infestation!", "A mindworm infestation has caused the crew to wander from their stations! ");

        mindwormInfestation.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");
        mindwormInfestation.AddChoice(HonsProj.EventType.CONTINUE, RandomiseCrewPositions, false, "+ (crew positions are scrambled)");
        
        negative_events_high.Add(mindwormInfestation);
        
        #endregion
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
        float goodEventChance = Random.value;
        float eventSeverityChance = Random.value;

        switch (GameController.INSTANCE.Current_Game_Phase)
        {
            case GamePhase.FIRST_PHASE:

                if (goodEventChance >= 0.5f)
                {
                    if (eventSeverityChance <= 0.75f)
                        current_event = positive_events_low[Random.Range(0, positive_events_low.Count)];
                    else
                        current_event = positive_events_med[Random.Range(0, positive_events_med.Count)];
                }
                else
                {
                    if (eventSeverityChance <= 0.75f)
                        current_event = negative_events_low[Random.Range(0, negative_events_low.Count)];
                    else
                        current_event = negative_events_med[Random.Range(0, negative_events_med.Count)];
                }

                break;
            case GamePhase.MIDDLE_PHASE:

                if (goodEventChance >= 0.5f)
                {
                    if (eventSeverityChance <= 0.5f)
                        current_event = positive_events_med[Random.Range(0, positive_events_med.Count)];
                    else if (eventSeverityChance <= 0.75f)
                        current_event = positive_events_high[Random.Range(0, positive_events_high.Count)];
                    else
                        current_event = positive_events_low[Random.Range(0, positive_events_low.Count)];

                }
                else
                {
                    if (eventSeverityChance <= 0.5f)
                        current_event = negative_events_med[Random.Range(0, negative_events_med.Count)];
                    else if (eventSeverityChance <= 0.75f)
                        current_event = negative_events_high[Random.Range(0, negative_events_high.Count)];
                    else
                        current_event = negative_events_low[Random.Range(0, negative_events_low.Count)];
                }

                break;
            case GamePhase.FINAL_PHASE:

                if (goodEventChance >= 0.5f)
                {
                    if (eventSeverityChance <= 0.5f)
                        current_event = positive_events_high[Random.Range(0, positive_events_high.Count)];
                    else if (eventSeverityChance <= 0.9f)
                        current_event = positive_events_med[Random.Range(0, positive_events_med.Count)];
                    else
                        current_event = positive_events_low[Random.Range(0, positive_events_low.Count)];
                }
                else
                {
                    if (eventSeverityChance <= 0.5f)
                        current_event = negative_events_high[Random.Range(0, negative_events_high.Count)];
                    else if (eventSeverityChance <= 0.9f)
                        current_event = negative_events_med[Random.Range(0, negative_events_med.Count)];
                    else
                        current_event = negative_events_low[Random.Range(0, negative_events_low.Count)];
                }

                break;

            default:
                break;
        }

        //note debug
        //current_event = negative_events_high[1];

        current_event_go.GetComponent<EventInfoSetter>().SetEventInfo(current_event);

        AudioController.INSTANCE.PlayEventAudio();
    }

    void RandomiseCrewPositions()
    {
        CrewController.INSTANCE.RandomiseCrewPositions();
    }

    void DestroyEvent()
    {
        Destroy(current_event_go);
        current_event = null;
        GameController.INSTANCE.FinishedEvent();
    }

    void LevelUpCrew()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            cm.LevelUp();
        }
    }

    void LevelUpRandomCrewMember()
    {
        CrewController.INSTANCE.GetRandomCrewMember().LevelUp();
    }

    void LevelDownCrew()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            cm.LevelDown();
        }
    }

    void LevelDownRandomCrewMember()
    {
        CrewController.INSTANCE.GetRandomCrewMember().LevelDown();
    }

    void GenerateTask(TaskType task_type)
    {
        Room r = ShipController.INSTANCE.GetRoomByTaskType(task_type);

        if (r != null)
        {
            ShipController.INSTANCE.AddTask(r, task_type);
        }
    }

    void IncreaseShipHullIntegrity()
    {
        ShipController.INSTANCE.IncreaseShipHullIntegrity(20.0f);
    }

    void DecreaseShipHullIntegrity()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.EVENT)
            return;

        ShipController.INSTANCE.DecreaseShipHullIntegrity(20.0f);
    }

    void IncreaseShieldCapacity()
    {
        ShipController.INSTANCE.IncreaseShieldCapacity(20.0f);
    }

    void DecreaseShieldCapacity()
    {
        ShipController.INSTANCE.DecreaseShieldCapacity(20.0f);
    }

    void IncreaseLifeSupportEfficiency()
    {
        ShipController.INSTANCE.IncreaseLifeSupportEfficiency(20.0f);
    }

    void DecreaseLifeSupportEfficiency()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.EVENT)
            return;

        ShipController.INSTANCE.DecreaseLifeSupportEfficiency(20.0f);
    }

    void DecreaseCrewStress()
    {
        ShipController.INSTANCE.DecreaseCrewStress(20.0f);
    }

    void IncreaseCrewStress()
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.EVENT)
            return;

        ShipController.INSTANCE.IncreaseCrewStress(20.0f);
    }

    void IncreaseShipSpeed()
    {
        ShipController.INSTANCE.ChangeShipSpeed(5.0f);
    }

    void DecreaseShipSpeed()
    {
        ShipController.INSTANCE.ChangeShipSpeed(-5.0f);
    }

    void IncreaseDistanceToEarth()
    {
        GameController.INSTANCE.ChangeDistanceToEarth(500.0f);
    }

    void DecreaseDistanceToEarth()
    {
        GameController.INSTANCE.ChangeDistanceToEarth(-500.0f);
    }
}
