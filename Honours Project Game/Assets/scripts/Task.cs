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
        //public Action<float> DoWorkCallBack;

        //public crew member that is doing this task
        public List<CrewMember> Current_Crew_Members { get; protected set; }

        private int crew_member_limit = 1;
        public Node Task_Node { get; protected set; }

        float workNeeded;

        public Task(TaskType t_type, int w_needed, Room rm, Node n)
        {
            Task_Type = t_type;
            Work = 0;
            Time_Left = 5f; //temp init time left
            workNeeded = w_needed;
            Parent_Room = rm;
            Task_Node = n;

            Current_Crew_Members = new List<CrewMember>();

            //todo - switch case on task type here to determine crew member limit
        }

        public void OnTick(float timeDecay)
        {
            if (Time_Left > 0)
            {
                Time_Left -= timeDecay;
            }
            else
            {
                if (IncreaseStressCallBack != null && (Current_Crew_Members == null || Current_Crew_Members.Count == 0))
                {
                    IncreaseStressCallBack(timeDecay);
                }
            }
        }

        public bool DoWork(float workDone, CrewMember crewMember)
        {
            if (Current_Crew_Members.Count < crew_member_limit && !Current_Crew_Members.Contains(crewMember))
            {
                Current_Crew_Members.Add(crewMember);
            }

            if (Current_Crew_Members.Contains(crewMember))
            {
                Work += workDone;

                //if (DoWorkCallBack != null)
                //{
                //    DoWorkCallBack(workDone);
                //}

                if (Work >= workNeeded)
                {
                    Parent_Room.RemoveTask(this);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public void RemoveCrewMember(CrewMember crewMember)
        {
            if (Current_Crew_Members.Contains(crewMember))
            {
                Current_Crew_Members.Remove(crewMember);
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
