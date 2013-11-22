using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OceanTest
{
    public static class GameConstants
    {
        public static float PlayerSpeed = 2000;
        public static float FishSpeed = 1000;
        public static float SharkSpeed = 500;
        public static float FlockFishSpeed = 1000;

        public static int OceanDepth = 3000;


        public static float Gravity = 500;

        //Flocking
        public static float cohesionWeight = .1f;
        public static float alignWeight = .5f;
        public static float seperateWeight = 1f;

        
            public static int LevelHeight = 100;
            public static int LevelWidth = 100;

            public static int TileWidth = 100;
            public static int TileHeight = 100;

         
        

    }
}
