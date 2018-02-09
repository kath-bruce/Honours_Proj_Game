using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public enum RoomType
    {
        COMMS, SHIELD_CHARGER,
        LIFE_SUPPORT, TORPEDO_LAUNCHER, NAVIGATION, MED_BAY
    }

    public struct RoomInfo
    {
        public float X, Y, width, height;
    }

    public class Room
    {
        public RoomType Room_Type { get; protected set; }
        public RoomInfo Room_Info { get; protected set; }

        private List<Task> tasks;

        public Action<Task, Node> AddTaskCallBack;      //todo null checking on callbacks
        public Action<Task> RemoveTaskCallBack;

        //todo should probably include list of crew in room

        public Room(RoomType r_type, RoomInfo r_info)
        {
            Room_Type = r_type;
            Room_Info = r_info;
            tasks = new List<Task>();
        }

        public void OnTick(float timeDecay)
        {
            //todo use this method for task generation -- maybe??????
        }

        public void AddTask(Task t, Node n)
        {
            tasks.Add(t);
            AddTaskCallBack(t, n);
        }

        public void RemoveTask(Task t)
        {
            tasks.Remove(t);
            RemoveTaskCallBack(t);
        }

        public int NumberOfTasks()
        {
            return tasks.Count;
        }

        public override string ToString()
        {
            return Room_Type.ToString();
        }

        public bool HasTask(Task t)
        {
            return tasks.Contains(t);
        }

        public bool HasAnyTask()
        {
            return tasks.Count > 0;
        }

        #region register and unregister call backs
        //public void AddTask_RegisterCallBack(Action<Task> cb)
        //{
        //    addTaskToShipController += cb;
        //}

        //public void AddTask_UnregisterCallBack(Action<Task> cb)
        //{
        //    addTaskToShipController -= cb;
        //}

        //public void RemoveTask_RegisterCallBack(Action<Task> cb)
        //{
        //    removeTaskFromShipController += cb;
        //}

        //public void RemoveTask_UnregisterCallBack(Action<Task> cb)
        //{
        //    removeTaskFromShipController -= cb;
        //}
        #endregion
    }
}
