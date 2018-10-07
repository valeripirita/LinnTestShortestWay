using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnTestShortestWay
{
    public class Location
    {
        public int X;
        public int Y;

        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class World
    {
        public class Cell : Location
        {
            // Defines cell passability from 0 (can't go) to 100 (normal passability)
            // The higher is passability, the quicker it is possible to pass the cell
            public byte Passability;

            public Cell(int x, int y, byte passability)
                : base(x, y)
            {
                this.Passability = passability;
            }
        }

        private readonly Cell[,] cells; // World map

        public World(int sizeX, int sizeY)
        {
            var rnd = new Random();

            // Build map and randomly set passability for each cell
            cells = new Cell[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    cells[x, y] = new Cell(x, y, (byte)rnd.Next(0, 10));
        }

        public Location[] FindShortestWay(Location startLoc, Location endLoc)
        {
            // TODO: Implement finding most shortest way between start and end locations
            return PathFinder.FindShortestWay(cells, startLoc, endLoc);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new Random();
            int maxWorldX = 40, maxWorldY = 20;
            do
            {
                Console.Clear();
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine(" *** (: WELCOME TO PATHFINDER - BASED ON DIJKSTRA'S ALGORITHM :) ***");
                Console.WriteLine("--------------------------------------------------------------------");

                int sizeX = rnd.Next(1, maxWorldX), sizeY = rnd.Next(1, maxWorldY);
                Location A = new Location(rnd.Next(0, sizeX - 1), rnd.Next(0, sizeY - 1));
                Location B = new Location(rnd.Next(0, sizeX - 1), rnd.Next(0, sizeY - 1));
                World world = new World(sizeX, sizeY);
                Console.WriteLine(" Current World [{0} x {1}]:", sizeX, sizeY);

                world.FindShortestWay(A, B);

                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("--- Press any key to create new random world and find path ");
                Console.WriteLine("--- Press the Escape (Esc) key to quit ");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
