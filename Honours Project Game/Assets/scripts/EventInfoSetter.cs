using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HonsProj;

public class EventInfoSetter : MonoBehaviour
{
    public void SetEventInfo(HonsProj.Event new_event)
    {
        foreach (Transform child in transform)
        {
            switch (child.gameObject.tag)
            {
                case "EventName":
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_Name;
                    break;

                case "EventText":
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_Text;
                    break;

                case "EventOptions":
                    SetEventOptions(child, new_event);
                    break;

                default:
                    break;
            }
        }
    }

    private void SetEventOptions(Transform options_parent, HonsProj.Event new_event)
    {
        Dictionary<HonsProj.EventType, HonsProj.Event.EventOption> event_choices = new_event.GetChoices();

        foreach (Transform child in options_parent)
        {
            switch (child.gameObject.tag)
            {
                case "EventContinue":

                    if (event_choices.ContainsKey(HonsProj.EventType.CONTINUE))
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_Continue_Text;
                        child.gameObject.GetComponent<EventOptionSelection>().SetEventOptionCallBack(event_choices[HonsProj.EventType.CONTINUE]);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }

                    break;

                case "EventFirstChoice":

                    if (event_choices.ContainsKey(HonsProj.EventType.FIRST_CHOICE))
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_First_Choice_Text;
                        child.gameObject.GetComponent<EventOptionSelection>().SetEventOptionCallBack(event_choices[HonsProj.EventType.FIRST_CHOICE]);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }

                    break;

                case "EventSecondChoice":

                    if (event_choices.ContainsKey(HonsProj.EventType.SECOND_CHOICE))
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_Second_Choice_Text;
                        child.gameObject.GetComponent<EventOptionSelection>().SetEventOptionCallBack(event_choices[HonsProj.EventType.SECOND_CHOICE]);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }

                    break;

                case "EventThirdChoice":

                    if (event_choices.ContainsKey(HonsProj.EventType.THIRD_CHOICE))
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().text = new_event.Event_Third_Choice_Text;
                        child.gameObject.GetComponent<EventOptionSelection>().SetEventOptionCallBack(event_choices[HonsProj.EventType.THIRD_CHOICE]);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }

                    break;

                default:
                    break;
            }
        }
    }
    
}
