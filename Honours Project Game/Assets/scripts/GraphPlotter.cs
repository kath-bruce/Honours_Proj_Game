using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HonsProj
{
    public static class GraphPlotter
    {
        public static Graph CreateGraph(List<Room> rooms)
        {
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();

            Dictionary<Room, List<Node>> roomToNodes = new Dictionary<Room, List<Node>>();

            //todo possibly add edges between door nodes that share a room?
            //may need to implement a*
            //possibly use this to find edges between doors that share a room
            //when a door is found between 2 rooms
            //add the room to hear with the door
            //after all doors are found 
            //iterate through each list of doors and add edges
            Dictionary<Room, List<Node>> roomToDoors = new Dictionary<Room, List<Node>>();

            foreach (Room rm in rooms)
            {
                List<Node> nodesInRoom = new List<Node>();

                Node n = new Node(rm.Room_Info.X, n.X = rm.Room_Info.Y);
                nodes.Add(n);
                nodesInRoom.Add(n);

                //add nodes in corners - make edges

                Node n_topLeft = new Node(rm.Room_Info.X - (rm.Room_Info.width / 4), rm.Room_Info.Y + (rm.Room_Info.height / 4));
                nodes.Add(n_topLeft);
                nodesInRoom.Add(n_topLeft);

                Node n_topRight = new Node(rm.Room_Info.X + (rm.Room_Info.width / 4), rm.Room_Info.Y + (rm.Room_Info.height / 4));
                nodes.Add(n_topRight);
                nodesInRoom.Add(n_topRight);

                Node n_bottomLeft = new Node(rm.Room_Info.X - (rm.Room_Info.width / 4), rm.Room_Info.Y - (rm.Room_Info.height / 4));
                nodes.Add(n_bottomLeft);
                nodesInRoom.Add(n_bottomLeft);

                Node n_bottomRight = new Node(rm.Room_Info.X + (rm.Room_Info.width / 4), rm.Room_Info.Y - (rm.Room_Info.height / 4));
                nodes.Add(n_bottomRight);
                nodesInRoom.Add(n_bottomRight);

                //should have 5 nodes in each room (not counting door(s))

                for (int i = 0; i < nodesInRoom.Count - 1; i++)
                {
                    for (int j = 1; j < nodesInRoom.Count; j++)
                    {
                        Edge edge;
                        edge.to = nodesInRoom[i];
                        edge.from = nodesInRoom[j];

                        edges.Add(edge);
                    }
                }
                roomToNodes.Add(rm, nodesInRoom);

                //where to makes edges to door???
            }

            for (int i = 0; i < rooms.Count - 1; i++)
            {
                for (int j = 1; j < rooms.Count; j++)
                {
                    Node node = FindDoor(rooms[i], rooms[j]);
                    if (node.X != null && node.Y != null)
                    {
                        nodes.Add(node);

                        List<Node> nodesInRoom = new List<Node>();

                        roomToNodes.TryGetValue(rooms[i], out nodesInRoom); //todo null checking

                        foreach (Node n in nodesInRoom)
                        {
                            //make edge with door (the variable called node)

                            Edge edge;
                            edge.to = n;
                            edge.from = node;

                            edges.Add(edge);
                        }

                        roomToNodes.TryGetValue(rooms[j], out nodesInRoom); //todo null checking

                        foreach (Node n in nodesInRoom)
                        {
                            //make edge with door (the variable called node)

                            Edge edge;
                            edge.to = n;
                            edge.from = node;

                            edges.Add(edge);
                        }

                        //node is a door

                        List<Node> doors;

                        if (roomToDoors.TryGetValue(rooms[i], out doors))
                        {
                            doors.Add(node);
                        }
                        else
                        {
                            doors = new List<Node>();

                            doors.Add(node);

                            roomToDoors.Add(rooms[i], doors);
                        }

                        List<Node> doors2;

                        if (roomToDoors.TryGetValue(rooms[j], out doors2))
                        {
                            doors2.Add(node);
                        }
                        else
                        {
                            doors2 = new List<Node>();

                            doors2.Add(node);

                            roomToDoors.Add(rooms[j], doors2);
                        }

                    }
                }
            }

            //find all doors in rooms and make edges
            foreach (Room r in roomToDoors.Keys)
            {
                List<Node> doors;

                if (roomToDoors.TryGetValue(r, out doors))
                {
                    for (int i = 0; i < doors.Count - 1; i++)
                    {
                        for (int j = 1; j < doors.Count; j++)
                        {
                            Edge doorEdge;
                            doorEdge.to = doors[i];
                            doorEdge.from = doors[j];

                            edges.Add(doorEdge);
                        }
                    }
                }
            }

            //also try and find out why player doesn't go straight from door (ne) to door (se) - is the edge even there??
            //edge was not there - find out what the other edge in edges is
            //todo debug
            //edge count should be 14 but it is 17
            //that said, it actually works pretty well, at least with the default house
            //note do i want player to move along wall to door?
            return new Graph(nodes, edges); //note possible duplicates?? is it really a problem? use hashset instead???
        }

        private static Node FindDoor(Room a, Room b)
        {
            //if right edge of a == left edge of b
            //      if diff in top and bottom edges are big enough
            //          door/node between a and b
            //
            //if left edge of a == right edge of b
            //      if diff in top and bottom edges are big enough
            //          door/node between a and b
            //
            //if top edge of a == bottom edge of b
            //      if diff in right and left edges are big enough
            //          door/node between a and b
            //
            //if bottom edge of a == top edge of b
            //      if diff in right and left edges are big enough
            //          door/node between a and b
            //
            //else no door/node

            Node node = new Node(null, null);

            if ((a.Room_Info.X + (a.Room_Info.width / 2)) == (b.Room_Info.X - (b.Room_Info.width / 2))
                || (a.Room_Info.X - (a.Room_Info.width / 2)) == (b.Room_Info.X + (b.Room_Info.width / 2)))
            {
                if (Mathf.Abs((a.Room_Info.Y + (a.Room_Info.height / 2))
                    - (b.Room_Info.Y - (b.Room_Info.height / 2))) > 0.5
                    ^ Mathf.Abs((a.Room_Info.Y - (a.Room_Info.height / 2))
                    - (b.Room_Info.Y + (b.Room_Info.height / 2))) < 0.5)
                {
                    node.X = (a.Room_Info.X + b.Room_Info.X) / 2;
                    node.Y = (a.Room_Info.Y + b.Room_Info.Y) / 2;
                }
            }
            else if ((a.Room_Info.Y - (a.Room_Info.height / 2)) == (b.Room_Info.Y + (b.Room_Info.height / 2))
                || (a.Room_Info.Y + (a.Room_Info.height / 2)) == (b.Room_Info.Y - (b.Room_Info.height / 2)))
            {
                if (Mathf.Abs((a.Room_Info.X + (a.Room_Info.width / 2))
                    - (b.Room_Info.X - (b.Room_Info.width / 2))) > 0.5
                    ^ Mathf.Abs((a.Room_Info.X - (a.Room_Info.width / 2))
                    - (b.Room_Info.X + (b.Room_Info.width / 2))) < 0.5)
                {
                    node.X = (a.Room_Info.X + b.Room_Info.X) / 2;
                    node.Y = (a.Room_Info.Y + b.Room_Info.Y) / 2;
                }
            }

            return node;
        }

    }
}
