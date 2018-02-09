using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HonsProj
{
    public struct Node
    {
        public float? X, Y;

        //todo see where you can use this
        public Node(float? new_x, float? new_y)
        {
            X = new_x;
            Y = new_y;
        }

        public void MakeNull()
        {
            X = null;
            Y = null;
        }

        public bool IsNull()
        {
            return (X == null || Y == null);
        }

        public void SetXandY(float new_x, float new_y)
        {
            X = new_x;
            Y = new_y;
        }
    }

    public struct Edge
    {
        public Node to, from;
    }

    public class Graph
    {
        private readonly List<Node> nodes;
        private readonly List<Edge> edges;

        private Node start;
        private Node end;

        public Graph(List<Node> ns, List<Edge> es)
        {
            nodes = ns;
            edges = es;

            start = nodes[0];
            end = nodes[nodes.Count - 1];
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public bool SetStartAndEnd(Node s, Node e)
        {
            if (!nodes.Contains(s) || !nodes.Contains(e))
                return false;

            start = s;
            end = e;

            return true;
        }

        public bool SetStartAndEnd(Room s, Room e)
        {
            Node n_s = new Node(s.Room_Info.X, s.Room_Info.Y);
            //n_s.X = s.Room_Info.X;
            //n_s.Y = s.Room_Info.Y;

            Node n_e = new Node(e.Room_Info.X, e.Room_Info.Y);
            //n_e.X = e.Room_Info.X;
            //n_e.Y = e.Room_Info.Y;

            return SetStartAndEnd(n_s, n_e);
        }

        public bool SetStartAndEnd(Node s, Task e)
        {
            Node t_e = new Node(e.Parent_Room.Room_Info.X, e.Parent_Room.Room_Info.Y);
            //t_e.X = e.Parent_Room.Room_Info.X;
            //t_e.Y = e.Parent_Room.Room_Info.Y;

            return SetStartAndEnd(s, t_e);
        }

        public Node[] FindPath() //breadth first search as generally not many different paths between nodes
        {
            Queue<Node> frontier = new Queue<Node>();

            Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();

            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                Node current = frontier.Dequeue();

                if (current.X == end.X && current.Y == end.Y)
                {
                    //goal found
                    return ReconstructPath(came_from);
                }

                foreach (Node neighbour in FindNeighbours(current))
                {
                    if (!came_from.ContainsKey(neighbour))
                    {
                        frontier.Enqueue(neighbour);
                        came_from[neighbour] = current;
                    }
                }
            }

            return null;
        }

        private Node[] ReconstructPath(Dictionary<Node, Node> came_from)
        {
            List<Node> path = new List<Node>();

            Node current = end;

            path.Add(current);

            while (current.X != start.X || current.Y != start.Y)
            {
                current = came_from[current];
                path.Add(current);
            }

            path.Reverse();

            return path.ToArray();
        }

        private Node[] FindNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            foreach (Edge edge in edges)
            {
                if (edge.from.X == node.X && edge.from.Y == node.Y)
                {
                    neighbours.Add(edge.to);
                }
                else if (edge.to.X == node.X && edge.to.Y == node.Y)
                {
                    neighbours.Add(edge.from);
                }
            }

            return neighbours.ToArray();
        }
    }
}