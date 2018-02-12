using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrewUISelection : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        UIManager.INSTANCE.SelectCrewMember(eventData.selectedObject);
    }
}
