using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventOptionSelection : MonoBehaviour, ISelectHandler
{
    HonsProj.Event.EventOption option;

    public void SetEventOptionCallBack(HonsProj.Event.EventOption cb)
    {
        option = cb;
    }

    public void OnSelect(BaseEventData eventData)
    {
        option();
    }
}
