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

    void LevelUpCrew()
    {
        foreach (CrewMember cm in CrewController.INSTANCE.GetCrewMembers())
        {
            cm.LevelUp();

            //Debug.Log(cm.Crew_Member_Name + " leveled up to " + cm.Crew_Member_Level + "!");
        }
    }
}
