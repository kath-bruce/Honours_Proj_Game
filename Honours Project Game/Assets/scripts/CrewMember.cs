using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public enum CrewMemberRole
    {
        NULL, COMMS_OFFICER, CAPTAIN, ENGINEER, PILOT, SHIP_MEDIC,
        FIRST_OFFICER, WEAPONS_OFFICER
    }

    public class CrewMember
    {
        public string Crew_Member_Name { get; protected set; }

        public CrewMemberRole Crew_Member_Role { get; protected set; }

        public int Crew_Member_Level { get; protected set; }

        private Task current_task;
        public Task Current_Task
        {
            get
            {                                                               //note only one crew member can do work on a task - check!!!
                if (current_task != null && current_task.WorkLeft() <= 0.0f)// || !current_task.Current_Crew_Members.Contains(this)))
                {
                    current_task.RemoveCrewMember(this);
                    current_task = null;
                }

                return current_task;
            }

            protected set
            {
                if (value == null && current_task != null)
                    current_task.RemoveCrewMember(this);

                current_task = value;
            }
        }

        Task target_task;
        List<Node> crew_member_path;
        Node last_node;

        public Action<CrewMember, float, float> SetCrewMemberPosCallBack;    //todo null checking on callbacks
        public Func<CrewMember, Node> GetCrewMemberPosCallBack;
        public Action<CrewMember, Node> MoveCrewMemberCallBack;

        //note crew members should have roles that define their tasks
        public CrewMember(string new_name, CrewMemberRole role)
        {
            Crew_Member_Name = new_name;
            Crew_Member_Role = role;
            Crew_Member_Level = 1;
        }

        public void LevelUp()
        {
            if (Crew_Member_Level < 10)
            {
                Crew_Member_Level++;
            }
            else
            {
                //do something else
            }
        }

        public void LevelDown()
        {
            if (Crew_Member_Level > 1)
            {
                Crew_Member_Level--;
            }
            else
            {
                //do something else
            }
        }

        public float GetWorkSpeed()
        {
            return (float)(Math.Pow(Crew_Member_Level, 2) * 0.1f) + 1;
        }

        public void SetPathAndTask(List<Node> new_q, Task t)
        {
            SetPath(new_q);
            target_task = t;
        }

        public void SetPath(List<Node> new_q)
        {
            crew_member_path = new_q;
            Current_Task = null;
        }

        public Node GetPrevNode()
        {
            if (crew_member_path == null)
                return GetPos();

            if (crew_member_path.Count == 0)
                return last_node;

            return crew_member_path[0];
        }

        public bool HasPath()
        {
            if (crew_member_path == null)
                return false;

            return (crew_member_path.Count > 0);
        }

        public void DequeueFromPath()
        {
            if (crew_member_path.Count == 1)
            {
                last_node = crew_member_path[0];
                Current_Task = target_task;
            }

            crew_member_path.RemoveAt(0);
        }

        public void SetPos(float new_x, float new_y)
        {
            SetCrewMemberPosCallBack(this, new_x, new_y);
        }

        public Node GetPos()
        {
            return GetCrewMemberPosCallBack(this);
        }

        public void Move()
        {
            MoveCrewMemberCallBack(this, crew_member_path[0]);
        }
    }
}