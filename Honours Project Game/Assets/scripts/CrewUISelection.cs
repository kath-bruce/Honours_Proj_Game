using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrewUISelection : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        if (GameController.INSTANCE.Current_Game_State != GameState.IN_PLAY)
            return;

        UIManager.INSTANCE.SelectCrewMember(eventData.selectedObject);
    }
}
