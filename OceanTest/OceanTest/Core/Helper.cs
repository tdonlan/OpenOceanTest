using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OceanTest
{
    public class Helper
    {

        public static  Vector2 Normalize(Vector2 vec)
        {
            if (vec.X != 0 && vec.Y != 0)
            {
                vec.Normalize();
            }
            return vec;
        }
    }
}
