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
        private bool isFull = false;

        private const int TASK_LIMIT = 5;

        public Action<Task, Node> AddTaskCallBack;      //todo null checking on callbacks
        public Action<Task, CrewMember> RemoveTaskCallBack;

        public Room(RoomType r_type, RoomInfo r_info)
        {
            Room_Type = r_type;
            Room_Info = r_info;
            tasks = new List<Task>();
        }
        
        public void AddTask(Task t, Node n)
        {
            if (!isFull)
            {
                tasks.Add(t);
                AddTaskCallBack(t, n);
            }

            if (tasks.Count == TASK_LIMIT)
            {
                isFull = true;
            }
        }

        public void RemoveTask(Task t, CrewMember cm)
        {
            tasks.Remove(t);
            RemoveTaskCallBack(t, cm);

            isFull = false;
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Room other = (Room)obj;
            
            return (Room_Type == other.Room_Type 
                && Room_Info.X == other.Room_Info.X 
                && Room_Info.Y == other.Room_Info.Y 
                && Room_Info.width == other.Room_Info.width 
                && Room_Info.height == other.Room_Info.height);
        }

        public override int GetHashCode()
        {
            return Room_Type.GetHashCode() 
                ^ Room_Info.X.GetHashCode() 
                ^ Room_Info.Y.GetHashCode() 
                ^ Room_Info.width.GetHashCode() 
                ^ Room_Info.height.GetHashCode();
        }
    }
}
