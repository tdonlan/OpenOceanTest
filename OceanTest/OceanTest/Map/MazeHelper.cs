using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OceanTest
{
    public class MazeHelper
    {
        //return true or false if the map is valid
        //for now, just make sure the map isn't majority # tiles
        public static bool checkMapValidity(char[,] map)
        {
            int blockCount = 0;
            int tileCount = map.GetLength(0) * map.GetLength(1);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '#')
                        blockCount++;
                }
            }

            float percent = (float)blockCount / (float)tileCount;

            if (percent > .6)
                return false;
            else
                return true;
        }


        public static char[,] PlacePlatforms(char[,] map)
        {
            // map = PlaceSpecial(map, '-',5,5, 1,1, 10, 99999);
            //map = PlaceSpecial(map, '-', 1, 1, 1, 1, 25, 99);

            map = PlaceSpecialPattern(map, '-', readPatternGrid("Platform1"), 50, 9999);
            return map;
        }

        public static char[,] PlaceEnemies(char[,] map)
        {
            //map = PlaceSpecial(map, 'A',2,0,5, 5, 5, 99999);
            //map = PlaceSpecialPattern(map, 'A', getEnemyPatternGrid(), 50, 99999);
            map = PlaceSpecialPattern(map, 'A', readPatternGrid("Enemy1"), 10, 99999);
            map = PlaceSpecialPattern(map, 'a', readPatternGrid("Enemy1"), 10, 99999);
            map = PlaceSpecialPattern(map, 'C', readPatternGrid("Enemy1"), 10, 99999);
            map = PlaceSpecialPattern(map, 'c', readPatternGrid("Enemy1"), 10, 99999);
            return map;
        }

        public static char[,] PlaceNests(char[,] map)
        {
            map = PlaceSpecialPattern(map, 'N', readPatternGrid("Nest1"), 10, 99999);
            return map;
        }

        //Place the entrance and exit of the map
        //Exits should be on opposite quadrants of the map
        //Entrance usually lower left
        //Exit on the right side
        public static char[,] PlaceExits(char[,] map, int levelIndex)
        {
            char start;
            if (levelIndex - 1 == 0)
            {
                start = '0';
            }
            else
            {
                start = (char)(levelIndex - 1);
            }
            char end = (char)(levelIndex + 1);

            start = '0';
            end = '1';

            map = PlaceSpecialPattern(map, start, readPatternGrid("ExitTile1"), 50, 1);
            map = PlaceSpecialPattern(map, end, readPatternGrid("ExitTile1"), 50, 1);
            return map;
        }



        private static Grid getEnemyPatternGrid()
        {
            char[,] c = new char[3, 5];
            c = getEmptyCharArray(c);
            c[2, 0] = '#';
            c[2, 1] = '#';
            c[2, 2] = '#';
            c[2, 3] = '#';
            c[2, 4] = '#';


            Grid g = new Grid();
            g.map = c;
            g.height = 3;
            g.width = 5;

            return g;
        }

        private static char[,] getEmptyCharArray(char[,] c)
        {

            int height = c.GetLength(0);
            int width = c.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    c[i, j] = '.';
                }
            }

            return c;
        }

        //creates a wall around the map.  If true, then a single exit with a key and door tile
        //done prior to map scaling?
        //need to make sure the exit space is connected to pathways, not in the middle of a wall -
        public static char[,] EncloseMap(char[,] map, bool keyed, char keyChar, char doorChar, int wallEntry, int wallExit)
        {
            Random r = new Random();

            //fill in every wall
            if (!keyed)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (i == 0 || i == map.GetLength(0) - 1 || j == 0 || j == map.GetLength(1) - 1)
                        {
                            map[i, j] = '#';
                        }
                        if (i == 1 || i == map.GetLength(0) - 2 || j == 1 || j == map.GetLength(1) - 2)
                        {

                            map[i, j] = '#';
                        }
                    }
                }
            }
            else
            {
                //int wallExit = r.Next(3) + 1;
                int spaceExit;
                if (wallExit == 1 || wallExit == 2) // top or bottom wall, x coord
                {
                    spaceExit = r.Next(map.GetLength(0));
                }
                else
                {
                    spaceExit = r.Next(map.GetLength(1));
                }

                map = PlaceSpecial(map, keyChar, 2, 0, 1, 1, 10, 1);

                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (i == 0)
                        {
                            if (wallExit == 1 && spaceExit == j)
                            {
                                map[i, j] = doorChar;
                            }
                            else if (wallEntry != 1)
                            {
                                map[i, j] = '#';
                            }
                        }
                        if (i == map.GetLength(0) - 1)
                        {
                            if (wallExit == 2 && spaceExit == j)
                            {
                                map[i, j] = doorChar;
                            }
                            else if (wallEntry != 2)
                            {
                                map[i, j] = '#';
                            }
                        }
                        else if (j == 0)
                        {
                            if (wallExit == 3 && spaceExit == i)
                            {
                                map[i, j] = doorChar;
                            }
                            else if (wallEntry != 3)
                            {
                                map[i, j] = '#';
                            }
                        }
                        else if (j == map.GetLength(1) - 1)
                        {
                            if (wallExit == 4 && spaceExit == i)
                            {
                                map[i, j] = doorChar;
                            }
                            else if (wallEntry != 4)
                            {
                                map[i, j] = '#';
                            }
                        }
                    }
                }

            }

            return map;
        }

        //will place the special character in a space in the map, if meets the empty height and width, given the percent
        //useful for placing enemies, traps, ladders, platforms (for easier maps)
        //can also specify how many of this character there will be (for chests, keys, etc)
        //how to ensure that we do place the character if the percent is not met???
        //north, south, east, west are the four cardinal directions surrounding the given point
        public static char[,] PlaceSpecial(char[,] map, char specialChar, int north, int south, int west, int east, float percent, int specialCount)
        {
            //build a grid object
            Grid g = new Grid();
            g.map = map;
            g.height = map.GetLength(0);
            g.width = map.GetLength(1);

            int count = 0;

            Random r = new Random();

            //traverse the map and make changes
            for (int i = 0; i < g.height; i++)
            {
                for (int j = 0; j < g.width; j++)
                {
                    if (checkSpace(g, j, i, north, south, west, east, '.'))
                    {
                        if (r.NextDouble() * 100 < percent && count < specialCount)
                        {
                            count++;
                            g.map[i, j] = specialChar;
                            //g = fillGrid(g, new Coord(i, j), north,south,east,west, specialChar);

                        }
                    }
                }
            }

            return g.map;

        }

        public static char[,] PlaceSpecialPattern(char[,] map, char specialChar, Grid pattern, float percent, int specialCount)
        {
            //build a grid object
            Grid g = new Grid();
            g.map = map;
            g.height = map.GetLength(0);
            g.width = map.GetLength(1);

            int count = 0;

            Random r = new Random();
            int top;
            int left;

            //traverse the map and make changes
            for (int i = 0; i < g.height; i++)
            {
                for (int j = 0; j < g.width; j++)
                {
                    top = i - (pattern.height / 2);
                    left = j - (pattern.width / 2);
                    if (checkPattern(g, left, top, pattern))
                    {
                        if (r.NextDouble() * 100 < percent && count < specialCount)
                        {
                            count++;
                            g.map[i, j] = specialChar;
                        }
                    }
                }
            }

            return g.map;

        }


        //finds a trough of at least depth, and fills in with lava char, given the percent
        public static char[,] PlaceLava(char[,] map, char lavaChar, int depth, float percent)
        {
            return map;

        }


        //fills in the grid given a point, height, width and character.  This assums the grid is large enough.  Use checkSpace.
        public static Grid fillGrid(Grid g, Coord p, int north, int south, int west, int east, char c)
        {
            int left = p.x - west;
            int right = p.x + east;
            int top = p.y - north;
            int bottom = p.y + south;

            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    g.map[i, j] = c;
                }
            }

            return g;
        }

        //check if the given map position matches the pattern
        //note, the x,y is the upper left space on the pattern grid object
        //? is a wildcard tile
        public static bool checkPattern(Grid map, int x, int y, Grid pattern)
        {
            Grid subGrid = getSubMap(map, x, y, 0, pattern.height - 1, 0, pattern.width - 1);

            if (subGrid == null)
            {
                return false;
            }
            if (subGrid.height != pattern.height || subGrid.width != pattern.width)
            {
                return false;
            }

            for (int i = 0; i < pattern.height; i++)
            {
                for (int j = 0; j < pattern.width; j++)
                {
                    if (pattern.map[i, j] != '?')
                    {
                        if (pattern.map[i, j] != subGrid.map[i, j])
                            return false;
                    }
                }
            }

            return true;
        }



        public static bool checkSpace(Grid map, int x, int y, int north, int south, int west, int east, char c)
        {
            //get the submap first
            Grid subMap = getSubMap(map, x, y, north, south, west, east);

            if (subMap == null)
            {
                return false;
            }
            else
            {


                //check if all spots in this grid are = c
                for (int i = 0; i < subMap.height; i++)
                {
                    for (int j = 0; j < subMap.width; j++)
                    {
                        if (subMap.map[i, j] != c)
                            return false;
                    }
                }
            }



            return true;
        }

        //get a submap of the passed in map, centered around the origin, of height, width
        public static Grid getSubMap(Grid map, int x, int y, int north, int south, int west, int east)
        {
            //check if we can fit the submap
            Grid retval = null;
            if (x - west < 0 || x + east >= map.width)
                return retval;
            if (y - north < 0 || y + south >= map.height)
                return retval;

            //width, height on either side of the x,y params

            retval = new Grid();
            retval.height = north + south + 1;
            retval.width = east + west + 1;
            retval.map = new char[retval.height, retval.width];

            int left = x - west;
            int top = y - north;

            //we can fit the submap
            for (int i = 0; i < retval.height; i++)
            {
                for (int j = 0; j < retval.width; j++)
                {
                    retval.map[i, j] = map.map[top + i, left + j];
                }
            }

            return retval;
        }

        private static Grid readPatternGrid(string fileName)
        {

            string path = @"..\..\Patterns\";
            System.IO.StreamReader file = new System.IO.StreamReader(path + fileName + ".txt");

            List<string> stringList = new List<string>();
            string line;
            while ((line = file.ReadLine()) != null)
            {
                stringList.Add(line);
            }

            return new Grid(StringListToCharArray(stringList));
        }

        public static char[,] StringToCharArray(string map)
        {
            //split the map into string array and call StringListToCharArray
            List<string> mapStrList = new List<string>();
            mapStrList = map.Split('\n').ToList();

            return StringListToCharArray(mapStrList);

        }

        public static char[,] StringListToCharArray(List<string> strList)
        {
            //get width and height
            if (strList.Count > 0)
            {
                int height = strList.Count;
                int width = strList[0].Length;

                char[,] charArray = new char[height, width];
                int counter = 0;
                foreach (string line in strList)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        charArray[counter, i] = line[i];
                    }
                    counter++;
                }

                return charArray;
            }

            return new char[0, 0];
        }

        public static string CharArrayToString(char[,] map)
        {
            string retval = "";
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    retval += map[i, j];
                }
                retval += "\n";
            }
            return retval;
        }

        public static void printMap(Grid map)
        {
            for (int i = 0; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    Console.Write(map.map[i, j]);
                }
                Console.Write('\n');

            }
        }


    }
}
