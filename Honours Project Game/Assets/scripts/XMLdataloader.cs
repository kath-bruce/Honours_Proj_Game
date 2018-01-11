using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HonsProj
{
    public static class XMLdataloader
    {
        public static Dictionary<RoomType, List<TaskType>> GetTasksForRoomType()
        {
            Dictionary<RoomType, List<TaskType>> tasks_for_room_type 
                = new Dictionary<RoomType, List<TaskType>>();

            //note need function for going the opposite way???

            return tasks_for_room_type;
        }
    }
}
