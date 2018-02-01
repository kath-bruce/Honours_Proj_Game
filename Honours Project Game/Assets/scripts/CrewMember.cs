using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public class CrewMember
    {
        public float Stress { get; set; }
        //public Room Current_Room { get; set; } //note probably don't need to know current room
        public Task Current_Task { get; protected set; }
        Task target_task;

        //Action<float, float> SetGameObjectPos;    //todo null checking on callbacks
        //Func<Node> GetGameObjectPos;
        //Action<Node> LerpGameObject;

        List<Node> player_path;
        Node last_node;

        public void SetPlayerPathAndTask(List<Node> new_q, Task t)
        {
            player_path = new_q;
            target_task = t;
            Current_Task = null;
        }

        public Node GetPlayerPrevNode()
        {
            if (player_path == null)
                return GetPlayerPos();

            if (player_path.Count == 0)
                return last_node;

            return player_path[0];
        }

        public bool PlayerHasPath()
        {
            if (player_path == null)
                return false;

            return (player_path.Count > 0);
        }

        public void DequeueFromPath()
        {
            if (player_path.Count == 1)
            {
                last_node = player_path[0];
                Current_Task = target_task;
            }

            player_path.RemoveAt(0);
        }

        public void SetPlayerPos(float new_x, float new_y)
        {
            //SetGameObjectPos(new_x, new_y);
        }

        public Node GetPlayerPos()
        {
            Node nullNode;
            nullNode.X = null;
            nullNode.Y = null;
            return nullNode;// GetGameObjectPos();
        }

        public void LerpPlayer()
        {
            //LerpGameObject(player_path[0]);
        }

        #region singleton
        //private Player() { }

        //private static Player instance;

        //public static Player INSTANCE
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new Player();
        //        }
        //        return instance;
        //    }
        //}
        #endregion

        #region register and unregister callbacks
        //public void SetGameObjectPos_RegisterCallback(Action<float, float> cb)
        //{
        //    //SetGameObjectPos.GetInvocationList()
        //    SetGameObjectPos += cb;
        //}

        //public void SetGameObjectPos_UnregisterCallback(Action<float, float> cb)
        //{
        //    SetGameObjectPos -= cb;
        //}

        //public void GetGameObjectPos_RegisterCallback(Func<Node> cb)
        //{
        //    GetGameObjectPos += cb;
        //}

        //public void GetGameObjectPos_UnregisterCallback(Func<Node> cb)
        //{
        //    GetGameObjectPos -= cb;
        //}

        //public void LerpGameObject_RegisterCallback(Action<Node> cb)
        //{
        //    LerpGameObject += cb;
        //}

        //public void LerpGameObject_UnregisterCallback(Action<Node> cb)
        //{
        //    LerpGameObject -= cb;
        //}
        #endregion
    }
}