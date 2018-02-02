using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public class CrewMember
    {
        //public float Stress { get; set; }
        //public Room Current_Room { get; set; } //note probably don't need to know current room
        public Task Current_Task { get; protected set; }

        public string Crew_Member_Name { get; protected set; }

        Task target_task;

        Action<CrewMember, float, float> SetCrewMemberPos;    //todo null checking on callbacks
        Func<CrewMember, Node> GetCrewMemberPos;
        Action<CrewMember, Node> MoveCrewMember;
        
        //note crew members should have roles that define their tasks
        public CrewMember(string new_name)
        {
            Crew_Member_Name = new_name;
        }

        List<Node> crew_member_path;
        Node last_node;

        public void SetPathAndTask(List<Node> new_q, Task t)
        {
            crew_member_path = new_q;
            target_task = t;
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
            SetCrewMemberPos(this, new_x, new_y);
        }

        public Node GetPos()
        {
            return GetCrewMemberPos(this);
        }

        public void Move()
        {
            MoveCrewMember(this, crew_member_path[0]);
        }

        #region register and unregister callbacks
        public void SetPos_RegisterCallback(Action<CrewMember, float, float> cb)
        {
            SetCrewMemberPos += cb;
        }

        public void SetPos_UnregisterCallback(Action<CrewMember, float, float> cb)
        {
            SetCrewMemberPos -= cb;
        }

        public void GetPos_RegisterCallback(Func<CrewMember, Node> cb)
        {
            GetCrewMemberPos += cb;
        }

        public void GetPos_UnregisterCallback(Func<CrewMember, Node> cb)
        {
            GetCrewMemberPos -= cb;
        }

        public void Move_RegisterCallback(Action<CrewMember, Node> cb)
        {
            MoveCrewMember += cb;
        }

        public void Move_UnregisterCallback(Action<CrewMember, Node> cb)
        {
            MoveCrewMember -= cb;
        }

        #endregion
    }
}