using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LinnTestShortestWay
{
    using static World;
    using Graph = Dictionary<World.Cell, PathFinder.Node>;

    class PathFinder
    {

        public static Graph BuildGraph(Cell[,] cells)
        {
            var graph = new Graph();
            foreach (Cell cell in cells)
            {
                graph.Add(cell, new Node(cell));
            }
            foreach (Node node in graph.Values)
            {
                foreach (Cell cell in GetNeighbors(node.Cell, cells))
                    node.Neighbors.Add(graph[cell]);
            }
            return graph;
        }

        private static List<Cell> GetNeighbors(Cell cell, Cell[,] cells)
        {
            var Neighbors = new List<Cell>();
            int sizeX = cells.GetLength(0), sizeY = cells.GetLength(1);
            for (int x = cell.X - 1; x <= cell.X + 1; x++)
                for (int y = cell.Y - 1; y <= cell.Y + 1; y++)
                    if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && cells[x, y] != cell)
                        Neighbors.Add(cells[x, y]);
            return Neighbors;
        }

        public class Node
        {
            public readonly Cell Cell;
            public readonly int? Cost = null;
            public int? CurrentCost { get; set; } = null;
            public bool IsChecked { get; set; } = false;
            public List<Node> Neighbors = new List<Node>();
            public Node PrevNode { get; set; } = null;

            public Node(Cell cell)
            {
                Cell = cell;
                Cost = Cell.Passability > 0 ? 1 + (100 - Cell.Passability) : Cost;
            }

            public void Init()
            {
                CurrentCost = null;
                PrevNode = null;
            }

            public bool HasLocation(Location loc)
            {
                return Cell.X == loc.X && Cell.Y == loc.Y;
            }

            public void Print(Node start = null, Node end = null, List<Node> path = null)
            {
                if (path != null && path.Contains(this))
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                if (Cell.Passability == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
                if (IsChecked) Console.ForegroundColor = ConsoleColor.Yellow;
                if (this == start) Console.BackgroundColor = ConsoleColor.DarkRed;
                if (this == end) Console.BackgroundColor = ConsoleColor.DarkGreen;
                string str = string.Format("[{0,3},{1,4}]", Cost, CurrentCost);
                //string str = string.Format("[{0,4}]", Cell.Passability);
                //string str = string.Format("[{0,4}]", CurrentCost);
                if (Cost == null)
                    str = string.Format("[{0,3},{1,4}]", " x ", " x ");
                Console.Write(str); Console.ResetColor();
            }
        }

        public static void GraphToString(Graph graph, Node start = null, Node end = null, List<Node> path = null)
        {
            Console.Write("Legend:");
            Console.BackgroundColor = ConsoleColor.DarkRed; Console.Write("[Start]"); Console.ResetColor(); Console.Write(",");
            Console.BackgroundColor = ConsoleColor.DarkBlue; Console.Write("[Path]"); Console.ResetColor();Console.Write(",");
            Console.BackgroundColor = ConsoleColor.DarkGreen; Console.Write("[End]"); Console.ResetColor();Console.Write(",");
            Console.BackgroundColor = ConsoleColor.DarkGray; Console.Write("[x: wall]"); Console.ResetColor(); Console.Write(",");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("[Checked]"); Console.ResetColor(); Console.Write(",");
            Console.Write("[unchecked]"); Console.ResetColor(); Console.Write("\n");
            foreach (Node node in graph.Values)
            {
                if (node.Cell.Y == 0)
                    Console.WriteLine();
                node.Print(start, end, path);
            }
        }

        public static Node InitGraphGetStartNode(Graph graph, Location startLoc)
        {
            Node startNode = null;
            foreach (Node node in graph.Values)
            {
                if (node.HasLocation(startLoc))
                    startNode = node;
                node.Init();
            }
            startNode.CurrentCost = 0;
            return startNode;
        }

        public static Location[] FindShortestWay(Cell[,] cells, Location startLoc, Location endLoc)
        {
            Graph graph = BuildGraph(cells);
            Node startNode = InitGraphGetStartNode(graph, startLoc);
            Node endNode = graph.Values.Single(n => n.HasLocation(endLoc));
            List<Node> priorityQueue = new List<Node> { startNode };

            bool pathFound = false;
            while (priorityQueue.Any() && !pathFound)
            {
                // 1) sort priority queue by current node costs:
                priorityQueue = priorityQueue.OrderBy(x => x.CurrentCost).ToList();
                // 2) take from the queue the lowest-cost node for the moment:
                Node curNode = priorityQueue.First();
                priorityQueue.Remove(curNode);

                foreach (Node nextNode in curNode.Neighbors.OrderBy(x => x.Cost))
                {
                    if (nextNode.IsChecked || nextNode.Cost == null)
                        continue;
                    var newCost = curNode.CurrentCost + nextNode.Cost;
                    if (nextNode.CurrentCost == null || newCost < nextNode.CurrentCost)
                    {
                        nextNode.CurrentCost = newCost;
                        nextNode.PrevNode = curNode;

                        if (!priorityQueue.Contains(nextNode))
                            priorityQueue.Add(nextNode);
                    }
                }
                pathFound = pathFound || curNode == endNode;
                curNode.IsChecked = true;
            }

            // Restore path (from the end node to start node):
            List<Node> bestPath = new List<Node>();
            Node node = pathFound ? endNode : null;
            while (node != null)
            {
                bestPath.Add(node);
                node = node.PrevNode;
            }
            bestPath.Reverse();

            // console output: current world with path on it and after it:
            GraphToString(graph, startNode, endNode, bestPath);
            Console.Write("\n...\nCompleted:\n >> ");
            if (!pathFound)
                Console.Write("Path not found (start/end are (or surrounded by) zeros?) ");
                //return null;
            bestPath.ForEach(n => n.Print(startNode, endNode, bestPath));
            Console.Write(" <<\n");

            return bestPath.Select(n => n.Cell).ToArray();
        }
    }
}
