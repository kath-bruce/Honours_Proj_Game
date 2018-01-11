using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HonsProj
{
    public static class XmlDataLoader
    {
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
                //find room type - switch case
                RoomType currentRoom = (RoomType) Enum.Parse(typeof(RoomType), roomNode.Attributes["type"].Value, true);
                
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
    }
}
