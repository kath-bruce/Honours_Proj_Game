using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HonsProj;

public class PlayerGameObject : MonoBehaviour
{
    void Awake()
    {
        //Player.INSTANCE.SetGameObjectPos_RegisterCallback(SetPlayerPos);
        //Player.INSTANCE.GetGameObjectPos_RegisterCallback(GetPlayerPos);
        //Player.INSTANCE.LerpGameObject_RegisterCallback(LerpPlayerToNode);
    }

    void SetPlayerPos(float new_x, float new_y)
    {
        Vector3 player_vec = gameObject.transform.position;

        player_vec.x = new_x;
        player_vec.y = new_y;

        gameObject.transform.position = player_vec;
    }

    Node GetPlayerPos()
    {
        Node playerPos;

        playerPos.X = gameObject.transform.position.x;
        playerPos.Y = gameObject.transform.position.y;

        return playerPos;
    }

    void LerpPlayerToNode(Node end)
    {
        Vector3 end_vec = new Vector3();
        end_vec.x = (float)end.X;
        end_vec.y = (float)end.Y;

        gameObject.transform.position = Vector3.LerpUnclamped(gameObject.transform.position, end_vec, 15 * Time.deltaTime);

        if (gameObject.transform.position == end_vec)
        {
            //Player.INSTANCE.DequeueFromPath();
        }
    }
}
