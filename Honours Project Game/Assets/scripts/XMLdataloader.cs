using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HonsProj
{
    public static class XmlDataLoader
    {
        public static List<CrewMember> GetCrewFromXML(string pathToXML)
        {
            List<CrewMember> crew = new List<CrewMember>();

            XmlDocument xmlDoc = new XmlDocument();

            if (!System.IO.File.Exists(pathToXML))
            {
                return null;
            }

            xmlDoc.Load(pathToXML);

            XmlNodeList crewList = xmlDoc.GetElementsByTagName("crew_member");

            foreach (XmlNode crewMemberNode in crewList)
            {
                string crewName = null;
                CrewMemberRole crewRole = CrewMemberRole.NULL;

                foreach (XmlNode crewMemberInfo in crewMemberNode.ChildNodes)
                {
                    switch (crewMemberInfo.Name)
                    {
                        case "name":
                            crewName = crewMemberInfo.InnerText;
                            break;
                        case "role":
                            crewRole = (CrewMemberRole)Enum.Parse(typeof(CrewMemberRole), crewMemberInfo.InnerText, true);
                            break;
                        default:
                            break;
                    }

                }

                if (crewName == null || crewRole == CrewMemberRole.NULL)
                {
                    return null;
                }

                crew.Add(new CrewMember(crewName, crewRole));
            }

            return crew;
        }

        public static Dictionary<RoomType, List<TaskType>>
            GetTasksForRoomType(string pathToXML)
        {
            Dictionary<RoomType, List<TaskType>> tasks_for_room_type
                = new Dictionary<RoomType, List<TaskType>>();

            XmlDocument xmlDoc = new XmlDocument();

            if (!System.IO.File.Exists(pathToXML))
            {
                return null;
            }

            xmlDoc.Load(pathToXML);

            XmlNodeList rooms = xmlDoc.GetElementsByTagName("room");

            foreach (XmlNode roomNode in rooms)
            {
                //key
                //find room type
                RoomType currentRoom = (RoomType)Enum.Parse(typeof(RoomType), roomNode.Attributes["type"].Value, true);

                //value
                //find tasks associated with room type

                XmlNodeList tasks = roomNode.ChildNodes;

                List<TaskType> currentTasks = new List<TaskType>();

                foreach (XmlNode task in tasks)
                {
                    TaskType currentTask = (TaskType)Enum.Parse(typeof(TaskType), task.InnerText, true);
                    currentTasks.Add(currentTask);
                }

                tasks_for_room_type.Add(currentRoom, currentTasks);
            }

            return tasks_for_room_type;
        }

        public static Dictionary<CrewMemberRole, List<TaskType>> GetTasksForRoles(string pathToXML)
        {
            Dictionary<CrewMemberRole, List<TaskType>> tasks_for_roles
                = new Dictionary<CrewMemberRole, List<TaskType>>();

            XmlDocument xmlDoc = new XmlDocument();

            if (!System.IO.File.Exists(pathToXML))
            {
                return null;
            }

            xmlDoc.Load(pathToXML);

            XmlNodeList roles = xmlDoc.GetElementsByTagName("role");

            foreach (XmlNode roleNode in roles)
            {
                //key
                //find role
                CrewMemberRole currentRole = (CrewMemberRole)Enum.Parse(typeof(CrewMemberRole), roleNode.Attributes["type"].Value, true);

                //value
                //find tasks associated with role

                XmlNodeList tasks = roleNode.ChildNodes;

                List<TaskType> currentTasks = new List<TaskType>();

                foreach (XmlNode task in tasks)
                {
                    TaskType currentTask = (TaskType)Enum.Parse(typeof(TaskType), task.InnerText, true);
                    currentTasks.Add(currentTask);
                }

                tasks_for_roles.Add(currentRole, currentTasks);
            }

            return tasks_for_roles;
        }
    }
}
