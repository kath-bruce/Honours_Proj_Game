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

    private const float event_timer = 5.0f;
    private float time_left_til_event = event_timer;

    [SerializeField]
    GameObject eventPrefab;

    private GameObject current_event_go;
    private HonsProj.Event current_event;

    // Use this for initialization
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        if (GameController.INSTANCE.Game_State != GameState.IN_PLAY)
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

        current_event = new HonsProj.Event("dummy event");

        current_event.AddChoice(HonsProj.EventType.CONTINUE, DestroyEvent, false, "continue");

        current_event_go.GetComponent<EventInfoSetter>().SetEventInfo(current_event);
    }

    void DestroyEvent(/*params object[] args*/)
    {
        Destroy(current_event_go);
        current_event = null;
        GameController.INSTANCE.FinishedEvent();
    }
}
