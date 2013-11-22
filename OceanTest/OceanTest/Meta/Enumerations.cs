using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OceanTest
{
    public enum FishType
    {
        Normal,
        Shark,

    }


    public enum FishingState
    {
        Retracted, //Fishing pole not in use
        Throwing, //Fishing pole is being thrown out in the ocean
        Waiting, //fishing pole has "landed" and waiting to be grabbed
        Hooked, ///fish has grabbed the bait
        Snapped,  //fish escaped, line breaks
        Caught, //Fish has been reeled in and is caught by the player

    }

    public enum TileType
    {
        None,
        Stone,
        Wood,
        Door,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
    
}
