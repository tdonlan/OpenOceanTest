using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace OceanTest
{
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    //Mazes are always stored Width,Height
    public class Grid
    {
        public char[,] map;
        public int height;
        public int width;

        public Grid()
        {

        }
        public Grid(char[,] c)
        {

            map = c;
            height = c.GetLength(0);
            width = c.GetLength(1);
        }
    }

    public class MazeBuilder
    {
        //public char[,] Maze { get{return maze;}  }
        public char[,] maze; //field
        private List<Coord> frontier;


        private int height;
        private int width;

        private Random r;

        private bool noDiagonals;
        private int branchrate;

        /*
         * Entry Wall should be blank, to allow the previously joined submap to contain the walls
         * Exit Wall is the wall that the door should be built on
         * Exit Wall /. Entry Wall
         * 0 = no exit
         * 1=north
         * 2=south
         * 3=west
         * 4=east
         */

        public MazeBuilder(int size, bool noDiagonals, int branchrate, int entryWall, int exitWall, Random r)
        {
            this.r = r;
            this.noDiagonals = noDiagonals;
            this.height = size;
            this.width = size;
            this.branchrate = branchrate;

            frontier = new List<Coord>();

            BuildMaze();
            DoubleSize(2);

            AddWalls(entryWall, exitWall);

        }

        //all maze2 to this maze, on on of the four directions (1=n,2=s,3=w,4=e)
        //left aligned
        //should only really add mazes that are of the same size, so we can do easy doubling of the appropriate dimension
        public void JoinMaze(Grid maze2, int wall)
        {
            Grid tempGrid = new Grid();
            switch (wall)
            {
                case 1: //north wall
                    tempGrid.height = height + maze2.height;
                    tempGrid.width = width;
                    tempGrid.map = new char[tempGrid.height, tempGrid.width];
                    for (int i = 0; i < maze2.height; i++)
                    {
                        for (int j = 0; j < maze2.width; j++)
                        {
                            tempGrid.map[i, j] = maze2.map[i, j];
                        }
                    }

                    for (int i = maze2.height; i < tempGrid.height; i++)
                    {
                        for (int j = 0; j < maze2.width; j++)
                        {
                            tempGrid.map[i, j] = maze[i - maze2.height, j];
                        }
                    }
                    break;
                case 2:
                    tempGrid.height = height + maze2.height;
                    tempGrid.width = width;
                    tempGrid.map = new char[tempGrid.height, tempGrid.width];
                    for (int i = 0; i < maze2.height; i++)
                    {
                        for (int j = 0; j < maze2.width; j++)
                        {
                            tempGrid.map[i, j] = maze[i, j];
                        }
                    }

                    for (int i = height; i < tempGrid.height; i++)
                    {
                        for (int j = 0; j < maze2.width; j++)
                        {
                            tempGrid.map[i, j] = maze2.map[i - height, j];
                        }
                    }
                    break;
                case 3:
                    tempGrid.height = height;
                    tempGrid.width = width + maze2.width;
                    tempGrid.map = new char[tempGrid.height, tempGrid.width];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < maze2.width; j++)
                        {
                            tempGrid.map[i, j] = maze2.map[i, j];
                        }
                        for (int k = maze2.width; k < tempGrid.width; k++)
                        {
                            tempGrid.map[i, k] = maze[i, k - maze2.width];
                        }
                    }
                    break;
                case 4:
                    tempGrid.height = height;
                    tempGrid.width = width + maze2.width;
                    tempGrid.map = new char[tempGrid.height, tempGrid.width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            tempGrid.map[i, j] = maze[i, j];
                        }
                        for (int k = width; k < tempGrid.width; k++)
                        {
                            tempGrid.map[i, k] = maze2.map[i, k - width];
                        }
                    }
                    break;
                default:
                    break;
            }

            maze = tempGrid.map;
            height = tempGrid.height;
            width = tempGrid.width;


        }





        public Grid getGrid()
        {
            Grid g = new Grid();
            g.map = maze;
            g.height = height;
            g.width = width;

            return g;
        }

        public void SubsetMaze(int x, int y, int north, int south, int west, int east)
        {
            /*
            height = north + south + 1;
            width = east + west + 1;
            Grid g = MazeHelper.getSubMap(new Grid(maze), x, y, north, south, west, east);
            if (g != null)
            {
                maze = g.map;
            }
             * */
        }



        private void AddWalls(int entryWall, int exitWall)
        {
            /*
            maze = MazeHelper.EncloseMap(maze, false, 'K', 'D', entryWall, exitWall);
             * */
        }


        //makes a T on the board
        //assumes 5 x 5
        public void makeT()
        {
            maze = new char[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    maze[i, j] = '.';
                }
            }

            maze[1, 1] = '#';
            maze[1, 2] = '#';
            maze[1, 3] = '#';
            maze[2, 2] = '#';
            maze[3, 2] = '#';

        }

        #region MazeAlgorithm

        private void BuildMaze()
        {
            maze = new char[height, width];

            //traverse the maze and fill in unknown blocks
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    maze[i, j] = '?';
                }
            }

            CarveRooms(10);

            //carve out a random spot
            //need to make sure this isn't one of the rooms
            //this is where the player should be?
            Carve(r.Next(width - 1), r.Next(height - 1));

            while (frontier.Count > 0)
            {
                double pos = Math.Pow(r.NextDouble(), Math.Pow(Math.E, branchrate));
                int index = (int)(pos * frontier.Count);
                Coord c = frontier[index];
                if (Check(c.x, c.y))
                    Carve(c.x, c.y);
                else
                    Harden(c.x, c.y);
                frontier.Remove(c);

            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    if (maze[i, j] == ',')
                        maze[i, j] = '.';

                    if (maze[i, j] == '?')
                        maze[i, j] = '#';
                }
            }

        }

        //carve out some large rooms to start out the maze
        //this currently allows overlapping
        private void CarveRooms(int numRooms)
        {
            int x, y;
            for (int i = 0; i < numRooms; i++)
            {
                x = r.Next(this.width);
                y = r.Next(this.height);
                switch (r.Next(3))
                {
                    case 0: //small
                        CarveRoom(x, y, r.Next(this.width / 8), r.Next(this.height / 8));
                        break;
                    case 1:
                        CarveRoom(x, y, r.Next(this.width / 4), r.Next(this.height / 4));
                        break;
                    case 2:
                        CarveRoom(x, y, r.Next(this.width / 2), r.Next(this.height / 2));
                        break;
                }

            }

        }

        private void CarveRoom(int x, int y, int width, int height)
        {
            //find top, left

            int halfWidth = (int)Math.Floor((double)width / 2);
            int halfHeight = (int)Math.Floor((double)height / 2);

            int left;
            int right;
            int top;
            int bottom;

            if (x > halfWidth)
                left = x - halfWidth;
            else
                left = 0;

            if (y > halfHeight)
                top = y - halfHeight;
            else
                top = 0;

            if (this.width > (x + halfWidth))
                right = x + halfWidth;
            else
                right = this.width;

            if (this.height > (y + halfHeight))
                bottom = y + halfHeight;
            else
                bottom = this.height;

            //carve out the space

            for (int i = top; i < bottom; i++)
            {
                for (int j = left; j < right; j++)
                {
                    maze[i, j] = ',';
                }
            }
        }


        // make the x,y space open, and add the surrounding spaces to the frontier list
        private void Carve(int x, int y)
        {

            maze[y, x] = '.';
            if (y > 0)
            {
                if (maze[y - 1, x] == '?')
                {
                    maze[y - 1, x] = ',';
                    frontier.Add(new Coord(x, y - 1));
                }
            }
            if (y < height - 1)
            {
                if (maze[y + 1, x] == '?')
                {
                    maze[y + 1, x] = ',';
                    frontier.Add(new Coord(x, y + 1));
                }
            }
            if (x > 0)
            {
                if (maze[y, x - 1] == '?')
                {
                    maze[y, x - 1] = ',';
                    frontier.Add(new Coord(x - 1, y));
                }
            }
            if (x < width - 1)
            {
                if (maze[y, x + 1] == '?')
                {
                    maze[y, x + 1] = ',';
                    frontier.Add(new Coord(x + 1, y));
                }
            }
            //shuffle frontier?

        }

        private void Harden(int x, int y)
        {
            maze[y, x] = '#';

        }

        private bool Check(int x, int y)
        {
            int edgestate = 0;
            if (y > 0)
            {
                if (maze[y - 1, x] == '.')
                    edgestate += 1;
            }
            if (y < height - 1)
            {
                if (maze[y + 1, x] == '.')
                    edgestate += 2;
            }
            if (x > 0)
            {
                if (maze[y, x - 1] == '.')
                    edgestate += 4;
            }
            if (x < width - 1)
            {
                if (maze[y, x + 1] == '.')
                    edgestate += 8;
            }

            if (this.noDiagonals)
            {
                if (edgestate == 1)
                {
                    if (y < height - 1)
                    {
                        if (x > 0)
                        {
                            if (maze[y + 1, x - 1] == '.')
                                return false;
                        }
                        if (x < width - 1)
                        {
                            if (maze[y + 1, x + 1] == '.')
                                return false;
                        }

                    }
                    return true;
                }
                else if (edgestate == 2)
                {
                    if (y > 0)
                    {
                        if (x > 0)
                        {
                            if (maze[y - 1, x - 1] == '.')
                                return false;
                        }
                        if (x < width - 1)
                        {
                            if (maze[y - 1, x + 1] == '.')
                                return false;
                        }
                    }
                    return true;
                }
                else if (edgestate == 4)
                {
                    if (x < width - 1)
                    {
                        if (y > 0)
                        {
                            if (maze[y - 1, x + 1] == '.')
                                return false;
                        }
                        if (y < height - 1)
                        {
                            if (maze[y + 1, x + 1] == '.')
                                return false;
                        }
                    }
                    return true;
                }
                else if (edgestate == 8)
                {
                    if (x > 0)
                    {
                        if (y > 0)
                        {
                            if (maze[y - 1, x - 1] == '.')
                                return false;
                        }
                        if (y < height - 1)
                        {
                            if (maze[y + 1, x - 1] == '.')
                                return false;
                        }
                    }
                    return true;

                }
                return false;
            }
            else
            {
                if (edgestate == 1 || edgestate == 2 || edgestate == 4 || edgestate == 8)
                    return true;
                else
                    return false;
            }

        }

        #endregion


        //takes the maze and stretches it out.  (each tile becomes 4)
        private void DoubleSize(int n)
        {
            for (int x = 0; x < n; x++)
            {
                char[,] newMaze = new char[this.height * 2, this.width * 2];

                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {
                        newMaze[i * 2, j * 2] = maze[i, j];
                        newMaze[i * 2 + 1, j * 2] = maze[i, j];
                        newMaze[i * 2, j * 2 + 1] = maze[i, j];
                        newMaze[i * 2 + 1, j * 2 + 1] = maze[i, j];
                    }
                }

                maze = newMaze;
                this.height *= 2;
                this.width *= 2;
            }

        }


        #region PrintSave
        public void PrintMaze()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write(maze[i, j]);
                }
                Console.Write('\n');
            }

        }

        public string DisplayMaze()
        {
            string retval = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    retval += maze[i, j];
                }
                retval += "\r\n";
            }

            return retval;

        }

        public void SaveMaze(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        sw.Write(maze[i, j]);
                    }
                    sw.Write('\n');
                }
            }
        }

        #endregion
    }
}
