using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    //note could potentially end up splitting up task type into different enums
    //  then a struct that contains ONLY one type of task??? 
    //  to provide single thing for tasks to use???
    //  with 2 constructors - default will spawn random one and another will be specific
    //  idk
    public enum TaskType
    {
        REPAIR, LAUNCH_TORPEDO,
        CHARGE_SHIELDS, MAINTAIN_LIFE_SUPPORT,
        STEER_SHIP, TALK_TO_OTHER_SHIP, HEAL_CREW_MEMBER
    }

    public class Task
    {
        public TaskType Task_Type { get; protected set; }
        public float Work { get; protected set; }
        public float Time_Left { get; protected set; }

        public Room Parent_Room { get; protected set; }

        public Action<float> IncreaseStressCallBack;

        float workNeeded;

        public Task(TaskType t_type, int w_needed, Room rm)
        {
            Task_Type = t_type;
            Work = 0;
            Time_Left = 5f; //temp init time left
            workNeeded = w_needed;
            Parent_Room = rm;
        }

        public void OnTick(float timeDecay)
        {
            if (Time_Left > 0)
            {
                Time_Left -= timeDecay;
            }
            else
            {
                //Player.INSTANCE.Stress += timeDecay;
                IncreaseStressCallBack(timeDecay);
            }
        }

        public void DoWork(float workDone)
        {
            Work += workDone;

            if (Work >= workNeeded)
            {
                Parent_Room.RemoveTask(this);
            }
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
