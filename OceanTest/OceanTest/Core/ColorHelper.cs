using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


    public class ColorHelper
    {
        /*
        public static Color getRandomColor(Random r)
        {
            return getColor((EnemyWeaknessType)r.Next(4), r);
        }

        public static Color getColor(EnemyWeaknessType type, Random r)
        {
            switch (type)
            {
                case EnemyWeaknessType.Red:
                    return new Color(r.Next(150, 255), r.Next(0, 75), r.Next(0, 75));

                case EnemyWeaknessType.Blue:
                    return new Color(r.Next(0, 130), r.Next(0, 130), r.Next(125, 255));

                case EnemyWeaknessType.Green:
                    return new Color(r.Next(0, 100), r.Next(140, 255), r.Next(0, 100));

                case EnemyWeaknessType.Yellow:
                    return new Color(r.Next(150, 255), r.Next(145, 255), r.Next(0, 150));
                default:
                    return Color.White;
            }

        }
        */

        //does a smooth lerp between all the colors in the list
        //val should be between 0 and 1
        public static Color getColorLerp(List<Color> colorList, float val)
        {
            if (colorList.Count > 0)
            {
                float interval = 1f / (float)(colorList.Count - 1);
                int index = (int)((colorList.Count - 1) * val);
                float newVal = (val - (interval * index)) / interval;

                if (index < 0)
                {
                    return colorList[0];
                }
                else if (index < colorList.Count - 1)
                {
                    return Color.Lerp(colorList[index], colorList[index + 1], newVal);
                }
                else
                {
                    return colorList[colorList.Count - 1];
                }
            }
            else
            {
                return Color.White;
            }
        }

    }

