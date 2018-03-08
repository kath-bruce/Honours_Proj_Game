using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public enum TaskType
    {
        REPAIR, TORPEDO_ASTEROIDS,
        CHARGE_SHIELDS, MAINTAIN_LIFE_SUPPORT,
        STEER_SHIP, TALK_TO_OTHER_SHIP, HEAL_CREW_MEMBER, MAINTAIN_COMMS
    }

    public class Task
    {
        public TaskType Task_Type { get; protected set; }
        public float Work { get; protected set; }
        //public float Time_Left { get; protected set; }

        public Room Parent_Room { get; protected set; }

        public Action<float> UncompletedCallBack;
        public Action<Task, bool> WorkedOnCallback;
        
        public Node Task_Node { get; protected set; }

        float workNeeded;

        public Task(TaskType t_type, int w_needed, Room rm, Node n)
        {
            Task_Type = t_type;
            Work = 0;
            //Time_Left = 5f; 
            workNeeded = w_needed;
            Parent_Room = rm;
            Task_Node = n;
        }

        public void OnTick(float timeDecay)
        {
            //if (UncompletedCallBack != null)
                UncompletedCallBack(timeDecay);
        }

        public bool DoWork(float workDone, CrewMember cm)
        {
            WorkedOnCallback(this, true);
            Work += workDone;

            if (Work >= workNeeded)
            {
                Parent_Room.RemoveTask(this, cm);
            }

            return true;
        }

        public float WorkLeft()
        {
            return workNeeded - Work;
        }

        public override string ToString()
        {
            return "Task: " + Task_Type.ToString() + "\nWork left: " + workNeeded;
        }

        public string ToStringTaskType()
        {
            string task_type_str = Task_Type.ToString();

            string[] task_type_str_arr;
            task_type_str_arr = task_type_str.Split('_');

            task_type_str = "";

            for (int i = 0; i < task_type_str_arr.Length; i++)
            {
                task_type_str += task_type_str_arr[i];

                if (i != task_type_str_arr.Length - 1)
                    task_type_str += "\n";
            }

            return task_type_str;
        }
    }
}
